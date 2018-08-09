﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions.Portuguese;
using Microsoft.Recognizers.Text.Number;

namespace Microsoft.Recognizers.Text.DateTime.Portuguese
{
    public class PortugueseDateTimePeriodExtractorConfiguration : BaseOptionsConfiguration, IDateTimePeriodExtractorConfiguration
    {
        public static readonly Regex NumberCombinedWithUnit = new Regex(DateTimeDefinitions.DateTimePeriodNumberCombinedWithUnit, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex FromRegex = new Regex(DateTimeDefinitions.FromRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex ConnectorAndRegex = new Regex(DateTimeDefinitions.ConnectorAndRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex BetweenRegex = new Regex(DateTimeDefinitions.BetweenRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public PortugueseDateTimePeriodExtractorConfiguration() : base(DateTimeOptions.None)
        {
            CardinalExtractor = Number.Portuguese.CardinalExtractor.GetInstance();

            SingleDateExtractor = new BaseDateExtractor(new PortugueseDateExtractorConfiguration());
            SingleTimeExtractor = new BaseTimeExtractor(new PortugueseTimeExtractorConfiguration());
            SingleDateTimeExtractor = new BaseDateTimeExtractor(new PortugueseDateTimeExtractorConfiguration());
            DurationExtractor = new BaseDurationExtractor(new PortugueseDurationExtractorConfiguration());
            TimePeriodExtractor = new BaseTimePeriodExtractor(new PortugueseTimePeriodExtractorConfiguration());
        }

        public IExtractor CardinalExtractor { get; }

        public IDateTimeExtractor SingleDateExtractor { get; }

        public IDateTimeExtractor SingleTimeExtractor { get; }

        public IDateTimeExtractor SingleDateTimeExtractor { get; }

        public IDateTimeExtractor DurationExtractor { get; }

        public IDateTimeExtractor TimePeriodExtractor { get; }

        public IEnumerable<Regex> SimpleCasesRegex => new[] 
        {
            PortugueseTimePeriodExtractorConfiguration.PureNumFromTo,
            PortugueseTimePeriodExtractorConfiguration.PureNumBetweenAnd
        };

        public Regex PrepositionRegex => PortugueseDateTimeExtractorConfiguration.PrepositionRegex;

        public Regex TillRegex => PortugueseDatePeriodExtractorConfiguration.TillRegex;

        public Regex SpecificTimeOfDayRegex => PortugueseDateTimeExtractorConfiguration.SpecificTimeOfDayRegex;

        public Regex TimeOfDayRegex => PortugueseDateTimeExtractorConfiguration.TimeOfDayRegex;

        public Regex FollowedUnit => PortugueseTimePeriodExtractorConfiguration.FollowedUnit;

        public Regex TimeUnitRegex => PortugueseTimePeriodExtractorConfiguration.UnitRegex;

        public Regex PastPrefixRegex => PortugueseDatePeriodExtractorConfiguration.PastRegex;

        public Regex NextPrefixRegex => PortugueseDatePeriodExtractorConfiguration.FutureRegex;

        //TODO: add this
        public static readonly Regex WeekDayRegex = new Regex(@"^[\.]", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        //TODO: add this according to the related part in English
        public static readonly Regex RestOfDateTimeRegex = new Regex(@"^[\.]", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex PeriodTimeOfDayWithDateRegex = new Regex(DateTimeDefinitions.PeriodTimeOfDayWithDateRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex RelativeTimeUnitRegex = new Regex(DateTimeDefinitions.RelativeTimeUnitRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex GeneralEndingRegex =
            new Regex(DateTimeDefinitions.GeneralEndingRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex MiddlePauseRegex =
            new Regex(DateTimeDefinitions.MiddlePauseRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        Regex IDateTimePeriodExtractorConfiguration.NumberCombinedWithUnit => NumberCombinedWithUnit;

        Regex IDateTimePeriodExtractorConfiguration.WeekDayRegex => WeekDayRegex;

        Regex IDateTimePeriodExtractorConfiguration.PeriodTimeOfDayWithDateRegex => PeriodTimeOfDayWithDateRegex;

        Regex IDateTimePeriodExtractorConfiguration.RelativeTimeUnitRegex => RelativeTimeUnitRegex;

        Regex IDateTimePeriodExtractorConfiguration.RestOfDateTimeRegex => RestOfDateTimeRegex;

        Regex IDateTimePeriodExtractorConfiguration.GeneralEndingRegex => GeneralEndingRegex;

        Regex IDateTimePeriodExtractorConfiguration.MiddlePauseRegex => MiddlePauseRegex;

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
