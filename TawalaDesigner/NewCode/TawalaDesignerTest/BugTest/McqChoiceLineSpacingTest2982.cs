using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;
using TawalaTest.TestingSupport;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class McqChoiceLineSpacingTest2982
	{

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			ComponentMaker.UseNewComponents(true);
		}

		[Test]
		public void ExtraParagraphsAreStrippedFromChoices()
		{
			string classicXml =
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<tabPositions>" +
				@"<tabStop position=""2880""/>" +
				@"</tabPositions>" +
				@"<font face=""Arial"" size=""200"" color=""000000"">One</font>" +
				@"</paragraph>" +
				@"</choice>";

			string newXml =
				@"<choice label=""a"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">One</font>" +
				@"</choice>";

			IChoice choice = new NewXmlChoice(new XmlElement(classicXml));

			Assert.AreEqual(newXml, choice.ToXml("a"));
		}
	}
}
