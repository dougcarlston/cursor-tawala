namespace Tawala.Controls
{
	partial class ConditionGroup
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
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.textBoxField = new Tawala.Controls.FieldTextBox();
            this.textBoxExpression = new Tawala.Controls.ExpressionTextBox();
            this.flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxOperator
            // 
            this.comboBoxOperator.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBoxOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOperator.FormattingEnabled = true;
            this.comboBoxOperator.Location = new System.Drawing.Point(134, 4);
            this.comboBoxOperator.Name = "comboBoxOperator";
            this.comboBoxOperator.Size = new System.Drawing.Size(125, 21);
            this.comboBoxOperator.TabIndex = 1;
            this.comboBoxOperator.SelectedIndexChanged += new System.EventHandler(this.comboBoxOperator_SelectedIndexChanged);
            // 
            // buttonPlus
            // 
            this.buttonPlus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonPlus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlus.Location = new System.Drawing.Point(396, 3);
            this.buttonPlus.MaximumSize = new System.Drawing.Size(26, 23);
            this.buttonPlus.Name = "buttonPlus";
            this.buttonPlus.Size = new System.Drawing.Size(26, 23);
            this.buttonPlus.TabIndex = 3;
            this.buttonPlus.Text = "+";
            this.buttonPlus.UseVisualStyleBackColor = true;
            this.buttonPlus.Click += new System.EventHandler(this.buttonPlus_Click);
            // 
            // buttonMinus
            // 
            this.buttonMinus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonMinus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonMinus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMinus.Location = new System.Drawing.Point(428, 3);
            this.buttonMinus.MaximumSize = new System.Drawing.Size(26, 23);
            this.buttonMinus.Name = "buttonMinus";
            this.buttonMinus.Size = new System.Drawing.Size(26, 23);
            this.buttonMinus.TabIndex = 4;
            this.buttonMinus.Text = "-";
            this.buttonMinus.UseVisualStyleBackColor = true;
            this.buttonMinus.Click += new System.EventHandler(this.buttonMinus_Click);
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoSize = true;
            this.flowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel.Controls.Add(this.textBoxField);
            this.flowLayoutPanel.Controls.Add(this.comboBoxOperator);
            this.flowLayoutPanel.Controls.Add(this.textBoxExpression);
            this.flowLayoutPanel.Controls.Add(this.buttonPlus);
            this.flowLayoutPanel.Controls.Add(this.buttonMinus);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(457, 29);
            this.flowLayoutPanel.TabIndex = 6;
            this.flowLayoutPanel.WrapContents = false;
            // 
            // textBoxField
            // 
            this.textBoxField.AllowDrop = true;
            this.textBoxField.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBoxField.Location = new System.Drawing.Point(3, 4);
            this.textBoxField.Name = "textBoxField";
            this.textBoxField.ReadOnly = true;
            this.textBoxField.Size = new System.Drawing.Size(125, 20);
            this.textBoxField.TabIndex = 0;
            this.textBoxField.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxField_DragDrop);
            this.textBoxField.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxField_KeyDown);
            this.textBoxField.Enter += new System.EventHandler(this.textBoxField_Enter);
            this.textBoxField.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxField_DragEnter);
            // 
            // textBoxExpression
            // 
            this.textBoxExpression.AllowDrop = true;
            this.textBoxExpression.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBoxExpression.Location = new System.Drawing.Point(265, 4);
            this.textBoxExpression.Name = "textBoxExpression";
            this.textBoxExpression.Size = new System.Drawing.Size(125, 20);
            this.textBoxExpression.TabIndex = 2;
            this.textBoxExpression.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxExpression_DragDrop);
            this.textBoxExpression.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxExpression_KeyDown);
            this.textBoxExpression.Enter += new System.EventHandler(this.textBoxExpression_Enter);
            this.textBoxExpression.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxExpression_DragEnter);
            // 
            // ConditionGroup
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowLayoutPanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(450, 26);
            this.Name = "ConditionGroup";
            this.Size = new System.Drawing.Size(457, 29);
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxOperator;
		private Tawala.Controls.FieldTextBox textBoxField;
		private Tawala.Controls.ExpressionTextBox textBoxExpression;
		private System.Windows.Forms.Button buttonPlus;
        private System.Windows.Forms.Button buttonMinus;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
	}
}
