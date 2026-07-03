// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.ProcessStatements
{
    /// <summary>
    /// Acceptance tests for story 1921 (Fields in Subject box of SEND statement).
    /// </summary>
    [TestFixture]
    public class FieldsInSubjectBoxTest1921
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();

            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);
            blank = fibItem.BlankList[0];

            process = Project.Current.AddProcess();
        }

        #endregion

        private IForm form;
        private Blank blank;
        private Process process;

        [Test]
        public void FieldInSubjectLineGeneratesCorrectXml()
        {
            var sendStatement = new SendStatement();
            sendStatement.AddressTo = new FieldOrLiteral("jdf@tawala.com", FieldOrLiteral.StringType.literal);
            sendStatement.SendBody = new SendDocumentBody(new Document("Document 1"));

            sendStatement.Subject = "<<Form 1:Q1:a>>";

            string expectedXml =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<subject>" +
                "<field name=\"Form 1:Q1:a\"/>" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            Assert.AreEqual(expectedXml, sendStatement.ToXml());
        }

        [Test]
        public void TextAndFieldInSubjectGeneratesCorrectXml()
        {
            var sendStatement = new SendStatement();
            sendStatement.AddressTo = new FieldOrLiteral("jdf@tawala.com", FieldOrLiteral.StringType.literal);
            sendStatement.SendBody = new SendDocumentBody(new Document("Document 1"));

            sendStatement.Subject = "Prefix Text <<Form 1:Q1:a>> Suffix Text";

            string expectedXml =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<subject>" +
                "Prefix Text " +
                "<field name=\"Form 1:Q1:a\"/>" +
                " Suffix Text" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            Assert.AreEqual(expectedXml, sendStatement.ToXml());
        }

        [Test]
        public void TextAndFieldInXmlGeneratesCorrectXml()
        {
            string xmlString =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<subject>" +
                "Prefix Text " +
                "<field name=\"Form 1:Q1:a\"/>" +
                " Suffix Text" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            var sendStatement = new SendStatement(new XmlElement(xmlString), process);

            Assert.AreEqual(xmlString, sendStatement.ToXml());
        }

        [Test]
        public void TextInXmlGeneratesCorrectXml()
        {
            string xmlString =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<subject>" +
                "Subject Text Only" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            var sendStatement = new SendStatement(new XmlElement(xmlString), process);

            Assert.AreEqual(xmlString, sendStatement.ToXml());
        }
    }
}