// $Workfile: HeadingView.Designer.cs $
// $Revision: 1 $	$Date: 9/12/07 3:03p $
// Copyright © 2005 Tawala Systems. All rights reserved.

namespace Tawala.Forms
{
	partial class HeadingView
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
			this.itemTextEditor = new Tawala.Forms.ItemTextEditor();
			this.SuspendLayout();
			// 
			// richTextBox
			// 
			this.itemTextEditor.AllowDrop = true;
			this.itemTextEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.itemTextEditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.itemTextEditor.Location = new System.Drawing.Point(80, 0);
			this.itemTextEditor.Name = "itemTextEditor";
			this.itemTextEditor.Size = new System.Drawing.Size(256, 128);
			this.itemTextEditor.TabIndex = 0;
			this.itemTextEditor.Text = "Text";
			// 
			// TextItem
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.itemTextEditor);
			this.Name = "TextItem";
			this.ResumeLayout(false);

		}

		#endregion

		private ItemTextEditor itemTextEditor;

	}
}

