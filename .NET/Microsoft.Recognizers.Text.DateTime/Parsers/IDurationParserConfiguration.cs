﻿using System.Collections.Immutable;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Text.Number;

namespace Microsoft.Recognizers.Text.DateTime
{
    public interface IDurationParserConfiguration : IOptionsConfiguration
    {
        IExtractor CardinalExtractor { get; }

        IExtractor DurationExtractor { get; }

        IParser NumberParser { get; }

        Regex NumberCombinedWithUnit { get; }

        Regex AnUnitRegex { get; }

        Regex AllDateUnitRegex { get; }

        Regex HalfDateUnitRegex { get; }

        Regex SuffixAndRegex { get; }

        Regex FollowedUnit { get; }

        Regex ConjunctionRegex { get; }

        Regex InExactNumberRegex { get; }

        Regex InExactNumberUnitRegex { get; }

        Regex DurationUnitRegex { get; }

        IImmutableDictionary<string, string> UnitMap { get; }

        IImmutableDictionary<string, long> UnitValueMap { get; }

        IImmutableDictionary<string, double> DoubleNumbers { get; }
    }
}
