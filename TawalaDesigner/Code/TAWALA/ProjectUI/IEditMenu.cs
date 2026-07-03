// $Workfile: IEditMenu.cs $
// $Revision: 4 $	$Date: 12/12/07 1:19p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;

namespace Tawala.ProjectUI
{
	/// <summary>
	/// Interface that a UI component like ProjectPane, Forms.View, Processes.View, Documents.View
	/// should implement (explicitly) to handle the Main Menu's Edit menu.
	/// </summary>
	public interface IEditMenu
	{
		bool CanCut();
		bool CanCopy();
		bool CanPaste();
		bool CanDelete();
		bool CanRename();
		void Cut();
		void Copy();
		void Paste();
		void Delete();
		void Rename();

		bool CanUndo();
		bool CanRedo();
		void Undo();
		void Redo();
		string UndoActionText { get; }
		string RedoActionText { get; }

		// called when edit menu pops up, return null if no additional items.
		// Expects item state to be set
		ToolStripMenuItem[] GetAdditionalMenuItems();
	}
}
