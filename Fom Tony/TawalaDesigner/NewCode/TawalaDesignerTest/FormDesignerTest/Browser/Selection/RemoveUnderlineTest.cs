// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;

namespace TawalaTest.FormDesignerTest.Browser.Selection
{
    [TestFixture]
    public class RemoveUnderlineTest : TestBase
    {
        [Test]
        public void RemoveWhenPartOfSelectionHasUnderline()
        {
            CreateViewWithTextItem("<u>One </u>Two Three Four");

            SelectViewContents("One Two ");
            SelectionRemoveFormatting();

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One Two Three Four</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void RemoveWhenSelectionIsUnderlined()
        {
            CreateViewWithTextItem("<u>One</u>");

            SelectViewContents("One");
            SelectionRemoveFormatting();

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void RemoveWhenSelectionHasNestedUnderline()
        {
            CreateViewWithTextItem("<u><i>One </i>Two</u>");

            SelectViewContents("One Two");
            SelectionRemoveFormatting();

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One Two</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Test]
        public void RemoveWhenSelectionHasNestedPartialUnderline()
        {
            CreateViewWithTextItem("<u><i>One </i>Two </u>Three");

            SelectViewContents("One Two Three");
            SelectionRemoveFormatting();

            string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One Two Three</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }
    }
}