// $Workfile: WebRequestFailure.Designer.cs $
// $Revision: 7 $	$Date: 8/13/07 11:29a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Common
{
	partial class WebRequestFailure
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
			this.labelErrorMessage = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(322, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "The following error occurred while trying to deploy the project:";
			// 
			// labelErrorMessage
			// 
			this.labelErrorMessage.Location = new System.Drawing.Point(25, 30);
			this.labelErrorMessage.Name = "labelErrorMessage";
			this.labelErrorMessage.Size = new System.Drawing.Size(302, 38);
			this.labelErrorMessage.TabIndex = 4;
			this.labelErrorMessage.Text = "ErrorMessage";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(15, 72);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(322, 50);
			this.label4.TabIndex = 5;
			this.label4.Text = "Press Retry to attempt deployment again at this time.\r\n\r\nPress Cancel to abort an" +
				"d try again later.";
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.button1.Location = new System.Drawing.Point(89, 126);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(73, 23);
			this.button1.TabIndex = 6;
			this.button1.Text = "Retry";
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(191, 127);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(73, 23);
			this.button2.TabIndex = 7;
			this.button2.Text = "Cancel";
			// 
			// WebRequestFailure
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(352, 157);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelErrorMessage);
			this.Controls.Add(this.label1);
			this.Name = "WebRequestFailure";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Web Request Failure";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelErrorMessage;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
	}
}