// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Interfaces;
using Tawala.ProjectUI;
using Tawala.Common;
using Tawala.MainApplication;

namespace Tawala.ProjectExplorer
{
	public partial class ProjectExplorerView : UserControl, IProjectExplorerView
	{
		private ImageList nodeImages;
		private const int imageFolderClosed = 0;
		private const int imageFolderOpen = 1;
		private const int imageForm = 2;
		private const int imageProcess = 3;
		private const int imageDocument = 4;
		private const int imageFormProcess = 5;
		private const int imageFormIsStartPoint = 6;
		private const int imageFormPreProcess = 7;
		private const int imageFormDisplayLastRecord = 8;
		private const int imageStartPointDisplayLastRecord = 9;

		public ProjectExplorerView()
		{
			InitializeComponent();

			initializeNodeImages();
			setRootNodeImages();

			Presenter = new ProjectExplorerPresenter(this);
		}

		private void initializeNodeImages()
		{
			nodeImages = new ImageList();
			nodeImages.ColorDepth = ColorDepth.Depth24Bit;
			nodeImages.ImageSize = new Size(16, 16);
			nodeImages.TransparentColor = Color.Magenta;

			nodeImages.Images.Add(Properties.Resources.Folder_Closed);
			nodeImages.Images.Add(Properties.Resources.Folder_Open);
			nodeImages.Images.Add(Properties.Resources.Form);
			nodeImages.Images.Add(Properties.Resources.Process);
			nodeImages.Images.Add(Properties.Resources.Document);
			nodeImages.Images.Add(Properties.Resources.Form_PostProcess);
			nodeImages.Images.Add(Properties.Resources.Form_IsStartPoint);
			nodeImages.Images.Add(Properties.Resources.Form_PreProcess);
			nodeImages.Images.Add(Properties.Resources.Form_LastRecord);
			nodeImages.Images.Add(Properties.Resources.StartPointLastRecord);

			treeViewProjectComponents.ImageList = nodeImages;
		}

		private void setRootNodeImages()
		{
			foreach (TreeNode node in treeViewProjectComponents.Nodes)
			{
				node.ImageIndex = node.SelectedImageIndex = imageFolderClosed;
			}
		}

		private void toolStripButtonNewForm_Click(object sender, EventArgs e)
		{
			Presenter.NewFormRequested();
		}

		private void toolStripButtonNewDocument_Click(object sender, EventArgs e)
		{
			Presenter.NewDocumentRequested();
		}

		private void toolStripButtonNewProcess_Click(object sender, EventArgs e)
		{
			Presenter.NewProcessRequested();
		}

		private void toolStripButtonStartPoint_Click(object sender, EventArgs e)
		{
			Presenter.StartingPointToggleRequested();
		}

		private void treeViewProjectComponents_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			toolStripButtonStartPoint.Enabled = false;

			IFormView formView = e.Node.Tag as IFormView;

			if (formView != null)
			{
				Presenter.FormSelected(formView);
				return;
			}

			IDocumentView documentView = e.Node.Tag as IDocumentView;

			if (documentView != null)
			{
				Presenter.DocumentSelected(documentView);
				return;
			}

			IProcessView processView = e.Node.Tag as IProcessView;

			if (processView != null)
			{
				Presenter.ProcessSelected(processView);
				return;
			}
		}

		private TreeNode getFormsNode()
		{
			return getRootNode("Forms");
		}

		private TreeNode getDocumentsNode()
		{
			return getRootNode("Documents");
		}

		private TreeNode getProcessesNode()
		{
			return getRootNode("Processes");
		}

		private TreeNode getRootNode(string rootNodeName)
		{
			foreach (TreeNode rootNode in treeViewProjectComponents.Nodes)
			{
				if (rootNode.Name == rootNodeName)
				{
					return rootNode;
				}
			}

			return null;
		}

		private bool isRootNode(TreeNode node)
		{
			foreach (TreeNode rootNode in treeViewProjectComponents.Nodes)
			{
				if (node == rootNode)
				{
					return true;
				}
			}

			return false;
		}

