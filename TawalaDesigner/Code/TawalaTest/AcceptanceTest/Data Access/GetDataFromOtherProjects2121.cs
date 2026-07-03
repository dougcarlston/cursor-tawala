// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using NMock2;
using NUnit.Framework;
using Tawala.Processes;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;
using GlobalSettings=Tawala.Common.GlobalSettings;

namespace TawalaTest.AcceptanceTest.DataAccess
{
    [TestFixture]
    public class GetDataFromOtherProjects2121
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            setup();
            mockery = new Mockery();
        }

        #endregion

        private IForm form;
        private Mockery mockery;
        private const string defaultFibStyleAtttribute = " style=\"topLabels\"";

        private void setup()
        {
            Util.NewTestProject();
            initializeTestDataSources();

            form = Project.Current.AddForm();

            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);
        }

        private void checkCombinedFormList(FormList combined)
        {
            Assert.AreEqual(3, combined.Count);

            Assert.AreEqual(form, combined[0]);
            Assert.AreEqual(1, combined[0].ItemList.Count);

            Assert.AreEqual(3, combined[1].ItemList.Count);
            Assert.AreEqual(2, combined[2].ItemList.Count);

            var Fib1 = combined[1].ItemList[0] as FibItem;
            Assert.IsNotNull(Fib1);
            Assert.AreEqual("ClientInfo:Q1:a", Fib1.BlankList[0].QualifiedFieldName);

            var Fib2 = combined[1].ItemList[1] as FibItem;
            Assert.IsNotNull(Fib2);
            Assert.AreEqual("ClientInfo:name", Fib2.BlankList[0].QualifiedFieldName);

            var mc1 = combined[1].ItemList[2] as McqItem;
            Assert.AreEqual("ClientInfo:Q3", mc1.QualifiedFieldName);
            Assert.IsNotNull(mc1);
            Assert.AreEqual(2, mc1.Choices.Count);
            Assert.IsTrue(mc1.SelectOnlyOne);

            var Fib3 = combined[2].ItemList[0] as FibItem;
            Assert.IsNotNull(Fib3);
            Assert.AreEqual("DataSource2:Q1:a", Fib3.BlankList[0].QualifiedFieldName);

            var mc2 = combined[2].ItemList[1] as McqItem;
            Assert.IsNotNull(mc2);
            Assert.AreEqual("DataSource2:Q2", mc2.QualifiedFieldName);
            Assert.AreEqual(3, mc2.Choices.Count);
            Assert.IsFalse(mc2.SelectOnlyOne);
        }

        private const string testDataSourceXml =
            "<datasources>" +
            "<datasource name=\"ClientInfo\">" +
            "<field name=\"Q1:a\" type=\"string\"/>" +
            "<field name=\"name\" type=\"string\"/>" +
            "<field name=\"Q3\" type=\"mcq\" choices=\"2\" onlyone=\"true\"/>" +
            "</datasource>" +
            "<datasource name=\"DataSource2\">" +
            "<field name=\"Q1:a\" type=\"string\"/>" +
            "<field name=\"Q2\" type=\"mcq\" choices=\"3\" onlyone=\"false\"/>" +
            "</datasource>" +
            "</datasources>";

        private void initializeTestDataSources()
        {
            initialize(testDataSourceXml);
        }

        private void initialize(string dataSourcesXml)
        {
            MethodInfo init = typeof(FieldProviders).GetMethod("initialize", BindingFlags.NonPublic | BindingFlags.Static);
            init.Invoke(null, new object[] {new XmlElement(dataSourcesXml)});
        }

        private static readonly string xmlWithDataSourceName =
            "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" dataSourceName=\"ClientInfo\" blockBackButton=\"false\">" +
            Environment.NewLine +
            "<items>" + Environment.NewLine +
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            XmlConstants.DefaultTabsXml +
            XmlConstants.FullBeginFont +
            "~@!~[Replace this with your question. Underscores create blanks.]~@!~ " +
            XmlConstants.EndFont +
            "<blank label=\"a\" length=\"20\" required=\"false\"></blank></paragraph></fib>" + Environment.NewLine +
            "</items>" + Environment.NewLine +
            "</form>" + Environment.NewLine;

        [Test]
        public void DataSourceNameInFormGeneratesDataSourceNameInXml()
        {
            form.DataSourceName = "ClientInfo";

            Console.WriteLine(form.ToXml());

            Assert.AreEqual(xmlWithDataSourceName, form.ToXml());

            form.DataSourceName = "";

            string expectedXml =
                "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" blockBackButton=\"false\">" + Environment.NewLine +
                "<items>" + Environment.NewLine +
                "<fib label=\"Q1\" style=\"topLabels\"><paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.DefaultTabsXml +
                XmlConstants.FullBeginFont +
                "~@!~[Replace this with your question. Underscores create blanks.]~@!~ " +
                XmlConstants.EndFont +
                "<blank label=\"a\" length=\"20\" required=\"false\"></blank></paragraph></fib>" + Environment.NewLine +
                "</items>" + Environment.NewLine +
                "</form>" + Environment.NewLine;

            Assert.AreEqual(expectedXml, form.ToXml());
        }

        [Test]
        public void DataSourceNameInXmlGeneratesDataSourceNameInForm()
        {
            var newForm = new Form(new XmlElement(xmlWithDataSourceName));

            Assert.AreEqual("ClientInfo", newForm.DataSourceName);
        }

        [Test]
        public void DataSourceObjectInGetStatementGeneratesGetStatementWithDataSourceXml()
        {
            var view = mockery.NewMock<IStatementView>();
            Stub.On(view).GetProperty("Process").Will(Return.Value(null));
            var presenter = new GetStatementPresenter(view);
            FormList presenterFormList = presenter.GetFormList();

            var getStatement = new GetStatement();
            var formList = new FormList();
            formList.Add(presenterFormList[0]);
            formList.Add(presenterFormList[1]);
            getStatement.Records = new RecordSet("Record Set 1", formList);

            string expectedXml =
                "<get recordList=\"Record Set 1\">" + Environment.NewLine +
                "<forms>" + Environment.NewLine +
                "<form name=\"Form 1\"/>" + Environment.NewLine +
                "<form name=\"ClientInfo\" externalSharedData=\"true\"/>" + Environment.NewLine +
                "</forms>" + Environment.NewLine +
                "</get>";

            Assert.AreEqual(expectedXml, getStatement.ToXml());
        }

        [Test]
        public void ExternalFieldsFromDataSourceXml()
        {
            Assert.AreEqual(1, Project.Current.FormList.Count);
            Assert.AreEqual(1, Project.Current.FormList[0].ItemList.Count);

            FormList combined = Project.Current.AllForms;
            checkCombinedFormList(combined);
        }

        [Test]
        public void FieldProvidersAllForms()
        {
            Assert.AreEqual(1, Project.Current.FormList.Count);
            Assert.AreEqual(1, Project.Current.FormList[0].ItemList.Count);

            FormList combined = Project.Current.AllForms;
            checkCombinedFormList(combined);
        }

        [Test]
        public void GetStatementPresenter_GetFieldProviderList_ReturnsAllProviders()
        {
            var view = mockery.NewMock<IStatementView>();
            Stub.On(view).GetProperty("Process").Will(Return.Value(null));
            var presenter = new GetStatementPresenter(view);
            FormList formList = presenter.GetFormList();

            Assert.AreEqual(3, formList.Count);
            Assert.AreEqual("Form 1", formList[0].Name);
            Assert.AreEqual("ClientInfo", formList[1].Name);
            Assert.AreEqual("DataSource2", formList[2].Name);

            FieldList fields1 = formList[0].GetAllFields();
            FieldList fields2 = formList[1].GetAllFields();
            FieldList fields3 = formList[2].GetAllFields();

            Assert.AreEqual(1, fields1.Count);
            Assert.AreEqual(3, fields2.Count);
            Assert.AreEqual(2, fields3.Count);

            Assert.IsInstanceOfType(typeof(Blank), fields1[0]);
            Assert.AreEqual("Q1:a", fields1[0].FieldName);

            Assert.IsInstanceOfType(typeof(Blank), fields2[0]);
            Assert.AreEqual("Q1:a", fields2[0].FieldName);
            Assert.IsInstanceOfType(typeof(Blank), fields2[1]);
            Assert.AreEqual("name", fields2[1].FieldName);
            Assert.IsInstanceOfType(typeof(McqItem), fields2[2]);
            Assert.AreEqual("Q3", fields2[2].FieldName);

            var mcItem = fields2[2] as McqItem;
            Assert.AreEqual(2, mcItem.Choices.Count);
            Assert.AreEqual(true, mcItem.SelectOnlyOne);

            Assert.IsInstanceOfType(typeof(Blank), fields3[0]);
            Assert.AreEqual("Q1:a", fields3[0].FieldName);
            Assert.IsInstanceOfType(typeof(McqItem), fields3[1]);
            Assert.AreEqual("Q2", fields3[1].FieldName);

            mcItem = fields3[1] as McqItem;
            Assert.AreEqual(3, mcItem.Choices.Count);
            Assert.AreEqual(false, mcItem.SelectOnlyOne);
        }

        [Test]
        public void GetStatementWithDataSourceXmlGeneratesDataSourceObjectInGetStatement()
        {
            string getWithDataSourceXml =
                "<get recordList=\"Record Set 1\">" + Environment.NewLine +
                "<forms>" + Environment.NewLine +
                "<form name=\"Form 1\" blockBackButton=\"false\"/>" + Environment.NewLine +
                "<form name=\"ClientInfo\" externalSharedData=\"true\" blockBackButton=\"false\"/>" + Environment.NewLine +
                "</forms>" + Environment.NewLine +
                "</get>";

            var getStatement = new GetStatement(new XmlElement(getWithDataSourceXml), new Process("Process 1"));

            Assert.AreEqual(2, getStatement.Records.Forms.Count);
            Assert.IsInstanceOfType(typeof(Form), getStatement.Records.Forms[0]);
            Assert.IsInstanceOfType(typeof(ExternalForm), getStatement.Records.Forms[1]);
        }

        [Test]
        public void QueryingServerForDataSourcesResultsInFormListWithCorrectFields()
        {
            Util.NewTestProject();

            string credentialsXml = GlobalSettings.CredentialsElement("TestDataSources3141", "thisisatest");
            bool bResult = FieldProviders.QueryServerAndSaveToFile(credentialsXml);
            FieldProviders.LoadDataSourcesFromFile();
            Assert.IsTrue(bResult);

            form = Project.Current.AddForm();

            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);

            FormList allForms = Project.Current.AllForms;

            Assert.AreEqual(2, allForms.Count);
            Assert.AreEqual("Form 1", allForms[0].Name);
            Assert.AreEqual("TestDataSource", allForms[1].Name);

            IForm dataSource = allForms[1];
            FieldList fieldList = dataSource.GetAllFields();
            Assert.AreEqual(5, fieldList.Count);
            Assert.AreEqual("Name", fieldList[0].FieldName);
            Assert.AreEqual("Q2:a", fieldList[1].FieldName);
            Assert.AreEqual("Q3", fieldList[2].FieldName);
            Assert.AreEqual("Q4", fieldList[3].FieldName);
            Assert.AreEqual("AVariable", fieldList[4].FieldName);

            Assert.IsInstanceOfType(typeof(McqItem), fieldList[2]);
            Assert.IsInstanceOfType(typeof(McqItem), fieldList[3]);

            Assert.IsTrue(((McqItem)fieldList[2]).SelectOnlyOne);
            Assert.IsFalse(((McqItem)fieldList[3]).SelectOnlyOne);
        }
    }
}