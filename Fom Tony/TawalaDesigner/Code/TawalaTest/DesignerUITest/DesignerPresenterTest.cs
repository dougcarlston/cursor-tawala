using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using Tawala.DesignerUI;
using Tawala.ProjectUI;
using TawalaTest.TestSupport;

namespace TawalaTest.DesignerUITest
{
	[TestFixture]
	public class DesignerPresenterTest
	{
		private IDesignerPresenter presenter;
		private IDesignerView view;
		private NMock2.Mockery mockery;

		[SetUp]
		public void Setup()
		{
			mockery = new NMock2.Mockery();
			view = mockery.NewMock<IDesignerView>();
			NMock2.Stub.On(view).GetProperty("Handle").Will(NMock2.Return.Value(null));
			NMock2.Stub.On(view).GetProperty("Text").Will(NMock2.Return.Value("Designer"));

			presenter = DesignerPresenter.Create(view);

			NMock2.Stub.On(view).GetProperty("Presenter").Will(NMock2.Return.Value(presenter));
		}

		[Test]
		public void _CreatePresenter()
		{
			Assert.IsNotNull(presenter);
		}
	}
}
