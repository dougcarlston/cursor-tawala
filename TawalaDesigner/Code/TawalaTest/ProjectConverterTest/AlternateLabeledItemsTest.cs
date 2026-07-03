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
	public class AlternateLabeledItemsTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("AlternateLabeledItems.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void ConvertXmlToProject()
		{
			Form form = (Form)Project.Current.FormList[0];

			// verify that project contains 1 form
			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);

			// verify that form contains one each of text, fib and mc
			Assert.AreEqual(3, form.ItemList.Count);
			Assert.AreEqual(typeof(TextItem), form.ItemList[0].GetType());
			Assert.AreEqual(typeof(FibItem), form.ItemList[1].GetType());
			Assert.AreEqual(typeof(McqItem), form.ItemList[2].GetType());

		}

		/// <summary>
		/// Tests the conversion of alternate labels in text items
		/// </summary>
		[Test]
		public void TextLabel()
		{
			Form form = (Form)Project.Current.FormList[0];

			TextItem item = (TextItem)form.ItemList[0];
			Assert.AreEqual("My Text", item.AlternateLabel);
		}

		/// <summary>
		/// Tests the conversion of alternate labels in FIB items
		/// </summary>
		[Test]
		public void FibLabel()
		{
			Form form = (Form)Project.Current.FormList[0];

			FibItem item = (FibItem)form.ItemList[1];
			Assert.AreEqual("My FIB", item.AlternateLabel);
		}

		/// <summary>
		/// Tests the conversion of alternate labels in MC items
		/// </summary>
		[Test]
		public void MCLabel()
		{
			Form form = (Form)Project.Current.FormList[0];

			McqItem item = (McqItem)form.ItemList[2];
			Assert.AreEqual("My MCQ", item.AlternateLabel);
		}

	}
}
