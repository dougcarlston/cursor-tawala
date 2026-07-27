namespace Tawala.Functions.Controls
{
    partial class FunctionConditionListControl
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
            this.groupBoxFunctions = new System.Windows.Forms.GroupBox();
            this.conditionListControl = new Tawala.Functions.Controls.ConditionListControl();
            this.groupBoxFunctions.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxFunctions
            // 
            this.groupBoxFunctions.AutoSize = true;
            this.groupBoxFunctions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxFunctions.Controls.Add(this.conditionListControl);
            this.groupBoxFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxFunctions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxFunctions.Location = new System.Drawing.Point(0, 0);
            this.groupBoxFunctions.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxFunctions.MinimumSize = new System.Drawing.Size(466, 81);
            this.groupBoxFunctions.Name = "groupBoxFunctions";
            this.groupBoxFunctions.Size = new System.Drawing.Size(472, 81);
            this.groupBoxFunctions.TabIndex = 1;
            this.groupBoxFunctions.TabStop = false;
            this.groupBoxFunctions.Text = "Function Conditions";
            // 
            // conditionListControl
            // 
            this.conditionListControl.AutoSize = true;
            this.conditionListControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.conditionListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conditionListControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionListControl.Location = new System.Drawing.Point(3, 16);
            this.conditionListControl.Margin = new System.Windows.Forms.Padding(0);
            this.conditionListControl.MaximumSize = new System.Drawing.Size(600, 0);
            this.conditionListControl.MinimumSize = new System.Drawing.Size(460, 62);
            this.conditionListControl.Name = "conditionListControl";
            this.conditionListControl.Size = new System.Drawing.Size(466, 62);
            this.conditionListControl.TabIndex = 0;
            this.conditionListControl.WhereText = "Limit output to records where";
            // 
            // FunctionConditionListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.groupBoxFunctions);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.MinimumSize = new System.Drawing.Size(472, 83);
            this.Name = "FunctionConditionListControl";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.Size = new System.Drawing.Size(472, 83);
            this.groupBoxFunctions.ResumeLayout(false);
            this.groupBoxFunctions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ConditionListControl conditionListControl;
        private System.Windows.Forms.GroupBox groupBoxFunctions;

    }
}
