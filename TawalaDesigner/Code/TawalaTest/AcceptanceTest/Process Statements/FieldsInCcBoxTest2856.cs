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
    /// Acceptance tests for story 2856 (Fields in Subject box of SEND statement).
    /// </summary>
    [TestFixture]
    public class FieldsInCcBoxTest2856
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

            document = Project.Current.AddDocument();
        }

        #endregion

        private IForm form;
        private Blank blank;
        private Process process;
        private IDocument document;

        private SendStatement setupSendStatement()
        {
            var sendStatement = new SendStatement();
            sendStatement.AddressTo = new FieldOrLiteral("jdf@tawala.com", FieldOrLiteral.StringType.literal);
            sendStatement.SendBody = new SendDocumentBody(document);
            sendStatement.Subject = "Subject";

            return sendStatement;
        }

        [Test]
        public void FieldInCcGeneratesCorrectXml()
        {
            SendStatement sendStatement = setupSendStatement();
            sendStatement.AddressCc = new FieldOrLiteral("Form 1:Q1:a", FieldOrLiteral.StringType.field);

            string expectedXml =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<cc addressField=\"Form 1:Q1:a\"/>\r\n" +
                "<subject>Subject</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            Assert.AreEqual(expectedXml, sendStatement.ToXml());
        }

        [Test]
        public void FieldInXmlGeneratesCorrectXml()
        {
            string xmlString =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<cc addressField=\"Form 1:Q1:a\"/>\r\n" +
                "<subject>Subject</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            var sendStatement = new SendStatement(new XmlElement(xmlString), process);

            Assert.AreEqual(xmlString, sendStatement.ToXml());
        }

        [Test]
        public void TextInCcGeneratesCorrectXml()
        {
            SendStatement sendStatement = setupSendStatement();
            sendStatement.AddressCc = new FieldOrLiteral("doug@tawala.com", FieldOrLiteral.StringType.literal);

            string expectedXml =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<cc addressLiteral=\"doug@tawala.com\"/>\r\n" +
                "<subject>Subject</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            Assert.AreEqual(expectedXml, sendStatement.ToXml());
        }

        [Test]
        public void TextInXmlGeneratesCorrectXml()
        {
            string xmlString =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<cc addressLiteral=\"doug@tawala.com\"/>\r\n" +
                "<subject>Subject</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            var sendStatement = new SendStatement(new XmlElement(xmlString), process);

            Assert.AreEqual(xmlString, sendStatement.ToXml());
        }
    }
}