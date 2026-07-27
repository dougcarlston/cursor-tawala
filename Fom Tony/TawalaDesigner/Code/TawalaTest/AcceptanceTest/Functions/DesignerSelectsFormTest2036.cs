// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.ProjectUI;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Functions
{
    [TestFixture]
    public class DesignerSelectsFormTest2036 : FunctionTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form1 = Project.Current.AddForm();
            form2 = Project.Current.AddForm();

            fibItem1 = new FibItem();
            blank1 = fibItem1.BlankList[0];
            fibItem2 = new FibItem();
            blank2 = fibItem2.BlankList[0];

            form1.ItemList.Add(fibItem1);
            form1.ItemList.Add(fibItem2);

            fibItem3 = new FibItem();
            blank3 = fibItem3.BlankList[0];

            form2.ItemList.Add(fibItem3);

            testPalette = new FieldsPalette();
            testPalette.Show();
            testPalette.RefreshFieldList();
        }

        #endregion

        private IForm form1;
        private IForm form2;
        private FibItem fibItem1;
        private FibItem fibItem2;
        private FibItem fibItem3;
        private Blank blank1;
        private Blank blank2;
        private Blank blank3;
        private FieldsPalette testPalette;

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

        [Test]
        public void ChangingFieldsPaletteFunctionConditionsFormsUpdatesRecordNode()
        {
            var forms = new FunctionFormCollection();
            forms.AddUnique(form1);
            testPalette.ConditionsForms = forms;

            TreeNode recordNode = testPalette.FieldsTreeView.Nodes[3];
            Assert.AreEqual("Record", recordNode.Text);

            Assert.AreEqual(2, recordNode.Nodes.Count);
            Assert.AreEqual("Form 1:Q1:a", recordNode.Nodes[0].Text);
            Assert.AreEqual("Form 1:Q2:a", recordNode.Nodes[1].Text);

            forms = new FunctionFormCollection();
            forms.AddUnique(form2);
            testPalette.ConditionsForms = forms;

            Assert.AreEqual(1, recordNode.Nodes.Count);
            Assert.AreEqual("Form 2:Q1:a", recordNode.Nodes[0].Text);
        }

        [Test]
        public void ClearingFieldsPaletteFunctionConditionsFormsRemovesRecordNode()
        {
            var forms = new FunctionFormCollection();
            forms.AddUnique(form1);
            testPalette.ConditionsForms = forms;

            Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);
            TreeNode recordNode = testPalette.FieldsTreeView.Nodes[3];
            Assert.AreEqual("Record", recordNode.Text);

            Assert.AreEqual(2, recordNode.Nodes.Count);
            Assert.AreEqual("Form 1:Q1:a", recordNode.Nodes[0].Text);
            Assert.AreEqual("Form 1:Q2:a", recordNode.Nodes[1].Text);

            forms = new FunctionFormCollection();
            testPalette.ConditionsForms = forms;

            Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);
        }

        [Test]
        public void ItemizationTableWithMultipleFormsProducesConditionsXmlWithMultipleForms()
        {
            IFunction function = functions["itemization-table"].CreateInstance();
            var collection = Activator.CreateInstance(function.Info.Parameters["column"].PropertyType) as ICompositeParameterCollection;
            function.SetValue("column", collection);
            collection.Add(collection.CreateItem());

            var functionConditions = new FunctionFilterConditions();
            var forms = new FunctionFormCollection();
            forms.AddUnique(form1);
            forms.AddUnique(form2);
            functionConditions.Forms = forms;
            function.SetValue("conditions", functionConditions);

			string expectedXml = "<itemization-table version=\"2\">" + "<show-print-control>false</show-print-control><show-export-control>false</show-export-control>" +
                                 "<column></column>" + "<conditions>" + "<form name=\"Form 1\" />" + "<form name=\"Form 2\" />" +
                                 "</conditions>" + "</itemization-table>";

            Assert.AreEqual(expectedXml, function.ToXml());
        }

        [Test]
        public void ItemizationTableWithSingleFormProducesConditionsXmlWithSingleForm()
        {
            IFunction function = functions["itemization-table"].CreateInstance();
            var collection = Activator.CreateInstance(function.Info.Parameters["column"].PropertyType) as ICompositeParameterCollection;
            function.SetValue("column", collection);
            collection.Add(collection.CreateItem());

            var functionConditions = new FunctionFilterConditions();
            functionConditions.Forms = new FunctionFormCollection(form1);
            function.SetValue("conditions", functionConditions);

			string expectedXml = "<itemization-table version=\"2\">" + "<show-print-control>false</show-print-control><show-export-control>false</show-export-control>" +
                                 "<column></column>" + "<conditions>" + "<form name=\"Form 1\" />" + "</conditions>" +
                                 "</itemization-table>";

            Assert.AreEqual(expectedXml, function.ToXml());
        }

        [Test]
        public void RecordCountProducesConditionsXmlWithSingleForm()
        {
            IFunction function = functions["record-count"].CreateInstance();

            function.SetValue("form-name", form1);

            var functionConditions = new FunctionFilterConditions();
            functionConditions.Forms = new FunctionFormCollection(form1);
            function.SetValue("conditions", functionConditions);

            string expectedXml = "<record-count version=\"3\">" + "<form-name>Form 1</form-name>" + "<conditions>" +
                                 "<form name=\"Form 1\" />" + "</conditions>" + "</record-count>";

            Console.WriteLine(function.ToXml());
            Assert.AreEqual(expectedXml, function.ToXml());
        }
    }
}