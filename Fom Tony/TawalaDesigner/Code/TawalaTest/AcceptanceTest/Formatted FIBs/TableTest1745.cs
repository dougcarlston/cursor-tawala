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
    /// Acceptance tests for story 1745 (Tables in FIBs).
    /// </summary>
    [TestFixture]
    public class TableTest1745
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

        private const string NEWLINE = "\r\n";

        private const string basicRtfPrefix =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + NEWLINE +
            @"{\fonttbl" + NEWLINE +
            @"{\f0\fswiss Arial;}" + NEWLINE +
            @"}" + NEWLINE +
            @"\fs20" +
            @"{\colortbl;" + NEWLINE +
            @"\red0\green0\blue0;" + NEWLINE +
            @"\red255\green255\blue255;" + NEWLINE +
            @"}" + NEWLINE;

        private const string rtfTableString =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
            @"\fs20\par\trowd\irow0\irowband0\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trp" +
            @"addr36\trpaddfl3\trpaddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth5400\cellx5400\clvertal" +
            @"t\clftsWidth3\clwWidth5400\cellx10800\pard\intbl Cell 1\cell Cell 2\cell\row\trowd\irow1\irowband1\l" +
            @"astrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpaddft" +
            @"3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth5400\cellx5400\clvertalt\clftsWidth3\clwWidth540" +
            @"0\cellx10800\pard\intbl Cell 3\cell Cell 4\cell\row\pard\itap0\par }";

        private const string defaultTabPositionsString =
            "<tabPositions>" +
            "<tabStop position=\"1134\"/>" +
            "<tabStop position=\"2268\"/>" +
            "<tabStop position=\"3402\"/>" +
            "<tabStop position=\"4536\"/>" +
            "<tabStop position=\"5670\"/>" +
            "<tabStop position=\"6804\"/>" +
            "<tabStop position=\"7938\"/>" +
            "<tabStop position=\"9072\"/>" +
            "<tabStop position=\"10206\"/>" +
            "<tabStop position=\"11340\"/>" +
            "<tabStop position=\"12474\"/>" +
            "<tabStop position=\"13608\"/>" +
            "<tabStop position=\"14742\"/>" +
            "<tabStop position=\"15876\"/>" +
            "</tabPositions>";

        private const string tenPointFontStartTag = "<font face=\"Arial\" size=\"200\" color=\"000000\">";
        private const string fontEndTag = "</font>";

        [Test]
        [Ignore("Story 1745 on hold - 12/06")]
        public void TableRtfProducesTableRtf()
        {
            var item = new FibItem();
            item.Rtf = rtfTableString;

            string expectedString =
                basicRtfPrefix +
                @"\pard " +
                @"\tx1134\tx2268\tx3402\tx4536\tx5670\tx6804\tx7938\tx9072\tx10206\tx11340\tx12474\tx13608\tx14742\tx15876" +
                @"\par " +
                @"\trowd\trleft0\clftsWidth3\clwWidth5400\cellx5400\clftsWidth3\clwWidth5400\cellx10800" +
                @"\pard\intbl \ql {\f0\fs20\cf1 Cell 1}\cell \ql {\f0\fs20\cf1 Cell 2}\cell \row" +
                @"\trowd\trleft0\clftsWidth3\clwWidth5400\cellx5400\clftsWidth3\clwWidth5400\cellx10800" +
                @"\pard\intbl \ql {\f0\fs20\cf1 Cell 3}\cell \ql {\f0\fs20\cf1 Cell 4}\cell \row" +
                @"\pard " +
                @"\tx1134\tx2268\tx3402\tx4536\tx5670\tx6804\tx7938\tx9072\tx10206\tx11340\tx12474\tx13608\tx14742\tx15876" +
                @"\par }";

            Console.WriteLine(item.ToRtf());
            Assert.AreEqual(expectedString, item.ToRtf());
        }

        [Test]
        [Ignore("Story 1745 on hold - 12/06")]
        public void TableRtfProducesTableXml()
        {
            var item = new TextItem();
            item.Rtf = rtfTableString;

            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                defaultTabPositionsString +
                "</paragraph>" +
                "<table indent=\"0\">" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 1" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 2" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "</row>" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 3" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 4" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "</row>" +
                "</table>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                defaultTabPositionsString +
                "</paragraph>" +
                "</text>\r\n";

            Assert.AreEqual(xmlString, item.ToXml("T1"));
        }

        [Test]
        [Ignore("Story 1745 on hold - 12/06")]
        public void TableXmlProducesTableRtf()
        {
            var item = new TextItem();
            item.Rtf = rtfTableString;

            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                defaultTabPositionsString +
                "</paragraph>" +
                "<table indent=\"0\">" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 1" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 2" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "</row>" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 3" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 4" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "</row>" +
                "</table>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                defaultTabPositionsString +
                "</paragraph>" +
                "</text>\r\n";

            Assert.AreEqual(xmlString, item.ToXml("T1"));
        }

        [Test]
        [Ignore("Story 1745 on hold - 12/06")]
        public void TableXmlProducesTableXml()
        {
            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                defaultTabPositionsString +
                "</paragraph>" +
                "<table indent=\"0\">" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 1" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 2" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "</row>" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 3" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                tenPointFontStartTag +
                "Cell 4" +
                fontEndTag +
                "</division>" +
                "</cell>" +
                "</row>" +
                "</table>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                defaultTabPositionsString +
                "</paragraph>" +
                "</text>\r\n";

            IXmlElement element = new XmlElement(xmlString);
            var item = new TextItem(element);

            Assert.AreEqual(xmlString, item.ToXml("T1"));
        }
    }
}