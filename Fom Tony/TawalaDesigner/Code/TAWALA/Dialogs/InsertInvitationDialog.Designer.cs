// $Workfile: InsertInvitationDialog.Designer.cs $
// $Revision: 2 $	$Date: 3/13/08 5:51p $
// Copyright © 2005 - 2006 Tawala Systems, Inc. All rights reserved.
namespace Tawala.Dialogs
{
	partial class InsertInvitationDialog
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
            System.Windows.Forms.Label labelIn;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label labelDisplay;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InsertInvitationDialog));
            this.comboBoxInvitationProject = new System.Windows.Forms.ComboBox();
            this.comboBoxInvitationStartingPoint = new System.Windows.Forms.ComboBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.textBoxDisplayText = new Tawala.Controls.ExpressionTextBox();
            this.checkBoxPrivateInvitation = new System.Windows.Forms.CheckBox();
            this.textBoxKeyExpression = new Tawala.Controls.ExpressionTextBox();
            this.labelPrivate = new System.Windows.Forms.Label();
            this.checkBoxOneTimeOnly = new System.Windows.Forms.CheckBox();
            labelIn = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            labelDisplay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelIn
            // 
            labelIn.Location = new System.Drawing.Point(176, 25);
            labelIn.Name = "labelIn";
            labelIn.Size = new System.Drawing.Size(28, 21);
            labelIn.TabIndex = 2;
            labelIn.Text = "in";
            labelIn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 8);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(33, 13);
            label1.TabIndex = 1;
            label1.Text = "Form:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(207, 8);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(43, 13);
            label2.TabIndex = 4;
            label2.Text = "Project:";
            // 
            // labelDisplay
            // 
            labelDisplay.AutoSize = true;
            labelDisplay.Location = new System.Drawing.Point(9, 52);
            labelDisplay.Name = "labelDisplay";
            labelDisplay.Size = new System.Drawing.Size(68, 13);
            labelDisplay.TabIndex = 6;
            labelDisplay.Text = "Display Text:";
            // 
            // comboBoxInvitationProject
            // 
            this.comboBoxInvitationProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInvitationProject.FormattingEnabled = true;
            this.comboBoxInvitationProject.Location = new System.Drawing.Point(208, 25);
            this.comboBoxInvitationProject.MaxDropDownItems = 20;
            this.comboBoxInvitationProject.Name = "comboBoxInvitationProject";
            this.comboBoxInvitationProject.Size = new System.Drawing.Size(160, 21);
            this.comboBoxInvitationProject.TabIndex = 5;
            // 
            // comboBoxInvitationStartingPoint
            // 
            this.comboBoxInvitationStartingPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInvitationStartingPoint.FormattingEnabled = true;
            this.comboBoxInvitationStartingPoint.Location = new System.Drawing.Point(10, 25);
            this.comboBoxInvitationStartingPoint.MaxDropDownItems = 20;
            this.comboBoxInvitationStartingPoint.Name = "comboBoxInvitationStartingPoint";
            this.comboBoxInvitationStartingPoint.Size = new System.Drawing.Size(160, 21);
            this.comboBoxInvitationStartingPoint.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(204, 263);
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
            this.okButton.Location = new System.Drawing.Point(101, 263);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // textBoxDisplayText
            // 
            this.textBoxDisplayText.AllowDrop = true;
            this.textBoxDisplayText.Location = new System.Drawing.Point(10, 69);
            this.textBoxDisplayText.Name = "textBoxDisplayText";
            this.textBoxDisplayText.Size = new System.Drawing.Size(358, 20);
            this.textBoxDisplayText.TabIndex = 7;
            // 
            // checkBoxPrivateInvitation
            // 
            this.checkBoxPrivateInvitation.AutoSize = true;
            this.checkBoxPrivateInvitation.Location = new System.Drawing.Point(10, 105);
            this.checkBoxPrivateInvitation.Name = "checkBoxPrivateInvitation";
            this.checkBoxPrivateInvitation.Size = new System.Drawing.Size(161, 17);
            this.checkBoxPrivateInvitation.TabIndex = 10;
            this.checkBoxPrivateInvitation.Text = "Make this a private invitation";
            this.checkBoxPrivateInvitation.UseVisualStyleBackColor = true;
            // 
            // textBoxKeyExpression
            // 
            this.textBoxKeyExpression.AllowDrop = true;
            this.textBoxKeyExpression.Location = new System.Drawing.Point(10, 195);
            this.textBoxKeyExpression.Name = "textBoxKeyExpression";
            this.textBoxKeyExpression.Size = new System.Drawing.Size(358, 20);
            this.textBoxKeyExpression.TabIndex = 11;
            // 
            // labelPrivate
            // 
            this.labelPrivate.Location = new System.Drawing.Point(13, 135);
            this.labelPrivate.Name = "labelPrivate";
            this.labelPrivate.Size = new System.Drawing.Size(355, 57);
            this.labelPrivate.TabIndex = 12;
            this.labelPrivate.Text = resources.GetString("labelPrivate.Text");
            // 
            // checkBoxOneTimeOnly
            // 
            this.checkBoxOneTimeOnly.AutoSize = true;
            this.checkBoxOneTimeOnly.Location = new System.Drawing.Point(10, 225);
            this.checkBoxOneTimeOnly.Name = "checkBoxOneTimeOnly";
            this.checkBoxOneTimeOnly.Size = new System.Drawing.Size(231, 17);
            this.checkBoxOneTimeOnly.TabIndex = 13;
            this.checkBoxOneTimeOnly.Text = "Allow only one access per user to the Form.";
            this.checkBoxOneTimeOnly.UseVisualStyleBackColor = true;
            // 
            // InsertInvitationDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(381, 296);
            this.Controls.Add(this.checkBoxOneTimeOnly);
            this.Controls.Add(this.labelPrivate);
            this.Controls.Add(this.textBoxKeyExpression);
            this.Controls.Add(this.checkBoxPrivateInvitation);
            this.Controls.Add(labelDisplay);
            this.Controls.Add(this.textBoxDisplayText);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.Controls.Add(this.comboBoxInvitationProject);
            this.Controls.Add(labelIn);
            this.Controls.Add(this.comboBoxInvitationStartingPoint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InsertInvitationDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Insert Invitation";
            this.Load += new System.EventHandler(this.InsertInvitation_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxInvitationProject;
		private System.Windows.Forms.ComboBox comboBoxInvitationStartingPoint;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
        private Tawala.Controls.ExpressionTextBox textBoxDisplayText;
		private System.Windows.Forms.CheckBox checkBoxPrivateInvitation;
		private Tawala.Controls.ExpressionTextBox textBoxKeyExpression;
		private System.Windows.Forms.Label labelPrivate;
		private System.Windows.Forms.CheckBox checkBoxOneTimeOnly;
	}
}