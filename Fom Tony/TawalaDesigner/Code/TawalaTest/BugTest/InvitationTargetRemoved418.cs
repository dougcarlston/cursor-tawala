using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;


namespace TawalaTest.BugTest
{
    [TestFixture]
    public class InvitationTargetRemoved418
    {
		private IForm form;
        private const string expectedXml = "<invitation form=\"Form 1\" project=\"\"><displayText><string value=\"Invitation Text\"/>\r\n</displayText></invitation>";

        [SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();
		}

		private DocumentNamedInvitationField makeInvitationField()
		{
            IXmlElement element = new XmlElement(expectedXml);
			return (new DocumentNamedInvitationField(element));
		}

		[Test]
		public void InvitationProducesExpectedXml()
        {
			DocumentNamedInvitationField invitationField = makeInvitationField();

			Assert.AreEqual(expectedXml, invitationField.ToXml());
		}

		[Test]
		public void CreatingInvitationAfterFormDeletionDoesNotThrowException()
		{
			Project.Current.RemoveForm(form.Name);

			try
			{
				DocumentNamedInvitationField invitationField = makeInvitationField();
			}
			catch (Exception e)
			{
				Assert.Fail(string.Format("TEST FAULT: Unexpected exception after Form deletion: {0}", e.ToString()));
			}
		}

		[Test]
		public void InvitationProducesExpectedXmlAfterFormDeletion()
		{
			Project.Current.RemoveForm(form.Name);

			DocumentNamedInvitationField invitationField = makeInvitationField();

			Assert.AreEqual(expectedXml, invitationField.ToXml());
		}
	}
}