// $Workfile: MDIForm.Designer.cs $
// $Revision: 13 $	$Date: 8/08/06 5:24p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
namespace Tawala.Processes
{
	partial class MDIForm
	{
		private ProcessEditor editor;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statementsPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editor = new Tawala.Processes.ProcessEditor();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.insertToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(562, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.TabStop = true;
			this.menuStrip.Text = "menuStrip";
			this.menuStrip.Visible = false;
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statementsPaletteToolStripMenuItem});
			this.viewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// statementsPaletteToolStripMenuItem
			// 
			this.statementsPaletteToolStripMenuItem.Name = "statementsPaletteToolStripMenuItem";
			this.statementsPaletteToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.statementsPaletteToolStripMenuItem.Text = "Statements Palette";
			this.statementsPaletteToolStripMenuItem.Click += new System.EventHandler(this.statementsPaletteToolStripMenuItem_Click);
			// 
			// insertToolStripMenuItem
			// 
			this.insertToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
			this.insertToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.insertToolStripMenuItem.Text = "&Insert";
			// 
			// editor
			// 
			this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editor.Location = new System.Drawing.Point(0, 24);
			this.editor.Margin = new System.Windows.Forms.Padding(0);
			this.editor.Name = "editor";
			this.editor.Process = null;
			this.editor.Size = new System.Drawing.Size(562, 342);
			this.editor.TabIndex = 1;
			// 
			// MDIForm
			// 
			this.ClientSize = new System.Drawing.Size(562, 366);
			this.Controls.Add(this.editor);
			this.Controls.Add(this.menuStrip);
			this.DoubleBuffered = true;
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MDIForm";
			this.ShowInTaskbar = false;
			this.Text = "Process - ";
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem statementsPaletteToolStripMenuItem;
	}
}
