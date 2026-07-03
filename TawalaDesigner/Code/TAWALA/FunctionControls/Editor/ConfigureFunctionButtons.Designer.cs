namespace Tawala.Functions.Controls
{
    partial class ConfigureFunctionButtons
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureFunctionButtons));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonPlus = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMinus = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMoveUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMoveDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOk = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.AutoSize = false;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonPlus,
            this.toolStripButtonMinus,
            this.toolStripButtonMoveUp,
            this.toolStripButtonMoveDown,
            this.toolStripButtonCancel,
            this.toolStripButtonOk});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.MaximumSize = new System.Drawing.Size(0, 28);
            this.toolStrip.MinimumSize = new System.Drawing.Size(540, 28);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(540, 28);
            this.toolStrip.TabIndex = 8;
            // 
            // toolStripButtonPlus
            // 
            this.toolStripButtonPlus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPlus.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonPlus.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPlus.Image")));
            this.toolStripButtonPlus.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonPlus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPlus.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
            this.toolStripButtonPlus.Name = "toolStripButtonPlus";
            this.toolStripButtonPlus.Size = new System.Drawing.Size(23, 25);
            this.toolStripButtonPlus.Text = "Add Column";
            this.toolStripButtonPlus.Visible = false;
            // 
            // toolStripButtonMinus
            // 
            this.toolStripButtonMinus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMinus.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMinus.Image")));
            this.toolStripButtonMinus.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonMinus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMinus.Name = "toolStripButtonMinus";
            this.toolStripButtonMinus.Size = new System.Drawing.Size(23, 25);
            this.toolStripButtonMinus.Text = "Remove Column";
            this.toolStripButtonMinus.Visible = false;
            // 
            // toolStripButtonMoveUp
            // 
            this.toolStripButtonMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveUp.Enabled = false;
            this.toolStripButtonMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveUp.Image")));
            this.toolStripButtonMoveUp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveUp.Name = "toolStripButtonMoveUp";
            this.toolStripButtonMoveUp.Size = new System.Drawing.Size(23, 25);
            this.toolStripButtonMoveUp.Text = "Move Column Up";
            this.toolStripButtonMoveUp.Visible = false;
            // 
            // toolStripButtonMoveDown
            // 
            this.toolStripButtonMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveDown.Enabled = false;
            this.toolStripButtonMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveDown.Image")));
            this.toolStripButtonMoveDown.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveDown.Name = "toolStripButtonMoveDown";
            this.toolStripButtonMoveDown.Size = new System.Drawing.Size(23, 25);
            this.toolStripButtonMoveDown.Text = "Move Column Down";
            this.toolStripButtonMoveDown.Visible = false;
            // 
            // toolStripButtonCancel
            // 
            this.toolStripButtonCancel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonCancel.AutoSize = false;
            this.toolStripButtonCancel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonCancel.Image = global::Tawala.Function.Controls.Properties.Resources.Cancel;
            this.toolStripButtonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonCancel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCancel.Name = "toolStripButtonCancel";
            this.toolStripButtonCancel.Size = new System.Drawing.Size(88, 25);
            this.toolStripButtonCancel.Text = "CANCEL";
            this.toolStripButtonCancel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripButtonOk
            // 
            this.toolStripButtonOk.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonOk.AutoSize = false;
            this.toolStripButtonOk.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonOk.Image = global::Tawala.Function.Controls.Properties.Resources.OK;
            this.toolStripButtonOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonOk.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonOk.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOk.Name = "toolStripButtonOk";
            this.toolStripButtonOk.Size = new System.Drawing.Size(88, 25);
            this.toolStripButtonOk.Text = "OK";
            this.toolStripButtonOk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ConfigureFunctionButtons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.toolStrip);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(540, 28);
            this.Name = "ConfigureFunctionButtons";
            this.Size = new System.Drawing.Size(540, 28);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonPlus;
        private System.Windows.Forms.ToolStripButton toolStripButtonMinus;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveUp;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveDown;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.ToolStripButton toolStripButtonOk;
    }
}
