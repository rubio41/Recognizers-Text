import { IExtractor, IParser, RegExpUtility } from "recognizers-text";
import { BaseNumberExtractor, FrenchIntegerExtractor } from "recognizers-text-number";
import { IDatePeriodExtractorConfiguration, IDatePeriodParserConfiguration } from "../baseDatePeriod";
import { BaseDateExtractor, BaseDateParser } from "../baseDate";
import { BaseDurationExtractor, BaseDurationParser } from "../baseDuration";
import { FrenchDateExtractorConfiguration } from "./dateConfiguration";
import { FrenchDurationExtractorConfiguration } from "./durationConfiguration";
import { FrenchDateTime } from "../../resources/frenchDateTime";
import { ICommonDateTimeParserConfiguration } from "../parsers";
import { IDateTimeExtractor } from "../baseDateTime"

export class FrenchDatePeriodExtractorConfiguration implements IDatePeriodExtractorConfiguration {
    readonly simpleCasesRegexes: RegExp[];
    readonly tillRegex: RegExp;
    readonly followedUnit: RegExp;
    readonly numberCombinedWithUnit: RegExp;
    readonly pastRegex: RegExp;
    readonly futureRegex: RegExp;
    readonly weekOfRegex: RegExp;
    readonly monthOfRegex: RegExp;
    readonly dateUnitRegex: RegExp;
    readonly inConnectorRegex: RegExp;
    readonly rangeUnitRegex: RegExp;
    readonly datePointExtractor: IDateTimeExtractor;
    readonly integerExtractor: BaseNumberExtractor;
    readonly durationExtractor: IDateTimeExtractor;

    readonly fromRegex: RegExp;
    readonly connectorAndRegex: RegExp;
    readonly beforeRegex: RegExp;

    readonly weekDayOfMonthRegex: RegExp;

    constructor() {
        this.simpleCasesRegexes = [
            RegExpUtility.getSafeRegExp(FrenchDateTime.SimpleCasesRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.BetweenRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.OneWordPeriodRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.MonthWithYear),
            RegExpUtility.getSafeRegExp(FrenchDateTime.MonthNumWithYear),
            RegExpUtility.getSafeRegExp(FrenchDateTime.YearRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.WeekDayOfMonthRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.WeekOfYearRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.MonthFrontBetweenRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.MonthFrontSimpleCasesRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.QuarterRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.QuarterRegexYearFront),
            RegExpUtility.getSafeRegExp(FrenchDateTime.SeasonRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.PastSuffixRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.NextSuffixRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.ThisPrefixRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.LaterEarlyPeriodRegex),
            RegExpUtility.getSafeRegExp(FrenchDateTime.WeekWithWeekDayRangeRegex)
        ];
        this.tillRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.TillRegex);
        this.followedUnit = RegExpUtility.getSafeRegExp(FrenchDateTime.FollowedDateUnit);
        this.numberCombinedWithUnit = RegExpUtility.getSafeRegExp(FrenchDateTime.NumberCombinedWithDateUnit);
        this.pastRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.PastSuffixRegex);
        this.futureRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.NextSuffixRegex);
        this.weekOfRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.WeekOfRegex);
        this.monthOfRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.MonthOfRegex);
        this.dateUnitRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.DateUnitRegex);
        this.inConnectorRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.InConnectorRegex);
        this.rangeUnitRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.RangeUnitRegex);
        this.weekDayOfMonthRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.WeekDayOfMonthRegex);

        this.fromRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.FromRegex);
        this.connectorAndRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.ConnectorAndRegex);
        this.beforeRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.BeforeRegex2);

        this.datePointExtractor = new BaseDateExtractor(new FrenchDateExtractorConfiguration());
        this.integerExtractor = new FrenchIntegerExtractor();
        this.durationExtractor = new BaseDurationExtractor(new FrenchDurationExtractorConfiguration());
    }

    getFromTokenIndex(source: string): { matched: boolean; index: number; } {
        return RegExpUtility.getFirstMatchIndex(this.fromRegex, source);
    }

    getBetweenTokenIndex(source: string): { matched: boolean; index: number; } {
        return RegExpUtility.getFirstMatchIndex(this.beforeRegex, source);
    }

    hasConnectorToken(source: string): boolean {
        return RegExpUtility.getFirstMatchIndex(this.connectorAndRegex, source).matched;
    }
}

