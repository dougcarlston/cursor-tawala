// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;
using Tawala.Interfaces;
using Tawala.MainApplication;
using Tawala.Processes;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Processes;

namespace Tawala.FormDesigner.Dialogs.SkipInstructionsDialog
{
	public partial class SkipInstructionsView : Form, IProcessView
	{
		private static readonly Type[] statementViewTypes =
			{
				typeof (IfStatementView),
				null,
				typeof (SkipToStatementView),
				null,
				typeof (SetStatementView),
				null,
				typeof (CommentStatementView),
				null
			};

		private readonly ISkipInstructionsItem skipInstructionsItem;
		private readonly IFormView view;

		public SkipInstructionsView(IFormView view, ISkipInstructionsItem skipInstructionsItem)
		{
			InitializeComponent();

			this.view = view;
			this.skipInstructionsItem = skipInstructionsItem;

			skipInstructionsStatementSelector.Init(statementViewTypes);
			skipInstructionsStatementSelector.ProcessEditor = processEditor;

			processEditor.Init(skipInstructionsStatementSelector, statementViewTypes);
			processEditor.Process = skipInstructionsItem.Instructions;
			processEditor.ConnectProjectEvents(true);

			MdiParent = ApplicationPresenter.MainApplicationForm;
		}

		#region IProcessView Members

		public IProcessPresenter Presenter
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public IApplicationView ParentView
		{
			get { return ParentForm as IApplicationView; }
		}

		public ProcessEditor ProcessEditor
		{
			get { return processEditor; }
		}

		public IProcess Process
		{
			get { return skipInstructionsItem.Instructions; }
		}

		public void SetProcessName(string processName)
		{
			Text = processName;
		}

		#endregion

		private void buttonClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void SkipInstructionsView_FormClosed(object sender, FormClosedEventArgs e)
		{
			view.SetAttribute(skipInstructionsItem.Id.ToString(), "Summary", skipInstructionsItem.GetSummary());
			processEditor.ConnectProjectEvents(false);
		}

		private void toolStripMenuItemCut_Click(object sender, EventArgs e)
		{
			processEditor.Cut();
		}

		private void toolStripMenuItemCopy_Click(object sender, EventArgs e)
		{
			processEditor.Copy();
		}

		private void toolStripMenuItemPaste_Click(object sender, EventArgs e)
		{
			processEditor.Paste();
		}

		private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
		{
			processEditor.Delete();
		}

		private void toolStripMenuItemUndo_Click(object sender, EventArgs e)
		{
			processEditor.Undo();
		}

		private void toolStripMenuItemRedo_Click(object sender, EventArgs e)
		{
			processEditor.Redo();
		}

		private void SkipInstructionsView_Load(object sender, EventArgs e)
		{
			Application.Idle += application_Idle;
		}

		private void application_Idle(object sender, EventArgs e)
		{
			toolStripButtonCut.Enabled = toolStripMenuItemCut.Enabled = processEditor.CanCut();

			toolStripButtonCopy.Enabled = toolStripMenuItemCopy.Enabled = processEditor.CanCopy();

			toolStripButtonPaste.Enabled = toolStripMenuItemPaste.Enabled = processEditor.CanPaste();

			toolStripButtonDelete.Enabled = toolStripMenuItemDelete.Enabled = processEditor.CanDelete();

			toolStripButtonUndo.Enabled = toolStripMenuItemUndo.Enabled = processEditor.CanUndo();
			toolStripButtonUndo.Text = toolStripMenuItemUndo.Text = "Undo " + processEditor.UndoActionText;

			toolStripButtonRedo.Enabled = toolStripMenuItemRedo.Enabled = processEditor.CanRedo();
			toolStripButtonRedo.Text = toolStripMenuItemRedo.Text = "Redo " + processEditor.RedoActionText;
		}

		private void SkipInstructionsView_Activated(object sender, EventArgs e)
		{
			ToolStripManager.Merge(toolStripSkipInstructions, "mainToolStrip");
		}

		private void SkipInstructionsView_Deactivate(object sender, EventArgs e)
		{
			ToolStripManager.RevertMerge("mainToolStrip");
		}
	}
}