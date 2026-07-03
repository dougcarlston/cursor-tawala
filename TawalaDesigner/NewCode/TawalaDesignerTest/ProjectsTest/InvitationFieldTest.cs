using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
	public class InvitationFieldTest
	{
#if FIXED
		private IForm form;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			form = Project.Current.AddForm();
		}

		[Test]
		public void InvitationFieldGeneratesXml()
		{
			IForm form = new NewForm("Form 1");

			InvitationField invitationField = new InvitationField();
			invitationField.Form = form;
			invitationField.DisplayText = "click here";

			Assert.AreEqual(string.Empty, invitationField.ProjectName);

			string expectedXml = "<invitation form=\"Form 1\" project=\"\">click here</invitation>";

			Assert.AreEqual(expectedXml, invitationField.ToXml());
		}

		[Test]
		public void ForeignProjectInvitationFieldGeneratesXmlWithForeignProject()
		{
			InvitationField invitationField = new InvitationField();
			invitationField.ProjectName = "Foreign Project";
			invitationField.InitialFormName = "Foreign Form";
			invitationField.DisplayText = "click here";

			Assert.AreEqual(NullObjects.Form, invitationField.Form);

			string expectedXml = "<invitation form=\"Foreign Form\" project=\"Foreign Project\">click here</invitation>";

			Assert.AreEqual(expectedXml, invitationField.ToXml());
		}

		[Test]
		public void XmlGeneratesInvitationField()
		{
			string persistenceXml = "<invitation form=\"Form 1\" project=\"\">click here</invitation>";

			DocumentNamedInvitationField documentInvitationField = new DocumentNamedInvitationField(new XmlElement(persistenceXml));

			Assert.IsNotNull(documentInvitationField.Invitation);
			Assert.AreEqual(form, documentInvitationField.Invitation.Form);
			Assert.AreEqual("", documentInvitationField.Invitation.ProjectName);
			Assert.AreEqual("Form 1", documentInvitationField.Invitation.FormName);
			Assert.AreEqual("click here", documentInvitationField.Invitation.DisplayText);
			Assert.AreEqual(persistenceXml, documentInvitationField.Invitation.ToXml());
		}

		[Test]
		public void XmlWithForeignProjectGeneratesInvitationFieldWithForeignProject()
		{
			string persistenceXml = "<invitation form=\"Foreign Form\" project=\"Foreign Project\">click here</invitation>";

			DocumentNamedInvitationField documentInvitationField = new DocumentNamedInvitationField(new XmlElement(persistenceXml));

			Assert.IsNotNull(documentInvitationField.Invitation);
			Assert.AreEqual(NullObjects.Form, documentInvitationField.Invitation.Form);
			Assert.AreEqual("Foreign Project", documentInvitationField.Invitation.ProjectName);
			Assert.AreEqual("Foreign Form", documentInvitationField.Invitation.FormName);
			Assert.AreEqual("click here", documentInvitationField.Invitation.DisplayText);
			Assert.AreEqual(persistenceXml, documentInvitationField.Invitation.ToXml());
		}
#endif
	}
}
