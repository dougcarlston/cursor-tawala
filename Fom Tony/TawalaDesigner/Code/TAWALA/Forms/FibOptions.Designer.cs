// $Workfile: FibOptions.Designer.cs $
// $Revision: 4 $	$Date: 6/13/07 11:29a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
namespace Tawala.Forms
{
	/// <summary>
	/// Options panel for a Fill-in-the-Blank Form Item (FibItemView)
	/// </summary>
	public partial class FibOptions
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
			this.textBoxLength = new System.Windows.Forms.TextBox();
			this.labelLengthTextBox = new System.Windows.Forms.Label();
			this.checkBoxRequired = new System.Windows.Forms.CheckBox();
			this.labelAlternateLabel = new System.Windows.Forms.Label();
			this.textBoxAlternateLabel = new System.Windows.Forms.TextBox();
			this.textBoxHeight = new System.Windows.Forms.TextBox();
			this.labelHeight = new System.Windows.Forms.Label();
			this.panel = new System.Windows.Forms.FlowLayoutPanel();
			this.comboBoxValidation = new System.Windows.Forms.ComboBox();
			this.linkValidationEdit = new System.Windows.Forms.LinkLabel();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxLength
			// 
			this.textBoxLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLength.BackColor = System.Drawing.Color.LightGray;
			this.textBoxLength.Enabled = false;
			this.panel.SetFlowBreak(this.textBoxLength, true);
			this.textBoxLength.Location = new System.Drawing.Point(301, 30);
			this.textBoxLength.Name = "textBoxLength";
			this.textBoxLength.Size = new System.Drawing.Size(32, 20);
			this.textBoxLength.TabIndex = 7;
			this.textBoxLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBoxLength.Visible = false;
			// 
			// labelLengthTextBox
			// 
			this.labelLengthTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.labelLengthTextBox.AutoSize = true;
			this.labelLengthTextBox.BackColor = System.Drawing.Color.LightGray;
			this.labelLengthTextBox.Enabled = false;
			this.labelLengthTextBox.Location = new System.Drawing.Point(255, 34);
			this.labelLengthTextBox.Name = "labelLengthTextBox";
			this.labelLengthTextBox.Size = new System.Drawing.Size(40, 13);
			this.labelLengthTextBox.TabIndex = 6;
			this.labelLengthTextBox.Text = "Length";
			this.labelLengthTextBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelLengthTextBox.Visible = false;
			// 
			// checkBoxRequired
			// 
			this.checkBoxRequired.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxRequired.AutoSize = true;
			this.checkBoxRequired.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBoxRequired.Location = new System.Drawing.Point(7, 31);
			this.checkBoxRequired.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.checkBoxRequired.Name = "checkBoxRequired";
			this.checkBoxRequired.Size = new System.Drawing.Size(75, 18);
			this.checkBoxRequired.TabIndex = 1;
			this.checkBoxRequired.Text = "Required";
			this.checkBoxRequired.CheckedChanged += new System.EventHandler(this.checkBoxRequired_CheckedChanged);
			// 
			// labelAlternateLabel
			// 
			this.labelAlternateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.labelAlternateLabel.AutoSize = true;
			this.labelAlternateLabel.Location = new System.Drawing.Point(4, 7);
			this.labelAlternateLabel.Name = "labelAlternateLabel";
			this.labelAlternateLabel.Size = new System.Drawing.Size(81, 13);
			this.labelAlternateLabel.TabIndex = 2;
			this.labelAlternateLabel.Text = "Alternate Label:";
			this.labelAlternateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxAlternateLabel
			// 
			this.textBoxAlternateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxAlternateLabel.Location = new System.Drawing.Point(91, 4);
			this.textBoxAlternateLabel.Margin = new System.Windows.Forms.Padding(3, 3, 18, 3);
			this.textBoxAlternateLabel.MaxLength = 255;
			this.textBoxAlternateLabel.Name = "textBoxAlternateLabel";
			this.textBoxAlternateLabel.Size = new System.Drawing.Size(120, 20);
			this.textBoxAlternateLabel.TabIndex = 3;
			this.textBoxAlternateLabel.Validated += new System.EventHandler(this.textBoxAlternateLabel_Validated);
			this.textBoxAlternateLabel.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxAlternateLabel_Validating);
			// 
			// textBoxHeight
			// 
			this.textBoxHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.panel.SetFlowBreak(this.textBoxHeight, true);
			this.textBoxHeight.Location = new System.Drawing.Point(279, 4);
			this.textBoxHeight.MaxLength = 2;
			this.textBoxHeight.Name = "textBoxHeight";
			this.textBoxHeight.Size = new System.Drawing.Size(22, 20);
			this.textBoxHeight.TabIndex = 5;
			this.textBoxHeight.Leave += new System.EventHandler(this.textBoxHeight_Leave);
			// 
			// labelHeight
			// 
			this.labelHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.labelHeight.AutoSize = true;
			this.labelHeight.Location = new System.Drawing.Point(232, 7);
			this.labelHeight.Name = "labelHeight";
			this.labelHeight.Size = new System.Drawing.Size(41, 13);
			this.labelHeight.TabIndex = 4;
			this.labelHeight.Text = "Height:";
			this.labelHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel
			// 
			this.panel.AutoSize = true;
			this.panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel.Controls.Add(this.labelAlternateLabel);
			this.panel.Controls.Add(this.textBoxAlternateLabel);
			this.panel.Controls.Add(this.labelHeight);
			this.panel.Controls.Add(this.textBoxHeight);
			this.panel.Controls.Add(this.checkBoxRequired);
			this.panel.Controls.Add(this.comboBoxValidation);
			this.panel.Controls.Add(this.linkValidationEdit);
			this.panel.Controls.Add(this.labelLengthTextBox);
			this.panel.Controls.Add(this.textBoxLength);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Margin = new System.Windows.Forms.Padding(0);
			this.panel.Name = "panel";
			this.panel.Padding = new System.Windows.Forms.Padding(1);
			this.panel.Size = new System.Drawing.Size(337, 55);
			this.panel.TabIndex = 7;
			// 
			// comboBoxValidation
			// 
			this.comboBoxValidation.FormattingEnabled = true;
			this.comboBoxValidation.Items.AddRange(new object[] {
            "  -- No Validation --  "});
			this.comboBoxValidation.Location = new System.Drawing.Point(88, 30);
			this.comboBoxValidation.Name = "comboBoxValidation";
			this.comboBoxValidation.Size = new System.Drawing.Size(121, 21);
			this.comboBoxValidation.TabIndex = 9;
			this.comboBoxValidation.Text = "  -- No Validation --  ";
			// 
			// linkValidationEdit
			// 
			this.linkValidationEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.linkValidationEdit.AutoSize = true;
			this.linkValidationEdit.Location = new System.Drawing.Point(215, 34);
			this.linkValidationEdit.Name = "linkValidationEdit";
			this.linkValidationEdit.Size = new System.Drawing.Size(34, 13);
			this.linkValidationEdit.TabIndex = 10;
			this.linkValidationEdit.TabStop = true;
			this.linkValidationEdit.Text = "Edit...";
			this.linkValidationEdit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEdit_LinkClicked);
			// 
			// FibOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.panel);
			this.Name = "FibOptions";
			this.Size = new System.Drawing.Size(337, 55);
			this.panel.ResumeLayout(false);
			this.panel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxLength;
		private System.Windows.Forms.Label labelLengthTextBox;
		private System.Windows.Forms.CheckBox checkBoxRequired;
		private System.Windows.Forms.Label labelAlternateLabel;
		private System.Windows.Forms.TextBox textBoxAlternateLabel;
		private System.Windows.Forms.TextBox textBoxHeight;
		private System.Windows.Forms.Label labelHeight;
        private System.Windows.Forms.FlowLayoutPanel panel;
        private System.Windows.Forms.ComboBox comboBoxValidation;
        private System.Windows.Forms.LinkLabel linkValidationEdit;
	}
}
