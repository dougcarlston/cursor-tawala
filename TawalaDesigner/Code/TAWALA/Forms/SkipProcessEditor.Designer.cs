// $Workfile: SkipProcessEditor.Designer.cs $
// $Revision: 5 $	$Date: 12/07/05 9:07a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Forms
{
	partial class SkipProcessEditor 
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
			this.components = new System.ComponentModel.Container();
			this.menuStripMergeInsert = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuStripMergeInsert.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStripMergeInsert
			// 
			this.menuStripMergeInsert.Enabled = true;
			this.menuStripMergeInsert.GripMargin = new System.Windows.Forms.Padding(2);
			this.menuStripMergeInsert.Location = new System.Drawing.Point(102, 203);
			this.menuStripMergeInsert.Name = "menuStripMergeInsert";
			this.menuStripMergeInsert.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.menuStripMergeInsert.Size = new System.Drawing.Size(153, 221);
			this.menuStripMergeInsert.Visible = true;
			// 
			// SkipViewInternal
			// 
			this.Name = "SkipViewInternal";
			this.Size = new System.Drawing.Size(504, 308);
			this.menuStripMergeInsert.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip menuStripMergeInsert;
	}
}
