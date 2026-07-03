using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestSupport;
using Tawala.Proj;
using Tawala.FormDesigner.FormItemOptions;
using Tawala.Proj.Forms.NewFormItems;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class McqOptionsUITest : UIOrientedTestBase
	{
		private IMcqItem mcqItem;
		private McqOptionsView optionsView;

		[SetUp]
		public void SetUp()
		{
			UITestSetUp();

			addMcqAndSetupOptionsPanel();
		}

		[TearDown]
		public void TearDown()
		{
			UITestTearDown();
		}

		private void addMcqAndSetupOptionsPanel()
		{
			formEditPresenter.InsertMcqItem(-1);
			mcqItem = GetFormItem<IMcqItem>();

			optionsView = new McqOptionsView(mcqItem);
			formEditPresenter.SetItemOptions(mcqItem.Id.ToString(), optionsView);
		}

		[Test]
		public void SelectMoreThanOneOptionIsReflectedInOptionsPanel()
		{
			Assert.IsFalse(optionsView.SelectMoreThanOne);

			mcqItem.SelectOnlyOne = false;
			formEditPresenter.SetItemOptions(mcqItem.Id.ToString(), optionsView);

			Assert.IsTrue(optionsView.SelectMoreThanOne);
		}

		[Test]
		public void ChangingSelectMoreThanOneOptionUpdatesModel()
		{
			Assert.IsTrue(mcqItem.SelectOnlyOne);

			optionsView.SelectMoreThanOne = true;

			Assert.IsFalse(mcqItem.SelectOnlyOne);
		}

		[Test]
		public void ChangingSelectMoreThanOneOptionUpdatesHtml()
		{
			string html = body.InnerHtml;

			Assert.IsTrue(body.InnerHtml.Contains("<INPUT type=radio"));

			optionsView.SelectMoreThanOne = true;
			formEditPresenter.UpdateMcqOptionsInView(optionsView);

			Assert.IsTrue(body.InnerHtml.Contains("<INPUT type=checkbox"));
		}

		[Test]
		public void SelectMoreThanOneOptionIsRetainedWhenChoicesAreModified()
		{
			string html = body.InnerHtml;
			optionsView.SelectMoreThanOne = true;

			formEditPresenter.UpdateMcqChoicesInView(mcqItem.Id.ToString());

			Assert.IsTrue(body.InnerHtml.Contains("<INPUT type=checkbox"));
		}

		[Test]
		public void ResponseRequiredOptionIsReflectedInOptionsPanel()
		{
			Assert.IsFalse(optionsView.ResponseRequired);

			mcqItem.RequireAtLeastOne = true;
			formEditPresenter.SetItemOptions(mcqItem.Id.ToString(), optionsView);

			Assert.IsTrue(optionsView.ResponseRequired);
		}

		[Test]
		public void ChangingResponseRequiredUpdatesModel()
		{
			Assert.IsFalse(mcqItem.RequireAtLeastOne);

			optionsView.ResponseRequired = true;

			Assert.IsTrue(mcqItem.RequireAtLeastOne);
		}
	}

	[TestFixture]
	public class McqOptionsModelTest : ModelOrientedTestBase
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
		public void SelectMoreThanOneOptionInModelIsReflectedInView()
		{
			IMcqItem mcqItem = new NewMcqItem();
			form.ItemList.Add(mcqItem);

			CreateViewFromForm();

			Assert.IsTrue(body.InnerHtml.Contains("<INPUT type=radio"));

			mcqItem.SelectOnlyOne = false;
			CreateViewFromForm();

			Assert.IsTrue(body.InnerHtml.Contains("<INPUT type=checkbox"));
		}
	}
}