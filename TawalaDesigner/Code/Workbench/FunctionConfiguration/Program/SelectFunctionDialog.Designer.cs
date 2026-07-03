namespace Program
{
	partial class SelectFunctionDialog
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
			this.listBoxCategories = new System.Windows.Forms.ListBox();
			this.labelCategories = new System.Windows.Forms.Label();
			this.labelFunctions = new System.Windows.Forms.Label();
			this.listBoxFunctions = new System.Windows.Forms.ListBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelRepositoryXmlVersion = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// listBoxCategories
			// 
			this.listBoxCategories.FormattingEnabled = true;
			this.listBoxCategories.Location = new System.Drawing.Point(13, 61);
			this.listBoxCategories.Name = "listBoxCategories";
			this.listBoxCategories.Size = new System.Drawing.Size(240, 121);
			this.listBoxCategories.TabIndex = 0;
			this.listBoxCategories.SelectedIndexChanged += new System.EventHandler(this.listBoxCategories_SelectedIndexChanged);
			// 
			// labelCategories
			// 
			this.labelCategories.AutoSize = true;
			this.labelCategories.Location = new System.Drawing.Point(10, 42);
			this.labelCategories.Name = "labelCategories";
			this.labelCategories.Size = new System.Drawing.Size(81, 13);
			this.labelCategories.TabIndex = 1;
			this.labelCategories.Text = "Select category";
			// 
			// labelFunctions
			// 
			this.labelFunctions.AutoSize = true;
			this.labelFunctions.Location = new System.Drawing.Point(263, 42);
			this.labelFunctions.Name = "labelFunctions";
			this.labelFunctions.Size = new System.Drawing.Size(78, 13);
			this.labelFunctions.TabIndex = 3;
			this.labelFunctions.Text = "Select function";
			// 
			// listBoxFunctions
			// 
			this.listBoxFunctions.FormattingEnabled = true;
			this.listBoxFunctions.Location = new System.Drawing.Point(266, 61);
			this.listBoxFunctions.Name = "listBoxFunctions";
			this.listBoxFunctions.Size = new System.Drawing.Size(245, 173);
			this.listBoxFunctions.TabIndex = 2;
			this.listBoxFunctions.SelectedIndexChanged += new System.EventHandler(this.listBoxFunctions_SelectedIndexChanged);
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(179, 256);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "Configure...";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(270, 256);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// labelRepositoryXmlVersion
			// 
			this.labelRepositoryXmlVersion.AutoSize = true;
			this.labelRepositoryXmlVersion.Location = new System.Drawing.Point(16, 13);
			this.labelRepositoryXmlVersion.Name = "labelRepositoryXmlVersion";
			this.labelRepositoryXmlVersion.Size = new System.Drawing.Size(131, 13);
			this.labelRepositoryXmlVersion.TabIndex = 6;
			this.labelRepositoryXmlVersion.Text = "labelRepositoryXmlVersion";
			// 
			// SelectFunctionDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(524, 303);
			this.Controls.Add(this.labelRepositoryXmlVersion);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.labelFunctions);
			this.Controls.Add(this.listBoxFunctions);
			this.Controls.Add(this.labelCategories);
			this.Controls.Add(this.listBoxCategories);
			this.Name = "SelectFunctionDialog";
			this.Text = "Select Function";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBoxCategories;
		private System.Windows.Forms.Label labelCategories;
		private System.Windows.Forms.Label labelFunctions;
		private System.Windows.Forms.ListBox listBoxFunctions;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelRepositoryXmlVersion;
	}
}