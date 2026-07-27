// $Workfile: MdiDocumentView.Designer.cs $
// $Revision: 57 $	$Date: 2/22/08 11:18a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Documents
{
	/// <summary>
	/// Summary description for UserControl1.
	/// </summary>
	partial class MdiDocumentView 
	{
		// Designer variables below

		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertFieldToolStripMenuItem;
		private DocumentEditor documentEditor;


		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
			}
			base.Dispose( disposing );
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
			this.normalViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pageViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.displayUploadedImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertInvitationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertHyperlinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.functionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.formatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.boldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.italicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.underlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemFontColor = new System.Windows.Forms.ToolStripMenuItem();
			this.themeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recentFontColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.chooseColorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemResetFormatting = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tableInsertSubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertColumnBeforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertColumnAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertRowBeforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertRowAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tableDeleteSubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.fontFamilyToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.fontPointSizeToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.fontColorToolStripButton = new System.Windows.Forms.ToolStripSplitButton();
			this.themeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recentFontColorButtonMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.chooseColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripButtonResetFormatting = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.boldToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.italicToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.underlineToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.outdentToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.indentToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.alignmentToolStripButton = new System.Windows.Forms.ToolStripSplitButton();
			this.leftAlignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.centerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rightAlignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.justifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.insertTableToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.deleteTableToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.insertOrDeleteRowColumntoolStripButton = new System.Windows.Forms.ToolStripSplitButton();
			this.insertColumnBeforeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.insertColumnAfterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.insertRowBeforeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.insertRowAfterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.deleteColumnToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonInsertField = new System.Windows.Forms.ToolStripButton();
			this.documentEditor = new Tawala.Documents.DocumentEditor();
			this.menuStrip.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.insertToolStripMenuItem,
            this.formatToolStripMenuItem,
            this.tablesToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(612, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Visible = false;
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.normalViewToolStripMenuItem,
            this.pageViewToolStripMenuItem});
			this.viewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// normalViewToolStripMenuItem
			// 
			this.normalViewToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.NormalView;
			this.normalViewToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.normalViewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.normalViewToolStripMenuItem.Name = "normalViewToolStripMenuItem";
			this.normalViewToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
			this.normalViewToolStripMenuItem.Text = "Normal View";
			this.normalViewToolStripMenuItem.Click += new System.EventHandler(this.normalViewToolStripMenuItem_Click);
			// 
			// pageViewToolStripMenuItem
			// 
			this.pageViewToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.PageView;
			this.pageViewToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.pageViewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.pageViewToolStripMenuItem.Name = "pageViewToolStripMenuItem";
			this.pageViewToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
			this.pageViewToolStripMenuItem.Text = "Page View";
			this.pageViewToolStripMenuItem.Click += new System.EventHandler(this.pageViewToolStripMenuItem_Click);
			// 
			// insertToolStripMenuItem
			// 
			this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertFieldToolStripMenuItem,
            this.insertImageToolStripMenuItem,
            this.insertInvitationToolStripMenuItem,
            this.insertHyperlinkToolStripMenuItem,
            this.functionToolStripMenuItem});
			this.insertToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
			this.insertToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.insertToolStripMenuItem.Text = "&Insert";
			// 
			// insertFieldToolStripMenuItem
			// 
			this.insertFieldToolStripMenuItem.Enabled = false;
			this.insertFieldToolStripMenuItem.Name = "insertFieldToolStripMenuItem";
			this.insertFieldToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.insertFieldToolStripMenuItem.Text = "&Field";
			this.insertFieldToolStripMenuItem.Click += new System.EventHandler(this.insertFieldToolStripMenuItem_Click);
			// 
			// insertImageToolStripMenuItem
			// 
			this.insertImageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromFileToolStripMenuItem,
            this.displayUploadedImageToolStripMenuItem});
			this.insertImageToolStripMenuItem.Enabled = false;
			this.insertImageToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Insert_Image;
			this.insertImageToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertImageToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.insertImageToolStripMenuItem.Name = "insertImageToolStripMenuItem";
			this.insertImageToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.insertImageToolStripMenuItem.Text = "&Image...";
			// 
			// fromFileToolStripMenuItem
			// 
			this.fromFileToolStripMenuItem.Name = "fromFileToolStripMenuItem";
			this.fromFileToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
			this.fromFileToolStripMenuItem.Text = "From your &PC...";
			this.fromFileToolStripMenuItem.Click += new System.EventHandler(this.insertImageFromFileToolStripMenuItem_Click);
			// 
			// displayUploadedImageToolStripMenuItem
			// 
			this.displayUploadedImageToolStripMenuItem.Name = "displayUploadedImageToolStripMenuItem";
			this.displayUploadedImageToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
			this.displayUploadedImageToolStripMenuItem.Text = "From the &Web or Tawala Upload...";
			this.displayUploadedImageToolStripMenuItem.Click += new System.EventHandler(this.displayUploadedImageToolStripMenuItem_Click);
			// 
			// insertInvitationToolStripMenuItem
			// 
			this.insertInvitationToolStripMenuItem.Name = "insertInvitationToolStripMenuItem";
			this.insertInvitationToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.insertInvitationToolStripMenuItem.Text = "I&nvitation...";
			this.insertInvitationToolStripMenuItem.Click += new System.EventHandler(this.insertInvitationToolStripMenuItem_Click);
			// 
			// insertHyperlinkToolStripMenuItem
			// 
			this.insertHyperlinkToolStripMenuItem.Name = "insertHyperlinkToolStripMenuItem";
			this.insertHyperlinkToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.insertHyperlinkToolStripMenuItem.Text = "H&yperlink...";
			this.insertHyperlinkToolStripMenuItem.Click += new System.EventHandler(this.insertHyperlinkToolStripMenuItem_Click);
			// 
			// functionToolStripMenuItem
			// 
			this.functionToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Function;
			this.functionToolStripMenuItem.Name = "functionToolStripMenuItem";
			this.functionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.functionToolStripMenuItem.Text = "F&unction...";
			this.functionToolStripMenuItem.Click += new System.EventHandler(this.functionToolStripMenuItem_Click);
			// 
			// formatToolStripMenuItem
			// 
			this.formatToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.boldToolStripMenuItem,
            this.italicToolStripMenuItem,
            this.underlineToolStripMenuItem,
            this.toolStripMenuItemFontColor,
            this.toolStripMenuItemResetFormatting,
            this.toolStripSeparator1,
            this.tabsToolStripMenuItem});
			this.formatToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.formatToolStripMenuItem.Name = "formatToolStripMenuItem";
			this.formatToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
			this.formatToolStripMenuItem.Text = "F&ormat";
			// 
			// boldToolStripMenuItem
			// 
			this.boldToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Font_Bold;
			this.boldToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.boldToolStripMenuItem.MergeIndex = 0;
			this.boldToolStripMenuItem.Name = "boldToolStripMenuItem";
			this.boldToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
			this.boldToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.boldToolStripMenuItem.Text = "&Bold";
			this.boldToolStripMenuItem.Click += new System.EventHandler(this.boldToolStripMenuItem_Click);
			// 
			// italicToolStripMenuItem
			// 
			this.italicToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Font_Italic;
			this.italicToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.italicToolStripMenuItem.MergeIndex = 1;
			this.italicToolStripMenuItem.Name = "italicToolStripMenuItem";
			this.italicToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
			this.italicToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.italicToolStripMenuItem.Text = "&Italic";
			this.italicToolStripMenuItem.Click += new System.EventHandler(this.italicToolStripMenuItem_Click);
			// 
			// underlineToolStripMenuItem
			// 
			this.underlineToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Font_Underline;
			this.underlineToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.underlineToolStripMenuItem.MergeIndex = 2;
			this.underlineToolStripMenuItem.Name = "underlineToolStripMenuItem";
			this.underlineToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
			this.underlineToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.underlineToolStripMenuItem.Text = "&Underline";
			this.underlineToolStripMenuItem.Click += new System.EventHandler(this.underlineToolStripMenuItem_Click);
			// 
			// toolStripMenuItemFontColor
			// 
			this.toolStripMenuItemFontColor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.themeToolStripMenuItem,
            this.recentFontColorToolStripMenuItem,
            this.toolStripSeparator7,
            this.chooseColorToolStripMenuItem1});
			this.toolStripMenuItemFontColor.Name = "toolStripMenuItemFontColor";
			this.toolStripMenuItemFontColor.Size = new System.Drawing.Size(167, 22);
			this.toolStripMenuItemFontColor.Text = "&Color";
			// 
			// themeToolStripMenuItem
			// 
			this.themeToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Font_ThemeColorSwatch;
			this.themeToolStripMenuItem.Name = "themeToolStripMenuItem";
			this.themeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.themeToolStripMenuItem.Text = "&Theme";
			this.themeToolStripMenuItem.Click += new System.EventHandler(this.themeColorToolStripMenuItem_Click);
			// 
			// recentFontColorToolStripMenuItem
			// 
			this.recentFontColorToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Font_DefaultColor;
			this.recentFontColorToolStripMenuItem.Name = "recentFontColorToolStripMenuItem";
			this.recentFontColorToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.recentFontColorToolStripMenuItem.Text = "&Recent";
			this.recentFontColorToolStripMenuItem.Click += new System.EventHandler(this.recentFontColorToolStripMenuItem_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(120, 6);
			// 
			// chooseColorToolStripMenuItem1
			// 
			this.chooseColorToolStripMenuItem1.Image = global::Tawala.Documents.Properties.Resources.Font_ChooseColor;
			this.chooseColorToolStripMenuItem1.Name = "chooseColorToolStripMenuItem1";
			this.chooseColorToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
			this.chooseColorToolStripMenuItem1.Text = "&Choose...";
			this.chooseColorToolStripMenuItem1.Click += new System.EventHandler(this.chooseColorToolStripMenuItem_Click);
			// 
			// toolStripMenuItemResetFormatting
			// 
			this.toolStripMenuItemResetFormatting.Image = global::Tawala.Documents.Properties.Resources.Text_ClearFormatting;
			this.toolStripMenuItemResetFormatting.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItemResetFormatting.MergeIndex = 3;
			this.toolStripMenuItemResetFormatting.Name = "toolStripMenuItemResetFormatting";
			this.toolStripMenuItemResetFormatting.Size = new System.Drawing.Size(167, 22);
			this.toolStripMenuItemResetFormatting.Text = "&Reset Formatting";
			this.toolStripMenuItemResetFormatting.Click += new System.EventHandler(this.toolStripButtonResetFormatting_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripSeparator1.MergeIndex = 4;
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
			// 
			// tabsToolStripMenuItem
			// 
			this.tabsToolStripMenuItem.Name = "tabsToolStripMenuItem";
			this.tabsToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.tabsToolStripMenuItem.Text = "&Tabs...";
			this.tabsToolStripMenuItem.Click += new System.EventHandler(this.tabsToolStripMenuItem_Click);
			// 
			// tablesToolStripMenuItem
			// 
			this.tablesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tableInsertSubToolStripMenuItem,
            this.tableDeleteSubToolStripMenuItem});
			this.tablesToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.tablesToolStripMenuItem.MergeIndex = 5;
			this.tablesToolStripMenuItem.Name = "tablesToolStripMenuItem";
			this.tablesToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.tablesToolStripMenuItem.Text = "T&ables";
			// 
			// tableInsertSubToolStripMenuItem
			// 
			this.tableInsertSubToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertTableToolStripMenuItem,
            this.insertColumnBeforeToolStripMenuItem,
            this.insertColumnAfterToolStripMenuItem,
            this.insertRowBeforeToolStripMenuItem,
            this.insertRowAfterToolStripMenuItem});
			this.tableInsertSubToolStripMenuItem.Name = "tableInsertSubToolStripMenuItem";
			this.tableInsertSubToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.tableInsertSubToolStripMenuItem.Text = "&Insert";
			// 
			// insertTableToolStripMenuItem
			// 
			this.insertTableToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Table_Insert;
			this.insertTableToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertTableToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.insertTableToolStripMenuItem.Name = "insertTableToolStripMenuItem";
			this.insertTableToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.insertTableToolStripMenuItem.Text = "&Table";
			this.insertTableToolStripMenuItem.Click += new System.EventHandler(this.insertTableToolStripMenuItem_Click);
			// 
			// insertColumnBeforeToolStripMenuItem
			// 
			this.insertColumnBeforeToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Table_AddColumnBefore;
			this.insertColumnBeforeToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertColumnBeforeToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.insertColumnBeforeToolStripMenuItem.Name = "insertColumnBeforeToolStripMenuItem";
			this.insertColumnBeforeToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.insertColumnBeforeToolStripMenuItem.Text = "&Column Before";
			this.insertColumnBeforeToolStripMenuItem.Click += new System.EventHandler(this.insertColumnBeforeToolStripMenuItem_Click);
			// 
			// insertColumnAfterToolStripMenuItem
			// 
			this.insertColumnAfterToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Table_AddColumnAfter;
			this.insertColumnAfterToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertColumnAfterToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.insertColumnAfterToolStripMenuItem.Name = "insertColumnAfterToolStripMenuItem";
			this.insertColumnAfterToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.insertColumnAfterToolStripMenuItem.Text = "C&olumn After";
			this.insertColumnAfterToolStripMenuItem.Click += new System.EventHandler(this.insertColumnAfterToolStripMenuItem_Click);
			// 
			// insertRowBeforeToolStripMenuItem
			// 
			this.insertRowBeforeToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Table_AddRowBefore;
			this.insertRowBeforeToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertRowBeforeToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.insertRowBeforeToolStripMenuItem.Name = "insertRowBeforeToolStripMenuItem";
			this.insertRowBeforeToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.insertRowBeforeToolStripMenuItem.Text = "&Row Before";
			this.insertRowBeforeToolStripMenuItem.Click += new System.EventHandler(this.insertRowBeforeToolStripMenuItem_Click);
			// 
			// insertRowAfterToolStripMenuItem
			// 
			this.insertRowAfterToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Table_AddRowAfter;
			this.insertRowAfterToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertRowAfterToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.insertRowAfterToolStripMenuItem.Name = "insertRowAfterToolStripMenuItem";
			this.insertRowAfterToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.insertRowAfterToolStripMenuItem.Text = "Ro&w After";
			this.insertRowAfterToolStripMenuItem.Click += new System.EventHandler(this.insertRowAfterToolStripMenuItem_Click);
			// 
			// tableDeleteSubToolStripMenuItem
			// 
			this.tableDeleteSubToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteTableToolStripMenuItem,
            this.deleteColumnToolStripMenuItem,
            this.deleteRowToolStripMenuItem});
			this.tableDeleteSubToolStripMenuItem.Name = "tableDeleteSubToolStripMenuItem";
			this.tableDeleteSubToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.tableDeleteSubToolStripMenuItem.Text = "&Delete";
			// 
			// deleteTableToolStripMenuItem
			// 
			this.deleteTableToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Table_Delete;
			this.deleteTableToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.deleteTableToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.deleteTableToolStripMenuItem.Name = "deleteTableToolStripMenuItem";
			this.deleteTableToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.deleteTableToolStripMenuItem.Text = "&Table";
			this.deleteTableToolStripMenuItem.Click += new System.EventHandler(this.deleteTableToolStripMenuItem_Click);
			// 
			// deleteColumnToolStripMenuItem
			// 
			this.deleteColumnToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Table_DeleteColumn;
			this.deleteColumnToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.deleteColumnToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.deleteColumnToolStripMenuItem.Name = "deleteColumnToolStripMenuItem";
			this.deleteColumnToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.deleteColumnToolStripMenuItem.Text = "&Column";
			this.deleteColumnToolStripMenuItem.Click += new System.EventHandler(this.deleteColumnToolStripMenuItem_Click);
			// 
			// deleteRowToolStripMenuItem
			// 
			this.deleteRowToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Table_DeleteRow;
			this.deleteRowToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.deleteRowToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
			this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.deleteRowToolStripMenuItem.Text = "&Row";
			this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
			// 
			// toolStrip
			// 
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator5,
            this.fontFamilyToolStripComboBox,
            this.fontPointSizeToolStripComboBox,
            this.fontColorToolStripButton,
            this.toolStripButtonResetFormatting,
            this.toolStripSeparator2,
            this.boldToolStripButton,
            this.italicToolStripButton,
            this.underlineToolStripButton,
            this.toolStripSeparator4,
            this.outdentToolStripButton,
            this.indentToolStripButton,
            this.alignmentToolStripButton,
            this.toolStripSeparator6,
            this.insertTableToolStripButton,
            this.deleteTableToolStripButton,
            this.insertOrDeleteRowColumntoolStripButton,
            this.toolStripSeparator3,
            this.toolStripButtonInsertField});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Padding = new System.Windows.Forms.Padding(4, 0, 1, 0);
			this.toolStrip.Size = new System.Drawing.Size(612, 25);
			this.toolStrip.TabIndex = 3;
			this.toolStrip.Text = "toolStrip1";
			this.toolStrip.Visible = false;
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
			// 
			// fontFamilyToolStripComboBox
			// 
			this.fontFamilyToolStripComboBox.AutoSize = false;
			this.fontFamilyToolStripComboBox.DropDownHeight = 300;
			this.fontFamilyToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.fontFamilyToolStripComboBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.fontFamilyToolStripComboBox.IntegralHeight = false;
			this.fontFamilyToolStripComboBox.MaxDropDownItems = 20;
			this.fontFamilyToolStripComboBox.Name = "fontFamilyToolStripComboBox";
			this.fontFamilyToolStripComboBox.Size = new System.Drawing.Size(130, 22);
			this.fontFamilyToolStripComboBox.ToolTipText = "Font Face";
			this.fontFamilyToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.fontFamilyToolStripComboBox_SelectedIndexChanged);
			// 
			// fontPointSizeToolStripComboBox
			// 
			this.fontPointSizeToolStripComboBox.AutoSize = false;
			this.fontPointSizeToolStripComboBox.DropDownHeight = 140;
			this.fontPointSizeToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.fontPointSizeToolStripComboBox.DropDownWidth = 40;
			this.fontPointSizeToolStripComboBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.fontPointSizeToolStripComboBox.IntegralHeight = false;
			this.fontPointSizeToolStripComboBox.Items.AddRange(new object[] {
            "Default Size",
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28",
            "36",
            "48",
            "72"});
			this.fontPointSizeToolStripComboBox.MaxDropDownItems = 16;
			this.fontPointSizeToolStripComboBox.Name = "fontPointSizeToolStripComboBox";
			this.fontPointSizeToolStripComboBox.Size = new System.Drawing.Size(92, 22);
			this.fontPointSizeToolStripComboBox.ToolTipText = "Font Point Size";
			this.fontPointSizeToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.fontPointSizeToolStripComboBox_SelectedIndexChanged);
			// 
			// fontColorToolStripButton
			// 
			this.fontColorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.fontColorToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.themeColorToolStripMenuItem,
            this.recentFontColorButtonMenuItem,
            this.toolStripSeparator9,
            this.chooseColorToolStripMenuItem});
			this.fontColorToolStripButton.Image = global::Tawala.Documents.Properties.Resources.Font_ColorButtonTheme;
			this.fontColorToolStripButton.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(2)))), ((int)(((byte)(3)))));
			this.fontColorToolStripButton.Name = "fontColorToolStripButton";
			this.fontColorToolStripButton.Size = new System.Drawing.Size(32, 22);
			this.fontColorToolStripButton.Text = "Font Color";
			this.fontColorToolStripButton.ButtonClick += new System.EventHandler(this.fontColorToolStripButton_ButtonClick);
			// 
			// themeColorToolStripMenuItem
			// 
			this.themeColorToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Font_ThemeColorSwatch;
			this.themeColorToolStripMenuItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.themeColorToolStripMenuItem.Name = "themeColorToolStripMenuItem";
			this.themeColorToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.themeColorToolStripMenuItem.Tag = "";
			this.themeColorToolStripMenuItem.Text = "&Theme Color";
			this.themeColorToolStripMenuItem.Click += new System.EventHandler(this.themeColorToolStripMenuItem_Click);
			// 
			// recentFontColorButtonMenuItem
			// 
			this.recentFontColorButtonMenuItem.Image = global::Tawala.Documents.Properties.Resources.Font_DefaultColor;
			this.recentFontColorButtonMenuItem.Name = "recentFontColorButtonMenuItem";
			this.recentFontColorButtonMenuItem.Size = new System.Drawing.Size(155, 22);
			this.recentFontColorButtonMenuItem.Text = "&Recent Color";
			this.recentFontColorButtonMenuItem.Click += new System.EventHandler(this.recentFontColorToolStripMenuItem_Click);
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(152, 6);
			// 
			// chooseColorToolStripMenuItem
			// 
			this.chooseColorToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Font_ChooseColor;
			this.chooseColorToolStripMenuItem.Name = "chooseColorToolStripMenuItem";
			this.chooseColorToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.chooseColorToolStripMenuItem.Text = "&Choose Color...";
			this.chooseColorToolStripMenuItem.Click += new System.EventHandler(this.chooseColorToolStripMenuItem_Click);
			// 
			// toolStripButtonResetFormatting
			// 
			this.toolStripButtonResetFormatting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonResetFormatting.Image = global::Tawala.Documents.Properties.Resources.Text_ClearFormatting;
			this.toolStripButtonResetFormatting.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonResetFormatting.Name = "toolStripButtonResetFormatting";
			this.toolStripButtonResetFormatting.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonResetFormatting.ToolTipText = "Reset Formatting";
			this.toolStripButtonResetFormatting.Click += new System.EventHandler(this.toolStripButtonResetFormatting_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// boldToolStripButton
			// 
			this.boldToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.boldToolStripButton.Image = global::Tawala.Documents.Properties.Resources.Font_Bold;
			this.boldToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.boldToolStripButton.Name = "boldToolStripButton";
			this.boldToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.boldToolStripButton.Text = "Bold";
			this.boldToolStripButton.Click += new System.EventHandler(this.boldToolStripMenuItem_Click);
			// 
			// italicToolStripButton
			// 
			this.italicToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.italicToolStripButton.Image = global::Tawala.Documents.Properties.Resources.Font_Italic;
			this.italicToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.italicToolStripButton.Name = "italicToolStripButton";
			this.italicToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.italicToolStripButton.Text = "Italic";
			this.italicToolStripButton.Click += new System.EventHandler(this.italicToolStripMenuItem_Click);
			// 
			// underlineToolStripButton
			// 
			this.underlineToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.underlineToolStripButton.Image = global::Tawala.Documents.Properties.Resources.Font_Underline;
			this.underlineToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.underlineToolStripButton.Name = "underlineToolStripButton";
			this.underlineToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.underlineToolStripButton.Text = "toolStripButton1";
			this.underlineToolStripButton.ToolTipText = "Underline";
			this.underlineToolStripButton.Click += new System.EventHandler(this.underlineToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// outdentToolStripButton
			// 
			this.outdentToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.outdentToolStripButton.Image = global::Tawala.Documents.Properties.Resources.Paragraph_Outdent;
			this.outdentToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.outdentToolStripButton.Name = "outdentToolStripButton";
			this.outdentToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.outdentToolStripButton.Text = "Outdent";
			this.outdentToolStripButton.Click += new System.EventHandler(this.outdentToolStripButton_Click);
			// 
			// indentToolStripButton
			// 
			this.indentToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.indentToolStripButton.Image = global::Tawala.Documents.Properties.Resources.Paragraph_Indent;
			this.indentToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.indentToolStripButton.Name = "indentToolStripButton";
			this.indentToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.indentToolStripButton.Text = "toolStripButton1";
			this.indentToolStripButton.ToolTipText = "Indent";
			this.indentToolStripButton.Click += new System.EventHandler(this.indentToolStripButton_Click);
			// 
			// alignmentToolStripButton
			// 
			this.alignmentToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.alignmentToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.leftAlignToolStripMenuItem,
            this.centerToolStripMenuItem,
            this.rightAlignToolStripMenuItem,
            this.justifyToolStripMenuItem});
			this.alignmentToolStripButton.Image = global::Tawala.Documents.Properties.Resources.Text_LeftAlign;
			this.alignmentToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.alignmentToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.alignmentToolStripButton.Name = "alignmentToolStripButton";
			this.alignmentToolStripButton.Size = new System.Drawing.Size(30, 22);
			this.alignmentToolStripButton.Text = "Paragraph Alignment";
			// 
			// leftAlignToolStripMenuItem
			// 
			this.leftAlignToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Text_LeftAlign;
			this.leftAlignToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.leftAlignToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.leftAlignToolStripMenuItem.Name = "leftAlignToolStripMenuItem";
			this.leftAlignToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.leftAlignToolStripMenuItem.Text = "Left Align";
			this.leftAlignToolStripMenuItem.Click += new System.EventHandler(this.leftAlignToolStripMenuItem_Click);
			// 
			// centerToolStripMenuItem
			// 
			this.centerToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Text_Center;
			this.centerToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.centerToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.centerToolStripMenuItem.Name = "centerToolStripMenuItem";
			this.centerToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.centerToolStripMenuItem.Text = "Center";
			this.centerToolStripMenuItem.Click += new System.EventHandler(this.centerToolStripMenuItem_Click);
			// 
			// rightAlignToolStripMenuItem
			// 
			this.rightAlignToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Text_RightAlign;
			this.rightAlignToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.rightAlignToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.rightAlignToolStripMenuItem.Name = "rightAlignToolStripMenuItem";
			this.rightAlignToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.rightAlignToolStripMenuItem.Text = "Right Align";
			this.rightAlignToolStripMenuItem.Click += new System.EventHandler(this.rightAlignToolStripMenuItem_Click);
			// 
			// justifyToolStripMenuItem
			// 
			this.justifyToolStripMenuItem.Image = global::Tawala.Documents.Properties.Resources.Text_Justify;
			this.justifyToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.justifyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.justifyToolStripMenuItem.Name = "justifyToolStripMenuItem";
			this.justifyToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.justifyToolStripMenuItem.Text = "Justify";
			this.justifyToolStripMenuItem.Click += new System.EventHandler(this.justifyToolStripMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
			// 
			// insertTableToolStripButton
			// 
			this.insertTableToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.insertTableToolStripButton.Image = global::Tawala.Documents.Properties.Resources.Table_Insert;
			this.insertTableToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertTableToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.insertTableToolStripButton.Name = "insertTableToolStripButton";
			this.insertTableToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.insertTableToolStripButton.Text = "Insert Table";
			this.insertTableToolStripButton.Click += new System.EventHandler(this.insertTableToolStripMenuItem_Click);
			// 
			// deleteTableToolStripButton
			// 
			this.deleteTableToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.deleteTableToolStripButton.Image = global::Tawala.Documents.Properties.Resources.Table_Delete;
			this.deleteTableToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.deleteTableToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.deleteTableToolStripButton.Name = "deleteTableToolStripButton";
			this.deleteTableToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.deleteTableToolStripButton.Text = "Delete Table";
			this.deleteTableToolStripButton.Click += new System.EventHandler(this.deleteTableToolStripMenuItem_Click);
			// 
			// insertOrDeleteRowColumntoolStripButton
			// 
			this.insertOrDeleteRowColumntoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.insertOrDeleteRowColumntoolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertColumnBeforeToolStripMenuItem1,
            this.insertColumnAfterToolStripMenuItem1,
            this.insertRowBeforeToolStripMenuItem1,
            this.insertRowAfterToolStripMenuItem1,
            this.toolStripSeparator8,
            this.deleteColumnToolStripMenuItem1,
            this.deleteRowToolStripMenuItem1});
			this.insertOrDeleteRowColumntoolStripButton.Image = global::Tawala.Documents.Properties.Resources.Table_InsertDeleteRowOrColumn;
			this.insertOrDeleteRowColumntoolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertOrDeleteRowColumntoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.insertOrDeleteRowColumntoolStripButton.Name = "insertOrDeleteRowColumntoolStripButton";
			this.insertOrDeleteRowColumntoolStripButton.Size = new System.Drawing.Size(32, 22);
			this.insertOrDeleteRowColumntoolStripButton.Text = "Insert or Delete Row or Column";
			this.insertOrDeleteRowColumntoolStripButton.ToolTipText = "Insert or Delete Row or Column";
			// 
			// insertColumnBeforeToolStripMenuItem1
			// 
			this.insertColumnBeforeToolStripMenuItem1.Image = global::Tawala.Documents.Properties.Resources.Table_AddColumnBefore;
			this.insertColumnBeforeToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertColumnBeforeToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.insertColumnBeforeToolStripMenuItem1.Name = "insertColumnBeforeToolStripMenuItem1";
			this.insertColumnBeforeToolStripMenuItem1.Size = new System.Drawing.Size(186, 22);
			this.insertColumnBeforeToolStripMenuItem1.Text = "Insert Column Before";
			this.insertColumnBeforeToolStripMenuItem1.Click += new System.EventHandler(this.insertColumnBeforeToolStripMenuItem_Click);
			// 
			// insertColumnAfterToolStripMenuItem1
			// 
			this.insertColumnAfterToolStripMenuItem1.Image = global::Tawala.Documents.Properties.Resources.Table_AddColumnAfter;
			this.insertColumnAfterToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertColumnAfterToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.insertColumnAfterToolStripMenuItem1.Name = "insertColumnAfterToolStripMenuItem1";
			this.insertColumnAfterToolStripMenuItem1.Size = new System.Drawing.Size(186, 22);
			this.insertColumnAfterToolStripMenuItem1.Text = "Insert Column After";
			this.insertColumnAfterToolStripMenuItem1.Click += new System.EventHandler(this.insertColumnAfterToolStripMenuItem_Click);
			// 
			// insertRowBeforeToolStripMenuItem1
			// 
			this.insertRowBeforeToolStripMenuItem1.Image = global::Tawala.Documents.Properties.Resources.Table_AddRowBefore;
			this.insertRowBeforeToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertRowBeforeToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.insertRowBeforeToolStripMenuItem1.Name = "insertRowBeforeToolStripMenuItem1";
			this.insertRowBeforeToolStripMenuItem1.Size = new System.Drawing.Size(186, 22);
			this.insertRowBeforeToolStripMenuItem1.Text = "Insert Row Before";
			this.insertRowBeforeToolStripMenuItem1.Click += new System.EventHandler(this.insertRowBeforeToolStripMenuItem_Click);
			// 
			// insertRowAfterToolStripMenuItem1
			// 
			this.insertRowAfterToolStripMenuItem1.Image = global::Tawala.Documents.Properties.Resources.Table_AddRowAfter;
			this.insertRowAfterToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.insertRowAfterToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.insertRowAfterToolStripMenuItem1.Name = "insertRowAfterToolStripMenuItem1";
			this.insertRowAfterToolStripMenuItem1.Size = new System.Drawing.Size(186, 22);
			this.insertRowAfterToolStripMenuItem1.Text = "Insert Row After";
			this.insertRowAfterToolStripMenuItem1.Click += new System.EventHandler(this.insertRowAfterToolStripMenuItem_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(183, 6);
			// 
			// deleteColumnToolStripMenuItem1
			// 
			this.deleteColumnToolStripMenuItem1.Image = global::Tawala.Documents.Properties.Resources.Table_DeleteColumn;
			this.deleteColumnToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.deleteColumnToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.deleteColumnToolStripMenuItem1.Name = "deleteColumnToolStripMenuItem1";
			this.deleteColumnToolStripMenuItem1.Size = new System.Drawing.Size(186, 22);
			this.deleteColumnToolStripMenuItem1.Text = "Delete Column";
			this.deleteColumnToolStripMenuItem1.Click += new System.EventHandler(this.deleteColumnToolStripMenuItem_Click);
			// 
			// deleteRowToolStripMenuItem1
			// 
			this.deleteRowToolStripMenuItem1.Image = global::Tawala.Documents.Properties.Resources.Table_DeleteRow;
			this.deleteRowToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.deleteRowToolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
			this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(186, 22);
			this.deleteRowToolStripMenuItem1.Text = "Delete Row";
			this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonInsertField
			// 
			this.toolStripButtonInsertField.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonInsertField.Image = global::Tawala.Documents.Properties.Resources.Function;
			this.toolStripButtonInsertField.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonInsertField.Name = "toolStripButtonInsertField";
			this.toolStripButtonInsertField.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonInsertField.Text = "Insert or edit a function";
			this.toolStripButtonInsertField.Click += new System.EventHandler(this.functionToolStripMenuItem_Click);
			// 
			// documentEditor
			// 
			this.documentEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.documentEditor.Location = new System.Drawing.Point(0, 24);
			this.documentEditor.Margin = new System.Windows.Forms.Padding(0);
			this.documentEditor.Name = "documentEditor";
			this.documentEditor.SelectionHighlightRequiresFocus = false;
			this.documentEditor.Size = new System.Drawing.Size(612, 342);
			this.documentEditor.TabIndex = 2;
			this.documentEditor.ViewMode = Tawala.TextEditor.ViewMode.Normal;
			// 
			// MdiDocumentView
			// 
			this.ClientSize = new System.Drawing.Size(612, 366);
			this.Controls.Add(this.documentEditor);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.menuStrip);
			this.DoubleBuffered = true;
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MdiDocumentView";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Document - ";
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.ToolStripMenuItem formatToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem boldToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem italicToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem underlineToolStripMenuItem;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton boldToolStripButton;
		private System.Windows.Forms.ToolStripButton italicToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton underlineToolStripButton;
		private System.Windows.Forms.ToolStripComboBox fontPointSizeToolStripComboBox;
		private System.Windows.Forms.ToolStripComboBox fontFamilyToolStripComboBox;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripButton indentToolStripButton;
		private System.Windows.Forms.ToolStripButton outdentToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripSplitButton alignmentToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem leftAlignToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem centerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rightAlignToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem justifyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tablesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tableInsertSubToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertTableToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertColumnBeforeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertColumnAfterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertRowBeforeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertRowAfterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tableDeleteSubToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteTableToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripButton insertTableToolStripButton;
		private System.Windows.Forms.ToolStripButton deleteTableToolStripButton;
		private System.Windows.Forms.ToolStripSplitButton insertOrDeleteRowColumntoolStripButton;
		private System.Windows.Forms.ToolStripMenuItem insertColumnBeforeToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem insertColumnAfterToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem insertRowBeforeToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem insertRowAfterToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem normalViewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pageViewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertImageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteColumnToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.ToolStripMenuItem deleteColumnToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem tabsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertInvitationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem functionToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton toolStripButtonInsertField;
		private System.Windows.Forms.ToolStripButton toolStripButtonResetFormatting;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemResetFormatting;
		private System.Windows.Forms.ToolStripSplitButton fontColorToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem themeColorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recentFontColorButtonMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
		private System.Windows.Forms.ToolStripMenuItem chooseColorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFontColor;
		private System.Windows.Forms.ToolStripMenuItem themeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recentFontColorToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem chooseColorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem insertHyperlinkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayUploadedImageToolStripMenuItem;


}
}
