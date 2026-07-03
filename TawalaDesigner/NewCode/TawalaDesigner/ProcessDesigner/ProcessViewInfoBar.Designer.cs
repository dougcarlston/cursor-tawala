namespace Tawala.ProcessDesigner
{
	partial class ProcessViewInfoBar
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
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.labelMessage = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Name = "contextMenuStrip1";
			this.contextMenuStrip.ShowCheckMargin = true;
			this.contextMenuStrip.ShowImageMargin = false;
			this.contextMenuStrip.Size = new System.Drawing.Size(153, 26);
			// 
			// labelMessage
			// 
			this.labelMessage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelMessage.Location = new System.Drawing.Point(0, 0);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(348, 22);
			this.labelMessage.TabIndex = 1;
			this.labelMessage.Text = "<message goes here>";
			this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelMessage.Click += new System.EventHandler(this.labelMessage_Click);
			// 
			// ProcessViewInfoBar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(216)))));
			this.Controls.Add(this.labelMessage);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "ProcessViewInfoBar";
			this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
			this.Size = new System.Drawing.Size(348, 24);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.Label labelMessage;
	}
}
