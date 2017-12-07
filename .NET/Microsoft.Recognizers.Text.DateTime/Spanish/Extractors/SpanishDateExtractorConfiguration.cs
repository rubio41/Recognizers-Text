﻿using System.Collections.Generic;
using System.Collections.Immutable;

using System.Text.RegularExpressions;
using Microsoft.Recognizers.Definitions.Spanish;
using Microsoft.Recognizers.Text.DateTime.Spanish.Utilities;
using Microsoft.Recognizers.Text.DateTime.Utilities;
using Microsoft.Recognizers.Text.Number;
using Microsoft.Recognizers.Text.Number.Spanish;

namespace Microsoft.Recognizers.Text.DateTime.Spanish
{
    public class SpanishDateExtractorConfiguration : BaseOptionsConfiguration, IDateExtractorConfiguration
    {
        public static readonly Regex MonthRegex = 
            new Regex(DateTimeDefinitions.MonthRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex DayRegex = 
            new Regex(DateTimeDefinitions.DayRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex MonthNumRegex = 
            new Regex(DateTimeDefinitions.MonthNumRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex YearRegex = 
            new Regex(DateTimeDefinitions.YearRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex WeekDayRegex = 
            new Regex(DateTimeDefinitions.WeekDayRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex OnRegex = 
            new Regex(DateTimeDefinitions.OnRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex RelaxedOnRegex = 
            new Regex(DateTimeDefinitions.RelaxedOnRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex ThisRegex = 
            new Regex(DateTimeDefinitions.ThisRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex LastDateRegex = 
            new Regex(DateTimeDefinitions.LastDateRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex NextDateRegex = 
            new Regex(DateTimeDefinitions.NextDateRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex SpecialDayRegex = 
            new Regex(DateTimeDefinitions.SpecialDayRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex DateUnitRegex = 
            new Regex(DateTimeDefinitions.DateUnitRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex WeekDayOfMonthRegex = 
            new Regex(DateTimeDefinitions.WeekDayOfMonthRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex SpecialDateRegex = 
            new Regex(DateTimeDefinitions.SpecialDateRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        //TODO: modify below regex according to the counterpart in English
        public static readonly Regex ForTheRegex = new Regex($@"^[.]",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        //TODO: modify below regex according to the counterpart in English
        public static readonly Regex WeekDayAndDayOfMothRegex = new Regex($@"^[.]",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        //TODO: modify below regex according to the counterpart in English
        public static readonly Regex RelativeMonthRegex = new Regex($@"^[.]",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex[] DateRegexList =
        {
            // (domingo,)? 5 de Abril
            new Regex(DateTimeDefinitions.DateExtractor1, RegexOptions.IgnoreCase | RegexOptions.Singleline),

            // (domingo,)? 5 de Abril 5, 2016
            new Regex(DateTimeDefinitions.DateExtractor2, RegexOptions.IgnoreCase | RegexOptions.Singleline),

            // (domingo,)? 6 de Abril
            new Regex(DateTimeDefinitions.DateExtractor3, RegexOptions.IgnoreCase | RegexOptions.Singleline),

            // 3-23-2017
            new Regex(DateTimeDefinitions.DateExtractor4, RegexOptions.IgnoreCase | RegexOptions.Singleline),

            // 23-3-2015
            new Regex(DateTimeDefinitions.DateExtractor5, RegexOptions.IgnoreCase | RegexOptions.Singleline),

            // el 1/3
            new Regex(DateTimeDefinitions.DateExtractor6, RegexOptions.IgnoreCase | RegexOptions.Singleline),

            // 7/23
            new Regex(DateTimeDefinitions.DateExtractor7, RegexOptions.IgnoreCase | RegexOptions.Singleline),

            // el 24-12
            new Regex(DateTimeDefinitions.DateExtractor8, RegexOptions.IgnoreCase | RegexOptions.Singleline),

            // 23/7
            new Regex(DateTimeDefinitions.DateExtractor9, RegexOptions.IgnoreCase | RegexOptions.Singleline),

            // 2015-12-23
            new Regex(DateTimeDefinitions.DateExtractor10, RegexOptions.IgnoreCase | RegexOptions.Singleline)
        };

        public static readonly Regex[] ImplicitDateList =
        {
            OnRegex, RelaxedOnRegex, SpecialDayRegex, ThisRegex, LastDateRegex, NextDateRegex,
            WeekDayRegex, WeekDayOfMonthRegex, SpecialDateRegex
        };

        public static readonly Regex OfMonth = new Regex(DateTimeDefinitions.OfMonthRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex MonthEnd = new Regex(DateTimeDefinitions.MonthEndRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly ImmutableDictionary<string, int> DayOfWeek = DateTimeDefinitions.DayOfWeek.ToImmutableDictionary();

        public SpanishDateExtractorConfiguration() : base(DateTimeOptions.None)
        {
            IntegerExtractor = new IntegerExtractor();
            OrdinalExtractor = new OrdinalExtractor();
            NumberParser = new BaseNumberParser(new SpanishNumberParserConfiguration());
            DurationExtractor = new BaseDurationExtractor(new SpanishDurationExtractorConfiguration());
            UtilityConfiguration = new SpanishDatetimeUtilityConfiguration();
        }

        public IExtractor IntegerExtractor { get; }

        public IExtractor OrdinalExtractor { get; }

        public IParser NumberParser { get; }

        public IDateTimeExtractor DurationExtractor { get; }

        public IDateTimeUtilityConfiguration UtilityConfiguration { get; }

        IEnumerable<Regex> IDateExtractorConfiguration.DateRegexList => DateRegexList;

        IEnumerable<Regex> IDateExtractorConfiguration.ImplicitDateList => ImplicitDateList;

        IImmutableDictionary<string, int> IDateExtractorConfiguration.DayOfWeek => DayOfWeek;

        Regex IDateExtractorConfiguration.OfMonth => OfMonth;

        Regex IDateExtractorConfiguration.MonthEnd => MonthEnd;

        Regex IDateExtractorConfiguration.DateUnitRegex => DateUnitRegex;

        Regex IDateExtractorConfiguration.ForTheRegex => ForTheRegex;

        Regex IDateExtractorConfiguration.WeekDayAndDayOfMothRegex => WeekDayAndDayOfMothRegex;

        Regex IDateExtractorConfiguration.RelativeMonthRegex => RelativeMonthRegex;

        Regex IDateExtractorConfiguration.WeekDayRegex => WeekDayRegex;
    }
}
