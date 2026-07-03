// $Workfile: DeleteStatementView.Designer.cs $
// $Revision: 4 $	$Date: 5/03/07 6:56p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Processes
{
	partial class DeleteStatementView
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
			this.labelFrom = new System.Windows.Forms.Label();
			this.buttonAddModify = new Tawala.Processes.AddModifyButton();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageDelete = new System.Windows.Forms.TabPage();
			this.labelWhere2 = new System.Windows.Forms.Label();
			this.comboBoxAndOr = new System.Windows.Forms.ComboBox();
			this.labelWhere = new System.Windows.Forms.Label();
			this.groupBoxWhere = new System.Windows.Forms.GroupBox();
			this.comboBoxForms = new System.Windows.Forms.ComboBox();
			this.tabControl.SuspendLayout();
			this.tabPageDelete.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelFrom
			// 
			this.labelFrom.Location = new System.Drawing.Point(20, 16);
			this.labelFrom.Name = "labelFrom";
			this.labelFrom.Size = new System.Drawing.Size(99, 23);
			this.labelFrom.TabIndex = 1;
			this.labelFrom.Text = "records from form:";
			this.labelFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonAddModify
			// 
			this.buttonAddModify.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonAddModify.Location = new System.Drawing.Point(258, 125);
			this.buttonAddModify.Name = "buttonAddModify";
			this.buttonAddModify.Size = new System.Drawing.Size(80, 24);
			this.buttonAddModify.TabIndex = 7;
			this.buttonAddModify.Text = "Add";
			this.buttonAddModify.Click += new System.EventHandler(this.buttonAddModify_Click);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageDelete);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(604, 186);
			this.tabControl.TabIndex = 0;
			this.tabControl.TabStop = false;
			// 
			// tabPageDelete
			// 
			this.tabPageDelete.Controls.Add(this.labelWhere2);
			this.tabPageDelete.Controls.Add(this.comboBoxAndOr);
			this.tabPageDelete.Controls.Add(this.labelWhere);
			this.tabPageDelete.Controls.Add(this.groupBoxWhere);
			this.tabPageDelete.Controls.Add(this.comboBoxForms);
			this.tabPageDelete.Controls.Add(this.labelFrom);
			this.tabPageDelete.Controls.Add(this.buttonAddModify);
			this.tabPageDelete.Location = new System.Drawing.Point(4, 22);
			this.tabPageDelete.Name = "tabPageDelete";
			this.tabPageDelete.Size = new System.Drawing.Size(596, 160);
			this.tabPageDelete.TabIndex = 0;
			this.tabPageDelete.Text = "Delete";
			this.tabPageDelete.UseVisualStyleBackColor = true;
			this.tabPageDelete.Click += new System.EventHandler(this.tabPageGet_Click);
			// 
			// labelWhere2
			// 
			this.labelWhere2.Location = new System.Drawing.Point(135, 39);
			this.labelWhere2.Margin = new System.Windows.Forms.Padding(0);
			this.labelWhere2.Name = "labelWhere2";
			this.labelWhere2.Size = new System.Drawing.Size(200, 23);
			this.labelWhere2.TabIndex = 5;
			this.labelWhere2.Text = "of the following condtions are true:";
			this.labelWhere2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelWhere2.Visible = false;
			// 
			// comboBoxAndOr
			// 
			this.comboBoxAndOr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxAndOr.FormattingEnabled = true;
			this.comboBoxAndOr.Items.AddRange(new object[] {
            "ALL",
            "ANY"});
			this.comboBoxAndOr.Location = new System.Drawing.Point(67, 41);
			this.comboBoxAndOr.MaxDropDownItems = 2;
			this.comboBoxAndOr.Name = "comboBoxAndOr";
			this.comboBoxAndOr.Size = new System.Drawing.Size(60, 21);
			this.comboBoxAndOr.TabIndex = 4;
			this.comboBoxAndOr.Visible = false;
			this.comboBoxAndOr.VisibleChanged += new System.EventHandler(this.comboBoxAndOr_VisibleChanged);
			// 
			// labelWhere
			// 
			this.labelWhere.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelWhere.Location = new System.Drawing.Point(20, 41);
			this.labelWhere.Margin = new System.Windows.Forms.Padding(0);
			this.labelWhere.Name = "labelWhere";
			this.labelWhere.Size = new System.Drawing.Size(44, 23);
			this.labelWhere.TabIndex = 3;
			this.labelWhere.Text = "Where";
			this.labelWhere.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBoxWhere
			// 
			this.groupBoxWhere.Location = new System.Drawing.Point(20, 63);
			this.groupBoxWhere.Name = "groupBoxWhere";
			this.groupBoxWhere.Size = new System.Drawing.Size(561, 50);
			this.groupBoxWhere.TabIndex = 6;
			this.groupBoxWhere.TabStop = false;
			this.groupBoxWhere.Layout += new System.Windows.Forms.LayoutEventHandler(this.groupBoxWhere_Layout);
			// 
			// comboBoxForms
			// 
			this.comboBoxForms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxForms.DropDownHeight = 100;
			this.comboBoxForms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxForms.FormattingEnabled = true;
			this.comboBoxForms.IntegralHeight = false;
			this.comboBoxForms.Location = new System.Drawing.Point(125, 16);
			this.comboBoxForms.Name = "comboBoxForms";
			this.comboBoxForms.Size = new System.Drawing.Size(192, 21);
			this.comboBoxForms.TabIndex = 2;
			this.comboBoxForms.SelectionChangeCommitted += new System.EventHandler(this.comboBoxForms_SelectionChangeCommitted);
			// 
			// DeleteStatementView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl);
			this.DoubleBuffered = true;
			this.Name = "DeleteStatementView";
			this.Size = new System.Drawing.Size(604, 186);
			this.tabControl.ResumeLayout(false);
			this.tabPageDelete.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelFrom;
		private AddModifyButton buttonAddModify;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageDelete;
		private System.Windows.Forms.ComboBox comboBoxForms;
        private System.Windows.Forms.GroupBox groupBoxWhere;
        private System.Windows.Forms.Label labelWhere2;
        private System.Windows.Forms.ComboBox comboBoxAndOr;
        private System.Windows.Forms.Label labelWhere;
	}
}
