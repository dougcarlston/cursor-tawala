// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.Controls;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FieldsAndFieldsPalette
{
    [TestFixture]
    public class FieldsAndFieldsPaletteTests
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            testPalette = new FieldsPalette();

            form = Project.Current.AddForm();

            var fibItem = new FibItem();
            blank = fibItem.BlankList[0];
            hiddenField = new HiddenField();
            mcItem = new McqItem();

            form.ItemList.Add(fibItem);
            form.ItemList.Add(hiddenField);
            form.ItemList.Add(mcItem);
        }

        [TearDown]
        public void Teardown()
        {
            if (testPalette != null)
            {
                testPalette.Dispose();
                testPalette = null;
            }
            clear();
        }

        #endregion

        private IForm form;
        private Blank blank;
        private McqItem mcItem;
        private HiddenField hiddenField;
        private FieldsPalette testPalette;

        private void clear()
        {
            form = null;
            blank = null;
            hiddenField = null;
            testPalette = null;
        }

        private class PaletteDropTest<T> where T : ProcessTextBox, new()
        {
            private readonly Record record = new Record("aRecord");
            private T textBox = new T();

            public bool AcceptsField(IPaletteField field)
            {
                var node = new TreeNode(field.FieldName);
                node.Tag = field;
                return test(node);
            }

            public bool AcceptsRecordField(IPaletteField field)
            {
                var node = new TreeNode(field.FieldName);
                node.Tag = new RecordField(record, field);
                return test(node);
            }

            private bool test(TreeNode node)
            {
                var textBox = new T();

                try
                {
                    return textBox.AcceptsDropOf(Reflect<FieldsPalette>.InvokeStaticMethod<IDataObject>("createDataObject", node));
                }
                finally
                {
                    textBox.Dispose();
                }
            }
        }

        public const string RecordName = "TheRecord";
        public const string RecordSetName = "Record List";

        private class ProcessWithRecordNode
        {
            private readonly Process process;
            private Record record;
            private RecordSet recordSet;

            public ProcessWithRecordNode(IForm form)
            {
                process = Project.Current.AddProcess();

                createRecordSet(form);

                addGetStatement();

                addForEachStatement();

                process.Records.AddUnique(record);

                insertCommentInsideForEach();
            }

            private void createRecordSet(IForm form)
            {
                var formList = new FormList();
                formList.Add(form);

                recordSet = new RecordSet(RecordSetName, formList);
                record = new Record(RecordName);
            }

            private void insertCommentInsideForEach()
            {
                var commentStatement = new CommentStatement("Comment");
                process.Lines.Insert(3, new ProcessLineList(commentStatement));
            }

            private void addForEachStatement()
            {
                var forEachStatement = new ForEachRecordStatement(record, recordSet);
                var forEachLine = new ProcessLineList(forEachStatement);
                process.Lines.Add(forEachLine);
            }

            private void addGetStatement()
            {
                var getStatement = new GetStatement(recordSet);
                var getLines = new ProcessLineList(getStatement);
                process.Lines.Add(getLines);
            }

            public void MoveInsideForEachStatement()
            {
                Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 3));
            }
        }

        [Test]
        public void AssignableFields()
        {
            IAssignableField assignableBlank = blank;
            IAssignableField assignableMCQ = mcItem;
            IAssignableField assignableField = hiddenField;
            IAssignableField assignableVar = new Variable("MyVariable");
            IAssignableField assignableRecordField = new RecordField(new Record("Rec"), hiddenField);

            Assert.IsNotNull(assignableBlank);
            Assert.AreEqual(blank.QualifiedFieldName, assignableBlank.AssignmentName);

            Assert.IsNotNull(assignableMCQ);
            Assert.AreEqual(mcItem.QualifiedFieldName, assignableMCQ.AssignmentName);

            Assert.IsNotNull(assignableField);
            Assert.AreEqual(hiddenField.QualifiedFieldName, assignableField.AssignmentName);

            Assert.IsNotNull(assignableVar);
            Assert.AreEqual(((Variable)assignableVar).FieldName, assignableVar.AssignmentName);

            Assert.IsNotNull(assignableRecordField);
            Assert.AreEqual(((RecordField)assignableRecordField).QualifiedFieldName, assignableRecordField.AssignmentName);
        }

        [Test]
        public void FieldsPaletteFormNode()
        {
            testPalette.Show();
            testPalette.RefreshFieldList();

            TreeNode formNode = Util.FindTreeNodeByText(testPalette.FieldsTreeView.Nodes, "Form 1");

            Assert.IsNotNull(formNode);
            Assert.AreEqual(3, formNode.Nodes.Count);
            Assert.AreEqual("Q1:a", formNode.Nodes[0].Text);
            Assert.AreEqual("Field1", formNode.Nodes[1].Text);
            Assert.AreEqual("Q2", formNode.Nodes[2].Text);
        }

        [Test]
        public void FieldsPaletteRecordNode()
        {
            var process = new ProcessWithRecordNode(form);

            testPalette.Show();
            process.MoveInsideForEachStatement();
            testPalette.RefreshFieldList();

            TreeNode recordNode = Util.FindTreeNodeByText(testPalette.FieldsTreeView.Nodes, RecordName);

            Assert.IsNotNull(recordNode);
            Assert.AreEqual(3, recordNode.Nodes.Count);
            Assert.AreEqual("Form 1:Q1:a", recordNode.Nodes[0].Text);
            Assert.AreEqual("Form 1:Field1", recordNode.Nodes[1].Text);
            Assert.AreEqual("Form 1:Q2", recordNode.Nodes[2].Text);
        }

        [Test]
        public void PaletteFieldsAcceptedByComplexExpressionTextBox()
        {
            var pdt = new PaletteDropTest<ComplexExpressionTextBox>();
            var variable = new Variable("var");

            Assert.IsTrue(pdt.AcceptsField(blank));
            Assert.IsTrue(pdt.AcceptsField(mcItem));
            Assert.IsTrue(pdt.AcceptsField(mcItem.Choices[0]));
            Assert.IsTrue(pdt.AcceptsField(hiddenField));
            Assert.IsTrue(pdt.AcceptsField(variable));

            Assert.IsTrue(pdt.AcceptsRecordField(blank));
            Assert.IsTrue(pdt.AcceptsRecordField(mcItem));
            Assert.IsTrue(pdt.AcceptsRecordField(hiddenField));
        }

        [Test]
        public void PaletteFieldsAcceptedByVariableTextBox()
        {
            var pdt = new PaletteDropTest<VariableTextBox>();
            var variable = new Variable("var");

            Assert.IsTrue(pdt.AcceptsField(blank));
            Assert.IsTrue(pdt.AcceptsField(mcItem));
            Assert.IsFalse(pdt.AcceptsField(mcItem.Choices[0]));
            Assert.IsTrue(pdt.AcceptsField(hiddenField));
            Assert.IsTrue(pdt.AcceptsField(variable));

            Assert.IsTrue(pdt.AcceptsRecordField(blank));
            Assert.IsTrue(pdt.AcceptsRecordField(mcItem));
            Assert.IsTrue(pdt.AcceptsRecordField(hiddenField));
        }
    }
}