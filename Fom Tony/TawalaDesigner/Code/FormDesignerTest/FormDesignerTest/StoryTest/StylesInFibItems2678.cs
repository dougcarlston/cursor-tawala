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
	public class StylesInFibItems2678
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

		private readonly string fibItemXmlWithTopLabelsStyle =
			@"<fib label=""Q1"" style=""topLabels"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"[Replace this with your question. Underscores create blanks.] " +
			@"<blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
			@"</paragraph>" +
			@"</fib>" + Environment.NewLine;

		[Test]
		public void DefaultFibItemHasTopLabelsStyle()
		{
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);
			IFibItem fibItem = getFormItem<IFibItem>(0);

			Assert.AreEqual(fibItemXmlWithTopLabelsStyle, fibItem.ToXml("Q1"));
		}

		[Test]
		public void ConstructingFibItemFromXmlWithNoStyleCreatesFibItemWithTopLabelsStyle()
		{
			string fibItemXmlWithNoStyle =
				@"<fib label=""Q1"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"[Replace this with your question. Underscores create blanks.] " +
				@"<blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
				@"</paragraph>" +
				@"</fib>" + Environment.NewLine;

			IFibItem fibItem = new NewFibItem(new XmlElement(fibItemXmlWithNoStyle));

			Assert.AreEqual(fibItemXmlWithTopLabelsStyle, fibItem.ToXml("Q1"));
		}

		[Test]
		public void ConstructingFibItemFromXmlWithTopLabelsStyleCreatesFibItemWithTopLabelsStyle()
		{
			IFibItem fibItem = new NewFibItem(new XmlElement(fibItemXmlWithTopLabelsStyle));

			Assert.AreEqual(fibItemXmlWithTopLabelsStyle, fibItem.ToXml("Q1"));
		}

		private readonly string fibItemXmlWithLeftAlignLabelsStyle =
			@"<fib label=""Q1"" style=""leftAlignLabels"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"[Replace this with your question. Underscores create blanks.] " +
			@"<blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
			@"</paragraph>" +
			@"</fib>" + Environment.NewLine;

		[Test]
		public void ConstructingFibItemFromXmlWithLeftAlignLabelsStyleCreatesFibItemWithLeftAlignLabelsStyle()
		{
			IFibItem fibItem = new NewFibItem(new XmlElement(fibItemXmlWithLeftAlignLabelsStyle));

			Assert.AreEqual(fibItemXmlWithLeftAlignLabelsStyle, fibItem.ToXml("Q1"));
		}

		[Test]
		public void SettingSelectedFibItemStyleToLeftAlignLabelsStyleGeneratesProperXml()
		{
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);

			IFibItem fibItem = getFormItem<IFibItem>(0);
			select(fibItem);

			setStyleOfSelectedFibItems("leftAlignLabels");

			Assert.AreEqual(fibItemXmlWithLeftAlignLabelsStyle, fibItem.ToXml("Q1"));
		}

		[Test]
		public void SettingSelectedFibItemStyleToLeftAlignLabelsStyleThenTopLabelStyleGeneratesProperXml()
		{
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);

			IFibItem fibItem = getFormItem<IFibItem>(0);
			select(fibItem);

			setStyleOfSelectedFibItems("leftAlignLabels");
			setStyleOfSelectedFibItems("topLabels");

			Assert.AreEqual(fibItemXmlWithTopLabelsStyle, fibItem.ToXml("Q1"));
		}

		private readonly string fibItemXmlWithRightAlignLabelsStyle =
			@"<fib label=""Q1"" style=""rightAlignLabels"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"[Replace this with your question. Underscores create blanks.] " +
			@"<blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
			@"</paragraph>" +
			@"</fib>" + Environment.NewLine;

		[Test]
		public void ConstructingFibItemFromXmlWithRightAlignLabelsStyleCreatesFibItemWithRightAlignLabelsStyle()
		{
			IFibItem fibItem = new NewFibItem(new XmlElement(fibItemXmlWithRightAlignLabelsStyle));

			Assert.AreEqual(fibItemXmlWithRightAlignLabelsStyle, fibItem.ToXml("Q1"));
		}

		[Test]
		public void SettingSelectedFibItemStyleToRightAlignLabelsStyleGeneratesProperXml()
		{
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);

			IFibItem fibItem = getFormItem<IFibItem>(0);
			select(fibItem);

			setStyleOfSelectedFibItems("rightAlignLabels");

			Assert.AreEqual(fibItemXmlWithRightAlignLabelsStyle, fibItem.ToXml("Q1"));
		}

		private readonly string fibItemXmlWithLeftAlignLabelsJustifiedStyle =
			@"<fib label=""Q1"" style=""leftAlignLabelsJustified"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"[Replace this with your question. Underscores create blanks.] " +
			@"<blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
			@"</paragraph>" +
			@"</fib>" + Environment.NewLine;

		[Test]
		public void ConstructingFibItemFromXmlWithLeftAlignLabelsJustifiedStyleCreatesFibItemWithLeftAlignLabelsJustifiedStyle()
		{
			IFibItem fibItem = new NewFibItem(new XmlElement(fibItemXmlWithLeftAlignLabelsJustifiedStyle));

			Assert.AreEqual(fibItemXmlWithLeftAlignLabelsJustifiedStyle, fibItem.ToXml("Q1"));
		}

		[Test]
		public void SettingSelectedFibItemStyleToLeftAlignLabelsJustifiedStyleGeneratesProperXml()
		{
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);

			IFibItem fibItem = getFormItem<IFibItem>(0);
			select(fibItem);

			setStyleOfSelectedFibItems("leftAlignLabelsJustified");

			Assert.AreEqual(fibItemXmlWithLeftAlignLabelsJustifiedStyle, fibItem.ToXml("Q1"));
		}

		private readonly string fibItemXmlWithRightAlignLabelsJustifiedStyle =
			@"<fib label=""Q1"" style=""rightAlignLabelsJustified"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"[Replace this with your question. Underscores create blanks.] " +
			@"<blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
			@"</paragraph>" +
			@"</fib>" + Environment.NewLine;

		[Test]
		public void ConstructingFibItemFromXmlWithRightAlignLabelsJustifiedStyleCreatesFibItemWithRightAlignLabelsJustifiedStyle()
		{
			IFibItem fibItem = new NewFibItem(new XmlElement(fibItemXmlWithRightAlignLabelsJustifiedStyle));

			Assert.AreEqual(fibItemXmlWithRightAlignLabelsJustifiedStyle, fibItem.ToXml("Q1"));
		}

		[Test]
		public void SettingSelectedFibItemStyleToRightAlignLabelsJustifiedStyleGeneratesProperXml()
		{
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);

			IFibItem fibItem = getFormItem<IFibItem>(0);
			select(fibItem);

			setStyleOfSelectedFibItems("rightAlignLabelsJustified");

			Assert.AreEqual(fibItemXmlWithRightAlignLabelsJustifiedStyle, fibItem.ToXml("Q1"));
		}

		private readonly string fibItemXmlWithFreeformStyle =
			@"<fib label=""Q1"" style=""freeform"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"[Replace this with your question. Underscores create blanks.] " +
			@"<blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
			@"</paragraph>" +
			@"</fib>" + Environment.NewLine;

		[Test]
		public void ConstructingFibItemFromXmlWithFreeformStyleCreatesFibItemWithFreeformStyle()
		{
			IFibItem fibItem = new NewFibItem(new XmlElement(fibItemXmlWithFreeformStyle));

			Assert.AreEqual(fibItemXmlWithFreeformStyle, fibItem.ToXml("Q1"));
		}

		[Test]
		public void SettingSelectedFibItemStyleToFreeformStyleGeneratesProperXml()
		{
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);

			IFibItem fibItem = getFormItem<IFibItem>(0);
			select(fibItem);

			setStyleOfSelectedFibItems("freeform");

			Assert.AreEqual(fibItemXmlWithFreeformStyle, fibItem.ToXml("Q1"));
		}

		[Test]
		public void ViewReportsIfAnyFibItemIsSelected()
		{
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);

			IFibItem fibItem1 = getFormItem<IFibItem>(0);
			select(fibItem1);
			Assert.IsTrue(view.AnyFibItemSelected(), "Selected FIB item not detected.");

			ITextItem textItem = getFormItem<ITextItem>(1);
			select(textItem);
			Assert.IsFalse(view.AnyFibItemSelected(), "Selected text item reported as FIB item.");

			IFibItem fibItem2 = getFormItem<IFibItem>(2);
			select(fibItem2);
			Assert.IsTrue(view.AnyFibItemSelected(), "Selected FIB item not detected.");
		}

		[Test]
		[Ignore("Incomplete test because we currently have no way to do multiple selection in a test. - jdf 6/6/08")]
		public void ViewReportsIfOnlyOneFibItemIsSelected()
		{
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);

			IFibItem fibItem1 = getFormItem<IFibItem>(0);
			select(fibItem1);
			Assert.IsTrue(view.OnlyOneFibItemSelected(), "Single selected text item not detected.");

			ITextItem textItem = getFormItem<ITextItem>(1);
			select(textItem);
			Assert.IsFalse(view.OnlyOneFibItemSelected(), "Single selected fib item reported as single selected text item.");
		}

		[Test]
		[Ignore ("Incomplete test because we currently have no way to do multiple selection in a test. - jdf 6/6/08")]
		public void ViewReportsIfAllSelectedFibItemsHaveSameStyle()
		{
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);

			IFibItem fibItem1 = getFormItem<IFibItem>(0);
			select(fibItem1);
			setStyleOfSelectedFibItems("rightAlignLabelsJustified");

			IFibItem fibItem2 = getFormItem<IFibItem>(1);
			select(fibItem2);

			Assert.IsFalse(view.SelectedFibItemsHaveSameStyle(), "Selected FIB items have same style.");

			setStyleOfSelectedFibItems("rightAlignLabelsJustified");

			Assert.IsTrue(view.SelectedFibItemsHaveSameStyle(), "Selected FIB items do not have same style.");
		}

		[Test]
		public void ViewReportsStyleOfSelectedFibItem()
		{
            presenter.InsertFibItem(FormPresenter.InsertAtEnd);
            IFibItem fibItem1 = getFormItem<IFibItem>(0);

            presenter.InsertFibItem(FormPresenter.InsertAtEnd);
            IFibItem fibItem2 = getFormItem<IFibItem>(1);
            select(fibItem2);
			setStyleOfSelectedFibItems("leftAlignLabels");

			presenter.InsertFibItem(FormPresenter.InsertAtEnd);
			IFibItem fibItem3 = getFormItem<IFibItem>(2);
			select(fibItem3);
			setStyleOfSelectedFibItems("freeform");

			select(fibItem1);
			Assert.AreEqual("topLabels", view.GetStyleOfFirstSelectedFibItem());

			select(fibItem2);
			Assert.AreEqual("leftAlignLabels", view.GetStyleOfFirstSelectedFibItem());

			select(fibItem3);
			Assert.AreEqual("freeform", view.GetStyleOfFirstSelectedFibItem());
		}

		[Test]
		public void ApplyingGlobalFibStyleSetsAllFibItemStyles()
		{
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);

			presenter.SetStyleOfAllFibItems("leftAlignLabels");

			Assert.AreEqual("leftAlignLabels", getFormItem<IFibItem>(0).Style);
			Assert.AreEqual("leftAlignLabels", getFormItem<IFibItem>(1).Style);
			Assert.AreEqual("leftAlignLabels", getFormItem<IFibItem>(2).Style);
		}

		private void select(IFormItem formItem)
		{
			TestUtil.SelectFormItem(view, formItem);
		}

		private static T getFormItem<T>(int itemIndex) where T : class, IFormItem
		{
			return Project.Current.FormList[0].ItemList[itemIndex] as T;
		}

		private void setStyleOfSelectedFibItems(string style)
		{
			object[] args = new object[] { style };
			Reflect<FormView>.InvokeMethod("setStyleOfSelectedFibItems", view, args);
		}
	}
}
