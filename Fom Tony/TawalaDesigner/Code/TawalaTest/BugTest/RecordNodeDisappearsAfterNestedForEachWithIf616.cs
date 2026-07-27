using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for Mantis issue 616 (Record node not available in fields palette after nested foreach with IF).
	/// </summary>
	[TestFixture]
	public class RecordNodeDisappearsAfterNestedForEachWithIf616
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			Util.NewTestProject();

			setupForms();
			setupProcess();
			setupFieldsPalette();
		}

		[TearDown]
		public void TearDown()
		{
			testPalette.Dispose();
		}

		#endregion

		private FieldsPalette testPalette;

		private IForm form1;
		private IForm form2;
		private Blank blank1;

		private Process process;

		private void setupForms()
		{
			form1 = Project.Current.AddForm();
			var fibItem1 = new FibItem();
			form1.ItemList.Add(fibItem1);
			blank1 = fibItem1.BlankList[0];

			form2 = Project.Current.AddForm();
			var fibItem2 = new FibItem();
			form2.ItemList.Add(fibItem2);
		}

		private void setupProcess()
		{
			process = Project.Current.AddProcess();

			var formList1 = new FormList();
			formList1.Add(form1);
			var recordSet1 = new RecordSet("Record List 1", formList1);
			var getStatement1 = new GetStatement(recordSet1);
			process.Lines.Add(new ProcessLineList(getStatement1));

			var forEachStatement1 = new ForEachRecordStatement(new Record("Record1"), recordSet1);
			process.Lines.Add(new ProcessLineList(forEachStatement1));

			var formList2 = new FormList();
			formList2.Add(form2);
			var recordSet2 = new RecordSet("Record List 2", formList2);
			var getStatement2 = new GetStatement(recordSet2);
			process.Lines.Insert(3, new ProcessLineList(getStatement2));

			var forEachStatement2 = new ForEachRecordStatement(new Record("Record2"), recordSet2);
			process.Lines.Insert(4, new ProcessLineList(forEachStatement2));

			var conditions = new Conditions(blank1, HybridOperator.List[HybridOperator.Ops.equals], new Expression("Foo"));
			var ifStatement = new IfStatement(conditions, false);
			process.Lines.Insert(6, new ProcessLineList(ifStatement));
		}

		private void setupFieldsPalette()
		{
			testPalette = new FieldsPalette();
			testPalette.Show();
		}

		[Test]
		public void VerifyProcessLines()
		{
			Assert.AreEqual(11, process.Lines.Count);

			Assert.AreEqual("Get Record List 1 from Form 1", process.Lines[0].ToString());
			Assert.AreEqual("For Each Record1 in Record List 1", process.Lines[1].ToString());
			Assert.AreEqual("(", process.Lines[2].ToString());
			Assert.AreEqual("Get Record List 2 from Form 2", process.Lines[3].ToString());
			Assert.AreEqual("For Each Record2 in Record List 2", process.Lines[4].ToString());
			Assert.AreEqual("(", process.Lines[5].ToString());
			Assert.AreEqual("If Form 1:Q1:a equals \"Foo\"", process.Lines[6].ToString());
			Assert.AreEqual("(", process.Lines[7].ToString());
			Assert.AreEqual(")", process.Lines[8].ToString());
			Assert.AreEqual(")", process.Lines[9].ToString());
			Assert.AreEqual(")", process.Lines[10].ToString());
		}

		[Test]
		public void OneRecordNodeForInsertionPointBelowInnerForEach()
		{
			SetInsertionPointAtProcessLineIndex(10);
			VerifyOneRecordNodeInFieldsPalette();
		}

		// Note: The above test is for the stated issue. I created the following to make sure we have decent coverage.
		//       The following tests are added to make sure we have sufficient coverage for the change I'm making to address the issue.
		//       jdf - 5/09
		[Test]
		public void NoRecordNodeForInsertionPointAboveOuterForEach()
		{
			SetInsertionPointAtProcessLineIndex(1);
			VerifyNoRecordNodesInFieldsPalette();
		}

		[Test]
		public void NoRecordNodeForInsertionPointBelowOuterForEach()
		{
			SetInsertionPointAtProcessLineIndex(11);
			VerifyNoRecordNodesInFieldsPalette();
		}

		[Test]
		public void OneRecordNodeForInsertionPointAboveInnerForEach()
		{
			SetInsertionPointAtProcessLineIndex(4);
			VerifyOneRecordNodeInFieldsPalette();
		}

		[Test]
		public void TwoRecordNodesForInsertionPointAboveNestedIf()
		{
			SetInsertionPointAtProcessLineIndex(6);
			VerifyTwoRecordNodesInFieldsPalette();
		}

		[Test]
		public void TwoRecordNodesForInsertionPointBelowNestedIf()
		{
			SetInsertionPointAtProcessLineIndex(9);
			VerifyTwoRecordNodesInFieldsPalette();
		}

		[Test]
		public void TwoRecordNodesForInsertionPointInsideNestedIf()
		{
			SetInsertionPointAtProcessLineIndex(8);
			VerifyTwoRecordNodesInFieldsPalette();
		}

		private void SetInsertionPointAtProcessLineIndex(int index)
		{
			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, index));
			testPalette.RefreshFieldList();
		}

		private void VerifyNoRecordNodesInFieldsPalette()
		{
			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);
			Assert.AreEqual("Form 1", testPalette.FieldsTreeView.Nodes[0].Text);
			Assert.AreEqual("Form 2", testPalette.FieldsTreeView.Nodes[1].Text);
			Assert.AreEqual("Variables", testPalette.FieldsTreeView.Nodes[2].Text);
		}

		private void VerifyOneRecordNodeInFieldsPalette()
		{
			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);
			Assert.AreEqual("Form 1", testPalette.FieldsTreeView.Nodes[0].Text);
			Assert.AreEqual("Form 2", testPalette.FieldsTreeView.Nodes[1].Text);
			Assert.AreEqual("Variables", testPalette.FieldsTreeView.Nodes[2].Text);
			Assert.AreEqual("Record1", testPalette.FieldsTreeView.Nodes[3].Text);
		}

		private void VerifyTwoRecordNodesInFieldsPalette()
		{
			Assert.AreEqual(5, testPalette.FieldsTreeView.Nodes.Count);
			Assert.AreEqual("Form 1", testPalette.FieldsTreeView.Nodes[0].Text);
			Assert.AreEqual("Form 2", testPalette.FieldsTreeView.Nodes[1].Text);
			Assert.AreEqual("Variables", testPalette.FieldsTreeView.Nodes[2].Text);
			Assert.AreEqual("Record1", testPalette.FieldsTreeView.Nodes[3].Text);
			Assert.AreEqual("Record2", testPalette.FieldsTreeView.Nodes[4].Text);
		}
	}
}