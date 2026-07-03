namespace Tawala.Functions.Controls
{
    partial class ConditionListControl
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
            this.flowLayoutConditions = new Tawala.Functions.Controls.ConditionListControl.ConditonsFlowLayoutPanel();
            this.labelWhere = new System.Windows.Forms.Label();
            this.comboBoxAndOr = new System.Windows.Forms.ComboBox();
            this.labelWhere2 = new System.Windows.Forms.Label();
            this.panel = new Tawala.Functions.Controls.ConditionListControl.DoubleBufferedFlowLayoutPanel();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutConditions
            // 
            this.flowLayoutConditions.AutoSize = true;
            this.flowLayoutConditions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutConditions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutConditions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutConditions.Location = new System.Drawing.Point(0, 25);
            this.flowLayoutConditions.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutConditions.MinimumSize = new System.Drawing.Size(460, 0);
            this.flowLayoutConditions.Name = "flowLayoutConditions";
            this.flowLayoutConditions.Size = new System.Drawing.Size(460, 0);
            this.flowLayoutConditions.TabIndex = 1;
            this.flowLayoutConditions.WrapContents = false;
            this.flowLayoutConditions.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.flowLayoutConditions_ControlAdded);
            this.flowLayoutConditions.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.flowLayoutConditions_ControlRemoved);
            // 
            // labelWhere
            // 
            this.labelWhere.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelWhere.AutoSize = true;
            this.labelWhere.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWhere.Location = new System.Drawing.Point(4, 7);
            this.labelWhere.Margin = new System.Windows.Forms.Padding(4, 2, 0, 0);
            this.labelWhere.Name = "labelWhere";
            this.labelWhere.Size = new System.Drawing.Size(143, 13);
            this.labelWhere.TabIndex = 1;
            this.labelWhere.Text = "Limit output to records where";
            this.labelWhere.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxAndOr
            // 
            this.comboBoxAndOr.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBoxAndOr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAndOr.FormattingEnabled = true;
            this.comboBoxAndOr.Location = new System.Drawing.Point(153, 2);
            this.comboBoxAndOr.Margin = new System.Windows.Forms.Padding(6, 2, 8, 2);
            this.comboBoxAndOr.MaxDropDownItems = 2;
            this.comboBoxAndOr.MinimumSize = new System.Drawing.Size(60, 0);
            this.comboBoxAndOr.Name = "comboBoxAndOr";
            this.comboBoxAndOr.Size = new System.Drawing.Size(60, 21);
            this.comboBoxAndOr.TabIndex = 2;
            this.comboBoxAndOr.Visible = false;
            // 
            // labelWhere2
            // 
            this.labelWhere2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelWhere2.AutoSize = true;
            this.labelWhere2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWhere2.Location = new System.Drawing.Point(221, 7);
            this.labelWhere2.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelWhere2.Name = "labelWhere2";
            this.labelWhere2.Size = new System.Drawing.Size(169, 13);
            this.labelWhere2.TabIndex = 3;
            this.labelWhere2.Text = "of the following condtions are true:";
            this.labelWhere2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelWhere2.Visible = false;
            // 
            // panel
            // 
            this.panel.Controls.Add(this.labelWhere);
            this.panel.Controls.Add(this.comboBoxAndOr);
            this.panel.Controls.Add(this.labelWhere2);
            this.panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.MaximumSize = new System.Drawing.Size(0, 25);
            this.panel.MinimumSize = new System.Drawing.Size(460, 25);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(460, 25);
            this.panel.TabIndex = 5;
            // 
            // ConditionListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowLayoutConditions);
            this.Controls.Add(this.panel);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(460, 0);
            this.Name = "ConditionListControl";
            this.Size = new System.Drawing.Size(460, 25);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ConditonsFlowLayoutPanel flowLayoutConditions;
        private System.Windows.Forms.Label labelWhere;
        private System.Windows.Forms.ComboBox comboBoxAndOr;
        private System.Windows.Forms.Label labelWhere2;
        private DoubleBufferedFlowLayoutPanel panel;
    }
}
