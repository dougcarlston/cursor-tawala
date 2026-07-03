// $Workfile: ParagraphTest.cs $
// $Revision: 16 $	$Date: 1/19/07 8:58a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the Paragraph class
	/// </summary>
	[TestFixture]
	public class ParagraphTest
	{
		[Test]
		public void Construct()
		{
			Paragraph paragraph = new Paragraph();
			Assert.AreEqual(0, paragraph.Indent);
			Assert.AreEqual("left", paragraph.Align);

			paragraph.Align = Paragraph.AlignRight;
			paragraph.Indent = 720;
			Assert.AreEqual(720, paragraph.Indent);
			Assert.AreEqual("right", paragraph.Align);
		}

		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
				"<paragraph indent=\"720\" align=\"left\">" +
				"</paragraph>";

			IXmlElement element = new XmlElement(xmlString);
			Paragraph paragraph = new Paragraph(element);

			Assert.AreEqual(720, paragraph.Indent);
			Assert.AreEqual("left", paragraph.Align);
		}

        //[Ignore("HtmlParagraph obsolete (?)")]
        //[Test]
        //public void ConstructFromHtml()
        //{
        //    string htmlString =
        //        "<p style=\"margin-left : 36pt\" align=\"right\">" +
        //        "</p>";

        //    HtmlParagraph htmlParagraph = new HtmlParagraph(htmlString);

        //    Paragraph paragraph = new Paragraph(htmlParagraph.MarginLeft * 20, htmlParagraph.Align);

        //    Assert.AreEqual(720, paragraph.Indent);
        //    Assert.AreEqual("right", paragraph.Align);
        //}

		[Test]
		public void GetXml()
		{
			Paragraph paragraph = new Paragraph();
			paragraph.Add("Text");

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"Text" +
				"</paragraph>";

			Assert.AreEqual(expectedString, paragraph.ToXml());
		}

        //[Ignore("HtmlParagraph obsolete (?)")]
        //[Test]
        //public void GetXmlFromHtml()
        //{
        //    string htmlString =
        //        "<p style=\"margin-left : 36pt\" align=\"right\">" +
        //        "<span style=\"font-family:'Times New Roman'; font-size:14pt\">" +
        //        "<b>Bold Text</b>" +
        //        "</span>" +
        //        "</p>";

        //    HtmlParagraph htmlParagraph = new HtmlParagraph(htmlString);

        //    Paragraph paragraph = new Paragraph();
        //    paragraph.Indent = htmlParagraph.MarginLeft * 20;
        //    paragraph.Align = htmlParagraph.Align;

        //    foreach (IParagraphComponent component in htmlParagraph.Contents)
        //    {
        //        paragraph.Add(component);
        //    }

        //    string expectedString =
        //        "<paragraph indent=\"720\" align=\"right\">" +
        //        "<font face=\"Times New Roman\" size=\"14\">" +
        //        "<b>Bold Text</b>" +
        //        "</font>" +
        //        "</paragraph>";

        //    Assert.AreEqual(expectedString, paragraph.ToXml());
        //}

		[Test]
		public void GetHtml()
		{
			Paragraph paragraph = new Paragraph();
			paragraph.Add("Text");

			string expectedString =
				"<p style=\"margin-left:0pt\" align=\"left\">" +
				"Text" +
				"</p>";

			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}

        //[Ignore("HtmlParagraph obsolete (?)")]
        //[Test]
        //public void GetHtmlFromHtml()
        //{
        //    string htmlString =
        //        "<p style=\"margin-left : 36pt\" align=\"right\">" +
        //        "<span style=\"font-family:'Times New Roman'; font-size:14pt\">" +
        //        "<b>Bold Text</b>" +
        //        "</span>" +
        //        "</p>";

        //    HtmlParagraph htmlParagraph = new HtmlParagraph(htmlString);

        //    Paragraph paragraph = new Paragraph();
        //    paragraph.Indent = htmlParagraph.MarginLeft * 20;
        //    paragraph.Align = htmlParagraph.Align;

        //    foreach (IParagraphComponent component in htmlParagraph.Contents)
        //    {
        //        paragraph.Add(component);
        //    }

        //    string expectedString =
        //        "<p style=\"margin-left:36pt\" align=\"right\">" +
        //        "<span style=\"font-family:'Times New Roman'; font-size:14pt\">" +
        //        "<b>Bold Text</b>" +
        //        "</span>" +
        //        "</p>";

        //    Assert.AreEqual(expectedString, paragraph.ToHtml());
        //}

		[Test]
		public void GetPlainTextHtmlFromXml()
		{
			string xmlString =
				"<paragraph indent=\"720\" align=\"left\">" +
				"Plain text" +
				"</paragraph>";

			IXmlElement element = new XmlElement(xmlString);
			Paragraph paragraph = new Paragraph(element);

			string expectedString =
				"<p style=\"margin-left:36pt\" align=\"left\">" +
				"Plain text" +
				"</p>";

			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}

		[Test]
		public void GetFormattedHtmlFromXml()
		{
			string xmlString =
				"<paragraph indent=\"720\" align=\"left\">" +
				"<b>Bold text</b>" +
				"<i>Italic text</i>" +
				"<u>Underline text</u>" +
				"A mixture of <b><i>Bold and Italic,</i></b><b> now just bold,</b><b><u> now Bold and Underline,</u></b> then plain text" +
				"</paragraph>";

			IXmlElement element = new XmlElement(xmlString);
			Paragraph paragraph = new Paragraph(element);

			string expectedString =
				"<p style=\"margin-left:36pt\" align=\"left\">" +
				"<b>Bold text</b>" +
				"<i>Italic text</i>" +
				"<u>Underline text</u>" +
				"A mixture of <b><i>Bold and Italic,</i></b><b> now just bold,</b><b><u> now Bold and Underline,</u></b> then plain text" +
				"</p>";

			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}

		[Test]
		public void GetParagraphIndentXml()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\"></paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			Assert.AreEqual(0, paragraph.Indent);
			Assert.AreEqual(xmlString, paragraph.ToXml());

			xmlString =	"<paragraph indent=\"720\" align=\"left\"></paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			Assert.AreEqual(720, paragraph.Indent);
			Assert.AreEqual(xmlString, paragraph.ToXml());
		}

		[Test]
		public void GetParagraphAlignXml()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\"></paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			Assert.AreEqual("left", paragraph.Align);
			Assert.AreEqual(xmlString, paragraph.ToXml());

			xmlString = "<paragraph indent=\"0\" align=\"center\"></paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			Assert.AreEqual("center", paragraph.Align);
			Assert.AreEqual(xmlString, paragraph.ToXml());

			xmlString = "<paragraph indent=\"0\" align=\"right\"></paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			Assert.AreEqual("right", paragraph.Align);
			Assert.AreEqual(xmlString, paragraph.ToXml());

			xmlString = "<paragraph indent=\"0\" align=\"justify\"></paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			Assert.AreEqual("justify", paragraph.Align);
			Assert.AreEqual(xmlString, paragraph.ToXml());
		}

		[Test]
		public void GetParagraphIndentHtml()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\">Some text.</paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			string expectedString = "<p style=\"margin-left:0pt\" align=\"left\">Some text.</p>";
			Assert.AreEqual(expectedString, paragraph.ToHtml());

			xmlString = "<paragraph indent=\"720\" align=\"left\">Some text.</paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			expectedString = "<p style=\"margin-left:36pt\" align=\"left\">Some text.</p>";
			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}

		[Test]
		public void GetParagraphAlignHtml()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\">Some text.</paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			string expectedString = "<p style=\"margin-left:0pt\" align=\"left\">Some text.</p>";
			Assert.AreEqual(expectedString, paragraph.ToHtml());

			xmlString = "<paragraph indent=\"0\" align=\"center\">Some text.</paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			expectedString = "<p style=\"margin-left:0pt\" align=\"center\">Some text.</p>";
			Assert.AreEqual(expectedString, paragraph.ToHtml());

			xmlString = "<paragraph indent=\"0\" align=\"right\">Some text.</paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			expectedString = "<p style=\"margin-left:0pt\" align=\"right\">Some text.</p>";
			Assert.AreEqual(expectedString, paragraph.ToHtml());

			xmlString = "<paragraph indent=\"0\" align=\"justify\">Some text.</paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			expectedString = "<p style=\"margin-left:0pt\" align=\"justify\">Some text.</p>";
			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}

		[Test]
		public void GetEmptyParagraphHtml()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\"></paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			string expectedString = "<p style=\"margin-left:0pt\" align=\"left\">&nbsp;</p>";
			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}

		[Test]
		public void GetBoldAndItalicSeparatedBySpaceXml()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\"><b>Bold</b><sp/><i>Italic</i></paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			Assert.AreEqual(3, paragraph.Contents.Count);
			Assert.AreEqual(xmlString, paragraph.ToXml());
		}

		[Test]
		public void GetBoldAndItalicSeparatedBySpaceHtml()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\"><b>Bold</b><sp/><i>Italic</i></paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			string expectedString = "<p style=\"margin-left:0pt\" align=\"left\"><b>Bold</b> <i>Italic</i></p>";
			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}

		[Test]
		public void GetBoldAndItalicSeparatedBySpaceRtf()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\"><b>Bold</b><sp/><i>Italic</i></paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			string expectedString =
				@"\pard " +
				@"\b Bold\b0 " +
				@" \i Italic\i0 " +
				@"\par ";
			Assert.AreEqual(expectedString, paragraph.ToRtf(RtfDocument.NULL));
		}

		[Test]
		public void GetMultipleSpacesHtml()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\">One two  three   four    five     done.</paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			string expectedString = "<p style=\"margin-left:0pt\" align=\"left\">One two &nbsp;three &nbsp; four &nbsp; &nbsp;five &nbsp; &nbsp; done.</p>";
			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}

		[Test]
		public void GetBoldAndItalicSeparatedByMultipleSpacesHtml()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\"><b>Bold</b><sp/><sp/><sp/><sp/><i>Italic</i></paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			string expectedString = "<p style=\"margin-left:0pt\" align=\"left\"><b>Bold</b> &nbsp; &nbsp;<i>Italic</i></p>";
			Assert.AreEqual(expectedString, paragraph.ToHtml());
		}

		[Test]
		public void GetTextFormatsRtf()
		{
			string xmlString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<b>Bold text</b>" +
				"<i>Italic text</i>" +
				"<u>Underline text</u>" +
				"A mixture of <b><i>Bold and Italic,</i></b><b> now just bold,</b><b><u> now Bold and Underline,</u></b> then plain text" +
				"</paragraph>";

			IXmlElement element = new XmlElement(xmlString);
			Paragraph paragraph = new Paragraph(element);

			string expectedString =
				@"\pard " +
				@"\b Bold text\b0 " +
				@"\i Italic text\i0 " +
				@"\ul Underline text\ul0 " +
				@"A mixture of \b \i Bold and Italic,\i0 \b0 \b  now just bold,\b0 \b \ul  now Bold and Underline,\ul0 \b0  then plain text" +
				@"\par ";

			Assert.AreEqual(expectedString, paragraph.ToRtf(RtfDocument.NULL));
		}

		[Test]
		public void GetParagraphIndentRtf()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\">Some text</paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			string expectedString = @"\pard Some text\par ";
			Assert.AreEqual(expectedString, paragraph.ToRtf(RtfDocument.NULL));
			
			xmlString = "<paragraph indent=\"720\" align=\"left\">Some text</paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			expectedString = @"\pard \li720 Some text\par ";
			Assert.AreEqual(expectedString, paragraph.ToRtf(RtfDocument.NULL));
		}

		[Test]
		public void GetParagraphAlignRtf()
		{
			string xmlString = "<paragraph indent=\"0\" align=\"left\">Some text</paragraph>";
			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			string expectedString = @"\pard Some text\par ";
			Assert.AreEqual(expectedString, paragraph.ToRtf(RtfDocument.NULL));

			xmlString = "<paragraph indent=\"0\" align=\"center\">Some text</paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			expectedString = @"\pard \qc Some text\par ";
			Assert.AreEqual(expectedString, paragraph.ToRtf(RtfDocument.NULL));

			xmlString = "<paragraph indent=\"0\" align=\"right\">Some text</paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			expectedString = @"\pard \qr Some text\par ";
			Assert.AreEqual(expectedString, paragraph.ToRtf(RtfDocument.NULL));

			xmlString = "<paragraph indent=\"720\" align=\"justify\">Some text</paragraph>";
			paragraph = new Paragraph(new XmlElement(xmlString));

			expectedString = @"\pard \qj \li720 Some text\par ";
			Assert.AreEqual(expectedString, paragraph.ToRtf(RtfDocument.NULL));
		}

		[Test]
		public void GetTabPositionsRtf()
		{
			string xmlString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<tabPositions>" +
				"<tabStop position=\"1440\"/>" +
				"<tabStop position=\"2880\"/>" +
				"</tabPositions>" +
				"</paragraph>";

			Paragraph paragraph = new Paragraph(new XmlElement(xmlString));

			string expectedRtf =
				@"\pard \tx1440\tx2880\par ";

			Assert.AreEqual(expectedRtf, paragraph.ToRtf(RtfDocument.NULL));
		}
	}
}
