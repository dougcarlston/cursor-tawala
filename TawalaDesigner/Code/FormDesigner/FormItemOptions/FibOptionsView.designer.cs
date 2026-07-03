namespace Tawala.FormDesigner.FormItemOptions
{
	partial class FibOptionsView
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
			this.checkBoxResponseRequired = new System.Windows.Forms.CheckBox();
			this.textBoxQuestionLabel = new System.Windows.Forms.TextBox();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.labelQuestion = new System.Windows.Forms.Label();
			this.labelBlank = new System.Windows.Forms.Label();
			this.textBoxBlankLabel = new System.Windows.Forms.TextBox();
			this.labelStatus = new System.Windows.Forms.Label();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkBoxResponseRequired
			// 
			this.checkBoxResponseRequired.AutoSize = true;
			this.flowLayoutPanel1.SetFlowBreak(this.checkBoxResponseRequired, true);
			this.checkBoxResponseRequired.Location = new System.Drawing.Point(7, 97);
			this.checkBoxResponseRequired.Name = "checkBoxResponseRequired";
			this.checkBoxResponseRequired.Size = new System.Drawing.Size(115, 17);
			this.checkBoxResponseRequired.TabIndex = 4;
			this.checkBoxResponseRequired.Text = "Response required";
			this.checkBoxResponseRequired.UseVisualStyleBackColor = true;
			// 
			// textBoxQuestionLabel
			// 
			this.flowLayoutPanel1.SetFlowBreak(this.textBoxQuestionLabel, true);
			this.textBoxQuestionLabel.Location = new System.Drawing.Point(7, 24);
			this.textBoxQuestionLabel.MaxLength = 64;
			this.textBoxQuestionLabel.Name = "textBoxQuestionLabel";
			this.textBoxQuestionLabel.Size = new System.Drawing.Size(132, 20);
			this.textBoxQuestionLabel.TabIndex = 1;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.labelQuestion);
			this.flowLayoutPanel1.Controls.Add(this.textBoxQuestionLabel);
			this.flowLayoutPanel1.Controls.Add(this.labelBlank);
			this.flowLayoutPanel1.Controls.Add(this.textBoxBlankLabel);
			this.flowLayoutPanel1.Controls.Add(this.checkBoxResponseRequired);
			this.flowLayoutPanel1.Controls.Add(this.labelStatus);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(4, 4, 0, 0);
			this.flowLayoutPanel1.Size = new System.Drawing.Size(150, 149);
			this.flowLayoutPanel1.TabIndex = 0;
			this.flowLayoutPanel1.WrapContents = false;
			// 
			// labelQuestion
			// 
			this.labelQuestion.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelQuestion.AutoSize = true;
			this.flowLayoutPanel1.SetFlowBreak(this.labelQuestion, true);
			this.labelQuestion.Location = new System.Drawing.Point(7, 4);
			this.labelQuestion.Name = "labelQuestion";
			this.labelQuestion.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this.labelQuestion.Size = new System.Drawing.Size(81, 17);
			this.labelQuestion.TabIndex = 0;
			this.labelQuestion.Text = "Question Label:";
			// 
			// labelBlank
			// 
			this.labelBlank.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelBlank.AutoSize = true;
			this.flowLayoutPanel1.SetFlowBreak(this.labelBlank, true);
			this.labelBlank.Location = new System.Drawing.Point(7, 47);
			this.labelBlank.Name = "labelBlank";
			this.labelBlank.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			this.labelBlank.Size = new System.Drawing.Size(66, 21);
			this.labelBlank.TabIndex = 2;
			this.labelBlank.Text = "Blank Label:";
			// 
			// textBoxBlankLabel
			// 
			this.flowLayoutPanel1.SetFlowBreak(this.textBoxBlankLabel, true);
			this.textBoxBlankLabel.Location = new System.Drawing.Point(7, 71);
			this.textBoxBlankLabel.MaxLength = 64;
			this.textBoxBlankLabel.Name = "textBoxBlankLabel";
			this.textBoxBlankLabel.Size = new System.Drawing.Size(132, 20);
			this.textBoxBlankLabel.TabIndex = 3;
			// 
			// labelStatus
			// 
			this.labelStatus.AutoSize = true;
			this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.labelStatus.Location = new System.Drawing.Point(7, 117);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			this.labelStatus.Size = new System.Drawing.Size(0, 21);
			this.labelStatus.TabIndex = 5;
			// 
			// FibOptionsView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "FibOptionsView";
			this.Size = new System.Drawing.Size(150, 149);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxResponseRequired;
		private System.Windows.Forms.TextBox textBoxQuestionLabel;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label labelQuestion;
		private System.Windows.Forms.Label labelBlank;
		private System.Windows.Forms.TextBox textBoxBlankLabel;
		private System.Windows.Forms.Label labelStatus;
	}
}
