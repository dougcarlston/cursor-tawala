using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TawalaDesigner
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            Tawala.Common.Config.Load();
            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new ApplicationView());
		}
	}
}
