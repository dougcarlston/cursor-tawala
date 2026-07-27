// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;

namespace TawalaTest.FormDesignerTest.Browser.Selection
{
    [TestFixture]
    public class RemoveColorTest : TestBase
    {
        [Test]
        public void RemoveWhenPartOfSelectionHasColor()
        {
            CreateViewWithTextItem("<font color=\"FF0000\">One </font>Two Three Four");

            SelectViewContents("One Two ");
            SelectionRemoveColor();

            const string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One Two Three Four</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }

        [Ignore("Ignored for commit")]
        [Test]
        public void RemoveWhenSelectionHasColor()
        {
            CreateViewWithTextItem("<font color=\"FF0000\">One </font>");

            SelectViewContents("One");
            SelectionRemoveColor();

            const string expectedHtml = @"<P style=""MARGIN-LEFT: 0pt"" align=left>One</P>";

            Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
        }
    }
}