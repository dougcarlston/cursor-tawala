// $Workfile: FieldsPalette.Designer.cs $
// $Revision: 3 $	$Date: 6/15/06 1:44p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.ProjectUI
{
	partial class FieldsPalette
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.fieldsContextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItemFieldsInsert = new System.Windows.Forms.MenuItem();
			this.menuItemFieldsDelete = new System.Windows.Forms.MenuItem();
			this.menuItemFieldsNew = new System.Windows.Forms.MenuItem();
			this.menuItemFieldsRename = new System.Windows.Forms.MenuItem();
			this.fieldsPanel = new Tawala.ProjectUI.FieldsPanel();
			this.fieldsTreeView = new System.Windows.Forms.TreeView();
			this.fieldsLabel = new Tawala.Common.GradientLabel();
			this.fieldsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// fieldsContextMenu
			// 
			this.fieldsContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFieldsInsert,
            this.menuItemFieldsDelete,
            this.menuItemFieldsNew,
            this.menuItemFieldsRename});
			// 
			// menuItemFieldsInsert
			// 
			this.menuItemFieldsInsert.Index = 0;
			this.menuItemFieldsInsert.Text = "Insert";
			// 
			// menuItemFieldsDelete
			// 
			this.menuItemFieldsDelete.Index = 1;
			this.menuItemFieldsDelete.Shortcut = System.Windows.Forms.Shortcut.Del;
			this.menuItemFieldsDelete.Text = "Delete";
			// 
			// menuItemFieldsNew
			// 
			this.menuItemFieldsNew.Index = 2;
			this.menuItemFieldsNew.Text = "New";
			// 
			// menuItemFieldsRename
			// 
			this.menuItemFieldsRename.Index = 3;
			this.menuItemFieldsRename.Shortcut = System.Windows.Forms.Shortcut.F2;
			this.menuItemFieldsRename.Text = "Rename";
			// 
			// fieldsPanel
			// 
			this.fieldsPanel.Controls.Add(this.fieldsTreeView);
			this.fieldsPanel.Controls.Add(this.fieldsLabel);
			this.fieldsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fieldsPanel.Location = new System.Drawing.Point(0, 0);
			this.fieldsPanel.Name = "fieldsPanel";
			this.fieldsPanel.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.fieldsPanel.Size = new System.Drawing.Size(120, 796);
			this.fieldsPanel.TabIndex = 1;
			// 
			// fieldsTreeView
			// 
			this.fieldsTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(252)))), ((int)(((byte)(248)))));
			this.fieldsTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.fieldsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fieldsTreeView.FullRowSelect = true;
			this.fieldsTreeView.HideSelection = false;
			this.fieldsTreeView.Location = new System.Drawing.Point(5, 20);
			this.fieldsTreeView.Name = "fieldsTreeView";
			this.fieldsTreeView.ShowNodeToolTips = true;
			this.fieldsTreeView.Size = new System.Drawing.Size(115, 776);
			this.fieldsTreeView.TabIndex = 3;
			this.fieldsTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.fieldsTreeView_NodeMouseDoubleClick);
			this.fieldsTreeView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.fieldsTreeView_KeyPress);
			this.fieldsTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.fieldsTreeView_NodeMouseClick);
			this.fieldsTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.fieldsTreeView_ItemDrag);
			// 
			// fieldsLabel
			// 
			this.fieldsLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(206)))), ((int)(((byte)(248)))));
			this.fieldsLabel.ContextMenu = this.fieldsContextMenu;
			this.fieldsLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.fieldsLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.fieldsLabel.ForeColor = System.Drawing.Color.White;
			this.fieldsLabel.Location = new System.Drawing.Point(5, 0);
			this.fieldsLabel.Margin = new System.Windows.Forms.Padding(0);
			this.fieldsLabel.Name = "fieldsLabel";
			this.fieldsLabel.Size = new System.Drawing.Size(115, 20);
			this.fieldsLabel.TabIndex = 2;
			this.fieldsLabel.Text = "Fields";
			this.fieldsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// FieldsPalette
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.fieldsPanel);
			this.DoubleBuffered = true;
			this.Name = "FieldsPalette";
			this.Size = new System.Drawing.Size(120, 796);
			this.fieldsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private FieldsPanel fieldsPanel;
		private System.Windows.Forms.TreeView fieldsTreeView;
		private Tawala.Common.GradientLabel fieldsLabel;
		private System.Windows.Forms.ContextMenu fieldsContextMenu;
		private System.Windows.Forms.MenuItem menuItemFieldsInsert;
		private System.Windows.Forms.MenuItem menuItemFieldsDelete;
		private System.Windows.Forms.MenuItem menuItemFieldsNew;
		private System.Windows.Forms.MenuItem menuItemFieldsRename;
	}
}
