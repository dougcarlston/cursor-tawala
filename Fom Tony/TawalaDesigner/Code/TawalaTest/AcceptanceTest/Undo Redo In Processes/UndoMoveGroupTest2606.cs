// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.UndoRedoInProcesses
{
    [TestFixture]
    public class UndoMoveGroupTest2606
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();

            setupProcessEditor();
        }

        #endregion

        private FormList forms;
        private IForm form;
        private FibItem fibItem;
        private Process process;
        private RecordSet recordList1;
        private Record record1;
        private TestProcessEditor processEditor;

        private void setupProcessEditor()
        {
            form = Project.Current.AddForm();
            fibItem = new FibItem();
            fibItem.Text = "Fib Item __________";
            form.ItemList.Add(fibItem);

            process = Project.Current.AddProcess();

            // create GET statement ('Get record list from Form 1')
            forms = new FormList();
            forms.Add(Project.Current.FormList[0]);
            recordList1 = new RecordSet("record list", forms);
            var getStatement = new GetStatement(recordList1);
            process.Lines.Add(new ProcessLineList(getStatement));

            // create FOR EACH statement ('For Each record in record list')
            record1 = new Record("record");
            ProcessLineList forEachLines1 = getForEachLines(recordList1, record1);
            process.Lines.Add(forEachLines1);

            // create IF statement ('If Form 1:Q1:a equals 100')
            var ifStatement = new IfStatement();
            ifStatement.Conditions =
                new Conditions(new Condition(fibItem.BlankList[0], HybridOperator.List[HybridOperator.Ops.equals], new Expression("100")));
            var ifLines = new ProcessLineList(ifStatement);
            process.Lines.Insert(3, ifLines);

            var setStatement1 = new SetStatement();
            setStatement1.Variable = new Variable("Variable 1");
            setStatement1.Expression = new Expression("100");
            var setLine = new ProcessLineList(setStatement1);
            process.Lines.Insert(5, setLine);

            processEditor = new TestProcessEditor(process);
        }

        private ProcessLineList getForEachLines(RecordSet recordList, Record record)
        {
            var forEachStatement = new ForEachRecordStatement(record, recordList);
            var forEachLines = new ProcessLineList(forEachStatement);
            return forEachLines;
        }

        [Test]
        public void CanMoveStatementGroupInProcess()
        {
            int i = 0;
            Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
            Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
            Assert.AreEqual("(", process.Lines[i++].ToString());
            Assert.AreEqual("If Form 1:Q1:a equals 100", process.Lines[i++].ToString());
            Assert.AreEqual("(", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual(")", process.Lines[i++].ToString());
            Assert.AreEqual(")", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);

            processEditor.Move(3, -1);

            i = 0;
            Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
            Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
            Assert.AreEqual("(", process.Lines[i++].ToString());
            Assert.AreEqual(")", process.Lines[i++].ToString());
            Assert.AreEqual("If Form 1:Q1:a equals 100", process.Lines[i++].ToString());
            Assert.AreEqual("(", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual(")", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }

        [Test]
        public void MoveCanBeRedone()
        {
            processEditor.Move(3, -1);
            processEditor.Undo();
            processEditor.Redo();

            int i = 0;
            Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
            Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
            Assert.AreEqual("(", process.Lines[i++].ToString());
            Assert.AreEqual(")", process.Lines[i++].ToString());
            Assert.AreEqual("If Form 1:Q1:a equals 100", process.Lines[i++].ToString());
            Assert.AreEqual("(", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual(")", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }

        [Test]
        public void MoveCanBeUndone()
        {
            processEditor.Move(3, -1);
            processEditor.Undo();

            int i = 0;
            Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
            Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
            Assert.AreEqual("(", process.Lines[i++].ToString());
            Assert.AreEqual("If Form 1:Q1:a equals 100", process.Lines[i++].ToString());
            Assert.AreEqual("(", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual(")", process.Lines[i++].ToString());
            Assert.AreEqual(")", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }
    }
}