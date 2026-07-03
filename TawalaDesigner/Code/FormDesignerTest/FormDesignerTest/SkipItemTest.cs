using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestSupport;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class SkipItemUITest : UIOrientedTestBase
	{
		[SetUp]
		public void SetUp()
		{
			UITestSetUp();
		}

		[TearDown]
		public void TearDown()
		{
			UITestTearDown();
		}

		[Test]
		public void AppendSkipItem()
		{
			Assert.IsFalse(body.InnerHtml.Contains("<TD"));

			formEditPresenter.InsertSkipItem(-1);

			VerifyItemTag("SkipItem");

			Assert.IsNotNull(GetFormItem<ISkipInstructionsItem>());
		}

		[Test]
		public void InsertSkipItem()
		{
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertSkipItem(1);

			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(ISkipInstructionsItem), Project.Current.FormList[0].ItemList[1]);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int skipItemIndex = body.InnerHtml.IndexOf("t:SkipItem");
			int breakItem2Index = body.InnerHtml.LastIndexOf("t:BreakItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(skipItemIndex > breakItem1Index);
			Assert.IsTrue(skipItemIndex < breakItem2Index);
		}

		[Test]
		public void InsertingSkipItemAtIndexOfNegativeOneAppends()
		{
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertSkipItem(-1);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int skipItemIndex = body.InnerHtml.IndexOf("t:SkipItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(skipItemIndex > breakItem1Index);
		}

		[Test]
		public void SkipItemInViewGeneratesCorrectXml()
		{
			string skipItemXml = 
				"<skipInstructions>" + Environment.NewLine +
				"</skipInstructions>";

			formEditPresenter.InsertSkipItem(-1);

			string projectXml = Project.Current.ToXml();

			Assert.IsTrue(projectXml.Contains(skipItemXml));
		}
	}

	[TestFixture]
	public class SkipItemModelTest : ModelOrientedTestBase
	{
		[SetUp]
		public void SetUp()
		{
			ModelTestSetUp();
		}

		[TearDown]
		public void TearDown()
		{
			ModelTestTearDown();
		}

		[Test]
		public void SkipItemInProjectGeneratesSkipInView()
		{
			ISkipInstructionsItem skipItem = new NewSkipInstructionsItem();
			form.ItemList.Add(skipItem);

			CreateViewFromForm();

			VerifyItemTag("SkipItem");
		}
	}
}
