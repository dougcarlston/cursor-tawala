namespace Tawala.Controls
{
    partial class ConditionEditControl
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
            this.components = new System.ComponentModel.Container();
            this.comboBoxOperator = new System.Windows.Forms.ComboBox();
            this.buttonPlus = new System.Windows.Forms.Button();
            this.buttonMinus = new System.Windows.Forms.Button();
            this.textBoxField = new Tawala.Controls.FieldTextBox();
            this.textBoxExpression = new Tawala.Controls.ExpressionTextBox();
            this.SuspendLayout();
            // 
            // comboBoxOperator
            // 
            this.comboBoxOperator.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBoxOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOperator.FormattingEnabled = true;
            this.comboBoxOperator.Location = new System.Drawing.Point(136, 1);
            this.comboBoxOperator.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.comboBoxOperator.MaximumSize = new System.Drawing.Size(138, 0);
            this.comboBoxOperator.MinimumSize = new System.Drawing.Size(138, 0);
            this.comboBoxOperator.Name = "comboBoxOperator";
            this.comboBoxOperator.Size = new System.Drawing.Size(138, 21);
            this.comboBoxOperator.TabIndex = 2;
            this.comboBoxOperator.SelectedIndexChanged += new System.EventHandler(this.comboBoxOperator_SelectedIndexChanged);
            // 
            // buttonPlus
            // 
            this.buttonPlus.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlus.Location = new System.Drawing.Point(410, 0);
            this.buttonPlus.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.buttonPlus.MaximumSize = new System.Drawing.Size(26, 23);
            this.buttonPlus.MinimumSize = new System.Drawing.Size(26, 23);
            this.buttonPlus.Name = "buttonPlus";
            this.buttonPlus.Size = new System.Drawing.Size(26, 23);
            this.buttonPlus.TabIndex = 4;
            this.buttonPlus.Text = "+";
            this.buttonPlus.UseVisualStyleBackColor = true;
            this.buttonPlus.Click += new System.EventHandler(this.buttonPlus_Click);
            // 
            // buttonMinus
            // 
            this.buttonMinus.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonMinus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMinus.Location = new System.Drawing.Point(436, 0);
            this.buttonMinus.Margin = new System.Windows.Forms.Padding(0);
            this.buttonMinus.MaximumSize = new System.Drawing.Size(26, 23);
            this.buttonMinus.MinimumSize = new System.Drawing.Size(26, 23);
            this.buttonMinus.Name = "buttonMinus";
            this.buttonMinus.Size = new System.Drawing.Size(26, 23);
            this.buttonMinus.TabIndex = 5;
            this.buttonMinus.Text = "-";
            this.buttonMinus.UseVisualStyleBackColor = true;
            this.buttonMinus.Click += new System.EventHandler(this.buttonMinus_Click);
            // 
            // textBoxField
            // 
            this.textBoxField.AllowDrop = true;
            this.textBoxField.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxField.Location = new System.Drawing.Point(0, 1);
            this.textBoxField.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxField.MinimumSize = new System.Drawing.Size(125, 20);
            this.textBoxField.Name = "textBoxField";
            this.textBoxField.ReadOnly = true;
            this.textBoxField.Size = new System.Drawing.Size(125, 20);
            this.textBoxField.TabIndex = 1;
            this.textBoxField.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxField_DragDrop);
            this.textBoxField.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxField_KeyDown);
            this.textBoxField.Enter += new System.EventHandler(this.textBoxField_Enter);
            this.textBoxField.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxField_DragEnter);
            // 
            // textBoxExpression
            // 
            this.textBoxExpression.AllowDrop = true;
            this.textBoxExpression.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBoxExpression.Location = new System.Drawing.Point(282, 1);
            this.textBoxExpression.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxExpression.MinimumSize = new System.Drawing.Size(125, 20);
            this.textBoxExpression.Name = "textBoxExpression";
            this.textBoxExpression.Size = new System.Drawing.Size(125, 20);
            this.textBoxExpression.TabIndex = 3;
            this.textBoxExpression.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxExpression_DragDrop);
            this.textBoxExpression.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxExpression_KeyDown);
            this.textBoxExpression.Enter += new System.EventHandler(this.textBoxExpression_Enter);
            this.textBoxExpression.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxExpression_DragEnter);
            // 
            // ConditionEditControl
            // 
            this.AllowDrop = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.textBoxField);
            this.Controls.Add(this.comboBoxOperator);
            this.Controls.Add(this.textBoxExpression);
            this.Controls.Add(this.buttonPlus);
            this.Controls.Add(this.buttonMinus);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(0, 23);
            this.MinimumSize = new System.Drawing.Size(462, 23);
            this.Name = "ConditionEditControl";
            this.Size = new System.Drawing.Size(462, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxOperator;
		private Tawala.Controls.FieldTextBox textBoxField;
		private Tawala.Controls.ExpressionTextBox textBoxExpression;
		private System.Windows.Forms.Button buttonPlus;
        private System.Windows.Forms.Button buttonMinus;
	}
}
