namespace TawalaDesigner.Dialogs
{
	partial class DeployingProjectView
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
			this.labelMessage = new System.Windows.Forms.Label();
			this.backgroundWorkerDeploy = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// labelMessage
			// 
			this.labelMessage.AutoSize = true;
			this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMessage.Location = new System.Drawing.Point(90, 61);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(230, 16);
			this.labelMessage.TabIndex = 0;
			this.labelMessage.Text = "Deploying project, please wait...";
			// 
			// backgroundWorkerDeploy
			// 
			this.backgroundWorkerDeploy.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerDeploy_DoWork);
			this.backgroundWorkerDeploy.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerDeploy_RunWorkerCompleted);
			// 
			// DeployingProjectView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(410, 138);
			this.ControlBox = false;
			this.Controls.Add(this.labelMessage);
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DeployingProjectView";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Deploying Project";
			this.Load += new System.EventHandler(this.DeployingProjectView_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelMessage;
		private System.ComponentModel.BackgroundWorker backgroundWorkerDeploy;
	}
}