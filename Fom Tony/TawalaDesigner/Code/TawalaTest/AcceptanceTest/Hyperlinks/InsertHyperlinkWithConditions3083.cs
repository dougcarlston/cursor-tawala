// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Links;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Hyperlinks
{
    [TestFixture]
    public class InsertHyperlinkWithoutDisplayText0000
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private static string hyperLinkXmlNoConditions =
            "<link>\r\n" +
            "<new-window/>\r\n" +
            "<description>\r\n" +
            "<string value=\"Display Text\"/>\r\n" +
            "</description>\r\n" +
            "<url>\r\n" +
            "<string value=\"http://foo.com\"/>\r\n" +
            "</url>\r\n" +
            "</link>\r\n";

        private const string hyperLinkXmlWithCondtions =
            "<link>\r\n" +
            "<description>\r\n" +
            "<string value=\"Display Text\"/>\r\n" +
            "</description>\r\n" +
            "<url>\r\n" +
            "<string value=\"http://foo.com\"/>\r\n" +
            "</url>\r\n" +
            "<displayConditions>\r\n" +
            "<equals field=\"Form 1:Field1\">\r\n" +
            "<string field=\"Form 1:Q2:a\"/>\r\n" +
            "</equals>\r\n" +
            "</displayConditions>" +
            "</link>\r\n";

        [Test]
        public void HyperlinkWithConditions()
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
                "color=\"0066CC\"><u>" +
                hyperLinkXmlWithCondtions +
                "</u></font></paragraph>" + Environment.NewLine +
                "</xmlData>" + Environment.NewLine +
                "</document>" + Environment.NewLine +
                "</documents>" + Environment.NewLine +
                "</project>" + Environment.NewLine;

            Util.OpenProjectXml(projectXml);

            var invitationMap =
                Reflect<ProjectInvitationMapById>.GetField<Dictionary<int, ILink>>("map", Project.InvitationMapById);

            Assert.AreEqual(1, invitationMap.Count);

            foreach (int id in invitationMap.Keys)
            {
                if (invitationMap[id] is Hyperlink && invitationMap[id] != Hyperlink.NULL)
                {
                    var hyperLink = invitationMap[id] as Hyperlink;
                    Assert.AreEqual(hyperLinkXmlWithCondtions, hyperLink.ToXml());
                    Assert.AreEqual("Display Text", hyperLink.DisplayText);
                    Assert.AreEqual("http://foo.com", hyperLink.Url);
                    Assert.AreEqual(1, hyperLink.Conditions.Count);
                }
            }
        }

        [Test]
        public void HyperlinkWithNoConditions()
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
                "color=\"0066CC\"><u>" +
                hyperLinkXmlNoConditions +
                "</u></font></paragraph>" + Environment.NewLine +
                "</xmlData>" + Environment.NewLine +
                "</document>" + Environment.NewLine +
                "</documents>" + Environment.NewLine +
                "</project>" + Environment.NewLine;

            Util.OpenProjectXml(projectXml);

            var invitationMap =
                Reflect<ProjectInvitationMapById>.GetField<Dictionary<int, ILink>>("map", Project.InvitationMapById);

            Assert.AreEqual(1, invitationMap.Count);

            foreach (int id in invitationMap.Keys)
            {
                if (invitationMap[id] is Hyperlink && invitationMap[id] != Hyperlink.NULL)
                {
                    var hyperLink = invitationMap[id] as Hyperlink;
                    Assert.AreEqual(hyperLinkXmlNoConditions, hyperLink.ToXml());
                    Assert.AreEqual("Display Text", hyperLink.DisplayText);
                    Assert.AreEqual("http://foo.com", hyperLink.Url);
                    Assert.IsNull(hyperLink.Conditions);
                }
            }
        }
    }
}