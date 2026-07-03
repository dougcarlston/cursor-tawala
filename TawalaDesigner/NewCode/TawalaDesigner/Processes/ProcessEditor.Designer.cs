// $Workfile: ProcessEditor.Designer.cs $
// $Revision: 17 $	$Date: 12/06/07 12:24p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
namespace Tawala.Processes
{
	partial class ProcessEditor
	{
		private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel detailsContainerPanel;
        private System.Windows.Forms.Panel editPanel;

		private System.Windows.Forms.Label detailsInstructionPanel;
		private System.Windows.Forms.ContextMenuStrip menuStripMergeView;
		private System.Windows.Forms.ToolStripMenuItem menuItemViewStatements;
		private System.ComponentModel.IContainer components;

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

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.detailsContainerPanel = new System.Windows.Forms.Panel();
			this.detailsInstructionPanel = new System.Windows.Forms.Label();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.editPanel = new System.Windows.Forms.Panel();
			this.listBoxStatements = new Tawala.Processes.StatementsListBox();
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemCut = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStripMergeView = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemViewStatements = new System.Windows.Forms.ToolStripMenuItem();
			this.scrollTimer = new System.Windows.Forms.Timer(this.components);
			this.detailsContainerPanel.SuspendLayout();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.editPanel.SuspendLayout();
			this.contextMenu.SuspendLayout();
			this.menuStripMergeView.SuspendLayout();
			this.SuspendLayout();
			// 
			// detailsPanel
			// 
			this.detailsContainerPanel.AutoSize = true;
			this.detailsContainerPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.detailsContainerPanel.Controls.Add(this.detailsInstructionPanel);
			this.detailsContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.detailsContainerPanel.Location = new System.Drawing.Point(0, 0);
			this.detailsContainerPanel.Name = "detailsPanel";
			this.detailsContainerPanel.Padding = new System.Windows.Forms.Padding(3, 2, 2, 0);
			this.detailsContainerPanel.Size = new System.Drawing.Size(576, 75);
			this.detailsContainerPanel.TabIndex = 0;
			this.detailsContainerPanel.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.detailsPanel_ControlAdded);
			this.detailsContainerPanel.Resize += new System.EventHandler(this.detailsPanel_Resize);
			this.detailsContainerPanel.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.detailsPanel_ControlRemoved);
			// 
			// detailsPanelInfo
			// 
			this.detailsInstructionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.detailsInstructionPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.detailsInstructionPanel.ForeColor = System.Drawing.Color.Blue;
			this.detailsInstructionPanel.Location = new System.Drawing.Point(3, 2);
			this.detailsInstructionPanel.Name = "detailsPanelInfo";
			this.detailsInstructionPanel.Padding = new System.Windows.Forms.Padding(8);
			this.detailsInstructionPanel.Size = new System.Drawing.Size(571, 73);
			this.detailsInstructionPanel.TabIndex = 0;
			this.detailsInstructionPanel.Text = "To create a new statement, click one of the buttons in the Statements palette.";
			this.detailsInstructionPanel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.detailsContainerPanel);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.editPanel);
			this.splitContainer.Size = new System.Drawing.Size(576, 415);
			this.splitContainer.SplitterDistance = 75;
			this.splitContainer.TabIndex = 2;
			// 
			// editPanel
			// 
			this.editPanel.Controls.Add(this.listBoxStatements);
			this.editPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editPanel.Location = new System.Drawing.Point(0, 0);
			this.editPanel.Name = "editPanel";
			this.editPanel.Padding = new System.Windows.Forms.Padding(20, 1, 0, 0);
			this.editPanel.Size = new System.Drawing.Size(576, 336);
			this.editPanel.TabIndex = 0;
			this.editPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.editPanel_Paint);
			this.editPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.editPanel_MouseMove);
			this.editPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.editPanel_MouseDown);
			this.editPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.editPanel_MouseUp);
			// 
			// listBoxStatements
			// 
			this.listBoxStatements.AllowDrop = true;
			this.listBoxStatements.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listBoxStatements.ContextMenuStrip = this.contextMenu;
			this.listBoxStatements.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxStatements.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listBoxStatements.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBoxStatements.FormattingEnabled = true;
			this.listBoxStatements.HorizontalScrollbar = true;
			this.listBoxStatements.InsertionIndex = 0;
			this.listBoxStatements.IntegralHeight = false;
			this.listBoxStatements.ItemHeight = 16;
			this.listBoxStatements.Location = new System.Drawing.Point(20, 1);
			this.listBoxStatements.Margin = new System.Windows.Forms.Padding(5, 3, 0, 3);
			this.listBoxStatements.Name = "listBoxStatements";
			this.listBoxStatements.SelectedLinesEndIndex = -1;
			this.listBoxStatements.SelectedLinesStartIndex = -1;
			this.listBoxStatements.SelectionDisabled = true;
			this.listBoxStatements.Size = new System.Drawing.Size(556, 335);
			this.listBoxStatements.TabIndex = 1;
			this.listBoxStatements.DragOver += new System.Windows.Forms.DragEventHandler(this.listBox_DragOver);
			this.listBoxStatements.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
			this.listBoxStatements.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_DragDrop);
			this.listBoxStatements.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseMove);
			this.listBoxStatements.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDown);
			this.listBoxStatements.DragLeave += new System.EventHandler(this.listBox_DragLeave);
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemCut,
            this.menuItemCopy,
            this.menuItemPaste,
            this.menuItemDelete});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(165, 92);
			this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
			// 
			// menuItemCut
			// 
			this.menuItemCut.Image = global::Tawala.Processes.Properties.Resources.Cut;
			this.menuItemCut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuItemCut.Name = "menuItemCut";
			this.menuItemCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.menuItemCut.Size = new System.Drawing.Size(164, 22);
			this.menuItemCut.Text = "Cut";
			this.menuItemCut.Click += new System.EventHandler(this.menuItemCut_Click);
			// 
			// menuItemCopy
			// 
			this.menuItemCopy.Image = global::Tawala.Processes.Properties.Resources.Copy;
			this.menuItemCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuItemCopy.Name = "menuItemCopy";
			this.menuItemCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.menuItemCopy.Size = new System.Drawing.Size(164, 22);
			this.menuItemCopy.Text = "Copy";
			this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
			// 
			// menuItemPaste
			// 
			this.menuItemPaste.Image = global::Tawala.Processes.Properties.Resources.Paste;
			this.menuItemPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuItemPaste.Name = "menuItemPaste";
			this.menuItemPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.menuItemPaste.Size = new System.Drawing.Size(164, 22);
			this.menuItemPaste.Text = "Paste";
			this.menuItemPaste.Click += new System.EventHandler(this.menuItemPaste_Click);
			// 
			// menuItemDelete
			// 
			this.menuItemDelete.Image = global::Tawala.Processes.Properties.Resources.Delete;
			this.menuItemDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuItemDelete.Name = "menuItemDelete";
			this.menuItemDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.menuItemDelete.Size = new System.Drawing.Size(164, 22);
			this.menuItemDelete.Text = "Delete";
			this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
			// 
			// menuStripMergeView
			// 
			this.menuStripMergeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemViewStatements});
			this.menuStripMergeView.Name = "menuStripMergeView";
			this.menuStripMergeView.Size = new System.Drawing.Size(155, 26);
			// 
			// menuItemViewStatements
			// 
			this.menuItemViewStatements.Name = "menuItemViewStatements";
			this.menuItemViewStatements.Size = new System.Drawing.Size(154, 22);
			this.menuItemViewStatements.Text = "&Statements";
			// 
			// scrollTimer
			// 
			this.scrollTimer.Tick += new System.EventHandler(this.scrollTimer_Tick);
			// 
			// ProcessEditor
			// 
			this.Controls.Add(this.splitContainer);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "ProcessEditor";
			this.Size = new System.Drawing.Size(576, 415);
			this.detailsContainerPanel.ResumeLayout(false);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel1.PerformLayout();
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.editPanel.ResumeLayout(false);
			this.contextMenu.ResumeLayout(false);
			this.menuStripMergeView.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItemCut;
		private System.Windows.Forms.ToolStripMenuItem menuItemCopy;
		private System.Windows.Forms.ToolStripMenuItem menuItemPaste;
		private System.Windows.Forms.ToolStripMenuItem menuItemDelete;
		private StatementsListBox listBoxStatements;
		private System.Windows.Forms.Timer scrollTimer;
	}
}
