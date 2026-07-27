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
    /// Acceptance tests for Tab characters for layout in Text Items (Story 1762).
    /// </summary>
    [TestFixture]
    public class TabCharactersTest1762
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
                @" Column 2" +
                @"\tab" +
                @" Column 3" +
                @"\par }";

            var item = new TextItem();
            form.ItemList.Add(item);

            item.Rtf = rtfString;

            string expectedRtf =
                RtfConstants.BasicRtfThemePrologue +
                @"\pard " +
                @"\tx1440\tx2880" +
                @"{\f0\fs20\cf1 " +
                @"Column 1}" +
                @"\tab " +
                @"{\f0\fs20\cf1 " +
                @"Column 2}" +
                @"\tab " +
                @"{\f0\fs20\cf1 " +
                @"Column 3}" +
                @"\par }";

            Console.WriteLine(item.ToRtf());
            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void TabPositionsInRtfProduceTabStopsInXml()
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
                @" Column 2" +
                @"\tab" +
                @" Column 3" +
                @"\par }";

            var item = new TextItem();
            form.ItemList.Add(item);

            item.Rtf = rtfString;

            string expectedXml =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Column 1" +
                XmlConstants.EndFont +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Column 2" +
                XmlConstants.EndFont +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Column 3" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</text>\r\n";

            Assert.AreEqual(expectedXml, item.ToXml("T1"));
        }

        [Test]
        public void TabStopsInXmlProduceTabStopsInXml()
        {
            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
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
                "</tabPositions>" +
                XmlConstants.BeginFont +
                "Column 1" +
                XmlConstants.EndFont +
                "<tab/>" +
                XmlConstants.BeginFont +
                "Column 2" +
                XmlConstants.EndFont +
                "<tab/>" +
                XmlConstants.BeginFont +
                "Column 3" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</text>\r\n";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new TextItem(element);

            Assert.AreEqual(xmlString, item.ToXml("T1"));
        }

        [Test]
        public void TextBetweenXmlTabsProducesTextBetweenTextTabs()
        {
            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
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
                "</text>";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new TextItem(element);

            Assert.AreEqual("Column 1\tColumn 2\tColumn 3", item.Text);
        }
    }
}