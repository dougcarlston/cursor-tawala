using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the ItalicText class
	/// </summary>
	[TestFixture]
	public class ItalicTextTest
	{
		private ItalicText italicText = new ItalicText(new XmlElement("<i>Italic text.</i>"));

		[Test]
		public void ConstructFromXml()
		{
			Assert.AreEqual("Italic text.", italicText.Text);
		}

		[Test]
		public void GetXml()
		{
			Assert.AreEqual("<i>Italic text.</i>", italicText.ToXml());
		}

		[Test]
		public void GetHtml()
		{
			Assert.AreEqual("<i>Italic text.</i>", italicText.ToHtml());
		}

		[Test]
		public void GetRtf()
		{
			Assert.AreEqual(@"\i Italic text.\i0 ", italicText.ToRtf(RtfDocument.NULL));
		}
	}
}
