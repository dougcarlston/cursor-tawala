namespace Tawala.ProjectExplorer
{
	partial class ProjectExplorerView
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Forms");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Processes");
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Documents");
			this.contextMenuStripRootNodes = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemPasteRootNodes = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonNewForm = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonNewProcess = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonNewDocument = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonStartPoint = new System.Windows.Forms.ToolStripButton();
			this.treeViewProjectComponents = new System.Windows.Forms.TreeView();
			this.contextMenuStripFormNode = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemCut = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemRename = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.startingPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.connectPreProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.disconnectPreProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.connectPostProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.disconnectPostProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.dataEntryOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemDataSourceName = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripTextBoxDataSourceName = new System.Windows.Forms.ToolStripTextBox();
			this.contextMenuStripComponentNode = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemCutComponentNode = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCopyComponentNode = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemPasteComponentNode = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemDeleteComponentNode = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemRenameComponentNode = new System.Windows.Forms.ToolStripMenuItem();
			this.gradientLabelProjectExplorer = new Tawala.Common.GradientLabel();
			this.contextMenuStripRootNodes.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.contextMenuStripFormNode.SuspendLayout();
			this.contextMenuStripComponentNode.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStripRootNodes
			// 
			this.contextMenuStripRootNodes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemPasteRootNodes});
			this.contextMenuStripRootNodes.Name = "contextMenuStripRootNodes";
			this.contextMenuStripRootNodes.Size = new System.Drawing.Size(165, 48);
			this.contextMenuStripRootNodes.Opened += new System.EventHandler(this.contextMenuStripRootNodes_Opened);
			// 
			// toolStripMenuItemPasteRootNodes
			// 
			this.toolStripMenuItemPasteRootNodes.Image = global::Tawala.ProjectExplorer.Properties.Resources.Edit_Paste;
			this.toolStripMenuItemPasteRootNodes.Name = "toolStripMenuItemPasteRootNodes";
			this.toolStripMenuItemPasteRootNodes.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.toolStripMenuItemPasteRootNodes.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemPasteRootNodes.Text = "Paste";
			this.toolStripMenuItemPasteRootNodes.Click += new System.EventHandler(this.toolStripMenuItemPasteRootNodes_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNewForm,
            this.toolStripButtonNewProcess,
            this.toolStripButtonNewDocument,
            this.toolStripSeparator1,
            this.toolStripButtonStartPoint});
			this.toolStrip1.Location = new System.Drawing.Point(0, 20);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(150, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonNewForm
			// 
			this.toolStripButtonNewForm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonNewForm.Image = global::Tawala.ProjectExplorer.Properties.Resources.Form_New;
			this.toolStripButtonNewForm.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonNewForm.Name = "toolStripButtonNewForm";
			this.toolStripButtonNewForm.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonNewForm.Text = "New Form";
			this.toolStripButtonNewForm.Click += new System.EventHandler(this.toolStripButtonNewForm_Click);
			// 
			// toolStripButtonNewProcess
			// 
			this.toolStripButtonNewProcess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonNewProcess.Image = global::Tawala.ProjectExplorer.Properties.Resources.Process_New;
			this.toolStripButtonNewProcess.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonNewProcess.Name = "toolStripButtonNewProcess";
			this.toolStripButtonNewProcess.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonNewProcess.Text = "New Process";
			this.toolStripButtonNewProcess.Click += new System.EventHandler(this.toolStripButtonNewProcess_Click);
			// 
			// toolStripButtonNewDocument
			// 
			this.toolStripButtonNewDocument.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonNewDocument.Image = global::Tawala.ProjectExplorer.Properties.Resources.Document_New;
			this.toolStripButtonNewDocument.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonNewDocument.Name = "toolStripButtonNewDocument";
			this.toolStripButtonNewDocument.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonNewDocument.Text = "toolStripButton1";
			this.toolStripButtonNewDocument.Click += new System.EventHandler(this.toolStripButtonNewDocument_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonStartPoint
			// 
			this.toolStripButtonStartPoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonStartPoint.Enabled = false;
			this.toolStripButtonStartPoint.Image = global::Tawala.ProjectExplorer.Properties.Resources.Form_MakeStartPoint;
			this.toolStripButtonStartPoint.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonStartPoint.Name = "toolStripButtonStartPoint";
			this.toolStripButtonStartPoint.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonStartPoint.Text = "Toggle Form Starting Point";
			this.toolStripButtonStartPoint.Click += new System.EventHandler(this.toolStripButtonStartPoint_Click);
			// 
			// treeViewProjectComponents
			// 
			this.treeViewProjectComponents.AllowDrop = true;
			this.treeViewProjectComponents.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewProjectComponents.HideSelection = false;
			this.treeViewProjectComponents.LabelEdit = true;
			this.treeViewProjectComponents.Location = new System.Drawing.Point(0, 45);
			this.treeViewProjectComponents.Name = "treeViewProjectComponents";
			treeNode1.ContextMenuStrip = this.contextMenuStripRootNodes;
			treeNode1.Name = "Forms";
			treeNode1.Text = "Forms";
			treeNode2.ContextMenuStrip = this.contextMenuStripRootNodes;
			treeNode2.Name = "Processes";
			treeNode2.Text = "Processes";
			treeNode3.ContextMenuStrip = this.contextMenuStripRootNodes;
			treeNode3.Name = "Documents";
			treeNode3.Text = "Documents";
			this.treeViewProjectComponents.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
			this.treeViewProjectComponents.Size = new System.Drawing.Size(150, 105);
			this.treeViewProjectComponents.TabIndex = 2;
			this.treeViewProjectComponents.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewProjectComponents_AfterLabelEdit);
			this.treeViewProjectComponents.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewProjectComponents_DragDrop);
			this.treeViewProjectComponents.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewProjectComponents_MouseDown);
			this.treeViewProjectComponents.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewProjectComponents_DragEnter);
			this.treeViewProjectComponents.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewProjectComponents_NodeMouseClick);
			this.treeViewProjectComponents.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewProjectComponents_BeforeLabelEdit);
			this.treeViewProjectComponents.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewProjectComponents_ItemDrag);
			this.treeViewProjectComponents.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewProjectComponents_DragOver);
			// 
			// contextMenuStripFormNode
			// 
			this.contextMenuStripFormNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCut,
            this.toolStripMenuItemCopy,
            this.toolStripMenuItemPaste,
            this.toolStripMenuItemDelete,
            this.toolStripMenuItemRename,
            this.toolStripSeparator3,
            this.startingPointToolStripMenuItem,
            this.toolStripSeparator2,
            this.connectPreProcessToolStripMenuItem,
            this.disconnectPreProcessToolStripMenuItem,
            this.connectPostProcessToolStripMenuItem,
            this.disconnectPostProcessToolStripMenuItem,
            this.toolStripSeparator4,
            this.dataEntryOnlyToolStripMenuItem,
            this.toolStripMenuItemDataSourceName});
			this.contextMenuStripFormNode.Name = "contextMenuStripFormNode";
			this.contextMenuStripFormNode.Size = new System.Drawing.Size(253, 286);
			this.contextMenuStripFormNode.Opened += new System.EventHandler(this.contextMenuStripFormNode_Opened);
			this.contextMenuStripFormNode.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripFormNode_Opening);
			// 
			// toolStripMenuItemCut
			// 
			this.toolStripMenuItemCut.Image = global::Tawala.ProjectExplorer.Properties.Resources.Edit_Cut;
			this.toolStripMenuItemCut.Name = "toolStripMenuItemCut";
			this.toolStripMenuItemCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.toolStripMenuItemCut.Size = new System.Drawing.Size(252, 22);
			this.toolStripMenuItemCut.Text = "Cut";
			this.toolStripMenuItemCut.Click += new System.EventHandler(this.toolStripMenuItemCut_Click);
			// 
			// toolStripMenuItemCopy
			// 
			this.toolStripMenuItemCopy.Image = global::Tawala.ProjectExplorer.Properties.Resources.Edit_Copy;
			this.toolStripMenuItemCopy.Name = "toolStripMenuItemCopy";
			this.toolStripMenuItemCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.toolStripMenuItemCopy.Size = new System.Drawing.Size(252, 22);
			this.toolStripMenuItemCopy.Text = "Copy";
			this.toolStripMenuItemCopy.Click += new System.EventHandler(this.toolStripMenuItemCopy_Click);
			// 
			// toolStripMenuItemPaste
			// 
			this.toolStripMenuItemPaste.Image = global::Tawala.ProjectExplorer.Properties.Resources.Edit_Paste;
			this.toolStripMenuItemPaste.Name = "toolStripMenuItemPaste";
			this.toolStripMenuItemPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.toolStripMenuItemPaste.Size = new System.Drawing.Size(252, 22);
			this.toolStripMenuItemPaste.Text = "Paste";
			this.toolStripMenuItemPaste.Click += new System.EventHandler(this.toolStripMenuItemPaste_Click);
			// 
			// toolStripMenuItemDelete
			// 
			this.toolStripMenuItemDelete.Image = global::Tawala.ProjectExplorer.Properties.Resources.Edit_Delete;
			this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
			this.toolStripMenuItemDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.toolStripMenuItemDelete.Size = new System.Drawing.Size(252, 22);
			this.toolStripMenuItemDelete.Text = "Delete";
			this.toolStripMenuItemDelete.Click += new System.EventHandler(this.toolStripMenuItemDelete_Click);
			// 
			// toolStripMenuItemRename
			// 
			this.toolStripMenuItemRename.Name = "toolStripMenuItemRename";
			this.toolStripMenuItemRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.toolStripMenuItemRename.Size = new System.Drawing.Size(252, 22);
			this.toolStripMenuItemRename.Text = "Rename";
			this.toolStripMenuItemRename.Click += new System.EventHandler(this.toolStripMenuItemRename_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(249, 6);
			// 
			// startingPointToolStripMenuItem
			// 
			this.startingPointToolStripMenuItem.CheckOnClick = true;
			this.startingPointToolStripMenuItem.Name = "startingPointToolStripMenuItem";
			this.startingPointToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
			this.startingPointToolStripMenuItem.Text = "Starting Point";
			this.startingPointToolStripMenuItem.Click += new System.EventHandler(this.startingPointToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(249, 6);
			// 
			// connectPreProcessToolStripMenuItem
			// 
			this.connectPreProcessToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
			this.connectPreProcessToolStripMenuItem.Enabled = false;
			this.connectPreProcessToolStripMenuItem.Name = "connectPreProcessToolStripMenuItem";
			this.connectPreProcessToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
			this.connectPreProcessToolStripMenuItem.Text = "Connect Pre-Process";
			// 
			// testToolStripMenuItem
			// 
			this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			this.testToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
			this.testToolStripMenuItem.Text = "Test";
			// 
			// disconnectPreProcessToolStripMenuItem
			// 
			this.disconnectPreProcessToolStripMenuItem.Enabled = false;
			this.disconnectPreProcessToolStripMenuItem.Name = "disconnectPreProcessToolStripMenuItem";
			this.disconnectPreProcessToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
			this.disconnectPreProcessToolStripMenuItem.Text = "Disconnect Pre-Process";
			this.disconnectPreProcessToolStripMenuItem.Click += new System.EventHandler(this.disconnectPreProcessToolStripMenuItem_Click);
			// 
			// connectPostProcessToolStripMenuItem
			// 
			this.connectPostProcessToolStripMenuItem.Enabled = false;
			this.connectPostProcessToolStripMenuItem.Name = "connectPostProcessToolStripMenuItem";
			this.connectPostProcessToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
			this.connectPostProcessToolStripMenuItem.Text = "Connect Post-Process";
			this.connectPostProcessToolStripMenuItem.Click += new System.EventHandler(this.connectPostProcessToolStripMenuItem_Click);
			// 
			// disconnectPostProcessToolStripMenuItem
			// 
			this.disconnectPostProcessToolStripMenuItem.Enabled = false;
			this.disconnectPostProcessToolStripMenuItem.Name = "disconnectPostProcessToolStripMenuItem";
			this.disconnectPostProcessToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
			this.disconnectPostProcessToolStripMenuItem.Text = "Disconnect Post-Process";
			this.disconnectPostProcessToolStripMenuItem.Click += new System.EventHandler(this.disconnectPostProcessToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(249, 6);
			// 
			// dataEntryOnlyToolStripMenuItem
			// 
			this.dataEntryOnlyToolStripMenuItem.CheckOnClick = true;
			this.dataEntryOnlyToolStripMenuItem.Name = "dataEntryOnlyToolStripMenuItem";
			this.dataEntryOnlyToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
			this.dataEntryOnlyToolStripMenuItem.Text = "Pre-populate With Last Entry";
			this.dataEntryOnlyToolStripMenuItem.Click += new System.EventHandler(this.dataEntryOnlyToolStripMenuItem_Click);
			// 
			// toolStripMenuItemDataSourceName
			// 
			this.toolStripMenuItemDataSourceName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripMenuItemDataSourceName.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBoxDataSourceName});
			this.toolStripMenuItemDataSourceName.ForeColor = System.Drawing.Color.Black;
			this.toolStripMenuItemDataSourceName.Name = "toolStripMenuItemDataSourceName";
			this.toolStripMenuItemDataSourceName.Size = new System.Drawing.Size(252, 22);
			this.toolStripMenuItemDataSourceName.Text = "Data Source Name";
			this.toolStripMenuItemDataSourceName.DropDownClosed += new System.EventHandler(this.toolStripMenuItemDataSourceName_DropDownClosed);
			// 
			// toolStripTextBoxDataSourceName
			// 
			this.toolStripTextBoxDataSourceName.BackColor = System.Drawing.SystemColors.Window;
			this.toolStripTextBoxDataSourceName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.toolStripTextBoxDataSourceName.MaxLength = 128;
			this.toolStripTextBoxDataSourceName.Name = "toolStripTextBoxDataSourceName";
			this.toolStripTextBoxDataSourceName.Size = new System.Drawing.Size(150, 23);
			this.toolStripTextBoxDataSourceName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBoxDataSourceName_KeyDown);
			this.toolStripTextBoxDataSourceName.TextChanged += new System.EventHandler(this.toolStripTextBoxDataSourceName_TextChanged);
			// 
			// contextMenuStripComponentNode
			// 
			this.contextMenuStripComponentNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCutComponentNode,
            this.toolStripMenuItemCopyComponentNode,
            this.toolStripMenuItemPasteComponentNode,
            this.toolStripMenuItemDeleteComponentNode,
            this.toolStripMenuItemRenameComponentNode});
			this.contextMenuStripComponentNode.Name = "contextMenuStripComponentNode";
			this.contextMenuStripComponentNode.Size = new System.Drawing.Size(165, 114);
			this.contextMenuStripComponentNode.Opened += new System.EventHandler(this.contextMenuStripComponentNode_Opened);
			// 
			// toolStripMenuItemCutComponentNode
			// 
			this.toolStripMenuItemCutComponentNode.Image = global::Tawala.ProjectExplorer.Properties.Resources.Edit_Cut;
			this.toolStripMenuItemCutComponentNode.Name = "toolStripMenuItemCutComponentNode";
			this.toolStripMenuItemCutComponentNode.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.toolStripMenuItemCutComponentNode.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemCutComponentNode.Text = "Cut";
			this.toolStripMenuItemCutComponentNode.Click += new System.EventHandler(this.toolStripMenuItemCutComponentNode_Click);
			// 
			// toolStripMenuItemCopyComponentNode
			// 
			this.toolStripMenuItemCopyComponentNode.Image = global::Tawala.ProjectExplorer.Properties.Resources.Edit_Copy;
			this.toolStripMenuItemCopyComponentNode.Name = "toolStripMenuItemCopyComponentNode";
			this.toolStripMenuItemCopyComponentNode.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.toolStripMenuItemCopyComponentNode.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemCopyComponentNode.Text = "Copy";
			this.toolStripMenuItemCopyComponentNode.Click += new System.EventHandler(this.toolStripMenuItemCopyComponentNode_Click);
			// 
			// toolStripMenuItemPasteComponentNode
			// 
			this.toolStripMenuItemPasteComponentNode.Image = global::Tawala.ProjectExplorer.Properties.Resources.Edit_Paste;
			this.toolStripMenuItemPasteComponentNode.Name = "toolStripMenuItemPasteComponentNode";
			this.toolStripMenuItemPasteComponentNode.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.toolStripMenuItemPasteComponentNode.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemPasteComponentNode.Text = "Paste";
			this.toolStripMenuItemPasteComponentNode.Click += new System.EventHandler(this.toolStripMenuItemPasteComponentNode_Click);
			// 
			// toolStripMenuItemDeleteComponentNode
			// 
			this.toolStripMenuItemDeleteComponentNode.Image = global::Tawala.ProjectExplorer.Properties.Resources.Edit_Delete;
			this.toolStripMenuItemDeleteComponentNode.Name = "toolStripMenuItemDeleteComponentNode";
			this.toolStripMenuItemDeleteComponentNode.ShortcutKeys = System.Windows.Forms.Keys.Delete;
			this.toolStripMenuItemDeleteComponentNode.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemDeleteComponentNode.Text = "Delete";
			this.toolStripMenuItemDeleteComponentNode.Click += new System.EventHandler(this.toolStripMenuItemDeleteComponentNode_Click);
			// 
			// toolStripMenuItemRenameComponentNode
			// 
			this.toolStripMenuItemRenameComponentNode.Name = "toolStripMenuItemRenameComponentNode";
			this.toolStripMenuItemRenameComponentNode.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.toolStripMenuItemRenameComponentNode.Size = new System.Drawing.Size(164, 22);
			this.toolStripMenuItemRenameComponentNode.Text = "Rename";
			this.toolStripMenuItemRenameComponentNode.Click += new System.EventHandler(this.toolStripMenuItemRenameComponentNode_Click);
			// 
			// gradientLabelProjectExplorer
			// 
			this.gradientLabelProjectExplorer.Dock = System.Windows.Forms.DockStyle.Top;
			this.gradientLabelProjectExplorer.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gradientLabelProjectExplorer.ForeColor = System.Drawing.Color.White;
			this.gradientLabelProjectExplorer.Location = new System.Drawing.Point(0, 0);
			this.gradientLabelProjectExplorer.Margin = new System.Windows.Forms.Padding(0);
			this.gradientLabelProjectExplorer.Name = "gradientLabelProjectExplorer";
			this.gradientLabelProjectExplorer.Size = new System.Drawing.Size(150, 20);
			this.gradientLabelProjectExplorer.TabIndex = 0;
			this.gradientLabelProjectExplorer.Text = "Project Explorer";
			this.gradientLabelProjectExplorer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ProjectExplorerView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.treeViewProjectComponents);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.gradientLabelProjectExplorer);
			this.Name = "ProjectExplorerView";
			this.contextMenuStripRootNodes.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.contextMenuStripFormNode.ResumeLayout(false);
			this.contextMenuStripComponentNode.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Tawala.Common.GradientLabel gradientLabelProjectExplorer;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonNewForm;
		private System.Windows.Forms.ToolStripButton toolStripButtonNewProcess;
		private System.Windows.Forms.ToolStripButton toolStripButtonNewDocument;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripButtonStartPoint;
		private System.Windows.Forms.TreeView treeViewProjectComponents;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripFormNode;
		private System.Windows.Forms.ToolStripMenuItem startingPointToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem connectPreProcessToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem disconnectPreProcessToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem connectPostProcessToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem disconnectPostProcessToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRename;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripComponentNode;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRenameComponentNode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDataSourceName;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxDataSourceName;
        private System.Windows.Forms.ToolStripMenuItem dataEntryOnlyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCut;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPaste;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripRootNodes;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPasteRootNodes;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopy;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelete;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCutComponentNode;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopyComponentNode;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPasteComponentNode;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDeleteComponentNode;

	}
}
