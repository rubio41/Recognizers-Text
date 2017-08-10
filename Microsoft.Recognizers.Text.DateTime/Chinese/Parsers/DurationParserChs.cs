using System.Collections.Generic;
using System.Globalization;
using Microsoft.Recognizers.Text.NumberWithUnit;
using Microsoft.Recognizers.Text.NumberWithUnit.Chinese;
using static Microsoft.Recognizers.Text.DateTime.Chinese.DurationExtractorChs;
using DateObject = System.DateTime;

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

        public static readonly Dictionary<string, int> UnitValueMap = new Dictionary<string, int>
        {
            {"Y", 31536000},
            {"Mon", 2592000},
            {"W", 604800},
            {"D", 86400},
            {"H", 3600},
            {"M", 60},
            {"S", 1}
        };
        
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
            var dtParseResult = new DateTimeResolutionResult();
            var unitStr = unitResult.Unit;
            var numStr = unitResult.Number;

            if (hasHalfSuffix)
            {
                numStr = (double.Parse(numStr) + 0.5).ToString();
            }

            dtParseResult.Timex = "P" + (BaseDurationParser.IsLessThanDay(unitStr) ? "T" : "") + numStr + unitStr[0];
            dtParseResult.FutureValue = dtParseResult.PastValue = double.Parse(numStr)*UnitValueMap[unitStr];
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

        // handle like "三年半" 
        /*private DateTimeResolutionResult ParserDurationWithHalf(string text, DateObject referenceDate)
        {
            var ret = new DateTimeResolutionResult();
            var durationRes = durationExtractor.Extract(text);
            var numStr = string.Empty;
            var unitStr = string.Empty;
            if (durationRes.Count > 0)
            {
                var match = DateExtractorChs.UnitRegex.Match(text);
                if (match.Success)
                {
                    var afterStr =
                        text.Substring((int)durationRes[0].Start + (int)durationRes[0].Length, 1)
                            .Trim()
                            .ToLowerInvariant();
                    var srcUnit = match.Groups["unit"].Value.ToLowerInvariant();
                    var numberStr =
                        text.Substring((int)durationRes[0].Start, match.Index - (int)durationRes[0].Start)
                            .Trim()
                            .ToLowerInvariant();
                    double number = ConvertChineseToNum(numberStr);
                    if (this.config.UnitMap.ContainsKey(srcUnit))
                    {
                        unitStr = this.config.UnitMap[srcUnit];
                        numStr = number.ToString();
                        if (afterStr.Equals("半"))
                        {
                            number += 0.5;
                            long unitValue = 0;
                            switch (unitStr)
                            {
                                case "Y":
                                    unitValue = this.config.UnitValueMap["year"];
                                    break;
                                case "M":
                                    unitValue = this.config.UnitValueMap["month"];
                                    break;
                                case "W":
                                    unitValue = this.config.UnitValueMap["week"];
                                    break;
                                case "D":
                                    unitValue = this.config.UnitValueMap["day"];
                                    break;
                                default:
                                    return ret;
                            }
                            var timeValue = number * unitValue;
                            ret.Timex = "P" + number.ToString() + unitStr;
                            ret.FutureValue = ret.PastValue = timeValue.ToString();
                            ret.Success = true;
                            return ret;
                        }
                    }
                }
            }

            return ret;
        }*/
    }
}