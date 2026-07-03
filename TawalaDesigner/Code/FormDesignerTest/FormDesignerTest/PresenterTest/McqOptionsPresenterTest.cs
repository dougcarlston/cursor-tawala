using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.Proj.Forms.FormItemContents;
using TawalaTest.TestSupport;
using Tawala.FormDesigner;
using Tawala.FormDesigner.FormItemOptions;
using Tawala.Interfaces;

namespace TawalaTest.FormDesignerTest.PresenterTest
{
	[TestFixture]
	public class McqOptionsPresenterTest
	{
		private IMcqItem mcqItem1;
		private IMcqItem mcqItem2;
		private IHiddenField hiddenField;

		private NewFibItem fibItemInForm2;
		private IBlank blankInForm2;
		private NewTextItem textItemInForm2;
		private NewHeadingItem headingItemInForm2;
		private IHiddenField hiddenFieldInForm2;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();

			hiddenField = new NewHiddenField();
			mcqItem1 = new NewMcqItem();
			mcqItem2 = new NewMcqItem();

			IForm form = Project.Current.AddForm();
			form.ItemList.Add(hiddenField);
			form.ItemList.Add(mcqItem1);
			form.ItemList.Add(mcqItem2);

			fibItemInForm2 = new NewFibItem();
			blankInForm2 = fibItemInForm2.BlankList[0];
			textItemInForm2 = new NewTextItem();
			headingItemInForm2 = new NewHeadingItem();
			hiddenFieldInForm2 = new NewHiddenField();

			IForm form2 = Project.Current.AddForm();
			form2.ItemList.Add(fibItemInForm2);
			form2.ItemList.Add(textItemInForm2);
			form2.ItemList.Add(headingItemInForm2);
			form2.ItemList.Add(hiddenFieldInForm2);
		}

		[Test]
		public void CreatingViewPopulatesViewWithDefaultQuestionLabel()
		{
			DummyView view = new DummyView(mcqItem1);

			Assert.AreEqual("Q1", view.QuestionLabel);
		}

		[Test]
		public void CreatingViewWithAlternateLabelInModelPopulatesViewWithAlternateLabel()
		{
			mcqItem1.AlternateLabel = "MCQ1";
			DummyView view = new DummyView(mcqItem1);

			Assert.AreEqual("MCQ1", view.QuestionLabel);
		}

		[Test]
		public void ChangingQuestionLabelInViewUpdatesLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged("New Label");

