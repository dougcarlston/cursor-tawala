// $Workfile: DeployedProject.Designer.cs $
// $Revision: 7 $	$Date: 7/02/07 9:02a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.DesignerUI
{
	partial class DeployedProjectForm
	{
		private System.Windows.Forms.Label labelSuccess;
		private System.Windows.Forms.LinkLabel depLink;
		private System.Windows.Forms.Button btnOK;

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
			this.depLink = new System.Windows.Forms.LinkLabel();
			this.btnOK = new System.Windows.Forms.Button();
			this.labelMessage = new System.Windows.Forms.Label();
			this.textBoxURL = new System.Windows.Forms.TextBox();
			this.comboBoxForms = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// labelSuccess
			// 
			this.labelSuccess.Location = new System.Drawing.Point(17, 16);
			this.labelSuccess.Name = "labelSuccess";
			this.labelSuccess.Size = new System.Drawing.Size(423, 31);
			this.labelSuccess.TabIndex = 0;
			this.labelSuccess.Text = "Success or Failure";
			// 
			// depLink
			// 
			this.depLink.Location = new System.Drawing.Point(30, 144);
			this.depLink.Name = "depLink";
			this.depLink.Size = new System.Drawing.Size(396, 25);
			this.depLink.TabIndex = 1;
			this.depLink.TabStop = true;
			this.depLink.Text = "Click here to view selected Form";
			this.depLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.depLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.depLink_LinkClicked);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(191, 210);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "Close";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// labelMessage
			// 
			this.labelMessage.Location = new System.Drawing.Point(17, 54);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(423, 58);
			this.labelMessage.TabIndex = 3;
			this.labelMessage.Text = "Additional message";
			// 
			// textBoxURL
			// 
			this.textBoxURL.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxURL.Location = new System.Drawing.Point(30, 171);
			this.textBoxURL.Name = "textBoxURL";
			this.textBoxURL.ReadOnly = true;
			this.textBoxURL.Size = new System.Drawing.Size(396, 13);
			this.textBoxURL.TabIndex = 4;
			this.textBoxURL.Text = "Start Point URL";
			this.textBoxURL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// comboBoxForms
			// 
			this.comboBoxForms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxForms.FormattingEnabled = true;
			this.comboBoxForms.Location = new System.Drawing.Point(152, 117);
			this.comboBoxForms.Name = "comboBoxForms";
			this.comboBoxForms.Size = new System.Drawing.Size(153, 21);
			this.comboBoxForms.TabIndex = 5;
			this.comboBoxForms.SelectedIndexChanged += new System.EventHandler(this.comboBoxForms_SelectedIndexChanged);
			// 
			// DeployedProjectForm
			// 
			this.ClientSize = new System.Drawing.Size(456, 239);
			this.Controls.Add(this.comboBoxForms);
			this.Controls.Add(this.textBoxURL);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.depLink);
			this.Controls.Add(this.labelSuccess);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DeployedProjectForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Deployed Project";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.TextBox textBoxURL;
		private System.Windows.Forms.ComboBox comboBoxForms;
	}
}
