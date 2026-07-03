namespace Program
{
	partial class ApplicationForm
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
			this.labelTreeView = new System.Windows.Forms.Label();
			this.labelOutputXml = new System.Windows.Forms.Label();
			this.textBoxOutputXml = new System.Windows.Forms.TextBox();
			this.fieldsPalette = new Tawala.ProjectUI.FieldsPalette();
			this.buttonReconfigureLatest = new System.Windows.Forms.Button();
			this.buttonSelectFunction = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelTreeView
			// 
			this.labelTreeView.AutoSize = true;
			this.labelTreeView.Location = new System.Drawing.Point(13, 13);
			this.labelTreeView.Name = "labelTreeView";
			this.labelTreeView.Size = new System.Drawing.Size(155, 13);
			this.labelTreeView.TabIndex = 1;
			this.labelTreeView.Text = "Drag items from here into dialog";
			// 
			// labelOutputXml
			// 
			this.labelOutputXml.AutoSize = true;
			this.labelOutputXml.Location = new System.Drawing.Point(13, 228);
			this.labelOutputXml.Name = "labelOutputXml";
			this.labelOutputXml.Size = new System.Drawing.Size(67, 13);
			this.labelOutputXml.TabIndex = 3;
			this.labelOutputXml.Text = "Output XML:";
			// 
			// textBoxOutputXml
			// 
			this.textBoxOutputXml.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.textBoxOutputXml.Location = new System.Drawing.Point(0, 252);
			this.textBoxOutputXml.Multiline = true;
			this.textBoxOutputXml.Name = "textBoxOutputXml";
			this.textBoxOutputXml.Size = new System.Drawing.Size(435, 110);
			this.textBoxOutputXml.TabIndex = 4;
			// 
			// fieldsPalette
			// 
			this.fieldsPalette.BackColor = System.Drawing.SystemColors.Control;
			this.fieldsPalette.Location = new System.Drawing.Point(12, 34);
			this.fieldsPalette.Name = "fieldsPalette";
			this.fieldsPalette.Size = new System.Drawing.Size(166, 150);
			this.fieldsPalette.TabIndex = 7;
			// 
			// buttonReconfigureLatest
			// 
			this.buttonReconfigureLatest.Location = new System.Drawing.Point(238, 79);
			this.buttonReconfigureLatest.Name = "buttonReconfigureLatest";
			this.buttonReconfigureLatest.Size = new System.Drawing.Size(165, 23);
			this.buttonReconfigureLatest.TabIndex = 8;
			this.buttonReconfigureLatest.Text = "Reconfigure Latest...";
			this.buttonReconfigureLatest.UseVisualStyleBackColor = true;
			this.buttonReconfigureLatest.Click += new System.EventHandler(this.buttonReconfigureLatest_Click);
			// 
			// buttonSelectFunction
			// 
			this.buttonSelectFunction.Location = new System.Drawing.Point(238, 34);
			this.buttonSelectFunction.Name = "buttonSelectFunction";
			this.buttonSelectFunction.Size = new System.Drawing.Size(165, 23);
			this.buttonSelectFunction.TabIndex = 9;
			this.buttonSelectFunction.Text = "Select Function...";
			this.buttonSelectFunction.UseVisualStyleBackColor = true;
			this.buttonSelectFunction.Click += new System.EventHandler(this.buttonSelectFunction_Click);
			// 
			// ApplicationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(435, 362);
			this.Controls.Add(this.buttonSelectFunction);
			this.Controls.Add(this.buttonReconfigureLatest);
			this.Controls.Add(this.fieldsPalette);
			this.Controls.Add(this.textBoxOutputXml);
			this.Controls.Add(this.labelOutputXml);
			this.Controls.Add(this.labelTreeView);
			this.Name = "ApplicationForm";
			this.Text = "Function Configuration Demo";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelTreeView;
		private System.Windows.Forms.Label labelOutputXml;
		private System.Windows.Forms.TextBox textBoxOutputXml;
		private Tawala.ProjectUI.FieldsPalette fieldsPalette;
		private System.Windows.Forms.Button buttonReconfigureLatest;
		private System.Windows.Forms.Button buttonSelectFunction;

	}
}