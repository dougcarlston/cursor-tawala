using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest.Functions
{
    [TestFixture]
    public class ItemizationTableFunctionTest2020 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();

            fibItem1 = new NewFibItem();
            blank1 = fibItem1.BlankList[0];
            fibItem2 = new NewFibItem();
            blank2 = fibItem2.BlankList[0];

            form.ItemList.Add(fibItem1);
            form.ItemList.Add(fibItem2);
        }

        #endregion

        private IForm form;
        private IFibItem fibItem1;
        private IFibItem fibItem2;
        private IBlank blank1;
        private IBlank blank2;

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

            function.Info.Parameters["number-of-columns"].SetValue(function, 2);

            IParameterInfo compositeCollection = function.Info.Parameters["column"];
            var collection = Activator.CreateInstance(compositeCollection.PropertyType) as ICompositeParameterCollection;
            string compositeName = compositeCollection.PropertyType.FullName.Replace("__composite_collection_",
                                                                                     "__composite_");

            Type compositeType = Type.GetType(compositeName);
            var composite1 = Activator.CreateInstance(compositeType) as ICompositeParameter;
            var expression1 =
                new FunctionCompoundExpression(new XmlElement("<container><string value=\"Name\"/></container>"));
            composite1.GetType().GetProperty("Header").SetValue(composite1, expression1, null);
            var contents1 = new FunctionContentsField(blank1);
            composite1.GetType().GetProperty("Contents").SetValue(composite1, contents1, null);

            var composite2 = Activator.CreateInstance(compositeType) as ICompositeParameter;
            var expression2 =
                new FunctionCompoundExpression(new XmlElement("<container><string value=\"Dish\"/></container>"));
            composite1.GetType().GetProperty("Header").SetValue(composite2, expression2, null);
            var contents2 = new FunctionContentsField(blank2);
            composite2.GetType().GetProperty("Contents").SetValue(composite2, contents2, null);

            collection.Add(composite1);
            collection.Add(composite2);

            Assert.AreEqual(2, collection.Count);

            compositeCollection.SetValue(function, collection);

            var functionConditions = new FunctionConditions();
            functionConditions.Forms = new FunctionFormCollection(form);
            function.SetValue("conditions", functionConditions);

            Project.FunctionMapById.AddUnique(function);

            return function;
        }

        [Test]
        public void FunctionProducesExpectedDisplayString()
        {
            IFunction instance = createItemizationTableFunction();

            const string expectedString = "<<MULTIPLE QUESTION LIST(2, Form 1:Q1:a, ...)>>";

            Assert.AreEqual(expectedString, instance.ToDisplayString());
        }

        [Test]
        public void ItemizationTableFunctionProducesItemizationTableXml()
        {
            IFunction function = createItemizationTableFunction();

            string expectedString =
                "<itemization-table version=\"2\">" +
                "<number-of-columns>2</number-of-columns>" +
                "<column>" +
                "<header><string value=\"Name\"/>" + Environment.NewLine + "</header>" +
                "<contents>" +
                "<field name=\"Form 1:Q1:a\" />" +
                "</contents>" +
                "</column>" +
                "<column>" +
                "<header><string value=\"Dish\"/>" + Environment.NewLine + "</header>" +
                "<contents>" +
                "<field name=\"Form 1:Q2:a\" />" +
                "</contents>" +
                "</column>" +
                "<conditions><form name=\"Form 1\" /></conditions>" +
                "</itemization-table>";

            Assert.AreEqual(expectedString, function.ToXml());
        }

        [Test]
        public void ItemizationTableXmlProducesItemizationTableXml()
        {
            string xmlString =
                "<itemization-table version=\"2\">" +
                "<number-of-columns>2</number-of-columns>" +
                "<column>" +
                "<header><string value=\"Name\"/>" + Environment.NewLine + "</header>" +
                "<contents>" +
                "<field name=\"Form 1:Q1:a\" />" +
                "</contents>" +
                "</column>" +
                "<column>" +
                "<header><string value=\"Dish\"/>" + Environment.NewLine + "</header>" +
                "<contents>" +
                "<field name=\"Form 1:Q2:a\" />" +
                "</contents>" +
                "</column>" +
                "<conditions>" +
                "<form name=\"Form 1\" />" +
                "<conditions>" + NEWLINE +
                "<equals field=\"Record:Form 1:Q1:a\">" + NEWLINE +
                "<string value=\"foo\"/>" + NEWLINE +
                "</equals>" + NEWLINE +
                "</conditions>" +
                "</conditions>" +
                "</itemization-table>";

            var functionReference = new FunctionXmlReference(new XmlElement(xmlString));
            var function = Project.FunctionMapById[functionReference.Function.InstanceId];

            Assert.IsNotNull(function);
            Assert.AreEqual("itemization-table", function.Info.Id);

            Assert.AreEqual(xmlString, function.ToXml());
        }

        [Test]
        public void ItemizationTableXmlWithEmptyFieldsProducesItemizationTableXmlWithEmptyFields()
        {
            string xmlString =
                "<itemization-table version=\"2\">" +
                "<number-of-columns>2</number-of-columns>" +
                "<column>" +
                "<contents>" +
                "<field name=\"Record:Form 1:Q1:a\" />" +
                "</contents>" +
                "</column>" +
                "<column>" +
                "<header><string value=\"Dish\"/>" + Environment.NewLine + "</header>" +
                "</column>" +
                "<conditions>" +
                "<form name=\"Form 1\" />" +
                "</conditions>" +
                "</itemization-table>";

            var functionReference = new FunctionXmlReference(new XmlElement(xmlString));
            var function = functionReference.Function;

            Assert.AreEqual(xmlString, function.ToXml());
        }

        [Test]
        public void ItemizationTableXmlWithoutNumberOfColumnsProducesItemizationTableXmlWithNumberOfColumns()
        {
            string xmlString =
                "<itemization-table version=\"2\">" +
                "<column>" +
                "<header><string value=\"Name\"/>" + Environment.NewLine + "</header>" +
                "<contents>" +
                "<field name=\"Form 1:Q1:a\" />" +
                "</contents>" +
                "</column>" +
                "<column>" +
                "<header><string value=\"Dish\"/>" + Environment.NewLine + "</header>" +
                "<contents>" +
                "<field name=\"Form 1:Q2:a\" />" +
                "</contents>" +
                "</column>" +
                "<conditions>" +
                "<form name=\"Form 1\" />" +
                "<conditions>" + NEWLINE +
                "<equals field=\"Record:Form 1:Q1:a\">" + NEWLINE +
                "<string value=\"foo\"/>" + NEWLINE +
                "</equals>" + NEWLINE +
                "</conditions>" +
                "</conditions>" +
                "</itemization-table>";

            var functionReference = new FunctionXmlReference(new XmlElement(xmlString));
            IFunction function = functionReference.Function;

            string expectedXml =
                "<itemization-table version=\"2\">" +
                "<number-of-columns>2</number-of-columns>" +
                "<column>" +
                "<header><string value=\"Name\"/>" + Environment.NewLine + "</header>" +
                "<contents>" +
                "<field name=\"Form 1:Q1:a\" />" +
                "</contents>" +
                "</column>" +
                "<column>" +
                "<header><string value=\"Dish\"/>" + Environment.NewLine + "</header>" +
                "<contents>" +
                "<field name=\"Form 1:Q2:a\" />" +
                "</contents>" +
                "</column>" +
                "<conditions>" +
                "<form name=\"Form 1\" />" +
                "<conditions>" + NEWLINE +
                "<equals field=\"Record:Form 1:Q1:a\">" + NEWLINE +
                "<string value=\"foo\"/>" + NEWLINE +
                "</equals>" + NEWLINE +
                "</conditions>" +
                "</conditions>" +
                "</itemization-table>";

            Assert.AreEqual(expectedXml, function.ToXml());
        }

        [Test]
        public void ItemizationTableXmlWithRecordFieldsProducesItemizationTableXmlWithRecordFields()
        {
            string xmlString =
                "<itemization-table version=\"2\">" +
                "<number-of-columns>2</number-of-columns>" +
                "<column>" +
                "<header><string value=\"Name\"/>" + Environment.NewLine + "</header>" +
                "<contents>" +
                "<field name=\"Record:Form 1:Q1:a\" />" +
                "</contents>" +
                "</column>" +
                "<column>" +
                "<header><string value=\"Dish\"/>" + Environment.NewLine + "</header>" +
                "<contents>" +
                "<field name=\"Record:Form 1:Q2:a\" />" +
                "</contents>" +
                "</column>" +
                "<conditions>" +
                "<form name=\"Form 1\" />" +
                "</conditions>" +
                "</itemization-table>";

            var functionReference = new FunctionXmlReference(new XmlElement(xmlString));
            var function = functionReference.Function;

            Assert.AreEqual(xmlString, function.ToXml());
        }
    }
}