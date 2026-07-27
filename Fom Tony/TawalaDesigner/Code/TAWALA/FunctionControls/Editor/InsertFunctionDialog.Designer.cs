namespace Tawala.Functions.Controls
{
	partial class InsertFunctionDialog
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
			this.labelSelectCategory = new System.Windows.Forms.Label();
			this.comboBoxCategory = new System.Windows.Forms.ComboBox();
			this.labelSelectFunction = new System.Windows.Forms.Label();
			this.listBoxFunctions = new System.Windows.Forms.ListBox();
			this.labelSelectedFunctionName = new System.Windows.Forms.Label();
			this.labelSelectedFunctionDescription = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelSelectCategory
			// 
			this.labelSelectCategory.AutoSize = true;
			this.labelSelectCategory.Location = new System.Drawing.Point(12, 23);
			this.labelSelectCategory.Name = "labelSelectCategory";
			this.labelSelectCategory.Size = new System.Drawing.Size(93, 13);
			this.labelSelectCategory.TabIndex = 0;
			this.labelSelectCategory.Text = "Select a &category:";
			this.labelSelectCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboBoxCategory
			// 
			this.comboBoxCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCategory.FormattingEnabled = true;
			this.comboBoxCategory.Location = new System.Drawing.Point(106, 21);
			this.comboBoxCategory.Name = "comboBoxCategory";
			this.comboBoxCategory.Size = new System.Drawing.Size(238, 21);
			this.comboBoxCategory.TabIndex = 1;
			this.comboBoxCategory.SelectedValueChanged += new System.EventHandler(this.comboBoxCategory_SelectedValueChanged);
			// 
			// labelSelectFunction
			// 
			this.labelSelectFunction.AutoSize = true;
			this.labelSelectFunction.Location = new System.Drawing.Point(12, 63);
			this.labelSelectFunction.Name = "labelSelectFunction";
			this.labelSelectFunction.Size = new System.Drawing.Size(90, 13);
			this.labelSelectFunction.TabIndex = 2;
			this.labelSelectFunction.Text = "Select a &function:";
			this.labelSelectFunction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBoxFunctions
			// 
			this.listBoxFunctions.FormattingEnabled = true;
			this.listBoxFunctions.Location = new System.Drawing.Point(15, 83);
			this.listBoxFunctions.Name = "listBoxFunctions";
			this.listBoxFunctions.Size = new System.Drawing.Size(333, 95);
			this.listBoxFunctions.TabIndex = 3;
			this.listBoxFunctions.DoubleClick += new System.EventHandler(this.listBoxFunctions_DoubleClick);
			this.listBoxFunctions.SelectedValueChanged += new System.EventHandler(this.listBoxFunctions_SelectedValueChanged);
			// 
			// labelSelectedFunctionName
			// 
			this.labelSelectedFunctionName.AutoSize = true;
			this.labelSelectedFunctionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSelectedFunctionName.Location = new System.Drawing.Point(12, 184);
			this.labelSelectedFunctionName.Name = "labelSelectedFunctionName";
			this.labelSelectedFunctionName.Size = new System.Drawing.Size(177, 16);
			this.labelSelectedFunctionName.TabIndex = 4;
			this.labelSelectedFunctionName.Text = "Selected Function Name";
			// 
			// labelSelectedFunctionDescription
			// 
			this.labelSelectedFunctionDescription.Location = new System.Drawing.Point(12, 205);
			this.labelSelectedFunctionDescription.Name = "labelSelectedFunctionDescription";
			this.labelSelectedFunctionDescription.Size = new System.Drawing.Size(333, 83);
			this.labelSelectedFunctionDescription.TabIndex = 5;
			this.labelSelectedFunctionDescription.Text = "Selected Function Description";
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(101, 300);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 25);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(194, 300);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 25);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// InsertFunctionDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(366, 336);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.labelSelectedFunctionDescription);
			this.Controls.Add(this.labelSelectedFunctionName);
			this.Controls.Add(this.listBoxFunctions);
			this.Controls.Add(this.labelSelectFunction);
			this.Controls.Add(this.comboBoxCategory);
			this.Controls.Add(this.labelSelectCategory);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InsertFunctionDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Insert Function";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelSelectCategory;
		private System.Windows.Forms.ComboBox comboBoxCategory;
		private System.Windows.Forms.Label labelSelectFunction;
		private System.Windows.Forms.ListBox listBoxFunctions;
		private System.Windows.Forms.Label labelSelectedFunctionName;
		private System.Windows.Forms.Label labelSelectedFunctionDescription;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
	}
}