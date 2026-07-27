// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormattedMCQs
{
    /// <summary>
    /// Acceptance tests for story 1739 (Convert MCQs to use TX text control).
    /// </summary>
    [TestFixture]
    public class RtfTest1739
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        [Test]
        public void ChoiceRtfProducesChoiceRtf()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\plain\f0\fs20" +
                @" Choose one of the following:" +
                @"\par" +
                @"    a) Choice 1" +
                @"\par" +
                @"    b) Choice 2" +
                @"\par }";

            var item = new McqItem();
            item.Rtf = rtfString;

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
                @"{\f0\fs20\cf1 Choose one of the following:}\par " +
                @"\pard " +
                @"{\f0\fs20\cf1    a) }{\f0\fs20\cf1 Choice 1}\par " +
                @"\pard " +
                @"{\f0\fs20\cf1    b) }{\f0\fs20\cf1 Choice 2}\par }";

            Console.WriteLine(item.ToRtf());
            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void EmptyChoiceRtfProducesEmptyChoiceXml()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\plain\f0\fs20" +
                @" Choose one of the following:" +
                @"\par" +
                @"    a) " +
                @"\par ";

            var item = new McqItem();
            item.Rtf = rtfString;

            string expectedXml =
                "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
                "<question>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Choose one of the following:" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</question>" +
                "<choice label=\"a\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "</choice>" +
                "</mc>\r\n";

            Assert.AreEqual(expectedXml, item.ToXml("Q1"));
        }

        [Test]
        public void EmptyChoiceXmlProducesEmptyChoiceXml()
        {
            string xmlString =
                "<mc label=\"Q1\" onlyone=\"false\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
                "<question>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.BeginFont +
                "Choose one of the following:" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</question>" +
                "<choice label=\"a\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.BeginFont +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</choice>" +
                "</mc>\r\n";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new McqItem(element);

            Assert.AreEqual(xmlString, item.ToXml("Q1"));
        }

        [Test]
        public void FormattedChoiceRtfProducesFormattedChoiceXml()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\plain\f0\fs20" +
                @" Choose one of the following:" +
                @"\par" +
                @"    a) Choice 1" +
                @"\par" +
                @"    b) Choice 2" +
                @"\par }";

            var item = new McqItem();
            item.Rtf = rtfString;

            string expectedXml =
                "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
                "<question>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Choose one of the following:" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</question>" +
                "<choice label=\"a\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Choice 1" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</choice>" +
                "<choice label=\"b\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Choice 2" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</choice>" +
                "</mc>\r\n";

            Console.WriteLine(item.ToXml("Q1"));
            Assert.AreEqual(expectedXml, item.ToXml("Q1"));
        }

        [Test]
        public void FormattedChoiceXmlProducesFormattedChoiceHtml()
        {
            string xmlString =
                "<choice label=\"a\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Cyan</font>" +
                "</paragraph>" +
                "</choice>";

            IXmlElement element = new XmlElement(xmlString, true);
            var choice = new Choice(element);

            string expectedHtml =
                "<div class=\"answer\">" +
                "<p>" +
                "<input name=\"Q1\" type=\"checkbox\" value=\"a\" />" +
                "<span style=\"font-family: Arial; font-size: 10pt; color:#000000;\">Cyan</span>" +
                "</p>" +
                "</div>";

            Assert.AreEqual(expectedHtml, choice.ToHtml("Q1", "checkbox"));
        }

        [Test]
        public void FormattedChoiceXmlProducesFormattedChoiceRtf()
        {
            string xmlString =
                "<mc label=\"Q1\">" +
                "<question>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Choose one of the following:</font>" +
                "</paragraph>" +
                "</question>" +
                "<choice label=\"a\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Choice 1</font>" +
                "</paragraph>" +
                "</choice>" +
                "<choice label=\"b\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Choice 2</font>" +
                "</paragraph>" +
                "</choice>" +
                "</mc>";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new McqItem(element);

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
                @"{\f0\fs20\cf1 Choose one of the following:}\par " +
                @"\pard " +
                @"{\f0\fs20\cf1    a) }{\f0\fs20\cf1 Choice 1}\par " +
                @"\pard " +
                @"{\f0\fs20\cf1    b) }{\f0\fs20\cf1 Choice 2}\par }";

            Console.WriteLine(item.ToRtf());
            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void FormattedChoiceXmlProducesFormattedChoiceXml()
        {
            string xmlString =
                "<mc label=\"Q1\" onlyone=\"false\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
                "<question>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.BeginFont +
                "Choose one of the following:" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</question>" +
                "<choice label=\"a\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.BeginFont +
                "Choice 1" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</choice>" +
                "<choice label=\"b\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.BeginFont +
                "Choice 2" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</choice>" +
                "</mc>\r\n";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new McqItem(element);

            Assert.AreEqual(xmlString, item.ToXml("Q1"));
        }

        [Test]
        public void FormattedQuestionXmlProducesFormattedQuestionRtf()
        {
            string xmlString =
                "<mc label=\"Q1\">" +
                "<question>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Choose one of the following:</font>" +
                "</paragraph>" +
                "</question>" +
                "</mc>";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new McqItem(element);

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
                @"{\f0\fs20\cf1 Choose one of the following:}\par }";

            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void FormattedQuestionXmlProducesFormattedQuestionXml()
        {
            string xmlString =
                "<mc label=\"Q1\" onlyone=\"false\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
                "<question>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.BeginFont +
                "Choose one of the following:" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</question>" +
                "</mc>\r\n";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new McqItem(element);

            Assert.AreEqual(xmlString, item.ToXml("Q1"));
        }

        [Test]
        public void QuestionRtfProducesQuestionRtf()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\plain\f0\fs20" +
                @" Choose one of the following:" +
                @"\par }";

            var item = new McqItem();
            item.Rtf = rtfString;

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
                @"{\f0\fs20\cf1 Choose one of the following:}\par }";

            Console.WriteLine(item.ToRtf());
            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void UnformattedChoiceXmlProducesFormattedChoiceRtf()
        {
            string xmlString =
                "<mc label=\"Q1\">" +
                "<question>" +
                "Choose one of the following:" +
                "</question>" +
                "<choice label=\"a\">" +
                "Choice 1" +
                "</choice>" +
                "<choice label=\"b\">" +
                "Choice 2" +
                "</choice>" +
                "</mc>";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new McqItem(element);

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
                @"{\f0\fs20\cf1 Choose one of the following:}\par " +
                @"\pard " +
                @"{\f0\fs20\cf1    a) }{\f0\fs20\cf1 Choice 1}\par " +
                @"\pard " +
                @"{\f0\fs20\cf1    b) }{\f0\fs20\cf1 Choice 2}\par }";

            Console.WriteLine(item.ToRtf());
            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void UnformattedQuestionXmlProducesFormattedQuestionRtf()
        {
            string xmlString =
                "<mc label=\"Q1\">" +
                "<question>Choose one of the following:</question>" +
                "</mc>";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new McqItem(element);

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
                @"{\f0\fs20\cf1 Choose one of the following:}\par }";

            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void UnformattedQuestionXmlProducesFormattedQuestionXml()
        {
            string xmlString =
                "<mc label=\"Q1\" onlyone=\"false\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
                "<question>" +
                "Choose one of the following:" +
                "</question>" +
                "</mc>\r\n";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new McqItem(element);

            string expectedString =
                "<mc label=\"Q1\" onlyone=\"false\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
                "<question>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Choose one of the following:" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</question>" +
                "</mc>\r\n";

            Assert.AreEqual(expectedString, item.ToXml("Q1"));
        }
    }
}