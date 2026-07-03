namespace Tawala.Forms
{
	partial class DynamicMcqConfigDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DynamicMcqConfigDialog));
			this.checkBoxChoiceFromStoredData = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboBoxIdFields = new System.Windows.Forms.ComboBox();
			this.comboBoxChoiceFields = new System.Windows.Forms.ComboBox();
			this.comboBoxForms = new System.Windows.Forms.ComboBox();
			this.labelForm = new System.Windows.Forms.Label();
			this.labelDisplayField = new System.Windows.Forms.Label();
			this.labelIdField = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkBoxChoiceFromStoredData
			// 
			this.checkBoxChoiceFromStoredData.AutoSize = true;
			this.checkBoxChoiceFromStoredData.Location = new System.Drawing.Point(8, 8);
			this.checkBoxChoiceFromStoredData.Name = "checkBoxChoiceFromStoredData";
			this.checkBoxChoiceFromStoredData.Size = new System.Drawing.Size(205, 17);
			this.checkBoxChoiceFromStoredData.TabIndex = 0;
			this.checkBoxChoiceFromStoredData.Text = "Populate the choices from stored data";
			this.checkBoxChoiceFromStoredData.UseVisualStyleBackColor = true;
			this.checkBoxChoiceFromStoredData.CheckedChanged += new System.EventHandler(this.checkBoxChoiceFromStoredData_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboBoxIdFields);
			this.groupBox1.Controls.Add(this.comboBoxChoiceFields);
			this.groupBox1.Controls.Add(this.comboBoxForms);
			this.groupBox1.Controls.Add(this.labelForm);
			this.groupBox1.Controls.Add(this.labelDisplayField);
			this.groupBox1.Controls.Add(this.labelIdField);
			this.groupBox1.Enabled = false;
			this.groupBox1.Location = new System.Drawing.Point(8, 32);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this.groupBox1.Size = new System.Drawing.Size(492, 114);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			// 
			// comboBoxIdFields
			// 
			this.comboBoxIdFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxIdFields.FormattingEnabled = true;
			this.comboBoxIdFields.Location = new System.Drawing.Point(270, 80);
			this.comboBoxIdFields.Name = "comboBoxIdFields";
			this.comboBoxIdFields.Size = new System.Drawing.Size(202, 21);
			this.comboBoxIdFields.TabIndex = 9;
			// 
			// comboBoxChoiceFields
			// 
			this.comboBoxChoiceFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxChoiceFields.FormattingEnabled = true;
			this.comboBoxChoiceFields.Location = new System.Drawing.Point(270, 48);
			this.comboBoxChoiceFields.Name = "comboBoxChoiceFields";
			this.comboBoxChoiceFields.Size = new System.Drawing.Size(202, 21);
			this.comboBoxChoiceFields.TabIndex = 8;
			// 
			// comboBoxForms
			// 
			this.comboBoxForms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxForms.FormattingEnabled = true;
			this.comboBoxForms.Location = new System.Drawing.Point(270, 16);
			this.comboBoxForms.Name = "comboBoxForms";
			this.comboBoxForms.Size = new System.Drawing.Size(202, 21);
			this.comboBoxForms.TabIndex = 7;
			this.comboBoxForms.SelectedValueChanged += new System.EventHandler(this.comboBoxForms_SelectedValueChanged);
			// 
			// labelForm
			// 
			this.labelForm.AutoSize = true;
			this.labelForm.Location = new System.Drawing.Point(8, 19);
			this.labelForm.Name = "labelForm";
			this.labelForm.Size = new System.Drawing.Size(209, 13);
			this.labelForm.TabIndex = 6;
			this.labelForm.Text = "Select the Form that contains the choices: ";
			// 
			// labelDisplayField
			// 
			this.labelDisplayField.AutoSize = true;
			this.labelDisplayField.Location = new System.Drawing.Point(8, 51);
			this.labelDisplayField.Margin = new System.Windows.Forms.Padding(6);
			this.labelDisplayField.Name = "labelDisplayField";
			this.labelDisplayField.Size = new System.Drawing.Size(237, 13);
			this.labelDisplayField.TabIndex = 2;
			this.labelDisplayField.Text = "Select the Field that will be displayed as choices:";
			this.labelDisplayField.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelIdField
			// 
			this.labelIdField.AutoSize = true;
			this.labelIdField.Location = new System.Drawing.Point(8, 83);
			this.labelIdField.Margin = new System.Windows.Forms.Padding(6);
			this.labelIdField.Name = "labelIdField";
			this.labelIdField.Size = new System.Drawing.Size(217, 13);
			this.labelIdField.TabIndex = 4;
			this.labelIdField.Text = "Select the Field to be used as the choice ID:";
			this.labelIdField.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonOK.Location = new System.Drawing.Point(156, 3);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(4);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(261, 3);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// panel1
			// 
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.buttonCancel);
			this.panel1.Controls.Add(this.buttonOK);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(8, 162);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(492, 30);
			this.panel1.TabIndex = 4;
			// 
			// DynamicMcqConfigDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(508, 200);
			this.Controls.Add(this.checkBoxChoiceFromStoredData);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(480, 192);
			this.Name = "DynamicMcqConfigDialog";
			this.Padding = new System.Windows.Forms.Padding(8);
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Choices Configuration";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxChoiceFromStoredData;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label labelDisplayField;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelIdField;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labelForm;
		private System.Windows.Forms.ComboBox comboBoxIdFields;
		private System.Windows.Forms.ComboBox comboBoxChoiceFields;
		private System.Windows.Forms.ComboBox comboBoxForms;

	}
}