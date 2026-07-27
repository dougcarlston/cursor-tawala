using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using NUnit.Framework;

namespace TawalaTest.FrameworkBehaviorTests
{
	[TestFixture]
	public class XmlDocumentTest
	{
		private const string xmlTestWithSpaces = "<doc> <spaces>   </spaces><stuff>  stuff  </stuff>  </doc>";

		private XmlDocument xmlDoc = null;

		[SetUp]
		public void Setup()
		{
			xmlDoc = new XmlDocument();
		}

		[Test]
		public void PreserveWhitespaceTrue()
		{
			xmlDoc.PreserveWhitespace = true;
			xmlDoc.LoadXml(xmlTestWithSpaces);

			string tempPath = Path.GetTempFileName();
			xmlDoc.Save(tempPath);

			string text = File.ReadAllText(tempPath);
			File.Delete(tempPath);

			Assert.AreEqual(xmlTestWithSpaces, text);
		}

		[Test]
		public void PreserveWhitespaceFalse()
		{
			const string expectedXml = "<doc>\r\n  <spaces>\r\n  </spaces>\r\n  <stuff>  stuff  </stuff>\r\n</doc>";

			xmlDoc.PreserveWhitespace = false;
			xmlDoc.LoadXml(xmlTestWithSpaces);

			string tempPath = Path.GetTempFileName();
			xmlDoc.Save(tempPath);

			string text = File.ReadAllText(tempPath);
			File.Delete(tempPath);

			Assert.AreEqual(expectedXml, text);
		}

		[Test]
		public void PreserveWhitespaceTrueSelectWhitespaceNodes()
		{
			xmlDoc.PreserveWhitespace = true;
			xmlDoc.LoadXml(xmlTestWithSpaces);

			System.Xml.XmlNodeList nodes = xmlDoc.SelectNodes("//text()");

			Assert.AreEqual(4, nodes.Count);
			Assert.AreEqual(" ", nodes[0].Value);
			Assert.AreEqual("   ", nodes[1].Value);
			Assert.AreEqual("  stuff  ", nodes[2].Value);
			Assert.AreEqual("  ", nodes[3].Value);
		}

		[Test]
		public void PreserveWhitespaceFalseSelectWhitespaceNodes()
		{
			xmlDoc.PreserveWhitespace = false;
			xmlDoc.LoadXml(xmlTestWithSpaces);

			System.Xml.XmlNodeList nodes = xmlDoc.SelectNodes("//text()");

			Assert.AreEqual(1, nodes.Count);
			Assert.AreEqual("  stuff  ", nodes[0].Value);
		}
	}
}
