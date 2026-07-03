// $Workfile: program.cs $
// $Revision: 7 $	$Date: 11/25/05 4:35p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Tawala.Common;

namespace Tawala.Invite
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Config.Load();

			Application.EnableVisualStyles();
			Application.Run(new MainForm());
		}
	}
}