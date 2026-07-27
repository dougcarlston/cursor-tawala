using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing the generation of valid fields by a Process
	/// </summary>
	[TestFixture]
	public class ProcessValidFieldsTest
	{
		private IForm form1;
		private Process process1;
		private ProcessLineList forEachLines1;
		private Record record1;

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
			form1 = Project.Current.AddForm();

			// create process
			process1 = Project.Current.AddProcess();

			// connect process to form
			Project.Current.ConnectProcessToForm(process1, form1.Name);

			//// add a Variable to the Process
			//process1.Variables.AddUnique("Var");

			// add new FIB item to form
			form1.ItemList.Add(new FibItem());
			HiddenField hidden = new HiddenField();
			hidden.Name = "HiddenField";
			form1.ItemList.Add(hidden);

			// create SET statement ('Set Var to 0')
			SetStatement setStatement1 = new SetStatement();
			setStatement1.Variable = new Variable("Var");
			setStatement1.Expression = new Expression("0");
			ProcessLineList setLine = new ProcessLineList(setStatement1);
			process1.Lines.Add(setLine);

			// create GET statement ('Get record list from Form 1')
			FormList forms = new FormList();
			forms.Add((Form)Project.Current.FormList[0]);
			RecordSet recordList1 = new RecordSet("Record List 1", forms);
			GetStatement getStatement1 = new GetStatement(recordList1);
			process1.Lines.Add(new ProcessLineList(getStatement1));

			// create FOR EACH statement ('For Each Record1 in Record List 1')
			record1 = new Record("Record1");
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record1, recordList1);
			forEachLines1 = new ProcessLineList(forEachStatement);
			process1.Lines.Add(forEachLines1);

			// create GET statement ('Get Record List 2 from Form 1')
			RecordSet recordList2 = new RecordSet("Record List 2", forms);
			GetStatement getStatement2 = new GetStatement(recordList2);
			process1.Lines.Insert(2, new ProcessLineList(getStatement2));

			// create FOR EACH statement ('For Each Record2 in Record list 2')
			Record record2 = new Record("Record2");
			ForEachRecordStatement forEachStatement2 = new ForEachRecordStatement(record2, recordList2);
			ProcessLineList forEachLines2 = new ProcessLineList(forEachStatement2);
			process1.Lines.Insert(5, forEachLines2);
		}

		[Test]
		public void CheckXml()
		{
			string expectedXml =
				"<process name=\"Process 1\">\r\n" +
				"<set field=\"Var\" arithmeticAsText=\"false\">\r\n" +
				"<string value=\"0\"/>\r\n" +
				"</set>\r\n" +
				"<get recordList=\"Record List 1\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>\r\n" +
				"<get recordList=\"Record List 2\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>\r\n" +
				"<foreach record=\"Record1\" recordList=\"Record List 1\">\r\n" +
				"<foreach record=\"Record2\" recordList=\"Record List 2\">\r\n" +
				"</foreach>\r\n" +
				"</foreach>\r\n" +
				"</process>\r\n";

			Assert.AreEqual(expectedXml, process1.ToXml());
		}

		[Test]
		public void CheckProcessLines()
		{
			int i = 0;
			Assert.AreEqual("Set Var to 0", process1.Lines[i++].ToString());
			Assert.AreEqual("Get Record List 1 from Form 1", process1.Lines[i++].ToString());
			Assert.AreEqual("Get Record List 2 from Form 1", process1.Lines[i++].ToString());
			Assert.AreEqual("For Each Record1 in Record List 1", process1.Lines[i++].ToString());
			Assert.AreEqual("(", process1.Lines[i++].ToString());
			Assert.AreEqual("For Each Record2 in Record List 2", process1.Lines[i++].ToString());
			Assert.AreEqual("(", process1.Lines[i++].ToString());
			Assert.AreEqual(")", process1.Lines[i++].ToString());
			Assert.AreEqual(")", process1.Lines[i++].ToString());
		}

		[Test]
		public void GetValidFields()
		{
			string[] fieldNames = new string[]
			{
				"Q1:a",
				"HiddenField",
				"Var",
				Tawala.Projects.Properties.Resources.PrivateInvitationVariableLabel
			};

			FieldList fieldList1 = process1.GetValidFields(0);

			int i = 0;

			foreach (IField field in fieldList1.RecursiveEnumerator)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}

			Assert.AreEqual(fieldNames.Length, i);
		}

		[Test]
		public void GetValidFieldsInForEach()
		{
			string[] fieldNames = new string[]
			{
				"Q1:a",
				"HiddenField",
				"Var",
				Tawala.Projects.Properties.Resources.PrivateInvitationVariableLabel,
				"Record1:Q1:a",
				"Record1:HiddenField",
				"Record1:Var",
			};

			FieldList fieldList1 = process1.GetValidFields(5);
			FieldList.DumpFields(process1.GetValidFields(5), "ProcessValidFieldsTest.GetValidFieldsInForEach");

			int i = 0;

			foreach (IField field in fieldList1.RecursiveEnumerator)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}

			Assert.AreEqual(fieldNames.Length, i);
		}

		[Test]
		public void GetValidFieldsInNestedForEach()
		{
			string[] fieldNames = new string[]
			{
				"Q1:a",
				"HiddenField",
				"Var",
				Tawala.Projects.Properties.Resources.PrivateInvitationVariableLabel,
				"Record2:Q1:a",
				"Record2:HiddenField",
				"Record2:Var",
				"Record1:Q1:a",
				"Record1:HiddenField",
				"Record1:Var",
			};

			FieldList fieldList1 = process1.GetValidFields(7);
			FieldList.DumpFields(process1.GetValidFields(7), "ProcessValidFieldsTest.GetValidFieldsInNestedForEach");

			int i = 0;

			foreach (IField field in fieldList1.RecursiveEnumerator)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}

			Assert.AreEqual(fieldNames.Length, i);
		}

	}
}
