// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Tawala.Dialogs;
using Tawala.Documents.Properties;
using Tawala.FontSupport;
using Tawala.Functions.Controls;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Components;
using Tawala.Projects.Documents;
using Tawala.Projects.Links;
using Tawala.ProjectUI;
using Tawala.TextEditor;
using HorizontalAlignment=Tawala.TextEditor.HorizontalAlignment;

namespace Tawala.Documents
{
    public partial class MdiDocumentView : MDIComponentView, IEditMenu
    {
        // each has their own to help track current selection in particular view
        private static PrivateFontCollection privateFontCollection;
        private readonly TextEditorToolStripPresenter editorToolStripPresenter;
        private DocumentPalette emptyPalette;
        private Collection<string> webSafeFonts = new Collection<string>();

        /// <summary>
        /// This is the constructor that the Designer form (Designer.cs) should use.
        /// </summary>
        protected MdiDocumentView(Document doc) : base(doc)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            Icon = Resources.Document;
            StartPosition = FormStartPosition.WindowsDefaultLocation;

            editorToolStripPresenter = new TextEditorToolStripPresenter(fontColorToolStripButton, recentFontColorButtonMenuItem,
                                                                        recentFontColorToolStripMenuItem);

            Project.Events.SynchronizeProject += project_SynchronizeProject;
            Project.Events.ComponentSerializing += componentSerializing;

            addDefaultFontFile();
        }

        /// <summary>
        /// This constructor is solely for keeping the VSN Form Designer happy
        /// </summary>
        internal MdiDocumentView()
        {
            InitializeComponent();
        }

        public override ComponentPalette Palette
        {
            get
            {
                if (emptyPalette == null)
                {
                    emptyPalette = new DocumentPalette();
                }

                return emptyPalette;
            }
        }

        #region IEditMenu Members

        bool IEditMenu.CanCut()
        {
            return false;
        }

        bool IEditMenu.CanCopy()
        {
            return false;
        }

        bool IEditMenu.CanDelete()
        {
            return false;
        }

        bool IEditMenu.CanPaste()
        {
            return false;
        }

        bool IEditMenu.CanRename()
        {
            return false;
        }

        void IEditMenu.Cut()
        {
        }

        void IEditMenu.Copy()
        {
        }

        void IEditMenu.Delete()
        {
        }

        void IEditMenu.Paste()
        {
        }

        void IEditMenu.Rename()
        {
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
            return null;
        }

        #endregion

        public void InsertField(IField field)
        {
            documentEditor.InsertField(field);
        }

        private static void addDefaultFontFile()
        {
            if (privateFontCollection == null)
            {
                privateFontCollection = new PrivateFontCollection();
                privateFontCollection.AddFontFile(Fonts.DefaultFontFilename);
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            Project.Events.SynchronizeProject -= project_SynchronizeProject;
            Project.Events.ComponentSerializing -= componentSerializing;
        }

        private void componentSerializing(object sender, ComponentEventArgs e)
        {
            if (ReferenceEquals(Tag, e.Component))
            {
                documentEditor.Save();
            }
        }

        private void project_SynchronizeProject(object sender, EventArgs e)
        {
            documentEditor.Save();
        }

        private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!ConfigureFunctionDialog.Exists)
            {
                InsertField(e.Node.Tag as IField);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            webSafeFonts = Fonts.WebSafeFonts;

            base.OnLoad(e);

            fontFamilyToolStripComboBox.ComboBox.DisplayMember = "Name";
            fontFamilyToolStripComboBox.Items.Add(privateFontCollection.Families[0]);

            var ifc = new InstalledFontCollection();

            FontFamily[] families = ifc.Families;

            foreach (FontFamily f in families)
            {
                if (webSafeFonts.Contains(f.Name))
                {
                    fontFamilyToolStripComboBox.Items.Add(f);
                }
            }

            int i = fontFamilyToolStripComboBox.FindString(privateFontCollection.Families[0].Name);
            fontFamilyToolStripComboBox.SelectedIndex = i;
            fontPointSizeToolStripComboBox.SelectedIndex = 0;

            documentEditor.Selection.SetFontColor(getThemeColor());

            normalViewToolStripMenuItem.PerformClick();
        }

        public static void Create(ref MDIComponentView f, IProjectComponent component)
        {
            if (f == null && component is Document)
            {
                f = new MdiDocumentView(component as Document);
            }
        }

