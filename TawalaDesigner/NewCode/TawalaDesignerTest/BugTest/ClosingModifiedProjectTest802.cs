using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Tawala.Interfaces;
using Tawala.MainApplication;
using Tawala.ProjectExplorer;
using Tawala.Projects;
using Tawala.Common;
using TawalaTest.TestingSupport;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class ClosingModifiedProjectTest802
	{
		private MockRepository mocks;
		private IProjectExplorerView view;
		private IProjectExplorerPresenter presenter;

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

		[Test]
		public void AddingFormFlagsProjectAsModified()
		{
			Assert.AreEqual(false, presenter.ProjectHasBeenModified);

			Project.Current.AddForm();

			Assert.AreEqual(true, presenter.ProjectHasBeenModified);
		}

		private string message = "The project may have changed.  Do you want to save the changes?";
		private string caption = "NUnit";

		[Test]
		public void ClosingModifiedProjectShowsProjectModifiedMessageBox()
		{
			Expect.Call(view.ShowMessageBox(message, caption)).Return(DialogResult.Cancel);
			mocks.ReplayAll();

			Project.Current.AddForm();
			presenter.ProjectCloseRequested();

			mocks.VerifyAll();
		}

		[Test]
		public void ClosingUnmodifiedProjectDoesNotShowProjectModifiedMessageBox()
		{
			mocks.ReplayAll();

			presenter.ProjectCloseRequested();

			mocks.VerifyAll();
		}

		[Test]
		public void ClickingNoInProjectModifiedMessageBoxAllowsProjectClosure()
		{
			Expect.Call(view.ShowMessageBox(message, caption)).Return(DialogResult.No);
			mocks.ReplayAll();

			Project.Current.AddForm();

			Assert.AreEqual(true, presenter.ProjectCloseRequested());

			mocks.VerifyAll();
		}

		[Test]
		public void ClickingCancelInProjectModifiedMessageBoxDisallowsProjectClosure()
		{
			Expect.Call(view.ShowMessageBox(message, caption)).Return(DialogResult.Cancel);
			mocks.ReplayAll();

			Project.Current.AddForm();

			Assert.AreEqual(false, presenter.ProjectCloseRequested());

			mocks.VerifyAll();
		}

		[Test]
		public void ClickingOkInSaveFileDialogAllowsProjectClosure()
		{
			Expect.Call(view.ShowMessageBox(message, caption)).Return(DialogResult.Yes);
			Expect.Call(view.ShowSaveFileDialog()).Return(new SaveFileDialogResult(DialogResult.OK, "UserSelectedFileName"));
			mocks.ReplayAll();

			Project.Current.AddForm();

			Assert.AreEqual(true, presenter.ProjectCloseRequested());

			mocks.VerifyAll();
		}

		[Test]
		public void ClickingCancelInSaveFileDialogDisallowsProjectClosure()
		{
			Expect.Call(view.ShowMessageBox(message, caption)).Return(DialogResult.Yes);
			Expect.Call(view.ShowSaveFileDialog()).Return(new SaveFileDialogResult(DialogResult.Cancel, "UserSelectedFileName"));
			mocks.ReplayAll();

			Project.Current.AddForm();

			Assert.AreEqual(false, presenter.ProjectCloseRequested());

			mocks.VerifyAll();
		}

		[Test]
		public void ClickingYesInProjectModifiedMessageBoxWithUnnamedProjectShowsSaveFileDialog()
		{
			Expect.Call(view.ShowMessageBox(message, caption)).Return(DialogResult.Yes);
			Expect.Call(view.ShowSaveFileDialog()).Return(new SaveFileDialogResult(DialogResult.OK, "UserSelectedFileName"));
			mocks.ReplayAll();

			Project.Current.AddForm();
			presenter.ProjectFilePath = null;
			presenter.ProjectCloseRequested();

			mocks.VerifyAll();
		}

		[Test]
		public void ClickingYesInProjectModifiedMessageBoxWithNamedProjectDoesNotShowSaveFileDialog()
		{
			Expect.Call(view.ShowMessageBox(message, caption)).Return(DialogResult.Yes);
			mocks.ReplayAll();

			Project.Current.AddForm();
			presenter.ProjectFilePath = "My Project";
			presenter.ProjectCloseRequested();

			mocks.VerifyAll();
		}

		[Test]
		public void OpeningProjectFlagsProjectAsUnmodified()
		{
			presenter.ProjectOpenRequested(Util.GetTestFilePath("OneForm.tawala"));

			Assert.AreEqual(false, presenter.ProjectHasBeenModified);
		}
	}
}
