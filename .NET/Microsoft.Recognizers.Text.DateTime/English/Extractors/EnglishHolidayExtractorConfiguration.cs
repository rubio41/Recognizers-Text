﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions.English;

namespace Microsoft.Recognizers.Text.DateTime.English
{
    public class EnglishHolidayExtractorConfiguration : BaseOptionsConfiguration, IHolidayExtractorConfiguration
    {
        public static readonly Regex YearRegex = new Regex(DateTimeDefinitions.YearRegex,
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex H1 =
            new Regex(
                DateTimeDefinitions.HolidayRegex1,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex H2 =
            new Regex(
                DateTimeDefinitions.HolidayRegex2,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex H3 =
            new Regex(
                DateTimeDefinitions.HolidayRegex3,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex[] HolidayRegexList =
        {
            H1,
            H2,
            H3
        };

        public EnglishHolidayExtractorConfiguration() : base(DateTimeOptions.None)
        {
        }

        public IEnumerable<Regex> HolidayRegexes => HolidayRegexList;
    }
}