		private bool isFormNode(TreeNode node)
		{
			foreach (TreeNode formNode in getFormsNode().Nodes)
			{
				if (node == formNode)
				{
					return true;
				}
			}

			return false;
		}

		private bool isDocumentNode(TreeNode node)
		{
			foreach (TreeNode documentNode in getDocumentsNode().Nodes)
			{
				if (node == documentNode)
				{
					return true;
				}
			}

			return false;
		}

		private bool isProcessNode(TreeNode node)
		{
			foreach (TreeNode processNode in getProcessesNode().Nodes)
			{
				if (node == processNode)
				{
					return true;
				}
			}

			return false;
		}

		#region IProjectExplorerView Members

		public void ClearProject()
		{
			clearFormNodes();
			clearDocumentNodes();
			clearProcessNodes();
		}

		private void clearFormNodes()
		{
			foreach (TreeNode node in getFormsNode().Nodes)
			{
				IFormView formView = node.Tag as IFormView;
				formView.Close();
			}

			getFormsNode().Nodes.Clear();
		}

		private void clearDocumentNodes()
		{
			foreach (TreeNode node in getDocumentsNode().Nodes)
			{
				IDocumentView documentView = node.Tag as IDocumentView;
				documentView.Close();
			}

			getDocumentsNode().Nodes.Clear();
		}

		private void clearProcessNodes()
		{
			foreach (TreeNode node in getProcessesNode().Nodes)
			{
				IProcessView processesView = node.Tag as IProcessView;
				processesView.Close();
			}

			getProcessesNode().Nodes.Clear();
		}

		public void AddForm(IFormView formView, bool isStartingPoint)
		{
			int imageIndex;
			int selectedImageIndex;
			imageIndex = selectedImageIndex = getFormImageIndex(isStartingPoint);

			TreeNode node = new TreeNode(formView.Presenter.Form.Name, imageIndex, selectedImageIndex);
			node.Tag = formView;
			node.ContextMenuStrip = contextMenuStripFormNode;

			getFormsNode().Nodes.Add(node);
		}

		public bool CanPasteForm
		{
			get { return treeViewProjectComponents.SelectedNode == getFormsNode() || isFormNode(treeViewProjectComponents.SelectedNode); }
		}

		public void CutForm(string formName)
		{
			removeForm(formName);
		}

		private void removeForm(string name)
		{
			IFormView formView = GetFormView(name);

			formView.Close();

			TreeNode formNode = getFormNode(name);
			formNode.Remove();
		}

		public void PasteForm(IFormView formView, bool isStartingPoint)
		{
			AddForm(formView, isStartingPoint);
		}

		public void DeleteForm(string formName)
		{
			if (ConfirmDialog.ConfirmDelete("Form", formName))
			{
				removeForm(formName);
			}
		}

		public void SelectForm(IFormView formView)
		{
			if (isValidFormView(formView))
			{
				toolStripButtonStartPoint.Enabled = true;
				highlightFormNode(formView);
			}
		}

		public void DeselectForm()
		{
			highlightFormNode(null);
		}

		public Collection<string> GetFormNames()
		{
			Collection<string> formNames = new Collection<string>();

			foreach (TreeNode formNode in getFormsNode().Nodes)
			{
				formNames.Add(formNode.Text);
			}

			return formNames;
		}

		public object SelectedComponent
		{
			get
			{
				return (treeViewProjectComponents.SelectedNode != null ? treeViewProjectComponents.SelectedNode.Tag : null);
			}
		}

		private void highlightFormNode(IFormView formView)
		{
			treeViewProjectComponents.SelectedNode = getFormNodeReferencing(formView);
		}

		private bool isValidFormView(IFormView formView)
		{
			return getFormNodeReferencing(formView) != null;
		}

		public void AddDocument(IDocumentView documentView)
		{
			TreeNode node = new TreeNode(documentView.Presenter.Document.Name, imageDocument, imageDocument);
			node.Tag = documentView;
			node.ContextMenuStrip = contextMenuStripComponentNode;
			getDocumentsNode().Nodes.Add(node);
		}

