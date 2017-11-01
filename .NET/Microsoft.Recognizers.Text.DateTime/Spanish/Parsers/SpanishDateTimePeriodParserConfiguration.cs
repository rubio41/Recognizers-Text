﻿using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Microsoft.Recognizers.Text.DateTime.Spanish
{
    public class SpanishDateTimePeriodParserConfiguration : IDateTimePeriodParserConfiguration
    {
        public IExtractor DateExtractor { get; }

        public IExtractor TimeExtractor { get; }

        public IExtractor DateTimeExtractor { get; }

        public IExtractor TimePeriodExtractor { get; }

        public IExtractor CardinalExtractor { get; }

        public IExtractor DurationExtractor { get; }

        public IParser NumberParser { get; }

        public IDateTimeParser DateParser { get; }

        public IDateTimeParser TimeParser { get; }

        public IDateTimeParser DateTimeParser { get; }

        public IDateTimeParser TimePeriodParser { get; }

        public IDateTimeParser DurationParser { get; }

        public Regex PureNumberFromToRegex { get; }

        public Regex PureNumberBetweenAndRegex { get; }

        public Regex SpecificTimeOfDayRegex { get; }

        public Regex TimeOfDayRegex { get; }

        public Regex PastRegex { get; }

        public Regex FutureRegex { get; }

        public Regex NumberCombinedWithUnitRegex { get; }

        public Regex UnitRegex { get; }

        public Regex PeriodTimeOfDayWithDateRegex { get; }

        public Regex RelativeTimeUnitRegex { get; }

        public Regex RestOfDateTimeRegex { get; }

        public IImmutableDictionary<string, string> UnitMap { get; }

        public IImmutableDictionary<string, int> Numbers { get; }

        public SpanishDateTimePeriodParserConfiguration(ICommonDateTimeParserConfiguration config)
        {
            DateExtractor = config.DateExtractor;
            TimeExtractor = config.TimeExtractor;
            DateTimeExtractor = config.DateTimeExtractor;
            TimePeriodExtractor = config.TimePeriodExtractor;
            CardinalExtractor = config.CardinalExtractor;
            DurationExtractor = config.DurationExtractor;
            NumberParser = config.NumberParser;
            DateParser = config.DateParser;
            TimeParser = config.TimeParser;
            DateTimeParser = config.DateTimeParser;
            TimePeriodParser = config.TimePeriodParser;
            DurationParser = config.DurationParser;

            PureNumberFromToRegex = SpanishTimePeriodExtractorConfiguration.PureNumFromTo;
            PureNumberBetweenAndRegex = SpanishTimePeriodExtractorConfiguration.PureNumBetweenAnd;
            SpecificTimeOfDayRegex = SpanishDateTimeExtractorConfiguration.SpecificTimeOfDayRegex;
            TimeOfDayRegex = SpanishDateTimeExtractorConfiguration.TimeOfDayRegex;
            PastRegex = SpanishDatePeriodExtractorConfiguration.PastRegex;
            FutureRegex = SpanishDatePeriodExtractorConfiguration.FutureRegex;
            NumberCombinedWithUnitRegex = SpanishDateTimePeriodExtractorConfiguration.NumberCombinedWithUnit;
            UnitRegex = SpanishTimePeriodExtractorConfiguration.UnitRegex;
            PeriodTimeOfDayWithDateRegex = SpanishDateTimePeriodExtractorConfiguration.PeriodTimeOfDayWithDateRegex;
            RelativeTimeUnitRegex = SpanishDateTimePeriodExtractorConfiguration.RelativeTimeUnitRegex;
            RestOfDateTimeRegex = SpanishDateTimePeriodExtractorConfiguration.RestOfDateTimeRegex;
            UnitMap = config.UnitMap;
            Numbers = config.Numbers;
        }

        public bool GetMatchedTimeRange(string text, out string timeStr, out int beginHour, out int endHour, out int endMin)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            beginHour = 0;
            endHour = 0;
            endMin = 0;

            //TODO: modify it according to the coresponding function in English part
            if (trimedText.EndsWith("madrugada"))
            {
                timeStr = "TDA";
                beginHour = 4;
                endHour = 8;
            }
            else if (trimedText.EndsWith("mañana"))
            {
                timeStr = "TMO";
                beginHour = 8;
                endHour = 12;
            }
            else if (trimedText.Contains("pasado mediodia") || trimedText.Contains("pasado el mediodia"))
            {
                timeStr = "TAF";
                beginHour = 12;
                endHour = 16;
            }
            else if (trimedText.EndsWith("tarde"))
            {
                timeStr = "TEV";
                beginHour = 16;
                endHour = 20;
            }
            else if (trimedText.EndsWith("noche"))
            {
                timeStr = "TNI";
                beginHour = 20;
                endHour = 23;
                endMin = 59;
            }
            else
            {
                timeStr = null;
                return false;
            }

            return true;
        }

        public int GetSwiftPrefix(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            var swift = 0;

            //TODO: Replace with a regex
            if (SpanishDatePeriodParserConfiguration.PastPrefixRegex.IsMatch(trimedText) ||
                trimedText.Equals("anoche"))
            {
                swift = -1;
            }
            else if (SpanishDatePeriodParserConfiguration.NextPrefixRegex.IsMatch(trimedText))
            {
                swift = 1;
            }

            return swift;
        }
    }
}
