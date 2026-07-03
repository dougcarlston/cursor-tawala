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
	public class FontColorTest : TestBase
	{
		[Test]
		public void SetOnPartialSelection()
		{
			CreateViewWithTextItem("One Two Three Four");

			SelectViewContents("Two ");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("FF0000");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One <SPAN style=""COLOR: #ff0000"">Two </SPAN>Three Four</P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SetWhenPartOfSelectionHasColor()
		{
			CreateViewWithTextItem("<font color=\"FF0000\">One </font>Two Three Four");

			SelectViewContents("One Two ");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("00FF00");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #00ff00"">One Two </SPAN>Three Four</P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SetWhenSelectionAlreadyHasColor()
		{
			CreateViewWithTextItem("<font color=\"FF0000\">One</font>");

			SelectViewContents("One");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("00FF00");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #00ff00"">One</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SetWhenSelectionHasNestedColors()
		{
			CreateViewWithTextItem("<font color=\"0000FF\"><font color=\"FF0000\">One </font>Two</font>");

			SelectViewContents("One Two");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("00FF00");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #00ff00"">One Two</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SetWhenSelectionHasNestedPartialColors()
		{
			CreateViewWithTextItem("<font color=\"0000FF\"><font color=\"FF0000\">One </font>Two </font>Three");

			SelectViewContents("One Two Three");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("00FF00");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #00ff00"">One Two Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordColorAfterSettingParagraphColorProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three");

			SelectViewContents("One Two Three");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("00FF00");
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Two");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("FF0000");

			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #00ff00"">One&nbsp;<SPAN style=""COLOR: #ff0000"">Two</SPAN> Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordColorAfterSettingPhraseColorProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three Four Five");

			SelectViewContents("Two Three Four");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("00FF00");
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Three");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("FF0000");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One&nbsp;<SPAN style=""COLOR: #00ff00"">Two&nbsp;<SPAN style=""COLOR: #ff0000"">Three</SPAN> Four</SPAN> Five</P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordColorAfterSettingParagraphSizeProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three");

			SelectViewContents("One Two Three");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontSize(24);
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Two");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("FF0000");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 24pt"">One&nbsp;<SPAN style=""COLOR: #ff0000"">Two</SPAN> Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordColorAfterSettingParagraphFaceProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three");

			SelectViewContents("One Two Three");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Two");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("FF0000");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">One&nbsp;<SPAN style=""COLOR: #ff0000"">Two</SPAN> Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordColorAfterSettingPhraseSizeProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three Four Five");

			SelectViewContents("Two Three Four");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontSize(24);
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Three");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("FF0000");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One&nbsp;<SPAN style=""FONT-SIZE: 24pt"">Two&nbsp;<SPAN style=""COLOR: #ff0000"">Three</SPAN> Four</SPAN> Five</P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordColorAfterSettingPhraseFaceProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three Four Five");

			SelectViewContents("Two Three Four");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Three");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("FF0000");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One&nbsp;<SPAN style=""FONT-FAMILY: Courier New"">Two&nbsp;<SPAN style=""COLOR: #ff0000"">Three</SPAN> Four</SPAN> Five</P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}
	}
}
