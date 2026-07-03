using System;
using System.Windows.Forms;
using NUnit.Framework;
using NMock2;
using Tawala.FormsUI;

namespace TawalaTest.AcceptanceTest
{
	[TestFixture]
	public class AddTextItemTest1862
	{
		private Mockery mocks;
		private ITextItemView mockTextItemView;
		private ITextItemModel mockTextItemModel;

		[SetUp]
		public void SetUp()
		{
			mocks = new Mockery();
			mockTextItemView = mocks.NewMock<ITextItemView>();
			mockTextItemModel = mocks.NewMock<ITextItemModel>();
		}

		[Test]
		public void TextItemCanBeAddedToForm()
		{
			IFormView mockFormView = mocks.NewMock<IFormView>();
			FormPresenter formPresenter = new FormPresenter(mockFormView);

			formPresenter.Add(mockTextItemView);

			Assert.AreEqual(1, formPresenter.Items.Count);
			Assert.AreSame(mockTextItemView, formPresenter.Items[0]);
		}

		[Test]
		public void NewTextItemContainsDefaultText()
		{
			Expect.Once.On(mockTextItemModel).GetProperty("Text").Will(Return.Value("[Replace this with text of your own.]"));

			ITextItemView textItemView = new TextItemTXView(mockTextItemModel);

			Assert.AreEqual("[Replace this with text of your own.]", textItemView.PlainText);

			mocks.VerifyAllExpectationsHaveBeenMet();
		}


		// ctrl+a selects all text
	}
}
