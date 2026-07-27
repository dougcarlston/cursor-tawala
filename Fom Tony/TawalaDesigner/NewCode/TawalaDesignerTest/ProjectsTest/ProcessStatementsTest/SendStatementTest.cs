using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

using Tawala.Projects.Documents;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
	public class SendStatementTest
	{
#if FIXED
		private IForm form;
		private IDocument document;
		private Process process;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();
			Project.Current.Name = "Test Project";

			form = Project.Current.AddForm();

			document = Project.Current.AddDocument();

			process = Project.Current.AddProcess();

			Project.Current.ConnectProcessToForm(process, form.Name);

			FibItem fibItem1 = new FibItem();
			FibItem fibItem2 = new FibItem();

			form.ItemList.Add(fibItem1);
			form.ItemList.Add(fibItem2);

			process.MappedFields.Fields.Add(fibItem1.BlankList[0]);
			process.MappedFields.Fields.Add(fibItem2.BlankList[0]);
			process.MappedFields.Map();
		}

		[Test]
		public void Construct() 
		{ 
			ProcessStatement statement = new SendStatement();
		
			Assert.IsNotNull(statement);
		}

		[Test]
		public void Name() 
		{ 
			ProcessStatement statement = new SendStatement();
		
			string name = statement.Name;

			Assert.AreEqual("Send", name);
		} 

		[Test]
		public void GetText() 
		{ 
			SendStatement statement = new SendStatement();
			statement.AddressTo.Text = "doug@carlston.net";
		
			Document doc = new Document("Test Doc");
			statement.SendBody = new SendDocumentBody(doc);

			Assert.AreEqual(doc, ((SendDocumentBody)statement.SendBody).Document);
			Assert.AreEqual("Send Test Doc to \"doug@carlston.net\"", statement.ToString());
		}

		[Test]
		public void GetTextWithReset()
		{
			SendStatement statement = new SendStatement();
			statement.AddressTo.Text = "doug@carlston.net";

			Document doc = new Document("Test Doc");
			SendDocumentBody body = new SendDocumentBody(doc);
			body.ResetDocumentAfterSend = true;
			statement.SendBody = body;

			Assert.AreEqual(doc, ((SendDocumentBody)statement.SendBody).Document);
			Assert.AreEqual("Send Test Doc to \"doug@carlston.net\" and reset Document", statement.ToString());
		}

		[Test]
		public void InvalidCharacters()
		{
			SendStatement statement = new SendStatement();

			// let's check for invalid XML characters
			statement.AddressTo.Text = "do<ug@ca>rlston's.net";
			statement.AddressCc.Text = "jdf@\"jd&ftech.com\"";
			statement.Subject = "Testing \"b&d\" c<ar>ter's";
			statement.SendBody = new SendDocumentBody(new Document("\"T&est's\" <Doc>"));

			string expString = "<send>\r\n" +
						"<to addressLiteral=\"do&lt;ug@ca&gt;rlston&apos;s.net\"/>\r\n" +
						"<cc addressLiteral=\"jdf@&quot;jd&amp;ftech.com&quot;\"/>\r\n" +
						"<subject>Testing \"b&amp;d\" c&lt;ar&gt;ter's</subject>\r\n" +
						"<body document=\"&quot;T&amp;est&apos;s&quot; &lt;Doc&gt;\" reset=\"false\" showHeader=\"true\"/>\r\n" +
						"</send>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void GetXml()
		{
			SendStatement statement = new SendStatement();

			// test with literal addresses
			statement.AddressTo.Text = "doug@carlston.net";
			statement.AddressCc.Text = "jdf@jdftech.com";
			statement.Subject = "Testing the Send command";
			statement.SendBody = new SendDocumentBody(new Document("Test Doc"));

			string expString =
				"<send>\r\n" +
				"<to addressLiteral=\"doug@carlston.net\"/>\r\n" +
				"<cc addressLiteral=\"jdf@jdftech.com\"/>\r\n" +
				"<subject>Testing the Send command</subject>\r\n" +
				"<body document=\"Test Doc\" reset=\"false\" showHeader=\"true\"/>\r\n" +
				"</send>";
			Assert.AreEqual(expString, statement.ToXml());


			// test with field addresses
			statement.AddressTo.Text = "Q1:a";
			statement.AddressTo.Type = FieldOrLiteral.StringType.field;
			statement.AddressCc.Text = "Q2:a";
			statement.AddressCc.Type = FieldOrLiteral.StringType.field;

			expString =
				"<send>\r\n" +
				"<to addressField=\"Form 1:Q1:a\"/>\r\n" +
				"<cc addressField=\"Form 1:Q2:a\"/>\r\n" +
				"<subject>Testing the Send command</subject>\r\n" +
				"<body document=\"Test Doc\" reset=\"false\" showHeader=\"true\"/>\r\n" +
				"</send>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void GetXmlWithReset()
		{
			SendStatement statement = new SendStatement();

			statement.AddressTo.Text = "doug@carlston.net";
			statement.AddressCc.Text = "jdf@jdftech.com";
			statement.Subject = "Testing the Send command";

			SendDocumentBody body = new SendDocumentBody(new Document("Test Doc"));
			body.ResetDocumentAfterSend = true;
			statement.SendBody = body;

			string expString =
				"<send>\r\n" +
				"<to addressLiteral=\"doug@carlston.net\"/>\r\n" +
				"<cc addressLiteral=\"jdf@jdftech.com\"/>\r\n" +
				"<subject>Testing the Send command</subject>\r\n" +
				"<body document=\"Test Doc\" reset=\"true\" showHeader=\"true\"/>\r\n" +
				"</send>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
				"<send>\r\n" +
				"<to addressField=\"Q1:a\"/>\r\n" +
				"<cc addressLiteral=\"ccRecipient@tawala.com\"/>\r\n" +
				"<subject>Testing Construction from XML</subject>\r\n" +
				"<body document=\"Document 1\"/>" +
				"</send>";

			IXmlElement element = new XmlElement(xmlString);

			SendStatement statement = new SendStatement(element, process.Name);

			Assert.AreEqual("Form 1:Q1:a", statement.AddressTo.Text);
			Assert.AreEqual(FieldOrLiteral.StringType.field, statement.AddressTo.Type);
			Assert.AreEqual("ccRecipient@tawala.com", statement.AddressCc.Text);
			Assert.AreEqual("Testing Construction from XML", statement.Subject);
			Assert.AreEqual("Document 1", ((SendDocumentBody)statement.SendBody).Document.Name);
			Assert.AreEqual(false, ((SendDocumentBody)statement.SendBody).ResetDocumentAfterSend);
		}

		[Test]
		public void ConstructFromXmlWithReset()
		{
			string xmlString =
				"<send>\r\n" +
				"<to addressField=\"Q1:a\"/>\r\n" +
				"<cc addressLiteral=\"ccRecipient@tawala.com\"/>\r\n" +
				"<subject>Testing Construction from XML</subject>\r\n" +
				"<body document=\"Document 1\" reset=\"true\" showHeader=\"true\"/>" +
				"</send>";

			IXmlElement element = new XmlElement(xmlString);

			SendStatement statement = new SendStatement(element, process.Name);

			Assert.AreEqual("Form 1:Q1:a", statement.AddressTo.Text);
			Assert.AreEqual(FieldOrLiteral.StringType.field, statement.AddressTo.Type);
			Assert.AreEqual("ccRecipient@tawala.com", statement.AddressCc.Text);
			Assert.AreEqual("Testing Construction from XML", statement.Subject);
			Assert.AreEqual("Document 1", ((SendDocumentBody)statement.SendBody).Document.Name);
			Assert.AreEqual(true, ((SendDocumentBody)statement.SendBody).ResetDocumentAfterSend);
		}

		[Test]
		public void ConstructSendVirtualDocumentFromXml()
		{
			string xmlString =
				"<send>\r\n" +
				"<to addressField=\"Q1:a\"/>\r\n" +
				"<cc addressLiteral=\"ccRecipient@tawala.com\"/>\r\n" +
				"<subject>Testing Construction from XML</subject>\r\n" +
				"<body document=\"Virtual Document\"/>" +
				"</send>";

			IXmlElement element = new XmlElement(xmlString);

			SendStatement statement = new SendStatement(element, process.Name);

			Assert.AreEqual("Form 1:Q1:a", statement.AddressTo.Text);
			Assert.AreEqual(FieldOrLiteral.StringType.field, statement.AddressTo.Type);
			Assert.AreEqual("ccRecipient@tawala.com", statement.AddressCc.Text);
			Assert.AreEqual("Testing Construction from XML", statement.Subject);
			Assert.AreEqual("Virtual Document", ((SendDocumentBody)statement.SendBody).Document.Name);
		}

		[Test]
		public void RepositionField()
		{
			string xmlString =
				"<send>\r\n" +
				"<to addressField=\"Q1:a\"/>\r\n" +
				"</send>";

			IXmlElement element = new XmlElement(xmlString);
			SendStatement statement = new SendStatement(element, process.GetValidFields(0));

			Assert.AreEqual("Form 1:Q1:a", statement.AddressTo.Text);

			FibItem fibItem = new FibItem();
			form.ItemList.Insert(0, fibItem);

			Assert.AreEqual("Form 1:Q2:a", statement.AddressTo.Text);
		}

		[Test]
		public void Copy()
		{
			SendStatement statement1 = new SendStatement();
			statement1.AddressTo.Text = "doug@carlston.net";
			statement1.SendBody = new SendForeignInvitationBody(form, "Foreign Project", "Body Text");
			statement1.SendBody = new SendDocumentBody(document);

			SendStatement statement2 = (SendStatement)statement1.Copy();

			Assert.AreNotSame(statement1, statement2);
			Assert.AreSame(statement1.AddressTo, statement2.AddressTo);

//			Assert.AreSame(statement1.Subject, statement2.Subject);
			Assert.AreEqual(statement1.Subject, statement2.Subject);

			Assert.AreSame(statement1.SendBody, statement2.SendBody);
			Assert.AreSame(document, ((SendDocumentBody)statement2.SendBody).Document);
		}

#region ObsoleteTests
		// tests for obsolete varieties of the Send statement

		[Test]
		public void ConstructSendEmailFromXml()
		{
			string xmlString =
				"<send>\r\n" +
				"<to addressLiteral=\"toRecipient@tawala.com\"/>\r\n" +
				"<cc addressLiteral=\"ccRecipient@tawala.com\"/>\r\n" +
				"<subject>Testing Construction from XML</subject>\r\n" +
				"<body>This is the body of the email.</body>\r\n" +
				"</send>";

			IXmlElement element = new XmlElement(xmlString);

			SendStatement statement = new SendStatement(element, process.Name);

			Assert.AreEqual("toRecipient@tawala.com", statement.AddressTo.Text);
			Assert.AreEqual("ccRecipient@tawala.com", statement.AddressCc.Text);
			Assert.AreEqual("Testing Construction from XML", statement.Subject);
			Assert.AreEqual("This is the body of the email.", ((SendEmailBody)statement.SendBody).Text);
		}

		[Test]
		public void GetSendEmailText()
		{
			SendStatement statement = new SendStatement();
			statement.AddressTo.Text = "doug@carlston.net";

			process.Lines.Add(new ProcessLineList(statement));

			Assert.AreEqual("Send Email to \"doug@carlston.net\"", statement.ToString());

			statement.AddressTo.Text = "Q1:a";
			statement.AddressTo.Type = FieldOrLiteral.StringType.field;

			Assert.AreEqual("Send Email to Form 1:Q1:a", statement.ToString());
		} 

		[Test]
		public void InvalidEmailCharacters() 
		{ 
			SendStatement statement = new SendStatement();

			// let's check for invalid XML characters
			statement.AddressTo.Text = "do<ug@ca>rlston's.net";
			statement.AddressCc.Text = "jdf@\"jd&ftech.com\"";
			statement.Subject = "Testing \"b&d\" c<ar>ter's";
			statement.SendBody = new SendEmailBody("Testing \"b&d\" c<ar>ter's");

			string expString = "<send>\r\n" +
						"<to addressLiteral=\"do&lt;ug@ca&gt;rlston&apos;s.net\"/>\r\n" +
						"<cc addressLiteral=\"jdf@&quot;jd&amp;ftech.com&quot;\"/>\r\n" +
						"<subject>Testing \"b&amp;d\" c&lt;ar&gt;ter's</subject>\r\n" +
						"<body>Testing \"b&amp;d\" c&lt;ar&gt;ter's</body>\r\n" +
						"</send>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void GetSendEmailXml()
		{
			SendStatement statement = new SendStatement();

			// test with literal addresses
			statement.AddressTo.Text = "doug@carlston.net";
			statement.AddressCc.Text = "jdf@jdftech.com";
			statement.Subject = "Testing the Send command";
			statement.SendBody = new SendEmailBody("Hi Doug, can you read this?");

			string expString =
				"<send>\r\n" +
				"<to addressLiteral=\"doug@carlston.net\"/>\r\n" +
				"<cc addressLiteral=\"jdf@jdftech.com\"/>\r\n" +
				"<subject>Testing the Send command</subject>\r\n" +
				"<body>Hi Doug, can you read this?</body>\r\n" +
				"</send>";
			Assert.AreEqual(expString, statement.ToXml());
		
			// test with field addresses
			statement.AddressTo.Text = "Q1:a";
			statement.AddressTo.Type = FieldOrLiteral.StringType.field;
			statement.AddressCc.Text = "Q2:a";
			statement.AddressCc.Type = FieldOrLiteral.StringType.field;

			expString =
				"<send>\r\n" +
				"<to addressField=\"Form 1:Q1:a\"/>\r\n" +
				"<cc addressField=\"Form 1:Q2:a\"/>\r\n" +
				"<subject>Testing the Send command</subject>\r\n" +
				"<body>Hi Doug, can you read this?</body>\r\n" +
				"</send>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void ConstructSendInvitationFromXml()
		{
			string xmlString =
				"<send>\r\n" +
				"<to addressField=\"Q1:a\"/>\r\n" +
				"<subject>Testing Construction from XML</subject>\r\n" +
				"<body inviteTo=\"Form 1\">Click the link below to take our survey:</body>\r\n" +
				"</send>";

			IXmlElement element = new XmlElement(xmlString);
			SendStatement statement = new SendStatement(element, process.Name);

			Assert.AreEqual("Form 1:Q1:a", statement.AddressTo.Text);
			Assert.AreEqual("Testing Construction from XML", statement.Subject);
			Assert.AreEqual("Click the link below to take our survey:", ((SendInvitationBody)statement.SendBody).Text);
			Assert.AreEqual("Form 1", ((SendInvitationBody)statement.SendBody).Form.Name);
		}

		[Test]
		public void ConstructSendForeignInvitationFromXml()
		{
			string xmlString =
				"<send>\r\n" +
				"<to addressField=\"Q1:a\"/>\r\n" +
				"<subject>Testing Construction from XML</subject>\r\n" +
				"<body inviteTo=\"Form 1\" project=\"Test Project\">Click the link below to take our survey:</body>\r\n" +
				"</send>";

			IXmlElement element = new XmlElement(xmlString);
			SendStatement statement = new SendStatement(element, process.Name);

			Assert.AreEqual("Form 1:Q1:a", statement.AddressTo.Text);
			Assert.AreEqual("Testing Construction from XML", statement.Subject);
			Assert.AreEqual("Click the link below to take our survey:", ((SendForeignInvitationBody)statement.SendBody).Text);
			Assert.AreEqual("Form 1", ((SendForeignInvitationBody)statement.SendBody).Form.Name);
			Assert.AreEqual("Test Project", ((SendForeignInvitationBody)statement.SendBody).ProjectName);
		}

		[Test]
		public void GetSendInvitationXml()
		{
			SendStatement statement = new SendStatement();

			// test with literal addresses
			statement.AddressTo.Text = "doug@carlston.net";
			statement.Subject = "Testing the Send command";
			statement.SendBody = new SendInvitationBody(new NewForm("Test Form"), "Hi Doug, can you read this?");

			string expString =
				"<send>\r\n" +
				"<to addressLiteral=\"doug@carlston.net\"/>\r\n" +
				"<subject>Testing the Send command</subject>\r\n" +
				"<body inviteTo=\"Test Form\">Hi Doug, can you read this?</body>\r\n" +
				"</send>";
			Assert.AreEqual(expString, statement.ToXml());


			// test with field addresses
			statement.AddressTo.Text = "Q1:a";
			statement.AddressTo.Type = FieldOrLiteral.StringType.field;

			expString =
				"<send>\r\n" +
				"<to addressField=\"Form 1:Q1:a\"/>\r\n" +
				"<subject>Testing the Send command</subject>\r\n" +
				"<body inviteTo=\"Test Form\">Hi Doug, can you read this?</body>\r\n" +
				"</send>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void GetSendForeignInvitationXml()
		{
			SendStatement statement = new SendStatement();

			// test with literal addresses
			statement.AddressTo.Text = "doug@carlston.net";
			statement.Subject = "Testing the Send command";
			statement.SendBody = new SendForeignInvitationBody(new NewForm("Test Form"), "Foreign Project", "Hi Doug, can you read this?");

			string expString =
				"<send>\r\n" +
				"<to addressLiteral=\"doug@carlston.net\"/>\r\n" +
				"<subject>Testing the Send command</subject>\r\n" +
				"<body inviteTo=\"Test Form\" project=\"Foreign Project\">Hi Doug, can you read this?</body>\r\n" +
				"</send>";
			Assert.AreEqual(expString, statement.ToXml());


			// test with field addresses
			statement.AddressTo.Text = "Q1:a";
			statement.AddressTo.Type = FieldOrLiteral.StringType.field;

			expString =
				"<send>\r\n" +
				"<to addressField=\"Form 1:Q1:a\"/>\r\n" +
				"<subject>Testing the Send command</subject>\r\n" +
				"<body inviteTo=\"Test Form\" project=\"Foreign Project\">Hi Doug, can you read this?</body>\r\n" +
				"</send>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void ConstructSendInvitation()
		{
			SendStatement statement = new SendStatement();
			statement.AddressTo.Text = "doug@carlston.net";
			statement.SendBody = new SendInvitationBody(form, "");

			Assert.AreSame(form, ((SendInvitationBody)statement.SendBody).Form);
			Assert.AreEqual("Send Invitation to Form 1 to \"doug@carlston.net\"", statement.ToString());
		}

		[Test]
		public void ConstructSendForeignInvitation()
		{
			SendStatement statement = new SendStatement();
			statement.AddressTo.Text = "doug@carlston.net";
			statement.SendBody = new SendForeignInvitationBody(form, "Foreign Project", "");

			Assert.AreSame(form, ((SendInvitationBody)statement.SendBody).Form);
			Assert.AreEqual("Send Invitation to Foreign Project:Form 1 to \"doug@carlston.net\"", statement.ToString());
		}

		[Test]
		public void ConstructSendForeignInvitationCurrentProject()
		{
			SendStatement statement = new SendStatement();
			statement.AddressTo.Text = "doug@carlston.net";
			statement.SendBody = new SendForeignInvitationBody(form, Project.Current.Name, "");

			Assert.AreSame(form, ((SendInvitationBody)statement.SendBody).Form);
			Assert.AreEqual("Send Invitation to Form 1 to \"doug@carlston.net\"", statement.ToString());
		}

		[Test]
		public void CopyInvitation()
		{
			SendStatement statement1 = new SendStatement();
			statement1.AddressTo.Text = "doug@carlston.net";
			statement1.SendBody = new SendForeignInvitationBody(form, "Foreign Project", "Body Text");

			SendStatement statement2 = (SendStatement)statement1.Copy();

			Assert.AreNotSame(statement1, statement2);
			Assert.AreSame(statement1.AddressTo, statement2.AddressTo);

//			Assert.AreSame(statement1.Subject, statement2.Subject);
			Assert.AreEqual(statement1.Subject, statement2.Subject);

			Assert.AreSame(statement1.SendBody, statement2.SendBody);
		}
#endregion
#endif
	}
}
