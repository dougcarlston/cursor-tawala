// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.DesignerUI.Properties;
using Tawala.Projects;
using Tawala.Projects.Components;
using Tawala.Projects.Documents;
using Tawala.ProjectUI;
using Form=Tawala.Projects.Form;
using Process=Tawala.Projects.Processes.Process;

namespace Tawala.DesignerUI
{
    internal sealed partial class ProjectExplorer : UserControl, IEditMenu, IProjectExplorerView
    {
        private const int imageFolderClosed = 8;
        private const int imageFolderOpen = 9;
        private const int imageFormPreProcess = 12;
        private const int imageFormProcess = 13;

        // These are initialized in ProjectPane_Load.  

        private readonly FolderNode documentsNode;
        private readonly FolderNode formsNode;

        private readonly int initialWidth;
        private readonly FolderNode processesNode;

        private bool inLabelEditMode;

        private ProjectPresenter presenter;

        public ProjectExplorer()
        {
            // This call is required by the Windows.Forms Form DesignerUI.
            InitializeComponent();

            presenter = new ProjectPresenter(this);

            initialWidth = Width;

            Project.Events.ComponentAdded += project_ComponentAdded;
            Project.Events.ComponentRemoved += project_ComponentRemoved;
            Project.Events.ComponentRenamed += project_ComponentRenamed;

            Project.Events.NewProject += project_NewProject;
            Project.Events.ProjectOpened += project_ProjectOpened;
            Project.Events.ProcessConnectedToForm += project_ProcessConnectedToForm;
            Project.Events.PreProcessConnectedToForm += project_PreProcessConnectedToForm;
            Project.Events.ProcessDisconnectedFromForm += project_ProcessDisconnectedFromForm;
            Project.Events.PreProcessDisconnectedFromForm += project_PreProcessDisconnectedFromForm;

            Project.Events.FormChanged += project_FormChanged;

            tree.ImageList = buildImageList();

            documentsNode = new FolderNode("Documents", imageFolderClosed);
            formsNode = new FolderNode("Forms", imageFolderClosed);
            processesNode = new FolderNode("Processes", imageFolderClosed);

            DoubleBuffered = true;
        }

        private FormNode SelectedNodeAsFormNode { get { return tree.SelectedNode as FormNode; } }

        #region IProjectExplorerView Members

        ProjectNode IProjectExplorerView.SelectedProjectNode { get { return tree.SelectedNode as ProjectNode; } }

        #endregion

        /// <summary>
        /// Clear all children of the folders in the project tree.
        /// </summary>
        private void clearProjectTree()
        {
            foreach (TreeNode node in tree.Nodes)
            {
                node.Collapse();
                node.Nodes.Clear();
                setClosedFolderIcons(node);
            }

            tree.SelectedNode = formsNode;
        }

        private static void setClosedFolderIcons(TreeNode node)
        {
            node.ImageIndex = node.SelectedImageIndex = imageFolderClosed;
        }

        private bool isComponentFolderNode(TreeNode node)
        {
            return node == null ? false : node == formsNode || node == processesNode || node == documentsNode;
        }

        private static bool isComponentNode(TreeNode node)
        {
            return isFormNode(node) || isProcessNode(node) || isDocumentNode(node);
        }

        private static bool isFormNode(TreeNode node)
        {
            return (node is FormNode);
        }

        private static bool isProcessNode(TreeNode node)
        {
            return (node is ProcessNode);
        }

        private static bool isDocumentNode(TreeNode node)
        {
            return (node is DocumentNode);
        }

        private static bool isConnectedProcessNode(TreeNode node)
        {
            return (node is ConnectedProcessNode);
        }

        private static bool isConnectedPreProcessNode(TreeNode node)
        {
            return (node is ConnectedPreProcessNode);
        }

        private static TreeNode findTreeNodeByName(TreeNode topLevelNode, string name)
        {
            for (int i = 0; i < topLevelNode.Nodes.Count; ++i)
            {
                if (topLevelNode.Nodes[i].Text == name)
                {
                    return topLevelNode.Nodes[i];
                }
            }

            return null;
        }

        private void sizeToWidth()
        {
            tree.ExpandAll();
            Width = tree.GetIdealWidth(tree.Nodes, initialWidth - Padding.Right);
        }

