// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.Common
{
	public partial class LoginForm : Form
	{
		public LoginForm()
		{
			InitializeComponent();
		}

		public string UserName
		{
			get
			{
				return textBoxUserName.Text;
			}
			set
			{
				textBoxUserName.Text = value;
			}
		}

		public string Password
		{
			get
			{
				return textBoxPassword.Text;
			}
			set
			{
				textBoxPassword.Text = value;
			}
		}
	}
}