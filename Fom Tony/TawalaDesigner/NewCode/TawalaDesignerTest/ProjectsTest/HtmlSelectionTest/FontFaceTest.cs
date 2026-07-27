using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest.HtmlSelectionTest
{
	[TestFixture]
	public class FontFaceTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void SettingFaceInSelectionWithExistingPartialFaceReplacesFace()
		{
			string htmlText = @"<SPAN style=""FONT-FAMILY: Arial;"">One </SPAN>Two";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontFace("Courier New");

			string expectedXhtml =
				@"<span style=""font-family: Courier New;"">One Two</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void SettingFaceInSelectionWithSizesSetsFaaceWithoutChangingSizes()
		{
			string htmlText = @"<SPAN style=""FONT-SIZE: 16pt;""><SPAN style=""FONT-SIZE: 28pt;"">One </SPAN>Two </SPAN>Three ";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontFace("Arial");

			string expectedXhtml =
				@"<span style=""font-family: Arial;""><span style=""font-size: 16pt;""><span style=""font-size: 28pt;"">One </span>Two </span>Three </span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void SettingFaceInSelectionWithColorsSetsFaceWithoutChangingColors()
		{
			string htmlText = @"<SPAN style=""COLOR: #F0F0F0;""><SPAN style=""COLOR: #00FF00;"">One </SPAN>Two </SPAN>Three ";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontFace("Arial");

			string expectedXhtml =
				@"<span style=""font-family: Arial;""><span style=""color: #F0F0F0;""><span style=""color: #00FF00;"">One </span>Two </span>Three </span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void SettingFaceInMultipleParagraphSelectionWithSizeAndColorPreservesSizeAndColor()
		{
			string htmlText =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>Paragraph <SPAN style=""COLOR: #FF0000"">One</SPAN></P>" +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 14pt""><SPAN style=""COLOR: #FF0000"">Paragraph</SPAN> Two</SPAN></P>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontFace("Impact");

			string expectedXhtml =
				@"<span style=""font-family: Impact;"">Paragraph <span style=""color: #FF0000;"">One</span></span></p>" +
				@"<p style=""margin-left: 0pt"" align=""left""><span style=""font-family: Impact;font-size: 14pt;""><span style=""color: #FF0000;"">Paragraph</span> Two</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}
	}
}
