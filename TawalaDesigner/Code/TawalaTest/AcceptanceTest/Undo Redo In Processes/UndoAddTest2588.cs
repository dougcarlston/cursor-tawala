// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.UndoRedoInProcesses
{
    [TestFixture]
    public class UndoAddTest2588
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();

            setupProcessEditor();
        }

        #endregion

        private IForm form;
        private FibItem fibItem;
        private IDocument document;
        private Process process;
        private TestProcessEditor processEditor;

        private SetStatement setStatement1;
        private SetStatement setStatement2;
        private SetStatement setStatement3;
        private SetStatement setStatement4;

        private void setupProcessEditor()
        {
            form = Project.Current.AddForm();
            fibItem = new FibItem();
            fibItem.Text = "Fib Item __________";
            form.ItemList.Add(fibItem);

            document = Project.Current.AddDocument();

            process = Project.Current.AddProcess();

            setStatement1 = new SetStatement();
            setStatement1.Variable = new Variable("Variable 1");
            setStatement1.Expression = new Expression("100");

            setStatement2 = new SetStatement();
            setStatement2.Variable = new Variable("Variable 2");
            setStatement2.Expression = new Expression("200");

            setStatement3 = new SetStatement();
            setStatement3.Variable = new Variable("Variable 3");
            setStatement3.Expression = new Expression("300");

            setStatement4 = new SetStatement();
            setStatement4.Variable = new Variable("Variable 4");
            setStatement4.Expression = new Expression(fibItem.BlankList[0]);

            processEditor = new TestProcessEditor(process);
        }

        [Test]
        public void AddCanBeRedone()
        {
            processEditor.InsertionIndex = -1;
            processEditor.AddStatement(setStatement1);
            processEditor.Undo();
            processEditor.Redo();

            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }

        [Test]
        public void AddCanBeUndone()
        {
            processEditor.InsertionIndex = -1;
            processEditor.AddStatement(setStatement1);
            processEditor.Undo();

            Assert.AreEqual(0, process.Lines.Count);
        }

        [Test]
        public void CanAddStatementToProcess()
        {
            Assert.AreEqual(0, process.Lines.Count);

            processEditor.InsertionIndex = -1;
            processEditor.AddStatement(setStatement1);

            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }

        [Test]
        public void MultipleAddsCanBeRedone()
        {
            processEditor.InsertionIndex = -1;
            processEditor.AddStatement(setStatement1);
            processEditor.AddStatement(setStatement2);
            processEditor.AddStatement(setStatement3);
            processEditor.AddStatement(setStatement4);

            processEditor.Undo();
            processEditor.Undo();
            processEditor.Undo();
            processEditor.Undo();

            processEditor.Redo();
            processEditor.Redo();
            processEditor.Redo();
            processEditor.Redo();

            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 3 to 300", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 4 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }

        [Test]
        public void MultipleAddsCanBeUndone()
        {
            processEditor.InsertionIndex = -1;
            processEditor.AddStatement(setStatement1);
            processEditor.AddStatement(setStatement2);
            processEditor.AddStatement(setStatement3);
            processEditor.AddStatement(setStatement4);

            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 3 to 300", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 4 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);

            processEditor.Undo();
            processEditor.Undo();
            processEditor.Undo();
            processEditor.Undo();

            Assert.AreEqual(0, process.Lines.Count);
        }
    }
}