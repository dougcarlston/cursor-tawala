namespace Tawala.Documents
{
	partial class DocumentPalette
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		public System.Windows.Forms.TableLayoutPanel commandTable;
		public System.Windows.Forms.Panel commandPanel;
		private Tawala.Common.GradientLabel label;

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
            this.commandPanel = new System.Windows.Forms.Panel();
            this.commandTable = new System.Windows.Forms.TableLayoutPanel();
            this.label = new Tawala.Common.GradientLabel();
            this.commandPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // commandPanel
            // 
            this.commandPanel.Controls.Add(this.commandTable);
            this.commandPanel.Controls.Add(this.label);
            this.commandPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commandPanel.Location = new System.Drawing.Point(0, 0);
            this.commandPanel.Margin = new System.Windows.Forms.Padding(0);
            this.commandPanel.Name = "commandPanel";
            this.commandPanel.Size = new System.Drawing.Size(92, 788);
            this.commandPanel.TabIndex = 0;
            // 
            // commandTable
            // 
            this.commandTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(227)))), ((int)(((byte)(242)))));
            this.commandTable.ColumnCount = 1;
            this.commandTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.commandTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commandTable.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.commandTable.Location = new System.Drawing.Point(0, 20);
            this.commandTable.Margin = new System.Windows.Forms.Padding(0);
            this.commandTable.Name = "commandTable";
            this.commandTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 794F));
            this.commandTable.Size = new System.Drawing.Size(92, 768);
            this.commandTable.TabIndex = 2;
            // 
            // label
            // 
            this.label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(206)))), ((int)(((byte)(248)))));
            this.label.Dock = System.Windows.Forms.DockStyle.Top;
            this.label.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.ForeColor = System.Drawing.Color.White;
            this.label.Location = new System.Drawing.Point(0, 0);
            this.label.Margin = new System.Windows.Forms.Padding(0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(92, 20);
            this.label.TabIndex = 1;
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DocumentPalette
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.commandPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(92, 0);
            this.Name = "DocumentPalette";
            this.Size = new System.Drawing.Size(92, 788);
            this.commandPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
	#endregion
	}
}
