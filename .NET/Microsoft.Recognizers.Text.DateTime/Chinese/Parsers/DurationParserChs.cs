﻿using System.Collections.Generic;
using System.Globalization;
using DateObject = System.DateTime;
using Microsoft.Recognizers.Definitions.Chinese;
using Microsoft.Recognizers.Text.NumberWithUnit;
using Microsoft.Recognizers.Text.NumberWithUnit.Chinese;
using static Microsoft.Recognizers.Text.DateTime.Chinese.DurationExtractorChs;

namespace Microsoft.Recognizers.Text.DateTime.Chinese
{
    public class DurationParserChs : IDateTimeParser
    {
        public static readonly string ParserName = Constants.SYS_DATETIME_DURATION; //"Duration";

        private static readonly IParser InternalParser = new NumberWithUnitParser(new DurationParserConfiguration());

        internal class DurationParserConfiguration : ChineseNumberWithUnitParserConfiguration
        {
            public DurationParserConfiguration() : base(new CultureInfo(Culture.Chinese))
            {
                this.BindDictionary(DurationExtractorConfiguration.DurationSuffixList);
            }
        }

        public static readonly Dictionary<string, int> UnitValueMap = DateTimeDefinitions.DurationUnitValueMap;

        private readonly IFullDateTimeParserConfiguration config;

        public DurationParserChs(IFullDateTimeParserConfiguration configuration)
        {
            config = configuration;
        }

        public ParseResult Parse(ExtractResult extResult)
        {
            return this.Parse(extResult, DateObject.Now);
        }

        public DateTimeParseResult Parse(ExtractResult er, DateObject refDate)
        {
            var referenceTime = refDate;

            // handle cases like "三年半"
            var hasHalfSuffix = false;
            if (er.Text.EndsWith("半"))
            {
                er.Length -= 1;
                er.Text = er.Text.Substring(0, er.Text.Length - 1);
                hasHalfSuffix = true;
            }

            var parseResult = InternalParser.Parse(er);
            var unitResult = parseResult.Value as UnitValue;

            if (unitResult == null)
            {
                return null;
            }

            var dtParseResult = new DateTimeResolutionResult();
            var unitStr = unitResult.Unit;
            var numStr = unitResult.Number;

            if (hasHalfSuffix)
            {
                numStr = (double.Parse(numStr) + 0.5).ToString();
            }

            dtParseResult.Timex = "P" + (BaseDurationParser.IsLessThanDay(unitStr) ? "T" : "") + numStr + unitStr[0];
            dtParseResult.FutureValue = dtParseResult.PastValue = double.Parse(numStr) * UnitValueMap[unitStr];
            dtParseResult.Success = true;

            if (dtParseResult.Success)
            {
                dtParseResult.FutureResolution = new Dictionary<string, string>
                {
                    {TimeTypeConstants.DURATION, dtParseResult.FutureValue.ToString()}
                };

                dtParseResult.PastResolution = new Dictionary<string, string>
                {
                    {TimeTypeConstants.DURATION, dtParseResult.PastValue.ToString()}
                };
            }

            var ret = new DateTimeParseResult
            {
                Text = er.Text,
                Start = er.Start,
                Length = er.Length,
                Type = er.Type,
                Data = er.Data,
                Value = dtParseResult,
                TimexStr = dtParseResult.Timex,
                ResolutionStr = ""
            };

            return ret;
        }
    }
}