using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Components;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

using Tawala.Projects.Documents;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class SendBodyTest
	{
		private IForm form;
		private IDocument document;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();
			document = Project.Current.AddDocument();
		}

		[Test]
		public void ConstructSendEmailBody()
		{
			SendEmailBody body = new SendEmailBody();
			body.Text = "Test Body";

			Assert.AreEqual("Test Body", body.Text);
		}

		[Test]
		public void ConstructSendDocumentBody()
		{
			SendDocumentBody body = new SendDocumentBody(ComponentMaker.MakeDocumentObject("Test Document"));

			Assert.AreEqual("Test Document", body.Document.Name);
			Assert.AreEqual(false, body.ResetDocumentAfterSend);
		}

		[Test]
		public void ConstructSendDocumentBodyWithReset()
		{
			SendDocumentBody body = new SendDocumentBody(ComponentMaker.MakeDocumentObject("Test Document"));
			body.ResetDocumentAfterSend = true;

			Assert.AreEqual("Test Document", body.Document.Name);
			Assert.AreEqual(true, body.ResetDocumentAfterSend);
		}

		[Test]
		public void ConstructSendInvitationBody()
		{
			SendInvitationBody body;

			body = new SendInvitationBody();
			body.Form = form;
			body.Text = "Test Text";
			Assert.AreEqual("Form 1", body.Form.Name);
			Assert.AreEqual("Test Text", body.Text);

			body = new SendInvitationBody(form);
			body.Text = "Test Text";
			Assert.AreEqual("Form 1", body.Form.Name);
			Assert.AreSame(form, body.Form);
			Assert.AreEqual("Test Text", body.Text);

			body = new SendInvitationBody(form, "Test Text");
			Assert.AreEqual("Form 1", body.Form.Name);
			Assert.AreSame(form, body.Form);
			Assert.AreEqual("Test Text", body.Text);
		}

		[Test]
		public void ConstructSendEmailBodyFromXml()
		{
			string xmlString =
				"<body>Test Body</body>";

			IXmlElement element = new XmlElement(xmlString);
			SendEmailBody body = new SendEmailBody(element);

			Assert.AreEqual("Test Body", body.Text);
		}

		[Test]
		public void ConstructSendDocumentBodyFromXml()
		{
			string xmlString =
				"<body document=\"Document 1\"/>";

			IXmlElement element = new XmlElement(xmlString);
			SendDocumentBody body = new SendDocumentBody(element);

			Assert.AreEqual("Document 1", body.Document.Name);
			Assert.AreSame(document, body.Document);
			Assert.AreEqual(false, body.ResetDocumentAfterSend);
		}

		[Test]
		public void ConstructSendDocumentBodyFromXmlWithReset()
		{
			string xmlString =
				"<body document=\"Document 1\" reset=\"true\" showHeader=\"true\"/>";

			IXmlElement element = new XmlElement(xmlString);
			SendDocumentBody body = new SendDocumentBody(element);

			Assert.AreEqual("Document 1", body.Document.Name);
			Assert.AreSame(document, body.Document);
			Assert.AreEqual(true, body.ResetDocumentAfterSend);
		}

		[Test]
		public void ConstructSendVirtualDocumentBodyFromXml()
		{
			string xmlString =
				"<body document=\"Virtual Document\"/>";

			IXmlElement element = new XmlElement(xmlString);
			SendDocumentBody body = new SendDocumentBody(element);

			Assert.AreEqual("Virtual Document", body.Document.Name);
			Assert.AreEqual(false, body.ResetDocumentAfterSend);
		}

		[Test]
		public void ConstructSendInvitationBodyFromXml()
		{
			string xmlString =
				"<body inviteTo=\"Form 1\">Test Body</body>";

			IXmlElement element = new XmlElement(xmlString);
			SendInvitationBody body = new SendInvitationBody(element);

			Assert.AreEqual("Form 1", body.Form.Name);
			Assert.AreEqual("Test Body", body.Text);
			Assert.AreSame(form, body.Form);
		}

		[Test]
		public void ConstructSendInvitationBodyFromXmlRemoteForm()
		{
			string xmlString =
				"<body inviteTo=\"Remote Form\">Test Body</body>";

			IXmlElement element = new XmlElement(xmlString);
			SendInvitationBody body = new SendInvitationBody(element);

			Assert.AreEqual("Remote Form", body.Form.Name);
			Assert.AreEqual("Test Body", body.Text);
		}

		[Test]
		public void ConstructForeignSendInvitationBodyFromXml()
		{
			string xmlString =
				"<body inviteTo=\"Form 1\" project=\"Foreign Project\">Test Body</body>";

			IXmlElement element = new XmlElement(xmlString);
			SendForeignInvitationBody body = new SendForeignInvitationBody(element);

			Assert.AreEqual("Form 1", body.Form.Name);
			Assert.AreEqual("Test Body", body.Text);
			Assert.AreEqual("Foreign Project", body.ProjectName);
			Assert.AreSame(form, body.Form);
		}

		[Test]
		public void GetInvitationBodyDisplayText()
		{
			SendInvitationBody body = new SendInvitationBody(new Form("Test Form"), "Can you read this?");

			string expString =
				"Invitation to Test Form";

			Assert.AreEqual(expString, body.ToString());
		}

		[Test]
		public void GetForeignInvitationBodyDisplayText()
		{
			SendForeignInvitationBody body = new SendForeignInvitationBody(new Form("Test Form"), "Foreign Project", "Can you read this?");

			string expString =
				"Invitation to Foreign Project:Test Form";

			Assert.AreEqual(expString, body.ToString());
		}

		[Test]
		public void GetForeignInvitationBodyDisplayTextCurrentProject()
		{
			// test with the actual current Project name
			SendForeignInvitationBody body = new SendForeignInvitationBody(new Form("Test Form"), "(Current Project)", "Can you read this?");

			string expString =
				"Invitation to Test Form";

			Assert.AreEqual(expString, body.ToString());
		}

		[Test]
		public void GetForeignInvitationBodyDisplayTextCurrentProjectByName()
		{
			// test with the actual current Project name
			SendForeignInvitationBody body = new SendForeignInvitationBody(new Form("Test Form"), Project.Current.Name, "Can you read this?");

			string expString =
				"Invitation to Test Form";

			Assert.AreEqual(expString, body.ToString());
		}

		[Test]
		public void GetForeignInvitationBodyDisplayTextNoProject()
		{
			SendForeignInvitationBody body = new SendForeignInvitationBody(new Form("Test Form"), "", "Can you read this?");

			string expString =
				"Invitation to Test Form";

			Assert.AreEqual(expString, body.ToString());
		}

		[Test]
		public void GetDocumentBodyXml()
		{
			SendDocumentBody body = new SendDocumentBody(Document.NULL);
			body.Document = new Document("Test Document");

			string expString =
				"<body document=\"Test Document\" reset=\"false\" showHeader=\"true\"/>\r\n";

			Assert.AreEqual(expString, body.ToXml());
		}

		[Test]
		public void GetDocumentBodyXmlWithReset()
		{
			SendDocumentBody body = new SendDocumentBody(Document.NULL);
			body.Document = new Document("Test Document");
			body.ResetDocumentAfterSend = true;

			string expString =
				"<body document=\"Test Document\" reset=\"true\" showHeader=\"true\"/>\r\n";

			Assert.AreEqual(expString, body.ToXml());
		}

		[Test]
		public void GetInvitationBodyXml()
		{
			SendInvitationBody body = new SendInvitationBody(new Form("Test Form"), "Can you read this?");

			string expString =
				"<body inviteTo=\"Test Form\">Can you read this?</body>\r\n";

			Assert.AreEqual(expString, body.ToXml());
		}

		[Test]
		public void GetForeignInvitationBodyXml()
		{
			SendForeignInvitationBody body = new SendForeignInvitationBody(new Form("Test Form"), "Foreign Project", "Can you read this?");

			string expString =
				"<body inviteTo=\"Test Form\" project=\"Foreign Project\">Can you read this?</body>\r\n";

			Assert.AreEqual(expString, body.ToXml());
		}

		[Test]
		public void GetForeignInvitationBodyXmlCurrentProject()
		{
			// test with "(Current Project)" as the name
			SendForeignInvitationBody body = new SendForeignInvitationBody(new Form("Test Form"), "(Current Project)", "Can you read this?");

			string expString =
				"<body inviteTo=\"Test Form\">Can you read this?</body>\r\n";

			Assert.AreEqual(expString, body.ToXml());
		}

		[Test]
		public void GetForeignInvitationBodyXmlCurrentProjectByName()
		{
			// test with the actual current Project name
			SendForeignInvitationBody body = new SendForeignInvitationBody(new Form("Test Form"), Project.Current.Name, "Can you read this?");

			string expString =
				"<body inviteTo=\"Test Form\">Can you read this?</body>\r\n";

			Assert.AreEqual(expString, body.ToXml());
		}

		[Test]
		public void GetForeignInvitationBodyXmlNoProject()
		{
			SendForeignInvitationBody body = new SendForeignInvitationBody(new Form("Test Form"), "", "Can you read this?");

			string expString =
				"<body inviteTo=\"Test Form\">Can you read this?</body>\r\n";

			Assert.AreEqual(expString, body.ToXml());
		}

	}
}
