using System;
using NUnit.Framework;
using Rhino.Mocks;
using Tawala.DocumentDesigner;
using Tawala.FormDesigner;
using Tawala.Interfaces;
using Tawala.ProcessDesigner;
using Tawala.ProjectExplorer;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest
{
    [TestFixture]
    public class RenameProjectComponentTest2907
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();

            view = mocks.CreateMock<IProjectExplorerView>();
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
        public void RenamingDocumentInPresenterRenamesDocumentInModel()
        {
            IDocument document = Project.Current.AddDocument();

            presenter.DocumentRenameRequested(new DocumentView(document, null, presenter), "Renamed Document 1");

            Assert.AreEqual("Renamed Document 1", document.Name);
        }

        [Test]
        public void RenamingDocumentInPresenterSetsDocumentNameInTitleBar()
        {
            IDocument document = Project.Current.AddDocument();
            var documentView = mocks.CreateMock<IDocumentView>();

            Expect.Call(documentView.Presenter).PropertyBehavior();
            Expect.Call(delegate { documentView.SetDocumentName("Renamed Document 1"); });

            documentView.Presenter = new DocumentPresenter(documentView, document);

            mocks.ReplayAll();
            presenter.DocumentRenameRequested(documentView, "Renamed Document 1");

            mocks.VerifyAll();
        }

        [Test]
        public void RenamingFormInPresenterRenamesFormInModel()
        {
            IForm form = Project.Current.AddForm();

            presenter.FormRenameRequested(new FormView(form), "Renamed Form 1");

            Assert.AreEqual("Renamed Form 1", form.Name);
        }

        [Test]
        public void RenamingFormInPresenterSetsFormNameInTitleBar()
        {
            IForm form = Project.Current.AddForm();
            formView = mocks.CreateMock<IFormView>();

            Expect.Call(formView.Presenter).PropertyBehavior();
            Expect.Call(delegate { formView.SetFormName("Renamed Form 1"); });

            formView.Presenter = new FormPresenter(formView, form);

            mocks.ReplayAll();
            presenter.FormRenameRequested(formView, "Renamed Form 1");

            mocks.VerifyAll();
        }

        [Test]
        public void RenamingProcessInPresenterRenamesProcessInModel()
        {
            IProcess process = Project.Current.AddProcess();

            presenter.ProcessRenameRequested(new ProcessView(process, null, presenter), "Renamed Process 1");

            Assert.AreEqual("Renamed Process 1", process.Name);
        }

        [Test]
        public void RenamingProcessInPresenterSetsProcessNameInTitleBar()
        {
            IProcess process = Project.Current.AddProcess();
            var processView = mocks.CreateMock<IProcessView>();

            Expect.Call(processView.Presenter).PropertyBehavior();
            Expect.Call(delegate { processView.SetProcessName("Renamed Process 1"); });

            processView.Presenter = new ProcessPresenter(processView, process);

            mocks.ReplayAll();
            presenter.ProcessRenameRequested(processView, "Renamed Process 1");

            mocks.VerifyAll();
        }

        [Test]
        public void RenamingToDuplicateDocumentNameProhibited()
        {
            Project.Current.AddDocument();
            IDocument document2 = Project.Current.AddDocument();

            bool renameSuccessful = presenter.DocumentRenameRequested(new DocumentView(document2, null, presenter),
                                                                      "Document 1");

            Assert.AreEqual(false, renameSuccessful);
            Assert.AreEqual("Document 2", document2.Name);
        }

        [Test]
        public void RenamingToDuplicateFormNameProhibited()
        {
            Project.Current.AddForm();
            IForm form2 = Project.Current.AddForm();

            bool renameSuccessful = presenter.FormRenameRequested(new FormView(form2), "Form 1");

            Assert.AreEqual(false, renameSuccessful);
            Assert.AreEqual("Form 2", form2.Name);
        }

        [Test]
        public void RenamingToDuplicateProcessNameProhibited()
        {
            Project.Current.AddProcess();
            IProcess process2 = Project.Current.AddProcess();

            bool renameSuccessful = presenter.ProcessRenameRequested(new ProcessView(process2, null, presenter),
                                                                     "Process 1");

            Assert.AreEqual(false, renameSuccessful);
            Assert.AreEqual("Process 2", process2.Name);
        }
    }
}