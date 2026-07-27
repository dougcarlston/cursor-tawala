// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest.HtmlSelectionTest
{
    [TestFixture]
    public class FontMultipleAttributeTest
    {
        [Test]
        public void CanSetFaceSizeAndColorInSelectionBridgingTwoParagraphs()
        {
            const string htmlText = @"<P style=""MARGIN-LEFT: 0pt"" align=left>Paragraph One</P>" +
                                    @"<P style=""MARGIN-LEFT: 0pt"" align=left>Paragraph Two</P>";

            var selection = new HtmlSelection(htmlText);
            selection.SetFontFace("Courier New");
            selection.SetFontSize(16);
            selection.SetFontColor("FF0000");

            string expectedXhtml =
                @"<span style=""font-family: Courier New;font-size: 16pt;color: #FF0000;"">Paragraph One</span></p>" +
                @"<p style=""margin-left: 0pt"" align=""left""><span style=""font-family: Courier New;font-size: 16pt;color: #FF0000;"">Paragraph Two</span>";

            Assert.AreEqual(expectedXhtml, selection.ToXhtml());
        }

        [Test]
        public void CanSetFaceSizeAndColorInSimpleSelection()
        {
            string htmlText = "Selection";

            var selection = new HtmlSelection(htmlText);
            selection.SetFontFace("Courier New");
            selection.SetFontSize(16);
            selection.SetFontColor("FF0000");

            string expectedXhtml = @"<span style=""font-family: Courier New;font-size: 16pt;color: #FF0000;"">Selection</span>";

            Assert.AreEqual(expectedXhtml, selection.ToXhtml());
        }
    }
}