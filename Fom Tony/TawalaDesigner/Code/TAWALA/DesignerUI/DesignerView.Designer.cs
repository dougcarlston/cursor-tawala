// $Workfile: DesignerView.Designer.cs $
// $Revision: 61 $	$Date: 12/06/07 12:24p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.DesignerUI
{
	partial class DesignerView
	{
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStrip mainToolStrip;
		private System.Windows.Forms.ToolStripSeparator miSep1;
		private System.Windows.Forms.ToolStripMenuItem fileAddNewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileAddNewFormToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileAddNewProcessToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileDeployToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileExitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuItemEdit;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuView;
		private System.Windows.Forms.ToolStripMenuItem menuItemViewToolbar;
		private System.Windows.Forms.ToolStripMenuItem menuItemViewStatusBar;
		private System.Windows.Forms.ToolStripMenuItem menuItemInsert;
		private System.Windows.Forms.ToolStripMenuItem menuItemFormat;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuItemHelp;
		private System.Windows.Forms.ToolStripMenuItem menuItemHelpAbout;
		private System.Windows.Forms.ToolStripSeparator miSep4;
		private System.Windows.Forms.ToolStripSeparator miSep2;
		private System.Windows.Forms.ToolStripMenuItem fileAddNewDocumentToolStripMenuItem;

		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileNewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileSaveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileSaveToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator miSep3;
		private System.Windows.Forms.ToolStripMenuItem fileOpenToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuItemEditCut;
		private System.Windows.Forms.ToolStripMenuItem menuItemEditCopy;
		private System.Windows.Forms.ToolStripMenuItem menuItemEditPaste;
		private System.Windows.Forms.ToolStripMenuItem menuItemEditDelete;
		private System.Windows.Forms.ToolStripButton tbbNewProject;
		private System.Windows.Forms.ToolStripButton tbbOpenProject;
		private System.Windows.Forms.ToolStripButton tbbSaveProject;
		private System.Windows.Forms.ToolStripButton tbbDeployProject;
		private System.Windows.Forms.ToolStripSeparator tbbSep1;
		private System.Windows.Forms.ToolStripButton tbbCut;
		private System.Windows.Forms.ToolStripButton tbbCopy;
		private System.Windows.Forms.ToolStripButton tbbPaste;
		private System.Windows.Forms.ToolStripButton tbbDelete;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileAddNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileAddNewDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileAddNewFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileAddNewProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.fileSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.fileDeployToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep4 = new System.Windows.Forms.ToolStripSeparator();
            this.filePrintPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filePrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.fileExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditRename = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemEditUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuViewProjectPane = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuViewFieldsPalette = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemViewToolbar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewStatusBar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemInsert = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemHeader = new System.Windows.Forms.ToolStripMenuItem();
            this.projectThemesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsCascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsTileHorzToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsTileVertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsCloseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windows1ToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.tbbNewProject = new System.Windows.Forms.ToolStripButton();
            this.tbbOpenProject = new System.Windows.Forms.ToolStripButton();
            this.tbbSaveProject = new System.Windows.Forms.ToolStripButton();
            this.tbbDeployProject = new System.Windows.Forms.ToolStripButton();
            this.tbbSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbbCut = new System.Windows.Forms.ToolStripButton();
            this.tbbCopy = new System.Windows.Forms.ToolStripButton();
            this.tbbPaste = new System.Windows.Forms.ToolStripButton();
            this.tbbDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.projectManagerToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelFunctions = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBarFunctions = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.fieldsPalette = new Tawala.ProjectUI.FieldsPalette();
            this.projectPane = new Tawala.DesignerUI.ProjectExplorer();
            this.menuStrip.SuspendLayout();
            this.mainToolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.menuItemEdit,
            this.toolStripMenuView,
            this.menuItemInsert,
            this.menuItemFormat,
            this.toolsToolStripMenuItem,
            this.windowsToolStripMenuItem,
            this.menuItemHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.MdiWindowListItem = this.windowsToolStripMenuItem;
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(992, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileNewToolStripMenuItem,
            this.fileOpenToolStripMenuItem,
            this.miSep1,
            this.fileAddNewToolStripMenuItem,
            this.miSep2,
            this.fileSaveToolStripMenuItem,
            this.fileSaveAsToolStripMenuItem,
            this.miSep3,
            this.fileDeployToolStripMenuItem,
            this.miSep4,
            this.filePrintPreviewToolStripMenuItem,
            this.filePrintToolStripMenuItem,
            this.toolStripSeparator3,
            this.fileExitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // fileNewToolStripMenuItem
            // 
            this.fileNewToolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.Project_New;
            this.fileNewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileNewToolStripMenuItem.Name = "fileNewToolStripMenuItem";
            this.fileNewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.fileNewToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.fileNewToolStripMenuItem.Text = "&New Project...";
            this.fileNewToolStripMenuItem.Click += new System.EventHandler(this.fileNewToolStripMenuItem_Click);
            // 
            // fileOpenToolStripMenuItem
            // 
            this.fileOpenToolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.Folder_Open;
            this.fileOpenToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileOpenToolStripMenuItem.Name = "fileOpenToolStripMenuItem";
            this.fileOpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.fileOpenToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.fileOpenToolStripMenuItem.Text = "&Open Project...";
            this.fileOpenToolStripMenuItem.Click += new System.EventHandler(this.fileOpenToolStripMenuItem_Click);
            // 
            // miSep1
            // 
            this.miSep1.Name = "miSep1";
            this.miSep1.Size = new System.Drawing.Size(192, 6);
            // 
            // fileAddNewToolStripMenuItem
            // 
            this.fileAddNewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileAddNewDocumentToolStripMenuItem,
            this.fileAddNewFormToolStripMenuItem,
            this.fileAddNewProcessToolStripMenuItem});
            this.fileAddNewToolStripMenuItem.Name = "fileAddNewToolStripMenuItem";
            this.fileAddNewToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.fileAddNewToolStripMenuItem.Text = "Add Ne&w";
            // 
            // fileAddNewDocumentToolStripMenuItem
            // 
            this.fileAddNewDocumentToolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.Document_New;
            this.fileAddNewDocumentToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileAddNewDocumentToolStripMenuItem.Name = "fileAddNewDocumentToolStripMenuItem";
            this.fileAddNewDocumentToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.fileAddNewDocumentToolStripMenuItem.Text = "&Document";
            this.fileAddNewDocumentToolStripMenuItem.Click += new System.EventHandler(this.fileAddNewDocumentToolStripMenuItem_Click);
            // 
            // fileAddNewFormToolStripMenuItem
            // 
            this.fileAddNewFormToolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.Form_New;
            this.fileAddNewFormToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileAddNewFormToolStripMenuItem.Name = "fileAddNewFormToolStripMenuItem";
            this.fileAddNewFormToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.fileAddNewFormToolStripMenuItem.Text = "&Form";
            this.fileAddNewFormToolStripMenuItem.Click += new System.EventHandler(this.fileAddNewFormToolStripMenuItem_Click);
            // 
            // fileAddNewProcessToolStripMenuItem
            // 
            this.fileAddNewProcessToolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.Process_New;
            this.fileAddNewProcessToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileAddNewProcessToolStripMenuItem.Name = "fileAddNewProcessToolStripMenuItem";
            this.fileAddNewProcessToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.fileAddNewProcessToolStripMenuItem.Text = "&Process";
            this.fileAddNewProcessToolStripMenuItem.Click += new System.EventHandler(this.fileAddNewProcessToolStripMenuItem_Click);
            // 
            // miSep2
            // 
            this.miSep2.Name = "miSep2";
            this.miSep2.Size = new System.Drawing.Size(192, 6);
            // 
            // fileSaveToolStripMenuItem
            // 
            this.fileSaveToolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.File_Save;
            this.fileSaveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileSaveToolStripMenuItem.Name = "fileSaveToolStripMenuItem";
            this.fileSaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.fileSaveToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.fileSaveToolStripMenuItem.Text = "&Save";
            this.fileSaveToolStripMenuItem.Click += new System.EventHandler(this.fileSaveToolStripMenuItem_Click);
            // 
            // fileSaveAsToolStripMenuItem
            // 
            this.fileSaveAsToolStripMenuItem.Name = "fileSaveAsToolStripMenuItem";
            this.fileSaveAsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.fileSaveAsToolStripMenuItem.Text = "Save &As...";
            this.fileSaveAsToolStripMenuItem.Click += new System.EventHandler(this.fileSaveAsToolStripMenuItem_Click);
            // 
            // miSep3
            // 
            this.miSep3.Name = "miSep3";
            this.miSep3.Size = new System.Drawing.Size(192, 6);
            // 
            // fileDeployToolStripMenuItem
            // 
            this.fileDeployToolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.Project_Deploy;
            this.fileDeployToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileDeployToolStripMenuItem.Name = "fileDeployToolStripMenuItem";
            this.fileDeployToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.fileDeployToolStripMenuItem.Text = "&Deploy Project";
            this.fileDeployToolStripMenuItem.Click += new System.EventHandler(this.fileDeployToolStripMenuItem_Click);
            // 
            // miSep4
            // 
            this.miSep4.Name = "miSep4";
            this.miSep4.Size = new System.Drawing.Size(192, 6);
            // 
            // filePrintPreviewToolStripMenuItem
            // 
            this.filePrintPreviewToolStripMenuItem.Enabled = false;
            this.filePrintPreviewToolStripMenuItem.Name = "filePrintPreviewToolStripMenuItem";
            this.filePrintPreviewToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.filePrintPreviewToolStripMenuItem.Text = "Print Preview...";
            this.filePrintPreviewToolStripMenuItem.Click += new System.EventHandler(this.filePrintPreviewToolStripMenuItem_Click);
            // 
            // filePrintToolStripMenuItem
            // 
            this.filePrintToolStripMenuItem.Enabled = false;
            this.filePrintToolStripMenuItem.Name = "filePrintToolStripMenuItem";
            this.filePrintToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.filePrintToolStripMenuItem.Text = "Print...";
            this.filePrintToolStripMenuItem.Click += new System.EventHandler(this.filePrintToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(192, 6);
            // 
            // fileExitToolStripMenuItem
            // 
            this.fileExitToolStripMenuItem.Name = "fileExitToolStripMenuItem";
            this.fileExitToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.fileExitToolStripMenuItem.Text = "E&xit";
            this.fileExitToolStripMenuItem.Click += new System.EventHandler(this.fileExitToolStripMenuItem_Click);
            // 
            // menuItemEdit
            // 
            this.menuItemEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemEditCut,
            this.menuItemEditCopy,
            this.menuItemEditPaste,
            this.menuItemEditDelete,
            this.menuItemEditRename,
            this.menuItemEditSeparator,
            this.menuItemEditUndo,
            this.menuItemEditRedo});
            this.menuItemEdit.Name = "menuItemEdit";
            this.menuItemEdit.Size = new System.Drawing.Size(39, 20);
            this.menuItemEdit.Text = "&Edit";
            this.menuItemEdit.DropDownOpening += new System.EventHandler(this.menuItemEdit_DropDownOpening);
            this.menuItemEdit.DropDownClosed += new System.EventHandler(this.menuItemEdit_DropDownClosed);
            // 
            // menuItemEditCut
            // 
            this.menuItemEditCut.Enabled = false;
            this.menuItemEditCut.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Cut;
            this.menuItemEditCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuItemEditCut.Name = "menuItemEditCut";
            this.menuItemEditCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.menuItemEditCut.Size = new System.Drawing.Size(144, 22);
            this.menuItemEditCut.Text = "Cu&t";
            this.menuItemEditCut.Click += new System.EventHandler(this.menuItemEditCut_Click);
            // 
            // menuItemEditCopy
            // 
            this.menuItemEditCopy.Enabled = false;
            this.menuItemEditCopy.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Copy;
            this.menuItemEditCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuItemEditCopy.Name = "menuItemEditCopy";
            this.menuItemEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuItemEditCopy.Size = new System.Drawing.Size(144, 22);
            this.menuItemEditCopy.Text = "&Copy";
            this.menuItemEditCopy.Click += new System.EventHandler(this.menuItemEditCopy_Click);
            // 
            // menuItemEditPaste
            // 
            this.menuItemEditPaste.Enabled = false;
            this.menuItemEditPaste.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Paste;
            this.menuItemEditPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuItemEditPaste.Name = "menuItemEditPaste";
            this.menuItemEditPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.menuItemEditPaste.Size = new System.Drawing.Size(144, 22);
            this.menuItemEditPaste.Text = "&Paste";
            this.menuItemEditPaste.Click += new System.EventHandler(this.menuItemEditPaste_Click);
            // 
            // menuItemEditDelete
            // 
            this.menuItemEditDelete.Enabled = false;
            this.menuItemEditDelete.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Delete;
            this.menuItemEditDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuItemEditDelete.Name = "menuItemEditDelete";
            this.menuItemEditDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.menuItemEditDelete.Size = new System.Drawing.Size(144, 22);
            this.menuItemEditDelete.Text = "&Delete";
            this.menuItemEditDelete.Click += new System.EventHandler(this.menuItemEditDelete_Click);
            // 
            // menuItemEditRename
            // 
            this.menuItemEditRename.Enabled = false;
            this.menuItemEditRename.Name = "menuItemEditRename";
            this.menuItemEditRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.menuItemEditRename.Size = new System.Drawing.Size(144, 22);
            this.menuItemEditRename.Text = "&Rename";
            this.menuItemEditRename.Visible = false;
            this.menuItemEditRename.Click += new System.EventHandler(this.menuItemEditRename_Click);
            // 
            // menuItemEditSeparator
            // 
            this.menuItemEditSeparator.Name = "menuItemEditSeparator";
            this.menuItemEditSeparator.Size = new System.Drawing.Size(141, 6);
            this.menuItemEditSeparator.Visible = false;
            // 
            // menuItemEditUndo
            // 
            this.menuItemEditUndo.Enabled = false;
            this.menuItemEditUndo.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Undo;
            this.menuItemEditUndo.Name = "menuItemEditUndo";
            this.menuItemEditUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.menuItemEditUndo.Size = new System.Drawing.Size(144, 22);
            this.menuItemEditUndo.Text = "&Undo";
            this.menuItemEditUndo.Click += new System.EventHandler(this.menuItemEditUndo_Click);
            // 
            // menuItemEditRedo
            // 
            this.menuItemEditRedo.Enabled = false;
            this.menuItemEditRedo.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Redo;
            this.menuItemEditRedo.Name = "menuItemEditRedo";
            this.menuItemEditRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.menuItemEditRedo.Size = new System.Drawing.Size(144, 22);
            this.menuItemEditRedo.Text = "R&edo";
            this.menuItemEditRedo.Click += new System.EventHandler(this.menuItemEditRedo_Click);
            // 
            // toolStripMenuView
            // 
            this.toolStripMenuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuViewProjectPane,
            this.toolStripMenuViewFieldsPalette,
            this.toolStripSeparator2,
            this.menuItemViewToolbar,
            this.menuItemViewStatusBar});
            this.toolStripMenuView.Name = "toolStripMenuView";
            this.toolStripMenuView.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuView.Text = "&View";
            this.toolStripMenuView.DropDownOpening += new System.EventHandler(this.toolStripMenuView_DropDownOpening);
            // 
            // toolStripMenuViewProjectPane
            // 
            this.toolStripMenuViewProjectPane.Name = "toolStripMenuViewProjectPane";
            this.toolStripMenuViewProjectPane.Size = new System.Drawing.Size(156, 22);
            this.toolStripMenuViewProjectPane.Text = "Project Explorer";
            this.toolStripMenuViewProjectPane.Click += new System.EventHandler(this.toolStripMenuViewProjectPane_Click);
            // 
            // toolStripMenuViewFieldsPalette
            // 
            this.toolStripMenuViewFieldsPalette.Name = "toolStripMenuViewFieldsPalette";
            this.toolStripMenuViewFieldsPalette.Size = new System.Drawing.Size(156, 22);
            this.toolStripMenuViewFieldsPalette.Text = "Fields Palette";
            this.toolStripMenuViewFieldsPalette.Click += new System.EventHandler(this.toolStripMenuViewFieldsPalette_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(153, 6);
            // 
            // menuItemViewToolbar
            // 
            this.menuItemViewToolbar.Checked = true;
            this.menuItemViewToolbar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuItemViewToolbar.Name = "menuItemViewToolbar";
            this.menuItemViewToolbar.Size = new System.Drawing.Size(156, 22);
            this.menuItemViewToolbar.Text = "&Toolbar";
            this.menuItemViewToolbar.Click += new System.EventHandler(this.menuItemViewToolbar_Click);
            // 
            // menuItemViewStatusBar
            // 
            this.menuItemViewStatusBar.Checked = true;
            this.menuItemViewStatusBar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuItemViewStatusBar.Name = "menuItemViewStatusBar";
            this.menuItemViewStatusBar.Size = new System.Drawing.Size(156, 22);
            this.menuItemViewStatusBar.Text = "&Status Bar";
            this.menuItemViewStatusBar.Click += new System.EventHandler(this.menuItemViewStatusBar_Click);
            // 
            // menuItemInsert
            // 
            this.menuItemInsert.Name = "menuItemInsert";
            this.menuItemInsert.Size = new System.Drawing.Size(48, 20);
            this.menuItemInsert.Text = "&Insert";
            // 
            // menuItemFormat
            // 
            this.menuItemFormat.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemHeader,
            this.projectThemesToolStripMenuItem});
            this.menuItemFormat.Name = "menuItemFormat";
            this.menuItemFormat.Size = new System.Drawing.Size(57, 20);
            this.menuItemFormat.Text = "F&ormat";
            this.menuItemFormat.DropDownOpening += new System.EventHandler(this.menuItemFormat_DropDownOpening);
            // 
            // toolStripMenuItemHeader
            // 
            this.toolStripMenuItemHeader.Name = "toolStripMenuItemHeader";
            this.toolStripMenuItemHeader.Size = new System.Drawing.Size(156, 22);
            this.toolStripMenuItemHeader.Text = "Page &Header...";
            this.toolStripMenuItemHeader.Click += new System.EventHandler(this.toolStripMenuItemHeader_Click);
            // 
            // projectThemesToolStripMenuItem
            // 
            this.projectThemesToolStripMenuItem.Name = "projectThemesToolStripMenuItem";
            this.projectThemesToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.projectThemesToolStripMenuItem.Text = "&Project Themes";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectManagerToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // projectManagerToolStripMenuItem
            // 
            this.projectManagerToolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.Project_Manager;
            this.projectManagerToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.projectManagerToolStripMenuItem.Name = "projectManagerToolStripMenuItem";
            this.projectManagerToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.projectManagerToolStripMenuItem.Text = "Project Manager...";
            this.projectManagerToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItemProjectManager_Click);
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowsCascadeToolStripMenuItem,
            this.windowsTileHorzToolStripMenuItem,
            this.windowsTileVertToolStripMenuItem,
            this.windowsCloseAllToolStripMenuItem,
            this.windows1ToolStripSeparator});
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.windowsToolStripMenuItem.Text = "&Windows";
            this.windowsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.toolStripMenuItemWindows_DropDownOpening);
            // 
            // windowsCascadeToolStripMenuItem
            // 
            this.windowsCascadeToolStripMenuItem.Name = "windowsCascadeToolStripMenuItem";
            this.windowsCascadeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.windowsCascadeToolStripMenuItem.Text = "&Cascade";
            this.windowsCascadeToolStripMenuItem.Click += new System.EventHandler(this.windowsCascadeToolStripMenuItem_Click);
            // 
            // windowsTileHorzToolStripMenuItem
            // 
            this.windowsTileHorzToolStripMenuItem.Name = "windowsTileHorzToolStripMenuItem";
            this.windowsTileHorzToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.windowsTileHorzToolStripMenuItem.Text = "Tile &Horizontally";
            this.windowsTileHorzToolStripMenuItem.Click += new System.EventHandler(this.windowsTileHorzToolStripMenuItem_Click);
            // 
            // windowsTileVertToolStripMenuItem
            // 
            this.windowsTileVertToolStripMenuItem.Name = "windowsTileVertToolStripMenuItem";
            this.windowsTileVertToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.windowsTileVertToolStripMenuItem.Text = "Tile &Vertically";
            this.windowsTileVertToolStripMenuItem.Click += new System.EventHandler(this.windowsTileVertToolStripMenuItem_Click_Click);
            // 
            // windowsCloseAllToolStripMenuItem
            // 
            this.windowsCloseAllToolStripMenuItem.Name = "windowsCloseAllToolStripMenuItem";
            this.windowsCloseAllToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.windowsCloseAllToolStripMenuItem.Text = "Close &All";
            this.windowsCloseAllToolStripMenuItem.Click += new System.EventHandler(this.windowsCloseAllToolStripMenuItem_Click);
            // 
            // windows1ToolStripSeparator
            // 
            this.windows1ToolStripSeparator.Name = "windows1ToolStripSeparator";
            this.windows1ToolStripSeparator.Size = new System.Drawing.Size(157, 6);
            // 
            // menuItemHelp
            // 
            this.menuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemHelpAbout});
            this.menuItemHelp.Name = "menuItemHelp";
            this.menuItemHelp.Size = new System.Drawing.Size(44, 20);
            this.menuItemHelp.Text = "&Help";
            // 
            // menuItemHelpAbout
            // 
            this.menuItemHelpAbout.Image = global::Tawala.DesignerUI.Properties.Resources.Help_Information;
            this.menuItemHelpAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuItemHelpAbout.Name = "menuItemHelpAbout";
            this.menuItemHelpAbout.Size = new System.Drawing.Size(107, 22);
            this.menuItemHelpAbout.Text = "&About";
            this.menuItemHelpAbout.Click += new System.EventHandler(this.menuItemHelpAbout_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 6);
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbNewProject,
            this.tbbOpenProject,
            this.tbbSaveProject,
            this.tbbDeployProject,
            this.tbbSep1,
            this.tbbCut,
            this.tbbCopy,
            this.tbbPaste,
            this.tbbDelete,
            this.toolStripSeparator5,
            this.toolStripButtonUndo,
            this.toolStripButtonRedo,
            this.toolStripSeparator1,
            this.projectManagerToolStripButton});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 24);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(992, 25);
            this.mainToolStrip.TabIndex = 1;
            // 
            // tbbNewProject
            // 
            this.tbbNewProject.Image = global::Tawala.DesignerUI.Properties.Resources.Project_New;
            this.tbbNewProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbNewProject.Name = "tbbNewProject";
            this.tbbNewProject.Size = new System.Drawing.Size(23, 22);
            this.tbbNewProject.ToolTipText = "New Project";
            this.tbbNewProject.Click += new System.EventHandler(this.fileNewToolStripMenuItem_Click);
            // 
            // tbbOpenProject
            // 
            this.tbbOpenProject.Image = global::Tawala.DesignerUI.Properties.Resources.Folder_OpenWithArrow;
            this.tbbOpenProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbOpenProject.Name = "tbbOpenProject";
            this.tbbOpenProject.Size = new System.Drawing.Size(23, 22);
            this.tbbOpenProject.ToolTipText = "Open Project";
            this.tbbOpenProject.Click += new System.EventHandler(this.fileOpenToolStripMenuItem_Click);
            // 
            // tbbSaveProject
            // 
            this.tbbSaveProject.Image = global::Tawala.DesignerUI.Properties.Resources.File_Save;
            this.tbbSaveProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbSaveProject.Name = "tbbSaveProject";
            this.tbbSaveProject.Size = new System.Drawing.Size(23, 22);
            this.tbbSaveProject.ToolTipText = "Save Project";
            this.tbbSaveProject.Click += new System.EventHandler(this.fileSaveToolStripMenuItem_Click);
            // 
            // tbbDeployProject
            // 
            this.tbbDeployProject.Image = global::Tawala.DesignerUI.Properties.Resources.Project_Deploy;
            this.tbbDeployProject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbDeployProject.Name = "tbbDeployProject";
            this.tbbDeployProject.Size = new System.Drawing.Size(23, 22);
            this.tbbDeployProject.ToolTipText = "Deploy Project";
            this.tbbDeployProject.Click += new System.EventHandler(this.fileDeployToolStripMenuItem_Click);
            // 
            // tbbSep1
            // 
            this.tbbSep1.Name = "tbbSep1";
            this.tbbSep1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbbCut
            // 
            this.tbbCut.Enabled = false;
            this.tbbCut.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Cut;
            this.tbbCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbCut.Name = "tbbCut";
            this.tbbCut.Size = new System.Drawing.Size(23, 22);
            this.tbbCut.ToolTipText = "Cut";
            this.tbbCut.Click += new System.EventHandler(this.menuItemEditCut_Click);
            // 
            // tbbCopy
            // 
            this.tbbCopy.Enabled = false;
            this.tbbCopy.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Copy;
            this.tbbCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbCopy.Name = "tbbCopy";
            this.tbbCopy.Size = new System.Drawing.Size(23, 22);
            this.tbbCopy.ToolTipText = "Copy";
            this.tbbCopy.Click += new System.EventHandler(this.menuItemEditCopy_Click);
            // 
            // tbbPaste
            // 
            this.tbbPaste.Enabled = false;
            this.tbbPaste.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Paste;
            this.tbbPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbPaste.Name = "tbbPaste";
            this.tbbPaste.Size = new System.Drawing.Size(23, 22);
            this.tbbPaste.ToolTipText = "Paste";
            this.tbbPaste.Click += new System.EventHandler(this.menuItemEditPaste_Click);
            // 
            // tbbDelete
            // 
            this.tbbDelete.Enabled = false;
            this.tbbDelete.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Delete;
            this.tbbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbDelete.Name = "tbbDelete";
            this.tbbDelete.Size = new System.Drawing.Size(23, 22);
            this.tbbDelete.ToolTipText = "Delete";
            this.tbbDelete.Click += new System.EventHandler(this.menuItemEditDelete_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonUndo
            // 
            this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUndo.Enabled = false;
            this.toolStripButtonUndo.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Undo;
            this.toolStripButtonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndo.Name = "toolStripButtonUndo";
            this.toolStripButtonUndo.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUndo.Text = "Undo";
            this.toolStripButtonUndo.Click += new System.EventHandler(this.menuItemEditUndo_Click);
            // 
            // toolStripButtonRedo
            // 
            this.toolStripButtonRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRedo.Enabled = false;
            this.toolStripButtonRedo.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Redo;
            this.toolStripButtonRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRedo.Name = "toolStripButtonRedo";
            this.toolStripButtonRedo.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRedo.Text = "Redo";
            this.toolStripButtonRedo.Click += new System.EventHandler(this.menuItemEditRedo_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // projectManagerToolStripButton
            // 
            this.projectManagerToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.projectManagerToolStripButton.Image = global::Tawala.DesignerUI.Properties.Resources.Project_Manager;
            this.projectManagerToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.projectManagerToolStripButton.Name = "projectManagerToolStripButton";
            this.projectManagerToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.projectManagerToolStripButton.Text = "Project Manager";
            this.projectManagerToolStripButton.Click += new System.EventHandler(this.toolStripMenuItemProjectManager_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelFunctions,
            this.toolStripProgressBarFunctions,
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 642);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(992, 22);
            this.statusStrip.TabIndex = 2;
            // 
            // toolStripStatusLabelFunctions
            // 
            this.toolStripStatusLabelFunctions.Image = global::Tawala.DesignerUI.Properties.Resources.Function;
            this.toolStripStatusLabelFunctions.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripStatusLabelFunctions.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusLabelFunctions.Name = "toolStripStatusLabelFunctions";
            this.toolStripStatusLabelFunctions.Size = new System.Drawing.Size(140, 17);
            this.toolStripStatusLabelFunctions.Text = "Retrieving Functions...";
            this.toolStripStatusLabelFunctions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStripProgressBarFunctions
            // 
            this.toolStripProgressBarFunctions.Name = "toolStripProgressBarFunctions";
            this.toolStripProgressBarFunctions.Size = new System.Drawing.Size(200, 16);
            this.toolStripProgressBarFunctions.Step = 20;
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(635, 17);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripStatusLabel.Visible = false;
            // 
            // fieldsPalette
            // 
            this.fieldsPalette.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fieldsPalette.BackColor = System.Drawing.SystemColors.Control;
            this.fieldsPalette.Dock = System.Windows.Forms.DockStyle.Right;
            this.fieldsPalette.Location = new System.Drawing.Point(872, 49);
            this.fieldsPalette.Name = "fieldsPalette";
            this.fieldsPalette.Size = new System.Drawing.Size(120, 593);
            this.fieldsPalette.TabIndex = 7;
            // 
            // projectPane
            // 
            this.projectPane.BackColor = System.Drawing.Color.White;
            this.projectPane.Dock = System.Windows.Forms.DockStyle.Left;
            this.projectPane.Location = new System.Drawing.Point(0, 49);
            this.projectPane.Margin = new System.Windows.Forms.Padding(0);
            this.projectPane.MinimumSize = new System.Drawing.Size(186, 0);
            this.projectPane.Name = "projectPane";
            this.projectPane.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.projectPane.Size = new System.Drawing.Size(186, 593);
            this.projectPane.TabIndex = 11;
            // 
            // DesignerView
            // 
            this.ClientSize = new System.Drawing.Size(992, 664);
            this.Controls.Add(this.projectPane);
            this.Controls.Add(this.fieldsPalette);
            this.Controls.Add(this.mainToolStrip);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = global::Tawala.DesignerUI.Properties.Resources.App;
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(900, 700);
            this.Name = "DesignerView";
            this.Text = "Tawala Project Designer - ";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
		private System.Windows.Forms.ToolStripMenuItem menuItemEditRename;
        private System.Windows.Forms.ToolStripSeparator menuItemEditSeparator;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem projectManagerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowsCascadeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowsTileHorzToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowsTileVertToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator windows1ToolStripSeparator;
		private System.Windows.Forms.ToolStripMenuItem windowsCloseAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuViewProjectPane;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem filePrintPreviewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem filePrintToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem projectThemesToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton projectManagerToolStripButton;
		private Tawala.ProjectUI.FieldsPalette fieldsPalette;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuViewFieldsPalette;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarFunctions;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFunctions;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHeader;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripButton toolStripButtonUndo;
		private System.Windows.Forms.ToolStripButton toolStripButtonRedo;
		private System.Windows.Forms.ToolStripMenuItem menuItemEditUndo;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditRedo;
        private ProjectExplorer projectPane;
	}
}
