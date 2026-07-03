namespace Tawala.Dialogs
{
	partial class InsertHyperlinkDialog
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InsertHyperlinkDialog));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelUrl = new System.Windows.Forms.Label();
			this.labelDisplayText = new System.Windows.Forms.Label();
            this.textBoxDisplayText = new Tawala.Controls.ComplexExpressionTextBox();
			this.checkBoxNewWindow = new System.Windows.Forms.CheckBox();
			this.textBoxUrl = new Tawala.Controls.ComplexExpressionTextBox();
			this.labelWhen2 = new System.Windows.Forms.Label();
			this.comboBoxAndOr = new System.Windows.Forms.ComboBox();
			this.groupBoxConditions = new System.Windows.Forms.GroupBox();
			this.labelWhen1 = new System.Windows.Forms.Label();
			this.checkBoxConditionalDisplay = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.labelDisplayTextComments = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonOK.Enabled = false;
			this.buttonOK.Location = new System.Drawing.Point(229, 262);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 12;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(339, 262);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 13;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// labelUrl
			// 
			this.labelUrl.AutoSize = true;
			this.labelUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelUrl.Location = new System.Drawing.Point(16, 16);
			this.labelUrl.MinimumSize = new System.Drawing.Size(80, 0);
			this.labelUrl.Name = "labelUrl";
			this.labelUrl.Size = new System.Drawing.Size(80, 16);
			this.labelUrl.TabIndex = 1;
			this.labelUrl.Text = "Url:";
			this.labelUrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDisplayText
			// 
			this.labelDisplayText.AutoSize = true;
			this.labelDisplayText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDisplayText.Location = new System.Drawing.Point(16, 49);
			this.labelDisplayText.MinimumSize = new System.Drawing.Size(80, 0);
			this.labelDisplayText.Name = "labelDisplayText";
			this.labelDisplayText.Size = new System.Drawing.Size(80, 16);
			this.labelDisplayText.TabIndex = 3;
			this.labelDisplayText.Text = "Display text:";
			this.labelDisplayText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxDisplayText
			// 
            this.textBoxDisplayText.AllowDrop = true;
            this.textBoxDisplayText.Location = new System.Drawing.Point(100, 47);
			this.textBoxDisplayText.Name = "textBoxDisplayText";
			this.textBoxDisplayText.Size = new System.Drawing.Size(524, 20);
			this.textBoxDisplayText.TabIndex = 4;
            this.textBoxDisplayText.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxExpression_DragDrop);
            this.textBoxDisplayText.Leave += new System.EventHandler(this.textBox_Leave);
            this.textBoxDisplayText.Enter += new System.EventHandler(this.textBox_Enter);
            this.textBoxDisplayText.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxExpression_DragEnter);
            // 
			// checkBoxNewWindow
			// 
			this.checkBoxNewWindow.AutoSize = true;
			this.checkBoxNewWindow.Location = new System.Drawing.Point(23, 100);
			this.checkBoxNewWindow.Name = "checkBoxNewWindow";
			this.checkBoxNewWindow.Size = new System.Drawing.Size(168, 17);
			this.checkBoxNewWindow.TabIndex = 5;
			this.checkBoxNewWindow.Text = "Open in new browser window.";
			this.toolTip.SetToolTip(this.checkBoxNewWindow, "Check to open the link in a new browser window rather than an existing one.");
			this.checkBoxNewWindow.UseVisualStyleBackColor = true;
			// 
			// textBoxUrl
			// 
			this.textBoxUrl.AllowDrop = true;
			this.textBoxUrl.Location = new System.Drawing.Point(100, 14);
			this.textBoxUrl.Name = "textBoxUrl";
			this.textBoxUrl.Size = new System.Drawing.Size(524, 20);
			this.textBoxUrl.TabIndex = 2;
			this.textBoxUrl.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxExpression_DragDrop);
			this.textBoxUrl.Leave += new System.EventHandler(this.textBox_Leave);
			this.textBoxUrl.Enter += new System.EventHandler(this.textBox_Enter);
			this.textBoxUrl.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxExpression_DragEnter);
			// 
			// labelWhen2
			// 
			this.labelWhen2.AutoSize = true;
			this.labelWhen2.Enabled = false;
			this.labelWhen2.Location = new System.Drawing.Point(187, 170);
			this.labelWhen2.Margin = new System.Windows.Forms.Padding(0);
			this.labelWhen2.Name = "labelWhen2";
			this.labelWhen2.Size = new System.Drawing.Size(171, 13);
			this.labelWhen2.TabIndex = 10;
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
			this.comboBoxAndOr.Location = new System.Drawing.Point(139, 166);
			this.comboBoxAndOr.MaxDropDownItems = 2;
			this.comboBoxAndOr.Name = "comboBoxAndOr";
			this.comboBoxAndOr.Size = new System.Drawing.Size(41, 21);
			this.comboBoxAndOr.TabIndex = 9;
			this.comboBoxAndOr.Visible = false;
			this.comboBoxAndOr.VisibleChanged += new System.EventHandler(this.comboBoxAndOr_VisibleChanged);
			// 
			// groupBoxConditions
			// 
			this.groupBoxConditions.Enabled = false;
			this.groupBoxConditions.Location = new System.Drawing.Point(19, 192);
			this.groupBoxConditions.Name = "groupBoxConditions";
			this.groupBoxConditions.Size = new System.Drawing.Size(605, 56);
			this.groupBoxConditions.TabIndex = 11;
			this.groupBoxConditions.TabStop = false;
			this.groupBoxConditions.Layout += new System.Windows.Forms.LayoutEventHandler(this.groupBoxConditions_Layout);
			// 
			// labelWhen1
			// 
			this.labelWhen1.AutoSize = true;
			this.labelWhen1.Enabled = false;
			this.labelWhen1.Location = new System.Drawing.Point(20, 170);
			this.labelWhen1.Name = "labelWhen1";
			this.labelWhen1.Size = new System.Drawing.Size(111, 13);
			this.labelWhen1.TabIndex = 8;
			this.labelWhen1.Text = "Display link only when";
			// 
			// checkBoxConditionalDisplay
			// 
			this.checkBoxConditionalDisplay.AutoSize = true;
			this.checkBoxConditionalDisplay.Location = new System.Drawing.Point(23, 136);
			this.checkBoxConditionalDisplay.Name = "checkBoxConditionalDisplay";
			this.checkBoxConditionalDisplay.Size = new System.Drawing.Size(140, 17);
			this.checkBoxConditionalDisplay.TabIndex = 7;
			this.checkBoxConditionalDisplay.Text = "Display link conditionally";
			this.toolTip.SetToolTip(this.checkBoxConditionalDisplay, "Check to enable option to display link based on conditions of your choice.  Unche" +
					"ck to disable this feature.");
			this.checkBoxConditionalDisplay.UseVisualStyleBackColor = true;
			// 
			// labelDisplayTextComments
			// 
			this.labelDisplayTextComments.AutoSize = true;
			this.labelDisplayTextComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDisplayTextComments.Location = new System.Drawing.Point(97, 68);
			this.labelDisplayTextComments.Name = "labelDisplayTextComments";
			this.labelDisplayTextComments.Size = new System.Drawing.Size(337, 13);
			this.labelDisplayTextComments.TabIndex = 15;
			this.labelDisplayTextComments.Text = "(optional; if you leave this blank the full URL or filename will be shown)";
			this.labelDisplayTextComments.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(19, 119);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(605, 10);
			this.groupBox1.TabIndex = 16;
			this.groupBox1.TabStop = false;
			// 
			// InsertHyperlinkDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(642, 298);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.labelDisplayTextComments);
			this.Controls.Add(this.labelWhen2);
			this.Controls.Add(this.comboBoxAndOr);
			this.Controls.Add(this.groupBoxConditions);
			this.Controls.Add(this.labelWhen1);
			this.Controls.Add(this.checkBoxConditionalDisplay);
			this.Controls.Add(this.textBoxUrl);
			this.Controls.Add(this.checkBoxNewWindow);
			this.Controls.Add(this.textBoxDisplayText);
			this.Controls.Add(this.labelDisplayText);
			this.Controls.Add(this.labelUrl);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InsertHyperlinkDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Hyperlink";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelUrl;
		private System.Windows.Forms.Label labelDisplayText;
        private Tawala.Controls.ComplexExpressionTextBox textBoxDisplayText;
		private System.Windows.Forms.CheckBox checkBoxNewWindow;
        private Tawala.Controls.ComplexExpressionTextBox textBoxUrl;
        private System.Windows.Forms.Label labelWhen2;
        private System.Windows.Forms.ComboBox comboBoxAndOr;
        private System.Windows.Forms.GroupBox groupBoxConditions;
        private System.Windows.Forms.Label labelWhen1;
        private System.Windows.Forms.CheckBox checkBoxConditionalDisplay;
		private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelDisplayTextComments;
        private System.Windows.Forms.GroupBox groupBox1;
	}
}