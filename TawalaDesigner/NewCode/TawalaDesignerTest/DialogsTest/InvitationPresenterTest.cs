using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using NMock2;

using Tawala.Dialogs;
using Tawala.Projects;
using Tawala.Projects.Forms;

using TawalaTest.TestingSupport;

namespace TawalaTest.DialogsTest
{
    [TestFixture]
    public class InvitationPresenterTest
    {
        IInvitationPresenter presenter;
        InvitationField invitation;
        IInvitationView view;
        Mockery mockery = new Mockery();

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
            mockery = new Mockery();
            invitation = new InvitationField();
            view = mockery.NewMock<IInvitationView>();
        }

        [TearDown]
        public void TearDown()
        {
            mockery.VerifyAllExpectationsHaveBeenMet();
            invitation = null;
            view = null;
            presenter = null;
            mockery.Dispose();
            mockery = null;
        }

        [Test]
        public void PresenterConstructorCanInitializeContentsOfViewFromNewInvitation()
        {
            NMock2.Expect.Once.On(view).EventAdd("HandleDestroyed", new NMock2.Matchers.AlwaysMatcher(true, "HandleDestroyed event handler"));

            NMock2.Expect.Once.On(view).SetProperty("InitialFormName").To("");
            NMock2.Expect.Once.On(view).SetProperty("ProjectName").To("");
            NMock2.Expect.Once.On(view).SetProperty("DisplayText").To(invitation.DisplayText);
            NMock2.Expect.Once.On(view).SetProperty("IsPrivate").To(invitation.IsPrivate);
            NMock2.Expect.Once.On(view).SetProperty("AuthenticationTokenExpression").To(invitation.AuthenticationTokenExpression);

            presenter = new InvitationPresenter(view, invitation);
        }

        [Test]
        public void PresenterConstructorCanInitializeContentsOfViewFromExistingPrivateInvitation()
        {
            invitation.DisplayText = "Display Text";
            invitation.IsPrivate = true;
            invitation.AuthenticationTokenExpression = new CompoundExpression("<<_InviteeID>>");
            
            NMock2.Expect.Once.On(view).EventAdd("HandleDestroyed", new NMock2.Matchers.AlwaysMatcher(true, "HandleDestroyed event handler"));

            NMock2.Expect.Once.On(view).SetProperty("InitialFormName").To(invitation.InitialFormName);
            NMock2.Expect.Once.On(view).SetProperty("ProjectName").To("");
            NMock2.Expect.Once.On(view).SetProperty("DisplayText").To(invitation.DisplayText);
            NMock2.Expect.Once.On(view).SetProperty("IsPrivate").To(invitation.IsPrivate);
            NMock2.Expect.Once.On(view).SetProperty("AuthenticationTokenExpression").To(invitation.AuthenticationTokenExpression);

            presenter = new InvitationPresenter(view, invitation);
        }

        [Test]
        public void PresenterApplyChangesToCreatePrivateInvitation()
        {
            IForm form = Project.Current.AddForm();

            invitation.DisplayText = "Display Text";

            NMock2.Expect.Once.On(view).EventAdd("HandleDestroyed", new NMock2.Matchers.AlwaysMatcher(true, "HandleDestroyed event handler"));

            NMock2.Expect.Once.On(view).SetProperty("InitialFormName").To(invitation.InitialFormName);
            NMock2.Expect.Once.On(view).SetProperty("ProjectName").To("");
            NMock2.Expect.Once.On(view).SetProperty("DisplayText").To(invitation.DisplayText);
            NMock2.Expect.Once.On(view).SetProperty("IsPrivate").To(invitation.IsPrivate);
            NMock2.Expect.Once.On(view).SetProperty("AuthenticationTokenExpression").To(invitation.AuthenticationTokenExpression);

            presenter = new InvitationPresenter(view, invitation);

            NMock2.Expect.Once.On(view).GetProperty("InitialFormName").Will(Return.Value(form.Name));
            NMock2.Expect.Once.On(view).GetProperty("ProjectName").Will(Return.Value("(Current Project)"));
            NMock2.Expect.Once.On(view).GetProperty("DisplayText").Will(Return.Value("Display Text 2"));
            NMock2.Expect.Once.On(view).GetProperty("IsPrivate").Will(Return.Value(true));
            NMock2.Expect.Once.On(view).GetProperty("AuthenticationTokenExpression").Will(Return.Value(new CompoundExpression("<<_InviteeID>>")));

            presenter.ApplyChanges();

            Assert.AreEqual(form.Name, invitation.InitialFormName);
            Assert.AreEqual(form.Name, invitation.FormName);
            Assert.AreEqual(form, invitation.Form);
            Assert.AreEqual(string.Empty, invitation.ProjectName);
            Assert.AreEqual("Display Text 2", invitation.DisplayText);
            Assert.AreEqual(true, invitation.IsPrivate);
            Assert.AreEqual("<<_InviteeID>>", invitation.AuthenticationTokenExpression.ToString());
        }
    }
}
