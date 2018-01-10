﻿using System;
using System.Collections.Generic;
using DateObject = System.DateTime;

using Microsoft.Recognizers.Text.Number;

namespace Microsoft.Recognizers.Text.DateTime
{
    public class BaseDateTimePeriodParser : IDateTimeParser
    {
        public static readonly string ParserName = Constants.SYS_DATETIME_DATETIMEPERIOD;
        
        protected readonly IDateTimePeriodParserConfiguration Config;

        public BaseDateTimePeriodParser(IDateTimePeriodParserConfiguration configuration)
        {
            Config = configuration;
        }

        public ParseResult Parse(ExtractResult result)
        {
            return this.Parse(result, DateObject.Now);
        }

        public DateTimeParseResult Parse(ExtractResult er, DateObject refTime)
        {
            var referenceTime = refTime;

            object value = null;
            if (er.Type.Equals(ParserName))
            {
                var innerResult = MergeDateAndTimePeriods(er.Text, referenceTime);
                if (!innerResult.Success)
                {
                    innerResult = MergeTwoTimePoints(er.Text, referenceTime);
                }

                if (!innerResult.Success)
                {
                    innerResult = ParseSpecificTimeOfDay(er.Text, referenceTime);
                }

                if (!innerResult.Success)
                {
                    innerResult = ParseDuration(er.Text, referenceTime);
                }

                if (!innerResult.Success)
                {
                    innerResult = ParseRelativeUnit(er.Text, referenceTime);
                }

                if (innerResult.Success)
                {
                    innerResult.FutureResolution = new Dictionary<string, string>
                    {
                        {
                            TimeTypeConstants.START_DATETIME,
                            FormatUtil.FormatDateTime(((Tuple<DateObject, DateObject>) innerResult.FutureValue).Item1)
                        },
                        {
                            TimeTypeConstants.END_DATETIME,
                            FormatUtil.FormatDateTime(((Tuple<DateObject, DateObject>) innerResult.FutureValue).Item2)
                        }
                    };

                    innerResult.PastResolution = new Dictionary<string, string>
                    {
                        {
                            TimeTypeConstants.START_DATETIME,
                            FormatUtil.FormatDateTime(((Tuple<DateObject, DateObject>) innerResult.PastValue).Item1)
                        },
                        {
                            TimeTypeConstants.END_DATETIME,
                            FormatUtil.FormatDateTime(((Tuple<DateObject, DateObject>) innerResult.PastValue).Item2)
                        }
                    };

                    value = innerResult;
                }
            }

            var ret = new DateTimeParseResult
            {
                Text = er.Text,
                Start = er.Start,
                Length = er.Length,
                Type = er.Type,
                Data = er.Data,
                Value = value,
                TimexStr = value == null ? "" : ((DateTimeResolutionResult) value).Timex,
                ResolutionStr = ""
            };

            return ret;
        }

