// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormattedMCQs
{
    /// <summary>
    /// Acceptance tests for story 1740 (Bold, italic and underline in MCQs).
    /// </summary>
    [TestFixture]
    public class Story1740_BoldItalicAndUnderline
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

        private readonly string rtfBoldItalicAndUnderlineInQuestionTextIn =
            RtfDocument.RtfStringPrefix +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080\plain\f0\fs20 " +
            @"\b\i\ul Question text" +
            @"\plain\f0\fs20\par\plain\f0\fs20" +
            @"    a) Choice 1" +
            @"\plain\f0\fs20\par\plain\f0\fs20" +
            @"    b) Choice 2" +
            @"\par }";

        private readonly string rtfBoldItalicAndUnderlineInQuestionTextOut =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
            @"{\fonttbl" + Environment.NewLine +
            @"{\f0\fswiss Arial;}" + Environment.NewLine +
            @"}" + Environment.NewLine +
            @"\fs20{\colortbl;" + Environment.NewLine +
            @"\red0\green0\blue0;" + Environment.NewLine +
            @"\red255\green255\blue255;" + Environment.NewLine +
            @"}" + Environment.NewLine +
            RtfConstants.DefaultTabsRtf +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1 \b \i \ul Question text\ul0 \i0 \b0 }\par " +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1    a) }{\f0\fs20\cf1 Choice 1}\par " +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1    b) }{\f0\fs20\cf1 Choice 2}\par }";

        private readonly string xmlBoldItalicAndUnderlineInQuestionText =
            "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.FullBeginFont +
            "<b><i><u>Question text</u></i></b>" +
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

        private readonly string fullXmlBoldItalicAndUnderlineInQuestionText =
            "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.FullBeginFont +
            "<b><i><u>Question text</u></i></b>" +
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

        private readonly string rtfBoldItalicAndUnderlineInChoiceTextIn =
            RtfDocument.RtfStringPrefix +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080\plain\f0\fs20 " +
            @"Question text" +
            @"\plain\f0\fs20\par\plain\f0\fs20" +
            @"    a) \b\i\ul Choice 1" +
            @"\plain\f0\fs20\par\plain\f0\fs20" +
            @"    b) Choice 2" +
            @"\par }";

        private readonly string rtfBoldItalicAndUnderlineInChoiceTextOut =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
            @"{\fonttbl" + Environment.NewLine +
            @"{\f0\fswiss Arial;}" + Environment.NewLine +
            @"}" + Environment.NewLine +
            @"\fs20{\colortbl;" + Environment.NewLine +
            @"\red0\green0\blue0;" + Environment.NewLine +
            @"\red255\green255\blue255;" + Environment.NewLine +
            @"}" + Environment.NewLine +
            RtfConstants.DefaultTabsRtf +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1 Question text}\par " +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1    a) }{\f0\fs20\cf1 \b \i \ul Choice 1\ul0 \i0 \b0 }\par " +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1    b) }{\f0\fs20\cf1 Choice 2}\par }";

        private readonly string rtfBoldAndPlainInChoiceTextOut =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
            @"{\fonttbl" + Environment.NewLine +
            @"{\f0\fswiss Arial;}" + Environment.NewLine +
            @"}" + Environment.NewLine +
            @"\fs20{\colortbl;" + Environment.NewLine +
            @"\red0\green0\blue0;" + Environment.NewLine +
            @"\red255\green255\blue255;" + Environment.NewLine +
            @"}" + Environment.NewLine +
            RtfConstants.DefaultTabsRtf +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1 Question text}\par " +
            @"\pard \tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"{\f0\fs20\cf1    a) }{\f0\fs20\cf1 \b Choice\b0 }{\f0\fs20\cf1  1}\par }";

        private readonly string xmlBoldItalicAndUnderlineInChoiceText =
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
            XmlConstants.FullBeginFont +
            "<b><i><u>Choice 1</u></i></b>" +
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

        private readonly string fullXmlBoldItalicAndUnderlineInChoiceText =
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
            XmlConstants.FullBeginFont +
            "<b><i><u>Choice 1</u></i></b>" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "<choice label=\"b\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.FullBeginFont +
            "Choice 2" +
            "</font>" +
            "</paragraph>" +
            "</choice>" +
            "</mc>\r\n";

        private readonly string xmlBoldAndPlainInChoiceText =
            "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.BeginFont +
            "Question text" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</question>" +
            "<choice label=\"a\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabPositionsString +
            XmlConstants.BeginFont +
            "<b>Choice</b>" +
            XmlConstants.EndFont +
            XmlConstants.BeginFont +
            " 1" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "</mc>\r\n";

        private readonly string fullXmlBoldAndPlainInChoiceText =
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
            XmlConstants.FullBeginFont +
            "<b>Choice</b>" +
            XmlConstants.EndFont +
            XmlConstants.FullBeginFont +
            " 1" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "</mc>\r\n";

        [Test]
        public void BoldAndPlainInChoiceXmlProducesBoldAndPlainInChoiceRtf()
        {
            IXmlElement element = new XmlElement(fullXmlBoldAndPlainInChoiceText);
            var mcItem = new McqItem(element);

            Console.WriteLine(mcItem.ToRtf());
            Assert.AreEqual(rtfBoldAndPlainInChoiceTextOut, mcItem.ToRtf());
        }

        [Test]
        public void BoldAndPlainInChoiceXmlProducesBoldAndPlainInChoiceXml()
        {
            IXmlElement element = new XmlElement(xmlBoldAndPlainInChoiceText);
            var mcItem = new McqItem(element);

            Assert.AreEqual(xmlBoldAndPlainInChoiceText, mcItem.ToXml("Q1"));
        }

        [Test]
        public void MixedFormatsInChoiceRtfProducesMixedFormatsInChoiceRtf()
        {
            var mcItem = new McqItem();
            mcItem.Rtf = rtfBoldItalicAndUnderlineInChoiceTextIn;

            Assert.AreEqual(rtfBoldItalicAndUnderlineInChoiceTextOut, mcItem.ToRtf());
        }

        [Test]
        public void MixedFormatsInChoiceRtfProducesMixedFormatsInChoiceXml()
        {
            var mcItem = new McqItem();
            mcItem.Rtf = rtfBoldItalicAndUnderlineInChoiceTextIn;

            Assert.AreEqual(xmlBoldItalicAndUnderlineInChoiceText, mcItem.ToXml("Q1"));
        }

        [Test]
        public void MixedFormatsInChoiceXmlProducesMixedFormatsInChoiceRtf()
        {
            IXmlElement element = new XmlElement(fullXmlBoldItalicAndUnderlineInChoiceText);
            var mcItem = new McqItem(element);

            Assert.AreEqual(rtfBoldItalicAndUnderlineInChoiceTextOut, mcItem.ToRtf());
        }

        [Test]
        public void MixedFormatsInChoiceXmlProducesMixedFormatsInChoiceXml()
        {
            IXmlElement element = new XmlElement(xmlBoldItalicAndUnderlineInChoiceText);
            var mcItem = new McqItem(element);

            Assert.AreEqual(xmlBoldItalicAndUnderlineInChoiceText, mcItem.ToXml("Q1"));
        }

        [Test]
        public void MixedFormatsInQuestionInXmlProducesMixedFormatsInQuestionInXml()
        {
            IXmlElement element = new XmlElement(xmlBoldItalicAndUnderlineInQuestionText);
            var mcItem = new McqItem(element);

            Assert.AreEqual(xmlBoldItalicAndUnderlineInQuestionText, mcItem.ToXml("Q1"));
        }

        [Test]
        public void MixedFormatsInQuestionRtfProducesMixedFormatsInQuestionRtf()
        {
            var mcItem = new McqItem();
            mcItem.Rtf = rtfBoldItalicAndUnderlineInQuestionTextIn;

            Assert.AreEqual(rtfBoldItalicAndUnderlineInQuestionTextOut, mcItem.ToRtf());
        }

        [Test]
        public void MixedFormatsInQuestionRtfProducesMixedFormatsInQuestionXml()
        {
            var mcItem = new McqItem();
            mcItem.Rtf = rtfBoldItalicAndUnderlineInQuestionTextIn;

            Assert.AreEqual(xmlBoldItalicAndUnderlineInQuestionText, mcItem.ToXml("Q1"));
        }

        [Test]
        public void MixedFormatsInQuestionXmlProducesMixedFormatsInQuestionRtf()
        {
            IXmlElement element = new XmlElement(fullXmlBoldItalicAndUnderlineInQuestionText);
            var mcItem = new McqItem(element);

            Assert.AreEqual(rtfBoldItalicAndUnderlineInQuestionTextOut, mcItem.ToRtf());
        }
    }
}