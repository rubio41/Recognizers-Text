﻿using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DateObject = System.DateTime;

using Microsoft.Recognizers.Text.Number;

namespace Microsoft.Recognizers.Text.DateTime
{
    public class BaseDateTimePeriodExtractor : IDateTimeExtractor
    {
        public static readonly string ExtractorName = Constants.SYS_DATETIME_DATETIMEPERIOD;

        private readonly IDateTimePeriodExtractorConfiguration config;

        public BaseDateTimePeriodExtractor(IDateTimePeriodExtractorConfiguration config)
        {
            this.config = config;
        }

        public List<ExtractResult> Extract(string text)
        {
            return Extract(text, DateObject.Now);
        }

        public List<ExtractResult> Extract(string text, DateObject reference)
        {
            var tokens = new List<Token>();
            tokens.AddRange(MatchSimpleCases(text, reference));
            tokens.AddRange(MergeTwoTimePoints(text, reference));
            tokens.AddRange(MatchDuration(text, reference));
            tokens.AddRange(MatchTimeOfDay(text, reference));
            tokens.AddRange(MatchRelativeUnit(text));

            return Token.MergeAllTokens(tokens, text, ExtractorName);
        }

        private List<Token> MatchSimpleCases(string text, DateObject reference)
        {
            var ret = new List<Token>();
            foreach (var regex in this.config.SimpleCasesRegex)
            {
                var matches = regex.Matches(text);
                foreach (Match match in matches)
                {
                    // Is there a date before it?
                    var hasBeforeDate = false;
                    var beforeStr = text.Substring(0, match.Index);
                    if (!string.IsNullOrEmpty(beforeStr))
                    {
                        var er = this.config.SingleDateExtractor.Extract(beforeStr, reference);
                        if (er.Count > 0)
                        {
                            var begin = er[0].Start ?? 0;
                            var end = (er[0].Start ?? 0) + (er[0].Length ?? 0);

                            var middleStr = beforeStr.Substring(begin + (er[0].Length ?? 0)).Trim().ToLower();
                            if (string.IsNullOrEmpty(middleStr) || this.config.PrepositionRegex.IsMatch(middleStr))
                            {
                                ret.Add(new Token(begin, match.Index + match.Length));
                                hasBeforeDate = true;
                            }
                        }
                    }

                    var followedStr = text.Substring(match.Index + match.Length);
                    if (!string.IsNullOrEmpty(followedStr) && !hasBeforeDate)
                    {
                        // Is it followed by a date?
                        var er = this.config.SingleDateExtractor.Extract(followedStr, reference);
                        if (er.Count > 0)
                        {
                            var begin = er[0].Start ?? 0;
                            var end = (er[0].Start ?? 0) + (er[0].Length ?? 0);
                            var middleStr = followedStr.Substring(0, begin).Trim().ToLower();
                            if (string.IsNullOrEmpty(middleStr) || this.config.PrepositionRegex.IsMatch(middleStr))
                            {
                                ret.Add(new Token(match.Index, match.Index + match.Length + end));
                            }
                        }
                    }
                }
            }

            return ret;
        }

