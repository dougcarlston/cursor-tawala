// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.Text;

namespace Tawala.DesignerUI
{
    /// <summary>
    /// Get information typically used for populating an About box for
    /// the Designer and the Invitation Manager.
    /// </summary>
    public static class AboutHelper
    {
        public static readonly string AppDirectory = string.Empty;

        public static readonly string[] Assemblies;

        public static readonly bool DebugBuild;
        public static readonly string FrameworkVersion = string.Empty;
        public static readonly string OsNameAndServicePack = string.Empty;

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
                using (var mc = new ManagementClass("Win32_OperatingSystem"))
                {
                    using (ManagementObjectCollection moc = mc.GetInstances())
                    {
                        Debug.Assert(moc.Count == 1); // only one object of this type ever returned

                        foreach (ManagementObject os in moc)
                        {
                            OsNameAndServicePack = os["Caption"].ToString();
                            if (os["CSDVersion"] != null)
                            {
                                OsNameAndServicePack += " " + os["CSDVersion"];
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

            var list = new SortedList();

            recurseAssemblies(entryAssembly, list);

            Assemblies = new string[list.Keys.Count + 1];

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
                    using (var mc = new ManagementClass("Win32_OperatingSystem"))
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
                                    result = memoryInKB + " MB";
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

        private static string GetAssemblyInfo(Assembly a)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0,-48} {1}", Path.GetFileName(a.Location), a.GetName().Version);
            return sb.ToString();
        }

        private static void recurseAssemblies(Assembly assembly, SortedList list)
        {
            AssemblyName[] refs = assembly.GetReferencedAssemblies();
            foreach (AssemblyName an in refs)
            {
                // don't recurse system assemblies (doesn't catch all but most)
                if (an.Name.StartsWith("System") || an.Name.StartsWith("mscorlib"))
                {
                    continue;
                }

                Assembly a = Assembly.Load(an);
                if (a != null)
                {
                    if (!Path.GetDirectoryName(a.Location).StartsWith(AppDirectory))
                    {
                        continue;
                    }

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