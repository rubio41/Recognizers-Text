﻿using Microsoft.Recognizers.Text.DateTime.Utilities;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Microsoft.Recognizers.Text.DateTime.French
{
    public class FrenchTimePeriodParserConfiguration : ITimePeriodParserConfiguration
    {
        public IExtractor TimeExtractor { get; }

        public IDateTimeParser TimeParser { get; }

        public Regex PureNumberFromToRegex { get; }

        public Regex PureNumberBetweenAndRegex { get; }

        public Regex TimeOfDayRegex { get; }

        public IImmutableDictionary<string, int> Numbers { get; }

        public IDateTimeUtilityConfiguration UtilityConfiguration { get; }

        public FrenchTimePeriodParserConfiguration(ICommonDateTimeParserConfiguration config)
        {
            TimeExtractor = config.TimeExtractor;
            TimeParser = config.TimeParser;
            PureNumberFromToRegex = FrenchTimePeriodExtractorConfiguration.PureNumFromTo; 
            PureNumberBetweenAndRegex = FrenchTimePeriodExtractorConfiguration.PureNumBetweenAnd; 
            TimeOfDayRegex = FrenchTimePeriodExtractorConfiguration.TimeOfDayRegex; 
            Numbers = config.Numbers;
            UtilityConfiguration = config.UtilityConfiguration;
        }

        public bool GetMatchedTimexRange(string text, out string timex, out int beginHour, out int endHour, out int endMin)
        {
            var trimedText = text.Trim().ToLowerInvariant();
            if (trimedText.EndsWith("s"))
            {
                trimedText = trimedText.Substring(0, trimedText.Length - 1);
            }
            beginHour = 0;
            endHour = 0;
            endMin = 0;
            if (trimedText.EndsWith("matinee") || trimedText.EndsWith("matin") || trimedText.EndsWith("matinée"))
            {
                timex = "TMO";
                beginHour = 8;
                endHour = 12;
            }
            else if (trimedText.EndsWith("apres-midi")||trimedText.EndsWith("apres midi") 
                || trimedText.EndsWith("après midi") || trimedText.EndsWith("après-midi"))
            {
                timex = "TAF";
                beginHour = 12;
                endHour = 16;
            } 
            else if (trimedText.EndsWith("soir") || trimedText.EndsWith("soiree") || trimedText.EndsWith("soirée"))
            {
                timex = "TEV";
                beginHour = 16;
                endHour = 20;
            }
            else if (trimedText.Equals("jour") || trimedText.EndsWith("journee") || trimedText.EndsWith("journée"))
            {
                timex = "TDT";
                beginHour = 8;
                endHour = 18;
            }
            else if (trimedText.EndsWith("nuit"))
            {
                timex = "TNI";
                beginHour = 20;
                endHour = 23;
                endMin = 59;
            }
            else
            {
                timex = null;
                return false;
            }

            return true;
        }

    }
}
