// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Text;
using Tawala.Common;
using Tawala.Functions.Runtime;

namespace Tawala.Projects
{
    public class TawalaProjectValidator
    {
        //
        // There are no tests for this code.  It failed under NUnit and I almost gave up
        // on the idea until I tried it in vitro and it worked.
        //

        private Info info;

        public string Message
        {
            get
            {
                if (info.error != null)
                {
                    return string.Format("ERROR: {0}", info.error);
                }
                else if (info.exception != null)
                {
                    return string.Format("EXCEPTION: {0}", info.exception);
                }
                else
                {
                    return info.xmlAfter;
                }
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

            AppDomain domain = AppDomain.CreateDomain("validate project xml");
            domain.SetData("results", info);

            domain.DoCallBack(CrossDomainCallback);
            info = domain.GetData("results") as Info;

            AppDomain.Unload(domain);

            return info.success;
        }

        public static void CrossDomainCallback()
        {
            var results = AppDomain.CurrentDomain.GetData("results") as Info;

            try
            {
                if (Project.Current != null)
                {
                    results.error = "Project.Current != null on callback entry";
                    return;
                }

                FieldProviders.ExternalForms = results.externalForms;

                FunctionLoader.Load(results.functionPath);

                byte[] byteArray = Encoding.UTF8.GetBytes(results.xmlBefore);
                var ms = new MemoryStream(byteArray.Length);
                ms.Write(byteArray, 0, byteArray.Length);
                var tpc = new TawalaProjectConverter(ms);
                tpc.ConvertXmlToProject();
                if (Project.Current != null)
                {
                    Project.Events.RaiseProjectOpenedEvent(new ProjectEventArgs(Project.Current.Name));

                    Config.SetClientUrlToValidateProjectXMLOnly(results.clientURL);

                    Config.SetBuildNumberToValidateProjectXMLOnly(results.buildNumber);

                    results.xmlAfter = Project.Current.ToXmlForSaving();

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

            AppDomain.CurrentDomain.SetData("results", results);
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