using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class BlankLengthsNotPreservedTest2985
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			ComponentMaker.UseNewComponents(true);
		}

		[TearDown]
		public void TearDown()
		{
			ComponentMaker.UseNewComponents(false);
		}

		[Test]
		public void ChangingLengthOfExistingBlankReplacesBlankInFieldMapById()
		{
			string htmlStringLength20 =
				@"<t:Blank id=""1001"">" +
				@"<input class=""blank"" size=""20"" style=""height:21px;"" value=""Q1:a"" >" +
				@"</t:Blank>";

			new XhtmlBlank(new XhtmlElement(htmlStringLength20, true));

			Assert.AreEqual(20, ((NewBlank)Project.FieldMapById[1001]).Length);

			string htmlStringLength40 =
				@"<t:Blank id=""1001"">" +
				@"<input class=""blank"" size=""40"" style=""height:21px;"" value=""Q1:a"" >" +
				@"</t:Blank>";

			new XhtmlBlank(new XhtmlElement(htmlStringLength40, true));

			Assert.AreEqual(40, ((NewBlank)Project.FieldMapById[1001]).Length);
		}
	}
}
