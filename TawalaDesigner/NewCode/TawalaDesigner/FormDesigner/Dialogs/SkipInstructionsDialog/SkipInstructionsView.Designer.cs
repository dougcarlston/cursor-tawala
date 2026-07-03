namespace Tawala.FormDesigner.Dialogs.SkipInstructionsDialog
{
	partial class SkipInstructionsView
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
			this.buttonClose = new System.Windows.Forms.Button();
			this.menuStripSkipInstructions = new System.Windows.Forms.MenuStrip();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCut = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItemUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemRedo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSkipInstructions = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonCut = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonCopy = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonPaste = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonRedo = new System.Windows.Forms.ToolStripButton();
			this.skipInstructionsStatementSelector = new Tawala.FormDesigner.Dialogs.SkipInstructionsDialog.SkipInstructionsStatementSelector();
			this.menuStripSkipInstructions.SuspendLayout();
			this.toolStripSkipInstructions.SuspendLayout();
			this.SuspendLayout();
			// 
			// processEditor
			// 
			this.processEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.processEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.processEditor.Location = new System.Drawing.Point(119, 29);
			this.processEditor.Margin = new System.Windows.Forms.Padding(0);
			this.processEditor.Name = "processEditor";
			this.processEditor.Process = null;
			this.processEditor.Size = new System.Drawing.Size(448, 319);
			this.processEditor.TabIndex = 0;
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonClose.Location = new System.Drawing.Point(251, 362);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 2;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// menuStripSkipInstructions
			// 
			this.menuStripSkipInstructions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem});
			this.menuStripSkipInstructions.Location = new System.Drawing.Point(0, 0);
			this.menuStripSkipInstructions.Name = "menuStripSkipInstructions";
			this.menuStripSkipInstructions.Size = new System.Drawing.Size(703, 24);
			this.menuStripSkipInstructions.TabIndex = 3;
			this.menuStripSkipInstructions.Text = "menuStrip1";
			this.menuStripSkipInstructions.Visible = false;
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCut,
            this.toolStripMenuItemCopy,
            this.toolStripMenuItemPaste,
            this.toolStripMenuItemDelete,
            this.toolStripSeparator1,
            this.toolStripMenuItemUndo,
            this.toolStripMenuItemRedo});
			this.editToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// toolStripMenuItemCut
			// 
			this.toolStripMenuItemCut.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Cut;
			this.toolStripMenuItemCut.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripMenuItemCut.Name = "toolStripMenuItemCut";
			this.toolStripMenuItemCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.toolStripMenuItemCut.Size = new System.Drawing.Size(174, 22);
			this.toolStripMenuItemCut.Text = "Cut";
			this.toolStripMenuItemCut.Click += new System.EventHandler(this.toolStripMenuItemCut_Click);
			// 
			// toolStripMenuItemCopy
			// 
			this.toolStripMenuItemCopy.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Copy;
			this.toolStripMenuItemCopy.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripMenuItemCopy.Name = "toolStripMenuItemCopy";
			this.toolStripMenuItemCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.toolStripMenuItemCopy.Size = new System.Drawing.Size(174, 22);
			this.toolStripMenuItemCopy.Text = "Copy";
			this.toolStripMenuItemCopy.Click += new System.EventHandler(this.toolStripMenuItemCopy_Click);
			// 
			// toolStripMenuItemPaste
			// 
			this.toolStripMenuItemPaste.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Paste;
			this.toolStripMenuItemPaste.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripMenuItemPaste.Name = "toolStripMenuItemPaste";
			this.toolStripMenuItemPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.toolStripMenuItemPaste.Size = new System.Drawing.Size(174, 22);
			this.toolStripMenuItemPaste.Text = "Paste";
			this.toolStripMenuItemPaste.Click += new System.EventHandler(this.toolStripMenuItemPaste_Click);
			// 
			// toolStripMenuItemDelete
			// 
			this.toolStripMenuItemDelete.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Delete;
			this.toolStripMenuItemDelete.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
			this.toolStripMenuItemDelete.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Delete)));
			this.toolStripMenuItemDelete.Size = new System.Drawing.Size(174, 22);
			this.toolStripMenuItemDelete.Text = "Delete";
			this.toolStripMenuItemDelete.Click += new System.EventHandler(this.toolStripMenuItemDelete_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
			// 
			// toolStripMenuItemUndo
			// 
			this.toolStripMenuItemUndo.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Undo;
			this.toolStripMenuItemUndo.Name = "toolStripMenuItemUndo";
			this.toolStripMenuItemUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.toolStripMenuItemUndo.Size = new System.Drawing.Size(174, 22);
			this.toolStripMenuItemUndo.Text = "Undo";
			this.toolStripMenuItemUndo.Click += new System.EventHandler(this.toolStripMenuItemUndo_Click);
			// 
			// toolStripMenuItemRedo
			// 
			this.toolStripMenuItemRedo.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Redo;
			this.toolStripMenuItemRedo.Name = "toolStripMenuItemRedo";
			this.toolStripMenuItemRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.toolStripMenuItemRedo.Size = new System.Drawing.Size(174, 22);
			this.toolStripMenuItemRedo.Text = "Redo";
			this.toolStripMenuItemRedo.Click += new System.EventHandler(this.toolStripMenuItemRedo_Click);
			// 
			// toolStripSkipInstructions
			// 
			this.toolStripSkipInstructions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonCut,
            this.toolStripButtonCopy,
            this.toolStripButtonPaste,
            this.toolStripButtonDelete,
            this.toolStripSeparator2,
            this.toolStripButtonUndo,
            this.toolStripButtonRedo});
			this.toolStripSkipInstructions.Location = new System.Drawing.Point(0, 0);
			this.toolStripSkipInstructions.Name = "toolStripSkipInstructions";
			this.toolStripSkipInstructions.Size = new System.Drawing.Size(576, 25);
			this.toolStripSkipInstructions.TabIndex = 4;
			this.toolStripSkipInstructions.Text = "toolStrip1";
			// 
			// toolStripButtonCut
			// 
			this.toolStripButtonCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonCut.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Cut;
			this.toolStripButtonCut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonCut.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripButtonCut.Name = "toolStripButtonCut";
			this.toolStripButtonCut.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonCut.Text = "Cut";
			this.toolStripButtonCut.Click += new System.EventHandler(this.toolStripMenuItemCut_Click);
			// 
			// toolStripButtonCopy
			// 
			this.toolStripButtonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonCopy.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Copy;
			this.toolStripButtonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonCopy.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripButtonCopy.Name = "toolStripButtonCopy";
			this.toolStripButtonCopy.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonCopy.Text = "Copy";
			this.toolStripButtonCopy.Click += new System.EventHandler(this.toolStripMenuItemCopy_Click);
			// 
			// toolStripButtonPaste
			// 
			this.toolStripButtonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonPaste.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Paste;
			this.toolStripButtonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonPaste.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripButtonPaste.Name = "toolStripButtonPaste";
			this.toolStripButtonPaste.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonPaste.Text = "Paste";
			this.toolStripButtonPaste.Click += new System.EventHandler(this.toolStripMenuItemPaste_Click);
			// 
			// toolStripButtonDelete
			// 
			this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonDelete.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Delete;
			this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDelete.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripButtonDelete.Name = "toolStripButtonDelete";
			this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonDelete.Text = "Delete";
			this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripMenuItemDelete_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonUndo
			// 
			this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonUndo.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Undo;
			this.toolStripButtonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonUndo.Name = "toolStripButtonUndo";
			this.toolStripButtonUndo.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonUndo.Text = "Undo";
			this.toolStripButtonUndo.Click += new System.EventHandler(this.toolStripMenuItemUndo_Click);
			// 
			// toolStripButtonRedo
			// 
			this.toolStripButtonRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonRedo.Image = global::Tawala.FormDesigner.Properties.Resources.Edit_Redo;
			this.toolStripButtonRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonRedo.Name = "toolStripButtonRedo";
			this.toolStripButtonRedo.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonRedo.Text = "Redo";
			this.toolStripButtonRedo.Click += new System.EventHandler(this.toolStripMenuItemRedo_Click);
			// 
			// skipInstructionsStatementSelector
			// 
			this.skipInstructionsStatementSelector.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.skipInstructionsStatementSelector.Location = new System.Drawing.Point(13, 29);
			this.skipInstructionsStatementSelector.Name = "skipInstructionsStatementSelector";
			this.skipInstructionsStatementSelector.ProcessEditor = null;
			this.skipInstructionsStatementSelector.Size = new System.Drawing.Size(103, 361);
			this.skipInstructionsStatementSelector.TabIndex = 1;
			// 
			// SkipInstructionsView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(576, 396);
			this.Controls.Add(this.toolStripSkipInstructions);
			this.Controls.Add(this.skipInstructionsStatementSelector);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.processEditor);
			this.Controls.Add(this.menuStripSkipInstructions);
			this.MainMenuStrip = this.menuStripSkipInstructions;
			this.MinimizeBox = false;
			this.Name = "SkipInstructionsView";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Edit Skip Instructions";
			this.TopMost = true;
			this.Deactivate += new System.EventHandler(this.SkipInstructionsView_Deactivate);
			this.Load += new System.EventHandler(this.SkipInstructionsView_Load);
			this.Activated += new System.EventHandler(this.SkipInstructionsView_Activated);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SkipInstructionsView_FormClosed);
			this.menuStripSkipInstructions.ResumeLayout(false);
			this.menuStripSkipInstructions.PerformLayout();
			this.toolStripSkipInstructions.ResumeLayout(false);
			this.toolStripSkipInstructions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Tawala.Processes.ProcessEditor processEditor;
		private SkipInstructionsStatementSelector skipInstructionsStatementSelector;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.MenuStrip menuStripSkipInstructions;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCut;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopy;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPaste;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelete;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUndo;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRedo;
		private System.Windows.Forms.ToolStrip toolStripSkipInstructions;
		private System.Windows.Forms.ToolStripButton toolStripButtonCut;
		private System.Windows.Forms.ToolStripButton toolStripButtonCopy;
		private System.Windows.Forms.ToolStripButton toolStripButtonPaste;
		private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton toolStripButtonUndo;
		private System.Windows.Forms.ToolStripButton toolStripButtonRedo;
	}
}