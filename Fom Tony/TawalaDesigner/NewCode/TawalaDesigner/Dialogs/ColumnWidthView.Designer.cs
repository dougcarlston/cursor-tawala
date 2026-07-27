namespace Tawala.Dialogs
{
	partial class ColumnWidthView
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
			this.numericUpDownColumnWidth = new System.Windows.Forms.NumericUpDown();
			this.labelColumnWidth = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownColumnWidth)).BeginInit();
			this.SuspendLayout();
			// 
			// numericUpDownColumnWidth
			// 
			this.numericUpDownColumnWidth.DecimalPlaces = 2;
			this.numericUpDownColumnWidth.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
			this.numericUpDownColumnWidth.Location = new System.Drawing.Point(179, 25);
			this.numericUpDownColumnWidth.Name = "numericUpDownColumnWidth";
			this.numericUpDownColumnWidth.Size = new System.Drawing.Size(54, 20);
			this.numericUpDownColumnWidth.TabIndex = 0;
			// 
			// labelColumnWidth
			// 
			this.labelColumnWidth.AutoSize = true;
			this.labelColumnWidth.Location = new System.Drawing.Point(48, 29);
			this.labelColumnWidth.Name = "labelColumnWidth";
			this.labelColumnWidth.Size = new System.Drawing.Size(116, 13);
			this.labelColumnWidth.TabIndex = 1;
			this.labelColumnWidth.Text = "Column Width (inches):";
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(80, 76);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(58, 23);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(152, 76);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(56, 23);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// ColumnWidthView
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(289, 120);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.labelColumnWidth);
			this.Controls.Add(this.numericUpDownColumnWidth);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ColumnWidthView";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Set Column Width";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownColumnWidth)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDownColumnWidth;
		private System.Windows.Forms.Label labelColumnWidth;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
	}
}