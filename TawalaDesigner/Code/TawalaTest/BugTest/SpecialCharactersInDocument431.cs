using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class SpecialCharactersInDocument431
    {
		private const string NEWLINE = "\r\n";

		[Test]
		public void BackslashCharacterInXmlProducesBackslashCharacterInRtf()
		{
			string xmlString =
				"<document name=\"Document 1\">\r\n" +
				"<xmlData>\r\n" +
				"<paragraph indent=\"0\" align=\"left\">" +
				@"text with backslash \" +
				"</paragraph>" +
				"\r\n</xmlData>\r\n" +
				"</document>";

			IXmlElement element = new XmlElement(xmlString);
			RtfDocument document = new RtfDocument(element);

			string expectedRtf =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard text with backslash \\\par }";

			Assert.AreEqual(expectedRtf, document.ToRtf());
		}

		[Test]
		public void BraceCharactersInXmlProduceBraceCharactersInRtf()
		{
			string xmlString =
				"<document name=\"Document 1\">\r\n" +
				"<xmlData>\r\n" +
				"<paragraph indent=\"0\" align=\"left\">" +
				@"text with {braces}" +
				"</paragraph>" +
				"\r\n</xmlData>\r\n" +
				"</document>";

			IXmlElement element = new XmlElement(xmlString);
			RtfDocument document = new RtfDocument(element);

			string expectedRtf =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard text with \{braces\}\par }";

			Assert.AreEqual(expectedRtf, document.ToRtf());
		}
	}
}