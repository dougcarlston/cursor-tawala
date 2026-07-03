// $Workfile: InvitationView.Designer.cs $
// $Revision: 6 $	$Date: 10/31/06 6:39p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Invite
{
	partial class InvitationView
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBoxSubject = new System.Windows.Forms.TextBox();
			this.labelSubject = new System.Windows.Forms.Label();
			this.panelTop = new System.Windows.Forms.Panel();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.buttonSend = new System.Windows.Forms.Button();
			this.richTextBox = new System.Windows.Forms.RichTextBox();
			this.panelTop.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxSubject
			// 
			this.textBoxSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxSubject.Location = new System.Drawing.Point(61, 8);
			this.textBoxSubject.MaxLength = 255;
			this.textBoxSubject.Name = "textBoxSubject";
			this.textBoxSubject.Size = new System.Drawing.Size(269, 20);
			this.textBoxSubject.TabIndex = 3;
			// 
			// labelSubject
			// 
			this.labelSubject.Location = new System.Drawing.Point(3, 8);
			this.labelSubject.Name = "labelSubject";
			this.labelSubject.Size = new System.Drawing.Size(52, 20);
			this.labelSubject.TabIndex = 2;
			this.labelSubject.Text = "Subject:";
			this.labelSubject.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.textBoxSubject);
			this.panelTop.Controls.Add(this.labelSubject);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Margin = new System.Windows.Forms.Padding(2);
			this.panelTop.Name = "panelTop";
			this.panelTop.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
			this.panelTop.Size = new System.Drawing.Size(349, 38);
			this.panelTop.TabIndex = 1;
			this.panelTop.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTop_Paint);
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.buttonSend);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 245);
			this.panelBottom.Margin = new System.Windows.Forms.Padding(2);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
			this.panelBottom.Size = new System.Drawing.Size(349, 41);
			this.panelBottom.TabIndex = 2;
			this.panelBottom.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBottom_Paint);
			// 
			// buttonSend
			// 
			this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSend.AutoSize = true;
			this.buttonSend.Enabled = false;
			this.buttonSend.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonSend.Location = new System.Drawing.Point(242, 10);
			this.buttonSend.Margin = new System.Windows.Forms.Padding(2);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(101, 22);
			this.buttonSend.TabIndex = 5;
			this.buttonSend.Text = "Send Invitation...";
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// richTextBox
			// 
			this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox.Location = new System.Drawing.Point(0, 38);
			this.richTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.Size = new System.Drawing.Size(349, 207);
			this.richTextBox.TabIndex = 4;
			this.richTextBox.Text = "";
			// 
			// InvitationView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.richTextBox);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.Name = "InvitationView";
			this.Size = new System.Drawing.Size(349, 286);
			this.Load += new System.EventHandler(this.invitationView_Load);
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			this.panelBottom.ResumeLayout(false);
			this.panelBottom.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxSubject;
		private System.Windows.Forms.Label labelSubject;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.RichTextBox richTextBox;
	}
}
