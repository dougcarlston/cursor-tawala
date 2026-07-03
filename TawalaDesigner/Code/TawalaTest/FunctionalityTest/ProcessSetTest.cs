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
	/// Class for testing SET statement functionality as relates to the Process
	/// </summary>
	[TestFixture]
	public class ProcessSetTest
	{
		private IForm form;
		private Process process;
		private Record record;
		private RecordSet recordList;

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
		}

		[Test]
		public void RepositionByInsertion()
		{
			// create SET statement ('Set Variable to <<Q1:a>>')
			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new Variable("Variable");

			Expression expression = new Expression("<<Q1:a>>");
			setStatement.Expression = expression;

			// make process line list from SET statement and add to process
			process.Lines.Add(new ProcessLineList(setStatement));

			Assert.AreEqual("Set Variable to Form 1:Q1:a", process.Lines[0].ToString());

			// create FIB item
			FibItem fibItem2 = new FibItem();

			// insert FIB item at beginning of form
			form.ItemList.Insert(0, fibItem2);

			Assert.AreEqual("Set Variable to Form 1:Q2:a", process.Lines[0].ToString());
		}

		[Test]
		public void AddAlternateLabel()
		{
			// create SET statement ('Set Variable to <<Q1:a>>')
			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new Variable("Variable");

			setStatement.Expression = new Expression("<<Q1:a>>");

			// make process line list from SET statement and add to process
			process.Lines.Add(new ProcessLineList(setStatement));

			Assert.AreEqual("Set Variable to Form 1:Q1:a", process.Lines[0].ToString());

			// add alternate label to FIB item
			IForm form = Project.Current.GetForm("Form 1");
			((FibItem)form.ItemList[0]).BlankList[0].AlternateLabel = "Name";

			Assert.AreEqual("Set Variable to Form 1:Name", process.Lines[0].ToString());
		}

		private void addGetAndForEachToProcess()
		{
			FormList formList = new FormList();
			formList.Add(form);

			recordList = new RecordSet("record list", formList);
			GetStatement getStatement = new GetStatement(recordList);
			process.Lines.Add(new ProcessLineList(getStatement));

			record = new Record("record");
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			process.Lines.Add(new ProcessLineList(forEachStatement));

			int i = 0;
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}

		[Test]
		public void RemoveRecordVariable()
		{
			addGetAndForEachToProcess();

			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new Variable("record:Score");
			setStatement.Expression = new Expression("100");
			SetLine setLine = new SetLine(setStatement);

			process.Lines.Insert(3, setLine);
			//Assert.AreEqual(1, form.RecordVariables.Count);

			process.Lines.Remove(setLine);
			//Assert.AreEqual(0, form.RecordVariables.Count);
		}

		[Test]
		public void ModifyRecordVariable()
		{
			addGetAndForEachToProcess();

			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new Variable("record:Score");
			setStatement.Expression = new Expression("100");
			SetLine setLine = new SetLine(setStatement);

			process.Lines.Insert(3, setLine);
			//Assert.AreEqual(1, form.RecordVariables.Count);
			//Assert.AreEqual("Score", form.RecordVariables[0].FieldName);

			setStatement.Variable = new Variable("record:Foo");
			process.Lines.Replace(3, setStatement);
			//Assert.AreEqual(1, form.RecordVariables.Count);
			//Assert.AreEqual("Foo", form.RecordVariables[0].FieldName);
		}
	}
}
