// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using Tawala.Common.Properties;

namespace Tawala.Common
{
    /// <summary>
    /// Loads information from config.xml in the directory containing
    /// the exe that distinguishes an iteration build from a release
    /// build.
    /// If no config.xml is found then assume an iteration build
    /// but the build # will be zero.
    /// </summary>
    /// <remarks>Need to figure out how to make this class testable</remarks>
    public static class Config
    {
        #region RuntimeType enum

        public enum RuntimeType
        {
            Build,
            Dev,
            Production
        } ;

        #endregion

        private static string appDirectory = string.Empty;
        private static string build = "0";
        private static string buildName = "no config file!";
        private static string clientURL = "http://build.tawala.com/client";
        private static string defaultProjectDir = string.Empty;

        /// <summary>
        /// Not sure how to test this because the ProductName comes from the running executable
        /// </summary>
        private static string localAppData;

        private static string projectManagerURL = "http://build.tawala.com/manage/projects";

        private static RuntimeType runtimeEnvironment = RuntimeType.Build;
        private static string temporaryFiles;

        public static RuntimeType RuntimeEnvironment
        {
            get { return runtimeEnvironment; }
        }

        public static string AppDirectory
        {
            get
            {
                if (appDirectory.Length == 0)
                {
                    appDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                }
                return appDirectory;
            }
        }

        /// <summary>
        /// Gets the path to the project directory under My Documents.
        /// Creates it if its doesn't yet exist.
        /// </summary>
        /// <remarks>
        /// For a release this would be "...\My Documents\Tawala"
        /// For an iteration this would be "...\My Documents\Tawala Iterations\Build nnn"
        /// </remarks>
        public static string DefaultProjectDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(defaultProjectDir))
                {
                    defaultProjectDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                     Application.ProductName);

                    if (runtimeEnvironment == RuntimeType.Build)
                    {
                        defaultProjectDir = Path.Combine(defaultProjectDir, "Build");
                    }
                    else if (runtimeEnvironment == RuntimeType.Dev)
                    {
                        defaultProjectDir = Path.Combine(defaultProjectDir, "Dev");
                    }

