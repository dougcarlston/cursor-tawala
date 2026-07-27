using System;
using NUnit.Framework;
using Tawala.FormDesigner.FormItemOptions;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using TawalaTest.TestingSupport;

namespace TawalaTest.FormDesignerTest.PresenterTest
{
    [TestFixture]
    public class HeadingOptionsPresenterTest
    {
        #region Setup/Teardown

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

        #endregion

        private NewHeadingItem headingItem1;
        private NewHeadingItem headingItem2;
        private IHiddenField hiddenField;

        private NewFibItem fibItemInForm2;
        private IBlank blankInForm2;
        private NewTextItem textItemInForm2;
        private NewHeadingItem headingItemInForm2;
        private IHiddenField hiddenFieldInForm2;

        private void changeItemLabelAndTestAgainstItemInOtherForm(IFormItem formItem)
        {
            const string alternateLabel = "AlternateLabel";
            formItem.AlternateLabel = alternateLabel;

            changeItemLabelAndTestIt(alternateLabel);
        }

        private void changeItemLabelAndTestIt(string label)
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged(label);

            Assert.AreEqual(label, headingItem1.FieldName);
        }

        private static IHeadingOptionsPresenter getPresenter(IHeadingOptionsView view)
        {
            var presenter = Reflect<DummyView>.GetField<IHeadingOptionsPresenter>("presenter", ((DummyView)view));
            return presenter;
        }

        private class DummyView : IHeadingOptionsView
        {
            private IHeadingOptionsPresenter presenter;

            internal DummyView(IHeadingItem headingItem)
            {
                presenter = new HeadingOptionsPresenter(this, headingItem);
            }

            #region IHeadingOptionsView Members

            public string HeadingLabel { get; set; }

            public string LabelStatusText
            {
                set { }
            }

            public HeadingType HeadingType
            {
                get { return HeadingType.Main; }
                set { }
            }

            #endregion
        }

        [Test]
        public void AlternateHeadingLabelContainingColonDoesNotUpdateLabelInModel()
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged("Contains:Colon");

            Assert.AreEqual("H1", headingItem1.FieldName);
        }

        [Test]
        public void AlternateHeadingLabelContainingIllegalCharactersDoesNotUpdateLabelInModel()
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged("<");
            Assert.AreEqual("H1", headingItem1.FieldName);

            getPresenter(view).HeadingLabelChanged(">");
            Assert.AreEqual("H1", headingItem1.FieldName);
        }

        [Test]
        public void AlternateHeadingLabelDuplicatingBlankLabelInAnotherFormUpdatesLabelInModel()
        {
            blankInForm2.AlternateLabel = "AlternateLabel";

            changeItemLabelAndTestIt("AlternateLabel");
        }

        [Test]
        public void AlternateHeadingLabelDuplicatingHeadingLabelInAnotherFormUpdatesLabelInModel()
        {
            changeItemLabelAndTestAgainstItemInOtherForm(headingItemInForm2);
        }

        [Test]
        public void AlternateHeadingLabelDuplicatingHiddenFieldNameDoesNotUpdateLabelInModel()
        {
            hiddenField.Name = "My Field";

            var view = new DummyView(headingItem2);
            getPresenter(view).HeadingLabelChanged("My Field");

            Assert.AreEqual("H2", headingItem2.FieldName);
        }

        [Test]
        public void AlternateHeadingLabelDuplicatingHiddenFieldNameInAnotherFormUpdatesLabelInModel()
        {
            changeItemLabelAndTestAgainstItemInOtherForm(hiddenFieldInForm2);
        }

        [Test]
        public void AlternateHeadingLabelDuplicatingQuestionLabelInAnotherFormUpdatesLabelInModel()
        {
            changeItemLabelAndTestAgainstItemInOtherForm(fibItemInForm2);
        }

        [Test]
        public void AlternateHeadingLabelDuplicatingTextLabelDoesNotUpdateLabelInModel()
        {
            headingItem1.AlternateLabel = "My Heading";

            var view = new DummyView(headingItem2);
            getPresenter(view).HeadingLabelChanged("My Heading");

            Assert.AreEqual("H2", headingItem2.FieldName);
        }

        [Test]
        public void AlternateHeadingLabelDuplicatingTextLabelInAnotherFormUpdatesLabelInModel()
        {
            changeItemLabelAndTestAgainstItemInOtherForm(textItemInForm2);
        }

        [Test]
        public void AlternateHeadingLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged("H99");

            Assert.AreEqual("H1", headingItem1.FieldName);
        }

        [Test]
        public void AlternateHeadingLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged("Q99");

            Assert.AreEqual("H1", headingItem1.FieldName);
        }

        [Test]
        public void AlternateHeadingLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged("T99");

            Assert.AreEqual("H1", headingItem1.FieldName);
        }

        [Test]
        public void AlternateHeadingLabelWithLeadingDoubleUnderscoresDoesNotUpdateLabelInModel()
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged("__LabelWithUnderscores");

            Assert.AreEqual("H1", headingItem1.FieldName);
        }

        [Test]
        public void AlternateHeadingLabelWithLeadingOrTrailingBlanksDuplicatingTextLabelDoesNotUpdateLabelInModel()
        {
            headingItem1.AlternateLabel = "My Heading";

            var view = new DummyView(headingItem2);
            getPresenter(view).HeadingLabelChanged(" My Heading ");

            Assert.AreEqual("H2", headingItem2.FieldName);
        }

        [Test]
        public void ChangingHeadingLabelInViewUpdatesLabelInModel()
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged("New Label");

            Assert.AreEqual("New Label", headingItem1.FieldName);
        }

        [Test]
        public void CreatingViewPopulatesViewWithDefaultHeadingLabel()
        {
            var view = new DummyView(headingItem1);

            Assert.AreEqual("H1", view.HeadingLabel);
        }

        [Test]
        public void CreatingViewWithAlternateLabelInModelPopulatesViewWithAlternateLabel()
        {
            headingItem1.AlternateLabel = "HEADING1";
            var view = new DummyView(headingItem1);

            Assert.AreEqual("HEADING1", view.HeadingLabel);
        }

        [Test]
        public void LowerCaseAlternateHeadingLabelResemblingDefaultHeadingLabelDoesNotUpdateLabelInModel()
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged("h99");

            Assert.AreEqual("H1", headingItem1.FieldName);
        }

        [Test]
        public void LowerCaseAlternateHeadingLabelResemblingDefaultQuestionLabelDoesNotUpdateLabelInModel()
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged("q99");

            Assert.AreEqual("H1", headingItem1.FieldName);
        }

        [Test]
        public void LowerCaseAlternateHeadingLabelResemblingDefaultTextLabelDoesNotUpdateLabelInModel()
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged("t99");

            Assert.AreEqual("H1", headingItem1.FieldName);
        }

        [Test]
        public void NumericAlternateHeadingLabelDoesNotUpdateLabelInModel()
        {
            var view = new DummyView(headingItem1);
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

        [Test]
        public void RevertingToEmptyAlternateHeadingLabelProducesDefaultLabelInModel()
        {
            var view = new DummyView(headingItem1);

            getPresenter(view).HeadingLabelChanged("A");
            getPresenter(view).HeadingLabelChanged("");

            Assert.AreEqual("H1", headingItem1.FieldName);
        }
    }
}