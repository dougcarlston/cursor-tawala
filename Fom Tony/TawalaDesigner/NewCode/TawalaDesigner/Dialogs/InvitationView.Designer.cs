namespace Tawala.Dialogs
{
    partial class InvitationView
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
            System.Windows.Forms.Label labelDisplay;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label labelIn;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InvitationView));
            this.labelPrivate = new System.Windows.Forms.Label();
            this.textBoxKeyExpression = new Tawala.Controls.ExpressionTextBox();
            this.checkBoxPrivateInvitation = new System.Windows.Forms.CheckBox();
            this.textBoxDisplayText = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.comboBoxInvitationProject = new System.Windows.Forms.ComboBox();
            this.comboBoxInvitationStartingPoint = new System.Windows.Forms.ComboBox();
            labelDisplay = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            labelIn = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelDisplay
            // 
            labelDisplay.AutoSize = true;
            labelDisplay.Location = new System.Drawing.Point(11, 53);
            labelDisplay.Name = "labelDisplay";
            labelDisplay.Size = new System.Drawing.Size(68, 13);
            labelDisplay.TabIndex = 19;
            labelDisplay.Text = "Display Text:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(209, 9);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(43, 13);
            label2.TabIndex = 17;
            label2.Text = "Project:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(11, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(33, 13);
            label1.TabIndex = 14;
            label1.Text = "Form:";
            // 
            // labelIn
            // 
            labelIn.Location = new System.Drawing.Point(178, 26);
            labelIn.Name = "labelIn";
            labelIn.Size = new System.Drawing.Size(28, 21);
            labelIn.TabIndex = 16;
            labelIn.Text = "in";
            labelIn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelPrivate
            // 
            this.labelPrivate.Location = new System.Drawing.Point(15, 136);
            this.labelPrivate.Name = "labelPrivate";
            this.labelPrivate.Size = new System.Drawing.Size(355, 57);
            this.labelPrivate.TabIndex = 25;
            this.labelPrivate.Text = resources.GetString("labelPrivate.Text");
            // 
            // textBoxKeyExpression
            // 
            this.textBoxKeyExpression.AllowDrop = true;
            this.textBoxKeyExpression.Location = new System.Drawing.Point(12, 196);
            this.textBoxKeyExpression.Name = "textBoxKeyExpression";
            this.textBoxKeyExpression.Size = new System.Drawing.Size(358, 20);
            this.textBoxKeyExpression.TabIndex = 24;
            this.textBoxKeyExpression.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxKeyExpression_DragDrop);
            this.textBoxKeyExpression.Leave += new System.EventHandler(this.textBoxKeyExpression_Leave);
            this.textBoxKeyExpression.Enter += new System.EventHandler(this.textBoxKeyExpression_Enter);
            this.textBoxKeyExpression.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxKeyExpression_DragEnter);
            // 
            // checkBoxPrivateInvitation
            // 
            this.checkBoxPrivateInvitation.AutoSize = true;
            this.checkBoxPrivateInvitation.Location = new System.Drawing.Point(12, 106);
            this.checkBoxPrivateInvitation.Name = "checkBoxPrivateInvitation";
            this.checkBoxPrivateInvitation.Size = new System.Drawing.Size(161, 17);
            this.checkBoxPrivateInvitation.TabIndex = 23;
            this.checkBoxPrivateInvitation.Text = "Make this a private invitation";
            this.checkBoxPrivateInvitation.UseVisualStyleBackColor = true;
            // 
            // textBoxDisplayText
            // 
            this.textBoxDisplayText.Location = new System.Drawing.Point(12, 70);
            this.textBoxDisplayText.Name = "textBoxDisplayText";
            this.textBoxDisplayText.Size = new System.Drawing.Size(358, 20);
            this.textBoxDisplayText.TabIndex = 20;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(206, 234);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 22;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(103, 234);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 21;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // comboBoxInvitationProject
            // 
            this.comboBoxInvitationProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInvitationProject.FormattingEnabled = true;
            this.comboBoxInvitationProject.Location = new System.Drawing.Point(210, 26);
            this.comboBoxInvitationProject.MaxDropDownItems = 20;
            this.comboBoxInvitationProject.Name = "comboBoxInvitationProject";
            this.comboBoxInvitationProject.Size = new System.Drawing.Size(160, 21);
            this.comboBoxInvitationProject.TabIndex = 18;
            // 
            // comboBoxInvitationStartingPoint
            // 
            this.comboBoxInvitationStartingPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInvitationStartingPoint.FormattingEnabled = true;
            this.comboBoxInvitationStartingPoint.Location = new System.Drawing.Point(12, 26);
            this.comboBoxInvitationStartingPoint.MaxDropDownItems = 20;
            this.comboBoxInvitationStartingPoint.Name = "comboBoxInvitationStartingPoint";
            this.comboBoxInvitationStartingPoint.Size = new System.Drawing.Size(160, 21);
            this.comboBoxInvitationStartingPoint.TabIndex = 15;
            // 
            // InvitationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(381, 266);
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
            this.Name = "InvitationView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Insert Invitation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPrivate;
        private Tawala.Controls.ExpressionTextBox textBoxKeyExpression;
        private System.Windows.Forms.CheckBox checkBoxPrivateInvitation;
        private System.Windows.Forms.TextBox textBoxDisplayText;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox comboBoxInvitationProject;
        private System.Windows.Forms.ComboBox comboBoxInvitationStartingPoint;
    }
}