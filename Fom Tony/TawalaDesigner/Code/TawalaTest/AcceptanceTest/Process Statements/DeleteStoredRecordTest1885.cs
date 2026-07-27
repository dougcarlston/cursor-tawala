// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.ProcessStatements
{
    /// <summary>
    /// Acceptance tests for story 1885 (Delete stored records for a Form).
    /// </summary>
    [TestFixture]
    public class DeleteStoredRecordsTest1885
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();
            fibItem = new FibItem();
            form.ItemList.Add(fibItem);

            process = Project.Current.AddProcess();

            testPalette = new FieldsPalette();
            testPalette.Show();
        }

        [TearDown]
        public void TearDown()
        {
            testPalette.Dispose();
        }

        #endregion

        private FieldsPalette testPalette;

        private IForm form;
        private FibItem fibItem;
        private Process process;

        private const string NEWLINE = "\r\n";

        private string deleteXmlString =
            "<delete>" +
            "<form name=\"Form 1\"/>" +
            "</delete>";

        private string deleteXmlStringWithCondition =
            "<delete>" +
            "<form name=\"Form 1\"/>" +
            "<conditions>" + NEWLINE +
            "<equals field=\"Record:Form 1:Q1:a\">" + NEWLINE +
            "<string value=\"foo\"/>" + NEWLINE +
            "</equals>" + NEWLINE +
            "</conditions>" +
            "</delete>";

        [Test]
        public void AddDeleteFormRecordNodeToFieldsPalette()
        {
            Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 0));

            var deleteForms = new FunctionFormCollection(form);
            testPalette.ConditionsForms = deleteForms;

            testPalette.RefreshFieldList();

            Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);
            Assert.AreEqual("Form 1", testPalette.FieldsTreeView.Nodes[0].Text);
            Assert.AreEqual("Variables", testPalette.FieldsTreeView.Nodes[1].Text);
            Assert.AreEqual("Record", testPalette.FieldsTreeView.Nodes[2].Text);

            Assert.AreEqual(1, testPalette.FieldsTreeView.Nodes[2].Nodes.Count);
            Assert.AreEqual("Form 1:Q1:a", testPalette.FieldsTreeView.Nodes[2].Nodes[0].Text);
        }

        [Test]
        public void DeleteStatementGeneratesDeleteStatementXml()
        {
            var deleteStatement = new DeleteStatement();
            deleteStatement.Form = form;

            Assert.AreEqual(deleteXmlString, deleteStatement.ToXml());
        }

        [Test]
        public void DeleteStatementWithConditionGeneratesDeleteStatementXmlWithCondition()
        {
            var deleteStatement = new DeleteStatement();
            deleteStatement.Form = form;

            var recordField = new RecordField(new Record("Record"), fibItem.BlankList[0]);
            deleteStatement.Conditions = new Conditions(recordField, HybridOperator.List[HybridOperator.Ops.equals], new Expression("foo"));

            Assert.AreEqual(deleteXmlStringWithCondition, deleteStatement.ToXml());
        }

        [Test]
        public void DeleteStatementXmlGeneratesDeleteStatement()
        {
            var deleteStatement = new DeleteStatement(new XmlElement(deleteXmlString), process);

            Assert.AreEqual("Delete", deleteStatement.Name);
            Assert.AreSame(form, deleteStatement.Form);
            Assert.AreEqual("Delete records from Form 1", deleteStatement.ToString());
        }

        [Test]
        public void DeleteStatementXmlWithConditionGeneratesDeleteStatementWithCondition()
        {
            Process process = Project.Current.AddProcess();

            var fibItem1 = new FibItem();
            form.ItemList.Add(fibItem1);

            IXmlElement element = new XmlElement(deleteXmlStringWithCondition);
            var deleteStatement = new DeleteStatement(element, process);

            Assert.IsNotNull(deleteStatement.Conditions);
            Assert.AreEqual("Record:Form 1:Q1:a equals \"foo\"", deleteStatement.Conditions.ToString());
            Assert.AreEqual("Delete records from Form 1 where Record:Form 1:Q1:a equals \"foo\"", deleteStatement.ToString());
        }
    }
}