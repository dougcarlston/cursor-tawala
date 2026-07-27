using System;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	[TestFixture]
	public class ProjectThemeTest : TestBase
	{
		/// <summary>
		/// Test when existing project file has no "themePath"
		/// </summary>
		[Test]
		public void NoThemeTest()
		{
			TawalaProjectConverter converter = GetConverter("ProjectNoTheme.xml");
			converter.ConvertXmlToProject();
			Assert.AreEqual("default", Project.Current.ThemePath);
		}

		/// <summary>
		/// Test when existing project file has "themePath"
		/// </summary>
		[Test]
		public void ThemeTest()
		{
			TawalaProjectConverter converter = GetConverter("ProjectTheme.xml");
			converter.ConvertXmlToProject();
			Assert.AreEqual("style2", Project.Current.ThemePath);
		}
	}
}
