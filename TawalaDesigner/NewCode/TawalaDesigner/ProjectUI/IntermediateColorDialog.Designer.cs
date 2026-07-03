namespace Tawala.ProjectUI
{
	partial class IntermediateColorDialog
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
			this.radioButtonUseThemeColor = new System.Windows.Forms.RadioButton();
			this.radioButtonChooseColor = new System.Windows.Forms.RadioButton();
			this.buttonOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// radioButtonUseThemeColor
			// 
			this.radioButtonUseThemeColor.AutoSize = true;
			this.radioButtonUseThemeColor.Checked = true;
			this.radioButtonUseThemeColor.Location = new System.Drawing.Point(29, 23);
			this.radioButtonUseThemeColor.Name = "radioButtonUseThemeColor";
			this.radioButtonUseThemeColor.Size = new System.Drawing.Size(180, 17);
			this.radioButtonUseThemeColor.TabIndex = 0;
			this.radioButtonUseThemeColor.TabStop = true;
			this.radioButtonUseThemeColor.Text = "Use Tawala Project Theme color";
			this.radioButtonUseThemeColor.UseVisualStyleBackColor = true;
			// 
			// radioButtonChooseColor
			// 
			this.radioButtonChooseColor.AutoSize = true;
			this.radioButtonChooseColor.Location = new System.Drawing.Point(29, 46);
			this.radioButtonChooseColor.Name = "radioButtonChooseColor";
			this.radioButtonChooseColor.Size = new System.Drawing.Size(141, 17);
			this.radioButtonChooseColor.TabIndex = 1;
			this.radioButtonChooseColor.TabStop = true;
			this.radioButtonChooseColor.Text = "Choose or create a color";
			this.radioButtonChooseColor.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(82, 82);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// IntermediateColorDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(239, 128);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.radioButtonChooseColor);
			this.Controls.Add(this.radioButtonUseThemeColor);
			this.Name = "IntermediateColorDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButtonUseThemeColor;
		private System.Windows.Forms.RadioButton radioButtonChooseColor;
		private System.Windows.Forms.Button buttonOK;
	}
}