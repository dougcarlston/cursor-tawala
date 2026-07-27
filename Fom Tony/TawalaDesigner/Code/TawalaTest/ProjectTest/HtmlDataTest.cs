// $Workfile: HtmlDataTest.cs $
// $Revision: 4 $	$Date: 5/10/06 10:51a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the HtmlData class
	/// </summary>
	[TestFixture]
	public class HtmlDataTest
	{
		[Test]
		public void ConstructFromXml() 
		{
			string xmlString =
				"<htmlData>" +
				"<![CDATA[" +
				"Something inside the CDATA block" +
				"]]>" +
				"</htmlData>";

			IXmlElement element = new XmlElement(xmlString);
			HtmlData data = new HtmlData(element);

			Assert.AreEqual("Something inside the CDATA block", data.Text);
		}

		[Test]
		public void MissingCDATA()
		{
			string xmlString =
				"<htmlData>" +
				"Something inside the CDATA block" +
				"</htmlData>";

			IXmlElement element = new XmlElement(xmlString);
			HtmlData data = new HtmlData(element);

			Assert.AreEqual("", data.Text);
		}

		[Test]
		public void ExtractText()
		{
			string xmlString =
				"<htmlData>" +
				"<![CDATA[" +
				"Something inside the CDATA block" +
				"]]>" +
				"</htmlData>";

			IXmlElement element = new XmlElement(xmlString);

			// tests static method
			Assert.AreEqual("Something inside the CDATA block", HtmlData.ExtractText(element));
		}

		[Test]
		public void ExtractTextWithoutStyleData()
		{
			string xmlString =
				"<rawHtmlData>\r\n" +
				"<![CDATA[<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n" +
				"<html>\r\n" +
				"<head>\r\n" +
				"<meta content=\"TX_HTML32 12.0.230.500\" name=\"GENERATOR\">\r\n" +
				"<title></title>\r\n" +
				"<style type=\"text/css\"> BODY {font-family: 'Arial';font-size: 12pt;font-weight: normal;font-style: normal;}\r\n" +
				" P {margin-top: 0.05pt;margin-bottom: 0.05pt;font-family: 'Arial';font-size: 12pt;font-style: normal;}\r\n" +
				"</style></head>\r\n" +
				"<body bgcolor=\"#FFFFFF\" text=\"#000000\">\r\n" +
				"<p><span style=\"font-size:10pt;\">Simple document with plain text.</span></p></body>\r\n" +
				"</html>]]>\r\n" +
				"</rawHtmlData>";

			IXmlElement element = new XmlElement(xmlString);

			string expectedText =
				"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n" +
				"<html>\r\n" +
				"<head>\r\n" +
				"<meta content=\"TX_HTML32 12.0.230.500\" name=\"GENERATOR\">\r\n" +
				"<title></title>\r\n" +
				"</head>\r\n" +
				"<body bgcolor=\"#FFFFFF\" text=\"#000000\">\r\n" +
				"<p><span style=\"font-size:10pt;\">Simple document with plain text.</span></p></body>\r\n" +
				"</html>";

			// tests static method
			Assert.AreEqual(expectedText, HtmlData.ExtractText(element));
		}

		[Test]
		public void ConvertPlainTextToPlainTextRtf()
		{
			string xmlString =
				"<rawHtmlData>\r\n" +
				"<![CDATA[<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n" +
				"<html>\r\n" +
				"<head>\r\n" +
				"<meta content=\"TX_HTML32 12.0.230.500\" name=\"GENERATOR\">\r\n" +
				"<title></title>\r\n" +
				"<style type=\"text/css\"> BODY {font-family: 'Arial';font-size: 12pt;font-weight: normal;font-style: normal;}\r\n" +
				" P {margin-top: 0.05pt;margin-bottom: 0.05pt;font-family: 'Arial';font-size: 12pt;font-style: normal;}\r\n" +
				"</style></head>\r\n" +
				"<body bgcolor=\"#FFFFFF\" text=\"#000000\">\r\n" +
				"<p><span style=\"font-size:10pt;\">Simple document with plain text.</span></p></body>\r\n" +
				"</html>]]>\r\n" +
				"</rawHtmlData>";

			IXmlElement element = new XmlElement(xmlString);

			string expectedRtf = @"Simple document with plain text.\par ";
			//rtfStringPrefix +
				//@"{\pard\itap0\sb2\sa2\plain\f0\fs20 Simple document with plain text.\par }";

			Assert.AreEqual(expectedRtf, HtmlData.ConvertToPlainTextRtf(element));
		}

		[Test]
		public void ConvertFormattedTextToPlainTextRtf()
		{
			string xmlString =
				"<rawHtmlData>\r\n" +
				"<![CDATA[<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n" +
				"<html>\r\n" +
				@"<head>" + "\r\n" +
				@"<meta content=""TX_HTML32 12.0.230.500"" name=GENERATOR>" + "\r\n" +
				@"<title></title>" + "\r\n" +
				@"<style type=""text/css""> BODY {font-family: 'Arial';font-size: 12pt;font-weight: normal;f" +
				@"ont-style: normal;}" + "\r\n" +
				@" P {margin-top: 0.05pt;margin-bottom: 0.05pt;font-family: 'Arial';font-size: 12pt;font-s" +
				@"tyle: normal;}" + "\r\n" +
				@"</style></head>" + "\r\n" +
				@"<body bgcolor=""#FFFFFF"" text=""#000000"">" + "\r\n" +
				@"<p><span style=""font-size:10pt;"">Here is a paragraph.</span></p>" + "\r\n" +
				@"<p><span style=""font-size:10pt;"">And another</span></p>" + "\r\n" +
				@"<p><span style=""font-size:10pt;""></span></p>" + "\r\n" +
				@"<p><span style=""font-size:10pt;"">&nbsp;</span></p>" + "\r\n" +
				@"<p><span style=""font-size:10pt;"">Two lines between.</span></p>" + "\r\n" +
				@"<p><span style=""font-size:10pt;"">With </span><span style=""font-size:10pt;""><b>bold </b><" +
				@"/span><span style=""font-size:10pt;""><b><i>and bold italic</i></b></span><span style=""fon" +
				@"t-size:10pt;""> then plain.</span></p>" + "\r\n" +
				@"<p style=""margin-left: 36pt;""><span style=""font-size:10pt;"">Indented.</span></p><p align" +
				@"= ""center""><span style=""font-size:10pt;"">Centered.</span></p></body>" + "\r\n" +
				"</html>]]>\r\n" +
				"</rawHtmlData>";

			IXmlElement element = new XmlElement(xmlString);

			string expectedRtf =
				@"Here is a paragraph.\par " +
				@"And another\par " +
				@"\par " +
				@"Two lines between.\par " +
				@"With bold and bold italic then plain.\par " +
				@"Indented.\par " +
				@"Centered.\par ";

			Assert.AreEqual(expectedRtf, HtmlData.ConvertToPlainTextRtf(element));
		}

		[Test]
		public void ConvertFieldToPlainTextRtf()
		{
			string xmlString =
				"<rawHtmlData>\r\n" +
				"<![CDATA[<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n" +
				"<html>\r\n" +
				"<head>\r\n" +
				"<meta content=\"TX_HTML32 12.0.230.500\" name=\"GENERATOR\">\r\n" +
				"<title></title>\r\n" +
				"<style type=\"text/css\"> BODY {font-family: 'Arial';font-size: 12pt;font-weight: normal;font-style: normal;}\r\n" +
				" P {margin-top: 0.05pt;margin-bottom: 0.05pt;font-family: 'Arial';font-size: 12pt;font-style: normal;}\r\n" +
				"</style></head>\r\n" +
				"<body bgcolor=\"#FFFFFF\" text=\"#000000\">\r\n" +
				"<p><span style=\"font-size:10pt;\">Text with a field &lt;&lt;Q1:a&gt;&gt; in it.</span></p></body>\r\n" +
				"</html>]]>\r\n" +
				"</rawHtmlData>";

			IXmlElement element = new XmlElement(xmlString);

			string expectedRtf = @"Text with a field <<Q1:a>> in it.\par ";

			Assert.AreEqual(expectedRtf, HtmlData.ConvertToPlainTextRtf(element));
		}

		[Test]
		public void ConvertAmpersandToPlainTextRtf()
		{
			string xmlString =
				"<rawHtmlData>\r\n" +
				"<![CDATA[<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n" +
				"<html>\r\n" +
				"<head>\r\n" +
				"<meta content=\"TX_HTML32 12.0.230.500\" name=\"GENERATOR\">\r\n" +
				"<title></title>\r\n" +
				"<style type=\"text/css\"> BODY {font-family: 'Arial';font-size: 12pt;font-weight: normal;font-style: normal;}\r\n" +
				" P {margin-top: 0.05pt;margin-bottom: 0.05pt;font-family: 'Arial';font-size: 12pt;font-style: normal;}\r\n" +
				"</style></head>\r\n" +
				"<body bgcolor=\"#FFFFFF\" text=\"#000000\">\r\n" +
				"<p><span style=\"font-size:10pt;\">Text with an ampersand (&amp;) in it.</span></p></body>\r\n" +
				"</html>]]>\r\n" +
				"</rawHtmlData>";

			IXmlElement element = new XmlElement(xmlString);

			string expectedRtf = @"Text with an ampersand (&) in it.\par ";

			Assert.AreEqual(expectedRtf, HtmlData.ConvertToPlainTextRtf(element));
		}
	}
}
