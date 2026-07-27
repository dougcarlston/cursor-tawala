// $Workfile: GlobalSettings.cs $
// $Revision: 9 $	$Date: 2/21/07 2:53p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
//using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Windows.Forms;

namespace Tawala.Common
{
	/// <summary>
	/// A class to contain settings that are accessible to both the Project Designer
	/// and the Invitation Manager applications
	/// </summary>
	/// <remarks>
	/// These settings are made accessible via static properties so they
	/// are available without needing to create and instance of this class.
	/// 
	/// The settings are stored in an XML file (without the .xml extension)
	/// under the current user's Local Settings/Application Data folder.
	/// 
	/// Note that the settings file should be read immediately before a value
	/// is fetched, and should be written immediately when a value is set,
	/// so that it is always current for either application to access.
	/// </remarks>
	public class GlobalSettings
	{
        static private readonly string settingsFileName = "settings";
		static private string settingsFullPath;

		/// <summary>
		/// constructor
		/// </summary>
		static GlobalSettings()
		{
			string settingsDir = Config.LocalApplicationData;

			// is it there?
			if (Directory.Exists(settingsDir))
			{
				// set the full path to the settings file
				settingsFullPath = Path.Combine(settingsDir, settingsFileName);

				// read settings if they already exist, if not that's okay
				try
				{
					readSettings();
				}
				catch (Exception)
				{
				}
			}
			else
			{
				settingsFullPath = null;
				Debug.Assert(false, "Could not establish folder for settings file.");
			}
		}

        private static readonly string functionsTag = "<functions assembly=\"{0}\" version=\"{1}\" />\r\n";

        private static string functionAssemblyPath = string.Empty;
        private static string functionAssemblyVer = string.Empty;

        public static string FunctionAssemblyPath
        {
            get { readSettings(); return functionAssemblyPath; }
            set { functionAssemblyPath = value; writeSettings();  }
        }


        public static string FunctionAssemblyVersion
        {
            get { readSettings(); return functionAssemblyVer; }
            set { functionAssemblyVer = value; writeSettings(); }
        }

        private static readonly string xmlCredentialsTag = "<credentials user=\"{0}\" password=\"{1}\" />\r\n";

		public static string CredentialsElement(string aUserName, string aPassword)
		{
			StringBuilder xmlString = new StringBuilder();
			xmlString.AppendFormat(xmlCredentialsTag, aUserName, aPassword);
			return xmlString.ToString();
		}

		public static string CredentialsElement()
		{
			return CredentialsElement(userName, password);
		}

		// xml strings
		private static readonly string xmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n";
		private static readonly string xmlSettingsStartTag = "<globalSettings protocol=\"1.0\">\r\n";
		private static readonly string xmlSettingsEndTag = "</globalSettings>\r\n";

		/// <summary>
		/// writes the current settings to the settings file
		/// </summary>
		private static void writeSettings()
		{
			// if the path is valid
			if (settingsFullPath != null)
			{
				XmlDocument xmlDoc = new XmlDocument();

				StringBuilder xmlString = new StringBuilder();

				// append tags
				xmlString.Append(xmlDeclaration);
				xmlString.Append(xmlSettingsStartTag);
				xmlString.Append(CredentialsElement());
                xmlString.Append(string.Format(functionsTag, functionAssemblyPath, functionAssemblyVer));
                xmlString.Append(xmlSettingsEndTag);

				// write it to the settings file
				xmlDoc.PreserveWhitespace = true;
				xmlDoc.LoadXml(xmlString.ToString());
				xmlDoc.Save(settingsFullPath);
			}
		}

		/// <summary>
		/// reads in the data from the settings file
		/// </summary>
		private static void readSettings()
		{
			// if the path is valid and the file exists
			if (settingsFullPath != null && File.Exists(settingsFullPath))
			{
				// load in the xml from the file
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(settingsFullPath);

                functionAssemblyPath = string.Empty;
                functionAssemblyVer = string.Empty;

                // get the functions tag
                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("functions");
                if (nodeList.Count == 1)
                {
                    XmlNode functions = nodeList[0];
                    if (functions.Attributes["assembly"] != null && functions.Attributes["version"] != null)
                    {
                        functionAssemblyPath = functions.Attributes["assembly"].Value;
                        functionAssemblyVer = functions.Attributes["version"].Value;
                    }
                }

                // get the credentials tag
				nodeList = xmlDoc.GetElementsByTagName("credentials");
				Debug.Assert(nodeList.Count == 1);

				XmlNode node = nodeList[0];
				XmlAttributeCollection attrColl = node.Attributes;

				// extract the user name and the password attributes
				XmlAttribute attr = (XmlAttribute)attrColl.GetNamedItem("user");
				userName = attr.Value;

				attr = (XmlAttribute)attrColl.GetNamedItem("password");
				password = attr.Value;
			}
		}

		// user name and password properties
		private static string userName = "";
		private static string password = "";

		public static string UserName
		{
			get
			{
				readSettings();
				return userName;
			}

			set
			{
				userName = value;
				writeSettings();
			}
		}

		public static string Password
		{
			get
			{
				readSettings();
				return password;
			}

			set
			{
				password = value;
				writeSettings();
			}
		}

		/// <summary>
		/// Displays dialog for user name and password
		/// </summary>
		/// <param name="parent">
		/// parent Form for modal relationship
		/// </param>
		/// <returns>
		/// DialogResult: OK or Cancel
		/// </returns>
		public static DialogResult PromptForCredentials(IWin32Window parent)
		{
			LoginForm loginForm = new LoginForm();

			// initialize the dialog tesxt boxes with the current credentials
			// NOTE: Use the properties, rather than the private Fields
			//		 so the settings file is accessed
			loginForm.UserName = UserName;
			loginForm.Password = Password;

			DialogResult result = loginForm.ShowDialog(parent);
			if (result == DialogResult.OK)
			{
				// store the input values
				UserName = loginForm.UserName;
				Password = loginForm.Password;
			}

			return result;
		}
	}
}
