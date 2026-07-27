// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Functions.Runtime;

namespace Tawala.Projects
{
    public class ProjectXmlValidator
    {
        private Info info;

        public string Message
        {
            get
            {
                if (info.error != null)
                {
                    return string.Format("ERROR: {0}", info.error);
                }
                if (info.exception != null)
                {
                    return string.Format("EXCEPTION: {0}", info.exception);
                }
                return info.xmlAfter;
            }
        }

        public bool ValidateXML()
        {
            info = new Info();

            if (Environment.CommandLine.ToLowerInvariant().Contains("/novalidation"))
            {
                return true;
            }

            info.xmlBefore = Project.Current.ToXmlForSaving();
            info.clientURL = Config.ClientURL;
            info.buildNumber = Config.Build;
            info.externalForms = FieldProviders.ExternalForms;
            info.functionPath = FunctionLoader.GetPossibleFunctionDllLocation();

            AppDomain domain = createDomain();

            try
            {
                domain.SetData("results", info);
                domain.DoCallBack(CrossDomainCallback);
                info = domain.GetData("results") as Info;
            }
            finally
            {
                AppDomain.Unload(domain);
            }

            return info.success;
        }

        public static void CrossDomainCallback()
        {
            var results = AppDomain.CurrentDomain.GetData("results") as Info;
            string tempfile = Path.GetTempFileName();
            string name = Path.GetFileNameWithoutExtension(tempfile);

            try
            {
                if (Project.Current != null)
                {
                    results.error = "Project.Current != null on callback entry";
                    return;
                }

                FieldProviders.ExternalForms = results.externalForms;

                FunctionLoader.Load(results.functionPath);

                results.xmlBefore = Regex.Replace(results.xmlBefore, "<project name=\"[^\"]+\"", "<project name=\"" + name + "\"");
                File.WriteAllText(tempfile, results.xmlBefore, Encoding.UTF8);

                Project.Open(tempfile);

                if (Project.Current != null)
                {
                    File.WriteAllText(tempfile, string.Empty);

                    Config.SetClientUrlToValidateProjectXMLOnly(results.clientURL);

                    Config.SetBuildNumberToValidateProjectXMLOnly(results.buildNumber);

                    Project.Save(tempfile);

                    results.xmlAfter = File.ReadAllText(tempfile);

                    if (results.xmlBefore.CompareTo(results.xmlAfter) == 0)
                    {
                        results.success = true;
                    }
                }
                else
                {
                    results.error = "Project.Current == null after roundtrip";
                }
            }
            catch (Exception e)
            {
                results.exception = e.ToString();
            }
            finally
            {
                try
                {
                    File.Delete(tempfile);
                }
                catch
                {
                }
                AppDomain.CurrentDomain.SetData("results", results);
            }
        }

        private static AppDomain createDomain()
        {
            if (Assembly.GetEntryAssembly() != null)
            {
                return AppDomain.CreateDomain("ProjectXmlValidator");
            }

            return createDomainInTestEnvironment();
        }

        private static AppDomain createDomainInTestEnvironment()
        {
            var setup = new AppDomainSetup();
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            setup.CachePath = AppDomain.CurrentDomain.RelativeSearchPath;

            return AppDomain.CreateDomain("TestProjectXmlValidator", null, setup);
        }

        #region Nested type: Info

        [Serializable]
        private class Info
        {
            public string buildNumber;
            public string clientURL;
            public string error;
            public string exception;
            public FormList externalForms;
            public string functionPath;

            public bool success;
            public string xmlAfter;
            public string xmlBefore;
        }

        #endregion
    }
}