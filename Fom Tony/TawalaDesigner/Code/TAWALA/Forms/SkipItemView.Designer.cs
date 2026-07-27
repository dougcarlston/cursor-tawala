// $Workfile: SkipItemView.Designer.cs $
// $Revision: 2 $	$Date: 11/16/06 4:44p $
// Copyright © 2005 Tawala Systems. All rights reserved.

namespace Tawala.Forms
{
	partial class SkipItemView
	{
		private System.Windows.Forms.LinkLabel linkLabel;

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
			this.linkLabel = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// linkLabel
			// 
			this.linkLabel.BackColor = System.Drawing.Color.NavajoWhite;
			this.linkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkLabel.LinkArea = new System.Windows.Forms.LinkArea(1, 4);
			this.linkLabel.Location = new System.Drawing.Point(80, 0);
			this.linkLabel.Margin = new System.Windows.Forms.Padding(0);
			this.linkLabel.Name = "linkLabel";
			this.linkLabel.Size = new System.Drawing.Size(256, 23);
			this.linkLabel.TabIndex = 1;
			this.linkLabel.TabStop = true;
			this.linkLabel.Text = " Edit   ({0})  ";
			this.linkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.linkLabel.UseCompatibleTextRendering = true;
			this.linkLabel.VisitedLinkColor = System.Drawing.Color.Blue;
			this.linkLabel.Click += new System.EventHandler(this.linkLabel_Click);
			this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// SkipItemView
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.linkLabel);
			this.Name = "SkipItemView";
			this.Size = new System.Drawing.Size(336, 24);
			this.ResumeLayout(false);

		}

		#endregion

	}
}

