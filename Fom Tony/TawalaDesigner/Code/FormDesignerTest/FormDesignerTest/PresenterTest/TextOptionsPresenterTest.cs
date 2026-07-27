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
	public class TextOptionsPresenterTest
	{
		private NewTextItem textItem1;
		private NewTextItem textItem2;
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
			textItem1 = new NewTextItem();
			textItem2 = new NewTextItem();

			IForm form = Project.Current.AddForm();
			form.ItemList.Add(hiddenField);
			form.ItemList.Add(textItem1);
			form.ItemList.Add(textItem2);

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
		public void CreatingViewPopulatesViewWithDefaultTextLabel()
		{
			DummyView view = new DummyView(textItem1);

			Assert.AreEqual("T1", view.TextLabel);
		}

		[Test]
		public void CreatingViewWithAlternateLabelInModelPopulatesViewWithAlternateLabel()
		{
			textItem1.AlternateLabel = "TEXT1";
			DummyView view = new DummyView(textItem1);

			Assert.AreEqual("TEXT1", view.TextLabel);
		}

		[Test]
		public void ChangingTextLabelInViewUpdatesLabelInModel()
		{
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged("New Label");

			Assert.AreEqual("New Label", textItem1.FieldName);
		}

		[Test]
		public void RevertingToEmptyAlternateTextLabelProducesDefaultLabelInModel()
		{
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged("A");
			getPresenter(view).TextLabelChanged("");

			Assert.AreEqual("T1", textItem1.FieldName);
		}

		#region Label tested against default label formats

		[Test]
		public void AlternateTextLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged("Q99");

			Assert.AreEqual("T1", textItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateTextLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged("q99");

			Assert.AreEqual("T1", textItem1.FieldName);
		}

		[Test]
		public void AlternateTextLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged("T99");

			Assert.AreEqual("T1", textItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateTextLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged("t99");

			Assert.AreEqual("T1", textItem1.FieldName);
		}

		[Test]
		public void AlternateTextLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged("H99");

			Assert.AreEqual("T1", textItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateTextLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged("h99");

			Assert.AreEqual("T1", textItem1.FieldName);
		}

		#endregion

		#region Label tested against other Alternate Labels in same Form

		[Test]
		public void AlternateTextLabelDuplicatingTextLabelDoesNotUpdateLabelInModel()
		{
			textItem1.AlternateLabel = "My Text";

			DummyView view = new DummyView(textItem2);
			getPresenter(view).TextLabelChanged("My Text");

			Assert.AreEqual("T2", textItem2.FieldName);
		}

		[Test]
		public void AlternateTextLabelWithLeadingOrTrailingBlanksDuplicatingTextLabelDoesNotUpdateLabelInModel()
		{
			textItem1.AlternateLabel = "My Text";

			DummyView view = new DummyView(textItem2);
			getPresenter(view).TextLabelChanged(" My Text ");

			Assert.AreEqual("T2", textItem2.FieldName);
		}

		[Test]
		public void AlternateTextLabelDuplicatingHiddenFieldNameDoesNotUpdateLabelInModel()
		{
			hiddenField.Name = "My Field";

			DummyView view = new DummyView(textItem2);
			getPresenter(view).TextLabelChanged("My Field");

			Assert.AreEqual("T2", textItem2.FieldName);
		}

		#endregion

		#region Label tested for other illegal label formats

		[Test]
		public void AlternateTextLabelWithLeadingDoubleUnderscoresDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged("__LabelWithUnderscores");

			Assert.AreEqual("T1", textItem1.FieldName);
		}

		[Test]
		public void AlternateTextLabelContainingColonDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged("Contains:Colon");

			Assert.AreEqual("T1", textItem1.FieldName);
		}

		[Test]
		public void AlternateTextLabelContainingIllegalCharactersDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged("<");
			Assert.AreEqual("T1", textItem1.FieldName);

			getPresenter(view).TextLabelChanged(">");
			Assert.AreEqual("T1", textItem1.FieldName);
		}

		[Test]
		public void NumericAlternateTextLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(textItem1);
			ITextOptionsPresenter presenter = getPresenter(view);

			presenter.TextLabelChanged("42");
			Assert.AreEqual("T1", textItem1.FieldName);

			presenter.TextLabelChanged("42.3");
			Assert.AreEqual("T1", textItem1.FieldName);

			presenter.TextLabelChanged("42.");
			Assert.AreEqual("T1", textItem1.FieldName);

			presenter.TextLabelChanged("-42");
			Assert.AreEqual("T1", textItem1.FieldName);

			presenter.TextLabelChanged("-42.3");
			Assert.AreEqual("T1", textItem1.FieldName);

			presenter.TextLabelChanged("-42.");
			Assert.AreEqual("T1", textItem1.FieldName);

			presenter.TextLabelChanged("+42");
			Assert.AreEqual("T1", textItem1.FieldName);

			presenter.TextLabelChanged("+42.3");
			Assert.AreEqual("T1", textItem1.FieldName);

			presenter.TextLabelChanged("+42.");
			Assert.AreEqual("T1", textItem1.FieldName);
		}

		#endregion

		#region Label tested against Alternate Labels in other Forms

		[Test]
		public void AlternateTextLabelDuplicatingQuestionLabelInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(fibItemInForm2);
		}

		[Test]
		public void AlternateTextLabelDuplicatingTextLabelInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(textItemInForm2);
		}

		[Test]
		public void AlternateTextLabelDuplicatingHeadingLabelInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(headingItemInForm2);
		}

		[Test]
		public void AlternateTextLabelDuplicatingHiddenFieldNameInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(hiddenFieldInForm2);
		}

		[Test]
		public void AlternateTextLabelDuplicatingBlankLabelInAnotherFormUpdatesLabelInModel()
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
			DummyView view = new DummyView(textItem1);

			getPresenter(view).TextLabelChanged(label);

			Assert.AreEqual(label, textItem1.FieldName);
		}

		#endregion

		private static ITextOptionsPresenter getPresenter(ITextOptionsView view)
		{
			ITextOptionsPresenter presenter = Reflect<DummyView>.GetField<ITextOptionsPresenter>("presenter", ((DummyView)view));
			return presenter;
		}

		private class DummyView : ITextOptionsView
		{
			private ITextOptionsPresenter presenter;
			private string textLabel;

			internal DummyView(ITextItem textItem)
			{
				presenter = new TextOptionsPresenter(this, textItem);
			}

			#region ITextOptionsView Members

			public string TextLabel
			{
				get
				{
					return textLabel;
				}
				set
				{
					textLabel = value;
				}
			}

			public string LabelStatusText
			{
				set {  }
			}

			#endregion
		}
	}

}