export class FrenchDatePeriodParserConfiguration implements IDatePeriodParserConfiguration {
    readonly dateExtractor: IDateTimeExtractor;
    readonly dateParser: BaseDateParser;
    readonly durationExtractor: IDateTimeExtractor;
    readonly durationParser: BaseDurationParser;
    readonly monthFrontBetweenRegex: RegExp;
    readonly betweenRegex: RegExp;
    readonly monthFrontSimpleCasesRegex: RegExp;
    readonly simpleCasesRegex: RegExp;
    readonly oneWordPeriodRegex: RegExp;
    readonly monthWithYear: RegExp;
    readonly monthNumWithYear: RegExp;
    readonly yearRegex: RegExp;
    readonly pastRegex: RegExp;
    readonly futureRegex: RegExp;
    readonly inConnectorRegex: RegExp;
    readonly weekOfMonthRegex: RegExp;
    readonly weekOfYearRegex: RegExp;
    readonly quarterRegex: RegExp;
    readonly quarterRegexYearFront: RegExp;
    readonly seasonRegex: RegExp;
    readonly weekOfRegex: RegExp;
    readonly monthOfRegex: RegExp;
    readonly whichWeekRegex: RegExp;
    readonly restOfDateRegex: RegExp;
    readonly tokenBeforeDate: string;
    readonly dayOfMonth: ReadonlyMap<string, number>;
    readonly monthOfYear: ReadonlyMap<string, number>;
    readonly cardinalMap: ReadonlyMap<string, number>;
    readonly seasonMap: ReadonlyMap<string, string>;
    readonly unitMap: ReadonlyMap<string, string>;

    readonly nextPrefixRegex: RegExp;
    readonly pastPrefixRegex: RegExp;
    readonly thisPrefixRegex: RegExp;
    readonly numberCombinedWithUnit: RegExp;
    readonly laterEarlyPeriodRegex: RegExp;
    readonly weekWithWeekDayRangeRegex: RegExp;

    readonly cardinalExtractor: IExtractor;
    readonly numberParser: IParser;

