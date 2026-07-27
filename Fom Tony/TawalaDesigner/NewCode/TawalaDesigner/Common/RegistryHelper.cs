// $Workfile: RegistryHelper.cs $
// $Revision: 1 $	$Date: 4/03/06 10:55a $
// Copyright © 2005 - 2006 Tawala Systems, Inc. All rights reserved.

using Microsoft.Win32;

namespace Tawala.Common
{
	/// <summary>
	/// Windows Registry access
	/// </summary>
	public static class RegistryHelper
	{
		/// <summary>
		/// Returns the name of the executable for the system's default browser
		/// </summary>
		/// <remarks>
		/// Thanks to Ryan Farley: http://ryanfarley.com/blog/archive/2004/05/16/649.aspx
		/// </remarks>
		public static string GetDefaultBrowser()
		{
			string browser = string.Empty;
			RegistryKey key = null;
			try
			{
				key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

				//trim off quotes
				browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");
				if (!browser.EndsWith("exe"))
				{
					//get rid of everything after the ".exe"
					browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
				}
			}
			finally
			{
				if (key != null)
				{
					key.Close();
				}
			}

			return browser;
		}
	}
}
