// $Workfile: UpdateNotification.Designer.cs $
// $Revision: 5 $	$Date: 6/26/07 1:26p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Common
{
	partial class UpdateNotification
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateNotification));
			this.label1 = new System.Windows.Forms.Label();
			this.labelMandatory = new System.Windows.Forms.Label();
			this.linkLabel = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(47, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(330, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "A new version of Tawala is available on the Tawala website.";
			// 
			// labelMandatory
			// 
			this.labelMandatory.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.labelMandatory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.labelMandatory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMandatory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.labelMandatory.Location = new System.Drawing.Point(0, 91);
			this.labelMandatory.Name = "labelMandatory";
			this.labelMandatory.Padding = new System.Windows.Forms.Padding(30, 0, 30, 4);
			this.labelMandatory.Size = new System.Drawing.Size(424, 39);
			this.labelMandatory.TabIndex = 5;
			this.labelMandatory.Text = "Note:  You will need to update to the new version before you can deploy a project" +
				".";
			this.labelMandatory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelMandatory.Visible = false;
			// 
			// linkLabel
			// 
			this.linkLabel.AutoSize = true;
			this.linkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkLabel.Location = new System.Drawing.Point(54, 52);
			this.linkLabel.Name = "linkLabel";
			this.linkLabel.Size = new System.Drawing.Size(316, 15);
			this.linkLabel.TabIndex = 6;
			this.linkLabel.TabStop = true;
			this.linkLabel.Text = "Click here to find out what\'s new and get the new version.";
			this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// UpdateNotification
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(424, 130);
			this.Controls.Add(this.linkLabel);
			this.Controls.Add(this.labelMandatory);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdateNotification";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Version of Tawala Available";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelMandatory;
		private System.Windows.Forms.LinkLabel linkLabel;
	}
}