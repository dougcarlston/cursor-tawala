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
    public class UndoModifyTest2590
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
        }

        [Test]
        public void CanModifyStatementInProcess()
        {
            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);

            processEditor.SelectedLineIndex = 0;
            processEditor.ModifyStatement(setStatement2);

            i = 0;
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }

        [Test]
        public void ModifyCanBeRedone()
        {
            processEditor.SelectedLineIndex = 0;
            processEditor.ModifyStatement(setStatement2);
            processEditor.Undo();
            processEditor.Redo();

            int i = 0;
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }

        [Test]
        public void ModifyCanBeUndone()
        {
            processEditor.SelectedLineIndex = 0;
            processEditor.ModifyStatement(setStatement2);
            processEditor.Undo();

            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }
    }
}