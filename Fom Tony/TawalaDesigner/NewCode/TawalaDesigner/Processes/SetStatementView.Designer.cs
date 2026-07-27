// $Workfile: SetStatementView.Designer.cs $
// $Revision: 1 $	$Date: 1/09/07 9:09a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Processes
{
	/// <summary>
	/// SetDetails for SetStatement
	/// </summary>
    partial class SetStatementView 
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
            this.buttonAddModify = new Tawala.Processes.AddModifyButton();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageSet = new System.Windows.Forms.TabPage();
            this.labelArithmeticAsText = new System.Windows.Forms.Label();
            this.checkBoxArithmeticAsText = new System.Windows.Forms.CheckBox();
            this.textBoxVariable = new Tawala.Controls.VariableTextBox();
            this.textBoxExpression = new Tawala.Controls.ComplexExpressionTextBox();
            this.labelTo = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPageSet.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAddModify
            // 
            this.buttonAddModify.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonAddModify.Location = new System.Drawing.Point(172, 124);
            this.buttonAddModify.Name = "buttonAddModify";
            this.buttonAddModify.Size = new System.Drawing.Size(80, 24);
            this.buttonAddModify.TabIndex = 5;
            this.buttonAddModify.Text = "Add";
            this.buttonAddModify.Click += new System.EventHandler(this.buttonAddModify_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageSet);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(432, 176);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageSet
            // 
            this.tabPageSet.Controls.Add(this.labelArithmeticAsText);
            this.tabPageSet.Controls.Add(this.checkBoxArithmeticAsText);
            this.tabPageSet.Controls.Add(this.textBoxVariable);
            this.tabPageSet.Controls.Add(this.textBoxExpression);
            this.tabPageSet.Controls.Add(this.labelTo);
            this.tabPageSet.Controls.Add(this.buttonAddModify);
            this.tabPageSet.Location = new System.Drawing.Point(4, 22);
            this.tabPageSet.Name = "tabPageSet";
            this.tabPageSet.Size = new System.Drawing.Size(424, 150);
            this.tabPageSet.TabIndex = 0;
            this.tabPageSet.Text = "Set";
            this.tabPageSet.UseVisualStyleBackColor = true;
            // 
            // labelArithmeticAsText
            // 
            this.labelArithmeticAsText.AutoSize = true;
            this.labelArithmeticAsText.Location = new System.Drawing.Point(206, 63);
            this.labelArithmeticAsText.Name = "labelArithmeticAsText";
            this.labelArithmeticAsText.Size = new System.Drawing.Size(172, 13);
            this.labelArithmeticAsText.TabIndex = 4;
            this.labelArithmeticAsText.Text = "(do not interpret +, -, * or / as math)";
            // 
            // checkBoxArithmeticAsText
            // 
            this.checkBoxArithmeticAsText.AutoSize = true;
            this.checkBoxArithmeticAsText.Location = new System.Drawing.Point(190, 46);
            this.checkBoxArithmeticAsText.Name = "checkBoxArithmeticAsText";
            this.checkBoxArithmeticAsText.Size = new System.Drawing.Size(186, 17);
            this.checkBoxArithmeticAsText.TabIndex = 3;
            this.checkBoxArithmeticAsText.Text = "Treat arithmetic expression as text";
            this.checkBoxArithmeticAsText.UseVisualStyleBackColor = true;
            // 
            // textBoxVariable
            // 
            this.textBoxVariable.AllowDrop = true;
            this.textBoxVariable.Location = new System.Drawing.Point(13, 20);
            this.textBoxVariable.Name = "textBoxVariable";
            this.textBoxVariable.Size = new System.Drawing.Size(148, 20);
            this.textBoxVariable.TabIndex = 0;
            this.textBoxVariable.Enter += new System.EventHandler(this.textBoxVariable_Enter);
            this.textBoxVariable.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxVariable_DragDrop);
            this.textBoxVariable.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxVariable_DragEnter);
            // 
            // textBoxExpression
            // 
            this.textBoxExpression.AllowDrop = true;
            this.textBoxExpression.Location = new System.Drawing.Point(190, 20);
            this.textBoxExpression.Name = "textBoxExpression";
            this.textBoxExpression.Size = new System.Drawing.Size(222, 20);
            this.textBoxExpression.TabIndex = 2;
            this.textBoxExpression.Enter += new System.EventHandler(this.textBoxExpression_Enter);
            this.textBoxExpression.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxExpression_DragDrop);
            this.textBoxExpression.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxExpression_DragEnter);
            // 
            // labelTo
            // 
            this.labelTo.Location = new System.Drawing.Point(160, 18);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(24, 23);
            this.labelTo.TabIndex = 1;
            this.labelTo.Text = "to";
            this.labelTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SetDetails
            // 
            this.Controls.Add(this.tabControl);
            this.Name = "SetDetails";
            this.tabControl.ResumeLayout(false);
            this.tabPageSet.ResumeLayout(false);
            this.tabPageSet.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private Tawala.Processes.AddModifyButton buttonAddModify;
        private System.Windows.Forms.TabPage tabPageSet;
        private System.Windows.Forms.Label labelTo;
        private Tawala.Controls.ComplexExpressionTextBox textBoxExpression;
        private System.Windows.Forms.TabControl tabControl;
        private Tawala.Controls.VariableTextBox textBoxVariable;

        private System.Windows.Forms.CheckBox checkBoxArithmeticAsText;
        private System.Windows.Forms.Label labelArithmeticAsText;

    }
}
