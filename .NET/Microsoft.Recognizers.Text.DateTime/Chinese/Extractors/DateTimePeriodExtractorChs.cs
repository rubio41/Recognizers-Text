﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Definitions.Chinese;
using Microsoft.Recognizers.Text.Number.Chinese;

namespace Microsoft.Recognizers.Text.DateTime.Chinese
{
    public class DateTimePeriodExtractorChs : IExtractor
    {
        public static readonly string ExtractorName = Constants.SYS_DATETIME_DATETIMEPERIOD;

        public static readonly Regex TillRegex = new Regex(DateTimeDefinitions.DateTimePeriodTillRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex PrepositionRegex = new Regex(DateTimeDefinitions.DateTimePeriodPrepositionRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex HourRegex = new Regex(DateTimeDefinitions.HourRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex HourNumRegex = new Regex(DateTimeDefinitions.HourNumRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex ZhijianRegex = new Regex(DateTimeDefinitions.ZhijianRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex ThisRegex = new Regex(DateTimeDefinitions.DateTimePeriodThisRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex LastRegex = new Regex(DateTimeDefinitions.DateTimePeriodLastRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex NextRegex = new Regex(DateTimeDefinitions.DateTimePeriodNextRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex DescRegex = new Regex(DateTimeDefinitions.DescRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex TimeOfDayRegex = new Regex(DateTimeDefinitions.TimeOfDayRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex SpecificTimeOfDayRegex = new Regex(DateTimeDefinitions.SpecificTimeOfDayRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex UnitRegex = new Regex(DateTimeDefinitions.DateTimePeriodUnitRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex FollowedUnit = new Regex(DateTimeDefinitions.DateTimePeriodFollowedUnit, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex NumberCombinedWithUnit = new Regex(DateTimeDefinitions.DateTimePeriodNumberCombinedWithUnit, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex PastRegex = new Regex(DateTimeDefinitions.PastRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex FutureRegex = new Regex(DateTimeDefinitions.FutureRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly TimeExtractorChs SingleTimeExtractor = new TimeExtractorChs();
        private static readonly DateTimeExtractorChs TimeWithDateExtractor = new DateTimeExtractorChs();
        private static readonly DateExtractorChs SingleDateExtractor = new DateExtractorChs();
        private static readonly CardinalExtractor CardinalExtractor = new CardinalExtractor();
        private static readonly TimePeriodExtractorChs TimePeriodExtractor = new TimePeriodExtractorChs();

        public List<ExtractResult> Extract(string text)
        {
            var tokens = new List<Token>();
            tokens.AddRange(MergeDateAndTimePeriod(text));
            tokens.AddRange(MergeTwoTimePoints(text));
            tokens.AddRange(MatchNubmerWithUnit(text));
            tokens.AddRange(MatchNight(text));

            return Token.MergeAllTokens(tokens, text, ExtractorName);
        }

        // merge Date and Time peroid
        private List<Token> MergeDateAndTimePeriod(string text)
        {
            var ret = new List<Token>();
            var er1 = SingleDateExtractor.Extract(text);
            var er2 = TimePeriodExtractor.Extract(text);
            var timePoints = new List<ExtractResult>();

            // handle the overlap problem
            var j = 0;
            for (var i = 0; i < er1.Count; i++)
            {
                timePoints.Add(er1[i]);
                while (j < er2.Count && er2[j].Start + er2[j].Length <= er1[i].Start)
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

            // merge {Date} {TimePeriod}
            var idx = 0;
            while (idx < timePoints.Count - 1)
            {
                if (timePoints[idx].Type.Equals(Constants.SYS_DATETIME_DATE) &&
                    timePoints[idx + 1].Type.Equals(Constants.SYS_DATETIME_TIMEPERIOD))
                {
                    var middleBegin = timePoints[idx].Start + timePoints[idx].Length ?? 0;
                    var middleEnd = timePoints[idx + 1].Start ?? 0;

                    var middleStr = text.Substring(middleBegin, middleEnd - middleBegin).Trim();
                    if (string.IsNullOrWhiteSpace(middleStr) || PrepositionRegex.IsMatch(middleStr))
                    {
                        var periodBegin = timePoints[idx].Start ?? 0;
                        var periodEnd = (timePoints[idx + 1].Start ?? 0) + (timePoints[idx + 1].Length ?? 0);
                        ret.Add(new Token(periodBegin, periodEnd));
                        idx += 2;
                        continue;
                    }
                    idx++;
                }
                idx++;
            }

            return ret;
        }

        private List<Token> MergeTwoTimePoints(string text)
        {
            var ret = new List<Token>();
            var er1 = TimeWithDateExtractor.Extract(text);
            var er2 = SingleTimeExtractor.Extract(text);
            var timePoints = new List<ExtractResult>();

            // handle the overlap problem
            var j = 0;
            for (var i = 0; i < er1.Count; i++)
            {
                timePoints.Add(er1[i]);
                while (j < er2.Count && er2[j].Start + er2[j].Length <= er1[i].Start)
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

            // merge "{TimePoint} to {TimePoint}", "between {TimePoint} and {TimePoint}"
            var idx = 0;
            while (idx < timePoints.Count - 1)
            {
                // if both ends are Time. then this is a TimePeriod, not a DateTimePeriod
                if (timePoints[idx].Type.Equals(Constants.SYS_DATETIME_TIME) &&
                    timePoints[idx + 1].Type.Equals(Constants.SYS_DATETIME_TIME))
                {
                    idx++;
                    continue;
                }

                var middleBegin = timePoints[idx].Start + timePoints[idx].Length ?? 0;
                var middleEnd = timePoints[idx + 1].Start ?? 0;

                var middleStr = text.Substring(middleBegin, middleEnd - middleBegin).Trim();
                var match = TillRegex.Match(middleStr);
                // handle "{TimePoint} to {TimePoint}"
                if (match.Success && match.Index == 0 && match.Length == middleStr.Length)
                {
                    var periodBegin = timePoints[idx].Start ?? 0;
                    var periodEnd = (timePoints[idx + 1].Start ?? 0) + (timePoints[idx + 1].Length ?? 0);

                    // handle "from"
                    var beforeStr = text.Substring(0, periodBegin).ToLowerInvariant();
                    if (beforeStr.Trim().EndsWith("从"))
                    {
                        periodBegin = beforeStr.LastIndexOf("从", StringComparison.Ordinal);
                    }

                    ret.Add(new Token(periodBegin, periodEnd));
                    idx += 2;
                    continue;
                }

                // handle "between {TimePoint} and {TimePoint}"
                if (middleStr.Equals("和") || middleStr.Equals("与") || middleStr.Equals("到"))
                {
                    var periodBegin = timePoints[idx].Start ?? 0;
                    var periodEnd = (timePoints[idx + 1].Start ?? 0) + (timePoints[idx + 1].Length ?? 0);

                    // handle "between"
                    var afterStr = text.Substring(periodEnd).ToLowerInvariant();
                    match = ZhijianRegex.Match(afterStr);
                    if (match.Success)
                    {
                        ret.Add(new Token(periodBegin, periodEnd + match.Length));
                        idx += 2;
                        continue;
                    }
                }

                idx++;
            }

            return ret;
        }

        private List<Token> MatchNight(string text)
        {
            var ret = new List<Token>();
            var matches = SpecificTimeOfDayRegex.Matches(text);
            foreach (Match match in matches)
            {
                ret.Add(new Token(match.Index, match.Index + match.Length));
            }

            // Date followed by morning, afternoon
            var ers = SingleDateExtractor.Extract(text);
            if (ers.Count == 0)
            {
                return ret;
            }

            foreach (var er in ers)
            {
                var afterStr = text.Substring(er.Start + er.Length ?? 0);
                var match = TimeOfDayRegex.Match(afterStr);
                if (match.Success)
                {
                    var middleStr = afterStr.Substring(0, match.Index);
                    if (string.IsNullOrWhiteSpace(middleStr) || PrepositionRegex.IsMatch(middleStr))
                    {
                        ret.Add(new Token(er.Start ?? 0, er.Start + er.Length + match.Index + match.Length ?? 0));
                    }
                }
            }

            return ret;
        }

        private List<Token> MatchNubmerWithUnit(string text)
        {
            var ret = new List<Token>();

            var durations = new List<Token>();
            var ers = CardinalExtractor.Extract(text);

            foreach (var er in ers)
            {
                var afterStr = text.Substring(er.Start + er.Length ?? 0);
                var match = FollowedUnit.Match(afterStr);
                if (match.Success && match.Index == 0)
                {
                    durations.Add(new Token(er.Start ?? 0, (er.Start + er.Length ?? 0) + match.Length));
                }
            }

            var matches = UnitRegex.Matches(text);
            foreach (Match match in matches)
            {
                durations.Add(new Token(match.Index, match.Index + match.Length));
            }

            foreach (var duration in durations)
            {
                var beforeStr = text.Substring(0, duration.Start).ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(beforeStr))
                {
                    continue;
                }

                var match = PastRegex.Match(beforeStr);
                if (match.Success && string.IsNullOrWhiteSpace(beforeStr.Substring(match.Index + match.Length)))
                {
                    ret.Add(new Token(match.Index, duration.End));
                    continue;
                }

                match = FutureRegex.Match(beforeStr);
                if (match.Success && string.IsNullOrWhiteSpace(beforeStr.Substring(match.Index + match.Length)))
                {
                    ret.Add(new Token(match.Index, duration.End));
                }
            }

            return ret;
        }
    }
}