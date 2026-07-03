// $Workfile: ITextEdit.cs $
// $Revision: 53 $	$Date: 4/26/07 2:40p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.TextEditor
{
	public interface ITextEdit
	{
		event EventHandler Changed;
		event EventHandler<InvitationFieldEventArgs> InvitationFieldDoubleClicked;
		event EventHandler InputPositionChanged;

		void ForceOnChanged();
		void OnChanged(EventArgs e);
        void OnInputPositionChanged(EventArgs e);

		void ClearAll();
		void SelectAll();

		bool CanCopy
		{
			get;
		}

		bool CanCut
		{
			get;
		}

		bool CanPaste
		{
			get;
		}

		bool CanDrop
		{
			get;
		}

		void Copy();
		void Cut();
		void Paste();
		void Delete();

		ViewMode ViewMode
		{
			get;
			set;
		}

		ITextSelection Selection
		{
			get;
		}

		void Select(int start, int length);
		void Select(Point screen, int length);

		void ToggleBold();
		void ToggleItalic();
		void ToggleUnderline();
		void Indent();
		void Outdent();

		bool InsertTable(double width, int rows, int cols);
		bool CanInsertTable
		{
			get;
		}

		bool CursorInTable
		{
			get;
		}

		void InsertRowsOrColumns(bool bBefore, int rows, int cols);
		void DeleteRowsOrColumns(int rows, int cols);

		bool DeleteTable();

		DialogResult ShowTabDialog();

		string GetText();
		void SetText(string s);

		string GetRTF();
		void SetRTF(string rtf);

		void InsertPageBreak();

		int GetPreferredHeight();

		bool AllowDrop
		{
			get;
			set;
		}

		void InsertImage();

		void InsertInvitation(int invitationFieldId, string displayText);

		void HandleDragEnter(DragEventArgs e);
		void HandleDragOver(DragEventArgs e);
		void HandleDragLeave(EventArgs e);
		void HandleDragDrop(DragEventArgs e);

		void HandleKeyDown(System.Windows.Forms.KeyEventArgs e);
		void HandleKeyPress(System.Windows.Forms.KeyPressEventArgs e);
		void HandleKeyUp(System.Windows.Forms.KeyEventArgs e);

        bool SelectionHighlightRequiresFocus
        {
            get;
            set;
        }

		Collection<string> Lines
		{
			get;
		}

		int LineCount
		{
			get;
		}
	}
}
