using System;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest.HtmlSelectionTest
{
    [TestFixture]
    public class RemoveFontSizeTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void CanRemoveSizeInSelectionBridgingTwoParagraphsWithExistingAttributesInOne()
        {
            const string htmlText =
                @"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 12pt;"">Paragraph One</SPAN></P>" +
                @"<P style=""MARGIN-LEFT: 0pt"" align=left>Paragraph Two</P>";

            var selection = new HtmlSelection(htmlText);
            selection.RemoveFontFormatting();

            const string expectedXhtml = @"<p style=""margin-left: 0pt"" align=""left"">Paragraph One</p>" +
                                         @"<p style=""margin-left: 0pt"" align=""left"">Paragraph Two</p>";

            Assert.AreEqual(expectedXhtml, selection.ToXhtml());
        }

        [Test]
        public void CanRemoveSizeOnSimpleSelection()
        {
            const string htmlText = @"<SPAN style=""FONT-SIZE: 12pt;"">Selection</SPAN>";

            var selection = new HtmlSelection(htmlText);
            selection.RemoveFontFormatting();

            const string expectedXhtml = @"Selection";

            Assert.AreEqual(expectedXhtml, selection.ToXhtml());
        }
    }
}