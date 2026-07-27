// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NMock2;
using TawalaTest.TestingSupport;

using TawalaDesigner.Dialogs;
using Tawala.Projects;

namespace TawalaTest.DialogsTest
{
	[TestFixture]
	public class ProjectDeployedPresenterTest
	{
		private Mockery mocks;
		private IProjectDeployedView view;
		private IProjectDeployedPresenter presenter;

		[SetUp]
		public void SetUp()
		{
			mocks = new Mockery();

			view = mocks.NewMock<IProjectDeployedView>();
			presenter = new ProjectDeployedPresenter(view);

			Util.NewTestProject();
			ComponentMaker.UseNewComponents(true);
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAllExpectationsHaveBeenMet();
			ComponentMaker.UseNewComponents(false);
		}

		[Test]
		public void ClickingCloseButtonHidesView()
		{
			Expect.Once.On(view).Method("Hide");
			Stub.On(view).Method(Is.Anything);

			presenter.CloseRequested();
		}

		[Test]
		public void ClickingViewSelectedFormLinkLaunchesBrowser()
		{
			Stub.On(view).Method(Is.Anything);

			//presenter.FormViewRequested("www.google.com");
		}
	}
}
