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
	public class HeadingOptionsPresenterTest
	{
		private NewHeadingItem headingItem1;
		private NewHeadingItem headingItem2;
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
			headingItem1 = new NewHeadingItem();
			headingItem2 = new NewHeadingItem();

			IForm form = Project.Current.AddForm();
			form.ItemList.Add(hiddenField);
			form.ItemList.Add(headingItem1);
			form.ItemList.Add(headingItem2);

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
		public void CreatingViewPopulatesViewWithDefaultHeadingLabel()
		{
			DummyView view = new DummyView(headingItem1);

			Assert.AreEqual("H1", view.HeadingLabel);
		}

		[Test]
		public void CreatingViewWithAlternateLabelInModelPopulatesViewWithAlternateLabel()
		{
			headingItem1.AlternateLabel = "HEADING1";
			DummyView view = new DummyView(headingItem1);

			Assert.AreEqual("HEADING1", view.HeadingLabel);
		}

		[Test]
		public void ChangingHeadingLabelInViewUpdatesLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged("New Label");

			Assert.AreEqual("New Label", headingItem1.FieldName);
		}

		[Test]
		public void RevertingToEmptyAlternateHeadingLabelProducesDefaultLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged("A");
			getPresenter(view).HeadingLabelChanged("");

			Assert.AreEqual("H1", headingItem1.FieldName);
		}

		#region Label tested against other Alternate Labels in same Form

		[Test]
		public void AlternateHeadingLabelDuplicatingTextLabelDoesNotUpdateLabelInModel()
		{
			headingItem1.AlternateLabel = "My Heading";

			DummyView view = new DummyView(headingItem2);
			getPresenter(view).HeadingLabelChanged("My Heading");

			Assert.AreEqual("H2", headingItem2.FieldName);
		}

		[Test]
		public void AlternateHeadingLabelWithLeadingOrTrailingBlanksDuplicatingTextLabelDoesNotUpdateLabelInModel()
		{
			headingItem1.AlternateLabel = "My Heading";

			DummyView view = new DummyView(headingItem2);
			getPresenter(view).HeadingLabelChanged(" My Heading ");

			Assert.AreEqual("H2", headingItem2.FieldName);
		}

		[Test]
		public void AlternateHeadingLabelDuplicatingHiddenFieldNameDoesNotUpdateLabelInModel()
		{
			hiddenField.Name = "My Field";

			DummyView view = new DummyView(headingItem2);
			getPresenter(view).HeadingLabelChanged("My Field");

			Assert.AreEqual("H2", headingItem2.FieldName);
		}

		#endregion

		#region Label tested against default label formats

		[Test]
		public void AlternateHeadingLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged("Q99");

			Assert.AreEqual("H1", headingItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateHeadingLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged("q99");

			Assert.AreEqual("H1", headingItem1.FieldName);
		}

		[Test]
		public void AlternateHeadingLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged("T99");

			Assert.AreEqual("H1", headingItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateHeadingLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged("t99");

			Assert.AreEqual("H1", headingItem1.FieldName);
		}

		[Test]
		public void AlternateHeadingLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged("H99");

			Assert.AreEqual("H1", headingItem1.FieldName);
		}

		[Test]
		public void LowerCaseAlternateHeadingLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged("h99");

			Assert.AreEqual("H1", headingItem1.FieldName);
		}

		#endregion

		#region Label tested for other illegal label formats

		[Test]
		public void AlternateHeadingLabelWithLeadingDoubleUnderscoresDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged("__LabelWithUnderscores");

			Assert.AreEqual("H1", headingItem1.FieldName);
		}

		[Test]
		public void AlternateHeadingLabelContainingColonDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged("Contains:Colon");

			Assert.AreEqual("H1", headingItem1.FieldName);
		}

		[Test]
		public void AlternateHeadingLabelContainingIllegalCharactersDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged("<");
			Assert.AreEqual("H1", headingItem1.FieldName);

			getPresenter(view).HeadingLabelChanged(">");
			Assert.AreEqual("H1", headingItem1.FieldName);
		}

		[Test]
		public void NumericAlternateHeadingLabelDoesNotUpdateLabelInModel()
		{
			DummyView view = new DummyView(headingItem1);
			IHeadingOptionsPresenter presenter = getPresenter(view);

			presenter.HeadingLabelChanged("42");
			Assert.AreEqual("H1", headingItem1.FieldName);

			presenter.HeadingLabelChanged("42.3");
			Assert.AreEqual("H1", headingItem1.FieldName);

			presenter.HeadingLabelChanged("42.");
			Assert.AreEqual("H1", headingItem1.FieldName);

			presenter.HeadingLabelChanged("-42");
			Assert.AreEqual("H1", headingItem1.FieldName);

			presenter.HeadingLabelChanged("-42.3");
			Assert.AreEqual("H1", headingItem1.FieldName);

			presenter.HeadingLabelChanged("-42.");
			Assert.AreEqual("H1", headingItem1.FieldName);

			presenter.HeadingLabelChanged("+42");
			Assert.AreEqual("H1", headingItem1.FieldName);

			presenter.HeadingLabelChanged("+42.3");
			Assert.AreEqual("H1", headingItem1.FieldName);

			presenter.HeadingLabelChanged("+42.");
			Assert.AreEqual("H1", headingItem1.FieldName);
		}

		#endregion

		#region Label tested against Alternate Labels in other Forms

		[Test]
		public void AlternateHeadingLabelDuplicatingQuestionLabelInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(fibItemInForm2);
		}

		[Test]
		public void AlternateHeadingLabelDuplicatingTextLabelInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(textItemInForm2);
		}

		[Test]
		public void AlternateHeadingLabelDuplicatingHeadingLabelInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(headingItemInForm2);
		}

		[Test]
		public void AlternateHeadingLabelDuplicatingHiddenFieldNameInAnotherFormUpdatesLabelInModel()
		{
			changeItemLabelAndTestAgainstItemInOtherForm(hiddenFieldInForm2);
		}

		[Test]
		public void AlternateHeadingLabelDuplicatingBlankLabelInAnotherFormUpdatesLabelInModel()
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
			DummyView view = new DummyView(headingItem1);

			getPresenter(view).HeadingLabelChanged(label);

			Assert.AreEqual(label, headingItem1.FieldName);
		}

		#endregion

		private static IHeadingOptionsPresenter getPresenter(IHeadingOptionsView view)
		{
			IHeadingOptionsPresenter presenter = Reflect<DummyView>.GetField<IHeadingOptionsPresenter>("presenter", ((DummyView)view));
			return presenter;
		}

		private class DummyView : IHeadingOptionsView
		{
			private IHeadingOptionsPresenter presenter;
			private string headingLabel;

			internal DummyView(IHeadingItem headingItem)
			{
				presenter = new HeadingOptionsPresenter(this, headingItem);
			}

			#region IHeadingOptionsView Members

			public string HeadingLabel
			{
				get
				{
					return headingLabel;
				}
				set
				{
					headingLabel = value;
				}
			}

			public string LabelStatusText
			{
				set { }
			}

			#endregion
		}
	}

}
