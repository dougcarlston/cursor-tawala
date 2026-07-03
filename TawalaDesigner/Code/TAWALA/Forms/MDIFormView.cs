// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Tawala.Dialogs;
using Tawala.FontSupport;
using Tawala.Forms.Properties;
using Tawala.Projects;
using Tawala.Projects.Components;
using Tawala.Projects.Forms;
using Tawala.Projects.Links;
using Tawala.ProjectUI;
using Tawala.TextEditor;
using Form=Tawala.Projects.Form;
using HorizontalAlignment=Tawala.TextEditor.HorizontalAlignment;

namespace Tawala.Forms
{
    /// <summary>
    /// This is the view added by the designer.
    /// Normally it contains a FormViewInternal docked within it but may 
    /// instead contain alternate views such as SkipViewInternal.
    /// </summary>
    public partial class MDIFormView : MDIComponentView
    {
        private static readonly ComponentPalette palette = new ComponentPalette();
        private static FormItemPalette itemPalette;
        private static PrivateFontCollection privateFontCollection;
        private readonly TextEditorToolStripPresenter editorToolStripPresenter;
        private readonly Timer timer;
        private Control curPalette;

        // In MDIForm case holds a child palette which is swapped as necessary based on item type
        // for instance SkipItemView vs other items
        private Form projForm;

        private ItemTextEditor targetTextEditor;
        private Collection<string> webSafeFonts = new Collection<string>();

        /// <summary>
        /// This is the constructor that the Designer form (Designer.cs) should use.
        /// </summary>
        public MDIFormView(Form form) : base(form)
        {
            projForm = form;

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            Icon = Resources.Form;
            //SetPalette(null);

            editorToolStripPresenter = new TextEditorToolStripPresenter(fontColorToolStripButton, recentFontColorButtonMenuItem,
                                                                        recentFontColorToolStripMenuItem);

            timer = new Timer(components);
            timer.Interval = 250;
            timer.Tick += layoutTimer_Tick;

            formPreview.SetPreviewForm(form);

            addDefaultFontFile();
        }

        /// <summary>
        /// This constructor is solely for keeping the VSN Form Designer happy
        /// </summary>
        internal MDIFormView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            SetPalette(null);
        }

        public ItemTextEditor TargetTextEditor { get { return targetTextEditor; } set { targetTextEditor = value; } }

        public FormItemContainer FormItemContainer { get { return formItemContainer; } }

        /// <summary>
        /// Override of the base class property
        /// </summary>
        public override ComponentPalette Palette { get { return palette; } }

        public override bool CanPrint { get { return tabControl.IsHandleCreated ? tabControl.SelectedTab == tabPagePreview && formPreview.CanPrint : false; } }

