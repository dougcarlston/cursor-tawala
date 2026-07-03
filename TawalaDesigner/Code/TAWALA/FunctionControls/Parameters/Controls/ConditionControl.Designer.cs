namespace Tawala.Functions.Controls
{
	partial class ConditionControl
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
            this.textBoxField = new Tawala.Functions.Controls.ConditionLeftFieldControl();
            this.comboBoxOperator = new Tawala.Functions.Controls.ConditionOperatorControl();
            this.textBoxExpression = new Tawala.Functions.Controls.ConditionRightExpressionControl();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxField
            // 
            this.textBoxField.AllowDrop = true;
            this.textBoxField.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxField.Location = new System.Drawing.Point(2, 3);
            this.textBoxField.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxField.MaxLength = 255;
            this.textBoxField.MinimumSize = new System.Drawing.Size(130, 20);
            this.textBoxField.Name = "textBoxField";
            this.textBoxField.ReadOnly = true;
            this.textBoxField.Size = new System.Drawing.Size(130, 20);
            this.textBoxField.TabIndex = 0;
            this.textBoxField.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxField_KeyDown);
            // 
            // comboBoxOperator
            // 
            this.comboBoxOperator.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOperator.FormattingEnabled = true;
            this.comboBoxOperator.Location = new System.Drawing.Point(138, 2);
            this.comboBoxOperator.Margin = new System.Windows.Forms.Padding(0);
            this.comboBoxOperator.MaximumSize = new System.Drawing.Size(125, 0);
            this.comboBoxOperator.MinimumSize = new System.Drawing.Size(125, 0);
            this.comboBoxOperator.Name = "comboBoxOperator";
            this.comboBoxOperator.Size = new System.Drawing.Size(125, 21);
            this.comboBoxOperator.TabIndex = 1;
            this.comboBoxOperator.SelectedIndexChanged += new System.EventHandler(this.comboBoxOperator_SelectedIndexChanged);
            // 
            // textBoxExpression
            // 
            this.textBoxExpression.AllowDrop = true;
            this.textBoxExpression.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxExpression.Expression = null;
            this.textBoxExpression.Location = new System.Drawing.Point(267, 3);
            this.textBoxExpression.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxExpression.MaxLength = 255;
            this.textBoxExpression.MinimumSize = new System.Drawing.Size(130, 20);
            this.textBoxExpression.Name = "textBoxExpression";
            this.textBoxExpression.Size = new System.Drawing.Size(130, 20);
            this.textBoxExpression.TabIndex = 2;
            this.textBoxExpression.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxExpression_KeyDown);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAdd.Location = new System.Drawing.Point(403, 1);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(0);
            this.buttonAdd.MaximumSize = new System.Drawing.Size(23, 23);
            this.buttonAdd.MinimumSize = new System.Drawing.Size(23, 23);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(23, 23);
            this.buttonAdd.TabIndex = 4;
            this.buttonAdd.Text = "+";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonPlus_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDelete.Location = new System.Drawing.Point(432, 1);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.buttonDelete.MaximumSize = new System.Drawing.Size(23, 23);
            this.buttonDelete.MinimumSize = new System.Drawing.Size(23, 23);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(23, 23);
            this.buttonDelete.TabIndex = 5;
            this.buttonDelete.Text = "-";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonMinus_Click);
            // 
            // ConditionControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.textBoxField);
            this.Controls.Add(this.comboBoxOperator);
            this.Controls.Add(this.textBoxExpression);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonDelete);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(0, 25);
            this.MinimumSize = new System.Drawing.Size(457, 25);
            this.Name = "ConditionControl";
            this.Size = new System.Drawing.Size(457, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private ConditionOperatorControl comboBoxOperator;
		private ConditionLeftFieldControl textBoxField;
        private ConditionRightExpressionControl textBoxExpression;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
	}
}
