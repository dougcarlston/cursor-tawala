using System;
using NMock2;
using NUnit.Framework;
using Tawala.FormDesigner;
using Tawala.Interfaces;
using Tawala.ProjectExplorer;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest
{
    [TestFixture]
    public class ConnectDisconnectPostProcessTest2909
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new Mockery();

            view = mocks.NewMock<IProjectExplorerView>();
            presenter = new ProjectExplorerPresenter(view);

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

        [Test]
        public void ConnectingPostProcessDisablesPostProcessConnectability()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            Stub.On(view).Method(Is.Anything);

            IFormView formView = new FormView(form);
            presenter.PostProcessConnectionRequested(formView, "Process 1");

            Assert.AreEqual(false, presenter.CanConnectPostProcess(formView));
        }

        [Test]
        public void ConnectingPostProcessEnablesPostProcessDisconnectability()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            Stub.On(view).Method(Is.Anything);

            IFormView formView = new FormView(form);
            presenter.PostProcessConnectionRequested(formView, "Process 1");

            Assert.AreEqual(true, presenter.CanDisconnectPostProcess(formView));
        }

        [Test]
        public void ConnnectingPostProcessInPresenterConnectsPostProcessInModel()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            Stub.On(view).Method(Is.Anything);

            presenter.PostProcessConnectionRequested(new FormView(form), "Process 1");

            Assert.AreEqual("Process 1", Project.Current.FormList[0].ConnectedProcess.Name);
        }

        [Test]
        public void ConnnectingPostProcessInPresenterConnectsPostProcessInView()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            Expect.Once.On(view).Method("ConnectPostProcess");
            Stub.On(view).Method(Is.Anything);

            presenter.PostProcessConnectionRequested(new FormView(form), "Process 1");
        }

        [Test]
        public void DisconnnectingPostProcessInPresenterDisconnectsPostProcessInModel()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            Stub.On(view).Method(Is.Anything);

            IFormView formView = new FormView(form);
            presenter.PostProcessConnectionRequested(formView, "Process 1");
            presenter.PostProcessDisconnectionRequested(formView);

            Assert.AreEqual(null, Project.Current.FormList[0].ConnectedProcess);
        }

        [Test]
        public void DisconnnectingPostProcessInPresenterDisconnectsPostProcessInView()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            Expect.Once.On(view).Method("DisconnectPostProcess");
            Stub.On(view).Method(Is.Anything);

            IFormView formView = new FormView(form);
            presenter.PostProcessConnectionRequested(formView, "Process 1");
            presenter.PostProcessDisconnectionRequested(formView);
        }

        [Test]
        public void ExistenceOfProcessEnablesPostProcessConnectability()
        {
            IForm form = Project.Current.AddForm();
            IProcess process = Project.Current.AddProcess();

            Stub.On(view).Method(Is.Anything);

            Assert.AreEqual(true, presenter.CanConnectPostProcess(new FormView(form)));
        }

        [Test]
        public void OpeningProjectFilePopulatesViewWithConnectedPostProcess()
        {
            Expect.Once.On(view).Method("ConnectPostProcess");
            Stub.On(view).Method(Is.Anything);

            presenter.ProjectOpenRequested(Util.GetTestFilePath("OneConnectedPostProcess.xml"));
        }
    }
}