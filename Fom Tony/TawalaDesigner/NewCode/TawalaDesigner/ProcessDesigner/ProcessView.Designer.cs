namespace Tawala.ProcessDesigner
{
	partial class ProcessView
	{
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
			this.processEditor = new Tawala.Processes.ProcessEditor();
			this.menuStripProcess = new System.Windows.Forms.MenuStrip();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCut = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemRename = new System.Windows.Forms.ToolStripMenuItem();
			this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ifStatementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showStatementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sendStatementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.appendStatementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.getStatementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.forEachStatementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteStatementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setStatementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.commentStatementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripProcess = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonCut = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonCopy = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonPaste = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonRedo = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemRedo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.menuStripProcess.SuspendLayout();
			this.toolStripProcess.SuspendLayout();
			this.SuspendLayout();
			// 
			// processEditor
			// 
			this.processEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.processEditor.Location = new System.Drawing.Point(0, 24);
			this.processEditor.Margin = new System.Windows.Forms.Padding(0);
			this.processEditor.Name = "processEditor";
			this.processEditor.Process = null;
			this.processEditor.Size = new System.Drawing.Size(562, 342);
			this.processEditor.TabIndex = 0;
			// 
			// menuStripProcess
			// 
			this.menuStripProcess.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.insertToolStripMenuItem});
			this.menuStripProcess.Location = new System.Drawing.Point(0, 0);
			this.menuStripProcess.Name = "menuStripProcess";
			this.menuStripProcess.Size = new System.Drawing.Size(562, 24);
			this.menuStripProcess.TabIndex = 1;
			this.menuStripProcess.Visible = false;
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCut,
            this.toolStripMenuItemCopy,
            this.toolStripMenuItemPaste,
            this.toolStripMenuItemDelete,
            this.toolStripMenuItemRename,
            this.toolStripSeparator2,
            this.toolStripMenuItemUndo,
            this.toolStripMenuItemRedo});
			this.editToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// toolStripMenuItemCut
			// 
			this.toolStripMenuItemCut.Image = global::Tawala.ProcessDesigner.Properties.Resources.Edit_Cut;
			this.toolStripMenuItemCut.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripMenuItemCut.Name = "toolStripMenuItemCut";
			this.toolStripMenuItemCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.toolStripMenuItemCut.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemCut.Text = "Cut";
			this.toolStripMenuItemCut.Click += new System.EventHandler(this.toolStripButtonCut_Click);
			// 
			// toolStripMenuItemCopy
			// 
			this.toolStripMenuItemCopy.Image = global::Tawala.ProcessDesigner.Properties.Resources.Edit_Copy;
			this.toolStripMenuItemCopy.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripMenuItemCopy.Name = "toolStripMenuItemCopy";
			this.toolStripMenuItemCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.toolStripMenuItemCopy.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemCopy.Text = "Copy";
			this.toolStripMenuItemCopy.Click += new System.EventHandler(this.toolStripButtonCopy_Click);
			// 
			// toolStripMenuItemPaste
			// 
			this.toolStripMenuItemPaste.Image = global::Tawala.ProcessDesigner.Properties.Resources.Edit_Paste;
			this.toolStripMenuItemPaste.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripMenuItemPaste.Name = "toolStripMenuItemPaste";
			this.toolStripMenuItemPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.toolStripMenuItemPaste.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemPaste.Text = "Paste";
			this.toolStripMenuItemPaste.Click += new System.EventHandler(this.toolStripButtonPaste_Click);
			// 
			// toolStripMenuItemDelete
			// 
			this.toolStripMenuItemDelete.Image = global::Tawala.ProcessDesigner.Properties.Resources.Edit_Delete;
			this.toolStripMenuItemDelete.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
			this.toolStripMenuItemDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.toolStripMenuItemDelete.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemDelete.Text = "Delete";
			this.toolStripMenuItemDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
			// 
			// toolStripMenuItemRename
			// 
			this.toolStripMenuItemRename.Name = "toolStripMenuItemRename";
			this.toolStripMenuItemRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.toolStripMenuItemRename.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemRename.Text = "Rename";
			this.toolStripMenuItemRename.Click += new System.EventHandler(this.toolStripMenuItemRename_Click);
			// 
			// insertToolStripMenuItem
			// 
			this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ifStatementToolStripMenuItem,
            this.showStatementToolStripMenuItem,
            this.sendStatementToolStripMenuItem,
            this.appendStatementToolStripMenuItem,
            this.getStatementToolStripMenuItem,
            this.forEachStatementToolStripMenuItem,
            this.deleteStatementToolStripMenuItem,
            this.setStatementToolStripMenuItem,
            this.commentStatementToolStripMenuItem});
			this.insertToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.insertToolStripMenuItem.MergeIndex = 4;
			this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
			this.insertToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.insertToolStripMenuItem.Text = "&Insert";
			// 
			// ifStatementToolStripMenuItem
			// 
			this.ifStatementToolStripMenuItem.Enabled = false;
			this.ifStatementToolStripMenuItem.Name = "ifStatementToolStripMenuItem";
			this.ifStatementToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.ifStatementToolStripMenuItem.Text = "&If Statement";
			// 
			// showStatementToolStripMenuItem
			// 
			this.showStatementToolStripMenuItem.Enabled = false;
			this.showStatementToolStripMenuItem.Name = "showStatementToolStripMenuItem";
			this.showStatementToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.showStatementToolStripMenuItem.Text = "&Show Statement";
			// 
			// sendStatementToolStripMenuItem
			// 
			this.sendStatementToolStripMenuItem.Enabled = false;
			this.sendStatementToolStripMenuItem.Name = "sendStatementToolStripMenuItem";
			this.sendStatementToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.sendStatementToolStripMenuItem.Text = "S&end Statement";
			// 
			// appendStatementToolStripMenuItem
			// 
			this.appendStatementToolStripMenuItem.Enabled = false;
			this.appendStatementToolStripMenuItem.Name = "appendStatementToolStripMenuItem";
			this.appendStatementToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.appendStatementToolStripMenuItem.Text = "&Append Statement";
			// 
			// getStatementToolStripMenuItem
			// 
			this.getStatementToolStripMenuItem.Enabled = false;
			this.getStatementToolStripMenuItem.Name = "getStatementToolStripMenuItem";
			this.getStatementToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.getStatementToolStripMenuItem.Text = "&Get Statement";
			// 
			// forEachStatementToolStripMenuItem
			// 
			this.forEachStatementToolStripMenuItem.Enabled = false;
			this.forEachStatementToolStripMenuItem.Name = "forEachStatementToolStripMenuItem";
			this.forEachStatementToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.forEachStatementToolStripMenuItem.Text = "&ForEach Statement";
			// 
			// deleteStatementToolStripMenuItem
			// 
			this.deleteStatementToolStripMenuItem.Enabled = false;
			this.deleteStatementToolStripMenuItem.Name = "deleteStatementToolStripMenuItem";
			this.deleteStatementToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.deleteStatementToolStripMenuItem.Text = "&Delete Statement";
			// 
			// setStatementToolStripMenuItem
			// 
			this.setStatementToolStripMenuItem.Enabled = false;
			this.setStatementToolStripMenuItem.Name = "setStatementToolStripMenuItem";
			this.setStatementToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.setStatementToolStripMenuItem.Text = "Se&t Statement";
			// 
			// commentStatementToolStripMenuItem
			// 
			this.commentStatementToolStripMenuItem.Enabled = false;
			this.commentStatementToolStripMenuItem.Name = "commentStatementToolStripMenuItem";
			this.commentStatementToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.commentStatementToolStripMenuItem.Text = "&Comment Statement";
			// 
			// toolStripProcess
			// 
			this.toolStripProcess.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonCut,
            this.toolStripButtonCopy,
            this.toolStripButtonPaste,
            this.toolStripButtonDelete,
            this.toolStripSeparator1,
            this.toolStripButtonUndo,
            this.toolStripButtonRedo});
			this.toolStripProcess.Location = new System.Drawing.Point(0, 24);
			this.toolStripProcess.Name = "toolStripProcess";
			this.toolStripProcess.Size = new System.Drawing.Size(562, 25);
			this.toolStripProcess.TabIndex = 2;
			this.toolStripProcess.Text = "toolStrip1";
			// 
			// toolStripButtonCut
			// 
			this.toolStripButtonCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonCut.Image = global::Tawala.ProcessDesigner.Properties.Resources.Edit_Cut;
			this.toolStripButtonCut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonCut.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripButtonCut.Name = "toolStripButtonCut";
			this.toolStripButtonCut.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonCut.Text = "Cut";
			this.toolStripButtonCut.Click += new System.EventHandler(this.toolStripButtonCut_Click);
			// 
			// toolStripButtonCopy
			// 
			this.toolStripButtonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonCopy.Image = global::Tawala.ProcessDesigner.Properties.Resources.Edit_Copy;
			this.toolStripButtonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonCopy.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripButtonCopy.Name = "toolStripButtonCopy";
			this.toolStripButtonCopy.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonCopy.Text = "Copy";
			this.toolStripButtonCopy.Click += new System.EventHandler(this.toolStripButtonCopy_Click);
			// 
			// toolStripButtonPaste
			// 
			this.toolStripButtonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonPaste.Image = global::Tawala.ProcessDesigner.Properties.Resources.Edit_Paste;
			this.toolStripButtonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonPaste.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripButtonPaste.Name = "toolStripButtonPaste";
			this.toolStripButtonPaste.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonPaste.Text = "Paste";
			this.toolStripButtonPaste.Click += new System.EventHandler(this.toolStripButtonPaste_Click);
			// 
			// toolStripButtonDelete
			// 
			this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonDelete.Image = global::Tawala.ProcessDesigner.Properties.Resources.Edit_Delete;
			this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDelete.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripButtonDelete.Name = "toolStripButtonDelete";
			this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonDelete.Text = "Delete";
			this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
			// 
			// toolStripButtonUndo
			// 
			this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonUndo.Image = global::Tawala.ProcessDesigner.Properties.Resources.Edit_Undo;
			this.toolStripButtonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonUndo.Name = "toolStripButtonUndo";
			this.toolStripButtonUndo.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonUndo.Text = "Undo";
			this.toolStripButtonUndo.Click += new System.EventHandler(this.toolStripButtonUndo_Click);
			// 
			// toolStripButtonRedo
			// 
			this.toolStripButtonRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonRedo.Image = global::Tawala.ProcessDesigner.Properties.Resources.Edit_Redo;
			this.toolStripButtonRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonRedo.Name = "toolStripButtonRedo";
			this.toolStripButtonRedo.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonRedo.Text = "Redo";
			this.toolStripButtonRedo.Click += new System.EventHandler(this.toolStripButtonRedo_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripMenuItemUndo
			// 
			this.toolStripMenuItemUndo.Name = "toolStripMenuItemUndo";
			this.toolStripMenuItemUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.toolStripMenuItemUndo.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemUndo.Text = "Undo";
			this.toolStripMenuItemUndo.Click += new System.EventHandler(this.toolStripButtonUndo_Click);
			// 
			// toolStripMenuItemRedo
			// 
			this.toolStripMenuItemRedo.Name = "toolStripMenuItemRedo";
			this.toolStripMenuItemRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.toolStripMenuItemRedo.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemRedo.Text = "Redo";
			this.toolStripMenuItemRedo.Click += new System.EventHandler(this.toolStripButtonRedo_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(161, 6);
			// 
			// ProcessView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(562, 366);
			this.Controls.Add(this.toolStripProcess);
			this.Controls.Add(this.processEditor);
			this.Controls.Add(this.menuStripProcess);
			this.Icon = global::Tawala.ProcessDesigner.Properties.Resources.Process;
			this.MainMenuStrip = this.menuStripProcess;
			this.Name = "ProcessView";
			this.Text = "ProcessView";
			this.Deactivate += new System.EventHandler(this.processView_Deactivate);
			this.Load += new System.EventHandler(this.ProcessView_Load);
			this.Activated += new System.EventHandler(this.processView_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.processView_FormClosing);
			this.menuStripProcess.ResumeLayout(false);
			this.menuStripProcess.PerformLayout();
			this.toolStripProcess.ResumeLayout(false);
			this.toolStripProcess.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Tawala.Processes.ProcessEditor processEditor;
		private System.Windows.Forms.MenuStrip menuStripProcess;
		private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ifStatementToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showStatementToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sendStatementToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem appendStatementToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem getStatementToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem forEachStatementToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteStatementToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setStatementToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem commentStatementToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRename;
		private System.Windows.Forms.ToolStrip toolStripProcess;
		private System.Windows.Forms.ToolStripButton toolStripButtonCut;
		private System.Windows.Forms.ToolStripButton toolStripButtonCopy;
		private System.Windows.Forms.ToolStripButton toolStripButtonPaste;
		private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCut;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopy;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPaste;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelete;
		private System.Windows.Forms.ToolStripButton toolStripButtonUndo;
		private System.Windows.Forms.ToolStripButton toolStripButtonRedo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUndo;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRedo;
	}
}