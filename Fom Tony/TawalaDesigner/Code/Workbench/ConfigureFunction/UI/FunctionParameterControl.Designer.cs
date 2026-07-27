namespace Tawala.Controls
{
	partial class FunctionParameterControl
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
			this.labelParameterName = new System.Windows.Forms.Label();
			this.labelParameterType = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelParameterName
			// 
			this.labelParameterName.Location = new System.Drawing.Point(3, 5);
			this.labelParameterName.Name = "labelParameterName";
			this.labelParameterName.Size = new System.Drawing.Size(125, 13);
			this.labelParameterName.TabIndex = 0;
			this.labelParameterName.Text = "Parameter Name";
			this.labelParameterName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelParameterType
			// 
			this.labelParameterType.Location = new System.Drawing.Point(320, 5);
			this.labelParameterType.Name = "labelParameterType";
			this.labelParameterType.Size = new System.Drawing.Size(125, 13);
			this.labelParameterType.TabIndex = 1;
			this.labelParameterType.Text = "- Parameter Type";
			this.labelParameterType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FunctionParameterControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelParameterType);
			this.Controls.Add(this.labelParameterName);
			this.Name = "FunctionParameterControl";
			this.Size = new System.Drawing.Size(450, 25);
			this.Load += new System.EventHandler(this.FunctionParameterControl_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelParameterName;
		private System.Windows.Forms.Label labelParameterType;
	}
}
