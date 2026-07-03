// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.IO;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.DesignerUI;

namespace Tawala.Designer
{
	/// <summary>
	/// Summary description for Program.
	/// </summary>
	static public class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            Application.EnableVisualStyles();

            Config.Load();

			tawalaTempDir = Config.TemporaryFiles;
			
			Log.Open();
			
			Application.EnterThreadModal += Application_EnterThreadModal;
			Application.LeaveThreadModal += Application_LeaveThreadModal;
			Log.LogInfo("Command Line = '{0}'", Environment.CommandLine);

			Functions.Runtime.RuntimeTypeResolver.Init();

			Application.DoEvents();

			var designer = new DesignerView();

			Application.Run(designer);

			Log.Close();

			deleteTempDirectory();
		}

		private static string tawalaTempDir;

		private static void deleteTempDirectory()
		{
			try
			{
				Directory.Delete(tawalaTempDir);
			}
			catch (Exception)
			{
			}
		}

		static void Application_LeaveThreadModal(object sender, EventArgs e)
		{
			Log.Flush();
		}

		static void Application_EnterThreadModal(object sender, EventArgs e)
		{
			Log.Flush();
		}
	}
}