        private void layoutTimer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Interval += 250;
            formItemContainer.PerformLayout();
            if (timer.Interval <= 1000)
            {
                timer.Start();
            }
        }

        public static void Create(ref MDIComponentView f, IProjectComponent component)
        {
            if (f == null && component is Form)
            {
                f = new MDIFormView(component as Form);
            }
        }

        private static void addDefaultFontFile()
        {
            if (privateFontCollection == null)
            {
                privateFontCollection = new PrivateFontCollection();
                privateFontCollection.AddFontFile(Fonts.DefaultFontFilename);
            }
        }

        /// <summary>
        /// Sets the palette for the form.  If null, specifies the default "Item Palette"
        /// </summary>
        public void SetPalette(Control c)
        {
            if (c == null)
            {
                if (itemPalette == null)
                {
                    itemPalette = new FormItemPalette();
                }

                c = itemPalette;
            }

            if (palette.Controls.Count == 0 || palette.Controls[0] != c)
            {
                palette.SuspendLayout();
                SuspendLayout();

                palette.Controls.Clear();
                curPalette = c;
                palette.Controls.Add(c);
                palette.Width = c.Width;

                palette.ResumeLayout(false);
                ResumeLayout(false);
                PerformLayout();
            }
        }

        public void PerformDelayedLayout()
        {
            if (timer != null && formItemContainer != null && formItemContainer.Visible)
            {
                timer.Stop();
                timer.Interval = 250;
                timer.Start();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            PerformDelayedLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            webSafeFonts = Fonts.WebSafeFonts;

            formItemContainer.SetForm(projForm);

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
        }

        public override void PrintPreview()
        {
            if (CanPrint)
            {
                formPreview.ShowPrintPreviewDialog();
            }
        }

        public override void Print()
        {
            if (CanPrint)
            {
                formPreview.ShowPrintDialog();
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            Application.Idle += Application_Idle;

            base.OnActivated(e);

            ToolStripManager.Merge(formsToolStrip, "mainToolStrip");

            editorToolStripPresenter.Synchronize();

            itemsToolStripMenuItem.Checked = Palette.Visible;

            SetPalette(curPalette);

            FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            Application.Idle -= Application_Idle;

            base.OnDeactivate(e);

            ToolStripManager.RevertMerge("mainToolStrip");
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (fontFamilyToolStripComboBox.DroppedDown)
            {
                return;
            }

            if (fontPointSizeToolStripComboBox.DroppedDown)
            {
                return;
            }

            enableOrDisableInsertImageButton();

            if (textEditorIsInactive() || TargetTextEditor.Parent is HeadingView)
            {
                disableTabDialogButton();

                disableBoldItems();
                disableItalicItems();
                disableUnderlineItems();

                disableFontFamilyComboBox();
                disableFontSizeComboBox();

                disableFontColorButton();
                disableResetFormattingButton();

                disableInsertTableButton();
                disableDeleteTableButton();
                disableTablesMenu();

                disableInsertOrDeleteRowColumnButton();

                disableIndentOutdentItems();
                disableAlignmentMenu();

                disableInsertFunctionButton();

                disableInsertHyperlink();
            }
            else
            {
                tabsToolStripMenuItem.Enabled = TargetTextEditor.Parent is FibItemView || TargetTextEditor.Parent is TextItemView ||
                                                TargetTextEditor.Parent is McqItemView;

                enableBoldItems();
                enableItalicItems();
                enableUnderlineItems();

                if (TargetTextEditor.Parent is FibItemView || TargetTextEditor.Parent is McqItemView)
                {
                    disableFontFamilyComboBox();
                    disableFontSizeComboBox();

                    disableFontColorButton();
                    disableResetFormattingButton();
                }
                else
                {
                    syncFontFamilyComboBox();
                    syncFontSizeComboBox();

                    enableFontColorButton();
                    enableResetFormattingButton();
                }

                enableInsertTableButton();
                enableDeleteTableButton();
                enableTablesMenu();

                enableInsertOrDeleteRowColumnButton();

                enableDeleteTableMenuItem();

                enableIndentOutdentItems();
                enableAlignmentMenu();

                enableOrDisableInsertFunctionButton();

                enableOrDisableInsertHyperlink();
            }
        }

        private void disableInsertOrDeleteRowColumnButton()
        {
            toolStripButtonInsertOrDeleteRowColumn.Enabled = false;
        }

        private void enableInsertOrDeleteRowColumnButton()
        {
            toolStripButtonInsertOrDeleteRowColumn.Enabled = TargetTextEditor.CursorInTable;
        }

        private void syncFontSizeComboBox()
        {
            enableFontSizeComboBox();

            double fontSize = TargetTextEditor.Selection.FontPointSize;
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
                int index = fontPointSizeToolStripComboBox.FindStringExact(Convert.ToInt32(fontSize).ToString());
                if (fontPointSizeToolStripComboBox.SelectedIndex != index)
                {
                    fontPointSizeToolStripComboBox.SelectedIndex = index;
                }
            }
        }

        private static bool fontSizeIsDefaultThemeFontSize(double fontSize)
        {
            return fontSize == Fonts.DefaultFontSize;
        }

        private void syncFontFamilyComboBox()
        {
            enableFontFamilyComboBox();

            string fontName = TargetTextEditor.Selection.FontName;
            if (fontName == string.Empty)
            {
                // If the font doesn't map on this system we should probably set it to what it is
                // This could result from moving to different PCs.
                // This may not be an issue because the text editor may have mapped it already and this case never occurs
                fontFamilyToolStripComboBox.SelectedIndex = -1;
            }
            else
            {
                int fontIndex = fontFamilyToolStripComboBox.FindStringExact(fontName);
                if (fontFamilyToolStripComboBox.SelectedIndex != fontIndex)
                {
                    fontFamilyToolStripComboBox.SelectedIndex = fontIndex;
                }
            }
        }

        private void enableFontColorButton()
        {
            fontColorToolStripButton.Enabled = true;
            toolStripMenuItemFontColor.Enabled = true;
            editorToolStripPresenter.SetTextEditor(TargetTextEditor);
        }

        private void enableResetFormattingButton()
        {
            toolStripButtonResetFormatting.Enabled = true;
            toolStripMenuItemResetFormatting.Enabled = true;
        }

        private void enableDeleteTableMenuItem()
        {
            deleteTableToolStripMenuItem.Enabled = TargetTextEditor.CursorInTable;
        }

        private void enableDeleteTableButton()
        {
            toolStripButtonDeleteTable.Enabled = TargetTextEditor.CursorInTable;
        }

        private void enableInsertTableButton()
        {
            // toolStripButtonInsertTable.Enabled = true;
            toolStripButtonInsertTable.Enabled = TargetTextEditor.Parent.GetType() == typeof(TextItemView);
        }

        private void enableOrDisableInsertHyperlink()
        {
            insertHyperlinkToolStripMenuItem.Enabled = TargetTextEditor.Parent.GetType() == typeof(TextItemView);
            insertInvitationToolStripMenuItem.Enabled = TargetTextEditor.Parent.GetType() == typeof(TextItemView);
        }

        private void enableTablesMenu()
        {
            // tablesToolStripMenuItem.Enabled = true;
            tablesToolStripMenuItem.Enabled = TargetTextEditor.Parent.GetType() == typeof(TextItemView);

            bool cursorInTable = TargetTextEditor.CursorInTable;

            insertTableToolStripMenuItem.Enabled = !cursorInTable;

            insertColumnBeforeToolStripMenuItem.Enabled = cursorInTable;
            insertColumnAfterToolStripMenuItem.Enabled = cursorInTable;
            insertRowBeforeToolStripMenuItem.Enabled = cursorInTable;
            insertRowAfterToolStripMenuItem.Enabled = cursorInTable;

            deleteTableToolStripMenuItem.Enabled = !cursorInTable;

            deleteColumnToolStripMenuItem.Enabled = cursorInTable;
            deleteRowToolStripMenuItem.Enabled = cursorInTable;
        }

        private void enableFontSizeComboBox()
        {
            if (!fontPointSizeToolStripComboBox.Enabled)
            {
                fontPointSizeToolStripComboBox.Enabled = true;
            }
        }

        private void enableFontFamilyComboBox()
        {
            if (!fontFamilyToolStripComboBox.Enabled)
            {
                fontFamilyToolStripComboBox.Enabled = true;
            }
        }

        private void enableUnderlineItems()
        {
            underlineToolStripButton.Enabled = underlineToolStripMenuItem.Enabled = true;
            underlineToolStripButton.Checked = underlineToolStripMenuItem.Checked = (TargetTextEditor.Selection.Underline == Tristate.True);
        }

        private void enableItalicItems()
        {
            italicToolStripButton.Enabled = italicToolStripMenuItem.Enabled = true;
            italicToolStripButton.Checked = italicToolStripMenuItem.Checked = (TargetTextEditor.Selection.Italic == Tristate.True);
        }

        private void enableBoldItems()
        {
            boldToolStripButton.Enabled = boldToolStripMenuItem.Enabled = true;
            boldToolStripButton.Checked = boldToolStripMenuItem.Checked = (TargetTextEditor.Selection.Bold == Tristate.True);
        }

        private void enableIndentOutdentItems()
        {
            indentToolStripButton.Enabled = TargetTextEditor.Parent.GetType() == typeof(TextItemView);
            outdentToolStripButton.Enabled = TargetTextEditor.Parent.GetType() == typeof(TextItemView);
        }

        private void enableAlignmentMenu()
        {
            alignmentToolStripButton.Enabled = TargetTextEditor.Parent.GetType() == typeof(TextItemView);

            if (alignmentToolStripButton.Enabled && TargetTextEditor.Selection != null)
            {
                HorizontalAlignment hza = TargetTextEditor.Selection.ParagraphHAlignment;
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
            }
        }

        private void disableTablesMenu()
        {
            tablesToolStripMenuItem.Enabled = false;
        }

        private void disableDeleteTableButton()
        {
            toolStripButtonDeleteTable.Enabled = false;
        }

        private void disableInsertTableButton()
        {
            toolStripButtonInsertTable.Enabled = false;
        }

        private void disableFontColorButton()
        {
            fontColorToolStripButton.Enabled = false;
            toolStripMenuItemFontColor.Enabled = false;
            editorToolStripPresenter.SetTextEditor(null);
        }

        private void disableFontSizeComboBox()
        {
            fontPointSizeToolStripComboBox.Enabled = false;
            fontPointSizeToolStripComboBox.SelectedIndex = -1;
        }

        private void disableFontFamilyComboBox()
        {
            fontFamilyToolStripComboBox.Enabled = false;
            fontFamilyToolStripComboBox.SelectedIndex = -1;
        }

        private void disableResetFormattingButton()
        {
            toolStripButtonResetFormatting.Enabled = false;
            toolStripMenuItemResetFormatting.Enabled = false;
        }

        private void disableUnderlineItems()
        {
            underlineToolStripButton.Checked = underlineToolStripMenuItem.Checked = false;
            underlineToolStripButton.Enabled = underlineToolStripMenuItem.Enabled = false;
        }

        private void disableItalicItems()
        {
            italicToolStripButton.Checked = italicToolStripMenuItem.Checked = false;
            italicToolStripButton.Enabled = italicToolStripMenuItem.Enabled = false;
        }

        private void disableBoldItems()
        {
            boldToolStripButton.Checked = boldToolStripMenuItem.Checked = false;
            boldToolStripButton.Enabled = boldToolStripMenuItem.Enabled = false;
        }

        private void disableTabDialogButton()
        {
            tabsToolStripMenuItem.Enabled = false;
        }

        private void disableIndentOutdentItems()
        {
            indentToolStripButton.Enabled = false;
            outdentToolStripButton.Enabled = false;
        }

        private void disableAlignmentMenu()
        {
            alignmentToolStripButton.Enabled = false;
        }

        private bool textEditorIsInactive()
        {
            return (TargetTextEditor == null || tabControl.SelectedTab != tabPageDesign);
        }

        private void enableOrDisableInsertImageButton()
        {
            insertImageToolStripMenuItem.Enabled = formItemContainer.CanInsertImage(TargetTextEditor);
        }

        private void enableOrDisableInsertFunctionButton()
        {
            toolStripButtonInsertFunction.Enabled = TargetTextEditor.Parent.GetType() == typeof(TextItemView);
            toolStripMenuItemInsertFunction.Enabled = TargetTextEditor.Parent.GetType() == typeof(TextItemView);
        }

        private void disableInsertFunctionButton()
        {
            toolStripMenuItemInsertFunction.Enabled = false;
            toolStripButtonInsertFunction.Enabled = false;
        }

        private void disableInsertHyperlink()
        {
            insertHyperlinkToolStripMenuItem.Enabled = false;
            insertInvitationToolStripMenuItem.Enabled = false;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (formPreview != null)
            {
                formPreview.Deactivate();
            }
            base.OnFormClosed(e);
        }

        /// <summary>
        /// Make sure non-static resources are freed at this time
        /// and non-static object references set to null
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            projForm = null;

            base.OnHandleDestroyed(e);
        }

        private void imageFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formItemContainer.InsertImage(TargetTextEditor);
        }

        private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null && TargetTextEditor != null)
            {
                var field = e.Node.Tag as IField;
                TargetTextEditor.InsertField(field as IPaletteField);
                TargetTextEditor.Focus();
            }
        }

        private void toolStripButtonResetFormatting_Click(object sender, EventArgs e)
        {
            if (TargetTextEditor != null)
            {
                TargetTextEditor.Selection.ResetFormatting();
            }
        }

        private void boldToolStripButton_Click(object sender, EventArgs e)
        {
            if (TargetTextEditor != null)
            {
                TargetTextEditor.ToggleBold();
            }
        }

        private void italicToolStripButton_Click(object sender, EventArgs e)
        {
            if (TargetTextEditor != null)
            {
                TargetTextEditor.ToggleItalic();
            }
        }

        private void underlineToolStripButton_Click(object sender, EventArgs e)
        {
            if (TargetTextEditor != null)
            {
                TargetTextEditor.ToggleUnderline();
            }
        }

        private void fontFamilyToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TargetTextEditor != null)
            {
                if (fontFamilyToolStripComboBox.SelectedIndex != -1)
                {
                    var family = fontFamilyToolStripComboBox.SelectedItem as FontFamily;
                    TargetTextEditor.Selection.FontName = family.Name;
                }
                TargetTextEditor.Focus();
            }
        }

        private void fontPointSizeToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TargetTextEditor != null)
            {
                if (fontPointSizeToolStripComboBox.SelectedIndex == 0)
                {
                    TargetTextEditor.Selection.FontPointSize = Fonts.DefaultFontSize;
                }
                else if (fontPointSizeToolStripComboBox.SelectedIndex != -1)
                {
                    TargetTextEditor.Selection.FontPointSize = Convert.ToDouble(fontPointSizeToolStripComboBox.SelectedItem);
                }
                TargetTextEditor.Focus();
            }
        }

        private void tabsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TargetTextEditor != null)
            {
                TargetTextEditor.ShowTabDialog();
            }
        }

        private static Color getThemeColor()
        {
            return Fonts.DefaultFontColor;
        }

        private void toolStripButtonInsertTable_Click(object sender, EventArgs e)
        {
            if (TargetTextEditor != null)
            {
                var itd = new InsertTableDialog();
                if (itd.ShowDialog(this) == DialogResult.OK)
                {
                    TargetTextEditor.InsertTable(itd.TableWidth, itd.Rows, itd.Columns);
                }
            }
        }

        private void toolStripButtonDeleteTable_Click(object sender, EventArgs e)
        {
            TargetTextEditor.DeleteTable();
        }

        private void insertColumnBeforeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TargetTextEditor.InsertRowsOrColumns(true, 0, 1);
        }

        private void insertColumnAfterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TargetTextEditor.InsertRowsOrColumns(false, 0, 1);
        }

        private void insertRowBeforeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TargetTextEditor.InsertRowsOrColumns(true, 1, 0);
        }

        private void insertRowAfterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TargetTextEditor.InsertRowsOrColumns(false, 1, 0);
        }

        private void deleteColumnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TargetTextEditor.DeleteRowsOrColumns(0, 1);
        }

        private void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TargetTextEditor.DeleteRowsOrColumns(1, 0);
        }

        private void outdentToolStripButton_Click(object sender, EventArgs e)
        {
            TargetTextEditor.Outdent();
        }

        private void indentToolStripButton_Click(object sender, EventArgs e)
        {
            TargetTextEditor.Indent();
        }

        private void leftAlignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TargetTextEditor.Selection.ParagraphHAlignment = HorizontalAlignment.Left;
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TargetTextEditor.Selection.ParagraphHAlignment = HorizontalAlignment.Center;
        }

        private void rightAlignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TargetTextEditor.Selection.ParagraphHAlignment = HorizontalAlignment.Right;
        }

        private void justifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TargetTextEditor.Selection.ParagraphHAlignment = HorizontalAlignment.Justify;
        }

        private void functionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TargetTextEditor.InsertFunction();
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPagePreview)
            {
                formPreview.Activate(formItemContainer.GetPreviewAnchorName());
            }
            else
            {
                formPreview.Deactivate();
            }
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

        private void fIBToolStripMenuItemStyle_Click(object sender, EventArgs e)
        {
            var stylesDialog = new FibStylesDialog(this);

            if (stylesDialog.ShowDialog() == DialogResult.OK)
            {
                if (stylesDialog.FibApplyAllSpecified)
                {
                    setAllFibStyles(stylesDialog);
                }
                else
                {
                    setSelectedFibStyles(stylesDialog);
                }
            }
        }

        private void mCQToolStripMenuItemStyle_Click(object sender, EventArgs e)
        {
            var stylesDialog = new McqItemStylesDialog(this);

            if (stylesDialog.ShowDialog() == DialogResult.OK)
            {
                if (stylesDialog.MCQApplyAllSpecified)
                {
                    setAllMCQStyles(stylesDialog);
                }
                else
                {
                    setSelectedMCQStyles(stylesDialog);
                }
            }
        }

        private void textToolStripMenuItemStyle_Click(object sender, EventArgs e)
        {
            var stylesDialog = new TextItemStylesDialog(this);

            if (stylesDialog.ShowDialog() == DialogResult.OK)
            {
                if (stylesDialog.TextApplyAllSpecified)
                {
                    setAllTextStyles(stylesDialog);
                }
                else
                {
                    setSelectedTextStyles(stylesDialog);
                }
            }
        }

        private static void setAllFibStyles(FibStylesDialog stylesDialog)
        {
            if (stylesDialog.FibFreeformSpecified)
            {
                Project.Current.SetAllFibStyles("freeform");
            }
            else if (stylesDialog.FibLeftLabelsSpecified)
            {
                Project.Current.SetAllFibStyles("leftAlignLabels");
            }
            else if (stylesDialog.FibRightLabelsSpecified)
            {
                Project.Current.SetAllFibStyles("rightAlignLabels");
            }
            else if (stylesDialog.FibLeftLabelsJustifiedSpecified)
            {
                Project.Current.SetAllFibStyles("leftAlignLabelsJustified");
            }
            else if (stylesDialog.FibRightLabelsJustifiedSpecified)
            {
                Project.Current.SetAllFibStyles("rightAlignLabelsJustified");
            }
            else if (stylesDialog.FibTopLabelsSpecified)
            {
                Project.Current.SetAllFibStyles("topLabels");
            }
        }

        private void setSelectedFibStyles(FibStylesDialog stylesDialog)
        {
            if (stylesDialog.FibFreeformSpecified)
            {
                setSelectedFibStyles("freeform");
            }
            else if (stylesDialog.FibLeftLabelsSpecified)
            {
                setSelectedFibStyles("leftAlignLabels");
            }
            else if (stylesDialog.FibRightLabelsSpecified)
            {
                setSelectedFibStyles("rightAlignLabels");
            }
            else if (stylesDialog.FibLeftLabelsJustifiedSpecified)
            {
                setSelectedFibStyles("leftAlignLabelsJustified");
            }
            else if (stylesDialog.FibRightLabelsJustifiedSpecified)
            {
                setSelectedFibStyles("rightAlignLabelsJustified");
            }
            else if (stylesDialog.FibTopLabelsSpecified)
            {
                setSelectedFibStyles("topLabels");
            }

            refreshFormPreview();
        }

        private static void refreshFormPreview()
        {
            Project.Events.RaiseThemeChangedEvent();
        }

        private void setSelectedFibStyles(string style)
        {
            if (projForm != null)
            {
                foreach (FormItem formItem in projForm.ItemList)
                {
                    var fibItem = formItem as FibItem;

                    if (fibItem != null)
                    {
                        if (fibItem.Selected)
                        {
                            fibItem.Style = style;
                        }
                    }
                }
            }
        }

        private static void setAllMCQStyles(McqItemStylesDialog stylesDialog)
        {
            if (stylesDialog.MCQVerticalSpecified)
            {
                Project.Current.SetAllMCQStyles("vertical", stylesDialog.PaddingBottom);
            }
            else if (stylesDialog.MCQHorizontalSpecified)
            {
                Project.Current.SetAllMCQStyles("horizontal", stylesDialog.PaddingBottom);
            }
            else if (stylesDialog.MCQMultiColumnSpecified)
            {
                Project.Current.SetAllMCQStyles("multicolumn", stylesDialog.MCQMultiColumnCount, stylesDialog.PaddingBottom);
            }
        }

        private void setSelectedMCQStyles(McqItemStylesDialog stylesDialog)
        {
            if (stylesDialog.MCQVerticalSpecified)
            {
                setSelectedMCQStyles("vertical", stylesDialog.PaddingBottom);
            }
            else if (stylesDialog.MCQHorizontalSpecified)
            {
                setSelectedMCQStyles("horizontal", stylesDialog.PaddingBottom);
            }
            else if (stylesDialog.MCQMultiColumnSpecified)
            {
                setSelectedMCQStyles("multicolumn", stylesDialog.MCQMultiColumnCount, stylesDialog.PaddingBottom);
            }

            refreshFormPreview();
        }

        private void setSelectedMCQStyles(string style, bool paddingBottom)
        {
            foreach (McqItem mcItem in getSelectedMCItems())
            {
                mcItem.Style = style;
                mcItem.ColumnCount = 0;
                mcItem.PaddingBottom = paddingBottom;
            }
        }

        private void setSelectedMCQStyles(string style, int columnCount, bool paddingBottom)
        {
            foreach (McqItem mcItem in getSelectedMCItems())
            {
                mcItem.Style = style;
                mcItem.ColumnCount = columnCount;
                mcItem.PaddingBottom = paddingBottom;
            }
        }

        private Collection<McqItem> getSelectedMCItems()
        {
            var mcItems = new Collection<McqItem>();

            if (projForm != null)
            {
                foreach (FormItem formItem in projForm.ItemList)
                {
                    var mcItem = formItem as McqItem;

                    if (mcItem != null)
                    {
                        if (mcItem.Selected)
                        {
                            mcItems.Add(mcItem);
                        }
                    }
                }
            }

            return mcItems;
        }

        private static void setAllTextStyles(TextItemStylesDialog stylesDialog)
        {
            if (stylesDialog.TextNormalSpecified)
            {
                Project.Current.SetAllTextItemStyles("normal", stylesDialog.PaddingBottom);
            }
            else if (stylesDialog.TextInstructionalSpecified)
            {
                Project.Current.SetAllTextItemStyles("instructional", stylesDialog.PaddingBottom);
            }
            else if (stylesDialog.TextErrorSpecified)
            {
                Project.Current.SetAllTextItemStyles("error", stylesDialog.PaddingBottom);
            }
        }

        private void setSelectedTextStyles(TextItemStylesDialog stylesDialog)
        {
            if (stylesDialog.TextNormalSpecified)
            {
                setSelectedTextStyles("normal", stylesDialog.PaddingBottom);
            }
            else if (stylesDialog.TextInstructionalSpecified)
            {
                setSelectedTextStyles("instructional", stylesDialog.PaddingBottom);
            }
            else if (stylesDialog.TextErrorSpecified)
            {
                setSelectedTextStyles("error", stylesDialog.PaddingBottom);
            }

            refreshFormPreview();
        }

        private void setSelectedTextStyles(string style, bool paddingBottom)
        {
            if (projForm != null)
            {
                foreach (FormItem formItem in projForm.ItemList)
                {
                    var textItem = formItem as TextItem;

                    if (textItem != null)
                    {
                        if (textItem.Selected)
                        {
                            textItem.Style = style;
                            textItem.PaddingBottom = paddingBottom;
                        }
                    }
                }
            }
        }

        #region Hyperlinks and Invitations

        private void insertEditInvitationSuccess(object sender, EventArgs e)
        {
            InvitationField invitationField = InsertInvitationDialog.Result;
            targetTextEditor.InsertInvitation(invitationField.Id, invitationField.DisplayText);
            returnFocusToTextItem();
            FieldsPalette.Palette.RefreshFieldList();
        }

        private void insertEditHyperlinkSuccess(object sender, EventArgs e)
        {
            ILink hyperlink = ((InsertHyperlinkDialog)sender).Hyperlink;
            targetTextEditor.InsertHyperlink(hyperlink.Id, hyperlink.DesignerDisplayText);
            returnFocusToTextItem();
            FieldsPalette.Palette.RefreshFieldList();
        }

        private void returnFocusToTextItem()
        {
            targetTextEditor.Focus();
        }

        internal void EditLinkField(int id)
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
            int id = targetTextEditor.SelectedInvitationFieldId();
            if (id > -1)
            {
                EditLinkField(id);
            }
            else
            {
                InsertInvitationDialog.New(insertEditInvitationSuccess);
            }
        }

        private void insertHyperlinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = targetTextEditor.SelectedHyperlinkFieldId();
            if (id > -1)
            {
                EditLinkField(id);
            }
            else
            {
                InsertHyperlinkDialog.New(insertEditHyperlinkSuccess);
            }
        }

        #endregion

        #region Menu Item Event Handlers

        private void insertToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            insertImageToolStripMenuItem.Enabled = TargetTextEditor != null;
        }

        private void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Palette.Visible = !Palette.Visible;
            itemsToolStripMenuItem.Checked = Palette.Visible;
        }

        private void toolStripMenuItemHeading_Click(object sender, EventArgs e)
        {
            formItemContainer.UserAddFormItem(new HeadingItem());
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formItemContainer.UserAddFormItem(new TextItem());
        }

        private void fillInTheBlankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formItemContainer.UserAddFormItem(new FibItem());
        }

        private void multipleChoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formItemContainer.UserAddFormItem(new McqItem());
        }

        private void pageBreakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formItemContainer.UserAddFormItem(new BreakItem());
        }

        private void skipInstructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formItemContainer.UserAddFormItem(new SkipInstructionsItem());
        }

        private void hiddenFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formItemContainer.UserAddFormItem(new HiddenField());
        }

        private void displayUploadedImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TargetTextEditor.InsertUploadedImage();
        }

        #endregion
    }
}