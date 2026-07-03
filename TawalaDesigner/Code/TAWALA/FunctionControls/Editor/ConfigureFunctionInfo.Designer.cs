namespace Tawala.FunctionControls.Editor
{
    partial class ConfigureFunctionInfo
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
            this.labelFunctionName = new System.Windows.Forms.Label();
            this.labelFunctionDescription = new System.Windows.Forms.Label();
            this.labelParameterName = new System.Windows.Forms.Label();
            this.labelParameterDescription = new System.Windows.Forms.Label();
            this.labelParameterTag = new System.Windows.Forms.Label();
            this.labelParameterRequired = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelFunctionName
            // 
            this.labelFunctionName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelFunctionName.AutoSize = true;
            this.labelFunctionName.BackColor = System.Drawing.Color.Transparent;
            this.SetFlowBreak(this.labelFunctionName, true);
            this.labelFunctionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFunctionName.Location = new System.Drawing.Point(3, 3);
            this.labelFunctionName.Margin = new System.Windows.Forms.Padding(3);
            this.labelFunctionName.Name = "labelFunctionName";
            this.labelFunctionName.Size = new System.Drawing.Size(119, 15);
            this.labelFunctionName.TabIndex = 0;
            this.labelFunctionName.Text = "FUNCTION NAME";
            this.labelFunctionName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFunctionDescription
            // 
            this.labelFunctionDescription.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelFunctionDescription.AutoSize = true;
            this.labelFunctionDescription.BackColor = System.Drawing.Color.Transparent;
            this.SetFlowBreak(this.labelFunctionDescription, true);
            this.labelFunctionDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFunctionDescription.Location = new System.Drawing.Point(3, 29);
            this.labelFunctionDescription.Margin = new System.Windows.Forms.Padding(3);
            this.labelFunctionDescription.MinimumSize = new System.Drawing.Size(0, 13);
            this.labelFunctionDescription.Name = "labelFunctionDescription";
            this.labelFunctionDescription.Size = new System.Drawing.Size(104, 13);
            this.labelFunctionDescription.TabIndex = 1;
            this.labelFunctionDescription.Text = "Function Description";
            this.labelFunctionDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelParameterName
            // 
            this.labelParameterName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelParameterName.AutoSize = true;
            this.labelParameterName.BackColor = System.Drawing.Color.Transparent;
            this.SetFlowBreak(this.labelParameterName, true);
            this.labelParameterName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelParameterName.Location = new System.Drawing.Point(3, 63);
            this.labelParameterName.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
            this.labelParameterName.Name = "labelParameterName";
            this.labelParameterName.Size = new System.Drawing.Size(116, 15);
            this.labelParameterName.TabIndex = 2;
            this.labelParameterName.Text = "Parameter Name";
            this.labelParameterName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelParameterDescription
            // 
            this.labelParameterDescription.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelParameterDescription.AutoSize = true;
            this.labelParameterDescription.BackColor = System.Drawing.Color.Transparent;
            this.SetFlowBreak(this.labelParameterDescription, true);
            this.labelParameterDescription.Location = new System.Drawing.Point(3, 84);
            this.labelParameterDescription.Margin = new System.Windows.Forms.Padding(3);
            this.labelParameterDescription.Name = "labelParameterDescription";
            this.labelParameterDescription.Size = new System.Drawing.Size(111, 13);
            this.labelParameterDescription.TabIndex = 3;
            this.labelParameterDescription.Text = "Parameter Description";
            this.labelParameterDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelParameterTag
            // 
            this.labelParameterTag.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelParameterTag.AutoSize = true;
            this.labelParameterTag.BackColor = System.Drawing.Color.Transparent;
            this.SetFlowBreak(this.labelParameterTag, true);
            this.labelParameterTag.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelParameterTag.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labelParameterTag.Location = new System.Drawing.Point(3, 103);
            this.labelParameterTag.Margin = new System.Windows.Forms.Padding(3);
            this.labelParameterTag.Name = "labelParameterTag";
            this.labelParameterTag.Size = new System.Drawing.Size(90, 13);
            this.labelParameterTag.TabIndex = 4;
            this.labelParameterTag.Text = "Parameter Tag";
            this.labelParameterTag.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelParameterRequired
            // 
            this.labelParameterRequired.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelParameterRequired.AutoSize = true;
            this.labelParameterRequired.BackColor = System.Drawing.Color.Transparent;
            this.SetFlowBreak(this.labelParameterRequired, true);
            this.labelParameterRequired.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelParameterRequired.ForeColor = System.Drawing.Color.Red;
            this.labelParameterRequired.Location = new System.Drawing.Point(3, 103);
            this.labelParameterRequired.Margin = new System.Windows.Forms.Padding(3);
            this.labelParameterRequired.Name = "labelParameterRequired";
            this.labelParameterRequired.Size = new System.Drawing.Size(90, 13);
            this.labelParameterRequired.TabIndex = 5;
            this.labelParameterRequired.Text = "REQUIRED";
            this.labelParameterRequired.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelParameterRequired.Visible = false;
            // 
            // ConfigureFunctionInfo
            // 
            this.BackColor = System.Drawing.SystemColors.Info;
            this.Controls.Add(this.labelFunctionName);
            this.Controls.Add(this.labelFunctionDescription);
            this.Controls.Add(this.labelParameterName);
            this.Controls.Add(this.labelParameterDescription);
            this.Controls.Add(this.labelParameterTag);
            this.Controls.Add(this.labelParameterRequired);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(240, 0);
            this.MinimumSize = new System.Drawing.Size(240, 460);
            this.Size = new System.Drawing.Size(240, 460);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelFunctionName;
        private System.Windows.Forms.Label labelFunctionDescription;
        private System.Windows.Forms.Label labelParameterName;
        private System.Windows.Forms.Label labelParameterDescription;
        private System.Windows.Forms.Label labelParameterTag;
        private System.Windows.Forms.Label labelParameterRequired;
    }
}