        private DateTimeResolutionResult MergeDateAndTimePeriods(string text, DateObject referenceTime)
        {
            var ret = new DateTimeResolutionResult();
            var trimedText = text.Trim().ToLower();

            var er = Config.TimePeriodExtractor.Extract(trimedText, referenceTime);
            if (er.Count != 1)
            {
                return ParseSimpleCases(text, referenceTime);
            }

            var timePeriodParseResult = Config.TimePeriodParser.Parse(er[0]);
            var timePeriodResolutionResult = (DateTimeResolutionResult) timePeriodParseResult.Value;

            if (timePeriodResolutionResult == null)
            {
                return ParseSimpleCases(text, referenceTime);
            }

            var timePeriodTimex = timePeriodResolutionResult.Timex;

            // If it is a range type timex
            if (!string.IsNullOrEmpty(timePeriodTimex) && timePeriodTimex.StartsWith("("))
            {

                var dateResult = this.Config.DateExtractor.Extract(trimedText.Replace(er[0].Text, ""), referenceTime);

                if (dateResult.Count == 1 && trimedText.Replace(er[0].Text, "").Trim().Equals(dateResult[0].Text))
                {
                    
                    string dateStr;
                    DateObject futureTime;
                    DateObject pastTime;

                    var pr = this.Config.DateParser.Parse(dateResult[0], referenceTime);

                    if (pr.Value != null)
                    {
                        futureTime = (DateObject)((DateTimeResolutionResult)pr.Value).FutureValue;
                        pastTime = (DateObject)((DateTimeResolutionResult)pr.Value).PastValue;

                        dateStr = pr.TimexStr;
                    }
                    else
                    {
                        return ParseSimpleCases(text, referenceTime);
                    }

                    timePeriodTimex = timePeriodTimex.Replace("(", "").Replace(")", "");
                    var timePeriodTimexArray = timePeriodTimex.Split(',');
                    var timePeriodFutureValue = (Tuple<DateObject, DateObject>)timePeriodResolutionResult.FutureValue;
                    var beginTime = timePeriodFutureValue.Item1;
                    var endTime = timePeriodFutureValue.Item2;

                    if (timePeriodTimexArray.Length == 3)
                    {
                        var beginStr = dateStr + timePeriodTimexArray[0];
                        var endStr = dateStr + timePeriodTimexArray[1];

                        ret.Timex = $"({beginStr},{endStr},{timePeriodTimexArray[2]})";

                        ret.FutureValue = new Tuple<DateObject, DateObject>(
                            DateObject.MinValue.SafeCreateFromValue(futureTime.Year, futureTime.Month, futureTime.Day,
                                beginTime.Hour, beginTime.Minute, beginTime.Second),
                            DateObject.MinValue.SafeCreateFromValue(futureTime.Year, futureTime.Month, futureTime.Day,
                                endTime.Hour, endTime.Minute, endTime.Second)
                                );

                        ret.PastValue = new Tuple<DateObject, DateObject>(
                            DateObject.MinValue.SafeCreateFromValue(pastTime.Year, pastTime.Month, pastTime.Day,
                                beginTime.Hour, beginTime.Minute, beginTime.Second),
                            DateObject.MinValue.SafeCreateFromValue(pastTime.Year, pastTime.Month, pastTime.Day,
                                endTime.Hour, endTime.Minute, endTime.Second)
                                );

                        if (!string.IsNullOrEmpty(timePeriodResolutionResult.Comment)
                            && timePeriodResolutionResult.Comment.Equals("ampm"))
                        {
                            ret.Comment = "ampm";
                        }

                        ret.Success = true;
                        ret.SubDateTimeEntities = new List<object> { pr, timePeriodParseResult };

                        return ret;
                    }
                }
                else
                {
                    return ParseSimpleCases(text, referenceTime);
                }
            }

            return ParseSimpleCases(text, referenceTime);
        }

