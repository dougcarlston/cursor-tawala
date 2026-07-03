// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Links;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Hyperlinks
{
    /// <summary>
    /// Acceptance tests for story 2747 (Designer Inserts External URL in Document)
    /// </summary>
    [TestFixture]
    public class DesignerInsertsHyperlinkInDocument2747
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private static string xml =
            "<link>" +
            "<description>" +
            "<string value=\"Display Text\"/>" +
            "</description>" +
            "<url>" +
            "<string value=\"http://foo.com\"/>" +
            "</url>" +
            "</link>";

        private static string xmlNewWindow =
            "<link>" +
            "<new-window/>" +
            "<description>" +
            "<string value=\"Display Text\"/>" +
            "</description>" +
            "<url>" +
            "<string value=\"http://foo.com\"/>" +
            "</url>" +
            "</link>";

        [Test]
        [Ignore("Obsoleted by Story 2814 development - SB 03/14/2008")]
        public void InvitationFieldWithUrlAndOpenNewWindowGeneratesCorrectXml()
        {
            var field = new InvitationField();
            field.DisplayText = "Display Text";
//			field.Url = "http://foo.com";
//			field.OpenNewWindow = true;

            Assert.AreEqual(xmlNewWindow, field.ToXml());
        }

        [Test]
        [Ignore("Obsoleted by Story 2814 development - SB 03/14/2008")]
        public void InvitationFieldWithUrlGeneratesCorrectXml()
        {
            var field = new InvitationField();
            field.DisplayText = "Display Text";
//			field.Url = "http://foo.com";

            Assert.AreEqual(xml, field.ToXml());
        }

        [Test]
        public void InvitationFieldWithUrlInDocumentLoadsCorrectly()
        {
            string projectXml =
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine +
                "<project name=\"Untitled\" themePath=\"default\" format=\"1.9\">" + Environment.NewLine +
                "<forms>" + Environment.NewLine +
                "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\">" + Environment.NewLine +
                "</form>" + Environment.NewLine +
                "</forms>" + Environment.NewLine +
                "<documents>" + Environment.NewLine +
                "<document name=\"Document 1\">" + Environment.NewLine +
                "<xmlData>" + Environment.NewLine +
                "<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font " +
                "color=\"0066CC\"><u><link><description><string value=\"Hyperlink\"/></description><url><string " +
                "value=\"www.tawala.com\"/></url></link></u></font></paragraph>" + Environment.NewLine +
                "</xmlData>" + Environment.NewLine +
                "</document>" + Environment.NewLine +
                "</documents>" + Environment.NewLine +
                "</project>" + Environment.NewLine;

            var invitationMap =
                Reflect<ProjectInvitationMapById>.GetField<Dictionary<int, ILink>>("map", Project.InvitationMapById);

            Assert.AreEqual(0, invitationMap.Count);

            Util.OpenProjectXml(projectXml);

            invitationMap =
                Reflect<ProjectInvitationMapById>.GetField<Dictionary<int, ILink>>("map", Project.InvitationMapById);
            Assert.AreEqual(2, invitationMap.Count);

            int fieldId = 0;

            foreach (int id in invitationMap.Keys)
            {
                fieldId = id;
            }
            foreach (int id in invitationMap.Keys)
            {
                fieldId = id;
            }

            var link = invitationMap[fieldId];

            Assert.AreEqual("Hyperlink", link.DisplayText);
        }

        [Test]
        [Ignore("Next")]
        public void InvitationFieldWithUrlInTextItemLoadsCorrectly()
        {
        }

        [Test]
        [Ignore("Obsoleted by Story 2814 development - SB 03/14/2008")]
        public void InvitationFieldWithUrlRestoresFromXmlCorrectly()
        {
            IXmlElement element = new XmlElement(xml);
            var field = new InvitationField(element);

            Assert.AreEqual("Display Text", field.DisplayText);
//			Assert.AreEqual("http://foo.com", field.Url);
        }
    }
}