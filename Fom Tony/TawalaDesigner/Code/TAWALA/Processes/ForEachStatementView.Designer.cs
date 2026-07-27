// $Workfile: ForEachStatementView.Designer.cs $
// $Revision: 5 $	$Date: 1/26/07 2:27p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Processes
{
	partial class ForEachStatementView
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
            this.tabPageRecord = new System.Windows.Forms.TabPage();
            this.comboBoxRecordSets = new System.Windows.Forms.ComboBox();
            this.labelIn = new System.Windows.Forms.Label();
            this.comboBoxRecords = new System.Windows.Forms.ComboBox();
            this.buttonAddModifyRecord = new Tawala.Processes.AddModifyButton();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageRecord.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageRecord
            // 
            this.tabPageRecord.Controls.Add(this.comboBoxRecordSets);
            this.tabPageRecord.Controls.Add(this.labelIn);
            this.tabPageRecord.Controls.Add(this.comboBoxRecords);
            this.tabPageRecord.Controls.Add(this.buttonAddModifyRecord);
            this.tabPageRecord.Location = new System.Drawing.Point(4, 22);
            this.tabPageRecord.Name = "tabPageRecord";
            this.tabPageRecord.Size = new System.Drawing.Size(424, 150);
            this.tabPageRecord.TabIndex = 0;
            this.tabPageRecord.Text = "Record";
            this.tabPageRecord.UseVisualStyleBackColor = true;
            // 
            // comboBoxRecordSets
            // 
            this.comboBoxRecordSets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRecordSets.FormattingEnabled = true;
            this.comboBoxRecordSets.Location = new System.Drawing.Point(237, 16);
            this.comboBoxRecordSets.Name = "comboBoxRecordSets";
            this.comboBoxRecordSets.Size = new System.Drawing.Size(165, 21);
            this.comboBoxRecordSets.TabIndex = 1;
            // 
            // labelIn
            // 
            this.labelIn.Location = new System.Drawing.Point(194, 16);
            this.labelIn.Name = "labelIn";
            this.labelIn.Size = new System.Drawing.Size(34, 23);
            this.labelIn.TabIndex = 1;
            this.labelIn.Text = "in";
            this.labelIn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBoxRecords
            // 
            this.comboBoxRecords.FormattingEnabled = true;
            this.comboBoxRecords.Location = new System.Drawing.Point(12, 16);
            this.comboBoxRecords.Name = "comboBoxRecords";
            this.comboBoxRecords.Size = new System.Drawing.Size(172, 21);
            this.comboBoxRecords.TabIndex = 0;
            // 
            // buttonAddModifyRecord
            // 
            this.buttonAddModifyRecord.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonAddModifyRecord.Location = new System.Drawing.Point(172, 124);
            this.buttonAddModifyRecord.Name = "buttonAddModifyRecord";
            this.buttonAddModifyRecord.Size = new System.Drawing.Size(80, 24);
            this.buttonAddModifyRecord.TabIndex = 2;
            this.buttonAddModifyRecord.Text = "Add";
            this.buttonAddModifyRecord.Click += new System.EventHandler(this.buttonAddModifyRecord_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageRecord);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(432, 176);
            this.tabControl.TabIndex = 2;
            this.tabControl.TabStop = false;
            // 
            // ForEachStatementView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Name = "ForEachStatementView";
            this.tabPageRecord.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.TabPage tabPageRecord;
        private System.Windows.Forms.ComboBox comboBoxRecordSets;
        private System.Windows.Forms.Label labelIn;
        private System.Windows.Forms.ComboBox comboBoxRecords;
        private AddModifyButton buttonAddModifyRecord;
        private System.Windows.Forms.TabControl tabControl;

    }
}
