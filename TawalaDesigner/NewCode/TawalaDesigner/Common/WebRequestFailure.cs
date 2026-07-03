// $Workfile: WebRequestFailure.cs $
// $Revision: 6 $	$Date: 8/13/07 11:29a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.Common
{
	public partial class WebRequestFailure : Form
	{
		public WebRequestFailure()
		{
			InitializeComponent();
		}

		public string ErrorMessage
		{
			set
			{
				labelErrorMessage.Text = value;
			}
		}

	}
}