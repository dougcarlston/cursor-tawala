using System;
using System.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Tawala.FormDesigner;
using Tawala.Interfaces;
using Tawala.ProjectExplorer;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest
{
    [TestFixture]
    public class CutCopyPasteDeleteFormTest2908
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();

            view = mocks.DynamicMock<IProjectExplorerView>();
            presenter = new ProjectExplorerPresenter(view);

            Util.NewTestProject();
            ComponentMaker.UseNewComponents(true);
        }

        [TearDown]
        public void TearDown()
        {
            ComponentMaker.UseNewComponents(false);
        }

        #endregion

        private MockRepository mocks;
        private IProjectExplorerView view;
        private IProjectExplorerPresenter presenter;
        private IFormView formView;

        [Test]
        public void CopyingAndPastingFormWithFibItemProducesUniqueFibItem()
        {
            IForm copiedForm = Project.Current.AddForm();
            copiedForm.ItemList.Add(new NewFibItem());

            presenter.FormCopyRequested(new FormView(copiedForm));
            presenter.FormPasteRequested();

            IForm pastedForm = Project.Current.FormList[1];

            var copiedFibItem = copiedForm.ItemList[0] as IFibItem;
            var pastedFibItem = pastedForm.ItemList[0] as IFibItem;

            Assert.AreNotEqual(copiedFibItem.Id, pastedFibItem.Id);

            IBlank copiedBlank = copiedFibItem.BlankList[0];
            IBlank pastedBlank = pastedFibItem.BlankList[0];

            Assert.AreNotEqual(copiedBlank.Id, pastedBlank.Id);
        }

        [Test]
        public void CopyingFormInPresenterPlacesFormOnClipboard()
        {
            IForm form = Project.Current.AddForm();

            presenter.FormCopyRequested(new FormView(form));

            Assert.AreEqual(true, Clipboard.GetDataObject().GetDataPresent(typeof(IForm)));
        }

        [Test]
        public void CuttingFormInPresenterCutsFormInModel()
        {
            IForm form = Project.Current.AddForm();

            presenter.FormCutRequested(new FormView(form));

            Assert.AreEqual(0, Project.Current.FormList.Count);
        }

        [Test]
        public void CuttingFormInPresenterCutsFormInView()
        {
            IForm form = Project.Current.AddForm();
            formView = mocks.DynamicMock<IFormView>();

            Expect.Call(formView.Presenter).PropertyBehavior();
            Expect.Call(delegate { view.CutForm(form.Name); });

            mocks.ReplayAll();
            formView.Presenter = new FormPresenter(formView, form);

            presenter.FormCutRequested(formView);

            mocks.VerifyAll();
        }

        [Test]
        public void CuttingFormInPresenterPlacesFormOnClipboard()
        {
            IForm form = Project.Current.AddForm();

            presenter.FormCutRequested(new FormView(form));

            Assert.AreEqual(true, Clipboard.GetDataObject().GetDataPresent(typeof(IForm)));
        }

        [Test]
        public void DeletingFormInPresenterDeletesFormInModel()
        {
            IForm form = Project.Current.AddForm();

            presenter.FormDeleteRequested(new FormView(form));

            Assert.AreEqual(0, Project.Current.FormList.Count);
        }

        [Test]
        public void DeletingFormInPresenterDeletesFormInView()
        {
            IForm form = Project.Current.AddForm();
            formView = mocks.DynamicMock<IFormView>();

            Expect.Call(formView.Presenter).PropertyBehavior();
            Expect.Call(delegate { view.DeleteForm(form.Name); });

            mocks.ReplayAll();
            formView.Presenter = new FormPresenter(formView, form);

            presenter.FormDeleteRequested(formView);

            mocks.VerifyAll();
        }

        [Test]
        public void PastedFormContentsProduceSameXmlAsCopiedFormContents()
        {
            IForm copiedForm = Project.Current.AddForm();
            copiedForm.ItemList.Add(new NewTextItem());
            copiedForm.ItemList.Add(new NewFibItem());

            presenter.FormCopyRequested(new FormView(copiedForm));
            presenter.FormPasteRequested();

            IForm pastedForm = Project.Current.FormList[1];

            var copiedTextItem = copiedForm.ItemList[0] as ITextItem;
            var pastedTextItem = pastedForm.ItemList[0] as ITextItem;

            Assert.AreEqual(copiedTextItem.ToXml("T1"), pastedTextItem.ToXml("T1"));

            var copiedFibItem = copiedForm.ItemList[1] as IFibItem;
            var pastedFibItem = pastedForm.ItemList[1] as IFibItem;

            Assert.AreEqual(copiedFibItem.ToXml("Q1"), pastedFibItem.ToXml("Q1"));
        }

        [Test]
        public void PastingFormInPresenterPastesFormInModel()
        {
            IForm form = Project.Current.AddForm();

            presenter.FormCutRequested(new FormView(form));
            presenter.FormPasteRequested();

            Assert.AreEqual(1, Project.Current.FormList.Count);
        }

        [Test]
        public void PastingFormInPresenterPastesFormInView()
        {
            IForm form = Project.Current.AddForm();
            formView = mocks.DynamicMock<IFormView>();

            Expect.Call(formView.Presenter).PropertyBehavior();
            Expect.Call(delegate { view.PasteForm(formView, form.StartingPoint); }).IgnoreArguments();

            mocks.ReplayAll();
            formView.Presenter = new FormPresenter(formView, form);

            presenter.FormCutRequested(formView);
            presenter.FormPasteRequested();

            mocks.VerifyAll();
        }
    }
}