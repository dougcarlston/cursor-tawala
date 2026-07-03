using System;
using NUnit.Framework;
using NMock2;
using Tawala.FormsUI;

namespace TawalaTest.UnitTest
{
	[TestFixture]
	public class TextItemPresenterTest
	{
		private Mockery mocks;
		private ITextItemView mockView;
		private TextItemPresenter presenter;
		private ITextItemModel mockModel;

		[SetUp]
		public void SetUp()
		{
			mocks = new Mockery();
			mockView = mocks.NewMock<ITextItemView>();
			mockModel = mocks.NewMock<ITextItemModel>();
			presenter = new TextItemPresenter(mockView, mockModel);
		}

	}
}
