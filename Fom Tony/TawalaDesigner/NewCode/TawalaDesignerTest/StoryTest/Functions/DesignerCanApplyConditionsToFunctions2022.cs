using System;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.ProjectUI;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest.Functions
{
    [TestFixture]
    public class DesignerCanApplyConditionsToFunctions2022 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form1 = Project.Current.AddForm();
            form1.ItemList.Add(new NewMcqItem());

            form2 = Project.Current.AddForm();
            form2.ItemList.Add(new NewFibItem());
            form2.ItemList.Add(new NewFibItem());

            testPalette = new FieldsPalette();
            testPalette.Show();
            testPalette.RefreshFieldList();
        }

        #endregion

        private IForm form1;
        private IForm form2;
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

        private IFunction createRecordCountFunctionWithConditions()
        {
            IFunction instance = functions["record-count"].CreateInstance();

            instance.SetValue("form-name", form2);

            var recordField = new RecordField(new Record("Record"), ((IFibItem)form2.ItemList[0]).BlankList[0]);

            ComparisonOperator equals = HybridOperator.List[HybridOperator.Ops.equals];
            var condition1 = new Condition(recordField, equals, new Expression("foo"));

            var functionConditions = new FunctionConditions();
            functionConditions.Forms = new FunctionFormCollection(form2);
            functionConditions.Conditions = new Conditions(condition1);

            instance.SetValue("conditions", functionConditions);

            return instance;
        }

        [Test]
        public void AddingConditionToFunctionProducesExpectedDisplayString()
        {
            IFunction instance = createRecordCountFunctionWithConditions();

            string expectedString = "<<FORM RECORD COUNT(Form 2, Record:Form 2:Q1:a equals \"foo\")>>";

            Assert.AreEqual(expectedString, instance.ToDisplayString());
        }

        [Test]
        public void AddingConditionToFunctionProducesPersistenceXmlWithConditions()
        {
            IFunction instance = createRecordCountFunctionWithConditions();

            string expectedXml =
                "<record-count version=\"3\">" +
                "<form-name>Form 2</form-name>" +
                "<conditions>" +
                "<form name=\"Form 2\" />" +
                "<conditions>" + NEWLINE +
                "<equals field=\"Record:Form 2:Q1:a\">" + NEWLINE +
                "<string value=\"foo\"/>" + NEWLINE +
                "</equals>" + NEWLINE +
                "</conditions>" +
                "</conditions>" +
                "</record-count>";

            Assert.AreEqual(expectedXml, instance.ToXml());
        }

        [Test]
        public void ChangingFunctionConditionsFormChangesRecordNodeInFieldsPalette()
        {
            var forms = new FunctionFormCollection();
            forms.AddUnique(form1);
            testPalette.ConditionsForms = forms;

            Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

            forms = new FunctionFormCollection();
            forms.AddUnique(form2);
            testPalette.ConditionsForms = forms;
            Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

            TreeNode recordNode = testPalette.FieldsTreeView.Nodes[3];
            Assert.AreEqual("Record", recordNode.Text);
            Assert.AreEqual(2, recordNode.Nodes.Count);
            Assert.AreEqual("Form 2:Q1:a", recordNode.Nodes[0].Text);
            Assert.AreEqual("Form 2:Q2:a", recordNode.Nodes[1].Text);
        }

        [Test]
        public void EmptyFunctionConditionsFormsRemovesRecordNodeFromFieldsPalette()
        {
            var forms = new FunctionFormCollection();
            forms.AddUnique(form1);
            testPalette.ConditionsForms = forms;
            Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

            testPalette.ConditionsForms = new FunctionFormCollection();
            Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);
            Assert.AreEqual("Form 1", testPalette.FieldsTreeView.Nodes[0].Text);
            Assert.AreEqual("Form 2", testPalette.FieldsTreeView.Nodes[1].Text);
        }

        [Test]
        public void PersistenceXmlWithConditionReferencingFieldProducesPersistenceXmlWithConditionReferencingField()
        {
            string xmlString =
                "<record-count version=\"3\">" +
                "<form-name>Form 2</form-name>" +
                "<conditions>" +
                "<form name=\"Form 2\" />" +
                "<conditions>" + NEWLINE +
                "<equals field=\"Record:Form 2:Q1:a\">" + NEWLINE +
                "<string field=\"Form 2:Q1:a\"/>" + NEWLINE +
                "</equals>" + NEWLINE +
                "</conditions>" +
                "</conditions>" +
                "</record-count>";

            IXmlElement element = new XmlElement(xmlString);
            var functionReference = new FunctionXmlReference(element);
            Assert.AreEqual(xmlString, functionReference.ToXml());
        }

        [Test]
        public void PersistenceXmlWithMultipleConditionsProducesPersistenceXmlWithMultipleConditions()
        {
            string xmlString =
                "<record-count version=\"3\">" +
                "<form-name>Form 2</form-name>" +
                "<conditions>" +
                "<form name=\"Form 2\" />" +
                "<conditions>" + NEWLINE +
                "<or>" + NEWLINE +
                "<equals field=\"Record:Form 2:Q1:a\">" + NEWLINE +
                "<string value=\"foo\"/>" + NEWLINE +
                "</equals>" + NEWLINE +
                "<equals field=\"Record:Form 2:Q1:a\">" + NEWLINE +
                "<string value=\"bar\"/>" + NEWLINE +
                "</equals>" + NEWLINE +
                "</or>" + NEWLINE +
                "</conditions>" +
                "</conditions>" +
                "</record-count>";

            IXmlElement element = new XmlElement(xmlString);
            var functionReference = new FunctionXmlReference(element);
            Assert.AreEqual(xmlString, functionReference.ToXml());
        }

        [Test]
        public void PersistenceXmlWithSingleConditionProducesPersistenceXmlWithSingleCondition()
        {
            string xmlString =
                "<record-count version=\"3\">" +
                "<form-name>Form 2</form-name>" +
                "<conditions>" +
                "<form name=\"Form 2\" />" +
                "<conditions>" + NEWLINE +
                "<equals field=\"Record:Form 2:Q1:a\">" + NEWLINE +
                "<string value=\"foo\"/>" + NEWLINE +
                "</equals>" + NEWLINE +
                "</conditions>" +
                "</conditions>" +
                "</record-count>";

            IXmlElement element = new XmlElement(xmlString);
            var functionReference = new FunctionXmlReference(element);
            Assert.AreEqual(xmlString, functionReference.ToXml());
        }

        [Test]
        public void PersistenceXmlWithSingleConditionProducesValidConditionInFunction()
        {
            string xmlString =
                "<record-count version=\"3\">" +
                "<form-name>Form 2</form-name>" +
                "<conditions>" +
                "<form name=\"Form 2\" />" +
                "<conditions>" + NEWLINE +
                "<equals field=\"Record:Form 2:Q1:a\">" + NEWLINE +
                "<string value=\"foo\"/>" + NEWLINE +
                "</equals>" + NEWLINE +
                "</conditions>" +
                "</conditions>" +
                "</record-count>";

            var functionReference = new FunctionXmlReference(new XmlElement(xmlString));

            IFunction function = Project.FunctionMapById[functionReference.Function.InstanceId];

            var functionConditions = function.Info.Parameters[1].GetValue(function) as FunctionConditions;
            Assert.IsNotNull(functionConditions);
            Assert.AreSame(form2, (functionConditions.Forms)[0]);

            var condition = functionConditions.Conditions[0] as Condition;

            var recordField = condition.Field as RecordField;
            Assert.IsNotNull(recordField);
            Assert.IsInstanceOfType(typeof(IBlank), recordField.ReferenceField);
        }

        [Test]
        public void SettingFunctionConditionsFormsCreatesRecordNodeInFieldsPalette()
        {
            Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);
            Assert.AreEqual("Form 1", testPalette.FieldsTreeView.Nodes[0].Text);
            Assert.AreEqual("Form 2", testPalette.FieldsTreeView.Nodes[1].Text);

            var forms = new FunctionFormCollection();
            forms.AddUnique(form1);
            testPalette.ConditionsForms = forms;

            Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);
            TreeNode recordNode = testPalette.FieldsTreeView.Nodes[3];
            Assert.AreEqual("Record", recordNode.Text);
            Assert.AreEqual(1, recordNode.Nodes.Count);
            Assert.AreEqual("Form 1:Q1", recordNode.Nodes[0].Text);
        }

        [Test]
        public void SettingFunctionConditionsFormWithNoFieldsDoesNotCreateRecordNodeInFieldsPalette()
        {
            IForm form3 = Project.Current.AddForm();
            testPalette.RefreshFieldList();
            Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

            testPalette.ConditionsForms = new FunctionFormCollection(form3);
            Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);
        }

        [Test]
        public void XmlConditionsParameterResultsInConditionsProperty()
        {
            IFunctionInfo info = functions["record-count"];
            IFunction instance = info.CreateInstance();
            IParameterInfo parameterInfo = info.Parameters[1];

            Assert.IsNotNull(parameterInfo);
            Assert.AreEqual(parameterInfo.PropertyType, typeof(FunctionConditions));
            Assert.AreEqual("Conditions", parameterInfo.PropertyName);
            Assert.AreEqual("Count only the records", parameterInfo.Name, "Friendly name doesn't match expectations");
        }
    }
}