using System;

namespace Tawala.FormsUI
{
	public class TextItemPresenter
	{
		private readonly ITextItemView view;
		private readonly ITextItemModel model;

		public TextItemPresenter(ITextItemView view, ITextItemModel model)
		{
			this.view = view;
			this.model = model;

			setTextInView();
		}

		private void setTextInView()
		{
			view.PlainText = model.Text;
		}
	}
}
