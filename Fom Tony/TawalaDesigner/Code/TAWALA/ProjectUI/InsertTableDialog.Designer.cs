namespace Tawala.ProjectUI
{
	partial class InsertTableDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InsertTableDialog));
			this.widthNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.columnsNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.rowsNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.widthNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.columnsNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rowsNumericUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// widthNumericUpDown
			// 
			this.widthNumericUpDown.DecimalPlaces = 2;
			this.widthNumericUpDown.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
			this.widthNumericUpDown.Location = new System.Drawing.Point(124, 17);
			this.widthNumericUpDown.Maximum = new decimal(new int[] {
            75,
            0,
            0,
            65536});
			this.widthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.widthNumericUpDown.Name = "widthNumericUpDown";
			this.widthNumericUpDown.Size = new System.Drawing.Size(60, 20);
			this.widthNumericUpDown.TabIndex = 1;
			this.widthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.widthNumericUpDown.Value = new decimal(new int[] {
            75,
            0,
            0,
            65536});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(108, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Table Width (inches):";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(191, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Columns:";
			// 
			// columnsNumericUpDown
			// 
			this.columnsNumericUpDown.Location = new System.Drawing.Point(246, 17);
			this.columnsNumericUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.columnsNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.columnsNumericUpDown.Name = "columnsNumericUpDown";
			this.columnsNumericUpDown.Size = new System.Drawing.Size(50, 20);
			this.columnsNumericUpDown.TabIndex = 3;
			this.columnsNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.columnsNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(304, 21);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Rows:";
			// 
			// rowsNumericUpDown
			// 
			this.rowsNumericUpDown.Location = new System.Drawing.Point(349, 17);
			this.rowsNumericUpDown.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.rowsNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.rowsNumericUpDown.Name = "rowsNumericUpDown";
			this.rowsNumericUpDown.Size = new System.Drawing.Size(58, 20);
			this.rowsNumericUpDown.TabIndex = 5;
			this.rowsNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.rowsNumericUpDown.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(118, 62);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 6;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(221, 62);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 7;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// InsertTableDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(419, 93);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.rowsNumericUpDown);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.columnsNumericUpDown);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.widthNumericUpDown);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InsertTableDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Insert Table";
			((System.ComponentModel.ISupportInitialize)(this.widthNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.columnsNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rowsNumericUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown widthNumericUpDown;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown columnsNumericUpDown;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown rowsNumericUpDown;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;

	}
}