		public bool CanPasteDocument
		{
			get { return treeViewProjectComponents.SelectedNode == getDocumentsNode() || isDocumentNode(treeViewProjectComponents.SelectedNode); }
		}

		public void CutDocument(string documentName)
		{
			removeDocument(documentName);
		}

		private void removeDocument(string name)
		{
			IDocumentView documentView = GetDocumentView(name);

			documentView.Close();

			TreeNode documentNode = getDocumentNode(name);
			documentNode.Remove();
		}

		public void PasteDocument(IDocumentView documentView)
		{
			AddDocument(documentView);
		}

		public void DeleteDocument(string documentName)
		{
			if (ConfirmDialog.ConfirmDelete("Document", documentName))
			{
				removeDocument(documentName);
			}
		}

		public void SelectDocument(IDocumentView documentView)
		{
			if (isValidDocumentView(documentView))
			{
				toolStripButtonStartPoint.Enabled = false;
				highlightDocumentNode(documentView);
			}
		}

		public void DeselectDocument()
		{
			highlightDocumentNode(null);
		}

		private bool isValidDocumentView(IDocumentView documentView)
		{
			return getDocumentNodeReferencing(documentView) != null;
		}

		private void highlightDocumentNode(IDocumentView documentView)
		{
			treeViewProjectComponents.SelectedNode = getDocumentNodeReferencing(documentView);
		}

		public void AddProcess(IProcessView processView)
		{
			TreeNode node = new TreeNode(processView.Presenter.Process.Name, imageProcess, imageProcess);
			node.Tag = processView;
			node.ContextMenuStrip = contextMenuStripComponentNode;
			getProcessesNode().Nodes.Add(node);
		}

		public bool CanPasteProcess
		{
			get { return treeViewProjectComponents.SelectedNode == getProcessesNode() || isProcessNode(treeViewProjectComponents.SelectedNode); }
		}

		public void CutProcess(string processName)
		{
			removeProcess(processName);
		}

		private void removeProcess(string processName)
		{
			IProcessView processView = GetProcessView(processName);

			processView.Close();

			TreeNode processNode = getProcessNode(processName);
			processNode.Remove();
		}

		public void PasteProcess(IProcessView processView)
		{
			AddProcess(processView);
		}

		public void DeleteProcess(string processName)
		{
			if (ConfirmDialog.ConfirmDelete("Process", processName))
			{
				removeProcess(processName);
			}
		}

		public void SelectProcess(IProcessView processView)
		{
			if (isValidProcessView(processView))
			{
				toolStripButtonStartPoint.Enabled = false;
				highlightProcessNode(processView);
			}
		}

		public void DeselectProcess()
		{
			highlightProcessNode(null);
		}

		public Collection<string> GetProcessNames()
		{
			Collection<string> processNames = new Collection<string>();

			foreach (TreeNode processNode in getProcessesNode().Nodes)
			{
				processNames.Add(processNode.Text);
			}

			return processNames;
		}

		public void ConnectPreProcess(IFormView formView, string processName)
		{
			TreeNode processNode = getProcessNode(processName);
			TreeNode connectedProcessNode = new TreeNode(processNode.Text, imageFormPreProcess, imageFormPreProcess);
			connectedProcessNode.Tag = processNode.Tag;

			TreeNode formNode = getFormNodeReferencing(formView);
			formNode.Nodes.Insert(0, connectedProcessNode);
		}

		public void DisconnectPreProcess(IFormView formView)
		{
			TreeNode formNode = getFormNodeReferencing(formView);

			int processNodeIndex = getPreProcessNodeIndex(formNode);
			formNode.Nodes[processNodeIndex].Remove();
		}

		public void ConnectPostProcess(IFormView formView, string processName)
		{
			TreeNode processNode = getProcessNode(processName);
			TreeNode connectedProcessNode = new TreeNode(processNode.Text, imageFormProcess, imageFormProcess);
			connectedProcessNode.Tag = processNode.Tag;

			TreeNode formNode = getFormNodeReferencing(formView);
			formNode.Nodes.Insert(1, connectedProcessNode);
		}

