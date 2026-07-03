// $Workfile: AboutHelper.cs $
// $Revision: 5 $	$Date: 5/29/07 11:46a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Management;
using System.Reflection;

namespace TawalaDesigner.Dialogs
{
	/// <summary>
	/// Get information typically used for populating an About box for
	/// the Designer and the Invitation Manager.
	/// </summary>
	static public class AboutHelper
	{
		static public readonly string OsNameAndServicePack = string.Empty;
		static public readonly string FrameworkVersion = string.Empty;
		static public readonly string AppDirectory = string.Empty;

		static public readonly string[] Assemblies = null;

		static public readonly bool DebugBuild = false;

		static AboutHelper()
		{
			#if DEBUG
			DebugBuild = true;
			#endif

			Assembly entryAssembly = Assembly.GetEntryAssembly();

			AppDirectory = Path.GetDirectoryName(entryAssembly.Location);


			// Use WMI to get OS Name name and service pack.  Normally wouldn't go to this extreme but...
			// If you use the .NET OperatingSystem class' VersionString property what you will 
			// get for Windows XP Service Pack 1, for example, would be something like this:
			//		"Microsoft Windows NT 5.1.2600.0 Service Pack 1"
			// Windows XP is also know has Windows NT 5.1 (and Windows 2000 is Windows NT 5.0)
			// By using WMI we get the name that most people expect, instead of the esoteric name.

			try
			{
				using (ManagementClass mc = new ManagementClass("Win32_OperatingSystem"))
				{
					using (ManagementObjectCollection moc = mc.GetInstances())
					{
						Debug.Assert(moc.Count == 1); // only one object of this type ever returned

						foreach (ManagementObject os in moc)
						{
							OsNameAndServicePack = os["Caption"].ToString();
							if (os["CSDVersion"] != null)
							{
								OsNameAndServicePack += " " + os["CSDVersion"].ToString();
							}
							os.Dispose();
						}
					}
				}
			}
			catch (Exception)
			{
			}

			FrameworkVersion = Environment.Version.ToString();

			SortedList list = new SortedList();

			recurseAssemblies(entryAssembly, list);

			Assemblies = new string[list.Keys.Count+1];

			Assemblies[0] = GetAssemblyInfo(entryAssembly);

			int index = 1;
			foreach (DictionaryEntry de in list)
			{
				Assemblies[index++] = GetAssemblyInfo(de.Value as Assembly);
			}
		}

		public static string FreePhysicalMemory
		{
			get
			{
				string result = "error";

				try
				{
					using (ManagementClass mc = new ManagementClass("Win32_OperatingSystem"))
					{
						using (ManagementObjectCollection moc = mc.GetInstances())
						{
							Debug.Assert(moc.Count == 1); // only one object of this type ever returned

							foreach (ManagementObject os in moc)
							{
								if (os["FreePhysicalMemory"] != null)
								{
									object o = os["FreePhysicalMemory"];
									long memoryInKB = Convert.ToInt64(o);
									memoryInKB /= 1024L;
									result = memoryInKB.ToString() + " MB";
								}

								os.Dispose();
							}
						}
					}
				}
				catch (Exception)
				{
				}

				return result;
			}
		}

		static private string GetAssemblyInfo(Assembly a)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0,-34} {1}  {2}     .NET {3}", Path.GetFileName(a.Location), 
                a.GetName().Version, 
                a.GetName().ProcessorArchitecture.ToString(), 
                a.ImageRuntimeVersion.ToString());
			return sb.ToString();
		}

		static private void recurseAssemblies(Assembly assembly, SortedList list)
		{
			AssemblyName[] refs = assembly.GetReferencedAssemblies();
			foreach (AssemblyName an in refs)
			{
				// don't recurse system assemblies (doesn't catch all but most)
				if (an.Name.StartsWith("System") || an.Name.StartsWith("mscorlib"))
					continue;

				Assembly a = Assembly.Load(an);
				if (a != null)
				{
					if (!Path.GetDirectoryName(a.Location).StartsWith(AppDirectory))
						continue;

					if (!list.Contains(a.GetName().Name))
					{
						list[a.GetName().Name] = a;
						recurseAssemblies(a, list);
					}
				}
			}
		}
	}
}
