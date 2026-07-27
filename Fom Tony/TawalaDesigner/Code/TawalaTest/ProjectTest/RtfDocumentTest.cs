// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;
using Tawala.Projects.Links;
using Tawala.Projects.Processes;
using Tawala.RtfSupport;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
    [TestFixture]
    public class RtfDocumentTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();
        }

        #endregion

        private const string NEWLINE = "\r\n";

        private readonly string rtfPlainTextString =
            RtfDocument.RtfStringPrefix +
            @"Plain Text\par " +
            RtfDocument.RtfStringPostfix;

        private IForm form;

        private readonly string rtfPlainTextTwoParagraphs =
            RtfDocument.RtfStringPrefix +
            @"Plain Text\par This is line two\par " +
            RtfDocument.RtfStringPostfix;

        private readonly string rtfBoldText =
            RtfDocument.RtfStringPrefix +
            @"\b Bold text\par " +
            RtfDocument.RtfStringPostfix;

        private readonly string rtfItalicText =
            RtfDocument.RtfStringPrefix +
            @"\i Italic text\par " +
            RtfDocument.RtfStringPostfix;

        private readonly string rtfUnderlineText =
            RtfDocument.RtfStringPrefix +
            @"\ul Underline text\par " +
            RtfDocument.RtfStringPostfix;

        private readonly string rtfMixedFormatsText =
            RtfDocument.RtfStringPrefix +
            @"Plain text, \plain\f0\fs20\b now bold\plain\f0\fs20 , \plain\f0\fs20\i now italic, \plain\f0\fs20\b\i now combined\plain\f0\fs20 , back to plain.\par " +
            RtfDocument.RtfStringPostfix;

        private string rtfDefaultFontFaceText =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
            @"\fs20 Default Font\par }";

        private string rtfNonDefaultFontFaceString =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\fmodern\fcharset0\fprq1 Courier New;}" + "\r\n" +
            @"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f1" +
            @"\fs20 Non-default Font\par }";

        private string rtfNonDefaultFontSizeString =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\fmodern\fcharset0\fprq1 Courier New;}" + "\r\n" +
            @"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
            @"\fs28 Non-default Size\par }";

        private string rtfFontColorString =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red255\green0\blue0;\red0\green255\blue0;\re" +
            @"d0\green0\blue255;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
            @"\fs20\cf3 Red \plain\f0\fs20\cf4 Green \plain\f0\fs20\cf5 Blue\par }";

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

        private string rtfTableCellIndentString =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
            @"d0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpa" +
            @"ddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800\pard\intbl\plain\f0\fs20 No" +
            @"t Indented\par\pard\intbl\li720 Indented\cell\row\pard\itap0\par }";

        private string rtfTableCellAlignCenterString =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
            @"d0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpa" +
            @"ddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800\pard\intbl\qc\plain\f0\fs20" +
            @" Center\cell\row\pard\itap0\par }";

        private string rtfTableCellAlignTwoParagraphsString =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
            @"d0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpa" +
            @"ddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800\pard\intbl\plain\f0\fs20 De" +
            @"fault Alignment\par\pard\intbl\qr Right Alignment\cell\row\pard\itap0\par }";

        private string rtfTableRowAlignRightCenterLeftString =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
            @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
            @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd" +
            @"\irow0\irowband0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36" +
            @"\trpaddr36\trpaddfl3\trpaddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth3600\cellx3600" +
            @"\clvertalt\clftsWidth3\clwWidth3600\cellx7200\clvertalt\clftsWidth3\clwWidth3600\cellx10800\pard" +
            @"\intbl\qr\plain\f0\fs20" +
            @" Right" +
            @"\cell\pard\intbl\qc" +
            @" Center" +
            @"\cell\pard\intbl" +
            @" Left" +
            @"\cell\row\pard\itap0\par }";

        private static readonly string rtfFieldStringPrefix =
            RtfDocument.RtfStringPrefix +
            @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219";

        private static string rtfFieldDataStringPrefix = @"\txfielddataval{0}";

        private string rtfFieldString =
            @"\txfielddata 540046002400510031003a0061000000}" +
            @"<<Q1:a>>{" +
            @"\*\txfieldend}\par }";

        private static Variable createProcessWithSetStatement(string variableName)
        {
            Process process = Project.Current.AddProcess();
            var setStatement = new SetStatement();
            var variable = new Variable(variableName);
            setStatement.Variable = variable;
            process.Lines.Add(new SetLine(setStatement));

            return variable;
        }

        [Test]
        public void BlueBlackRedToXml()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0\blue255;\red255\green0\blue0;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
                @"\fs20\cf3 Blue \plain\f0\fs20 Black \plain\f0\fs20\cf4 Red\par }";

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"200\" color=\"0000FF\">" +
                "Blue " +
                "</font>" +
                XmlConstants.FullBeginFont +
                "Black " +
                XmlConstants.EndFont +
                "<font face=\"Arial\" size=\"200\" color=\"FF0000\">" +
                "Red" +
                "</font>" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void BoldTextToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfBoldText;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];
            var fontAttributes = (FontAttributes)paragraph.Contents[0];

            foreach (IParagraphComponent component in fontAttributes.Contents)
            {
                Assert.AreEqual("Bold text", component.Text);
            }
        }

        [Test]
        public void BoldTextToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfBoldText;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "<b>Bold text</b>" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void ColorTableFromFromXml()
        {
            string xmlString =
                "<document name=\"Document 1\">" +
                "<xmlData>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"200\" color=\"FF0000\">" +
                "Red " +
                "</font>" +
                "<font face=\"Arial\" size=\"200\" color=\"00FF00\">" +
                "Green " +
                "</font>" +
                "<font face=\"Arial\" size=\"200\" color=\"0000FF\">" +
                "Blue " +
                "</font>" +
                "Black" +
                "</paragraph>" +
                "</xmlData>" +
                "</document>";

            IXmlElement element = new XmlElement(xmlString);
            var document = new RtfDocument(element);

            Assert.AreEqual(6, document.ColorTable.Count);
            Assert.AreEqual(0x000000, document.ColorTable[0].ToRgbColor());
            Assert.AreEqual(0xffffff, document.ColorTable[1].ToRgbColor());
            Assert.AreEqual(0x000001, document.ColorTable[2].ToRgbColor());
            Assert.AreEqual(0xff0000, document.ColorTable[3].ToRgbColor());
            Assert.AreEqual(0x00ff00, document.ColorTable[4].ToRgbColor());
            Assert.AreEqual(0x0000ff, document.ColorTable[5].ToRgbColor());
        }

        [Test]
        public void Construct()
        {
            Document rtfDocument = new RtfDocument("Document 1");
            Assert.AreEqual("Document 1", rtfDocument.Name);
        }

        [Test]
        public void ConstructFromHtmlWithField()
        {
            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<rawHtmlData>\r\n" +
                "<![CDATA[" +
                Document.RawHtmlPrefix +
                "<p><span style=\"font-size:10pt;\">Document 1 has one field: &lt;&lt;Q1:a&gt;&gt;</span></p>" +
                Document.RawHtmlPostfix +
                "]]>\r\n" +
                "</rawHtmlData>\r\n" +
                "</document>";

            IXmlElement element = new XmlElement(xmlString);
            var document = new RtfDocument(element);

            string expectedRtf =
                RtfConstants.BasicRtfPrologue +
                @"\pard " +
                @"{\f0\fs20\cf1 Document 1 has one field: <<Q1:a>>}\par }";

            Assert.AreEqual("Document 1", document.Name);
            Assert.AreEqual(expectedRtf, document.Rtf);
            Assert.AreEqual(1, document.GetFields().Count);
            Assert.AreEqual("", document.Text);
        }

        [Test]
        public void ConstructFromOldHtmlData()
        {
            // old (pre-HTML) format
            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<rawHtmlData>\r\n" +
                "<![CDATA[" +
                Document.RawHtmlPrefix +
                "<p><span style=\"font-size:10pt;\">This is some HTML document text.</span></p>" +
                Document.RawHtmlPostfix +
                "]]>\r\n" +
                "</rawHtmlData>\r\n" +
                "</document>";

            IXmlElement element = new XmlElement(xmlString);
            var document = new RtfDocument(element);

            string expectedRtf =
                RtfConstants.BasicRtfPrologue +
                @"\pard " +
                @"{\f0\fs20\cf1 This is some HTML document text.}\par }";

            Assert.AreEqual("Document 1", document.Name);
            Assert.AreEqual(expectedRtf, document.Rtf);
            Assert.AreEqual("", document.Text);
        }

        [Test]
        public void ConstructFromXml()
        {
            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "Here is some RTF document text." +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>";

            IXmlElement element = new XmlElement(xmlString);
            var document = new RtfDocument(element);

            Assert.AreEqual("Document 1", document.Name);
            Assert.AreEqual(2, document.FontTable.Count);

            string expectedRtf =
                RtfConstants.BasicRtfThemePrologue +
                @"\pard " +
                @"Here is some RTF document text.\par }";

            Assert.AreEqual(expectedRtf, document.Rtf);
            Assert.AreEqual("", document.Text);
        }

        [Test]
        public void ConstructFromXmlRawText()
        {
            // old (pre-HTML) format
            string xmlString =
                "<document name=\"Document 1\">" +
                "Here is some document text." +
                "</document>";

            IXmlElement element = new XmlElement(xmlString);
            var document = new RtfDocument(element);

            string expectedRtf =
                RtfConstants.BasicRtfPrologue +
                @"\pard " +
                @"{\f0\fs20\cf1 Here is some document text.}\par }";

            Assert.AreEqual("Document 1", document.Name);
            Assert.AreEqual(expectedRtf, document.Rtf);
            Assert.AreEqual("", document.Text);
        }

        [Test]
        public void ConstructFromXmlRawTextWithEscapes()
        {
            // old (pre-HTML) format
            string xmlString =
                "<document name=\"Document 1\">" +
                "Contents in &lt;brackets&gt;; &amp; with an ampersand; in &quot;double quotes&quot;; in 'single quotes'." +
                "</document>";

            IXmlElement element = new XmlElement(xmlString);
            var document = new RtfDocument(element);

            string expectedRtf =
                RtfConstants.BasicRtfPrologue +
                "\\pard " +
                "{\\f0\\fs20\\cf1 Contents in <brackets>; & with an ampersand; in \"double quotes\"; in 'single quotes'.}\\par }";

            Assert.AreEqual(expectedRtf, document.Rtf);
            Assert.AreEqual(0, document.GetFields().Count);
        }

        [Test]
        public void ConstructFromXmlRawTextWithField()
        {
            // old (pre-HTML) format
            string xmlString =
                "<document name=\"Document 1\">" +
                "Document text with a field: <field name=\"Q1:a\"/>" +
                "</document>";

            IXmlElement element = new XmlElement(xmlString);
            var document = new RtfDocument(element);

            string expectedRtf =
                RtfConstants.BasicRtfPrologue +
                @"\pard " +
                @"{\f0\fs20\cf1 Document text with a field: <<Q1:a>>}\par }";

            Assert.AreEqual(expectedRtf, document.Rtf);
            Assert.AreEqual(1, document.GetFields().Count);
        }

        [Test]
        public void ConstructUnknownFieldFromXml()
        {
            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">" +
                "<field name=\"Unknown Field\"/>" +
                "</font>" +
                "</paragraph>\r\n" +
                "</xmlData>\r\n" +
                "</document>";

            IXmlElement element = new XmlElement(xmlString);
            var document = new RtfDocument(element);

            string expectedRtf =
                RtfConstants.BasicRtfThemePrologue +
                @"\pard " +
                @"{\f0\fs20\cf1 " +
                @"{\*\txfieldstart\txfieldtype0\txfieldflags219" +
                string.Format(@"\txfielddataval{0}", Util.NextUniqueIDValue - 1) +
                @"\txfielddata 54004600240055006e006b006e006f0077006e0020004600690065006c0064000000}" +
                @"<<Unknown Field>>" +
                @"{\*\txfieldend}" +
                "}" +
                @"\par }";

            Console.WriteLine(document.Rtf);

            Assert.AreEqual("Document 1", document.Name);
            Console.WriteLine(document.Rtf);
            Assert.AreEqual(expectedRtf, document.Rtf);
            Assert.AreEqual(1, document.GetFields().Count);
            Assert.IsInstanceOfType(typeof(Field), document.GetFields()[0]);
        }

        [Test]
        public void DefaultFontFaceToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfDefaultFontFaceText;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];
            var fontAttributes = (FontAttributes)paragraph.Contents[0];

            foreach (IParagraphComponent component in fontAttributes.Contents)
            {
                Assert.AreEqual("Default Font", component.Text);
            }
        }

        [Test]
        public void EmptyBoldTextToXml()
        {
            string rtfEmptyBoldText =
                RtfDocument.RtfStringPrefix +
                @"\b \par " +
                RtfDocument.RtfStringPostfix;

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfEmptyBoldText;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void Fields()
        {
            string rtfString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                               @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                               @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                               @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                               @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                               @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                               @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
                               @"\fs20 Here is a Form field <<Q1:a>> and and a variable <<Q2:a>>.\par }";

            var doc = new RtfDocument("Document Name");
            doc.Rtf = rtfString;

            FieldList docFields = doc.GetFields();

            Assert.AreEqual(2, docFields.Count);
            Assert.AreEqual("Q1:a", docFields[0].FieldName);
            Assert.AreEqual("Q2:a", docFields[1].FieldName);
        }

        [Test]
        public void FieldsFromHtml()
        {
            string htmlContent =
                Document.RawHtmlPrefix +
                "<p><span style=\"font-size:10pt;\">Here is a field &lt;&lt;Q3:b&gt;&gt; in HTML.</span></p>" +
                Document.RawHtmlPostfix;

            var doc = new RtfDocument("Document Name");
            doc.Text = htmlContent;

            FieldList docFields = doc.GetFields();

            Assert.AreEqual(1, docFields.Count);
            Assert.AreEqual("Q3:b", docFields[0].FieldName);
        }

        [Test]
        public void FieldToObject()
        {
            Util.NewTestProject();
            var var = new Variable("variable");

            string rtfString =
                rtfFieldStringPrefix +
                string.Format(rtfFieldDataStringPrefix, var.Id) +
                rtfFieldString;

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];
            Assert.IsInstanceOfType(typeof(DocumentField), paragraph.Contents[0]);
            Assert.IsNotNull(((DocumentField)paragraph.Contents[0]).Field);
            Assert.AreSame(var, ((DocumentField)paragraph.Contents[0]).Field);
        }

        [Test]
        public void FieldToRtf()
        {
            Variable variable = createProcessWithSetStatement("FirstName");

            string rtfString =
                rtfFieldStringPrefix +
                string.Format(rtfFieldDataStringPrefix, variable.Id) +
                rtfFieldString;

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            string expectedString =
                RtfConstants.BasicRtfPrologue +
                @"\pard " +
                @"{\*\txfieldstart\txfieldtype0\txfieldflags219" +
                string.Format(@"\txfielddataval{0}", variable.Id) +
                @"\txfielddata 540046002400460069007200730074004e0061006d0065000000}" +
                @"<<FirstName>>{" +
                @"\*\txfieldend}\par }";

            Assert.AreEqual(expectedString, rtfDocument.ToRtf());
        }

        [Test]
        public void FieldToXml()
        {
            Variable variable = createProcessWithSetStatement("FirstName");

            string rtfString =
                rtfFieldStringPrefix +
                string.Format(rtfFieldDataStringPrefix, variable.Id) +
                rtfFieldString;

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<field name=\"FirstName\"/>" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void FontColorToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfFontColorString;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];

            Assert.AreEqual(3, paragraph.Contents.Count);
            Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[0]);

            var redAttributes = (FontAttributes)paragraph.Contents[0];
            Assert.AreEqual("Red ", redAttributes.Contents.Text);
            Assert.AreEqual(255 << 16, redAttributes.Color);

            var greenAttributes = (FontAttributes)paragraph.Contents[1];
            Assert.AreEqual("Green ", greenAttributes.Contents.Text);
            Assert.AreEqual(255 << 8, greenAttributes.Color);

            var blueAttributes = (FontAttributes)paragraph.Contents[2];
            Assert.AreEqual("Blue", blueAttributes.Contents.Text);
            Assert.AreEqual(255, blueAttributes.Color);
        }

        [Test]
        public void FontColorToRtf()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfFontColorString;

            string expectedString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + NEWLINE +
                @"{\fonttbl" + NEWLINE +
                @"{\f0\fswiss Arial;}" + NEWLINE +
                @"{\f1\froman Symbol;}" + NEWLINE +
                @"}" + NEWLINE +
                @"\fs20" +
                @"{\colortbl;" + NEWLINE +
                @"\red0\green0\blue0;" + NEWLINE +
                @"\red255\green255\blue255;" + NEWLINE +
                @"\red255\green0\blue0;" + NEWLINE +
                @"\red0\green255\blue0;" + NEWLINE +
                @"\red0\green0\blue255;" + NEWLINE +
                @"}" + NEWLINE +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"{\f0\fs20\cf3 Red }{\f0\fs20\cf4 Green }{\f0\fs20\cf5 Blue}\par }";

            Assert.AreEqual(expectedString, rtfDocument.ToRtf());
        }

        [Test]
        public void FontColorToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfFontColorString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"200\" color=\"FF0000\">" +
                "Red " +
                "</font>" +
                "<font face=\"Arial\" size=\"200\" color=\"00FF00\">" +
                "Green " +
                "</font>" +
                "<font face=\"Arial\" size=\"200\" color=\"0000FF\">" +
                "Blue" +
                "</font>" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void IndentandTabsToXml()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\li720\tx" +
                @"720\tx2880\plain\f0\fs20 Text1\tab Text2\par }";

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"720\" align=\"left\">" +
                "<tabPositions>" +
                "<tabStop position=\"720\"/>" +
                "<tabStop position=\"2880\"/>" +
                "</tabPositions>" +
                XmlConstants.FullBeginFont +
                "Text1" +
                XmlConstants.EndFont +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Text2" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void InvitationFieldRtfGeneratesInvitationFieldObject()
        {
            var invitationField = new InvitationField();

            string encryptedInvitationData = RtfUtility.EncodeHexString(@"IF$" + invitationField.Id);

            string rtfString =
                rtfFieldStringPrefix +
                @"\txfielddataval" + invitationField.Id +
                @"\txfielddata " + encryptedInvitationData + "}" +
                @"click here{" +
                @"\*\txfieldend}\par }";

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];
            Assert.IsInstanceOfType(typeof(DocumentIdedInvitationField), paragraph.Contents[0]);

            var documentInvitationField = paragraph.Contents[0] as DocumentIdedInvitationField;
            Assert.AreEqual(invitationField, documentInvitationField.Invitation);
        }

        [Test]
        public void ItalicTextToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfItalicText;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];
            var fontAttributes = (FontAttributes)paragraph.Contents[0];

            foreach (IParagraphComponent component in fontAttributes.Contents)
            {
                Assert.AreEqual("Italic text", component.Text);
            }
        }

        [Test]
        public void ItalicTextToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfItalicText;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "<i>Italic text</i>" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void LeadingTabToXml()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\tx720\tx2880\tx5760\plain\f0\fs20\tab" +
                @" Text1" +
                @"\tab" +
                @" Text2" +
                @"\tab" +
                @" Text3" +
                @"\par }";

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<tabPositions>" +
                "<tabStop position=\"720\"/>" +
                "<tabStop position=\"2880\"/>" +
                "<tabStop position=\"5760\"/>" +
                "</tabPositions>" +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Text1" +
                XmlConstants.EndFont +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Text2" +
                XmlConstants.EndFont +
                "<tab/>" +
                XmlConstants.FullBeginFont +
                "Text3" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void MixedFormatsToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfMixedFormatsText;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];
            Assert.AreEqual(6, paragraph.Contents.Count);
            Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[0]);
            Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[1]);
            Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[2]);
            Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[3]);
            Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[4]);
            Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[5]);

            var types = new[]
                        {
                            typeof(BoldText), typeof(ItalicText), typeof(DocumentText)
                        };

            var fontAttributes = (FontAttributes)paragraph.Contents[4];
            int i = 0;

            foreach (IParagraphComponent component in fontAttributes.Contents.RecursiveEnumerator)
            {
                Assert.AreEqual(types[i++], component.GetType());
            }
        }

        [Test]
        public void MixedFormatsToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfMixedFormatsText;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Plain text, " +
                XmlConstants.EndFont +
                XmlConstants.FullBeginFont +
                "<b>now bold</b>" +
                XmlConstants.EndFont +
                XmlConstants.FullBeginFont +
                ", " +
                XmlConstants.EndFont +
                XmlConstants.FullBeginFont +
                "<i>now italic, </i>" +
                XmlConstants.EndFont +
                XmlConstants.FullBeginFont +
                "<b><i>now combined</i></b>" +
                XmlConstants.EndFont +
                XmlConstants.FullBeginFont +
                ", back to plain." +
                XmlConstants.EndFont +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void NonDefaultFontFaceToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfNonDefaultFontFaceString;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];

            Assert.AreEqual(1, paragraph.Contents.Count);
            Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[0]);

            var attributes = (FontAttributes)paragraph.Contents[0];
            Assert.AreEqual("Non-default Font", attributes.Contents.Text);

            Assert.AreEqual("Courier New", attributes.Face);
        }

        [Test]
        public void NonDefaultFontFaceToRtf()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfNonDefaultFontFaceString;

            string expectedString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + NEWLINE +
                @"{\fonttbl" + NEWLINE +
                @"{\f0\fswiss Arial;}" + NEWLINE +
                @"{\f1\fmodern Courier New;}" + NEWLINE +
                @"{\f2\froman Symbol;}" + NEWLINE +
                @"}" + NEWLINE +
                @"\fs20" +
                @"{\colortbl;" + NEWLINE +
                @"\red0\green0\blue0;" + NEWLINE +
                @"\red255\green255\blue255;" + NEWLINE +
                @"}" + NEWLINE +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"{\f1\fs20\cf1 Non-default Font}\par }";

            Assert.AreEqual(expectedString, rtfDocument.ToRtf());
        }

        [Test]
        public void NonDefaultFontFaceToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfNonDefaultFontFaceString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Courier New\" size=\"200\" color=\"000000\">" +
                "Non-default Font" +
                "</font>" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void NonDefaultFontSizeToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfNonDefaultFontSizeString;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];

            Assert.AreEqual(1, paragraph.Contents.Count);
            Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[0]);

            var attributes = (FontAttributes)paragraph.Contents[0];
            Assert.AreEqual("Non-default Size", attributes.Contents.Text);

            Assert.AreEqual(280, attributes.Size);
        }

        [Test]
        public void NonDefaultFontSizeToRtf()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfNonDefaultFontSizeString;

            string expectedString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + NEWLINE +
                @"{\fonttbl" + NEWLINE +
                @"{\f0\fswiss Arial;}" + NEWLINE +
                @"{\f1\fmodern Courier New;}" + NEWLINE +
                @"{\f2\froman Symbol;}" + NEWLINE +
                @"}" + NEWLINE +
                @"\fs20" +
                @"{\colortbl;" + NEWLINE +
                @"\red0\green0\blue0;" + NEWLINE +
                @"\red255\green255\blue255;" + NEWLINE +
                @"}" + NEWLINE +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"{\f0\fs28\cf1 Non-default Size}\par }";

            Assert.AreEqual(expectedString, rtfDocument.ToRtf());
        }

        [Test]
        public void NonDefaultFontSizeToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfNonDefaultFontSizeString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"280\" color=\"000000\">" +
                "Non-default Size" +
                "</font>" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void PlainTextToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfPlainTextString;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];
            var fontAttributes = (FontAttributes)paragraph.Contents[0];

            foreach (IParagraphComponent component in fontAttributes.Contents)
            {
                Assert.AreEqual("Plain Text", component.Text);
            }
        }

        [Test]
        public void PlainTextToRtf()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfPlainTextString;

            string expectedString =
                RtfConstants.BasicRtfPrologue +
                @"\pard " +
                @"{\f0\fs20\cf1 Plain Text}\par }";

            Assert.AreEqual(expectedString, rtfDocument.ToRtf());
        }

        [Test]
        public void PlainTextToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfPlainTextString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Plain Text" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void PlainTextTwoParagraphsToRtf()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfPlainTextTwoParagraphs;

            string expectedString =
                RtfConstants.BasicRtfPrologue +
                @"\pard " +
                @"{\f0\fs20\cf1 Plain Text}\par \pard " +
                @"{\f0\fs20\cf1 This is line two}\par }";

            Assert.AreEqual(expectedString, rtfDocument.ToRtf());
        }

        [Test]
        public void PlainTextTwoParagraphsToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfPlainTextTwoParagraphs;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Plain Text" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "This is line two" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void TableCellAlignCenterToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableCellAlignCenterString;

            Assert.AreEqual(2, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Table), rtfDocument.Contents[0]);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[1]);

            var table = (Table)rtfDocument.Contents[0];

            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual(1, table.Rows[0].Cells.Count);
            Assert.AreEqual(1, table.Rows[0].Cells[0].Divisions.Count);

            Assert.AreEqual("Center", table.Rows[0].Cells[0].Divisions[0].Contents[0].Text);
            Assert.AreEqual("center", table.Rows[0].Cells[0].Divisions[0].Align);
        }

        [Test]
        public void TableCellAlignCenterToRtf()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableCellAlignCenterString;

            string expectedString =
                RtfConstants.BasicRtfPrologue +
                @"\trowd\trleft0\clftsWidth3\clwWidth10800\cellx10800" +
                @"\pard\intbl \qc {\f0\fs20\cf1 Center}\cell \row" +
                @"\pard " +
                @"\par }";

            Assert.AreEqual(expectedString, rtfDocument.ToRtf());
        }

        [Test]
        public void TableCellAlignCenterToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableCellAlignCenterString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<table indent=\"0\">" +
                "<row>" +
                "<cell width=\"10800\">" +
                "<division indent=\"0\" align=\"center\">" +
                XmlConstants.FullBeginFont +
                "Center" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "</row>" +
                "</table>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void TableCellAlignTwoParagraphsToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableCellAlignTwoParagraphsString;

            Assert.AreEqual(2, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Table), rtfDocument.Contents[0]);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[1]);

            var table = (Table)rtfDocument.Contents[0];

            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual(1, table.Rows[0].Cells.Count);
            Assert.AreEqual(2, table.Rows[0].Cells[0].Divisions.Count);

            Assert.AreEqual("Default Alignment", table.Rows[0].Cells[0].Divisions[0].Contents[0].Text);
            Assert.AreEqual("Right Alignment", table.Rows[0].Cells[0].Divisions[1].Contents[0].Text);

            Assert.AreEqual("left", table.Rows[0].Cells[0].Divisions[0].Align);
            Assert.AreEqual("right", table.Rows[0].Cells[0].Divisions[1].Align);
        }

        [Test]
        public void TableCellAlignTwoParagraphsToRtf()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableCellAlignTwoParagraphsString;

            string expectedString =
                RtfConstants.BasicRtfPrologue +
                @"\trowd\trleft0\clftsWidth3\clwWidth10800\cellx10800" +
                @"\pard\intbl \ql {\f0\fs20\cf1 Default Alignment}\par \qr {\f0\fs20\cf1 Right Alignment}\cell \row" +
                @"\pard " +
                @"\par }";

            Assert.AreEqual(expectedString, rtfDocument.ToRtf());
        }

        [Test]
        public void TableCellAlignTwoParagraphsToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableCellAlignTwoParagraphsString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<table indent=\"0\">" +
                "<row>" +
                "<cell width=\"10800\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Default Alignment" +
                XmlConstants.EndFont +
                "</division>" +
                "<division indent=\"0\" align=\"right\">" +
                XmlConstants.FullBeginFont +
                "Right Alignment" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "</row>" +
                "</table>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void TableCellIndentToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableCellIndentString;

            Assert.AreEqual(2, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Table), rtfDocument.Contents[0]);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[1]);

            var table = (Table)rtfDocument.Contents[0];

            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual(1, table.Rows[0].Cells.Count);
            Assert.AreEqual(2, table.Rows[0].Cells[0].Divisions.Count);

            Assert.AreEqual("Not Indented", table.Rows[0].Cells[0].Divisions[0].Contents[0].Text);
            Assert.AreEqual("Indented", table.Rows[0].Cells[0].Divisions[1].Contents[0].Text);

            Assert.AreEqual(0, table.Rows[0].Cells[0].Divisions[0].Indent);
            Assert.AreEqual(720, table.Rows[0].Cells[0].Divisions[1].Indent);
        }

        [Test]
        public void TableCellIndentToRtf()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableCellIndentString;

            string expectedString =
                RtfConstants.BasicRtfPrologue +
                @"\trowd\trleft0\clftsWidth3\clwWidth10800\cellx10800" +
                @"\pard\intbl \ql {\f0\fs20\cf1 Not Indented}\par \ql \li720 {\f0\fs20\cf1 Indented}\cell \row" +
                @"\pard " +
                @"\par }";

            Assert.AreEqual(expectedString, rtfDocument.ToRtf());
        }

        [Test]
        public void TableCellIndentToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableCellIndentString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<table indent=\"0\">" +
                "<row>" +
                "<cell width=\"10800\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Not Indented" +
                XmlConstants.EndFont +
                "</division>" +
                "<division indent=\"720\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Indented" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "</row>" +
                "</table>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void TableRowAlignRightCenterLeftToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableRowAlignRightCenterLeftString;

            Assert.AreEqual(2, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Table), rtfDocument.Contents[0]);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[1]);

            var table = (Table)rtfDocument.Contents[0];

            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual(3, table.Rows[0].Cells.Count);
            Assert.AreEqual(1, table.Rows[0].Cells[0].Divisions.Count);
            Assert.AreEqual(1, table.Rows[0].Cells[1].Divisions.Count);
            Assert.AreEqual(1, table.Rows[0].Cells[2].Divisions.Count);

            Assert.AreEqual("Right", table.Rows[0].Cells[0].Divisions[0].Contents[0].Text);
            Assert.AreEqual("right", table.Rows[0].Cells[0].Divisions[0].Align);
            Assert.AreEqual("Center", table.Rows[0].Cells[1].Divisions[0].Contents[0].Text);
            Assert.AreEqual("center", table.Rows[0].Cells[1].Divisions[0].Align);
            Assert.AreEqual("Left", table.Rows[0].Cells[2].Divisions[0].Contents[0].Text);
            Assert.AreEqual("left", table.Rows[0].Cells[2].Divisions[0].Align);
        }

        [Test]
        public void TableRowAlignRightCenterLeftToRtf()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableRowAlignRightCenterLeftString;

            string expectedString =
                RtfConstants.BasicRtfPrologue +
                @"\trowd\trleft0" +
                @"\clftsWidth3\clwWidth3600\cellx3600" +
                @"\clftsWidth3\clwWidth3600\cellx7200" +
                @"\clftsWidth3\clwWidth3600\cellx10800" +
                @"\pard\intbl \qr {\f0\fs20\cf1 Right}\cell " +
                @"\qc {\f0\fs20\cf1 Center}\cell " +
                @"\ql {\f0\fs20\cf1 Left}\cell " +
                @"\row" +
                @"\pard " +
                @"\par }";

            Assert.AreEqual(expectedString, rtfDocument.ToRtf());
        }

        [Test]
        public void TableRowAlignRightCenterLeftToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableRowAlignRightCenterLeftString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<table indent=\"0\">" +
                "<row>" +
                "<cell width=\"3600\">" +
                "<division indent=\"0\" align=\"right\">" +
                XmlConstants.FullBeginFont +
                "Right" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "<cell width=\"3600\">" +
                "<division indent=\"0\" align=\"center\">" +
                XmlConstants.FullBeginFont +
                "Center" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "<cell width=\"3600\">" +
                "<division indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Left" +
                XmlConstants.EndFont +
                "</division>" +
                "</cell>" +
                "</row>" +
                "</table>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void TableToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableString;

            Assert.AreEqual(3, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);
            Assert.IsInstanceOfType(typeof(Table), rtfDocument.Contents[1]);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[2]);

            var table = (Table)rtfDocument.Contents[1];

            Assert.AreEqual(2, table.Rows.Count);
            Assert.AreEqual(2, table.Rows[0].Cells.Count);
            Assert.AreEqual(1, table.Rows[0].Cells[0].Divisions.Count);
            Assert.AreEqual(1, table.Rows[0].Cells[1].Divisions.Count);
            Assert.AreEqual(2, table.Rows[1].Cells.Count);
            Assert.AreEqual(1, table.Rows[1].Cells[0].Divisions.Count);
            Assert.AreEqual(1, table.Rows[1].Cells[1].Divisions.Count);

            Assert.AreEqual("Cell 1", table.Rows[0].Cells[0].Divisions[0].Contents[0].Text);
            Assert.AreEqual("Cell 2", table.Rows[0].Cells[1].Divisions[0].Contents[0].Text);
            Assert.AreEqual("Cell 3", table.Rows[1].Cells[0].Divisions[0].Contents[0].Text);
            Assert.AreEqual("Cell 4", table.Rows[1].Cells[1].Divisions[0].Contents[0].Text);
        }

        [Test]
        public void TableToRtf()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableString;

            string expectedString =
                RtfConstants.BasicRtfPrologue +
                @"\pard " +
                @"\par " +
                @"\trowd\trleft0\clftsWidth3\clwWidth5400\cellx5400\clftsWidth3\clwWidth5400\cellx10800" +
                @"\pard\intbl \ql {\f0\fs20\cf1 Cell 1}\cell \ql {\f0\fs20\cf1 Cell 2}\cell \row" +
                @"\trowd\trleft0\clftsWidth3\clwWidth5400\cellx5400\clftsWidth3\clwWidth5400\cellx10800" +
                @"\pard\intbl \ql {\f0\fs20\cf1 Cell 3}\cell \ql {\f0\fs20\cf1 Cell 4}\cell \row" +
                @"\pard " +
                @"\par }";

            Assert.AreEqual(expectedString, rtfDocument.ToRtf());
        }

        [Test]
        public void TableToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfTableString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
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
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void TabWithMixedFontFacesToXml()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\fmodern\fcharset0\fprq1 Courier New;}" + "\r\n" +
                @"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\plain\f0\fs20" +
                @" Arial" +
                @"\tab\plain\f1\fs20" +
                @" Courier New" +
                @"\par }";

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "Arial" +
                XmlConstants.EndFont +
                "<tab/>" +
                "<font face=\"Courier New\" size=\"200\" color=\"000000\">" +
                "Courier New" +
                "</font>" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void TabWithNonDefaultFontFaceToXml()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\fmodern\fcharset0\fprq1 Courier New;}" + "\r\n" +
                @"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\plain\f1\fs20" +
                @" Courier" +
                @"\tab" +
                @" New" +
                @"\par }";

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Courier New\" size=\"200\" color=\"000000\">" +
                "Courier" +
                "</font>" +
                "<tab/>" +
                "<font face=\"Courier New\" size=\"200\" color=\"000000\">" +
                "New" +
                "</font>" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void TextAndFieldWithTrailingTabToXml()
        {
            Variable variable = createProcessWithSetStatement("First Name");

            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\tx720\tx2880\tx5760\plain\f0\fs20" +
                @"Text with field and tab " +
                @"\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags219" +
                string.Format(@"\txfielddataval{0}", variable.Id) +
                @"\txfielddata 540046002400460069007200730074004e0061006d0065000000}" +
                @"<<FirstName>>{" +
                @"\*\txfieldend}\plain\f0\fs20\tab\par }";

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<tabPositions>" +
                "<tabStop position=\"720\"/>" +
                "<tabStop position=\"2880\"/>" +
                "<tabStop position=\"5760\"/>" +
                "</tabPositions>" +
                XmlConstants.FullBeginFont +
                "Text with field and tab " +
                XmlConstants.EndFont +
                XmlConstants.FullBeginFont +
                "<field name=\"First Name\"/>" +
                XmlConstants.EndFont +
                "<tab/>" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void TextWithTrailingTabToXml()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\tx720\tx2880\tx5760\plain\f0\fs20" +
                @" Text with trailing tab" +
                @"\tab\par }";

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<tabPositions>" +
                "<tabStop position=\"720\"/>" +
                "<tabStop position=\"2880\"/>" +
                "<tabStop position=\"5760\"/>" +
                "</tabPositions>" +
                XmlConstants.FullBeginFont +
                "Text with trailing tab" +
                XmlConstants.EndFont +
                "<tab/>" +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void UnderlineTextToObject()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfUnderlineText;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];
            var fontAttributes = (FontAttributes)paragraph.Contents[0];

            foreach (IParagraphComponent component in fontAttributes.Contents)
            {
                Assert.AreEqual("Underline text", component.Text);
            }
        }

        [Test]
        public void UnderlineTextToXml()
        {
            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfUnderlineText;

            string xmlString =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "<u>Underline text</u>" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "\r\n</xmlData>\r\n" +
                "</document>\r\n";

            Assert.AreEqual(xmlString, rtfDocument.ToXml());
        }

        [Test]
        public void UnmappedFieldToObject()
        {
            Util.NewTestProject();

            string rtfString =
                rtfFieldStringPrefix +
                string.Format(rtfFieldDataStringPrefix, 3141) +
                rtfFieldString;

            var rtfDocument = new RtfDocument("Document 1");
            rtfDocument.Rtf = rtfString;

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            var paragraph = (Paragraph)rtfDocument.Contents[0];
            Assert.IsNotNull(((DocumentField)paragraph.Contents[0]).Field);
            Assert.AreSame(PaletteField.NULL, ((DocumentField)paragraph.Contents[0]).Field);
        }
    }
}