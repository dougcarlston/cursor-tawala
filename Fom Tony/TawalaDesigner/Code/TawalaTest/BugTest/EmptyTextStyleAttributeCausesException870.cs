using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using TawalaTest.TestSupport;
using Tawala.XmlSupport;


namespace TawalaTest.BugTest
{
	[TestFixture]
	public class EmptyTextStyleAttributeCausesException870
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		private readonly string xmlStringWithNoStyleAttribute =
			@"<text label=""T1"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			XmlConstants.DefaultTabsXml +
			XmlConstants.DefaultBeginFont +
			@"[Replace this with text of your own.]" +
			XmlConstants.EndFont +
			@"</paragraph>" +
			@"</text>" +
			Environment.NewLine;

		private readonly string xmlStringWithEmptyStyleAttribute =
			@"<text label=""T1"" style="""">" +
			@"<paragraph indent=""0"" align=""left"">" +
			XmlConstants.DefaultTabsXml +
			XmlConstants.DefaultBeginFont +
			@"[Replace this with text of your own.]" +
			XmlConstants.EndFont +
			@"</paragraph>" +
			@"</text>" +
			Environment.NewLine;

		private readonly string xmlStringWithNormalStyleAttribute =
			@"<text label=""T1"" style=""normal"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			XmlConstants.DefaultTabsXml +
			XmlConstants.DefaultBeginFont +
			@"[Replace this with text of your own.]" +
			XmlConstants.EndFont +
			@"</paragraph>" +
			@"</text>" +
			Environment.NewLine;

		[Test]
		public void XmlWithNoStyleAttributeProducesXmlWithNormalStyleAttribute()
		{
			TextItem textItem = new TextItem(new XmlElement(xmlStringWithNoStyleAttribute));

			Assert.AreEqual(xmlStringWithNormalStyleAttribute, textItem.ToXml("T1"));
		}

		[Test]
		public void XmlWithEmptyStyleAttributeProducesXmlWithNormalStyleAttribute()
		{
			TextItem textItem = new TextItem(new XmlElement(xmlStringWithEmptyStyleAttribute));

			Assert.AreEqual(xmlStringWithNormalStyleAttribute, textItem.ToXml("T1"));
		}
	}
}
