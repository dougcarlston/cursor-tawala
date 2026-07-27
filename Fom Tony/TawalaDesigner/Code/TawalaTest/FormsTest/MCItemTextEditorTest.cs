using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Tawala.Projects;
using Tawala.Forms;
using TXTextControl;
using NUnit.Framework;
//using NUnit.Extensions.Forms;

namespace TawalaTest.FormsTest
{
	[TestFixture]
	public class MCItemTextEditorTest// : NUnitFormTest
	{
		//private class MCItemTextEditorTester : NUnit.Extensions.Forms.ControlTester
		//{
		//    public MCItemTextEditorTester(string name) : base(name)
		//    {
		//    }
		//}

		private McqItemTextEditor textEditor;
		//private MCItemTextEditorTester textEditorTester;

		private TXTextControl.TextControl txTextControl;
		//private NUnit.Extensions.Forms.ControlTester txTextControlTester;

		private TXTextControl.TextControl getTxControl(object o)
		{
			Type t = typeof(Tawala.TextEditor.TextEdit);
			FieldInfo fi = t.GetField("txTextControl", BindingFlags.Instance | BindingFlags.NonPublic);
			return fi.GetValue(o) as TXTextControl.TextControl;
		}

		private void fireKeyDownEventInTxControl(Keys key)
		{
			KeyEventArgs args = new KeyEventArgs(key);
			//txTextControlTester.FireEvent("KeyDown", args);
		}

		private void fireKeyPressEventInTxControl(char key)
		{
			KeyPressEventArgs args = new KeyPressEventArgs(key);
			//txTextControlTester.FireEvent("KeyPress", args);
		}

		[SetUp]
		public /*override*/ void Setup()
		{
			//base.Setup();

			System.Windows.Forms.Form windowsForm = new System.Windows.Forms.Form();

			textEditor = new McqItemTextEditor();
			textEditor.Name = "testTextEditor";
			windowsForm.Controls.Add(textEditor);

			//textEditorTester = new MCItemTextEditorTester("testTextEditor");
			
			windowsForm.Show();
			
			txTextControl = getTxControl(textEditor);
			//txTextControlTester = new NUnit.Extensions.Forms.ControlTester(txTextControl.Name);
		}

		[Test]
		public void EditorIsInitiallyEmpty()
		{
			Assert.AreEqual("", textEditor.GetCleanText());
		}

		[Test]
		public void SettingTextRetainsText()
		{
			string textString =
				"Choose one:" +
				"\n   a) ";

			textEditor.SetText(textString);

			Assert.AreEqual(textString, textEditor.GetCleanText());
		}

		[Test]
		[Ignore("Ignored due to reliance on NUnitForms - SB 03/11/2008")]
		public void EnterKeyAddsLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) ";

			textEditor.SetText(textString);

			textEditor.Select(textString.Length, 0);

			fireKeyPressEventInTxControl('\r');

			string expectedString =
				"Choose one:" +
				"\n   a) " +
				"\n   b) ";

			Assert.AreEqual(expectedString, textEditor.GetCleanText());
			Assert.AreEqual(textEditor.Lines.Count, 3);
		}

		[Test]
		[Ignore("Ignored due to reliance on NUnitForms - SB 03/11/2008")]
		public void EnterKeyAddsThirdLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) " +
				"\n   b) ";

			textEditor.SetText(textString);

			textEditor.Select(textString.Length, 0);

			fireKeyPressEventInTxControl('\r');

			string expectedString =
				"Choose one:" +
				"\n   a) " +
				"\n   b) " +
				"\n   c) ";

			Assert.AreEqual(expectedString, textEditor.GetCleanText());
			Assert.AreEqual(textEditor.Lines.Count, 4);
		}

		[Test]
		[Ignore("Ignored due to reliance on NUnitForms - SB 03/11/2008")]
		public void EnterKeyInsertsLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) First" +
				"\n   b) Second";

			textEditor.SetText(textString);

			string searchString = "\n   a) First";
			textEditor.Select(textString.IndexOf(searchString) + searchString.Length, 0);

			fireKeyPressEventInTxControl('\r');

			string expectedString =
				"Choose one:" +
				"\n   a) First" +
				"\n   b) " +
				"\n   c) Second";

			Assert.AreEqual(expectedString, textEditor.GetCleanText());
			Assert.AreEqual(textEditor.Lines.Count, 4);
		}

		[Test]
		public void LabelLineReturnsChoiceLineBeginnings()
		{
			Assert.AreEqual("\n   a) ", textEditor.LabelLine(0));
			Assert.AreEqual("\n   b) ", textEditor.LabelLine(1));
			Assert.AreEqual("\n   c) ", textEditor.LabelLine(2));
		}

		[Test]
		public void SelectYieldsProperSelectedText()
		{
			string textString =
				"Choose one:" +
				"\n   a) ";

			textEditor.SetText(textString);

			string searchString = "\n   a) ";
			textEditor.Select(textString.IndexOf(searchString), searchString.Length);

			Assert.AreEqual(11, textEditor.SelectionStart);
			Assert.AreEqual(7, textEditor.SelectionLength);

			Assert.AreEqual(searchString, textEditor.SelectedText);
		}

		[Test]
		public void SelectedTextYieldsProperSelectedText()
		{
			string textString =
				"Choose one:" +
				"\n   a) ";

			textEditor.SetText(textString);

			string searchString = "\n   a) ";
			textEditor.Select(textString.IndexOf(searchString), searchString.Length);

			textEditor.SelectedText = "\n   b) ";

			string expectedString =
				"Choose one:" +
				"\n   b) ";

			Assert.AreEqual(expectedString, textEditor.SelectedText);
		}

