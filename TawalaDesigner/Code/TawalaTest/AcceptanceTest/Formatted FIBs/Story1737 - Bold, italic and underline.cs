// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormattedFIBs
{
    /// <summary>
    /// Acceptance tests for story 1737 (Bold, italic and underline in FIBs).
    /// </summary>
    [TestFixture]
    public class Story1737_BoldItalicAndUnderline
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();
        }

        #endregion

        private const string defaultFibStyleAtttribute = " style=\"topLabels\"";

        private IForm form;

        private readonly string xmlBoldString =
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            XmlConstants.FullBeginFont +
            "<b>Bold text</b>" +
            XmlConstants.EndFont +
            XmlConstants.FullBeginFont +
            " in FIB " +
            XmlConstants.EndFont +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
            "</paragraph>" +
            "</fib>\r\n";

        private readonly string fullXmlBoldString =
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            XmlConstants.FullBeginFont +
            "<b>Bold text</b>" +
            XmlConstants.EndFont +
            XmlConstants.FullBeginFont +
            " in FIB " +
            XmlConstants.EndFont +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
            "</paragraph>" +
            "</fib>\r\n";

        private readonly string rtfBoldStringIn =
            RtfDocument.RtfStringPrefix +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\plain\f0\fs20 " +
            @"\b Bold text\b  in FIB __________" +
            @"\par }";

        private readonly string rtfBoldStringOut =
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
            @"{\f0\fs20\cf1 \b Bold text\b0 }" +
            @"{\f0\fs20\cf1  in FIB }__________\par }";

        private readonly string xmlItalicString =
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            XmlConstants.FullBeginFont +
            "<i>Italic text</i>" +
            XmlConstants.EndFont +
            XmlConstants.FullBeginFont +
            " in FIB " +
            XmlConstants.EndFont +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
            "</paragraph>" +
            "</fib>\r\n";

        private readonly string fullXmlItalicString =
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            XmlConstants.FullBeginFont +
            "<i>Italic text</i>" +
            XmlConstants.EndFont +
            XmlConstants.FullBeginFont +
            " in FIB " +
            XmlConstants.EndFont +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
            "</paragraph>" +
            "</fib>\r\n";

        private readonly string rtfItalicStringIn =
            RtfDocument.RtfStringPrefix +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\plain\f0\fs20 " +
            @"\i Italic text\i  in FIB __________" +
            @"\par }";

        private readonly string rtfItalicStringOut =
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
            @"{\f0\fs20\cf1 \i Italic text\i0 }" +
            @"{\f0\fs20\cf1  in FIB }__________\par }";

        private readonly string xmlUnderlineString =
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            XmlConstants.FullBeginFont +
            "<u>Underline text</u>" +
            XmlConstants.EndFont +
            XmlConstants.FullBeginFont +
            " in FIB " +
            XmlConstants.EndFont +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
            "</paragraph>" +
            "</fib>\r\n";

        private readonly string fullXmlUnderlineString =
            "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
            "<paragraph indent=\"0\" align=\"left\">" +
            XmlConstants.FullBeginFont +
            "<u>Underline text</u>" +
            XmlConstants.EndFont +
            XmlConstants.FullBeginFont +
            " in FIB " +
            XmlConstants.EndFont +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
            "</paragraph>" +
            "</fib>\r\n";

        private readonly string rtfUnderlineStringIn =
            RtfDocument.RtfStringPrefix +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\plain\f0\fs20 " +
            @"\ul Underline text\ul  in FIB __________" +
            @"\par }";

        private readonly string rtfUnderlineStringOut =
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
            @"{\f0\fs20\cf1 \ul Underline text\ul0 }" +
            @"{\f0\fs20\cf1  in FIB }__________\par }";

        [Test]
        public void BoldInRtfProducesBoldInRtf()
        {
            var fibItem = new FibItem();
            fibItem.Rtf = rtfBoldStringIn;

            Assert.AreEqual(rtfBoldStringOut, fibItem.ToRtf());
        }

        [Test]
        public void BoldInRtfProducesBoldInXml()
        {
            var fibItem = new FibItem();
            fibItem.Rtf = rtfBoldStringIn;

            Assert.AreEqual(xmlBoldString, fibItem.ToXml("Q1"));
        }

        [Test]
        public void BoldInXmlProducesBoldInRtf()
        {
            IXmlElement element = new XmlElement(fullXmlBoldString);
            var fibItem = new FibItem(element);

            Assert.AreEqual(rtfBoldStringOut, fibItem.ToRtf());
        }

        [Test]
        public void BoldInXmlProducesBoldInXml()
        {
            IXmlElement element = new XmlElement(xmlBoldString);
            var fibItem = new FibItem(element);

            Assert.AreEqual(xmlBoldString, fibItem.ToXml("Q1"));
        }

        [Test]
        public void ItalicInRtfProducesItalicInRtf()
        {
            var fibItem = new FibItem();
            fibItem.Rtf = rtfItalicStringIn;

            Assert.AreEqual(rtfItalicStringOut, fibItem.ToRtf());
        }

        [Test]
        public void ItalicInRtfProducesItalicInXml()
        {
            var fibItem = new FibItem();
            fibItem.Rtf = rtfItalicStringIn;

            Assert.AreEqual(xmlItalicString, fibItem.ToXml("Q1"));
        }

        [Test]
        public void ItalicInXmlProducesItalicInRtf()
        {
            IXmlElement element = new XmlElement(fullXmlItalicString);
            var fibItem = new FibItem(element);

            Assert.AreEqual(rtfItalicStringOut, fibItem.ToRtf());
        }

        [Test]
        public void ItalicInXmlProducesItalicInXml()
        {
            IXmlElement element = new XmlElement(xmlItalicString);
            var fibItem = new FibItem(element);

            Assert.AreEqual(xmlItalicString, fibItem.ToXml("Q1"));
        }

        [Test]
        public void UnderlineInRtfProducesUnderlineInRtf()
        {
            var fibItem = new FibItem();
            fibItem.Rtf = rtfUnderlineStringIn;

            Assert.AreEqual(rtfUnderlineStringOut, fibItem.ToRtf());
        }

        [Test]
        public void UnderlineInRtfProducesUnderlineInXml()
        {
            var fibItem = new FibItem();
            fibItem.Rtf = rtfUnderlineStringIn;

            Assert.AreEqual(xmlUnderlineString, fibItem.ToXml("Q1"));
        }

        [Test]
        public void UnderlineInXmlProducesUnderlineInRtf()
        {
            IXmlElement element = new XmlElement(fullXmlUnderlineString);
            var fibItem = new FibItem(element);

            Assert.AreEqual(rtfUnderlineStringOut, fibItem.ToRtf());
        }

        [Test]
        public void UnderlineInXmlProducesUnderlineInXml()
        {
            IXmlElement element = new XmlElement(xmlUnderlineString);
            var fibItem = new FibItem(element);

            Assert.AreEqual(xmlUnderlineString, fibItem.ToXml("Q1"));
        }
    }
}