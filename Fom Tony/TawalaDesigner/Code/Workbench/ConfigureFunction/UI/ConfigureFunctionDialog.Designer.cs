namespace Tawala.Controls
{
	partial class ConfigureFunctionDialogPhase2
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
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelFunctionName = new System.Windows.Forms.Label();
			this.labelParameterName = new System.Windows.Forms.Label();
			this.labelParameterDescription = new System.Windows.Forms.Label();
			this.panelParameters = new System.Windows.Forms.Panel();
			this.labelFunctionDescription = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(155, 281);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(254, 281);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// labelFunctionName
			// 
			this.labelFunctionName.AutoSize = true;
			this.labelFunctionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFunctionName.Location = new System.Drawing.Point(13, 13);
			this.labelFunctionName.Name = "labelFunctionName";
			this.labelFunctionName.Size = new System.Drawing.Size(109, 13);
			this.labelFunctionName.TabIndex = 2;
			this.labelFunctionName.Text = "FUNCTION NAME";
			// 
			// labelParameterName
			// 
			this.labelParameterName.AutoSize = true;
			this.labelParameterName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelParameterName.Location = new System.Drawing.Point(13, 215);
			this.labelParameterName.Name = "labelParameterName";
			this.labelParameterName.Size = new System.Drawing.Size(100, 13);
			this.labelParameterName.TabIndex = 3;
			this.labelParameterName.Text = "Parameter Name";
			// 
			// labelParameterDescription
			// 
			this.labelParameterDescription.Location = new System.Drawing.Point(13, 230);
			this.labelParameterDescription.Name = "labelParameterDescription";
			this.labelParameterDescription.Size = new System.Drawing.Size(453, 31);
			this.labelParameterDescription.TabIndex = 4;
			this.labelParameterDescription.Text = "Parameter description goes here.";
			// 
			// panelParameters
			// 
			this.panelParameters.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panelParameters.Location = new System.Drawing.Point(16, 80);
			this.panelParameters.Name = "panelParameters";
			this.panelParameters.Size = new System.Drawing.Size(450, 128);
			this.panelParameters.TabIndex = 5;
			// 
			// labelFunctionDescription
			// 
			this.labelFunctionDescription.Location = new System.Drawing.Point(13, 31);
			this.labelFunctionDescription.Name = "labelFunctionDescription";
			this.labelFunctionDescription.Size = new System.Drawing.Size(453, 27);
			this.labelFunctionDescription.TabIndex = 6;
			this.labelFunctionDescription.Text = "Function description goes here.";
			// 
			// ConfigureFunctionDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(485, 326);
			this.Controls.Add(this.labelFunctionDescription);
			this.Controls.Add(this.panelParameters);
			this.Controls.Add(this.labelParameterDescription);
			this.Controls.Add(this.labelParameterName);
			this.Controls.Add(this.labelFunctionName);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Name = "ConfigureFunctionDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Configure Function";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelFunctionName;
		private System.Windows.Forms.Label labelParameterName;
		private System.Windows.Forms.Label labelParameterDescription;
		private System.Windows.Forms.Panel panelParameters;
		private System.Windows.Forms.Label labelFunctionDescription;
	}
}