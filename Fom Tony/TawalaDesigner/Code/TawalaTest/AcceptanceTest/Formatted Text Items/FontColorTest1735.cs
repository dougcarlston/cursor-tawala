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
    /// Acceptance tests for story 1735 (Font color in Text Items).
    /// </summary>
    [TestFixture]
    public class FontColorTest1735
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
        public void FontColorInRtfProducesFontColorInRtf()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red255\green0" +
                @"\blue0;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\plain\f0\fs24\cf3" +
                @" Red" +
                @"\par }";

            var item = new TextItem();
            form.ItemList.Add(item);

            item.Rtf = rtfString;

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"{\f0\fswiss Arial;}" + Environment.NewLine +
                RtfConstants.DefaultFontNameRtf + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20{\colortbl;" + Environment.NewLine +
                @"\red0\green0\blue0;" + Environment.NewLine +
                @"\red255\green255\blue255;" + Environment.NewLine +
                @"\red0\green0\blue1;" + Environment.NewLine +
                @"\red255\green0\blue0;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"{\f0\fs24\cf4 " +
                @"Red}" +
                @"\par }";

            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void FontColorInRtfProducesFontColorInXml()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red255\green0" +
                @"\blue0;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\plain\f0\fs24\cf3" +
                @" Red" +
                @"\par }";

            var item = new TextItem();
            form.ItemList.Add(item);

            item.Rtf = rtfString;

            string expectedXml =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"240\" color=\"FF0000\">Red</font>" +
                "</paragraph>" +
                "</text>\r\n";

            Assert.AreEqual(expectedXml, item.ToXml("T1"));
        }

        [Test]
        public void FontColorInXmlProducesFontColorInRtf()
        {
            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"240\" color=\"FF0000\">Red</font>" +
                "</paragraph>" +
                "</text>\r\n";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new TextItem(element);

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"{\f0\fswiss Arial;}" + Environment.NewLine +
                RtfConstants.DefaultFontNameRtf + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20{\colortbl;" + Environment.NewLine +
                @"\red0\green0\blue0;" + Environment.NewLine +
                @"\red255\green255\blue255;" + Environment.NewLine +
                @"\red0\green0\blue1;" + Environment.NewLine +
                @"\red255\green0\blue0;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"{\f0\fs24\cf4 " +
                @"Red}" +
                @"\par }";

            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void FontColorInXmlProducesFontColorInXml()
        {
            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font size=\"240\" color=\"FF0000\">Red</font>" +
                "</paragraph>" +
                "</text>\r\n";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new TextItem(element);

            Assert.AreEqual(xmlString, item.ToXml("T1"));
        }
    }
}