using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Function;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest.Functions
{
    [TestFixture]
    public class ExpressionsInHeadersInMultipleQuestionListFunction2924 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            IForm form = Project.Current.AddForm();

            form.ItemList.Add(new NewFibItem());
            form.ItemList.Add(new NewFibItem());
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

        private static readonly string multipleQuestionListFunctionXmlWithExpressionInColumnHeader =
            "<itemization-table version=\"2\">" +
            "<number-of-columns>1</number-of-columns>" +
            "<column>" +
            "<header><field name=\"Form 1:Field1\"/>" + Environment.NewLine + "<string value=\" Address\"/>" +
            Environment.NewLine + "</header>" +
            "<contents><field name=\"Record:Form 1:Q2:a\" /></contents>" +
            "</column>" +
            "<conditions><form name=\"Form 1\" />" +
            "<conditions>" + Environment.NewLine +
            "<equals field=\"Form 1:Q1:a\">" + Environment.NewLine +
            "<string value=\"nonsense\"/>" + Environment.NewLine +
            "</equals>" + Environment.NewLine +
            "</conditions>" +
            "</conditions>" +
            "</itemization-table>";

        [Test]
        public void ItemizationTableFunctionWithExpressionInColumnHeaderProducesExpectedXml()
        {
            var converter = new XmlToFunctionConverter();
            IFunction function =
                converter.ConvertFrom(new XmlElement(multipleQuestionListFunctionXmlWithExpressionInColumnHeader));

            Assert.IsNotNull(function, "Failed to creation IFunction from Xml");
            Assert.AreEqual("itemization-table", function.Info.Id);

            Assert.AreEqual(3, function.Info.Parameters.Count);
            Assert.AreEqual("number-of-columns", function.Info.Parameters[0].Id);
            Assert.AreEqual("column", function.Info.Parameters[1].Id);
            Assert.AreEqual("conditions", function.Info.Parameters[2].Id);

            Assert.AreEqual(multipleQuestionListFunctionXmlWithExpressionInColumnHeader, function.ToXml(),
                            "Roundtrip of function XML failed");
        }
    }
}