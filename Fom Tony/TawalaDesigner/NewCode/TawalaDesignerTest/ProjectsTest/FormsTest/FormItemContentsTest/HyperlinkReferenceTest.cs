// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectsTest.FormsTest.FormItemContentsTest
{
    [TestFixture]
    public class HyperlinkReferenceTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            ComponentMaker.UseNewComponents(true);
        }

        [TearDown]
        public void TearDown()
        {
            ComponentMaker.UseNewComponents(false);
        }

        #endregion

        private readonly string hyperLinkXmlWithText =
            @"<link>" + Environment.NewLine +
            @"<new-window/>" + Environment.NewLine +
            @"<description>" + Environment.NewLine +
            @"<string value=""Display text""/>" + Environment.NewLine +
            @"</description>" + Environment.NewLine +
            @"<url>" + Environment.NewLine +
            @"<string value=""http://www.tawala.com""/>" + Environment.NewLine +
            @"</url>" + Environment.NewLine +
            @"</link>" + Environment.NewLine;

        private readonly string projectXmlHyperlinkInDocumentWithVariable =
            @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
            @"<project name=""HyperlinkWithVariable"" themePath=""default"" format=""" + Project.XmlFormatVersion +
            @""" designerBuild=""0"">" + Environment.NewLine +
            @"<pageHeader></pageHeader>" +
            @"<documents>" + Environment.NewLine +
            @"<document name=""Document 1"">" + Environment.NewLine +
            @"<xmlData>" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<font color=""0066CC""><u>" +
            @"<link>" + Environment.NewLine +
            @"<new-window/>" + Environment.NewLine +
            @"<description>" + Environment.NewLine +
            @"<string value=""HyperLink""/>" + Environment.NewLine +
            @"</description>" + Environment.NewLine +
            @"<url>" + Environment.NewLine +
            @"<field name=""_InviteeID""/>" + Environment.NewLine +
            @"</url>" + Environment.NewLine +
            @"</link>" + Environment.NewLine +
            @"</u></font>" +
            @"</paragraph>" +
            @"</xmlData>" + Environment.NewLine +
            @"</document>" + Environment.NewLine +
            @"</documents>" + Environment.NewLine +
            @"</project>" + Environment.NewLine;

        [Test]
        public void CanConstructHyperlinkReferenceFromXhtml()
        {
            var testLink = new Hyperlink();
            testLink.DisplayText = "Display Text";
            testLink.Url = "Some string";

            string xhtml = string.Format("<t:link id=\"link_" + testLink.Id + "\">" + testLink.DisplayText + "</t:link>");

            IFormItemContents contents = FormItemContentsFactory.MakeObject(new XhtmlElement(xhtml, true));
            Assert.IsNotNull(contents);

            var linkRef = contents as HyperlinkReference;
            Assert.IsNotNull(linkRef);

            Hyperlink hyperlink = linkRef.Hyperlink;

            Assert.IsNotNull(hyperlink);
            Assert.AreEqual("Display Text", hyperlink.DisplayText);
            Assert.AreEqual("Some string", hyperlink.Url);
        }

        [Test]
        public void CanConstructHyperlinkReferenceFromXml()
        {
            var linkRef = new HyperlinkReference(new XmlElement(hyperLinkXmlWithText));

            Hyperlink hyperlink = linkRef.Hyperlink;

            Assert.IsNotNull(hyperlink);
            Assert.AreEqual("Display text", hyperlink.DisplayText);
            Assert.AreEqual("http://www.tawala.com", hyperlink.Url);
        }

        [Test]
        public void CanConstructProjectContainingHyperlinkWithVariableFromXml()
        {
            Project.Create(new XmlElement(projectXmlHyperlinkInDocumentWithVariable));

            IDocument document = Project.Current.DocumentList[0];
            FormItemContentsCollection collection = document.NewContents.GetDescendants(typeof(HyperlinkReference));

            Assert.AreEqual(1, collection.Count);

            var linkRef = collection[0] as HyperlinkReference;
            Hyperlink hyperlink = linkRef.Hyperlink;

            Assert.IsNotNull(hyperlink);
            Assert.AreEqual("HyperLink", hyperlink.DisplayText);
            Assert.AreEqual("<<_InviteeID>>", hyperlink.Url);
        }

        [Test]
        public void CanConvertHyperlinkReferenceToXhtml()
        {
            var testLink = new Hyperlink();
            testLink.DisplayText = "Display Text";
            testLink.Url = "Some string";

            var linkRef = new HyperlinkReference();
            linkRef.Hyperlink = testLink;

            string expectedXhtml = string.Format("<t:link id=\"link_" + testLink.Id + "\">" + testLink.DisplayText + "</t:link>");
            string outputXhtml = linkRef.ToXhtml(null);

            Assert.AreEqual(expectedXhtml, outputXhtml);
        }

        [Test]
        public void CanConvertHyperlinkReferenceToXml()
        {
            var testLink = new Hyperlink();
            testLink.OpenNewWindow = true;
            testLink.DisplayText = "Display text";
            testLink.Url = "http://www.tawala.com";

            var linkRef = new HyperlinkReference();
            linkRef.Hyperlink = testLink;

            Assert.AreEqual(hyperLinkXmlWithText, linkRef.ToXml());
        }

        [Test]
        public void CanConvertProjectWithHyperlinkReferenceToXml()
        {
            Project.Create(new XmlElement(projectXmlHyperlinkInDocumentWithVariable));

            Assert.AreEqual(projectXmlHyperlinkInDocumentWithVariable, Project.Current.ToXmlForSaving());
        }
    }
}