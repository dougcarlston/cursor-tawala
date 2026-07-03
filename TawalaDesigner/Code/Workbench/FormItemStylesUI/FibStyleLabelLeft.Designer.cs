namespace FormItemStylesUI
{
	partial class FibStyleLabelLeft
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
			this.radioButtonFibStyle = new System.Windows.Forms.RadioButton();
			this.labelFibLabel = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// radioButtonFibStyle
			// 
			this.radioButtonFibStyle.AutoSize = true;
			this.radioButtonFibStyle.Location = new System.Drawing.Point(4, 12);
			this.radioButtonFibStyle.Name = "radioButtonFibStyle";
			this.radioButtonFibStyle.Size = new System.Drawing.Size(14, 13);
			this.radioButtonFibStyle.TabIndex = 0;
			this.radioButtonFibStyle.TabStop = true;
			this.radioButtonFibStyle.UseVisualStyleBackColor = true;
			this.radioButtonFibStyle.CheckedChanged += new System.EventHandler(this.radioButtonFibStyle_CheckedChanged);
			// 
			// labelFibLabel
			// 
			this.labelFibLabel.Location = new System.Drawing.Point(25, 12);
			this.labelFibLabel.Name = "labelFibLabel";
			this.labelFibLabel.Size = new System.Drawing.Size(126, 13);
			this.labelFibLabel.TabIndex = 1;
			this.labelFibLabel.Text = "Left-aligned Label:";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(157, 8);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(204, 20);
			this.textBox1.TabIndex = 2;
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// FibStyleLabelLeft
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.labelFibLabel);
			this.Controls.Add(this.radioButtonFibStyle);
			this.Name = "FibStyleLabelLeft";
			this.Size = new System.Drawing.Size(388, 36);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButtonFibStyle;
		private System.Windows.Forms.Label labelFibLabel;
		private System.Windows.Forms.TextBox textBox1;
	}
}
