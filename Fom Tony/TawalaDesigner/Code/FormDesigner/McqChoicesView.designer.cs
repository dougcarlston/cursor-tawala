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
			this.textBoxChoices = new System.Windows.Forms.TextBox();
			this.panelOkCancel = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.comboBoxSource = new System.Windows.Forms.ComboBox();
			this.linkLabelConfigure = new System.Windows.Forms.LinkLabel();
			this.panelOkCancel.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxChoices
			// 
			this.textBoxChoices.AllowDrop = true;
			this.textBoxChoices.Location = new System.Drawing.Point(12, 12);
			this.textBoxChoices.Multiline = true;
			this.textBoxChoices.Name = "textBoxChoices";
			this.textBoxChoices.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxChoices.Size = new System.Drawing.Size(241, 178);
			this.textBoxChoices.TabIndex = 0;
			this.textBoxChoices.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxChoices_DragDrop);
			this.textBoxChoices.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxChoices_DragEnter);
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
			// McqChoicesView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(267, 270);
			this.Controls.Add(this.linkLabelConfigure);
			this.Controls.Add(this.comboBoxSource);
			this.Controls.Add(this.textBoxChoices);
			this.Controls.Add(this.panelOkCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MinimumSize = new System.Drawing.Size(200, 298);
			this.Name = "McqChoicesView";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Edit Choices";
			this.Load += new System.EventHandler(this.McqChoicesView_Load);
			this.panelOkCancel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxChoices;
		private System.Windows.Forms.Panel panelOkCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ComboBox comboBoxSource;
		private System.Windows.Forms.LinkLabel linkLabelConfigure;
	}
}