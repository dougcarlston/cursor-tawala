namespace Tawala.Controls
{
	partial class ConditionsBuilder
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
			this.radioButtonAnd = new System.Windows.Forms.RadioButton();
			this.radioButtonOr = new System.Windows.Forms.RadioButton();
			this.panelGroups = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// radioButtonAnd
			// 
			this.radioButtonAnd.AutoSize = true;
			this.radioButtonAnd.Checked = true;
			this.radioButtonAnd.Location = new System.Drawing.Point(6, 46);
			this.radioButtonAnd.Name = "radioButtonAnd";
			this.radioButtonAnd.Size = new System.Drawing.Size(48, 17);
			this.radioButtonAnd.TabIndex = 2;
			this.radioButtonAnd.TabStop = true;
			this.radioButtonAnd.Text = "AND";
			this.radioButtonAnd.UseVisualStyleBackColor = true;
			this.radioButtonAnd.Visible = false;
			this.radioButtonAnd.CheckedChanged += new System.EventHandler(this.radioButtonAnd_CheckedChanged);
			// 
			// radioButtonOr
			// 
			this.radioButtonOr.AutoSize = true;
			this.radioButtonOr.Location = new System.Drawing.Point(59, 46);
			this.radioButtonOr.Name = "radioButtonOr";
			this.radioButtonOr.Size = new System.Drawing.Size(41, 17);
			this.radioButtonOr.TabIndex = 3;
			this.radioButtonOr.Text = "OR";
			this.radioButtonOr.UseVisualStyleBackColor = true;
			this.radioButtonOr.Visible = false;
			this.radioButtonOr.CheckedChanged += new System.EventHandler(this.radioButtonOr_CheckedChanged);
			// 
			// panelGroups
			// 
			this.panelGroups.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelGroups.Location = new System.Drawing.Point(0, 0);
			this.panelGroups.Name = "panelGroups";
			this.panelGroups.Size = new System.Drawing.Size(606, 90);
			this.panelGroups.TabIndex = 4;
			// 
			// ConditionsBuilder
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelGroups);
			this.Controls.Add(this.radioButtonOr);
			this.Controls.Add(this.radioButtonAnd);
			this.Name = "ConditionsBuilder";
			this.Size = new System.Drawing.Size(606, 90);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButtonAnd;
		private System.Windows.Forms.RadioButton radioButtonOr;
		private System.Windows.Forms.Panel panelGroups;

	}
}
