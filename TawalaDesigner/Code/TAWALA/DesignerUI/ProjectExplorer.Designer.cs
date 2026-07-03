// $Workfile: ProjectExplorer.Designer.cs $
// $Revision: 24 $	$Date: 11/05/07 1:43p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.DesignerUI
{
	partial class ProjectExplorer
	{
		private ProjectTree tree;
		private System.Windows.Forms.ContextMenuStrip folderContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem folderMenuItemAddNew;
		private System.Windows.Forms.ToolStripMenuItem itemMenuItemDelete;
		private System.Windows.Forms.ToolStripMenuItem itemMenuItemRename;
		private System.Windows.Forms.ToolStripMenuItem itemMenuItemCut;
        private System.Windows.Forms.ToolStripMenuItem itemMenuItemCopy;
		private System.Windows.Forms.ToolStripMenuItem itemMenuItemConnect;
		private System.Windows.Forms.ToolStripMenuItem itemMenuItemDisconnect;
		private Tawala.Common.GradientLabel label;
		private System.Windows.Forms.ToolStripMenuItem folderMenuItemPaste;
		private System.Windows.Forms.ToolStripMenuItem itemMenuItemPaste;
		private System.Windows.Forms.ContextMenuStrip componentContextMenuStrip;

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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator toolStripSep1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectExplorer));
            this.folderContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.folderMenuItemAddNew = new System.Windows.Forms.ToolStripMenuItem();
            this.folderMenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.folderMenuChildNodesSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.folderMenuItemCollapseChildNodes = new System.Windows.Forms.ToolStripMenuItem();
            this.folderMenuItemExpandChildNodes = new System.Windows.Forms.ToolStripMenuItem();
            this.componentContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewItemtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.itemMenuItemCut = new System.Windows.Forms.ToolStripMenuItem();
            this.itemMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.itemMenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.itemMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.itemMenuItemRename = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.connectPreProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectPreProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemMenuItemConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.itemMenuItemDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.blockBackButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startingPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataEntryOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataSourceNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxDataSourceName = new System.Windows.Forms.ToolStripTextBox();
            this.label = new Tawala.Common.GradientLabel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.newFormToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.newProjectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.newDocumentToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nodeMoveUpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.nodeMoveDownToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.startPointToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.blockBackButtonToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tree = new Tawala.DesignerUI.ProjectTree();
            toolStripSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.folderContextMenuStrip.SuspendLayout();
            this.componentContextMenuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSep1
            // 
            toolStripSep1.Name = "toolStripSep1";
            toolStripSep1.Size = new System.Drawing.Size(6, 25);
            // 
            // folderContextMenuStrip
            // 
            this.folderContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.folderMenuItemAddNew,
            this.folderMenuItemPaste,
            this.folderMenuChildNodesSeparator,
            this.folderMenuItemCollapseChildNodes,
            this.folderMenuItemExpandChildNodes});
            this.folderContextMenuStrip.Name = "folderContextMenu";
            this.folderContextMenuStrip.Size = new System.Drawing.Size(188, 98);
            this.folderContextMenuStrip.Opened += new System.EventHandler(this.folderContextMenu_Popup);
            // 
            // folderMenuItemAddNew
            // 
            this.folderMenuItemAddNew.Name = "folderMenuItemAddNew";
            this.folderMenuItemAddNew.Size = new System.Drawing.Size(187, 22);
            this.folderMenuItemAddNew.Text = "Add New";
            this.folderMenuItemAddNew.Click += new System.EventHandler(this.folderMenuItemAddNew_Click);
            // 
            // folderMenuItemPaste
            // 
            this.folderMenuItemPaste.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Paste;
            this.folderMenuItemPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.folderMenuItemPaste.Name = "folderMenuItemPaste";
            this.folderMenuItemPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.folderMenuItemPaste.Size = new System.Drawing.Size(187, 22);
            this.folderMenuItemPaste.Text = "Paste";
            this.folderMenuItemPaste.Click += new System.EventHandler(this.folderMenuItemPaste_Click);
            // 
            // folderMenuChildNodesSeparator
            // 
            this.folderMenuChildNodesSeparator.Name = "folderMenuChildNodesSeparator";
            this.folderMenuChildNodesSeparator.Size = new System.Drawing.Size(184, 6);
            // 
            // folderMenuItemCollapseChildNodes
            // 
            this.folderMenuItemCollapseChildNodes.Name = "folderMenuItemCollapseChildNodes";
            this.folderMenuItemCollapseChildNodes.Size = new System.Drawing.Size(187, 22);
            this.folderMenuItemCollapseChildNodes.Text = "Collapse Child Nodes";
            this.folderMenuItemCollapseChildNodes.Click += new System.EventHandler(this.folderMenuItemCollapseChildNodes_Click);
            // 
            // folderMenuItemExpandChildNodes
            // 
            this.folderMenuItemExpandChildNodes.Name = "folderMenuItemExpandChildNodes";
            this.folderMenuItemExpandChildNodes.Size = new System.Drawing.Size(187, 22);
            this.folderMenuItemExpandChildNodes.Text = "Expand Child Nodes";
            this.folderMenuItemExpandChildNodes.Click += new System.EventHandler(this.folderMenuItemExpandChildNodes_Click);
            // 
            // componentContextMenuStrip
            // 
            this.componentContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewItemtoolStripMenuItem,
            this.toolStripSeparator1,
            this.itemMenuItemCut,
            this.itemMenuItemCopy,
            this.itemMenuItemPaste,
            this.itemMenuItemDelete,
            this.itemMenuItemRename,
            this.toolStripSeparator2,
            this.connectPreProcessToolStripMenuItem,
            this.disconnectPreProcessToolStripMenuItem,
            this.itemMenuItemConnect,
            this.itemMenuItemDisconnect,
            this.toolStripSeparator4,
            this.blockBackButtonToolStripMenuItem,
            this.startingPointToolStripMenuItem,
            this.dataEntryOnlyToolStripMenuItem,
            this.dataSourceNameToolStripMenuItem});
            this.componentContextMenuStrip.Name = "itemContextMenu";
            this.componentContextMenuStrip.Size = new System.Drawing.Size(226, 352);
            this.componentContextMenuStrip.Opened += new System.EventHandler(this.componentContextMenuStrip_Opened);
            // 
            // viewItemtoolStripMenuItem
            // 
            this.viewItemtoolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.View;
            this.viewItemtoolStripMenuItem.Name = "viewItemtoolStripMenuItem";
            this.viewItemtoolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.viewItemtoolStripMenuItem.Text = "View";
            this.viewItemtoolStripMenuItem.Click += new System.EventHandler(this.viewItemtoolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(222, 6);
            // 
            // itemMenuItemCut
            // 
            this.itemMenuItemCut.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Cut;
            this.itemMenuItemCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemMenuItemCut.Name = "itemMenuItemCut";
            this.itemMenuItemCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.itemMenuItemCut.Size = new System.Drawing.Size(225, 22);
            this.itemMenuItemCut.Text = "Cut";
            this.itemMenuItemCut.Click += new System.EventHandler(this.itemMenuItemCut_Click);
            // 
            // itemMenuItemCopy
            // 
            this.itemMenuItemCopy.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Copy;
            this.itemMenuItemCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemMenuItemCopy.Name = "itemMenuItemCopy";
            this.itemMenuItemCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.itemMenuItemCopy.Size = new System.Drawing.Size(225, 22);
            this.itemMenuItemCopy.Text = "Copy";
            this.itemMenuItemCopy.Click += new System.EventHandler(this.itemMenuItemCopy_Click);
            // 
            // itemMenuItemPaste
            // 
            this.itemMenuItemPaste.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Paste;
            this.itemMenuItemPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemMenuItemPaste.Name = "itemMenuItemPaste";
            this.itemMenuItemPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.itemMenuItemPaste.Size = new System.Drawing.Size(225, 22);
            this.itemMenuItemPaste.Text = "Paste";
            this.itemMenuItemPaste.Click += new System.EventHandler(this.folderMenuItemPaste_Click);
            // 
            // itemMenuItemDelete
            // 
            this.itemMenuItemDelete.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Delete;
            this.itemMenuItemDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemMenuItemDelete.Name = "itemMenuItemDelete";
            this.itemMenuItemDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.itemMenuItemDelete.Size = new System.Drawing.Size(225, 22);
            this.itemMenuItemDelete.Text = "Delete";
            this.itemMenuItemDelete.Click += new System.EventHandler(this.itemMenuItemDelete_Click);
            // 
            // itemMenuItemRename
            // 
            this.itemMenuItemRename.Name = "itemMenuItemRename";
            this.itemMenuItemRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.itemMenuItemRename.Size = new System.Drawing.Size(225, 22);
            this.itemMenuItemRename.Text = "Rename";
            this.itemMenuItemRename.Click += new System.EventHandler(this.itemMenuItemRename_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(222, 6);
            this.toolStripSeparator2.Visible = false;
            // 
            // connectPreProcessToolStripMenuItem
            // 
            this.connectPreProcessToolStripMenuItem.Name = "connectPreProcessToolStripMenuItem";
            this.connectPreProcessToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.connectPreProcessToolStripMenuItem.Text = "Connect Pre-Process";
            // 
            // disconnectPreProcessToolStripMenuItem
            // 
            this.disconnectPreProcessToolStripMenuItem.Name = "disconnectPreProcessToolStripMenuItem";
            this.disconnectPreProcessToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.disconnectPreProcessToolStripMenuItem.Text = "Disconnect Pre-Process";
            this.disconnectPreProcessToolStripMenuItem.Click += new System.EventHandler(this.disconnectPreProcessToolStripMenuItem_Click);
            // 
            // itemMenuItemConnect
            // 
            this.itemMenuItemConnect.Name = "itemMenuItemConnect";
            this.itemMenuItemConnect.Size = new System.Drawing.Size(225, 22);
            this.itemMenuItemConnect.Text = "Connect Post-Process";
            this.itemMenuItemConnect.Visible = false;
            // 
            // itemMenuItemDisconnect
            // 
            this.itemMenuItemDisconnect.Name = "itemMenuItemDisconnect";
            this.itemMenuItemDisconnect.Size = new System.Drawing.Size(225, 22);
            this.itemMenuItemDisconnect.Text = "Disconnect Post-Process";
            this.itemMenuItemDisconnect.Visible = false;
            this.itemMenuItemDisconnect.Click += new System.EventHandler(this.itemMenuItemFormDisconnect_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(222, 6);
            this.toolStripSeparator4.Visible = false;
            // 
            // blockBackButtonToolStripMenuItem
            // 
            this.blockBackButtonToolStripMenuItem.CheckOnClick = true;
            this.blockBackButtonToolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.BlockBackButtonTrue;
            this.blockBackButtonToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.blockBackButtonToolStripMenuItem.Name = "blockBackButtonToolStripMenuItem";
            this.blockBackButtonToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.blockBackButtonToolStripMenuItem.Text = "Block Back Button";
            this.blockBackButtonToolStripMenuItem.CheckedChanged += new System.EventHandler(this.blockBackButtonToolStripMenuItem_CheckedChanged);
            // 
            // startingPointToolStripMenuItem
            // 
            this.startingPointToolStripMenuItem.CheckOnClick = true;
            this.startingPointToolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.Form_MakeStartPoint;
            this.startingPointToolStripMenuItem.Name = "startingPointToolStripMenuItem";
            this.startingPointToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.startingPointToolStripMenuItem.Text = "Starting Point";
            this.startingPointToolStripMenuItem.Visible = false;
            this.startingPointToolStripMenuItem.CheckedChanged += new System.EventHandler(this.startingPointToolStripMenuItem_CheckedChanged);
            // 
            // dataEntryOnlyToolStripMenuItem
            // 
            this.dataEntryOnlyToolStripMenuItem.Image = global::Tawala.DesignerUI.Properties.Resources.Prepopulate;
            this.dataEntryOnlyToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.dataEntryOnlyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.dataEntryOnlyToolStripMenuItem.Name = "dataEntryOnlyToolStripMenuItem";
            this.dataEntryOnlyToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.dataEntryOnlyToolStripMenuItem.Text = "Pre-populate With Last Entry";
            this.dataEntryOnlyToolStripMenuItem.CheckedChanged += new System.EventHandler(this.dataEntryOnlyToolStripMenuItem_CheckedChanged);
            // 
            // dataSourceNameToolStripMenuItem
            // 
            this.dataSourceNameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBoxDataSourceName});
            this.dataSourceNameToolStripMenuItem.Name = "dataSourceNameToolStripMenuItem";
            this.dataSourceNameToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.dataSourceNameToolStripMenuItem.Text = "Data Source Name";
            // 
            // toolStripTextBoxDataSourceName
            // 
            this.toolStripTextBoxDataSourceName.Name = "toolStripTextBoxDataSourceName";
            this.toolStripTextBoxDataSourceName.Size = new System.Drawing.Size(150, 23);
            this.toolStripTextBoxDataSourceName.Text = "Type Data Source Name Here";
            this.toolStripTextBoxDataSourceName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.toolStripTextBoxDataSourceName_KeyPress);
            this.toolStripTextBoxDataSourceName.TextChanged += new System.EventHandler(this.toolStripTextBoxDataSourceName_TextChanged);
            // 
            // label
            // 
            this.label.Dock = System.Windows.Forms.DockStyle.Top;
            this.label.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.ForeColor = System.Drawing.Color.White;
            this.label.Location = new System.Drawing.Point(0, 0);
            this.label.Margin = new System.Windows.Forms.Padding(0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(181, 20);
            this.label.TabIndex = 2;
            this.label.Text = "Project Explorer";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolStrip
            // 
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFormToolStripButton,
            this.newProjectToolStripButton,
            this.newDocumentToolStripButton,
            toolStripSep1,
            this.nodeMoveUpToolStripButton,
            this.nodeMoveDownToolStripButton,
            this.toolStripSeparator3,
            this.startPointToolStripButton,
            this.blockBackButtonToolStripButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 20);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip.Size = new System.Drawing.Size(181, 25);
            this.toolStrip.TabIndex = 3;
            this.toolStrip.Text = "toolStrip";
            // 
            // newFormToolStripButton
            // 
            this.newFormToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newFormToolStripButton.DoubleClickEnabled = true;
            this.newFormToolStripButton.Image = global::Tawala.DesignerUI.Properties.Resources.Form_New;
            this.newFormToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newFormToolStripButton.Name = "newFormToolStripButton";
            this.newFormToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newFormToolStripButton.Text = "New Form";
            this.newFormToolStripButton.Click += new System.EventHandler(this.newFormToolStripButton_Click);
            // 
            // newProjectToolStripButton
            // 
            this.newProjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newProjectToolStripButton.DoubleClickEnabled = true;
            this.newProjectToolStripButton.Image = global::Tawala.DesignerUI.Properties.Resources.Process_New;
            this.newProjectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newProjectToolStripButton.Name = "newProjectToolStripButton";
            this.newProjectToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newProjectToolStripButton.Text = "New Process";
            this.newProjectToolStripButton.Click += new System.EventHandler(this.newProjectToolStripButton_Click);
            // 
            // newDocumentToolStripButton
            // 
            this.newDocumentToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newDocumentToolStripButton.DoubleClickEnabled = true;
            this.newDocumentToolStripButton.Image = global::Tawala.DesignerUI.Properties.Resources.Document_New;
            this.newDocumentToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newDocumentToolStripButton.Name = "newDocumentToolStripButton";
            this.newDocumentToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newDocumentToolStripButton.Text = "New Document";
            this.newDocumentToolStripButton.Click += new System.EventHandler(this.newDocumentToolStripButton_Click);
            // 
            // nodeMoveUpToolStripButton
            // 
            this.nodeMoveUpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nodeMoveUpToolStripButton.Image = global::Tawala.DesignerUI.Properties.Resources.MovePrevious;
            this.nodeMoveUpToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.nodeMoveUpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nodeMoveUpToolStripButton.Name = "nodeMoveUpToolStripButton";
            this.nodeMoveUpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nodeMoveUpToolStripButton.Text = "Move Node Up";
            this.nodeMoveUpToolStripButton.Click += new System.EventHandler(this.nodeMoveUpToolStripButton_Click);
            // 
            // nodeMoveDownToolStripButton
            // 
            this.nodeMoveDownToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nodeMoveDownToolStripButton.Image = global::Tawala.DesignerUI.Properties.Resources.MoveNext;
            this.nodeMoveDownToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.nodeMoveDownToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nodeMoveDownToolStripButton.Name = "nodeMoveDownToolStripButton";
            this.nodeMoveDownToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.nodeMoveDownToolStripButton.Text = "Move Node Down";
            this.nodeMoveDownToolStripButton.Click += new System.EventHandler(this.nodeMoveDownToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // startPointToolStripButton
            // 
            this.startPointToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.startPointToolStripButton.Image = global::Tawala.DesignerUI.Properties.Resources.Form_MakeStartPoint;
            this.startPointToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.startPointToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startPointToolStripButton.Name = "startPointToolStripButton";
            this.startPointToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.startPointToolStripButton.Text = "Toggle Form Starting Point";
            this.startPointToolStripButton.Click += new System.EventHandler(this.startPointToolStripButton_Click);
            // 
            // blockBackButtonToolStripButton
            // 
            this.blockBackButtonToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.blockBackButtonToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("blockBackButtonToolStripButton.Image")));
            this.blockBackButtonToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.blockBackButtonToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.blockBackButtonToolStripButton.Name = "blockBackButtonToolStripButton";
            this.blockBackButtonToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.blockBackButtonToolStripButton.Text = "Block Back Button";
            this.blockBackButtonToolStripButton.Click += new System.EventHandler(this.blockBackButtonToolStripButton_Click);
            // 
            // tree
            // 
            this.tree.AllowDrop = true;
            this.tree.BackColor = System.Drawing.Color.White;
            this.tree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tree.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tree.ForeColor = System.Drawing.Color.Black;
            this.tree.HideSelection = false;
            this.tree.Indent = 19;
            this.tree.ItemHeight = 16;
            this.tree.LabelEdit = true;
            this.tree.Location = new System.Drawing.Point(0, 45);
            this.tree.Margin = new System.Windows.Forms.Padding(0);
            this.tree.Name = "tree";
            this.tree.Size = new System.Drawing.Size(146, 79);
            this.tree.TabIndex = 1;
            this.tree.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterCollapse);
            this.tree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_AfterLabelEdit);
            this.tree.DragDrop += new System.Windows.Forms.DragEventHandler(this.tree_DragDrop);
            this.tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
            this.tree.DragEnter += new System.Windows.Forms.DragEventHandler(this.tree_DragEnter);
            this.tree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tree_NodeMouseClick);
            this.tree.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_BeforeLabelEdit);
            this.tree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tree_KeyDown);
            this.tree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterExpand);
            this.tree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tree_ItemDrag);
            this.tree.DragOver += new System.Windows.Forms.DragEventHandler(this.tree_DragOver);
            // 
            // ProjectExplorer
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tree);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.label);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(186, 0);
            this.Name = "ProjectExplorer";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.Size = new System.Drawing.Size(186, 124);
            this.folderContextMenuStrip.ResumeLayout(false);
            this.componentContextMenuStrip.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton newFormToolStripButton;
		private System.Windows.Forms.ToolStripButton newProjectToolStripButton;
		private System.Windows.Forms.ToolStripButton newDocumentToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem viewItemtoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startingPointToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton startPointToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem connectPreProcessToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem disconnectPreProcessToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem dataEntryOnlyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem dataSourceNameToolStripMenuItem;
		private System.Windows.Forms.ToolStripTextBox toolStripTextBoxDataSourceName;
		private System.Windows.Forms.ToolStripMenuItem folderMenuItemCollapseChildNodes;
		private System.Windows.Forms.ToolStripSeparator folderMenuChildNodesSeparator;
		private System.Windows.Forms.ToolStripMenuItem folderMenuItemExpandChildNodes;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton nodeMoveUpToolStripButton;
        private System.Windows.Forms.ToolStripButton nodeMoveDownToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem blockBackButtonToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton blockBackButtonToolStripButton;
	}
}
