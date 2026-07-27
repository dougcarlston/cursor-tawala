// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormattedDocuments
{
    /// <summary>
    /// Acceptance tests for story 1794 (Bug 308: Tab positions not maintained in Documents).
    /// </summary>
    [TestFixture]
    public class TabPositions1794
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private const string NEWLINE = "\r\n";

        [Test]
        public void NonDefaultTabPositionsInRtfProduceNonDefaultTabPositionsXml()
        {
            string rtfTabPositionsString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0" +
                @"\tx1440\tx2880" +
                @"\plain\f0\fs20 Text1\tab Text2\tab Text3\par }";

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTabPositionsString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<tabPositions>" +
                "<tabStop position=\"1440\"/>" +
                "<tabStop position=\"2880\"/>" +
                "</tabPositions>" +
                XmlConstants.FullBeginFont +
                "Text1" +
                XmlConstants.EndFont +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Text2" +
                XmlConstants.EndFont +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Text3" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void NonDefaultTabPositionsInXmlProduceNonDefaultTabPositionsRtf()
        {
            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<tabPositions>" +
                "<tabStop position=\"1440\"/>" +
                "<tabStop position=\"2880\"/>" +
                "</tabPositions>" +
                XmlConstants.FullBeginFont +
                "Text1" +
                XmlConstants.EndFont +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Text2" +
                XmlConstants.EndFont +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Text3" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "<htmlData>\r\n" +
                "<![CDATA[" +
                "<table style=\"margin-left:0pt;\">" +
                "<tr>" +
                "<td>" +
                "<div style=\"width:36pt;\">" +
                "<div style=\"margin-left:0pt;text-align:left\">" +
                "<span style=\"font-family:'Arial';font-size:10pt;color:#000000\">" +
                "Text1" +
                "</span>" +
                "</div>" +
                "</div>" +
                "</td>" +
                "<td>" +
                "<div style=\"width:36pt;\">" +
                "<div style=\"margin-left:0pt;text-align:left\">" +
                "<span style=\"font-family:'Arial';font-size:10pt;color:#000000\">" +
                "Text2" +
                "</span>" +
                "</div>" +
                "</div>" +
                "</td>" +
                "<td>" +
                "<div style=\"width:468pt;\">" +
                "<div style=\"margin-left:0pt;text-align:left\">" +
                "<span style=\"font-family:'Arial';font-size:10pt;color:#000000\">" +
                "Text3" +
                "</span>" +
                "</div>" +
                "</div>" +
                "</td>" +
                "</tr>" +
                "</table>" +
                "]]>\r\n" +
                "</htmlData>\r\n" +
                "</document>\r\n";

            var rtfDocument = new RtfDocument(new XmlElement(xmlString));

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + NEWLINE +
                @"{\fonttbl" + NEWLINE +
                @"{\f0\fswiss Arial;}" + NEWLINE +
                @"{\f1\fnil Default Font;}" + NEWLINE +
                @"}" + NEWLINE +
                @"\fs20" +
                @"{\colortbl;" + NEWLINE +
                @"\red0\green0\blue0;" + NEWLINE +
                @"\red255\green255\blue255;" + NEWLINE +
                @"\red0\green0\blue1;" + NEWLINE +
                @"}" + NEWLINE +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"\tx1440\tx2880" +
                @"{\f0\fs20\cf1 Text1}\tab {\f0\fs20\cf1 Text2}\tab {\f0\fs20\cf1 Text3}\par }";

            Assert.AreEqual(expectedRtf, rtfDocument.ToRtf());
        }
    }
}