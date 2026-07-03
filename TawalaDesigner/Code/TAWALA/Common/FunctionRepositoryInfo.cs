// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Text;
using Tawala.Functions.Runtime;

namespace Tawala.Common
{
    public static class FunctionRepositoryInfo
    {
        public static bool IsLoaded { get { return FunctionLoader.Repository != null; } }

        public static string QueryServerRepository(string credentialsXml, string parameters)
        {
            try
            {
                var transceiver = new XMLTransceiver(Config.FunctionRepositoryURL + parameters);

                var sb = new StringBuilder();

                transceiver.Transmit(sb.ToString());
                return transceiver.Receive();
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }

        public static void Build()
        {
            FunctionAssemblyCompiler.Build();
        }

        #region Nested type: FunctionAssemblyCompiler

        private static class FunctionAssemblyCompiler
        {
            private const string commandLineXmlSwitch = "/function-rebuild=";

            public static void Build()
            {
				if (lastKnownSignatureMatchesWeb() || serverAppearsOffline())
                {
                    if (tryLoad(FunctionLoader.GetPossibleFunctionDllLocation()))
                    {
                        return;
                    }
                }

                buildAndLoad();
            }

            private static void buildAndLoad()
            {
                string repositoryXmlString = getRepositoryXml();
                FunctionLoader.BuildAndLoad(repositoryXmlString);
            }

            private static string getRepositoryXml()
            {
                string repositoryXmlString = getXmlFromFileSpecifiedOnCommandLine();
                ;

                if (string.IsNullOrEmpty(repositoryXmlString))
                {
                    repositoryXmlString = QueryServerRepository("", "");
                }

                return repositoryXmlString;
            }

            private static string getXmlFromFileSpecifiedOnCommandLine()
            {
                if (Environment.CommandLine.Contains(commandLineXmlSwitch))
                {
                    foreach (string arg in Environment.GetCommandLineArgs())
                    {
                        if (!arg.StartsWith(commandLineXmlSwitch))
                        {
                            continue;
                        }

                        string xmlFilename = arg.Substring(commandLineXmlSwitch.Length).Trim();
                        if (string.IsNullOrEmpty(xmlFilename) || !File.Exists(xmlFilename))
                        {
                            return null;
                        }

                        return File.ReadAllText(xmlFilename);
                    }
                }

                return null;
            }

            private static bool lastKnownSignatureMatchesWeb()
            {
                if (Environment.CommandLine.Contains(commandLineXmlSwitch))
                {
                    return false;
                }

                string xml = QueryServerRepository("", "?signature=" + FunctionLoader.GetLastKnownSignature());
				return xml.Contains("component-repository-is-current");
            }

			private static bool serverAppearsOffline()
			{
				return string.IsNullOrEmpty(QueryServerRepository("", "?signature=" + FunctionLoader.GetLastKnownSignature()));
			}

        	private static bool localRepositoryIsCurrent(string xml)
        	{
        		return xml.Contains("component-repository-is-current");
        	}

            private static bool tryLoad(string path)
            {
                try
                {
                    FunctionLoader.Load(path);
                }
                catch
                {
                }
                return FunctionLoader.Repository != null;
            }
        }

        #endregion
    }
}