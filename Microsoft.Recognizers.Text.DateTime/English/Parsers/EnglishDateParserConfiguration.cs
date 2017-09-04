﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Text.DateTime.Utilities;
using Microsoft.Recognizers.Definitions.English;

namespace Microsoft.Recognizers.Text.DateTime.English
{
    public class EnglishDateParserConfiguration : IDateParserConfiguration
    {
        public string DateTokenPrefix { get; }

        public IExtractor IntegerExtractor { get; }

        public IExtractor OrdinalExtractor { get; }

        public IExtractor CardinalExtractor { get; }

        public IParser NumberParser { get; }

        public IExtractor DurationExtractor { get; }

        public IParser DurationParser { get; }

        public IEnumerable<Regex> DateRegexes { get; }

        public IImmutableDictionary<string, string> UnitMap { get; }

        public Regex OnRegex { get; }

        public Regex SpecialDayRegex { get; }

        public Regex NextRegex { get; }

        public Regex ThisRegex { get; }

        public Regex LastRegex { get; }

        public Regex UnitRegex { get; }

        public Regex WeekDayRegex { get; }

        public Regex MonthRegex { get; }

        public Regex WeekDayOfMonthRegex { get; }

        public IImmutableDictionary<string, int> DayOfMonth { get; }

        public IImmutableDictionary<string, int> DayOfWeek { get; }

        public IImmutableDictionary<string, int> MonthOfYear { get; }

        public IImmutableDictionary<string, int> CardinalMap { get; }

        public IDateTimeUtilityConfiguration UtilityConfiguration { get; }

        public EnglishDateParserConfiguration(ICommonDateTimeParserConfiguration config)
        {
            DateTokenPrefix = DateTimeDefinitions.DateTokenPrefix;
            IntegerExtractor = config.IntegerExtractor;
            OrdinalExtractor = config.OrdinalExtractor;
            CardinalExtractor = config.CardinalExtractor;
            NumberParser = config.NumberParser;
            DurationExtractor = config.DurationExtractor;
            DurationParser = config.DurationParser;
            DateRegexes = EnglishDateExtractorConfiguration.DateRegexList;
            OnRegex = EnglishDateExtractorConfiguration.OnRegex;
            SpecialDayRegex = EnglishDateExtractorConfiguration.SpecialDayRegex;
            NextRegex = EnglishDateExtractorConfiguration.NextDateRegex;
            ThisRegex = EnglishDateExtractorConfiguration.ThisRegex;
            LastRegex = EnglishDateExtractorConfiguration.LastDateRegex;
            UnitRegex = EnglishDateExtractorConfiguration.DateUnitRegex;
            WeekDayRegex = EnglishDateExtractorConfiguration.WeekDayRegex;
            MonthRegex = EnglishDateExtractorConfiguration.MonthRegex;
            WeekDayOfMonthRegex = EnglishDateExtractorConfiguration.WeekDayOfMonthRegex;
            DayOfMonth = config.DayOfMonth;
            DayOfWeek = config.DayOfWeek;
            MonthOfYear = config.MonthOfYear;
            CardinalMap = config.CardinalMap;
            UnitMap = config.UnitMap;
            UtilityConfiguration = config.UtilityConfiguration;
        }

        public int GetSwiftDay(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            var swift = 0;
            if (trimedText.Equals("today") || trimedText.Equals("the day"))
            {
                swift = 0;
            }
            else if (trimedText.Equals("tomorrow") || trimedText.Equals("tmr") ||
                     trimedText.Equals("next day") || trimedText.Equals("the next day"))
            {
                swift = 1;
            }
            else if (trimedText.Equals("yesterday"))
            {
                swift = -1;
            }
            else if (trimedText.EndsWith("day after tomorrow") ||
                     trimedText.EndsWith("day after tmr"))
            {
                swift = 2;
            }
            else if (trimedText.EndsWith("day before yesterday"))
            {
                swift = -2;
            }
            else if (trimedText.EndsWith("last day"))
            {
                swift = -1;
            }
            return swift;
        }

        public int GetSwiftMonth(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            var swift = 0;
            if (trimedText.StartsWith("next") || trimedText.StartsWith("upcoming"))
            {
                swift = 1;
            }
            else if (trimedText.StartsWith("last"))
            {
                swift = -1;
            }
            return swift;
        }

        public bool IsCardinalLast(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            return trimedText.Equals("last");
        }
    }
}
