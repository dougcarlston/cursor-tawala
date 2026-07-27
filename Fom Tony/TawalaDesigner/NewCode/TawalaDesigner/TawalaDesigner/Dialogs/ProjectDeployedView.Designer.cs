namespace TawalaDesigner.Dialogs
{
	partial class ProjectDeployedView
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
			this.labelSuccess = new System.Windows.Forms.Label();
			this.labelMessage = new System.Windows.Forms.Label();
			this.comboBoxForms = new System.Windows.Forms.ComboBox();
			this.linkLabelViewSelectedForm = new System.Windows.Forms.LinkLabel();
			this.textBoxFormUrl = new System.Windows.Forms.TextBox();
			this.buttonClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelSuccess
			// 
			this.labelSuccess.Location = new System.Drawing.Point(13, 13);
			this.labelSuccess.Name = "labelSuccess";
			this.labelSuccess.Size = new System.Drawing.Size(429, 23);
			this.labelSuccess.TabIndex = 0;
			this.labelSuccess.Text = "Success or Failure";
			// 
			// labelMessage
			// 
			this.labelMessage.Location = new System.Drawing.Point(13, 43);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(429, 59);
			this.labelMessage.TabIndex = 1;
			this.labelMessage.Text = "Message";
			// 
			// comboBoxForms
			// 
			this.comboBoxForms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxForms.FormattingEnabled = true;
			this.comboBoxForms.Location = new System.Drawing.Point(151, 116);
			this.comboBoxForms.Name = "comboBoxForms";
			this.comboBoxForms.Size = new System.Drawing.Size(153, 21);
			this.comboBoxForms.TabIndex = 2;
			this.comboBoxForms.SelectedIndexChanged += new System.EventHandler(this.comboBoxForms_SelectedIndexChanged);
			// 
			// linkLabelViewSelectedForm
			// 
			this.linkLabelViewSelectedForm.AutoSize = true;
			this.linkLabelViewSelectedForm.LinkArea = new System.Windows.Forms.LinkArea(0, 32);
			this.linkLabelViewSelectedForm.Location = new System.Drawing.Point(147, 150);
			this.linkLabelViewSelectedForm.Name = "linkLabelViewSelectedForm";
			this.linkLabelViewSelectedForm.Size = new System.Drawing.Size(160, 13);
			this.linkLabelViewSelectedForm.TabIndex = 3;
			this.linkLabelViewSelectedForm.TabStop = true;
			this.linkLabelViewSelectedForm.Text = "Click here to view selected Form";
			this.linkLabelViewSelectedForm.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelViewSelectedForm_LinkClicked);
			// 
			// textBoxFormUrl
			// 
			this.textBoxFormUrl.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxFormUrl.Location = new System.Drawing.Point(29, 174);
			this.textBoxFormUrl.Name = "textBoxFormUrl";
			this.textBoxFormUrl.ReadOnly = true;
			this.textBoxFormUrl.Size = new System.Drawing.Size(396, 13);
			this.textBoxFormUrl.TabIndex = 4;
			this.textBoxFormUrl.Text = "Form Url";
			this.textBoxFormUrl.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(190, 209);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 5;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// ProjectDeployedView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(454, 237);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.textBoxFormUrl);
			this.Controls.Add(this.linkLabelViewSelectedForm);
			this.Controls.Add(this.comboBoxForms);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.labelSuccess);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProjectDeployedView";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Project Deployed";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelSuccess;
		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.ComboBox comboBoxForms;
		private System.Windows.Forms.LinkLabel linkLabelViewSelectedForm;
		private System.Windows.Forms.TextBox textBoxFormUrl;
		private System.Windows.Forms.Button buttonClose;
	}
}