        private DateTimeResolutionResult ParseSimpleCases(string text, DateObject referenceTime)
        {
            var ret = new DateTimeResolutionResult();
            var trimedText = text.Trim().ToLower();

            var match = this.Config.PureNumberFromToRegex.Match(trimedText);
            if (!match.Success)
            {
                match = this.Config.PureNumberBetweenAndRegex.Match(trimedText);
            }

            if (match.Success && match.Index == 0)
            {
                // This "from .. to .." pattern is valid if followed by a Date OR "pm"
                var hasAm = false;
                var hasPm = false;
                var dateStr = "XXXX-XX-XX";

                // Get hours
                var hourGroup = match.Groups["hour"];
                var hourStr = hourGroup.Captures[0].Value;
                var beginHour = 0;

                if (this.Config.Numbers.ContainsKey(hourStr))
                {
                    beginHour = this.Config.Numbers[hourStr];
                }
                else
                {
                    beginHour = int.Parse(hourStr);
                }

                hourStr = hourGroup.Captures[1].Value;
                var endHour = 0;

                if (this.Config.Numbers.ContainsKey(hourStr))
                {
                    endHour = this.Config.Numbers[hourStr];
                }
                else
                {
                    endHour = int.Parse(hourStr);
                }

                // Parse following date
                var er = this.Config.DateExtractor.Extract(trimedText.Replace(match.Value, ""), referenceTime);

                DateObject futureTime;
                DateObject pastTime;
                if (er.Count > 0)
                {
                    var pr = this.Config.DateParser.Parse(er[0], referenceTime);
                    if (pr.Value != null)
                    {
                        futureTime = (DateObject) ((DateTimeResolutionResult) pr.Value).FutureValue;
                        pastTime = (DateObject) ((DateTimeResolutionResult) pr.Value).PastValue;

                        dateStr = pr.TimexStr;
                    }
                    else
                    {
                        return ret;
                    }
                }
                else
                {
                    return ret;
                }

                // Parse "pm" 
                var pmStr = match.Groups["pm"].Value;
                var amStr = match.Groups["am"].Value;
                var descStr = match.Groups["desc"].Value;
                if (!string.IsNullOrEmpty(amStr) || !string.IsNullOrEmpty(descStr) && descStr.StartsWith("a"))
                {
                    if (beginHour >= 12)
                    {
                        beginHour -= 12;
                    }

                    if (endHour >= 12)
                    {
                        endHour -= 12;
                    }

                    hasAm = true;
                }
                else if (!string.IsNullOrEmpty(pmStr) || !string.IsNullOrEmpty(descStr) && descStr.StartsWith("p"))
                {
                    if (beginHour < 12)
                    {
                        beginHour += 12;
                    }

                    if (endHour < 12)
                    {
                        endHour += 12;
                    }

                    hasPm = true;
                }

                if (!hasAm && !hasPm && beginHour <= 12 && endHour <= 12)
                {
                    ret.Comment = "ampm";
                }

                var beginStr = dateStr + "T" + beginHour.ToString("D2");
                var endStr = dateStr + "T" + endHour.ToString("D2");

                ret.Timex = $"({beginStr},{endStr},PT{endHour - beginHour}H)";

                ret.FutureValue = new Tuple<DateObject, DateObject>(
                    DateObject.MinValue.SafeCreateFromValue(futureTime.Year, futureTime.Month, futureTime.Day, beginHour, 0, 0),
                    DateObject.MinValue.SafeCreateFromValue(futureTime.Year, futureTime.Month, futureTime.Day, endHour, 0, 0));

                ret.PastValue = new Tuple<DateObject, DateObject>(
                    DateObject.MinValue.SafeCreateFromValue(pastTime.Year, pastTime.Month, pastTime.Day, beginHour, 0, 0),
                    DateObject.MinValue.SafeCreateFromValue(pastTime.Year, pastTime.Month, pastTime.Day, endHour, 0, 0));

                ret.Success = true;

                return ret;
            }

            return ret;
        }

