import { IModel, ModelResult, IExtractor, IParser, ParseResult } from "recognizers-text";

export abstract class OptionsModel implements IModel {
    public abstract readonly modelTypeName: string;

    protected readonly extractor: IExtractor;
    protected readonly parser: IParser;

    constructor(parser: IParser, extractor: IExtractor) {
        this.extractor = extractor;
        this.parser = parser;
    }

    parse(source: string): ModelResult[] {
        let extractResults = this.extractor.extract(source);
        let parseResults = extractResults.map(r => this.parser.parse(r));

        return parseResults
            .map(o => o as ParseResult)
            .map(o => ({
                start: o.start,
                end: o.start + o.length - 1,
                resolution: this.getResolution(o),
                text: o.text,
                typeName: this.modelTypeName
            }));
    }

    protected abstract getResolution(data: any): any;
}

export class BooleanModel extends OptionsModel {
    public readonly modelTypeName = 'boolean';

    protected getResolution(sources: any): any {
        let results: any = {
            value: sources.value,
            score: sources.data.score
        }
        if (sources.data.otherMatches) {
            results.otherResults = sources.data.otherMatches.map(o => ({
                text: o.text,
                value: o.value,
                score: o.data.score
            }))
        }
        return results;
    }
}