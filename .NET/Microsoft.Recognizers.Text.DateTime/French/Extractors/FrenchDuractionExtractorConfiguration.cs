﻿using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions.French;
using Microsoft.Recognizers.Text.Number;
using System.Collections.Immutable;

namespace Microsoft.Recognizers.Text.DateTime.French
{
    public class FrenchDurationExtractorConfiguration : BaseOptionsConfiguration, IDurationExtractorConfiguration
    {
        public static readonly Regex DurationUnitRegex =
            new Regex(
                DateTimeDefinitions.DurationUnitRegex,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex SuffixAndRegex = 
            new Regex(
                DateTimeDefinitions.SuffixAndRegex,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex DurationFollowedUnit = 
            new Regex(
                DateTimeDefinitions.DurationFollowedUnit,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex NumberCombinedWithDurationUnit = 
            new Regex(
                DateTimeDefinitions.NumberCombinedWithDurationUnit,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex AnUnitRegex = 
            new Regex(
                DateTimeDefinitions.AnUnitRegex,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex AllRegex = 
            new Regex(
                DateTimeDefinitions.AllRegex,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex HalfRegex = 
            new Regex(
                DateTimeDefinitions.HalfRegex,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex ConjunctionRegex = 
            new Regex(
                DateTimeDefinitions.ConjunctionRegex, 
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex InExactNumberRegex = 
            new Regex(
                DateTimeDefinitions.InExactNumberRegex, //quelques = "a few, some," etc 
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex InExactNumberUnitRegex = 
            new Regex(
                DateTimeDefinitions.InExactNumberUnitRegex,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex RelativeDurationUnitRegex =
            new Regex(DateTimeDefinitions.RelativeDurationUnitRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static readonly Regex DurationConnectorRegex =
            new Regex(DateTimeDefinitions.DurationConnectorRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public FrenchDurationExtractorConfiguration() : base(DateTimeOptions.None)
        {
            CardinalExtractor = Number.French.CardinalExtractor.GetInstance();
            UnitMap = DateTimeDefinitions.UnitMap.ToImmutableDictionary();
            UnitValueMap = DateTimeDefinitions.UnitValueMap.ToImmutableDictionary();
        }

        public IExtractor CardinalExtractor { get; }

        public IImmutableDictionary<string, string> UnitMap { get; }

        public IImmutableDictionary<string, long> UnitValueMap { get; }

        Regex IDurationExtractorConfiguration.FollowedUnit => DurationFollowedUnit; 

        Regex IDurationExtractorConfiguration.NumberCombinedWithUnit => NumberCombinedWithDurationUnit;

        Regex IDurationExtractorConfiguration.AnUnitRegex => AnUnitRegex;

        Regex IDurationExtractorConfiguration.AllRegex => AllRegex;

        Regex IDurationExtractorConfiguration.HalfRegex => HalfRegex;

        Regex IDurationExtractorConfiguration.SuffixAndRegex => SuffixAndRegex;

        Regex IDurationExtractorConfiguration.ConjunctionRegex => ConjunctionRegex;

        Regex IDurationExtractorConfiguration.InExactNumberRegex => InExactNumberRegex;

        Regex IDurationExtractorConfiguration.InExactNumberUnitRegex => InExactNumberUnitRegex;

        Regex IDurationExtractorConfiguration.RelativeDurationUnitRegex => RelativeDurationUnitRegex;

        Regex IDurationExtractorConfiguration.DurationUnitRegex => DurationUnitRegex;

        Regex IDurationExtractorConfiguration.DurationConnectorRegex => DurationConnectorRegex;
    }
}
