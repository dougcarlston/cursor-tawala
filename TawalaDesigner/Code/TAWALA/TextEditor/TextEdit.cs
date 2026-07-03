// $Workfile: TextEdit.cs $
// $Revision: 101 $	$Date: 3/13/08 5:51p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace Tawala.TextEditor
{
	public partial class TextEdit : UserControl, ITextEdit, IFieldSupport, IFunctionFieldSupport
	{
		#region ITextEdit

        public event EventHandler<InvitationFieldEventArgs> InvitationFieldDoubleClicked;

		public void ClearAll()
		{
			txTextControl.Text = "";
		}

		public void SelectAll()
		{
			txTextControl.SelectAll();
		}

		public bool CanCopy
		{
			get
			{
				return txTextControl.CanCopy;
			}
		}

		public bool CanCut
		{
			get
			{
				return txTextControl.CanCopy;
			}
		}

		public bool CanPaste
		{
			get
			{
				return txTextControl.CanPaste;
			}
		}

		public bool CanDrop
		{
			get
			{
				int index = CharIndexFromScreenPoint(MousePosition);
				TFProximity proximity = CharIndexProximityToTextField(index);
				return proximity != TFProximity.Inside;
			}
		}

		public void Copy()
		{
			txTextControl.Copy();
		}

		public void Cut()
		{
			txTextControl.Cut();
		}

		public void Paste()
		{
			txTextControl.Paste();
		}

		public void Delete()
		{
			txTextControl.DeleteSelection();
		}

		public ViewMode ViewMode
		{
			get
			{
				switch (txTextControl.ViewMode)
				{
					case TXTextControl.ViewMode.FloatingText: return ViewMode.Unlimited;
					case TXTextControl.ViewMode.Normal: return ViewMode.Normal;
					case TXTextControl.ViewMode.PageView: return ViewMode.Page;
                    case TXTextControl.ViewMode.SimpleControl: return ViewMode.Simple;
                }
				return ViewMode.Unlimited;
			}
			set
			{
				switch (value)
				{
					case ViewMode.Unlimited: 
						txTextControl.ViewMode = TXTextControl.ViewMode.FloatingText;
						txTextControl.ScrollBars = ScrollBars.None;
						return;
					case ViewMode.Normal: txTextControl.ViewMode = TXTextControl.ViewMode.Normal; return;
					case ViewMode.Page: txTextControl.ViewMode = TXTextControl.ViewMode.PageView; return;
                    case ViewMode.Simple: txTextControl.ViewMode = TXTextControl.ViewMode.SimpleControl; return;
				}
			}
		}

        public bool SelectionHighlightRequiresFocus
        {
            get
            {
                return txTextControl.HideSelection;
            }
            set
            {
                txTextControl.HideSelection = value;
            }
        }

        public ITextSelection Selection
		{
		    get
		    {
				return new TextSelection(txTextControl.Selection);
			}
		}

		public TXTextControl.Selection TxSelection
		{
			get
			{
				return txTextControl.Selection;
			}
		}

		public void Select(int start, int length)
		{
			txTextControl.Select(start, length);
		}

		public void Select(Point ptScreen, int length)
		{
			int start = CharIndexFromScreenPoint(ptScreen);
			Select(start, length);
		}

		public void ToggleBold()
		{
			TXTextControl.Selection txSel = txTextControl.Selection;

			if (!txSel.IsCommonValueSelected(TXTextControl.Selection.Attribute.Bold))
			{
				txSel.Bold = true;
			}
			else if (txSel.Bold)
			{
				txSel.Bold = false;
			}
			else
			{
				txSel.Bold = true;
			}
		}

		public void ToggleItalic()
		{
			TXTextControl.Selection txSel = txTextControl.Selection;

			if (!txSel.IsCommonValueSelected(TXTextControl.Selection.Attribute.Italic))
			{
				txSel.Italic = true;
			}
			else if (txSel.Italic)
			{
				txSel.Italic = false;
			}
			else
			{
				txSel.Italic = true;
			}
		}

		public void ToggleUnderline()
		{
			TXTextControl.Selection txSel = txTextControl.Selection;

			if (!txSel.IsCommonValueSelected(TXTextControl.Selection.Attribute.Underline))
			{
				txSel.Underline = TXTextControl.FontUnderlineStyle.Single;
			}
			else if (txSel.Underline == TXTextControl.FontUnderlineStyle.None)
			{
				txSel.Underline = TXTextControl.FontUnderlineStyle.Single; 
			}
			else
			{
				txSel.Underline = TXTextControl.FontUnderlineStyle.None;
			}
		}

		public void Indent()
		{
			TXTextControl.Selection txSel = txTextControl.Selection;
			if (txSel.ParagraphFormat.LeftIndent <= 5 * 1440)
			{
				txSel.ParagraphFormat.LeftIndent += 720;
			}
		}

		public void Outdent()
		{
			TXTextControl.Selection txSel = txTextControl.Selection;
			if (txSel.ParagraphFormat.LeftIndent <= 720)
			{
				txSel.ParagraphFormat.LeftIndent = 0;
			}
			else
			{
				txSel.ParagraphFormat.LeftIndent -= 720;
			}
		}

		public bool InsertTable(double width, int rows, int cols)
		{
			double maxWidth = (txTextControl.PageSize.Width - txTextControl.PageMargins.Left - txTextControl.PageMargins.Right) / 100.0;
			Console.WriteLine("maxWidth = {0:F}", maxWidth);
			if (width < 1.0 || width > maxWidth)
			{
				width = maxWidth;
			}
			TXTextControl.Selection txSel = txTextControl.Selection;
			txTextControl.Select(txSel.Start + txSel.Length, 0);
			txSel.Text = "\r\n";
			int start = txSel.Start+1;
			txTextControl.Select(start, 0);

			if (CanInsertTable) // must not currently be in a table (no nesting for now)
			{
				if (txTextControl.Tables.Add(rows, cols))
				{
					txTextControl.Select(start, 0);
					TXTextControl.Table table = txTextControl.Tables.GetItem();
					if (table != null)
					{
						TXTextControl.TableCell cell = table.Cells.GetItem(1, 1);
						if (cell != null)
						{
							cell.Select();
							txTextControl.Select(txTextControl.Selection.Start, 0);

							int colCount = table.Columns.Count;
							Debug.Assert(colCount == cols);
							int twips_per_column = Convert.ToInt32((width * 1440.0) / colCount);
							for (int i = 1; i <= colCount; ++i)
							{
								TXTextControl.TableColumn c = table.Columns.GetItem(i);
								c.Width = twips_per_column;
							}
							return true;
						}
					}
				}
			}
			return false;
		}

		public Collection<string> Lines
		{
			get
			{
				Collection<string> allLines = new Collection<string>();
				foreach (TXTextControl.Line line in txTextControl.Lines)
				{
					allLines.Add(line.Text);
				}
				return allLines;
			}
		}

		public int LineCount
		{
			get
			{
				return txTextControl.Lines.Count;
			}
		}

		public bool CanInsertTable
		{
			get
			{
				TXTextControl.Table table = txTextControl.Tables.GetItem();
				if (table == null) // must not currently be in a table (no nesting for now)
				{
					if (txTextControl.Tables.CanAdd)
					{
						return true;
					}
				}

				return false;
			}
		}

		public bool CursorInTable
		{
			get
			{
                if (txTextControl.Tables != null)
                {
                    TXTextControl.Table table = txTextControl.Tables.GetItem();
                    return table != null ? true : false;
                }

                return false;
			}
		}

		public bool DeleteTable()
		{
			TXTextControl.Table table = txTextControl.Tables.GetItem();
			if (table != null)
			{
				txTextControl.Tables.Remove();
				return true;
			}
			return false;
		}

		public void InsertRowsOrColumns(bool before, int rows, int columns)
		{
			TXTextControl.TableAddPosition addPos = convertBeforeToAddPosition(before);

			TXTextControl.Table table = txTextControl.Tables.GetItem();

			if (table != null)
			{
				if (rows > 0)
				{
					table.Rows.Add(addPos, rows);
				}

				int maxColumns = 20;
				columns = limitColumnCount(columns, maxColumns, table);

				if (columns > 0)
				{
					int tableWidth = getTableWidth(table);
					int columnWidth = tableWidth / (table.Columns.Count + columns);

					insertColumns(columns, addPos, table, columnWidth);
				}
			}
		}

		private static int limitColumnCount(int columns, int maxColumns, TXTextControl.Table table)
		{
			if (columns + table.Columns.Count > maxColumns)
			{
				columns = maxColumns - table.Columns.Count;
			}

			return columns;
		}

		private static TXTextControl.TableAddPosition convertBeforeToAddPosition(bool before)
		{
			TXTextControl.TableAddPosition addPos = before ? TXTextControl.TableAddPosition.Before : TXTextControl.TableAddPosition.After;
			return addPos;
		}

		private static void insertColumns(int columnCount, TXTextControl.TableAddPosition addPos, TXTextControl.Table table, int columnWidth)
		{
			for (int i = 1; i <= table.Columns.Count; ++i)
			{
				TXTextControl.TableColumn column = table.Columns.GetItem(i);
				column.Width = 1;
			}

			for (int i = 0; i < columnCount; ++i)
			{
				table.Columns.Add(addPos);
			}

			for (int i = 1; i <= table.Columns.Count; ++i)
			{
				TXTextControl.TableColumn column = table.Columns.GetItem(i);
				column.Width = columnWidth;
			}
		}

		private static int getTableWidth(TXTextControl.Table table)
		{
			int tableWidth = 0;

			for (int i = 1; i <= table.Columns.Count; ++i)
			{
				TXTextControl.TableColumn column = table.Columns.GetItem(i);
				tableWidth += column.Width;
			}
			return tableWidth;
		}

		public void DeleteRowsOrColumns(int rows, int cols)
		{
			TXTextControl.Table table = txTextControl.Tables.GetItem();
			if (table != null)
			{
				if (rows > 0)
				{
					table.Rows.Remove(); // remove current row
				}

				if (cols > 0)
				{
					txTextControl.Selection.Length = 0;
					table.Columns.Remove(); // remove current column
				}
			}
		}

		public DialogResult ShowTabDialog()
		{
            TabDialog td = new TabDialog(TXTextControl.ParagraphFormat.MaxTabs);
            td.TabsInInches = txTextControl.TabsInInches;

            DialogResult result = td.ShowDialog(FindForm());

            if (result == DialogResult.OK)
            {
                txTextControl.TabsInInches = td.TabsInInches;
            }

            return result;
		}

		public string GetText()
		{
			return txTextControl.Text;
		}

		public virtual void SetText(string s)
		{
			txTextControl.Text = s;
		}
		
		public string GetRTF()
		{
			if (txTextControl.Images != null)
			{
				foreach (TXTextControl.Image img in txTextControl.Images)
				{
					img.SaveMode = TXTextControl.ImageSaveMode.SaveAsData;
				}
			}

			string rtfString;
			txTextControl.Save(out rtfString, TXTextControl.StringStreamType.RichTextFormat);

			//return removeExtraParagraphs(rtfString);
			return removeParagraphFollowingTable(rtfString);
		}

		private string removeExtraParagraphs(string rtfString)
		{
			string alteredString = removeDoubledParagraph(rtfString);
			alteredString = removeParagraphFollowingTable(alteredString);

			return alteredString;
		}

		private static string removeParagraphFollowingTable(string rtfString)
		{
			if (rtfString != null)
			{
				return Regex.Replace(rtfString, @"\\row\\pard(\\[a-zA-Z0-9]+)+\\par }$", @"\row\pard\itap0 }");
			}

			return null;
		}

		private static string removeDoubledParagraph(string rtfString)
		{
			return Regex.Replace(rtfString, @"\\par\\par }$", @"\par }");
		}

		public void SetRTF(string rtf)
		{
			if (rtf != null && rtf.Length > 0)
			{
				txTextControl.Load(rtf, TXTextControl.StringStreamType.RichTextFormat);

				stripTrailingNewlineCreatedByTXControl(rtf);
			}
		}

		private void stripTrailingNewlineCreatedByTXControl(string rtf)
		{
			if (!rtf.EndsWith(@"\pard \par }"))
			{
				SelectAll();
				Selection.Start = Selection.Length - 1;
				Selection.Length = 1;
				Selection.Text = "";
			}
		}

		public void InsertPageBreak()
		{
			Selection.Text = "\0x0C";
		}

		public virtual int GetPreferredHeight()
		{
			int heightInTwips =  240;

			if (txTextControl != null)
			{
				heightInTwips += getControlHeight();

				txTextControl.ScrollLocation = new Point(0, 0);
				txTextControl.ScrollBars = ScrollBars.None;
			}

			return txTextControl.TwipsToPixels(heightInTwips);
		}

		// REVISIT:	Need to do a better job at calculating "cellHeight", so that it reflects the height of individual cells.
		//			Until that occurs, extra space might appear at the bottom of Form Items that contain tables. - SB 10/19/2006
		//
		private int getControlHeight()
		{
			int heightInTwips = 0;
			int cellHeight = 15840;

			if (txTextControl.Lines != null)
			{
				foreach (TXTextControl.Line line in txTextControl.Lines)
				{
					heightInTwips += line.TextBounds.Height;
					cellHeight = Math.Min(cellHeight, line.TextBounds.Height);
				}

				foreach (TXTextControl.Table table in txTextControl.Tables)
				{
					heightInTwips -= table.Cells.Count * cellHeight;

					foreach (TXTextControl.TableRow row in table.Rows)
					{
						heightInTwips += cellHeight;
					}
				}
			}

			return heightInTwips;
		}

		public Point GetScrollOffset()
		{
			int inputY = txTextControl.InputPosition.Location.Y;
			int offsetY = txTextControl.TwipsToPixels(inputY);

			return new Point(0, -offsetY);
		}

		public virtual void InsertImage()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Image Files (*.gif; *.jpg; *.png) | *.gif; *.jpg; *.png;";

			if (ofd.ShowDialog(this) == DialogResult.OK)
			{
				string tempFilename = null;

				try
				{
					string filename = ofd.FileName.ToLower();
					using (Image image = Image.FromFile(filename))
					{
						using (Bitmap bitmap = new Bitmap(image))
						{
							tempFilename = addImageAs24BitBitmap(bitmap, filename);
						}
					}
				}
				finally
				{
					// Deleting the temporary file causes Bug 353, so don't delete for now. - SB 11/13/2006
					//deleteFileWithNoThrow(tempFilename);
				}
			}
		}

		private string addImageAs24BitBitmap(Bitmap bitmap, string filename)
		{
			Rectangle r = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			using (Bitmap bitmapClone = bitmap.Clone(r, PixelFormat.Format24bppRgb))
			{
				string tempFilename = createTempBmpFile(bitmapClone, filename);
				TXTextControl.Image txImage = new TXTextControl.Image();
				txImage.FileName = tempFilename;
				txImage.SaveMode = TXTextControl.ImageSaveMode.SaveAsData;
				txTextControl.Images.Add(txImage, -1);
				return tempFilename;
			}
		}

		/// <summary>
		/// Creates a temporary BMP file whose name is formed from the specified filename. This method was created specifically
		/// to address Bug 353, and is designed to re-use the same temporary filename upon insertion of an image, thus preventing
		/// excessive temporary file creation. - SB 11/13/2006
		/// </summary>
		private static string createTempBmpFile(Bitmap bitmap, string filename)
		{
			string tempFilename = Path.GetTempPath() + Path.GetFileName(filename) + ".bmp";
			bitmap.Save(tempFilename, ImageFormat.Bmp);
			return tempFilename;
		}

		private static void deleteFileWithNoThrow(string tempFilename)
		{
			if (tempFilename != null)
			{
				try
				{
					File.Delete(tempFilename);
				}
				finally
				{
				}
			}
		}

		public virtual void InsertInvitation(int invitationFieldId, string displayText)
		{
			insertLinkField(invitationFieldId, displayText, "IF$");
		}

		public virtual void InsertHyperlink(int hyperlinkFieldId, string displayText)
		{
			insertLinkField(hyperlinkFieldId, displayText, "HF$");
		}

		private void insertLinkField(int linkFieldId, string displayText, string idPrefix)
		{
			bool bNew = false;
			TXTextControl.TextField tf = txTextControl.TextFields.GetItem(linkFieldId);

			if (tf == null)
			{
				bNew = true;
				tf = new TXTextControl.TextField();
			}

			tf.ID = linkFieldId;
			tf.Name = idPrefix + linkFieldId;
			tf.Text = displayText;
			tf.ShowActivated = true;
			tf.DoubledInputPosition = true;
			tf.Editable = false;
			tf.Deleteable = false;

			if (bNew)
			{
				txTextControl.TextFields.Add(tf);
			}

			Color priorColor;
			bool bUndefinedColor = !Selection.GetFontColor(out priorColor);
			Tristate priorUnderline = Selection.Underline;

			Selection.Start = tf.Start - 1;
			Selection.Length = tf.Length;
			Selection.SetFontColor(SystemColors.HotTrack);
			Selection.Underline = Tristate.True;
			Selection.Length = 0;
			Selection.Start = tf.Start + tf.Length;

			if (!bUndefinedColor)
			{
				Selection.SetFontColor(priorColor);
			}

			Selection.Underline = priorUnderline;
		}

		public int SelectedInvitationFieldId()
		{
			TXTextControl.TextField tf = txTextControl.TextFields.GetItem();

			if (tf != null && tf.Name.StartsWith("IF$"))
			{
				return tf.ID;
			}

			return -1;
		}

		public int SelectedHyperlinkFieldId()
		{
			TXTextControl.TextField tf = txTextControl.TextFields.GetItem();

			if (tf != null && tf.Name.StartsWith("HF$"))
			{
				return tf.ID;
			}

			return -1;
		}

		#region IFieldSupport // general

		public bool IsFieldIdInUse(int id)
		{
			return txTextControl.TextFields.GetItem(id) != null;
		}

		public void AddField(int id, string name)
		{
			TXTextControl.TextField tf = txTextControl.TextFields.GetItem();
			bool bNew = false;

			if (tf == null)
			{
				tf = new TXTextControl.TextField();
				bNew = true;
			}

			tf.ID = id;
			tf.Name = "TF$" + name;
			tf.Text = "<<" + name + ">>";
			tf.Deleteable = false;
			tf.Editable = false;
			tf.DoubledInputPosition = true;
			tf.ShowActivated = true;

			if (bNew)
			{
				txTextControl.TextFields.Add(tf);
			}
		}

		protected bool ContainsStandardFields()
		{
			foreach (TXTextControl.TextField tf in txTextControl.TextFields)
			{
				if (tf.Name.StartsWith("TF$"))
				{
					return true;
				}
			}
			return false;
		}

		public void UpdateFieldNames(Dictionary<int, string> fieldMap)
		{
			foreach (TXTextControl.TextField tf in txTextControl.TextFields)
			{
				if (tf.Name.StartsWith("TF$"))
				{
					string name = tf.Name.Substring(3);

					if (fieldMap.ContainsKey(tf.ID) && !name.Equals(fieldMap[tf.ID]))
					{
						string newName = fieldMap[tf.ID];
						tf.Name = "TF$" + newName;
						tf.Text = "<<" + newName + ">>";
					}
				}
			}
		}

		protected Collection<int> getFieldIds()
		{
			Collection<int> fieldIds = new Collection<int>();

			foreach (TXTextControl.TextField tf in txTextControl.TextFields)
			{
				if (tf.Name.StartsWith("TF$"))
				{
					fieldIds.Add(tf.ID);
				}
			}

			return fieldIds;
		}

		protected bool ContainsFunctionFields()
		{
			foreach (TXTextControl.TextField tf in txTextControl.TextFields)
			{
				if (tf.Name.StartsWith("FF$"))
				{
					return true;
				}
			}

			return false;
		}

		public void UpdateFunctionFieldText(Dictionary<int, string> textMap)
		{
			foreach (TXTextControl.TextField tf in txTextControl.TextFields)
			{
				if (tf.Name.StartsWith("FF$"))
				{
					int instanceId = Convert.ToInt32(tf.Name.Substring(3));

					if (textMap.ContainsKey(instanceId))
					{
						tf.Text = textMap[instanceId];
					}
				}
			}
		}

		#endregion

		#region IFunctionFieldSupport

		public event EventHandler<FunctionFieldEventArgs> FunctionFieldDoubleClicked;

		public void ChangeSelectedFunctionFieldIdToUniqueId(int oldIdForSanityCheck, int newId)
		{
			TXTextControl.TextField tf = txTextControl.TextFields.GetItem();
			Debug.Assert(txTextControl.TextFields.GetItem(newId) == null, "Another field has same Id");
			Debug.Assert(tf != null, "There must be a selected function field.  Method call out of context.");
			Debug.Assert(tf.ID == oldIdForSanityCheck, "Old ID failed sanity check.");
			tf.ID = newId;
			tf.Name = "FF$" + newId.ToString();
		}


		public void UpdateFunctionFieldByUniqueId(int oldId, int newId, string displayText)
		{
			TXTextControl.TextField tf = txTextControl.TextFields.GetItem(oldId);
			Debug.Assert(tf != null, "Can't update function field Id that doesn't exist.");
			tf.ID = newId;
			tf.Name = "FF$" + newId.ToString();
			tf.Text = displayText;
		}

		public void InsertFunctionField(int newFunctionId, string displayText)
		{
			if (Selection.Length != 0)
			{
				Selection.Text = string.Empty;
			}

			TXTextControl.TextField tf = txTextControl.TextFields.GetItem(newFunctionId);
			Debug.Assert(tf == null, "newFunctionId matches existing text field ID");

			tf = new TXTextControl.TextField();

			tf.ID = newFunctionId;
			tf.Name = "FF$" + newFunctionId.ToString();
			tf.Text = displayText;
			tf.ShowActivated = true;
			tf.DoubledInputPosition = true;
			tf.Editable = false;
			tf.Deleteable = false;

			txTextControl.TextFields.Add(tf);
		}

		public int SelectedFunctionFieldInstanceId
		{
			get
			{
				TXTextControl.TextField tf = txTextControl.TextFields.GetItem();
				return (tf != null && tf.Name.StartsWith("FF$")) ? tf.ID : -1;
			}
		}

		#endregion

		public virtual void HandleDragEnter(DragEventArgs e)
		{
		}

		public virtual void HandleDragOver(DragEventArgs e)
		{
		}

		public virtual void HandleDragLeave(EventArgs e)
		{
		}

		public virtual void HandleDragDrop(DragEventArgs e)
		{
		}

		public event EventHandler Changed;

		public virtual void ForceOnChanged()
		{
			OnChanged(EventArgs.Empty);
		}

		public virtual void OnChanged(EventArgs e)
		{
			if (Changed != null)
			{
				Changed(this, EventArgs.Empty);
			}
		}

        public event EventHandler InputPositionChanged;

        public virtual void OnInputPositionChanged(EventArgs e)
        {
            if (InputPositionChanged != null)
            {
                InputPositionChanged(this, EventArgs.Empty);
            }
        }

        public override bool AllowDrop
		{
			get
			{
				return txTextControl.AllowDrop;
			}
			set
			{
				txTextControl.AllowDrop = value;
				base.AllowDrop = value;
			}
		}

		#endregion 

		#region Implementation Details

		public TextEdit()
		{
			InitializeComponent();

			txTextControl.ViewMode = TXTextControl.ViewMode.Normal;
		}

		public void HandleTextFieldDoubleClicked(TXTextControl.TextField tf)
		{
			if (tf != null)
			{
				if (tf.Name.StartsWith("IF$") && InvitationFieldDoubleClicked != null)
				{
					InvitationFieldDoubleClicked(this, getInvitationFieldEventArgs(tf));
				}
				else if (tf.Name.StartsWith("HF$") && InvitationFieldDoubleClicked != null)
				{
					InvitationFieldDoubleClicked(this, getInvitationFieldEventArgs(tf));
				}
				else if (tf.Name.StartsWith("FF$") && FunctionFieldDoubleClicked != null)
				{
					FunctionFieldDoubleClicked(this, getFunctionFieldEventArgs(tf));
				}
			}
		}

		private InvitationFieldEventArgs getInvitationFieldEventArgs(TXTextControl.TextField tf)
		{
			return new InvitationFieldEventArgs(tf.ID);
		}

		private FunctionFieldEventArgs getFunctionFieldEventArgs(TXTextControl.TextField tf)
		{
			return new FunctionFieldEventArgs(tf.ID);
		}

		public void HandleChildGotFocus(EventArgs e)
		{
			OnGotFocus(e);
		}

		public void HandleChildLostFocus(EventArgs e)
		{
			OnLostFocus(e);
		}

		public void HandleChildMouseDown(MouseEventArgs e)
		{
			OnMouseDown(e);
		}

		public void HandleChildMouseUp(MouseEventArgs e)
		{
			OnMouseUp(e);
		}

		public void HandleChildMouseClick(MouseEventArgs e)
		{
			OnMouseClick(e);
		}

		public void HandleChildMouseEnter(EventArgs e)
		{
			OnMouseEnter(e);
		}

		public void HandleChildMouseLeave(EventArgs e)
		{
			OnMouseLeave(e);
		}

		public void HandleKeyDown(KeyEventArgs e)
		{
			OnKeyDown(e);
		}

		public void HandleKeyPress(KeyPressEventArgs e)
		{
			OnKeyPress(e);
		}

		public void HandleKeyUp(KeyEventArgs e)
		{
			OnKeyUp(e);
		}

		protected bool FixUnexpectedScrollBehavior
		{
			get { return txTextControl.FixUnexpectedScrollBehavior; }
			set { txTextControl.FixUnexpectedScrollBehavior = value; }
		}

		protected int CharIndexFromScreenPoint(Point pt)
		{
			// Selection start is 0 based, char number is 1 based (lovely ain't it?)
			int charCount = txTextControl.TextChars.Count;
			Point ptClient = txTextControl.PointToClient(pt);
			TXTextControl.TextChar charPos = txTextControl.TextChars.GetItem(ptClient, true);
			TXTextControl.TextChar nearPos = txTextControl.TextChars.GetItem(ptClient, false);

			int start = -1;

			if (charPos == null || charCount == 0)
			{
				start = 0; // doc must be empty
			}
			else
			{

				if (charPos.Number == charCount && nearPos == null)
				{
					start = charPos.Number;
				}
				else
				{
					start = charPos.Number - 1;
				}
			}

			Debug.Assert(start >= 0);
			return start;
		}

		protected enum TFProximity { NotNextToOrIn, Inside, LeftSide, RightSide };

		protected TFProximity CharIndexProximityToTextField(int zeroBasedIndex)
		{
			TXTextControl.TextField tfFound = null;
			TFProximity proximity = TFProximity.NotNextToOrIn;
			int count = txTextControl.TextFields.Count;

			foreach (TXTextControl.TextField tf in txTextControl.TextFields)
			{
				// tf.Start is 1 based
				int before = tf.Start - 1;
				int after = before + tf.Length;

				if (zeroBasedIndex == before)
				{
					tfFound = tf;
					proximity = TFProximity.LeftSide;
					break;
				}
				else if (zeroBasedIndex == after)
				{
					tfFound = tf;
					proximity = TFProximity.RightSide;
					break;
				}
				else if (zeroBasedIndex > before && zeroBasedIndex < after)
				{
					tfFound = tf;
					proximity = TFProximity.Inside;
					break;
				}
			}

			//if (tfFound == null)
			//{
			//    Debug.WriteLine(string.Format("#{0}: index = {1}, {2}", debug++, zeroBasedIndex, proximity.ToString()));
			//}
			//else
			//{
			//    Debug.WriteLine(string.Format("#{0}: index = {1}, {2} TF[s:{3}, l:{4}, n:{5}]", debug++, zeroBasedIndex, proximity.ToString(),
			//        tfFound.Start, tfFound.Length, tfFound.Name));
			//}

			return proximity;
		}

		protected new string Text
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		#endregion
	}
}