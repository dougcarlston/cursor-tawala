namespace Tawala.DesignerUI
{
	partial class PageHeaderDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageHeaderDialog));
			this.textBoxHeader = new System.Windows.Forms.TextBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonBrowseImage = new System.Windows.Forms.Button();
			this.buttonRemoveImage = new System.Windows.Forms.Button();
			this.groupBoxImage = new System.Windows.Forms.GroupBox();
			this.panelImage = new System.Windows.Forms.Panel();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.groupBoxText = new System.Windows.Forms.GroupBox();
			this.panel = new System.Windows.Forms.Panel();
			this.groupBoxImage.SuspendLayout();
			this.panelImage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.groupBoxText.SuspendLayout();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxHeader
			// 
			this.textBoxHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxHeader.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxHeader.Location = new System.Drawing.Point(10, 20);
			this.textBoxHeader.Margin = new System.Windows.Forms.Padding(0);
			this.textBoxHeader.MaxLength = 256;
			this.textBoxHeader.MinimumSize = new System.Drawing.Size(604, 26);
			this.textBoxHeader.Name = "textBoxHeader";
			this.textBoxHeader.Size = new System.Drawing.Size(604, 26);
			this.textBoxHeader.TabIndex = 2;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(233, 342);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(336, 342);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonBrowseImage
			// 
			this.buttonBrowseImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonBrowseImage.AutoSize = true;
			this.buttonBrowseImage.BackColor = System.Drawing.SystemColors.Control;
			this.buttonBrowseImage.Image = global::Tawala.DesignerUI.Properties.Resources.Folder_Open;
			this.buttonBrowseImage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonBrowseImage.Location = new System.Drawing.Point(534, 18);
			this.buttonBrowseImage.Margin = new System.Windows.Forms.Padding(3, 18, 6, 3);
			this.buttonBrowseImage.Name = "buttonBrowseImage";
			this.buttonBrowseImage.Size = new System.Drawing.Size(84, 23);
			this.buttonBrowseImage.TabIndex = 4;
			this.buttonBrowseImage.Text = "Browse...";
			this.buttonBrowseImage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonBrowseImage.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.buttonBrowseImage.UseVisualStyleBackColor = true;
			this.buttonBrowseImage.Click += new System.EventHandler(this.buttonBrowseImage_Click);
			// 
			// buttonRemoveImage
			// 
			this.buttonRemoveImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemoveImage.AutoSize = true;
			this.buttonRemoveImage.Image = global::Tawala.DesignerUI.Properties.Resources.Edit_Delete;
			this.buttonRemoveImage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonRemoveImage.Location = new System.Drawing.Point(534, 46);
			this.buttonRemoveImage.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
			this.buttonRemoveImage.Name = "buttonRemoveImage";
			this.buttonRemoveImage.Size = new System.Drawing.Size(84, 23);
			this.buttonRemoveImage.TabIndex = 5;
			this.buttonRemoveImage.Text = "Remove";
			this.buttonRemoveImage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonRemoveImage.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.buttonRemoveImage.UseVisualStyleBackColor = true;
			this.buttonRemoveImage.Click += new System.EventHandler(this.buttonRemoveImage_Click);
			// 
			// groupBoxImage
			// 
			this.groupBoxImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxImage.Controls.Add(this.panelImage);
			this.groupBoxImage.Controls.Add(this.buttonRemoveImage);
			this.groupBoxImage.Controls.Add(this.buttonBrowseImage);
			this.groupBoxImage.Location = new System.Drawing.Point(10, 78);
			this.groupBoxImage.Margin = new System.Windows.Forms.Padding(0);
			this.groupBoxImage.MinimumSize = new System.Drawing.Size(624, 250);
			this.groupBoxImage.Name = "groupBoxImage";
			this.groupBoxImage.Size = new System.Drawing.Size(624, 250);
			this.groupBoxImage.TabIndex = 3;
			this.groupBoxImage.TabStop = false;
			this.groupBoxImage.Text = "Image";
			// 
			// panelImage
			// 
			this.panelImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelImage.AutoScroll = true;
			this.panelImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelImage.Controls.Add(this.pictureBox);
			this.panelImage.Location = new System.Drawing.Point(8, 18);
			this.panelImage.Margin = new System.Windows.Forms.Padding(0);
			this.panelImage.MinimumSize = new System.Drawing.Size(520, 220);
			this.panelImage.Name = "panelImage";
			this.panelImage.Size = new System.Drawing.Size(520, 220);
			this.panelImage.TabIndex = 6;
			// 
			// pictureBox
			// 
			this.pictureBox.ErrorImage = null;
			this.pictureBox.InitialImage = null;
			this.pictureBox.Location = new System.Drawing.Point(0, 0);
			this.pictureBox.Margin = new System.Windows.Forms.Padding(0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(100, 50);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			this.pictureBox.WaitOnLoad = true;
			// 
			// groupBoxText
			// 
			this.groupBoxText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.groupBoxText.Controls.Add(this.textBoxHeader);
			this.groupBoxText.Location = new System.Drawing.Point(10, 12);
			this.groupBoxText.Margin = new System.Windows.Forms.Padding(0);
			this.groupBoxText.MinimumSize = new System.Drawing.Size(624, 56);
			this.groupBoxText.Name = "groupBoxText";
			this.groupBoxText.Padding = new System.Windows.Forms.Padding(0);
			this.groupBoxText.Size = new System.Drawing.Size(624, 56);
			this.groupBoxText.TabIndex = 1;
			this.groupBoxText.TabStop = false;
			this.groupBoxText.Text = "Text";
			// 
			// panel
			// 
			this.panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.panel.Controls.Add(this.buttonCancel);
			this.panel.Controls.Add(this.buttonOK);
			this.panel.Controls.Add(this.groupBoxText);
			this.panel.Controls.Add(this.groupBoxImage);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Margin = new System.Windows.Forms.Padding(0);
			this.panel.MinimumSize = new System.Drawing.Size(645, 378);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(645, 378);
			this.panel.TabIndex = 2;
			// 
			// PageHeaderDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(645, 378);
			this.Controls.Add(this.panel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(661, 414);
			this.Name = "PageHeaderDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Page Header";
			this.groupBoxImage.ResumeLayout(false);
			this.groupBoxImage.PerformLayout();
			this.panelImage.ResumeLayout(false);
			this.panelImage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.groupBoxText.ResumeLayout(false);
			this.groupBoxText.PerformLayout();
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxHeader;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonBrowseImage;
		private System.Windows.Forms.Button buttonRemoveImage;
		private System.Windows.Forms.GroupBox groupBoxImage;
		private System.Windows.Forms.GroupBox groupBoxText;
		private System.Windows.Forms.Panel panelImage;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.Panel panel;
	}
}