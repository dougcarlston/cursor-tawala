// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Forms.Dialogs
{
	partial class FormItemConditionalDisplayView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormItemConditionalDisplayView));
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.checkBoxConditionalDisplay = new System.Windows.Forms.CheckBox();
			this.labelWhen1 = new System.Windows.Forms.Label();
			this.labelWhen2 = new System.Windows.Forms.Label();
			this.comboBoxAndOr = new System.Windows.Forms.ComboBox();
			this.groupBoxConditions = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(361, 146);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 9;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// okButton
			// 
			this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(258, 146);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 8;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// checkBoxConditionalDisplay
			// 
			this.checkBoxConditionalDisplay.AutoSize = true;
			this.checkBoxConditionalDisplay.Location = new System.Drawing.Point(12, 12);
			this.checkBoxConditionalDisplay.Name = "checkBoxConditionalDisplay";
			this.checkBoxConditionalDisplay.Size = new System.Drawing.Size(165, 17);
			this.checkBoxConditionalDisplay.TabIndex = 10;
			this.checkBoxConditionalDisplay.Text = "Display this item conditionally ";
			this.checkBoxConditionalDisplay.UseVisualStyleBackColor = true;
			this.checkBoxConditionalDisplay.CheckedChanged += new System.EventHandler(this.checkBoxConditionalDisplay_CheckedChanged);
			// 
			// labelWhen1
			// 
			this.labelWhen1.AutoSize = true;
			this.labelWhen1.Enabled = false;
			this.labelWhen1.Location = new System.Drawing.Point(28, 47);
			this.labelWhen1.Name = "labelWhen1";
			this.labelWhen1.Size = new System.Drawing.Size(92, 13);
			this.labelWhen1.TabIndex = 11;
			this.labelWhen1.Text = "Display only when";
			// 
			// labelWhen2
			// 
			this.labelWhen2.Enabled = false;
			this.labelWhen2.Location = new System.Drawing.Point(177, 42);
			this.labelWhen2.Margin = new System.Windows.Forms.Padding(0);
			this.labelWhen2.Name = "labelWhen2";
			this.labelWhen2.Size = new System.Drawing.Size(470, 23);
			this.labelWhen2.TabIndex = 13;
			this.labelWhen2.Text = "of the following conditions are true:";
			this.labelWhen2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelWhen2.Visible = false;
			// 
			// comboBoxAndOr
			// 
			this.comboBoxAndOr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxAndOr.DropDownWidth = 50;
			this.comboBoxAndOr.Enabled = false;
			this.comboBoxAndOr.FormattingEnabled = true;
			this.comboBoxAndOr.Items.AddRange(new object[] {
            "ALL",
            "ANY"});
			this.comboBoxAndOr.Location = new System.Drawing.Point(124, 43);
			this.comboBoxAndOr.MaxDropDownItems = 2;
			this.comboBoxAndOr.Name = "comboBoxAndOr";
			this.comboBoxAndOr.Size = new System.Drawing.Size(50, 21);
			this.comboBoxAndOr.TabIndex = 12;
			this.comboBoxAndOr.Visible = false;
			this.comboBoxAndOr.VisibleChanged += new System.EventHandler(this.comboBoxAndOr_VisibleChanged);
			// 
			// groupBoxConditions
			// 
			this.groupBoxConditions.Enabled = false;
			this.groupBoxConditions.Location = new System.Drawing.Point(12, 70);
			this.groupBoxConditions.Name = "groupBoxConditions";
			this.groupBoxConditions.Size = new System.Drawing.Size(660, 56);
			this.groupBoxConditions.TabIndex = 14;
			this.groupBoxConditions.TabStop = false;
			this.groupBoxConditions.Layout += new System.Windows.Forms.LayoutEventHandler(this.groupBoxConditions_Layout);
			// 
			// FormItemConditionalDisplayView
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(694, 179);
			this.Controls.Add(this.labelWhen2);
			this.Controls.Add(this.comboBoxAndOr);
			this.Controls.Add(this.groupBoxConditions);
			this.Controls.Add(this.labelWhen1);
			this.Controls.Add(this.checkBoxConditionalDisplay);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormItemConditionalDisplayView";
			this.ShowInTaskbar = false;
			this.Text = "Conditional Display of Form Item";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.CheckBox checkBoxConditionalDisplay;
		private System.Windows.Forms.Label labelWhen1;
		private System.Windows.Forms.ComboBox comboBoxAndOr;
		private System.Windows.Forms.GroupBox groupBoxConditions;
		private System.Windows.Forms.Label labelWhen2;
	}
}