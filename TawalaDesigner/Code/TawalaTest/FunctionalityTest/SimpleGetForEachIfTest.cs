using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing the simple GET statement with FOR EACH and IF statements
	/// </summary>
	[TestFixture]
	public class SimpleGetForEachIfTest
	{
		private IForm form;
		private McqItem mcItem;
		private Process process;
		private FormList forms;
		private RecordSet recordList1;
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
			form = Project.Current.AddForm();

			// create process
			process = Project.Current.AddProcess();

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			// add new FIB item to form
			form.ItemList.Add(new FibItem());

			// add new MC item to form
			mcItem = new McqItem();
			form.ItemList.Add(mcItem);

			// create GET statement ('Get record list from Form 1')
			forms = new FormList();
			forms.Add((Form)Project.Current.FormList[0]);
			recordList1 = new RecordSet("record list", forms);
			GetStatement getStatement = new GetStatement(recordList1);
			process.Lines.Add(new ProcessLineList(getStatement));

			// create FOR EACH statement ('For Each record in record list')
			record1 = new Record("record");
			ProcessLineList forEachLines1 = getForEachLines(recordList1, record1);
			process.Lines.Add(forEachLines1);
		}

		private static ProcessLineList getForEachLines(RecordSet recordList, Record record)
		{
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			return forEachLines;
		}

		[Test]
		public void CheckQualifiedFieldAfterReposition()
		{
			ProcessLineList ifLines = getIfWithQualifiedField();

			// place 'If' lines within 'For Each' lines
			process.Lines.Insert(3, ifLines);

			int i = 0;
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If record:Q1:a equals Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			form.ItemList.Insert(0, new McqItem());

			Assert.AreEqual("If record:Q2:a equals Form 1:Q2:a", process.Lines[3].ToString());
		}

		private ProcessLineList getIfWithQualifiedField()
		{
			// create process line 'If record:Q1:a equals Q1:a'
			QualifiedFieldList qualifiedField = new QualifiedFieldList(record1, form.GetFields()["Q1:a"]);
			Blank blank = ((FibItem)form.ItemList[0]).BlankList[0];
			Expression expression = new Expression(blank);
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(qualifiedField, HybridOperator.List[HybridOperator.Ops.equals], expression);
			ProcessLineList ifLines = new ProcessLineList(ifStatement);
			return ifLines;
		}

		private ProcessLineList getIfWithQualifiedMCField()
		{
			// create process line 'If Q2 equals record:Q2'
			IfStatement ifStatement = new IfStatement();
			Expression expression = new Expression(new RecordField(record1, mcItem));
			ifStatement.Conditions = new Conditions(form.GetFields()["Q2"], MCOneOperator.List[MCOneOperator.Ops.mcEquals], expression);
			ProcessLineList ifLines = new ProcessLineList(ifStatement);
			return ifLines;
		}

		[Test]
		public void CheckQualifiedExpressionAfterReposition()
		{
			ProcessLineList ifLines = getIfWithQualifiedExpression();

			// place 'If' lines within 'For Each' lines
			process.Lines.Insert(3, ifLines);

			int i = 0;
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If Form 1:Q1:a equals record:Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			form.ItemList.Insert(0, new McqItem());

			Assert.AreEqual("If Form 1:Q2:a equals record:Form 1:Q2:a", process.Lines[3].ToString());
		}

		private ProcessLineList getIfWithQualifiedExpression()
		{
			// create process line 'If Q1:a equals record:Q1:a'
			IPaletteField field = (IPaletteField)form.GetFields()["Q1:a"];
			Expression qualifiedExpression = new Expression(new RecordField(record1, (IPaletteField)form.GetFields()["Q1:a"]));
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(field, HybridOperator.List[HybridOperator.Ops.equals], qualifiedExpression);
			ProcessLineList ifLines = new ProcessLineList(ifStatement);
			return ifLines;
		}

		[Test]
		public void ConsecutiveForEachStatements()
		{
			ProcessLineList forEachLines2 = getForEachLines(recordList1, record1);
			ProcessLineList forEachLines3 = getForEachLines(recordList1, record1);
			ProcessLineList forEachLines4 = getForEachLines(recordList1, record1);

			process.Lines.Add(forEachLines2);
			process.Lines.Add(forEachLines3);
			process.Lines.Add(forEachLines4);

			process.Lines.Insert(3, getIfWithQualifiedExpression());
			process.Lines.Insert(9, getIfWithQualifiedExpression());
			process.Lines.Insert(15, getIfWithQualifiedField());
			process.Lines.Insert(21, getIfWithQualifiedField());

			int i = 0;
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If Form 1:Q1:a equals record:Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If Form 1:Q1:a equals record:Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If record:Q1:a equals Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If record:Q1:a equals Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}

		[Test]
		public void QualifiedMC()
		{
			ProcessLineList ifLines = getIfWithQualifiedMCField();

			// place 'If' lines within 'For Each' lines
			process.Lines.Insert(3, ifLines);

			int i = 0;
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If Form 1:Q2 equals record:Form 1:Q2", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}


	}
}