        private DateTimeResolutionResult MergeTwoTimePoints(string text, DateObject referenceTime)
        {
            var ret = new DateTimeResolutionResult();
            DateTimeParseResult pr1 = null, pr2 = null;
            bool bothHaveDates = false, beginHasDate = false, endHasDate = false;

            var er1 = this.Config.TimeExtractor.Extract(text, referenceTime);
            var er2 = this.Config.DateTimeExtractor.Extract(text, referenceTime);

            if (er2.Count == 2)
            {
                pr1 = this.Config.DateTimeParser.Parse(er2[0], referenceTime);
                pr2 = this.Config.DateTimeParser.Parse(er2[1], referenceTime);
                bothHaveDates = true;
            }
            else if (er2.Count == 1 && er1.Count == 2)
            {
                if (!er2[0].IsOverlap(er1[0]))
                {
                    pr1 = this.Config.TimeParser.Parse(er1[0], referenceTime);
                    pr2 = this.Config.DateTimeParser.Parse(er2[0], referenceTime);
                    endHasDate = true;
                }
                else
                {
                    pr1 = this.Config.DateTimeParser.Parse(er2[0], referenceTime);
                    pr2 = this.Config.TimeParser.Parse(er1[1], referenceTime);
                    beginHasDate = true;
                }
            }
            else if (er2.Count == 1 && er1.Count == 1)
            {
                if (er1[0].Start < er2[0].Start)
                {
                    pr1 = this.Config.TimeParser.Parse(er1[0], referenceTime);
                    pr2 = this.Config.DateTimeParser.Parse(er2[0], referenceTime);
                    endHasDate = true;
                }
                else
                {
                    pr1 = this.Config.DateTimeParser.Parse(er2[0], referenceTime);
                    pr2 = this.Config.TimeParser.Parse(er1[0], referenceTime);
                    beginHasDate = true;
                }
            }
            else if (er1.Count == 2)
            {
                // If both ends are Time. then this is a TimePeriod, not a DateTimePeriod
                return ret;
            }
            else
            {
                return ret;
            }

            if (pr1.Value == null || pr2.Value == null)
            {
                return ret;
            }

            DateObject futureBegin = (DateObject)((DateTimeResolutionResult)pr1.Value).FutureValue,
                       futureEnd = (DateObject)((DateTimeResolutionResult)pr2.Value).FutureValue;

            DateObject pastBegin = (DateObject)((DateTimeResolutionResult)pr1.Value).PastValue,
                       pastEnd = (DateObject)((DateTimeResolutionResult)pr2.Value).PastValue;

            if (bothHaveDates)
            {
                if (futureBegin > futureEnd)
                {
                    futureBegin = pastBegin;
                }

                if (pastEnd < pastBegin)
                {
                    pastEnd = futureEnd;
                }
            }

            if (bothHaveDates)
            {
                ret.Timex = $"({pr1.TimexStr},{pr2.TimexStr},PT{Convert.ToInt32((futureEnd - futureBegin).TotalHours)}H)";
                // Do nothing
            }
            else if (beginHasDate)
            {
                futureEnd = DateObject.MinValue.SafeCreateFromValue(futureBegin.Year, futureBegin.Month, futureBegin.Day,
                    futureEnd.Hour, futureEnd.Minute, futureEnd.Second);

                pastEnd = DateObject.MinValue.SafeCreateFromValue(pastBegin.Year, pastBegin.Month, pastBegin.Day,
                    pastEnd.Hour, pastEnd.Minute, pastEnd.Second);

                var dateStr = pr1.TimexStr.Split('T')[0];
                ret.Timex = $"({pr1.TimexStr},{dateStr + pr2.TimexStr},PT{Convert.ToInt32((futureEnd - futureBegin).TotalHours)}H)";
            }
            else if (endHasDate)
            {
                futureBegin = DateObject.MinValue.SafeCreateFromValue(futureEnd.Year, futureEnd.Month, futureEnd.Day,
                    futureBegin.Hour, futureBegin.Minute, futureBegin.Second);

                pastBegin = DateObject.MinValue.SafeCreateFromValue(pastEnd.Year, pastEnd.Month, pastEnd.Day,
                    pastBegin.Hour, pastBegin.Minute, pastBegin.Second);

                var dateStr = pr2.TimexStr.Split('T')[0];
                ret.Timex = $"({dateStr + pr1.TimexStr},{pr2.TimexStr},PT{Convert.ToInt32((futureEnd - futureBegin).TotalHours)}H)";
            }

            var ampmStr1 = ((DateTimeResolutionResult)pr1.Value).Comment;
            var ampmStr2 = ((DateTimeResolutionResult)pr2.Value).Comment;
            if (!string.IsNullOrEmpty(ampmStr1) && ampmStr1.EndsWith("ampm") && !string.IsNullOrEmpty(ampmStr2) && ampmStr2.EndsWith("ampm"))
            {
                ret.Comment = "ampm";
            }

            ret.FutureValue = new Tuple<DateObject, DateObject>(futureBegin, futureEnd);
            ret.PastValue = new Tuple<DateObject, DateObject>(pastBegin, pastEnd);
            ret.Success = true;

            ret.SubDateTimeEntities = new List<object> { pr1, pr2 };

            return ret;
        }

