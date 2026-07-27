using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.Projects.Forms.FormItemContents;
using TawalaTest.TestingSupport;
using Tawala.Common;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class TextItemContentsConversionTest : ModelOrientedTestBase
	{
		[SetUp]
		public void SetUp()
		{
			ModelTestSetUp();
		}

		[TearDown]
		public void TearDown()
		{
			ModelTestTearDown();
		}

		[Test]
		public void CanConvertSimpleParagraphContentsToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\">Text</P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"left\">Text</paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertSimpleParagraphContentsToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font>Text</font></paragraph></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 0pt\" align=\"left\">Text</p>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertParagraphContentsWithBoldToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\"><STRONG>Text</STRONG></P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"left\"><b>Text</b></paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertParagraphContentsWithBoldToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"0\" align=\"left\"><b>Text</b></paragraph></container>";

			IXmlElement xmlElement = new XmlElement(xmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 0pt\" align=\"left\"><strong>Text</strong></p>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertParagraphContentsWithItalicToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\"><EM>Text</EM></P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"left\"><i>Text</i></paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertParagraphContentsWithItalicToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"0\" align=\"left\"><i>Text</i></paragraph></container>";

			IXmlElement xmlElement = new XmlElement(xmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 0pt\" align=\"left\"><em>Text</em></p>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertParagraphContentsWithUnderlineToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\"><U>Text</U></P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"left\"><u>Text</u></paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertParagraphContentsWithUnderlineToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"0\" align=\"left\"><u>Text</u></paragraph></container>";

			IXmlElement xmlElement = new XmlElement(xmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 0pt\" align=\"left\"><u>Text</u></p>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertParagraphContentsWithSeparateBoldAndUnderlineToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\"><STRONG>Bold</STRONG><U>Text</U></P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"left\"><b>Bold</b><u>Text</u></paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertParagraphContentsWithNestedInlineStylesToXml()
		{
			string htmlString =
				"<container>" +
				"<P style=\"MARGIN-LEFT: 0pt\">" +
				"<STRONG>" +
				"<U>" +
				"<EM>" +
				"BoldUnderlineItalic" +
				"</EM>" +
				"BoldUnderline" +
				"</U>" +
				"Bold" +
				"</STRONG>" +
				"Plain" +
				"</P>" +
				"</container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			string expectedXml =
				"<container>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<b>" +
				"<u>" +
				"<i>" +
				"BoldUnderlineItalic" +
				"</i>" +
				"BoldUnderline" +
				"</u>" +
				"Bold" +
				"</b>" +
				"Plain" +
				"</paragraph>" +
				"</container>";

			Assert.AreEqual(expectedXml, formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertParagraphContentsWithNestedInlineStylesToXhtml()
		{
			string xmlString =
				"<container>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font>" +
				"<b>" +
				"<u>" +
				"<i>" +
				"BoldUnderlineItalic" +
				"</i>" +
				"BoldUnderline" +
				"</u>" +
				"Bold" +
				"</b>" +
				"Plain" +
				"</font>" +
				"</paragraph>" +
				"</container>";

			IXmlElement xmlElement = new XmlElement(xmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			string expectedXhtml =
				"<p style=\"margin-left: 0pt\" align=\"left\">" +
				"<strong>" +
				"<u>" +
				"<em>" +
				"BoldUnderlineItalic" +
				"</em>" +
				"BoldUnderline" +
				"</u>" +
				"Bold" +
				"</strong>" +
				"Plain" +
				"</p>";

			Assert.AreEqual(expectedXhtml, formItemContentsConverter.ToXhtml(FormItem.NULL));
		}


		[Test]
		public void CanConvertIndentedParagraphToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 36pt\">Text</P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"720\" align=\"left\">Text</paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertIndentedParagraphToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"720\" align=\"left\">Text</paragraph></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 36pt\" align=\"left\">Text</p>", formItemContentsConverter.ToXhtml(FormItem.NULL));			
		}

		[Test]
		public void CanConvertMultipleParagraphContentsToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\">Text</P><P style=\"MARGIN-LEFT: 0pt\">Text</P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			string expectedXml =
				"<container>" +
				"<paragraph indent=\"0\" align=\"left\">Text</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">Text</paragraph>" +
				"</container>";

			Assert.AreEqual(expectedXml, formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertMultipleParagraphContentsToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"0\" align=\"left\">Line One</paragraph><paragraph indent=\"0\" align=\"left\">Line Two</paragraph></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 0pt\" align=\"left\">Line One</p><p style=\"margin-left: 0pt\" align=\"left\">Line Two</p>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertTableContentsToXml()
		{
			string htmlString = 
				"<container>" +
				"<P style=\"MARGIN-LEFT: 0pt\">" + Environment.NewLine +
				"<TABLE class=user>" + Environment.NewLine +
				"<TBODY>" + Environment.NewLine +
				"<TR style=\"HEIGHT: 12pt\">" + Environment.NewLine +
				"<TD style=\"WIDTH: 108pt\">R1C1</TD>" + Environment.NewLine +
				"<TD style=\"WIDTH: 108pt\">R1C2</TD></TR>" + Environment.NewLine +
				"<TR style=\"HEIGHT: 12pt\">" + Environment.NewLine +
				"<TD style=\"WIDTH: 108pt\">R2C1</TD>" + Environment.NewLine +
				"<TD style=\"WIDTH: 108pt\">R2C2</TD></TR></TBODY></TABLE></P>" +
				"</container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			string expectedXml =
				"<container>" +
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"2160\"><division indent=\"0\" align=\"left\"><font>R1C1</font></division></cell>" +
				"<cell width=\"2160\"><division indent=\"0\" align=\"left\"><font>R1C2</font></division></cell>" + 
				"</row>" + 
				"<row>" +
				"<cell width=\"2160\"><division indent=\"0\" align=\"left\"><font>R2C1</font></division></cell>" +
				"<cell width=\"2160\"><division indent=\"0\" align=\"left\"><font>R2C2</font></division></cell>" + 
				"</row>" + 
				"</table>" + 
				"</container>";

			Assert.AreEqual(expectedXml, formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertTableContentsToXhtml()
		{
			string xmlString =
				"<container>" +
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"2160\"><division indent=\"0\" align=\"left\"><font>R1C1</font></division></cell>" +
				"<cell width=\"2160\"><division indent=\"0\" align=\"left\"><font>R1C2</font></division></cell>" +
				"</row>" +
				"<row>" +
				"<cell width=\"2160\"><division indent=\"0\" align=\"left\"><font>R2C1</font></division></cell>" +
				"<cell width=\"2160\"><division indent=\"0\" align=\"left\"><font>R2C2</font></division></cell>" +
				"</row>" +
				"</table>" +
				"</container>";

			IXmlElement xmlElement = new XmlElement(xmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);


			string expectedHtml =
				"<table class=\"user\" style=\"width: 216pt;\">" +
				"<tbody>" +
				"<tr style=\"height: 12pt\">" +
				"<td style=\"width: 108pt\">R1C1</td>" +
				"<td style=\"width: 108pt\">R1C2</td></tr>" +
				"<tr style=\"height: 12pt\">" +
				"<td style=\"width: 108pt\">R2C1</td>" +
				"<td style=\"width: 108pt\">R2C2</td></tr></tbody></table>";

			Assert.AreEqual(expectedHtml, formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertParagraphContentsWithSpecialCharactersToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\">&amp;, &lt; and &gt; are special characters.</P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"left\">&amp;, &lt; and &gt; are special characters.</paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertSimpleParagraphContentsWithSpecialCharactersToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font>&amp;, &lt; and &gt; are special characters.</font></paragraph></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 0pt\" align=\"left\">&amp;, &lt; and &gt; are special characters.</p>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertParagraphContentsWithInlineStylesAndSpacesCharactersToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\">Formatting:  <STRONG>bold</STRONG> <EM>italic</EM> <U>underline</U>.</P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"left\">Formatting:  <b>bold</b><sp/><i>italic</i><sp/><u>underline</u>.</paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertParagraphContentsWithInlineStylesAndSpacesCharactersToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"0\" align=\"left\"><font>Formatting:  <b>bold</b><sp/><i>italic</i><sp/><u>underline</u>.</font></paragraph></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 0pt\" align=\"left\">Formatting:  <strong>bold</strong> <em>italic</em> <u>underline</u>.</p>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

        [Test]
		public void CanConvertFieldToXml()
		{
			form.ItemList.Add(new NewFibItem());

			string htmlString =
				"<container><P style=\"MARGIN-LEFT: 0pt\">" +
				"<t:field class=field id=\"\" contentEditable=\"false\" fieldID=\"1014\">&lt;&lt;Form 1:Q1:a&gt;&gt;</t:field>" +
				"</P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"left\"><field name=\"Form 1:Q1:a\"/></paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertFieldToXhtml()
		{
			Util.NewTestProject();

			IForm form = Project.Current.AddForm();
			IFibItem fibItem = new NewFibItem();
			form.ItemList.Add(fibItem as IFormItem);

			string xmlString = "<container><paragraph indent=\"0\" align=\"left\"><field name=\"Form 1:Q1:a\"/></paragraph></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			int fieldId = fibItem.BlankList[0].Id;

			string expectedXhtml =
				String.Format("<p style=\"margin-left: 0pt\" align=\"left\"><t:field fieldID=\"{0}\">Form 1:Q1:a</t:field></p>", fieldId);

			Assert.AreEqual(expectedXhtml, formItemContentsConverter.ToXhtml(fibItem as FormItem));
		}

		[Test]
		public void CanConvertParagraphContentsWithAlignLeftToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\" align=left>Text</P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"left\">Text</paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertParagraphContentsWithAlignLeftToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font>Text</font></paragraph></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 0pt\" align=\"left\">Text</p>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertParagraphContentsWithAlignCenterToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\" align=center>Text</P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"center\">Text</paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertParagraphContentsWithAlignCenterToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"0\" align=\"center\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font>Text</font></paragraph></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 0pt\" align=\"center\">Text</p>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertParagraphContentsWithAlignRightToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\" align=right>Text</P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"right\">Text</paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertParagraphContentsWithAlignRightToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"0\" align=\"right\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font>Text</font></paragraph></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 0pt\" align=\"right\">Text</p>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertParagraphContentsWithAlignJustifyToXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\" align=justify>Text</P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"justify\">Text</paragraph></container>", formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertParagraphContentsWithAlignJustifyToXhtml()
		{
			string xmlString = "<container><paragraph indent=\"0\" align=\"justify\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font>Text</font></paragraph></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<p style=\"margin-left: 0pt\" align=\"justify\">Text</p>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertParagraphContentsWithoutAlignToAlignLeftXml()
		{
			string htmlString = "<container><P style=\"MARGIN-LEFT: 0pt\">Text</P></container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			Assert.AreEqual("<container><paragraph indent=\"0\" align=\"left\">Text</paragraph></container>", formItemContentsConverter.ToXml());
		}
	}
}
