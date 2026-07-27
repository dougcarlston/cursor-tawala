// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Documents;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;
using Form=System.Windows.Forms.Form;

namespace TawalaTest.AcceptanceTest.DocumentStyles
{
    [TestFixture]
    public class DesignerAppliesTextStyles2454
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = new Form();
            editor = new DocumentEditor();
            form.Controls.Add(editor);

            form.Show();
            editor.SetRTF(RtfConstants.DocumentPrologue +
                          " This is the first paragraph.\\par Second paragraph.\\par Paragraph number 3.\\par }");
        }

        [TearDown]
        public void TearDown()
        {
            form.Close();
            form = null;
        }

        #endregion

        private Form form;
        private DocumentEditor editor;

        [Test]
        public void DefaultFontPreservedOnLoad()
        {
            string xmlString =
                @"<document name=""Document 1"">" + Environment.NewLine +
                @"<xmlData>" + Environment.NewLine +
                @"<paragraph indent=""0"" align=""left"">" +
                @"<tabPositions>" +
                @"<tabStop position=""2880""/>" +
                @"</tabPositions>" +
                @"<font>" +
                @"Default Font" +
                @"</font>" +
                @"</paragraph>" + Environment.NewLine +
                @"</xmlData>" + Environment.NewLine +
                @"</document>" + Environment.NewLine;

            var document = new RtfDocument(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, document.ToXml());
        }

        [Test]
        public void ParagraphStyleNotSetByDefault()
        {
            string expectedXml =
                "<document name=\"Document 1\">" + Environment.NewLine +
                "<xmlData>" + Environment.NewLine +
                "<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions>" +
                XmlConstants.FullBeginFont +
                "This is the first paragraph." +
                XmlConstants.EndFont +
                "</paragraph>" +
                "<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions>" +
                XmlConstants.FullBeginFont +
                "Second paragraph." +
                XmlConstants.EndFont +
                "</paragraph>" +
                "<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions>" +
                XmlConstants.FullBeginFont +
                "Paragraph number 3." +
                XmlConstants.EndFont +
                "</paragraph>" + Environment.NewLine +
                "</xmlData>" + Environment.NewLine +
                "</document>" + Environment.NewLine;

            var document = Project.Current.AddDocument() as RtfDocument;
            document.Rtf = editor.GetRTF();

            Assert.AreEqual(expectedXml, document.ToXml());
        }

        [Test]
        [Ignore("Failing as expected.")]
        public void SetStyleInParagraph()
        {
            string expectedXml =
                "<document name=\"Document 1\">" + Environment.NewLine +
                "<xmlData>" + Environment.NewLine +
                "<paragraph indent=\"0\" align=\"left\" style=\"Normal\"><tabPositions><tabStop position=\"2880\"/></tabPositions>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">This is the first paragraph." +
                "</font></paragraph>" +
                "<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Second paragraph." +
                "</font></paragraph>" +
                "<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Paragraph number 3." +
                "</font></paragraph>" + Environment.NewLine +
                "</xmlData>" + Environment.NewLine +
                "</document>" + Environment.NewLine;

            var document = Project.Current.AddDocument() as RtfDocument;

            editor.Selection.Start = 10;
            editor.Selection.Length = 0;
            editor.SetStyle("Normal");
            document.Rtf = editor.GetRTF();

            Assert.AreEqual(expectedXml, document.ToXml());
        }

        [Test]
        [Ignore("Failing as expected.")]
        public void SetStyleInSelectedParagraphs()
        {
            string expectedXml =
                "<document name=\"Document 1\">" + Environment.NewLine +
                "<xmlData>" + Environment.NewLine +
                "<paragraph indent=\"0\" align=\"left\" style=\"Normal\"><tabPositions><tabStop position=\"2880\"/></tabPositions>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">This is the first paragraph." +
                "</font></paragraph>" +
                "<paragraph indent=\"0\" align=\"left\" style=\"Normal\"><tabPositions><tabStop position=\"2880\"/></tabPositions>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Second paragraph." +
                "</font></paragraph>" +
                "<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">Paragraph number 3." +
                "</font></paragraph>" + Environment.NewLine +
                "</xmlData>" + Environment.NewLine +
                "</document>" + Environment.NewLine;

            var document = Project.Current.AddDocument() as RtfDocument;

            editor.Selection.Start = 10;
            editor.Selection.Length = 25;
            editor.SetStyle("Normal");
            document.Rtf = editor.GetRTF();

            Assert.AreEqual(expectedXml, document.ToXml());
        }
    }
}