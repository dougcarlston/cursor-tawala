// $Workfile: IfStatementView.Designer.cs $
// $Revision: 2 $	$Date: 1/09/07 9:50a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Processes
{
	partial class IfStatementView
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageIf = new System.Windows.Forms.TabPage();
            this.labelIf2 = new System.Windows.Forms.Label();
            this.comboBoxAndOr = new System.Windows.Forms.ComboBox();
            this.checkBoxOtherwise = new System.Windows.Forms.CheckBox();
            this.buttonAddModify = new Tawala.Processes.AddModifyButton();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.labelIf1 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPageIf.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageIf);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(694, 200);
            this.tabControl.TabIndex = 0;
            this.tabControl.TabStop = false;
            // 
            // tabPageIf
            // 
            this.tabPageIf.Controls.Add(this.labelIf2);
            this.tabPageIf.Controls.Add(this.comboBoxAndOr);
            this.tabPageIf.Controls.Add(this.checkBoxOtherwise);
            this.tabPageIf.Controls.Add(this.buttonAddModify);
            this.tabPageIf.Controls.Add(this.groupBox);
            this.tabPageIf.Controls.Add(this.labelIf1);
            this.tabPageIf.Location = new System.Drawing.Point(4, 22);
            this.tabPageIf.Name = "tabPageIf";
            this.tabPageIf.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIf.Size = new System.Drawing.Size(686, 174);
            this.tabPageIf.TabIndex = 0;
            this.tabPageIf.Text = "If";
            this.tabPageIf.UseVisualStyleBackColor = true;
            // 
            // labelIf2
            // 
            this.labelIf2.Location = new System.Drawing.Point(86, 6);
            this.labelIf2.Margin = new System.Windows.Forms.Padding(0);
            this.labelIf2.Name = "labelIf2";
            this.labelIf2.Size = new System.Drawing.Size(470, 23);
            this.labelIf2.TabIndex = 2;
            this.labelIf2.Text = " place holder text";
            this.labelIf2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxAndOr
            // 
            this.comboBoxAndOr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAndOr.DropDownWidth = 50;
            this.comboBoxAndOr.FormattingEnabled = true;
            this.comboBoxAndOr.Items.AddRange(new object[] {
            "ALL",
            "ANY"});
            this.comboBoxAndOr.Location = new System.Drawing.Point(27, 8);
            this.comboBoxAndOr.MaxDropDownItems = 2;
            this.comboBoxAndOr.Name = "comboBoxAndOr";
            this.comboBoxAndOr.Size = new System.Drawing.Size(50, 21);
            this.comboBoxAndOr.TabIndex = 1;
            this.comboBoxAndOr.Visible = false;
            // 
            // checkBoxOtherwise
            // 
            this.checkBoxOtherwise.Location = new System.Drawing.Point(9, 96);
            this.checkBoxOtherwise.Name = "checkBoxOtherwise";
            this.checkBoxOtherwise.Size = new System.Drawing.Size(256, 26);
            this.checkBoxOtherwise.TabIndex = 4;
            this.checkBoxOtherwise.Text = "Otherwise execute second set of commands";
            // 
            // buttonAddModify
            // 
            this.buttonAddModify.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonAddModify.Location = new System.Drawing.Point(303, 144);
            this.buttonAddModify.Name = "buttonAddModify";
            this.buttonAddModify.Size = new System.Drawing.Size(80, 24);
            this.buttonAddModify.TabIndex = 5;
            this.buttonAddModify.Text = "Add";
            this.buttonAddModify.Click += new System.EventHandler(this.buttonAddModify_Click);
            // 
            // groupBox
            // 
            this.groupBox.Location = new System.Drawing.Point(10, 29);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(660, 56);
            this.groupBox.TabIndex = 3;
            this.groupBox.TabStop = false;
            // 
            // labelIf1
            // 
            this.labelIf1.Location = new System.Drawing.Point(12, 6);
            this.labelIf1.Name = "labelIf1";
            this.labelIf1.Size = new System.Drawing.Size(16, 23);
            this.labelIf1.TabIndex = 0;
            this.labelIf1.Text = "If";
            this.labelIf1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // IfDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tabControl);
            this.Name = "IfDetails";
            this.Size = new System.Drawing.Size(694, 200);
            this.tabControl.ResumeLayout(false);
            this.tabPageIf.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.TabPage tabPageIf;
        private System.Windows.Forms.GroupBox groupBox;
		private AddModifyButton buttonAddModify;
        private System.Windows.Forms.CheckBox checkBoxOtherwise;
        private System.Windows.Forms.ComboBox comboBoxAndOr;
        private System.Windows.Forms.Label labelIf2;
        protected System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.Label labelIf1;
	}
}
