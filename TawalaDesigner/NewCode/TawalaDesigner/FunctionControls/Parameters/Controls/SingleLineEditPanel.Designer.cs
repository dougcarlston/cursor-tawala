namespace Tawala.Functions.Controls
{
	partial class SingleLineEditPanel
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
			this.labelName = new System.Windows.Forms.Label();
			this.labelTag = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelName
			// 
			this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelName.Location = new System.Drawing.Point(0, 0);
			this.labelName.Margin = new System.Windows.Forms.Padding(0);
			this.labelName.MinimumSize = new System.Drawing.Size(100, 26);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(100, 26);
			this.labelName.TabIndex = 1;
			this.labelName.Text = "Parameter Label";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTag
			// 
			this.labelTag.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.labelTag.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTag.Location = new System.Drawing.Point(200, 0);
			this.labelTag.Margin = new System.Windows.Forms.Padding(0);
			this.labelTag.MinimumSize = new System.Drawing.Size(100, 26);
			this.labelTag.Name = "labelTag";
			this.labelTag.Size = new System.Drawing.Size(100, 26);
			this.labelTag.TabIndex = 3;
			this.labelTag.Text = "- Tag Line";
			this.labelTag.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelTag.UseMnemonic = false;
			// 
			// SingleLineEditPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.labelTag);
			this.Controls.Add(this.labelName);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.MaximumSize = new System.Drawing.Size(0, 26);
			this.MinimumSize = new System.Drawing.Size(300, 26);
			this.Name = "SingleLineEditPanel";
			this.Size = new System.Drawing.Size(300, 26);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Label labelTag;
	}
}
