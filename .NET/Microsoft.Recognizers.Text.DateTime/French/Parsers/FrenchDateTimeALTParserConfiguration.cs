﻿namespace Microsoft.Recognizers.Text.DateTime.French
{
    public class FrenchDateTimeAltParserConfiguration : IDateTimeAltParserConfiguration
    {
        public IDateTimeParser DateTimeParser { get; }

        public IDateTimeParser DateParser { get; }

        public IDateTimeParser TimeParser { get; }

        public IDateTimeParser DateTimePeriodParser { get; }

        public IDateTimeParser TimePeriodParser { get; }

        public FrenchDateTimeAltParserConfiguration(ICommonDateTimeParserConfiguration config)
        {
            DateTimeParser = config.DateTimeParser;
            DateParser = config.DateParser;
            TimeParser = config.TimeParser;
            DateTimePeriodParser = config.DateTimePeriodParser;
            TimePeriodParser = config.TimePeriodParser;
        }

    }
}
