namespace Tawala.Functions.Controls
{
	partial class ColumnControl
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
            this.labelHeading = new System.Windows.Forms.Label();
            this.labelContents = new System.Windows.Forms.Label();
            this.linkLabelConditions = new System.Windows.Forms.LinkLabel();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.headingControl = new Tawala.Functions.Controls.ColumnHeadingControl();
            this.contentControl = new Tawala.Functions.Controls.ColumnContentsControl();
            this.conditionListControl = new Tawala.Functions.Controls.ConditionListControl();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeading.Location = new System.Drawing.Point(4, 18);
            this.labelHeading.MaximumSize = new System.Drawing.Size(56, 20);
            this.labelHeading.MinimumSize = new System.Drawing.Size(56, 20);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(56, 20);
            this.labelHeading.TabIndex = 1;
            this.labelHeading.Text = "Heading:";
            this.labelHeading.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelContents
            // 
            this.labelContents.AutoSize = true;
            this.labelContents.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelContents.Location = new System.Drawing.Point(4, 42);
            this.labelContents.MaximumSize = new System.Drawing.Size(56, 20);
            this.labelContents.MinimumSize = new System.Drawing.Size(56, 20);
            this.labelContents.Name = "labelContents";
            this.labelContents.Size = new System.Drawing.Size(56, 20);
            this.labelContents.TabIndex = 3;
            this.labelContents.Text = "Contents:";
            this.labelContents.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // linkLabelConditions
            // 
            this.linkLabelConditions.ActiveLinkColor = System.Drawing.Color.Blue;
            this.linkLabelConditions.AutoSize = true;
            this.linkLabelConditions.DisabledLinkColor = System.Drawing.Color.Blue;
            this.linkLabelConditions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelConditions.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkLabelConditions.Location = new System.Drawing.Point(6, 66);
            this.linkLabelConditions.Margin = new System.Windows.Forms.Padding(3, 2, 0, 0);
            this.linkLabelConditions.MinimumSize = new System.Drawing.Size(75, 13);
            this.linkLabelConditions.Name = "linkLabelConditions";
            this.linkLabelConditions.Size = new System.Drawing.Size(75, 13);
            this.linkLabelConditions.TabIndex = 5;
            this.linkLabelConditions.TabStop = true;
            this.linkLabelConditions.Text = "edit conditions";
            this.linkLabelConditions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabelConditions.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabelConditions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.linkLabelConditions_MouseUp);
            // 
            // groupBox
            // 
            this.groupBox.AutoSize = true;
            this.groupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox.Controls.Add(this.labelHeading);
            this.groupBox.Controls.Add(this.headingControl);
            this.groupBox.Controls.Add(this.labelContents);
            this.groupBox.Controls.Add(this.contentControl);
            this.groupBox.Controls.Add(this.linkLabelConditions);
            this.groupBox.Controls.Add(this.conditionListControl);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.Location = new System.Drawing.Point(1, 1);
            this.groupBox.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox.MinimumSize = new System.Drawing.Size(490, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox.Size = new System.Drawing.Size(490, 122);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Column {0}";
            this.groupBox.Layout += new System.Windows.Forms.LayoutEventHandler(this.groupBox_Layout);
            // 
            // headingControl
            // 
            this.headingControl.AllowDrop = true;
            this.headingControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.headingControl.BackColor = System.Drawing.SystemColors.Window;
            this.headingControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headingControl.ForeColor = System.Drawing.SystemColors.WindowText;
            this.headingControl.Location = new System.Drawing.Point(66, 18);
            this.headingControl.Margin = new System.Windows.Forms.Padding(0);
            this.headingControl.MinimumSize = new System.Drawing.Size(100, 20);
            this.headingControl.Name = "headingControl";
            this.headingControl.Size = new System.Drawing.Size(413, 20);
            this.headingControl.TabIndex = 2;
            // 
            // contentControl
            // 
            this.contentControl.AllowDrop = true;
            this.contentControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contentControl.BackColor = System.Drawing.SystemColors.Window;
            this.contentControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contentControl.ForeColor = System.Drawing.SystemColors.WindowText;
            this.contentControl.Location = new System.Drawing.Point(66, 42);
            this.contentControl.Margin = new System.Windows.Forms.Padding(0);
            this.contentControl.MinimumSize = new System.Drawing.Size(100, 20);
            this.contentControl.Name = "contentControl";
            this.contentControl.ReadOnly = true;
            this.contentControl.Size = new System.Drawing.Size(413, 20);
            this.contentControl.TabIndex = 4;
            this.contentControl.Value = null;
            // 
            // conditionListControl
            // 
            this.conditionListControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.conditionListControl.AutoSize = true;
            this.conditionListControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.conditionListControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionListControl.Location = new System.Drawing.Point(8, 84);
            this.conditionListControl.Margin = new System.Windows.Forms.Padding(0);
            this.conditionListControl.MinimumSize = new System.Drawing.Size(460, 0);
            this.conditionListControl.Name = "conditionListControl";
            this.conditionListControl.Size = new System.Drawing.Size(460, 25);
            this.conditionListControl.TabIndex = 6;
            this.conditionListControl.WhereText = "Limit output to records where";
            // 
            // ColumnControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.groupBox);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.MinimumSize = new System.Drawing.Size(492, 0);
            this.Name = "ColumnControl";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(492, 124);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Label labelHeading;
		private ColumnContentsControl contentControl;
		private System.Windows.Forms.Label labelContents;
        private ColumnHeadingControl headingControl;
        private System.Windows.Forms.LinkLabel linkLabelConditions;
        private Tawala.Functions.Controls.ConditionListControl conditionListControl;
        private System.Windows.Forms.GroupBox groupBox;
	}
}
