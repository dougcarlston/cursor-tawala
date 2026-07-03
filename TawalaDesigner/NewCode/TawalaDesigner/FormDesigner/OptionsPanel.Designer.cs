namespace Tawala.FormDesigner
{
	partial class OptionsPanel
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.toolWindowCaption1 = new Tawala.FormDesigner.ToolWindowCaption();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(116, 91);
			this.panel1.TabIndex = 1;
			// 
			// toolWindowCaption1
			// 
			this.toolWindowCaption1.AutoSize = true;
			this.toolWindowCaption1.Dock = System.Windows.Forms.DockStyle.Top;
			this.toolWindowCaption1.Font = new System.Drawing.Font("Arial", 8.25F);
			this.toolWindowCaption1.Location = new System.Drawing.Point(0, 0);
			this.toolWindowCaption1.Margin = new System.Windows.Forms.Padding(0);
			this.toolWindowCaption1.MinimumSize = new System.Drawing.Size(116, 21);
			this.toolWindowCaption1.Name = "toolWindowCaption1";
			this.toolWindowCaption1.Size = new System.Drawing.Size(116, 21);
			this.toolWindowCaption1.TabIndex = 0;
			this.toolWindowCaption1.Text = "toolWindowCaption1";
			this.toolWindowCaption1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// OptionsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.toolWindowCaption1);
			this.Controls.Add(this.panel1);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "OptionsPanel";
			this.Size = new System.Drawing.Size(116, 91);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private ToolWindowCaption toolWindowCaption1;
	}
}
