// $Workfile: LoginForm.Designer.cs $
// $Revision: 4 $	$Date: 11/25/05 5:00p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Common
{
	partial class LoginForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxUserName = new System.Windows.Forms.TextBox();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.buttonSubmit = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(290, 34);
			this.label1.TabIndex = 0;
			this.label1.Text = "Please enter your user name and password as they are registered with Tawala.com.";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(58, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Username:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Password:";
			// 
			// textBoxUserName
			// 
			this.textBoxUserName.Location = new System.Drawing.Point(72, 49);
			this.textBoxUserName.Name = "textBoxUserName";
			this.textBoxUserName.Size = new System.Drawing.Size(230, 20);
			this.textBoxUserName.TabIndex = 3;
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(72, 85);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.PasswordChar = '•';
			this.textBoxPassword.Size = new System.Drawing.Size(230, 20);
			this.textBoxPassword.TabIndex = 4;
			// 
			// buttonSubmit
			// 
			this.buttonSubmit.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonSubmit.Location = new System.Drawing.Point(79, 121);
			this.buttonSubmit.Name = "buttonSubmit";
			this.buttonSubmit.Size = new System.Drawing.Size(67, 23);
			this.buttonSubmit.TabIndex = 5;
			this.buttonSubmit.Text = "Submit";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(168, 121);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(67, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			// 
			// LoginForm
			// 
			this.AcceptButton = this.buttonSubmit;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(314, 159);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonSubmit);
			this.Controls.Add(this.textBoxPassword);
			this.Controls.Add(this.textBoxUserName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Login";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxUserName;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.Button buttonSubmit;
		private System.Windows.Forms.Button buttonCancel;
	}
}