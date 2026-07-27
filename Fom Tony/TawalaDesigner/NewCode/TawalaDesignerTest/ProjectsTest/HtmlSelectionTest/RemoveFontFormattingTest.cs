// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest.HtmlSelectionTest
{
    [TestFixture]
    public class RemoveFontFormattingTest
    {
        [Test]
        public void ReturnsFalseIfNoFormatting()
        {
            const string htmlWithNoFormattingText = @"<P style=""MARGIN-LEFT: 0pt"" align=left>Paragraph Two</P>";
            var selection = new HtmlSelection(htmlWithNoFormattingText);
            var result = selection.RemoveFontFormatting();
            Assert.IsFalse(result);
        }

        [Test]
        public void ReturnsFalseIfNoFontFormatting()
        {
            const string htmlWithBoldItalicUnderlineText = @"<P style=""MARGIN-LEFT: 0pt"" align=left>Paragraph Two</P>";
            var selection = new HtmlSelection(htmlWithBoldItalicUnderlineText);
            var result = selection.RemoveFontFormatting();
            Assert.IsFalse(result);
        }

        [Test]
        public void CanRemoveFontColorOnSimpleSelection()
        {
            const string htmlFontFaceSizeColorText =
               @"<SPAN style=""FONT-FAMILY: Impact; FONT-SIZE: 12pt; COLOR: #FF0000;"">Selection</SPAN>";

            var selection = new HtmlSelection(htmlFontFaceSizeColorText);
            var result = selection.RemoveFontFormatting();

            const string expectedXhtml = @"Selection";

            Assert.IsTrue(result);
            Assert.AreEqual(expectedXhtml, selection.ToXhtml());
        }

        [Test]
        public void CanRemoveMultipleFontAttributesOnSelectionBridgingTwoParagraphsWithExistingAttributesInOne()
        {
            const string htmlMultiParagrahFontFaceSizeColorText =
              @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #FF0000;""><SPAN style=""FONT-SIZE: 12pt;"">Paragraph</SPAN><SPAN style=""FONT-FAMILY: Impact;""> One</SPAN></SPAN></P>" +
              @"<P style=""MARGIN-LEFT: 0pt"" align=left>Paragraph Two</P>";

            var selection = new HtmlSelection(htmlMultiParagrahFontFaceSizeColorText);
            var result = selection.RemoveFontFormatting();

            const string expectedXhtml = @"<p style=""margin-left: 0pt"" align=""left"">Paragraph One</p>" +
                                         @"<p style=""margin-left: 0pt"" align=""left"">Paragraph Two</p>";

            Assert.IsTrue(result);
            Assert.AreEqual(expectedXhtml, selection.ToXhtml());
        }
    }
}