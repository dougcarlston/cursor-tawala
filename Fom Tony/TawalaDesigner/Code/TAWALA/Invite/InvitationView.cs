// $Workfile: InvitationView.cs $
// $Revision: 4 $	$Date: 11/25/05 4:35p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Tawala.Invite
{
	internal partial class InvitationView : UserControl
	{
		private Invitation invitation = null;

		internal Invitation Invitation
		{
			get { return invitation; }
			set 
			{
				SaveChanges();
				invitation = value;
				textBoxSubject.Text = invitation.Subject;
				richTextBox.Text = invitation.Body;
			}
		}

		internal InvitationView()
		{
			InitializeComponent();
		}

		/// <summary>
		/// When one of this control's children has focus ignore command keys 
		/// for context menu shortcuts, etc.  This prevents keys like Del from being processed
		/// by the ItemContainer's menu handlers.
		/// </summary>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			// don't allow command keys to bubble up to form when the ActiveControl
			// they occur in is derived from TextBoxBase.  This allows the controls to handle the
			// keys -- primarily for cut, copy, paste, delete

			if (ActiveControl != null && ActiveControl is TextBoxBase)
			{
				return false;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			SaveChanges();
			invitation.Send();
		}

		private void panelTop_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.DarkGray, 0, panelTop.Height - 1, panelTop.Width, panelTop.Height - 1);
		}

		private void panelBottom_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.DarkGray, 0, 0, panelBottom.Width, 0);
		}

		private void invitationView_Load(object sender, EventArgs e)
		{
			Application.Idle += new EventHandler(application_Idle);
		}

		private void application_Idle(object sender, EventArgs e)
		{
			if (textBoxSubject.TextLength > 0)
			{
				buttonSend.Enabled = true;
			}
			else
			{
				buttonSend.Enabled = false;
			}
		}

		public void SaveChanges()
		{
			if (invitation != null)
			{
				invitation.Body = richTextBox.Text;
				invitation.Subject = textBoxSubject.Text;
			}
		}
	}
}
