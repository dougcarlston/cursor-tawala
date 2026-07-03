// $Workfile: SendStatementView.Designer.cs $
// $Revision: 6 $	$Date: 6/01/07 5:10p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using Tawala.Projects;

namespace Tawala.Processes
{
	partial class SendStatementView
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
			this.tabPageEmail = new System.Windows.Forms.TabPage();
			this.labelEmailFromAlias = new System.Windows.Forms.Label();
			this.textBoxEmailFromAlias = new Tawala.Controls.FieldTextBox();
			this.labelEmailFromAddress = new System.Windows.Forms.Label();
			this.textBoxEmailFromAddress = new Tawala.Controls.FieldTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkBoxReset = new System.Windows.Forms.CheckBox();
			this.textBoxEmailRecipient = new Tawala.Controls.FieldTextBox();
			this.buttonAddModifyDoc = new Tawala.Processes.AddModifyButton();
			this.comboBoxEmailDoc = new System.Windows.Forms.ComboBox();
			this.labelEmailDoc = new System.Windows.Forms.Label();
			this.labelEmailCC = new System.Windows.Forms.Label();
			this.textBoxEmailCC = new Tawala.Controls.FieldTextBox();
			this.textBoxEmailSubject = new Tawala.Controls.ComplexExpressionTextBox();
			this.labelEmailRecipient = new System.Windows.Forms.Label();
			this.labelEmailSubject = new System.Windows.Forms.Label();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.checkBoxShowPageHeader = new System.Windows.Forms.CheckBox();
			this.tabPageEmail.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabPageEmail
			// 
			this.tabPageEmail.Controls.Add(this.checkBoxShowPageHeader);
			this.tabPageEmail.Controls.Add(this.labelEmailFromAlias);
			this.tabPageEmail.Controls.Add(this.textBoxEmailFromAlias);
			this.tabPageEmail.Controls.Add(this.labelEmailFromAddress);
			this.tabPageEmail.Controls.Add(this.textBoxEmailFromAddress);
			this.tabPageEmail.Controls.Add(this.label1);
			this.tabPageEmail.Controls.Add(this.checkBoxReset);
			this.tabPageEmail.Controls.Add(this.textBoxEmailRecipient);
			this.tabPageEmail.Controls.Add(this.buttonAddModifyDoc);
			this.tabPageEmail.Controls.Add(this.comboBoxEmailDoc);
			this.tabPageEmail.Controls.Add(this.labelEmailDoc);
			this.tabPageEmail.Controls.Add(this.labelEmailCC);
			this.tabPageEmail.Controls.Add(this.textBoxEmailCC);
			this.tabPageEmail.Controls.Add(this.textBoxEmailSubject);
			this.tabPageEmail.Controls.Add(this.labelEmailRecipient);
			this.tabPageEmail.Controls.Add(this.labelEmailSubject);
			this.tabPageEmail.Location = new System.Drawing.Point(4, 22);
			this.tabPageEmail.Margin = new System.Windows.Forms.Padding(0);
			this.tabPageEmail.Name = "tabPageEmail";
			this.tabPageEmail.Size = new System.Drawing.Size(424, 194);
			this.tabPageEmail.TabIndex = 1;
			this.tabPageEmail.Text = "Email";
			this.tabPageEmail.UseVisualStyleBackColor = true;
			this.tabPageEmail.Layout += new System.Windows.Forms.LayoutEventHandler(this.tabPageDocument_Layout);
			// 
			// labelEmailFromAlias
			// 
			this.labelEmailFromAlias.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.labelEmailFromAlias.Location = new System.Drawing.Point(219, 38);
			this.labelEmailFromAlias.Name = "labelEmailFromAlias";
			this.labelEmailFromAlias.Size = new System.Drawing.Size(45, 13);
			this.labelEmailFromAlias.TabIndex = 14;
			this.labelEmailFromAlias.Text = "(Name):";
			this.labelEmailFromAlias.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxEmailFromAlias
			// 
			this.textBoxEmailFromAlias.AllowDrop = true;
			this.textBoxEmailFromAlias.Location = new System.Drawing.Point(270, 34);
			this.textBoxEmailFromAlias.Name = "textBoxEmailFromAlias";
			this.textBoxEmailFromAlias.Size = new System.Drawing.Size(146, 20);
			this.textBoxEmailFromAlias.TabIndex = 3;
			this.textBoxEmailFromAlias.TextChanged += new System.EventHandler(this.textBoxEmailFromAlias_TextChanged);
			this.textBoxEmailFromAlias.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxEmailFromAlias_DragDrop);
			this.textBoxEmailFromAlias.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxEmailFromAlias_KeyPress);
			this.textBoxEmailFromAlias.Enter += new System.EventHandler(this.textBoxEmailFromAlias_Enter);
			this.textBoxEmailFromAlias.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxEmailFromAlias_DragEnter);
			// 
			// labelEmailFromAddress
			// 
			this.labelEmailFromAddress.Location = new System.Drawing.Point(3, 37);
			this.labelEmailFromAddress.Name = "labelEmailFromAddress";
			this.labelEmailFromAddress.Size = new System.Drawing.Size(89, 13);
			this.labelEmailFromAddress.TabIndex = 12;
			this.labelEmailFromAddress.Text = "From (Address):";
			this.labelEmailFromAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxEmailFromAddress
			// 
			this.textBoxEmailFromAddress.AllowDrop = true;
			this.textBoxEmailFromAddress.Location = new System.Drawing.Point(98, 34);
			this.textBoxEmailFromAddress.Name = "textBoxEmailFromAddress";
			this.textBoxEmailFromAddress.Size = new System.Drawing.Size(100, 20);
			this.textBoxEmailFromAddress.TabIndex = 2;
			this.textBoxEmailFromAddress.TextChanged += new System.EventHandler(this.textBoxEmailFromAddress_TextChanged);
			this.textBoxEmailFromAddress.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxEmailFromAddress_DragDrop);
			this.textBoxEmailFromAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxEmailFromAddress_KeyPress);
			this.textBoxEmailFromAddress.Enter += new System.EventHandler(this.textBoxEmailFromAddress_Enter);
			this.textBoxEmailFromAddress.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxEmailFromAddress_DragEnter);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(216, 131);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(179, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "(removes any appended documents)";
			// 
			// checkBoxReset
			// 
			this.checkBoxReset.AutoSize = true;
			this.checkBoxReset.Location = new System.Drawing.Point(201, 115);
			this.checkBoxReset.Name = "checkBoxReset";
			this.checkBoxReset.Size = new System.Drawing.Size(230, 17);
			this.checkBoxReset.TabIndex = 6;
			this.checkBoxReset.Text = "Reset document to original state after Send";
			this.checkBoxReset.UseVisualStyleBackColor = true;
			// 
			// textBoxEmailRecipient
			// 
			this.textBoxEmailRecipient.AllowDrop = true;
			this.textBoxEmailRecipient.Location = new System.Drawing.Point(98, 9);
			this.textBoxEmailRecipient.Name = "textBoxEmailRecipient";
			this.textBoxEmailRecipient.Size = new System.Drawing.Size(100, 20);
			this.textBoxEmailRecipient.TabIndex = 0;
			this.textBoxEmailRecipient.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxEmailRecipient_DragDrop);
			this.textBoxEmailRecipient.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxEmailRecipient_KeyPress);
			this.textBoxEmailRecipient.Enter += new System.EventHandler(this.textBoxEmailRecipient_Enter);
			this.textBoxEmailRecipient.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxEmailRecipient_DragEnter);
			// 
			// buttonAddModifyDoc
			// 
			this.buttonAddModifyDoc.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonAddModifyDoc.Location = new System.Drawing.Point(172, 166);
			this.buttonAddModifyDoc.Name = "buttonAddModifyDoc";
			this.buttonAddModifyDoc.Size = new System.Drawing.Size(80, 24);
			this.buttonAddModifyDoc.TabIndex = 7;
			this.buttonAddModifyDoc.Text = "Add";
			this.buttonAddModifyDoc.Click += new System.EventHandler(this.buttonAddModifyDoc_Click);
			// 
			// comboBoxEmailDoc
			// 
			this.comboBoxEmailDoc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxEmailDoc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxEmailDoc.FormattingEnabled = true;
			this.comboBoxEmailDoc.ItemHeight = 13;
			this.comboBoxEmailDoc.Location = new System.Drawing.Point(201, 88);
			this.comboBoxEmailDoc.MaxDropDownItems = 20;
			this.comboBoxEmailDoc.Name = "comboBoxEmailDoc";
			this.comboBoxEmailDoc.Size = new System.Drawing.Size(215, 21);
			this.comboBoxEmailDoc.TabIndex = 5;
			// 
			// labelEmailDoc
			// 
			this.labelEmailDoc.Location = new System.Drawing.Point(4, 88);
			this.labelEmailDoc.Name = "labelEmailDoc";
			this.labelEmailDoc.Size = new System.Drawing.Size(191, 21);
			this.labelEmailDoc.TabIndex = 7;
			this.labelEmailDoc.Text = "Document to be used as Body text:";
			this.labelEmailDoc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelEmailCC
			// 
			this.labelEmailCC.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.labelEmailCC.Location = new System.Drawing.Point(219, 11);
			this.labelEmailCC.Name = "labelEmailCC";
			this.labelEmailCC.Size = new System.Drawing.Size(45, 13);
			this.labelEmailCC.TabIndex = 8;
			this.labelEmailCC.Text = "Cc:";
			this.labelEmailCC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxEmailCC
			// 
			this.textBoxEmailCC.AllowDrop = true;
			this.textBoxEmailCC.Location = new System.Drawing.Point(270, 8);
			this.textBoxEmailCC.Name = "textBoxEmailCC";
			this.textBoxEmailCC.Size = new System.Drawing.Size(146, 20);
			this.textBoxEmailCC.TabIndex = 1;
			this.textBoxEmailCC.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxEmailCC_DragDrop);
			this.textBoxEmailCC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxEmailCC_KeyPress);
			this.textBoxEmailCC.Enter += new System.EventHandler(this.textBoxEmailCC_Enter);
			this.textBoxEmailCC.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxEmailCC_DragEnter);
			// 
			// textBoxEmailSubject
			// 
			this.textBoxEmailSubject.AllowDrop = true;
			this.textBoxEmailSubject.Location = new System.Drawing.Point(98, 59);
			this.textBoxEmailSubject.Name = "textBoxEmailSubject";
			this.textBoxEmailSubject.Size = new System.Drawing.Size(318, 20);
			this.textBoxEmailSubject.TabIndex = 4;
			this.textBoxEmailSubject.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxEmailSubject_DragDrop);
			this.textBoxEmailSubject.Enter += new System.EventHandler(this.textBoxEmailSubject_Enter);
			this.textBoxEmailSubject.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxEmailSubject_DragEnter);
			// 
			// labelEmailRecipient
			// 
			this.labelEmailRecipient.Location = new System.Drawing.Point(3, 12);
			this.labelEmailRecipient.Name = "labelEmailRecipient";
			this.labelEmailRecipient.Size = new System.Drawing.Size(89, 13);
			this.labelEmailRecipient.TabIndex = 5;
			this.labelEmailRecipient.Text = "To:";
			this.labelEmailRecipient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelEmailSubject
			// 
			this.labelEmailSubject.AutoSize = true;
			this.labelEmailSubject.Location = new System.Drawing.Point(46, 62);
			this.labelEmailSubject.Name = "labelEmailSubject";
			this.labelEmailSubject.Size = new System.Drawing.Size(46, 13);
			this.labelEmailSubject.TabIndex = 6;
			this.labelEmailSubject.Text = "Subject:";
			this.labelEmailSubject.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageEmail);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Margin = new System.Windows.Forms.Padding(0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(432, 220);
			this.tabControl.TabIndex = 0;
			this.tabControl.TabStop = false;
			// 
			// checkBoxShowPageHeader
			// 
			this.checkBoxShowPageHeader.AutoSize = true;
			this.checkBoxShowPageHeader.Location = new System.Drawing.Point(26, 113);
			this.checkBoxShowPageHeader.Name = "checkBoxShowPageHeader";
			this.checkBoxShowPageHeader.Size = new System.Drawing.Size(127, 17);
			this.checkBoxShowPageHeader.TabIndex = 15;
			this.checkBoxShowPageHeader.Text = "Include Page Header";
			this.checkBoxShowPageHeader.UseVisualStyleBackColor = true;
			// 
			// SendStatementView
			// 
			this.Controls.Add(this.tabControl);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "SendStatementView";
			this.Size = new System.Drawing.Size(432, 220);
			this.tabPageEmail.ResumeLayout(false);
			this.tabPageEmail.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private TabPage tabPageEmail;
		private Label label1;
		private CheckBox checkBoxReset;
		private Tawala.Controls.FieldTextBox textBoxEmailRecipient;
		private AddModifyButton buttonAddModifyDoc;
		private ComboBox comboBoxEmailDoc;
		private Label labelEmailDoc;
		private Label labelEmailCC;
		private Tawala.Controls.FieldTextBox textBoxEmailCC;
		private Tawala.Controls.ComplexExpressionTextBox textBoxEmailSubject;
		private Label labelEmailRecipient;
		private Label labelEmailSubject;
		private TabControl tabControl;
		private Tawala.Controls.FieldTextBox textBoxEmailFromAddress;
		private Label labelEmailFromAddress;
		private Tawala.Controls.FieldTextBox textBoxEmailFromAlias;
		private Label labelEmailFromAlias;
		private CheckBox checkBoxShowPageHeader;


	}
}
