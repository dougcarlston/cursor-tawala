namespace Tawala.Forms.Preview
{
	partial class FormPreviewControl
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
			this.browser = new Tawala.Forms.FormPreview();
			this.SuspendLayout();
			// 
			// browser
			// 
			this.browser.AllowNavigation = false;
			this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.browser.IsWebBrowserContextMenuEnabled = false;
			this.browser.Location = new System.Drawing.Point(0, 0);
			this.browser.Margin = new System.Windows.Forms.Padding(0);
			this.browser.MinimumSize = new System.Drawing.Size(20, 20);
			this.browser.Name = "browser";
			this.browser.ScriptErrorsSuppressed = true;
			this.browser.Size = new System.Drawing.Size(308, 291);
			this.browser.TabIndex = 0;
			this.browser.WebBrowserShortcutsEnabled = false;
			// 
			// FormPreviewControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.browser);
			this.Name = "FormPreviewControl";
			this.Size = new System.Drawing.Size(308, 291);
			this.ResumeLayout(false);

		}

		#endregion

		private FormPreview browser;
	}
}
