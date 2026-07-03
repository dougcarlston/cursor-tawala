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
    public class UndoDeleteTest2592
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

        private void setupProcessEditor()
        {
            form = Project.Current.AddForm();
            fibItem = new FibItem();
            fibItem.Text = "Fib Item __________";
            form.ItemList.Add(fibItem);

            document = Project.Current.AddDocument();

            process = Project.Current.AddProcess();

            var setStatement1 = new SetStatement();
            setStatement1.Variable = new Variable("Variable 1");
            setStatement1.Expression = new Expression("100");
            process.Lines.Add(new ProcessLineList(setStatement1));

            var setStatement2 = new SetStatement();
            setStatement2.Variable = new Variable("Variable 2");
            setStatement2.Expression = new Expression("200");
            process.Lines.Add(new ProcessLineList(setStatement2));

            var setStatement3 = new SetStatement();
            setStatement3.Variable = new Variable("Variable 3");
            setStatement3.Expression = new Expression("300");
            process.Lines.Add(new ProcessLineList(setStatement3));

            var setStatement4 = new SetStatement();
            setStatement4.Variable = new Variable("Variable 4");
            setStatement4.Expression = new Expression(fibItem.BlankList[0]);
            process.Lines.Add(new ProcessLineList(setStatement4));

            var sendStatement = new SendStatement();
            sendStatement.AddressTo = new FieldOrLiteral("Form 1:Q1:a", FieldOrLiteral.StringType.field);
            sendStatement.SendBody = new SendDocumentBody(document);
            sendStatement.Subject = "Testing";
            process.Lines.Add(new ProcessLineList(sendStatement));

            var deleteStatement = new DeleteStatement();
            deleteStatement.Form = form;
            process.Lines.Add(new ProcessLineList(deleteStatement));

            processEditor = new TestProcessEditor(process);
        }

        private void validateProcessLines()
        {
            process.Lines[0].Validate();
            Assert.AreEqual(true, process.Lines[0].IsValid);

            process.Lines[1].Validate();
            Assert.AreEqual(true, process.Lines[1].IsValid);

            process.Lines[2].Validate();
            Assert.AreEqual(true, process.Lines[2].IsValid);

            process.Lines[3].Validate();
            Assert.AreEqual(true, process.Lines[3].IsValid);

            process.Lines[4].Validate();
            Assert.AreEqual(true, process.Lines[4].IsValid);

            process.Lines[5].Validate();
            Assert.AreEqual(true, process.Lines[5].IsValid);
        }

        [Test]
        public void CanDeleteStatementFromProcess()
        {
            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 3 to 300", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 4 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual("Send Document 1 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual("Delete records from Form 1", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);

            validateProcessLines();

            processEditor.SelectedLineIndex = 1;
            processEditor.Delete();

            i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 3 to 300", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 4 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual("Send Document 1 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual("Delete records from Form 1", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }

        [Test]
        public void DeleteCanBeRedone()
        {
            processEditor.SelectedLineIndex = 1;
            processEditor.Delete();
            processEditor.Undo();
            processEditor.Redo();

            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 3 to 300", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 4 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual("Send Document 1 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual("Delete records from Form 1", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }

        [Test]
        public void DeleteCanBeUndone()
        {
            processEditor.SelectedLineIndex = 1;
            processEditor.Delete();
            processEditor.Undo();

            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 3 to 300", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 4 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual("Send Document 1 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual("Delete records from Form 1", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);

            validateProcessLines();
        }

        [Test]
        public void MultipleDeletesCanBeRedone()
        {
            processEditor.SelectedLineIndex = 0;
            processEditor.Delete();
            processEditor.Delete();
            processEditor.Delete();
            processEditor.Delete();
            processEditor.Delete();
            processEditor.Delete();

            processEditor.Undo();
            processEditor.Undo();
            processEditor.Undo();
            processEditor.Undo();
            processEditor.Undo();
            processEditor.Undo();

            processEditor.Redo();
            processEditor.Redo();
            processEditor.Redo();
            processEditor.Redo();
            processEditor.Redo();
            processEditor.Redo();

            Assert.AreEqual(0, process.Lines.Count);
        }

        [Test]
        public void MultipleDeletesCanBeUndone()
        {
            processEditor.SelectedLineIndex = 0;
            processEditor.Delete();
            processEditor.Delete();
            processEditor.Delete();
            processEditor.Delete();
            processEditor.Delete();

            processEditor.Undo();
            processEditor.Undo();
            processEditor.Undo();
            processEditor.Undo();
            processEditor.Undo();

            int i = 0;
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 2 to 200", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 3 to 300", process.Lines[i++].ToString());
            Assert.AreEqual("Set Variable 4 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual("Send Document 1 to Form 1:Q1:a", process.Lines[i++].ToString());
            Assert.AreEqual("Delete records from Form 1", process.Lines[i++].ToString());
            Assert.AreEqual(i, process.Lines.Count);
        }
    }
}