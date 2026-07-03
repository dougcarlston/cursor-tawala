// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using NMock2;
using NUnit.Framework;
using Tawala.Common;
using Tawala.DocumentDesigner;
using Tawala.FormDesigner;
using Tawala.Interfaces;
using Tawala.MainApplication;
using Tawala.ProjectExplorer;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestingSupport;

namespace ProjectExplorerTest
{
    [TestFixture]
    public class ProjectExplorerPresenterTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new Mockery();

            view = mocks.NewMock<IProjectExplorerView>();
            presenter = new ProjectExplorerPresenter(view);

            applicationView = mocks.NewMock<IApplicationView>();
            ApplicationPresenter.View = applicationView;

            Util.NewTestProject();
            ComponentMaker.UseNewComponents(true);
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAllExpectationsHaveBeenMet();
            ComponentMaker.UseNewComponents(false);
        }

        #endregion

        private Mockery mocks;
        private IProjectExplorerView view;
        private IProjectExplorerPresenter presenter;
        private IApplicationView applicationView;

        private void stubViewMethods()
        {
            Stub.On(view).Method(Is.Anything);
            Stub.On(applicationView).Method(Is.Anything);
        }

        [Test]
        public void ClickingNewDocumentButtonActivatesDocumentDesigner()
        {
            stubViewMethods();

            presenter.NewDocumentRequested();

            Assert.IsInstanceOfType(typeof(TawalaDocumentDesigner), presenter.CurrentDesigner);
        }

        [Test]
        public void ClickingNewDocumentButtonAddsDocumentToProject()
        {
            stubViewMethods();

            presenter.NewDocumentRequested();

            Assert.AreEqual(1, Project.Current.DocumentList.Count);
        }

        [Test]
        public void ClickingNewDocumentButtonAddsDocumentToView()
        {
            Expect.Once.On(view).Method("AddDocument");
            stubViewMethods();

            presenter.NewDocumentRequested();
        }

        [Test]
        public void ClickingNewFormButtonActivatesFormDesigner()
        {
            stubViewMethods();

            presenter.NewFormRequested();

            Assert.IsInstanceOfType(typeof(TawalaFormDesigner), presenter.CurrentDesigner);
        }

        [Test]
        public void ClickingNewFormButtonAddsFormToProject()
        {
            Expect.Once.On(view).Method("AddForm");
            stubViewMethods();

            presenter.NewFormRequested();

            Assert.AreEqual(1, Project.Current.FormList.Count);
        }

        [Test]
        public void ClickingNewFormButtonAddsFormToView()
        {
            Expect.Once.On(view).Method("AddForm");
            stubViewMethods();

            presenter.NewFormRequested();
        }

        [Test]
        [Ignore("Ignored because lack of Fields Palette currently throws exception")]
        public void ClickingNewProcessButtonAddsProcessToProject()
        {
            stubViewMethods();

            presenter.NewProcessRequested();

            Assert.AreEqual(1, Project.Current.ProcessList.Count);
        }

        [Test]
        [Ignore("Ignored because lack of Fields Palette currently throws exception")]
        public void ClickingNewProcessButtonAddsProcessToView()
        {
            Expect.Once.On(view).Method("AddProcess");

            presenter.NewProcessRequested();
        }

        [Test]
        public void ClickingPrePopulateMenuItemTogglesPrePopulateInModel()
        {
            IForm form = Project.Current.AddForm();
            Assert.AreEqual(false, form.DataEntryOnly);

            stubViewMethods();

            presenter.FormSelected(new FormView(form));

            presenter.PrePopulateToggleRequested();

            Assert.AreEqual(true, form.DataEntryOnly);
        }

        [Test]
        public void ClickingStartingPointButtonChangesFormIconInView()
        {
            IForm form = Project.Current.AddForm();
            Assert.AreEqual(true, form.StartingPoint);

            Expect.Once.On(view).Method("SetStartingPoint");
            stubViewMethods();

            presenter.FormSelected(new FormView(form));

            presenter.StartingPointToggleRequested();
        }

        [Test]
        public void ClickingStartingPointButtonTogglesStartingPointInModel()
        {
            IForm form = Project.Current.AddForm();
            Assert.AreEqual(true, form.StartingPoint);

            stubViewMethods();

            presenter.FormSelected(new FormView(form));

            presenter.StartingPointToggleRequested();

            Assert.AreEqual(false, form.StartingPoint);
        }

        [Test]
        public void ConnectingPreProcessDisablesPreProcessConnectability()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            stubViewMethods();

            IFormView formView = new FormView(form);
            presenter.PreProcessConnectionRequested(formView, "Process 1");

            Assert.AreEqual(false, presenter.CanConnectPreProcess(formView));
        }

        [Test]
        public void ConnectingPreProcessEnablesPreProcessDisconnectability()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            stubViewMethods();

            IFormView formView = new FormView(form);
            presenter.PreProcessConnectionRequested(formView, "Process 1");

            Assert.AreEqual(true, presenter.CanDisconnectPreProcess(formView));
        }

        [Test]
        public void ConnnectingPreProcessInPresenterConnectsPreProcessInModel()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            stubViewMethods();

            presenter.PreProcessConnectionRequested(new FormView(form), "Process 1");

            Assert.AreEqual("Process 1", Project.Current.FormList[0].ConnectedPreProcess.Name);
        }

        [Test]
        public void ConnnectingPreProcessInPresenterConnectsPreProcessInView()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            Expect.Once.On(view).Method("ConnectPreProcess");
            stubViewMethods();

            presenter.PreProcessConnectionRequested(new FormView(form), "Process 1");
        }

        [Test]
        public void DisconnnectingPreProcessInPresenterDisconnectsPreProcessInModel()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            stubViewMethods();

            IFormView formView = new FormView(form);
            presenter.PreProcessConnectionRequested(formView, "Process 1");
            presenter.PreProcessDisconnectionRequested(formView);

            Assert.AreEqual(null, Project.Current.FormList[0].ConnectedPreProcess);
        }

        [Test]
        public void DisconnnectingPreProcessInPresenterDisconnectsPreProcessInView()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            Expect.Once.On(view).Method("DisconnectPreProcess");
            stubViewMethods();

            IFormView formView = new FormView(form);
            presenter.PreProcessConnectionRequested(formView, "Process 1");
            presenter.PreProcessDisconnectionRequested(formView);
        }

        [Test]
        public void ExistenceOfProcessEnablesPreProcessConnectability()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            stubViewMethods();

            Assert.AreEqual(true, presenter.CanConnectPreProcess(new FormView(form)));
        }

        [Test]
        public void OpeningProjectFileClearsProjectInView()
        {
            Expect.Once.On(view).Method("ClearProject");
            stubViewMethods();

            presenter.ProjectOpenRequested(Util.GetTestFilePath("ThreeProcesses.xml"));
        }

        [Test]
        public void OpeningProjectFilePopulatesProjectWithDocuments()
        {
            stubViewMethods();

            presenter.ProjectOpenRequested(Util.GetTestFilePath("ThreeDocuments.xml"));

            Assert.AreEqual(3, Project.Current.DocumentList.Count);
        }

        [Test]
        public void OpeningProjectFilePopulatesProjectWithForms()
        {
            stubViewMethods();

            presenter.ProjectOpenRequested(Util.GetTestFilePath("ThreeForms.xml"));

            Assert.AreEqual(3, Project.Current.FormList.Count);
        }

        [Test]
        public void OpeningProjectFilePopulatesViewWithConnectedProcess()
        {
            Expect.Once.On(view).Method("ConnectPreProcess");
            stubViewMethods();

            presenter.ProjectOpenRequested(Util.GetTestFilePath("OneConnectedPreProcess.xml"));
        }

        [Test]
        public void OpeningProjectFilePopulatesViewWithDocuments()
        {
            Expect.Exactly(3).On(view).Method("AddDocument");
            stubViewMethods();

            presenter.ProjectOpenRequested(Util.GetTestFilePath("ThreeDocuments.xml"));
        }

        [Test]
        public void OpeningProjectFilePopulatesViewWithForms()
        {
            Expect.Once.On(view).Method("ClearProject");
            Expect.Exactly(3).On(view).Method("AddForm");
            stubViewMethods();

            presenter.ProjectOpenRequested(Util.GetTestFilePath("ThreeForms.xml"));
        }

        [Test]
        public void OpeningProjectFilePopulatesViewWithProcesses()
        {
            Expect.Exactly(3).On(view).Method("AddProcess");
            stubViewMethods();

            presenter.ProjectOpenRequested(Util.GetTestFilePath("ThreeProcesses.xml"));
        }

        [Test]
        public void OpeningProjectFileUpdatesApplicationTitleBar()
        {
            Expect.Once.On(applicationView).Method("SetProjectNameInTitleBar").With("Test");
            stubViewMethods();

            string filePath = Util.GetTestFilePath("Test");
            presenter.ProjectOpenRequested(filePath);
        }

        [Test]
        public void OpeningProjectFileUpdatesProjectFilePath()
        {
            stubViewMethods();

            Assert.AreEqual("Untitled", Project.Current.Name);

            string filePath = Util.GetTestFilePath("Test");
            presenter.ProjectOpenRequested(filePath);

            Assert.AreEqual(filePath, presenter.ProjectFilePath);
        }

        [Test]
        public void OpeningProjectFileUpdatesProjectFolder()
        {
            stubViewMethods();

            Assert.AreEqual(Config.DefaultProjectDirectory, presenter.ProjectFolder);

            string filePath = Util.GetTestFilePath("Test");
            presenter.ProjectOpenRequested(filePath);

            string folderPath = Path.GetDirectoryName(filePath);

            Assert.AreEqual(folderPath, presenter.ProjectFolder);
        }

        [Test]
        public void RequestingNewProjectResetsProjectFilePath()
        {
            stubViewMethods();

            string filePath = Util.GetTestFilePath("Test");
            presenter.ProjectSaveRequested(filePath);
            Assert.AreEqual(filePath, presenter.ProjectFilePath);

            presenter.NewProjectRequested();
            Assert.AreEqual(null, presenter.ProjectFilePath);
        }

        [Test]
        public void RequestingNewProjectUpdatesApplicationTitleBar()
        {
            Expect.Once.On(applicationView).Method("SetProjectNameInTitleBar").With("");
            stubViewMethods();

            presenter.NewProjectRequested();
        }

        [Test]
        public void SavingProjectFileUpdatesApplicationTitleBar()
        {
            Expect.Once.On(applicationView).Method("SetProjectNameInTitleBar").With("Test");
            stubViewMethods();

            presenter.ProjectSaveRequested(Util.GetTestFilePath("Test"));
        }

        [Test]
        public void SavingProjectFileUpdatesProjectFilePath()
        {
            stubViewMethods();

            Assert.AreEqual("Untitled", Project.Current.Name);

            string filePath = Util.GetTestFilePath("Test");
            presenter.ProjectSaveRequested(filePath);

            Assert.AreEqual(filePath, presenter.ProjectFilePath);
        }

        [Test]
        public void SavingProjectFileUpdatesProjectFolder()
        {
            stubViewMethods();

            Assert.AreEqual(Config.DefaultProjectDirectory, presenter.ProjectFolder);

            string filePath = Util.GetTestFilePath("Test");
            presenter.ProjectSaveRequested(filePath);

            string folderPath = Path.GetDirectoryName(filePath);

            Assert.AreNotEqual(folderPath, Config.DefaultProjectDirectory);
            Assert.AreEqual(folderPath, presenter.ProjectFolder);
        }

        [Test]
        public void SavingProjectFileUpdatesProjectName()
        {
            stubViewMethods();

            Assert.AreEqual("Untitled", presenter.ProjectName);

            presenter.ProjectSaveRequested(Util.GetTestFilePath("Test"));

            Assert.AreEqual("Test", presenter.ProjectName);
        }

        [Test]
        public void SelectingDocumentActivatesDocumentDesigner()
        {
            stubViewMethods();

            presenter.DocumentSelected(new DocumentView(Project.Current.AddDocument()));

            Assert.IsInstanceOfType(typeof(TawalaDocumentDesigner), presenter.CurrentDesigner);
        }

        [Test]
        public void SelectingFormActivatesFormDesigner()
        {
            stubViewMethods();

            presenter.FormSelected(new FormView(Project.Current.AddForm()));

            Assert.IsInstanceOfType(typeof(TawalaFormDesigner), presenter.CurrentDesigner);
        }
    }
}