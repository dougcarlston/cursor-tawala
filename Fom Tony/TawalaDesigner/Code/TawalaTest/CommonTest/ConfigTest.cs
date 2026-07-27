using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using NUnit.Framework;
using Tawala.Common;

namespace TawalaTest.CommonTest
{
	/// <summary>
	/// Not comprehensive nor perfect but its a start considering I created the Config class a couple months ago. (shame on me)
	/// </summary>
	[TestFixture]
	public class ConfigTest
	{
		[Test]
		public void RootURL()
		{
			string url = Config.RootURL;

			// in unit test environment it should be the dev root url
			Assert.AreEqual("http://build.tawala.com/", url);
		}

		[Test]
		public void ThemesURL()
		{
			string url = Config.ThemesURL;

			// in unit test environment it should be based on the dev root url
			Assert.AreEqual("http://build.tawala.com/projectThemes", url);
		}

		[Test]
		public void CheckForNewVersion()
		{
			XmlDocument xmlDoc = Config.CheckForNewVersion();

			Assert.IsNotNull(xmlDoc);

			XmlNode buildNumNode = xmlDoc.SelectSingleNode("/clientInfo/buildNumber");
			Assert.IsNotNull(buildNumNode);
			Assert.IsTrue(Convert.ToInt32(buildNumNode.InnerText) > 0);

			XmlNode buildMinNode = xmlDoc.SelectSingleNode("/clientInfo/buildMinimum");
			Assert.IsNotNull(buildMinNode);
			Assert.IsTrue(Convert.ToInt32(buildMinNode.InnerText) > 0);
			Assert.IsTrue(Convert.ToInt32(buildMinNode.InnerText) <= Convert.ToInt32(buildNumNode.InnerText));

			XmlNode urlNode = xmlDoc.SelectSingleNode("/clientInfo/url");
			Assert.IsNotNull(urlNode);
			Assert.IsTrue(urlNode.InnerText.Length > 0);
		}

		[Test]
		public void GetProjectThemesFromServer()
		{
			XmlDocument xmlDoc = Config.GetProjectThemesFromServer();

			Assert.IsNotNull(xmlDoc);
			Assert.IsNotNull(xmlDoc.SelectSingleNode("/projectThemes/theme/name"));
			Assert.IsNotNull(xmlDoc.SelectSingleNode("/projectThemes/theme/path"));
		}

		[Test]
		public void GetProjectThemeList()
		{
			XmlDocument xml = Config.GetProjectThemesFromServer();
			xml.Save(Path.Combine(Config.LocalApplicationData, "ProjectThemes.xml"));

			SortedDictionary<string, string> themes = Config.GetProjectThemeList();

			Assert.IsTrue(themes.Count > 0);
			Assert.AreEqual("default", themes["Default"]);
		}

		[Test, Ignore("Config use Application property for LocalApplicationData; doesn't work inside test")]  
		public void LocalApplicationData()
		{
			string path = Config.LocalApplicationData;
			Assert.IsTrue(path.EndsWith("1.0"));
		}
	}
}
