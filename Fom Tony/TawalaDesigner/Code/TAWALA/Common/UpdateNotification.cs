// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.Common
{
	public partial class UpdateNotification : Form
	{
		private string url = string.Empty;

		public UpdateNotification(string url, bool mandatory)
		{
			InitializeComponent();
			this.url = url;
			labelMandatory.Visible = mandatory;
			linkLabel.VisitedLinkColor = linkLabel.LinkColor;
		}

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs args)
		{
			Log.LogInfo("Update url = {0}", url);

			try
			{
				Process.Start(url);
			}
			catch (Exception e)
			{
				Log.LogException(e);
				throw e;
			}
			Close();
		}
	}
}