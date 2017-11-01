﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Definitions;
using Microsoft.Recognizers.Text.Number;
using Microsoft.Recognizers.Text.Number.English;

namespace Microsoft.Recognizers.Text.DateTime
{
    public class BaseTimeExtractor : IExtractor
    {
        private static readonly string ExtractorName = Constants.SYS_DATETIME_TIME; // "Time";

        public static readonly Regex HourRegex = new Regex(BaseDateTime.HourRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex MinuteRegex = new Regex(BaseDateTime.MinuteRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex SecondRegex = new Regex(BaseDateTime.SecondRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly ITimeExtractorConfiguration config;

        public BaseTimeExtractor(ITimeExtractorConfiguration config)
        {
            this.config = config;
        }

        public virtual List<ExtractResult> Extract(string text)
        {
            var tokens = new List<Token>();
            tokens.AddRange(BasicRegexMatch(text));
            tokens.AddRange(AtRegexMatch(text));
            tokens.AddRange(SpecialsRegexMatch(text));

            return Token.MergeAllTokens(tokens, text, ExtractorName);
        }

        private List<Token> BasicRegexMatch(string text)
        {
            var ret = new List<Token>();
            foreach (var regex in this.config.TimeRegexList)
            {
                var matches = regex.Matches(text);
                foreach (Match match in matches)
                {
                    ret.Add(new Token(match.Index, match.Index + match.Length));
                }
            }
            return ret;
        }

        private List<Token> AtRegexMatch(string text)
        {
            var ret = new List<Token>();
            // handle "at 5", "at seven"
            if (this.config.AtRegex.IsMatch(text))
            {
                var matches = this.config.AtRegex.Matches(text);
                foreach (Match match in matches)
                {
                    if (match.Index + match.Length < text.Length &&
                        text[match.Index + match.Length].Equals('%'))
                    {
                        continue;
                    }
                    ret.Add(new Token(match.Index, match.Index + match.Length));
                }
            }
            return ret;
        }

        private List<Token> SpecialsRegexMatch(string text)
        {
            var ret = new List<Token>();
            MatchCollection matches;
            // handle "ish"
            if (this.config.IshRegex != null && this.config.IshRegex.IsMatch(text))
            {
                matches = this.config.IshRegex.Matches(text);

                foreach (Match match in matches)
                {
                    ret.Add(new Token(match.Index, match.Index + match.Length));
                }
            }
            // handle special patterns
            //Regex r = new Regex($@"\b((?<oldTime>.*)\s+(?<meeting>meeting|appointment|conference|assembly)\s+to\s+(?<newTime>.*)(.|,)?)\b",
            //RegexOptions.IgnoreCase | RegexOptions.Singleline);
            matches = this.config.SpecialTimePattern.Matches(text);
            foreach (Match match in matches)
            {
                // check if oldTime string is a time format
                var oldTime = match.Groups["oldTime"];
                var timeRes = this.config.TimeExtractor.Extract(oldTime.ToString());
                if (timeRes.Count == 0) continue;
                // check if newTime string is a number format
                var newTime = match.Groups["newTime"];
                var numRes = this.config.NumExtractor.Extract(newTime.ToString());
                if (numRes.Count == 0) continue;

                ret.Add(new Token(newTime.Index, newTime.Index + newTime.Length));
            }
            return ret;
        }
    }
}
