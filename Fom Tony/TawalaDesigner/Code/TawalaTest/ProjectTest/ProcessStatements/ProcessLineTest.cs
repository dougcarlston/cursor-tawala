//#define ForEachQuestionStatement
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the ProcessLine class
	/// </summary>
	[TestFixture]
	public class ProcessLineTest
	{
		private IForm form;
		private Process process;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			// create clean project
			Project.NewTestProject();

			// create form
			form = Project.Current.AddForm();

			// create process
			process = Project.Current.AddProcess();

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			// create FIB items
			FibItem fibItem1 = new FibItem();
			FibItem fibItem2 = new FibItem();

			// add new FIB items to form
			form.ItemList.Add(fibItem1);
			form.ItemList.Add(fibItem2);
		}

		[Test]
		public void ConstructAppend()
		{
			// create uninitialized process line
			ProcessLine unInitLine = new AppendLine();
			Assert.IsNotNull(unInitLine);

			// create process line from append statement
			Document doc1 = new Document("Document 1");
			Document doc2 = new Document("Document 2");
			AppendStatement appendStatement = new AppendStatement(doc1, doc2);

			ProcessLine line = new AppendLine(appendStatement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual("Append " + doc1.Name + " to " + doc2.Name, appendStatement.ToString());
			Assert.AreEqual("<append document=\"" + doc2.Name + "\" appendage=\"" + doc1.Name + "\"/>", line.ToXml());
		}

		[Test]
		public void ConstructShowDocument() 
		{ 
			// create uninitialized process line
			ProcessLine unInitLine = new ShowDocumentLine();
			Assert.IsNotNull(unInitLine);

			// create process line from show statement
			//Document doc1 = new Document("Document 1");
			IDocument doc1 = Project.Current.AddDocument();
			ShowDocumentStatement showStatement = new ShowDocumentStatement(doc1);

			ProcessLine line = new ShowDocumentLine(showStatement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual("Show Document Document 1", line.ToString());
			Assert.AreEqual("<show document=\"Document 1\" reset=\"false\"/>", line.ToXml());
		}

		[Test]
		public void ConstructShowForm()
		{
			// create uninitialized process line
			ProcessLine unInitLine = new ShowFormLine();
			Assert.IsNotNull(unInitLine);

			// create process line from show statement
			ShowFormStatement showStatement = new ShowFormStatement(form);

			ProcessLine line = new ShowFormLine(showStatement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual("Show Form Form 1", line.ToString());
			Assert.AreEqual("<show form=\"Form 1\"/>", line.ToXml());
		}

		[Test]
		public void ConstructShowRecord()
		{
			// create process line from show statement
			ShowRecordStatement showStatement = new ShowRecordStatement(form);

			ProcessLine line = new ShowRecordLine(showStatement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual("Show stored record from Form 1", line.ToString());
			Assert.AreEqual("<edit form=\"Form 1\" submit=\"modify\">\r\n</edit>", line.ToXml());
		}

		[Test]
		public void ConstructIf() 
		{ 
			// create top process line from if statement
			ComparisonOperator compOp = StringOperator.List[StringOperator.Ops.beginsWith];
			Condition condition = new Condition(new Field("Q1:a"), compOp, new Expression("S"));
			Conditions conditions = new Conditions(condition);
			IfStatement ifStatement = new IfStatement(conditions);

			ProcessLine line = new IfLine(ifStatement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(true, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(ifStatement, line.Statement);

			string expString = "If Q1:a begins with \"S\"";

			Assert.AreEqual(expString, line.ToString());
		}

		[Test]
		public void ConstructSendEmail() 
		{ 
			// create process line from Send statement
			SendStatement sendStatement = new SendStatement();
			sendStatement.AddressTo.Text = "doug@carlston.net";
			sendStatement.AddressCc.Text = "jdf@jdftech.com";
			sendStatement.Subject = "Testing the Send command";
			sendStatement.SendBody = new SendEmailBody("Hi Doug, can you read this?");

			ProcessLine line = new SendLine(sendStatement);
			process.Lines.Add(line);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(sendStatement, line.Statement);
			Assert.AreEqual("Send Email to \"doug@carlston.net\"", line.ToString());

			// test with a field label as the address
			sendStatement.AddressTo.Text = "Q1:a";
			sendStatement.AddressTo.Type = FieldOrLiteral.StringType.field;

			ProcessLine line2 = new SendLine(sendStatement);

			//Assertions 
			Assert.IsNotNull(line2);
			Assert.AreEqual(false, line2.SelectsGroup);
			Assert.AreEqual(true, line2.IsSelectable);
			Assert.AreEqual(true, line2.IsDeletable);
			Assert.AreEqual(true, line2.CanInsertBefore);
			Assert.AreEqual(0, line2.IndentLevel);
			Assert.AreEqual(sendStatement, line2.Statement);
			Assert.AreEqual("Send Email to Form 1:Q1:a", line2.ToString());
		}

		[Test]
		public void ConstructSendDocument()
		{
			// create process line from Send statement
			SendStatement sendStatement = new SendStatement();
			sendStatement.AddressTo.Text = "doug@carlston.net";
			sendStatement.AddressCc.Text = "jdf@jdftech.com";
			sendStatement.Subject = "Testing the Send command";
			sendStatement.SendBody = new SendDocumentBody(new Document("Test Document"));

			ProcessLine line = new SendLine(sendStatement);
			process.Lines.Add(line);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(sendStatement, line.Statement);
			Assert.AreEqual("Send Test Document to \"doug@carlston.net\"", line.ToString());

			// test with a field label as the address
			sendStatement.AddressTo.Text = "Q1:a";
			sendStatement.AddressTo.Type = FieldOrLiteral.StringType.field;

			ProcessLine line2 = new SendLine(sendStatement);

			//Assertions 
			Assert.IsNotNull(line2);
			Assert.AreEqual(false, line2.SelectsGroup);
			Assert.AreEqual(true, line2.IsSelectable);
			Assert.AreEqual(true, line2.IsDeletable);
			Assert.AreEqual(true, line2.CanInsertBefore);
			Assert.AreEqual(0, line2.IndentLevel);
			Assert.AreEqual(sendStatement, line2.Statement);
			Assert.AreEqual("Send Test Document to Form 1:Q1:a", line2.ToString());
		}

		[Test]
		public void ConstructSendInvitation()
		{
			// create process line from Send statement
			SendStatement sendStatement = new SendStatement();
			sendStatement.AddressTo.Text = "doug@carlston.net";
			sendStatement.AddressCc.Text = "jdf@jdftech.com";
			sendStatement.Subject = "Testing the Send command";
			sendStatement.SendBody = new SendInvitationBody(new Form("Test Form"), "Test Body");

			ProcessLine line = new SendLine(sendStatement);
			process.Lines.Add(line);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(sendStatement, line.Statement);
			Assert.AreEqual("Send Invitation to Test Form to \"doug@carlston.net\"", line.ToString());

			// test with a field label as the address
			sendStatement.AddressTo.Text = "Q1:a";
			sendStatement.AddressTo.Type = FieldOrLiteral.StringType.field;

			ProcessLine line2 = new SendLine(sendStatement);

			//Assertions 
			Assert.IsNotNull(line2);
			Assert.AreEqual(false, line2.SelectsGroup);
			Assert.AreEqual(true, line2.IsSelectable);
			Assert.AreEqual(true, line2.IsDeletable);
			Assert.AreEqual(true, line2.CanInsertBefore);
			Assert.AreEqual(0, line2.IndentLevel);
			Assert.AreEqual(sendStatement, line2.Statement);
			Assert.AreEqual("Send Invitation to Test Form to Form 1:Q1:a", line2.ToString());
		}

		[Test]
		public void ConstructSet()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("a variable");
			statement.Expression = new Expression("a value");

			ProcessLine line = new SetLine(statement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(statement, line.Statement);
			Assert.AreEqual("Set a variable to \"a value\"", line.ToString());
		}

		[Test]
		public void ConstructAdd()
		{
			AddStatement statement = new AddStatement();
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;
			statement.Variable = "a variable";

			ProcessLine line = new ArithmeticLine(statement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(statement, line.Statement);
			Assert.AreEqual("Add 2 to a variable", line.ToString());
		}

		[Test]
		public void ConstructSubtract()
		{
			SubtractStatement statement = new SubtractStatement();
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;
			statement.Variable = "a variable";

			ProcessLine line = new ArithmeticLine(statement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(statement, line.Statement);
			Assert.AreEqual("Subtract 2 from a variable", line.ToString());
		}

		[Test]
		public void ConstructMultiply()
		{
			MultiplyStatement statement = new MultiplyStatement();
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;
			statement.Variable = "a variable";

			ProcessLine line = new ArithmeticLine(statement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(statement, line.Statement);
			Assert.AreEqual("Multiply a variable by 2", line.ToString());
		}

		[Test]
		public void ConstructDivide()
		{
			DivideStatement statement = new DivideStatement();
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;
			statement.Variable = "a variable";

			ProcessLine line = new ArithmeticLine(statement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(statement, line.Statement);
			Assert.AreEqual("Divide a variable by 2", line.ToString());
		}

		[Test]
		public void ConstructGet()
		{
			FormList forms = new FormList();
			forms.Add(form);
			GetStatement statement = new GetStatement(new RecordSet("record list", forms));

			ProcessLine line = new GetLine(statement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(statement, line.Statement);
			Assert.AreEqual("Get record list from Form 1", line.ToString());
		}

		[Test]
		public void IsValidLineGet()
		{
			FormList forms = new FormList();
			forms.Add(Project.Current.FormList[0]);
			GetStatement statement = new GetStatement(new RecordSet("record list", forms));

			ProcessLine line = new GetLine(statement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.IsTrue(line.IsValidLine(null));

			Project.Current.RemoveForm(forms[0].Name);

			Assert.IsFalse(line.IsValidLine(null));
		}

		[Test]
		public void ConstructForEachRecord()
		{
			// create top process line from for each statement
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(new Record("record"), new RecordSet("record list", forms));

			ProcessLine line = new ForEachRecordLine(forEachStatement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(true, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(forEachStatement, line.Statement);

			string expString = "For Each record in record list";

			Assert.AreEqual(expString, line.ToString());
		}

#if ForEachQuestionStatement
		public void ConstructForEachQuestion()
		{
			// create top process line from for each statement
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			ForEachQuestionStatement forEachStatement = new ForEachQuestionStatement();

			ProcessLine line = new ForEachQuestionLine(forEachStatement);

			//Assertions 
			Assert.IsNotNull(line);
			Assert.AreEqual(true, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
			Assert.AreEqual(0, line.IndentLevel);
			Assert.AreEqual(forEachStatement, line.Statement);

			string expString = "For Each record in record list";

			Assert.AreEqual(expString, line.ToString());
		}
#endif

		[Test]
		public void Copy()
		{
			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement = new ShowDocumentStatement(doc1);
			ShowLine line = new ShowDocumentLine(showStatement);

			ShowLine copiedLine = (ShowDocumentLine)line.Copy();

			Assert.IsTrue(copiedLine is ShowDocumentLine);
			Assert.IsFalse(copiedLine.Statement == line.Statement);
			Assert.AreSame(doc1, ((ShowStatement)copiedLine.Statement).Document);
		}
	}
}
