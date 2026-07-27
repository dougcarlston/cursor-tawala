// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Functions
{
    [TestFixture]
    public class ExpressionsInHeadersInMultipleQuestionListFunction2924 : FunctionTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            IForm form = Project.Current.AddForm();

            form.ItemList.Add(new FibItem());
            form.ItemList.Add(new FibItem());
        }

        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            functionSetup();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            functionTearDown();
        }

        private static readonly string multipleQuestionListFunctionXmlWithExpressionInColumnHeader = "<itemization-table version=\"2\">" +
																									 "<show-print-control>false</show-print-control><show-export-control>false</show-export-control>" +
                                                                                                     "<column>" +
                                                                                                     "<header><field name=\"Form 1:Field1\"/>" +
                                                                                                     Environment.NewLine +
                                                                                                     "<string value=\" Address\"/>" +
                                                                                                     Environment.NewLine + "</header>" +
                                                                                                     "<contents><field name=\"Record:Form 1:Q2:a\" /></contents>" +
                                                                                                     "</column>" +
                                                                                                     "<conditions><form name=\"Form 1\" />" +
                                                                                                     "<conditions>" + Environment.NewLine +
                                                                                                     "<equals field=\"Form 1:Q1:a\">" +
                                                                                                     Environment.NewLine +
                                                                                                     "<string value=\"nonsense\"/>" +
                                                                                                     Environment.NewLine + "</equals>" +
                                                                                                     Environment.NewLine + "</conditions>" +
                                                                                                     "</conditions>" +
                                                                                                     "</itemization-table>";

        [Test]
        public void ItemizationTableFunctionWithExpressionInColumnHeaderProducesExpectedXml()
        {
            var converter = new XmlToFunctionConverter();
            IFunction function = converter.ConvertFrom(new XmlElement(multipleQuestionListFunctionXmlWithExpressionInColumnHeader));

            Assert.IsNotNull(function, "Failed to creation IFunction from Xml");
            Assert.AreEqual("itemization-table", function.Info.Id);

            Assert.AreEqual(4, function.Info.Parameters.Count);
			Assert.AreEqual("show-print-control", function.Info.Parameters[0].Id);
			Assert.AreEqual("show-export-control", function.Info.Parameters[1].Id);
			Assert.AreEqual("column", function.Info.Parameters[2].Id);
			Assert.AreEqual("conditions", function.Info.Parameters[3].Id);

            Assert.AreEqual(multipleQuestionListFunctionXmlWithExpressionInColumnHeader, function.ToXml(),
                            "Roundtrip of function XML failed");
        }
    }
}