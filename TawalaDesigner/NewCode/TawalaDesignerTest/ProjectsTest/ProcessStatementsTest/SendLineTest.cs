using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
	public class SendLineTest
	{
#if FIXED
		private IForm form;
		private Process process;
		private SendStatement sendStatement;
		private SendLine sendLine;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
            Util.NewTestProject();

			// create form
			form = Project.Current.AddForm();

			// create process
			process = Project.Current.AddProcess();

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			sendStatement = new SendStatement();
			sendLine = new SendLine(sendStatement);
			process.Lines.Add(sendLine);
		}

		[Test]
		public void InvalidSendEmail()
		{

			// it shouldn't be valid unless it has at least one To: addressee AND a Subject sendLine
			// (both are null to begin with)
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);

			// just subject text is not enough
			sendStatement.Subject = "Some text.";
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);

			// now we have both (plus a body); should be valid
			sendStatement.SendBody = new SendEmailBody();
			sendStatement.AddressTo.Text = "jdf@jdftech.com";
			process.Lines.ValidateLines();
			Assert.AreEqual(true, sendLine.IsValid);

			// test empty strings, as well as null
			sendStatement.AddressTo.Text = "";
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);

			sendStatement.AddressTo.Text = "jdf@jdftech.com";
			sendStatement.Subject = "";

			//Assertion
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);
		}

		[Test]
		[Ignore("This test uses obsolete method of setting document Text with HTML!")]
		public void InvalidSendDocumentAsBody()
		{
			// it shouldn't be valid unless it has at least one To: addressee AND a Subject sendLine
			// (both are null to begin with)
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);

			// just subject text is not enough
			sendStatement.Subject = "Some text.";
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);

			// nor is subject + address
			sendStatement.AddressTo.Text = "jdf@jdftech.com";
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);

			// now add a document; should be valid
			IDocument doc = Project.Current.AddDocument();
			sendStatement.SendBody = new SendDocumentBody(doc);
			process.Lines.ValidateLines();
			Assert.AreEqual(true, sendLine.IsValid);

			// test empty strings, as well as null
			sendStatement.AddressTo.Text = "";
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);

			sendStatement.AddressTo.Text = "jdf@jdftech.com";
			sendStatement.Subject = "";
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);

			// now recreate a valid subject string, and we'll test for valid fields in the document
			sendStatement.Subject = "Some text.";

			string htmlContent =
				Document.RawHtmlPrefix +
				"<p><span style=\"font-size:10pt;\">Here is a field &lt;&lt;Q1:a&gt;&gt; within some document text.</span></p>" +
				Document.RawHtmlPostfix;
			doc.Text = htmlContent;

			// sendStatement should be invalid because the Form does not contain Q1:a
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);

			// add the missing field
			FibItem fib = new FibItem();
			form.ItemList.Add(fib);

			process.Lines.ValidateLines();
			Assert.AreEqual(true, sendLine.IsValid);
		}

		[Test]
		public void InvalidSendInvitation()
		{
			sendStatement.AddressTo.Text = "jdf@jdftech.com";
			sendStatement.Subject = "Some text.";

			// no starting point assigned yet, should be invalid
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);

			sendStatement.SendBody = new SendInvitationBody(form);
			process.Lines.ValidateLines();
			Assert.AreEqual(true, sendLine.IsValid);

			Project.Current.RemoveForm("Form 1");
			process.Lines.ValidateLines();
			Assert.AreEqual(false, sendLine.IsValid);
		}
#endif
	}
}
