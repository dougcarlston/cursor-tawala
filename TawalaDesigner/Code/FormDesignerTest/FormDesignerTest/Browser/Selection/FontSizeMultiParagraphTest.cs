using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.FormDesigner;

using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.XmlSupport;

using TawalaTest.TestSupport;

namespace TawalaTest.FormDesignerTest.Browser.Selection
{
	[TestFixture]
	public class FontSizeMultiParagraphTest : TestBase
	{
		[Test]
		public void SetOnPartialCrossParagraphSelectionWithTrailingSpace()
		{
			CreateViewWithTextItem("One Two", "Three Four");

			setSizeOnMultiParagraphViewSelection(24, "Two", "Three ");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>One <SPAN style=""FONT-SIZE: 24pt"">Two</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 24pt"">Three </SPAN>Four</P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SetWhenMultiParagraphSelectionAlreadyHasSize()
		{
			CreateViewWithTextItem("<font size=\"480\">One</font>", "<font size=\"480\">Two</font>");

			setSizeOnMultiParagraphViewSelection(16, "One", "Two");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">Two</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SetWhenPartialCrossParagraphSelectionHasSize()
		{
			CreateViewWithTextItem("One <font size=\"480\">Two</font>", "<font size=\"480\">Three</font> Four");

			setSizeOnMultiParagraphViewSelection(16, "Two", "Three");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>One&nbsp;<SPAN style=""FONT-SIZE: 16pt"">Two</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">Three</SPAN> Four</P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingMiddleParagraphSizeAfterSettingMultiParagraphSizeProducesExpectedHtml()
		{
			CreateViewWithTextItem("One", "Two", "Three");

			SelectViewContentsAcrossParagraphs("One", "Two", "Three");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontSize(16);
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Two");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontSize(24);
			View.SetSelection(selection.ToXhtml());

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 24pt"">Two</SPAN></P>" +
				Environment.NewLine + 
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingMiddleParagraphSizeAfterSettingMultiParagraphColorProducesExpectedHtml()
		{
			CreateViewWithTextItem("One", "Two", "Three");

			SelectViewContentsAcrossParagraphs("One", "Two", "Three");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("FF0000");
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Two");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontSize(24);
			View.SetSelection(selection.ToXhtml());

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #ff0000"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 24pt; COLOR: #ff0000"">Two</SPAN></P>" +
				Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #ff0000"">Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingMiddleParagraphSizeAfterSettingMultiParagraphFaceProducesExpectedHtml()
		{
			CreateViewWithTextItem("One", "Two", "Three");

			SelectViewContentsAcrossParagraphs("One", "Two", "Three");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Two");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontSize(24);
			View.SetSelection(selection.ToXhtml());

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 24pt; FONT-FAMILY: Courier New"">Two</SPAN></P>" +
				Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingPartialMultiParagraphSizeAfterSettingEntireMultiParagraphSizeProducesUnexpectedHtml()
		{
			CreateViewWithTextItem("One", "Two Three Four", "Five Six");

			setSizeOnMultiParagraphViewSelection(32, "One", "Five Six");
			setSizeOnMultiParagraphViewSelection(24, "Three Four", "Five");

			// note: browser control produces HTML with unbalanced span tags
			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 32pt"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 32pt"">Two&nbsp;<SPAN style=""FONT-SIZE: 24pt"">Three Four</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 24pt"">Five</SPAN></SPAN><SPAN style=""FONT-SIZE: 32pt""> Six</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

	}
}
