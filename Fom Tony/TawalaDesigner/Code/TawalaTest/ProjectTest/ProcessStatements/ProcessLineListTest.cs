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
	/// Test class for ProcessLineList class.
	/// </summary>
	[TestFixture]
	public class ProcessLineListTest
	{
		private IForm form;
		private Process process;
		private FibItem fibItem1;

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
			fibItem1 = new FibItem();
			FibItem fibItem2 = new FibItem();

			// add new FIB items to form
			form.ItemList.Add(fibItem1);
			form.ItemList.Add(fibItem2);
		}

		[Test]
		public void ConstructShowDocument() 
		{ 
			// create process line list from show statement
			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement = new ShowDocumentStatement(doc1);

			ProcessLineList showList = new ProcessLineList(showStatement);

			//Assertions 
			Assert.AreEqual(1, showList.Count);
			Assert.AreEqual(false, showList[0].SelectsGroup);
			Assert.AreEqual(showStatement, showList[0].Statement);
		}

		[Test]
		public void ConstructShowForm()
		{
			// create process line list from show statement
			ShowFormStatement showStatement = new ShowFormStatement(form);

			ProcessLineList showList = new ProcessLineList(showStatement);

			//Assertions 
			Assert.AreEqual(1, showList.Count);
			Assert.AreEqual(false, showList[0].SelectsGroup);
			Assert.AreEqual(showStatement, showList[0].Statement);
		}

		[Test]
		public void ConstructIf() 
		{ 
			// create process line list from if statement
			Field field = new Field("Q1:a");
			ComparisonOperator compOp = StringOperator.List[StringOperator.Ops.beginsWith];
			string expressionString = "S";
			Conditions conditions = new Conditions(field, compOp, new Expression(expressionString));
			IfStatement ifStatement1 = new IfStatement(conditions);
			ProcessLineList ifList1 = new ProcessLineList(ifStatement1);

			// Assertions
			Assert.AreEqual(3, ifList1.Count);
			Assert.AreEqual("If Q1:a begins with \"S\"", ifList1[0].ToString());
			Assert.AreEqual("(", ifList1[1].ToString());
			Assert.AreEqual(")", ifList1[2].ToString());

			string expString1 =
				"If Q1:a begins with \"S\"\r\n" +
				"(\r\n" +
				")\r\n";

			Assert.AreEqual(expString1, ifList1.ToString());

			//Assertions 
			Assert.AreEqual(true, ifList1[0].SelectsGroup);
			Assert.AreEqual(ifStatement1, ifList1[0].Statement);

			Assert.AreEqual(false, ifList1[1].SelectsGroup);
			Assert.AreEqual(false, ifList1[2].SelectsGroup);

			// create process line list from if statement (with otherwise clause)
			IfStatement ifStatement2 = new IfStatement(conditions, true);
			ProcessLineList ifList2 = new ProcessLineList(ifStatement2);

			string expString2 =
				"If Q1:a begins with \"S\"\r\n" +
				"(\r\n" +
				")\r\n" +
				"Otherwise\r\n" +
				"(\r\n" +
				")\r\n";

			Assert.AreEqual(expString2, ifList2.ToString());

		} 

		[Test]
		public void ConstructSend() 
		{ 
			// create process line list from send statement
			SendStatement sendStatement = new SendStatement();
			process.Lines.Add(new ProcessLineList(sendStatement));

			sendStatement.AddressTo.Text = "doug@carlston.net";
			sendStatement.SendBody = new SendEmailBody("Some body text.");

			ProcessLineList sendList = new ProcessLineList(sendStatement);

			//Assertions 
			Assert.AreEqual(1, sendList.Count);
			Assert.AreEqual(false, sendList[0].SelectsGroup);
			Assert.AreEqual(sendStatement, sendList[0].Statement);
			Assert.AreEqual("Send Email to \"doug@carlston.net\"", sendList[0].ToString());

			// test with a field label as the address
			sendStatement.AddressTo.Text = "Q1:a";
			sendStatement.AddressTo.Type = FieldOrLiteral.StringType.field;

			ProcessLineList sendList2 = new ProcessLineList(sendStatement);

			//Assertions 
			Assert.AreEqual(1, sendList2.Count);
			Assert.AreEqual(false, sendList2[0].SelectsGroup);
			Assert.AreEqual(sendStatement, sendList2[0].Statement);
			Assert.AreEqual("Send Email to Form 1:Q1:a", sendList2[0].ToString());
		}

		[Test]
		public void ConstructSet()
		{
			// create process line list from set statement
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("a variable");
			statement.Expression = new Expression("a value");

			ProcessLineList list = new ProcessLineList(statement);

			//Assertions 
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(false, list[0].SelectsGroup);
			Assert.AreEqual(statement, list[0].Statement);
			Assert.AreEqual("Set a variable to \"a value\"", list[0].ToString());
		}

		[Test]
		public void ConstructAppend()
		{
			// create process line list from statement
			Document doc1 = new Document("Document 1");
			Document doc2 = new Document("Document 2");
			AppendStatement statement = new AppendStatement(doc1, doc2);

			ProcessLineList list = new ProcessLineList(statement);

			//Assertions 
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(false, list[0].SelectsGroup);
			Assert.AreEqual(statement, list[0].Statement);
			Assert.AreEqual("Append Document 1 to Document 2", list[0].ToString());
		}

		[Test]
		public void ConstructSkip()
		{
			// create process line list from skip statement
			SkipToStatement statement = new SkipToStatement();
			ProcessLineList list = new ProcessLineList(statement);

			//Assertions 
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(false, list[0].SelectsGroup);
			Assert.AreEqual(statement, list[0].Statement);
		}

		[Test]
		public void ConstructGet()
		{
			FormList forms = new FormList();
			forms.Add(form);
			GetStatement statement = new GetStatement(new RecordSet("record list", forms));

			ProcessLineList list = new ProcessLineList(statement);

			//Assertions 
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(false, list[0].SelectsGroup);
			Assert.AreEqual(statement, list[0].Statement);
			Assert.AreEqual("Get record list from Form 1", list[0].ToString());
		}

		[Test]
		public void ConstructDelete()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			DeleteStatement statement = new DeleteStatement();
			statement.Form = Project.Current.FormList[0];

			ProcessLineList list = new ProcessLineList(statement);

			//Assertions 
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(false, list[0].SelectsGroup);
			Assert.AreEqual(statement, list[0].Statement);
			Assert.AreEqual("Delete records from Form 1", list[0].ToString());
		}

		[Test]
		public void ConstructForEach()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			ForEachRecordStatement statement = new ForEachRecordStatement(new Record("record"), new RecordSet("record list", forms));

			ProcessLineList list = new ProcessLineList(statement);

			//Assertions 
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(true, list[0].SelectsGroup);
			Assert.AreEqual(statement, list[0].Statement);
			Assert.AreEqual("For Each record in record list", list[0].ToString());
			Assert.AreEqual("(", list[1].ToString());
			Assert.AreEqual(")", list[2].ToString());
		}

		[Test]
		public void ConstructComment()
		{
			CommentStatement statement = new CommentStatement("A comment.");
			ProcessLineList list = new ProcessLineList(statement);

			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(false, list[0].SelectsGroup);
			Assert.AreEqual(statement, list[0].Statement);
			Assert.AreEqual("A comment.", list[0].ToString());
		}

		[Test]
		public void IsInsideForEachInsideIf()
		{
			// create process line list from if statement ('If Q1:a is not blank')
			ComparisonOperator compOp = HybridOperator.List[HybridOperator.Ops.isNotBlank];
			Conditions conditions = new Conditions(fibItem1.BlankList[0], compOp);
			IfStatement ifStatement = new IfStatement(conditions);
			ProcessLineList ifList = new ProcessLineList(ifStatement);

			// create process line list from for each statement ('For Each record in record list')
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			Record record = new Record("record");
			ForEachRecordStatement statement = new ForEachRecordStatement(record, new RecordSet("record list", forms));

			ProcessLineList forEachList = new ProcessLineList(statement);

			ifList.Insert(2, forEachList);

			Assert.IsTrue(ifList.IsInsideForEach(4), "index not in For Each as expected");
		}

		[Test]
		public void ReplaceIfEnclosingForEach()
		{
			// create process line list from if statement ('If Q1:a is not blank')
			ComparisonOperator compOp = HybridOperator.List[HybridOperator.Ops.isNotBlank];
			Conditions conditions = new Conditions(fibItem1.BlankList[0], compOp);
			IfStatement ifStatement = new IfStatement(conditions);
			ProcessLineList ifList = new ProcessLineList(ifStatement);

			// create process line list from for each statement ('For Each record in record list')
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			Record record = new Record("record");
			ProcessStatement statement = new ForEachRecordStatement(record, new RecordSet("record list", forms));

			ProcessLineList forEachList = new ProcessLineList(statement);

			ifList.Insert(2, forEachList);

			Assert.IsTrue(ifList.IsInsideForEach(4), "index not in For Each as expected");

			// verify that index is still seen as inside For Each after replacement
			ifList.Replace(0, ifStatement);
			Assert.IsTrue(ifList.IsInsideForEach(4), "index not in For Each as expected");
		}

		[Test]
		public void IsInsideForEach()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			Record record = new Record("record");
			ForEachRecordStatement statement = new ForEachRecordStatement(record, new RecordSet("record list", forms));

			ProcessLineList list = new ProcessLineList(statement);

			Assert.IsTrue(!list.IsInsideForEach(0), "index unexpectedly in For Each");
			Assert.IsTrue(list.IsInsideForEach(2), "index not in For Each as expected");
		}

		[Test]
		public void GetEnclosingForEach()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			Record record = new Record("record");
			ForEachRecordStatement statement = new ForEachRecordStatement(record, new RecordSet("record list", forms));

			ProcessLineList list = new ProcessLineList(statement);

			Assert.AreEqual(statement, list.GetEnclosingForEachStatement(2));
		}

		[Test]
		public void EnclosingForEachNested()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));

			Record record1 = new Record("record 1");
			ForEachRecordStatement statement1 = new ForEachRecordStatement(record1, new RecordSet("record list 1", forms));
			
			Record record2 = new Record("record 2");
			ForEachRecordStatement statement2 = new ForEachRecordStatement(record2, new RecordSet("record list 2", forms));

			ProcessLineList list1 = new ProcessLineList(statement1);
			ProcessLineList list2 = new ProcessLineList(statement2);

			list1.Insert(2, list2);

			int i = 0;
			Assert.AreEqual("For Each record 1 in record list 1", list1[i++].ToString());
			Assert.AreEqual("(", list1[i++].ToString());
			Assert.AreEqual("For Each record 2 in record list 2", list1[i++].ToString());
			Assert.AreEqual("(", list1[i++].ToString());
			Assert.AreEqual(")", list1[i++].ToString());
			Assert.AreEqual(")", list1[i++].ToString());

			Assert.AreEqual(statement1, list1.GetEnclosingForEachStatement(2));
			Assert.AreEqual(statement1, list1.GetEnclosingForEachStatement(3));
			Assert.AreEqual(statement1, list1.GetEnclosingForEachStatement(5));

			Assert.AreEqual(statement2, list1.GetEnclosingForEachStatement(4));
		}

		[Test]
		public void GetEnclosingForEachStatements()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));

			Record record1 = new Record("record 1");
			ForEachRecordStatement statement1 = new ForEachRecordStatement(record1, new RecordSet("record list 1", forms));

			Record record2 = new Record("record 2");
			ForEachRecordStatement statement2 = new ForEachRecordStatement(record2, new RecordSet("record list 2", forms));

			ProcessLineList list1 = new ProcessLineList(statement1);
			ProcessLineList list2 = new ProcessLineList(statement2);

			list1.Insert(2, list2);

			int i = 0;
			Assert.AreEqual("For Each record 1 in record list 1", list1[i++].ToString());
			Assert.AreEqual("(", list1[i++].ToString());
			Assert.AreEqual("For Each record 2 in record list 2", list1[i++].ToString());
			Assert.AreEqual("(", list1[i++].ToString());
			Assert.AreEqual(")", list1[i++].ToString());
			Assert.AreEqual(")", list1[i++].ToString());

			ProcessStatementList statementList = list1.GetEnclosingForEachStatements(4);
			Assert.AreEqual(statement1, statementList[0]);
			Assert.AreEqual(statement2, statementList[1]);
		}

		[Test]
		public void ForEachIf()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));

			Record record1 = new Record("record 1");
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record1, new RecordSet("record list 1", forms));
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);

			// create process line list from if statement ('If Q1:a is not blank')
			ComparisonOperator compOp = HybridOperator.List[HybridOperator.Ops.isNotBlank];
			Conditions conditions = new Conditions(fibItem1.BlankList[0], compOp);
			IfStatement ifStatement = new IfStatement(conditions);
			ProcessLineList ifLines = new ProcessLineList(ifStatement);

			forEachLines.Insert(2, ifLines);

			int i = 0;
			Assert.AreEqual("For Each record 1 in record list 1", forEachLines[i++].ToString());
			Assert.AreEqual("(", forEachLines[i++].ToString());
			Assert.AreEqual("If Form 1:Q1:a is not blank", forEachLines[i++].ToString());
			Assert.AreEqual("(", forEachLines[i++].ToString());
			Assert.AreEqual(")", forEachLines[i++].ToString());
			Assert.AreEqual(")", forEachLines[i++].ToString());

			Assert.AreEqual(forEachStatement, forEachLines.GetEnclosingForEachStatement(2));
			Assert.AreEqual(forEachStatement, forEachLines.GetEnclosingForEachStatement(4));
			Assert.AreEqual(forEachStatement, forEachLines.GetEnclosingForEachStatement(5));
		}

		[Test]
		public void ForEachIfForEach()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));

			Record record1 = new Record("record 1");
			ForEachRecordStatement forEachStatement1 = new ForEachRecordStatement(record1, new RecordSet("record list 1", forms));
			ProcessLineList forEachLines1 = new ProcessLineList(forEachStatement1);

			// create process line list from if statement ('If Q1:a is not blank')
			ComparisonOperator compOp = HybridOperator.List[HybridOperator.Ops.isNotBlank];
			Conditions conditions = new Conditions(fibItem1.BlankList[0], compOp);
			IfStatement ifStatement = new IfStatement(conditions);
			ProcessLineList ifLines = new ProcessLineList(ifStatement);

			Record record2 = new Record("record 2");
			ForEachRecordStatement forEachStatement2 = new ForEachRecordStatement(record2, new RecordSet("record list 2", forms));
			ProcessLineList forEachLines2 = new ProcessLineList(forEachStatement2);

			forEachLines1.Insert(2, ifLines);
			forEachLines1.Insert(4, forEachLines2);

			int i = 0;
			Assert.AreEqual("For Each record 1 in record list 1", forEachLines1[i++].ToString());
			Assert.AreEqual("(", forEachLines1[i++].ToString());
			Assert.AreEqual("If Form 1:Q1:a is not blank", forEachLines1[i++].ToString());
			Assert.AreEqual("(", forEachLines1[i++].ToString());
			Assert.AreEqual("For Each record 2 in record list 2", forEachLines1[i++].ToString());
			Assert.AreEqual("(", forEachLines1[i++].ToString());
			Assert.AreEqual(")", forEachLines1[i++].ToString());
			Assert.AreEqual(")", forEachLines1[i++].ToString());
			Assert.AreEqual(")", forEachLines1[i++].ToString());

			Assert.AreEqual(forEachStatement1, forEachLines1.GetEnclosingForEachStatement(2));
			Assert.AreEqual(forEachStatement1, forEachLines1.GetEnclosingForEachStatement(4));
			Assert.AreEqual(forEachStatement1, forEachLines1.GetEnclosingForEachStatement(5));
			Assert.AreEqual(forEachStatement1, forEachLines1.GetEnclosingForEachStatement(7));

			Assert.AreEqual(forEachStatement2, forEachLines1.GetEnclosingForEachStatement(6));
		}

		[Test]
		public void Add() 
		{ 
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from show statement
			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement1 = new ShowDocumentStatement(doc1);
			ProcessLine showLine1 = new ShowDocumentLine(showStatement1);

			// create process line 2 from show statement
			Document doc2 = new Document("Document 2");
			ShowDocumentStatement showStatement2 = new ShowDocumentStatement(doc2);
			ProcessLine showLine2 = new ShowDocumentLine(showStatement2);

			// add show line 1 to list
			lineList.Add(showLine1);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual(0, lineList.IndexOf(showLine1));

			// add show line 2 to list
			lineList.Add(showLine2);

			Assert.AreEqual(2, lineList.Count);
			Assert.AreEqual(1, lineList.IndexOf(showLine2));
			Assert.AreEqual(showLine1, lineList[0]);
			Assert.AreEqual(showLine2, lineList[1]);
		}

		[Test]
		public void AddList() 
		{ 
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from show statement
			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement1 = new ShowDocumentStatement(doc1);
			ProcessLine showLine1 = new ShowDocumentLine(showStatement1);

			// add show line 1 to list
			lineList.Add(showLine1);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual(0, lineList.IndexOf(showLine1));

			// create process line list from if statement
			Field field = new Field("Q1:a");
			ComparisonOperator compOp = StringOperator.List[StringOperator.Ops.beginsWith];
			string expressionString = "S";
			Conditions conditions = new Conditions(field, compOp, new Expression(expressionString));
			IfStatement ifStatement = new IfStatement(conditions);

			ProcessLineList ifList = new ProcessLineList(ifStatement);

			// add if list to main list
			lineList.Add(ifList);

			Assert.AreEqual(4, lineList.Count);
			Assert.AreEqual(showLine1, lineList[0]);
			Assert.AreEqual("If Q1:a begins with \"S\"", lineList[1].ToString());
			Assert.AreEqual("(", lineList[2].ToString());
			Assert.AreEqual(")", lineList[3].ToString());
		}

		[Test]
		public void RemoveLine() 
		{ 
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from show statement
			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement1 = new ShowDocumentStatement(doc1);
			ProcessLine showLine1 = new ShowDocumentLine(showStatement1);

			// create process line 2 from show statement
			Document doc2 = new Document("Document 2");
			ShowDocumentStatement showStatement2 = new ShowDocumentStatement(doc2);
			ProcessLine showLine2 = new ShowDocumentLine(showStatement2);

			// add show lines 1 and 2 to list
			lineList.Add(showLine1);
			lineList.Add(showLine2);

			Assert.AreEqual(2, lineList.Count);

			// remove first line from list
			lineList.Remove(showLine1);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual(showLine2, lineList[0]);

			// remove remaining line from list
			lineList.Remove(showLine2);

			Assert.AreEqual(0, lineList.Count);
		}

		[Test]
		public void RemoveIfLines() 
		{ 
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from show statement
			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement1 = new ShowDocumentStatement(doc1);
			ProcessLine showLine1 = new ShowDocumentLine(showStatement1);

			// add show line 1 to list
			lineList.Add(showLine1);

			Assert.AreEqual(1, lineList.Count);

			// create process line list from if statement
			Field field = new Field("Q1:a");
			ComparisonOperator compOp = StringOperator.List[StringOperator.Ops.beginsWith];

			string expressionString = "S";
			Conditions conditions = new Conditions(field, compOp, new Expression(expressionString));
			IfStatement ifStatement = new IfStatement(conditions);

			ProcessLineList ifList = new ProcessLineList(ifStatement);

			// insert if list at beginning of main list
			lineList.Insert(0, ifList);

			Assert.AreEqual(4, lineList.Count);
			Assert.AreEqual("If Q1:a begins with \"S\"", lineList[0].ToString());
			Assert.AreEqual("(", lineList[1].ToString());
			Assert.AreEqual(")", lineList[2].ToString());
			Assert.AreEqual(showLine1, lineList[3]);

			// remove all if statement lines from list
			lineList.Remove(0, (((ProcessLine)lineList[0]).Statement));

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual(showLine1, lineList[0]);
		}

		[Test]
		public void Insert() 
		{ 
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from show statement
			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement1 = new ShowDocumentStatement(doc1);
			ProcessLine showLine1 = new ShowDocumentLine(showStatement1);

			// create process line 2 from show statement
			Document doc2 = new Document("Document 2");
			ShowDocumentStatement showStatement2 = new ShowDocumentStatement(doc2);
			ProcessLine showLine2 = new ShowDocumentLine(showStatement2);

			// create process line 3 from show statement
			Document doc3 = new Document("Document 3");
			ShowDocumentStatement showStatement3 = new ShowDocumentStatement(doc3);
			ProcessLine showLine3 = new ShowDocumentLine(showStatement3);

			// add show lines 1 and 2 to list
			lineList.Add(showLine1);
			lineList.Add(showLine2);

			Assert.AreEqual(2, lineList.Count);

			// insert third line into middle of list
			lineList.Insert(1, showLine3);

			Assert.AreEqual(3, lineList.Count);
			Assert.AreEqual(showLine1, lineList[0]);
			Assert.AreEqual(showLine3, lineList[1]);
			Assert.AreEqual(showLine2, lineList[2]);
		}

		[Test]
		public void DragDrop() 
		{ 
			Process process = new Process("Process 1");

			// create process line 1 from show statement
			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement1 = new ShowDocumentStatement(doc1);
			ProcessLine line1 = new ShowDocumentLine(showStatement1);

			// create process line 2 from show statement
			Document doc2 = new Document("Document 2");
			ShowDocumentStatement showStatement2 = new ShowDocumentStatement(doc2);
			ProcessLine line2 = new ShowDocumentLine(showStatement2);

			// create process line 3 from show statement
			Document doc3 = new Document("Document 3");
			ShowDocumentStatement showStatement3 = new ShowDocumentStatement(doc3);
			ProcessLine line3 = new ShowDocumentLine(showStatement3);

			// add process lines to process
			process.Lines.Add(line1);
			process.Lines.Add(line2);
			process.Lines.Add(line3);

			Assert.AreEqual(3, process.Lines.Count);
			Assert.AreEqual(showStatement1.ToString(), process.Lines[0].ToString());
			Assert.AreEqual(showStatement2.ToString(), process.Lines[1].ToString());
			Assert.AreEqual(showStatement3.ToString(), process.Lines[2].ToString());

			// drag and drop line 1 onto line 3
			process.Lines.DragDrop (0, 2);

			Assert.AreEqual(3, process.Lines.Count);
			Assert.AreEqual(showStatement2.ToString(), process.Lines[0].ToString());
			Assert.AreEqual(showStatement1.ToString(), process.Lines[1].ToString());
			Assert.AreEqual(showStatement3.ToString(), process.Lines[2].ToString());

			// drag and drop line 3 onto line 1
			process.Lines.DragDrop (2, 0);

			Assert.AreEqual(3, process.Lines.Count);
			Assert.AreEqual(showStatement3.ToString(), process.Lines[0].ToString());
			Assert.AreEqual(showStatement2.ToString(), process.Lines[1].ToString());
			Assert.AreEqual(showStatement1.ToString(), process.Lines[2].ToString());

			// drag and drop line 2 at end of list
			process.Lines.DragDrop (1, -1);

			Assert.AreEqual(3, process.Lines.Count);
			Assert.AreEqual(showStatement3.ToString(), process.Lines[0].ToString());
			Assert.AreEqual(showStatement1.ToString(), process.Lines[1].ToString());
			Assert.AreEqual(showStatement2.ToString(), process.Lines[2].ToString());
		}

		[Test]
		public void InsertList() 
		{ 
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from show statement
			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement1 = new ShowDocumentStatement(doc1);
			ProcessLine showLine1 = new ShowDocumentLine(showStatement1);

			// add show line 1 to list
			lineList.Add(showLine1);

			Assert.AreEqual(1, lineList.Count);

			// create process line list from if statement
			Field field = new Field("Q1:a");
			ComparisonOperator compOp = StringOperator.List[StringOperator.Ops.beginsWith];
			string expressionString = "S";
			Conditions conditions = new Conditions(field, compOp, new Expression(expressionString));
			IfStatement ifStatement = new IfStatement(conditions);

			ProcessLineList ifList = new ProcessLineList(ifStatement);

			// insert if list at beginning of main list
			lineList.Insert(0, ifList);

			Assert.AreEqual(4, lineList.Count);
			Assert.AreEqual("If Q1:a begins with \"S\"", lineList[0].ToString());
			Assert.AreEqual("(", lineList[1].ToString());
			Assert.AreEqual(")", lineList[2].ToString());
			Assert.AreEqual(showStatement1.ToString(), lineList[3].ToString());
		}

		[Test]
		public void NestedIf() 
		{ 
			// create main process line list and associate statement list with it
			ProcessLineList mainLineList = new ProcessLineList();

			// create process line list from first if statement
			IfStatement ifStatement1 = new IfStatement(new Conditions(new Field("Q1:a"), StringOperator.List[StringOperator.Ops.beginsWith], new Expression("S")));
			ProcessLineList ifList1 = new ProcessLineList(ifStatement1);

			// create process line list from second if statement
			IfStatement ifStatement2 = new IfStatement(new Conditions(new Field("Q1:b"), StringOperator.List[StringOperator.Ops.endsWith], new Expression("e")));
			ProcessLineList ifList2 = new ProcessLineList(ifStatement2);

			// create process line list from show statement
			ShowDocumentStatement showStatement = new ShowDocumentStatement(new Document("Document 1"));
			ProcessLineList showList = new ProcessLineList(showStatement);

			// add first if statement list to main line list
			mainLineList.Add (ifList1);

			Assert.AreEqual(3, mainLineList.Count);
			Assert.AreEqual("If Q1:a begins with \"S\"", mainLineList[0].ToString());
			Assert.AreEqual("(", mainLineList[1].ToString());
			Assert.AreEqual(")", mainLineList[2].ToString());

			// insert second if statement line list before ')' in main list
			mainLineList.Insert(2, ifList2);

			Assert.AreEqual(6, mainLineList.Count);
			Assert.AreEqual("If Q1:b ends with \"e\"", mainLineList[2].ToString());
			Assert.AreEqual("(", mainLineList[3].ToString());
			Assert.AreEqual(")", mainLineList[4].ToString());

			// insert show statement line list before second ')' in main list
			mainLineList.Insert(4, showList);

			Assert.AreEqual(7, mainLineList.Count);
			Assert.AreEqual(showStatement.ToString(), mainLineList[4].ToString());

			mainLineList.SetIndentLevels();

			Assert.AreEqual(0, mainLineList[0].IndentLevel);
			Assert.AreEqual(0, mainLineList[1].IndentLevel);
			Assert.AreEqual(1, mainLineList[2].IndentLevel);
			Assert.AreEqual(1, mainLineList[3].IndentLevel);
			Assert.AreEqual(2, mainLineList[4].IndentLevel);
			Assert.AreEqual(1, mainLineList[5].IndentLevel);
			Assert.AreEqual(0, mainLineList[6].IndentLevel);
		}

		[Test]
		public void ReplaceShowDocumentLine() 
		{ 
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from show statement
			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement1 = new ShowDocumentStatement(doc1);
			ProcessLine showLine1 = new ShowDocumentLine(showStatement1);

			// create second show statement
			Document doc2 = new Document("Document 2");
			ShowDocumentStatement showStatement2 = new ShowDocumentStatement(doc2);

			// add show line 1 to list
			lineList.Add(showLine1);

			Assert.AreEqual(1, lineList.Count);

			// remove first line in list with line created from second show statement
			lineList.Replace(0, showStatement2);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual(showStatement2.ToString(), lineList[0].ToString());
		}

		[Test]
		public void ReplaceIfLine()
		{
			string xmlString;

			// create empty line list
			ProcessLineList lineList = new ProcessLineList();

			// create first if statement (without otherwise clause)
			Field field = new Field("Q1:a");
			ComparisonOperator compOp = StringOperator.List[StringOperator.Ops.beginsWith];
			string expressionString = "S";
			Conditions conditions = new Conditions(field, compOp, new Expression(expressionString));
			IfStatement statement1 = new IfStatement(conditions);

			// add "if" lines to main list
			ProcessLineList list1 = new ProcessLineList(statement1);
			lineList.Add(list1);

			Assert.AreEqual(3, lineList.Count);

			xmlString =
				"<if>\r\n" +
				"<conditions>\r\n" +
				"<beginsWith field=\"Q1:a\">\r\n" +
				"<string value=\"S\"/>\r\n" +
				"</beginsWith>\r\n" +
				"</conditions>\r\n" +
				"<trueSet>\r\n" +
				"</trueSet>\r\n" +
				"</if>\r\n";

			Assert.AreEqual(xmlString, lineList.ToXml());

			for (int i = 0; i < 3; i++)
			{
				Assert.AreEqual(statement1, lineList[i].Group);
			}

			// create second if statement (with otherwise clause)
			Field field2 = new Field("Q2:b");
			ComparisonOperator compOp2 = StringOperator.List[StringOperator.Ops.equals];
			string expressionString2 = "Fred";
			Conditions conditions2 = new Conditions(field2, compOp2, new Expression(expressionString2));
			IfStatement statement2 = new IfStatement(conditions2, true);

			// replace "if" lines in list with lines created from second if statement
			lineList.Replace(0, statement2);

			Assert.AreEqual(6, lineList.Count);
			Assert.AreEqual("If Q2:b equals \"Fred\"", lineList[0].ToString());

			xmlString =
				"<if>\r\n" +
				"<conditions>\r\n" +
				"<equals field=\"Q2:b\">\r\n" +
				"<string value=\"Fred\"/>\r\n" +
				"</equals>\r\n" +
				"</conditions>\r\n" +
				"<trueSet>\r\n" +
				"</trueSet>\r\n" +
				"<falseSet>\r\n" +
				"</falseSet>\r\n" +
				"</if>\r\n";

			Assert.AreEqual(xmlString, lineList.ToXml());

			for (int i = 0; i < 6; i++)
			{
				Assert.AreEqual(statement2, lineList[i].Group, "line index = " + i.ToString());
			}

			// create third if statement (without otherwise clause)
			Field field3 = new Field("Q3:c");
			ComparisonOperator compOp3 = StringOperator.List[StringOperator.Ops.doesNotEqual];
			string expressionString3 = "Barney";
			Conditions conditions3 = new Conditions(field3, compOp3, new Expression(expressionString3));
			IfStatement statement3 = new IfStatement(conditions3);

			// replace "if" lines in list with lines created from third if statement
			lineList.Replace(0, statement3);

			Assert.AreEqual(3, lineList.Count);
			Assert.AreEqual("If Q3:c does not equal \"Barney\"", lineList[0].ToString());

			xmlString =
				"<if>\r\n" +
				"<conditions>\r\n" +
				"<doesNotEqual field=\"Q3:c\">\r\n" +
				"<string value=\"Barney\"/>\r\n" +
				"</doesNotEqual>\r\n" +
				"</conditions>\r\n" +
				"<trueSet>\r\n" +
				"</trueSet>\r\n" +
				"</if>\r\n";

			Assert.AreEqual(xmlString, lineList.ToXml());

			for (int i = 0; i < 3; i++)
			{
				Assert.AreEqual(statement3, lineList[i].Group);
			}

		}

		[Test]
		public void ReplaceSendLine()
		{
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from send statement
			SendStatement statement1 = new SendStatement();
			statement1.AddressTo.Text = "doug@carlston.net";
			statement1.SendBody = new SendEmailBody("Some body text.");
			ProcessLine line1 = new SendLine(statement1);

			// create second show statement
			SendStatement statement2 = new SendStatement();
			statement2.AddressTo.Text = "jdf@jdftech.com";
			statement2.SendBody = new SendEmailBody("Some other body text.");

			// add show line 1 to list
			lineList.Add(line1);

			Assert.AreEqual(1, lineList.Count);

			// remove first line in list with line created from second show statement
			lineList.Replace(0, statement2);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual("Send Email to \"jdf@jdftech.com\"", lineList[0].ToString());
		}

		[Test]
		public void ReplaceSetLine()
		{
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from set statement
			SetStatement statement1 = new SetStatement();
			statement1.Variable = new Variable("a variable");
			statement1.Expression = new Expression("a value");
			ProcessLine line1 = new SetLine(statement1);

			// create second set statement
			SetStatement statement2 = new SetStatement();
			statement2.Variable = new Variable("variable 2");
			statement2.Expression = new Expression("a different value");

			// add show line 1 to list
			lineList.Add(line1);

			Assert.AreEqual(1, lineList.Count);

			// remove first line in list with line created from second show statement
			lineList.Replace(0, statement2);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual("Set variable 2 to \"a different value\"", lineList[0].ToString());
		}

		[Test]
		public void ReplaceAppendLine()
		{
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line list 1 from statement
			Document doc1 = new Document("Document 1");
			Document doc2 = new Document("Document 2");
			AppendStatement statement = new AppendStatement(doc1, doc2);
			ProcessLine line1 = new AppendLine(statement);

			// create second Append statement
			AppendStatement statement2 = new AppendStatement(doc2, doc1);

			// add show line 1 to list
			lineList.Add(line1);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual("Append Document 1 to Document 2", lineList[0].ToString());

			// remove first line in list with line created from second Append statement
			lineList.Replace(0, statement2);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual("Append Document 2 to Document 1", lineList[0].ToString());
		}

		[Test]
		public void ReplaceSkipToLine()
		{
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line list from Skip statement
			SkipToStatement statement1 = new SkipToStatement();
			ProcessLine line1 = new SkipToLine(statement1);

			// create second Skip statement
			SkipToStatement statement2 = new SkipToStatement();

			// add show line 1 to list
			lineList.Add(line1);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual(statement1, lineList[0].Statement);

			// replace first line in list with line created from second statement
			lineList.Replace(0, statement2);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual(statement2, lineList[0].Statement);
		}

		[Test]
		public void ReplaceGetLine()
		{
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from Get statement
			FormList forms1 = new FormList();
			forms1.Add(form);
			GetStatement statement1 = new GetStatement(new RecordSet("record list", forms1));
			ProcessLine line1 = new GetLine(statement1);

			// create second line from a Get statement
			FormList forms2 = new FormList();
			forms2.Add(Project.Current.AddForm());
			GetStatement statement2 = new GetStatement(new RecordSet("record list 2", forms2));
			ProcessLine line2 = new GetLine(statement2);

			// add line 1 to list
			lineList.Add(line1);

			Assert.AreEqual(1, lineList.Count);

			// remove first line in list with line created from second show statement
			lineList.Replace(0, statement2);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual("Get record list 2 from Form 2", lineList[0].ToString());
		}

		[Test]
		public void ReplaceDeleteLine()
		{
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from Delete statement
			IForm form1 = Project.Current.AddForm();
			DeleteStatement statement1 = new DeleteStatement();
			statement1.Form = form1;
			ProcessLine line1 = new DeleteLine(statement1);

			// create second line from a Delete statement
			IForm form2 = Project.Current.AddForm();
			DeleteStatement statement2 = new DeleteStatement();
			statement2.Form = form2;
			ProcessLine line2 = new DeleteLine(statement2);

			// add line 1 to list
			lineList.Add(line1);

			Assert.AreEqual(1, lineList.Count);

			// remove first line in list with line created from second show statement
			lineList.Replace(0, statement2);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual("Delete records from Form 3", lineList[0].ToString());
		}

		[Test]
		public void ReplaceForEachRecordLine()
		{
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from ForEach statement
			FormList forms1 = new FormList();
			forms1.Add(new Form("Form 1"));
			ForEachRecordStatement statement1 = new ForEachRecordStatement(new Record("record 1"), new RecordSet("record list 1", forms1));
			ProcessLine line1 = new ForEachRecordLine(statement1);

			// create second line from a ForEach statement
			FormList forms2 = new FormList();
			forms2.Add(new Form("Form 2"));
			ForEachRecordStatement statement2 = new ForEachRecordStatement(new Record("record 2"), new RecordSet("record list 2", forms2));
			ProcessLine line2 = new ForEachRecordLine(statement2);

			// add line 1 to list
			lineList.Add(line1);

			Assert.AreEqual(1, lineList.Count);

			// remove first line in list with line created from second show statement
			lineList.Replace(0, statement2);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual("For Each record 2 in record list 2", lineList[0].ToString());
		}

