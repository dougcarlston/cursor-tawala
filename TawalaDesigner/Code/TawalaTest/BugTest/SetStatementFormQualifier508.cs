using System;
using System.IO;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
    [TestFixture]
	public class SetStatementFormQualifier508
    {
    	private IForm form;
    	private IFibItem fib;
    	private IBlank blank;
    	private Process process;
		private FormList forms;
		private RecordSet recordList1;
		private Record record1;

		[SetUp]
		public void Setup()
		{
			Util.NewTestProject();

			form = Project.Current.AddForm();
			
			fib = new FibItem();
			form.ItemList.Add(fib);
			blank = fib.BlankList[0];

			process = Project.Current.AddProcess();

			addGetRecordList1FromForm1Lines();
			addForEachRecordInRecordList1Lines();
			insertSetRecordForm1Q1aLines();
		}

    	private void addForEachRecordInRecordList1Lines()
    	{
    		record1 = new Record("Record");
    		ProcessLineList forEachLines1 = getForEachLines(recordList1, record1);
    		process.Lines.Add(forEachLines1);
    	}

    	private void addGetRecordList1FromForm1Lines()
    	{
    		forms = new FormList();
    		forms.Add(form);
    		recordList1 = new RecordSet("Record List 1", forms);
    		process.Lines.Add(getGetLines(recordList1));
    	}

		private void insertSetRecordForm1Q1aLines()
		{
			forms = new FormList();
			forms.Add(form);
			recordList1 = new RecordSet("Record List 1", forms);
			process.Lines.Insert(3, getSetLines(record1));
		}

    	private static ProcessLineList getGetLines(RecordSet recordList)
		{
			GetStatement getStatement = new GetStatement(recordList);
			return (new ProcessLineList(getStatement));
		}

		private static ProcessLineList getForEachLines(RecordSet recordList, Record record)
		{
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			return forEachLines;
		}

		private ProcessLineList getSetLines(Record record)
		{
			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new RecordField(record, blank);
			setStatement.Expression = new Expression("100");
			return (new ProcessLineList(setStatement));
		}

    	[Test]
		public void ConfirmProcessLines()
		{
    		int i = 0;

			Assert.AreEqual("Get Record List 1 from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record in Record List 1", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Set Record:Form 1:Q1:a to 100", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}

		[Test]
		public void RenamingFormUpdatesFormNameInProcess()
		{
			Project.Current.RenameForm("Form 1", "Renamed Form 1");

			Assert.AreEqual("Set Record:Renamed Form 1:Q1:a to 100", process.Lines[3].ToString());
		}

		[Test]
		public void CanCreateRecordFieldFromBlankInSetStatementXml()
		{
			string projectXmlString =
				@"<?xml version=""1.0"" encoding=""utf-8"" ?>" +
				@"<project name=""AAA Test"" themePath=""default"" format=""1.12"" designerBuild=""222"">" +
				@"<pageHeader></pageHeader>" +
				@"<forms>" +
				@"<form name=""Renamed Form 1"" startPoint=""true"" themePath=""default"">" +
				@"<items>" +
				@"<fib label=""Q1"" style=""topLabels"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<tabPositions>" +
				@"<tabStop position=""2880""/>" +
				@"</tabPositions>" +
				@"<font face=""Arial"" size=""200"" color=""000000"">[Replace this with your question. Underscores " +
				@"create blanks.] </font>" +
                @"<blank label=""a"" length=""20"" required=""false""></blank>" +
				@"</paragraph>" +
				@"</fib>" +
				@"</items>" +
				@"</form>" +
				@"</forms>" +
				@"<processes>" +
				@"<process name=""Process 1"">" +
				@"<get recordList=""Record List 1"">" +
				@"<forms>" +
				@"<form name=""Renamed Form 1""/>" +
				@"</forms>" +
				@"</get>" +
				@"<foreach record=""Record"" recordList=""Record List 1"">" +
				@"<set field=""Record:Renamed Form 1:Q1:a"" arithmeticAsText=""false"">" +
				@"<string value=""100""/>" +
				@"</set>" +
				@"</foreach>" +
				@"</process>" +
				@"</processes>" +
				@"</project>";

			using (MemoryStream xmlStream = new MemoryStream())
			{
				byte[] xmlByteArray = System.Text.Encoding.UTF8.GetBytes(projectXmlString);
				xmlStream.Write(xmlByteArray, 0, xmlByteArray.Length);

				TawalaProjectConverter converter = new TawalaProjectConverter(xmlStream);
				converter.ConvertXmlToProject();
			}

			SetLine setLine = Project.Current.ProcessList[0].Lines[3] as SetLine;
			SetStatement setStatement = setLine.Statement as SetStatement;

			Assert.IsInstanceOfType(typeof(RecordField), setStatement.Variable);
		}
	}
}
