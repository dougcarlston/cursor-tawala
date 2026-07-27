// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using Tawala.Projects;

namespace Tawala.Processes
{
	public interface IProcessEditor
	{
		System.Collections.ObjectModel.Collection<System.Windows.Forms.ToolStripItem> Init(IStatementSelector statementSelector, Type[] statementViewTypes);
		void StatementButtonClicked(System.ComponentModel.Component component);
		Process Process { get; set; }
		string UndoActionText { get; }
		string RedoActionText { get; }

		bool CanCut();
		void Cut();
		bool CanCopy();
		void Copy();
		bool CanPaste();
		void Paste();
		bool CanDelete();
		void Delete();
		bool CanUndo();
		bool CanRedo();
		void Undo();
		void Redo();
	}
}