        private void application_Idle(object sender, EventArgs e)
        {
            if (ParentForm != null && ParentForm.ActiveMdiChild == this)
            {
                toolStripButtonInsertField.Enabled = functionToolStripMenuItem.Enabled = Project.Current.FormList.Count > 0;
            }

            insertImageToolStripMenuItem.Enabled = true;

            insertFieldToolStripMenuItem.Enabled = FieldsPalette.Palette != null &&
                                                   FieldsPalette.Palette != null &&
                                                   FieldsPalette.Palette.SelectedNode != null &&
                                                   FieldsPalette.Palette.SelectedNode.Tag is IField;

            // Undefined is considered false because only part of the range has the attribute

            bool bBold = documentEditor.Selection.Bold == Tristate.True;
            boldToolStripButton.Checked = bBold;
            boldToolStripMenuItem.Checked = bBold;

            bool bItalic = documentEditor.Selection.Italic == Tristate.True;
            italicToolStripButton.Checked = bItalic;
            italicToolStripMenuItem.Checked = bItalic;

            bool bUnderline = documentEditor.Selection.Underline == Tristate.True;
            underlineToolStripButton.Checked = bUnderline;
            underlineToolStripMenuItem.Checked = bUnderline;

            if (!fontFamilyToolStripComboBox.DroppedDown)
            {
                syncFontFamilyComboBox();
            }

            if (!fontPointSizeToolStripComboBox.DroppedDown)
            {
                syncFontSizeComboBox();
            }

            if (documentEditor.Selection != null)
            {
                HorizontalAlignment hza = documentEditor.Selection.ParagraphHAlignment;
                if (hza != HorizontalAlignment.Undefined)
                {
                    switch (hza)
                    {
                        case HorizontalAlignment.Left:
                        {
                            if (alignmentToolStripButton.Image != leftAlignToolStripMenuItem.Image)
                            {
                                alignmentToolStripButton.Image = leftAlignToolStripMenuItem.Image;
                            }
                            break;
                        }

                        case HorizontalAlignment.Center:
                        {
                            if (alignmentToolStripButton.Image != centerToolStripMenuItem.Image)
                            {
                                alignmentToolStripButton.Image = centerToolStripMenuItem.Image;
                            }
                            break;
                        }

                        case HorizontalAlignment.Right:
                        {
                            if (alignmentToolStripButton.Image != rightAlignToolStripMenuItem.Image)
                            {
                                alignmentToolStripButton.Image = rightAlignToolStripMenuItem.Image;
                            }
                            break;
                        }

                        case HorizontalAlignment.Justify:
                        {
                            if (alignmentToolStripButton.Image != justifyToolStripMenuItem.Image)
                            {
                                alignmentToolStripButton.Image = justifyToolStripMenuItem.Image;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    // What to do?
                }
            }

            if (documentEditor.CanInsertTable)
            {
                insertTableToolStripButton.Enabled = true;
                insertTableToolStripMenuItem.Enabled = true;
            }
            else
            {
                insertTableToolStripButton.Enabled = false;
                insertTableToolStripMenuItem.Enabled = false;
            }

            bool bCursorInTable = documentEditor.CursorInTable;
            insertOrDeleteRowColumntoolStripButton.Enabled = bCursorInTable;
            deleteTableToolStripButton.Enabled = bCursorInTable;
            deleteTableToolStripMenuItem.Enabled = bCursorInTable;
            insertColumnBeforeToolStripMenuItem.Enabled = bCursorInTable;
            insertColumnBeforeToolStripMenuItem1.Enabled = bCursorInTable;
            insertColumnAfterToolStripMenuItem.Enabled = bCursorInTable;
            insertColumnAfterToolStripMenuItem1.Enabled = bCursorInTable;
            insertRowBeforeToolStripMenuItem.Enabled = bCursorInTable;
            insertRowBeforeToolStripMenuItem1.Enabled = bCursorInTable;
            insertRowAfterToolStripMenuItem.Enabled = bCursorInTable;
            insertRowAfterToolStripMenuItem1.Enabled = bCursorInTable;
            deleteColumnToolStripMenuItem.Enabled = bCursorInTable;
            deleteColumnToolStripMenuItem1.Enabled = bCursorInTable;
            deleteRowToolStripMenuItem.Enabled = bCursorInTable;
            deleteRowToolStripMenuItem1.Enabled = bCursorInTable;
        }

        private void syncFontFamilyComboBox()
        {
            string fontName = documentEditor.Selection.FontName;
            if (fontName == string.Empty)
            {
                fontFamilyToolStripComboBox.SelectedIndex = -1;
            }
            else
            {
                fontFamilyToolStripComboBox.SelectedIndex =
                    fontFamilyToolStripComboBox.FindStringExact(fontName);
            }
        }

        private void syncFontSizeComboBox()
        {
            double fontSize = documentEditor.Selection.FontPointSize;
            if (fontSize == 0.0)
            {
                fontPointSizeToolStripComboBox.SelectedIndex = -1;
            }
            else if (fontSizeIsDefaultThemeFontSize(fontSize))
            {
                fontPointSizeToolStripComboBox.SelectedIndex = 0;
            }
            else
            {
                fontPointSizeToolStripComboBox.SelectedIndex =
                    fontPointSizeToolStripComboBox.FindStringExact(Convert.ToInt32(fontSize).ToString());
            }
        }

        private static bool fontSizeIsDefaultThemeFontSize(double fontSize)
        {
            return fontSize == Fonts.DefaultFontSize;
        }

        private void fontFamilyToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fontFamilyToolStripComboBox.SelectedIndex != -1)
            {
                var family = fontFamilyToolStripComboBox.SelectedItem as FontFamily;
                documentEditor.Selection.FontName = family.Name;
            }
            documentEditor.Focus();
        }

        private void fontPointSizeToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fontPointSizeToolStripComboBox.SelectedIndex == 0)
            {
                documentEditor.Selection.FontPointSize = Fonts.DefaultFontSize;
            }
            else if (fontPointSizeToolStripComboBox.SelectedIndex != -1)
            {
                documentEditor.Selection.FontPointSize = Convert.ToDouble(fontPointSizeToolStripComboBox.SelectedItem);
            }

