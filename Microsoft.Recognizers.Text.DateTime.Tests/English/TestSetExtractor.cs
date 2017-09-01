﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Recognizers.Text.DateTime.English.Tests
{
    [TestClass]
    public class TestSetExtractor
    {
        private readonly BaseSetExtractor extractor = new BaseSetExtractor(new EnglishSetExtractorConfiguration());

        public void BasicTest(string text, int start, int length)
        {
            var results = extractor.Extract(text);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(start, results[0].Start);
            Assert.AreEqual(length, results[0].Length);
            Assert.AreEqual(Constants.SYS_DATETIME_SET, results[0].Type);
        }

        [TestMethod]
        public void TestSetExtract()
        {
            BasicTest("I'll leave weekly", 11, 6);
            BasicTest("I'll leave daily", 11, 5);
            BasicTest("I'll leave every day", 11, 9);
            BasicTest("I'll leave each month", 11, 10);
            BasicTest("I'll leave annually", 11, 8);
            BasicTest("I'll leave annual", 11, 6);

            BasicTest("I'll leave each two days", 11, 13);
            BasicTest("I'll leave every three week", 11, 16);

            BasicTest("I'll leave 3pm every day", 11, 13);
            BasicTest("I'll leave 3pm each day", 11, 12);

            BasicTest("I'll leave each 4/15", 11, 9);
            BasicTest("I'll leave every monday", 11, 12);
            BasicTest("I'll leave each monday 4pm", 11, 15);

            BasicTest("I'll leave every morning", 11, 13);
        }

        [TestMethod]
        public void TestSetExtractMergeTimePeriod_Time()
        {
            BasicTest("I'll leave every morning at 9am", 11, 20);
            BasicTest("I'll leave every afternoon at 4pm", 11, 22);
            BasicTest("I'll leave every night at 9pm", 11, 18);
            BasicTest("I'll leave every night at 9", 11, 16);
            BasicTest("I'll leave mornings at 9am", 11, 15);
            BasicTest("I'll leave on mornings at 9", 11, 16);
        }

        [TestMethod]
        public void TestSetExtractMergeDate_Time()
        {
            BasicTest("I'll leave at 9am every Sunday", 14, 16);
            BasicTest("I'll leave at 9am on Mondays", 14, 14);
            BasicTest("I'll leave at 9am Mondays", 14, 11);
        }

        [TestMethod]
        public void TestSetExtractDate()
        {
            BasicTest("I'll leave on Mondays", 11, 10);
            BasicTest("I'll leave on Sundays", 11, 10);
            BasicTest("I'll leave Sundays", 11, 7);
        }
    }
}