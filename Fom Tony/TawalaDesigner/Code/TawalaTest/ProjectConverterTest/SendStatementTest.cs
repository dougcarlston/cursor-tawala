using System;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Processes;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion of SEND statements.
	/// </summary>
	[TestFixture]
	public class SendStatementTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("SendStatements.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void DocumentReferences()
		{
			Document doc = (Document)Project.Current.DocumentList[0];

			Process process = (Process)Project.Current.ProcessList[0];

			// verify that identical statements reference same document
			Assert.AreEqual(doc, ((SendDocumentBody)((SendStatement)process.Lines[1].Statement).SendBody).Document);
		}

		/// <summary>
		/// Tests the conversion of the XML file to a Tawala Project.
		/// </summary>
		[Test]
		public void ConvertXmlToProject()
		{

			// verify that project contains 1 form and 1 process
			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Process 1", ((Process)Project.Current.ProcessList[0]).Name);

			Process process = (Process)Project.Current.ProcessList[0];

			// verify that form and process are connected
            Assert.AreEqual(process, ((Form)Project.Current.FormList[0]).ConnectedPostProcess);

			// verify that process line is correct
			Assert.AreEqual("Send Email to Email Address", process.Lines[0].ToString());

			// verify that send statement contents are correct
			SendStatement sendStatement1 = (SendStatement)process.Lines[0].Statement;
			Assert.AreEqual("Email Address", sendStatement1.AddressTo.Text);
			Assert.AreEqual("example@tawala.com", sendStatement1.AddressCc.Text);
			Assert.AreEqual("Test Body", ((SendEmailBody)sendStatement1.SendBody).Text);
			Assert.AreEqual("Test Subject", sendStatement1.Subject);

			// verify that process line is correct
			Assert.AreEqual("Send Document 1 to Email Address", process.Lines[1].ToString());

			// verify that send statement contents are correct
			SendStatement sendStatement2 = (SendStatement)process.Lines[1].Statement;
			Assert.AreEqual("Email Address", sendStatement2.AddressTo.Text);
			Assert.AreEqual("example@tawala.com", sendStatement2.AddressCc.Text);
			Assert.AreEqual("Document 1", ((SendDocumentBody)sendStatement2.SendBody).Document.Name);
			Assert.AreEqual("Test Subject", sendStatement2.Subject);

			// verify that process line is correct
			Assert.AreEqual("Send Invitation to Form 1 to Email Address", process.Lines[2].ToString());

			// verify that send statement contents are correct
			SendStatement sendStatement3 = (SendStatement)process.Lines[2].Statement;
			Assert.AreEqual("Email Address", sendStatement3.AddressTo.Text);
			Assert.AreEqual("", sendStatement3.AddressCc.Text);
			Assert.AreEqual("Form 1", ((SendInvitationBody)sendStatement3.SendBody).Form.Name);
			Assert.AreEqual("Test Subject", sendStatement2.Subject);

		}


	}
}
