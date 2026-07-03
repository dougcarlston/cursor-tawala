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
	/// Class to test conversion of Multiple Choice items.
	/// </summary>
	[TestFixture]
	public class MCItemsTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("MCItems.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void GetMCItem()
		{
			// create XML reader
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreWhitespace = true;

			XmlReader reader = XmlReader.Create(GetTestFilePath("MCItems.xml"), settings);

			reader.ReadToFollowing("mc");

			// get converter's private getMCItem method
			MethodInfo method = GetConverterMethodInfo("getMCItem");

			// create arguments appropriate for getMCItem method
			Object[] args = new object[1];
			args[0] = reader;

			// invoke getMCItem method
			McqItem mcItem = (McqItem)method.Invoke(converter, args);

			Assert.AreEqual("Choice a", mcItem.Choices[0].Text);
			Assert.AreEqual("Choice b", mcItem.Choices[1].Text);
			Assert.AreEqual("Choice c", mcItem.Choices[2].Text);
			Assert.AreEqual("Choice d", mcItem.Choices[3].Text);
			Assert.AreEqual(4, mcItem.Choices.Count);

		}


		/// <summary>
		/// Tests the conversion of the XML file to a Tawala Project.
		/// </summary>
		[Test]
		public void ConvertXmlToProject()
		{
			// verify that project contains 1 form
			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);

		}


		/// <summary>
		/// Verifies that all questions have been converted.
		/// </summary>
		[Test]
		public void Questions()
		{
			Form form = (Form)Project.Current.FormList[0];

			McqItem mcItem1 = (McqItem)form.ItemList[0];

			Assert.AreEqual("Select only one", mcItem1.Text);

			McqItem mcItem2 = (McqItem)form.ItemList[1];

			Assert.AreEqual("Select one or more", mcItem2.Text);
		}


		/// <summary>
		/// Verifies that all choices have been converted.
		/// </summary>
		[Test]
		public void Choices()
		{
			Form form = (Form)Project.Current.FormList[0];

			McqItem mcItem1 = (McqItem)form.ItemList[0];

			Assert.AreEqual("Choice a", mcItem1.Choices[0].Text);
			Assert.AreEqual("Choice b", mcItem1.Choices[1].Text);
			Assert.AreEqual("Choice c", mcItem1.Choices[2].Text);
			Assert.AreEqual("Choice d", mcItem1.Choices[3].Text);
			Assert.AreEqual(4, mcItem1.Choices.Count);

			McqItem mcItem2 = (McqItem)form.ItemList[1];

			Assert.AreEqual(4, mcItem2.Choices.Count);
			Assert.AreEqual("Choice a", mcItem2.Choices[0].Text);
			Assert.AreEqual("Choice b", mcItem2.Choices[1].Text);
			Assert.AreEqual("Choice c", mcItem2.Choices[2].Text);
			Assert.AreEqual("Choice d", mcItem2.Choices[3].Text);
		}


		/// <summary>
		/// Tests the conversion of an MC item in which only one choice may be selected.
		/// </summary>
		[Test]
		public void SelectOnlyOne()
		{
			Form form = (Form)Project.Current.FormList[0];

			McqItem mcItem = (McqItem)form.ItemList[0];

			Assert.IsTrue(mcItem.SelectOnlyOne, "SelectOnlyOne is false");

		}


		/// <summary>
		/// Tests the conversion of an MC item in which more than one choice may be selected.
		/// </summary>
		[Test]
		public void SelectOneOrMore()
		{
			Form form = (Form)Project.Current.FormList[0];

			McqItem mcItem = (McqItem)form.ItemList[1];

			Assert.IsFalse(mcItem.SelectOnlyOne, "SelectOnlyOne is true");

		}


	}
}
