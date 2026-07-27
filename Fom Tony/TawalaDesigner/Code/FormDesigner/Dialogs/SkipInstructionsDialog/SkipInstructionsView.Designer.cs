namespace Tawala.FormDesigner.Dialogs.SkipInstructionsDialog
{
	partial class SkipInstructionsView
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
			this.processEditor = new Tawala.Processes.ProcessEditor();
			this.buttonClose = new System.Windows.Forms.Button();
			this.skipInstructionsStatementSelector = new Tawala.FormDesigner.Dialogs.SkipInstructionsDialog.SkipInstructionsStatementSelector();
			this.SuspendLayout();
			// 
			// processEditor
			// 
			this.processEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.processEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.processEditor.Location = new System.Drawing.Point(119, 12);
			this.processEditor.Margin = new System.Windows.Forms.Padding(0);
			this.processEditor.Name = "processEditor";
			this.processEditor.Process = null;
			this.processEditor.Size = new System.Drawing.Size(575, 378);
			this.processEditor.TabIndex = 0;
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonClose.Location = new System.Drawing.Point(314, 404);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 2;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// skipInstructionsStatementSelector
			// 
			this.skipInstructionsStatementSelector.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.skipInstructionsStatementSelector.Location = new System.Drawing.Point(13, 12);
			this.skipInstructionsStatementSelector.Name = "skipInstructionsStatementSelector";
			this.skipInstructionsStatementSelector.ProcessEditor = null;
			this.skipInstructionsStatementSelector.Size = new System.Drawing.Size(103, 378);
			this.skipInstructionsStatementSelector.TabIndex = 1;
			// 
			// SkipInstructionsView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(703, 438);
			this.Controls.Add(this.skipInstructionsStatementSelector);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.processEditor);
			this.MinimizeBox = false;
			this.Name = "SkipInstructionsView";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Edit Skip Instructions";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SkipInstructionsView_FormClosed);
			this.ResumeLayout(false);

		}

		#endregion

		private Tawala.Processes.ProcessEditor processEditor;
		private SkipInstructionsStatementSelector skipInstructionsStatementSelector;
		private System.Windows.Forms.Button buttonClose;
	}
}