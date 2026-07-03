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
    public class RecordQualfiedParametersTest2056 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form1 = Project.Current.AddForm();

            fibItem1 = new NewFibItem();
            blank1 = fibItem1.BlankList[0];
            fibItem2 = new NewFibItem();
            blank2 = fibItem2.BlankList[0];

            mcItem = new NewMcqItem();

            form1.ItemList.Add(fibItem1);
            form1.ItemList.Add(fibItem2);
            form1.ItemList.Add(mcItem);
        }

        #endregion

        private IForm form1;
        private IFibItem fibItem1;
        private IFibItem fibItem2;
        private IBlank blank1;
        private IBlank blank2;
        private IMcqItem mcItem;

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

        private IFunction makeListFunction()
        {
            IFunction functionObject = functions["simple-list"].CreateInstance();
            Assert.IsNotNull(functionObject);

            var field = new RecordField(new Record("Record"), blank1);

            var functionBlank = new FunctionBlank();
            functionBlank.Field = field;

            functionObject.SetValue("simple-list-field", functionBlank);
            return functionObject;
        }

        private IFunction makeChoiceTallyTableFunction()
        {
            IFunction functionObject = functions["choice-tally-table"].CreateInstance();
            Assert.IsNotNull(functionObject);

            var field = new RecordField(new Record("Record"), mcItem);

            var functionMCItem = new FunctionMCItem();
            functionMCItem.Field = field;

            functionObject.SetValue("field", functionMCItem);

            return functionObject;
        }

        [Test]
        public void RecordFieldInPersistenceXmlGeneratesRecordQualifiedBlankInPersistenceXml()
        {
            string xmlString =
                "<simple-list version=\"2\">" +
                "<simple-list-field>Record:Form 1:Q1:a</simple-list-field>" +
                "<conditions></conditions>" +
                "</simple-list>";

            IXmlElement element = new XmlElement(xmlString);

            var functionReference = new FunctionXmlReference(element);

            Assert.AreEqual(xmlString, functionReference.ToXml());
        }

        [Test]
        public void RecordFieldInPersistenceXmlGeneratesRecordQualifiedMCItemInPersistenceXml()
        {
            string xmlString =
                "<choice-tally-table version=\"1\">" +
                "<field>Record:Form 1:Q3</field>" +
                "<conditions>" +
                "</conditions>" +
                "</choice-tally-table>";

            IXmlElement element = new XmlElement(xmlString);

            var functionReference = new FunctionXmlReference(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, functionReference.ToXml());
        }

        [Test]
        public void SettingBlankInParameterCreatesRecordField()
        {
            IFunction functionObject = makeListFunction();

            Assert.AreEqual("Record:Form 1:Q1:a", functionObject.GetValue("simple-list-field").ToString());
        }

        [Test]
        public void SettingBlankInParameterGeneratesRecordFieldInPersistenceXml()
        {
            IFunction functionObject = makeListFunction();

            string expectedXml =
                "<simple-list version=\"2\">" +
                "<simple-list-field>Record:Form 1:Q1:a</simple-list-field>" +
                "<conditions></conditions>" +
                "</simple-list>";

            Assert.AreEqual(expectedXml, functionObject.ToXml());
        }

        [Test]
        public void SettingMCItemInParameterCreatesRecordField()
        {
            IFunction functionObject = makeChoiceTallyTableFunction();

            Assert.AreEqual("Record:Form 1:Q3", functionObject.GetValue("field").ToString());
        }

        [Test]
        public void SettingMCItemInParameterGeneratesRecordFieldInPersistenceXml()
        {
            IFunction functionObject = makeChoiceTallyTableFunction();

            string expectedXml =
                "<choice-tally-table version=\"1\">" +
                "<field>Record:Form 1:Q3</field>" +
                "<conditions>" +
                "</conditions>" +
                "</choice-tally-table>";

            Assert.AreEqual(expectedXml, functionObject.ToXml());
        }
    }
}