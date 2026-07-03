// $Workfile: AppendStatementView.Designer.cs $
// $Revision: 1 $	$Date: 1/08/07 5:57p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.

namespace Tawala.Processes
{
	partial class AppendStatementView
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
			this.comboBoxDoc = new System.Windows.Forms.ComboBox();
			this.buttonAddModify = new Tawala.Processes.AddModifyButton();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageDocument = new System.Windows.Forms.TabPage();
			this.comboBoxAppend = new System.Windows.Forms.ComboBox();
			this.labelTo = new System.Windows.Forms.Label();
			this.tabControl.SuspendLayout();
			this.tabPageDocument.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBoxDoc
			// 
			this.comboBoxDoc.FormattingEnabled = true;
			this.comboBoxDoc.Location = new System.Drawing.Point(230, 8);
			this.comboBoxDoc.MaxDropDownItems = 20;
			this.comboBoxDoc.Name = "comboBoxDoc";
			this.comboBoxDoc.Size = new System.Drawing.Size(185, 21);
			this.comboBoxDoc.TabIndex = 1;
			// 
			// buttonAddModify
			// 
			this.buttonAddModify.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonAddModify.Location = new System.Drawing.Point(172, 124);
			this.buttonAddModify.Name = "buttonAddModify";
			this.buttonAddModify.Size = new System.Drawing.Size(80, 24);
			this.buttonAddModify.TabIndex = 2;
			this.buttonAddModify.Text = "Add";
			this.buttonAddModify.Click += new System.EventHandler(this.buttonAddModify_Click);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageDocument);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(432, 176);
			this.tabControl.TabIndex = 0;
			this.tabControl.TabStop = false;
			// 
			// tabPageDocument
			// 
			this.tabPageDocument.Controls.Add(this.comboBoxAppend);
			this.tabPageDocument.Controls.Add(this.labelTo);
			this.tabPageDocument.Controls.Add(this.comboBoxDoc);
			this.tabPageDocument.Controls.Add(this.buttonAddModify);
			this.tabPageDocument.Location = new System.Drawing.Point(4, 22);
			this.tabPageDocument.Name = "tabPageDocument";
			this.tabPageDocument.Size = new System.Drawing.Size(424, 150);
			this.tabPageDocument.TabIndex = 0;
			this.tabPageDocument.Text = "Documents";
			this.tabPageDocument.Layout += new System.Windows.Forms.LayoutEventHandler(this.tabPageDocument_Layout);
			// 
			// comboBoxAppend
			// 
			this.comboBoxAppend.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxAppend.FormattingEnabled = true;
			this.comboBoxAppend.Location = new System.Drawing.Point(8, 8);
			this.comboBoxAppend.MaxDropDownItems = 20;
			this.comboBoxAppend.Name = "comboBoxAppend";
			this.comboBoxAppend.Size = new System.Drawing.Size(185, 21);
			this.comboBoxAppend.TabIndex = 0;
			// 
			// labelTo
			// 
			this.labelTo.Location = new System.Drawing.Point(194, 8);
			this.labelTo.Name = "labelTo";
			this.labelTo.Size = new System.Drawing.Size(30, 21);
			this.labelTo.TabIndex = 1;
			this.labelTo.Text = "to";
			this.labelTo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// AppendDetails
			// 
			this.Controls.Add(this.tabControl);
			this.Name = "AppendStatementView";
			this.tabControl.ResumeLayout(false);
			this.tabPageDocument.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxDoc;
        private Tawala.Processes.AddModifyButton buttonAddModify;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.ComboBox comboBoxAppend;
        private System.Windows.Forms.TabPage tabPageDocument;
    }
}