        // Parse specific TimeOfDay like "this night", "early morning", "late evening"
        protected virtual DateTimeResolutionResult ParseSpecificTimeOfDay(string text, DateObject referenceTime)
        {
            var ret = new DateTimeResolutionResult();
            var trimedText = text.Trim().ToLowerInvariant();
            var timeText = trimedText;

            var match = this.Config.PeriodTimeOfDayWithDateRegex.Match(trimedText);

            // Extract early/late prefix from text if any
            bool hasEarly = false, hasLate = false;
            if (match.Success)
            {
                timeText = match.Groups["timeOfDay"].Value;
                if (!string.IsNullOrEmpty(match.Groups["early"].Value))
                {
                    hasEarly = true;
                    ret.Comment = "early";
                }

                if (!hasEarly && !string.IsNullOrEmpty(match.Groups["late"].Value))
                {
                    hasLate = true;
                    ret.Comment = "late";
                }
            }

            // Handle time of day

            // Late/early only works with time of day
            // Only standard time of day (morinng, afternoon, evening and night) will not directly return
            if (!this.Config.GetMatchedTimeRange(timeText, out string timeStr, out int beginHour, out int endHour, out int endMin))
            {
                return ret;
            }

            // Modify time period if "early" or "late" exists
            // Since 'time of day' is defined as four hour periods, 
            // the first 2 hours represent early, the later 2 hours represent late
            if (hasEarly)
            {
                endHour = beginHour + 2;
                // Handling speical case: night ends with 23:59
                if (endMin == 59)
                {
                    endMin = 0;
                }
            }
            else if (hasLate)
            {
                beginHour = beginHour + 2;
            }

            match = this.Config.SpecificTimeOfDayRegex.Match(trimedText);
            if (match.Success && match.Index == 0 && match.Length == trimedText.Length)
            {
                var swift = this.Config.GetSwiftPrefix(trimedText);

                var date = referenceTime.AddDays(swift).Date;
                int day = date.Day, month = date.Month, year = date.Year;

                ret.Timex = FormatUtil.FormatDate(date) + timeStr;

                ret.FutureValue =
                    ret.PastValue =
                        new Tuple<DateObject, DateObject>(DateObject.MinValue.SafeCreateFromValue(year, month, day, beginHour, 0, 0),
                            DateObject.MinValue.SafeCreateFromValue(year, month, day, endHour, endMin, endMin));

                ret.Success = true;
                return ret;
            }

            // Handle Date followed by morning, afternoon and morning, afternoon followed by Date
            match = this.Config.PeriodTimeOfDayWithDateRegex.Match(trimedText);
            if (match.Success)
            {
                var beforeStr = trimedText.Substring(0, match.Index).Trim();
                var afterStr = trimedText.Substring(match.Index + match.Length).Trim();
                
                // Eliminate time period, if any
                var timePeriodErs = this.Config.TimePeriodExtractor.Extract(beforeStr);
                if (timePeriodErs.Count > 0)
                {
                    beforeStr = beforeStr.Remove(timePeriodErs[0].Start ?? 0, timePeriodErs[0].Length ?? 0).Trim();
                }
                else
                {
                    timePeriodErs = this.Config.TimePeriodExtractor.Extract(afterStr);
                    if (timePeriodErs.Count > 0)
                    {
                        afterStr = afterStr.Remove(timePeriodErs[0].Start ?? 0, timePeriodErs[0].Length ?? 0).Trim();
                    }
                }

                var ers = this.Config.DateExtractor.Extract(beforeStr, referenceTime);

                if (ers.Count == 0 || ers[0].Length != beforeStr.Length)
                {
                    var valid = false;

                    if (ers.Count > 0 && ers[0].Start == 0)
                    {
                        var midStr = beforeStr.Substring(ers[0].Start + ers[0].Length ?? 0);
                        if (string.IsNullOrWhiteSpace(midStr.Replace(',', ' ')))
                        {
                            valid = true;
                        }
                    }

                    if (!valid)
                    {
                        ers = this.Config.DateExtractor.Extract(afterStr, referenceTime);

                        if (ers.Count == 0 || ers[0].Length != afterStr.Length)
                        {
                            if (ers.Count > 0 && ers[0].Start + ers[0].Length == afterStr.Length)
                            {
                                var midStr = afterStr.Substring(0, ers[0].Start?? 0);
                                if (string.IsNullOrWhiteSpace(midStr.Replace(',', ' ')))
                                {
                                    valid = true;
                                }
                            }
                        }
                        else
                        {
                            valid = true;
                        }
                    }

                    if (!valid)
                    {
                        return ret;
                    }
                }

                var hasSpecificTimePeriod = false;
                if (timePeriodErs.Count > 0)
                {
                    var timePr = this.Config.TimePeriodParser.Parse(timePeriodErs[0]);
                    if (timePr != null)
                    {
                        var periodFuture = (Tuple<DateObject, DateObject>)(((DateTimeResolutionResult)timePr.Value).FutureValue);
                        var periodPast = (Tuple<DateObject, DateObject>)((DateTimeResolutionResult)timePr.Value).PastValue;

                        if (periodFuture == periodPast)
                        {
                            beginHour = periodFuture.Item1.Hour;
                            endHour = periodFuture.Item2.Hour;
                        }
                        else
                        {
                            if (periodFuture.Item1.Hour >= beginHour || periodFuture.Item2.Hour <= endHour)
                            {
                                beginHour = periodFuture.Item1.Hour;
                                endHour = periodFuture.Item2.Hour;
                            }
                            else
                            {
                                beginHour = periodPast.Item1.Hour;
                                endHour = periodPast.Item2.Hour;
                            }
                        }

                        hasSpecificTimePeriod = true;
                    }
                }

                var pr = this.Config.DateParser.Parse(ers[0], referenceTime);
                var futureDate = (DateObject)((DateTimeResolutionResult)pr.Value).FutureValue;
                var pastDate = (DateObject)((DateTimeResolutionResult)pr.Value).PastValue;

                if (!hasSpecificTimePeriod)
                {
                    ret.Timex = pr.TimexStr + timeStr;
                }
                else
                {
                    ret.Timex = string.Format("{0}T{1},{0}T{2},PT{3}H", pr.TimexStr, beginHour, endHour, endHour - beginHour);
                }

                ret.FutureValue =
                    new Tuple<DateObject, DateObject>(
                        DateObject.MinValue.SafeCreateFromValue(futureDate.Year, futureDate.Month, futureDate.Day, beginHour, 0, 0),
                        DateObject.MinValue.SafeCreateFromValue(futureDate.Year, futureDate.Month, futureDate.Day, endHour, endMin, endMin));

                ret.PastValue =
                    new Tuple<DateObject, DateObject>(
                        DateObject.MinValue.SafeCreateFromValue(pastDate.Year, pastDate.Month, pastDate.Day, beginHour, 0, 0),
                        DateObject.MinValue.SafeCreateFromValue(pastDate.Year, pastDate.Month, pastDate.Day, endHour, endMin, endMin));

                ret.Success = true;

                return ret;
            }

            return ret;
        }

