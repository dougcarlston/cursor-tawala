namespace Tawala.Functions.Controls
{
	partial class ColumnParameterControl
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
            this.groupBoxColumnInfo = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelHeading = new System.Windows.Forms.Label();
            this.labelContents = new System.Windows.Forms.Label();
            this.linkLabelConditions = new System.Windows.Forms.LinkLabel();
            this.textBoxHeading = new Tawala.Functions.Controls.CompoundExpressionParameterTextBox();
            this.textBoxColumnContents = new Tawala.Functions.Controls.ContentsFieldParameterTextBox();
            this.conditionsControl = new Tawala.Functions.Controls.ColumnConditionsParameterControl();
            this.groupBoxColumnInfo.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxColumnInfo
            // 
            this.groupBoxColumnInfo.AutoSize = true;
            this.groupBoxColumnInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxColumnInfo.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxColumnInfo.Controls.Add(this.tableLayoutPanel);
            this.groupBoxColumnInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxColumnInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxColumnInfo.Location = new System.Drawing.Point(0, 0);
            this.groupBoxColumnInfo.Name = "groupBoxColumnInfo";
            this.groupBoxColumnInfo.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxColumnInfo.Size = new System.Drawing.Size(682, 154);
            this.groupBoxColumnInfo.TabIndex = 0;
            this.groupBoxColumnInfo.TabStop = false;
            this.groupBoxColumnInfo.Text = "Column {0}";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.labelHeading, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxHeading, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelContents, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxColumnContents, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.linkLabelConditions, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.conditionsControl, 0, 3);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel.Location = new System.Drawing.Point(4, 17);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(674, 133);
            this.tableLayoutPanel.TabIndex = 7;
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeading.Location = new System.Drawing.Point(3, 0);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(52, 28);
            this.labelHeading.TabIndex = 1;
            this.labelHeading.Text = "Heading:";
            this.labelHeading.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelContents
            // 
            this.labelContents.AutoSize = true;
            this.labelContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelContents.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelContents.Location = new System.Drawing.Point(3, 28);
            this.labelContents.Name = "labelContents";
            this.labelContents.Size = new System.Drawing.Size(52, 28);
            this.labelContents.TabIndex = 3;
            this.labelContents.Text = "Contents:";
            this.labelContents.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // linkLabelConditions
            // 
            this.linkLabelConditions.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.linkLabelConditions, 2);
            this.linkLabelConditions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelConditions.Location = new System.Drawing.Point(3, 56);
            this.linkLabelConditions.Margin = new System.Windows.Forms.Padding(3, 0, 3, 1);
            this.linkLabelConditions.Name = "linkLabelConditions";
            this.linkLabelConditions.Size = new System.Drawing.Size(75, 13);
            this.linkLabelConditions.TabIndex = 1;
            this.linkLabelConditions.TabStop = true;
            this.linkLabelConditions.Text = "edit conditions";
            this.linkLabelConditions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelConditions_LinkClicked);
            // 
            // textBoxHeading
            // 
            this.textBoxHeading.AllowDrop = true;
            this.textBoxHeading.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxHeading.CustomDataSource = null;
            this.textBoxHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxHeading.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxHeading.Location = new System.Drawing.Point(62, 4);
            this.textBoxHeading.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxHeading.MinimumSize = new System.Drawing.Size(100, 4);
            this.textBoxHeading.Name = "textBoxHeading";
            this.textBoxHeading.Size = new System.Drawing.Size(608, 20);
            this.textBoxHeading.TabIndex = 2;
            // 
            // textBoxColumnContents
            // 
            this.textBoxColumnContents.AllowDrop = true;
            this.textBoxColumnContents.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxColumnContents.CustomDataSource = null;
            this.textBoxColumnContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxColumnContents.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxColumnContents.Location = new System.Drawing.Point(62, 32);
            this.textBoxColumnContents.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxColumnContents.MinimumSize = new System.Drawing.Size(100, 4);
            this.textBoxColumnContents.Name = "textBoxColumnContents";
            this.textBoxColumnContents.ReadOnly = true;
            this.textBoxColumnContents.Size = new System.Drawing.Size(608, 20);
            this.textBoxColumnContents.TabIndex = 4;
            // 
            // conditionsControl
            // 
            this.conditionsControl.AutoSize = true;
            this.conditionsControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.SetColumnSpan(this.conditionsControl, 2);
            this.conditionsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conditionsControl.Location = new System.Drawing.Point(2, 72);
            this.conditionsControl.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.conditionsControl.MinimumSize = new System.Drawing.Size(460, 0);
            this.conditionsControl.Name = "conditionsControl";
            this.conditionsControl.Size = new System.Drawing.Size(672, 59);
            this.conditionsControl.TabIndex = 5;
            this.conditionsControl.Visible = false;
            this.conditionsControl.WhereText = "Limit output to records where";
            // 
            // ColumnParameterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.groupBoxColumnInfo);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(150, 102);
            this.Name = "ColumnParameterControl";
            this.Size = new System.Drawing.Size(682, 154);
            this.groupBoxColumnInfo.ResumeLayout(false);
            this.groupBoxColumnInfo.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxColumnInfo;
		private System.Windows.Forms.Label labelHeading;
		private ContentsFieldParameterTextBox textBoxColumnContents;
		private System.Windows.Forms.Label labelContents;
        private Tawala.Functions.Controls.CompoundExpressionParameterTextBox textBoxHeading;
        private ColumnConditionsParameterControl conditionsControl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.LinkLabel linkLabelConditions;
	}
}
