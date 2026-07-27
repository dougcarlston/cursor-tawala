using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestSupport;
using Tawala.Proj;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class DeleteItemTest : UIOrientedTestBase
	{
		private BreakItem breakItem;
		private string htmlId;
		private int breakItemId;

		[SetUp]
		public void SetUp()
		{
			breakItem = null;
			UITestSetUp();

			addBreakItem();

			breakItemId = breakItem.Id;
			htmlId = breakItem.Id.ToString();
		}

		[TearDown]
		public void TearDown()
		{
			breakItem = null;
			UITestTearDown();
		}

		private void addBreakItem()
		{
			formEditPresenter.InsertBreakItem(-1);

			breakItem = GetFormItem<BreakItem>();
			Assert.IsNotNull(breakItem);
		}

		[Test]
		public void DeletingItemRemovesItFromModel()
		{
			formEditPresenter.DeleteFormItems(new Collection<string>() { htmlId });

			breakItem = GetFormItem<BreakItem>();
			Assert.IsNull(breakItem);
		}

		[Test]
		public void DeletingSelectedItemRemovesItFromModel()
		{
			HtmlElement customBreakItemElement = document.GetElementById(htmlId);
			HtmlElement parentCell = customBreakItemElement.Parent;
			HtmlElement parentRow = parentCell.Parent;
			parentRow.SetAttribute("Selected", "true");

			formEditPresenter.View.DeleteSelectedItems();

			breakItem = GetFormItem<BreakItem>();
			Assert.IsNull(breakItem);
		}
	}
}
