namespace Tawala.FunctionConfiguration
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
			this.groupBoxConditions = new System.Windows.Forms.GroupBox();
			this.comboBoxAllAny = new System.Windows.Forms.ComboBox();
			this.labelAllAnyPrefix = new System.Windows.Forms.Label();
			this.labelAllAnySuffix = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(209, 419);
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
			this.buttonCancel.Location = new System.Drawing.Point(308, 419);
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
			this.labelParameterName.Location = new System.Drawing.Point(13, 352);
			this.labelParameterName.Name = "labelParameterName";
			this.labelParameterName.Size = new System.Drawing.Size(100, 13);
			this.labelParameterName.TabIndex = 3;
			this.labelParameterName.Text = "Parameter Name";
			// 
			// labelParameterDescription
			// 
			this.labelParameterDescription.Location = new System.Drawing.Point(13, 367);
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
			this.panelParameters.Size = new System.Drawing.Size(509, 128);
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
			// groupBoxConditions
			// 
			this.groupBoxConditions.Location = new System.Drawing.Point(16, 260);
			this.groupBoxConditions.Name = "groupBoxConditions";
			this.groupBoxConditions.Size = new System.Drawing.Size(509, 76);
			this.groupBoxConditions.TabIndex = 7;
			this.groupBoxConditions.TabStop = false;
			// 
			// comboBoxAllAny
			// 
			this.comboBoxAllAny.FormattingEnabled = true;
			this.comboBoxAllAny.Items.AddRange(new object[] {
            "all",
            "any"});
			this.comboBoxAllAny.Location = new System.Drawing.Point(69, 233);
			this.comboBoxAllAny.Name = "comboBoxAllAny";
			this.comboBoxAllAny.Size = new System.Drawing.Size(53, 21);
			this.comboBoxAllAny.TabIndex = 8;
			this.comboBoxAllAny.VisibleChanged += new System.EventHandler(this.comboBoxAllAny_VisibleChanged);
			// 
			// labelAllAnyPrefix
			// 
			this.labelAllAnyPrefix.AutoSize = true;
			this.labelAllAnyPrefix.Location = new System.Drawing.Point(24, 236);
			this.labelAllAnyPrefix.Name = "labelAllAnyPrefix";
			this.labelAllAnyPrefix.Size = new System.Drawing.Size(39, 13);
			this.labelAllAnyPrefix.TabIndex = 9;
			this.labelAllAnyPrefix.Text = "Where";
			// 
			// labelAllAnySuffix
			// 
			this.labelAllAnySuffix.AutoSize = true;
			this.labelAllAnySuffix.Location = new System.Drawing.Point(132, 236);
			this.labelAllAnySuffix.Name = "labelAllAnySuffix";
			this.labelAllAnySuffix.Size = new System.Drawing.Size(171, 13);
			this.labelAllAnySuffix.TabIndex = 10;
			this.labelAllAnySuffix.Text = "of the following conditions are true:";
			// 
			// ConfigureFunctionDialogPhase2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(545, 454);
			this.Controls.Add(this.labelAllAnySuffix);
			this.Controls.Add(this.labelAllAnyPrefix);
			this.Controls.Add(this.comboBoxAllAny);
			this.Controls.Add(this.groupBoxConditions);
			this.Controls.Add(this.labelFunctionDescription);
			this.Controls.Add(this.panelParameters);
			this.Controls.Add(this.labelParameterDescription);
			this.Controls.Add(this.labelParameterName);
			this.Controls.Add(this.labelFunctionName);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Name = "ConfigureFunctionDialogPhase2";
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
		private System.Windows.Forms.GroupBox groupBoxConditions;
		private System.Windows.Forms.ComboBox comboBoxAllAny;
		private System.Windows.Forms.Label labelAllAnyPrefix;
		private System.Windows.Forms.Label labelAllAnySuffix;
	}
}