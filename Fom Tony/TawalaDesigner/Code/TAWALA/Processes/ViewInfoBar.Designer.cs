// $Workfile: ViewInfoBar.Designer.cs $
// $Revision: 6 $	$Date: 11/25/05 5:00p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Processes
{
	partial class ViewInfoBar
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
			this.components = new System.ComponentModel.Container();
			this.label = new System.Windows.Forms.Label();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.AutoEllipsis = true;
			this.label.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label.Location = new System.Drawing.Point(0, 0);
			this.label.Margin = new System.Windows.Forms.Padding(0);
			this.label.Name = "label";
			this.label.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this.label.Size = new System.Drawing.Size(348, 22);
			this.label.TabIndex = 0;
			this.label.Text = "<msg here>";
			this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label.Click += new System.EventHandler(this.label_Click);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Enabled = true;
			this.contextMenuStrip.GripMargin = new System.Windows.Forms.Padding(2);
			this.contextMenuStrip.Location = new System.Drawing.Point(21, 36);
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.contextMenuStrip.ShowCheckMargin = true;
			this.contextMenuStrip.ShowImageMargin = false;
			this.contextMenuStrip.Size = new System.Drawing.Size(61, 4);
			this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
			// 
			// ViewInfoBar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(216)))));
			this.Controls.Add(this.label);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "ViewInfoBar";
			this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
			this.Size = new System.Drawing.Size(348, 24);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;



	}
}
