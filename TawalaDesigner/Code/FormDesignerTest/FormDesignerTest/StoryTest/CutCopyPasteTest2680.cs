using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.FormDesigner;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using TawalaTest.TestSupport;

namespace TawalaTest.FormDesignerTest.StoryTest
{
	[TestFixture]
	public class CutCopyPasteTest2680
	{
		private FormView view;

		private void select(IFormItem formItem)
		{
			TestUtil.SelectFormItem(view, formItem);
		}

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			ComponentMaker.UseNewComponents(true);

			view = new FormView(Project.Current.AddForm());
			TestUtil.PumpMessagesUntilUIReady(view);
		}

		[TearDown]
		public void TearDown()
		{
			((FormView)view).Close();
			view = null;

			ComponentMaker.UseNewComponents(false);
		}

		[Test]
		public void CuttingSelectingFormItemRemovesItFromModel()
		{
			view.Presenter.InsertBreakItem(-1);

			BreakItem breakItem = Project.Current.FormList[0].ItemList[0] as BreakItem;
			select(breakItem);

			view.CutSelectedItems();

			Assert.AreEqual(0, Project.Current.FormList[0].ItemList.Count);
		}

		[Test]
		public void CuttingSelectedFormItemPlacesItOnClipboard()
		{
			view.Presenter.InsertBreakItem(-1);

			BreakItem breakItem = Project.Current.FormList[0].ItemList[0] as BreakItem;
			select(breakItem);

			view.CutSelectedItems();

			IDataObject dataObject = Clipboard.GetDataObject();

			Assert.IsTrue(dataObject.GetDataPresent(typeof(IFormItem[])));
		}

		[Test]
		public void CopyingSelectedFormItemPlacesItOnClipboard()
		{
			view.Presenter.InsertBreakItem(-1);

			BreakItem breakItem = Project.Current.FormList[0].ItemList[0] as BreakItem;
			select(breakItem);

			view.CopySelectedItems();

			IDataObject dataObject = Clipboard.GetDataObject();

			Assert.IsTrue(dataObject.GetDataPresent(typeof(IFormItem[])));
		}

		[Test]
		public void PastingFormItemAddsItToModel()
		{
			view.Presenter.InsertBreakItem(-1);

			BreakItem breakItem = Project.Current.FormList[0].ItemList[0] as BreakItem;
			select(breakItem);

			view.CopySelectedItems();
			view.PasteItems();

			Assert.AreEqual(2, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(BreakItem), Project.Current.FormList[0].ItemList[1]);
		}

		[Test]
		public void PastingFormItemInsertsItBeforeSelectedItem()
		{
			view.Presenter.InsertBreakItem(-1);
			view.Presenter.InsertTextItem(-1);

			BreakItem breakItem = getFormItem<BreakItem>(0);
			ITextItem textItem = getFormItem<ITextItem>(1);

			select(textItem);
			view.CopySelectedItems();

			select(breakItem);
			view.PasteItems();

			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(ITextItem), getFormItem<IFormItem>(0));
			Assert.IsInstanceOfType(typeof(BreakItem), getFormItem<IFormItem>(1));
			Assert.IsInstanceOfType(typeof(ITextItem), getFormItem<IFormItem>(2));
		}

		[Test]
		public void CuttingAndPastingFormItemAppendsItWhenNoItemSelected()
		{
			view.Presenter.InsertTextItem(-1);
			view.Presenter.InsertBreakItem(-1);

			ITextItem textItem = getFormItem<ITextItem>(0);
			BreakItem breakItem = getFormItem<BreakItem>(1);

			select(textItem);
			view.CutSelectedItems();

			view.PasteItems();

			Assert.AreEqual(2, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(BreakItem), getFormItem<IFormItem>(0));
			Assert.IsInstanceOfType(typeof(ITextItem), getFormItem<IFormItem>(1));
		}

		[Test]
		public void CuttingThenPastingTwiceResultsInItemsWithDifferentIds()
		{
			view.Presenter.InsertTextItem(-1);
			ITextItem textItem = getFormItem<ITextItem>(0);
			select(textItem);
			view.CutSelectedItems();

			view.PasteItems();
			view.PasteItems();

			Assert.AreEqual(2, Project.Current.FormList[0].ItemList.Count);
			Assert.AreNotEqual(getFormItem<IFormItem>(0), getFormItem<IFormItem>(1));
			Assert.AreNotEqual(getFormItem<IFormItem>(0).Id, getFormItem<IFormItem>(1).Id);
		}

		[Test]
		public void PastingHiddenFieldResultsInUniqueLabels()
		{
			view.Presenter.InsertFieldItem(-1);
			IHiddenField hiddenFieldItem = getFormItem<IHiddenField>(0);
			select(hiddenFieldItem);
			view.CopySelectedItems();

			view.PasteItems();
			view.PasteItems();

			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.AreEqual("Field3", getFormItem<IHiddenField>(0).FieldName);
			Assert.AreEqual("Field3", getFormItem<IHiddenField>(0).AlternateLabel);

			Assert.AreEqual("Field2", getFormItem<IHiddenField>(1).FieldName);
			Assert.AreEqual("Field2", getFormItem<IHiddenField>(1).AlternateLabel);

			Assert.AreEqual("Field1", getFormItem<IHiddenField>(2).FieldName);
			Assert.AreEqual("Field1", getFormItem<IHiddenField>(2).AlternateLabel);
		}

		[Test]
		public void PastingTextItemWithAlternateLabelResultsInUniqueLabels()
		{
			view.Presenter.InsertTextItem(-1);
			ITextItem textItem = getFormItem<ITextItem>(0);
			textItem.AlternateLabel = "My Text";
			select(textItem);
			view.CopySelectedItems();

			view.PasteItems();
			view.PasteItems();

			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.AreEqual(string.Empty, getFormItem<IFormItem>(0).AlternateLabel);
			Assert.AreEqual("T1", getFormItem<IFormItem>(0).FieldName);

			Assert.AreEqual(string.Empty, getFormItem<IFormItem>(1).AlternateLabel);
			Assert.AreEqual("T2", getFormItem<IFormItem>(1).FieldName);

			Assert.AreEqual("My Text", getFormItem<IFormItem>(2).AlternateLabel);
			Assert.AreEqual("My Text", getFormItem<IFormItem>(2).FieldName);
		}

		[Test]
		public void PastingFibItemWithBlankWithAlternateLabelResultsInUniqueLabels()
		{
			view.Presenter.InsertFibItem(-1);
			IFibItem fibItem = getFormItem<NewFibItem>(0);
			fibItem.BlankList[0].AlternateLabel = "My Blank";
			select(fibItem as IFormItem);
			view.CopySelectedItems();

			view.PasteItems();
			view.PasteItems();

			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);

			Assert.AreEqual(string.Empty, getFormItem<NewFibItem>(0).BlankList[0].AlternateLabel);
			Assert.AreEqual("Q1:a", getFormItem<NewFibItem>(0).BlankList[0].FieldName);

			Assert.AreEqual(string.Empty, getFormItem<NewFibItem>(1).BlankList[0].AlternateLabel);
			Assert.AreEqual("Q2:a", getFormItem<NewFibItem>(1).BlankList[0].FieldName);

			Assert.AreEqual("My Blank", getFormItem<NewFibItem>(2).BlankList[0].AlternateLabel);
			Assert.AreEqual("My Blank", getFormItem<NewFibItem>(2).BlankList[0].FieldName);
		}

		private static T getFormItem<T>(int index) where T : class, IFormItem
		{
			return Project.Current.FormList[0].ItemList[index] as T;
		}
	}
}
