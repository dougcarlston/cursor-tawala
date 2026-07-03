// $Workfile: About.Designer.cs $
// $Revision: 31 $	$Date: 6/26/07 1:23p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
namespace Tawala.DesignerUI
{
	partial class About
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
			this.labelBuild = new System.Windows.Forms.Label();
			this.labelCopyRight = new System.Windows.Forms.Label();
			this.listBoxVersions = new System.Windows.Forms.ListBox();
			this.labelNetVer = new System.Windows.Forms.Label();
			this.labelFileVers = new System.Windows.Forms.Label();
			this.labelMemory = new System.Windows.Forms.Label();
			this.labelOS = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelBuild
			// 
			this.labelBuild.BackColor = System.Drawing.Color.Transparent;
			this.labelBuild.Font = new System.Drawing.Font("Arial", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelBuild.Location = new System.Drawing.Point(365, 9);
			this.labelBuild.Name = "labelBuild";
			this.labelBuild.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelBuild.Size = new System.Drawing.Size(118, 52);
			this.labelBuild.TabIndex = 1;
			this.labelBuild.Text = "5";
			this.labelBuild.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCopyRight
			// 
			this.labelCopyRight.AutoSize = true;
			this.labelCopyRight.BackColor = System.Drawing.Color.Transparent;
			this.labelCopyRight.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCopyRight.Location = new System.Drawing.Point(14, 88);
			this.labelCopyRight.Margin = new System.Windows.Forms.Padding(0);
			this.labelCopyRight.Name = "labelCopyRight";
			this.labelCopyRight.Size = new System.Drawing.Size(389, 16);
			this.labelCopyRight.TabIndex = 2;
			this.labelCopyRight.Text = "Copyright © 2005 - 2009 Tawala Systems, Inc. All rights reserved.";
			this.labelCopyRight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBoxVersions
			// 
			this.listBoxVersions.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBoxVersions.FormattingEnabled = true;
			this.listBoxVersions.ItemHeight = 15;
			this.listBoxVersions.Location = new System.Drawing.Point(14, 200);
			this.listBoxVersions.Name = "listBoxVersions";
			this.listBoxVersions.Size = new System.Drawing.Size(505, 199);
			this.listBoxVersions.TabIndex = 3;
			// 
			// labelNetVer
			// 
			this.labelNetVer.BackColor = System.Drawing.Color.Transparent;
			this.labelNetVer.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelNetVer.Location = new System.Drawing.Point(14, 110);
			this.labelNetVer.Margin = new System.Windows.Forms.Padding(0);
			this.labelNetVer.Name = "labelNetVer";
			this.labelNetVer.Size = new System.Drawing.Size(505, 16);
			this.labelNetVer.TabIndex = 4;
			this.labelNetVer.Text = "Microsoft .NET Framework Version ";
			this.labelNetVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelFileVers
			// 
			this.labelFileVers.AutoSize = true;
			this.labelFileVers.BackColor = System.Drawing.Color.Transparent;
			this.labelFileVers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFileVers.Location = new System.Drawing.Point(14, 176);
			this.labelFileVers.Margin = new System.Windows.Forms.Padding(0);
			this.labelFileVers.Name = "labelFileVers";
			this.labelFileVers.Size = new System.Drawing.Size(88, 16);
			this.labelFileVers.TabIndex = 5;
			this.labelFileVers.Text = "File Versions:";
			this.labelFileVers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelMemory
			// 
			this.labelMemory.BackColor = System.Drawing.Color.Transparent;
			this.labelMemory.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMemory.Location = new System.Drawing.Point(14, 154);
			this.labelMemory.Margin = new System.Windows.Forms.Padding(0);
			this.labelMemory.Name = "labelMemory";
			this.labelMemory.Size = new System.Drawing.Size(505, 16);
			this.labelMemory.TabIndex = 7;
			this.labelMemory.Text = "Free Physical Memory:  ";
			this.labelMemory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelOS
			// 
			this.labelOS.BackColor = System.Drawing.Color.Transparent;
			this.labelOS.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelOS.Location = new System.Drawing.Point(14, 132);
			this.labelOS.Margin = new System.Windows.Forms.Padding(0);
			this.labelOS.Name = "labelOS";
			this.labelOS.Size = new System.Drawing.Size(505, 16);
			this.labelOS.TabIndex = 6;
			this.labelOS.Text = "<os>";
			this.labelOS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// About
			// 
			this.BackColor = System.Drawing.Color.Azure;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ClientSize = new System.Drawing.Size(531, 405);
			this.Controls.Add(this.labelMemory);
			this.Controls.Add(this.labelOS);
			this.Controls.Add(this.labelFileVers);
			this.Controls.Add(this.labelNetVer);
			this.Controls.Add(this.listBoxVersions);
			this.Controls.Add(this.labelCopyRight);
			this.Controls.Add(this.labelBuild);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(320, 240);
			this.Name = "About";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About Tawala Project Designer";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelMemory;
		private System.Windows.Forms.Label labelOS;
	}
}
