﻿using System.Collections.Immutable;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions.Portuguese;
using Microsoft.Recognizers.Text.Number;

namespace Microsoft.Recognizers.Text.DateTime.Portuguese
{
    public class PortugueseDatePeriodParserConfiguration : BaseOptionsConfiguration, IDatePeriodParserConfiguration
    {
        public string TokenBeforeDate { get; }

        #region internalParsers

        public IDateTimeExtractor DateExtractor { get; }

        public IExtractor CardinalExtractor { get; }

        public IDateTimeExtractor DurationExtractor { get; }

        public IParser NumberParser { get; }

        public IDateTimeParser DateParser { get; }

        public IDateTimeParser DurationParser { get; }

        #endregion

        #region Regexes

        public Regex MonthFrontBetweenRegex { get; }
        public Regex BetweenRegex { get; }
        public Regex MonthFrontSimpleCasesRegex { get; }
        public Regex SimpleCasesRegex { get; }
        public Regex OneWordPeriodRegex { get; }
        public Regex MonthWithYear { get; }
        public Regex MonthNumWithYear { get; }
        public Regex YearRegex { get; }
        public Regex PastRegex { get; }
        public Regex FutureRegex { get; }
        public Regex NumberCombinedWithUnit { get; }
        public Regex WeekOfMonthRegex { get; }
        public Regex WeekOfYearRegex { get; }
        public Regex QuarterRegex { get; }
        public Regex QuarterRegexYearFront { get; }
        public Regex SeasonRegex { get; }
        public Regex WhichWeekRegex { get; }
        public Regex WeekOfRegex { get; }
        public Regex MonthOfRegex { get; }
        public Regex InConnectorRegex { get; }
        public Regex RestOfDateRegex { get; }
        public Regex LaterEarlyPeriodRegex { get; }
        public Regex WeekWithWeekDayRangeRegex { get; }

