// $Workfile: TextEditTest.cs $
// $Revision: 60 $	$Date: 5/01/07 6:29p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.TextEditor;

namespace TawalaTest.TextEditorTest
{
	[TestFixture]
	public class TextEditTest : TextEditTestBase
	{
		[Test]
		public void AssumptionsAboutInitialState()
		{
			Assert.IsTrue(editor is ITextEdit);
			Assert.IsNotNull(editor.Selection);
			Assert.AreEqual(0, txTextControl.TextChars.Count);
			Assert.IsFalse(editor.CanCopy);
			Assert.IsFalse(editor.CanCut);
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20\par }";
			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		#region Clipboard

		[Test]
		public void CanCopy_Property()
		{
			editor.Selection.Text = "Gargle";
			editor.Selection.Length = 0;
			Assert.IsFalse(editor.CanCopy);
			editor.SelectAll();
			Assert.IsTrue(editor.CanCopy);
		}

		[Test]
		public void CanCut_Property()
		{
			editor.Selection.Text = "Gargle";
			editor.Selection.Length = 0;
			Assert.IsFalse(editor.CanCut);
			editor.SelectAll();
			Assert.IsTrue(editor.CanCut);
		}

		[Test]
		public void CanPaste_Property()
		{
			Assert.IsFalse(editor.CanPaste);

			Clipboard.SetText("Some text");
			Assert.IsTrue(editor.CanPaste);
		}

		[Test]
		public void Copy_Method()
		{
			Assert.IsFalse(editor.CanCopy);
			Assert.IsFalse(editor.CanPaste);
			editor.Selection.Text = "Walk the dog.";
			editor.Selection.Start = 1;
			editor.Selection.Length = 2;
			editor.Copy();
			Assert.IsTrue(editor.CanPaste);
		}

		[Test]
		public void Cut_Method()
		{
			Assert.IsFalse(editor.CanCut);
			Assert.IsFalse(editor.CanPaste);
			editor.Selection.Text = "Walk the dog.";
			editor.Selection.Start = 1;
			editor.Selection.Length = 2;
			editor.Copy();
			Assert.IsTrue(editor.CanPaste);
		}

		[Test]
		public void Paste_Method()
		{
			Clipboard.SetText("Some text");
			Assert.AreEqual("Some text", Clipboard.GetText());
			editor.Paste();
			editor.SelectAll();
			Assert.AreEqual("Some text", editor.Selection.Text);
		}

		#endregion

		private int raisedChange = -2;

		[Test]
		public void ForceRaiseChange()
		{
			raisedChange = 0;
			Assert.AreEqual(0, raisedChange);
			editor.Changed += new EventHandler(delegate { ++raisedChange; });
			editor.ForceOnChanged();
			Assert.AreEqual(1, raisedChange);
		}
	
		[Test]
		public void Changed_Event()
		{
			// modifying Text through Text property does not fire Changed event
			changedCount = 0;
			editor.Changed += new EventHandler(delegate { ++changedCount; });
			Assert.AreEqual(0, changedCount);

			// modifying Text through Selection does fire changed event
			// this is akin to user input where the Selection equals selected
			// text or a 0 length selection is an insertion point
			editor.Selection.Text = "Test";
			Assert.AreEqual(1, changedCount);
		}

		private int changedCount = 0;

		[Test]
		public void ClearAll_Method()
		{
			Assert.IsTrue(txTextControl.TextChars.Count == 0);
			editor.Selection.Text = "Walk the dog.";
			Assert.IsTrue(txTextControl.TextChars.Count == 13);
			editor.ClearAll();
			Assert.IsTrue(txTextControl.TextChars.Count == 0);
		}

		[Test]
		public void Selection_Property()
		{
			// The Selection object is thoroughly tested in TextSelectionTest.cs
			// This is just a placeholder to indicate it wasn't forgotten
			Assert.IsNotNull(editor.Selection);
		}

		[Test]
		public void Select_Method()
		{
			string test = "Walk the dog.";
			editor.Selection.Text = test;

			editor.Select(3, 4);

			Assert.AreEqual(3, editor.Selection.Start);
			Assert.AreEqual(4, editor.Selection.Length);
			Assert.AreEqual("k th", editor.Selection.Text);
		}

