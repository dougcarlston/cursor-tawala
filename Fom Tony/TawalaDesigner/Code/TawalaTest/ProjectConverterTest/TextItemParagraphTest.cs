using System;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.ProjectTest;
using TawalaTest.TestSupport;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion of Page Break items.
	/// </summary>
	[TestFixture]
	public class TextItemParagraphTest : TestBase
	{
		TawalaProjectConverter converter;

		private const string NEWLINE = "\r\n";

		private const string halfInchTabPositionsString =
				"<tabPositions>" +
				"<tabStop position=\"720\"/>" +
				"<tabStop position=\"1440\"/>" +
				"<tabStop position=\"2160\"/>" +
				"<tabStop position=\"2880\"/>" +
				"<tabStop position=\"3600\"/>" +
				"<tabStop position=\"4320\"/>" +
				"<tabStop position=\"5040\"/>" +
				"<tabStop position=\"5760\"/>" +
				"<tabStop position=\"6480\"/>" +
				"<tabStop position=\"7200\"/>" +
				"<tabStop position=\"7920\"/>" +
				"<tabStop position=\"8640\"/>" +
				"<tabStop position=\"9360\"/>" +
				"<tabStop position=\"10080\"/>" +
				"</tabPositions>";

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("TextItemParagraph.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void PlainText()
		{
			Form form = (Form)Project.Current.FormList[0];

			TextItem item = (TextItem)form.ItemList[0];

			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				halfInchTabPositionsString +
				XmlConstants.FullBeginFont +
				"Plain Text" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</text>\r\n";

			Assert.AreEqual(xmlString, item.ToXml("T1"));
		}

		[Test]
		public void MultipleParagraphs()
		{
			Form form = (Form)Project.Current.FormList[0];

			TextItem item = (TextItem)form.ItemList[1];

			string xmlString =
				"<text label=\"T2\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				halfInchTabPositionsString +
				XmlConstants.FullBeginFont +
				"Paragraph 1" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				halfInchTabPositionsString +
				XmlConstants.FullBeginFont +
				"Paragraph 2" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</text>\r\n";

			Assert.AreEqual(xmlString, item.ToXml("T2"));
		}

	}
}
