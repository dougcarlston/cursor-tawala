using System;
using NUnit.Framework;
using Tawala.FormDesigner;
using TawalaTest.TestSupport;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.XmlSupport;
using Tawala.Interfaces;

namespace TawalaTest.FormDesignerTest.StoryTest
{
	[TestFixture]
	public class StylesInMcqItemsTest2678
	{
		private IFormPresenter presenter;
		private FormView view;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();

			view = new FormView(Project.Current.AddForm());
			TestUtil.PumpMessagesUntilUIReady(view);
			presenter = view.Presenter;
		}

		[TearDown]
		public void TearDown()
		{
			((FormView)view).Close();
			view = null;
            presenter = null;
		}

		private readonly string mcqItemXmlWithVerticalStyle =
			"<mc label=\"Q1\" onlyone=\"true\" required=\"false\" style=\"vertical\">" +
			"<question>" +
			"<paragraph indent=\"0\" align=\"left\">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">" +
			"[Replace this with your question. Use Enter key to add choices below.]" +
			"</font>" +
			"</paragraph>" +
			"</question>" +
			"<choice label=\"a\">" +
			"</choice>" +
			"</mc>" + Environment.NewLine;

		[Test]
		public void DefaultMcqItemHasVerticalStyle()
		{
			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);
			IMcqItem mcqItem = getFormItem<IMcqItem>(0);

			Assert.AreEqual(mcqItemXmlWithVerticalStyle, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void ConstructingMcqItemFromXmlWithNoStyleCreatesMcqItemWithVerticalStyle()
		{
			string mcqItemXmlWithNoStyle =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"false\">" +
				"<question>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				"[Replace this with your question. Use Enter key to add choices below.]" +
				"</font>" +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"</choice>" +
				"</mc>" + Environment.NewLine;

			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqItemXmlWithNoStyle));

			Assert.AreEqual(mcqItemXmlWithVerticalStyle, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void ConstructingMcqItemFromXmlWithVerticalStyleCreatesMcqItemWithVerticalStyle()
		{
			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqItemXmlWithVerticalStyle));

