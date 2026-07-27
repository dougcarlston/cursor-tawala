using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing Pasting functionality in the Process
	/// </summary>
	[TestFixture]
	public class ProcessPasteTest
	{
		private IForm form;
		private Process process;
		private int selectedIndex;
		private FibItem fibItem;

		private static ProcessLineList clipboardLines;

		protected void setDataObject(ProcessLineList lines)
		{
			clipboardLines = lines;
		}

		protected ProcessLineList getData()
		{
			// return a shallow copy of line list
			return clipboardLines.Copy();
		}

		private void Copy()
		{
			ProcessLineList lines = new ProcessLineList(selectedIndex, process.Lines);
			setDataObject(lines);
		}

		void Cut()
		{
			ProcessLine line = process.Lines[selectedIndex];

			if (line.IsDeletable)
			{
				ProcessLineList lines = new ProcessLineList(selectedIndex, process.Lines);
				setDataObject(lines);

				// if selection of this line selects a group
				if (line.SelectsGroup)
				{
					// remove all lines in group, starting at index
					process.Lines.Remove(selectedIndex, line.Group);
				}
				else
				{
					// remove this line only
					process.Lines.RemoveAt(selectedIndex);
				}
			}
		}

		void Paste()
		{
			ProcessLineList lines = getData();
			process.Lines.Insert(selectedIndex, lines);
		}

		void Delete()
		{
			ProcessLine line = process.Lines[selectedIndex];

			if (line.IsDeletable)
			{
				// if selection of this line selects a group
				if (line.SelectsGroup)
				{
					// remove all lines in group, starting at index
					process.Lines.Remove(selectedIndex, line.Group);
				}
				else
				{
					// remove this line only
					process.Lines.RemoveAt(selectedIndex);
				}
			}
		}

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
			fibItem = new FibItem();
			form.ItemList.Add(fibItem);
		}

		[Test]
		public void CopySinglePaste()
		{
			// create IF statement ('If Q1:a is not blank')
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(fibItem, HybridOperator.List["is not blank"]);

			// make process line list from IF statement and add to process
			process.Lines.Add(new ProcessLineList(ifStatement));

			selectedIndex = 0;
			Copy();
			Paste();

			Assert.AreEqual(6, process.Lines.Count);
			Assert.IsFalse(((IfLine)process.Lines[0]).Statement == ((IfLine)process.Lines[3]).Statement);
		}

		[Test]
		public void CopyMultiplePaste()
		{
			// create IF statement ('If Q1:a is not blank')
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(fibItem, HybridOperator.List["is not blank"]);

			// make process line list from IF statement and add to process
			process.Lines.Add(new ProcessLineList(ifStatement));

			selectedIndex = 0;
			Copy();
			Paste();
			Paste();

			Assert.AreEqual(9, process.Lines.Count);
			Assert.IsFalse(((IfLine)process.Lines[0]).Statement == ((IfLine)process.Lines[3]).Statement);
			Assert.IsFalse(((IfLine)process.Lines[0]).Statement == ((IfLine)process.Lines[6]).Statement);
			Assert.IsFalse(((IfLine)process.Lines[3]).Statement == ((IfLine)process.Lines[6]).Statement);
		}

		[Test]
		public void CopyNestedPaste()
		{
			// create IF statement ('If Q1:a is not blank')
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(fibItem, HybridOperator.List["is not blank"]);

			// make process line list from IF statement and add to process
			process.Lines.Add(new ProcessLineList(ifStatement));

			selectedIndex = 0;
			Copy();
			selectedIndex = 2;
			Paste();

			Assert.AreEqual(6, process.Lines.Count);
			Assert.IsFalse(process.Lines[0] == process.Lines[2]);
			Assert.IsFalse(((IfLine)process.Lines[0]).Statement == ((IfLine)process.Lines[2]).Statement);
		}
		
		[Test]
		public void CopyMultiNestedPaste()
		{
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(fibItem, HybridOperator.List["is not blank"]);

			process.Lines.Add(new ProcessLineList(ifStatement));

			// paste a copy the If statement inside the block of the original statement
			selectedIndex = 0;
			Copy();
			selectedIndex = 2;
			Paste();

			// copy the entire group of nested lines
			selectedIndex = 0;
			Copy();
			Paste();

			Assert.AreEqual(12, process.Lines.Count);

			// the copied block:
			Assert.IsTrue(process.Lines[0] is IfLine);
			Assert.IsTrue(process.Lines[1] is BlockOpenLine);
			Assert.IsTrue(process.Lines[2] is IfLine);
			Assert.IsTrue(process.Lines[3] is BlockOpenLine);
			Assert.IsTrue(process.Lines[4] is BlockCloseLine);
			Assert.IsTrue(process.Lines[5] is BlockCloseLine);

			// the original block:
			Assert.IsTrue(process.Lines[6] is IfLine);
			Assert.IsTrue(process.Lines[7] is BlockOpenLine);
			Assert.IsTrue(process.Lines[8] is IfLine);
			Assert.IsTrue(process.Lines[9] is BlockOpenLine);
			Assert.IsTrue(process.Lines[10] is BlockCloseLine);
			Assert.IsTrue(process.Lines[11] is BlockCloseLine);

			// check for unique instances of all of the If Process lines
			Assert.IsFalse(process.Lines[0] == process.Lines[6]);
			Assert.IsFalse(process.Lines[0] == process.Lines[8]);
			Assert.IsFalse(process.Lines[2] == process.Lines[6]);
			Assert.IsFalse(process.Lines[2] == process.Lines[8]);

			// and unique instances of all of the contained If statements
			Assert.IsFalse(((IfLine)process.Lines[0]).Statement == ((IfLine)process.Lines[6]).Statement);
			Assert.IsFalse(((IfLine)process.Lines[0]).Statement == ((IfLine)process.Lines[8]).Statement);
			Assert.IsFalse(((IfLine)process.Lines[2]).Statement == ((IfLine)process.Lines[6]).Statement);
			Assert.IsFalse(((IfLine)process.Lines[2]).Statement == ((IfLine)process.Lines[8]).Statement);
		}
		
		[Test]
		public void CopyNestedPasteDelete()
		{
			// create IF statement ('If Q1:a is not blank')
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(fibItem, HybridOperator.List["is not blank"]);

			// make process line list from IF statement and add to process
			process.Lines.Add(new ProcessLineList(ifStatement));

			selectedIndex = 0;
			Copy();
			selectedIndex = 2;
			Paste();

			selectedIndex = 2;
			Delete();

			Assert.AreEqual(3, process.Lines.Count);
			Assert.IsTrue(process.Lines[0] is IfLine);
			Assert.IsTrue(process.Lines[1] is BlockOpenLine);
			Assert.IsTrue(process.Lines[2] is BlockCloseLine);
		}

		[Test]
		public void CutSinglePaste()
		{
			// create IF statement ('If Q1:a is not blank')
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(fibItem, HybridOperator.List["is not blank"]);

			// make process line list from IF statement and add to process
			process.Lines.Add(new ProcessLineList(ifStatement));

			selectedIndex = 0;
			Cut();
			Paste();

			Assert.AreEqual(3, process.Lines.Count);
			Assert.IsTrue(process.Lines[0] is IfLine);
		}

		[Test]
		public void CutMultiplePaste()
		{
			// create IF statement ('If Q1:a is not blank')
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(fibItem, HybridOperator.List["is not blank"]);

			// make process line list from IF statement and add to process
			process.Lines.Add(new ProcessLineList(ifStatement));

			selectedIndex = 0;
			Cut();
			Paste();
			Paste();

			Assert.AreEqual(6, process.Lines.Count);
			Assert.IsFalse(((IfLine)process.Lines[0]).Statement == ((IfLine)process.Lines[3]).Statement);
		}


	}
}
