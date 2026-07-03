// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects.Links;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Invitations
{
    [TestFixture]
    public class InvitationDisplayTextIsExpression3193
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        private const string oldXmlBefore = @"<invitation form=""Form 1"" project="""">Display Text</invitation>";

        private const string newXmlAfter =
            "<invitation form=\"Form 1\" project=\"\"><displayText><string value=\"Display Text\"/>\r\n</displayText></invitation>";

        [Test]
        public void InvitationFromOldXml()
        {
            var invitation = new InvitationField(new XmlElement(oldXmlBefore));
            Assert.AreEqual(newXmlAfter, invitation.ToXml());
        }

        [Test]
        public void InvitationFromNewXml()
        {
            var invitation = new InvitationField(new XmlElement(newXmlAfter));
            Assert.AreEqual(newXmlAfter, invitation.ToXml());
        }

        private const string oldPrivateXmlBefore =
           "<invitation form=\"Form 1\" project=\"\" private=\"true\"><authenticationTokenValue><string value=\"Empty\"/>\r\n</authenticationTokenValue>Display Text</invitation>";

        private const string newPrivateXmlAfter =
            "<invitation form=\"Form 1\" project=\"\" private=\"true\"><authenticationTokenValue><string value=\"Empty\"/>\r\n</authenticationTokenValue><displayText><string value=\"Display Text\"/>\r\n</displayText></invitation>";

        [Test]
        public void PrivateInvitationFromOldXml()
        {
            var invitation = new InvitationField(new XmlElement(oldPrivateXmlBefore));
            Assert.AreEqual(newPrivateXmlAfter, invitation.ToXml());
        }

        [Test]
        public void PrivateInvitationFromNewXml()
        {
            var invitation = new InvitationField(new XmlElement(newPrivateXmlAfter));
            Assert.AreEqual(newPrivateXmlAfter, invitation.ToXml());
        }

    }
}