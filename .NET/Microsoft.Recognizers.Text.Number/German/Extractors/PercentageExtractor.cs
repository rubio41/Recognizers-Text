﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions.German;

namespace Microsoft.Recognizers.Text.Number.German
{
    public sealed class PercentageExtractor : BasePercentageExtractor
    {
        public PercentageExtractor() : base(NumberExtractor.GetInstance()) { }

        protected override ImmutableHashSet<Regex> InitRegexes()
        {
            HashSet<string> regexStrs = new HashSet<string>
            {
                NumbersDefinitions.NumberWithSuffixPercentage,
                NumbersDefinitions.NumberWithPrefixPercentage
            };

            return BuildRegexes(regexStrs);
        }
    }
}