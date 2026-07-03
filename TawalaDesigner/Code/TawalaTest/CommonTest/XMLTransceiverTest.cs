using System;
using NUnit.Framework;
using Tawala.Common;

namespace TawalaTest.CommonTest
{
	[TestFixture]
	public class XMLTransceiverTest 
	{ 
		[Test]
		public void SonicEcho() 
		{ 
			// instantiate transceiver
			XMLTransceiver transceiver = new XMLTransceiver ("http://www.sonic.net/stevenb/20qtest/echo.php");

			// send text to URL
			transceiver.Transmit ("Simple Text String");
			// receive text from URL
			string rcvString = transceiver.Receive();
			
			//Assertions 
			Assert.AreEqual ("Simple Text String\r\n", rcvString);
		}

		[Test]
		public void CalcRequestTimeout()
		{
			const int halfMB = 1048576 / 2;

			Assert.AreEqual(30000, XMLTransceiver.CalcRequestTimeout(0));
			Assert.AreEqual(30000, XMLTransceiver.CalcRequestTimeout(halfMB-1));
			Assert.AreEqual(45000, XMLTransceiver.CalcRequestTimeout(halfMB));
			Assert.AreEqual(45000, XMLTransceiver.CalcRequestTimeout(halfMB * 2 - 1));
			Assert.AreEqual(60000, XMLTransceiver.CalcRequestTimeout(halfMB * 2));
			Assert.AreEqual(75000, XMLTransceiver.CalcRequestTimeout(halfMB * 3));
			Assert.AreEqual(90000, XMLTransceiver.CalcRequestTimeout(halfMB * 4));
			Assert.AreEqual(105000, XMLTransceiver.CalcRequestTimeout(halfMB * 5));
			Assert.AreEqual(120000, XMLTransceiver.CalcRequestTimeout(halfMB * 6));
		}

		private void testDeploy(string deploymentURL, string scrambledProjectURL)
		{
			XMLTransceiver transceiver = new XMLTransceiver(deploymentURL + "/client");

			// NOTE: The aTestUser account is a special one set up for testing communications
			//		with the Dev Server. Don't change the name of the Project or the Form, and
			//		don't add any additional Forms, or the response XML will change and this
			//		test and others elsewhere will fail.
			string xmlString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
								"<request type=\"uploadProject\" protocol=\"1.0\">" +
								Tawala.Common.GlobalSettings.CredentialsElement("aTestUser", "JFSBKM185") +
								"<project name=\"Test Project\" themePath=\"default\" format=\"1.7\">" +
								"<forms>" +
								"<form name=\"Form 1\">" +
								"</form>" +
								"</forms>" +
								"</project>" +
								"</request>";

			string expectedString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n\r\n" +
									"<response status=\"success\">\r\n" +
									"  <deployments user=\"aTestUser\">\r\n" +
									"    <deployment project=\"Test Project\">\r\n" +
									"      <startpoint form=\"Form 1\" url=\"" + deploymentURL + "/p/" + scrambledProjectURL + "/Form+1\"/>\r\n" +
									"    </deployment>\r\n" +
									"  </deployments>\r\n" +
									"</response>\r\n";

			transceiver.Transmit(xmlString);

			string rcvString = transceiver.Receive();

			Console.WriteLine("XMLTransceiverTest.testDeploy: rcvString = {0}", rcvString);

			Assert.AreEqual(expectedString, rcvString);
		}

		[Test]
		public void DeployToBuildServer()
		{
			testDeploy("http://build.tawala.com", "h0nz9irhdmbb6hmxu8go");
		}

		[Test]
		public void DeployToDevServer()
		{
			testDeploy("http://dev.tawala.com", "gl1oqktohc1xnppo4xdl");
		}
	} 
}