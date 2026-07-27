namespace Tawala.Forms
{
	partial class FibStylesDialog
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
			this.panelChoices = new System.Windows.Forms.Panel();
			this.panelPreview = new System.Windows.Forms.Panel();
			this.groupBoxBlanks = new System.Windows.Forms.GroupBox();
			this.checkBoxAlignRight = new System.Windows.Forms.CheckBox();
			this.groupBoxLabels = new System.Windows.Forms.GroupBox();
			this.radioButtonFreeform = new System.Windows.Forms.RadioButton();
			this.radioButtonRightJustified = new System.Windows.Forms.RadioButton();
			this.radioButtonLeftJustified = new System.Windows.Forms.RadioButton();
			this.radioButtonAbove = new System.Windows.Forms.RadioButton();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.labelApplyOptions = new System.Windows.Forms.Label();
			this.buttonApplySelected = new System.Windows.Forms.Button();
			this.buttonApplyAll = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panelChoices.SuspendLayout();
			this.groupBoxBlanks.SuspendLayout();
			this.groupBoxLabels.SuspendLayout();
			this.panelButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelChoices
			// 
			this.panelChoices.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panelChoices.Controls.Add(this.panelPreview);
			this.panelChoices.Controls.Add(this.groupBoxBlanks);
			this.panelChoices.Controls.Add(this.groupBoxLabels);
			this.panelChoices.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelChoices.Location = new System.Drawing.Point(0, 0);
			this.panelChoices.Name = "panelChoices";
			this.panelChoices.Size = new System.Drawing.Size(449, 221);
			this.panelChoices.TabIndex = 0;
			// 
			// panelPreview
			// 
			this.panelPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
			this.panelPreview.Location = new System.Drawing.Point(139, 21);
			this.panelPreview.Name = "panelPreview";
			this.panelPreview.Size = new System.Drawing.Size(297, 185);
			this.panelPreview.TabIndex = 2;
			// 
			// groupBoxBlanks
			// 
			this.groupBoxBlanks.Controls.Add(this.checkBoxAlignRight);
			this.groupBoxBlanks.Location = new System.Drawing.Point(10, 154);
			this.groupBoxBlanks.Name = "groupBoxBlanks";
			this.groupBoxBlanks.Size = new System.Drawing.Size(114, 52);
			this.groupBoxBlanks.TabIndex = 1;
			this.groupBoxBlanks.TabStop = false;
			this.groupBoxBlanks.Text = "Blanks";
			// 
			// checkBoxAlignRight
			// 
			this.checkBoxAlignRight.AutoSize = true;
			this.checkBoxAlignRight.Location = new System.Drawing.Point(7, 22);
			this.checkBoxAlignRight.Name = "checkBoxAlignRight";
			this.checkBoxAlignRight.Size = new System.Drawing.Size(94, 17);
			this.checkBoxAlignRight.TabIndex = 0;
			this.checkBoxAlignRight.Text = "Align right side";
			this.checkBoxAlignRight.UseVisualStyleBackColor = true;
			this.checkBoxAlignRight.CheckedChanged += new System.EventHandler(this.checkBoxAlignRight_CheckedChanged);
			// 
			// groupBoxLabels
			// 
			this.groupBoxLabels.Controls.Add(this.radioButtonFreeform);
			this.groupBoxLabels.Controls.Add(this.radioButtonRightJustified);
			this.groupBoxLabels.Controls.Add(this.radioButtonLeftJustified);
			this.groupBoxLabels.Controls.Add(this.radioButtonAbove);
			this.groupBoxLabels.Location = new System.Drawing.Point(10, 15);
			this.groupBoxLabels.Name = "groupBoxLabels";
			this.groupBoxLabels.Size = new System.Drawing.Size(114, 126);
			this.groupBoxLabels.TabIndex = 0;
			this.groupBoxLabels.TabStop = false;
			this.groupBoxLabels.Text = "Labels";
			// 
			// radioButtonFreeform
			// 
			this.radioButtonFreeform.AutoSize = true;
			this.radioButtonFreeform.Location = new System.Drawing.Point(7, 95);
			this.radioButtonFreeform.Name = "radioButtonFreeform";
			this.radioButtonFreeform.Size = new System.Drawing.Size(66, 17);
			this.radioButtonFreeform.TabIndex = 3;
			this.radioButtonFreeform.Text = "Freeform";
			this.radioButtonFreeform.UseVisualStyleBackColor = true;
			this.radioButtonFreeform.Click += new System.EventHandler(this.radioButtonFreeform_Click);
			this.radioButtonFreeform.CheckedChanged += new System.EventHandler(this.radioButtonFreeform_CheckedChanged);
			// 
			// radioButtonRightJustified
			// 
			this.radioButtonRightJustified.AutoSize = true;
			this.radioButtonRightJustified.Location = new System.Drawing.Point(7, 71);
			this.radioButtonRightJustified.Name = "radioButtonRightJustified";
			this.radioButtonRightJustified.Size = new System.Drawing.Size(88, 17);
			this.radioButtonRightJustified.TabIndex = 2;
			this.radioButtonRightJustified.Text = "Right justified";
			this.radioButtonRightJustified.UseVisualStyleBackColor = true;
			this.radioButtonRightJustified.Click += new System.EventHandler(this.radioButtonRightJustified_Click);
			this.radioButtonRightJustified.CheckedChanged += new System.EventHandler(this.radioButtonRightJustified_CheckedChanged);
			// 
			// radioButtonLeftJustified
			// 
			this.radioButtonLeftJustified.AutoSize = true;
			this.radioButtonLeftJustified.Location = new System.Drawing.Point(7, 47);
			this.radioButtonLeftJustified.Name = "radioButtonLeftJustified";
			this.radioButtonLeftJustified.Size = new System.Drawing.Size(81, 17);
			this.radioButtonLeftJustified.TabIndex = 1;
			this.radioButtonLeftJustified.Text = "Left justified";
			this.radioButtonLeftJustified.UseVisualStyleBackColor = true;
			this.radioButtonLeftJustified.Click += new System.EventHandler(this.radioButtonLeftJustified_Click);
			this.radioButtonLeftJustified.CheckedChanged += new System.EventHandler(this.radioButtonLeftJustified_CheckedChanged);
			// 
			// radioButtonAbove
			// 
			this.radioButtonAbove.AutoSize = true;
			this.radioButtonAbove.Location = new System.Drawing.Point(7, 23);
			this.radioButtonAbove.Name = "radioButtonAbove";
			this.radioButtonAbove.Size = new System.Drawing.Size(56, 17);
			this.radioButtonAbove.TabIndex = 0;
			this.radioButtonAbove.Text = "Above";
			this.radioButtonAbove.UseVisualStyleBackColor = true;
			this.radioButtonAbove.Click += new System.EventHandler(this.radioButtonAbove_Click);
			this.radioButtonAbove.CheckedChanged += new System.EventHandler(this.radioButtonAbove_CheckedChanged);
			// 
			// panelButtons
			// 
			this.panelButtons.Controls.Add(this.labelApplyOptions);
			this.panelButtons.Controls.Add(this.buttonApplySelected);
			this.panelButtons.Controls.Add(this.buttonApplyAll);
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelButtons.Location = new System.Drawing.Point(0, 221);
			this.panelButtons.Name = "panelButtons";
			this.panelButtons.Size = new System.Drawing.Size(449, 90);
			this.panelButtons.TabIndex = 1;
			// 
			// labelApplyOptions
			// 
			this.labelApplyOptions.Location = new System.Drawing.Point(8, 11);
			this.labelApplyOptions.Name = "labelApplyOptions";
			this.labelApplyOptions.Size = new System.Drawing.Size(434, 31);
			this.labelApplyOptions.TabIndex = 9;
			this.labelApplyOptions.Text = "Note: Style may be applied only to selected Fill in the Blank questions in the ac" +
				"tive form, if any. The \"Apply to All\" feature has been disabled.";
			// 
			// buttonApplySelected
			// 
			this.buttonApplySelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonApplySelected.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonApplySelected.Location = new System.Drawing.Point(100, 56);
			this.buttonApplySelected.Name = "buttonApplySelected";
			this.buttonApplySelected.Size = new System.Drawing.Size(101, 23);
			this.buttonApplySelected.TabIndex = 6;
			this.buttonApplySelected.Text = "Apply to Selected";
			this.buttonApplySelected.UseVisualStyleBackColor = true;
			this.buttonApplySelected.Click += new System.EventHandler(this.buttonApplySelected_Click);
			// 
			// buttonApplyAll
			// 
			this.buttonApplyAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonApplyAll.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonApplyAll.Location = new System.Drawing.Point(207, 56);
			this.buttonApplyAll.Name = "buttonApplyAll";
			this.buttonApplyAll.Size = new System.Drawing.Size(101, 23);
			this.buttonApplyAll.TabIndex = 7;
			this.buttonApplyAll.Text = "Apply to All";
			this.buttonApplyAll.UseVisualStyleBackColor = true;
			this.buttonApplyAll.Click += new System.EventHandler(this.buttonApplyAll_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(335, 56);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(101, 23);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// FibStylesDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(449, 311);
			this.Controls.Add(this.panelChoices);
			this.Controls.Add(this.panelButtons);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FibStylesDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Fill in the Blank Styles";
			this.Activated += new System.EventHandler(this.FormItemStylesDialog_Activated);
			this.panelChoices.ResumeLayout(false);
			this.groupBoxBlanks.ResumeLayout(false);
			this.groupBoxBlanks.PerformLayout();
			this.groupBoxLabels.ResumeLayout(false);
			this.groupBoxLabels.PerformLayout();
			this.panelButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelChoices;
		private System.Windows.Forms.Panel panelButtons;
		private System.Windows.Forms.Panel panelPreview;
		private System.Windows.Forms.Label labelApplyOptions;
		private System.Windows.Forms.Button buttonApplySelected;
		private System.Windows.Forms.Button buttonApplyAll;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBoxLabels;
		private System.Windows.Forms.RadioButton radioButtonFreeform;
		private System.Windows.Forms.RadioButton radioButtonRightJustified;
		private System.Windows.Forms.RadioButton radioButtonLeftJustified;
		private System.Windows.Forms.RadioButton radioButtonAbove;
		private System.Windows.Forms.GroupBox groupBoxBlanks;
		private System.Windows.Forms.CheckBox checkBoxAlignRight;
	}
}