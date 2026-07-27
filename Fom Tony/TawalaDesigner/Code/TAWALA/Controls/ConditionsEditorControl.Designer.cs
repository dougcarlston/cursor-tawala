using System.Windows.Forms;

namespace Tawala.Controls
{
    partial class ConditionsEditorControl
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
            this.labelAfter = new System.Windows.Forms.Label();
            this.comboBoxAndOr = new System.Windows.Forms.ComboBox();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.editControlsLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.conditionEditControl1 = new Tawala.Controls.ConditionEditControl();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.labelBefore = new System.Windows.Forms.Label();
            this.groupBox.SuspendLayout();
            this.editControlsLayout.SuspendLayout();
            this.flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelAfter
            // 
            this.labelAfter.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelAfter.AutoSize = true;
            this.flowLayoutPanel.SetFlowBreak(this.labelAfter, true);
            this.labelAfter.Location = new System.Drawing.Point(115, 7);
            this.labelAfter.Margin = new System.Windows.Forms.Padding(9, 0, 0, 0);
            this.labelAfter.Name = "labelAfter";
            this.labelAfter.Size = new System.Drawing.Size(29, 13);
            this.labelAfter.TabIndex = 3;
            this.labelAfter.Text = "After";
            this.labelAfter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxAndOr
            // 
            this.comboBoxAndOr.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBoxAndOr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAndOr.DropDownWidth = 50;
            this.comboBoxAndOr.FormattingEnabled = true;
            this.comboBoxAndOr.Items.AddRange(new object[] {
            "ALL",
            "ANY"});
            this.comboBoxAndOr.Location = new System.Drawing.Point(53, 3);
            this.comboBoxAndOr.MaxDropDownItems = 2;
            this.comboBoxAndOr.Name = "comboBoxAndOr";
            this.comboBoxAndOr.Size = new System.Drawing.Size(50, 21);
            this.comboBoxAndOr.TabIndex = 2;
            this.comboBoxAndOr.VisibleChanged += new System.EventHandler(this.comboBoxAndOr_VisibleChanged);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.editControlsLayout);
            this.groupBox.Controls.Add(this.flowLayoutPanel);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Padding = new System.Windows.Forms.Padding(1);
            this.groupBox.Size = new System.Drawing.Size(468, 75);
            this.groupBox.TabIndex = 4;
            this.groupBox.TabStop = false;
            this.groupBox.Layout += new System.Windows.Forms.LayoutEventHandler(this.groupBox_Layout);
            // 
            // editControlsLayout
            // 
            this.editControlsLayout.Controls.Add(this.conditionEditControl1);
            this.editControlsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editControlsLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.editControlsLayout.Location = new System.Drawing.Point(1, 47);
            this.editControlsLayout.Margin = new System.Windows.Forms.Padding(0);
            this.editControlsLayout.MinimumSize = new System.Drawing.Size(462, 23);
            this.editControlsLayout.Name = "editControlsLayout";
            this.editControlsLayout.Padding = new System.Windows.Forms.Padding(2);
            this.editControlsLayout.Size = new System.Drawing.Size(466, 27);
            this.editControlsLayout.TabIndex = 5;
            this.editControlsLayout.WrapContents = false;
            this.editControlsLayout.Layout += new System.Windows.Forms.LayoutEventHandler(this.editControlsLayout_Layout);
            this.editControlsLayout.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.editControlsLayout_ControlAdded);
            this.editControlsLayout.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.editControlsLayout_ControlRemoved);
            // 
            // conditionEditControl1
            // 
            this.conditionEditControl1.AllowDrop = true;
            this.conditionEditControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.conditionEditControl1.BackColor = System.Drawing.SystemColors.Control;
            this.conditionEditControl1.Location = new System.Drawing.Point(2, 2);
            this.conditionEditControl1.Margin = new System.Windows.Forms.Padding(0);
            this.conditionEditControl1.MaximumSize = new System.Drawing.Size(0, 27);
            this.conditionEditControl1.MinimumSize = new System.Drawing.Size(466, 27);
            this.conditionEditControl1.Name = "conditionEditControl1";
            this.conditionEditControl1.Size = new System.Drawing.Size(466, 27);
            this.conditionEditControl1.TabIndex = 0;
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.Controls.Add(this.labelBefore);
            this.flowLayoutPanel.Controls.Add(this.comboBoxAndOr);
            this.flowLayoutPanel.Controls.Add(this.labelAfter);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.Location = new System.Drawing.Point(1, 14);
            this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.flowLayoutPanel.Size = new System.Drawing.Size(466, 33);
            this.flowLayoutPanel.TabIndex = 8;
            this.flowLayoutPanel.WrapContents = false;
            // 
            // labelBefore
            // 
            this.labelBefore.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelBefore.AutoSize = true;
            this.labelBefore.Location = new System.Drawing.Point(3, 7);
            this.labelBefore.Margin = new System.Windows.Forms.Padding(3, 0, 9, 0);
            this.labelBefore.Name = "labelBefore";
            this.labelBefore.Size = new System.Drawing.Size(38, 13);
            this.labelBefore.TabIndex = 1;
            this.labelBefore.Text = "Before";
            this.labelBefore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ConditionsEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(468, 75);
            this.Name = "ConditionsEditorControl";
            this.Size = new System.Drawing.Size(468, 75);
            this.groupBox.ResumeLayout(false);
            this.editControlsLayout.ResumeLayout(false);
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelAfter;
        private System.Windows.Forms.ComboBox comboBoxAndOr;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Label labelBefore;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel editControlsLayout;
        private ConditionEditControl conditionEditControl1;
    }
}
