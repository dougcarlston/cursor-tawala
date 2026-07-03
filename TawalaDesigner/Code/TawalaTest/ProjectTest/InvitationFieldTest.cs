using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Links;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the InvitationField class
	/// </summary>
	[TestFixture]
	public class InvitationFieldTest
	{
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
			var form = new Form("Form 1");

			var invitationField = new InvitationField();
			invitationField.Form = form;
			invitationField.DisplayText = "click here";

			Assert.AreEqual(string.Empty, invitationField.ProjectName);

            string expectedXml = "<invitation form=\"Form 1\" project=\"\"><displayText><string value=\"click here\"/>\r\n</displayText></invitation>";

			Assert.AreEqual(expectedXml, invitationField.ToXml());
		}

		[Test]
		public void ForeignProjectInvitationFieldGeneratesXmlWithForeignProject()
		{
			var invitationField = new InvitationField();
			invitationField.ProjectName = "Foreign Project";
			invitationField.InitialFormName = "Foreign Form";
			invitationField.DisplayText = "click here";

			Assert.AreEqual(Form.NULL, invitationField.Form);

            string expectedXml = "<invitation form=\"Foreign Form\" project=\"Foreign Project\"><displayText><string value=\"click here\"/>\r\n</displayText></invitation>";

			Assert.AreEqual(expectedXml, invitationField.ToXml());
		}

		[Test]
		public void XmlGeneratesInvitationField()
		{
            string persistenceXml = "<invitation form=\"Form 1\" project=\"\"><displayText><string value=\"click here\"/>\r\n</displayText></invitation>";

			var documentInvitationField = new DocumentNamedInvitationField(new XmlElement(persistenceXml));

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
            string persistenceXml = "<invitation form=\"Foreign Form\" project=\"Foreign Project\"><displayText><string value=\"click here\"/>\r\n</displayText></invitation>";

			var documentInvitationField = new DocumentNamedInvitationField(new XmlElement(persistenceXml));

			Assert.IsNotNull(documentInvitationField.Invitation);
			Assert.AreEqual(Form.NULL, documentInvitationField.Invitation.Form);
			Assert.AreEqual("Foreign Project", documentInvitationField.Invitation.ProjectName);
			Assert.AreEqual("Foreign Form", documentInvitationField.Invitation.FormName);
			Assert.AreEqual("click here", documentInvitationField.Invitation.DisplayText);
			Assert.AreEqual(persistenceXml, documentInvitationField.Invitation.ToXml());
		}
	}
}
