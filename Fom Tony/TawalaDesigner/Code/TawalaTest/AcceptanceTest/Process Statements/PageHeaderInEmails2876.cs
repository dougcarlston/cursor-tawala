// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.ProcessStatements
{
    /// <summary>
    /// Acceptance tests for story 2876 (Designer can send emails without Page Headers).
    /// </summary>
    [TestFixture]
    public class PageHeaderInEmails2876
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();

            process = Project.Current.AddProcess();
            document = Project.Current.AddDocument();
        }

        #endregion

        private Process process;
        private IDocument document;

        private readonly string sendStatementXml =
            "<send>\r\n" +
            "<to addressLiteral=\"jdf@tawala.com\"/>" + Environment.NewLine +
            "<subject>Subject</subject>" + Environment.NewLine +
            "<body document=\"Document 1\" reset=\"false\" showHeader=\"false\"/>" + Environment.NewLine +
            "</send>";

        [Test]
        public void ShowPageHeaderDefaultsToTrue()
        {
            var body = new SendDocumentBody(document);

            Assert.IsTrue(body.ShowPageHeader, "Show Page Header does not default to true");
        }

        [Test]
        public void ShowPageHeaderInModelGeneratesCorrectXml()
        {
            var sendStatement = new SendStatement();
            sendStatement.AddressTo = new FieldOrLiteral("jdf@tawala.com", FieldOrLiteral.StringType.literal);
            sendStatement.Subject = "Subject";

            var body = new SendDocumentBody(document);
            body.ShowPageHeader = false;
            sendStatement.SendBody = body;

            Assert.AreEqual(sendStatementXml, sendStatement.ToXml());
        }

        [Test]
        public void ShowPageHeaderInXmlGeneratesCorrectXml()
        {
            var sendStatement = new SendStatement(new XmlElement(sendStatementXml), process);

            Assert.AreEqual(sendStatementXml, sendStatement.ToXml());
        }
    }
}