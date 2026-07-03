using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class ProcessFromXmlTest
	{
		private IForm form;
		private Process process;
		private FormList forms;
		private RecordSet recordList1;
		private Record record1;
		private McqItem mcItem1;
		private FibItem fibItem1;

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
			fibItem1 = new FibItem();
			fibItem1.BlankList.Add(new Blank(new FibItem(), 20));
			form.ItemList.Add(fibItem1);

			// add new field to form
			HiddenField field1 = new HiddenField();
			field1.Name = "Score";
			form.ItemList.Add(field1);

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
		public void IfRecordAndFormQualifiedFieldEqualsVariable()
		{
			string xmlString =
				@"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
				@"<project name=""Untitled"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true"" process=""Process 1"" themePath=""default"">" + Environment.NewLine +
				@"<items>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				"</form>" + Environment.NewLine +
				"</forms>" + Environment.NewLine +
				"<processes>" + Environment.NewLine +
				"<process name=\"Process 1\">" + Environment.NewLine +
				"<set field=\"Score\" arithmeticAsText=\"false\">" + Environment.NewLine +
				"<string value=\"100\"/>" + Environment.NewLine +
				"</set>" + Environment.NewLine +
				"<get recordList=\"Record List 1\">" + Environment.NewLine +
				"<forms>" + Environment.NewLine +
				"<form name=\"Form 1\"/>" + Environment.NewLine +
				"</forms>" + Environment.NewLine +
				"</get>" + Environment.NewLine +
				"<foreach record=\"Record\" recordList=\"Record List 1\">" + Environment.NewLine +
				"<if>" + Environment.NewLine +
				"<conditions>" + Environment.NewLine +
				"<equals field=\"Record:Form 1:Score\">" + Environment.NewLine +
				"<string field=\"Score\"/>" + Environment.NewLine +
				"</equals>" + Environment.NewLine +
				"</conditions>" + Environment.NewLine +
				"<trueSet>" + Environment.NewLine +
				"</trueSet>" + Environment.NewLine +
				"</if>" + Environment.NewLine +
				"</foreach>" + Environment.NewLine +
				"</process>" + Environment.NewLine +
				"</processes>" + Environment.NewLine +
				"</project>";

			TestSupport.Util.OpenProjectXml(xmlString);

			Process process = Project.Current.ProcessList[0];

			Assert.AreEqual(process.Name, "Process 1");
			
			int i = 0;
			Assert.AreEqual("Set Score to 100", process.Lines[i++].ToString());
			Assert.AreEqual("Set Form 1:Score to Score", process.Lines[i++].ToString());
			Assert.AreEqual("Get Record List 1 from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record in Record List 1", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());

			Assert.AreEqual("If Record:Form 1:Score equals Score", process.Lines[i++].ToString());
			
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(i, process.Lines.Count);
		}

		[Test]
		public void IfMCQEqualsChoice()
		{
			string xmlString =
				"<process name=\"Process 1\">" +
				"<if>" +
				"<conditions>" +
				"<mcEquals field=\"Form 1:Q1\" value=\"a\"/>" +
				"</conditions>" +
				"<trueSet>" +
				"</trueSet>" +
				"</if>" +
				"</process>";

			IXmlElement element = new XmlElement(xmlString);
			Process process = new Process(element);

			Assert.AreEqual("Process 1", process.Name);

			int i = 0;
			Assert.AreEqual("If Form 1:Q1 equals a", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(3, process.Lines.Count);
		}

		[Test]
		public void IfFibEqualsConstant()
		{
			string xmlString =
				"<process name=\"Process 1\">" +
				"<if>" +
				"<conditions>" +
				"<equals field=\"Form 1:Q2:a\">" +
				"<string value=\"100\"/>" +
				"</equals>" +
				"</conditions>" +
				"<trueSet>" +
				"</trueSet>" +
				"</if>" +
				"</process>";

			IXmlElement element = new XmlElement(xmlString);
			Process process = new Process(element);

			Assert.AreEqual("Process 1", process.Name);

			int i = 0;
			Assert.AreEqual("If Form 1:Q2:a equals 100", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(3, process.Lines.Count);
		}

		[Test]
		public void IfAlternateFibEqualsConstant()
		{
			fibItem1.BlankList[0].AlternateLabel = "Name";

			string xmlString =
				"<process name=\"Process 1\">" +
				"<if>" +
				"<conditions>" +
				"<equals field=\"Form 1:Name\">" +
				"<string value=\"Archie\"/>" +
				"</equals>" +
				"</conditions>" +
				"<trueSet>" +
				"</trueSet>" +
				"</if>" +
				"</process>";

			IXmlElement element = new XmlElement(xmlString);
			Process process = new Process(element);

			Assert.AreEqual("Process 1", process.Name);

			int i = 0;
			Assert.AreEqual("If Form 1:Name equals \"Archie\"", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(3, process.Lines.Count);
		}

		[Test]
		public void IfVariableEqualsRecordAndFormQualifiedField()
		{
			string xmlString =
				"<process name=\"Process 1\">" +
				"<set field=\"Score\" arithmeticAsText=\"false\">" +
				"<string value=\"100\"/>" +
				"</set>" +
				"<get recordList=\"Record List 1\">" +
				"<forms>" +
				"<form name=\"Form 1\"/>" +
				"</forms>" +
				"</get>" +
				"<foreach record=\"Record\" recordList=\"Record List 1\">" +
				"<if>" +
				"<conditions>" +
				"<equals field=\"Score\">" +
				"<string field=\"Record:Form 1:Score\"/>" +
				"</equals>" +
				"</conditions>" +
				"<trueSet>" +
				"</trueSet>" +
				"</if>" +
				"</foreach>" +
				"</process>";

			IXmlElement element = new XmlElement(xmlString);
			Process process = new Process(element);

			Assert.AreEqual("Process 1", process.Name);

			int i = 0;
			Assert.AreEqual("Set Score to 100", process.Lines[i++].ToString());
			Assert.AreEqual("Get Record List 1 from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record in Record List 1", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());

			Assert.AreEqual("If Score equals Record:Form 1:Score", process.Lines[i++].ToString());
			
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(i, process.Lines.Count);
		}

		[Test]
		public void SetVariableToRecordAndFormQualfiedField()
		{
			string xmlString =
				"<process name=\"Process 1\">" +
				"<set field=\"Score\" arithmeticAsText=\"false\">" +
				"<string value=\"100\"/>" +
				"</set>" +
				"<get recordList=\"Record List 1\">" +
				"<forms>" +
				"<form name=\"Form 1\"/>" +
				"</forms>" +
				"</get>" +
				"<foreach record=\"Record\" recordList=\"Record List 1\">" +
				"<set field=\"Score\" arithmeticAsText=\"false\">" +
				"<string field=\"Record:Form 1:Score\"/>" +
				"</set>" +
				"</foreach>" +
				"</process>";

			IXmlElement element = new XmlElement(xmlString);
			Process process = new Process(element);

			Assert.AreEqual("Process 1", process.Name);

			int i = 0;
			Assert.AreEqual("Set Score to 100", process.Lines[i++].ToString());
			Assert.AreEqual("Get Record List 1 from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record in Record List 1", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());

			Assert.AreEqual("Set Score to Record:Form 1:Score", process.Lines[i++].ToString());
			
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(i, process.Lines.Count);
		}

		[Test]
		public void SetRecordAndFormQualfiedFieldToVariable()
		{
			string xmlString =
				"<process name=\"Process 1\">" +
				"<set field=\"Score\" arithmeticAsText=\"false\">" +
				"<string value=\"100\"/>" +
				"</set>" +
				"<get recordList=\"Record List 1\">" +
				"<forms>" +
				"<form name=\"Form 1\"/>" +
				"</forms>" +
				"</get>" +
				"<foreach record=\"Record\" recordList=\"Record List 1\">" +
				"<set field=\"Record:Form 1:Score\" arithmeticAsText=\"false\">" +
				"<string field=\"Score\"/>" +
				"</set>" +
				"</foreach>" +
				"</process>";

			IXmlElement element = new XmlElement(xmlString);
			Process process = new Process(element);

			Assert.AreEqual("Process 1", process.Name);

			int i = 0;
			Assert.AreEqual("Set Score to 100", process.Lines[i++].ToString());
			Assert.AreEqual("Get Record List 1 from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record in Record List 1", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Set Record:Form 1:Score to Score", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(6, process.Lines.Count);
		}

	}
}