        /// <summary>
        /// Sets the component associated with the selected tree node as current component.
        /// Activates and displays that component's MDI child window, restoring it if minimized.
        /// </summary>
        private void activateSelectedComponent()
        {
            if (tree.SelectedNode != null)
            {
                if (tree.SelectedNode is FolderNode)
                {
                    Project.Current.SetCurrentComponent(null);
                }
                else
                {
                    var component = tree.SelectedNode.Tag as IProjectComponent;

                    if (isConnectedProcessNode(tree.SelectedNode))
                    {
                        component = (tree.SelectedNode.Tag as Form).ConnectedPostProcess;
                    }
                    else if (isConnectedPreProcessNode(tree.SelectedNode))
                    {
                        component = (tree.SelectedNode.Tag as Form).ConnectedPreProcess;
                    }

                    if (Project.Current.CurrentComponent != component)
                    {
                        Project.Current.SetCurrentComponent(component);
                    }

                    foreach (MDIComponentView mdiForm in ParentForm.MdiChildren)
                    {
                        if (mdiForm.Tag == component)
                        {
                            mdiForm.Activate();

                            if (mdiForm.WindowState == FormWindowState.Minimized)
                            {
                                mdiForm.WindowState = FormWindowState.Normal;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void connectPreProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;

            if (menuItem != null)
            {
                var processNode = findTreeNodeByName(processesNode, menuItem.Text) as ProcessNode;
                SelectedNodeAsFormNode.ConnectPreProcess(processNode);
            }
        }

        private void disconnectPreProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedNodeAsFormNode.DisconnectPreProcess();
        }

        private void toolStripTextBoxDataSourceName_TextChanged(object sender, EventArgs e)
        {
            bool isNamed = validDataSourceNameEntered();

            string dataSourceName = isNamed ? toolStripTextBoxDataSourceName.Text : string.Empty;
            SelectedNodeAsFormNode.DataSourceName = dataSourceName;

            dataSourceNameToolStripMenuItem.Checked = isNamed;
        }

        private bool validDataSourceNameEntered()
        {
            return (toolStripTextBoxDataSourceName.Text != string.Empty) &&
                   (toolStripTextBoxDataSourceName.Text != Resources.ResourceManager.GetString("DefaultDataSourceNameText"));
        }

        private void toolStripTextBoxDataSourceName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                componentContextMenuStrip.Hide();
            }
        }

        private ImageList buildImageList()
        {
            var list = new ImageList();
            list.ColorDepth = ColorDepth.Depth32Bit;
            list.ImageSize = new Size(20, 16);

            list.Images.Add(Resources.Form_InTree); // 0

            var compositor = new ImageCompositor(Resources.Form_InTree);

            list.Images.Add(compositor.Render(Resources.Form_IsStartPoint_Overlay)); // 1
            list.Images.Add(compositor.Render(Resources.Form_IsPrepopulated_Overlay)); // 2
            list.Images.Add(compositor.Render(Resources.Form_IsPrepopulated_Overlay, Resources.Form_IsStartPoint_Overlay)); // 3
            list.Images.Add(compositor.Render(Resources.Form_BlockBackButton_Overlay)); // 4
            list.Images.Add(compositor.Render(Resources.Form_BlockBackButton_Overlay, Resources.Form_IsStartPoint_Overlay)); // 5
            list.Images.Add(compositor.Render(Resources.Form_BlockBackButton_Overlay, Resources.Form_IsPrepopulated_Overlay)); // 6
            list.Images.Add(compositor.Render(Resources.Form_BlockBackButton_Overlay, Resources.Form_IsPrepopulated_Overlay,
                                              Resources.Form_IsStartPoint_Overlay)); // 7

            list.Images.Add(Resources.Folder_Closed); // 8
            list.Images.Add(Resources.Folder_Open); // 9
            list.Images.Add(Resources.Document_InTree); // 10
            list.Images.Add(Resources.Process_InTree); // 11
            list.Images.Add(Resources.Form_PreProcess); // 12
            list.Images.Add(Resources.Form_PostProcess); // 13

            return list;
        }

        #region Project Explorer Events

        // The following three methods will draw a rectangle and allow 
        // the user to use the mouse to resize the rectangle.  If the 
        // rectangle intersects a control's client rectangle, the 
        // control's color will change.

        private bool isDrag;
        private Point lastMousePos;
        private Rectangle theRectangle;

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (Cursor != Cursors.Default)
            {
                Cursor = Cursors.Default;
            }
        }

        private bool isMouseOverResizeBar()
        {
            if (MouseButtons == MouseButtons.None || MouseButtons == MouseButtons.Left)
            {
                Point ptClient = PointToClient(MousePosition);
                return ptClient.X >= Right - Padding.Right && ptClient.X < Right - 1;
            }
            return false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (isMouseOverResizeBar())
            {
                isDrag = true;
                Cursor = Cursors.SizeWE;
                theRectangle = RectangleToScreen(new Rectangle(Right - Padding.Right, 0, Padding.Right, Height));
                Invalidate(RectangleToClient(theRectangle));
                lastMousePos = MousePosition;
            }
            else
            {
                isDrag = false;
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isMouseOverResizeBar())
            {
                Cursor = Cursors.SizeWE;
            }

            if (isDrag)
            {
                Point mp = MousePosition;
                int changeX = mp.X - lastMousePos.X;

                if (Width + changeX > 130 && Width + changeX < Parent.Width - 40)
                {
                    theRectangle = RectangleToScreen(new Rectangle(Right - Padding.Right, 0, Padding.Right, Height));
                    theRectangle.Offset(changeX, 0);

                    Width += changeX;
                    Invalidate(true);
                    Update();
                }

                lastMousePos = mp;
            }
            else
            {
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isDrag)
            {
                isDrag = false;
                Cursor = Cursors.Default;
                Invalidate(RectangleToClient(theRectangle));
            }
            else
            {
                base.OnMouseUp(e);
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            tree.Left = 0;
            tree.Top = toolStrip.Bottom + 1;
            tree.Width = Width - Padding.Right;
            tree.Height = Height - label.Height - toolStrip.Height - 1;

            base.OnLayout(e);
        }

        /// <summary>
        /// The top level folder nodes for the tree are setup here rather than in VSN's DesignerUI.
        /// This avoids a bug where the horizontal scrollbar appears when it shouldn't.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            tree.BeginUpdate();
            tree.Nodes.Add(formsNode);
            tree.Nodes.Add(processesNode);
            tree.Nodes.Add(documentsNode);
            tree.EndUpdate();

            sizeToWidth();

            formsNode.ContextMenuStrip = folderContextMenuStrip;
            processesNode.ContextMenuStrip = folderContextMenuStrip;
            documentsNode.ContextMenuStrip = folderContextMenuStrip;
        }

        /// <summary>
        /// OnPaint - Draw border on right side
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Pen border = isDrag ? Pens.DarkGray : Pens.Gray;
            Pen interior = isDrag ? Pens.Gray : Pens.LightGray;

            // Draw right border for sizinging and separation

            e.Graphics.DrawLine(border, Right - Padding.Right, 0, Right - Padding.Right, Height);
            for (int i = Padding.Right - 1; i > 1; --i)
            {
                e.Graphics.DrawLine(interior, Right - i, 0, Right - i, Height);
            }
            e.Graphics.DrawLine(border, Right - 1, 0, Right - 1, Height);
        }

        #endregion

        #region Project Events

        private void project_ComponentAdded(object sender, ComponentEventArgs e)
        {
            addComponentNode(findParentNode(e.Component), makeComponentNode(e.Component), true);
        }

        private TreeNode findParentNode(IProjectComponent component)
        {
            if (component is Form)
            {
                return (formsNode);
            }
            if (component is RtfDocument)
            {
                return (documentsNode);
            }
            if (component is Process)
            {
                return (processesNode);
            }

            return null;
        }

        /// <summary>
        /// Factory method to create a component node from the specified component.
        /// </summary>
        private ComponentNode makeComponentNode(IProjectComponent component)
        {
            ComponentNode componentNode = null;

            if (component is Form)
            {
                componentNode = new FormNode(component as Form);
            }
            else if (component is RtfDocument)
            {
                componentNode = new DocumentNode(component as RtfDocument);
            }
            else if (component is Process)
            {
                componentNode = new ProcessNode(component as Process);
            }

            return componentNode;
        }