        //TODO: this can be abstracted with the similar method in BaseDatePeriodParser
        // Parse "in 20 minutes"
        private DateTimeResolutionResult ParseDuration(string text, DateObject referenceTime)
        {
            var ret = new DateTimeResolutionResult();

            // For the rest of datetime, it will be handled in next function
            if (Config.RestOfDateTimeRegex.IsMatch(text))
            {
                return ret;
            }

            var ers = Config.DurationExtractor.Extract(text, referenceTime);
            if (ers.Count == 1)
            {
                var pr = Config.DurationParser.Parse(ers[0]);

                var beforeStr = text.Substring(0, pr.Start ?? 0).Trim().ToLowerInvariant();
                var afterStr = text.Substring((pr.Start ?? 0) + (pr.Length ?? 0)).Trim().ToLowerInvariant();

                if (pr.Value != null)
                {
                    var swiftSeconds = 0;
                    var mod = "";
                    var durationResult = (DateTimeResolutionResult)pr.Value;
                    if (durationResult.PastValue is double && durationResult.FutureValue is double)
                    {
                        swiftSeconds = (int)((double)durationResult.FutureValue);
                    }

                    DateObject beginTime;
                    var  endTime = beginTime = referenceTime;

                    var prefixMatch = Config.PastRegex.Match(beforeStr);
                    if (prefixMatch.Success && prefixMatch.Length == beforeStr.Length)
                    {
                        mod = TimeTypeConstants.beforeMod;
                        beginTime = referenceTime.AddSeconds(-swiftSeconds);
                    }

                    prefixMatch = Config.FutureRegex.Match(beforeStr);
                    if (prefixMatch.Success && prefixMatch.Length == beforeStr.Length)
                    {
                        mod = TimeTypeConstants.afterMod;
                        endTime = beginTime.AddSeconds(swiftSeconds);
                    }

                    var suffixMatch = Config.PastRegex.Match(afterStr);
                    if (suffixMatch.Success && suffixMatch.Length == afterStr.Length)
                    {
                        mod = TimeTypeConstants.beforeMod;
                        beginTime = referenceTime.AddSeconds(-swiftSeconds);
                    }

                    suffixMatch = Config.FutureRegex.Match(afterStr);
                    if (suffixMatch.Success && suffixMatch.Length == afterStr.Length)
                    {
                        mod = TimeTypeConstants.afterMod;
                        endTime = beginTime.AddSeconds(swiftSeconds);
                    }

                    ret.Timex =
                        $"({FormatUtil.LuisDate(beginTime)}T{FormatUtil.LuisTime(beginTime)}," +
                        $"{FormatUtil.LuisDate(endTime)}T{FormatUtil.LuisTime(endTime)}," +
                        $"{durationResult.Timex})";

                    ret.FutureValue = ret.PastValue = new Tuple<DateObject, DateObject>(beginTime, endTime);
                    ret.Success = true;

                    if (!string.IsNullOrEmpty(mod))
                    {
                        ((DateTimeResolutionResult)pr.Value).Mod = mod;
                    }

                    ret.SubDateTimeEntities = new List<object> { pr };

                    return ret;
                }
            }

            return ret;
        }

