using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing the simple GET statement with FOR EACH statements
	/// </summary>
	[TestFixture]
	public class SimpleGetForEachTest
	{
		private IForm form;
		private Process process;
		private ProcessLineList forEachLines;
		private Record record;
		private FormList forms;

		// execute this once at beginning of tests
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
		}

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

			// add new FIB item to form
			form.ItemList.Add(new FibItem());

			// create SET statement ('Set Var1 to 0')
			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new Variable("Var1");
			setStatement.Expression = new Expression("0");
			ProcessLineList setLine = new ProcessLineList(setStatement);
			process.Lines.Add(setLine);

			// create GET statement ('Get record list from Form 1')
			forms = new FormList();
			forms.Add((Form)Project.Current.FormList[0]);
			RecordSet recordList = new RecordSet("record list", forms);
			GetStatement getStatement = new GetStatement(recordList);
			process.Lines.Add(new ProcessLineList(getStatement));

			// create FOR EACH statement ('For Each record in record list')
			record = new Record("record");
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			forEachLines = new ProcessLineList(forEachStatement);

			Assert.AreEqual(3, forEachLines.Count);

		}

		[Test]
		public void GetBasicXml()
		{
			process.Lines.Add(forEachLines);

			string expectedXml =
				"<process name=\"Process 1\">\r\n" +
				"<set field=\"Var1\" arithmeticAsText=\"false\">\r\n" +
				"<string value=\"0\"/>\r\n" +
				"</set>\r\n" +
				"<get recordList=\"record list\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>\r\n" +
				"<foreach record=\"record\" recordList=\"record list\">\r\n" +
				"</foreach>\r\n" +
				"</process>\r\n";

			Assert.AreEqual(expectedXml, process.ToXml());
		}

		[Test]
		public void AddValue()
		{
			AddStatement addStatement = new AddStatement();
			addStatement.Variable = "TotalScore";
			addStatement.Value = new FieldOrLiteral("10", FieldOrLiteral.StringType.literal);
			ProcessLineList addToLine = new ProcessLineList(addStatement);
			forEachLines.Insert(2, addToLine);
			process.Lines.Add(forEachLines);

			int i = 0;
			Assert.AreEqual("Set Var1 to 0", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Add 10 to TotalScore", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			string expectedXml =
				"<process name=\"Process 1\">\r\n" +
				"<set field=\"Var1\" arithmeticAsText=\"false\">\r\n" +
				"<string value=\"0\"/>\r\n" +
				"</set>\r\n" +
				"<get recordList=\"record list\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>\r\n" +
				"<foreach record=\"record\" recordList=\"record list\">\r\n" +
				"<addTo field=\"TotalScore\">\r\n" +
				"<operand value=\"10\"/>\r\n" +
				"</addTo>\r\n" +
				"</foreach>\r\n" +
				"</process>\r\n";

			Assert.AreEqual(expectedXml, process.ToXml());
		}

		[Test]
		public void AddFibField()
		{
			AddStatement addStatement = new AddStatement();
			addStatement.Variable = "TotalScore";
			addStatement.Value = new FieldOrLiteral("Q1:a", FieldOrLiteral.StringType.field);
			ProcessLineList addToLine = new ProcessLineList(addStatement);
			forEachLines.Insert(2, addToLine);
			process.Lines.Add(forEachLines);

			int i = 0;
			Assert.AreEqual("Set Var1 to 0", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Add Form 1:Q1:a to TotalScore", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			string expectedXml =
				"<process name=\"Process 1\">\r\n" +
				"<set field=\"Var1\" arithmeticAsText=\"false\">\r\n" +
				"<string value=\"0\"/>\r\n" +
				"</set>\r\n" +
				"<get recordList=\"record list\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>\r\n" +
				"<foreach record=\"record\" recordList=\"record list\">\r\n" +
				"<addTo field=\"TotalScore\">\r\n" +
				"<operand field=\"Form 1:Q1:a\"/>\r\n" +
				"</addTo>\r\n" +
				"</foreach>\r\n" +
				"</process>\r\n";

			Assert.AreEqual(expectedXml, process.ToXml());
		}

		[Test]
		public void AddQualifiedFibField()
		{
			AddStatement addStatement = new AddStatement();
			addStatement.Variable = "TotalScore";

			QualifiedFieldList qualifiedField = new QualifiedFieldList(record, form.GetFields()["Q1:a"]);
			addStatement.Value = new FieldOrLiteral(qualifiedField);

			ProcessLineList addToLine = new ProcessLineList(addStatement);
			forEachLines.Insert(2, addToLine);
			process.Lines.Add(forEachLines);

			int i = 0;
			Assert.AreEqual("Set Var1 to 0", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Add record:Q1:a to TotalScore", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			string expectedXml =
				"<process name=\"Process 1\">\r\n" +
				"<set field=\"Var1\" arithmeticAsText=\"false\">\r\n" +
				"<string value=\"0\"/>\r\n" +
				"</set>\r\n" +
				"<get recordList=\"record list\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>\r\n" +
				"<foreach record=\"record\" recordList=\"record list\">\r\n" +
				"<addTo field=\"TotalScore\">\r\n" +
				"<operand field=\"record:Q1:a\"/>\r\n" +
				"</addTo>\r\n" +
				"</foreach>\r\n" +
				"</process>\r\n";

			Assert.AreEqual(expectedXml, process.ToXml());
		}

		[Test]
		public void RepositionQualifiedFibField()
		{
			AddStatement addStatement = new AddStatement();
			addStatement.Variable = "TotalScore";

			QualifiedFieldList qualifiedField = new QualifiedFieldList(record, form.GetFields()["Q1:a"]);
			addStatement.Value = new FieldOrLiteral(qualifiedField);

			ProcessLineList addToLine = new ProcessLineList(addStatement);
			forEachLines.Insert(2, addToLine);
			process.Lines.Add(forEachLines);

			Assert.AreEqual("Add record:Q1:a to TotalScore", process.Lines[4].ToString());

			form.ItemList.Insert(0, new FibItem());

			Assert.AreEqual("Add record:Q2:a to TotalScore", process.Lines[4].ToString());
		}

		[Test]
		public void ArithmeticLineStatus()
		{
			// create process line 'Add record:Q1:a to Var1'
			AddStatement addStatement = new AddStatement();
			addStatement.Variable = "Var1";

			QualifiedFieldList qualifiedField = new QualifiedFieldList(record, form.GetFields()["Q1:a"]);
			addStatement.Value = new FieldOrLiteral(qualifiedField);

			ProcessLineList addToLineList = new ProcessLineList(addStatement);

			// place 'Add' line within 'For Each' lines
			forEachLines.Insert(2, addToLineList);

			// add 'For Each' lines to Process
			process.Lines.Add(forEachLines);

			process.Lines.ValidateLines();

			// verify that process line is valid
			Assert.IsTrue(addToLineList[0].IsValid, "Process line is not valid");
		}

		[Test]
		public void SetLineStatus()
		{
			// add 'For Each' lines to Process
			process.Lines.Add(forEachLines);

			// create process line 'Set Variable to record:Q1:a'
			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new Variable("Variable");
			setStatement.Expression = new Expression("<<record:Q1:a>>", process.GetValidFields(4));
			ProcessLineList setLines = new ProcessLineList(setStatement);
			process.Variables.AddUnique("Variable");

			// place 'Set' line within 'For Each' lines
			process.Lines.Insert(4, setLines);

			int i = 0;
			Assert.AreEqual("Set Var1 to 0", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Set Variable to record:Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			process.Lines.ValidateLines();

			// verify that process line is valid
			Assert.IsTrue(setLines[0].IsValid, "Process line is not valid");
		}

		[Test]
		public void ForEachIfSet()
		{
			// add 'For Each' lines to Process
			process.Lines.Add(forEachLines);

			// create process line 'If record:Q1:a equals 10'
			QualifiedFieldList qualifiedField = new QualifiedFieldList(record, form.GetFields()["Q1:a"]);
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(qualifiedField, HybridOperator.List[HybridOperator.Ops.equals], new Expression("10"));
			ProcessLineList ifLines = new ProcessLineList(ifStatement);

			// place 'If' lines within 'For Each' lines
			process.Lines.Insert(4, ifLines);

			// create process line 'Set Variable to record:Q1:a'
			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new Variable("Variable");
			setStatement.Expression = new Expression("<<record:Q1:a>>", process.GetValidFields(4));
			ProcessLineList setLines = new ProcessLineList(setStatement);

			// place 'Set' line within 'If' lines
			process.Lines.Insert(6, setLines);

			int i = 0;
			Assert.AreEqual("Set Var1 to 0", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If record:Q1:a equals 10", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Set Variable to record:Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}


		[Test]
		public void ForEachIfForEachIfSet()
		{
			// add 'For Each' lines to Process
			process.Lines.Add(forEachLines);

			// create process line 'If record:Q1:a equals 10'
			QualifiedFieldList qualifiedField = new QualifiedFieldList(record, form.GetFields()["Q1:a"]);
			IfStatement ifStatement1 = new IfStatement();
			ifStatement1.Conditions = new Conditions(qualifiedField, HybridOperator.List[HybridOperator.Ops.equals], new Expression("10"));
			ProcessLineList ifLines1 = new ProcessLineList(ifStatement1);

			// create second GET statement ('Get record list 2 from Form 1')
			RecordSet recordList2 = new RecordSet("record list 2", forms);
			GetStatement getStatement2 = new GetStatement(recordList2);
			ProcessLineList getLines2 = new ProcessLineList(getStatement2);

			// create FOR EACH statement ('For Each record in record list')
			Record record2 = new Record("record2");
			ForEachRecordStatement forEachStatement2 = new ForEachRecordStatement(record2, recordList2);
			ProcessLineList forEachLines2 = new ProcessLineList(forEachStatement2);

			IfStatement ifStatement2 = new IfStatement();
			ifStatement2.Conditions = new Conditions(qualifiedField, HybridOperator.List[HybridOperator.Ops.equals], new Expression("10"));
			ProcessLineList ifLines2 = new ProcessLineList(ifStatement2);

			process.Lines.Insert(2, getLines2);
			process.Lines.Insert(5, ifLines1);
			process.Lines.Insert(7, forEachLines2);
			process.Lines.Insert(9, ifLines2);

			// create process line 'Set Variable to record:Q1:a' (first record set)
			SetStatement setStatement1 = new SetStatement();
			setStatement1.Variable = new Variable("Variable");
			setStatement1.Expression = new Expression("<<record:Q1:a>>", process.GetValidFields(11));
			ProcessLineList setLines1 = new ProcessLineList(setStatement1);

			process.Lines.Insert(11, setLines1);

			// create process line 'Set Variable 2 to record2:Q1:a' (second record set)
			SetStatement setStatement2 = new SetStatement();
			setStatement2.Variable = new Variable("Variable 2");
			setStatement2.Expression = new Expression("<<record2:Q1:a>>", process.GetValidFields(12));
			ProcessLineList setLines2 = new ProcessLineList(setStatement2);

			process.Lines.Insert(12, setLines2);

			int i = 0;
			Assert.AreEqual("Set Var1 to 0", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list 2 from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If record:Q1:a equals 10", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record2 in record list 2", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If record:Q1:a equals 10", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());

			// make sure record fields from both record sets are available
			Assert.AreEqual("Set Variable to record:Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual("Set Variable 2 to record2:Form 1:Q1:a", process.Lines[i++].ToString());

			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}

		[Test]
		public void SendLineStatus()
		{
			// add 'For Each' lines to Process
			process.Lines.Add(forEachLines);

			QualifiedFieldList qualifiedField = new QualifiedFieldList(record, form.GetFields()["Q1:a"]);


			// add a document to send as the body
			IDocument doc = Project.Current.AddDocument();

			// create process line 'Send Email to record:Q1:a'
			SendStatement sendStatement = new SendStatement();
			sendStatement.AddressTo = new FieldOrLiteral(qualifiedField);
			sendStatement.Subject = "Email subject";
			sendStatement.SendBody = new SendDocumentBody(doc);

			ProcessLineList sendLineList = new ProcessLineList(sendStatement);

			// place 'Send' line within 'For Each' lines
			process.Lines.Insert(4, sendLineList);

			process.Lines.ValidateLines();
			
			// verify that process line is valid
			Assert.IsTrue(sendLineList[0].IsValid, "Process line is not valid");
		}

		[Test]
		public void IfLineStatus()
		{
			// create process line 'If record:Q1:a equals 10'
			QualifiedFieldList qualifiedField = new QualifiedFieldList(record, form.GetFields()["Q1:a"]);

			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(qualifiedField, HybridOperator.List[HybridOperator.Ops.equals], new Expression("10"));
			ProcessLineList ifLines = new ProcessLineList(ifStatement);

			// place 'If' line within 'For Each' lines
			forEachLines.Insert(2, ifLines);

			// add 'For Each' lines to Process
			process.Lines.Add(forEachLines);

			process.Lines.ValidateLines();

			Assert.AreEqual("If record:Q1:a equals 10", process.Lines[4].ToString());

			// verify that process line is valid
			Assert.IsTrue(ifLines[0].IsValid, "Process line is not valid");
		}

		[Test]
		public void NestedForEachStatements()
		{
			process.Lines.Add(forEachLines);

			// create GET statement ('Get record list 2 from Form 1')
			FormList forms = new FormList();
			forms.Add((Form)Project.Current.FormList[0]);
			RecordSet recordList2 = new RecordSet("record list 2", forms);
			GetStatement getStatement = new GetStatement(recordList2);
			process.Lines.Insert(2, new ProcessLineList(getStatement));

			// create FOR EACH statement ('For Each record 2 in record list 2')
			Record record2 = new Record("record 2");
			ForEachRecordStatement forEachStatement2 = new ForEachRecordStatement(record2, recordList2);
			ProcessLineList forEachLines2 = new ProcessLineList(forEachStatement2);
			process.Lines.Insert(5, forEachLines2);

			int i = 0;
			Assert.AreEqual("Set Var1 to 0", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list 2 from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record 2 in record list 2", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			string expectedXml =
				"<process name=\"Process 1\">\r\n" +
				"<set field=\"Var1\" arithmeticAsText=\"false\">\r\n" +
				"<string value=\"0\"/>\r\n" +
				"</set>\r\n" +
				"<get recordList=\"record list\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>\r\n" +
				"<get recordList=\"record list 2\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>\r\n" +
				"<foreach record=\"record\" recordList=\"record list\">\r\n" +
				"<foreach record=\"record 2\" recordList=\"record list 2\">\r\n" +
				"</foreach>\r\n" +
				"</foreach>\r\n" +
				"</process>\r\n";

			Assert.AreEqual(expectedXml, process.ToXml());
		}


		[Test]
		public void QualifiedFibFieldInNestedForEachStatements()
		{
			process.Lines.Add(forEachLines);

			// create GET statement ('Get record list 2 from Form 1')
			FormList forms = new FormList();
			forms.Add((Form)Project.Current.FormList[0]);
			RecordSet recordList2 = new RecordSet("record list 2", forms);
			GetStatement getStatement = new GetStatement(recordList2);
			process.Lines.Insert(2, new ProcessLineList(getStatement));

			// create FOR EACH statement ('For Each record 2 in record list 2')
			Record record2 = new Record("record 2");
			ForEachRecordStatement forEachStatement2 = new ForEachRecordStatement(record2, recordList2);
			ProcessLineList forEachLines2 = new ProcessLineList(forEachStatement2);
			process.Lines.Insert(5, forEachLines2);

			// create SET statement ('Set Variable 1 to record 2:Q1:a')
			QualifiedFieldList qualifiedField = new QualifiedFieldList(record2, form.GetFields()["Q1:a"]);
			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new Variable("Variable 1");
			setStatement.Expression = new Expression(qualifiedField);
			ProcessLineList setLines = new ProcessLineList(setStatement);
			process.Lines.Insert(7, setLines);

			int i = 0;
			Assert.AreEqual("Set Var1 to 0", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list 2 from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record 2 in record list 2", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Set Variable 1 to record 2:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			string expectedXml =
				"<process name=\"Process 1\">\r\n" +
				"<set field=\"Var1\" arithmeticAsText=\"false\">\r\n" +
				"<string value=\"0\"/>\r\n" +
				"</set>\r\n" +
				"<get recordList=\"record list\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>\r\n" +
				"<get recordList=\"record list 2\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>\r\n" +
				"<foreach record=\"record\" recordList=\"record list\">\r\n" +
				"<foreach record=\"record 2\" recordList=\"record list 2\">\r\n" +
				"<set field=\"Variable 1\" arithmeticAsText=\"false\">\r\n" +
				"<string field=\"record 2:Q1:a\"/>\r\n" +
				"</set>\r\n" +
				"</foreach>\r\n" +
				"</foreach>\r\n" +
				"</process>\r\n";

			Assert.AreEqual(expectedXml, process.ToXml());
		}


	}
}
