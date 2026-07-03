using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the BoldText class
	/// </summary>
	[TestFixture]
	public class BoldTextTest
	{
		private BoldText boldText = new BoldText(new XmlElement("<b>Bold text.</b>"));

		[Test]
		public void ConstructFromXml()
		{
			Assert.AreEqual("Bold text.", boldText.Text);
		}

		[Test]
		public void GetXml()
		{
			Assert.AreEqual("<b>Bold text.</b>", boldText.ToXml());
		}

		[Test]
		public void GetHtml()
		{
			Assert.AreEqual("<b>Bold text.</b>", boldText.ToHtml());
		}

		[Test]
		public void GetRtf()
		{
			Assert.AreEqual(@"\b Bold text.\b0 ", boldText.ToRtf());
		}
	}
}