        // Parse "last minute", "next hour"
        private DateTimeResolutionResult ParseRelativeUnit(string text, DateObject referenceTime)
        {
            var ret = new DateTimeResolutionResult();

            var match = Config.RelativeTimeUnitRegex.Match(text);
            
            if (!match.Success)
            {
                match = this.Config.RestOfDateTimeRegex.Match(text);
            }

            if (match.Success)
            {
                var srcUnit = match.Groups["unit"].Value.ToLower();

                var unitStr = Config.UnitMap[srcUnit];

                int swiftValue = 1;
                var prefixMatch = Config.PastRegex.Match(text);
                if (prefixMatch.Success)
                {
                    swiftValue = -1;
                }

                DateObject beginTime;
                var endTime = beginTime = referenceTime;
                var ptTimex = string.Empty;

                if (Config.UnitMap.ContainsKey(srcUnit))
                {
                    switch (unitStr)
                    {
                        case "D":
                            endTime = DateObject.MinValue.SafeCreateFromValue(beginTime.Year, beginTime.Month, beginTime.Day);
                            endTime = endTime.AddDays(1).AddSeconds(-1);
                            ptTimex = "PT" + (endTime - beginTime).TotalSeconds + "S";
                            break;
                        case "H":
                            beginTime = swiftValue > 0 ? beginTime : referenceTime.AddHours(swiftValue);
                            endTime = swiftValue > 0 ? referenceTime.AddHours(swiftValue) : endTime;
                            ptTimex = "PT1H";
                            break;
                        case "M":
                            beginTime = swiftValue > 0 ? beginTime : referenceTime.AddMinutes(swiftValue);
                            endTime = swiftValue > 0 ? referenceTime.AddMinutes(swiftValue) : endTime;
                            ptTimex = "PT1M";
                            break;
                        case "S":
                            beginTime = swiftValue > 0 ? beginTime : referenceTime.AddSeconds(swiftValue);
                            endTime = swiftValue > 0 ? referenceTime.AddSeconds(swiftValue) : endTime;
                            ptTimex = "PT1S";
                            break;
                        default:
                            return ret;
                    }

                    ret.Timex =
                            $"({FormatUtil.LuisDate(beginTime)}T{FormatUtil.LuisTime(beginTime)}," +
                            $"{FormatUtil.LuisDate(endTime)}T{FormatUtil.LuisTime(endTime)},{ptTimex})";

                    ret.FutureValue = ret.PastValue = new Tuple<DateObject, DateObject>(beginTime, endTime);
                    ret.Success = true;

                    return ret;
                }
            }

            return ret;
        }


    }
}