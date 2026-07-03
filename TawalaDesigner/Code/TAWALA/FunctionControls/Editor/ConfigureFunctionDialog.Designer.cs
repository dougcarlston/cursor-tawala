namespace Tawala.Functions.Controls
{
    partial class ConfigureFunctionDialog
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
            this.configureParametersLayoutPanel = new Tawala.Functions.Controls.ConfigureParametersLayoutPanel();
            this.configureFunctionButtons = new Tawala.Functions.Controls.ConfigureFunctionButtons();
            this.configureFunctionInfo = new Tawala.FunctionControls.Editor.ConfigureFunctionInfo();
            this.SuspendLayout();
            // 
            // configureParametersLayoutPanel
            // 
            this.configureParametersLayoutPanel.AutoScroll = true;
            this.configureParametersLayoutPanel.AutoSize = true;
            this.configureParametersLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.configureParametersLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configureParametersLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.configureParametersLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.configureParametersLayoutPanel.Name = "configureParametersLayoutPanel";
            this.configureParametersLayoutPanel.Padding = new System.Windows.Forms.Padding(4, 6, 2, 6);
            this.configureParametersLayoutPanel.Size = new System.Drawing.Size(540, 428);
            this.configureParametersLayoutPanel.TabIndex = 0;
            // 
            // configureFunctionButtons
            // 
            this.configureFunctionButtons.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.configureFunctionButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.configureFunctionButtons.Location = new System.Drawing.Point(0, 428);
            this.configureFunctionButtons.Margin = new System.Windows.Forms.Padding(0);
            this.configureFunctionButtons.MinimumSize = new System.Drawing.Size(540, 32);
            this.configureFunctionButtons.Name = "configureFunctionButtons";
            this.configureFunctionButtons.OKEnabled = true;
            this.configureFunctionButtons.Size = new System.Drawing.Size(540, 32);
            this.configureFunctionButtons.TabIndex = 2;
            // 
            // configureFunctionInfo
            // 
            this.configureFunctionInfo.BackColor = System.Drawing.SystemColors.Info;
            this.configureFunctionInfo.Dock = System.Windows.Forms.DockStyle.Right;
            this.configureFunctionInfo.Location = new System.Drawing.Point(540, 0);
            this.configureFunctionInfo.Margin = new System.Windows.Forms.Padding(0);
            this.configureFunctionInfo.MaximumSize = new System.Drawing.Size(240, 0);
            this.configureFunctionInfo.MinimumSize = new System.Drawing.Size(240, 460);
            this.configureFunctionInfo.Name = "configureFunctionInfo";
            this.configureFunctionInfo.Size = new System.Drawing.Size(240, 460);
            this.configureFunctionInfo.TabIndex = 1;
            // 
            // ConfigureFunctionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 460);
            this.Controls.Add(this.configureParametersLayoutPanel);
            this.Controls.Add(this.configureFunctionButtons);
            this.Controls.Add(this.configureFunctionInfo);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(796, 496);
            this.Name = "ConfigureFunctionDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Configure Function";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ConfigureParametersLayoutPanel configureParametersLayoutPanel;
        private Tawala.FunctionControls.Editor.ConfigureFunctionInfo configureFunctionInfo;
        private ConfigureFunctionButtons configureFunctionButtons;

    }
}