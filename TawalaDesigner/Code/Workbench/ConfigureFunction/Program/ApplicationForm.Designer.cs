namespace Program
{
	partial class ApplicationForm
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
			this.treeViewFieldsPalette = new System.Windows.Forms.TreeView();
			this.labelTreeView = new System.Windows.Forms.Label();
			this.labelOutputXml = new System.Windows.Forms.Label();
			this.textBoxOutputXml = new System.Windows.Forms.TextBox();
			this.buttonConfigureDisplayComponent = new System.Windows.Forms.Button();
			this.buttonConfigureFunction = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// treeViewFieldsPalette
			// 
			this.treeViewFieldsPalette.Location = new System.Drawing.Point(12, 34);
			this.treeViewFieldsPalette.Name = "treeViewFieldsPalette";
			this.treeViewFieldsPalette.Size = new System.Drawing.Size(166, 103);
			this.treeViewFieldsPalette.TabIndex = 0;
			this.treeViewFieldsPalette.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewFieldsPalette_ItemDrag);
			// 
			// labelTreeView
			// 
			this.labelTreeView.AutoSize = true;
			this.labelTreeView.Location = new System.Drawing.Point(13, 13);
			this.labelTreeView.Name = "labelTreeView";
			this.labelTreeView.Size = new System.Drawing.Size(155, 13);
			this.labelTreeView.TabIndex = 1;
			this.labelTreeView.Text = "Drag items from here into dialog";
			// 
			// labelOutputXml
			// 
			this.labelOutputXml.AutoSize = true;
			this.labelOutputXml.Location = new System.Drawing.Point(13, 168);
			this.labelOutputXml.Name = "labelOutputXml";
			this.labelOutputXml.Size = new System.Drawing.Size(67, 13);
			this.labelOutputXml.TabIndex = 3;
			this.labelOutputXml.Text = "Output XML:";
			// 
			// textBoxOutputXml
			// 
			this.textBoxOutputXml.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.textBoxOutputXml.Location = new System.Drawing.Point(0, 190);
			this.textBoxOutputXml.Multiline = true;
			this.textBoxOutputXml.Name = "textBoxOutputXml";
			this.textBoxOutputXml.Size = new System.Drawing.Size(435, 110);
			this.textBoxOutputXml.TabIndex = 4;
			// 
			// buttonConfigureDisplayComponent
			// 
			this.buttonConfigureDisplayComponent.Location = new System.Drawing.Point(238, 34);
			this.buttonConfigureDisplayComponent.Name = "buttonConfigureDisplayComponent";
			this.buttonConfigureDisplayComponent.Size = new System.Drawing.Size(165, 23);
			this.buttonConfigureDisplayComponent.TabIndex = 5;
			this.buttonConfigureDisplayComponent.Text = "Configure Display Component...";
			this.buttonConfigureDisplayComponent.UseVisualStyleBackColor = true;
			this.buttonConfigureDisplayComponent.Click += new System.EventHandler(this.buttonConfigureDisplayComponent_Click);
			// 
			// buttonConfigureFunction
			// 
			this.buttonConfigureFunction.Location = new System.Drawing.Point(238, 76);
			this.buttonConfigureFunction.Name = "buttonConfigureFunction";
			this.buttonConfigureFunction.Size = new System.Drawing.Size(165, 23);
			this.buttonConfigureFunction.TabIndex = 6;
			this.buttonConfigureFunction.Text = "Configure Function...";
			this.buttonConfigureFunction.UseVisualStyleBackColor = true;
			this.buttonConfigureFunction.Click += new System.EventHandler(this.buttonConfigureFunction_Click);
			// 
			// ApplicationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(435, 300);
			this.Controls.Add(this.buttonConfigureFunction);
			this.Controls.Add(this.buttonConfigureDisplayComponent);
			this.Controls.Add(this.textBoxOutputXml);
			this.Controls.Add(this.labelOutputXml);
			this.Controls.Add(this.labelTreeView);
			this.Controls.Add(this.treeViewFieldsPalette);
			this.Name = "ApplicationForm";
			this.Text = "ApplicationForm";
			this.Load += new System.EventHandler(this.ApplicationForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView treeViewFieldsPalette;
		private System.Windows.Forms.Label labelTreeView;
		private System.Windows.Forms.Label labelOutputXml;
		private System.Windows.Forms.TextBox textBoxOutputXml;
		private System.Windows.Forms.Button buttonConfigureDisplayComponent;
		private System.Windows.Forms.Button buttonConfigureFunction;

	}
}