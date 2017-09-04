﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Text.DateTime.Utilities;

namespace Microsoft.Recognizers.Text.DateTime.Spanish
{
    public class SpanishDateParserConfiguration : IDateParserConfiguration
    {
        public string DateTokenPrefix { get; }

        public IExtractor IntegerExtractor { get; }

        public IExtractor OrdinalExtractor { get; }

        public IExtractor CardinalExtractor { get; }

        public IParser NumberParser { get; }

        public IExtractor DurationExtractor { get; }

        public IParser DurationParser { get; }

        public IImmutableDictionary<string, string> UnitMap { get; }

        public IEnumerable<Regex> DateRegexes { get; }

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

        public SpanishDateParserConfiguration(ICommonDateTimeParserConfiguration config)
        {
            DateTokenPrefix = "en ";
            DateRegexes = SpanishDateExtractorConfiguration.DateRegexList;
            OnRegex = SpanishDateExtractorConfiguration.OnRegex;
            SpecialDayRegex = SpanishDateExtractorConfiguration.SpecialDayRegex;
            NextRegex = SpanishDateExtractorConfiguration.NextDateRegex;
            ThisRegex = SpanishDateExtractorConfiguration.ThisRegex;
            LastRegex = SpanishDateExtractorConfiguration.LastDateRegex;
            UnitRegex = SpanishDateExtractorConfiguration.DateUnitRegex;
            WeekDayRegex = SpanishDateExtractorConfiguration.WeekDayRegex;
            MonthRegex = SpanishDateExtractorConfiguration.MonthRegex;
            WeekDayOfMonthRegex = SpanishDateExtractorConfiguration.WeekDayOfMonthRegex;
            DayOfMonth = config.DayOfMonth;
            DayOfWeek = config.DayOfWeek;
            MonthOfYear = config.MonthOfYear;
            CardinalMap = config.CardinalMap;
            IntegerExtractor = config.IntegerExtractor;
            OrdinalExtractor = config.OrdinalExtractor;
            CardinalExtractor = config.CardinalExtractor;
            NumberParser = config.NumberParser;
            DurationExtractor = config.DurationExtractor;
            DurationParser = config.DurationParser;
            UnitMap = config.UnitMap;
            UtilityConfiguration = config.UtilityConfiguration;
        }

        public int GetSwiftDay(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant().Normalized();
            var swift = 0;

            if (trimedText.Equals("hoy") || trimedText.Equals("el dia"))
            {
                swift = 0;
            }
            else if (trimedText.Equals("mañana") ||
                     trimedText.EndsWith("dia siguiente") ||
                     trimedText.EndsWith("el dia de mañana") ||
                     trimedText.EndsWith("proximo dia"))
            {
                swift = 1;
            }
            else if (trimedText.Equals("ayer"))
            {
                swift = -1;
            }
            else if (trimedText.EndsWith("pasado mañana") ||
                     trimedText.EndsWith("dia despues de mañana"))
            {
                swift = 2;
            }
            else if (trimedText.EndsWith("anteayer") ||
                     trimedText.EndsWith("dia antes de ayer"))
            {
                swift = -2;
            }
            else if (trimedText.EndsWith("ultimo dia"))
            {
                swift = -1;
            }

            return swift;
        }

        public int GetSwiftMonth(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            var swift = 0;

            //TODO: Replace with a regex
            //TODO: Add 'upcoming' key word
            if (trimedText.StartsWith("proximo") || trimedText.StartsWith("próximo") ||
                trimedText.StartsWith("proxima") || trimedText.StartsWith("próxima"))
            {
                swift = 1;
            }

            //TODO: Replace with a regex
            if (trimedText.StartsWith("ultimo") || trimedText.StartsWith("último") ||
                trimedText.StartsWith("ultima") || trimedText.StartsWith("última"))
            {
                swift = -1;
            }

            return swift;
        }

        public bool IsCardinalLast(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();

            //TODO: Replace with a regex
            return (
                trimedText.Equals("ultimo") || trimedText.Equals("último") ||
                trimedText.Equals("ultima") || trimedText.Equals("última"));
        }
    }

    public static class StringExtension
    {
        public static string Normalized(this string text)
        {
            return text
                .Replace('á', 'a')
                .Replace('é', 'e')
                .Replace('í', 'i')
                .Replace('ó', 'o')
                .Replace('ú', 'u');
        }
    }
}