#if false		// couldn't get this test to succeed; but the behavior works in the UI - jdf 12/06
		[Test]
		public void UpdateLabelAlphaYieldsProperSelectedText()
		{
			string textString =
				"Choose one:" +
				"\n   a) ";

			textEditor.SetText(textString);

			string searchString = "\n";
			int index = textString.IndexOf(searchString);
			textEditorTester.Invoke("updateLabelAlpha", index, 0);

			Assert.AreEqual("   a", textEditor.SelectedText);
			Assert.AreEqual(textString.IndexOf(searchString) + 1, textEditor.SelectionStart);
			Assert.AreEqual(4, textEditor.SelectionLength);
		}
#endif

		[Test]
		public void SelectRecognizesLF()
		{
			string textString =
				"\n123456789abcde";

			textEditor.SetText(textString);

			textEditor.Select(9, 5);

			Assert.AreEqual("9abcd", textEditor.SelectedText);
			Assert.AreEqual(9, textEditor.SelectionStart);
			Assert.AreEqual(5, textEditor.SelectionLength);
		}

		[Test]
		[Ignore("Ignored due to reliance on NUnitForms - SB 03/11/2008")]
		public void BackspaceAfterLabelRemovesLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) " +
				"\n   b) ";

			textEditor.SetText(textString);

			textEditor.Select(textString.Length, 0);

			fireKeyDownEventInTxControl(Keys.Back);

			string expectedString =
				"Choose one:" +
				"\n   a) ";

			Assert.AreEqual(expectedString, textEditor.GetCleanText());
			Assert.AreEqual(textEditor.Lines.Count, 2);
		}

		[Test]
		public void BackspaceAfterFirstAndOnlyLabelRetainsLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) ";

			textEditor.SetText(textString);

			textEditor.Select(textString.Length, 0);

			fireKeyDownEventInTxControl(Keys.Back);

			string expectedString =
				"Choose one:" +
				"\n   a) ";

			Assert.AreEqual(expectedString, textEditor.GetCleanText());
			Assert.AreEqual(textEditor.Lines.Count, 2);
		}

		[Test]
		[Ignore("Ignored due to reliance on NUnitForms - SB 03/11/2008")]
		public void BackspaceWithSelectionIncludingLabelRenamesRemainingLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) Choice uno" +
				"\n   b) Choice dos" +
				"\n   c) Choice tres";

			textEditor.SetText(textString);

			int startIndex = textString.IndexOf(" uno");
			int endIndex = textString.IndexOf(" dos");

			textEditor.Select(startIndex, endIndex - startIndex);

			fireKeyDownEventInTxControl(Keys.Back);

			string expectedString =
				"Choose one:" +
				"\n   a) Choice dos" +
				"\n   b) Choice tres";

			Assert.AreEqual(expectedString, textEditor.GetCleanText());
			Assert.AreEqual(textEditor.Lines.Count, 3);
		}

		[Test]
		[Ignore("Ignored due to reliance on NUnitForms - SB 03/11/2008")]
		public void DeleteBeforeLabelRenamesRemainingLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) " +
				"\n   b) ";

			textEditor.SetText(textString);

			textEditor.Select(textString.IndexOf("\n"), 0);

			fireKeyDownEventInTxControl(Keys.Delete);

			string expectedString =
				"Choose one:" +
				"\n   a) ";

			Assert.AreEqual(expectedString, textEditor.GetCleanText());
			Assert.AreEqual(textEditor.Lines.Count, 2);
		}
		
		[Test]
		public void DeleteBeforeFirstAndOnlyLabelRetainsLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) ";

			textEditor.SetText(textString);

			textEditor.Select(textString.IndexOf("\n"), 0);

			fireKeyDownEventInTxControl(Keys.Delete);

			string expectedString =
				"Choose one:" +
				"\n   a) ";

			Assert.AreEqual(expectedString, textEditor.GetCleanText());
			Assert.AreEqual(textEditor.Lines.Count, 2);
		}

		[Test]
		[Ignore("Ignored due to reliance on NUnitForms - SB 03/11/2008")]
		public void DeletingSelectionWithLabelRenamesRemainingLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) Choice uno" +
				"\n   b) Choice dos" +
				"\n   c) Choice tres";

			textEditor.SetText(textString);

			int startIndex = textString.IndexOf(" uno");
			int endIndex = textString.IndexOf(" dos");

			textEditor.Select(startIndex, endIndex - startIndex);

			fireKeyDownEventInTxControl(Keys.Delete);

			string expectedString =
				"Choose one:" +
				"\n   a) Choice dos" +
				"\n   b) Choice tres";

			Assert.AreEqual(expectedString, textEditor.GetCleanText());
			Assert.AreEqual(textEditor.Lines.Count, 3);
		}

		[Test]
		public void SettingInsertionPointInLabelMovesInsertionPointPastLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) " +
				"\n   b) ";

			textEditor.SetText(textString);

			int selectionIndexInLabel = textString.IndexOf("   a) ");
			textEditor.Select(selectionIndexInLabel, 0);

			int expectedIndex = selectionIndexInLabel + 6;

			Assert.AreEqual(expectedIndex, textEditor.SelectionStart);
		}

		[Test]
		public void SelectionsEntirelyInLabelMovesInsertionPointPastLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) " +
				"\n   b) ";

			textEditor.SetText(textString);

			int selectionIndexInLabel = textString.IndexOf("   a) ");
			textEditor.Select(selectionIndexInLabel, 6);

			int expectedIndex = selectionIndexInLabel + 6;

			Assert.AreEqual(expectedIndex, textEditor.SelectionStart);
			Assert.AreEqual(0, textEditor.SelectionLength);

			textEditor.Select(selectionIndexInLabel + 2, 2);
			Assert.AreEqual(expectedIndex, textEditor.SelectionStart);
			Assert.AreEqual(0, textEditor.SelectionLength);
		}

