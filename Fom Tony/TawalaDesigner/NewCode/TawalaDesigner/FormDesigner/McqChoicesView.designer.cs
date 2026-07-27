namespace Tawala.FormDesigner
{
	partial class McqChoicesView
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panelOkCancel = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.comboBoxSource = new System.Windows.Forms.ComboBox();
			this.linkLabelConfigure = new System.Windows.Forms.LinkLabel();
			this.toolStripEditChoices = new System.Windows.Forms.ToolStrip();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonBold = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonItalic = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonUnderline = new System.Windows.Forms.ToolStripButton();
			this.webBrowserChoices = new System.Windows.Forms.WebBrowser();
			this.panel1 = new System.Windows.Forms.Panel();
			this.menuStripEditChoices = new System.Windows.Forms.MenuStrip();
			this.toolStripMenuItemFormat = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemBold = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemItalic = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemUnderline = new System.Windows.Forms.ToolStripMenuItem();
			this.panelOkCancel.SuspendLayout();
			this.toolStripEditChoices.SuspendLayout();
			this.panel1.SuspendLayout();
			this.menuStripEditChoices.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelOkCancel
			// 
			this.panelOkCancel.AutoSize = true;
			this.panelOkCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelOkCancel.Controls.Add(this.buttonCancel);
			this.panelOkCancel.Controls.Add(this.buttonOK);
			this.panelOkCancel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelOkCancel.Location = new System.Drawing.Point(0, 231);
			this.panelOkCancel.Name = "panelOkCancel";
			this.panelOkCancel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.panelOkCancel.Size = new System.Drawing.Size(267, 39);
			this.panelOkCancel.TabIndex = 4;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(136, 9);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.toolStripButtonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(55, 9);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.toolStripButtonOK_Click);
			// 
			// comboBoxSource
			// 
			this.comboBoxSource.FormattingEnabled = true;
			this.comboBoxSource.Items.AddRange(new object[] {
            "Enter choices above",
            "Get choices from stored data"});
			this.comboBoxSource.Location = new System.Drawing.Point(12, 200);
			this.comboBoxSource.Name = "comboBoxSource";
			this.comboBoxSource.Size = new System.Drawing.Size(169, 21);
			this.comboBoxSource.TabIndex = 5;
			this.comboBoxSource.SelectedIndexChanged += new System.EventHandler(this.comboBoxSource_SelectedIndexChanged);
			// 
			// linkLabelConfigure
			// 
			this.linkLabelConfigure.AutoSize = true;
			this.linkLabelConfigure.Location = new System.Drawing.Point(201, 205);
			this.linkLabelConfigure.Name = "linkLabelConfigure";
			this.linkLabelConfigure.Size = new System.Drawing.Size(52, 13);
			this.linkLabelConfigure.TabIndex = 6;
			this.linkLabelConfigure.TabStop = true;
			this.linkLabelConfigure.Text = "Configure";
			this.linkLabelConfigure.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelConfigure_LinkClicked);
			// 
			// toolStripEditChoices
			// 
			this.toolStripEditChoices.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.toolStripButtonBold,
            this.toolStripButtonItalic,
            this.toolStripButtonUnderline});
			this.toolStripEditChoices.Location = new System.Drawing.Point(0, 0);
			this.toolStripEditChoices.Name = "toolStripEditChoices";
			this.toolStripEditChoices.Size = new System.Drawing.Size(267, 25);
			this.toolStripEditChoices.TabIndex = 7;
			this.toolStripEditChoices.Text = "toolStrip1";
			this.toolStripEditChoices.Visible = false;
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonBold
			// 
			this.toolStripButtonBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonBold.Image = global::Tawala.FormDesigner.Properties.Resources.Bold;
			this.toolStripButtonBold.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonBold.Name = "toolStripButtonBold";
			this.toolStripButtonBold.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonBold.Text = "Bold";
			this.toolStripButtonBold.Click += new System.EventHandler(this.toolStripButtonBold_Click);
			// 
			// toolStripButtonItalic
			// 
			this.toolStripButtonItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonItalic.Image = global::Tawala.FormDesigner.Properties.Resources.Italic;
			this.toolStripButtonItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonItalic.Name = "toolStripButtonItalic";
			this.toolStripButtonItalic.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonItalic.Text = "Italic";
			this.toolStripButtonItalic.Click += new System.EventHandler(this.toolStripButtonItalic_Click);
			// 
			// toolStripButtonUnderline
			// 
			this.toolStripButtonUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonUnderline.Image = global::Tawala.FormDesigner.Properties.Resources.Underline;
			this.toolStripButtonUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonUnderline.Name = "toolStripButtonUnderline";
			this.toolStripButtonUnderline.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonUnderline.Text = "Underline";
			this.toolStripButtonUnderline.Click += new System.EventHandler(this.toolStripButtonUnderline_Click);
			// 
			// webBrowserChoices
			// 
			this.webBrowserChoices.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowserChoices.Location = new System.Drawing.Point(0, 0);
			this.webBrowserChoices.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserChoices.Name = "webBrowserChoices";
			this.webBrowserChoices.Size = new System.Drawing.Size(236, 174);
			this.webBrowserChoices.TabIndex = 8;
			this.webBrowserChoices.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserChoices_DocumentCompleted);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.webBrowserChoices);
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(240, 178);
			this.panel1.TabIndex = 9;
			// 
			// menuStripEditChoices
			// 
			this.menuStripEditChoices.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemFormat});
			this.menuStripEditChoices.Location = new System.Drawing.Point(0, 0);
			this.menuStripEditChoices.Name = "menuStripEditChoices";
			this.menuStripEditChoices.Size = new System.Drawing.Size(267, 24);
			this.menuStripEditChoices.TabIndex = 10;
			this.menuStripEditChoices.Text = "menuStrip1";
			this.menuStripEditChoices.Visible = false;
			// 
			// toolStripMenuItemFormat
			// 
			this.toolStripMenuItemFormat.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemBold,
            this.toolStripMenuItemItalic,
            this.toolStripMenuItemUnderline});
			this.toolStripMenuItemFormat.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItemFormat.MergeIndex = 4;
			this.toolStripMenuItemFormat.Name = "toolStripMenuItemFormat";
			this.toolStripMenuItemFormat.Size = new System.Drawing.Size(61, 20);
			this.toolStripMenuItemFormat.Text = "Format";
			// 
			// toolStripMenuItemBold
			// 
			this.toolStripMenuItemBold.Image = global::Tawala.FormDesigner.Properties.Resources.Bold;
			this.toolStripMenuItemBold.Name = "toolStripMenuItemBold";
			this.toolStripMenuItemBold.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
			this.toolStripMenuItemBold.Size = new System.Drawing.Size(187, 22);
			this.toolStripMenuItemBold.Text = "Bold";
			this.toolStripMenuItemBold.Click += new System.EventHandler(this.toolStripButtonBold_Click);
			// 
			// toolStripMenuItemItalic
			// 
			this.toolStripMenuItemItalic.Image = global::Tawala.FormDesigner.Properties.Resources.Italic;
			this.toolStripMenuItemItalic.Name = "toolStripMenuItemItalic";
			this.toolStripMenuItemItalic.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
			this.toolStripMenuItemItalic.Size = new System.Drawing.Size(187, 22);
			this.toolStripMenuItemItalic.Text = "Italic";
			this.toolStripMenuItemItalic.Click += new System.EventHandler(this.toolStripButtonItalic_Click);
			// 
			// toolStripMenuItemUnderline
			// 
			this.toolStripMenuItemUnderline.Image = global::Tawala.FormDesigner.Properties.Resources.Underline;
			this.toolStripMenuItemUnderline.Name = "toolStripMenuItemUnderline";
			this.toolStripMenuItemUnderline.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
			this.toolStripMenuItemUnderline.Size = new System.Drawing.Size(187, 22);
			this.toolStripMenuItemUnderline.Text = "Underline";
			this.toolStripMenuItemUnderline.Click += new System.EventHandler(this.toolStripButtonUnderline_Click);
			// 
			// McqChoicesView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(267, 270);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.linkLabelConfigure);
			this.Controls.Add(this.comboBoxSource);
			this.Controls.Add(this.panelOkCancel);
			this.Controls.Add(this.menuStripEditChoices);
			this.Controls.Add(this.toolStripEditChoices);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MainMenuStrip = this.menuStripEditChoices;
			this.MinimumSize = new System.Drawing.Size(200, 298);
			this.Name = "McqChoicesView";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Edit Choices";
			this.TopMost = true;
			this.Deactivate += new System.EventHandler(this.McqChoicesView_Deactivate);
			this.Load += new System.EventHandler(this.McqChoicesView_Load);
			this.Activated += new System.EventHandler(this.McqChoicesView_Activated);
			this.panelOkCancel.ResumeLayout(false);
			this.toolStripEditChoices.ResumeLayout(false);
			this.toolStripEditChoices.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.menuStripEditChoices.ResumeLayout(false);
			this.menuStripEditChoices.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panelOkCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ComboBox comboBoxSource;
		private System.Windows.Forms.LinkLabel linkLabelConfigure;
		private System.Windows.Forms.ToolStrip toolStripEditChoices;
		private System.Windows.Forms.ToolStripButton toolStripButtonBold;
		private System.Windows.Forms.ToolStripButton toolStripButtonItalic;
		private System.Windows.Forms.ToolStripButton toolStripButtonUnderline;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.WebBrowser webBrowserChoices;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.MenuStrip menuStripEditChoices;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFormat;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBold;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemItalic;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUnderline;
	}
}