// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormattedMCQs
{
    /// <summary>
    /// Acceptance tests for story 1741 (Font selection, size and color in MCQ Items).
    /// </summary>
    [TestFixture]
    public class Story1741_FontSelectionSizeAndColor
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private const string halfInchTabPositionsString =
            "<tabPositions>" +
            "<tabStop position=\"720\"/>" +
            "<tabStop position=\"1440\"/>" +
            "<tabStop position=\"2160\"/>" +
            "<tabStop position=\"2880\"/>" +
            "<tabStop position=\"3600\"/>" +
            "<tabStop position=\"4320\"/>" +
            "<tabStop position=\"5040\"/>" +
            "<tabStop position=\"5760\"/>" +
            "<tabStop position=\"6480\"/>" +
            "<tabStop position=\"7200\"/>" +
            "<tabStop position=\"7920\"/>" +
            "<tabStop position=\"8640\"/>" +
            "<tabStop position=\"9360\"/>" +
            "<tabStop position=\"10080\"/>" +
            "</tabPositions>";

        private string rtfFontSelectionSizeAndColorInQuestionTextIn =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset0\fprq2 Times New Roman;}" + "\r\n" +
            @"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red255\green0\blue0;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080\plain" +
            @"\f1\fs28\cf3 Question text" +
            @"\plain\f0\fs20\par\plain\f0\fs20" +
            @"    a) Choice 1" +
            @"\plain\f0\fs20\par\plain\f0\fs20" +
            @"    b) Choice 2" +
            @"\par }";

        private readonly string rtfFontSelectionSizeAndColorInQuestionTextOut =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
            @"{\fonttbl" + Environment.NewLine +
            @"{\f0\fswiss Arial;}" + Environment.NewLine +
            @"{\f1\fnil Times New Roman;}" + Environment.NewLine +
            @"}" + Environment.NewLine +
            @"\fs20{\colortbl;" + Environment.NewLine +
            @"\red0\green0\blue0;" + Environment.NewLine +
            @"\red255\green255\blue255;" + Environment.NewLine +
            @"\red255\green0\blue0;" + Environment.NewLine +
            @"}" + Environment.NewLine +
            RtfConstants.DefaultTabsRtf +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f1\fs28\cf3 Question text}\par " +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1    a) }{\f0\fs20\cf1 Choice 1}\par " +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1    b) }{\f0\fs20\cf1 Choice 2}\par }";

        private readonly string xmlFontSelectionSizeAndColorInQuestionText =
            "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            "<font face=\"Times New Roman\" size=\"280\" color=\"FF0000\">" +
            "Question text" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</question>" +
            "<choice label=\"a\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.FullBeginFont +
            "Choice 1" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "<choice label=\"b\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.FullBeginFont +
            "Choice 2" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "</mc>\r\n";

        private readonly string fullXmlFontSelectionSizeAndColorInQuestionText =
            "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            "<font face=\"Times New Roman\" size=\"280\" color=\"FF0000\">" +
            "Question text" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</question>" +
            "<choice label=\"a\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.FullBeginFont +
            "Choice 1" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "<choice label=\"b\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.FullBeginFont +
            "Choice 2" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "</mc>\r\n";

        private string rtfFontSelectionSizeAndColorInChoiceTextIn =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset0\fprq2 Times New Roman;}" + "\r\n" +
            @"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red255\green0\blue0;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080\plain" +
            @"\f0\fs20 Question text" +
            @"\plain\f0\fs20\par\plain" +
            @"\f1\fs28\cf3" +
            @"    a) Choice 1" +
            @"\plain\f0\fs20\par\plain\f0\fs20" +
            @"    b) Choice 2" +
            @"\par }";

        private readonly string rtfFontSelectionSizeAndColorInChoiceTextOut =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
            @"{\fonttbl" + Environment.NewLine +
            @"{\f0\fswiss Arial;}" + Environment.NewLine +
            @"{\f1\fnil Times New Roman;}" + Environment.NewLine +
            @"}" + Environment.NewLine +
            @"\fs20{\colortbl;" + Environment.NewLine +
            @"\red0\green0\blue0;" + Environment.NewLine +
            @"\red255\green255\blue255;" + Environment.NewLine +
            @"\red255\green0\blue0;" + Environment.NewLine +
            @"}" + Environment.NewLine +
            RtfConstants.DefaultTabsRtf +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1 Question text}\par " +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1    a) }{\f1\fs28\cf3 Choice 1}\par " +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1    b) }{\f0\fs20\cf1 Choice 2}\par }";

        private readonly string xmlFontSelectionSizeAndColorInChoiceText =
            "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.FullBeginFont +
            "Question text" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</question>" +
            "<choice label=\"a\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            "<font face=\"Times New Roman\" size=\"280\" color=\"FF0000\">" +
            "Choice 1" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "<choice label=\"b\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.FullBeginFont +
            "Choice 2" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "</mc>\r\n";

        private readonly string fullXmlFontSelectionSizeAndColorInChoiceText =
            "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.FullBeginFont +
            "Question text" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</question>" +
            "<choice label=\"a\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            "<font face=\"Times New Roman\" size=\"280\" color=\"FF0000\">" +
            "Choice 1" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "<choice label=\"b\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.FullBeginFont +
            "Choice 2" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "</mc>\r\n";

        [Test]
        public void CombinedFontAttributesInChoiceRtfProducesCombinedFontAttributesInChoiceRtf()
        {
            var mcItem = new McqItem();
            mcItem.Rtf = rtfFontSelectionSizeAndColorInChoiceTextIn;

            Assert.AreEqual(rtfFontSelectionSizeAndColorInChoiceTextOut, mcItem.ToRtf());
        }

        [Test]
        public void CombinedFontAttributesInChoiceRtfProducesCombinedFontAttributesInChoiceXml()
        {
            var mcItem = new McqItem();
            mcItem.Rtf = rtfFontSelectionSizeAndColorInChoiceTextIn;

            Assert.AreEqual(xmlFontSelectionSizeAndColorInChoiceText, mcItem.ToXml("Q1"));
        }

        [Test]
        public void CombinedFontAttributesInChoiceXmlProducesCombinedFontAttributesInChoiceRtf()
        {
            IXmlElement element = new XmlElement(fullXmlFontSelectionSizeAndColorInChoiceText, true);
            var mcItem = new McqItem(element);

            Assert.AreEqual(rtfFontSelectionSizeAndColorInChoiceTextOut, mcItem.ToRtf());
        }

        [Test]
        public void CombinedFontAttributesInChoiceXmlProducesCombinedFontAttributesInChoiceXml()
        {
            IXmlElement element = new XmlElement(xmlFontSelectionSizeAndColorInChoiceText, true);
            var mcItem = new McqItem(element);

            Assert.AreEqual(xmlFontSelectionSizeAndColorInChoiceText, mcItem.ToXml("Q1"));
        }

        [Test]
        public void CombinedFontAttributesInQuestionRtfProducesCombinedFontAttributesInQuestionRtf()
        {
            var mcItem = new McqItem();
            mcItem.Rtf = rtfFontSelectionSizeAndColorInQuestionTextIn;

            Assert.AreEqual(rtfFontSelectionSizeAndColorInQuestionTextOut, mcItem.ToRtf());
        }

        [Test]
        public void CombinedFontAttributesInQuestionRtfProducesCombinedFontAttributesInQuestionXml()
        {
            var mcItem = new McqItem();
            mcItem.Rtf = rtfFontSelectionSizeAndColorInQuestionTextIn;

            Assert.AreEqual(xmlFontSelectionSizeAndColorInQuestionText, mcItem.ToXml("Q1"));
        }

        [Test]
        public void CombinedFontAttributesInQuestionXmlProducesCombinedFontAttributesInQuestionRtf()
        {
            IXmlElement element = new XmlElement(fullXmlFontSelectionSizeAndColorInQuestionText, true);
            var mcItem = new McqItem(element);

            Assert.AreEqual(rtfFontSelectionSizeAndColorInQuestionTextOut, mcItem.ToRtf());
        }

        [Test]
        public void CombinedFontAttributesInQuestionXmlProducesCombinedFontAttributesInQuestionXml()
        {
            IXmlElement element = new XmlElement(xmlFontSelectionSizeAndColorInQuestionText, true);
            var mcItem = new McqItem(element);

            Assert.AreEqual(xmlFontSelectionSizeAndColorInQuestionText, mcItem.ToXml("Q1"));
        }
    }
}