        private void addFormNode(TreeNode parentNode, Form form, bool ExpandSelect)
        {
            addComponentNode(parentNode, new FormNode(form), ExpandSelect);
        }

        private void addProcessNode(TreeNode parentNode, Process process, bool ExpandSelect)
        {
            addComponentNode(parentNode, new ProcessNode(process), ExpandSelect);
        }

        private void addDocumentNode(TreeNode parentNode, RtfDocument document, bool ExpandSelect)
        {
            addComponentNode(parentNode, new DocumentNode(document), ExpandSelect);
        }

        private void addComponentNode(TreeNode parentNode, TreeNode node, bool ExpandSelect)
        {
            node.ContextMenuStrip = componentContextMenuStrip;
            parentNode.Nodes.Add(node);

            if (ExpandSelect)
            {
                parentNode.Expand();
                tree.SelectedNode = node;
            }
        }

        private void insertComponentNode(int index, TreeNode parentNode, TreeNode node, bool ExpandSelect)
        {
            node.ContextMenuStrip = componentContextMenuStrip;
            parentNode.Nodes.Insert(index, node);

            if (ExpandSelect)
            {
                parentNode.Expand();
                tree.SelectedNode = node;
            }
        }

        private void project_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            if (e.Component is Form)
            {
                formRemovedFromProject(e);
            }
            else if (e.Component is RtfDocument)
            {
                documentRemovedFromProject(e);
            }
            else if (e.Component is Process)
            {
                processRemovedFromProject(e);
            }

            collapseChildlessNodes();
        }

        private void processRemovedFromProject(ComponentEventArgs e)
        {
            foreach (TreeNode processNode in processesNode.Nodes)
            {
                if (processNode.Text == e.Component.Name)
                {
                    processNode.Remove();
                    break;
                }
            }

            foreach (TreeNode formNode in formsNode.Nodes)
            {
                foreach (TreeNode processNode in formNode.Nodes)
                {
                    if (processNode.Text == e.Component.Name)
                    {
                        if (processNode is ConnectedProcessNode)
                        {
                            (processNode as ProcessNode).DisconnectProcess();
                        }
                        else if (processNode is ConnectedPreProcessNode)
                        {
                            (processNode as ProcessNode).DisconnectPreProcess();
                        }
                    }
                }
            }
        }

        private void documentRemovedFromProject(ComponentEventArgs e)
        {
            foreach (TreeNode child in documentsNode.Nodes)
            {
                if (child.Text == e.Component.Name)
                {
                    child.Remove();
                    break;
                }
            }
        }

        private void formRemovedFromProject(ComponentEventArgs e)
        {
            foreach (FormNode formNode in formsNode.Nodes)
            {
                if (formNode.Text == e.Component.Name)
                {
                    foreach (TreeNode processNode in formNode.Nodes)
                    {
                        if (processNode is ConnectedProcessNode)
                        {
                            (processNode as ProcessNode).DisconnectProcess();
                        }
                        else if (processNode is ConnectedPreProcessNode)
                        {
                            (processNode as ProcessNode).DisconnectPreProcess();
                        }
                    }

                    formNode.Remove();
                    break;
                }
            }
        }

        private void collapseChildlessNodes()
        {
            foreach (TreeNode topLevelNode in tree.Nodes)
            {
                if (topLevelNode.Nodes.Count == 0)
                {
                    topLevelNode.Collapse();
                }
            }
        }

        private void project_ComponentRenamed(object sender, ComponentRenamedEventArgs e)
        {
            if (e.Component is Process)
            {
                foreach (TreeNode formNode in formsNode.Nodes)
                {
                    foreach (TreeNode processNode in formNode.Nodes)
                    {
                        if (processNode.Text == e.OldName)
                        {
                            processNode.Text = e.Component.Name;
                        }
                    }
                }

                foreach (TreeNode processNode in processesNode.Nodes)
                {
                    if (processNode.Text == e.OldName)
                    {
                        processNode.Text = e.Component.Name;
                    }
                }
            }
        }

        private void project_NewProject(object sender, ProjectEventArgs e)
        {
            clearProjectTree();
        }

        /// <summary>
        /// Populate the tree with all the project components
        /// and form-process associations.
        /// </summary>
        private void project_ProjectOpened(object sender, ProjectEventArgs e)
        {
            clearProjectTree();

            addFormNodes();
            addProcessNodes();
            addDocumentNodes();
            addConnectedProcessNodes();
            addConnectedPreProcessNodes();

            tree.ExpandAll();
        }

        private void addFormNodes()
        {
            foreach (Form form in Project.Current.FormList)
            {
                addFormNode(formsNode, form, false);
            }
        }

        private void addProcessNodes()
        {
            foreach (Process process in Project.Current.ProcessList)
            {
                addProcessNode(processesNode, process, false);
            }
        }

        private void addDocumentNodes()
        {
            foreach (RtfDocument document in Project.Current.DocumentList)
            {
                addDocumentNode(documentsNode, document, false);
            }
        }

