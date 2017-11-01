﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Definitions.Spanish;

namespace Microsoft.Recognizers.Text.DateTime.Spanish
{
    public class SpanishDatePeriodExtractorConfiguration : IDatePeriodExtractorConfiguration
    {
        // base regexes
        public static readonly Regex TillRegex = new Regex(DateTimeDefinitions.TillRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex AndRegex = new Regex(DateTimeDefinitions.AndRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex DayRegex = new Regex(DateTimeDefinitions.DayRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex MonthNumRegex = new Regex(DateTimeDefinitions.MonthNumRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex YearRegex = new Regex(DateTimeDefinitions.YearRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex RelativeMonthRegex = new Regex(DateTimeDefinitions.RelativeMonthRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex MonthRegex = new Regex(DateTimeDefinitions.MonthRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex MonthSuffixRegex = new Regex(DateTimeDefinitions.MonthSuffixRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex DateUnitRegex = new Regex(DateTimeDefinitions.DateUnitRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex PastRegex = new Regex(DateTimeDefinitions.PastRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex FutureRegex = new Regex(DateTimeDefinitions.FutureRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // composite regexes
        public static readonly Regex SimpleCasesRegex = new Regex(DateTimeDefinitions.SimpleCasesRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex MonthFrontSimpleCasesRegex = new Regex(DateTimeDefinitions.MonthFrontSimpleCasesRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex MonthFrontBetweenRegex = new Regex(DateTimeDefinitions.MonthFrontBetweenRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex DayBetweenRegex = new Regex(DateTimeDefinitions.DayBetweenRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        //TODO: modify it according to the related regex in English
        public static readonly Regex OneWordPeriodRegex = new Regex(DateTimeDefinitions.OneWordPeriodRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex MonthWithYearRegex = new Regex(DateTimeDefinitions.MonthWithYearRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex MonthNumWithYearRegex = new Regex(DateTimeDefinitions.MonthNumWithYearRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex WeekOfMonthRegex = new Regex(DateTimeDefinitions.WeekOfMonthRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex WeekOfYearRegex = new Regex(DateTimeDefinitions.WeekOfYearRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex FollowedDateUnit = new Regex(DateTimeDefinitions.FollowedDateUnit, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex NumberCombinedWithDateUnit = new Regex(DateTimeDefinitions.NumberCombinedWithDateUnit, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex QuarterRegex = new Regex(DateTimeDefinitions.QuarterRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex QuarterRegexYearFront = new Regex(DateTimeDefinitions.QuarterRegexYearFront, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex SeasonRegex = new Regex(DateTimeDefinitions.SeasonRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex WhichWeekRegex = new Regex(DateTimeDefinitions.WhichWeekRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex WeekOfRegex = new Regex(DateTimeDefinitions.WeekOfRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex MonthOfRegex = new Regex(DateTimeDefinitions.MonthOfRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex RangeUnitRegex = new Regex(DateTimeDefinitions.RangeUnitRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public static readonly Regex InConnectorRegex = new Regex(DateTimeDefinitions.InConnectorRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex FromRegex = new Regex(DateTimeDefinitions.FromRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex ConnectorAndRegex = new Regex(DateTimeDefinitions.ConnectorAndRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex BetweenRegex = new Regex(DateTimeDefinitions.BetweenRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        //TODO: add this regex, let it correspond to the one in English
        public static readonly Regex RestOfDateRegex =
            new Regex(@"^[.]", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex[] SimpleCasesRegexes =
        {
            SimpleCasesRegex,
            DayBetweenRegex,
            OneWordPeriodRegex,
            MonthWithYearRegex,
            MonthNumWithYearRegex,
            YearRegex,
            WeekOfMonthRegex,
            WeekOfYearRegex,
            MonthFrontBetweenRegex,
            MonthFrontSimpleCasesRegex,
            QuarterRegex,
            QuarterRegexYearFront,
            SeasonRegex,
            RestOfDateRegex
        };

        public SpanishDatePeriodExtractorConfiguration()
        {
            DatePointExtractor = new BaseDateExtractor(new SpanishDateExtractorConfiguration());
            CardinalExtractor = Number.Spanish.CardinalExtractor.GetInstance();
            DurationExtractor = new BaseDurationExtractor(new SpanishDurationExtractorConfiguration());
        }

        public IExtractor DatePointExtractor { get; }

        public IExtractor CardinalExtractor { get; }

        public IExtractor DurationExtractor { get; }

        IEnumerable<Regex> IDatePeriodExtractorConfiguration.SimpleCasesRegexes => SimpleCasesRegexes;

        Regex IDatePeriodExtractorConfiguration.TillRegex => TillRegex;

        Regex IDatePeriodExtractorConfiguration.DateUnitRegex => DateUnitRegex;

        Regex IDatePeriodExtractorConfiguration.FollowedDateUnit => FollowedDateUnit;

        Regex IDatePeriodExtractorConfiguration.NumberCombinedWithDateUnit => NumberCombinedWithDateUnit;

        Regex IDatePeriodExtractorConfiguration.PastRegex => PastRegex;

        Regex IDatePeriodExtractorConfiguration.FutureRegex => FutureRegex;

        Regex IDatePeriodExtractorConfiguration.WeekOfRegex => WeekOfRegex;

        Regex IDatePeriodExtractorConfiguration.MonthOfRegex => MonthOfRegex;

        Regex IDatePeriodExtractorConfiguration.RangeUnitRegex => RangeUnitRegex;

        Regex IDatePeriodExtractorConfiguration.InConnectorRegex => InConnectorRegex;

        public bool GetFromTokenIndex(string text, out int index)
        {
            index = -1;

            var fromMatch = FromRegex.Match(text);
            if (fromMatch.Success)
            {
                index = fromMatch.Index;
            }

            return fromMatch.Success;
        }

        public bool GetBetweenTokenIndex(string text, out int index)
        {
            index = -1;

            var match = BetweenRegex.Match(text);
            if (match.Success)
            {
                index = match.Index;
            }

            return match.Success;
        }

        public bool HasConnectorToken(string text)
        {
            return ConnectorAndRegex.IsMatch(text);
        }
    }
}
