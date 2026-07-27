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
    public class UndoRedoActionVisibleTest2610
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
        public void AddYieldsProperActionText()
        {
            processEditor.AddStatement(setStatement1);

            Assert.AreEqual("Add", processEditor.UndoActionText);
            Assert.AreEqual("", processEditor.RedoActionText);
        }

        [Test]
        public void DefaultRedoActionTextIsEmpty()
        {
            Assert.AreEqual("", processEditor.RedoActionText);
        }

        [Test]
        public void DefaultUndoActionTextIsEmpty()
        {
            Assert.AreEqual("", processEditor.UndoActionText);
        }

        [Test]
        public void RedoAddYieldsProperActionText()
        {
            processEditor.AddStatement(setStatement1);
            processEditor.Undo();
            processEditor.Redo();

            Assert.AreEqual("Add", processEditor.UndoActionText);
            Assert.AreEqual("", processEditor.RedoActionText);
        }

        [Test]
        public void UndoAddYieldsProperActionText()
        {
            processEditor.AddStatement(setStatement1);
            processEditor.Undo();

            Assert.AreEqual("", processEditor.UndoActionText);
            Assert.AreEqual("Add", processEditor.RedoActionText);
        }
    }
}