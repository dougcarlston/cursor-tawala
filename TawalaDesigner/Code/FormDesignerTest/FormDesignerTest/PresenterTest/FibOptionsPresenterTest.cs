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

namespace TawalaTest.FormDesignerTest.PresenterTest
{
	[TestFixture]
	public class FibOptionsPresenterTest
	{
		private NewFibItem fibItem1;
		private IBlank blank1;
		private NewFibItem fibItem2;
		private IBlank blank2;
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
			fibItem1 = new NewFibItem();
			blank1 = fibItem1.BlankList[0];
			fibItem2 = new NewFibItem();
			blank2 = fibItem2.BlankList[0];

			IForm form = Project.Current.AddForm();
			form.ItemList.Add(hiddenField);
			form.ItemList.Add(fibItem1);
			form.ItemList.Add(fibItem2);

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
			DummyView view = new DummyView(fibItem1, null);

			Assert.AreEqual("Q1", view.QuestionLabel);
		}

		[Test]
		public void CreatingViewWithAlternateLabelInModelPopulatesViewWithAlternateLabel()
		{
			fibItem1.AlternateLabel = "FIB1";
			DummyView view = new DummyView(fibItem1, null);

			Assert.AreEqual("FIB1", view.QuestionLabel);
		}

		[Test]
		public void CreatingViewPopulatesViewWithSelectedBlankLabel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			Assert.AreEqual("Q1:a", view.SelectedBlankLabel);
		}

		[Test]
		public void CreatingViewPopulatesViewWithSelectedBlankRequired()
		{
			Assert.IsFalse(blank1.Required);

			blank1.Required = true;

			DummyView view = new DummyView(fibItem1, blank1);

			Assert.IsTrue(view.SelectedBlankRequired);
		}

		[Test]
		public void CreatingViewPopulatesViewWithoutSelectedBlankClearsBlankLabel()
		{
			DummyView view = new DummyView(fibItem1, null);

			Assert.AreEqual(string.Empty, view.SelectedBlankLabel);
		}

		[Test]
		public void ChangingQuestionLabelInViewUpdatesLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("New Label");

			Assert.AreEqual("New Label", fibItem1.FieldName);
		}

		[Test]
		public void ChangingBlankLabelInViewUpdatesLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("Alternate");

			Assert.AreEqual("Alternate", blank1.FieldName);
		}

		[Test]
		public void ChangingBlankResponseRequiredInViewUpdatesRequiredInModel()
		{
			Assert.IsFalse(blank1.Required);

			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankResponseRequiredChanged(true);

			Assert.IsTrue(blank1.Required);
		}

		[Test]
		public void RevertingToEmptyAlternateQuestionLabelProducesDefaultLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("A");
			getPresenter(view).QuestionLabelChanged("");

			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		[Test]
		public void RevertingToEmptyAlternateBlankLabelProducesDefaultLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("A");
			getPresenter(view).BlankLabelChanged("");

			Assert.AreEqual("Q1:a", blank1.FieldName);
		}

		#region Question Label tested against default label formats

		[Test]
		public void AlternateQuestionLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("Q99");

			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateQuestionLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("q99");

			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("T99");

			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateQuestionLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("t99");

			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("H99");

			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateQuestionLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("h99");

			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		#endregion

		#region Question Label tested against other Alternate Labels in same Form

		[Test]
		public void AlternateQuestionLabelDuplicatingQuestionLabelDoesNotUpdateLabelInModel()
		{
			fibItem1.AlternateLabel = "My FIB";

			DummyView view = new DummyView(fibItem2, blank2);
			getPresenter(view).QuestionLabelChanged("My FIB");

			Assert.AreEqual("Q2", fibItem2.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelWithLeadingOrTrailingBlanksDuplicatingQuestionLabelDoesNotUpdateLabelInModel()
		{
			fibItem1.AlternateLabel = "My FIB";

			DummyView view = new DummyView(fibItem2, blank2);
			getPresenter(view).QuestionLabelChanged(" My FIB ");

			Assert.AreEqual("Q2", fibItem2.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelDuplicatingBlankLabelDoesNotUpdateLabelInModel()
		{
			blank1.AlternateLabel = "My Blank";

			DummyView view = new DummyView(fibItem2, blank2);
			getPresenter(view).QuestionLabelChanged("My Blank");

			Assert.AreEqual("Q2", fibItem2.FieldName);
		}

		#endregion

		#region Question Label tested for other illegal label formats

		[Test]
		public void AlternateQuestionLabelWithLeadingDoubleUnderscoresDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("__LabelWithUnderscores");

			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelDuplicatingHiddenFieldNameDoesNotUpdateLabelInModel()
		{
			hiddenField.Name = "My Field";

			DummyView view = new DummyView(fibItem2, blank2);
			getPresenter(view).QuestionLabelChanged("My Field");

			Assert.AreEqual("Q2", fibItem2.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelContainingColonDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("Contains:Colon");

			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		[Test]
		public void AlternateQuestionLabelContainingIllegalCharactersDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("<");
			Assert.AreEqual("Q1", fibItem1.FieldName);

			getPresenter(view).QuestionLabelChanged(">");
			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		[Test]
		public void NumericAlternateQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged("42");
			Assert.AreEqual("Q1", fibItem1.FieldName);

			getPresenter(view).QuestionLabelChanged("42.3");
			Assert.AreEqual("Q1", fibItem1.FieldName);

			getPresenter(view).QuestionLabelChanged("42.");
			Assert.AreEqual("Q1", fibItem1.FieldName);

			getPresenter(view).QuestionLabelChanged("-42");
			Assert.AreEqual("Q1", fibItem1.FieldName);

			getPresenter(view).QuestionLabelChanged("-42.3");
			Assert.AreEqual("Q1", fibItem1.FieldName);

			getPresenter(view).QuestionLabelChanged("-42.");
			Assert.AreEqual("Q1", fibItem1.FieldName);

			getPresenter(view).QuestionLabelChanged("+42");
			Assert.AreEqual("Q1", fibItem1.FieldName);

			getPresenter(view).QuestionLabelChanged("+42.3");
			Assert.AreEqual("Q1", fibItem1.FieldName);

			getPresenter(view).QuestionLabelChanged("+42.");
			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		#endregion

		#region Question Label tested against Alternate Labels in other Forms

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
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).QuestionLabelChanged(label);

			Assert.AreEqual(label, fibItem1.FieldName);
		}

		#endregion

		#region Blank Label tested against default label formats

		[Test]
		public void AlternateBlankLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("Q99");

			Assert.AreEqual("Q1:a", blank1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateBlankLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("q99");

			Assert.AreEqual("Q1:a", blank1.FieldName);
		}

		[Test]
		public void AlternateBlankLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("T99");

			Assert.AreEqual("Q1:a", blank1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateBlankLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("t99");

			Assert.AreEqual("Q1:a", blank1.FieldName);
		}

		[Test]
		public void AlternateBlankLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("H99");

			Assert.AreEqual("Q1:a", blank1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateBlankLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("h99");

			Assert.AreEqual("Q1:a", blank1.FieldName);
		}

		[Test]
		public void AlternateBlankLabelDuplicatingHiddenFieldNameDoesNotUpdateLabelInModel()
		{
			hiddenField.Name = "My Field";

			DummyView view = new DummyView(fibItem2, blank2);
			getPresenter(view).BlankLabelChanged("My Field");

			Assert.AreEqual("Q2:a", blank2.FieldName);
		}

		#endregion

		#region Blank Label tested against other Alternate Labels in same Form

		[Test]
		public void AlternateBlankLabelDuplicatingQuestionLabelDoesNotUpdateLabelInModel()
		{
			fibItem1.AlternateLabel = "My Fib";

			DummyView view = new DummyView(fibItem2, blank2);
			getPresenter(view).BlankLabelChanged("My Fib");

			Assert.AreEqual("Q2:a", blank2.FieldName);
		}

		[Test]
		public void AlternateBlankLabelDuplicatingBlankLabelDoesNotUpdateLabelInModel()
		{
			blank1.AlternateLabel = "My Blank";

			DummyView view = new DummyView(fibItem2, blank2);
			getPresenter(view).BlankLabelChanged("My Blank");

			Assert.AreEqual("Q2:a", blank2.FieldName);
		}

		[Test]
		public void AlternateBlankLabelWithLeadingOrTrailingBlanksDuplicatingBlankLabelDoesNotUpdateLabelInModel()
		{
			blank1.AlternateLabel = "My Blank";

			DummyView view = new DummyView(fibItem2, blank2);
			getPresenter(view).BlankLabelChanged(" My Blank ");

			Assert.AreEqual("Q2:a", blank2.FieldName);
		}

		#endregion

		#region Blank Label tested for other illegal label formats

		[Test]
		public void AlternateBlankLabelWithLeadingDoubleUnderscoresDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("__LabelWithUnderscores");

			Assert.AreEqual("Q1:a", blank1.FieldName);
		}

		[Test]
		public void AlternateBlankLabelContainingColonDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("Contains:Colon");

			Assert.AreEqual("Q1:a", blank1.FieldName);
		}

		[Test]
		public void AlternateBlankLabelContainingIllegalCharactersDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("<");
			Assert.AreEqual("Q1:a", blank1.FieldName);

			getPresenter(view).BlankLabelChanged(">");
			Assert.AreEqual("Q1:a", blank1.FieldName);
		}

		[Test]
		public void NumericAlternateBlankLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(fibItem1, blank1);

			getPresenter(view).BlankLabelChanged("2");
			Assert.AreEqual("Q1:a", blank1.FieldName);

			getPresenter(view).BlankLabelChanged("2.3");
			Assert.AreEqual("Q1:a", blank1.FieldName);

			getPresenter(view).BlankLabelChanged("2.");
			Assert.AreEqual("Q1:a", blank1.FieldName);

			getPresenter(view).BlankLabelChanged("-2");
			Assert.AreEqual("Q1:a", blank1.FieldName);

			getPresenter(view).BlankLabelChanged("-2.3");
			Assert.AreEqual("Q1:a", blank1.FieldName);

			getPresenter(view).BlankLabelChanged("-2.");
			Assert.AreEqual("Q1:a", blank1.FieldName);

			getPresenter(view).BlankLabelChanged("+2");
			Assert.AreEqual("Q1:a", blank1.FieldName);

			getPresenter(view).BlankLabelChanged("+2.3");
			Assert.AreEqual("Q1:a", blank1.FieldName);

			getPresenter(view).BlankLabelChanged("+2.");
			Assert.AreEqual("Q1:a", blank1.FieldName);
		}

		#endregion

		#region Blank Label tested against Alternate Labels in other Forms

		[Test]
		public void AlternateBlankLabelDuplicatingBlankLabelInAnotherFormUpdatesLabelInModel()
		{
			blankInForm2.AlternateLabel = "My Blank";

			changeBlankLabelAndTestIt("My Blank");
		}

		[Test]
		public void AlternateBlankLabelDuplicatingQuestionLabelInAnotherFormUpdatesLabelInModel()
		{
			changeBlankLabelAndTestAgainstItemInOtherForm(fibItemInForm2);
		}

		[Test]
		public void AlternateBlankLabelDuplicatingTextLabelInAnotherFormUpdatesLabelInModel()
		{
			changeBlankLabelAndTestAgainstItemInOtherForm(textItemInForm2);
		}

		[Test]
		public void AlternateBlankLabelDuplicatingHeadingLabelInAnotherFormUpdatesLabelInModel()
		{
			changeBlankLabelAndTestAgainstItemInOtherForm(headingItemInForm2);
		}

		[Test]
		public void AlternateBlankLabelDuplicatingHiddenFieldNameInAnotherFormUpdatesLabelInModel()
		{
			changeBlankLabelAndTestAgainstItemInOtherForm(hiddenFieldInForm2);
		}

		private void changeBlankLabelAndTestAgainstItemInOtherForm(IFormItem formItem)
		{
			string alternateLabel = "AlternateLabel";
			formItem.AlternateLabel = alternateLabel;

			changeBlankLabelAndTestIt(alternateLabel);
		}

		private void changeBlankLabelAndTestIt(string label)
		{
			DummyView view = new DummyView(fibItem2, blank2);

			getPresenter(view).BlankLabelChanged(label);

			Assert.AreEqual(label, blank2.FieldName);
		}

		#endregion

		private static IFibOptionsPresenter getPresenter(IFibOptionsView view)
		{
			IFibOptionsPresenter presenter = Reflect<DummyView>.GetField<IFibOptionsPresenter>("presenter", ((DummyView)view));
			return presenter;
		}

		private class DummyView : IFibOptionsView
		{
			private IFibOptionsPresenter presenter;
			private string questionLabel;
			private string blankLabel = "Random Stuff";
			private bool blankRequired = false;
			private string labelStatusText;

			internal DummyView(IFibItem fibItem, IBlank selectedBlank)
			{
				presenter = new FibOptionsPresenter(this, fibItem, selectedBlank);
			}

			#region IFibOptionsView Members

			public string QuestionLabel
			{
				get { return questionLabel; }
				set { questionLabel = value; }
			}

			public string SelectedBlankLabel
			{
				get { return blankLabel; }
				set { blankLabel = value; }
			}

			public bool SelectedBlankRequired
			{
				get { return blankRequired; }
				set { blankRequired = true; }
			}

			public string LabelStatusText
			{
				set { labelStatusText = value; }
			}

			#endregion
		}
	}

}