			Assert.AreEqual("New Label", mcqItem1.FieldName);
		}

		[Test]
		public void CreatingViewPopulatesViewWithResponseRequired()
		{
			Assert.IsFalse(mcqItem1.RequireAtLeastOne);

			mcqItem1.RequireAtLeastOne = true;

			DummyView view = new DummyView(mcqItem1);

			Assert.IsTrue(view.ResponseRequired);
		}

		[Test]
		public void CreatingViewPopulatesViewWithSelectMoreThanOne()
		{
			Assert.IsTrue(mcqItem1.SelectOnlyOne);

			mcqItem1.SelectOnlyOne = false;

			DummyView view = new DummyView(mcqItem1);

			Assert.IsTrue(view.SelectMoreThanOne);
		}

		[Test]
		public void RevertingToEmptyAlternateQuestionLabelProducesDefaultLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged("A");
			getPresenter(view).QuestionLabelChanged("");

			Assert.AreEqual("Q1", mcqItem1.FieldName);
		}

		#region Label tested against default label formats

		[Test]
		public void AlternateQuestionLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged("Q99");

			Assert.AreEqual("Q1", mcqItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateQuestionLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged("q99");

			Assert.AreEqual("Q1", mcqItem1.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged("T99");

			Assert.AreEqual("Q1", mcqItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateQuestionLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged("t99");

			Assert.AreEqual("Q1", mcqItem1.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged("H99");

			Assert.AreEqual("Q1", mcqItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateQuestionLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged("h99");

			Assert.AreEqual("Q1", mcqItem1.FieldName);
		}

		#endregion

		#region Label tested against other Alternate Labels in same Form

		[Test]
		public void AlternateQuestionLabelDuplicatingQuestionLabelDoesNotUpdateLabelInModel()
		{
			mcqItem1.AlternateLabel = "My MCQ";

			DummyView view = new DummyView(mcqItem2);
			getPresenter(view).QuestionLabelChanged("My MCQ");

			Assert.AreEqual("Q2", mcqItem2.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelWithLeadingOrTrailingBlanksDuplicatingQuestionLabelDoesNotUpdateLabelInModel()
		{
			mcqItem1.AlternateLabel = "My MCQ";

			DummyView view = new DummyView(mcqItem2);
			getPresenter(view).QuestionLabelChanged(" My MCQ ");

			Assert.AreEqual("Q2", mcqItem2.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelDuplicatingHiddenFieldNameDoesNotUpdateLabelInModel()
		{
			hiddenField.Name = "My Field";

			DummyView view = new DummyView(mcqItem2);
			getPresenter(view).QuestionLabelChanged("My Field");

			Assert.AreEqual("Q2", mcqItem2.FieldName);
		}

		#endregion

		#region Label tested for other illegal label formats

		[Test]
		public void AlternateQuestionLabelWithLeadingDoubleUnderscoresDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged("__LabelWithUnderscores");

			Assert.AreEqual("Q1", mcqItem1.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelContainingColonDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged("Contains:Colon");

			Assert.AreEqual("Q1", mcqItem1.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelContainingIllegalCharactersDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged("<");
			Assert.AreEqual("Q1", mcqItem1.FieldName);

			getPresenter(view).QuestionLabelChanged(">");
			Assert.AreEqual("Q1", mcqItem1.FieldName);
		}

		[Test]
		public void NumericAlternateQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(mcqItem1);
			IMcqOptionsPresenter presenter = getPresenter(view);

			presenter.QuestionLabelChanged("42");
			Assert.AreEqual("Q1", mcqItem1.FieldName);

			presenter.QuestionLabelChanged("42.3");
			Assert.AreEqual("Q1", mcqItem1.FieldName);

			presenter.QuestionLabelChanged("42.");
			Assert.AreEqual("Q1", mcqItem1.FieldName);

			presenter.QuestionLabelChanged("-42");
			Assert.AreEqual("Q1", mcqItem1.FieldName);

			presenter.QuestionLabelChanged("-42.3");
			Assert.AreEqual("Q1", mcqItem1.FieldName);

			presenter.QuestionLabelChanged("-42.");
			Assert.AreEqual("Q1", mcqItem1.FieldName);

			presenter.QuestionLabelChanged("+42");
			Assert.AreEqual("Q1", mcqItem1.FieldName);

			presenter.QuestionLabelChanged("+42.3");
			Assert.AreEqual("Q1", mcqItem1.FieldName);

			presenter.QuestionLabelChanged("+42.");
			Assert.AreEqual("Q1", mcqItem1.FieldName);
		}

		#endregion

		#region Label tested against Alternate Labels in other Forms

		[Test]
		public void AlternateQuestionLabelDuplicatingQuestionLabelInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(fibItemInForm2);
		}

		[Test]
		public void AlternateQuestionLabelDuplicatingTextLabelInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(textItemInForm2);
		}

		[Test]
		public void AlternateQuestionLabelDuplicatingHeadingLabelInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(headingItemInForm2);
		}

		[Test]
		public void AlternateQuestionLabelDuplicatingHiddenFieldNameInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(hiddenFieldInForm2);
		}

		[Test]
		public void AlternateQuestionLabelDuplicatingBlankLabelInAnotherFormUpdatesLabelInModel()
		{
			blankInForm2.AlternateLabel = "AlternateLabel";

			changeItemLabelAndTestIt("AlternateLabel");
		}

		private void changeItemLabelAndTestAgainstItemInOtherForm(IFormItem formItem)
		{
			string alternateLabel = "AlternateLabel";
			formItem.AlternateLabel = alternateLabel;

			changeItemLabelAndTestIt(alternateLabel);
		}

		private void changeItemLabelAndTestIt(string label)
		{
			DummyView view = new DummyView(mcqItem1);

			getPresenter(view).QuestionLabelChanged(label);

			Assert.AreEqual(label, mcqItem1.FieldName);
		}

		#endregion

		private static IMcqOptionsPresenter getPresenter(IMcqOptionsView view)
		{
			IMcqOptionsPresenter presenter = Reflect<DummyView>.GetField<IMcqOptionsPresenter>("presenter", ((DummyView)view));
			return presenter;
		}

		private class DummyView : IMcqOptionsView
		{
			private IMcqOptionsPresenter presenter;
			private string questionLabel;
			private bool responseRequired;
			private bool selectMoreThanOne;

			internal DummyView(IMcqItem mcqItem)
			{
				presenter = new McqOptionsPresenter(this, mcqItem);
			}

			#region IMcqOptionsView Members

			public string QuestionLabel
			{
				get { return questionLabel; }
				set { questionLabel = value; }
			}

			public bool ResponseRequired
			{
				get { return responseRequired; }
				set { responseRequired = value; }
			}

			public bool SelectMoreThanOne
			{
				get { return selectMoreThanOne; }
				set { selectMoreThanOne = value; }
			}

			public IFormItemsPalette GetFormItemsPalette()
			{
				throw new NotImplementedException();
			}

			public string LabelStatusText
			{
				set {  }
			}

			#endregion
		}
	}

}
