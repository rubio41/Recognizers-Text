﻿using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Microsoft.Recognizers.Text.DateTime
{
    public interface ISetParserConfiguration
    {
        IExtractor DurationExtractor { get; }

        IDateTimeParser DurationParser { get; }

        IExtractor TimeExtractor { get; }

        IDateTimeParser TimeParser { get; }

        IExtractor DateExtractor { get; }

        IDateTimeParser DateParser { get; }

        IExtractor DateTimeExtractor { get; }

        IDateTimeParser DateTimeParser { get; }

        IExtractor DatePeriodExtractor { get; }

        IDateTimeParser DatePeriodParser { get; }

        IExtractor TimePeriodExtractor { get; }

        IDateTimeParser TimePeriodParser { get; }

        IExtractor DateTimePeriodExtractor { get; }

        IDateTimeParser DateTimePeriodParser { get; }

        IImmutableDictionary<string, string> UnitMap { get; }

        Regex EachPrefixRegex { get; }

        Regex PeriodicRegex { get; }

        Regex EachUnitRegex { get; }

        Regex EachDayRegex { get; }

        Regex StrictWeekDayRegex { get; }

        bool GetMatchedDailyTimex(string text, out string timex);

        bool GetMatchedUnitTimex(string text, out string timex);
    }
}
