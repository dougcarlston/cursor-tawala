using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestingSupport;
using Tawala.Projects;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class BreakItemUITest : UIOrientedTestBase
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
		public void AppendBreakItem()
		{
			Assert.IsFalse(body.InnerHtml.Contains("<TD"));

			formEditPresenter.InsertBreakItem(-1);

			VerifyItemTag("BreakItem");

			Assert.IsNotNull(GetFormItem<BreakItem>());
		}

		[Test]
		public void InsertBreakItem()
		{
			formEditPresenter.InsertTextItem(-1);
			formEditPresenter.InsertTextItem(-1);
			formEditPresenter.InsertBreakItem(1);

			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(BreakItem), Project.Current.FormList[0].ItemList[1]);

			int textItem1Index = body.InnerHtml.IndexOf("t:TextItem");
			int breakItemIndex = body.InnerHtml.IndexOf("t:BreakItem");
			int textItem2Index = body.InnerHtml.LastIndexOf("t:TextItem");

			Assert.IsTrue(textItem1Index > -1);
			Assert.IsTrue(breakItemIndex > textItem1Index);
			Assert.IsTrue(breakItemIndex < textItem2Index);
		}

		[Test]
		public void InsertingBreakItemAtIndexOfNegativeOneAppends()
		{
			formEditPresenter.InsertTextItem(-1);
			formEditPresenter.InsertBreakItem(-1);

			int textItemIndex = body.InnerHtml.IndexOf("t:TextItem");
			int breakItemIndex = body.InnerHtml.IndexOf("t:BreakItem");

			Assert.IsTrue(textItemIndex > -1);
			Assert.IsTrue(breakItemIndex > textItemIndex);
		}

		[Test]
		public void BreakItemInViewGeneratesCorrectXml()
		{
			string breakItemXml = "<break/>";

			formEditPresenter.InsertBreakItem(-1);

			string projectXml = Project.Current.ToXml();

			Assert.IsTrue(projectXml.Contains(breakItemXml));
		}
	}

	[TestFixture]
	public class BreakItemModelTest : ModelOrientedTestBase
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
		public void BreakItemInProjectGeneratesPageBreakInView()
		{
			BreakItem breakItem = new BreakItem();
			form.ItemList.Add(breakItem);

			CreateViewFromForm();

			VerifyItemTag("BreakItem");
		}
	}
}
