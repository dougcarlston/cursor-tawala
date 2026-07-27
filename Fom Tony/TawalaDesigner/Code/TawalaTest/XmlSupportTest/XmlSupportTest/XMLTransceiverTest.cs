using System;
using NUnit.Framework;
using XmlSupport;

namespace XmlSupportTest
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
		public void ScissorDeploy() 
		{ 
			// instantiate transceiver
			XMLTransceiver transceiver = new XMLTransceiver ("http://tawala.scissor.com/upload");

			string xmtString =	"<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
				"<project name=\"Hello World\" designer=\"UserName\" format=\"1.1\">" +
				"<forms>" +
				"<form name=\"Hello\">" +
				"<items>" +
				"<text>Hello, World!</text>" +
				"</items>" +
				"</form>" +
				"</forms>" +
				"</project>";
			

			string expString =	"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n\r\n" +
				"<deployment projectName=\"Hello World\" successful=\"TRUE\">\r\n  " +
				"<startpoint url=\"http://tawala.scissor.com/projects/UserName/Hello+World\"/>\r\n" +
				"</deployment>\r\n";

			// send text to URL
			transceiver.Transmit (xmtString);
			
			// receive text from URL
			string rcvString = transceiver.Receive();
			
			//Assertions 
			Assert.AreEqual (expString, rcvString);
		} 
	} 
}
