// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.RtfSupport;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItems
{
    /// <summary>
    /// Acceptance tests for story 1759 (Fields in MCQs).
    /// </summary>
    [TestFixture]
    public class Story1759_FieldsInMCQs
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();
            referenceField = new FibItem();
            form.ItemList.Add(referenceField);
        }

        #endregion

        private IForm form;
        private FibItem referenceField;

        private const string halfInchTabStopString =
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

        private readonly string rtfFieldInQuestionString1 =
            RtfDocument.RtfStringPrefix +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\plain\f0\fs20 " +
            @"MCQ with a Field " +
            @"\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags219";

        private string rtfFieldInQuestionString2 =
            @"\txfielddataval{0}";

        private readonly string rtfFieldInQuestionString3 =
            @"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
            @"<<Form 1:Q1:a>>{" +
            @"\*\txfieldend}\par }";

        private readonly string xmlFieldInQuestionString =
            "<mc label=\"Q2\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabStopString +
            XmlConstants.FullBeginFont +
            "MCQ with a Field " +
            XmlConstants.EndFont +
            XmlConstants.FullBeginFont +
            "<field name=\"Form 1:Q1:a\"/>" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</question>" +
            "</mc>\r\n";

        private readonly string fullXmlFieldInQuestionString =
            "<mc label=\"Q2\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabStopString +
            XmlConstants.FullBeginFont +
            "MCQ with a Field " +
            XmlConstants.EndFont +
            XmlConstants.FullBeginFont +
            "<field name=\"Form 1:Q1:a\"/>" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</question>" +
            "</mc>\r\n";

        private string rtfTestString =
            RtfDocument.RtfStringPrefix +
            @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"\itap0\plain\f0\fs20 " +
            @"Question text " +
            @"\par" +
            @"    a) Choice with field " +
            @"\par }";

        private readonly string rtfFieldAndTextInChoiceString1 =
            RtfDocument.RtfStringPrefix +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"\itap0\plain\f0\fs20 " +
            @"Question text " +
            @"\par" +
            @"    a) Choice with field " +
            @"\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags219";

        private readonly string rtfFieldInChoiceString1 =
            RtfDocument.RtfStringPrefix +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
            @"\itap0\plain\f0\fs20 " +
            @"Question text " +
            @"\par" +
            @"\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags219";

        private string rtfFieldInChoiceString2 =
            @"\txfielddataval{0}";

        private readonly string rtfFieldInChoiceString3 =
            @"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
            @"<<Form 1:Q1:a>>{" +
            @"\*\txfieldend}\par }";

        private readonly string xmlFieldAndTextInChoiceString =
            "<mc label=\"Q2\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabStopString +
            XmlConstants.FullBeginFont +
            "Question text " +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</question>" +
            "<choice label=\"a\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabStopString +
            XmlConstants.FullBeginFont +
            "Choice with field " +
            XmlConstants.EndFont +
            XmlConstants.FullBeginFont +
            "<field name=\"Form 1:Q1:a\"/>" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "</mc>\r\n";

        private readonly string fullXmlFieldAndTextInChoiceString =
            "<mc label=\"Q2\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabStopString +
            XmlConstants.FullBeginFont +
            "Question text " +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</question>" +
            "<choice label=\"a\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabStopString +
            XmlConstants.FullBeginFont +
            "Choice with field " +
            XmlConstants.EndFont +
            XmlConstants.FullBeginFont +
            "<field name=\"Form 1:Q1:a\"/>" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "</mc>\r\n";

        private readonly string xmlFieldInChoiceString =
            "<mc label=\"Q2\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabStopString +
            XmlConstants.FullBeginFont +
            "Question text " +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</question>" +
            "<choice label=\"a\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabStopString +
            XmlConstants.FullBeginFont +
            "<field name=\"Form 1:Q1:a\"/>" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "</mc>\r\n";

        private readonly string fullXmlFieldInChoiceString =
            "<mc label=\"Q2\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            "<question>" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabStopString +
            XmlConstants.FullBeginFont +
            "Question text " +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</question>" +
            "<choice label=\"a\">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            halfInchTabStopString +
            XmlConstants.FullBeginFont +
            "<field name=\"Form 1:Q1:a\"/>" +
            XmlConstants.EndFont +
            "</paragraph>" +
            "</choice>" +
            "</mc>\r\n";

        [Test]
        public void FieldAndTextInChoiceRtfProducesFieldAndTextInChoiceRtf()
        {
            string rtfString =
                rtfFieldAndTextInChoiceString1 +
                string.Format(rtfFieldInChoiceString2, referenceField.BlankList[0].Id) +
                rtfFieldInChoiceString3;

            var mcItem = new McqItem();
            mcItem.Rtf = rtfString;

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"{\f0\fswiss Arial;}" + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20" +
                @"{\colortbl;" + Environment.NewLine +
                @"\red0\green0\blue0;" + Environment.NewLine +
                @"\red255\green255\blue255;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
                @"{\f0\fs20\cf1 Question text }" +
                @"\par " +
                @"\pard " +
                @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
                @"{\f0\fs20\cf1    a) }{\f0\fs20\cf1 Choice with field }" +
                @"{\f0\fs20\cf1 {\*\txfieldstart\txfieldtype0\txfieldflags219" +
                string.Format(rtfFieldInChoiceString2, referenceField.BlankList[0].Id) +
                @"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
                @"<<Form 1:Q1:a>>{" +
                @"\*\txfieldend}}\par }";

            Assert.AreEqual(expectedRtf, mcItem.ToRtf());
        }

        [Test]
        public void FieldAndTextInChoiceRtfProducesFieldAndTextInChoiceXml()
        {
            string rtfString =
                rtfFieldAndTextInChoiceString1 +
                string.Format(rtfFieldInChoiceString2, referenceField.BlankList[0].Id) +
                rtfFieldInChoiceString3;

            var mcItem = new McqItem();
            mcItem.Rtf = rtfString;

            Console.WriteLine(mcItem.ToXml("Q2"));

            Assert.AreEqual(xmlFieldAndTextInChoiceString, mcItem.ToXml("Q2"));
        }

        [Test]
        public void FieldAndTextInChoiceXmlProducesFieldAndTextInChoiceRtf()
        {
            IXmlElement element = new XmlElement(fullXmlFieldAndTextInChoiceString);
            var mcItem = new McqItem(element);

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"{\f0\fswiss Arial;}" + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20" +
                @"{\colortbl;" + Environment.NewLine +
                @"\red0\green0\blue0;" + Environment.NewLine +
                @"\red255\green255\blue255;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
                @"{\f0\fs20\cf1 Question text }" +
                @"\par " +
                @"\pard " +
                @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
                @"{\f0\fs20\cf1    a) }{\f0\fs20\cf1 Choice with field }" +
                @"{\f0\fs20\cf1 {\*\txfieldstart\txfieldtype0\txfieldflags219" +
                string.Format(rtfFieldInChoiceString2, referenceField.BlankList[0].Id) +
                @"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
                @"<<Form 1:Q1:a>>{" +
                @"\*\txfieldend}}\par }";

            Console.WriteLine(mcItem.ToRtf());
            Assert.AreEqual(expectedRtf, mcItem.ToRtf());
        }

        [Test]
        public void FieldAndTextInChoiceXmlProducesFieldAndTextInChoiceXml()
        {
            IXmlElement element = new XmlElement(xmlFieldAndTextInChoiceString);
            var mcItem = new McqItem(element);

            Assert.AreEqual(xmlFieldAndTextInChoiceString, mcItem.ToXml("Q2"));
        }

        [Test]
        public void FieldInChoiceRtfProducesFieldInChoiceRtf()
        {
            string rtfString =
                rtfFieldInChoiceString1 +
                string.Format(rtfFieldInChoiceString2, referenceField.BlankList[0].Id) +
                rtfFieldInChoiceString3;

            var mcItem = new McqItem();
            mcItem.Rtf = rtfString;

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"{\f0\fswiss Arial;}" + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20" +
                @"{\colortbl;" + Environment.NewLine +
                @"\red0\green0\blue0;" + Environment.NewLine +
                @"\red255\green255\blue255;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
                @"{\f0\fs20\cf1 Question text }" +
                @"\par " +
                @"\pard " +
                @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
                @"{\f0\fs20\cf1    a) }{\f0\fs20\cf1 {\*\txfieldstart\txfieldtype0\txfieldflags219" +
                string.Format(rtfFieldInChoiceString2, referenceField.BlankList[0].Id) +
                @"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
                @"<<Form 1:Q1:a>>{" +
                @"\*\txfieldend}}\par }";

            Assert.AreEqual(expectedRtf, mcItem.ToRtf());
        }

        [Test]
        public void FieldInChoiceRtfProducesFieldInChoiceXml()
        {
            string rtfString =
                rtfFieldInChoiceString1 +
                string.Format(rtfFieldInChoiceString2, referenceField.BlankList[0].Id) +
                rtfFieldInChoiceString3;

            var mcItem = new McqItem();
            mcItem.Rtf = rtfString;

            Console.WriteLine(mcItem.ToXml("Q2"));

            Assert.AreEqual(xmlFieldInChoiceString, mcItem.ToXml("Q2"));
        }

        [Test]
        public void FieldInChoiceXmlProducesFieldInChoiceRtf()
        {
            IXmlElement element = new XmlElement(fullXmlFieldInChoiceString);
            var mcItem = new McqItem(element);

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"{\f0\fswiss Arial;}" + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20" +
                @"{\colortbl;" + Environment.NewLine +
                @"\red0\green0\blue0;" + Environment.NewLine +
                @"\red255\green255\blue255;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
                @"{\f0\fs20\cf1 Question text }" +
                @"\par " +
                @"\pard " +
                @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
                @"{\f0\fs20\cf1    a) }{\f0\fs20\cf1 {\*\txfieldstart\txfieldtype0\txfieldflags219" +
                string.Format(rtfFieldInChoiceString2, referenceField.BlankList[0].Id) +
                @"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
                @"<<Form 1:Q1:a>>{" +
                @"\*\txfieldend}}\par }";

            Console.WriteLine(mcItem.ToRtf());
            Assert.AreEqual(expectedRtf, mcItem.ToRtf());
        }

        [Test]
        public void FieldInChoiceXmlProducesFieldInChoiceXml()
        {
            IXmlElement element = new XmlElement(xmlFieldInChoiceString);
            var mcItem = new McqItem(element);

            Assert.AreEqual(xmlFieldInChoiceString, mcItem.ToXml("Q2"));
        }

        [Test]
        public void FieldInQuestionRtfProducesFieldInQuestionRtf()
        {
            string rtfString =
                rtfFieldInQuestionString1 +
                string.Format(rtfFieldInQuestionString2, referenceField.BlankList[0].Id) +
                rtfFieldInQuestionString3;

            var mcItem = new McqItem();
            mcItem.Rtf = rtfString;

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"{\f0\fswiss Arial;}" + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20" +
                @"{\colortbl;" + Environment.NewLine +
                @"\red0\green0\blue0;" + Environment.NewLine +
                @"\red255\green255\blue255;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"{\f0\fs20\cf1 MCQ with a Field }" +
                @"{\f0\fs20\cf1 {\*\txfieldstart\txfieldtype0\txfieldflags219" +
                string.Format(rtfFieldInQuestionString2, referenceField.BlankList[0].Id) +
                @"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
                @"<<Form 1:Q1:a>>{" +
                @"\*\txfieldend}}\par }";

            Assert.AreEqual(expectedRtf, mcItem.ToRtf());
        }

        [Test]
        public void FieldInQuestionRtfProducesFieldInQuestionXml()
        {
            string rtfString =
                rtfFieldInQuestionString1 +
                string.Format(rtfFieldInQuestionString2, referenceField.BlankList[0].Id) +
                rtfFieldInQuestionString3;

            var mcItem = new McqItem();
            mcItem.Rtf = rtfString;

            string expectedXml =
                "<mc label=\"Q2\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
                "<question>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "MCQ with a Field " +
                XmlConstants.EndFont +
                XmlConstants.FullBeginFont +
                "<field name=\"Form 1:Q1:a\"/>" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</question>" +
                "</mc>\r\n";

            Assert.AreEqual(expectedXml, mcItem.ToXml("Q2"));
        }

        [Test]
        public void FieldInQuestionXmlProducesFieldInQuestionRtf()
        {
            IXmlElement element = new XmlElement(fullXmlFieldInQuestionString);
            var mcItem = new McqItem(element);

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"{\f0\fswiss Arial;}" + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20" +
                @"{\colortbl;" + Environment.NewLine +
                @"\red0\green0\blue0;" + Environment.NewLine +
                @"\red255\green255\blue255;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080" +
                @"{\f0\fs20\cf1 MCQ with a Field }" +
                @"{\f0\fs20\cf1 {\*\txfieldstart\txfieldtype0\txfieldflags219" +
                string.Format(rtfFieldInQuestionString2, referenceField.BlankList[0].Id) +
                @"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
                @"<<Form 1:Q1:a>>{" +
                @"\*\txfieldend}}\par }";

            Assert.AreEqual(expectedRtf, mcItem.ToRtf());
        }

        [Test]
        public void FieldInQuestionXmlProducesFieldInQuestionXml()
        {
            IXmlElement element = new XmlElement(fullXmlFieldInQuestionString);
            var mcItem = new McqItem(element);

            Assert.AreEqual(xmlFieldInQuestionString, mcItem.ToXml("Q2"));
        }
    }
}