// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.FormDesignerTest.Browser.Selection
{
    [TestFixture]
    public class FontSizeTest : TestBase
    {
        [Test]
        public void SetOnPartialSelection()
        {
            CreateViewWithTextItem("One Two Three Four");

            SelectViewContents("Two ");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(24);
            View.SetSelection(selection.ToXhtml());

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One <SPAN style=""FONT-SIZE: 24pt"">Two </SPAN>Three Four</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void SettingWordSizeAfterSettingParagraphColorProducesExpectedHtml()
        {
            CreateViewWithTextItem("One Two Three");

            SelectViewContents("One Two Three");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontColor("FF0000");
            View.SetSelection(selection.ToXhtml());

            SelectViewContents("Two");
            selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(24);
            View.SetSelection(selection.ToXhtml());

            string expectedHtml =
                @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #ff0000"">One&nbsp;<SPAN style=""FONT-SIZE: 24pt"">Two</SPAN> Three</SPAN></P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void SettingWordSizeAfterSettingParagraphFaceProducesExpectedHtml()
        {
            CreateViewWithTextItem("One Two Three");

            SelectViewContents("One Two Three");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontFace("Courier New");
            View.SetSelection(selection.ToXhtml());

            SelectViewContents("Two");
            selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(24);
            View.SetSelection(selection.ToXhtml());

            string expectedHtml =
                @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">One&nbsp;<SPAN style=""FONT-SIZE: 24pt"">Two</SPAN> Three</SPAN></P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void SettingWordSizeAfterSettingParagraphSizeProducesExpectedHtml()
        {
            CreateViewWithTextItem("One Two Three");

            SelectViewContents("One Two Three");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(16);
            View.SetSelection(selection.ToXhtml());

            SelectViewContents("Two");
            selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(24);

            View.SetSelection(selection.ToXhtml());

            string expectedHtml =
                @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">One&nbsp;<SPAN style=""FONT-SIZE: 24pt"">Two</SPAN> Three</SPAN></P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void SettingWordSizeAfterSettingPhraseColorProducesExpectedHtml()
        {
            CreateViewWithTextItem("One Two Three Four Five");

            SelectViewContents("Two Three Four");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontColor("FF0000");
            View.SetSelection(selection.ToXhtml());

            SelectViewContents("Three");
            selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(24);
            View.SetSelection(selection.ToXhtml());

            string expectedHtml =
                @"<P style=""MARGIN-LEFT: 0pt"" align=left>One&nbsp;<SPAN style=""COLOR: #ff0000"">Two&nbsp;<SPAN style=""FONT-SIZE: 24pt"">Three</SPAN> Four</SPAN> Five</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void SettingWordSizeAfterSettingPhraseFaceProducesExpectedHtml()
        {
            CreateViewWithTextItem("One Two Three Four Five");

            SelectViewContents("Two Three Four");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontFace("Courier New");
            View.SetSelection(selection.ToXhtml());

            SelectViewContents("Three");
            selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(24);
            View.SetSelection(selection.ToXhtml());

            string expectedHtml =
                @"<P style=""MARGIN-LEFT: 0pt"" align=left>One&nbsp;<SPAN style=""FONT-FAMILY: Courier New"">Two&nbsp;<SPAN style=""FONT-SIZE: 24pt"">Three</SPAN> Four</SPAN> Five</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void SettingWordSizeAfterSettingPhraseSizeProducesExpectedHtml()
        {
            CreateViewWithTextItem("One Two Three Four Five");

            SelectViewContents("Two Three Four");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(16);
            View.SetSelection(selection.ToXhtml());

            SelectViewContents("Three");
            selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(24);
            View.SetSelection(selection.ToXhtml());

            string expectedHtml =
                @"<P style=""MARGIN-LEFT: 0pt"" align=left>One&nbsp;<SPAN style=""FONT-SIZE: 16pt"">Two&nbsp;<SPAN style=""FONT-SIZE: 24pt"">Three</SPAN> Four</SPAN> Five</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        [Ignore("Bug to revisit later - JF 5/14/08")]
        public void SettingWordSizeIncludingSpacesProducesExpectedHtml()
        {
            CreateViewWithTextItem("One Two Three");

            SelectViewContents(" Two ");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(24);
            View.SetSelection(selection.ToXhtml());

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One<SPAN style=""FONT-SIZE: 24pt""> Two </SPAN>Three</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void SetWhenPartOfSelectionHasSize()
        {
            CreateViewWithTextItem("<font size=\"480\">One </font>Two Three Four");

            SelectViewContents("One Two ");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(16);
            View.SetSelection(selection.ToXhtml());

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">One Two </SPAN>Three Four</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void SetWhenSelectionAlreadyHasSize()
        {
            CreateViewWithTextItem("<font size=\"480\">One</font>");

            SelectViewContents("One");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(16);
            View.SetSelection(selection.ToXhtml());

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">One</SPAN></P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void SetWhenSelectionHasNestedPartialSizes()
        {
            CreateViewWithTextItem("<font size=\"400\"><font size=\"480\">One </font>Two </font>Three");

            SelectViewContents("One Two Three");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(16);
            View.SetSelection(selection.ToXhtml());

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">One Two Three</SPAN></P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void SetWhenSelectionWithinNestedSizes()
        {
            CreateViewWithTextItem("<font size=\"400\"><font size=\"480\">One </font>Two</font>");

            SelectViewContents("On");
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(16);
            View.SetSelection(selection.ToXhtml());

            string expectedHtml =
                @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 20pt""><SPAN style=""FONT-SIZE: 24pt""><SPAN style=""FONT-SIZE: 16pt"">On</SPAN>e </SPAN>Two</SPAN></P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }
    }
}