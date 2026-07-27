// $Workfile: ShowStatementView.Designer.cs $
// $Revision: 10 $	$Date: 2/28/08 2:01p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
namespace Tawala.Processes
{
	partial class ShowStatementView 
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
			this.comboBoxDoc = new System.Windows.Forms.ComboBox();
			this.buttonAddModifyDoc = new Tawala.Processes.AddModifyButton();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageDocument = new System.Windows.Forms.TabPage();
			this.label1 = new System.Windows.Forms.Label();
			this.checkBoxReset = new System.Windows.Forms.CheckBox();
			this.tabPageForm = new System.Windows.Forms.TabPage();
			this.comboBoxForm = new System.Windows.Forms.ComboBox();
			this.buttonAddModifyForm = new Tawala.Processes.AddModifyButton();
			this.tabPageStoredRecord = new System.Windows.Forms.TabPage();
			this.buttonAddModifyRecord = new Tawala.Processes.AddModifyButton();
			this.labelWhere2 = new System.Windows.Forms.Label();
			this.comboBoxAllAny = new System.Windows.Forms.ComboBox();
			this.groupBoxWhere = new System.Windows.Forms.GroupBox();
			this.labelWhere = new System.Windows.Forms.Label();
			this.labelFrom = new System.Windows.Forms.Label();
			this.labelUponSubmit = new System.Windows.Forms.Label();
			this.radioButtonModify = new System.Windows.Forms.RadioButton();
			this.radioButtonCreate = new System.Windows.Forms.RadioButton();
			this.comboBoxRecordForm = new System.Windows.Forms.ComboBox();
			this.tabPageUrl = new System.Windows.Forms.TabPage();
			this.buttonAddModifyUrl = new Tawala.Processes.AddModifyButton();
			this.labelUrl = new System.Windows.Forms.Label();
			this.textBoxUrl = new System.Windows.Forms.TextBox();
			this.tabControl.SuspendLayout();
			this.tabPageDocument.SuspendLayout();
			this.tabPageForm.SuspendLayout();
			this.tabPageStoredRecord.SuspendLayout();
			this.tabPageUrl.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBoxDoc
			// 
			this.comboBoxDoc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDoc.FormattingEnabled = true;
			this.comboBoxDoc.Location = new System.Drawing.Point(8, 8);
			this.comboBoxDoc.MaxDropDownItems = 20;
			this.comboBoxDoc.Name = "comboBoxDoc";
			this.comboBoxDoc.Size = new System.Drawing.Size(192, 21);
			this.comboBoxDoc.TabIndex = 0;
			// 
			// buttonAddModifyDoc
			// 
			this.buttonAddModifyDoc.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonAddModifyDoc.Location = new System.Drawing.Point(207, 149);
			this.buttonAddModifyDoc.Name = "buttonAddModifyDoc";
			this.buttonAddModifyDoc.Size = new System.Drawing.Size(80, 24);
			this.buttonAddModifyDoc.TabIndex = 1;
			this.buttonAddModifyDoc.Text = "Add";
			this.buttonAddModifyDoc.Click += new System.EventHandler(this.buttonAddModifyDoc_Click);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageDocument);
			this.tabControl.Controls.Add(this.tabPageForm);
			this.tabControl.Controls.Add(this.tabPageStoredRecord);
			this.tabControl.Controls.Add(this.tabPageUrl);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(503, 201);
			this.tabControl.TabIndex = 0;
			this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
			// 
			// tabPageDocument
			// 
			this.tabPageDocument.Controls.Add(this.label1);
			this.tabPageDocument.Controls.Add(this.checkBoxReset);
			this.tabPageDocument.Controls.Add(this.comboBoxDoc);
			this.tabPageDocument.Controls.Add(this.buttonAddModifyDoc);
			this.tabPageDocument.Location = new System.Drawing.Point(4, 22);
			this.tabPageDocument.Name = "tabPageDocument";
			this.tabPageDocument.Size = new System.Drawing.Size(495, 175);
			this.tabPageDocument.TabIndex = 0;
			this.tabPageDocument.Text = "Document";
			this.tabPageDocument.UseVisualStyleBackColor = true;
			this.tabPageDocument.Layout += new System.Windows.Forms.LayoutEventHandler(this.tabPageDocument_Layout);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(29, 61);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(179, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "(removes any appended documents)";
			// 
			// checkBoxReset
			// 
			this.checkBoxReset.AutoSize = true;
			this.checkBoxReset.Location = new System.Drawing.Point(14, 45);
			this.checkBoxReset.Name = "checkBoxReset";
			this.checkBoxReset.Size = new System.Drawing.Size(232, 17);
			this.checkBoxReset.TabIndex = 2;
			this.checkBoxReset.Text = "Reset document to original state after Show";
			this.checkBoxReset.UseVisualStyleBackColor = true;
			// 
			// tabPageForm
			// 
			this.tabPageForm.Controls.Add(this.comboBoxForm);
			this.tabPageForm.Controls.Add(this.buttonAddModifyForm);
			this.tabPageForm.Location = new System.Drawing.Point(4, 22);
			this.tabPageForm.Name = "tabPageForm";
			this.tabPageForm.Size = new System.Drawing.Size(495, 175);
			this.tabPageForm.TabIndex = 1;
			this.tabPageForm.Text = "Form";
			this.tabPageForm.UseVisualStyleBackColor = true;
			this.tabPageForm.Layout += new System.Windows.Forms.LayoutEventHandler(this.tabPageForm_Layout);
			// 
			// comboBoxForm
			// 
			this.comboBoxForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxForm.FormattingEnabled = true;
			this.comboBoxForm.Location = new System.Drawing.Point(8, 8);
			this.comboBoxForm.MaxDropDownItems = 20;
			this.comboBoxForm.Name = "comboBoxForm";
			this.comboBoxForm.Size = new System.Drawing.Size(192, 21);
			this.comboBoxForm.TabIndex = 0;
			// 
			// buttonAddModifyForm
			// 
			this.buttonAddModifyForm.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonAddModifyForm.Location = new System.Drawing.Point(207, 149);
			this.buttonAddModifyForm.Name = "buttonAddModifyForm";
			this.buttonAddModifyForm.Size = new System.Drawing.Size(80, 24);
			this.buttonAddModifyForm.TabIndex = 1;
			this.buttonAddModifyForm.Text = "Add";
			this.buttonAddModifyForm.Click += new System.EventHandler(this.buttonAddModifyForm_Click);
			// 
			// tabPageStoredRecord
			// 
			this.tabPageStoredRecord.Controls.Add(this.buttonAddModifyRecord);
			this.tabPageStoredRecord.Controls.Add(this.labelWhere2);
			this.tabPageStoredRecord.Controls.Add(this.comboBoxAllAny);
			this.tabPageStoredRecord.Controls.Add(this.groupBoxWhere);
			this.tabPageStoredRecord.Controls.Add(this.labelWhere);
			this.tabPageStoredRecord.Controls.Add(this.labelFrom);
			this.tabPageStoredRecord.Controls.Add(this.labelUponSubmit);
			this.tabPageStoredRecord.Controls.Add(this.radioButtonModify);
			this.tabPageStoredRecord.Controls.Add(this.radioButtonCreate);
			this.tabPageStoredRecord.Controls.Add(this.comboBoxRecordForm);
			this.tabPageStoredRecord.Location = new System.Drawing.Point(4, 22);
			this.tabPageStoredRecord.Name = "tabPageStoredRecord";
			this.tabPageStoredRecord.Size = new System.Drawing.Size(495, 175);
			this.tabPageStoredRecord.TabIndex = 2;
			this.tabPageStoredRecord.Text = "Stored Record";
			this.tabPageStoredRecord.UseVisualStyleBackColor = true;
			// 
			// buttonAddModifyRecord
			// 
			this.buttonAddModifyRecord.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonAddModifyRecord.Location = new System.Drawing.Point(207, 144);
			this.buttonAddModifyRecord.Name = "buttonAddModifyRecord";
			this.buttonAddModifyRecord.Size = new System.Drawing.Size(80, 24);
			this.buttonAddModifyRecord.TabIndex = 9;
			this.buttonAddModifyRecord.Text = "Add";
			this.buttonAddModifyRecord.Click += new System.EventHandler(this.buttonAddModifyRecord_Click);
			// 
			// labelWhere2
			// 
			this.labelWhere2.AutoSize = true;
			this.labelWhere2.Location = new System.Drawing.Point(137, 61);
			this.labelWhere2.Name = "labelWhere2";
			this.labelWhere2.Size = new System.Drawing.Size(171, 13);
			this.labelWhere2.TabIndex = 8;
			this.labelWhere2.Text = "of the following conditions are true:";
			this.labelWhere2.Visible = false;
			// 
			// comboBoxAllAny
			// 
			this.comboBoxAllAny.FormattingEnabled = true;
			this.comboBoxAllAny.Items.AddRange(new object[] {
            "ALL",
            "ANY"});
			this.comboBoxAllAny.Location = new System.Drawing.Point(58, 57);
			this.comboBoxAllAny.Name = "comboBoxAllAny";
			this.comboBoxAllAny.Size = new System.Drawing.Size(72, 21);
			this.comboBoxAllAny.TabIndex = 7;
			this.comboBoxAllAny.VisibleChanged += new System.EventHandler(this.comboBoxAllAny_VisibleChanged);
			// 
			// groupBoxWhere
			// 
			this.groupBoxWhere.Location = new System.Drawing.Point(15, 84);
			this.groupBoxWhere.Name = "groupBoxWhere";
			this.groupBoxWhere.Size = new System.Drawing.Size(461, 54);
			this.groupBoxWhere.TabIndex = 6;
			this.groupBoxWhere.TabStop = false;
			this.groupBoxWhere.Layout += new System.Windows.Forms.LayoutEventHandler(this.groupBoxWhere_Layout);
			// 
			// labelWhere
			// 
			this.labelWhere.AutoSize = true;
			this.labelWhere.Location = new System.Drawing.Point(12, 61);
			this.labelWhere.Name = "labelWhere";
			this.labelWhere.Size = new System.Drawing.Size(39, 13);
			this.labelWhere.TabIndex = 5;
			this.labelWhere.Text = "Where";
			// 
			// labelFrom
			// 
			this.labelFrom.AutoSize = true;
			this.labelFrom.Location = new System.Drawing.Point(9, 21);
			this.labelFrom.Name = "labelFrom";
			this.labelFrom.Size = new System.Drawing.Size(27, 13);
			this.labelFrom.TabIndex = 4;
			this.labelFrom.Text = "from";
			// 
			// labelUponSubmit
			// 
			this.labelUponSubmit.AutoSize = true;
			this.labelUponSubmit.Location = new System.Drawing.Point(274, 21);
			this.labelUponSubmit.Name = "labelUponSubmit";
			this.labelUponSubmit.Size = new System.Drawing.Size(69, 13);
			this.labelUponSubmit.TabIndex = 3;
			this.labelUponSubmit.Text = "Upon submit:";
			// 
			// radioButtonModify
			// 
			this.radioButtonModify.AutoSize = true;
			this.radioButtonModify.Location = new System.Drawing.Point(349, 11);
			this.radioButtonModify.Name = "radioButtonModify";
			this.radioButtonModify.Size = new System.Drawing.Size(127, 17);
			this.radioButtonModify.TabIndex = 2;
			this.radioButtonModify.TabStop = true;
			this.radioButtonModify.Text = "Modify existing record";
			this.radioButtonModify.UseVisualStyleBackColor = true;
			this.radioButtonModify.CheckedChanged += new System.EventHandler(this.radioButtonModify_CheckedChanged);
			// 
			// radioButtonCreate
			// 
			this.radioButtonCreate.AutoSize = true;
			this.radioButtonCreate.Location = new System.Drawing.Point(349, 29);
			this.radioButtonCreate.Name = "radioButtonCreate";
			this.radioButtonCreate.Size = new System.Drawing.Size(112, 17);
			this.radioButtonCreate.TabIndex = 1;
			this.radioButtonCreate.TabStop = true;
			this.radioButtonCreate.Text = "Create new record";
			this.radioButtonCreate.UseVisualStyleBackColor = true;
			this.radioButtonCreate.CheckedChanged += new System.EventHandler(this.radioButtonCreate_CheckedChanged);
			// 
			// comboBoxRecordForm
			// 
			this.comboBoxRecordForm.FormattingEnabled = true;
			this.comboBoxRecordForm.Location = new System.Drawing.Point(46, 17);
			this.comboBoxRecordForm.Name = "comboBoxRecordForm";
			this.comboBoxRecordForm.Size = new System.Drawing.Size(192, 21);
			this.comboBoxRecordForm.TabIndex = 0;
			this.comboBoxRecordForm.SelectionChangeCommitted += new System.EventHandler(this.comboBoxRecordForm_SelectionChangeCommitted);
			// 
			// tabPageUrl
			// 
			this.tabPageUrl.Controls.Add(this.buttonAddModifyUrl);
			this.tabPageUrl.Controls.Add(this.labelUrl);
			this.tabPageUrl.Controls.Add(this.textBoxUrl);
			this.tabPageUrl.Location = new System.Drawing.Point(4, 22);
			this.tabPageUrl.Name = "tabPageUrl";
			this.tabPageUrl.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageUrl.Size = new System.Drawing.Size(495, 175);
			this.tabPageUrl.TabIndex = 3;
			this.tabPageUrl.Text = "URL";
			this.tabPageUrl.UseVisualStyleBackColor = true;
			this.tabPageUrl.Layout += new System.Windows.Forms.LayoutEventHandler(this.tabPageUrl_Layout);
			// 
			// buttonAddModifyUrl
			// 
			this.buttonAddModifyUrl.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonAddModifyUrl.Location = new System.Drawing.Point(207, 149);
			this.buttonAddModifyUrl.Name = "buttonAddModifyUrl";
			this.buttonAddModifyUrl.Size = new System.Drawing.Size(80, 24);
			this.buttonAddModifyUrl.TabIndex = 0;
			this.buttonAddModifyUrl.Text = "Add";
			this.buttonAddModifyUrl.Click += new System.EventHandler(this.buttonAddModifyUrl_Click);
			// 
			// labelUrl
			// 
			this.labelUrl.AutoSize = true;
			this.labelUrl.Location = new System.Drawing.Point(7, 17);
			this.labelUrl.Name = "labelUrl";
			this.labelUrl.Size = new System.Drawing.Size(124, 13);
			this.labelUrl.TabIndex = 1;
			this.labelUrl.Text = "Type or paste URL here:";
			// 
			// textBoxUrl
			// 
			this.textBoxUrl.Location = new System.Drawing.Point(10, 36);
			this.textBoxUrl.Name = "textBoxUrl";
			this.textBoxUrl.Size = new System.Drawing.Size(463, 20);
			this.textBoxUrl.TabIndex = 0;
			// 
			// ShowStatementView
			// 
			this.Controls.Add(this.tabControl);
			this.Name = "ShowStatementView";
			this.Size = new System.Drawing.Size(503, 201);
			this.tabControl.ResumeLayout(false);
			this.tabPageDocument.ResumeLayout(false);
			this.tabPageDocument.PerformLayout();
			this.tabPageForm.ResumeLayout(false);
			this.tabPageStoredRecord.ResumeLayout(false);
			this.tabPageStoredRecord.PerformLayout();
			this.tabPageUrl.ResumeLayout(false);
			this.tabPageUrl.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

        private System.Windows.Forms.ComboBox comboBoxDoc;
        private Tawala.Processes.AddModifyButton buttonAddModifyDoc;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageDocument;
        private System.Windows.Forms.TabPage tabPageForm;
        private System.Windows.Forms.ComboBox comboBoxForm;
        private Tawala.Processes.AddModifyButton buttonAddModifyForm;
        private System.Windows.Forms.CheckBox checkBoxReset;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabPageStoredRecord;
		private System.Windows.Forms.ComboBox comboBoxRecordForm;
		private System.Windows.Forms.RadioButton radioButtonModify;
		private System.Windows.Forms.RadioButton radioButtonCreate;
		private System.Windows.Forms.Label labelFrom;
		private System.Windows.Forms.Label labelUponSubmit;
		private System.Windows.Forms.GroupBox groupBoxWhere;
		private System.Windows.Forms.Label labelWhere;
		private System.Windows.Forms.ComboBox comboBoxAllAny;
		private System.Windows.Forms.Label labelWhere2;
		private AddModifyButton buttonAddModifyRecord;
		private System.Windows.Forms.TabPage tabPageUrl;
		private System.Windows.Forms.Label labelUrl;
		private System.Windows.Forms.TextBox textBoxUrl;
		private AddModifyButton buttonAddModifyUrl;
    }
}
