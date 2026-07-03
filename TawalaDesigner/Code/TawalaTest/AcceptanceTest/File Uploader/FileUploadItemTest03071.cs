// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FileUploader
{
    [TestFixture]
    public class FileUploadItemTest03071
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        [TearDown]
        public void Teardown()
        {
        }

        #endregion

        private static readonly string defaultRtfAsXml =
            @"<file label=""F1"" required=""false"" style=""topLabels"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<tabPositions>" +
            @"<tabStop position=""2880""/>" +
            @"</tabPositions>" +
            @"<font>Select a file on your system for upload to the server.</font>" +
            @"<fileNameInput/>" +
            @"</paragraph>" +
            @"</file>" + Environment.NewLine;

        private static readonly string customXml =
            @"<file label=""F2"" alternateLabel=""MyFile"" required=""false"" style=""topLabels"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<tabPositions>" +
            @"<tabStop position=""2880""/>" +
            @"</tabPositions>" +
            @"<font face=""Arial"" size=""480"" color=""FF0000"">" +
            @"Instructional text" +
            @"</font>" +
            @"<fileNameInput/>" +
            @"</paragraph>" +
            @"</file>" + Environment.NewLine;

        private readonly string headingDefaultRtfForTestFileUploadItem =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
            @"{\fonttbl" + Environment.NewLine +
            @"{\f0\fswiss Arial;}" + Environment.NewLine +
            @"}" + Environment.NewLine +
            @"\fs20{\colortbl;" + Environment.NewLine +
            @"\red0\green0\blue0;" + Environment.NewLine +
            @"\red255\green255\blue255;" + Environment.NewLine +
            @"}" + Environment.NewLine +
            @"\deftab0\tx2880\pard \tx2880{\f0\fs36\cf1 [Replace this with heading of your own.]}\par }";

        private static readonly string headingDefaultRtfAsXml =
            @"<file label=""F3"" required=""false"" style=""topLabels"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<tabPositions>" +
            @"<tabStop position=""2880""/>" +
            @"</tabPositions>" +
            @"<font face=""Arial"" size=""360"" color=""000000"">" +
            @"[Replace this with heading of your own.]" +
            @"</font>" +
            @"<fileNameInput/>" +
            @"</paragraph>" +
            @"</file>" + Environment.NewLine;

        [Test]
        public void ConstructingWithCustomXmlResultsInExpectedRtf()
        {
            IFileUploadItem fileUploadItem = new FileUploadItem(new XmlElement(headingDefaultRtfAsXml, true));
            string rtf = fileUploadItem.Rtf;
            Assert.AreEqual(headingDefaultRtfForTestFileUploadItem, rtf, rtf);
        }

        [Test]
        public void NewHasExpectedDefaultProperties()
        {
            IFileUploadItem fileUploadItem = new FileUploadItem();

            Assert.AreEqual(fileUploadItem.AlternateLabel, string.Empty);
            Assert.IsFalse(fileUploadItem.Required);
            Assert.AreEqual(fileUploadItem.Style, "topLabels");
        }

        [Test]
        public void NewWithDefaultRtfToXml()
        {
            IFileUploadItem fileUploadItem = new FileUploadItem();

            string xml = fileUploadItem.ToXml("F1");

            Assert.AreEqual(defaultRtfAsXml, xml, xml);
        }

        [Test]
        public void RoundTripCustomXml()
        {
            IFileUploadItem fileUploadItem = new FileUploadItem(new XmlElement(customXml, true));
            string xml = fileUploadItem.ToXml("F2");
            Assert.AreEqual(customXml, xml, xml);
        }

        [Test]
        public void SettingCustomRtfResultsInCorrectCustomXml()
        {
            IFileUploadItem fileUploadItem = new FileUploadItem();
            fileUploadItem.Rtf = headingDefaultRtfForTestFileUploadItem;
            string xml = fileUploadItem.ToXml("F3");
            Assert.AreEqual(headingDefaultRtfAsXml, xml, xml);
        }
    }
}