using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.FormDesigner;

using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

using TawalaTest.TestingSupport;

namespace TawalaTest.FormDesignerTest.Browser.Selection
{
	[TestFixture]
	public class FontFaceTest : TestBase
	{
		[Test]
		public void SetOnPartialSelection()
		{
			CreateViewWithTextItem("One Two Three Four");

			SelectViewContents("Two ");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Impact");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One <SPAN style=""FONT-FAMILY: Impact"">Two </SPAN>Three Four</P>";
			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SetWhenPartOfSelectionHasFace()
		{
			CreateViewWithTextItem("<font face=\"Impact\">One </font>Two Three Four");

			Assert.IsTrue(GetParagraphHtmlFromBody().Contains("Impact"));
			SelectViewContents("One Two ");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");

			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">One Two </SPAN>Three Four</P>";
			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SetWhenSelectionAlreadyHasFace()
		{
			CreateViewWithTextItem("<font face=\"Impact\">One</font>");

			SelectViewContents("One");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">One</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SetWhenSelectionHasNestedFaces()
		{
			CreateViewWithTextItem("<font face=\"Arial\"><font face=\"Impact\">One </font>Two</font>");

			SelectViewContents("One Two");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">One Two</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SetWhenSelectionHasNestedPartialFaces()
		{
			CreateViewWithTextItem("<font face=\"Arial\"><font face=\"Impact\">One </font>Two </font>Three");

			SelectViewContents("One Two Three");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">One Two Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordFaceAfterSettingParagraphFaceProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three");

			SelectViewContents("One Two Three");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Two");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Arial");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">One&nbsp;<SPAN style=""FONT-FAMILY: Arial"">Two</SPAN> Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordFaceAfterSettingPhraseFaceProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three Four Five");

			SelectViewContents("Two Three Four");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Three");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Arial");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One&nbsp;<SPAN style=""FONT-FAMILY: Courier New"">Two&nbsp;<SPAN style=""FONT-FAMILY: Arial"">Three</SPAN> Four</SPAN> Five</P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordFaceAfterSettingParagraphSizeProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three");

			SelectViewContents("One Two Three");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontSize(24);
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Two");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 24pt"">One&nbsp;<SPAN style=""FONT-FAMILY: Courier New"">Two</SPAN> Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordFaceAfterSettingParagraphColorProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three");

			SelectViewContents("One Two Three");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("FF0000");
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Two");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #ff0000"">One&nbsp;<SPAN style=""FONT-FAMILY: Courier New"">Two</SPAN> Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordFaceAfterSettingPhraseSizeProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three Four Five");

			SelectViewContents("Two Three Four");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontSize(24);
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Three");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One&nbsp;<SPAN style=""FONT-SIZE: 24pt"">Two&nbsp;<SPAN style=""FONT-FAMILY: Courier New"">Three</SPAN> Four</SPAN> Five</P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingWordFaceAfterSettingPhraseColorProducesExpectedHtml()
		{
			CreateViewWithTextItem("One Two Three Four Five");

			SelectViewContents("Two Three Four");
			HtmlSelection selection = new HtmlSelection(View.GetSelection());
			selection.SetFontColor("FF0000");
			View.SetSelection(selection.ToXhtml());

			SelectViewContents("Three");
			selection = new HtmlSelection(View.GetSelection());
			selection.SetFontFace("Courier New");
			View.SetSelection(selection.ToXhtml());

			string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One&nbsp;<SPAN style=""COLOR: #ff0000"">Two&nbsp;<SPAN style=""FONT-FAMILY: Courier New"">Three</SPAN> Four</SPAN> Five</P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}
	}
}
