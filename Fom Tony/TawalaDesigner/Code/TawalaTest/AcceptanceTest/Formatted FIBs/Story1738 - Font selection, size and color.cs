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
    /// Acceptance tests for story 1738 (Font selection, size and color in FIB Items).
    /// </summary>
    [TestFixture]
    public class Story1738_FontSelectionSizeAndColor
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
        private const string defaultFibStyleAtttribute = " style=\"topLabels\"";

        //private string xmlFontSelectionString =
        //    "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
        //    "<paragraph indent=\"0\" align=\"left\">" +
        //    "<font face=\"Times New Roman\" size=\"200\">Times New Roman in FIB </font>" +
        //    "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
        //    "</paragraph>" +
        //    "</fib>\r\n";

        private string fullXmlFontSelectionString =
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            "<font face=\"Times New Roman\" size=\"200\" color=\"000000\">Times New Roman in FIB </font>" +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
            "</paragraph>" +
            "</fib>\r\n";

        private string rtfFontSelectionStringIn =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset0\fprq2 Times New Roman;}" + "\r\n" +
            @"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\plain\f1\fs20 " +
            @"Times New Roman in FIB __________" +
            @"\par }";

        private readonly string rtfFontSelectionStringOut =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
            @"{\fonttbl" + Environment.NewLine +
            @"{\f0\fswiss Arial;}" + Environment.NewLine +
            @"{\f1\fnil Times New Roman;}" + Environment.NewLine +
            @"}" + Environment.NewLine +
            @"\fs20" +
            @"{\colortbl;" + Environment.NewLine +
            @"\red0\green0\blue0;" + Environment.NewLine +
            @"\red255\green255\blue255;" + Environment.NewLine +
            @"}" + Environment.NewLine +
            RtfConstants.DefaultTabsRtf +
            @"\pard " +
            @"{\f1\fs20\cf1 Times New Roman in FIB }" +
            @"__________\par }";

        private string xmlFontSizeString =
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            "<font face=\"Arial\" size=\"280\">Arial 14 </font>" +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
            "</paragraph>" +
            "</fib>\r\n";

        private string fullXmlFontSizeString =
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            "<font face=\"Arial\" size=\"280\" color=\"000000\">Arial 14 </font>" +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
            "</paragraph>" +
            "</fib>\r\n";

        private string rtfFontSizeStringIn =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\plain\f0\fs28" +
            @" Arial 14 __________" +
            @"\par }";

        private readonly string rtfFontSizeStringOut =
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
            @"{\f0\fs28\cf1 Arial 14 }" +
            @"__________\par }";

        private string xmlFontColorString =
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            "<font face=\"Arial\" color=\"FF0000\">Red Text </font>" +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
            "</paragraph>" +
            "</fib>\r\n";

        private string fullXmlFontColorString =
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            "<font face=\"Arial\" size=\"200\" color=\"FF0000\">Red Text </font>" +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
            "</paragraph>" +
            "</fib>\r\n";

        private string rtfFontColorStringIn =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red255\green0\blue0;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\plain\f0\fs20\cf3" +
            @" Red Text __________" +
            @"\par }";

        private readonly string rtfFontColorStringOut =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
            @"{\fonttbl" + Environment.NewLine +
            @"{\f0\fswiss Arial;}" + Environment.NewLine +
            @"}" + Environment.NewLine +
            @"\fs20" +
            @"{\colortbl;" + Environment.NewLine +
            @"\red0\green0\blue0;" + Environment.NewLine +
            @"\red255\green255\blue255;" + Environment.NewLine +
            @"\red255\green0\blue0;" + Environment.NewLine +
            @"}" + Environment.NewLine +
            RtfConstants.DefaultTabsRtf +
            @"\pard " +
            @"{\f0\fs20\cf3 Red Text }" +
            @"__________\par }";

        [Test]
        public void FontColorInRtfProducesFontColorInRtf()
        {
            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);
            fibItem.Rtf = rtfFontColorStringIn;

            Assert.AreEqual(rtfFontColorStringOut, fibItem.ToRtf());
        }

        [Test]
        public void FontColorInRtfProducesFontColorInXml()
        {
            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);
            fibItem.Rtf = rtfFontColorStringIn;

            Assert.AreEqual(fullXmlFontColorString, fibItem.ToXml("Q1"));
        }

        [Test]
        public void FontColorInXmlProducesFontColorInRtf()
        {
            IXmlElement element = new XmlElement(fullXmlFontColorString, true);
            var fibItem = new FibItem(element);

            Assert.AreEqual(rtfFontColorStringOut, fibItem.ToRtf());
        }

        [Test]
        public void FontColorInXmlProducesFontColorInXml()
        {
            IXmlElement element = new XmlElement(xmlFontColorString, true);
            var fibItem = new FibItem(element);

            Assert.AreEqual(xmlFontColorString, fibItem.ToXml("Q1"));
        }

        [Test]
        public void FontSelectionInRtfProducesFontSelectionInRtf()
        {
            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);
            fibItem.Rtf = rtfFontSelectionStringIn;

            Assert.AreEqual(rtfFontSelectionStringOut, fibItem.ToRtf());
        }

        [Test]
        public void FontSelectionInRtfProducesFontSelectionInXml()
        {
            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);
            fibItem.Rtf = rtfFontSelectionStringIn;

            Assert.AreEqual(fullXmlFontSelectionString, fibItem.ToXml("Q1"));
        }

        [Test]
        public void FontSelectionInXmlProducesFontSelectionInRtf()
        {
            IXmlElement element = new XmlElement(fullXmlFontSelectionString, true);
            var fibItem = new FibItem(element);

            Assert.AreEqual(rtfFontSelectionStringOut, fibItem.ToRtf());
        }

        [Test]
        public void FontSelectionInXmlProducesFontSelectionInXml()
        {
            IXmlElement element = new XmlElement(fullXmlFontSelectionString, true);
            var fibItem = new FibItem(element);

            Assert.AreEqual(fullXmlFontSelectionString, fibItem.ToXml("Q1"));
        }

        [Test]
        public void FontSizeInRtfProducesFontSizeInRtf()
        {
            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);
            fibItem.Rtf = rtfFontSizeStringIn;

            Assert.AreEqual(rtfFontSizeStringOut, fibItem.ToRtf());
        }

        [Test]
        public void FontSizeInRtfProducesFontSizeInXml()
        {
            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);
            fibItem.Rtf = rtfFontSizeStringIn;

            Assert.AreEqual(fullXmlFontSizeString, fibItem.ToXml("Q1"));
        }

        [Test]
        public void FontSizeInXmlProducesFontSizeInRtf()
        {
            IXmlElement element = new XmlElement(fullXmlFontSizeString, true);
            var fibItem = new FibItem(element);

            Assert.AreEqual(rtfFontSizeStringOut, fibItem.ToRtf());
        }

        [Test]
        public void FontSizeInXmlProducesFontSizeInXml()
        {
            IXmlElement element = new XmlElement(xmlFontSizeString, true);
            var fibItem = new FibItem(element);

            Assert.AreEqual(fullXmlFontSizeString, fibItem.ToXml("Q1"));
        }
    }
}