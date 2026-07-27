using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using NMock2;

using Tawala.Dialogs;
using Tawala.Projects;

using TawalaTest.TestingSupport;

namespace TawalaTest.DialogsTest
{
    [TestFixture]
    public class HyperlinkPresenterTest
    {
        IHyperlinkPresenter presenter;
        Hyperlink hyperlink;
        IHyperlinkView view;
        Mockery mockery = new Mockery();

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
            mockery = new Mockery();
            hyperlink = new Hyperlink();
            view = mockery.NewMock<IHyperlinkView>();
        }

        [TearDown]
        public void TearDown()
        {
            mockery.VerifyAllExpectationsHaveBeenMet();
            hyperlink = null;
            view = null;
            presenter = null;
            mockery.Dispose();
            mockery = null;
        }

        [Test]
        public void PresenterConstructorCanInitializeContentsOfViewFromNewHyperlink()
        {
            NMock2.Expect.Once.On(view).EventAdd("HandleDestroyed", new NMock2.Matchers.AlwaysMatcher(true, "HandleDestroyed event handler"));
            NMock2.Expect.Once.On(view).SetProperty("DisplayText").To(hyperlink.DisplayText);
            NMock2.Expect.Once.On(view).SetProperty("Url").To(hyperlink.Url);
            NMock2.Expect.Once.On(view).SetProperty("NewWindow").To(hyperlink.OpenNewWindow);

            presenter = new HyperlinkPresenter(view, hyperlink);
        }

        [Test]
        public void PresenterConstructorCanInitializeContentsOfViewFromExistingHyperlink()
        {
            hyperlink.DisplayText = "Display Text";
            hyperlink.Url = "<<_InviteeID>>";
            hyperlink.OpenNewWindow = true;

            NMock2.Expect.Once.On(view).EventAdd("HandleDestroyed", new NMock2.Matchers.AlwaysMatcher(true, "HandleDestroyed event handler"));
            NMock2.Expect.Once.On(view).SetProperty("DisplayText").To(hyperlink.DisplayText);
            NMock2.Expect.Once.On(view).SetProperty("Url").To(hyperlink.Url);
            NMock2.Expect.Once.On(view).SetProperty("NewWindow").To(hyperlink.OpenNewWindow);

            presenter = new HyperlinkPresenter(view, hyperlink);

            NMock2.Expect.Once.On(view).GetProperty("DisplayText").Will(Return.Value(hyperlink.DisplayText));
            NMock2.Expect.Once.On(view).GetProperty("Url").Will(Return.Value(hyperlink.Url));
            NMock2.Expect.Once.On(view).GetProperty("NewWindow").Will(Return.Value(hyperlink.OpenNewWindow));

            presenter.ApplyChanges();

        }
    }
}
