using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using TawalaTest.TestingSupport;
using NUnit.Framework;

namespace TawalaTest.ProjectTest.Forms.FormItemContents
{
	[TestFixture]
	public class NewSkipInstructionsTest
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
		public void CanConstructSkipInstructions()
		{
			IFormItemContents skipInstructions = new NewSkipInstructions();
		}
	}
}
