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
			this.panelButtons = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.configureFunctionControl = new Tawala.Functions.Controls.ConfigureFunctionControl();
			this.panelButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelButtons
			// 
			this.panelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Controls.Add(this.buttonOK);
			this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelButtons.Location = new System.Drawing.Point(0, 402);
			this.panelButtons.Margin = new System.Windows.Forms.Padding(0);
			this.panelButtons.MaximumSize = new System.Drawing.Size(0, 48);
			this.panelButtons.MinimumSize = new System.Drawing.Size(0, 48);
			this.panelButtons.Name = "panelButtons";
			this.panelButtons.Size = new System.Drawing.Size(556, 48);
			this.panelButtons.TabIndex = 2;
			this.panelButtons.TabStop = true;
			this.panelButtons.Layout += new System.Windows.Forms.LayoutEventHandler(this.panelButtons_Layout);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(291, 13);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(178, 13);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// configureFunctionControl
			// 
			this.configureFunctionControl.AutoSize = true;
			this.configureFunctionControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.configureFunctionControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.configureFunctionControl.Location = new System.Drawing.Point(0, 0);
			this.configureFunctionControl.Margin = new System.Windows.Forms.Padding(0);
			this.configureFunctionControl.MinimumSize = new System.Drawing.Size(400, 300);
			this.configureFunctionControl.Name = "configureFunctionControl";
			this.configureFunctionControl.Size = new System.Drawing.Size(556, 402);
			this.configureFunctionControl.TabIndex = 1;
			// 
			// ConfigureFunctionDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(556, 450);
			this.Controls.Add(this.configureFunctionControl);
			this.Controls.Add(this.panelButtons);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(550, 450);
			this.Name = "ConfigureFunctionDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure Function";
			this.panelButtons.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Panel panelButtons;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private ConfigureFunctionControl configureFunctionControl;
    }
}