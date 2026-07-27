using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the UnderlineText class
	/// </summary>
	[TestFixture]
	public class UnderlineTextTest
	{
		private UnderlineText underlineText = new UnderlineText(new XmlElement("<b>Underline text.</b>"));

		[Test]
		public void ConstructFromXml()
		{
			Assert.AreEqual("Underline text.", underlineText.Text);
		}

		[Test]
		public void GetXml()
		{
			Assert.AreEqual("<u>Underline text.</u>", underlineText.ToXml());
		}

		[Test]
		public void GetHtml()
		{
			Assert.AreEqual("<u>Underline text.</u>", underlineText.ToHtml());
		}

		[Test]
		public void GetRtf()
		{
			Assert.AreEqual(@"\ul Underline text.\ul0 ", underlineText.ToRtf(RtfDocument.NULL));
		}
	}
}
