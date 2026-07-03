// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Links;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.YouthSportsServices
{
    /// <summary>
    /// Acceptance tests for story 2814 (Designer uses expressions for URL in Hyperlink dialog)
    /// </summary>
    [TestFixture]
    public class HyperlinkExpressionTest2814
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();

            fibItem1 = new FibItem();
            form.ItemList.Add(fibItem1);

            fibItem2 = new FibItem();
            form.ItemList.Add(fibItem2);
        }

        #endregion

        private IForm form;
        private FibItem fibItem1;
        private FibItem fibItem2;

        [Test]
        public void HyperlinkWithFieldOnlyGeneratesCorrectXml()
        {
            string xmlString =
                "<link>" + Environment.NewLine +
                "<description>" + Environment.NewLine +
                "<string value=\"Google\"/>" + Environment.NewLine +
                "</description>" + Environment.NewLine +
                "<url>" + Environment.NewLine +
                "<field name=\"Form 1:Q1:a\"/>" + Environment.NewLine +
                "</url>" + Environment.NewLine +
                "</link>" + Environment.NewLine;

            var hyperlink = new Hyperlink();
            hyperlink.DisplayText = "Google";
            hyperlink.Url = "<<Form 1:Q1:a>>";

            Assert.AreEqual(xmlString, hyperlink.ToXml());
        }

        [Test]
        public void HyperlinkWithOpenNewWindowSetGeneratesCorrectXml()
        {
            string xmlString =
                "<link>" + Environment.NewLine +
                "<new-window/>" + Environment.NewLine +
                "<description>" + Environment.NewLine +
                "<string value=\"Google\"/>" + Environment.NewLine +
                "</description>" + Environment.NewLine +
                "<url>" + Environment.NewLine +
                "<string value=\"http://google.com\"/>" + Environment.NewLine +
                "</url>" + Environment.NewLine +
                "</link>" + Environment.NewLine;

            var hyperlink = new Hyperlink();
            hyperlink.DisplayText = "Google";
            hyperlink.Url = "http://google.com";
            hyperlink.OpenNewWindow = true;

            Assert.AreEqual(xmlString, hyperlink.ToXml());
        }

        [Test]
        public void HyperlinkWithTextAndFieldGeneratesCorrectXml()
        {
            string xmlString =
                "<link>" + Environment.NewLine +
                "<description>" + Environment.NewLine +
                "<string value=\"Google\"/>" + Environment.NewLine +
                "</description>" + Environment.NewLine +
                "<url>" + Environment.NewLine +
                "<string value=\"http://\"/>" + Environment.NewLine +
                "<field name=\"Form 1:Q1:a\"/>" + Environment.NewLine +
                "</url>" + Environment.NewLine +
                "</link>" + Environment.NewLine;

            var hyperlink = new Hyperlink();
            hyperlink.DisplayText = "Google";
            hyperlink.Url = "http://<<Form 1:Q1:a>>";

            Assert.AreEqual(xmlString, hyperlink.ToXml());
        }

        [Test]
        public void HyperlinkWithTextOnlyGeneratesCorrectXml()
        {
            string xmlString =
                "<link>" + Environment.NewLine +
                "<description>" + Environment.NewLine +
                "<string value=\"Google\"/>" + Environment.NewLine +
                "</description>" + Environment.NewLine +
                "<url>" + Environment.NewLine +
                "<string value=\"http://google.com\"/>" + Environment.NewLine +
                "</url>" + Environment.NewLine +
                "</link>" + Environment.NewLine;

            var hyperlink = new Hyperlink();
            hyperlink.DisplayText = "Google";
            hyperlink.Url = "http://google.com";

            Assert.AreEqual(xmlString, hyperlink.ToXml());
        }

        [Test]
        public void SpacesBetweenFieldsArePreservedInXml()
        {
            string xmlString =
                "<link>" + Environment.NewLine +
                "<description>" + Environment.NewLine +
                "<string value=\"Google\"/>" + Environment.NewLine +
                "</description>" + Environment.NewLine +
                "<url>" + Environment.NewLine +
                "<field name=\"Form 1:Q1:a\"/>" + Environment.NewLine +
                "<string value=\" \"/>" + Environment.NewLine +
                "<field name=\"Form 1:Q2:a\"/>" + Environment.NewLine +
                "</url>" + Environment.NewLine +
                "</link>" + Environment.NewLine;

            var hyperlink = new Hyperlink(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, hyperlink.ToXml());
        }

        [Test]
        public void SpacesInTextArePreservedInXml()
        {
            string xmlString =
                "<link>" + Environment.NewLine +
                "<description>" + Environment.NewLine +
                "<string value=\"Google\"/>" + Environment.NewLine +
                "</description>" + Environment.NewLine +
                "<url>" + Environment.NewLine +
                "<string value=\"Text Text Text\"/>" + Environment.NewLine +
                "</url>" + Environment.NewLine +
                "</link>" + Environment.NewLine;

            var hyperlink = new Hyperlink(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, hyperlink.ToXml());
        }

        [Test]
        public void XmlProducesValidHyperlink()
        {
            string xmlString =
                "<link>" + Environment.NewLine +
                "<description>" + Environment.NewLine +
                "<string value=\"Google\"/>" + Environment.NewLine +
                "</description>" + Environment.NewLine +
                "<url>" + Environment.NewLine +
                "<string value=\"http://\"/>" + Environment.NewLine +
                "<field name=\"Form 1:Q1:a\"/>" + Environment.NewLine +
                "</url>" + Environment.NewLine +
                "</link>" + Environment.NewLine;

            var hyperlink = new Hyperlink(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, hyperlink.ToXml());
        }

        [Test]
        public void XmlWithSpacesBetweenFieldsProducesValidHyperlink()
        {
            string xmlString =
                "<link>" + Environment.NewLine +
                "<description>" + Environment.NewLine +
                "<string value=\"Google\"/>" + Environment.NewLine +
                "</description>" + Environment.NewLine +
                "<url>" + Environment.NewLine +
                "<field name=\"Form 1:Q1:a\"/>" + Environment.NewLine +
                "<string value=\" \"/>" + Environment.NewLine +
                "<field name=\"Form 1:Q2:a\"/>" + Environment.NewLine +
                "</url>" + Environment.NewLine +
                "</link>" + Environment.NewLine;

            var hyperlink = new Hyperlink(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, hyperlink.ToXml());
        }

        [Test]
        public void XmlWithSpacesInTextProducesValidHyperlink()
        {
            string xmlString =
                "<link>" + Environment.NewLine +
                "<description>" + Environment.NewLine +
                "<string value=\"Google\"/>" + Environment.NewLine +
                "</description>" + Environment.NewLine +
                "<url>" + Environment.NewLine +
                "<string value=\"Text Text Text\"/>" + Environment.NewLine +
                "</url>" + Environment.NewLine +
                "</link>" + Environment.NewLine;

            var hyperlink = new Hyperlink(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, hyperlink.ToXml());
        }
    }
}