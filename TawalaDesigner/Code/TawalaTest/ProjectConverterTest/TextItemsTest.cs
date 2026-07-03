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
	public class TextItemsTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("TextItems.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void NoDisplayField()
		{
			Form form = (Form)Project.Current.FormList[0];

			TextItem item = (TextItem)form.ItemList[0];

			Assert.AreEqual("Text Item with no display field", item.Text);
		}

		[Test]
		public void FibBlankDisplayField()
		{
			Form form = (Form)Project.Current.FormList[0];

			TextItem item = (TextItem)form.ItemList[3];

			Assert.AreEqual("Text Item with FIB blank display field: <<Form 1:Q2:a>>", item.Text);
		}


		/// <summary>
		/// Checks for text item with MCQ display field
		/// </summary>
		[Test]
		public void MCQDisplayField()
		{
			Form form = (Form)Project.Current.FormList[0];

			TextItem item = (TextItem)form.ItemList[4];

			Assert.AreEqual("Text Item with MCQ display field: <<Form 1:Q1>>", item.Text);
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

			// verify that form contains 3 text items, one MCQ and one FIB
			Assert.AreEqual(5, form.ItemList.Count);
			Assert.AreEqual(typeof(TextItem), form.ItemList[0].GetType());
			Assert.AreEqual(typeof(McqItem), form.ItemList[1].GetType());
			Assert.AreEqual(typeof(FibItem), form.ItemList[2].GetType());
			Assert.AreEqual(typeof(TextItem), form.ItemList[3].GetType());
			Assert.AreEqual(typeof(TextItem), form.ItemList[4].GetType());

		}

	}
}
