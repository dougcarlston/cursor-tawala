// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.UndoRedoInProcesses
{
    [TestFixture]
    public class UndoPasteTest2600
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();

            setupProcessEditor();
        }

        #endregion

        private Process process;
        private TestProcessEditor processEditor;

        private SetStatement setStatement1;
        private SetStatement setStatement2;
        private SetStatement setStatement3;

        private void setupProcessEditor()
        {
            process = Project.Current.AddProcess();
            processEditor = new TestProcessEditor(process);

            setStatement1 = new SetStatement();
            setStatement1.Variable = new Variable("Variable 1");
            setStatement1.Expression = new Expression("100");
            processEditor.InsertionIndex = -1;
            processEditor.AddStatement(setStatement1);

            setStatement2 = new SetStatement();
            setStatement2.Variable = new Variable("Variable 2");
            setStatement2.Expression = new Expression("200");
            processEditor.InsertionIndex = -1;
            processEditor.AddStatement(setStatement2);

            setStatement3 = new SetStatement();
            setStatement3.Variable = new Variable("Variable 3");
            setStatement3.Expression = new Expression("300");
            processEditor.InsertionIndex = -1;
            processEditor.AddStatement(setStatement3);
        }

        [Test]
        public void CanPasteStatementInProcess()
        {
            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 3 to 300", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);

            processEditor.SelectedLineIndex = 1;
            processEditor.Copy();
            processEditor.SelectedLineIndex = -1;
            processEditor.InsertionIndex = 3;
            processEditor.Paste();

            i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 3 to 300", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }

        [Test]
        public void PasteCanBeRedone()
        {
            processEditor.SelectedLineIndex = 1;
            processEditor.Copy();
            processEditor.SelectedLineIndex = -1;
            processEditor.InsertionIndex = 3;
            processEditor.Paste();
            processEditor.Undo();
            processEditor.Redo();

            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 3 to 300", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }

        [Test]
        public void PasteCanBeUndone()
        {
            processEditor.SelectedLineIndex = 1;
            processEditor.Copy();
            processEditor.SelectedLineIndex = -1;
            processEditor.InsertionIndex = 3;
            processEditor.Paste();
            processEditor.Undo();

            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 3 to 300", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }
    }
}