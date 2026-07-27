namespace HtmlEditorDemo
{
	partial class Form1
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
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonDefaultText = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.htmlEditor = new System.Windows.Forms.WebBrowser();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonBold = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonItalic = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonUnderline = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonPaste = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonCopy = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonDefaultText,
            this.toolStripSeparator1,
            this.toolStripButtonBold,
            this.toolStripButtonItalic,
            this.toolStripButtonUnderline,
            this.toolStripSeparator2,
            this.toolStripButtonCopy,
            this.toolStripButtonPaste});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(647, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonDefaultText
			// 
			this.toolStripButtonDefaultText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonDefaultText.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDefaultText.Name = "toolStripButtonDefaultText";
			this.toolStripButtonDefaultText.Size = new System.Drawing.Size(118, 22);
			this.toolStripButtonDefaultText.Text = "Insert Default Text";
			this.toolStripButtonDefaultText.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
			this.toolStripButtonDefaultText.Click += new System.EventHandler(this.toolStripButtonDefaultText_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// htmlEditor
			// 
			this.htmlEditor.AllowNavigation = false;
			this.htmlEditor.AllowWebBrowserDrop = false;
			this.htmlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.htmlEditor.IsWebBrowserContextMenuEnabled = false;
			this.htmlEditor.Location = new System.Drawing.Point(0, 25);
			this.htmlEditor.MinimumSize = new System.Drawing.Size(20, 20);
			this.htmlEditor.Name = "htmlEditor";
			this.htmlEditor.ScriptErrorsSuppressed = true;
			this.htmlEditor.Size = new System.Drawing.Size(647, 237);
			this.htmlEditor.TabIndex = 1;
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonBold
			// 
			this.toolStripButtonBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonBold.Image = global::HtmlEditorDemo.Properties.Resources.Bold;
			this.toolStripButtonBold.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonBold.Name = "toolStripButtonBold";
			this.toolStripButtonBold.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonBold.Text = "Bold";
			this.toolStripButtonBold.Click += new System.EventHandler(this.toolStripButtonBold_Click);
			// 
			// toolStripButtonItalic
			// 
			this.toolStripButtonItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonItalic.Image = global::HtmlEditorDemo.Properties.Resources.Italic;
			this.toolStripButtonItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonItalic.Name = "toolStripButtonItalic";
			this.toolStripButtonItalic.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonItalic.Text = "Italic";
			this.toolStripButtonItalic.Click += new System.EventHandler(this.toolStripButtonItalic_Click);
			// 
			// toolStripButtonUnderline
			// 
			this.toolStripButtonUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonUnderline.Image = global::HtmlEditorDemo.Properties.Resources.Underline;
			this.toolStripButtonUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonUnderline.Name = "toolStripButtonUnderline";
			this.toolStripButtonUnderline.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonUnderline.Text = "Underline";
			this.toolStripButtonUnderline.Click += new System.EventHandler(this.toolStripButtonUnderline_Click);
			// 
			// toolStripButtonPaste
			// 
			this.toolStripButtonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonPaste.Image = global::HtmlEditorDemo.Properties.Resources.Edit_Paste;
			this.toolStripButtonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonPaste.Name = "toolStripButtonPaste";
			this.toolStripButtonPaste.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonPaste.Text = "Paste";
			this.toolStripButtonPaste.Click += new System.EventHandler(this.toolStripButtonPaste_Click);
			// 
			// toolStripButtonCopy
			// 
			this.toolStripButtonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonCopy.Image = global::HtmlEditorDemo.Properties.Resources.Edit_Copy;
			this.toolStripButtonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonCopy.Name = "toolStripButtonCopy";
			this.toolStripButtonCopy.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonCopy.Text = "Copy";
			this.toolStripButtonCopy.Click += new System.EventHandler(this.toolStripButtonCopy_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(647, 262);
			this.Controls.Add(this.htmlEditor);
			this.Controls.Add(this.toolStrip1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.WebBrowser htmlEditor;
		private System.Windows.Forms.ToolStripButton toolStripButtonBold;
		private System.Windows.Forms.ToolStripButton toolStripButtonDefaultText;
		private System.Windows.Forms.ToolStripButton toolStripButtonItalic;
		private System.Windows.Forms.ToolStripButton toolStripButtonUnderline;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton toolStripButtonPaste;
		private System.Windows.Forms.ToolStripButton toolStripButtonCopy;
	}
}

