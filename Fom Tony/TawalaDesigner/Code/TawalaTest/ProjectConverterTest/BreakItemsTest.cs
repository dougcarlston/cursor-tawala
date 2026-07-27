using System;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion of Page Break items.
	/// </summary>
	[TestFixture]
	public class BreakItemsTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("BreakItems.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void GetBreakItem()
		{
			// create XML reader
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreWhitespace = true;

			XmlReader reader = XmlReader.Create(TawalaTest.TestSupport.Util.GetTestFilePath("BreakItems.xml"), settings);

			reader.ReadToFollowing("break");

			// get converter's private getBreakItem method
			MethodInfo method = GetConverterMethodInfo("getBreakItem");

			// create arguments appropriate for getBreakItem method
			Object[] args = new object[1];
			args[0] = reader;

			// invoke getBreakItem method
            BreakItem breakItem = (BreakItem)method.Invoke(converter, args);

			Assert.IsNotNull(breakItem, "Break item should not be null");
		}


		/// <summary>
		/// Tests the conversion of the XML file to a Tawala Project.
		/// </summary>
		[Test]
		public void ConvertXmlToProject()
		{
			Form form = (Form)Project.Current.FormList[0];

			// verify that project contains 1 form
			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual("Form 1", form.Name);

			// verify that form contains a page break
			Assert.AreEqual(3, form.ItemList.Count);
			Assert.AreEqual(typeof(BreakItem), form.ItemList[1].GetType());

		}
	}
}
