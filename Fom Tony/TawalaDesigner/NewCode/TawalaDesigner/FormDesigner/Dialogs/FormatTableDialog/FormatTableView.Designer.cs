namespace Tawala.FormDesigner.Dialogs.FormatTableDialog
{
	partial class FormatTableView
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
			this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.tabPageColumns = new System.Windows.Forms.TabPage();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.flowLayoutPanelButtons.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanelButtons
			// 
			this.flowLayoutPanelButtons.AutoSize = true;
			this.flowLayoutPanelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanelButtons.Controls.Add(this.buttonCancel);
			this.flowLayoutPanelButtons.Controls.Add(this.buttonOK);
			this.flowLayoutPanelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.flowLayoutPanelButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanelButtons.Location = new System.Drawing.Point(0, 284);
			this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
			this.flowLayoutPanelButtons.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
			this.flowLayoutPanelButtons.Size = new System.Drawing.Size(460, 29);
			this.flowLayoutPanelButtons.TabIndex = 0;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(289, 3);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(370, 3);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// tabPageColumns
			// 
			this.tabPageColumns.Location = new System.Drawing.Point(4, 22);
			this.tabPageColumns.Name = "tabPageColumns";
			this.tabPageColumns.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageColumns.Size = new System.Drawing.Size(452, 258);
			this.tabPageColumns.TabIndex = 0;
			this.tabPageColumns.Text = "Columns";
			this.tabPageColumns.UseVisualStyleBackColor = true;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageColumns);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(460, 284);
			this.tabControl.TabIndex = 1;
			// 
			// FormatTableView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(460, 313);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.flowLayoutPanelButtons);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormatTableView";
			this.Text = "Format Table";
			this.flowLayoutPanelButtons.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.TabPage tabPageColumns;
		private System.Windows.Forms.TabControl tabControl;

	}
}