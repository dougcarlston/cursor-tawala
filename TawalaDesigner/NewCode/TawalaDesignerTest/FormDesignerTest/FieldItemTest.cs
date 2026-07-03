using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestingSupport;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class FieldItemUITest : UIOrientedTestBase
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
		public void AppendFieldItem()
		{
			Assert.IsFalse(body.InnerHtml.Contains("<TD"));

			formEditPresenter.InsertFieldItem(-1);

			VerifyItemTag("FieldItem");

			Assert.IsNotNull(GetFormItem<IHiddenField>());
		}

		[Test]
		public void InsertFieldItem()
		{
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertFieldItem(1);

			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(IHiddenField), Project.Current.FormList[0].ItemList[1]);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int fieldItemIndex = body.InnerHtml.IndexOf("t:FieldItem");
			int breakItem2Index = body.InnerHtml.LastIndexOf("t:BreakItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(fieldItemIndex > breakItem1Index);
			Assert.IsTrue(fieldItemIndex < breakItem2Index);
		}

		[Test]
		public void InsertingFieldItemAtIndexOfNegativeOneAppends()
		{
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertFieldItem(-1);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int fieldItemIndex = body.InnerHtml.IndexOf("t:FieldItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(fieldItemIndex > breakItem1Index);
		}

		[Test]
		public void InsertedFieldItemHasDefaultName()
		{
			formEditPresenter.InsertFieldItem(-1);

			Assert.IsTrue(body.InnerHtml.Contains("Field1"));
		}

		[Test]
		public void FieldItemGeneratesCorrectXml()
		{
			string fieldItemXml =
				"<field name=\"Field1\"/>";

			formEditPresenter.InsertFieldItem(-1);

			string projectXml = Project.Current.ToXml();

			Assert.IsTrue(projectXml.Contains(fieldItemXml));
		}

		[Test]
		public void ModifyingFieldUpdatesProjectFieldItem()
		{
			string fieldItemXml =
				"<field name=\"FieldNewName\"/>";

			formEditPresenter.InsertFieldItem(-1);
			IHiddenField fieldItem = GetFormItem<IHiddenField>();

			formView.SetAttribute(fieldItem.Id.ToString(), "FieldName", "FieldNewName");

			string projectXml = Project.Current.ToXml();

			Assert.IsTrue(projectXml.Contains(fieldItemXml));
		}
	}

	[TestFixture]
	public class FieldItemModelTest : ModelOrientedTestBase
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
		public void FieldItemInProjectGeneratesFieldItemInView()
		{
			IHiddenField fieldItem = new NewHiddenField();
			form.ItemList.Add(fieldItem);

			CreateViewFromForm();

			string html = body.InnerHtml;

			VerifyItemTag("FieldItem");
			Assert.IsTrue(html.Contains(fieldItem.Name));
		}
	}
}
