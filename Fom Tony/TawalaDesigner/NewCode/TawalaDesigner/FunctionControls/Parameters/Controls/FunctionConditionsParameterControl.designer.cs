namespace Tawala.Functions.Controls
{
    partial class FunctionConditionsParameterControl
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
            this.labelWhere2 = new System.Windows.Forms.Label();
            this.comboBoxAndOr = new System.Windows.Forms.ComboBox();
            this.labelWhere = new System.Windows.Forms.Label();
            this.groupBoxWhere = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelWhere2
            // 
            this.labelWhere2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelWhere2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWhere2.Location = new System.Drawing.Point(274, 0);
            this.labelWhere2.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.labelWhere2.Name = "labelWhere2";
            this.labelWhere2.Size = new System.Drawing.Size(184, 20);
            this.labelWhere2.TabIndex = 9;
            this.labelWhere2.Text = "of the following condtions are true:";
            this.labelWhere2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxAndOr
            // 
            this.comboBoxAndOr.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBoxAndOr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAndOr.FormattingEnabled = true;
            this.comboBoxAndOr.Items.AddRange(new object[] {
            "ALL",
            "ANY"});
            this.comboBoxAndOr.Location = new System.Drawing.Point(210, 0);
            this.comboBoxAndOr.Margin = new System.Windows.Forms.Padding(0);
            this.comboBoxAndOr.MaxDropDownItems = 2;
            this.comboBoxAndOr.Name = "comboBoxAndOr";
            this.comboBoxAndOr.Size = new System.Drawing.Size(60, 21);
            this.comboBoxAndOr.TabIndex = 8;
            this.comboBoxAndOr.VisibleChanged += new System.EventHandler(this.comboBoxAndOr_VisibleChanged);
            // 
            // labelWhere
            // 
            this.labelWhere.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelWhere.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWhere.Location = new System.Drawing.Point(0, 0);
            this.labelWhere.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.labelWhere.Name = "labelWhere";
            this.labelWhere.Size = new System.Drawing.Size(206, 20);
            this.labelWhere.TabIndex = 7;
            this.labelWhere.Text = "Limit output to records where";
            this.labelWhere.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBoxWhere
            // 
            this.groupBoxWhere.AutoSize = true;
            this.groupBoxWhere.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxWhere.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxWhere.Location = new System.Drawing.Point(0, 21);
            this.groupBoxWhere.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.groupBoxWhere.MinimumSize = new System.Drawing.Size(470, 36);
            this.groupBoxWhere.Name = "groupBoxWhere";
            this.groupBoxWhere.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxWhere.Size = new System.Drawing.Size(470, 36);
            this.groupBoxWhere.TabIndex = 102;
            this.groupBoxWhere.TabStop = false;
            this.groupBoxWhere.Layout += new System.Windows.Forms.LayoutEventHandler(this.groupBoxWhere_Layout);
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoSize = true;
            this.flowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel.Controls.Add(this.labelWhere);
            this.flowLayoutPanel.Controls.Add(this.comboBoxAndOr);
            this.flowLayoutPanel.Controls.Add(this.labelWhere2);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.flowLayoutPanel.MinimumSize = new System.Drawing.Size(470, 21);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(470, 21);
            this.flowLayoutPanel.TabIndex = 101;
            // 
            // FunctionConditionsParameterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.groupBoxWhere);
            this.Controls.Add(this.flowLayoutPanel);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.Name = "FunctionConditionsParameterControl";
            this.Size = new System.Drawing.Size(470, 57);
            this.flowLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWhere2;
        private System.Windows.Forms.ComboBox comboBoxAndOr;
        private System.Windows.Forms.Label labelWhere;
        private System.Windows.Forms.GroupBox groupBoxWhere;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
    }
}
