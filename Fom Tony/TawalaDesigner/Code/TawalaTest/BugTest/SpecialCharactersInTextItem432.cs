using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class SpecialCharactersInTextItem432
    {
		private const string defaultTabPositionsString =
				"<tabPositions>" +
				"<tabStop position=\"1134\"/>" +
				"<tabStop position=\"2268\"/>" +
				"<tabStop position=\"3402\"/>" +
				"<tabStop position=\"4536\"/>" +
				"<tabStop position=\"5670\"/>" +
				"<tabStop position=\"6804\"/>" +
				"<tabStop position=\"7938\"/>" +
				"<tabStop position=\"9072\"/>" +
				"<tabStop position=\"10206\"/>" +
				"<tabStop position=\"11340\"/>" +
				"<tabStop position=\"12474\"/>" +
				"<tabStop position=\"13608\"/>" +
				"<tabStop position=\"14742\"/>" +
				"<tabStop position=\"15876\"/>" +
				"</tabPositions>";

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();
		}

		[Test]
		public void BackslashCharacterInXmlProducesBackslashCharacterInRtf()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"text with backslash \\" +
				"</paragraph>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			TextItem textItem = new TextItem(element);

			Assert.AreEqual("text with backslash \\", textItem.Text);

			string expectedRtf =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard text with backslash \\\par }";

			Assert.AreEqual(expectedRtf, textItem.ToRtf());
		}

		[Test]
		public void BraceCharactersInXmlProduceBraceCharactersInRtf()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"text with {braces}" +
				"</paragraph>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			TextItem textItem = new TextItem(element);

			Assert.AreEqual("text with {braces}", textItem.Text);

			string expectedRtf =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard text with \{braces\}\par }";

			Assert.AreEqual(expectedRtf, textItem.ToRtf());
		}
	}
}