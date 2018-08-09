﻿using Microsoft.Recognizers.Text.Number.Chinese;
using Microsoft.Recognizers.Text.Number.English;
using Microsoft.Recognizers.Text.Number.French;
using Microsoft.Recognizers.Text.Number.German;
using Microsoft.Recognizers.Text.Number.Spanish;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Recognizers.Text.Number.Tests
{
    [TestClass]
    public class TestParserFactory
    {
        private ExtractResult getNumberToParse(string number, string data)
        {
            return new ExtractResult
            {
                Type = Constants.SYS_NUM,
                Data = data,
                Text = number
            };
        }

        [TestMethod]
        public void TestEnglishParser()
        {
            IParser parserNumber = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Number, new EnglishNumberParserConfiguration());
            IParser parserCardinal = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Cardinal, new EnglishNumberParserConfiguration());
            IParser parserPercentaje = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Percentage, new EnglishNumberParserConfiguration());

            Assert.IsTrue(parserNumber is BaseNumberParser);
            Assert.IsTrue(parserCardinal is BaseNumberParser);
            Assert.IsTrue(parserPercentaje is BasePercentageParser);
        }

        [TestMethod]
        public void TestSpanishParser()
        {
            IParser parserNumber = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Number, new SpanishNumberParserConfiguration());
            IParser parserCardinal = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Cardinal, new SpanishNumberParserConfiguration());
            IParser parserPercentaje = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Percentage, new SpanishNumberParserConfiguration());

            Assert.IsTrue(parserNumber is BaseNumberParser);
            Assert.IsTrue(parserCardinal is BaseNumberParser);
            Assert.IsTrue(parserPercentaje is BasePercentageParser);
        }

        [TestMethod]
        public void TestChineseParser()
        {
            IParser parserNumber = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Number, new ChineseNumberParserConfiguration());
            IParser parserCardinal = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Cardinal, new ChineseNumberParserConfiguration());
            IParser parserPercentaje = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Percentage, new ChineseNumberParserConfiguration());

            Assert.IsTrue(parserNumber is ChineseNumberParser);
            Assert.IsTrue(parserCardinal is ChineseNumberParser);
            Assert.IsTrue(parserPercentaje is ChineseNumberParser);
        }

        [TestMethod]
        public void TestFrenchParser()
        {
            IParser parseNumber = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Number, new FrenchNumberParserConfiguration());
            IParser parseCardinal = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Cardinal, new FrenchNumberParserConfiguration());
            IParser parsePercentage = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Percentage, new FrenchNumberParserConfiguration());

            Assert.IsTrue(parseNumber is BaseNumberParser);
            Assert.IsTrue(parseCardinal is BaseNumberParser);
            Assert.IsTrue(parsePercentage is BasePercentageParser);
        }

        [TestMethod]
        public void TestGermanParser()
        {
            IParser parseNumber = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Number, new GermanNumberParserConfiguration());
            IParser parseCardinal = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Cardinal, new GermanNumberParserConfiguration());
            IParser parsePercentage = AgnosticNumberParserFactory.GetParser(AgnosticNumberParserType.Percentage, new GermanNumberParserConfiguration());
 
            Assert.IsTrue(parseNumber is BaseNumberParser);
            Assert.IsTrue(parseCardinal is BaseNumberParser);
            Assert.IsTrue(parsePercentage is BasePercentageParser);
        }
}
}
