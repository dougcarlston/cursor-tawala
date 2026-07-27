using System;
using System.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Tawala.DocumentDesigner;
using Tawala.Interfaces;
using Tawala.ProjectExplorer;
using Tawala.Projects;
using Tawala.Projects.Documents;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest
{
    [TestFixture]
    public class CutCopyPasteDeleteDocumentTest2908
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
        private IDocumentView documentView;

        [Test]
        public void CopyingDocumentInPresenterPlacesDocumentOnClipboard()
        {
            IDocument document = Project.Current.AddDocument();

            presenter.DocumentCopyRequested(new DocumentView(document));

            Assert.AreEqual(true, Clipboard.GetDataObject().GetDataPresent(typeof(NewDocument)));
        }

        [Test]
        public void CuttingDocumentInPresenterCutsDocumentInModel()
        {
            IDocument document = Project.Current.AddDocument();

            presenter.DocumentCutRequested(new DocumentView(document));

            Assert.AreEqual(0, Project.Current.FormList.Count);
        }

        [Test]
        public void CuttingDocumentInPresenterCutsDocumentInView()
        {
            IDocument document = Project.Current.AddDocument();
            documentView = mocks.DynamicMock<IDocumentView>();

            Expect.Call(documentView.Presenter).PropertyBehavior();
            Expect.Call(delegate { view.CutDocument(document.Name); });

            mocks.ReplayAll();
            documentView.Presenter = new DocumentPresenter(documentView, document);

            presenter.DocumentCutRequested(documentView);

            mocks.VerifyAll();
        }

        [Test]
        public void CuttingDocumentInPresenterPlacesDocumentOnClipboard()
        {
            IDocument document = Project.Current.AddDocument();

            presenter.DocumentCutRequested(new DocumentView(document));

            Assert.AreEqual(true, Clipboard.GetDataObject().GetDataPresent(typeof(NewDocument)));
        }

        [Test]
        public void DeletingDocumentInPresenterDeletesDocumentInModel()
        {
            IDocument document = Project.Current.AddDocument();

            presenter.DocumentDeleteRequested(new DocumentView(document));

            Assert.AreEqual(0, Project.Current.DocumentList.Count);
        }

        [Test]
        public void DeletingDocumentInPresenterDeletesDocumentInView()
        {
            IDocument document = Project.Current.AddDocument();
            documentView = mocks.DynamicMock<IDocumentView>();

            Expect.Call(documentView.Presenter).PropertyBehavior();
            Expect.Call(delegate { view.DeleteDocument(document.Name); });

            mocks.ReplayAll();
            documentView.Presenter = new DocumentPresenter(documentView, document);

            presenter.DocumentDeleteRequested(documentView);

            mocks.VerifyAll();
        }

        [Test]
        public void PastingDocumentInPresenterPastesDocumentInModel()
        {
            IDocument document = Project.Current.AddDocument();

            presenter.DocumentCutRequested(new DocumentView(document));
            presenter.DocumentPasteRequested();

            Assert.AreEqual(1, Project.Current.DocumentList.Count);
        }

        [Test]
        public void PastingDocumentInPresenterPastesDocumentInView()
        {
            IDocument document = Project.Current.AddDocument();
            documentView = mocks.DynamicMock<IDocumentView>();

            Expect.Call(documentView.Presenter).PropertyBehavior();
            Expect.Call(delegate { view.PasteDocument(documentView); }).IgnoreArguments();

            mocks.ReplayAll();
            documentView.Presenter = new DocumentPresenter(documentView, document);

            presenter.DocumentCutRequested(documentView);
            presenter.DocumentPasteRequested();

            mocks.VerifyAll();
        }
    }
}