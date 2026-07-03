// $Workfile: MCOptions.Designer.cs $
// $Revision: 5 $	$Date: 2/18/08 2:05p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Tawala.Forms
{
	public partial class McqOptions : OptionsView
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
			this.checkBoxMoreThanOneAllowed = new System.Windows.Forms.CheckBox();
			this.panel = new System.Windows.Forms.FlowLayoutPanel();
			this.comboBoxSource = new System.Windows.Forms.ComboBox();
			this.linkLabelEdit = new System.Windows.Forms.LinkLabel();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkBoxRequired
			// 
			this.checkBoxRequired.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxRequired.AutoSize = true;
			this.checkBoxRequired.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBoxRequired.Location = new System.Drawing.Point(4, 31);
			this.checkBoxRequired.Name = "checkBoxRequired";
			this.checkBoxRequired.Size = new System.Drawing.Size(121, 18);
			this.checkBoxRequired.TabIndex = 2;
			this.checkBoxRequired.Text = "Response required";
			this.checkBoxRequired.CheckedChanged += new System.EventHandler(this.checkBoxRequired_CheckedChanged);
			// 
			// checkBoxMoreThanOneAllowed
			// 
			this.checkBoxMoreThanOneAllowed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxMoreThanOneAllowed.AutoSize = true;
			this.checkBoxMoreThanOneAllowed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBoxMoreThanOneAllowed.Location = new System.Drawing.Point(4, 5);
			this.checkBoxMoreThanOneAllowed.Name = "checkBoxMoreThanOneAllowed";
			this.checkBoxMoreThanOneAllowed.Size = new System.Drawing.Size(178, 18);
			this.checkBoxMoreThanOneAllowed.TabIndex = 3;
			this.checkBoxMoreThanOneAllowed.Text = "User may select more than one";
			this.checkBoxMoreThanOneAllowed.CheckedChanged += new System.EventHandler(this.checkBoxOnlyOne_CheckedChanged);
			// 
			// panel
			// 
			this.panel.AutoSize = true;
			this.panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel.Controls.Add(this.checkBoxMoreThanOneAllowed);
			this.panel.Controls.Add(this.comboBoxSource);
			this.panel.Controls.Add(this.linkLabelEdit);
			this.panel.Controls.Add(this.checkBoxRequired);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Margin = new System.Windows.Forms.Padding(0);
			this.panel.Name = "panel";
			this.panel.Padding = new System.Windows.Forms.Padding(1);
			this.panel.Size = new System.Drawing.Size(396, 53);
			this.panel.TabIndex = 4;
			// 
			// comboBoxSource
			// 
			this.comboBoxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSource.FormattingEnabled = true;
			this.comboBoxSource.Items.AddRange(new object[] {
            "Choices are entered above",
            "Choices are from stored data"});
			this.comboBoxSource.Location = new System.Drawing.Point(188, 4);
			this.comboBoxSource.Name = "comboBoxSource";
			this.comboBoxSource.Size = new System.Drawing.Size(173, 21);
			this.comboBoxSource.TabIndex = 3;
			this.comboBoxSource.SelectedIndexChanged += new System.EventHandler(this.comboBoxSource_SelectedIndexChanged);
			// 
			// linkLabelEdit
			// 
			this.linkLabelEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.linkLabelEdit.AutoSize = true;
			this.linkLabelEdit.Enabled = false;
			this.panel.SetFlowBreak(this.linkLabelEdit, true);
			this.linkLabelEdit.Location = new System.Drawing.Point(367, 8);
			this.linkLabelEdit.Name = "linkLabelEdit";
			this.linkLabelEdit.Size = new System.Drawing.Size(25, 13);
			this.linkLabelEdit.TabIndex = 4;
			this.linkLabelEdit.TabStop = true;
			this.linkLabelEdit.Text = "Edit";
			this.linkLabelEdit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEdit_LinkClicked);
			// 
			// McqOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.panel);
			this.Name = "McqOptions";
			this.Size = new System.Drawing.Size(396, 53);
			this.panel.ResumeLayout(false);
			this.panel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxRequired;
		private System.Windows.Forms.CheckBox checkBoxMoreThanOneAllowed;
		private FlowLayoutPanel panel;
		private ComboBox comboBoxSource;
		private LinkLabel linkLabelEdit;
	}
}
