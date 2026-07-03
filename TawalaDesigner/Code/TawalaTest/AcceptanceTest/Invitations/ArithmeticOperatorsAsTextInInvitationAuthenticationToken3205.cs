// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects.Links;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Invitations
{
    [TestFixture]
    public class ArithmeticOperatorsAsTextInInvitationAuthenticationToken3205
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

        private const string newPrivateXmlAfter =
            "<invitation form=\"Form 1\" project=\"\" private=\"true\"><authenticationTokenValue><string value=\"2+2\"/>\r\n</authenticationTokenValue><displayText></displayText></invitation>";

        [Test]
        public void PrivateInvitationFromNewXml()
        {
            var invitation = new InvitationField(new XmlElement(newPrivateXmlAfter));
            Assert.AreEqual(newPrivateXmlAfter, invitation.ToXml());
        }
    }
}