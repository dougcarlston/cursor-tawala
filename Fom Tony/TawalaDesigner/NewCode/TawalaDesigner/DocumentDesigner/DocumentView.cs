// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Browser;
using Tawala.ComponentDesigner;
using Tawala.Dialogs;
using Tawala.Functions.Controls;
using Tawala.Functions.ViewPresenter;
using Tawala.Interfaces;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.ProjectUI;

namespace Tawala.DocumentDesigner
{
    public partial class DocumentView : ProjectComponentView, IDocumentView
    {
        public DocumentView(IDocument document)
            : this()
        {
            Text = document.Name;
            Presenter = new DocumentPresenter(this, document);
            browser.DocumentCompleted += browser_DocumentCompleted;
            browser.FunctionDoubleClicked += browser_FunctionDoubleClicked;
            browser.LinkDoubleClicked += browser_LinkDoubleClicked;
            browser.LoadDocument();
        }

        protected DocumentView()
        {
            InitializeComponent();
        }

        public DocumentView(IDocument document, Form mdiParent, IProjectExplorerPresenter projectExplorerPresenter)
            : this(document)
        {
            MdiParent = mdiParent; // this causes handle to be recreated (which triggers things like OnHandleDestroyed
            ProjectExplorerPresenter = projectExplorerPresenter;
        }

        protected IProjectExplorerPresenter ProjectExplorerPresenter { get; private set; }

        private bool canCutDocumentContents
        {
            get { return true; }
        }

        private bool canCopyDocumentContents
        {
            get { return true; }
        }

        private bool canPasteDocumentContents
        {
            get { return true; }
        }

        private bool canDeleteDocumentContents
        {
            get { return true; }
        }

        #region IDocumentView Members

        public IDocumentPresenter Presenter { get; set; }

        public IApplicationView ParentView
        {
            get { return ParentForm as IApplicationView; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Contents
        {
            get { return browser.Contents; }
            set { browser.Contents = value; }
        }

        public void InsertFunction(int id, string text)
        {
            browser.InsertFunction(id, text);
        }

        public void UpdateFunction(int oldId, int newId, string text)
        {
            browser.UpdateFunction(oldId, newId, text);
        }

        public void InsertLink(int id, string text)
        {
            browser.InsertLink(id, text);
        }

        public void UpdateLink(int id, string text)
        {
            browser.UpdateLink(id, text);
        }

        public void SetDocumentName(string documentName)
        {
            Text = documentName;
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            var mainWindow = ParentForm as IApplicationView;

            if (mainWindow != null)
            {
                toolStripComboBoxFontFace.ComboBox.DisplayMember = "Name";
                toolStripComboBoxFontFace.Items.AddRange(mainWindow.GetFontList());
            }

            base.OnLoad(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (Disposing)
            {
                ProjectExplorerPresenter = null;
                Presenter.ViewClosed();
            }
            base.OnHandleDestroyed(e);
        }

        private void hookFieldsPalette()
        {
            if (FieldsPalette.Palette != null)
            {
                FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
            }
        }

        private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            browser.InsertField(e.Node.Tag as IField);
        }

        private void document_MouseDown(object sender, HtmlElementEventArgs e)
        {
            hookFieldsPalette();
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            browser.Document.MouseDown += document_MouseDown;
            Presenter.ViewInitializationCompleted();
            Activated += view_Activated;
            Deactivate += view_Deactivated;

            if (ParentForm != null && ParentForm.ActiveMdiChild == this)
            {
                view_Activated(this, EventArgs.Empty);
            }
        }

        private void view_Activated(object sender, EventArgs e)
        {
            if (ProjectExplorerPresenter != null)
            {
                ProjectExplorerPresenter.DocumentSelected(this);
            }

            ToolStripManager.Merge(documentMergeToolStrip, "mainToolStrip");

            hookFieldsPalette();

            Presenter.ViewActivated();
        }

        private void view_Deactivated(object sender, EventArgs e)
        {
            if (ProjectExplorerPresenter != null)
            {
                ProjectExplorerPresenter.DocumentDeselected();
            }

            ToolStripManager.RevertMerge("mainToolStrip");

            Presenter.ViewDeactivated();
        }

        private void functionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string activeElementId = browser.ActiveElementId;

            if (activeElementId != null && activeElementId.StartsWith("func_"))
            {
                int functionId = Convert.ToInt32(activeElementId.Replace("func_", ""));
                ConfigureFunctionDialog.Presenter.EditFunction(Project.FunctionMapById[functionId], functionEdited);
            }
            else
            {
                browser.SetBookmark();
                var insertFunctionDialog = new InsertFunctionDialog(functionInserted);
                insertFunctionDialog.ShowDialog(ParentForm);
            }
        }

        private void functionInserted(object sender, FunctionConfiguredEventArgs args)
        {
            Presenter.InsertFunction(args);
            browser.ClearBookmark();
        }

        private void functionEdited(object sender, FunctionConfiguredEventArgs args)
        {
            Presenter.UpdateFunction(args);
        }

        private void browser_FunctionDoubleClicked(object sender, ObjectDoubleClickedEventArgs e)
        {
            ConfigureFunctionDialog.Presenter.EditFunction(Project.FunctionMapById[e.Id], functionEdited);
        }

        private void viewSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.ViewSource(this);
        }

