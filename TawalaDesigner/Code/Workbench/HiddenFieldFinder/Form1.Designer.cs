namespace HiddenFieldFinder
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
			this.menuStripMain = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.textBoxFileName = new System.Windows.Forms.TextBox();
			this.buttonFindFields = new System.Windows.Forms.Button();
			this.textBoxFields = new System.Windows.Forms.TextBox();
			this.labelFields = new System.Windows.Forms.Label();
			this.menuStripMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStripMain
			// 
			this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStripMain.Location = new System.Drawing.Point(0, 0);
			this.menuStripMain.Name = "menuStripMain";
			this.menuStripMain.Size = new System.Drawing.Size(445, 24);
			this.menuStripMain.TabIndex = 0;
			this.menuStripMain.Text = "menuStripMain";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// textBoxFileName
			// 
			this.textBoxFileName.Location = new System.Drawing.Point(13, 41);
			this.textBoxFileName.Name = "textBoxFileName";
			this.textBoxFileName.ReadOnly = true;
			this.textBoxFileName.Size = new System.Drawing.Size(282, 20);
			this.textBoxFileName.TabIndex = 1;
			// 
			// buttonFindFields
			// 
			this.buttonFindFields.Location = new System.Drawing.Point(316, 39);
			this.buttonFindFields.Name = "buttonFindFields";
			this.buttonFindFields.Size = new System.Drawing.Size(117, 23);
			this.buttonFindFields.TabIndex = 2;
			this.buttonFindFields.Text = "Find Hidden Fields";
			this.buttonFindFields.UseVisualStyleBackColor = true;
			this.buttonFindFields.Click += new System.EventHandler(this.buttonFindFields_Click);
			// 
			// textBoxFields
			// 
			this.textBoxFields.Location = new System.Drawing.Point(13, 92);
			this.textBoxFields.Multiline = true;
			this.textBoxFields.Name = "textBoxFields";
			this.textBoxFields.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxFields.Size = new System.Drawing.Size(420, 225);
			this.textBoxFields.TabIndex = 4;
			// 
			// labelFields
			// 
			this.labelFields.AutoSize = true;
			this.labelFields.Location = new System.Drawing.Point(13, 73);
			this.labelFields.Name = "labelFields";
			this.labelFields.Size = new System.Drawing.Size(74, 13);
			this.labelFields.TabIndex = 5;
			this.labelFields.Text = "Hidden Fields:";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(445, 331);
			this.Controls.Add(this.labelFields);
			this.Controls.Add(this.textBoxFields);
			this.Controls.Add(this.buttonFindFields);
			this.Controls.Add(this.textBoxFileName);
			this.Controls.Add(this.menuStripMain);
			this.MainMenuStrip = this.menuStripMain;
			this.Name = "Form1";
			this.Text = "Hidden Field Finder";
			this.Activated += new System.EventHandler(this.Form1_Activated);
			this.menuStripMain.ResumeLayout(false);
			this.menuStripMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStripMain;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.TextBox textBoxFileName;
		private System.Windows.Forms.Button buttonFindFields;
		private System.Windows.Forms.TextBox textBoxFields;
		private System.Windows.Forms.Label labelFields;
	}
}

