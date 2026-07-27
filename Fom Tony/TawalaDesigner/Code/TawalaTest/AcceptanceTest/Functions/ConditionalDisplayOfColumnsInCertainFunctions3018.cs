// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Functions
{
    [TestFixture]
    public class ConditionalDisplayOfColumnsInCertainFunctions3018 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form1 = Project.Current.AddForm();
            form1FibItem1 = new FibItem();
            form1.ItemList.Add(form1FibItem1);
            form1FibItem2 = new FibItem();
            form1.ItemList.Add(form1FibItem2);

            form2 = Project.Current.AddForm();
            form2McqItem = new McqItem();
            form2.ItemList.Add(form2McqItem);

            form3 = Project.Current.AddForm();
            form3.ItemList.Add(new HiddenField());

            form4 = Project.Current.AddForm();
        }

        #endregion

        private IForm form1;
        private IForm form2;
        private IForm form3;
        private IForm form4;

        private FibItem form1FibItem1;
        private FibItem form1FibItem2;

        private McqItem form2McqItem;

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

        private static readonly string multipleQuestionListFunctionXmlWithColumnDisplayConditions = "<itemization-table version=\"2\">" +
																									"<show-print-control>false</show-print-control><show-export-control>false</show-export-control>" +
                                                                                                    "<column>" +
                                                                                                    "<header><string value=\"Column Heading\"/>" +
                                                                                                    Environment.NewLine + "</header>" +
                                                                                                    "<contents><field name=\"Record:Form 1:Q2:a\" /></contents>" +
                                                                                                    "<display-conditions>" +
                                                                                                    "<equals field=\"Form 1:Q1:a\">" +
                                                                                                    Environment.NewLine +
                                                                                                    "<string value=\"nonsense\"/>" +
                                                                                                    Environment.NewLine + "</equals>" +
                                                                                                    Environment.NewLine +
                                                                                                    "</display-conditions>" + "</column>" +
                                                                                                    "<conditions></conditions>" +
                                                                                                    "</itemization-table>";

        private static readonly string categorizerFunctionXmlWithColumnDisplayConditions = "<categorizer version=\"2\">" +
                                                                                           "<category-names>Record:Form 1:Q1:a</category-names>" +
                                                                                           "<category-ids>Record:Form 1:Q1:a</category-ids>" +
                                                                                           "<category-storage-field>Record:Form 3:Field1</category-storage-field>" +
                                                                                           "<navigate-to>Form 4</navigate-to>" + "<column>" +
                                                                                           "<header><string value=\"Column Heading\"/>" +
                                                                                           Environment.NewLine + "</header>" +
                                                                                           "<contents><field name=\"Record:Form 1:Q2:a\" /></contents>" +
                                                                                           "<display-conditions>" +
                                                                                           "<equals field=\"Form 1:Q1:a\">" +
                                                                                           Environment.NewLine +
                                                                                           "<string value=\"nonsense\"/>" +
                                                                                           Environment.NewLine + "</equals>" +
                                                                                           Environment.NewLine + "</display-conditions>" +
                                                                                           "</column>" + "<conditions></conditions>" +
                                                                                           "</categorizer>";

        private IFunction createItemizationTableFunctionWithColumnDisplayConditions()
        {
            IFunction function = FunctionLoader.Repository.Functions["itemization-table"].CreateInstance();

            var collection = function["column"] as ICompositeParameterCollection;

            ICompositeParameter composite = collection.CreateItem();
            composite["header"] = new FunctionCompoundExpression(new XmlElement("<container><string value=\"Heading\"/></container>"));
            composite["contents"] = new FunctionContentsField(form1FibItem1.BlankList[0]);
            composite["display-conditions"] =
                new FunctionParameterConditions(new Conditions(form2McqItem, MCOneOperator.List[MCOneOperator.Ops.mcEquals], new Choice("a")));

            collection.Add(composite);

            Assert.AreEqual(1, collection.Count);

            return function;
        }

        [Test]
        public void CategorizerFunctionWithColumnDisplayConditionsProducesExpectedXml()
        {
            var converter = new XmlToFunctionConverter();
            IFunction function = converter.ConvertFrom(new XmlElement(categorizerFunctionXmlWithColumnDisplayConditions));

            Assert.IsNotNull(function, "Failed to creation IFunction from Xml");

            IParameterInfo columnInfo = function.Info.Parameters[4];
            Assert.AreEqual(3, columnInfo.Parameters.Count);
            Assert.AreEqual("display-conditions", columnInfo.Parameters[2].Id);

            Assert.AreEqual(categorizerFunctionXmlWithColumnDisplayConditions, function.ToXml(), "Roundtrip of function XML failed");
        }

        [Test]
        public void FormReferencedInColumnDisplayConditionsIsNotReferencedInFunctionConditions()
        {
            Type formsHelperType = getReferencedFormsHelperType();
            Assert.IsNotNull(formsHelperType);

            IFunction function = createItemizationTableFunctionWithColumnDisplayConditions();
            Assert.IsNotNull(function);

            var functionConditions = function["conditions"] as FunctionFilterConditions;

            Assert.AreEqual(0, functionConditions.Forms.Count);
        }

        [Test]
        public void ItemizationTableFunctionWithColumnDisplayConditionsProducesExpectedXml()
        {
            var converter = new XmlToFunctionConverter();
            IFunction function = converter.ConvertFrom(new XmlElement(multipleQuestionListFunctionXmlWithColumnDisplayConditions));

            Assert.IsNotNull(function, "Failed to creation IFunction from Xml");

            IParameterInfo columnInfo = function.Info.Parameters[2];
            Assert.AreEqual(3, columnInfo.Parameters.Count);
            Assert.AreEqual("display-conditions", columnInfo.Parameters[2].Id);

            Assert.AreEqual(multipleQuestionListFunctionXmlWithColumnDisplayConditions, function.ToXml(), "Roundtrip of function XML failed");
        }
    }
}