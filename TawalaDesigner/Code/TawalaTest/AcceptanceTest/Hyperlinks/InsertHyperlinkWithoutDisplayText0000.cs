// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects.Links;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Hyperlinks
{
    [TestFixture]
    public class InsertHyperlinkWithConditions3083
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private static string hyperLinkXml =
            "<link>\r\n" +
            "<new-window/>\r\n" +
            "<description>\r\n" +
            "</description>\r\n" +
            "<url>\r\n" +
            "<string value=\"http://foo.com\"/>\r\n" +
            "</url>\r\n" +
            "</link>\r\n";

        private const string designerDisplayTextWhenDisplayTextEmpty = "(Link appears here)";

        [Test]
        public void HyperlinkWithNoDisplayProducesCorrectXml()
        {
            var link = new Hyperlink();
            Assert.IsEmpty(link.DisplayText);
            Assert.IsNotNull(link.DesignerDisplayText);

            link.Url = "http://foo.com";
            link.OpenNewWindow = true;

            Assert.AreEqual(hyperLinkXml, link.ToXml());
        }

        [Test]
        public void HyperlinkWithNoDisplayProducesCorrectXml2()
        {
            string xml =
                "<link>\r\n" +
                "<new-window/>\r\n" +
                "<description>\r\n" +
                "</description>\r\n" +
                "<url>\r\n" +
                "<string value=\"http://foo.com\"/>\r\n" +
                "</url>\r\n" +
                "</link>\r\n";

            var link = new Hyperlink();
            link.DisplayText = string.Empty;
            Assert.IsEmpty(link.DisplayText);
            Assert.IsNotNull(link.DesignerDisplayText);

            link.Url = "http://foo.com";
            link.OpenNewWindow = true;

            Assert.AreEqual(xml, link.ToXml());
        }

        [Test]
        public void HyperlinkWithNoDisplayTextHasExpectedDocumentText()
        {
            var link = new Hyperlink();
            link.Url = "http://foo.com";
            Assert.AreEqual(designerDisplayTextWhenDisplayTextEmpty, link.DesignerDisplayText);
        }
    }
}