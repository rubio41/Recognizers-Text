﻿using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions.French;
using System.Collections.Generic;
using Microsoft.Recognizers.Text.Number;

namespace Microsoft.Recognizers.Text.DateTime.French
{
    public class FrenchMergedExtractorConfiguration : BaseOptionsConfiguration, IMergedExtractorConfiguration
    {
        public static readonly Regex BeforeRegex =
            new Regex(DateTimeDefinitions.BeforeRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline); // avant - 'before'

        public static readonly Regex AfterRegex =
            new Regex(DateTimeDefinitions.AfterRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline); // ensuite/puis are for adverbs, i.e 'i ate and then i walked', so we'll use apres 

        public static readonly Regex SinceRegex =
            new Regex(DateTimeDefinitions.SinceRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex FromToRegex =
            new Regex(DateTimeDefinitions.FromToRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline); // 'Je vais du lundi au mecredi' - I will go from monday to weds

        public static readonly Regex SingleAmbiguousMonthRegex =
            new Regex(DateTimeDefinitions.SingleAmbiguousMonthRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex PrepositionSuffixRegex =
            new Regex(DateTimeDefinitions.PrepositionSuffixRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex NumberEndingPattern =
            new Regex(DateTimeDefinitions.NumberEndingPattern,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex[] FilterWordRegexList =
        {

        };

        public IDateTimeExtractor DateExtractor { get; }

        public IDateTimeExtractor TimeExtractor { get; }

        public IDateTimeExtractor DateTimeExtractor { get; }

        public IDateTimeExtractor DatePeriodExtractor { get; }

        public IDateTimeExtractor TimePeriodExtractor { get; }

        public IDateTimeExtractor DateTimePeriodExtractor { get; }

        public IDateTimeExtractor DurationExtractor { get; }

        public IDateTimeExtractor SetExtractor { get; }

        public IDateTimeExtractor HolidayExtractor { get; }

        public IExtractor IntegerExtractor { get; }

        public FrenchMergedExtractorConfiguration(DateTimeOptions options) : base(options)
        {

            DateExtractor = new BaseDateExtractor(new FrenchDateExtractorConfiguration());
            TimeExtractor = new BaseTimeExtractor(new FrenchTimeExtractorConfiguration(options));
            DateTimeExtractor = new BaseDateTimeExtractor(new FrenchDateTimeExtractorConfiguration(options));
            DatePeriodExtractor = new BaseDatePeriodExtractor(new FrenchDatePeriodExtractorConfiguration());
            TimePeriodExtractor = new BaseTimePeriodExtractor(new FrenchTimePeriodExtractorConfiguration());
            DateTimePeriodExtractor = new BaseDateTimePeriodExtractor(new FrenchDateTimePeriodExtractorConfiguration());
            DurationExtractor = new BaseDurationExtractor(new FrenchDurationExtractorConfiguration());
            SetExtractor = new BaseSetExtractor(new FrenchSetExtractorConfiguration());
            HolidayExtractor = new BaseHolidayExtractor(new FrenchHolidayExtractorConfiguration());
            IntegerExtractor = new Number.French.IntegerExtractor();
        }

        Regex IMergedExtractorConfiguration.AfterRegex => AfterRegex;
        Regex IMergedExtractorConfiguration.BeforeRegex => BeforeRegex;
        Regex IMergedExtractorConfiguration.SinceRegex => SinceRegex;
        Regex IMergedExtractorConfiguration.FromToRegex => FromToRegex;
        Regex IMergedExtractorConfiguration.SingleAmbiguousMonthRegex => SingleAmbiguousMonthRegex;
        Regex IMergedExtractorConfiguration.PrepositionSuffixRegex => PrepositionSuffixRegex;
        Regex IMergedExtractorConfiguration.NumberEndingPattern => NumberEndingPattern;
        IEnumerable<Regex> IMergedExtractorConfiguration.FilterWordRegexList => FilterWordRegexList;
    }
}
