// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;

namespace TawalaTest.FormDesignerTest.Browser.Selection
{
    [TestFixture]
    public class RemoveBoldTest : TestBase
    {
        [Test]
        public void RemoveWhenPartOfSelectionHasBold()
        {
            CreateViewWithTextItem("<b>One </b>Two Three Four");

            SelectViewContents("One Two ");
            SelectionRemoveFormatting();

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One Two Three Four</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

//        [Ignore("Browser adds extra paragraph tag when html is updated")]
        [Test]
        public void RemoveWhenSelectionHasBold()
        {
            CreateViewWithTextItem("<b>One</b>");

            SelectViewContents("One");
            SelectionRemoveFormatting();

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void RemoveWhenSelectionHasNestedBold()
        {
            CreateViewWithTextItem("<b><i>One </i>Two</b>");

            SelectViewContents("One Two");
            SelectionRemoveFormatting();

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One Two</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void RemoveWhenSelectionHasNestedPartialBold()
        {
            CreateViewWithTextItem("<b><i>One </i>Two </b>Three");

            SelectViewContents("One Two Three");
            SelectionRemoveFormatting();

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One Two Three</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }
    }
}