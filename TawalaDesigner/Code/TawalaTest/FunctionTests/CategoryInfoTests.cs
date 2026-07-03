// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;

namespace TawalaTest.FunctionTests
{
    [TestFixture]
    public class CategoryInfoTests : FunctionTestBase
    {
        private ICategoryInfoCollection categories;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            FixtureSetup();
            categories = functionRepository.Categories;
            expectedCategories = categoryStrings();
        }

        private string[] expectedCategories;

        [Test]
        public void AssociatedListOfFunctionInfosAllHaveCorrectCount()
        {
            Assert.AreEqual(7, categories["Tables"].Functions.Count);
            Assert.AreEqual(1, categories["Math Functions"].Functions.Count);
            Assert.AreEqual(3, categories["Database Functions"].Functions.Count);
            Assert.AreEqual(1, categories["Payments"].Functions.Count);
            Assert.AreEqual(16, categories["All"].Functions.Count);
        }

        [Test]
        public void AssociatedListOfFunctionInfosIsSorted()
        {
            ICategoryInfo info = categories["Tables"];

            var expected = new[]
                           {
                               "CATEGORIZER",
                               "MULTIPLE QUESTION LIST",
                               "QUESTION CORRELATION TABLE",
                               "RANKED MULTIQUESTION RESPONSE LIST",
                               "RESPONSE BAR GRAPH",
                               "RESPONSE TOTALS",
                               "SINGLE QUESTION LIST"
                           };

            for (int i = 0; i < expected.Length; ++i)
            {
                Assert.AreEqual(expected[i], info.Functions[i].Name);
            }
        }

        [Test]
        public void FunctionAssemblyHasCategoryList()
        {
            Assert.AreEqual(categoryCount(), categories.Count); // no longer includes synthetic "All" - SB
            for (int i = 0; i < expectedCategories.Length; ++i)
            {
                Assert.AreEqual(expectedCategories[i], functionRepository.Categories[i].Name);
            }
        }

        [Test]
        public void FunctionAssemblyHasFunctionInfoByCategory()
        {
            ICategoryInfo categoryInfo = functionRepository.Categories["Tables"];
            IFunctionInfoCollection functionInfos = categoryInfo.Functions;

            Assert.AreEqual(7, functionInfos.Count);
            Assert.IsTrue(functionInfos.Contains("itemization-table"));
            Assert.IsTrue(functionInfos.Contains("choice-tally-table"));
            Assert.IsTrue(functionInfos.Contains("response-totals-table"));
            Assert.IsTrue(functionInfos.Contains("popular-choice-correlation-table"));
            Assert.IsTrue(functionInfos.Contains("simple-list"));
            Assert.IsTrue(functionInfos.Contains("question-correlation-table"));
            Assert.IsTrue(functionInfos.Contains("categorizer"));
        }

        [Test]
        public void XmlPropertyIsValid()
        {
            string xml = categories[1].Xml.OuterXml;
            Assert.IsTrue(xml.StartsWith("<category"));
            Assert.IsTrue(xml.EndsWith("</category>"));
        }
    }
}