        private List<Token> MergeTwoTimePoints(string text, DateObject reference)
        {
            var ret = new List<Token>();
            var er1 = this.config.SingleDateTimeExtractor.Extract(text, reference);
            var er2 = this.config.SingleTimeExtractor.Extract(text, reference);
            var timePoints = new List<ExtractResult>();
            
            // Handle the overlap problem
            var j = 0;
            for (var i = 0; i < er1.Count; i++)
            {
                timePoints.Add(er1[i]);

                while (j < er2.Count && er2[j].Start + er2[j].Length < er1[i].Start)
                {
                    timePoints.Add(er2[j]);
                    j++;
                }

                while (j < er2.Count && er2[j].IsOverlap(er1[i]))
                {
                    j++;
                }
            }

            for (; j < er2.Count; j++)
            {
                timePoints.Add(er2[j]);
            }

            timePoints = timePoints.OrderBy(o => o.Start).ToList();

            // Merge "{TimePoint} to {TimePoint}", "between {TimePoint} and {TimePoint}"
            var idx = 0;
            while (idx < timePoints.Count - 1)
            {
                // If both ends are Time. then this is a TimePeriod, not a DateTimePeriod
                if (timePoints[idx].Type.Equals(Constants.SYS_DATETIME_TIME) &&
                    timePoints[idx + 1].Type.Equals(Constants.SYS_DATETIME_TIME))
                {
                    idx++;
                    continue;
                }

                var middleBegin = timePoints[idx].Start + timePoints[idx].Length ?? 0;
                var middleEnd = timePoints[idx + 1].Start ?? 0;

                var middleStr = text.Substring(middleBegin, middleEnd - middleBegin).Trim();
                var match = this.config.TillRegex.Match(middleStr);

                // Handle "{TimePoint} to {TimePoint}"
                if (match.Success && match.Index == 0 && match.Length == middleStr.Length)
                {
                    var periodBegin = timePoints[idx].Start ?? 0;
                    var periodEnd = (timePoints[idx + 1].Start ?? 0) + (timePoints[idx + 1].Length ?? 0);

                    // Handle "from"
                    var beforeStr = text.Substring(0, periodBegin).Trim().ToLowerInvariant();

                    if (this.config.GetFromTokenIndex(beforeStr, out int fromIndex)
                        || this.config.GetBetweenTokenIndex(beforeStr, out fromIndex))
                    {
                        periodBegin = fromIndex;
                    }

                    ret.Add(new Token(periodBegin, periodEnd));
                    idx += 2;
                    continue;
                }

                // Handle "between {TimePoint} and {TimePoint}"
                if (this.config.HasConnectorToken(middleStr))
                {
                    var periodBegin = timePoints[idx].Start ?? 0;
                    var periodEnd = (timePoints[idx + 1].Start ?? 0) + (timePoints[idx + 1].Length ?? 0);

                    // Handle "between"
                    var beforeStr = text.Substring(0, periodBegin).Trim().ToLowerInvariant();

                    if (this.config.GetBetweenTokenIndex(beforeStr, out int beforeIndex))
                    {
                        periodBegin = beforeIndex;
                        ret.Add(new Token(periodBegin, periodEnd));
                        idx += 2;
                        continue;
                    }
                }
                idx++;
            }

            // Regarding the pharse as-- {Date} {TimePeriod}, like "2015-9-23 1pm to 4"
            er1 = this.config.SingleDateExtractor.Extract(text, reference);
            er2 = this.config.TimePeriodExtractor.Extract(text, reference);
            er1.AddRange(er2);

            var points = er1.OrderBy(x => x.Start).ToList();

            for (idx = 0; idx < points.Count-1; idx++)
            {
                if (points[idx].Type == points[idx + 1].Type)
                {
                    continue;
                }

                var midBegin = points[idx].Start + points[idx].Length ?? 0;
                var midEnd = points[idx + 1].Start?? 0;

                if (midEnd - midBegin > 0)
                {
                    var midStr = text.Substring(midBegin, midEnd-midBegin);
                    if (string.IsNullOrWhiteSpace(midStr) && !string.IsNullOrEmpty(midStr))
                    {
                        ret.Add(new Token(points[idx].Start ?? 0, points[idx + 1].Start + points[idx + 1].Length ?? 0));
                        idx += 2;
                    }
                }
            }

            return ret;
        }

