namespace Tawala.Functions.Controls
{
	partial class ConfigureFunctionControl
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
            this.panelFunctionInfo = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelFunctionName = new System.Windows.Forms.Label();
            this.panelCurrentParameterInfo = new System.Windows.Forms.Panel();
            this.labelParamDescription = new System.Windows.Forms.Label();
            this.labelParamName = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.panelFunctionInfo.SuspendLayout();
            this.panelCurrentParameterInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelFunctionInfo
            // 
            this.panelFunctionInfo.AutoSize = true;
            this.panelFunctionInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelFunctionInfo.Controls.Add(this.groupBox2);
            this.panelFunctionInfo.Controls.Add(this.labelDescription);
            this.panelFunctionInfo.Controls.Add(this.labelFunctionName);
            this.panelFunctionInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFunctionInfo.Location = new System.Drawing.Point(0, 0);
            this.panelFunctionInfo.Margin = new System.Windows.Forms.Padding(0);
            this.panelFunctionInfo.MaximumSize = new System.Drawing.Size(0, 70);
            this.panelFunctionInfo.MinimumSize = new System.Drawing.Size(400, 70);
            this.panelFunctionInfo.Name = "panelFunctionInfo";
            this.panelFunctionInfo.Size = new System.Drawing.Size(400, 70);
            this.panelFunctionInfo.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 60);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 10);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // labelDescription
            // 
            this.labelDescription.BackColor = System.Drawing.SystemColors.Control;
            this.labelDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription.Location = new System.Drawing.Point(0, 20);
            this.labelDescription.Margin = new System.Windows.Forms.Padding(0);
            this.labelDescription.MinimumSize = new System.Drawing.Size(0, 50);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(400, 50);
            this.labelDescription.TabIndex = 2;
            this.labelDescription.Text = "Function Description";
            // 
            // labelFunctionName
            // 
            this.labelFunctionName.BackColor = System.Drawing.SystemColors.Control;
            this.labelFunctionName.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelFunctionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFunctionName.Location = new System.Drawing.Point(0, 0);
            this.labelFunctionName.Margin = new System.Windows.Forms.Padding(0);
            this.labelFunctionName.Name = "labelFunctionName";
            this.labelFunctionName.Size = new System.Drawing.Size(400, 20);
            this.labelFunctionName.TabIndex = 1;
            this.labelFunctionName.Text = "FUNCTION NAME";
            this.labelFunctionName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelCurrentParameterInfo
            // 
            this.panelCurrentParameterInfo.AutoSize = true;
            this.panelCurrentParameterInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelCurrentParameterInfo.Controls.Add(this.labelParamDescription);
            this.panelCurrentParameterInfo.Controls.Add(this.labelParamName);
            this.panelCurrentParameterInfo.Controls.Add(this.groupBox1);
            this.panelCurrentParameterInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelCurrentParameterInfo.Location = new System.Drawing.Point(0, 270);
            this.panelCurrentParameterInfo.Margin = new System.Windows.Forms.Padding(0);
            this.panelCurrentParameterInfo.MaximumSize = new System.Drawing.Size(0, 70);
            this.panelCurrentParameterInfo.MinimumSize = new System.Drawing.Size(400, 70);
            this.panelCurrentParameterInfo.Name = "panelCurrentParameterInfo";
            this.panelCurrentParameterInfo.Size = new System.Drawing.Size(400, 70);
            this.panelCurrentParameterInfo.TabIndex = 3;
            // 
            // labelParamDescription
            // 
            this.labelParamDescription.BackColor = System.Drawing.SystemColors.Control;
            this.labelParamDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelParamDescription.Location = new System.Drawing.Point(0, 30);
            this.labelParamDescription.Margin = new System.Windows.Forms.Padding(0);
            this.labelParamDescription.MaximumSize = new System.Drawing.Size(0, 50);
            this.labelParamDescription.MinimumSize = new System.Drawing.Size(0, 50);
            this.labelParamDescription.Name = "labelParamDescription";
            this.labelParamDescription.Size = new System.Drawing.Size(400, 50);
            this.labelParamDescription.TabIndex = 0;
            this.labelParamDescription.Text = "Parameter Description";
            // 
            // labelParamName
            // 
            this.labelParamName.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelParamName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelParamName.Location = new System.Drawing.Point(0, 10);
            this.labelParamName.Margin = new System.Windows.Forms.Padding(0);
            this.labelParamName.Name = "labelParamName";
            this.labelParamName.Size = new System.Drawing.Size(400, 20);
            this.labelParamName.TabIndex = 1;
            this.labelParamName.Text = "Parameter Name";
            this.labelParamName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 10);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // flowLayout
            // 
            this.flowLayout.AllowDrop = true;
            this.flowLayout.AutoScroll = true;
            this.flowLayout.AutoSize = true;
            this.flowLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayout.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayout.Location = new System.Drawing.Point(0, 70);
            this.flowLayout.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayout.MinimumSize = new System.Drawing.Size(400, 200);
            this.flowLayout.Name = "flowLayout";
            this.flowLayout.Padding = new System.Windows.Forms.Padding(2, 6, 2, 6);
            this.flowLayout.Size = new System.Drawing.Size(400, 200);
            this.flowLayout.TabIndex = 2;
            this.flowLayout.TabStop = true;
            // 
            // ConfigureFunctionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowLayout);
            this.Controls.Add(this.panelCurrentParameterInfo);
            this.Controls.Add(this.panelFunctionInfo);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "ConfigureFunctionControl";
            this.Size = new System.Drawing.Size(400, 340);
            this.panelFunctionInfo.ResumeLayout(false);
            this.panelCurrentParameterInfo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panelFunctionInfo;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.Label labelFunctionName;
		private System.Windows.Forms.Panel panelCurrentParameterInfo;
		private System.Windows.Forms.Label labelParamDescription;
		private System.Windows.Forms.Label labelParamName;
		private System.Windows.Forms.FlowLayoutPanel flowLayout;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
	}
}
