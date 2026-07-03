namespace Tawala.Common
{
	partial class DataSourcesMissing
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataSourcesMissing));
			this.labelError = new System.Windows.Forms.Label();
			this.labelDeploy = new System.Windows.Forms.Label();
			this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.labelMissing = new System.Windows.Forms.Label();
			this.buttonClose = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.flowLayoutPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelError
			// 
			this.labelError.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelError.AutoSize = true;
			this.flowLayoutPanel.SetFlowBreak(this.labelError, true);
			this.labelError.Location = new System.Drawing.Point(3, 3);
			this.labelError.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
			this.labelError.Name = "labelError";
			this.labelError.Size = new System.Drawing.Size(421, 32);
			this.labelError.TabIndex = 1;
			this.labelError.Text = "Tawala could not open this project because it references one or more shared datab" +
				"ases that were not found.";
			// 
			// labelDeploy
			// 
			this.labelDeploy.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelDeploy.AutoSize = true;
			this.flowLayoutPanel.SetFlowBreak(this.labelDeploy, true);
			this.labelDeploy.Location = new System.Drawing.Point(3, 44);
			this.labelDeploy.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
			this.labelDeploy.Name = "labelDeploy";
			this.labelDeploy.Size = new System.Drawing.Size(447, 32);
			this.labelDeploy.TabIndex = 2;
			this.labelDeploy.Text = "Project(s) that contain the shared databases must be deployed to your My Tawala a" +
				"ccount before this project can be opened.";
			// 
			// flowLayoutPanel
			// 
			this.flowLayoutPanel.AutoSize = true;
			this.flowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel.Controls.Add(this.labelError);
			this.flowLayoutPanel.Controls.Add(this.labelDeploy);
			this.flowLayoutPanel.Controls.Add(this.labelMissing);
			this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel.Location = new System.Drawing.Point(8, 7);
			this.flowLayoutPanel.MaximumSize = new System.Drawing.Size(466, 800);
			this.flowLayoutPanel.Name = "flowLayoutPanel";
			this.flowLayoutPanel.Size = new System.Drawing.Size(466, 134);
			this.flowLayoutPanel.TabIndex = 4;
			// 
			// labelMissing
			// 
			this.labelMissing.AutoSize = true;
			this.flowLayoutPanel.SetFlowBreak(this.labelMissing, true);
			this.labelMissing.Location = new System.Drawing.Point(3, 85);
			this.labelMissing.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
			this.labelMissing.Name = "labelMissing";
			this.labelMissing.Size = new System.Drawing.Size(170, 16);
			this.labelMissing.TabIndex = 3;
			this.labelMissing.Text = "Missing shared databases:";
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(196, 3);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 4;
			this.buttonClose.Text = "OK";
			this.buttonClose.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.buttonClose);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(8, 141);
			this.panel1.Margin = new System.Windows.Forms.Padding(6, 12, 6, 6);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(3, 12, 3, 3);
			this.panel1.Size = new System.Drawing.Size(467, 29);
			this.panel1.TabIndex = 5;
			// 
			// DataSourcesMissing
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(483, 177);
			this.Controls.Add(this.flowLayoutPanel);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataSourcesMissing";
			this.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Unable to Open Project";
			this.flowLayoutPanel.ResumeLayout(false);
			this.flowLayoutPanel.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelError;
		private System.Windows.Forms.Label labelDeploy;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labelMissing;
	}
}