		public void DisconnectPostProcess(IFormView formView)
		{
			TreeNode formNode = getFormNodeReferencing(formView);

			int processNodeIndex = getPostProcessNodeIndex(formNode);
			formNode.Nodes[processNodeIndex].Remove();
		}

		private int getPreProcessNodeIndex(TreeNode formNode)
		{
			for (int i = 0; i < formNode.Nodes.Count; i++)
			{
				if (formNode.Nodes[i].ImageIndex == imageFormPreProcess)
				{
					return i;
				}
			}

			return -1;
		}

		private int getPostProcessNodeIndex(TreeNode formNode)
		{
			for (int i = 0; i < formNode.Nodes.Count; i++)
			{
				if (formNode.Nodes[i].ImageIndex == imageFormProcess)
				{
					return i;
				}
			}

			return -1;
		}

		private bool isValidProcessView(IProcessView processView)
		{
			return getProcessNodeReferencing(processView) != null;
		}

		private void highlightProcessNode(IProcessView processView)
		{
			treeViewProjectComponents.SelectedNode = getProcessNodeReferencing(processView);
		}

		public void OpenProjectFile(string projectFilePath)
		{
			if (!string.IsNullOrEmpty(projectFilePath))
			{
				Presenter.ProjectOpenRequested(projectFilePath);
			}
		}

		public void OpenProjectTemplateFile(string projectFilePath)
		{
			if (!string.IsNullOrEmpty(projectFilePath))
			{
				Presenter.ProjectTemplateOpenRequested(projectFilePath);
			}
		}

		public IProjectExplorerPresenter Presenter
		{
			get;
			private set;
		}

		public void SetStartingPoint(IFormView formView, bool isStartingPoint)
		{
			TreeNode formNode = getFormNodeReferencing(formView);

			if (formNode != null)
			{
				formNode.ImageIndex = formNode.SelectedImageIndex = getFormImageIndex(isStartingPoint);
			}
		}

		private int getFormImageIndex(bool isStartingPoint)
		{
			return (isStartingPoint ? imageFormIsStartPoint : imageForm);
		}

		private TreeNode getFormNodeReferencing(IFormView formView)
		{
			foreach (TreeNode node in getFormsNode().Nodes)
			{
				if (node.Tag == formView)
				{
					return node;
				}
			}

			return null;
		}

		private TreeNode getFormNode(string formName)
		{
			foreach (TreeNode node in getFormsNode().Nodes)
			{
				if (node.Text == formName)
				{
					return node;
				}
			}

			return null;
		}

		private TreeNode getDocumentNode(string documentName)
		{
			foreach (TreeNode node in getDocumentsNode().Nodes)
			{
				if (node.Text == documentName)
				{
					return node;
				}
			}

			return null;
		}

		public IFormView GetFormView(string formName)
		{
			foreach (TreeNode node in getFormsNode().Nodes)
			{
				IFormView formView = node.Tag as IFormView;

				if (formView.Presenter.Form.Name == formName)
				{
					return formView;
				}
			}

			return null;
		}

		public IDocumentView GetDocumentView(string documentName)
		{
			foreach (TreeNode node in getDocumentsNode().Nodes)
			{
				IDocumentView documentView = node.Tag as IDocumentView;

				if (documentView.Presenter.Document.Name == documentName)
				{
					return documentView;
				}
			}

			return null;
		}

		public IProcessView GetProcessView(string processName)
		{
			foreach (TreeNode node in getProcessesNode().Nodes)
			{
				IProcessView processView = node.Tag as IProcessView;

				if (processView.Presenter.Process.Name == processName)
				{
					return processView;
				}
			}

			return null;
		}

		private TreeNode getDocumentNodeReferencing(IDocumentView documentView)
		{
			foreach (TreeNode node in getDocumentsNode().Nodes)
			{
				if (node.Tag == documentView)
				{
					return node;
				}
			}

			return null;
		}