			Assert.AreEqual(mcqItemXmlWithVerticalStyle, mcqItem.ToXml("Q1"));
		}

		private readonly string mcqItemXmlWithHorizontalStyle =
			"<mc label=\"Q1\" onlyone=\"true\" required=\"false\" style=\"horizontal\">" +
			"<question>" +
			"<paragraph indent=\"0\" align=\"left\">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">" +
			"[Replace this with your question. Use Enter key to add choices below.]" +
			"</font>" +
			"</paragraph>" +
			"</question>" +
			"<choice label=\"a\">" +
			"</choice>" +
			"</mc>" + Environment.NewLine;

		[Test]
		public void ConstructingMcqItemFromXmlWithHorizontalStyleCreatesMcqItemWithHorizontalStyle()
		{
			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqItemXmlWithHorizontalStyle));

			Assert.AreEqual(mcqItemXmlWithHorizontalStyle, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void SettingSelectedMcqItemStyleToHorizontalGeneratesProperXml()
		{
			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);

			IMcqItem mcqItem = getFormItem<IMcqItem>(0);
			select(mcqItem);

			setStyleOfSelectedMcqItems("horizontal");

			Assert.AreEqual(mcqItemXmlWithHorizontalStyle, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void SettingSelectedMcqItemStyleToHorizontalThenVerticalGeneratesProperXml()
		{
			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);

			IMcqItem mcqItem = getFormItem<IMcqItem>(0);
			select(mcqItem);

			setStyleOfSelectedMcqItems("horizontal");
			setStyleOfSelectedMcqItems("vertical");

			Assert.AreEqual(mcqItemXmlWithVerticalStyle, mcqItem.ToXml("Q1"));
		}

		private readonly string mcqItemXmlWithMultiColumnStyle =
			"<mc label=\"Q1\" onlyone=\"true\" required=\"false\" style=\"multicolumn\" columnCount=\"3\">" +
			"<question>" +
			"<paragraph indent=\"0\" align=\"left\">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">" +
			"[Replace this with your question. Use Enter key to add choices below.]" +
			"</font>" +
			"</paragraph>" +
			"</question>" +
			"<choice label=\"a\">" +
			"</choice>" +
			"</mc>" + Environment.NewLine;

		[Test]
		public void ConstructingMcqItemFromXmlWithMultiColumnStyleCreatesMcqItemWithMultiColumnStyle()
		{
			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqItemXmlWithMultiColumnStyle));

			Assert.AreEqual(mcqItemXmlWithMultiColumnStyle, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void SettingSelectedMcqItemStyleToMultiColumnGeneratesProperXml()
		{
			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);

			IMcqItem mcqItem = getFormItem<IMcqItem>(0);
			select(mcqItem);

			setStyleOfSelectedMcqItems("multicolumn", 3);

			Assert.AreEqual(mcqItemXmlWithMultiColumnStyle, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void ViewReportsIfAnyMcqItemIsSelected()
		{
			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);
			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);

			IMcqItem mcqItem1 = getFormItem<IMcqItem>(0);
			select(mcqItem1);
			Assert.IsTrue(view.AnyMcqItemSelected(), "Selected mcq item not detected.");

			IFibItem fibItem = getFormItem<IFibItem>(1);
			select(fibItem);
			Assert.IsFalse(view.AnyMcqItemSelected(), "Selected fib item reported as mcq item.");

			IMcqItem mcqItem2 = getFormItem<IMcqItem>(2);
			select(mcqItem2);
			Assert.IsTrue(view.AnyMcqItemSelected(), "Selected mcq item not detected.");
		}

		[Test]
		public void ViewReportsStyleOfSelectedMcqItem()
		{
			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);
			IMcqItem mcqItem1 = getFormItem<IMcqItem>(0);

			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);
			IMcqItem mcqItem2 = getFormItem<IMcqItem>(1);
			select(mcqItem2);
			setStyleOfSelectedMcqItems("horizontal");

			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);
			IMcqItem mcqItem3 = getFormItem<IMcqItem>(2);
			select(mcqItem3);
			setStyleOfSelectedMcqItems("multicolumn");

			select(mcqItem1);
			Assert.AreEqual("vertical", view.GetStyleOfSelectedMcqItem());

			select(mcqItem2);
			Assert.AreEqual("horizontal", view.GetStyleOfSelectedMcqItem());

			select(mcqItem3);
			Assert.AreEqual("multicolumn", view.GetStyleOfSelectedMcqItem());
		}

		[Test]
		public void ApplyingGlobalMcqStyleSetsAllMcqItemStyles()
		{
			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);
			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);
			presenter.InsertMcqItem(FormPresenter.InsertAtEnd);

			presenter.SetStyleOfAllMcqItems("horizontal");

			Assert.AreEqual("horizontal", getFormItem<IMcqItem>(0).Style);
			Assert.AreEqual("horizontal", getFormItem<IMcqItem>(1).Style);
			Assert.AreEqual("horizontal", getFormItem<IMcqItem>(2).Style);
		}

		private void select(IFormItem formItem)
		{
			TestUtil.SelectFormItem(view, formItem);
		}

		private static T getFormItem<T>(int itemIndex) where T : class, IFormItem
		{
			return Project.Current.FormList[0].ItemList[itemIndex] as T;
		}

		private void setStyleOfSelectedMcqItems(string style)
		{
			object[] args = new object[] { style };
			Reflect<FormView>.InvokeMethod("setStyleOfSelectedMcqItems", view, args);
		}

		private void setStyleOfSelectedMcqItems(string style, int columnCount)
		{
			object[] args = new object[] { style, columnCount };
			Reflect<FormView>.InvokeMethod("setStyleOfSelectedMcqItems", view, args);
		}
	}
}
