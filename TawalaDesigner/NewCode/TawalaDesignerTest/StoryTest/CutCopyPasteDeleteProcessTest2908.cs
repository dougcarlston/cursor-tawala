using System;
using System.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Tawala.Interfaces;
using Tawala.ProcessDesigner;
using Tawala.ProjectExplorer;
using Tawala.Projects;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest
{
    [TestFixture]
    public class CutCopyPasteDeleteProcessTest2908
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

            new FieldsPalette();
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
        private IProcessView processView;

        [Test]
        public void CopyingProcessInPresenterPlacesProcessOnClipboard()
        {
            IProcess process = Project.Current.AddProcess();

            presenter.ProcessCopyRequested(new ProcessView(process, null, presenter));

            Assert.AreEqual(true, Clipboard.GetDataObject().GetDataPresent(typeof(Process)));
        }

        [Test]
        public void CuttingProcessInPresenterCutsProcessInModel()
        {
            IProcess process = Project.Current.AddProcess();

            presenter.ProcessCutRequested(new ProcessView(process, null, presenter));

            Assert.AreEqual(0, Project.Current.ProcessList.Count);
        }

        [Test]
        public void CuttingProcessInPresenterCutsProcessInView()
        {
            IProcess process = Project.Current.AddProcess();
            processView = mocks.DynamicMock<IProcessView>();

            Expect.Call(processView.Presenter).PropertyBehavior();
            Expect.Call(delegate { view.CutProcess(process.Name); });

            mocks.ReplayAll();
            processView.Presenter = new ProcessPresenter(processView, process);

            presenter.ProcessCutRequested(processView);

            mocks.VerifyAll();
        }

        [Test]
        public void CuttingProcessInPresenterPlacesProcessOnClipboard()
        {
            IProcess process = Project.Current.AddProcess();

            presenter.ProcessCutRequested(new ProcessView(process, null, presenter));

            Assert.AreEqual(true, Clipboard.GetDataObject().GetDataPresent(typeof(Process)));
        }

        [Test]
        public void DeletingProcessInPresenterDeletesProcessInModel()
        {
            IProcess process = Project.Current.AddProcess();

            presenter.ProcessDeleteRequested(new ProcessView(process, null, presenter));

            Assert.AreEqual(0, Project.Current.ProcessList.Count);
        }

        [Test]
        public void DeletingProcessInPresenterDeletesProcessInView()
        {
            IProcess process = Project.Current.AddProcess();
            processView = mocks.DynamicMock<IProcessView>();

            Expect.Call(processView.Presenter).PropertyBehavior();
            Expect.Call(delegate { view.DeleteProcess(process.Name); });

            mocks.ReplayAll();
            processView.Presenter = new ProcessPresenter(processView, process);

            presenter.ProcessDeleteRequested(processView);

            mocks.VerifyAll();
        }

        [Test]
        public void PastingProcessInPresenterPastesProcessInModel()
        {
            IProcess process = Project.Current.AddProcess();

            presenter.ProcessCutRequested(new ProcessView(process, null, presenter));
            presenter.ProcessPasteRequested();

            Assert.AreEqual(1, Project.Current.ProcessList.Count);
        }

        [Test]
        public void PastingProcessInPresenterPastesProcessInView()
        {
            IProcess process = Project.Current.AddProcess();
            processView = mocks.DynamicMock<IProcessView>();

            Expect.Call(processView.Presenter).PropertyBehavior();
            Expect.Call(delegate { view.PasteProcess(processView); }).IgnoreArguments();

            mocks.ReplayAll();
            processView.Presenter = new ProcessPresenter(processView, process);

            presenter.ProcessCutRequested(processView);
            presenter.ProcessPasteRequested();

            mocks.VerifyAll();
        }

        [Test]
        public void SelectingProcessEnablesProcessCopy()
        {
            IProcess process = Project.Current.AddProcess();
            IProcessView processView = new ProcessView(process, null, presenter);

            Expect.Call(view.SelectedComponent).Return(processView);
            mocks.ReplayAll();

            presenter.ProcessSelected(processView);

            Assert.IsTrue(presenter.CanCopyProcess(processView));

            mocks.VerifyAll();
        }

        [Test]
        public void SelectingProcessEnablesProcessCut()
        {
            IProcess process = Project.Current.AddProcess();
            IProcessView processView = new ProcessView(process, null, presenter);

            Expect.Call(view.SelectedComponent).Return(processView);
            mocks.ReplayAll();

            presenter.ProcessSelected(processView);

            Assert.IsTrue(presenter.CanCutProcess(processView));

            mocks.VerifyAll();
        }

        [Test]
        public void SelectingProcessEnablesProcessDelete()
        {
            IProcess process = Project.Current.AddProcess();
            IProcessView processView = new ProcessView(process, null, presenter);

            Expect.Call(view.SelectedComponent).Return(processView);
            mocks.ReplayAll();

            presenter.ProcessSelected(processView);

            Assert.IsTrue(presenter.CanDeleteProcess(processView));

            mocks.VerifyAll();
        }
    }
}