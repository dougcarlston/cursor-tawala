// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NMock2;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.ProcessStatements
{
    [TestFixture]
    public class ShowRecordStatement2067
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
            form = Project.Current.AddForm();
            process = Project.Current.AddProcess();

            fibItem = new FibItem();
            fibItem.BlankList.Clear();
            blank = new Blank(fibItem, 1);
            fibItem.BlankList.Add(blank);
            form.ItemList.Add(fibItem);
            mockery = new Mockery();
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        private Process process;
        private IForm form;
        private FibItem fibItem;
        private Blank blank;
        private Mockery mockery;

        private const string NEWLINE = "\r\n";

        private void addConditionListWithOneCondition(ShowRecordStatement statement)
        {
            var recordField = new RecordField(new Record("Record"), fibItem.BlankList[0]);
            statement.Conditions = new Conditions(recordField, HybridOperator.List[HybridOperator.Ops.equals], new Expression("foo"));
        }

        [Test]
        public void ConstructShowRecordLine()
        {
            var showRecordStatement = new ShowRecordStatement(form);
            ProcessLine line = new ShowRecordLine(showRecordStatement);

            //Assertions 
            Assert.AreEqual(false, line.SelectsGroup);
            Assert.AreEqual(true, line.IsSelectable);
            Assert.AreEqual(true, line.IsDeletable);
            Assert.AreEqual(true, line.CanInsertBefore);
            Assert.AreEqual(0, line.IndentLevel);
            Assert.AreEqual("Show stored record from Form 1", line.ToString());
            Assert.AreEqual("<edit form=\"Form 1\" submit=\"modify\">\r\n</edit>", line.ToXml());
        }

        [Test]
        public void ProcessLineListConstructedWithStatementResultsInShowRecordLine()
        {
            var statement = new ShowRecordStatement(form);
            var list = new ProcessLineList(statement);
            process.Lines.Add(list);

            Assert.IsTrue(process.Lines[0] is ShowRecordLine);
        }

        [Test]
        public void StatementConstructedFromEditRecordXmlWithConditionsGeneratesSameXml()
        {
            string xml = "<edit form=\"Form 1\" submit=\"modify\">" + NEWLINE +
                         "<conditions>" + NEWLINE +
                         "<equals field=\"Record:Form 1:Q1:a\">" + NEWLINE +
                         "<string value=\"foo\"/>" + NEWLINE +
                         "</equals>" + NEWLINE +
                         "</conditions>" + NEWLINE +
                         "</edit>";

            var recordField = new RecordField(new Record("Record"), fibItem.BlankList[0]);

            var statement = new ShowRecordStatement(new XmlElement(xml), process);

            Assert.AreEqual(xml, statement.ToXml());
        }

        [Test]
        public void StatementWithConditionProducesEditRecordXml()
        {
            string xml = "<edit form=\"Form 1\" submit=\"modify\">" + NEWLINE +
                         "<conditions>" + NEWLINE +
                         "<equals field=\"Record:Form 1:Q1:a\">" + NEWLINE +
                         "<string value=\"foo\"/>" + NEWLINE +
                         "</equals>" + NEWLINE +
                         "</conditions>" + NEWLINE +
                         "</edit>";

            var statement = new ShowRecordStatement(form);
            addConditionListWithOneCondition(statement);
            Assert.AreEqual(xml, statement.ToXml());
        }

        [Test]
        public void StatementWithConditionToString()
        {
            var statement = new ShowRecordStatement(form);
            addConditionListWithOneCondition(statement);
            Assert.AreEqual("Show stored record from Form 1 where Record:Form 1:Q1:a equals \"foo\"", statement.ToString());
        }

        [Test]
        public void StatementWithModifyOnSubmitFalseProducesEditRecordXml()
        {
            string xml =
                "<edit form=\"Form 1\" submit=\"new\">" + NEWLINE +
                "</edit>";

            var statement = new ShowRecordStatement(form);
            statement.ModifyOnSubmit = false;
            Assert.AreEqual(xml, statement.ToXml());
        }

        [Test]
        public void StatementWithModifyOnSubmitTrueProducesEditRecordXml()
        {
            string xml =
                "<edit form=\"Form 1\" submit=\"modify\">" + NEWLINE +
                "</edit>";

            var statement = new ShowRecordStatement(form);
            Assert.AreEqual(xml, statement.ToXml());
        }
    }
}