    constructor(config: ICommonDateTimeParserConfiguration) {
        this.tokenBeforeDate = FrenchDateTime.TokenBeforeDate;
        this.cardinalExtractor = config.cardinalExtractor;
        this.numberParser = config.numberParser;
        this.durationExtractor = config.durationExtractor;
        this.dateExtractor = config.dateExtractor;
        this.durationParser = config.durationParser;
        this.dateParser = config.dateParser;

        this.monthFrontBetweenRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.MonthFrontBetweenRegex);
        this.betweenRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.BetweenRegex);
        this.monthFrontSimpleCasesRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.MonthFrontSimpleCasesRegex);
        this.simpleCasesRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.SimpleCasesRegex);
        this.oneWordPeriodRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.OneWordPeriodRegex);
        this.monthWithYear = RegExpUtility.getSafeRegExp(FrenchDateTime.MonthWithYear);
        this.monthNumWithYear = RegExpUtility.getSafeRegExp(FrenchDateTime.MonthNumWithYear);
        this.yearRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.YearRegex);
        this.pastRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.PastSuffixRegex);
        this.futureRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.NextSuffixRegex);
        this.numberCombinedWithUnit = RegExpUtility.getSafeRegExp(FrenchDateTime.NumberCombinedWithDurationUnit);
        this.weekOfMonthRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.WeekOfMonthRegex);
        this.weekOfYearRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.WeekOfYearRegex);
        this.quarterRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.QuarterRegex);
        this.quarterRegexYearFront = RegExpUtility.getSafeRegExp(FrenchDateTime.QuarterRegexYearFront);
        this.seasonRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.SeasonRegex);
        this.whichWeekRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.WhichWeekRegex);
        this.weekOfRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.WeekOfRegex);
        this.monthOfRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.MonthOfRegex);
        this.restOfDateRegex = RegExpUtility.getSafeRegExp(FrenchDateTime.RestOfDateRegex);

        this.nextPrefixRegex = RegExpUtility.getSafeRegExp("(prochain|prochaine)\b");
        this.pastPrefixRegex = RegExpUtility.getSafeRegExp("(dernier)\b");
        this.thisPrefixRegex = RegExpUtility.getSafeRegExp("(ce|cette)\b");

        this.inConnectorRegex = config.utilityConfiguration.inConnectorRegex;
        this.unitMap = config.unitMap;
        this.cardinalMap = config.cardinalMap;
        this.dayOfMonth = config.dayOfMonth;
        this.monthOfYear = config.monthOfYear;
        this.seasonMap = config.seasonMap;
    }

    getSwiftDayOrMonth(source: string): number {
        let trimedText = source.trim().toLowerCase();
        let swift = 0;

        if (trimedText.endsWith("prochain") || trimedText.endsWith("prochaine")) {
            swift = 1;
        }

        if (trimedText.endsWith("dernière") || 
            trimedText.endsWith("dernières") ||
            trimedText.endsWith("derniere") || 
            trimedText.endsWith("dernieres")) {
            swift = -1;
        }

        return swift;
    }

    getSwiftYear(source: string): number {
        let trimedText = source.trim().toLowerCase();
        let swift = -10;
        if (trimedText.endsWith("prochain") || trimedText.endsWith("prochaine")){
            swift = 1;
        }

        if (trimedText.endsWith("dernières") || 
            trimedText.endsWith("dernière") ||
            trimedText.endsWith("dernieres") || 
            trimedText.endsWith("derniere") || 
            trimedText.endsWith("dernier")) {
            swift = -1;
        } else if (trimedText.startsWith("cette"))
        {
            swift = 0;
        }

        return swift;
    }

    isFuture(source: string): boolean {
        let trimedText = source.trim().toLowerCase();
        return (trimedText.startsWith("cette") ||
            trimedText.endsWith("prochaine") || 
            trimedText.endsWith("prochain"));
    }

    isYearToDate(source: string): boolean {
        let trimedText = source.trim().toLowerCase();
        return (trimedText === "année à ce jour" || 
            trimedText === "an à ce jour");
    }

    isMonthToDate(source: string): boolean {
        let trimedText = source.trim().toLowerCase();
        return trimedText === "mois à ce jour";
    }

    isWeekOnly(source: string): boolean {
        let trimedText = source.trim().toLowerCase();
        return (trimedText.endsWith("semaine") && 
            !trimedText.endsWith("fin de semaine"));
    }

    isWeekend(source: string): boolean {
        let trimedText = source.trim().toLowerCase();
        return (trimedText.endsWith("fin de semaine") || 
            trimedText.endsWith("le weekend"));
    }

    isMonthOnly(source: string): boolean {
        let trimedText = source.trim().toLowerCase();
        return trimedText.endsWith("mois");
    }

    isYearOnly(source: string): boolean {
        let trimedText = source.trim().toLowerCase();
        return (trimedText.endsWith("années") || 
            trimedText.endsWith("ans") || 
            (trimedText.endsWith("l'annees") || 
            trimedText.endsWith("l'annee"))
        );
    }

    isLastCardinal(source: string): boolean {
        let trimedText = source.trim().toLowerCase();
        return (trimedText === "dernières" ||
            trimedText === "dernière" ||
            trimedText === "dernieres" || 
            trimedText === "derniere"||
            trimedText === "dernier");
    }
}