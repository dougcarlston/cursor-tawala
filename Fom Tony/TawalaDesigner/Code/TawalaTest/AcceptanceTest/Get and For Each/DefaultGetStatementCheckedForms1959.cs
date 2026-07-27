// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NMock2;
using NUnit.Framework;
using Tawala.Processes;
using Tawala.Projects;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.GetAndForEach
{
    [TestFixture]
    public class DefaultGetStatementForms1959
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();
            mockery = new Mockery();
            mockView = mockery.NewMock<IStatementView>();
        }

        [TearDown]
        public void TearDown()
        {
            mockView = null;
            mockery = null;
        }

        #endregion

        private Mockery mockery;
        private IStatementView mockView;

        [Test]
        public void DefaultFormListIsEmptyWhenConnected()
        {
            IForm form1 = Project.Current.AddForm();
            IForm form2 = Project.Current.AddForm();
            Process process = Project.Current.AddProcess();
            form2.ConnectedPostProcess = process;

            Stub.On(mockView).GetProperty("Process").Will(Return.Value(process));

            var presenter = new GetStatementPresenter(mockView);
            FormList list = presenter.GetDefaultCheckedFormList();

            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void DefaultFormListIsEmptyWhenNotConnected()
        {
            IForm form1 = Project.Current.AddForm();
            IForm form2 = Project.Current.AddForm();
            Process process = Project.Current.AddProcess();

            Stub.On(mockView).GetProperty("Process").Will(Return.Value(process));

            var presenter = new GetStatementPresenter(mockView);
            FormList list = presenter.GetDefaultCheckedFormList();

            Assert.AreEqual(0, list.Count);
        }
    }
}