using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Class to test Tawala.Common.Process class.
	/// </summary>
	[TestFixture]
	public class ProcessTest
	{
		private IForm form;
		private Process process;
		private FormList forms;
		private RecordSet recordList1;
		private Record record1;
		private McqItem mcItem1;

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

			// add new MC item to form
			mcItem1 = new McqItem();
			form.ItemList.Add(mcItem1);

			// add new FIB item to form
			form.ItemList.Add(new FibItem());

			// create GET statement ('Get record list from Form 1')
			forms = new FormList();
			forms.Add(Project.Current.FormList[0]);
			recordList1 = new RecordSet("record list", forms);
			GetStatement getStatement = new GetStatement(recordList1);
			process.Lines.Add(new ProcessLineList(getStatement));

			// create FOR EACH statement ('For Each record in record list')
			record1 = new Record("record");
			ProcessLineList forEachLines1 = getForEachLines(recordList1, record1);
			process.Lines.Add(forEachLines1);

			// create IF STATEMENT 'If Q1 equals record:Q1'
			ProcessLineList ifLines = getIfWithQualifiedMCField();
			process.Lines.Insert(3, ifLines);
		}

		private static ProcessLineList getForEachLines(RecordSet recordList, Record record)
		{
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			return forEachLines;
		}

		private ProcessLineList getIfWithQualifiedMCField()
		{
			// create process line 'If Form 1:Q1 equals record:Form 1:Q1'
			IfStatement ifStatement = new IfStatement();
			//FieldOrLiteral expression = new FieldOrLiteral(new RecordField(record1, mcItem1));
			Expression expression = new Expression(new RecordField(record1, mcItem1));
			ifStatement.Conditions = new Conditions(form.GetFields()["Q1"], MCOneOperator.List[MCOneOperator.Ops.mcEquals], expression);
			ProcessLineList ifLines = new ProcessLineList(ifStatement);
			return ifLines;
		}

		private static void dumpFields(IField fields)
		{
			Console.WriteLine("ProcessTest.dumpfields:");

			foreach (IField field in fields.RecursiveEnumerator)
			{
				Console.WriteLine(" field.fieldName = {0}", field.FieldName);
			}
		}

		[Test]
		public void CheckProcess()
		{
			int i = 0;
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If Form 1:Q1 equals record:Form 1:Q1", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

		}

		[Test]
		public void NewProcess() 
		{ 
			Process process = new Process("Test");

			//Assertions 
			Assert.IsNotNull(process);
			Assert.AreEqual("Test", process.Name);
		} 

		[Test]
		public void NameProcess() 
		{ 
			Process process = new Process("Renamed Process");

			//Assertions 
			Assert.AreEqual("Renamed Process", process.Name);
		} 

		[Test]
		public void NameProcessViaConstructor() 
		{ 
			Process process = new Process("MyProcess");

			//Assertions 
			Assert.AreEqual("MyProcess", process.Name);
		}

		[Test]
		public void TrimWhitespaceFromName()
		{
			Process process = new Process("    Process 1  ");
			Assert.AreEqual("Process 1", process.Name);

			process.Name = "   Renamed Process   ";
			Assert.AreEqual("Renamed Process", process.Name);
		}

		[Test]
		public void AddShowDocumentLine() 
		{ 
			// create new test project
			Project.NewTestProject();

			// create process
			Process process = new Process("MyProcess");

			// add show line to process
			IDocument document1 = Project.Current.AddDocument();
			ShowStatement statement = new ShowDocumentStatement(document1);
			ProcessLineList lineList = new ProcessLineList(statement);

			process.Lines.Add (lineList);

			Assert.AreEqual(1, process.Lines.Count);
		}

		[Test]
		public void RemoveShowStatement() 
		{ 
			Process process = new Process("MyProcess");

			IDocument document1 = new Document("Document 1");
			ShowDocumentStatement st1 = new ShowDocumentStatement(document1);
			ShowLine line1 = new ShowDocumentLine(st1);

			IDocument document2 = new Document("Document 2");
			ShowDocumentStatement st2 = new ShowDocumentStatement(document2);
			ShowLine line2 = new ShowDocumentLine(st2);

			// add 2 show statements
			process.Lines.Add(line1);
			process.Lines.Add(line2);

			Assert.AreEqual(2, process.Lines.Count);
			Assert.AreEqual("Document 1", ((ShowStatement)process.Lines[0].Statement).Document.Name);
			Assert.AreEqual("Document 2", ((ShowStatement)process.Lines[1].Statement).Document.Name);

			// remove first statement
			process.Lines.Remove(line1);

			Assert.AreEqual(1, process.Lines.Count);
			Assert.AreEqual("Document 2", ((ShowStatement)process.Lines[0].Statement).Document.Name);
		} 

		[Test]
		public void InsertShowStatement() 
		{ 
			Process process = new Process("MyProcess");

			Document document1 = new Document("Document 1");
			ShowDocumentStatement st1 = new ShowDocumentStatement(document1);
			ShowLine line1 = new ShowDocumentLine(st1);

			Document document2 = new Document("Document 2");
			ShowDocumentStatement st2 = new ShowDocumentStatement(document2);
			ShowLine line2 = new ShowDocumentLine(st2);

			Document document3 = new Document("Document 3");
			ShowDocumentStatement st3 = new ShowDocumentStatement(document3);
			ShowLine line3 = new ShowDocumentLine(st3);

			// add 2 show statements
			process.Lines.Add(line1);
			process.Lines.Add(line2);

			Assert.AreEqual(2, process.Lines.Count);
			Assert.AreEqual("Document 1", ((ShowStatement)process.Lines[0].Statement).Document.Name);
			Assert.AreEqual("Document 2", ((ShowStatement)process.Lines[1].Statement).Document.Name);

			// insert third statement
			process.Lines.Insert(1, line3);

			Assert.AreEqual(3, process.Lines.Count);
			Assert.AreEqual("Document 1", ((ShowStatement)process.Lines[0].Statement).Document.Name);
			Assert.AreEqual("Document 3", ((ShowStatement)process.Lines[1].Statement).Document.Name);
			Assert.AreEqual("Document 2", ((ShowStatement)process.Lines[2].Statement).Document.Name);
		}

		[Test]
		public void NewRecord()
		{
			Process process = new Process("Test");

			Assert.AreEqual(0, process.Records.Count);

			Record record1 = new Record("Record 1");
			process.Records.Add(record1);

			Assert.AreEqual(1, process.Records.Count);
			Assert.AreEqual("Record 1", process.Records[0].FieldName);
		}

		[Test]
		public void NewRecordSet()
		{
			Process process = new Process("Test");

			Assert.AreEqual(0, process.RecordSets.Count);

			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			RecordSet recordSet1 = new RecordSet("Record Set 1", forms);
			process.RecordSets.Add(recordSet1);

			Assert.AreEqual(1, process.RecordSets.Count);
			Assert.AreEqual("Record Set 1", process.RecordSets[0].FieldName);
		}

		[Test]
		public void GetXml() 
		{ 
			Process process = new Process("Process Name");

			// add show line to process
			//Document document1 = new Document("Document 1");
			IDocument document1 = Project.Current.AddDocument();
			ShowDocumentStatement st1 = new ShowDocumentStatement(document1);
			ProcessLine line1 = new ShowDocumentLine(st1);
			process.Lines.Add(line1);

			string expString =	"<process name=\"Process Name\">\r\n" +
								"<show document=\"Document 1\" reset=\"false\"/>\r\n" +
								"</process>\r\n";

			//Assertions 
			Assert.AreEqual(expString, process.ToXml());

			// check for illegal XML characters
			process.Name = "&<Process's \"Bad\" Name>";
			expString = "<process name=\"&amp;&lt;Process&apos;s &quot;Bad&quot; Name&gt;\">\r\n" +
						"<show document=\"Document 1\" reset=\"false\"/>\r\n" +
						"</process>\r\n";
			Assert.AreEqual(expString, process.ToXml());
		}

		[Test]
		public void VariableListTest()
		{
			Process process = new Process("Test");

			Assert.AreEqual(0, process.Variables.Count);

			process.Variables.AddUnique("Variable 1");
			process.Variables.AddUnique("Variable 2");
			process.Variables.AddUnique("Variable 3");

			Assert.AreEqual(3, process.Variables.Count);
			Assert.AreEqual("Variable 1", process.Variables[0].FieldName);
			Assert.AreEqual("Variable 2", process.Variables[1].FieldName);
			Assert.AreEqual("Variable 3", process.Variables[2].FieldName);
		}


		[Test]
		public void GetValidMCFields()
		{
			FieldList fields = process.GetValidMCFields(3);

			string[] fieldNames = new string[]
			{
				"Q1",
				"record:Q1"
			};

			dumpFields(fields);

			int i = 0;

			foreach (IField field in fields.RecursiveEnumerator)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}

			Assert.AreEqual(i, fieldNames.Length);
		}
	}
}
