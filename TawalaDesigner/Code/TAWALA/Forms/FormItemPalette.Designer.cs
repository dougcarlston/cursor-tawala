// $Workfile: FormItemPalette.Designer.cs $
// $Revision: 15 $	$Date: 11/05/07 1:43p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Forms
{
	partial class FormItemPalette 
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
            this.components = new System.ComponentModel.Container();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.labelHeadingItem = new System.Windows.Forms.Label();
            this.labelTextItem = new System.Windows.Forms.Label();
            this.labelBlankItem = new System.Windows.Forms.Label();
            this.labelChoiceItem = new System.Windows.Forms.Label();
            this.labelFileUploadItem = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelHiddenFieldItem = new System.Windows.Forms.Label();
            this.labelBreakItem = new System.Windows.Forms.Label();
            this.labelSkipItem = new System.Windows.Forms.Label();
            this.gradientLabel = new Tawala.Common.GradientLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.AutoSize = true;
            this.flowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(227)))), ((int)(((byte)(242)))));
            this.flowLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel.Controls.Add(this.labelHeadingItem);
            this.flowLayoutPanel.Controls.Add(this.labelTextItem);
            this.flowLayoutPanel.Controls.Add(this.labelBlankItem);
            this.flowLayoutPanel.Controls.Add(this.labelChoiceItem);
            this.flowLayoutPanel.Controls.Add(this.labelFileUploadItem);
            this.flowLayoutPanel.Controls.Add(this.panel1);
            this.flowLayoutPanel.Controls.Add(this.labelHiddenFieldItem);
            this.flowLayoutPanel.Controls.Add(this.labelBreakItem);
            this.flowLayoutPanel.Controls.Add(this.labelSkipItem);
            this.flowLayoutPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 20);
            this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.flowLayoutPanel.Size = new System.Drawing.Size(91, 562);
            this.flowLayoutPanel.TabIndex = 1;
            this.flowLayoutPanel.WrapContents = false;
            // 
            // labelHeadingItem
            // 
            this.labelHeadingItem.AutoSize = true;
            this.labelHeadingItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(199)))), ((int)(((byte)(231)))));
            this.labelHeadingItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelHeadingItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelHeadingItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.SetFlowBreak(this.labelHeadingItem, true);
            this.labelHeadingItem.Image = global::Tawala.Forms.Properties.Resources.Form_ItemHeading;
            this.labelHeadingItem.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelHeadingItem.Location = new System.Drawing.Point(4, 6);
            this.labelHeadingItem.Margin = new System.Windows.Forms.Padding(0, 6, 0, 8);
            this.labelHeadingItem.MaximumSize = new System.Drawing.Size(80, 200);
            this.labelHeadingItem.MinimumSize = new System.Drawing.Size(80, 60);
            this.labelHeadingItem.Name = "labelHeadingItem";
            this.labelHeadingItem.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.labelHeadingItem.Size = new System.Drawing.Size(80, 60);
            this.labelHeadingItem.TabIndex = 1;
            this.labelHeadingItem.Text = "Heading";
            this.labelHeadingItem.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip.SetToolTip(this.labelHeadingItem, "Add a heading item to the form to highlight sections of the form");
            this.labelHeadingItem.DoubleClick += new System.EventHandler(this.label_DoubleClick);
            this.labelHeadingItem.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_MouseMove);
            this.labelHeadingItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_MouseDown);
            this.labelHeadingItem.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_MouseUp);
            // 
            // labelTextItem
            // 
            this.labelTextItem.AutoSize = true;
            this.labelTextItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(199)))), ((int)(((byte)(231)))));
            this.labelTextItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelTextItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelTextItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.SetFlowBreak(this.labelTextItem, true);
            this.labelTextItem.Image = global::Tawala.Forms.Properties.Resources.Form_ItemText;
            this.labelTextItem.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelTextItem.Location = new System.Drawing.Point(4, 74);
            this.labelTextItem.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.labelTextItem.MaximumSize = new System.Drawing.Size(80, 200);
            this.labelTextItem.MinimumSize = new System.Drawing.Size(80, 60);
            this.labelTextItem.Name = "labelTextItem";
            this.labelTextItem.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.labelTextItem.Size = new System.Drawing.Size(80, 60);
            this.labelTextItem.TabIndex = 2;
            this.labelTextItem.Text = "Text";
            this.labelTextItem.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip.SetToolTip(this.labelTextItem, "Add a text item for displaying formatted text");
            this.labelTextItem.DoubleClick += new System.EventHandler(this.label_DoubleClick);
            this.labelTextItem.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_MouseMove);
            this.labelTextItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_MouseDown);
            this.labelTextItem.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_MouseUp);
            // 
            // labelBlankItem
            // 
            this.labelBlankItem.AutoSize = true;
            this.labelBlankItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(199)))), ((int)(((byte)(231)))));
            this.labelBlankItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelBlankItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelBlankItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.SetFlowBreak(this.labelBlankItem, true);
            this.labelBlankItem.Image = global::Tawala.Forms.Properties.Resources.Form_ItemFib;
            this.labelBlankItem.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelBlankItem.Location = new System.Drawing.Point(4, 142);
            this.labelBlankItem.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.labelBlankItem.MaximumSize = new System.Drawing.Size(80, 200);
            this.labelBlankItem.MinimumSize = new System.Drawing.Size(80, 60);
            this.labelBlankItem.Name = "labelBlankItem";
            this.labelBlankItem.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.labelBlankItem.Size = new System.Drawing.Size(80, 60);
            this.labelBlankItem.TabIndex = 3;
            this.labelBlankItem.Text = "Fill in the Blank";
            this.labelBlankItem.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip.SetToolTip(this.labelBlankItem, "Add question with one or more blanks");
            this.labelBlankItem.DoubleClick += new System.EventHandler(this.label_DoubleClick);
            this.labelBlankItem.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_MouseMove);
            this.labelBlankItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_MouseDown);
            this.labelBlankItem.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_MouseUp);
            // 
            // labelChoiceItem
            // 
            this.labelChoiceItem.AutoSize = true;
            this.labelChoiceItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(199)))), ((int)(((byte)(231)))));
            this.labelChoiceItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelChoiceItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelChoiceItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.SetFlowBreak(this.labelChoiceItem, true);
            this.labelChoiceItem.Image = global::Tawala.Forms.Properties.Resources.Form_ItemMcq;
            this.labelChoiceItem.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelChoiceItem.Location = new System.Drawing.Point(4, 210);
            this.labelChoiceItem.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.labelChoiceItem.MaximumSize = new System.Drawing.Size(80, 200);
            this.labelChoiceItem.MinimumSize = new System.Drawing.Size(80, 60);
            this.labelChoiceItem.Name = "labelChoiceItem";
            this.labelChoiceItem.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.labelChoiceItem.Size = new System.Drawing.Size(80, 60);
            this.labelChoiceItem.TabIndex = 4;
            this.labelChoiceItem.Text = "Multiple Choice";
            this.labelChoiceItem.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip.SetToolTip(this.labelChoiceItem, "Add single or multiple choice question");
            this.labelChoiceItem.DoubleClick += new System.EventHandler(this.label_DoubleClick);
            this.labelChoiceItem.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_MouseMove);
            this.labelChoiceItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_MouseDown);
            this.labelChoiceItem.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_MouseUp);
            // 
            // labelFileUploadItem
            // 
            this.labelFileUploadItem.AutoSize = true;
            this.labelFileUploadItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(199)))), ((int)(((byte)(231)))));
            this.labelFileUploadItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelFileUploadItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelFileUploadItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.SetFlowBreak(this.labelFileUploadItem, true);
            this.labelFileUploadItem.Image = global::Tawala.Forms.Properties.Resources.UploadToWeb;
            this.labelFileUploadItem.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelFileUploadItem.Location = new System.Drawing.Point(4, 278);
            this.labelFileUploadItem.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.labelFileUploadItem.MaximumSize = new System.Drawing.Size(80, 200);
            this.labelFileUploadItem.MinimumSize = new System.Drawing.Size(80, 60);
            this.labelFileUploadItem.Name = "labelFileUploadItem";
            this.labelFileUploadItem.Padding = new System.Windows.Forms.Padding(6, 0, 6, 2);
            this.labelFileUploadItem.Size = new System.Drawing.Size(80, 60);
            this.labelFileUploadItem.TabIndex = 8;
            this.labelFileUploadItem.Text = "File Uploader";
            this.labelFileUploadItem.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip.SetToolTip(this.labelFileUploadItem, "Add a form item that requests a file upload from the user and includes room for a" +
                    " paragraph of instructions.");
            this.labelFileUploadItem.DoubleClick += new System.EventHandler(this.label_DoubleClick);
            this.labelFileUploadItem.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_MouseMove);
            this.labelFileUploadItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_MouseDown);
            this.labelFileUploadItem.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_MouseUp);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(4, 346);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.panel1.MinimumSize = new System.Drawing.Size(70, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(80, 2);
            this.panel1.TabIndex = 7;
            // 
            // labelHiddenFieldItem
            // 
            this.labelHiddenFieldItem.AutoSize = true;
            this.labelHiddenFieldItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(199)))), ((int)(((byte)(231)))));
            this.labelHiddenFieldItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelHiddenFieldItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelHiddenFieldItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.SetFlowBreak(this.labelHiddenFieldItem, true);
            this.labelHiddenFieldItem.Image = global::Tawala.Forms.Properties.Resources.Form_ItemHiddenField;
            this.labelHiddenFieldItem.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelHiddenFieldItem.Location = new System.Drawing.Point(4, 356);
            this.labelHiddenFieldItem.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.labelHiddenFieldItem.MaximumSize = new System.Drawing.Size(80, 200);
            this.labelHiddenFieldItem.MinimumSize = new System.Drawing.Size(80, 60);
            this.labelHiddenFieldItem.Name = "labelHiddenFieldItem";
            this.labelHiddenFieldItem.Padding = new System.Windows.Forms.Padding(4, 0, 4, 2);
            this.labelHiddenFieldItem.Size = new System.Drawing.Size(80, 60);
            this.labelHiddenFieldItem.TabIndex = 5;
            this.labelHiddenFieldItem.Text = "Hidden Field";
            this.labelHiddenFieldItem.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip.SetToolTip(this.labelHiddenFieldItem, "Add hidden field to store additional data associated with the form");
            this.labelHiddenFieldItem.DoubleClick += new System.EventHandler(this.label_DoubleClick);
            this.labelHiddenFieldItem.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_MouseMove);
            this.labelHiddenFieldItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_MouseDown);
            this.labelHiddenFieldItem.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_MouseUp);
            // 
            // labelBreakItem
            // 
            this.labelBreakItem.AutoSize = true;
            this.labelBreakItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(199)))), ((int)(((byte)(231)))));
            this.labelBreakItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelBreakItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelBreakItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.SetFlowBreak(this.labelBreakItem, true);
            this.labelBreakItem.Image = global::Tawala.Forms.Properties.Resources.Form_ItemBreak;
            this.labelBreakItem.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelBreakItem.Location = new System.Drawing.Point(4, 424);
            this.labelBreakItem.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.labelBreakItem.MaximumSize = new System.Drawing.Size(80, 200);
            this.labelBreakItem.MinimumSize = new System.Drawing.Size(80, 60);
            this.labelBreakItem.Name = "labelBreakItem";
            this.labelBreakItem.Padding = new System.Windows.Forms.Padding(6, 0, 6, 2);
            this.labelBreakItem.Size = new System.Drawing.Size(80, 60);
            this.labelBreakItem.TabIndex = 6;
            this.labelBreakItem.Text = "Page Break";
            this.labelBreakItem.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip.SetToolTip(this.labelBreakItem, "Add a page break which causes a submit button to appear");
            this.labelBreakItem.DoubleClick += new System.EventHandler(this.label_DoubleClick);
            this.labelBreakItem.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_MouseMove);
            this.labelBreakItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_MouseDown);
            this.labelBreakItem.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_MouseUp);
            // 
            // labelSkipItem
            // 
            this.labelSkipItem.AutoSize = true;
            this.labelSkipItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(199)))), ((int)(((byte)(231)))));
            this.labelSkipItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelSkipItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelSkipItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.SetFlowBreak(this.labelSkipItem, true);
            this.labelSkipItem.Image = global::Tawala.Forms.Properties.Resources.Form_ItemSkip;
            this.labelSkipItem.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelSkipItem.Location = new System.Drawing.Point(4, 492);
            this.labelSkipItem.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.labelSkipItem.MaximumSize = new System.Drawing.Size(80, 200);
            this.labelSkipItem.MinimumSize = new System.Drawing.Size(80, 60);
            this.labelSkipItem.Name = "labelSkipItem";
            this.labelSkipItem.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.labelSkipItem.Size = new System.Drawing.Size(80, 60);
            this.labelSkipItem.TabIndex = 7;
            this.labelSkipItem.Text = "Skip Instructions";
            this.labelSkipItem.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip.SetToolTip(this.labelSkipItem, "Add processing instructions to skip over form items");
            this.labelSkipItem.DoubleClick += new System.EventHandler(this.label_DoubleClick);
            this.labelSkipItem.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_MouseMove);
            this.labelSkipItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_MouseDown);
            this.labelSkipItem.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_MouseUp);
            // 
            // gradientLabel
            // 
            this.gradientLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.gradientLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.gradientLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gradientLabel.ForeColor = System.Drawing.Color.White;
            this.gradientLabel.Location = new System.Drawing.Point(0, 0);
            this.gradientLabel.Margin = new System.Windows.Forms.Padding(0);
            this.gradientLabel.Name = "gradientLabel";
            this.gradientLabel.Size = new System.Drawing.Size(91, 20);
            this.gradientLabel.TabIndex = 0;
            this.gradientLabel.Text = "Items";
            this.gradientLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolTip
            // 
            this.toolTip.IsBalloon = true;
            // 
            // FormItemPalette
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(206)))), ((int)(((byte)(248)))));
            this.Controls.Add(this.flowLayoutPanel);
            this.Controls.Add(this.gradientLabel);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(92, 0);
            this.Name = "FormItemPalette";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.Size = new System.Drawing.Size(92, 582);
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
		private Tawala.Common.GradientLabel gradientLabel;
		private System.Windows.Forms.Label labelTextItem;
		private System.Windows.Forms.Label labelBlankItem;
		private System.Windows.Forms.Label labelChoiceItem;
		private System.Windows.Forms.Label labelBreakItem;
		private System.Windows.Forms.Label labelSkipItem;
		private System.Windows.Forms.Label labelHiddenFieldItem;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labelHeadingItem;
        private System.Windows.Forms.Label labelFileUploadItem;
	}
}
