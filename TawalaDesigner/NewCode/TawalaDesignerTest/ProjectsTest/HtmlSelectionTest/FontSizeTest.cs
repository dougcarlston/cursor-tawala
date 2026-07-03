using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest.HtmlSelectionTest
{
	[TestFixture]
	public class FontSizeTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void CanSetSizeInSimpleSelection()
		{
			string htmlText = "Selection";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(16);

			string expectedXhtml = @"<span style=""font-size: 16pt;"">Selection</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void CanSetSizeInSimpleSelectionWithExistingAttributes()
		{
			string htmlText = @"<SPAN style=""FONT-FAMILY: Courier New;"">Selection</SPAN>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(18);

			string expectedXhtml = @"<span style=""font-family: Courier New;font-size: 18pt;"">Selection</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void CanSetSizeInSimpleSelectionWithExistingAttributesAndBold()
		{
			string htmlText = @"<SPAN style=""FONT-FAMILY: Courier New;""><STRONG>Selection</STRONG></SPAN>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(18);

			string expectedXhtml = @"<span style=""font-family: Courier New;font-size: 18pt;""><strong>Selection</strong></span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void CanSetSizeInSelectionBridgingTwoParagraphs()
		{
			string htmlText =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>Paragraph One</P>" +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>Paragraph Two</P>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(16);

			string expectedXhtml =
				@"<span style=""font-size: 16pt;"">Paragraph One</span></p>" +
				@"<p style=""margin-left: 0pt"" align=""left""><span style=""font-size: 16pt;"">Paragraph Two</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void CanSetSizeInSelectionBridgingTwoParagraphsWithExistingAttributesInOne()
		{
			string htmlText =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New; FONT-SIZE: 12pt; COLOR: #FF0000;"">Paragraph One</SPAN></P>" +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>Paragraph Two</P>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(18);

			string expectedXhtml =
				@"<span style=""font-family: Courier New;font-size: 18pt;color: #FF0000;"">Paragraph One</span></p>" +
				@"<p style=""margin-left: 0pt"" align=""left""><span style=""font-size: 18pt;"">Paragraph Two</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void CanSetSizeInSelectionBridgingTwoParagraphsWithExistingAttributesInBoth()
		{
			string htmlText =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New; FONT-SIZE: 12pt; COLOR: #FF0000;"">Paragraph One</SPAN></P>" +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New; FONT-SIZE: 12pt; COLOR: #FF0000;"">Paragraph Two</SPAN></P>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(18);

			string expectedXhtml =
				@"<span style=""font-family: Courier New;font-size: 18pt;color: #FF0000;"">Paragraph One</span></p>" +
				@"<p style=""margin-left: 0pt"" align=""left""><span style=""font-family: Courier New;font-size: 18pt;color: #FF0000;"">Paragraph Two</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void CanSetSizeInSelectionBridgingTwoParagraphsWithExistingPartialColorInBoth()
		{
			string htmlText =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>Paragraph <SPAN style=""COLOR: #00FF00;"">One</SPAN></P>" +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #00FF00;"">Paragraph</SPAN> Two</P>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(18);

			string expectedXhtml =
				@"<span style=""font-size: 18pt;"">Paragraph <span style=""color: #00FF00;"">One</span></span></p>" +
				@"<p style=""margin-left: 0pt"" align=""left""><span style=""font-size: 18pt;""><span style=""color: #00FF00;"">Paragraph</span> Two</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void CanSetSizeInSelectionWithExistingPartialFontSize()
		{
			string htmlText = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt;"">One </SPAN>Two</P>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(24);

			string expectedXhtml =
				@"<span style=""font-size: 24pt;"">One Two</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void SettingSizeInSelectionWithExistingPartialSizeReplacesSize()
		{
			string htmlText = @"<SPAN style=""FONT-SIZE: 16pt;"">One </SPAN>Two";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(24);

			string expectedXhtml =
				@"<span style=""font-size: 24pt;"">One Two</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void SettingSizeInSelectionWithColorPreservesSizeAndColor()
		{
			string htmlText =
				@"<SPAN style=""COLOR: #FF0000"">One </SPAN>Two";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(18);

			string expectedXhtml =
				@"<span style=""font-size: 18pt;""><span style=""color: #FF0000;"">One </span>Two</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void SettingSizeInParagraphSelectionWithColorPreservesSizeAndColor()
		{
			string htmlText =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #FF0000"">One </SPAN>Two</P>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(18);

			string expectedXhtml =
				@"<span style=""font-size: 18pt;""><span style=""color: #FF0000;"">One </span>Two</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void SettingSizeInSimpleSelectionWithDifferentSizeReplacesSize()
		{
			string htmlText = @"<SPAN style=""FONT-SIZE: 12pt;"">Selection</SPAN>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(16);

			string expectedXhtml = @"<span style=""font-size: 16pt;"">Selection</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void SettingSizeInSimpleSelectionWithSameSizeReplacesSize()
		{
			string htmlText = @"<SPAN style=""FONT-SIZE: 16pt;"">Selection</SPAN>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(16);

			string expectedXhtml = @"<span style=""font-size: 16pt;"">Selection</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}
	}
}