        //TODO: config this according to English
        public static readonly Regex NextPrefixRegex = new Regex(DateTimeDefinitions.NextPrefixRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex PastPrefixRegex = new Regex(DateTimeDefinitions.PastPrefixRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex ThisPrefixRegex = new Regex(DateTimeDefinitions.ThisPrefixRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        Regex IDatePeriodParserConfiguration.NextPrefixRegex => NextPrefixRegex;
        Regex IDatePeriodParserConfiguration.PastPrefixRegex => PastPrefixRegex;
        Regex IDatePeriodParserConfiguration.ThisPrefixRegex => ThisPrefixRegex;

        #endregion

        #region Dictionaries

        public IImmutableDictionary<string, string> UnitMap { get; }

        public IImmutableDictionary<string, int> CardinalMap { get; }

        public IImmutableDictionary<string, int> DayOfMonth { get; }

        public IImmutableDictionary<string, int> MonthOfYear { get; }

        public IImmutableDictionary<string, string> SeasonMap { get; }
        
        #endregion


        public PortugueseDatePeriodParserConfiguration(ICommonDateTimeParserConfiguration config) : base(config.Options)
        {
            TokenBeforeDate = DateTimeDefinitions.TokenBeforeDate;
            CardinalExtractor = config.CardinalExtractor;
            NumberParser = config.NumberParser;
            DurationExtractor = config.DurationExtractor;
            DateExtractor = config.DateExtractor;
            DurationParser = config.DurationParser;
            DateParser = config.DateParser;
            MonthFrontBetweenRegex = PortugueseDatePeriodExtractorConfiguration.MonthFrontBetweenRegex;
            BetweenRegex = PortugueseDatePeriodExtractorConfiguration.DayBetweenRegex;
            MonthFrontSimpleCasesRegex = PortugueseDatePeriodExtractorConfiguration.MonthFrontSimpleCasesRegex;
            SimpleCasesRegex = PortugueseDatePeriodExtractorConfiguration.SimpleCasesRegex;
            OneWordPeriodRegex = PortugueseDatePeriodExtractorConfiguration.OneWordPeriodRegex;
            MonthWithYear = PortugueseDatePeriodExtractorConfiguration.MonthWithYearRegex;
            MonthNumWithYear = PortugueseDatePeriodExtractorConfiguration.MonthNumWithYearRegex;
            YearRegex = PortugueseDatePeriodExtractorConfiguration.YearRegex;
            PastRegex = PortugueseDatePeriodExtractorConfiguration.PastRegex;
            FutureRegex = PortugueseDatePeriodExtractorConfiguration.FutureRegex;
            NumberCombinedWithUnit = PortugueseDurationExtractorConfiguration.NumberCombinedWithUnit;
            WeekOfMonthRegex = PortugueseDatePeriodExtractorConfiguration.WeekOfMonthRegex;
            WeekOfYearRegex = PortugueseDatePeriodExtractorConfiguration.WeekOfYearRegex;
            QuarterRegex = PortugueseDatePeriodExtractorConfiguration.QuarterRegex;
            QuarterRegexYearFront = PortugueseDatePeriodExtractorConfiguration.QuarterRegexYearFront;
            SeasonRegex = PortugueseDatePeriodExtractorConfiguration.SeasonRegex;
            WhichWeekRegex = PortugueseDatePeriodExtractorConfiguration.WhichWeekRegex;
            WeekOfRegex = PortugueseDatePeriodExtractorConfiguration.WeekOfRegex;
            MonthOfRegex = PortugueseDatePeriodExtractorConfiguration.MonthOfRegex;
            RestOfDateRegex = PortugueseDatePeriodExtractorConfiguration.RestOfDateRegex;
            LaterEarlyPeriodRegex = PortugueseDatePeriodExtractorConfiguration.LaterEarlyPeriodRegex;
            WeekWithWeekDayRangeRegex = PortugueseDatePeriodExtractorConfiguration.WeekWithWeekDayRangeRegex;
            InConnectorRegex = config.UtilityConfiguration.InConnectorRegex;
            UnitMap = config.UnitMap;
            CardinalMap = config.CardinalMap;
            DayOfMonth = config.DayOfMonth;
            MonthOfYear = config.MonthOfYear;
            SeasonMap = config.SeasonMap;
        }

        public int GetSwiftDayOrMonth(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            var swift = 0;

            if (NextPrefixRegex.IsMatch(trimedText))
            {
                swift = 1;
            }

            if (PastPrefixRegex.IsMatch(trimedText))
            {
                swift = -1;
            }
            return swift;
        }

        public int GetSwiftYear(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            var swift = -10;
            if (NextPrefixRegex.IsMatch(trimedText))
            {
                swift = 1;
            }

            if (PastPrefixRegex.IsMatch(trimedText))
            {
                swift = -1;
            }
            else if (ThisPrefixRegex.IsMatch(trimedText))
            {
                swift = 0;
            }

            return swift;
        }

        public bool IsFuture(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            return ThisPrefixRegex.IsMatch(trimedText) || NextPrefixRegex.IsMatch(trimedText);
        }

        public bool IsLastCardinal(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            return PastPrefixRegex.IsMatch(trimedText);
        }

        public bool IsMonthOnly(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            return (trimedText.EndsWith("mes") || trimedText.EndsWith("meses"));
        }

        public bool IsMonthToDate(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            return (trimedText.Equals("mes a la fecha") || trimedText.Equals("meses a la fecha"));
        }

        public bool IsWeekend(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            return trimedText.EndsWith("fin de semana");
        }

        public bool IsWeekOnly(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            return (trimedText.EndsWith("semana") && !trimedText.EndsWith("fin de semana"));
        }

        public bool IsYearOnly(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            return (trimedText.EndsWith("año") || trimedText.EndsWith("años"));
        }

        public bool IsYearToDate(string text)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            return (trimedText.Equals("año a la fecha") || trimedText.Equals("años a la fecha"));
        }
    }
}
