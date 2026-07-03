// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormattedFIBs
{
    /// <summary>
    /// Acceptance tests for story 1731 (Tab characters for layout in FIBs).
    /// </summary>
    [TestFixture]
    public class Story1731TestTabCharacters
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();
        }

        #endregion

        private IForm form;
        private string defaultFibStyleAtttribute = " style=\"topLabels\"";

        [Test]
        public void BlankBetweenXmlTabsProducesUnderscoresBetweenTextTabs()
        {
            string xmlString =
                "<fib label=\"Q1\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<tabPositions>" +
                "<tabStop position=\"1440\"/>" +
                "<tabStop position=\"2880\"/>" +
                "</tabPositions>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Column 1</font>" +
                "<tab/>" +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
                "<tab/>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Column 3</font>" +
                "</paragraph>" +
                "</fib>";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new FibItem(element);

            Assert.AreEqual("Column 1\t__________\tColumn 3", item.Text);
        }

        [Test]
        public void TabPositionsInRtfProduceTabPositionsInRtf()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\tx1440\tx2880\plain\f0\fs20" +
                @" Column 1" +
                @"\tab" +
                @" __________" +
                @"\tab" +
                @" Column 3" +
                @"\par }";

            var item = new FibItem();
            form.ItemList.Add(item);

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
                @"\tx1440\tx2880" +
                @"{\f0\fs20\cf1 " +
                @"Column 1}" +
                @"\tab " +
                @"__________" +
                @"\tab " +
                @"{\f0\fs20\cf1 " +
                @"Column 3}" +
                @"\par }";

            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void TabStopsInRtfProduceTabStopsInXml()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\plain\f0\fs20" +
                @" Column 1" +
                @"\tab" +
                @" __________" +
                @"\tab" +
                @" Column 3" +
                @"\par }";

            var item = new FibItem();
            form.ItemList.Add(item);

            item.Rtf = rtfString;

            string expectedXml =
                "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Column 1" +
                XmlConstants.EndFont +
                "<tab/>" +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Column 3" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</fib>\r\n";

            Assert.AreEqual(expectedXml, item.ToXml("Q1"));
        }

        [Test]
        public void TextBetweenXmlTabsProducesTextBetweenTextTabs()
        {
            string xmlString =
                "<fib label=\"Q1\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<tabPositions>" +
                "<tabStop position=\"1440\"/>" +
                "<tabStop position=\"2880\"/>" +
                "</tabPositions>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Column 1</font>" +
                "<tab/>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Column 2</font>" +
                "<tab/>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Column 3</font>" +
                "</paragraph>" +
                "</fib>";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new FibItem(element);

            Assert.AreEqual("Column 1\tColumn 2\tColumn 3", item.Text);
        }

        [Test]
        public void UnderscoresBetweenRtfTabsProduceBlankBetweenXmlTabs()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\plain\f0\fs20" +
                @" Column 1" +
                @"\tab" +
                @" __________" +
                @"\tab" +
                @" Column 3" +
                @"\par }";

            var item = new FibItem();
            form.ItemList.Add(item);

            item.Rtf = rtfString;

            string expectedXml =
                "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Column 1" +
                XmlConstants.EndFont +
                "<tab/>" +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Column 3" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</fib>\r\n";

            Assert.AreEqual(expectedXml, item.ToXml("Q1"));
        }

        [Test]
        public void UnderscoresBetweenRtfTabsProduceUnderscoresBetweenRtfTabs()
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
                @" Column 1" +
                @"\tab" +
                @" __________" +
                @"\tab" +
                @" Column 3" +
                @"\par }";

            var item = new FibItem();
            form.ItemList.Add(item);

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
                @"{\f0\fs20\cf1 " +
                @"Column 1}" +
                @"\tab " +
                @"__________" +
                @"\tab " +
                @"{\f0\fs20\cf1 " +
                @"Column 3}" +
                @"\par }";

            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void UnderscoresBetweenRtfTabsProduceUnderscoresBetweenTextTabs()
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
                @" Column 1" +
                @"\tab" +
                @" __________" +
                @"\tab" +
                @" Column 3" +
                @"\par }";

            var item = new FibItem();
            form.ItemList.Add(item);

            item.Rtf = rtfString;

            Assert.AreEqual("Column 1\t__________\tColumn 3", item.Text);
        }
    }
}