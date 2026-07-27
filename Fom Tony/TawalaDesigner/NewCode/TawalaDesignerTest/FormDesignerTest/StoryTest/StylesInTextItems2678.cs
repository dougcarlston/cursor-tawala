using System;
using NUnit.Framework;
using Tawala.FormDesigner;
using TawalaTest.TestingSupport;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using Tawala.Interfaces;

namespace TawalaTest.FormDesignerTest.StoryTest
{
	[TestFixture]
	public class StylesInTextItems2678
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

		private readonly string textItemXmlWithNormalStyle =
			@"<text label=""T1"" style=""normal"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"[Replace this with text of your own.]" +
			@"</paragraph></text>" + Environment.NewLine;

		[Test]
		public void DefaultTextItemHasNormalStyle()
		{
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);
			ITextItem textItem = getFormItem<ITextItem>(0);

			Assert.AreEqual(textItemXmlWithNormalStyle, textItem.ToXml("T1"));
		}

		[Test]
		public void ConstructingTextItemFromXmlWithNoStyleCreatesTextItemWithNormalStyle()
		{
			string textItemXmlWithNoStyle =
				@"<text label=""T1"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"[Replace this with text of your own.]" +
				@"</paragraph></text>" + Environment.NewLine;

			ITextItem textItem = new NewTextItem(new XmlElement(textItemXmlWithNoStyle));

			Assert.AreEqual(textItemXmlWithNormalStyle, textItem.ToXml("T1"));
		}

		[Test]
		public void ConstructingTextItemFromXmlWithNormalStyleCreatesTextItemWithNormalStyle()
		{
			ITextItem textItem = new NewTextItem(new XmlElement(textItemXmlWithNormalStyle));

			Assert.AreEqual(textItemXmlWithNormalStyle, textItem.ToXml("T1"));
		}

		private readonly string textItemXmlWithInstructionalStyle =
			@"<text label=""T1"" style=""instructional"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"[Replace this with text of your own.]" +
			@"</paragraph></text>" + Environment.NewLine;

		[Test]
		public void ConstructingTextItemFromXmlWithInstructionalStyleCreatesTextItemWithInstructionalStyle()
		{
			ITextItem textItem = new NewTextItem(new XmlElement(textItemXmlWithInstructionalStyle));

			Assert.AreEqual(textItemXmlWithInstructionalStyle, textItem.ToXml("T1"));
		}

		[Test]
		public void SettingSelectedTextItemStyleToInstructionalGeneratesProperXml()
		{
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);

			ITextItem textItem = getFormItem<ITextItem>(0);
			select(textItem);

			setStyleOfSelectedTextItems("instructional");

			Assert.AreEqual(textItemXmlWithInstructionalStyle, textItem.ToXml("T1"));
		}

		[Test]
		public void SettingSelectedTextItemStyleToInstructionalThenNormalGeneratesProperXml()
		{
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);

			ITextItem textItem = getFormItem<ITextItem>(0);
			select(textItem);

			setStyleOfSelectedTextItems("instructional");
			setStyleOfSelectedTextItems("normal");

			Assert.AreEqual(textItemXmlWithNormalStyle, textItem.ToXml("T1"));
		}

		private readonly string textItemXmlWithErrorStyle =
			@"<text label=""T1"" style=""error"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"[Replace this with text of your own.]" +
			@"</paragraph></text>" + Environment.NewLine;

		[Test]
		public void ConstructingTextItemFromXmlWithErrorStyleCreatesTextItemWithErrorStyle()
		{
			ITextItem textItem = new NewTextItem(new XmlElement(textItemXmlWithErrorStyle));

			Assert.AreEqual(textItemXmlWithErrorStyle, textItem.ToXml("T1"));
		}

		[Test]
		public void SettingSelectedTextItemStyleToErrorGeneratesProperXml()
		{
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);

			ITextItem textItem = getFormItem<ITextItem>(0);
			select(textItem);

			setStyleOfSelectedTextItems("error");

			Assert.AreEqual(textItemXmlWithErrorStyle, textItem.ToXml("T1"));
		}

		[Test]
		public void ViewReportsIfAnyTextItemIsSelected()
		{
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);

			ITextItem textItem1 = getFormItem<ITextItem>(0);
			select(textItem1);
			Assert.IsTrue(view.AnyTextItemSelected(), "Selected text item not detected.");

			IFibItem fibItem = getFormItem<IFibItem>(1);
			select(fibItem);
			Assert.IsFalse(view.AnyTextItemSelected(), "Selected fib item reported as text item.");

			ITextItem textItem2 = getFormItem<ITextItem>(2);
			select(textItem2);
			Assert.IsTrue(view.AnyTextItemSelected(), "Selected text item not detected.");
		}

		[Test]
		// REVISIT: Incomplete test because we currently have no way to do multiple selection in a test. - jdf 6/4/08
		public void ViewReportsIfOnlyOneTextItemIsSelected()
		{
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);
			presenter.InsertFibItem(FormPresenter.InsertAtEnd);
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);

			ITextItem textItem1 = getFormItem<ITextItem>(0);
			select(textItem1);
			Assert.IsTrue(view.OnlyOneTextItemSelected(), "Single selected text item not detected.");

			IFibItem fibItem = getFormItem<IFibItem>(1);
			select(fibItem);
			Assert.IsFalse(view.OnlyOneTextItemSelected(), "Single selected fib item reported as single selected text item.");
		}

		[Test]
		public void ViewReportsStyleOfSelectedTextItem()
		{
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);
			ITextItem textItem1 = getFormItem<ITextItem>(0);

			presenter.InsertTextItem(FormPresenter.InsertAtEnd);
			ITextItem textItem2 = getFormItem<ITextItem>(1);
			select(textItem2);
			setStyleOfSelectedTextItems("error");

			presenter.InsertTextItem(FormPresenter.InsertAtEnd);
			ITextItem textItem3 = getFormItem<ITextItem>(2);
			select(textItem3);
			setStyleOfSelectedTextItems("instructional");

			select(textItem1);
			Assert.AreEqual("normal", view.GetStyleOfSelectedTextItem());

			select(textItem2);
			Assert.AreEqual("error", view.GetStyleOfSelectedTextItem());

			select(textItem3);
			Assert.AreEqual("instructional", view.GetStyleOfSelectedTextItem());
		}

		[Test]
		public void ApplyingGlobalTextStyleSetsAllTextItemStyles()
		{
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);
			presenter.InsertTextItem(FormPresenter.InsertAtEnd);

			presenter.SetStyleOfAllTextItems("instructional");

			Assert.AreEqual("instructional", getFormItem<ITextItem>(0).Style);
			Assert.AreEqual("instructional", getFormItem<ITextItem>(1).Style);
			Assert.AreEqual("instructional", getFormItem<ITextItem>(2).Style);
		}

		private void select(IFormItem formItem)
		{
			TestUtil.SelectFormItem(view, formItem);
		}

		private static T getFormItem<T>(int itemIndex) where T : class, IFormItem
		{
			return Project.Current.FormList[0].ItemList[itemIndex] as T;
		}

		private void setStyleOfSelectedTextItems(string style)
		{
			object[] args = new object[] { style };
			Reflect<FormView>.InvokeMethod("setStyleOfSelectedTextItems", view, args);
		}
	}
}
