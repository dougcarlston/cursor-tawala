using System;
using NUnit.Framework;
using Tawala.Proj;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class HtmlParagraphTest
	{
		[Test]
		public void ConstructFromXml() 
		{
			string xmlString =
				"<p style=\"margin-top : 2pt ;margin-bottom:3pt ;margin-left:36pt\">" +
				"</p>";

			IXmlElement element = new XmlElement(xmlString);
			HtmlParagraph paragraph = new HtmlParagraph(element);

			Assert.AreEqual(2, paragraph.MarginTop);
			Assert.AreEqual(3, paragraph.MarginBottom);
			Assert.AreEqual(36, paragraph.MarginLeft);
		}

		[Test]
		public void SpansOnly()
		{
			string xmlString =
				"<p style=\"margin-top : 2pt ;margin-bottom:3pt\">" +
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"</span>" +
				"<span style=\"font-family:'Times New Roman';font-size:14pt;\">" +
				"</span>" +
				"</p>";

			IXmlElement element = new XmlElement(xmlString);
			HtmlParagraph paragraph = new HtmlParagraph(element);

			Assert.AreEqual(2, paragraph.Contents.Count);
			Assert.AreEqual("Arial", ((HtmlSpan)paragraph.Contents[0]).FontFamily);
			Assert.AreEqual(12, ((HtmlSpan)paragraph.Contents[0]).FontSize);
			Assert.AreEqual("Times New Roman", ((HtmlSpan)paragraph.Contents[1]).FontFamily);
			Assert.AreEqual(14, ((HtmlSpan)paragraph.Contents[1]).FontSize);
		}

		[Test]
		public void SpansContainingText()
		{
			string xmlString =
				"<p style=\"margin-top : 2pt ;margin-bottom:3pt\">" +
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"Text in Span 1" +
				"</span>" +
				"<span style=\"font-family:'Times New Roman';font-size:14pt;\">" +
				"Text in Span 2" +
				"</span>" +
				"</p>";

			IXmlElement element = new XmlElement(xmlString);
			HtmlParagraph paragraph = new HtmlParagraph(element);

			Assert.AreEqual(2, paragraph.Contents.Count);
			Assert.AreEqual("Text in Span 1", paragraph.Contents[0].Text);
			Assert.AreEqual("Text in Span 2", paragraph.Contents[1].Text);
		}

		[Test]
		public void TextOnly()
		{
			string xmlString =
				"<p style=\"margin-top : 2pt ;margin-bottom:3pt\">" +
				"Text" +
				"</p>";

			IXmlElement element = new XmlElement(xmlString);
			HtmlParagraph paragraph = new HtmlParagraph(element);

			Assert.AreEqual(1, paragraph.Contents.Count);
			Assert.AreEqual("Text", paragraph.Contents[0].Text);
		}

		[Test]
		public void SpansAndText()
		{
			string xmlString =
				"<p style=\"margin-top : 2pt ;margin-bottom:3pt\">" +
				"Text Before Spans" +
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"Text in Span 1" +
				"</span>" +
				"<span style=\"font-family:'Times New Roman';font-size:14pt;\">" +
				"Text in Span 2" +
				"</span>" +
				"Text After Spans" +
				"</p>";

			IXmlElement element = new XmlElement(xmlString);
			HtmlParagraph paragraph = new HtmlParagraph(element);

			Assert.AreEqual(4, paragraph.Contents.Count);
			Assert.AreEqual("Text Before Spans", paragraph.Contents[0].Text);
			Assert.AreEqual("Text in Span 1", paragraph.Contents[1].Text);
			Assert.AreEqual("Text in Span 2", paragraph.Contents[2].Text);
			Assert.AreEqual("Text After Spans", paragraph.Contents[3].Text);
		}

		[Test]
		public void ColumnCount()
		{
			string xmlString =
				"<p style=\"margin-top : 2pt ;margin-bottom:3pt\">" +
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"Column1&#9;Column2&#9;Column3" +
				"</span>" +
				"</p>";

			IXmlElement element = new XmlElement(xmlString);
			HtmlParagraph paragraph = new HtmlParagraph(element);

			Assert.AreEqual(3, paragraph.ColumnCount);
		}

		[Test]
		public void ToTable()
		{
			string xmlString =
				"<p style=\"margin-top : 2pt ;margin-bottom:3pt\">" +
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"Column1&#9;Column2&#9;Column3" +
				"</span>" +
				"</p>";

			string expectedString =
				"<table width=\"700\">" +
				"<tr>" +
				"<td width=\"33%\">" +
				"Column1" +
				"</td>" +
				"<td width=\"33%\">" +
				"Column2" +
				"</td>" +
				"<td width=\"33%\">" +
				"Column3" +
				"</td>" +
				"</tr>" +
				"</table>";

			IXmlElement element = new XmlElement(xmlString);
			HtmlParagraph paragraph = new HtmlParagraph(element);

			Assert.AreEqual(1, paragraph.Contents.Count);
			Assert.AreEqual("Column1\tColumn2\tColumn3", paragraph.Contents[0].Text);

			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}

		[Test]
		public void ToTableWithFormatting()
		{
			string xmlString =
				"<p style=\"margin-top: 0pt;margin-bottom: 0pt;\">" +
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"<b>Column1</b>" +
				"</span>" +
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"&#9;" +
				"</span>" +
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"<i>Column2</i>" +
				"</span>" +
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"&#9;" +
				"</span>" +
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"<u>Column3</u>" +
				"</span>" +
				"</p>";

			string expectedString =
				"<table width=\"700\">" +
				"<tr>" +
				"<td width=\"33%\">" +
				"<b>Column1</b>" +
				"</td>" +
				"<td width=\"33%\">" +
				"<i>Column2</i>" +
				"</td>" +
				"<td width=\"33%\">" +
				"<u>Column3</u>" +
				"</td>" +
				"</tr>" +
				"</table>";

			IXmlElement element = new XmlElement(xmlString);
			HtmlParagraph paragraph = new HtmlParagraph(element);

			Assert.AreEqual(5, paragraph.Contents.Count);
			Assert.AreEqual("<b>Column1</b>", paragraph.Contents[0].Text);
			Assert.AreEqual("\t", paragraph.Contents[1].Text);
			Assert.AreEqual("<i>Column2</i>", paragraph.Contents[2].Text);
			Assert.AreEqual("\t", paragraph.Contents[3].Text);
			Assert.AreEqual("<u>Column3</u>", paragraph.Contents[4].Text);

			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}

		[Test]
		public void ToTableWithFormattingNoSpans()
		{
			string xmlString =
				"<p style=\"margin-top: 0pt;margin-bottom: 0pt;\">" +
				"<b>Column1</b>" +
				"&#9;" +
				"<i>Column2</i>" +
				"&#9;" +
				"<u>Column3</u>" +
				"</p>";

			string expectedString =
				"<table width=\"700\">" +
				"<tr>" +
				"<td width=\"33%\">" +
				"<b>Column1</b>" +
				"</td>" +
				"<td width=\"33%\">" +
				"<i>Column2</i>" +
				"</td>" +
				"<td width=\"33%\">" +
				"<u>Column3</u>" +
				"</td>" +
				"</tr>" +
				"</table>";

			IXmlElement element = new XmlElement(xmlString);
			HtmlParagraph paragraph = new HtmlParagraph(element);

			Assert.AreEqual(5, paragraph.Contents.Count);
			Assert.AreEqual("<b>Column1</b>", paragraph.Contents[0].Text);
			Assert.AreEqual("\t", paragraph.Contents[1].Text);
			Assert.AreEqual("<i>Column2</i>", paragraph.Contents[2].Text);
			Assert.AreEqual("\t", paragraph.Contents[3].Text);
			Assert.AreEqual("<u>Column3</u>", paragraph.Contents[4].Text);

			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}


	}
}
