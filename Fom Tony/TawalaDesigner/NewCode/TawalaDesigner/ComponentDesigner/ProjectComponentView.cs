// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Tawala.ComponentDesigner
{
	public class ProjectComponentView : Form
	{
		protected static bool userClickedCloseBox(FormClosingEventArgs e)
		{
			return e.CloseReason == CloseReason.UserClosing;
		}

		protected void hideInsteadOfClosing(FormClosingEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}
	}
}