            documentEditor.Focus();
        }

        private static Color getThemeColor()
        {
            return Fonts.DefaultFontColor;
        }

        private void normalViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            normalViewToolStripMenuItem.Checked = true;
            pageViewToolStripMenuItem.Checked = false;
            documentEditor.ViewMode = ViewMode.Normal;
        }

        private void pageViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            normalViewToolStripMenuItem.Checked = false;
            pageViewToolStripMenuItem.Checked = true;
            documentEditor.ViewMode = ViewMode.Page;
        }

        private void indentToolStripButton_Click(object sender, EventArgs e)
        {
            documentEditor.Indent();
        }

        private void outdentToolStripButton_Click(object sender, EventArgs e)
        {
            documentEditor.Outdent();
        }

        private void leftAlignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (documentEditor.Selection != null)
            {
                documentEditor.Selection.ParagraphHAlignment = HorizontalAlignment.Left;
            }
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (documentEditor.Selection != null)
            {
                documentEditor.Selection.ParagraphHAlignment = HorizontalAlignment.Center;
            }
        }

        private void rightAlignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (documentEditor.Selection != null)
            {
                documentEditor.Selection.ParagraphHAlignment = HorizontalAlignment.Right;
            }
        }

        private void justifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (documentEditor.Selection != null)
            {
                documentEditor.Selection.ParagraphHAlignment = HorizontalAlignment.Justify;
            }
        }

        private void insertColumnBeforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.InsertRowsOrColumns(true, 0, 1);
        }

        private void insertColumnAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.InsertRowsOrColumns(false, 0, 1);
        }

        private void insertRowBeforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.InsertRowsOrColumns(true, 1, 0);
        }

        private void insertRowAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.InsertRowsOrColumns(false, 1, 0);
        }

        private void insertTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var itd = new InsertTableDialog();
            if (itd.ShowDialog(this) == DialogResult.OK)
            {
                documentEditor.InsertTable(itd.TableWidth, itd.Rows, itd.Columns);
            }
        }

        private void deleteTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.DeleteTable();
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.DeleteRowsOrColumns(1, 0);
        }

        private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.DeleteRowsOrColumns(0, 1);
        }

        private void tabsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.ShowTabDialog();
        }

        private void insertEditInvitationSuccess(object sender, EventArgs e)
        {
            InvitationField invitationField = InsertInvitationDialog.Result;
            documentEditor.InsertInvitation(invitationField.Id, invitationField.DisplayText);
            FieldsPalette.Palette.RefreshFieldList();
        }

        private void insertEditHyperlinkSuccess(object sender, EventArgs e)
        {
            ILink hyperlinkField = ((InsertHyperlinkDialog)sender).Hyperlink;
            documentEditor.InsertHyperlink(hyperlinkField.Id, hyperlinkField.DesignerDisplayText);
            FieldsPalette.Palette.RefreshFieldList();
        }

        private void documentEditor_InvitationFieldDoubleClicked(object sender, InvitationFieldEventArgs fe)
        {
            editLinkField(fe.Id);
        }

        private void editLinkField(int id)
        {
            ILink linkField = Project.InvitationMapById[id];

            if (isInvitationField(linkField))
            {
                InsertInvitationDialog.Edit((InvitationField)linkField, insertEditInvitationSuccess);
            }
            else
            {
                InsertHyperlinkDialog.Edit((Hyperlink)linkField, insertEditHyperlinkSuccess);
            }
        }

        private static bool isInvitationField(ILink linkField)
        {
            //return string.IsNullOrEmpty(linkField.Url);
            return linkField is InvitationField;
        }

        private void insertInvitationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = documentEditor.SelectedInvitationFieldId();
            if (id > -1)
            {
                editLinkField(id);
            }
            else
            {
                InsertInvitationDialog.New(insertEditInvitationSuccess);
            }
        }

        private void insertHyperlinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = documentEditor.SelectedHyperlinkFieldId();
            if (id > -1)
            {
                editLinkField(id);
            }
            else
            {
                InsertHyperlinkDialog.New(insertEditHyperlinkSuccess);
            }
        }

        private void functionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.InsertFunction();
        }

        private void documentEditor_FunctionFieldDoubleClicked(object sender, FunctionFieldEventArgs fe)
        {
            documentEditor.EditSelectedFunctionConfiguration(fe.InstanceId);
        }

        private void toolStripButtonResetFormatting_Click(object sender, EventArgs e)
        {
            documentEditor.Selection.ResetFormatting();
        }

        private void fontColorToolStripButton_ButtonClick(object sender, EventArgs e)
        {
            var c = (Color)fontColorToolStripButton.Tag;
            editorToolStripPresenter.UpdateSelectedTextColor(c);
        }

        private void themeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editorToolStripPresenter.UpdateSelectedTextColor(getThemeColor());
        }

        private void recentFontColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var c = (Color)recentFontColorButtonMenuItem.Tag;
            editorToolStripPresenter.UpdateSelectedTextColor(c);
        }

        private void chooseColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editorToolStripPresenter.ChooseFontTextColor();
        }

        #region MDIDocument Events

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            ToolStripManager.Merge(toolStrip, "mainToolStrip");

            editorToolStripPresenter.Synchronize();
            editorToolStripPresenter.SetTextEditor(documentEditor);

            documentEditor.InvitationFieldDoubleClicked += documentEditor_InvitationFieldDoubleClicked;
            documentEditor.FunctionFieldDoubleClicked += documentEditor_FunctionFieldDoubleClicked;

            Application.Idle += application_Idle;

            FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            documentEditor.FunctionFieldDoubleClicked -= documentEditor_FunctionFieldDoubleClicked;
            documentEditor.InvitationFieldDoubleClicked -= documentEditor_InvitationFieldDoubleClicked;
            documentEditor.Save();

            Application.Idle -= application_Idle;

            base.OnDeactivate(e);

            ToolStripManager.RevertMerge("mainToolStrip");
        }

        #endregion

        #region Main Menu Events (Merged Menu Items)

        private void boldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.ToggleBold();
        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.ToggleItalic();
        }

        private void underlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.ToggleUnderline();
        }

        private void insertFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ParentForm.ActiveMdiChild == this && FieldsPalette.Palette.SelectedNode != null)
            {
                InsertField(FieldsPalette.Palette.SelectedNode.Tag as IField);
            }
        }

        private void displayUploadedImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.InsertUploadedImage();
        }
        
        private void insertImageFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            documentEditor.InsertImage();
        }

        #endregion
    }
}