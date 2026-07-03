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
	public class ItemInsertionPointUITest : UIOrientedTestBase
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
		public void InsertTextItemAtInsertionPoint()
		{
			formEditPresenter.InsertBreakItem(FormPresenter.InsertAtEnd);
			formEditPresenter.InsertBreakItem(FormPresenter.InsertAtEnd);
			formEditPresenter.InsertTextItem(FormPresenter.InsertAtInsertionPoint);

			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(ITextItem), Project.Current.FormList[0].ItemList[0]);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int textItemIndex = body.InnerHtml.IndexOf("t:TextItem");
			int breakItem2Index = body.InnerHtml.LastIndexOf("t:BreakItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(breakItem2Index > breakItem1Index);
			Assert.IsTrue(textItemIndex < breakItem1Index);
		}
	}
}
