namespace Tawala.Forms
{
	partial class TextItemStylesDialog
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
			this.panelText = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBoxNoPaddingBottom = new System.Windows.Forms.CheckBox();
			this.tableLayoutPanelText = new System.Windows.Forms.TableLayoutPanel();
			this.radioButtonTextError = new System.Windows.Forms.RadioButton();
			this.radioButtonTextInstructional = new System.Windows.Forms.RadioButton();
			this.radioButtonTextNormal = new System.Windows.Forms.RadioButton();
			this.panelNormalText = new System.Windows.Forms.Panel();
			this.labelTextNormal = new System.Windows.Forms.Label();
			this.panelInstructional = new System.Windows.Forms.Panel();
			this.labelTextInstructional = new System.Windows.Forms.Label();
			this.panelTextError = new System.Windows.Forms.Panel();
			this.labelTextError = new System.Windows.Forms.Label();
			this.buttonApplyAll = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.labelApplyOptions = new System.Windows.Forms.Label();
			this.buttonApplySelected = new System.Windows.Forms.Button();
			this.panelText.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanelText.SuspendLayout();
			this.panelNormalText.SuspendLayout();
			this.panelInstructional.SuspendLayout();
			this.panelTextError.SuspendLayout();
			this.panelButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelText
			// 
			this.panelText.AutoScroll = true;
			this.panelText.Controls.Add(this.groupBox1);
			this.panelText.Controls.Add(this.tableLayoutPanelText);
			this.panelText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelText.Location = new System.Drawing.Point(0, 0);
			this.panelText.Name = "panelText";
			this.panelText.Size = new System.Drawing.Size(443, 260);
			this.panelText.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkBoxNoPaddingBottom);
			this.groupBox1.Location = new System.Drawing.Point(6, 203);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(426, 49);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Spacing";
			// 
			// checkBoxNoPaddingBottom
			// 
			this.checkBoxNoPaddingBottom.AutoSize = true;
			this.checkBoxNoPaddingBottom.Location = new System.Drawing.Point(16, 19);
			this.checkBoxNoPaddingBottom.Name = "checkBoxNoPaddingBottom";
			this.checkBoxNoPaddingBottom.Size = new System.Drawing.Size(270, 17);
			this.checkBoxNoPaddingBottom.TabIndex = 0;
			this.checkBoxNoPaddingBottom.Text = "Do not add blank space below text when displayed.";
			this.checkBoxNoPaddingBottom.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanelText
			// 
			this.tableLayoutPanelText.ColumnCount = 2;
			this.tableLayoutPanelText.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.27737F));
			this.tableLayoutPanelText.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.72263F));
			this.tableLayoutPanelText.Controls.Add(this.radioButtonTextError, 0, 2);
			this.tableLayoutPanelText.Controls.Add(this.radioButtonTextInstructional, 0, 1);
			this.tableLayoutPanelText.Controls.Add(this.radioButtonTextNormal, 0, 0);
			this.tableLayoutPanelText.Controls.Add(this.panelNormalText, 1, 0);
			this.tableLayoutPanelText.Controls.Add(this.panelInstructional, 1, 1);
			this.tableLayoutPanelText.Controls.Add(this.panelTextError, 1, 2);
			this.tableLayoutPanelText.Location = new System.Drawing.Point(6, 6);
			this.tableLayoutPanelText.Name = "tableLayoutPanelText";
			this.tableLayoutPanelText.RowCount = 3;
			this.tableLayoutPanelText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanelText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanelText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanelText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanelText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanelText.Size = new System.Drawing.Size(429, 190);
			this.tableLayoutPanelText.TabIndex = 2;
			// 
			// radioButtonTextError
			// 
			this.radioButtonTextError.AutoSize = true;
			this.radioButtonTextError.Dock = System.Windows.Forms.DockStyle.Fill;
			this.radioButtonTextError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.radioButtonTextError.Location = new System.Drawing.Point(3, 129);
			this.radioButtonTextError.Name = "radioButtonTextError";
			this.radioButtonTextError.Size = new System.Drawing.Size(106, 58);
			this.radioButtonTextError.TabIndex = 8;
			this.radioButtonTextError.Text = "Error";
			this.radioButtonTextError.UseVisualStyleBackColor = true;
			// 
			// radioButtonTextInstructional
			// 
			this.radioButtonTextInstructional.AutoSize = true;
			this.radioButtonTextInstructional.Dock = System.Windows.Forms.DockStyle.Fill;
			this.radioButtonTextInstructional.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.radioButtonTextInstructional.Location = new System.Drawing.Point(3, 66);
			this.radioButtonTextInstructional.Name = "radioButtonTextInstructional";
			this.radioButtonTextInstructional.Size = new System.Drawing.Size(106, 57);
			this.radioButtonTextInstructional.TabIndex = 6;
			this.radioButtonTextInstructional.Text = "Instructional";
			this.radioButtonTextInstructional.UseVisualStyleBackColor = true;
			// 
			// radioButtonTextNormal
			// 
			this.radioButtonTextNormal.AutoSize = true;
			this.radioButtonTextNormal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.radioButtonTextNormal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.radioButtonTextNormal.Location = new System.Drawing.Point(3, 3);
			this.radioButtonTextNormal.Name = "radioButtonTextNormal";
			this.radioButtonTextNormal.Size = new System.Drawing.Size(106, 57);
			this.radioButtonTextNormal.TabIndex = 0;
			this.radioButtonTextNormal.Text = "Normal";
			this.radioButtonTextNormal.UseVisualStyleBackColor = true;
			// 
			// panelNormalText
			// 
			this.panelNormalText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
			this.panelNormalText.Controls.Add(this.labelTextNormal);
			this.panelNormalText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelNormalText.Location = new System.Drawing.Point(115, 3);
			this.panelNormalText.Name = "panelNormalText";
			this.panelNormalText.Size = new System.Drawing.Size(311, 57);
			this.panelNormalText.TabIndex = 1;
			// 
			// labelTextNormal
			// 
			this.labelTextNormal.AutoSize = true;
			this.labelTextNormal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTextNormal.Location = new System.Drawing.Point(4, 10);
			this.labelTextNormal.Name = "labelTextNormal";
			this.labelTextNormal.Size = new System.Drawing.Size(106, 15);
			this.labelTextNormal.TabIndex = 0;
			this.labelTextNormal.Text = "This is normal text";
			// 
			// panelInstructional
			// 
			this.panelInstructional.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
			this.panelInstructional.Controls.Add(this.labelTextInstructional);
			this.panelInstructional.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelInstructional.Location = new System.Drawing.Point(115, 66);
			this.panelInstructional.Name = "panelInstructional";
			this.panelInstructional.Size = new System.Drawing.Size(311, 57);
			this.panelInstructional.TabIndex = 7;
			// 
			// labelTextInstructional
			// 
			this.labelTextInstructional.AutoSize = true;
			this.labelTextInstructional.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTextInstructional.ForeColor = System.Drawing.SystemColors.Desktop;
			this.labelTextInstructional.Location = new System.Drawing.Point(4, 8);
			this.labelTextInstructional.Name = "labelTextInstructional";
			this.labelTextInstructional.Size = new System.Drawing.Size(181, 17);
			this.labelTextInstructional.TabIndex = 1;
			this.labelTextInstructional.Text = "This is instructional text";
			// 
			// panelTextError
			// 
			this.panelTextError.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
			this.panelTextError.Controls.Add(this.labelTextError);
			this.panelTextError.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelTextError.Location = new System.Drawing.Point(115, 129);
			this.panelTextError.Name = "panelTextError";
			this.panelTextError.Size = new System.Drawing.Size(311, 58);
			this.panelTextError.TabIndex = 9;
			// 
			// labelTextError
			// 
			this.labelTextError.AutoSize = true;
			this.labelTextError.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTextError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.labelTextError.Location = new System.Drawing.Point(3, 4);
			this.labelTextError.Name = "labelTextError";
			this.labelTextError.Size = new System.Drawing.Size(128, 17);
			this.labelTextError.TabIndex = 2;
			this.labelTextError.Text = "This is error text";
			// 
			// buttonApplyAll
			// 
			this.buttonApplyAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonApplyAll.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonApplyAll.Location = new System.Drawing.Point(214, 50);
			this.buttonApplyAll.Name = "buttonApplyAll";
			this.buttonApplyAll.Size = new System.Drawing.Size(101, 23);
			this.buttonApplyAll.TabIndex = 1;
			this.buttonApplyAll.Text = "Apply to All";
			this.buttonApplyAll.UseVisualStyleBackColor = true;
			this.buttonApplyAll.Click += new System.EventHandler(this.buttonApplyAll_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(330, 50);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(101, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// panelButtons
			// 
			this.panelButtons.Controls.Add(this.labelApplyOptions);
			this.panelButtons.Controls.Add(this.buttonApplySelected);
			this.panelButtons.Controls.Add(this.buttonApplyAll);
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelButtons.Location = new System.Drawing.Point(0, 260);
			this.panelButtons.Name = "panelButtons";
			this.panelButtons.Size = new System.Drawing.Size(443, 85);
			this.panelButtons.TabIndex = 6;
			// 
			// labelApplyOptions
			// 
			this.labelApplyOptions.Location = new System.Drawing.Point(13, 13);
			this.labelApplyOptions.Name = "labelApplyOptions";
			this.labelApplyOptions.Size = new System.Drawing.Size(417, 31);
			this.labelApplyOptions.TabIndex = 3;
			this.labelApplyOptions.Text = "Note: Style may be applied only to selected Text items in the active form, if any" +
				". The \"Apply to All\" feature has been disabled.";
			// 
			// buttonApplySelected
			// 
			this.buttonApplySelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonApplySelected.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonApplySelected.Location = new System.Drawing.Point(107, 50);
			this.buttonApplySelected.Name = "buttonApplySelected";
			this.buttonApplySelected.Size = new System.Drawing.Size(101, 23);
			this.buttonApplySelected.TabIndex = 0;
			this.buttonApplySelected.Text = "Apply to Selected";
			this.buttonApplySelected.UseVisualStyleBackColor = true;
			this.buttonApplySelected.Click += new System.EventHandler(this.buttonApplySelected_Click);
			// 
			// TextItemStylesDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(443, 345);
			this.Controls.Add(this.panelText);
			this.Controls.Add(this.panelButtons);
			this.Name = "TextItemStylesDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Styles";
			this.Load += new System.EventHandler(this.FormItemStylesDialog_Load);
			this.Activated += new System.EventHandler(this.FormItemStylesDialog_Activated);
			this.panelText.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tableLayoutPanelText.ResumeLayout(false);
			this.tableLayoutPanelText.PerformLayout();
			this.panelNormalText.ResumeLayout(false);
			this.panelNormalText.PerformLayout();
			this.panelInstructional.ResumeLayout(false);
			this.panelInstructional.PerformLayout();
			this.panelTextError.ResumeLayout(false);
			this.panelTextError.PerformLayout();
			this.panelButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonApplyAll;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelText;
		private System.Windows.Forms.RadioButton radioButtonTextNormal;
		private System.Windows.Forms.Panel panelNormalText;
		private System.Windows.Forms.Label labelTextNormal;
		private System.Windows.Forms.RadioButton radioButtonTextInstructional;
		private System.Windows.Forms.RadioButton radioButtonTextError;
		private System.Windows.Forms.Panel panelInstructional;
		private System.Windows.Forms.Label labelTextInstructional;
		private System.Windows.Forms.Panel panelTextError;
		private System.Windows.Forms.Label labelTextError;
		private System.Windows.Forms.Panel panelText;
		private System.Windows.Forms.Panel panelButtons;
		private System.Windows.Forms.Button buttonApplySelected;
		private System.Windows.Forms.Label labelApplyOptions;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxNoPaddingBottom;
	}
}