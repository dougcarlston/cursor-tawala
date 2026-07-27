// $Workfile: TextEdit.Designer.cs $
// $Revision: 6 $	$Date: 11/01/06 3:36p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
namespace Tawala.TextEditor
{
	partial class TextEdit
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
			TXTextControl.Selection selection1 = new TXTextControl.Selection();
			TXTextControl.ListFormat listFormat1 = new TXTextControl.ListFormat();
			TXTextControl.ParagraphFormat paragraphFormat1 = new TXTextControl.ParagraphFormat();
			this.txTextControl = new Tawala.TextEditor.DerivedTxControl();
			this.SuspendLayout();
			// 
			// txTextControl
			// 
			this.txTextControl.BorderStyle = TXTextControl.BorderStyle.None;
			this.txTextControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txTextControl.Font = new System.Drawing.Font("Arial", 10F);
			this.txTextControl.Location = new System.Drawing.Point(0, 0);
			this.txTextControl.Margin = new System.Windows.Forms.Padding(0);
			this.txTextControl.Name = "txTextControl";
			selection1.FontName = "Arial";
			selection1.FontSize = 200;
			selection1.ForeColor = System.Drawing.SystemColors.WindowText;
			selection1.FormattingStyle = "[Normal]";
			listFormat1.CharAfterNumber = '\0';
			listFormat1.HangingIndent = 0;
			selection1.ListFormat = listFormat1;
			paragraphFormat1.AbsoluteLineSpacing = 228;
			selection1.ParagraphFormat = paragraphFormat1;
			selection1.Text = "";
			selection1.TextBackColor = System.Drawing.SystemColors.Window;
			this.txTextControl.Selection = selection1;
			this.txTextControl.Size = new System.Drawing.Size(150, 150);
			this.txTextControl.TabIndex = 4;
			this.txTextControl.Text = "";
			// 
			// TextEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.txTextControl);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "TextEdit";
			this.ResumeLayout(false);

		}

		#endregion

        [System.ComponentModel.Browsable(false)]
		private Tawala.TextEditor.DerivedTxControl txTextControl;
	}
}
