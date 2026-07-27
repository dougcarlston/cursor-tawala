using System;
using NUnit.Framework;
using Rhino.Mocks;
using Tawala.Common;
using Tawala.Interfaces;
using Tawala.MainApplication;
using Tawala.ProjectExplorer;
using Tawala.Projects;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest
{
    [TestFixture]
    public class NewProjectWithTemplatesTest2915
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();

            view = mocks.CreateMock<IProjectExplorerView>();
            presenter = new ProjectExplorerPresenter(view);

            applicationView = mocks.CreateMock<IApplicationView>();
            ApplicationPresenter.View = applicationView;

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
        private IApplicationView applicationView;

        [Test]
        public void OpeningTemplateProjectLeavesProjectFolderUnchanged()
        {
            presenter.ProjectTemplateOpenRequested(Util.GetTestFilePath("Empty Project.tawala"));

            Assert.AreEqual(Config.DefaultProjectDirectory, presenter.ProjectFolder);
        }

        [Test]
        public void OpeningTemplateProjectSetsProjectNameToNull()
        {
            presenter.ProjectTemplateOpenRequested(Util.GetTestFilePath("Empty Project.tawala"));

            Assert.AreEqual(null, presenter.ProjectFilePath);
        }

        [Test]
        public void OpeningTemplateProjectSetsUntitledInApplicationTitleBar()
        {
            Expect.Call(delegate { applicationView.SetProjectNameInTitleBar("Untitled"); });
            Expect.Call(delegate { view.ClearProject(); });

            mocks.ReplayAll();
            presenter.ProjectTemplateOpenRequested(Util.GetTestFilePath("Empty Project.tawala"));

            mocks.VerifyAll();
        }
    }
}