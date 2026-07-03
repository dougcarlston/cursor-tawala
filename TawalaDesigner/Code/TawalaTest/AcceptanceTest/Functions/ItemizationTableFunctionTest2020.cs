// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Function;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Functions
{
    [TestFixture]
    public class ItemizationTableFunctionTest2020 : FunctionTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();

            fibItem1 = new FibItem();
            blank1 = fibItem1.BlankList[0];
            fibItem2 = new FibItem();
            blank2 = fibItem2.BlankList[0];

            form.ItemList.Add(fibItem1);
            form.ItemList.Add(fibItem2);
        }

        #endregion

        private IForm form;
        private FibItem fibItem1;
        private FibItem fibItem2;
        private Blank blank1;
        private Blank blank2;

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

        private IFunction createItemizationTableFunction()
        {
            IFunction function = functions["itemization-table"].CreateInstance();

            IParameterInfo compositeCollection = function.Info.Parameters["column"];
            var collection = Activator.CreateInstance(compositeCollection.PropertyType) as ICompositeParameterCollection;

            ICompositeParameter composite1 = collection.CreateItem();

            composite1["header"] = new FunctionCompoundExpression(new XmlElement("<container><string value=\"Name\"/></container>"));
            composite1["contents"] = new FunctionContentsField(blank1);

            ICompositeParameter composite2 = collection.CreateItem();

            composite2["header"] = new FunctionCompoundExpression(new XmlElement("<container><string value=\"Dish\"/></container>"));
            composite2["contents"] = new FunctionContentsField(blank2);

            collection.Add(composite1);
            collection.Add(composite2);

            Assert.AreEqual(2, collection.Count);

            compositeCollection.SetValue(function, collection);

            var functionConditions = new FunctionFilterConditions();
            functionConditions.Forms = new FunctionFormCollection(form);
            function.SetValue("conditions", functionConditions);

            Project.FunctionMapById.AddUnique(function);

            return function;
        }

        [Test]
        public void FunctionProducesExpectedDisplayString()
        {
            IFunction instance = createItemizationTableFunction();

            string expectedString = "<<MULTIPLE QUESTION LIST(false, false, ...)>>";

            Assert.AreEqual(expectedString, instance.ToDisplayString());
        }

        [Test]
        public void ItemizationTableFunctionProducesItemizationTableXml()
        {
            IFunction function = createItemizationTableFunction();

			string expectedString = "<itemization-table version=\"2\">" + "<show-print-control>false</show-print-control><show-export-control>false</show-export-control>" + "<column>" +
                                    "<header><string value=\"Name\"/>" + Environment.NewLine + "</header>" + "<contents>" +
                                    "<field name=\"Form 1:Q1:a\" />" + "</contents>" + "</column>" + "<column>" +
                                    "<header><string value=\"Dish\"/>" + Environment.NewLine + "</header>" + "<contents>" +
                                    "<field name=\"Form 1:Q2:a\" />" + "</contents>" + "</column>" +
                                    "<conditions><form name=\"Form 1\" /></conditions>" + "</itemization-table>";

            Assert.AreEqual(expectedString, function.ToXml());
        }

        [Test]
        public void ItemizationTableXmlProducesItemizationTableXml()
        {
			string xmlString = "<itemization-table version=\"2\">" + "<show-print-control>false</show-print-control><show-export-control>false</show-export-control>" + "<column>" +
                               "<header><string value=\"Name\"/>" + Environment.NewLine + "</header>" + "<contents>" +
                               "<field name=\"Form 1:Q1:a\" />" + "</contents>" + "</column>" + "<column>" +
                               "<header><string value=\"Dish\"/>" + Environment.NewLine + "</header>" + "<contents>" +
                               "<field name=\"Form 1:Q2:a\" />" + "</contents>" + "</column>" + "<conditions>" + "<form name=\"Form 1\" />" +
                               "<conditions>" + NEWLINE + "<equals field=\"Record:Form 1:Q1:a\">" + NEWLINE + "<string value=\"foo\"/>" +
                               NEWLINE + "</equals>" + NEWLINE + "</conditions>" + "</conditions>" + "</itemization-table>";

            DocumentFunctionField functionField = new DocumentPersistedFunctionField(new XmlElement(xmlString));

            IFunction function = Project.FunctionMapById[functionField.FunctionInstanceId];

            Assert.IsNotNull(function);
            Assert.AreEqual("itemization-table", function.Info.Id);

            Assert.AreEqual(xmlString, function.ToXml());
        }

        [Test]
        public void ItemizationTableXmlWithEmptyFieldsProducesItemizationTableXmlWithEmptyFields()
        {
			string xmlString = "<itemization-table version=\"2\">" + "<show-print-control>false</show-print-control><show-export-control>false</show-export-control>" + "<column>" +
                               "<contents>" + "<field name=\"Record:Form 1:Q1:a\" />" + "</contents>" + "</column>" + "<column>" +
                               "<header><string value=\"Dish\"/>" + Environment.NewLine + "</header>" + "</column>" + "<conditions>" +
                               "<form name=\"Form 1\" />" + "</conditions>" + "</itemization-table>";

            DocumentFunctionField functionField = new DocumentPersistedFunctionField(new XmlElement(xmlString));

            IFunction function = Project.FunctionMapById[functionField.FunctionInstanceId];

            Assert.AreEqual(xmlString, function.ToXml());
        }

        [Test]
        public void ItemizationTableXmlWithRecordFieldsProducesItemizationTableXmlWithRecordFields()
        {
			string xmlString = "<itemization-table version=\"2\">" + "<show-print-control>false</show-print-control><show-export-control>false</show-export-control>" + "<column>" +
                               "<header><string value=\"Name\"/>" + Environment.NewLine + "</header>" + "<contents>" +
                               "<field name=\"Record:Form 1:Q1:a\" />" + "</contents>" + "</column>" + "<column>" +
                               "<header><string value=\"Dish\"/>" + Environment.NewLine + "</header>" + "<contents>" +
                               "<field name=\"Record:Form 1:Q2:a\" />" + "</contents>" + "</column>" + "<conditions>" +
                               "<form name=\"Form 1\" />" + "</conditions>" + "</itemization-table>";

            DocumentFunctionField functionField = new DocumentPersistedFunctionField(new XmlElement(xmlString));

            IFunction function = Project.FunctionMapById[functionField.FunctionInstanceId];

            Assert.AreEqual(xmlString, function.ToXml());
        }
    }
}