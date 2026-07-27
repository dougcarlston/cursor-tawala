using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.FormDesigner;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;
using TawalaTest.TestingSupport;
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest.StoryTest
{
	[TestFixture]
	public class CutCopyPasteTest2681
	{
		private FormView view1;
		private FormView view2;
		private WebBrowser browser1;
		private WebBrowser browser2;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			
			view1 = new FormView(Project.Current.AddForm());
			browser1 = TestUtil.PumpMessagesUntilUIReady(view1);

			view2 = new FormView(Project.Current.AddForm());
			browser2 = TestUtil.PumpMessagesUntilUIReady(view2);
		}

		[TearDown]
		public void TearDown()
		{
			browser1 = null;
			((FormView)view1).Close();
			view1 = null;

			browser2 = null;
			((FormView)view2).Close();
			view2 = null;
		}

		[Test]
		public void PastingCopiedBreakItemInSecondFormResultsInBreakItem()
		{
			view1.Presenter.InsertBreakItem(-1);

			BreakItem breakItem = getFormItem<BreakItem>(0, 0);
			select(breakItem);

			view1.CopySelectedItems();
			view2.PasteItems();

			Assert.AreEqual(1, getForm(1).ItemList.Count);
			Assert.IsInstanceOfType(typeof(BreakItem), getFormItem<BreakItem>(1, 0));
		}

		[Test]
		public void PastingCopiedFibItemInSecondFormResultsInCompleteFibItem()
		{
			view1.Presenter.InsertFibItem(-1);

			NewFibItem fibItem = getFormItem<NewFibItem>(0, 0);
			select(fibItem);

			view1.CopySelectedItems();
			Assert.AreEqual(1, Project.Current.FormList[0].ItemList.Count);

			view2.PasteItems();

			Assert.IsInstanceOfType(typeof(NewFibItem), getFormItem<NewFibItem>(1, 0));
			Assert.AreEqual(1, getFormItem<NewFibItem>(1, 0).BlankList.Count);
		}

		[Test]
		public void PastingCutFibItemInSecondFormResultsInCompleteFibItem()
		{
			view1.Presenter.InsertFibItem(-1);

			NewFibItem fibItem = getFormItem<NewFibItem>(0, 0);
			select(fibItem);

			view1.CutSelectedItems();
			Assert.AreEqual(0, Project.Current.FormList[0].ItemList.Count);

			view2.PasteItems();

			Assert.IsInstanceOfType(typeof(NewFibItem), getFormItem<NewFibItem>(1, 0));
			Assert.AreEqual(1, getFormItem<NewFibItem>(1, 0).BlankList.Count);
		}

		[Test]
		public void PastingCopiedMcqItemInSecondFormResultsInCompleteMcqItem()
		{
			string mcqItemXml =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"false\" style=\"vertical\">" +
				"<question><paragraph indent=\"0\" align=\"left\">" +
				"New question text:" +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"One" +
				"</paragraph>" +
				"</choice>" +
				"<choice label=\"b\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Two" +
				"</paragraph>" +
				"</choice>" +
				"</mc>";

			addMcqItemToForm1(mcqItemXml);
			IMcqItem mcqItem = getFormItem<IMcqItem>(0, 0);
			select(mcqItem);

			view1.CopySelectedItems();
			view2.PasteItems();

			Assert.IsInstanceOfType(typeof(IMcqItem), getFormItem<IMcqItem>(1, 0));
			Assert.AreEqual(2, getFormItem<IMcqItem>(1, 0).Choices.Count);
			Assert.AreEqual("One", getFormItem<IMcqItem>(1, 0).Choices[0].Text);
			Assert.AreEqual("Two", getFormItem<IMcqItem>(1, 0).Choices[1].Text);
		}

		[Test]
		public void PastingCopiedFormattedTextItemInSecondFormResultsInFormattedTextItem()
		{
			string textItemXml = @"<text label=""T1"" style=""normal""><paragraph indent=""0"" align=""left""><font size=""320"">My Text</font></paragraph></text>" + Environment.NewLine;
			addTextItemToForm1(textItemXml);

			NewTextItem textItem = getFormItem<NewTextItem>(0, 0);
			select(textItem);

			view1.CopySelectedItems();
			view2.PasteItems();

			Assert.IsInstanceOfType(typeof(NewTextItem), getFormItem<NewTextItem>(1, 0));
			NewTextItem copiedTextItem = getFormItem<NewTextItem>(1, 0);
			Assert.AreEqual(textItemXml, copiedTextItem.ToXml("T1"));
		}

		[Test]
		public void PastingCopiedTextItemWithFieldIntoSecondFormResultsInTextItemWithField()
		{
			view1.Presenter.InsertFibItem(-1);

			string textItemXml = @"<text label=""T1"" style=""normal""><paragraph indent=""0"" align=""left""><field name=""Form 1:Q1:a""/></paragraph></text>" + Environment.NewLine;
			addTextItemToForm1(textItemXml);

			NewTextItem textItem = getFormItem<NewTextItem>(0, 1);
			select(textItem);

			view1.CopySelectedItems();
			view2.PasteItems();

			NewTextItem copiedTextItem = getFormItem<NewTextItem>(1, 0);
			Assert.AreEqual(textItemXml, copiedTextItem.ToXml("T1"));
		}

		private void addTextItemToForm1(string textItemXml)
		{
			object[] args = new object[] { new NewTextItem(new XmlElement(textItemXml)), -1 };
			Reflect<FormPresenter>.InvokeMethod("insertTextItemIntoModelAndView", view1.Presenter as FormPresenter, args);
		}

		[Test]
		public void PastingCopiedMcqItemWithFieldInChoiceIntoSecondFormResultsInMcqItemWithFieldInChoice()
		{
			view1.Presenter.InsertFibItem(-1);

			string mcqItemXml =
				@"<mc label=""Q2"" onlyone=""true"" required=""false"" style=""vertical"">" +
				@"<question><paragraph indent=""0"" align=""left""><font face=""Arial"" size=""200"" color=""000000"">My MCQ</font></paragraph></question>" +
				@"<choice label=""a""><font face=""Arial"" size=""200"" color=""000000"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"</font></choice>" +
				@"</mc>" + Environment.NewLine;

			addMcqItemToForm1(mcqItemXml);

			IMcqItem mcqItem = getFormItem<IMcqItem>(0, 1);
			select(mcqItem);

			view1.CopySelectedItems();
			view2.PasteItems();

			IMcqItem copiedMcqItem = getFormItem<IMcqItem>(1, 0);
			Assert.AreEqual(mcqItemXml, copiedMcqItem.ToXml("Q2"));
		}

		// we have no tests for cut

		[Test]
		[Ignore ("We have no way to mimic multiple selection. It works ok in the UI, so maybe we don't need this test.")]
		public void PastingCopiedMultipleItemsIntoSecondFormResultsInMultipleItems()
		{
			view1.Presenter.InsertBreakItem(-1);
			view1.Presenter.InsertFibItem(-1);

			BreakItem breakItem = getFormItem<BreakItem>(0, 0);
			select(breakItem);

			NewFibItem fibItem = getFormItem<NewFibItem>(0, 1);
			select(fibItem);

			view1.CopySelectedItems();
			view2.PasteItems();

			Assert.AreEqual(2, getForm(1).ItemList.Count);
			Assert.IsInstanceOfType(typeof(BreakItem), getFormItem<BreakItem>(1, 0));
			Assert.IsInstanceOfType(typeof(BreakItem), getFormItem<BreakItem>(1, 1));
		}

		[Test]
		[Ignore("future test: copy / paste with Invitations once we have Invitations")]
		public void PastingCopiedTextItemWithInvitationInSecondFormResultsInTextItemWithInvitation()
		{
			Assert.Fail("TBD");
		}

		[Test]
		[Ignore("future test: copy / paste with Functions once we have Functions")]
		public void PastingCopiedTextItemWithFunctionInSecondFormResultsInTextItemWithFunction()
		{
			Assert.Fail("TBD");
		}

		[Test]
		[Ignore("future test: copy / paste with images once we have images")]
		public void PastingCopiedTextItemWithImageInSecondFormResultsInTextItemWithImage()
		{
			Assert.Fail("TBD");
		}

		private void addMcqItemToForm1(string mcqItemXml)
		{
			object[] args = new object[] { new NewMcqItem(new XmlElement(mcqItemXml)), -1 };
			Reflect<FormPresenter>.InvokeMethod("insertMcqItemIntoModelAndView", view1.Presenter as FormPresenter, args);
		}

		private void select(IFormItem formItem)
		{
			TestUtil.SelectFormItem(view1, formItem);
		}
		
		private static IForm getForm(int index)
		{
			return Project.Current.FormList[index];
		}

		private static T getFormItem<T>(int formIndex, int itemIndex) where T : class, IFormItem
		{
			return Project.Current.FormList[formIndex].ItemList[itemIndex] as T;
		}
	}
}
