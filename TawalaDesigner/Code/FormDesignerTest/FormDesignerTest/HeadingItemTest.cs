using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestSupport;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class HeadingItemUITest : UIOrientedTestBase
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
		public void AppendHeadingItem()
		{
			Assert.IsFalse(body.InnerHtml.Contains("<TD"));

			formEditPresenter.InsertHeadingItem(-1);

			VerifyItemTag("HeadingItem");
			Assert.IsNotNull(GetFormItem<IHeadingItem>());
		}

		[Test]
		public void InsertHeadingItem()
		{
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertHeadingItem(1);

			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(IHeadingItem), Project.Current.FormList[0].ItemList[1]);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int headingItemIndex = body.InnerHtml.IndexOf("t:HeadingItem");
			int breakItem2Index = body.InnerHtml.LastIndexOf("t:BreakItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(headingItemIndex > breakItem1Index);
			Assert.IsTrue(headingItemIndex < breakItem2Index);
		}

		[Test]
		public void InsertingHeadingItemAtIndexOfNegativeOneAppends()
		{
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertHeadingItem(-1);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int headingItemIndex = body.InnerHtml.IndexOf("t:HeadingItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(headingItemIndex > breakItem1Index);
		}

		[Test]
		public void InsertedHeadingItemsHaveCorrectDefaultLabels()
		{
			formEditPresenter.InsertHeadingItem(-1);
			formEditPresenter.InsertHeadingItem(0);

			int headingItem1Label = body.InnerHtml.IndexOf("H2</DIV>");
			int headingItem2Label = body.InnerHtml.IndexOf("H1</DIV>");

			Assert.IsTrue(headingItem1Label > 0);
			Assert.IsTrue(headingItem2Label > 0);
			Assert.IsTrue(headingItem1Label > headingItem2Label);
		}

		[Test]
		public void InsertedHeadingItemContainsDefaultText()
		{
			formEditPresenter.InsertHeadingItem(-1);

			Assert.IsTrue(body.InnerHtml.Contains("[Replace this with heading of your own.]"));
		}

		[Test]
		public void HeadingItemGeneratesCorrectXml()
		{
			string headingItemXml =
				"<heading label=\"H1\" type=\"Main\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with heading of your own.]" +
				"</paragraph>" + 
				"</heading>";

			formEditPresenter.InsertHeadingItem(-1);

			string projectXml = Project.Current.ToXml();

			Assert.IsTrue(projectXml.Contains(headingItemXml));
		}

		[Test]
		public void ModifyingHeadingContentsUpdatesProjectHeadingItem()
		{
			string headingItemXml =
				"<heading label=\"H1\" type=\"Main\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"modified text" +
				"</paragraph>" +
				"</heading>";

			formEditPresenter.InsertHeadingItem(-1);
			IHeadingItem headingItem = GetFormItem<IHeadingItem>();

			formView.SetAttribute(headingItem.Id.ToString(), "Contents", "<P style=\"MARGIN-LEFT: 0pt\">modified text</P>");

			string actualXml = Regex.Match(Project.Current.ToXml(), "<heading.*</heading>").Value;

			Assert.AreEqual(headingItemXml, actualXml);
		}
	}

	[TestFixture]
	public class HeadingItemModelTest : ModelOrientedTestBase
	{
		private IHeadingItem headingItem;

		[SetUp]
		public void SetUp()
		{
			headingItem = null;
			ModelTestSetUp();
		}

		[TearDown]
		public void TearDown()
		{
			headingItem = null;
			ModelTestTearDown();
		}

		[Test]
		public void HeadingItemInProjectGeneratesHeadingItemInView()
		{
			headingItem = new NewHeadingItem();
			form.ItemList.Add(headingItem);

			CreateViewFromForm();

			string html = body.InnerHtml;

			VerifyItemTag("HeadingItem");
			Assert.IsTrue(html.Contains("[Replace this with heading of your own.]"));
		}
	}
}
