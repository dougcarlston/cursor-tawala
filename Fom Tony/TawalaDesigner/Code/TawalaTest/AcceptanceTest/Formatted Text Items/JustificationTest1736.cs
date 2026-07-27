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
    /// Acceptance tests for story 1736 (Indent and Justification in Text Items).
    /// </summary>
    [TestFixture]
    public class JustificationTest1736
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
        public void JustificationInRtfProducesJustificationinXml()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\qc\plain\f0\fs24" +
                @" Some Text" +
                @"\par }";

            var item = new TextItem();
            form.ItemList.Add(item);

            item.Rtf = rtfString;

            string expectedXml =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"center\">" +
                "Some Text" +
                "</paragraph>" +
                "</text>\r\n";

            Assert.AreEqual(expectedXml, item.ToXml("T1"));
        }

        [Test]
        public void JustificationInXmlProducesJustificationinRtf()
        {
            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"center\">" +
                "Some Text" +
                "</paragraph>" +
                "</text>\r\n";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new TextItem(element);

            string expectedRtf =
                RtfConstants.BasicRtfThemePrologue +
                @"\pard " +
                @"\qc " +
                @"Some Text" +
                @"\par }";

            Assert.AreEqual(expectedRtf, item.ToRtf());
        }

        [Test]
        public void JustificationInXmlProducesJustificationinXml()
        {
            string xmlString =
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"justify\">" +
                XmlConstants.BeginFont +
                "Arial" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</text>\r\n";

            IXmlElement element = new XmlElement(xmlString, true);
            var item = new TextItem(element);

            Assert.AreEqual(xmlString, item.ToXml("T1"));
        }

        [Test]
        public void JustifictionInRtfProducesJustificationinRtf()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs20 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\qc " +
                @" Some Text" +
                @"\par }";

            var item = new TextItem();
            form.ItemList.Add(item);

            item.Rtf = rtfString;

            string expectedRtf =
                RtfConstants.BasicRtfThemePrologue +
                @"\pard " +
                @"\qc " +
                @" Some Text" +
                @"\par }";

            Assert.AreEqual(expectedRtf, item.ToRtf());
        }
    }
}