        private void addConnectedProcessNodes()
        {
            foreach (Form form in Project.Current.FormList)
            {
                if (form.HasConnectedProcess)
                {
                    foreach (TreeNode formNode in formsNode.Nodes)
                    {
                        if (formNode.Tag == form)
                        {
                            foreach (TreeNode processNode in processesNode.Nodes)
                            {
                                if (processNode.Tag == form.ConnectedPostProcess)
                                {
                                    var connectedProcessNode = new ConnectedProcessNode(form, imageFormProcess);
                                    addComponentNode(formNode, connectedProcessNode, false);
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }

        private void addConnectedPreProcessNodes()
        {
            foreach (Form form in Project.Current.FormList)
            {
                if (form.HasConnectedPreProcess)
                {
                    foreach (TreeNode formNode in formsNode.Nodes)
                    {
                        if (formNode.Tag == form)
                        {
                            foreach (TreeNode processNode in processesNode.Nodes)
                            {
                                if (processNode.Tag == form.ConnectedPreProcess)
                                {
                                    var connectedPreProcessNode = new ConnectedPreProcessNode(form, imageFormPreProcess);
                                    insertComponentNode(0, formNode, connectedPreProcessNode, false);
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create connected Process tree node when Process is connected to form.
        /// </summary>
        private void project_ProcessConnectedToForm(object sender, ProcessConnectionArgs e)
        {
            TreeNode formNode = findTreeNodeByName(formsNode, e.ConnectedForm.Name);

            if (formNode != null)
            {
                TreeNode connectedProcessNode = new ConnectedProcessNode(e.ConnectedForm, imageFormProcess);
                addComponentNode(formNode, connectedProcessNode, true);
            }
        }

        /// <summary>
        /// Create connected PreProcess tree node when PreProcess is connected to form.
        /// </summary>
        private void project_PreProcessConnectedToForm(object sender, ProcessConnectionArgs e)
        {
            TreeNode formNode = findTreeNodeByName(formsNode, e.ConnectedForm.Name);

            if (formNode != null)
            {
                TreeNode connectedPreProcessNode = new ConnectedPreProcessNode(e.ConnectedForm, imageFormPreProcess);
                insertComponentNode(0, formNode, connectedPreProcessNode, true);
            }
        }

        /// <summary>
        /// Remove connected Process tree node on disconnect.
        /// </summary>
        private void project_ProcessDisconnectedFromForm(object sender, ProcessConnectionArgs e)
        {
            var formNode = findTreeNodeByName(formsNode, e.ConnectedForm.Name) as FormNode;

            if (formNode != null)
            {
                TreeNode processNode = findTreeNodeByName(processesNode, e.ConnectedProcess.Name);

                if (processNode != null)
                {
                    tree.SelectedNode = processNode;
                }

                formNode.RemoveConnectedProcessNode();
            }
        }

        /// <summary>
        /// Remove connected PreProcess tree node on disconnect.
        /// </summary>
        private void project_PreProcessDisconnectedFromForm(object sender, ProcessConnectionArgs e)
        {
            var formNode = findTreeNodeByName(formsNode, e.ConnectedForm.Name) as FormNode;

            if (formNode != null)
            {
                TreeNode processNode = findTreeNodeByName(processesNode, e.ConnectedProcess.Name);

                if (processNode != null)
                {
                    tree.SelectedNode = processNode;
                }

                formNode.RemoveConnectedPreProcessNode();
            }
        }

        private void project_FormChanged(object sender, ComponentEventArgs e)
        {
            var form = e.Component as Form;

            foreach (TreeNode node in formsNode.Nodes)
            {
                if (node.Tag == form)
                {
                    int imageIndex = FormNode.GetFormIconImageIndex(form);

                    if (node.ImageIndex != imageIndex)
                    {
                        node.ImageIndex = imageIndex;
                        node.SelectedImageIndex = imageIndex;
                    }

                    if (tree.SelectedNode == node)
                    {
                        startPointToolStripButton.Checked = form.StartingPoint;
                        blockBackButtonToolStripButton.Checked = form.BlockBackButton;
                    }
                    break;
                }
            }
        }

        #endregion

        #region Tree events

        /// <summary> 
        /// When a root node is collapsed, change its image to a closed folder
        /// </summary>
        private void tree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node is FolderNode)
            {
                e.Node.ImageIndex = e.Node.SelectedImageIndex = imageFolderClosed;
            }
        }

        /// <summary> 
        /// When a root node is expanded, change its image to an open folder.
        /// Note: recently made the two images the same but we may want to change them again.
        /// </summary>
        private void tree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node is FolderNode)
            {
                e.Node.ImageIndex = e.Node.SelectedImageIndex = imageFolderOpen;
            }
        }

        /// <summary>
        /// After user has edited a tree node label let the Project know that the associated project item name has changed.
        /// NOTE: This code assumes anything being renamed is a Form Project Item at the moment.
        /// </summary>
        private void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            inLabelEditMode = false;

            if (escapePressed(e))
            {
                return;
            }

            string trimmedLabel = e.Label.Trim();

            if (trimmedLabel.Length == 0)
            {
                e.CancelEdit = true;
                return;
            }

            e.CancelEdit = (e.Node as ComponentNode).Rename(trimmedLabel);
        }

        private static bool escapePressed(NodeLabelEditEventArgs e)
        {
            return (e.Label == null);
        }

        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (isFormNode(tree.SelectedNode))
            {
                var form = tree.SelectedNode.Tag as Form;

                showFormSpecificItems(form, true);
            }
            else
            {
                showFormSpecificItems(null, false);
            }

            updateMoveNodeButtonsStatus();
        }

        private void showFormSpecificItems(Form form, bool show)
        {
            if (startPointToolStripButton.Visible != show)
            {
                startPointToolStripButton.Visible = show;
                blockBackButtonToolStripButton.Visible = show;

                startingPointToolStripMenuItem.Visible = show;
                blockBackButtonToolStripMenuItem.Visible = show;

                startPointToolStripButton.Enabled = show;
                blockBackButtonToolStripButton.Enabled = show;
            }

            if (form != null)
            {
                startPointToolStripButton.Checked = form.StartingPoint;
                blockBackButtonToolStripButton.Checked = form.BlockBackButton;
            }
        }

        private void updateMoveNodeButtonsStatus()
        {
            bool enableMoveNode = !isComponentFolderNode(tree.SelectedNode) && tree.SelectedNode.Parent.Nodes.Count > 1;

            nodeMoveDownToolStripButton.Enabled = enableMoveNode;
            nodeMoveUpToolStripButton.Enabled = enableMoveNode;
        }

        /// <summary>
        /// If the user is trying to edit a tree node label, cancel the edit if the node is a root folder.
        /// </summary>
        private void tree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            inLabelEditMode = true;

            if (e.Node is FolderNode)
            {
                e.CancelEdit = true;
                inLabelEditMode = false;
            }
        }

        /// <summary>
        /// The user has started to drag a tree item.  See if its a process.
        /// </summary>
        private void tree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var node = e.Item as TreeNode;
                if (node != null && node.Parent == processesNode)
                {
                    tree.SelectedNode = node;
                    DoDragDrop(node, DragDropEffects.Link);
                }
            }
        }

        /// <summary>
        /// If a drag was initiated then allow it.
        /// </summary>
        private void tree_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        /// <summary>
        /// As the drag continue update the effect based on whether the drop target is appropriate or not.
        /// If over forms folders and it is collapsed then expand it.
        /// </summary>
        private void tree_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            if (draggingProcessNode(e))
            {
                TreeNode dragNode = e.Data.GetData(typeof(ProcessNode)) as ProcessNode;

                Point pt = tree.PointToClient(new Point(e.X, e.Y));

                var dropNode = tree.GetNodeAt(pt) as FormNode;

                if (dropNode != null)
                {
                    if (!dropNode.HasConnectedProcess)
                    {
                        e.Effect = e.AllowedEffect;
                    }
                }

                var folderNode = tree.GetNodeAt(pt) as FolderNode;

                if (folderNode == formsNode && !formsNode.IsExpanded)
                {
                    formsNode.Expand();
                }

                tree.SelectedNode = dragNode;
            }
        }

        /// <summary>
        /// On Enter key, set current component to selected item
        /// </summary>
        private void tree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                activateSelectedComponent();
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.Down && e.Control)
            {
                nodeMoveDownToolStripButton.PerformClick();
            }

            if (e.KeyCode == Keys.Up && e.Control)
            {
                nodeMoveUpToolStripButton.PerformClick();
            }
        }

        private void tree_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            if (draggingProcessNode(e))
            {
                var dragNode = e.Data.GetData(typeof(ProcessNode)) as ProcessNode;

                Point pt = tree.PointToClient(new Point(e.X, e.Y));

                var dropNode = tree.GetNodeAt(pt) as FormNode;

                if (!dropNode.HasConnectedProcess)
                {
                    dropNode.ConnectProcess(dragNode);
                    e.Effect = e.AllowedEffect;
                }
            }
        }

