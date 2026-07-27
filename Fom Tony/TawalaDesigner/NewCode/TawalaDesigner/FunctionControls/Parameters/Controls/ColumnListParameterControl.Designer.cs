namespace Tawala.Functions.Controls
{
	partial class ColumnListParameterControl
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
            this.flowLayoutPanelColumns = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flowLayoutPanelColumns
            // 
            this.flowLayoutPanelColumns.AutoSize = true;
            this.flowLayoutPanelColumns.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelColumns.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelColumns.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanelColumns.MinimumSize = new System.Drawing.Size(300, 79);
            this.flowLayoutPanelColumns.Name = "flowLayoutPanelColumns";
            this.flowLayoutPanelColumns.Size = new System.Drawing.Size(300, 90);
            this.flowLayoutPanelColumns.TabIndex = 2;
            // 
            // ColumnListParameterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.flowLayoutPanelColumns);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(208, 90);
            this.Name = "ColumnListParameterControl";
            this.Size = new System.Drawing.Size(300, 90);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelColumns;

	}
}
