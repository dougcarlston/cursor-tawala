namespace Tawala.FormDesigner.FormItemOptions
{
	partial class HeadingOptionsView
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
			this.textBoxHeadingLabel = new System.Windows.Forms.TextBox();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.labelHeading = new System.Windows.Forms.Label();
			this.labelStatus = new System.Windows.Forms.Label();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxHeadingLabel
			// 
			this.flowLayoutPanel1.SetFlowBreak(this.textBoxHeadingLabel, true);
			this.textBoxHeadingLabel.Location = new System.Drawing.Point(7, 24);
			this.textBoxHeadingLabel.MaxLength = 64;
			this.textBoxHeadingLabel.Name = "textBoxHeadingLabel";
			this.textBoxHeadingLabel.Size = new System.Drawing.Size(132, 20);
			this.textBoxHeadingLabel.TabIndex = 6;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.labelHeading);
			this.flowLayoutPanel1.Controls.Add(this.textBoxHeadingLabel);
			this.flowLayoutPanel1.Controls.Add(this.labelStatus);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(4, 4, 0, 0);
			this.flowLayoutPanel1.Size = new System.Drawing.Size(150, 150);
			this.flowLayoutPanel1.TabIndex = 10;
			this.flowLayoutPanel1.WrapContents = false;
			// 
			// labelHeading
			// 
			this.labelHeading.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelHeading.AutoSize = true;
			this.flowLayoutPanel1.SetFlowBreak(this.labelHeading, true);
			this.labelHeading.Location = new System.Drawing.Point(7, 4);
			this.labelHeading.Name = "labelHeading";
			this.labelHeading.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this.labelHeading.Size = new System.Drawing.Size(79, 17);
			this.labelHeading.TabIndex = 11;
			this.labelHeading.Text = "Heading Label:";
			// 
			// labelStatus
			// 
			this.labelStatus.AutoSize = true;
			this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.labelStatus.Location = new System.Drawing.Point(7, 47);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			this.labelStatus.Size = new System.Drawing.Size(0, 21);
			this.labelStatus.TabIndex = 16;
			// 
			// HeadingOptionsView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "HeadingOptionsView";
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxHeadingLabel;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label labelHeading;
		private System.Windows.Forms.Label labelStatus;
	}
}
