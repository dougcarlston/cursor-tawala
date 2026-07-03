using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tawala.Projects;
using NUnit.Framework;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class ClipboardCopyPasteProcess417 : ClipboardTester<Process>
    {
        private Process process;
		private IForm form;
		private FormList formList;
		private FibItem fib1;

        [SetUp]
        public void SetUp()
        {
            SetUpTest();
            process = project.AddProcess();
			
			form = project.AddForm();
			fib1 = new FibItem();
			form.ItemList.Add(fib1);

			formList = new FormList();
			formList.Add(form);
		}
        [Test]
        public void CopyPasteEmptyProcess()
        {
            Process clipboardCopy = CopyPaste();
            Assert.IsNotNull(clipboardCopy, ErrorMessage);
        }

        [Test]
        public void CopyPasteProcessWithAppendStatement()
        {
            AppendStatement statement = new AppendStatement(project.AddDocument(), project.AddDocument());
            process.Lines.Add(new AppendLine(statement));

            Process clipboardCopy = CopyPaste();
            Assert.IsNotNull(clipboardCopy, ErrorMessage);

            AppendStatement statementCopy = clipboardCopy.Lines[0].Statement as AppendStatement;
            Assert.AreNotSame(statement, statementCopy);
            Assert.AreSame(statementCopy.Appendage, statement.Appendage);
            Assert.AreSame(statementCopy.Document, statementCopy.Document);
        }

        [Test]
        public void CopyPasteProcessWithGetStatement()
        {
			GetStatement statement = createGetStatement();

            Process clipboardCopy = CopyPaste();
            Assert.IsNotNull(clipboardCopy, ErrorMessage);

            GetStatement statementCopy = clipboardCopy.Lines[0].Statement as GetStatement;
            Assert.AreNotSame(statement, statementCopy);
            Assert.AreNotSame(statement.Records, statementCopy.Records);
            Assert.AreNotSame(statement.Records.Forms, statementCopy.Records.Forms);
            Assert.AreSame(statement.Records.Forms[0], statementCopy.Records.Forms[0]);
            Assert.AreEqual(statement.Records.Forms[0].Name, statementCopy.Records.Forms[0].Name);
        }

		[Test]
		public void CopyPasteProcessWithGetStatementContainingConditionsWithRecordSetField()
		{
			GetStatement statement = createGetStatement();
			RecordSet recordSet = statement.Records;

			RecordSetField recordSetField = new RecordSetField(recordSet, fib1.BlankList[0]);
			statement.Conditions = new Conditions(recordSetField, HybridOperator.List[HybridOperator.Ops.isBlank]);
			process.Lines.Add(new GetLine(statement));

			Process clipboardCopy = CopyPaste();
			Assert.IsNotNull(clipboardCopy, ErrorMessage);

			GetStatement statementCopy = clipboardCopy.Lines[0].Statement as GetStatement;
			Assert.AreNotSame(statement, statementCopy);
			Assert.AreNotSame(statement.Conditions, statementCopy.Conditions);
			Assert.AreNotSame(statement.Conditions[0], statementCopy.Conditions[0]);

			RecordSetField copiedRecordSetField = ((Condition)statementCopy.Conditions[0]).Field as RecordSetField;
			Assert.AreNotSame(recordSetField, copiedRecordSetField, "Field references in copied conditions are the same.");

			Assert.AreSame(recordSetField.ReferenceField, copiedRecordSetField.ReferenceField);
			Assert.AreEqual("Get aRecordSet from Form 1 where aRecordSet:Form 1:Q1:a is blank", statementCopy.ToString());
		}

		[Test]
		public void CopyPasteProcessWithForEachStatement()
		{
			GetStatement getStatement = createGetStatement();
			ForEachRecordStatement statement = new ForEachRecordStatement(new Record("aRecord"), getStatement.Records);
			process.Lines.Add(new ForEachRecordLine(statement));

			Process clipboardCopy = CopyPaste();
			Assert.IsNotNull(clipboardCopy, ErrorMessage);

			ForEachRecordStatement statementCopy = clipboardCopy.Lines[1].Statement as ForEachRecordStatement;
			Assert.AreNotSame(statement, statementCopy);
			Assert.AreNotSame(statement.RecordList, statementCopy.RecordList);
			Assert.AreNotSame(statement.Record, statementCopy.Record);
			Assert.AreNotEqual(statement.Record.Id, statementCopy.Record.Id);
			Assert.AreNotSame(statement.Qualifier, statementCopy.Qualifier);
		}

		[Test]
        public void CopyPasteProcessWithShowDocumentStatement()
        {
            ShowDocumentStatement statement = new ShowDocumentStatement(project.AddDocument());
            process.Lines.Add(new ShowDocumentLine(statement));

            Process clipboardCopy = CopyPaste();
            Assert.IsNotNull(clipboardCopy, ErrorMessage);

            ShowDocumentStatement statementCopy = clipboardCopy.Lines[0].Statement as ShowDocumentStatement;
            Assert.AreNotSame(statement, statementCopy);
            Assert.AreSame(statementCopy.Document, statement.Document);
        }

        [Test]
        public void CopyPasteProcessWithShowFormStatement()
        {
            ShowFormStatement statement = new ShowFormStatement(project.AddForm());
            process.Lines.Add(new ShowFormLine(statement));

            Process clipboardCopy = CopyPaste();
            Assert.IsNotNull(clipboardCopy, ErrorMessage);

            ShowFormStatement statementCopy = clipboardCopy.Lines[0].Statement as ShowFormStatement;
            Assert.AreNotSame(statement, statementCopy);
            Assert.AreSame(statementCopy.Form, statement.Form);
        }

		[Test]
		public void CopyPasteProcessWithSetStatementContainingQualifiedField()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("var1");
			statement.Expression = new Expression(((FibItem)(form.ItemList[0])).BlankList[0]);
			process.Lines.Add(new SetLine(statement));

			Process clipboardCopy = CopyPaste();
			Assert.IsNotNull(clipboardCopy, ErrorMessage);

			SetStatement statementCopy = clipboardCopy.Lines[0].Statement as SetStatement;
			Assert.AreNotSame(statement, statementCopy);
			Assert.AreNotSame(statement.Expression, statementCopy.Expression);
			Assert.AreSame(((FieldElement)(statement.Expression.Elements[0])).Field, ((FieldElement)(statementCopy.Expression.Elements[0])).Field);
			Assert.AreSame(statement.Variable, statementCopy.Variable);
			Assert.AreEqual("Set var1 to Form 1:Q1:a", statementCopy.ToString());
		}

		[Test]
		public void CopyPasteProcessWithSetStatementContainingRecordQualifiedField()
		{
			Record record = createGetAndForEachStatements();

			RecordField recordField = new RecordField(record, fib1.BlankList[0]);

			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("var1");
			statement.Expression = new Expression(recordField);
			process.Lines.Insert(2, new SetLine(statement));

			Process clipboardCopy = CopyPaste();
			Assert.IsNotNull(clipboardCopy, ErrorMessage);

			SetStatement statementCopy = clipboardCopy.Lines[2].Statement as SetStatement;
			
			RecordField copiedRecordField = ((FieldElement)(statementCopy.Expression.Elements[0])).Field as RecordField;
			Assert.AreNotSame(recordField, copiedRecordField);
			Assert.AreSame(recordField.ReferenceField, copiedRecordField.ReferenceField);
			Assert.AreEqual("Set var1 to aRecord:Form 1:Q1:a", statementCopy.ToString());
		}


		[Test]
		public void CopyPasteProcessWithIfStatementContainingQualifiedField()
		{
			Conditions conditions = new Conditions(fib1.BlankList[0], HybridOperator.List[HybridOperator.Ops.isBlank]);
			IfStatement statement = new IfStatement(conditions);
			process.Lines.Add(new IfLine(statement));

			Process clipboardCopy = CopyPaste();
			Assert.IsNotNull(clipboardCopy, ErrorMessage);

			IfStatement statementCopy = clipboardCopy.Lines[0].Statement as IfStatement;
			Assert.AreNotSame(statement, statementCopy);
			Assert.AreNotSame(statement.Conditions, statementCopy.Conditions);
			Assert.AreNotSame(statement.Conditions[0], statementCopy.Conditions[0]);
			Assert.AreSame(((Condition)statement.Conditions[0]).Field, ((Condition)statementCopy.Conditions[0]).Field, "Field reference in copied conditions is different.");
			Assert.AreEqual("If Form 1:Q1:a is blank", statementCopy.ToString());
		}

		[Test]
		public void CopyPasteProcessWithIfStatementContainingRecordQualifiedField()
		{
			Record record = createGetAndForEachStatements();

			RecordField recordField = new RecordField(record, fib1.BlankList[0]);

			Conditions conditions = new Conditions(recordField, HybridOperator.List[HybridOperator.Ops.isBlank]);
			IfStatement statement = new IfStatement(conditions);
			process.Lines.Insert(2,new IfLine(statement));

			Process clipboardCopy = CopyPaste();
			Assert.IsNotNull(clipboardCopy, ErrorMessage);

			IfStatement statementCopy = clipboardCopy.Lines[2].Statement as IfStatement;
			RecordField copiedRecordField = ((Condition)statementCopy.Conditions[0]).Field as RecordField;
			Assert.AreNotSame(recordField, copiedRecordField, "Field references in copied conditions are the same.");
			Assert.AreSame(recordField.ReferenceField, copiedRecordField.ReferenceField);
			Assert.AreEqual("If aRecord:Form 1:Q1:a is blank", statementCopy.ToString());
		}

		private Record createGetAndForEachStatements()
		{
			GetStatement getStatement = createGetStatement();

			Record record = new Record("aRecord");
			process.Lines.Add(new ForEachRecordLine(new ForEachRecordStatement(record, getStatement.Records)));
			return record;
		}

		private GetStatement createGetStatement()
		{
			GetStatement getStatement = new GetStatement(new RecordSet("aRecordSet", formList));
			process.Lines.Add(new GetLine(getStatement));

			return getStatement;
		}

		protected override Process GetComponent()
        {
            return Project.Current.ProcessList[0] as Process;
        }
    }
}