		[Test]
		public void SelectAll_Method()
		{
			string test = "Globs of goo.";
			editor.Selection.Text = test;
			editor.Selection.Length = 0;
			Assert.AreEqual(0, editor.Selection.Length);
			Assert.IsFalse(editor.CanCopy);
			
			editor.SelectAll();
			Assert.AreEqual(test.Length, editor.Selection.Length);
			Assert.AreEqual(0, editor.Selection.Start);
			Assert.IsTrue(editor.CanCopy);
		}

		#region Bold, Italic, Underline

		[Test]
		public void ToggleBold_Method1() 
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.Bold = Tristate.True;
			editor.ToggleBold();
			Assert.AreEqual(Tristate.False, editor.Selection.Bold);
		}

		[Test]
		public void ToggleBold_Method2()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.Bold = Tristate.Undefined;
			editor.ToggleBold();
			Assert.AreEqual(Tristate.True, editor.Selection.Bold);
		}

		[Test]
		public void ToggleBold_Method3()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.Bold = Tristate.False;
			editor.ToggleBold();
			Assert.AreEqual(Tristate.True, editor.Selection.Bold);
		}

		[Test]
		public void ToggleItalic_Method1()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.Italic = Tristate.True;
			editor.ToggleItalic();
			Assert.AreEqual(Tristate.False, editor.Selection.Italic);
		}

		[Test]
		public void ToggleItalic_Method2()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.Italic = Tristate.Undefined;
			editor.ToggleItalic();
			Assert.AreEqual(Tristate.True, editor.Selection.Italic);
		}

		[Test]
		public void ToggleItalic_Method3()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.Italic = Tristate.False;
			editor.ToggleItalic();
			Assert.AreEqual(Tristate.True, editor.Selection.Italic);
		}

		[Test]
		public void ToggleUnderline_Method1()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.Underline = Tristate.True;
			editor.ToggleUnderline();
			Assert.AreEqual(Tristate.False, editor.Selection.Underline);
		}

		[Test]
		public void ToggleUnderline_Method2()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.Underline = Tristate.Undefined;
			editor.ToggleUnderline();
			Assert.AreEqual(Tristate.True, editor.Selection.Underline);
		}

		[Test]
		public void ToggleUnderline_Method3()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.Underline = Tristate.False;
			editor.ToggleUnderline();
			Assert.AreEqual(Tristate.True, editor.Selection.Underline);
		}

		#endregion

		#region Get/Set RTF
		
		private static string rtfString =
			@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
			@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
			@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
			@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
			@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
			@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
			@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0\fs20\b Walk the dog.\par }";
		
		[Test]
		public void GetRTF_Method1()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.Bold = Tristate.True;

			string rtf = editor.GetRTF();
			Assert.AreEqual(rtfString, rtf);
		}

		[Test]
		public void SetRTF_Method1()
		{
			editor.SetRTF(rtfString);
			editor.Selection.Start = 2;
			Assert.IsTrue(editor.Selection.Bold == Tristate.True);
			string rtf = editor.GetRTF();
//			Assert.AreEqual(rtfString.Replace(@"\par }", @"\par\par }"), rtf);
			Assert.AreEqual(rtfString, rtf);
		}

		#endregion

		[Test]
		public void ViewMode_Property1()
		{
			editor.ViewMode = ViewMode.Normal;
			Assert.AreEqual(ViewMode.Normal, editor.ViewMode);
			Assert.AreEqual(TXTextControl.ViewMode.Normal, txTextControl.ViewMode);
		}

		[Test]
		public void ViewMode_Property2()
		{
			editor.ViewMode = ViewMode.Unlimited;
			Assert.AreEqual(ViewMode.Unlimited, editor.ViewMode);
			Assert.AreEqual(TXTextControl.ViewMode.FloatingText, txTextControl.ViewMode);
		}

		[Test]
		public void ViewMode_Property3()
		{
			editor.ViewMode = ViewMode.Page;
			Assert.AreEqual(ViewMode.Page, editor.ViewMode);
			Assert.AreEqual(TXTextControl.ViewMode.PageView, txTextControl.ViewMode);
		}

		[Test]
		public void Indent_Method1()
		{
			editor.Selection.Text = "Vultures are circling";
			editor.Selection.Length = 0;
			Assert.AreEqual(0, txTextControl.Selection.ParagraphFormat.LeftIndent);
			editor.Indent();
			Assert.AreEqual(720, txTextControl.Selection.ParagraphFormat.LeftIndent);

			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\li720\plain\f0\fs20" +
				@" Vultures are circling" +
				@"\par }";

			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		[Test]
		public void Outdent_Method1()
		{
			editor.Selection.Text = "Vultures are circling";
			editor.Selection.Length = 0;
			editor.Indent();
			Assert.AreEqual(720, txTextControl.Selection.ParagraphFormat.LeftIndent);
			editor.Outdent();
			Assert.AreEqual(0, txTextControl.Selection.ParagraphFormat.LeftIndent);

			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20" +
				@" Vultures are circling" +
				@"\par }";

			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		#region Tables

		[Test]
		public void InsertTable_Method1()
		{
			bool success = editor.InsertTable(5.0, 5, 4);
			// selection should be somewhere with table so this should return new table
			TXTextControl.Table table = txTextControl.Tables.GetItem();
			Assert.IsTrue(success);
			Assert.IsNotNull(table);
			Assert.AreEqual(4, table.Columns.Count);
			Assert.AreEqual(5, table.Rows.Count);
			Assert.AreEqual(1, txTextControl.Tables.Count);
			// margins default to 1" each and page width defaults to 8.5"
			Assert.AreEqual(5.0 / 4.0, table.Columns.GetItem(1).Width / 1440.0);

			// First cell should hold selection
			TXTextControl.TableCell cell = table.Cells.GetItem();
			Assert.IsNotNull(cell);
			Assert.AreEqual(1, cell.Row);
			Assert.AreEqual(1, cell.Column);
		}

		[Test]
		public void InsertTable_Method2()
		{
			bool success = editor.InsertTable(6.0, 5, 19);
			// selection should be somewhere with table so this should return new table
			TXTextControl.Table table = txTextControl.Tables.GetItem();
			Assert.IsTrue(success);
			Assert.IsNotNull(table);
			Assert.AreEqual(19, table.Columns.Count);
			Assert.AreEqual(5, table.Rows.Count);
			Assert.AreEqual(1, txTextControl.Tables.Count);
			// margins default to 1" each and page width defaults to 8.5"
			Assert.AreEqual(Math.Round(6.0 / 19.0, 3), Math.Round(table.Columns.GetItem(7).Width / 1440.0, 3));

			// First cell should hold selection
			TXTextControl.TableCell cell = table.Cells.GetItem();
			Assert.IsNotNull(cell);
			Assert.AreEqual(1, cell.Row);
			Assert.AreEqual(1, cell.Column);
		}

		[Test]
		public void CanInsertTable_Property1()
		{
			Assert.IsTrue(editor.CanInsertTable);
			bool success = editor.InsertTable(0, 5, 4);
			Assert.IsTrue(success);
			Assert.IsFalse(editor.CanInsertTable);
		}

		[Test]
		public void CursorInTable_Property1()
		{
			bool success = editor.InsertTable(4, 5, 4);
			Assert.IsTrue(success);
			Assert.IsTrue(editor.CursorInTable);
			editor.SelectAll();
			editor.Select(editor.Selection.Start + editor.Selection.Length, 0);
			Assert.IsFalse(editor.CursorInTable);
		}

		[Test]
		public void DeleteTable_Method1()
		{
			Assert.AreEqual(0, txTextControl.Tables.Count);
			bool success = editor.InsertTable(0, 5, 4);
			Assert.IsTrue(success);
			Assert.IsTrue(editor.CursorInTable);
			Assert.AreEqual(1, txTextControl.Tables.Count);

			success = editor.DeleteTable();
			Assert.IsTrue(success);
			Assert.IsFalse(editor.CursorInTable);
			Assert.AreEqual(0, txTextControl.Tables.Count);
		}

		[Test]
		public void InsertRowsOrColumns_Method_RowsAfterTest()
		{
			TXTextControl.Table t = setupInsertDeleteRowOrColumnTest();
			editor.InsertRowsOrColumns(false, 2, 0);
			Assert.AreEqual(6, t.Rows.Count);
			Assert.AreEqual("R1C1", t.Cells.GetItem(1, 1).Text);
			Assert.AreEqual("R2C1", t.Cells.GetItem(4, 1).Text);
			Assert.AreEqual("R3C2", t.Cells.GetItem(5, 2).Text);
		}

		[Test]
		public void InsertRowsOrColumns_Method_RowsBeforeTest()
		{
			TXTextControl.Table t = setupInsertDeleteRowOrColumnTest();
			editor.InsertRowsOrColumns(true, 2, 0);
			Assert.AreEqual(6, t.Rows.Count);
			Assert.AreEqual("R1C1", t.Cells.GetItem(3, 1).Text);
			Assert.AreEqual("R1C2", t.Cells.GetItem(3, 2).Text);
			Assert.AreEqual("R3C2", t.Cells.GetItem(5, 2).Text);
		}

		[Test]
		public void InsertRowsOrColumns_Method_ColumnsAfterTest()
		{
			TXTextControl.Table t = setupInsertDeleteRowOrColumnTest();
			editor.InsertRowsOrColumns(false, 0, 2);
			Assert.AreEqual(4, t.Columns.Count);
			Assert.AreEqual("R1C1", t.Cells.GetItem(1, 1).Text);
			Assert.AreEqual("R1C2", t.Cells.GetItem(1, 4).Text);
			Assert.AreEqual(1.5, t.Columns.GetItem(1).Width / 1440.0);
			Assert.AreEqual(1.5, t.Columns.GetItem(2).Width / 1440.0);
			Assert.AreEqual(1.5, t.Columns.GetItem(3).Width / 1440.0);
			Assert.AreEqual(1.5, t.Columns.GetItem(4).Width / 1440.0);
		}

		[Test]
		public void InsertRowsOrColumns_Method_ColumnsBeforeTest()
		{
			TXTextControl.Table t = setupInsertDeleteRowOrColumnTest();
			editor.InsertRowsOrColumns(true, 0, 2);
			Assert.AreEqual(4, t.Columns.Count);
			Assert.AreEqual("R1C1", t.Cells.GetItem(1, 3).Text);
			Assert.AreEqual(1.5, t.Columns.GetItem(1).Width / 1440.0);
			Assert.AreEqual(1.5, t.Columns.GetItem(2).Width / 1440.0);
			Assert.AreEqual(1.5, t.Columns.GetItem(3).Width / 1440.0);
			Assert.AreEqual(1.5, t.Columns.GetItem(4).Width / 1440.0);
		}

		[Test]
		public void DeleteRowsOrColumns_Method_ColumnTest()
		{
			TXTextControl.Table t = setupInsertDeleteRowOrColumnTest();
			t.Cells.GetItem(2, 2).Select();
			editor.DeleteRowsOrColumns(0, 1);
			Assert.AreEqual(4, t.Rows.Count);
			Assert.AreEqual(4, t.Cells.Count);
			Assert.AreEqual(1, t.Columns.Count);
			Assert.AreEqual("R2C1", t.Cells.GetItem(2, 1).Text);
		}

		[Test]
		public void DeleteRowsOrColumns_Method_RowTest()
		{
			TXTextControl.Table t = setupInsertDeleteRowOrColumnTest();
			t.Cells.GetItem(2, 2).Select();
			editor.DeleteRowsOrColumns(1, 0);
			Assert.AreEqual(3, t.Rows.Count);
			Assert.AreEqual(2, t.Columns.Count);
			Assert.AreEqual(6, t.Cells.Count);
			Assert.AreEqual("R3C2", t.Cells.GetItem(2, 2).Text);
		}

		private TXTextControl.Table setupInsertDeleteRowOrColumnTest()
		{
			bool success = editor.InsertTable(6.0, 4, 2);
			Assert.IsTrue(success);

			TXTextControl.Table t = txTextControl.Tables.GetItem();
			Assert.IsNotNull(t);
			Assert.AreEqual(8, t.Cells.Count);
			Assert.AreEqual(3.0, t.Columns.GetItem(1).Width / 1440.0);
			Assert.AreEqual(3.0, t.Columns.GetItem(2).Width / 1440.0);

			for (int r = 1; r <= t.Rows.Count; ++r)
			{
				for (int c = 1; c <= t.Columns.Count; ++c)
				{
					TXTextControl.TableCell cell = t.Cells.GetItem(r, c);
					cell.Text = string.Format("R{0}C{1}", r, c);
				}
			}

			for (int r = 1; r <= t.Rows.Count; ++r)
			{
				for (int c = 1; c <= t.Columns.Count; ++c)
				{
					TXTextControl.TableCell cell = t.Cells.GetItem(r, c);
					Assert.AreEqual(string.Format("R{0}C{1}", r, c), cell.Text);
				}
			}

			return t;
		}

		#endregion

		#region Fields

		[Test]
		public void InsertTextField_Method1()
		{
			editor.AddField(1001, "bar");
			editor.AddField(1002, "foo");
			editor.AddField(1001, "bar");

			Assert.AreEqual(3, txTextControl.TextFields.Count);

			TXTextControl.TextField[] array = new TXTextControl.TextField[3];
			txTextControl.TextFields.CopyTo(array, 0);

			Assert.AreEqual(1001, array[0].ID);
			Assert.AreEqual(1002, array[1].ID);
			Assert.AreEqual(1001, array[2].ID);

			Assert.AreEqual("TF$bar", array[0].Name);
			Assert.AreEqual("TF$foo", array[1].Name);
			Assert.AreEqual("TF$bar", array[2].Name);

			Assert.AreEqual("<<bar>>", array[0].Text);
			Assert.AreEqual("<<foo>>", array[1].Text);
			Assert.AreEqual("<<bar>>", array[2].Text);
		}

		[Test]
		public void UpdateTextFieldNames()
		{
			editor.AddField(2001, "bar");
			editor.AddField(2002, "foo");
			editor.AddField(2001, "bar");

			Dictionary<int, string> fieldMap = new Dictionary<int, string>();
			fieldMap.Add(2001, "bar");
			fieldMap.Add(2002, "foo");

			fieldMap[2001] = "apple";

			editor.UpdateFieldNames(fieldMap);

			TXTextControl.TextField[] array = new TXTextControl.TextField[3];
			txTextControl.TextFields.CopyTo(array, 0);

			Assert.AreEqual(2001, array[0].ID);
			Assert.AreEqual(2002, array[1].ID);
			Assert.AreEqual(2001, array[2].ID);

			Assert.AreEqual("TF$apple", array[0].Name);
			Assert.AreEqual("TF$foo", array[1].Name);
			Assert.AreEqual("TF$apple", array[2].Name);

			Assert.AreEqual("<<apple>>", array[0].Text);
			Assert.AreEqual("<<foo>>", array[1].Text);
			Assert.AreEqual("<<apple>>", array[2].Text);
		}

		[Test]
		public void UpdateTextFieldQualifiedNames()
		{
			editor.AddField(2001, "Form 1:bar");
			editor.AddField(2002, "Form 1:foo");
			editor.AddField(2003, "Form 2:bar");

			Dictionary<int, string> fieldMap = new Dictionary<int, string>();
			fieldMap.Add(2001, "Form 1:bar");
			fieldMap.Add(2002, "Form 1:foo");
			fieldMap.Add(2003, "Form 2:bar");

			fieldMap[2001] = "Form 1 Renamed:bar";
			fieldMap[2002] = "Form 1 Renamed:foo";
			fieldMap[2003] = "Form 2:bar";

			editor.UpdateFieldNames(fieldMap);

			TXTextControl.TextField[] array = new TXTextControl.TextField[3];
			txTextControl.TextFields.CopyTo(array, 0);

			Assert.AreEqual("TF$Form 1 Renamed:bar", array[0].Name);
			Assert.AreEqual("TF$Form 1 Renamed:foo", array[1].Name);
			Assert.AreEqual("TF$Form 2:bar", array[2].Name);

			Assert.AreEqual("<<Form 1 Renamed:bar>>", array[0].Text);
			Assert.AreEqual("<<Form 1 Renamed:foo>>", array[1].Text);
			Assert.AreEqual("<<Form 2:bar>>", array[2].Text);
		}

		#endregion

		[Test]
		public void InsertPageBreak()
		{
			editor.Selection.Text = "Vultures are circling.";
			editor.Selection.Start = editor.Selection.Length;
			Assert.AreEqual(1,txTextControl.InputPosition.Page);
			editor.InsertPageBreak();
			Assert.AreEqual(1, txTextControl.InputPosition.Page);
		}
	}
}
