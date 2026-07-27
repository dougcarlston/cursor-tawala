// $Workfile: FibOptions.Designer.cs $
// $Revision: 4 $	$Date: 6/13/07 11:29a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
namespace Tawala.Forms
{
	/// <summary>
	/// Options panel for a Fill-in-the-Blank Form Item (FIBItem)
	/// </summary>
    public partial class FileUploadItemViewOptions
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
			this.checkBoxRequired = new System.Windows.Forms.CheckBox();
			this.panel = new System.Windows.Forms.FlowLayoutPanel();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkBoxRequired
			// 
			this.checkBoxRequired.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxRequired.AutoSize = true;
			this.checkBoxRequired.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBoxRequired.Location = new System.Drawing.Point(4, 5);
			this.checkBoxRequired.Name = "checkBoxRequired";
			this.checkBoxRequired.Size = new System.Drawing.Size(121, 18);
			this.checkBoxRequired.TabIndex = 1;
			this.checkBoxRequired.Text = "Response required";
			this.checkBoxRequired.CheckedChanged += new System.EventHandler(this.checkBoxRequired_CheckedChanged);
			// 
			// panel
			// 
			this.panel.AutoSize = true;
			this.panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel.Controls.Add(this.checkBoxRequired);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Margin = new System.Windows.Forms.Padding(0);
			this.panel.Name = "panel";
			this.panel.Padding = new System.Windows.Forms.Padding(1);
			this.panel.Size = new System.Drawing.Size(516, 28);
			this.panel.TabIndex = 7;
			// 
			// FibOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.panel);
			this.Name = "FileUploadItemViewOptions";
			this.Size = new System.Drawing.Size(516, 28);
			this.panel.ResumeLayout(false);
			this.panel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxRequired;
		private System.Windows.Forms.FlowLayoutPanel panel;
	}
}
