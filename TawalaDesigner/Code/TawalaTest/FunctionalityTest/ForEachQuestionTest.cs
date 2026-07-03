using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.FunctionalityTest
{
#if false
	[TestFixture]
	public class ForEachQuestionTest
	{
		private IForm form;
		private Process process;
		private FormList forms;
		private RecordSet recordList1;
		private Record record1;

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

			// add new MC item to form
			form.ItemList.Add(new MCItem());

			// create SET statement ('Set Variable 1 to Q1:a, Set Variable 2 to Q1:a')
			process.Variables.AddUnique("Variable 1");
			process.Variables.AddUnique("Variable 2");
			ProcessLineList setLines = getSetLines();
			process.Lines.Add(setLines);

			// create GET statement ('Get record list from Form 1')
			forms = new FormList();
			forms.Add((Form)Project.Current.FormList[0]);
			recordList1 = new RecordSet("Record Set", forms);
			GetStatement getStatement = new GetStatement(recordList1);
			process.Lines.Add(new ProcessLineList(getStatement));

			// create FOR EACH RECORD statement ('For Each record in record list')
			record1 = new Record("Record");
			ProcessLineList forEachRecordLines = getForEachRecordLines(recordList1, record1);
			process.Lines.Add(forEachRecordLines);

			// create FOR EACH QUESTION statement ('For Each Multiple Choice Question')
			ProcessLineList forEachQuestionLines = getForEachQuestionLines();
			process.Lines.Insert(5, forEachQuestionLines);

			Assert.AreEqual(2, process.Variables.Count);
		}

		private static ProcessLineList getSetLines()
		{
			ProcessLineList setLines = new ProcessLineList();
			SetStatement setStatement;

			setStatement = new SetStatement();
			setStatement.Variable = new Variable("Variable 1");
			setStatement.Expression = new Expression("<<Q1:a>>");
			setLines.Add(new ProcessLineList(setStatement));

			setStatement = new SetStatement();
			setStatement.Variable = new Variable("Variable 2");
			setStatement.Expression = new Expression("<<Q1:a>>");
			setLines.Add(new ProcessLineList(setStatement));

			return setLines;
		}

		private static ProcessLineList getForEachRecordLines(RecordSet recordList, Record record)
		{
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			return forEachLines;
		}

		private static ProcessLineList getForEachQuestionLines()
		{
			ForEachQuestionStatement forEachStatement = new ForEachQuestionStatement();
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			return forEachLines;
		}


		[Test]
		public void CheckProcess()
		{
			int i = 0;
			Assert.AreEqual("Set Variable 1 to Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual("Set Variable 2 to Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual("Get Record Set from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record in Record Set", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Multiple Choice Question", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}


		private static void checkFieldNamesAreEqual(IField fields, string[] fieldNames)
		{
			int i = 0;

			foreach (IField field in fields.RecursiveEnumerator)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}

			Assert.AreEqual(fieldNames.Length, i);
		}

		private static void checkAreEqual(Collection<string> strings, string[] expectedStrings)
		{
			int i = 0;

			foreach (string s in strings)
			{
				Assert.AreEqual(expectedStrings[i++], s);
			}

			Assert.AreEqual(expectedStrings.Length, i);
		}

		[Test]
		public void UnqualifiedRecordStatementFields()
		{
			ForEachRecordStatement forEachRecordStatement = (ForEachRecordStatement)process.Lines[3].Statement;

			IField fields = forEachRecordStatement.GetUnqualifiedFields();

			string[] fieldNames = new string[]
			{
			};

			checkFieldNamesAreEqual(fields, fieldNames);
		}


		[Test]
		public void QualifiedRecordStatementFields()
		{
			ForEachRecordStatement forEachRecordStatement = (ForEachRecordStatement)process.Lines[3].Statement;

			IField fields = forEachRecordStatement.GetQualifiedFields(process);

			string[] fieldNames = new string[]
			{
				"Record:Q1:a",
				"Record:Q2",
				"Record:Variable 1",
				"Record:Variable 2",
			};

			checkFieldNamesAreEqual(fields, fieldNames);
		}

		[Test]
		public void UnqualifiedRecordStatementMCFields()
		{
			ForEachRecordStatement forEachRecordStatement = (ForEachRecordStatement)process.Lines[3].Statement;

			IField fields = forEachRecordStatement.GetUnqualifiedMCFields();

			string[] fieldNames = new string[]
			{
			};

			checkFieldNamesAreEqual(fields, fieldNames);
		}


		[Test]
		public void QualifiedRecordStatementMCFields()
		{
			ForEachRecordStatement forEachRecordStatement = (ForEachRecordStatement)process.Lines[3].Statement;

			IField fields = forEachRecordStatement.GetQualifiedMCFields(process);

			string[] fieldNames = new string[]
			{
				"Record:Q2",
			};

			checkFieldNamesAreEqual(fields, fieldNames);
		}


		[Test]
		public void UnqualifiedQuestionStatementFields()
		{
			ForEachQuestionStatement forEachQuestionStatement = (ForEachQuestionStatement)process.Lines[5].Statement;

			IField fields = forEachQuestionStatement.GetUnqualifiedFields();

			string[] fieldNames = new string[]
			{
				"(selection)"
			};

			checkFieldNamesAreEqual(fields, fieldNames);
		}


		[Test]
		public void QualifiedQuestionStatementFields()
		{
			ForEachQuestionStatement forEachQuestionStatement = (ForEachQuestionStatement)process.Lines[5].Statement;

			IField fields = forEachQuestionStatement.GetQualifiedFields(process);

			string[] fieldNames = new string[]
			{
				"Record:Q1:a",
				"Record:Q2",
				"Record:Variable 1",
				"Record:Variable 2",
				"Record:(selection)",
			};

			FieldList.DumpFields(process.GetValidFields(7), "ForEachQuestionTest.QualifiedQuestionStatementFields");
			checkFieldNamesAreEqual(fields, fieldNames);
		}


		[Test]
		public void UnqualifiedQuestionStatementMCFields()
		{
			ForEachQuestionStatement forEachQuestionStatement = (ForEachQuestionStatement)process.Lines[5].Statement;

			IField fields = forEachQuestionStatement.GetUnqualifiedMCFields();

			string[] fieldNames = new string[]
			{
				"(selection)"
			};

			checkFieldNamesAreEqual(fields, fieldNames);
		}

		[Test]
		public void QualifiedQuestionStatementMCFields()
		{
			ForEachQuestionStatement forEachQuestionStatement = (ForEachQuestionStatement)process.Lines[5].Statement;

			IField fields = forEachQuestionStatement.GetQualifiedMCFields(process);

			string[] fieldNames = new string[]
			{
				"Record:Q2",
				"Record:(selection)",
			};

			FieldList.DumpFields(process.GetValidFields(7), "ForEachQuestionTest.QualifiedQuestionStatementMCFields");
			checkFieldNamesAreEqual(fields, fieldNames);
		}


		[Test]
		public void QualifiedRecordStatementVariables()
		{
			ForEachRecordStatement forEachRecordStatement = (ForEachRecordStatement)process.Lines[3].Statement;

			IField variables = forEachRecordStatement.GetQualifiedVariables(process);

			string[] variableNames = new string[]
			{
				"Record:Variable 1",
				"Record:Variable 2"
			};

			checkFieldNamesAreEqual(variables, variableNames);
		}


		[Test]
		public void QualifiedQuestionStatementVariables()
		{
			ForEachQuestionStatement forEachQuestionStatement = (ForEachQuestionStatement)process.Lines[5].Statement;

			IField variables = forEachQuestionStatement.GetQualifiedVariables(process);

			string[] variableNames = new string[]
			{
			};

			checkFieldNamesAreEqual(variables, variableNames);
		}

		[Test]
		public void ValidFields()
		{
			string[] fieldNames = new string[]
			{
				"Q1:a",
				"Q2",
				"Variable 1",
				"Variable 2",
				Project.Resources.GetString("PrivateInvitationVariableLabel"),
				"(selection)",
				"Record:Q1:a",
				"Record:Q2",
				"Record:Variable 1",
				"Record:Variable 2",
				"Record:(selection)",
			};

			FieldList.DumpFields(process.GetValidFields(7), "ForEachQuestionTest.ValidFields");

			checkFieldNamesAreEqual(process.GetValidFields(7), fieldNames);
		}

		[Test]
		public void ValidMCFields()
		{
			string[] fieldNames = new string[]
			{
				"Q2",
				"(selection)",
				"Record:Q2",
				"Record:(selection)",
			};

			FieldList.DumpFields(process.GetValidFields(7), "ForEachQuestionTest.ValidMCFields");

			checkFieldNamesAreEqual(process.GetValidMCFields(7), fieldNames);
		}


	}
#endif
}
