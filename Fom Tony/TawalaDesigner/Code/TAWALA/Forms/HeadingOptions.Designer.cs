// $Workfile: HeadingOptions.Designer.cs $
// $Revision: 1 $	$Date: 9/13/07 2:46p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
namespace Tawala.Forms
{
	/// <summary>
	/// Options panel for a Fill-in-the-Blank Form Item (FIBItem)
	/// </summary>
	public partial class HeadingOptions
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

		private void InitializeComponent()
		{
			this.panel = new System.Windows.Forms.FlowLayoutPanel();
			this.labelType = new System.Windows.Forms.Label();
			this.comboBoxType = new System.Windows.Forms.ComboBox();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.AutoSize = true;
			this.panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel.Controls.Add(this.labelType);
			this.panel.Controls.Add(this.comboBoxType);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Margin = new System.Windows.Forms.Padding(0);
			this.panel.Name = "panel";
			this.panel.Padding = new System.Windows.Forms.Padding(1);
			this.panel.Size = new System.Drawing.Size(400, 29);
			this.panel.TabIndex = 7;
			// 
			// labelType
			// 
			this.labelType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.labelType.AutoSize = true;
			this.labelType.Location = new System.Drawing.Point(4, 8);
			this.labelType.Name = "labelType";
			this.labelType.Size = new System.Drawing.Size(77, 13);
			this.labelType.TabIndex = 0;
			this.labelType.Text = "Heading Type:";
			this.labelType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxType
			// 
			this.comboBoxType.FormattingEnabled = true;
			this.comboBoxType.Location = new System.Drawing.Point(87, 4);
			this.comboBoxType.Name = "comboBoxType";
			this.comboBoxType.Size = new System.Drawing.Size(121, 21);
			this.comboBoxType.TabIndex = 1;
			this.comboBoxType.SelectionChangeCommitted += new System.EventHandler(this.comboBoxType_SelectionChangeCommitted);
			// 
			// HeadingOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.panel);
			this.MinimumSize = new System.Drawing.Size(400, 20);
			this.Name = "HeadingOptions";
			this.Size = new System.Drawing.Size(400, 29);
			this.panel.ResumeLayout(false);
			this.panel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel panel;
		private System.Windows.Forms.Label labelType;
		private System.Windows.Forms.ComboBox comboBoxType;
	}
}
