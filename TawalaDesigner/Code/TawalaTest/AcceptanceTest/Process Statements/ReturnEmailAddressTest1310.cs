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
    /// Acceptance tests for story 1310 (Designer specifies return email address for SEND).
    /// </summary>
    [TestFixture]
    public class ReturnEmailAddressTest1310
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();

            setupForm();
            setupProcess();

            process.MappedFields.Fields.Add(blank);
            process.MappedFields.Map();
        }

        #endregion

        private IForm form;
        private Blank blank;
        private Process process;
        private SendStatement sendStatement;

        private void setupForm()
        {
            form = Project.Current.AddForm();

            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);
            blank = fibItem.BlankList[0];
        }

        private void setupProcess()
        {
            process = Project.Current.AddProcess();

            sendStatement = getSendStatement();
            addSendStatement(sendStatement);
        }

        private void addSendStatement(SendStatement sendStatement)
        {
            process.Lines.Add(getSendLine(sendStatement));
        }

        private SendStatement getSendStatement()
        {
            return (new SendStatement());
        }

        private static ProcessLineList getSendLine(SendStatement sendStatement)
        {
            return (new ProcessLineList(sendStatement));
        }

        [Test]
        public void FieldInFromAddressGeneratesCorrectXml()
        {
            var sendStatement = new SendStatement();
            sendStatement.AddressTo = new FieldOrLiteral("jdf@tawala.com", FieldOrLiteral.StringType.literal);
            sendStatement.AddressFrom = new Expression(blank);
            sendStatement.SendBody = new SendDocumentBody(new Document("Document 1"));
            sendStatement.Subject = "Testing From Address";

            string expectedXml =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<from addressField=\"Form 1:Q1:a\"/>\r\n" +
                "<subject>" +
                "Testing From Address" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            Assert.AreEqual(expectedXml, sendStatement.ToXml());
        }

        [Test]
        public void FieldInFromAddressXmlGeneratesCorrectXml()
        {
            string xmlString =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<from addressField=\"Form 1:Q1:a\"/>\r\n" +
                "<subject>" +
                "Testing From Address" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            var sendStatement = new SendStatement(new XmlElement(xmlString), process);

            Assert.AreEqual(xmlString, sendStatement.ToXml());
        }

        [Test]
        public void FieldInFromAliasGeneratesCorrectXml()
        {
            var sendStatement = new SendStatement();
            sendStatement.AddressTo = new FieldOrLiteral("to@address.com", FieldOrLiteral.StringType.literal);
            sendStatement.AddressFrom = new Expression("from@address.com");
            sendStatement.AliasFrom = new Expression(blank);
            sendStatement.SendBody = new SendDocumentBody(new Document("Document 1"));
            sendStatement.Subject = "Testing From Address";

            string expectedXml =
                "<send>\r\n" +
                "<to addressLiteral=\"to@address.com\"/>\r\n" +
                "<from addressLiteral=\"from@address.com\" aliasField=\"Form 1:Q1:a\"/>\r\n" +
                "<subject>" +
                "Testing From Address" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            Assert.AreEqual(expectedXml, sendStatement.ToXml());
        }

        [Test]
        public void FieldInFromAliasXmlGeneratesCorrectXml()
        {
            string xmlString =
                "<send>\r\n" +
                "<to addressLiteral=\"to@address.com\"/>\r\n" +
                "<from addressLiteral=\"from@address.com\" aliasField=\"Form 1:Q1:a\"/>\r\n" +
                "<subject>" +
                "Testing From Address" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            var sendStatement = new SendStatement(new XmlElement(xmlString), process);

            Assert.AreEqual(xmlString, sendStatement.ToXml());
        }

        [Test]
        public void FromAliasOnlyGeneratesCorrectXml()
        {
            var sendStatement = new SendStatement();
            sendStatement.AddressTo = new FieldOrLiteral("to@address.com", FieldOrLiteral.StringType.literal);
            sendStatement.AliasFrom = new Expression("Alias");
            sendStatement.SendBody = new SendDocumentBody(new Document("Document 1"));
            sendStatement.Subject = "Testing From Alias";

            string expectedXml =
                "<send>\r\n" +
                "<to addressLiteral=\"to@address.com\"/>\r\n" +
                "<from aliasLiteral=\"Alias\"/>\r\n" +
                "<subject>" +
                "Testing From Alias" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            Assert.AreEqual(expectedXml, sendStatement.ToXml());
        }

        [Test]
        public void LiteralInFromAddressXmlGeneratesCorrectXml()
        {
            string xmlString =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<from addressLiteral=\"from@address.com\"/>\r\n" +
                "<subject>" +
                "Testing From Address" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            var sendStatement = new SendStatement(new XmlElement(xmlString), process);

            Assert.AreEqual(xmlString, sendStatement.ToXml());
        }

        [Test]
        public void LiteralInFromAliasGeneratesCorrectXml()
        {
            var sendStatement = new SendStatement();
            sendStatement.AddressTo = new FieldOrLiteral("to@address.com", FieldOrLiteral.StringType.literal);
            sendStatement.AddressFrom = new Expression("from@address.com");
            sendStatement.AliasFrom = new Expression("Alias");
            sendStatement.SendBody = new SendDocumentBody(new Document("Document 1"));
            sendStatement.Subject = "Testing From Address";

            string expectedXml =
                "<send>\r\n" +
                "<to addressLiteral=\"to@address.com\"/>\r\n" +
                "<from addressLiteral=\"from@address.com\" aliasLiteral=\"Alias\"/>\r\n" +
                "<subject>" +
                "Testing From Address" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            Assert.AreEqual(expectedXml, sendStatement.ToXml());
        }

        [Test]
        public void LiteralInFromAliasXmlGeneratesCorrectXml()
        {
            string xmlString =
                "<send>\r\n" +
                "<to addressLiteral=\"to@address.com\"/>\r\n" +
                "<from addressLiteral=\"from@address.com\" aliasLiteral=\"Alias\"/>\r\n" +
                "<subject>" +
                "Testing From Address" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            var sendStatement = new SendStatement(new XmlElement(xmlString), process);

            Assert.AreEqual(xmlString, sendStatement.ToXml());
        }

        [Test]
        public void NoFromAddressGeneratesCorrectXml()
        {
            var sendStatement = new SendStatement();
            sendStatement.AddressTo = new FieldOrLiteral("jdf@tawala.com", FieldOrLiteral.StringType.literal);
            sendStatement.SendBody = new SendDocumentBody(new Document("Document 1"));
            sendStatement.Subject = "Testing From Address";

            string expectedXml =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<subject>" +
                "Testing From Address" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            Assert.AreEqual(expectedXml, sendStatement.ToXml());
        }

        [Test]
        public void TextInFromAddressGeneratesCorrectXml()
        {
            var sendStatement = new SendStatement();
            sendStatement.AddressTo = new FieldOrLiteral("jdf@tawala.com", FieldOrLiteral.StringType.literal);
            sendStatement.AddressFrom = new Expression("from@address.com");
            sendStatement.SendBody = new SendDocumentBody(new Document("Document 1"));
            sendStatement.Subject = "Testing From Address";

            string expectedXml =
                "<send>\r\n" +
                "<to addressLiteral=\"jdf@tawala.com\"/>\r\n" +
                "<from addressLiteral=\"from@address.com\"/>\r\n" +
                "<subject>" +
                "Testing From Address" +
                "</subject>\r\n" +
                "<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>\r\n" +
                "</send>";

            Assert.AreEqual(expectedXml, sendStatement.ToXml());
        }
    }
}