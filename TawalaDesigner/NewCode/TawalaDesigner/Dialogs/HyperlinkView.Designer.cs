namespace Tawala.Dialogs
{
    partial class HyperlinkView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HyperlinkView));
            this.checkBoxNewWindow = new System.Windows.Forms.CheckBox();
            this.textBoxDisplayText = new System.Windows.Forms.TextBox();
            this.labelDisplayText = new System.Windows.Forms.Label();
            this.labelUrl = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxUrl = new Tawala.Controls.ComplexExpressionTextBox();
            this.SuspendLayout();
            // 
            // checkBoxNewWindow
            // 
            this.checkBoxNewWindow.AutoSize = true;
            this.checkBoxNewWindow.Location = new System.Drawing.Point(12, 120);
            this.checkBoxNewWindow.Name = "checkBoxNewWindow";
            this.checkBoxNewWindow.Size = new System.Drawing.Size(187, 17);
            this.checkBoxNewWindow.TabIndex = 5;
            this.checkBoxNewWindow.Text = "Open link in new browser window.";
            this.checkBoxNewWindow.UseVisualStyleBackColor = true;
            // 
            // textBoxDisplayText
            // 
            this.textBoxDisplayText.Location = new System.Drawing.Point(12, 32);
            this.textBoxDisplayText.Name = "textBoxDisplayText";
            this.textBoxDisplayText.Size = new System.Drawing.Size(362, 20);
            this.textBoxDisplayText.TabIndex = 2;
            this.textBoxDisplayText.TextChanged += new System.EventHandler(this.textBoxDisplayText_TextChanged);
            // 
            // labelDisplayText
            // 
            this.labelDisplayText.AutoSize = true;
            this.labelDisplayText.Location = new System.Drawing.Point(12, 14);
            this.labelDisplayText.Name = "labelDisplayText";
            this.labelDisplayText.Size = new System.Drawing.Size(64, 13);
            this.labelDisplayText.TabIndex = 1;
            this.labelDisplayText.Text = "Display text:";
            // 
            // labelUrl
            // 
            this.labelUrl.AutoSize = true;
            this.labelUrl.Location = new System.Drawing.Point(12, 64);
            this.labelUrl.Name = "labelUrl";
            this.labelUrl.Size = new System.Drawing.Size(23, 13);
            this.labelUrl.TabIndex = 3;
            this.labelUrl.Text = "Url:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(215, 158);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(105, 158);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxUrl
            // 
            this.textBoxUrl.AllowDrop = true;
            this.textBoxUrl.Location = new System.Drawing.Point(12, 83);
            this.textBoxUrl.Name = "textBoxUrl";
            this.textBoxUrl.Size = new System.Drawing.Size(362, 20);
            this.textBoxUrl.TabIndex = 4;
            this.textBoxUrl.TextChanged += new System.EventHandler(this.textBoxUrl_TextChanged);
            this.textBoxUrl.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxUrl_DragDrop);
            this.textBoxUrl.Leave += new System.EventHandler(this.textBoxUrl_Leave);
            this.textBoxUrl.Enter += new System.EventHandler(this.textBoxUrl_Enter);
            this.textBoxUrl.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxUrl_DragEnter);
            // 
            // HyperlinkView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(387, 195);
            this.Controls.Add(this.textBoxUrl);
            this.Controls.Add(this.checkBoxNewWindow);
            this.Controls.Add(this.textBoxDisplayText);
            this.Controls.Add(this.labelDisplayText);
            this.Controls.Add(this.labelUrl);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HyperlinkView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Hyperlink";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxNewWindow;
        private System.Windows.Forms.TextBox textBoxDisplayText;
        private System.Windows.Forms.Label labelDisplayText;
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private Tawala.Controls.ComplexExpressionTextBox textBoxUrl;
    }
}