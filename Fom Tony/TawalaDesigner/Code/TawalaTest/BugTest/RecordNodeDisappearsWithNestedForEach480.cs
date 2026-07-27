using System;
using System.IO;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Tawala.XmlSupport;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 480 (Nested FOR EACH statements causes Record node to disappear from Fields Palette).
	/// </summary>
	[TestFixture]
	public class RecordNodeDisappearsWithNestedForEach480
	{
		private FieldsPalette testPalette;

		private IForm form1;
		private IForm form2;
		private Blank blank1;
		private Blank blank2;

		private Process process;

		[SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();

			setupForms();
			setupProcess();
			setupFieldsPalette();
		}

		private void setupForms()
		{
			form1 = Project.Current.AddForm();
			FibItem fibItem1 = new FibItem();
			form1.ItemList.Add(fibItem1);
			blank1 = fibItem1.BlankList[0];

			form2 = Project.Current.AddForm();
			FibItem fibItem2 = new FibItem();
			form2.ItemList.Add(fibItem2);
			blank2 = fibItem2.BlankList[0];
		}

		private void setupProcess()
		{
			process = Project.Current.AddProcess();

			FormList formList1 = new FormList();
			formList1.Add(form1);
			RecordSet recordSet1 = new RecordSet("Record List 1", formList1);
			GetStatement getStatement1 = new GetStatement(recordSet1);
			process.Lines.Add(new ProcessLineList(getStatement1));

			ForEachRecordStatement forEachStatement1 = new ForEachRecordStatement(new Record("Record1"), recordSet1);
			process.Lines.Add(new ProcessLineList(forEachStatement1));

			Conditions conditions1 = new Conditions(blank1, HybridOperator.List[HybridOperator.Ops.equals], new Expression("Foo"));
			IfStatement ifStatement1 = new IfStatement(conditions1, true);
			process.Lines.Insert(3, new ProcessLineList(ifStatement1));

			FormList formList2 = new FormList();
			formList2.Add(form2);
			RecordSet recordSet2 = new RecordSet("Record List 2", formList2);
			GetStatement getStatement2 = new GetStatement(recordSet2);
			process.Lines.Insert(5, new ProcessLineList(getStatement2));

			ForEachRecordStatement forEachStatement2 = new ForEachRecordStatement(new Record("Record2"), recordSet2);
			process.Lines.Insert(6, new ProcessLineList(forEachStatement2));

			Conditions conditions2 = new Conditions(blank2, HybridOperator.List[HybridOperator.Ops.equals], new Expression("Bar"));
			IfStatement ifStatement2 = new IfStatement(conditions2);
			process.Lines.Insert(12, new ProcessLineList(ifStatement2));
		}

		private void setupFieldsPalette()
		{
			testPalette = new FieldsPalette();
			testPalette.Show();
		}

		[TearDown]
		public void TearDown()
		{
			testPalette.Dispose();
		}

		[Test]
		public void VerifyProcessLines()
		{
			Assert.AreEqual(17, process.Lines.Count);
			
			Assert.AreEqual("Get Record List 1 from Form 1", process.Lines[0].ToString());
			Assert.AreEqual("For Each Record1 in Record List 1", process.Lines[1].ToString());
			Assert.AreEqual("(", process.Lines[2].ToString());
			Assert.AreEqual("If Form 1:Q1:a equals \"Foo\"", process.Lines[3].ToString());
			Assert.AreEqual("(", process.Lines[4].ToString());
			Assert.AreEqual("Get Record List 2 from Form 2", process.Lines[5].ToString());
			Assert.AreEqual("For Each Record2 in Record List 2", process.Lines[6].ToString());
			Assert.AreEqual("(", process.Lines[7].ToString());
			Assert.AreEqual(")", process.Lines[8].ToString());
			Assert.AreEqual(")", process.Lines[9].ToString());
			Assert.AreEqual("Otherwise", process.Lines[10].ToString());
			Assert.AreEqual("(", process.Lines[11].ToString());
			Assert.AreEqual("If Form 2:Q1:a equals \"Bar\"", process.Lines[12].ToString());
			Assert.AreEqual("(", process.Lines[13].ToString());
			Assert.AreEqual(")", process.Lines[14].ToString());
			Assert.AreEqual(")", process.Lines[15].ToString());
			Assert.AreEqual(")", process.Lines[16].ToString());
		}

		[Test]
		public void BothRecordNodesAppearWithInsertionPointInsideNestedForEach()
		{
			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 8));
			testPalette.RefreshFieldList();

			Assert.AreEqual(5, testPalette.FieldsTreeView.Nodes.Count);
			Assert.AreEqual("Form 1", testPalette.FieldsTreeView.Nodes[0].Text);
			Assert.AreEqual("Form 2", testPalette.FieldsTreeView.Nodes[1].Text);
			Assert.AreEqual("Variables", testPalette.FieldsTreeView.Nodes[2].Text);
			Assert.AreEqual("Record1", testPalette.FieldsTreeView.Nodes[3].Text);
			Assert.AreEqual("Record2", testPalette.FieldsTreeView.Nodes[4].Text);
		}

		[Test]
		public void RecordNodeAppearsWithInsertionPointOutsideSecondNestedIf()
		{
			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 12));
			testPalette.RefreshFieldList();

			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);
			Assert.AreEqual("Form 1", testPalette.FieldsTreeView.Nodes[0].Text);
			Assert.AreEqual("Form 2", testPalette.FieldsTreeView.Nodes[1].Text);
			Assert.AreEqual("Variables", testPalette.FieldsTreeView.Nodes[2].Text);
			Assert.AreEqual("Record1", testPalette.FieldsTreeView.Nodes[3].Text);
		}

		[Test]
		public void RecordNodeAppearsWithInsertionPointInsideSecondNestedIf()
		{
			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 14));
			testPalette.RefreshFieldList();

			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);
			Assert.AreEqual("Form 1", testPalette.FieldsTreeView.Nodes[0].Text);
			Assert.AreEqual("Form 2", testPalette.FieldsTreeView.Nodes[1].Text);
			Assert.AreEqual("Variables", testPalette.FieldsTreeView.Nodes[2].Text);
			Assert.AreEqual("Record1", testPalette.FieldsTreeView.Nodes[3].Text);
		}
	}
}
