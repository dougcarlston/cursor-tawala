// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
namespace TawalaDesigner.Dialogs
{
	partial class HelpAboutView
    {
		private System.Windows.Forms.Label labelBuild;
		private System.Windows.Forms.Label labelCopyRight;
		private System.Windows.Forms.ListBox listBoxVersions;
		private System.Windows.Forms.Label labelNetVer;
		private System.Windows.Forms.Label labelFileVers;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpAboutView));
            this.labelBuild = new System.Windows.Forms.Label();
            this.labelCopyRight = new System.Windows.Forms.Label();
            this.listBoxVersions = new System.Windows.Forms.ListBox();
            this.labelNetVer = new System.Windows.Forms.Label();
            this.labelFileVers = new System.Windows.Forms.Label();
            this.labelMemory = new System.Windows.Forms.Label();
            this.labelOS = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelBuild
            // 
            this.labelBuild.BackColor = System.Drawing.Color.Transparent;
            this.labelBuild.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBuild.Location = new System.Drawing.Point(453, 12);
            this.labelBuild.Name = "labelBuild";
            this.labelBuild.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelBuild.Size = new System.Drawing.Size(118, 52);
            this.labelBuild.TabIndex = 1;
            this.labelBuild.Text = "5";
            this.labelBuild.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelCopyRight
            // 
            this.labelCopyRight.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelCopyRight.AutoSize = true;
            this.labelCopyRight.BackColor = System.Drawing.Color.Transparent;
            this.labelCopyRight.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCopyRight.Location = new System.Drawing.Point(47, 418);
            this.labelCopyRight.Margin = new System.Windows.Forms.Padding(0);
            this.labelCopyRight.Name = "labelCopyRight";
            this.labelCopyRight.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelCopyRight.Size = new System.Drawing.Size(499, 23);
            this.labelCopyRight.TabIndex = 6;
            this.labelCopyRight.Text = "Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.";
            this.labelCopyRight.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // listBoxVersions
            // 
            this.listBoxVersions.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.listBoxVersions.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxVersions.FormattingEnabled = true;
            this.listBoxVersions.ItemHeight = 15;
            this.listBoxVersions.Location = new System.Drawing.Point(8, 84);
            this.listBoxVersions.Margin = new System.Windows.Forms.Padding(0);
            this.listBoxVersions.MinimumSize = new System.Drawing.Size(576, 334);
            this.listBoxVersions.Name = "listBoxVersions";
            this.listBoxVersions.Size = new System.Drawing.Size(577, 334);
            this.listBoxVersions.TabIndex = 5;
            // 
            // labelNetVer
            // 
            this.labelNetVer.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelNetVer.AutoSize = true;
            this.labelNetVer.BackColor = System.Drawing.Color.Transparent;
            this.labelNetVer.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNetVer.Location = new System.Drawing.Point(8, 8);
            this.labelNetVer.Margin = new System.Windows.Forms.Padding(0);
            this.labelNetVer.Name = "labelNetVer";
            this.labelNetVer.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.labelNetVer.Size = new System.Drawing.Size(199, 17);
            this.labelNetVer.TabIndex = 1;
            this.labelNetVer.Text = "Microsoft .NET Framework Version ";
            this.labelNetVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFileVers
            // 
            this.labelFileVers.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelFileVers.AutoSize = true;
            this.labelFileVers.BackColor = System.Drawing.Color.Transparent;
            this.labelFileVers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFileVers.Location = new System.Drawing.Point(8, 59);
            this.labelFileVers.Margin = new System.Windows.Forms.Padding(0);
            this.labelFileVers.Name = "labelFileVers";
            this.labelFileVers.Padding = new System.Windows.Forms.Padding(0, 5, 0, 4);
            this.labelFileVers.Size = new System.Drawing.Size(88, 25);
            this.labelFileVers.TabIndex = 4;
            this.labelFileVers.Text = "File Versions:";
            this.labelFileVers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelMemory
            // 
            this.labelMemory.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelMemory.AutoSize = true;
            this.labelMemory.BackColor = System.Drawing.Color.Transparent;
            this.labelMemory.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMemory.Location = new System.Drawing.Point(8, 42);
            this.labelMemory.Margin = new System.Windows.Forms.Padding(0);
            this.labelMemory.Name = "labelMemory";
            this.labelMemory.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.labelMemory.Size = new System.Drawing.Size(136, 17);
            this.labelMemory.TabIndex = 3;
            this.labelMemory.Text = "Free Physical Memory:  ";
            this.labelMemory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelOS
            // 
            this.labelOS.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelOS.AutoSize = true;
            this.labelOS.BackColor = System.Drawing.Color.Transparent;
            this.labelOS.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOS.Location = new System.Drawing.Point(8, 25);
            this.labelOS.Margin = new System.Windows.Forms.Padding(0);
            this.labelOS.Name = "labelOS";
            this.labelOS.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.labelOS.Size = new System.Drawing.Size(35, 17);
            this.labelOS.TabIndex = 2;
            this.labelOS.Text = "<os>";
            this.labelOS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.labelNetVer);
            this.flowLayoutPanel1.Controls.Add(this.labelOS);
            this.flowLayoutPanel1.Controls.Add(this.labelMemory);
            this.flowLayoutPanel1.Controls.Add(this.labelFileVers);
            this.flowLayoutPanel1.Controls.Add(this.listBoxVersions);
            this.flowLayoutPanel1.Controls.Add(this.labelCopyRight);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 82);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(593, 441);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // HelpAboutView
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(594, 531);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.labelBuild);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(320, 240);
            this.Name = "HelpAboutView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Tawala Project Designer";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelMemory;
		private System.Windows.Forms.Label labelOS;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
	}
}
