import * as generator from "./lib/base-code-generator";

let resourcesPath = '../../Patterns/';
let outputPath = "./src/resources/";

let configs = [
    // COMMON NUMERIC
    {
        yaml: `${resourcesPath}Base-Numbers.yaml`,
        output: `${outputPath}baseNumbers.ts`,
        header: `export namespace BaseNumbers {`,
        footer: `}`
    },
    // ENGLISH NUMERIC WITH UNIT
    {
        yaml: `${resourcesPath}/English/English-NumbersWithUnit.yaml`,
        output: `${outputPath}englishNumericWithUnit.ts`,
        header:
        `import { BaseNumbers } from "./baseNumbers";
export namespace EnglishNumericWithUnit {`,
        footer: `}`
    },
    // SPANISH NUMERIC WITH UNIT
    {
        yaml: `${resourcesPath}/Spanish/Spanish-NumbersWithUnit.yaml`,
        output: `${outputPath}spanishNumericWithUnit.ts`,
        header:
        `import { BaseNumbers } from "./baseNumbers";
export namespace SpanishNumericWithUnit {`,
        footer: `}`
    },
    // PORTUGUESE NUMERIC WITH UNIT
    {
        yaml: `${resourcesPath}/Portuguese/Portuguese-NumbersWithUnit.yaml`,
        output: `${outputPath}portugueseNumericWithUnit.ts`,
        header:
        `import { BaseNumbers } from "./baseNumbers";
export namespace PortugueseNumericWithUnit {`,
        footer: `}`
    },
    // CHINESE NUMERIC WITH UNIT
    {
        yaml: `${resourcesPath}/Chinese/Chinese-NumbersWithUnit.yaml`,
        output: `${outputPath}chineseNumericWithUnit.ts`,
        header:
        `import { BaseNumbers } from "./baseNumbers";
export namespace ChineseNumericWithUnit {`,
        footer: `}`
    }
];

class Startup {
    public static main(): number {
        configs.forEach(config => {
            generator.generate(config.yaml, config.output, config.header, config.footer);
        });

        return 0;
    }
}

Startup.main();