		private TreeNode getProcessNodeReferencing(IProcessView processView)
		{
			foreach (TreeNode node in getProcessesNode().Nodes)
			{
				if (node.Tag == processView)
				{
					return node;
				}
			}

			return null;
		}

		private TreeNode getProcessNode(string processName)
		{
			foreach (TreeNode node in getProcessesNode().Nodes)
			{
				if (node.Text == processName)
				{
					return node;
				}
			}

			return null;
		}

		#endregion

        private void startingPointToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.StartingPointToggleRequested();
		}

        private void dataEntryOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Presenter.PrePopulateToggleRequested();
        }

		private void connectPreProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

			if (menuItem != null)
			{
				Presenter.PreProcessConnectionRequested(treeViewProjectComponents.SelectedNode.Tag as IFormView, menuItem.Text);
			}
		}

		private void disconnectPreProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.PreProcessDisconnectionRequested(treeViewProjectComponents.SelectedNode.Tag as IFormView);
		}

		private void connectPostProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

			if (menuItem != null)
			{
				Presenter.PostProcessConnectionRequested(treeViewProjectComponents.SelectedNode.Tag as IFormView, menuItem.Text);
			}
		}

		private void disconnectPostProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.PostProcessDisconnectionRequested(treeViewProjectComponents.SelectedNode.Tag as IFormView);
		}

        private void contextMenuStripFormNode_Opening(object sender, CancelEventArgs e)
        {
            populateDataSourceNameItems();

            startingPointToolStripMenuItem.Checked = Presenter.SelectedFormIsStartingPoint;
            dataEntryOnlyToolStripMenuItem.Checked = Presenter.SelectedFormIsPrePopulated;
        }

        private void populateDataSourceNameItems()
        {
            dataSourceNameChanged = false;
            string dataSourceName = Presenter.GetFormDataSourceName ?? string.Empty;
            toolStripMenuItemDataSourceName.Checked = dataSourceName.Length > 0;
            toolStripTextBoxDataSourceName.Text = dataSourceName;
        }

        private void contextMenuStripFormNode_Opened(object sender, EventArgs e)
		{
			populateConnectPreProcessDropDown();
			enableOrDisablePreProcessConnectAndDisconnectMenuItems();

			populateConnectPostProcessDropDown();
			enableOrDisablePostProcessConnectAndDisconnectMenuItems();

        	toolStripMenuItemPaste.Enabled = Presenter.CanPasteForm;
		}

        private void populateConnectPreProcessDropDown()
		{
			connectPreProcessToolStripMenuItem.DropDownItems.Clear();

			foreach (TreeNode processNode in getProcessesNode().Nodes)
			{
				ToolStripItem processMenuItem = connectPreProcessToolStripMenuItem.DropDownItems.Add(processNode.Text);
				processMenuItem.Click += new EventHandler(connectPreProcessToolStripMenuItem_Click);
			}
		}

		private void populateConnectPostProcessDropDown()
		{
			connectPostProcessToolStripMenuItem.DropDownItems.Clear();

			foreach (TreeNode processNode in getProcessesNode().Nodes)
			{
				ToolStripItem processMenuItem = connectPostProcessToolStripMenuItem.DropDownItems.Add(processNode.Text);
				processMenuItem.Click += new EventHandler(connectPostProcessToolStripMenuItem_Click);
			}
		}

		private void enableOrDisablePreProcessConnectAndDisconnectMenuItems()
		{
			IFormView formView = treeViewProjectComponents.SelectedNode.Tag as IFormView;

			if (formView != null)
			{
				connectPreProcessToolStripMenuItem.Enabled = Presenter.CanConnectPreProcess(formView);
				disconnectPreProcessToolStripMenuItem.Enabled = Presenter.CanDisconnectPreProcess(formView);
			}
		}

		private void enableOrDisablePostProcessConnectAndDisconnectMenuItems()
		{
			IFormView formView = treeViewProjectComponents.SelectedNode.Tag as IFormView;

			if (formView != null)
			{
				connectPostProcessToolStripMenuItem.Enabled = Presenter.CanConnectPostProcess(formView);
				disconnectPostProcessToolStripMenuItem.Enabled = Presenter.CanDisconnectPostProcess(formView);
			}
		}

		private void treeViewProjectComponents_ItemDrag(object sender, ItemDragEventArgs e)
		{
			TreeNode draggedNode = e.Item as TreeNode;
			TreeNode processNode = getProcessNode(draggedNode.Text);

			if (processNode != null)
			{
				DoDragDrop(e.Item, DragDropEffects.Copy);
			}
		}

		private void treeViewProjectComponents_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.AllowedEffect;
		}

		private void treeViewProjectComponents_DragOver(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.None;

			Point targetPoint = treeViewProjectComponents.PointToClient(new Point(e.X, e.Y));
			TreeNode targetNode = treeViewProjectComponents.GetNodeAt(targetPoint);

			if (targetNode != null)
			{
				TreeNode formNode = getFormNode(targetNode.Text);

				if (formNode != null)
				{
					IFormView formView = GetFormView(targetNode.Text);

					if (Presenter.CanConnectPostProcess(formView))
					{
						e.Effect = e.AllowedEffect;
					}
				}
			}
		}

		private void treeViewProjectComponents_DragDrop(object sender, DragEventArgs e)
		{
			Point targetPoint = treeViewProjectComponents.PointToClient(new Point(e.X, e.Y));
			TreeNode targetNode = treeViewProjectComponents.GetNodeAt(targetPoint);

			if (targetNode != null)
			{
				TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

				if (e.Effect == DragDropEffects.Copy)
				{
					IFormView formView = GetFormView(targetNode.Text);

					if (Presenter.CanConnectPostProcess(formView))
					{
						string processName = draggedNode.Text;
						Presenter.PostProcessConnectionRequested(formView, processName);
					}
				}

				targetNode.Expand();
			}
		}

		private void treeViewProjectComponents_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			if (isRootNode(e.Node))
			{
				e.CancelEdit = true;
			}
		}

		private void treeViewProjectComponents_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			IFormView formView = e.Node.Tag as IFormView;

			if (formView != null)
			{
				if (!Presenter.FormRenameRequested(formView, e.Label))
				{
					e.CancelEdit = true;
				}
			}

			IProcessView processView = e.Node.Tag as IProcessView;

			if (processView != null)
			{
				if (!Presenter.ProcessRenameRequested(processView, e.Label))
				{
					e.CancelEdit = true;
				}
			}

			IDocumentView documentView = e.Node.Tag as IDocumentView;

			if (documentView != null)
			{
				if (!Presenter.DocumentRenameRequested(documentView, e.Label))
				{
					e.CancelEdit = true;
				}
			}
		}

		private void toolStripMenuItemRename_Click(object sender, EventArgs e)
		{
			treeViewProjectComponents.SelectedNode.BeginEdit();
		}

		private void toolStripMenuItemRenameComponentNode_Click(object sender, EventArgs e)
		{
			treeViewProjectComponents.SelectedNode.BeginEdit();
		}

		public void EditSelectedComponent()
		{
			treeViewProjectComponents.SelectedNode.BeginEdit();
		}

		public DialogResult ShowMessageBox(string messageText, string caption)
		{
			return MessageBox.Show (this, messageText, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
		}

		public SaveFileDialogResult ShowSaveFileDialog()
		{
			SaveFileDialog saveFileDialog = ApplicationPresenter.View.CreateSaveFileDialog();

			DialogResult dialogResult = saveFileDialog.ShowDialog(this);

			return new SaveFileDialogResult(dialogResult, saveFileDialog.FileName);
		}

		public bool ProjectExplorerHasFocus
		{
			get { return treeViewProjectComponents.Focused; }
		}

		private bool dataSourceNameChanged = false;

        private void toolStripTextBoxDataSourceName_TextChanged(object sender, EventArgs e)
        {
            dataSourceNameChanged = true;
        }

        private void toolStripMenuItemDataSourceName_DropDownClosed(object sender, EventArgs e)
        {
            if (dataSourceNameChanged)
            {
			    IFormView formView = treeViewProjectComponents.SelectedNode.Tag as IFormView;
                Presenter.RequestChangeDataSourceName(formView, toolStripTextBoxDataSourceName.Text);
                dataSourceNameChanged = false;
            }
        }

        private void toolStripTextBoxDataSourceName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                contextMenuStripFormNode.Close();
            }
        }

		private void toolStripMenuItemCut_Click(object sender, EventArgs e)
		{
		    IFormView formView = treeViewProjectComponents.SelectedNode.Tag as IFormView;
			Presenter.FormCutRequested(formView);
		}

		private void toolStripMenuItemCopy_Click(object sender, EventArgs e)
		{
			IFormView formView = treeViewProjectComponents.SelectedNode.Tag as IFormView;
			Presenter.FormCopyRequested(formView);
		}

		private void toolStripMenuItemPaste_Click(object sender, EventArgs e)
		{
			if (isFormNode(treeViewProjectComponents.SelectedNode))
			{
				Presenter.FormPasteRequested();
			}
		}

		private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
		{
			IFormView formView = treeViewProjectComponents.SelectedNode.Tag as IFormView;
			Presenter.FormDeleteRequested(formView);
		}

		private void toolStripMenuItemPasteRootNodes_Click(object sender, EventArgs e)
		{
			Presenter.ComponentPasteRequested();
		}

		private void toolStripMenuItemCutComponentNode_Click(object sender, EventArgs e)
		{
			IDocumentView documentView = treeViewProjectComponents.SelectedNode.Tag as IDocumentView;

			if (documentView != null)
			{
				Presenter.DocumentCutRequested(documentView);
			}

			IProcessView processView = treeViewProjectComponents.SelectedNode.Tag as IProcessView;

			if (processView != null)
			{
				Presenter.ProcessCutRequested(processView);
			}
		}

		private void toolStripMenuItemCopyComponentNode_Click(object sender, EventArgs e)
		{
			IDocumentView documentView = treeViewProjectComponents.SelectedNode.Tag as IDocumentView;

			if (documentView != null)
			{
				Presenter.DocumentCopyRequested(documentView);
			}

			IProcessView processView = treeViewProjectComponents.SelectedNode.Tag as IProcessView;

			if (processView != null)
			{
				Presenter.ProcessCopyRequested(processView);
			}
		}

		private void toolStripMenuItemPasteComponentNode_Click(object sender, EventArgs e)
		{
			if (isDocumentNode(treeViewProjectComponents.SelectedNode))
			{
				Presenter.DocumentPasteRequested();
			}
			else if (isProcessNode(treeViewProjectComponents.SelectedNode))
			{
				Presenter.ProcessPasteRequested();
			}

		}

		private void toolStripMenuItemDeleteComponentNode_Click(object sender, EventArgs e)
		{
			if (treeViewProjectComponents.SelectedNode != null)
			{
				IDocumentView documentView = treeViewProjectComponents.SelectedNode.Tag as IDocumentView;

				if (documentView != null)
				{
					Presenter.DocumentDeleteRequested(documentView);
				}

				IProcessView processView = treeViewProjectComponents.SelectedNode.Tag as IProcessView;

				if (processView != null)
				{
					Presenter.ProcessDeleteRequested(processView);
				}
			}
		}

		private void contextMenuStripComponentNode_Opened(object sender, EventArgs e)
		{
			toolStripMenuItemPasteComponentNode.Enabled = Presenter.CanPasteDocument || Presenter.CanPasteProcess;
		}

		private void contextMenuStripRootNodes_Opened(object sender, EventArgs e)
		{
			toolStripMenuItemPasteRootNodes.Enabled = Presenter.CanPasteForm || Presenter.CanPasteDocument || Presenter.CanPasteProcess;
		}

		private void treeViewProjectComponents_MouseDown(object sender, MouseEventArgs e)
		{
			selectTreeNodeOnRightMouseDown(e);
		}

		private void selectTreeNodeOnRightMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				treeViewProjectComponents.SelectedNode = treeViewProjectComponents.GetNodeAt(e.X, e.Y);
			}
		}
	}
}