        private static bool draggingProcessNode(DragEventArgs e)
        {
            return e.Data.GetDataPresent(typeof(ProcessNode));
        }

        /// <summary>
        /// Regardless of which mouse button is clicked, select the tree node.
        /// Otherwise on right click, context menu handlers never see the temporary selection of a right clicked node
        /// when it differs from the SelectedNode.
        /// </summary>
        private void tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var pt = new Point(e.X, e.Y);
            TreeViewHitTestLocations loc = tree.HitTest(pt).Location;
            if (loc != TreeViewHitTestLocations.PlusMinus)
            {
                tree.SelectedNode = e.Node;

                // however, we don't necessarily need to display the component
                if (e.Button == MouseButtons.Left)
                {
                    activateSelectedComponent();
                }
            }

            // Workaround for edit menu bugs
            tree.Focus();
        }

        #endregion

        #region Context Menu Item Event Handlers

        /// <summary>
        /// When the folderContextMenu's Add New item is clicked tell the Project to create and add a new component.
        /// </summary>
        private void folderMenuItemAddNew_Click(object sender, EventArgs e)
        {
            if (tree.SelectedNode == documentsNode)
            {
                Project.Current.AddDocument();
            }
            else if (tree.SelectedNode == formsNode)
            {
                Project.Current.AddForm();
            }
            else if (tree.SelectedNode == processesNode)
            {
                Project.Current.AddProcess();
            }
        }

        /// <summary>
        /// Paste the item on the clipboard into the folder.
        /// The paste context menu item should've only been enabled for specified folder
        /// if an object of the appropriate type was present on the clipboard.
        /// NOTE: Handler also used by Paste on the itemContextMenu.
        /// </summary>
        private void folderMenuItemPaste_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Paste();
        }

        private void folderMenuItemCollapseChildNodes_Click(object sender, EventArgs e)
        {
            if (tree.SelectedNode != null)
            {
                if (tree.SelectedNode is FolderNode)
                {
                    foreach (TreeNode node in tree.SelectedNode.Nodes)
                    {
                        node.Collapse();
                    }
                }
            }
        }

        private void folderMenuItemExpandChildNodes_Click(object sender, EventArgs e)
        {
            if (tree.SelectedNode != null)
            {
                if (tree.SelectedNode is FolderNode)
                {
                    foreach (TreeNode node in tree.SelectedNode.Nodes)
                    {
                        node.Expand();
                    }
                }
            }
        }

        /// <summary>
        /// Cut a project component (item) and put it on the clipboard.
        /// Actually removes it from the project.
        /// </summary>
        private void itemMenuItemCut_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Cut();
        }

        /// <summary>
        /// Copy a project component (item) to the clipboard.
        /// </summary>
        private void itemMenuItemCopy_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Copy();
        }

        private void itemMenuItemDelete_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Delete();
        }

        /// <summary>
        /// Handler for the itemContextMenu's Rename menuitem
        /// </summary>
        private void itemMenuItemRename_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Rename();
        }

        /// <summary>
        /// Find the process by the menu item text and connect it to the form which is should be the tree.SelectedNode
        /// </summary>
        private void itemMenuItemFormConnect_Click(object sender, EventArgs e)
        {
            var menu = sender as ToolStripMenuItem;

            Debug.Assert(tree.SelectedNode != null);
            Debug.Assert(menu != null);
            Debug.Assert(isFormNode(tree.SelectedNode));

            if (menu != null)
            {
                foreach (ProcessNode processNode in processesNode.Nodes)
                {
                    if (processNode.Text == menu.Text)
                    {
                        SelectedNodeAsFormNode.ConnectProcess(processNode);
                        break;
                    }
                }
            }
        }

        private void itemMenuItemFormDisconnect_Click(object sender, EventArgs e)
        {
            SelectedNodeAsFormNode.DisconnectProcess();
        }

        /// <summary>
        /// When the Folder Context Menu is about to be displayed, set state of menu items.
        /// </summary>
        private void folderContextMenu_Popup(object sender, EventArgs e)
        {
            folderMenuItemAddNew.Image = null;

            if (tree.SelectedNode == formsNode)
            {
                folderMenuItemAddNew.Image = Resources.Form_New;
            }
            else if (tree.SelectedNode == processesNode)
            {
                folderMenuItemAddNew.Image = Resources.Process_New;
            }
            else if (tree.SelectedNode == documentsNode)
            {
                folderMenuItemAddNew.Image = Resources.Document_New;
            }

            folderMenuItemAddNew.ImageTransparentColor = Color.Magenta;

            folderMenuItemPaste.Enabled = ((IEditMenu)this).CanPaste();

            if (selectedNodeIsFormsFolder())
            {
                folderMenuChildNodesSeparator.Visible = true;

                folderMenuItemCollapseChildNodes.Visible = true;
                folderMenuItemCollapseChildNodes.Enabled = anyChildNodesExpanded(tree.SelectedNode);

                folderMenuItemExpandChildNodes.Visible = true;
                folderMenuItemExpandChildNodes.Enabled = anyChildNodesCollapsed(tree.SelectedNode);
            }
            else
            {
                folderMenuChildNodesSeparator.Visible = false;
                folderMenuItemCollapseChildNodes.Visible = false;
                folderMenuItemExpandChildNodes.Visible = false;
            }
        }

        private bool selectedNodeIsFormsFolder()
        {
            return (tree.SelectedNode is FolderNode) && (tree.SelectedNode.Text == "Forms");
        }

        private static bool anyChildNodesExpanded(TreeNode parentNode)
        {
            foreach (TreeNode node in parentNode.Nodes)
            {
                if (node.Nodes.Count > 0 && node.IsExpanded)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool anyChildNodesCollapsed(TreeNode parentNode)
        {
            foreach (TreeNode node in parentNode.Nodes)
            {
                if (node.Nodes.Count > 0 && !node.IsExpanded)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// When the Item Context Menu is about to be displayed, set state of menu items.
        /// </summary>
        private void componentContextMenuStrip_Opened(object sender, EventArgs e)
        {
            itemMenuItemCopy.Enabled = ((IEditMenu)this).CanCopy();
            itemMenuItemCut.Enabled = ((IEditMenu)this).CanCut();
            itemMenuItemDelete.Enabled = ((IEditMenu)this).CanDelete();
            itemMenuItemPaste.Enabled = ((IEditMenu)this).CanPaste();
            itemMenuItemRename.Enabled = ((IEditMenu)this).CanRename();

            viewItemtoolStripMenuItem.Enabled = isComponentNode(tree.SelectedNode) || isConnectedProcessNode(tree.SelectedNode);

            Form form = isFormNode(tree.SelectedNode) ? tree.SelectedNode.Tag as Form : null;

            // always set enable state even if ultimate made !Visible

            if (form != null)
            {
                toolStripSeparator2.Visible = true;
                setupConnectAndDisconnectProcessMenuItems(itemMenuItemConnect, itemMenuItemDisconnect);
                setupConnectAndDisconnectPreProcessMenuItems(connectPreProcessToolStripMenuItem, disconnectPreProcessToolStripMenuItem);

                toolStripSeparator4.Visible = true;

                startingPointToolStripMenuItem.Checked = form.StartingPoint;
                startingPointToolStripMenuItem.Visible = true;
                startingPointToolStripMenuItem.Enabled = true;
                startingPointToolStripMenuItem.CheckOnClick = true;

                dataEntryOnlyToolStripMenuItem.Checked = form.DataEntryOnly;
                dataEntryOnlyToolStripMenuItem.CheckOnClick = true;
                dataEntryOnlyToolStripMenuItem.Visible = true;
                dataSourceNameToolStripMenuItem.Visible = true;

                blockBackButtonToolStripMenuItem.Checked = form.BlockBackButton;
                blockBackButtonToolStripMenuItem.Visible = true;
                blockBackButtonToolStripMenuItem.Enabled = true;
                blockBackButtonToolStripMenuItem.CheckOnClick = true;

                string dataSourceName = (SelectedNodeAsFormNode.DataSourceName == ""
                                             ? Resources.ResourceManager.GetString("DefaultDataSourceNameText")
                                             : SelectedNodeAsFormNode.DataSourceName);

                toolStripTextBoxDataSourceName.Text = dataSourceName;
            }
            else
            {
                toolStripSeparator2.Visible = false;
                itemMenuItemConnect.Visible = false;
                itemMenuItemDisconnect.Visible = false;
                connectPreProcessToolStripMenuItem.Visible = false;
                disconnectPreProcessToolStripMenuItem.Visible = false;

                toolStripSeparator4.Visible = false;
                startingPointToolStripMenuItem.Visible = false;
                startingPointToolStripMenuItem.Enabled = false;

                dataEntryOnlyToolStripMenuItem.Visible = false;

                dataSourceNameToolStripMenuItem.Visible = false;

                blockBackButtonToolStripMenuItem.Visible = false;
                blockBackButtonToolStripMenuItem.Enabled = false;
            }
        }

        private IDesignerPresenter getDesignerPresenter()
        {
            var view = ParentForm as IDesignerView;
            return view == null ? null : view.Presenter;
        }

        private void startingPointToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            var form = tree.SelectedNode.Tag as Form;
            form.StartingPoint = ((ToolStripMenuItem)sender).Checked;
            tree.SelectedNode.ImageIndex = tree.SelectedNode.SelectedImageIndex = FormNode.GetFormIconImageIndex(form);
            startPointToolStripButton.Checked = form.StartingPoint;
        }

        private void dataEntryOnlyToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            var form = tree.SelectedNode.Tag as Form;
            form.DataEntryOnly = ((ToolStripMenuItem)sender).Checked;
            tree.SelectedNode.ImageIndex = tree.SelectedNode.SelectedImageIndex = FormNode.GetFormIconImageIndex(form);
        }

        private void blockBackButtonToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            var form = tree.SelectedNode.Tag as Form;
            form.BlockBackButton = ((ToolStripMenuItem)sender).Checked;
            tree.SelectedNode.ImageIndex = tree.SelectedNode.SelectedImageIndex = FormNode.GetFormIconImageIndex(form);
            blockBackButtonToolStripButton.Checked = form.BlockBackButton;
        }

        private void viewItemtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            activateSelectedComponent();
        }

        // REVISIT:  Hack to support IEditMenu.GetAdditionMenuItems

        private void setupConnectAndDisconnectProcessMenuItems(ToolStripMenuItem connectProcessItem, ToolStripMenuItem disconnectProcessItem)
        {
            connectProcessItem.Visible = true;
            disconnectProcessItem.Visible = true;
            connectProcessItem.Enabled = false;
            disconnectProcessItem.Enabled = false;

            if (projectHasProcesses())
            {
                connectProcessItem.Enabled = true;
            }

            if (selectedFormHasConnectedProcess())
            {
                connectProcessItem.Enabled = false;
                disconnectProcessItem.Enabled = true;
            }

            if (connectProcessItem.Enabled)
            {
                connectProcessItem.DropDownItems.Clear();
                foreach (TreeNode treeNode in processesNode.Nodes)
                {
                    ToolStripItem item = connectProcessItem.DropDownItems.Add(treeNode.Text);
                    item.Click += itemMenuItemFormConnect_Click;
                }
            }
        }

        private bool selectedFormHasConnectedProcess()
        {
            var form = tree.SelectedNode.Tag as Form;
            return form.ConnectedPostProcess != null;
        }

        private bool projectHasProcesses()
        {
            return processesNode.Nodes.Count > 0;
        }

        private void setupConnectAndDisconnectPreProcessMenuItems(ToolStripMenuItem connectPreProcessItem,
                                                                  ToolStripMenuItem disconnectPreProcessItem)
        {
            connectPreProcessItem.Visible = true;
            disconnectPreProcessItem.Visible = true;
            connectPreProcessItem.Enabled = false;
            disconnectPreProcessItem.Enabled = false;

            if (projectHasProcesses())
            {
                connectPreProcessItem.Enabled = true;
            }

            if (selectedFormHasConnectedPreProcess())
            {
                connectPreProcessItem.Enabled = false;
                disconnectPreProcessItem.Enabled = true;
            }

            if (connectPreProcessItem.Enabled)
            {
                connectPreProcessItem.DropDownItems.Clear();
                foreach (TreeNode treeNode in processesNode.Nodes)
                {
                    ToolStripItem item = connectPreProcessItem.DropDownItems.Add(treeNode.Text);
                    item.Click += connectPreProcessToolStripMenuItem_Click;
                }
            }
        }

        private bool selectedFormHasConnectedPreProcess()
        {
            var form = tree.SelectedNode.Tag as Form;
            return form.ConnectedPreProcess != null;
        }

        #endregion

        #region IEditMenu for Main Form's Edit Menu

        // explicit implementation hides these unless instance cast to interface

        bool IEditMenu.CanCut()
        {
            return ((IEditMenu)this).CanCopy();
        }

        bool IEditMenu.CanCopy()
        {
            TreeNode n = tree.SelectedNode;
            return !inLabelEditMode && (isProcessNode(n) || isDocumentNode(n) || isFormNode(n));
        }

        bool IEditMenu.CanPaste()
        {
            IDataObject clipData = Clipboard.GetDataObject();

            if (clipData != null && !inLabelEditMode)
            {
                TreeNode node = tree.SelectedNode;

                if (node == processesNode || isProcessNode(node))
                {
                    return clipData.GetDataPresent(typeof(Process));
                }
                if (node == documentsNode || isDocumentNode(node))
                {
                    return clipData.GetDataPresent(typeof(RtfDocument));
                }
                if (node == formsNode || isFormNode(node))
                {
                    return clipData.GetDataPresent(typeof(Form));
                }
            }
            return false;
        }

        bool IEditMenu.CanDelete()
        {
            if (inLabelEditMode)
            {
                return false;
            }

            TreeNode node = tree.SelectedNode;

            return isProcessNode(node) || isDocumentNode(node) || isFormNode(node) || isConnectedProcessNode(node);
        }

        bool IEditMenu.CanRename()
        {
            return (!(tree.SelectedNode is FolderNode));
        }

        void IEditMenu.Cut()
        {
            if (((IEditMenu)this).CanCut())
            {
                if (isProcessNode(tree.SelectedNode))
                {
                    Clipboard.SetDataObject(Project.Current.GetProcess(tree.SelectedNode.Text));
                    Project.Current.RemoveProcess(tree.SelectedNode.Text);
                }
                else if (isDocumentNode(tree.SelectedNode))
                {
                    Clipboard.SetDataObject(Project.Current.GetDocument(tree.SelectedNode.Text));
                    Project.Current.RemoveDocument(tree.SelectedNode.Text);
                }
                else if (isFormNode(tree.SelectedNode))
                {
                    Clipboard.SetDataObject(Project.Current.GetForm(tree.SelectedNode.Text));
                    Project.Current.RemoveForm(tree.SelectedNode.Text);
                }
            }
        }

        void IEditMenu.Copy()
        {
            if (!((IEditMenu)this).CanCopy())
            {
                return;
            }

            if (isProcessNode(tree.SelectedNode))
            {
                Clipboard.SetDataObject(Project.Current.GetProcess(tree.SelectedNode.Text));
            }
            else if (isDocumentNode(tree.SelectedNode))
            {
                Clipboard.SetDataObject(Project.Current.GetDocument(tree.SelectedNode.Text));
            }
            else if (isFormNode(tree.SelectedNode))
            {
                Clipboard.SetDataObject(Project.Current.GetForm(tree.SelectedNode.Text));
            }
        }

        void IEditMenu.Paste()
        {
            if (((IEditMenu)this).CanPaste())
            {
                IDataObject clipData = Clipboard.GetDataObject();

                if (tree.SelectedNode == processesNode || isProcessNode(tree.SelectedNode))
                {
                    var p = clipData.GetData(typeof(Process)) as Process;

                    if (p != null)
                    {
                        Project.Current.PasteProcess(p);
                    }
                }
                else if (tree.SelectedNode == documentsNode || isDocumentNode(tree.SelectedNode))
                {
                    var d = clipData.GetData(typeof(RtfDocument)) as RtfDocument;

                    if (d != null)
                    {
                        Project.Current.PasteDocument(d);
                    }
                }
                else if (tree.SelectedNode == formsNode || isFormNode(tree.SelectedNode))
                {
                    var f = clipData.GetData(typeof(Form)) as Form;

                    if (f != null)
                    {
                        Project.Current.PasteForm(f);
                    }
                }
            }
        }

        void IEditMenu.Delete()
        {
            if (((IEditMenu)this).CanDelete())
            {
                if (isDocumentNode(tree.SelectedNode))
                {
                    Project.Current.RemoveDocument(tree.SelectedNode.Text);
                }
                else if (isFormNode(tree.SelectedNode))
                {
                    Project.Current.RemoveForm(tree.SelectedNode.Text);
                }
                else if (isProcessNode(tree.SelectedNode))
                {
                    Project.Current.RemoveProcess(tree.SelectedNode.Text);
                }
                else if (isConnectedProcessNode(tree.SelectedNode))
                {
                    (tree.SelectedNode as ConnectedProcessNode).DisconnectProcess();
                }
                else if (isConnectedPreProcessNode(tree.SelectedNode))
                {
                    (tree.SelectedNode as ConnectedPreProcessNode).DisconnectProcess();
                }
            }
        }

        void IEditMenu.Rename()
        {
            if (((IEditMenu)this).CanRename())
            {
                tree.SelectedNode.BeginEdit();
            }
        }

        bool IEditMenu.CanUndo()
        {
            return false;
        }

        bool IEditMenu.CanRedo()
        {
            return false;
        }

        void IEditMenu.Undo()
        {
        }

        void IEditMenu.Redo()
        {
        }

        string IEditMenu.UndoActionText { get { return ""; } }

        string IEditMenu.RedoActionText { get { return ""; } }

        ToolStripMenuItem[] IEditMenu.GetAdditionalMenuItems()
        {
            // REVISIT: This whole block -- See comment in DesignerUI.cs

            if (isFormNode(tree.SelectedNode))
            {
                var connectPreProcessItem = new ToolStripMenuItem(connectPreProcessToolStripMenuItem.Text, null,
                                                                  new EventHandler(connectPreProcessToolStripMenuItem_Click));
                var disconnectPreProcessItem = new ToolStripMenuItem(disconnectPreProcessToolStripMenuItem.Text, null,
                                                                     new EventHandler(disconnectPreProcessToolStripMenuItem_Click));
                var connectPostProcessItem = new ToolStripMenuItem(itemMenuItemConnect.Text, null,
                                                                   new EventHandler(itemMenuItemFormConnect_Click));
                var disconnectPostProcessItem = new ToolStripMenuItem(itemMenuItemDisconnect.Text, null,
                                                                      new EventHandler(itemMenuItemFormDisconnect_Click));

                var form = tree.SelectedNode.Tag as Form;
                var startPointMenu = new ToolStripMenuItem(startingPointToolStripMenuItem.Text, null,
                                                           new EventHandler(startingPointToolStripMenuItem_CheckedChanged));
                startPointMenu.Checked = form.StartingPoint;
                startPointMenu.CheckOnClick = true;

                var dataEntryOnlyMenu = new ToolStripMenuItem(dataEntryOnlyToolStripMenuItem.Text, null,
                                                              new EventHandler(dataEntryOnlyToolStripMenuItem_CheckedChanged));
                dataEntryOnlyMenu.Checked = form.DataEntryOnly;
                dataEntryOnlyMenu.CheckOnClick = true;

                var backButtonBlockedMenu = new ToolStripMenuItem(blockBackButtonToolStripMenuItem.Text, null,
                                                                  new EventHandler(blockBackButtonToolStripMenuItem_CheckedChanged));
                backButtonBlockedMenu.Checked = form.BlockBackButton;
                backButtonBlockedMenu.CheckOnClick = true;

                setupConnectAndDisconnectPreProcessMenuItems(connectPreProcessItem, disconnectPreProcessItem);
                setupConnectAndDisconnectProcessMenuItems(connectPostProcessItem, disconnectPostProcessItem);

                return new[]
                       {
                           connectPreProcessItem, disconnectPreProcessItem, connectPostProcessItem, disconnectPostProcessItem, null,
                           startPointMenu, dataEntryOnlyMenu, backButtonBlockedMenu
                       };
            }

            return null;
        }

        #endregion

        #region ToolStrip Events

        private void newFormToolStripButton_Click(object sender, EventArgs e)
        {
            Project.Current.AddForm();
        }

        private void newProjectToolStripButton_Click(object sender, EventArgs e)
        {
            Project.Current.AddProcess();
        }

        private void newDocumentToolStripButton_Click(object sender, EventArgs e)
        {
            Project.Current.AddDocument();
        }

        private void startPointToolStripButton_Click(object sender, EventArgs e)
        {
            if (isFormNode(tree.SelectedNode))
            {
                var form = tree.SelectedNode.Tag as IForm;
                form.StartingPoint = !form.StartingPoint;
                startPointToolStripButton.Checked = form.StartingPoint;
                tree.SelectedNode.ImageIndex = tree.SelectedNode.SelectedImageIndex = FormNode.GetFormIconImageIndex(form);
            }
        }

        private void blockBackButtonToolStripButton_Click(object sender, EventArgs e)
        {
            if (isFormNode(tree.SelectedNode))
            {
                var form = tree.SelectedNode.Tag as IForm;
                form.BlockBackButton = !form.BlockBackButton;
                blockBackButtonToolStripButton.Checked = form.BlockBackButton;
                tree.SelectedNode.ImageIndex = tree.SelectedNode.SelectedImageIndex = FormNode.GetFormIconImageIndex(form);
            }
        }

        private void nodeMoveUpToolStripButton_Click(object sender, EventArgs e)
        {
            TreeNode node = tree.SelectedNode;

            if (isComponentNode(node) && node.Index != 0)
            {
                Project.Current.MoveComponent(node.Tag as Component, node.Index - 1);

                tree.BeginUpdate();
                TreeNode parent = node.Parent;
                parent.Nodes.Remove(node);
                parent.Nodes.Insert(node.Index - 1, node);
                tree.SelectedNode = node;
                tree.EndUpdate();

                FieldsPalette.Palette.RefreshPalette();
            }
        }

        private void nodeMoveDownToolStripButton_Click(object sender, EventArgs e)
        {
            TreeNode node = tree.SelectedNode;

            if (isComponentNode(node) && node.Index != node.Parent.Nodes.Count - 1)
            {
                Project.Current.MoveComponent(node.Tag as Component, node.Index + 1);

                tree.BeginUpdate();
                TreeNode parent = node.Parent;
                parent.Nodes.Remove(node);
                parent.Nodes.Insert(node.Index + 1, node);
                tree.SelectedNode = node;
                tree.EndUpdate();

                FieldsPalette.Palette.RefreshPalette();
            }
        }

        #endregion
    }
}