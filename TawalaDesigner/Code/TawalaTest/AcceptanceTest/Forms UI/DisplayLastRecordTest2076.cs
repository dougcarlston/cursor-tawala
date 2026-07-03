// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.ProcessStatements
{
    /// <summary>
    /// Acceptance tests for story 2076 (Property to display last saved record in a Form).
    /// </summary>
    [TestFixture]
    public class DisplayLastRecordTest2076
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();

            form = new Form("Form 1");
        }

        #endregion

        private IForm form;

        [Test]
        public void DataEntryOnlyFormGeneratesCorrectXml()
        {
            form.DataEntryOnly = true;

            string expectedXml =
                "<form name=\"Form 1\" startPoint=\"false\" themePath=\"default\" dataEntryOnly=\"true\" blockBackButton=\"false\">\r\n" +
                "</form>\r\n";

            Assert.AreEqual(expectedXml, form.ToXml());
        }

        [Test]
        public void DataEntryOnlyFormXmlGeneratesCorrectXml()
        {
            string xmlString =
                "<form name=\"Form 1\" startPoint=\"false\" themePath=\"default\" dataEntryOnly=\"true\" blockBackButton=\"false\">\r\n" +
                "</form>\r\n";

            var form = new Form(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, form.ToXml());
        }

        [Test]
        public void FormIsNotDataEntryOnlyByDefault()
        {
            Assert.AreEqual(form.DataEntryOnly, false);
        }

        [Test]
        public void NonDataEntryOnlyFormGeneratesCorrectXml()
        {
            string expectedXml =
                "<form name=\"Form 1\" startPoint=\"false\" themePath=\"default\" blockBackButton=\"false\">\r\n" +
                "</form>\r\n";

            Assert.AreEqual(expectedXml, form.ToXml());
        }
    }
}