        private void toolStripMenuItemCut_Click(object sender, EventArgs e)
        {
            cutDocumentContentsOrDocument();
        }

        private void cutDocumentContentsOrDocument()
        {
            if (ProjectExplorerPresenter.ProjectExplorerHasFocus)
            {
                ProjectExplorerPresenter.DocumentCutRequested(this);
            }
            else
            {
                browser.Cut();
            }
        }

        private void toolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            copyDocumentContentsOrDocument();
        }

        private void copyDocumentContentsOrDocument()
        {
            if (ProjectExplorerPresenter.ProjectExplorerHasFocus)
            {
                ProjectExplorerPresenter.DocumentCopyRequested(this);
            }
            else
            {
                browser.Copy();
            }
        }

        private void toolStripMenuItemPaste_Click(object sender, EventArgs e)
        {
            pasteDocumentContentsOrProjectComponent();
        }

        private void pasteDocumentContentsOrProjectComponent()
        {
            if (ProjectExplorerPresenter.ProjectExplorerHasFocus)
            {
                ProjectExplorerPresenter.ComponentPasteRequested();
            }
            else
            {
                browser.Paste();
            }
        }

        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            deleteDocumentContentsOrDocument();
        }

        private void deleteDocumentContentsOrDocument()
        {
            if (ProjectExplorerPresenter.ProjectExplorerHasFocus)
            {
                ProjectExplorerPresenter.DocumentDeleteRequested(this);
            }
            else
            {
                browser.Delete();
            }
        }

        private void toolStripMenuItemUndo_Click(object sender, EventArgs e)
        {
            browser.Undo();
        }

        private void toolStripMenuItemRedo_Click(object sender, EventArgs e)
        {
            browser.Redo();
        }

        private void boldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.ToggleBold();
        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.ToggleItalic();
        }

        private void underlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.ToggleUnderline();
        }

        private void chooseColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog();
            cd.AllowFullOpen = true;
            cd.FullOpen = true;
            cd.AnyColor = true;

            if (cd.ShowDialog(ParentForm) == DialogResult.OK)
            {
                Color color = cd.Color;
                string colorString = String.Format("{0:X6}", (color.R << 16) + (color.G << 8) + color.B);

                HtmlSelection selection = getHtmlSelection();
                selection.SetFontColor(colorString);
                setHtmlSelection(selection);
            }
        }

        private void resetFormattingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.RemoveFormatting();
            HtmlSelection selection = getHtmlSelection();
            if (selection.RemoveFontFormatting())
            {
                setHtmlSelection(selection);
            }
        }

        private void setHtmlSelection(HtmlSelection selection)
        {
            browser.SetHtml(selection.ToXhtml());
        }

        private HtmlSelection getHtmlSelection()
        {
            return new HtmlSelection(browser.GetHtml());
        }

        private void insertTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var view = new InsertTableView();
            if (view.ShowDialog(this) == DialogResult.OK)
            {
                browser.InsertTable(view.TableWidthInPoints, view.Rows, view.Columns);
            }
        }

        private void deleteTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.DeleteCurrentTable();
        }

        private void insertColumnBeforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.InsertTableColumnBeforeCurrentColumn();
        }

        private void insertColumnAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.InsertTableColumnAfterCurrentColumn();
        }

        private void insertRowBeforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.InsertTableRowBeforeCurrentRow();
        }

        private void insertRowAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.InsertTableRowAfterCurrentRow();
        }

        private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.DeleteCurrentTableColumn();
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.DeleteCurrentTableRow();
        }

        private void indentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.Indent();
        }

        private void outdentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.Outdent();
        }

        private void alignLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.AlignLeft();
        }

        private void alignCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.AlignCenter();
        }

        private void alignRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.AlignRight();
        }

        private void justifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.AlignJustify();
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = ImageFileDialog.Browse(this);

            if (fileName != null)
            {
                browser.InsertImage(fileName);
            }
        }

        private void invitationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = getActiveLinkId();

            if (id != -1)
            {
                LinkEditor.EditLink(id, linkEditor_EditLinkComplete);
            }
            else
            {
                browser.SetBookmark();
                LinkEditor.NewInvitation(linkEditor_NewLink);
            }
        }

        private void hyperlinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = getActiveLinkId();

            if (id != -1)
            {
                LinkEditor.EditLink(id, linkEditor_EditLinkComplete);
            }
            else
            {
                browser.SetBookmark();
                LinkEditor.NewHyperlink(linkEditor_NewLink);
            }
        }

        private int getActiveLinkId()
        {
            if (browser.ActiveElementId != null && browser.ActiveElementId.StartsWith("link_"))
            {
                return Convert.ToInt32(browser.ActiveElementId.Replace("link_", ""));
            }
            return -1;
        }

        private void linkEditor_NewLink(object sender, LinkEditCompleteEventArgs e)
        {
            if (!e.Canceled)
            {
                browser.InsertLink(e.Link.Id, e.Link.DisplayText);
            }
            browser.ClearBookmark();
        }

        private void browser_LinkDoubleClicked(object sender, ObjectDoubleClickedEventArgs e)
        {
            LinkEditor.EditLink(e.Id, linkEditor_EditLinkComplete);
        }

        private void linkEditor_EditLinkComplete(object sender, LinkEditCompleteEventArgs e)
        {
            if (!e.Canceled)
            {
                browser.UpdateLink(e.Link.Id, e.Link.DisplayText);
            }
        }

        private void fontDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void fieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode fieldNode = FieldsPalette.Palette.SelectedNode;

            if (ParentForm.ActiveMdiChild == this && fieldNode != null)
            {
                if (fieldNode.Tag != null)
                {
                    browser.InsertField(fieldNode.Tag as IField);
                }
            }
        }

        private void documentView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (userClickedCloseBox(e))
            {
                hideInsteadOfClosing(e);
            }
        }

        private void documentInsertToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            hyperlinkToolStripMenuItem.Enabled = !LinkEditor.IsDialogActive;
            invitationToolStripMenuItem.Enabled = !LinkEditor.IsDialogActive;
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            showOrHideRenameItem();
        }

        private void showOrHideRenameItem()
        {
            toolStripMenuItemRename.Visible = ProjectExplorerPresenter.DocumentIsSelected;
        }

        private void toolStripMenuItemRename_Click(object sender, EventArgs e)
        {
            ProjectExplorerPresenter.EditComponentName();
        }

        private void DocumentView_Load(object sender, EventArgs e)
        {
            Application.Idle += application_Idle;
        }

        private void application_Idle(object sender, EventArgs e)
        {
            toolStripButtonCut.Enabled = documentContentsOrDocumentAreCuttable();
            toolStripMenuItemCut.Enabled = documentContentsOrDocumentAreCuttable();

            toolStripButtonCopy.Enabled = documentContentsOrDocumentAreCopyable();
            toolStripMenuItemCopy.Enabled = documentContentsOrDocumentAreCopyable();

            toolStripButtonPaste.Enabled = documentContentsOrProjectComponentArePasteable();
            toolStripMenuItemPaste.Enabled = documentContentsOrProjectComponentArePasteable();

            toolStripButtonDelete.Enabled = documentContentsOrDocumentAreDeletable();
            toolStripMenuItemDelete.Enabled = documentContentsOrDocumentAreDeletable();
        }

        private bool documentContentsOrDocumentAreCuttable()
        {
            return (ProjectExplorerPresenter.ProjectExplorerHasFocus
                        ? ProjectExplorerPresenter.CanCutDocument(this)
                        : canCutDocumentContents);
        }

        private bool documentContentsOrDocumentAreCopyable()
        {
            return (ProjectExplorerPresenter.ProjectExplorerHasFocus
                        ? ProjectExplorerPresenter.CanCopyDocument(this)
                        : canCopyDocumentContents);
        }

        private bool documentContentsOrProjectComponentArePasteable()
        {
            return (ProjectExplorerPresenter.ProjectExplorerHasFocus ? ProjectExplorerPresenter.CanPasteComponent : canPasteDocumentContents);
        }

        private bool documentContentsOrDocumentAreDeletable()
        {
            return (ProjectExplorerPresenter.ProjectExplorerHasFocus
                        ? ProjectExplorerPresenter.CanDeleteDocument(this)
                        : canDeleteDocumentContents);
        }

        private void toolStripComboBoxFontFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBoxFontFace.SelectedItem is FontFamily)
            {
                var family = toolStripComboBoxFontFace.SelectedItem as FontFamily;
                var selection = new HtmlSelection(browser.GetHtml());
                selection.SetFontFace(family.Name);
                browser.SetHtml(selection.ToXhtml());
            }
        }

        private void toolStripComboBoxFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBoxFontSize.SelectedIndex >= 0)
            {
                var selection = new HtmlSelection(browser.GetHtml());

                try
                {
                    selection.SetFontSize(Convert.ToInt32(toolStripComboBoxFontSize.SelectedItem));
                }
                catch
                {
                    selection.SetFontSize(NewFont.DefaultFontSize);
                }

                browser.SetHtml(selection.ToXhtml());
            }
        }

        private void defaultColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selection = new HtmlSelection(browser.GetHtml());
            if (selection.RemoveFontColor())
            {
                browser.SetHtml(selection.ToXhtml());               
            }
        }
    }
}