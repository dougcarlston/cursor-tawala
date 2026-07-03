// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormattedTextItems
{
    /// <summary>
    /// Acceptance tests for story 1573 (Tables in Text Items).
    /// </summary>
    [TestFixture]
    public class TableTest1573
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

        private static readonly string basicRtfPrefix =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
            @"{\fonttbl" + Environment.NewLine +
            @"{\f0\fswiss Arial;}" + Environment.NewLine +
            @"}" + Environment.NewLine +
            @"\fs20" +
            @"{\colortbl;" + Environment.NewLine +
            @"\red0\green0\blue0;" + Environment.NewLine +
            @"\red255\green255\blue255;" + Environment.NewLine +
            @"}" + Environment.NewLine;

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

        [Test]
        public void TableRtfProducesTableRtf()
        {
            var item = new TextItem();
            item.Rtf = rtfTableString;

            string expectedString =
                RtfConstants.BasicRtfThemePrologue +
                @"\pard " +
                @"\par " +
                @"\trowd\trleft0\clftsWidth3\clwWidth5400\cellx5400\clftsWidth3\clwWidth5400\cellx10800" +
                @"\pard\intbl \ql {\f0\fs20\cf1 Cell 1}\cell \ql {\f0\fs20\cf1 Cell 2}\cell \row" +
                @"\trowd\trleft0\clftsWidth3\clwWidth5400\cellx5400\clftsWidth3\clwWidth5400\cellx10800" +
                @"\pard\intbl \ql {\f0\fs20\cf1 Cell 3}\cell \ql {\f0\fs20\cf1 Cell 4}\cell \row" +
                @"\pard " +
                @"\par }";

            Console.WriteLine(item.ToRtf());
            Assert.AreEqual(expectedString, item.ToRtf());
        }

        [Test]
        public void TableRtfProducesTableXml()
        {
            var item = new TextItem();
            item.Rtf = rtfTableString;

            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "<table indent=\"0\">" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Cell 1" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Cell 2" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "</row>" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Cell 3" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Cell 4" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "</row>" +
                "</table>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "</text>\r\n";

            Assert.AreEqual(xmlString, item.ToXml("T1"));
        }

        [Test]
        public void TableXmlProducesTableRtf()
        {
            var item = new TextItem();
            item.Rtf = rtfTableString;

            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "<table indent=\"0\">" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Cell 1" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Cell 2" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "</row>" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Cell 3" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Cell 4" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "</row>" +
                "</table>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "</text>\r\n";

            Assert.AreEqual(xmlString, item.ToXml("T1"));
        }

        [Test]
        public void TableXmlProducesTableXml()
        {
            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "<table indent=\"0\">" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.DefaultBeginFont +
                "Cell 1" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.DefaultBeginFont +
                "Cell 2" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "</row>" +
                "<row>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.DefaultBeginFont +
                "Cell 3" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "<cell width=\"5400\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.DefaultBeginFont +
                "Cell 4" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "</row>" +
                "</table>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "</text>\r\n";

            IXmlElement element = new XmlElement(xmlString);
            var item = new TextItem(element);

            Assert.AreEqual(xmlString, item.ToXml("T1"));
        }
    }
}