        private List<Token> MatchTimeOfDay(string text, DateObject reference)
        {
            var ret = new List<Token>();

            var matches = this.config.SpecificTimeOfDayRegex.Matches(text);
            foreach (Match match in matches)
            {
                ret.Add(new Token(match.Index, match.Index + match.Length));
            }

            // Date followed by morning, afternoon or morning, afternoon followed by Date
            var ers = this.config.SingleDateExtractor.Extract(text, reference);
            if (ers.Count == 0)
            {
                return ret;
            }

            foreach (var er in ers)
            {
                var afterStr = text.Substring(er.Start + er.Length ?? 0);

                var match = this.config.PeriodTimeOfDayWithDateRegex.Match(afterStr);
                if (match.Success)
                {
                    if (string.IsNullOrWhiteSpace(afterStr.Substring(0, match.Index)))
                    {
                        ret.Add(new Token(er.Start ?? 0, er.Start + er.Length + match.Index + match.Length ?? 0));
                    }
                    else
                    {
                        var connectorStr = afterStr.Substring(0, match.Index);
                        var pauseMatch = config.MiddlePauseRegex.Match(connectorStr);

                        if (pauseMatch.Success && pauseMatch.Length == connectorStr.Length)
                        {
                            var suffix = afterStr.Substring(match.Index + match.Length).TrimStart(' ');
    
                            var endingMatch = config.GeneralEndingRegex.Match(suffix);
                            if (endingMatch.Success)
                            {
                                ret.Add(new Token(er.Start ?? 0, er.Start + er.Length + match.Index + match.Length ?? 0));
                            }
                        }
                    }
                }

                var prefixStr = text.Substring(0, er.Start?? 0);

                match = this.config.PeriodTimeOfDayWithDateRegex.Match(prefixStr);
                if (match.Success)
                {
                    if (string.IsNullOrWhiteSpace(prefixStr.Substring(match.Index + match.Length)))
                    {
                        var midStr = text.Substring(match.Index + match.Length, er.Start - match.Index - match.Length ?? 0);
                        if (!string.IsNullOrEmpty(midStr) && string.IsNullOrWhiteSpace(midStr))
                        {
                            ret.Add(new Token(match.Index, er.Start + er.Length ?? 0));
                        }
                    }
                    else
                    {
                        var connectorStr = prefixStr.Substring(match.Index + match.Length);
                        var pauseMatch = config.MiddlePauseRegex.Match(connectorStr);

                        if (pauseMatch.Success && pauseMatch.Length == connectorStr.Length)
                        {
                            var suffix = text.Substring(er.Start + er.Length?? 0).TrimStart(' ');

                            var endingMatch = config.GeneralEndingRegex.Match(suffix);
                            if (endingMatch.Success)
                            {
                                ret.Add(new Token(match.Index, er.Start + er.Length ?? 0));
                            }

                        }
                    }
                }
            }

            // Check whether there are adjacent time period strings, before or after
            foreach (var e in ret.ToArray())
            {
                // Try to extract a time period in before-string 
                if (e.Start > 0)
                {
                    var beforeStr = text.Substring(0, e.Start);
                    if (!string.IsNullOrEmpty(beforeStr))
                    {
                        var timeErs = this.config.TimePeriodExtractor.Extract(beforeStr);
                        if (timeErs.Count > 0)
                        {
                            foreach (var tp in timeErs)
                            {
                                var midStr = beforeStr.Substring(tp.Start + tp.Length ?? 0);
                                if (string.IsNullOrWhiteSpace(midStr))
                                {
                                    ret.Add(new Token(tp.Start ?? 0, tp.Start + tp.Length + midStr.Length + e.Length ?? 0));
                                }
                            }
                        }
                    }
                }

                // Try to extract a time period in after-string
                if (e.Start + e.Length <= text.Length)
                {
                    var afterStr = text.Substring(e.Start + e.Length);
                    if (!string.IsNullOrEmpty(afterStr))
                    {
                        var timeErs = this.config.TimePeriodExtractor.Extract(afterStr);
                        if (timeErs.Count > 0)
                        {
                            foreach (var tp in timeErs)
                            {
                                var midStr = afterStr.Substring(0, tp.Start ?? 0);
                                if (string.IsNullOrWhiteSpace(midStr))
                                {
                                    ret.Add(new Token(e.Start, e.Start + e.Length + midStr.Length + tp.Length ?? 0));
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        //TODO: this can be abstracted with the similar method in BaseDatePeriodExtractor
        private List<Token> MatchDuration(string text, DateObject reference)
        {
            var ret = new List<Token>();

            var durations = new List<Token>();
            var durationExtractions = config.DurationExtractor.Extract(text, reference);

            foreach (var durationExtraction in durationExtractions)
            {
                var match = config.TimeUnitRegex.Match(durationExtraction.Text);
                if (match.Success)
                {
                    durations.Add(new Token(durationExtraction.Start ?? 0,
                        (durationExtraction.Start + durationExtraction.Length ?? 0)));
                }
            }

            foreach (var duration in durations)
            {
                var beforeStr = text.Substring(0, duration.Start).ToLowerInvariant();
                var afterStr = text.Substring(duration.Start + duration.Length);

                if (string.IsNullOrWhiteSpace(beforeStr) && string.IsNullOrWhiteSpace(afterStr))
                {
                    continue;
                }

                var match = this.config.PastPrefixRegex.Match(beforeStr);
                if (match.Success && string.IsNullOrWhiteSpace(beforeStr.Substring(match.Index + match.Length)))
                {
                    ret.Add(new Token(match.Index, duration.End));
                    continue;
                }

                match = this.config.NextPrefixRegex.Match(beforeStr);
                if (match.Success && string.IsNullOrWhiteSpace(beforeStr.Substring(match.Index + match.Length)))
                {
                    ret.Add(new Token(match.Index, duration.End));
                    continue;
                }
                
                match = this.config.PastPrefixRegex.Match(afterStr);
                if (match.Success && string.IsNullOrWhiteSpace(afterStr.Substring(match.Index + match.Length)))
                {
                    ret.Add(new Token(duration.Start, duration.Start + duration.Length + match.Index + match.Length));
                    continue;
                }

                match = this.config.NextPrefixRegex.Match(afterStr);
                if (match.Success && string.IsNullOrWhiteSpace(afterStr.Substring(match.Index + match.Length)))
                {
                    ret.Add(new Token(duration.Start, duration.Start + duration.Length + match.Index + match.Length));
                }

            }

            return ret;
        }

        private List<Token> MatchRelativeUnit(string text)
        {
            var ret = new List<Token>();

            var matches = config.RelativeTimeUnitRegex.Matches(text);
            if (matches.Count == 0)
            {
                matches = this.config.RestOfDateTimeRegex.Matches(text);
            }

            foreach(Match match in matches)
            {
                ret.Add(new Token(match.Index, match.Index + match.Length));
            }

            return ret;
        }
    }
}