#if ForEachQuestionStatement
		[Test]
		public void ReplaceForEachQuestionLine()
		{
			// create empty process line list
			ProcessLineList lineList = new ProcessLineList();

			// create process line 1 from ForEach statement
			FormList forms1 = new FormList();
			forms1.Add(new Form("Form 1"));
			ForEachQuestionStatement statement1 = new ForEachQuestionStatement();
			ProcessLine line1 = new ForEachQuestionLine(statement1);

			// create second line from a ForEach statement
			FormList forms2 = new FormList();
			forms2.Add(new Form("Form 2"));
			ForEachQuestionStatement statement2 = new ForEachQuestionStatement();
			ProcessLine line2 = new ForEachQuestionLine(statement2);

			// add line 1 to list
			lineList.Add(line1);

			Assert.AreEqual(1, lineList.Count);

			// remove first line in list with line created from second show statement
			lineList.Replace(0, statement2);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual("For Each Multiple Choice Question", lineList[0].ToString());
		}
#endif

		[Test]
		public void ReplaceCommentLine()
		{
			ProcessLineList lineList = new ProcessLineList();

			CommentStatement statement1 = new CommentStatement("First comment.");
			ProcessLine line1 = new CommentLine(statement1);

			CommentStatement statement2 = new CommentStatement("Second comment.");
			ProcessLine line2 = new CommentLine(statement2);

			lineList.Add(line1);

			Assert.AreEqual(1, lineList.Count);

			lineList.Replace(0, statement2);

			Assert.AreEqual(1, lineList.Count);
			Assert.AreEqual("Second comment.", lineList[0].ToString());
		}

		[Test]
		public void Extract() 
		{ 
			// create main process line list
			ProcessLineList mainLineList = new ProcessLineList();

			// create process line list from first if statement
			IfStatement ifStatement1 = new IfStatement(new Conditions(new Field("Q1:a"), StringOperator.List[StringOperator.Ops.beginsWith], new Expression("S")));
			ProcessLineList ifList1 = new ProcessLineList(ifStatement1);

			// create process line list from second if statement
			IfStatement ifStatement2 = new IfStatement(new Conditions(new Field("Q1:b"), StringOperator.List[StringOperator.Ops.endsWith], new Expression("e")));
			ProcessLineList ifList2 = new ProcessLineList(ifStatement2);

			// create process line list from show statement
			ShowDocumentStatement showStatement1 = new ShowDocumentStatement(new Document("Document 1"));
			ProcessLineList showList1 = new ProcessLineList(showStatement1);

			// add first if statement list to main line list
			mainLineList.Add (ifList1);

			Assert.AreEqual(3, mainLineList.Count);
			Assert.AreEqual("If Q1:a begins with \"S\"", mainLineList[0].ToString());
			Assert.AreEqual("(", mainLineList[1].ToString());
			Assert.AreEqual(")", mainLineList[2].ToString());

			// insert second if statement line list before ')' in main list
			mainLineList.Insert(2, ifList2);

			Assert.AreEqual(6, mainLineList.Count);
			Assert.AreEqual("If Q1:b ends with \"e\"", mainLineList[2].ToString());
			Assert.AreEqual("(", mainLineList[3].ToString());
			Assert.AreEqual(")", mainLineList[4].ToString());

			// insert show statement line list before second ')' in main list
			mainLineList.Insert(4, showList1);

			Assert.AreEqual(7, mainLineList.Count);
			Assert.AreEqual(showStatement1.ToString(), mainLineList[4].ToString());

			// create process line list from show line in main list
			ProcessLineList showList2 = new ProcessLineList(4, mainLineList);

			Assert.AreEqual(1, showList2.Count);
			Assert.AreEqual(showStatement1.ToString(), showList2[0].ToString());

			// create process line list from inner if lines in main list
			ProcessLineList ifList3 = new ProcessLineList(2, mainLineList);

			Assert.AreEqual(4, ifList3.Count);
			Assert.AreEqual("If Q1:b ends with \"e\"", ifList3[0].ToString());
			Assert.AreEqual("(", ifList3[1].ToString());
			Assert.AreEqual(showStatement1.ToString(), ifList3[2].ToString());
			Assert.AreEqual(")", ifList3[3].ToString());
		}

		[Test]
		public void Copy()
		{
			ProcessLineList lineList = new ProcessLineList();

			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement1 = new ShowDocumentStatement(doc1);
			ProcessLine showLine1 = new ShowDocumentLine(showStatement1);

			lineList.Add(showLine1);

			ProcessLineList lineListCopy = lineList.Copy();

			Assert.AreEqual(1, lineListCopy.Count);
			Assert.IsTrue(lineListCopy[0] is ShowDocumentLine);
			Assert.IsFalse(lineList[0] == lineListCopy[0]);
			Assert.IsFalse(showStatement1 == ((ShowDocumentLine)lineListCopy[0]).Statement);
			Assert.AreSame(doc1, ((ShowDocumentStatement)((ShowDocumentLine)lineListCopy[0]).Statement).Document);
		}

		[Test]
		public void CopyIfLines()
		{
			ProcessLineList lineList = new ProcessLineList();

			Field field = new Field("Q1:a");
			ComparisonOperator compOp = HybridOperator.List[HybridOperator.Ops.isNotBlank];
			Conditions conditions = new Conditions(field, compOp);
			IfStatement ifStatement = new IfStatement(conditions);
			ProcessLineList ifLines = new ProcessLineList(ifStatement);

			lineList.Add(ifLines);

			Document doc1 = new Document("Document 1");
			ShowDocumentStatement showStatement = new ShowDocumentStatement(doc1);
			ProcessLine showLine = new ShowDocumentLine(showStatement);

			lineList.Insert(2, showLine);

			ProcessLineList lineListCopy = lineList.Copy();

			Assert.AreEqual(4, lineListCopy.Count);
			Assert.IsTrue(lineListCopy[0] is IfLine);
			Assert.IsTrue(lineListCopy[2] is ShowDocumentLine);
			Assert.IsFalse(lineList[0] == lineListCopy[0]);
			Assert.IsFalse(showStatement == ((IfLine)lineListCopy[0]).Statement);
			Assert.AreSame(field, ((Condition)((IfStatement)((IfLine)lineListCopy[0]).Statement).Conditions[0]).Field);

			Assert.IsFalse(lineList[2] == lineListCopy[2]);
			Assert.IsFalse(showStatement == ((ShowDocumentLine)lineListCopy[2]).Statement);
			Assert.AreSame(doc1, ((ShowDocumentStatement)((ShowDocumentLine)lineListCopy[2]).Statement).Document);
		}

		[Test]
		public void LineGroupIndexes()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));

			Record record1 = new Record("record 1");
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record1, new RecordSet("record list 1", forms));
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);

			// create process line list from if statement ('If Q1:a is not blank')
			ComparisonOperator compOp = HybridOperator.List[HybridOperator.Ops.isNotBlank];
			Conditions conditions = new Conditions(fibItem1.BlankList[0], compOp);
			IfStatement ifStatement = new IfStatement(conditions);
			ProcessLineList ifLines = new ProcessLineList(ifStatement);
			forEachLines.Insert(2, ifLines);

			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new Variable("a variable");
			setStatement.Expression = new Expression("10");
			ProcessLineList setLines = new ProcessLineList(setStatement);
			forEachLines.Insert(4, setLines);

			ProcessLine line = forEachLines[0];
			Assert.AreEqual(0, forEachLines.LineGroupStartIndex(line));
			Assert.AreEqual(6, forEachLines.LineGroupEndIndex(line));

			line = ifLines[0];
			Assert.AreEqual(2, forEachLines.LineGroupStartIndex(line));
			Assert.AreEqual(5, forEachLines.LineGroupEndIndex(line));

			line = setLines[0];
			Assert.AreEqual(4, forEachLines.LineGroupStartIndex(line));
			Assert.AreEqual(4, forEachLines.LineGroupEndIndex(line));
		}

		[Test]
		public void RemoveOtherwiseFromOuterIf()
		{
			// create main process line list and associate statement list with it
			ProcessLineList mainLineList = new ProcessLineList();

			// create process line list from first if statement
			IfStatement ifStatement1 = new IfStatement(new Conditions(new Field("Q1:a"), StringOperator.List[StringOperator.Ops.beginsWith], new Expression("S")));
			ifStatement1.HasOtherwise = true;
			ProcessLineList ifList1 = new ProcessLineList(ifStatement1);

			// create process line list from second if statement
			IfStatement ifStatement2 = new IfStatement(new Conditions(new Field("Q1:b"), StringOperator.List[StringOperator.Ops.endsWith], new Expression("e")));
			ProcessLineList ifList2 = new ProcessLineList(ifStatement2);

			// add first if statement list to main line list
			mainLineList.Add(ifList1);

			// insert second if statement line list before ')' in main list
			mainLineList.Insert(2, ifList2);

			Assert.AreEqual(9, mainLineList.Count);
			Assert.AreEqual("If Q1:a begins with \"S\"", mainLineList[0].ToString());
			Assert.AreEqual("(", mainLineList[1].ToString());
			Assert.AreEqual("If Q1:b ends with \"e\"", mainLineList[2].ToString());
			Assert.AreEqual("(", mainLineList[3].ToString());
			Assert.AreEqual(")", mainLineList[4].ToString());
			Assert.AreEqual(")", mainLineList[5].ToString());
			Assert.AreEqual("Otherwise", mainLineList[6].ToString());
			Assert.AreEqual("(", mainLineList[7].ToString());
			Assert.AreEqual(")", mainLineList[8].ToString());
			Assert.IsTrue(ifStatement1.HasOtherwise);

			ifStatement1 = new IfStatement(new Conditions(new Field("Q1:a"), StringOperator.List[StringOperator.Ops.beginsWith], new Expression("S")));
			ifStatement1.HasOtherwise = false; 

			mainLineList.Replace(0, ifStatement1);

			Assert.IsFalse(ifStatement1.HasOtherwise);
			Assert.AreEqual("If Q1:a begins with \"S\"", mainLineList[0].ToString());
			Assert.AreEqual("(", mainLineList[1].ToString());
			Assert.AreEqual("If Q1:b ends with \"e\"", mainLineList[2].ToString());
			Assert.AreEqual("(", mainLineList[3].ToString());
			Assert.AreEqual(")", mainLineList[4].ToString());
			Assert.AreEqual(")", mainLineList[5].ToString());
			Assert.AreEqual(6, mainLineList.Count);
		}

	}
}
