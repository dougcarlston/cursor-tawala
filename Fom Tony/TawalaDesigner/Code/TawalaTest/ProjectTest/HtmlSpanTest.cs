using System;
using NUnit.Framework;
using Tawala.Proj;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class HtmlSpanTest
	{
		[Test]
		public void ConstructFromHtml() 
		{
			string htmlString =
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"</span>";

			IXmlElement element = new XmlElement(htmlString);
			HtmlSpan span = new HtmlSpan(element);

			Assert.AreEqual("Arial", span.FontFamily);
			Assert.AreEqual(12, span.FontSize);
		}

		[Test]
		public void PlainText()
		{
			string htmlString =
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"Text" +
				"</span>";

			IXmlElement element = new XmlElement(htmlString);
			HtmlSpan span = new HtmlSpan(element);

			Assert.AreEqual("Text", span.Text);
		}

		[Test]
		public void TabbedText()
		{
			string htmlString =
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"Column1&#9;Column2" +
				"</span>";

			IXmlElement element = new XmlElement(htmlString);
			HtmlSpan span = new HtmlSpan(element);

			Assert.AreEqual("Column1\tColumn2", span.Text);
		}

		[Test]
		public void InnerXml()
		{
			string htmlString =
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"<b>Text</b>" +
				"</span>";

			IXmlElement element = new XmlElement(htmlString);
			HtmlSpan span = new HtmlSpan(element);

			Assert.AreEqual("<b>Text</b>", span.InnerXml);
		}

		[Test]
		public void GetXml()
		{
			string htmlString =
				"<span style=\"font-family:'Times';font-size:14pt;\">" +
				"<b>Bold Text</b>" +
				"</span>";

			IXmlElement element = new XmlElement(htmlString);
			HtmlSpan span = new HtmlSpan(element);

			string expectedString =
				"<font face=\"Times\" size=\"14\">" +
				"<b>Bold Text</b>" +
				"</font>";

			Assert.AreEqual(expectedString, span.ToXml());
		}

	}
}
