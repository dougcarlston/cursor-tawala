// $Workfile: XMLAuthenticateTest.cs $
// $Revision: 11 $	$Date: 10/31/07 11:34a $
// Copyright © 2005 Douglas G. Carlston. All rights reserved.

using System;
using NUnit.Framework;
using Tawala.Common;

namespace TawalaTest.FunctionalityTest
{
	[TestFixture]
	public class XMLAuthenticateTest
	{
		private const string xmlStringFormat = 
			"<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
			"<request type=\"uploadProject\" protocol=\"1.0\">" +
			"{0}" +
			"<project name=\"Test Project\" themePath=\"default\" format=\"1.7\">" +
			"<forms>" +
			"<form name=\"Form 1\">" +
			"</form>" +
			"</forms>" +
			"</project>" +
			"</request>";

		private XMLTransceiver transceiver;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			transceiver = new XMLTransceiver("http://dev.tawala.com/client");
		}

		[TearDown]
		public void TearDown()
		{
			transceiver = null;
		}

		[Test]
		public void UploadValidCredentialsTest()
		{
			string expectedResponse = 
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n\r\n" +
				"<response status=\"success\">\r\n" +
				"  <deployments user=\"aTestUser\">\r\n" +
				"    <deployment project=\"Test Project\">\r\n" +
				"      <startpoint form=\"Form 1\" url=\"http://dev.tawala.com/p/gl1oqktohc1xnppo4xdl/Form+1\"/>\r\n" +
				"    </deployment>\r\n" +
				"  </deployments>\r\n" +
				"</response>\r\n";

			string rcvString = tranceive("aTestUser", "JFSBKM185");

			// Assertion
			Assert.AreEqual(expectedResponse, rcvString);
		}

		[Test]
		public void UploadInvalidUserTest()
		{
			string expectedResponse =
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n\r\n" +
				"<response status=\"failure\">\r\n" +
				"  <error id=\"auth.failed\" message=\"unknown userid or password\"/>\r\n" +
				"</response>\r\n";

			string rcvString = tranceive("anInvalidUser", "JFSBKM185");

			Assert.AreEqual(expectedResponse, rcvString);
		}

		[Test]
		public void UploadInvalidPasswordTest()
		{
			string expectedResponse = 
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n\r\n" +
				"<response status=\"failure\">\r\n" +
				"  <error id=\"auth.failed\" message=\"unknown userid or password\"/>\r\n" +
				"</response>\r\n";

			string rcvString = tranceive("aTestUser", "bopusPW");
			Assert.AreEqual(expectedResponse, rcvString);
		}

		private string tranceive(string userName, string password)
		{
			string xmlCredentials = Tawala.Common.GlobalSettings.CredentialsElement(userName, password);
			transceiver.Transmit(string.Format(xmlStringFormat, xmlCredentials));
			return transceiver.Receive();
		}
	}
}
