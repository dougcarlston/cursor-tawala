// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Xml.XPath;
using NUnit.Framework;
using Tawala.Functions.Runtime;

namespace TawalaTest.FunctionTests
{
    [TestFixture]
    public class FunctionInfoTests : FunctionTestBase
    {
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            FixtureSetup();
        }

        [Test]
        public void FunctionInfosXmlPropertyIsValid()
        {
            Assert.AreEqual("function", functionRepository.Functions["record-count"].Xml.Name);
            Assert.AreEqual("display-component", functionRepository.Functions["popular-choice-correlation-table"].Xml.Name);
        }

        // This test isn't really important -- we don't and shouldn't care about the class names
        // I don't know why I made it but I'm leaving it in for now.

        [Test]
        public void LookupFunctionInfoByIdAndIndex()
        {
            XPathNodeIterator iterator = GetAllComponentTypesFromXmlSortedByName();

            int index = 0;

            while (iterator.MoveNext())
            {
                if (iterator.Current.GetAttribute("id", "").CompareTo("record-count") == 0)
                {
                    break;
                }
                index++;
            }
            Assert.AreEqual("record-count", functionRepository.Functions["record-count"].Id);
            Assert.IsTrue(functionRepository.Functions["record-count"].Type == functionRepository.Functions[index-1].Type);
        }

        [Test]
        public void LookupFunctionInfoByType()
        {
            IFunction instance = functionRepository.Functions["record-count"].CreateInstance();
            Assert.AreSame(instance.Info, functionRepository.Functions[instance.GetType()]);
        }

        [Test]
        public void XmlFunctionsIdsBecomeClassNamesWithVersionNum()
        {
            Assert.AreEqual("RecordCount_V3", functionRepository.Functions["record-count"].Type.Name);
            Assert.AreEqual("PopularChoiceCorrelationTable_V1", functionRepository.Functions["popular-choice-correlation-table"].Type.Name);
            Assert.AreEqual("PopularChoiceCount_V1", functionRepository.Functions["popular-choice-count"].Type.Name);
        }

        [Test]
        public void XmlFunctionsNamesBecomeFunctionInfoName()
        {
            Assert.AreEqual("FORM RECORD COUNT", functionRepository.Functions["record-count"].Name);
            Assert.AreEqual("RANKED MULTIQUESTION RESPONSE LIST", functionRepository.Functions["popular-choice-correlation-table"].Name);
            Assert.AreEqual("SUM", functionRepository.Functions["sum"].Name);
        }
    }
}