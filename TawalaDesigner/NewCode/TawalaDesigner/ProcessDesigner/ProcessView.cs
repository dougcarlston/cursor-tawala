// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;

using Tawala.Interfaces;
using Tawala.Projects.Processes;
using Tawala.Processes;
using Tawala.ComponentDesigner;
using Tawala.MainApplication;

namespace Tawala.ProcessDesigner
{
	public partial class ProcessView : ProjectComponentView, IProcessView
	{
		private readonly IProcess process;
		private ProcessViewInfoBar viewInfoBar;

		public ProcessView()
		{
			InitializeComponent();
		}

		public ProcessView(IProcess process, Form mdiParent, IProjectExplorerPresenter projectExplorerPresenter)
			: this()
		{
			this.process = process;

			MdiParent = mdiParent;
			Presenter = new ProcessPresenter(this, process);
			ProjectExplorerPresenter = projectExplorerPresenter;
		}

		protected override void OnCreateControl()
		{
			if (!processEditor.SplitContainer.Panel1.Controls.Contains(viewInfoBar))
			{
				viewInfoBar = new ProcessViewInfoBar(process, ProjectExplorerPresenter) {Dock = DockStyle.Top};
				processEditor.SplitContainer.Panel1.Controls.Add(viewInfoBar);
			}

			base.OnCreateControl();
		}

		#region IProcessView Members

		public IProcessPresenter Presenter
		{
			get;
			set;
		}

		public IProjectExplorerPresenter ProjectExplorerPresenter
		{
			get;
			private set;
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
			get { return process; }
		}

		public void SetProcessName(string processName)
		{
			Text = processName;
		}

		#endregion

		private void processView_Activated(object sender, EventArgs e)
		{
			if (ProjectExplorerPresenter != null)
			{
				ProjectExplorerPresenter.ProcessSelected(this);
			}

			ToolStripManager.Merge(toolStripProcess, "mainToolStrip");
		}

		private void processView_Deactivate(object sender, EventArgs e)
		{
			if (ProjectExplorerPresenter != null)
			{
				ProjectExplorerPresenter.ProcessDeselected();
			}
	
			ToolStripManager.RevertMerge("mainToolStrip");
		}

		private void processView_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (userClickedCloseBox(e))
			{
				hideInsteadOfClosing(e);
			}
		}

		private void ProcessView_Load(object sender, EventArgs e)
		{
			Text = process.Name;

			ApplicationPresenter.EditDropDownOpened += applicationPresenter_EditDropDownOpened;
			Application.Idle += application_Idle;
		}

		void application_Idle(object sender, EventArgs e)
		{
			toolStripButtonCut.Enabled = toolStripMenuItemCut.Enabled = processStatementsOrProcessAreCuttable();

			toolStripButtonCopy.Enabled = toolStripMenuItemCopy.Enabled = processStatementsOrProcessAreCopyable();

			toolStripButtonPaste.Enabled = toolStripMenuItemPaste.Enabled = processStatementsOrProjectComponentArePasteable();

			toolStripButtonDelete.Enabled = toolStripMenuItemDelete.Enabled = processStatementsOrProcessAreDeletable();

			toolStripButtonUndo.Enabled = toolStripMenuItemUndo.Enabled = processEditor.CanUndo();
			toolStripButtonUndo.Text = toolStripMenuItemUndo.Text = "Undo " + processEditor.UndoActionText;

			toolStripButtonRedo.Enabled = toolStripMenuItemRedo.Enabled = processEditor.CanRedo();
			toolStripButtonRedo.Text = toolStripMenuItemRedo.Text = "Redo " + processEditor.RedoActionText;
		}

		private bool processStatementsOrProcessAreCuttable()
		{
			return (ProjectExplorerPresenter.ProjectExplorerHasFocus ? ProjectExplorerPresenter.CanCutProcess(this) : canCutSelectedProcessLines());
		}

		private bool canCutSelectedProcessLines()
		{
			return processEditor.CanCut();
		}

		private bool processStatementsOrProcessAreCopyable()
		{
			return (ProjectExplorerPresenter.ProjectExplorerHasFocus ? ProjectExplorerPresenter.CanCopyProcess(this) : canCopySelectedProcessLines());
		}

		private bool canCopySelectedProcessLines()
		{
			return processEditor.CanCopy();
		}

		private bool processStatementsOrProjectComponentArePasteable()
		{
			return (ProjectExplorerPresenter.ProjectExplorerHasFocus ? ProjectExplorerPresenter.CanPasteComponent : canPasteProcessLines());
		}

		private bool canPasteProcessLines()
		{
			return processEditor.CanPaste();
		}

		private bool processStatementsOrProcessAreDeletable()
		{
			return (ProjectExplorerPresenter.ProjectExplorerHasFocus ? ProjectExplorerPresenter.CanDeleteProcess(this) : canDeleteSelectedProcessLines());
		}

		private bool canDeleteSelectedProcessLines()
		{
			return processEditor.CanDelete();
		}

		void applicationPresenter_EditDropDownOpened(object sender, DropDownOpenedEventArgs e)
		{
			showOrHideRenameItem();
		}

		private void showOrHideRenameItem()
		{
			toolStripMenuItemRename.Visible = ProjectExplorerPresenter.ProcessIsSelected;
		}

		private void toolStripMenuItemRename_Click(object sender, EventArgs e)
		{
			ProjectExplorerPresenter.EditComponentName();
		}

		private void toolStripButtonCut_Click(object sender, EventArgs e)
		{
			cutProcessStatementsOrProcess();
		}

		private void cutProcessStatementsOrProcess()
		{
			if (ProjectExplorerPresenter.ProjectExplorerHasFocus)
			{
				ProjectExplorerPresenter.ProcessCutRequested(this);
			}
			else
			{
				processEditor.Cut();
			}
		}

		private void toolStripButtonCopy_Click(object sender, EventArgs e)
		{
			copyProcessStatementsOrProcess();
		}

		private void copyProcessStatementsOrProcess()
		{
			if (ProjectExplorerPresenter.ProjectExplorerHasFocus)
			{
				ProjectExplorerPresenter.ProcessCopyRequested(this);
			}
			else
			{
				processEditor.Copy();
			}
		}

		private void toolStripButtonPaste_Click(object sender, EventArgs e)
		{
			pasteProcessStatementsOrProjectComponent();
		}

		private void pasteProcessStatementsOrProjectComponent()
		{
			if (ProjectExplorerPresenter.ProjectExplorerHasFocus)
			{
				ProjectExplorerPresenter.ComponentPasteRequested();
			}
			else
			{
				processEditor.Paste();
			}
		}

		private void toolStripButtonDelete_Click(object sender, EventArgs e)
		{
			deleteProcessStatementsOrProcess();
		}

		private void deleteProcessStatementsOrProcess()
		{
			if (ProjectExplorerPresenter.ProjectExplorerHasFocus)
			{
				ProjectExplorerPresenter.ProcessDeleteRequested(this);
			}
			else
			{
				processEditor.Delete();
			}
		}

		private void toolStripButtonUndo_Click(object sender, EventArgs e)
		{
			processEditor.Undo();
		}

		private void toolStripButtonRedo_Click(object sender, EventArgs e)
		{
			processEditor.Redo();
		}
	}
}
