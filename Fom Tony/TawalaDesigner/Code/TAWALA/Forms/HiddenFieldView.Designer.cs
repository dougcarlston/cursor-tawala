// $Workfile: HiddenFieldView.Designer.cs $
// $Revision: 6 $	$Date: 6/14/07 11:18a $
// Copyright © 2005 Tawala Systems. All rights reserved.

namespace Tawala.Forms
{
	partial class HiddenFieldView
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
			this.textBox = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.BackColor = System.Drawing.SystemColors.Window;
			this.textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox.Location = new System.Drawing.Point(131, 2);
			this.textBox.Margin = new System.Windows.Forms.Padding(0);
			this.textBox.MaxLength = 50;
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(177, 22);
			this.textBox.TabIndex = 0;
			this.textBox.Enter += new System.EventHandler(this.textBox_Enter);
			this.textBox.Leave += new System.EventHandler(this.textBox_Leave);
			this.textBox.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
			// 
			// labelName
			// 
			this.labelName.AutoSize = true;
			this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelName.Location = new System.Drawing.Point(81, 6);
			this.labelName.Name = "labelName";
			this.labelName.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelName.Size = new System.Drawing.Size(52, 15);
			this.labelName.TabIndex = 1;
			this.labelName.Text = "Name:";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// HiddenFieldView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.textBox);
			this.Name = "HiddenFieldView";
			this.Size = new System.Drawing.Size(314, 28);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.ErrorProvider errorProvider;


	}
}