                    try
                    {
                        // attempt to create defaultProjectDir under MyDocuments if it doesn't exist
                        Directory.CreateDirectory(defaultProjectDir);
                    }
                    catch (Exception)
                    {
                    }
                }

                return defaultProjectDir;
            }
        }

        public static string ClientURL
        {
            get { return clientURL; }
        }

        public static string FunctionRepositoryURL
        {
            get { return RootURL + "display-component-repository"; }
        }

        public static string FunctionRepositoryVersionURL
        {
            get { return RootURL + "display-component-repository-version"; }
        }

        public static string RootURL
        {
            get { return Regex.Match(clientURL, "[^:]*://[^/]*/").Value; }
        }

        public static string ProjectManagerURL
        {
            get { return projectManagerURL; }
        }

        public static string Build
        {
            get { return build; }
        }

        public static string BuildName
        {
            get { return buildName; }
        }

        public static string ThemesURL
        {
            get { return RootURL + "projectThemes"; }
        }

        public static string LocalApplicationData
        {
            get
            {
                if (localAppData == null)
                {
                    try
                    {
                        // attempt to create a Tawala directory under the user's LocalApplicationData
                        localAppData = Application.LocalUserAppDataPath;
                        Directory.CreateDirectory(localAppData);
                    }
                    catch (Exception)
                    {
                    }
                }

                return localAppData;
            }
        }

        public static string TemporaryFiles
        {
            get
            {
                if (temporaryFiles == null)
                {
                    try
                    {
                        temporaryFiles = Path.Combine(LocalApplicationData, "~temp");

                        Directory.CreateDirectory(temporaryFiles);
                    }
                    catch (Exception)
                    {
                    }
                }

                return temporaryFiles;
            }
        }

        public static void SetClientUrlToValidateProjectXMLOnly(string validateURL)
        {
            clientURL = validateURL;
        }

        public static void SetBuildNumberToValidateProjectXMLOnly(string validateBuild)
        {
            build = validateBuild;
        }

        public static void Load()
        {
            runtimeEnvironment = RuntimeType.Production;

            // check for special "runtime" config file (used for developers' build environment)
            string configRuntime = Path.Combine(AppDirectory, "config.runtime.xml");
            if (File.Exists(configRuntime))
            {
                XPathNavigator xml = new XPathDocument(configRuntime).CreateNavigator();
                runtimeEnvironment = xml.SelectSingleNode("tawalaRuntime/@build-runtime").Value == "true"
                                         ? RuntimeType.Build
                                         : RuntimeType.Dev;

                if (runtimeEnvironment == RuntimeType.Build)
                {
                    clientURL = xml.SelectSingleNode("tawalaRuntime/@buildClientURL").Value;
                    projectManagerURL = xml.SelectSingleNode("tawalaRuntime/@buildProjectManagerURL").Value;
                }
            }

            // command line arg can override dev vs. production flag
            string[] arguments = Environment.GetCommandLineArgs();
            foreach (string arg in arguments)
            {
                if (arg.ToLower().StartsWith("dev") || arg.ToLower().StartsWith("/dev"))
                {
                    int index = arg.IndexOf('=');
                    runtimeEnvironment = arg.Substring(index + 1).Trim().ToLower() == "true" ? RuntimeType.Dev : RuntimeType.Production;
                    break;
                }

                else if (arg.ToLower().StartsWith("bld") || arg.ToLower().StartsWith("/bld"))
                {
                    int index = arg.IndexOf('=');
                    runtimeEnvironment = arg.Substring(index + 1).Trim().ToLower() == "true" ? RuntimeType.Build : RuntimeType.Production;
                    break;
                }
            }

            // the normal config file
            string config = Path.Combine(AppDirectory, "config.xml");

            if (File.Exists(config))
            {
                XPathNavigator xml = new XPathDocument(config).CreateNavigator();

                if (runtimeEnvironment == RuntimeType.Dev)
                {
                    clientURL = xml.SelectSingleNode("tawala/@devClientURL").Value;
                    projectManagerURL = xml.SelectSingleNode("tawala/@devProjectManagerURL").Value;
                }
                else if (runtimeEnvironment == RuntimeType.Production)
                {
                    clientURL = xml.SelectSingleNode("tawala/@clientURL").Value;
                    projectManagerURL = xml.SelectSingleNode("tawala/@projectManagerURL").Value;
                }

                build = xml.SelectSingleNode("tawala/@build").Value;
                buildName = xml.SelectSingleNode("tawala/@build-name").Value + "\n\n" +
                            Resources.ResourceManager.GetString("BuildString") + " " + build;

                if (runtimeEnvironment == RuntimeType.Dev)
                {
                    buildName += " (DEV)";
                }
                else if (runtimeEnvironment == RuntimeType.Build)
                {
                    buildName += " (BUILD)";
                }
            }
            else
            {
                runtimeEnvironment = RuntimeType.Dev;
            }

            // check the command line to see if a different URL is specified for client
            foreach (string arg in arguments)
            {
                if (arg.StartsWith("deploymentURL=") || arg.StartsWith("clientURL="))
                {
                    int index = arg.IndexOf('=');
                    clientURL = arg.Substring(index + 1).Trim();
                    break;
                }
            }
        }

        /// <summary>
        /// Gets an xml from the website containing info about the most recent version of the
        /// tawala client on the server.
        /// </summary>
        /// <returns>XmlDocument object that is null on failure</returns>
        public static XmlDocument CheckForNewVersion()
        {
            try
            {
                // instantiate transceiver for the Tawala web server
                var transceiver = new XMLTransceiver(RootURL + "clientinfo");
                string xml = transceiver.Receive();
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                return xmlDoc;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Used to test the xml returned from CheckForNewVersion()
        /// </summary>
        public static bool IsNewerVersion(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
            {
                return false;
            }

            return Convert.ToInt32(xmlDoc.SelectSingleNode("//buildNumber").InnerText) > Convert.ToInt32(build);
        }

        public static bool IsMandatoryUpdate(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
            {
                return false;
            }

            return Convert.ToInt32(xmlDoc.SelectSingleNode("//buildMinimum").InnerText) > Convert.ToInt32(build);
        }

        public static XmlDocument GetProjectThemesFromServer()
        {
            var transceiver = new XMLTransceiver(ThemesURL);

            // get back the result
            var xml = new XmlDocument();
            xml.LoadXml(transceiver.Receive());
            return xml;
        }

        public static SortedDictionary<string, string> GetProjectThemeList()
        {
            var themes = new SortedDictionary<string, string>();
            var xml = new XPathDocument(Path.Combine(LocalApplicationData, "ProjectThemes.xml"));
            XPathNavigator nav = xml.CreateNavigator();

            XPathNodeIterator iterator = nav.Select("/projectThemes/theme");

            while (iterator.MoveNext())
            {
                string name = iterator.Current.SelectSingleNode("name").Value;
                string path = iterator.Current.SelectSingleNode("path").Value;
                themes.Add(name, path);
            }

            return themes;
        }
    }
}