#if false		// couldn't get this test to succeed; but the behavior works in the UI - jdf 12/06
		[Test]
		public void RightArrowBeforeLabelMovesInsertionPointPastLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) " +
				"\n   b) ";

			textEditor.SetText(textString);

			int selectionIndexBeforeLabel = textString.IndexOf("\n   a) ");
			textEditor.Select(selectionIndexBeforeLabel, 0);

			fireKeyDownEventInTxControl(Keys.Right);

			txTextControlTester.FireEvent("InputPositionChanged", EventArgs.Empty);

			int expectedIndex = selectionIndexBeforeLabel + 7;

			Assert.AreEqual(expectedIndex, textEditor.SelectionStart);
		}
#endif

		[Test]
		[Ignore("Ignored due to reliance on NUnitForms - SB 03/11/2008")]
		public void LeftArrowAfterLabelMovesInsertionPointBeforeLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) " +
				"\n   b) ";

			textEditor.SetText(textString);

			int selectionIndexAfterLabel = textString.IndexOf("\n   a) ") + 7;
			textEditor.Select(selectionIndexAfterLabel, 0);

			fireKeyDownEventInTxControl(Keys.Left);

			//txTextControlTester.FireEvent("InputPositionChanged", EventArgs.Empty);

			int expectedIndex = selectionIndexAfterLabel - 7;

			Assert.AreEqual(expectedIndex, textEditor.SelectionStart);
		}

		[Test]
		[Ignore("Ignored due to reliance on NUnitForms - SB 03/11/2008")]
		public void EnterKeyWithNewlineSelectedInsertsLabel()
		{
			string textString =
				"Choose one:" +
				"\n   a) " +
				"\n   b) ";

			textEditor.SetText(textString);

			int selectionIndexAfterLabel = textString.IndexOf("\n   a) ") + 7;
			textEditor.Select(selectionIndexAfterLabel, 1);

			fireKeyPressEventInTxControl('\r');

			string expectedString =
				"Choose one:" +
				"\n   a) " +
				"\n   b) " +
				"\n   c) ";

			Assert.AreEqual(expectedString, textEditor.GetCleanText());
			Assert.AreEqual(textEditor.Lines.Count, 4);
		}

		// add tests at TextEdit level for new Key related methods
		// tests where selection includes label
	}
}
