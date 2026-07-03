// $Workfile: SkipToStatementView.Designer.cs $
// $Revision: 1 $	$Date: 1/09/07 8:39a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
namespace Tawala.Processes
{
	partial class SkipToStatementView
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
			this.comboBoxDestination = new System.Windows.Forms.ComboBox();
			this.buttonAddModify = new Tawala.Processes.AddModifyButton();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageSkipTo = new System.Windows.Forms.TabPage();
			this.tabControl.SuspendLayout();
			this.tabPageSkipTo.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBoxDestination
			// 
			this.comboBoxDestination.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDestination.FormattingEnabled = true;
			this.comboBoxDestination.Location = new System.Drawing.Point(8, 8);
			this.comboBoxDestination.MaxDropDownItems = 20;
			this.comboBoxDestination.Name = "comboBoxDestination";
			this.comboBoxDestination.Size = new System.Drawing.Size(192, 21);
			this.comboBoxDestination.TabIndex = 0;
			// 
			// buttonAddModify
			// 
			this.buttonAddModify.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonAddModify.Location = new System.Drawing.Point(172, 124);
			this.buttonAddModify.Name = "buttonAddModify";
			this.buttonAddModify.Size = new System.Drawing.Size(80, 24);
			this.buttonAddModify.TabIndex = 1;
			this.buttonAddModify.Text = "Add";
			this.buttonAddModify.Click += new System.EventHandler(this.buttonAddModify_Click);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageSkipTo);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(432, 176);
			this.tabControl.TabIndex = 0;
			// 
			// tabPageSkipTo
			// 
			this.tabPageSkipTo.Controls.Add(this.comboBoxDestination);
			this.tabPageSkipTo.Controls.Add(this.buttonAddModify);
			this.tabPageSkipTo.Location = new System.Drawing.Point(4, 22);
			this.tabPageSkipTo.Name = "tabPageSkipTo";
			this.tabPageSkipTo.Size = new System.Drawing.Size(424, 150);
			this.tabPageSkipTo.TabIndex = 0;
			this.tabPageSkipTo.Text = "Skip To";
			// 
			// SkipToDetails
			// 
			this.Controls.Add(this.tabControl);
			this.Name = "SkipToDetails";
			this.tabControl.ResumeLayout(false);
			this.tabPageSkipTo.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

        private System.Windows.Forms.ComboBox comboBoxDestination;
        private Tawala.Processes.AddModifyButton buttonAddModify;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageSkipTo;
	}
}
