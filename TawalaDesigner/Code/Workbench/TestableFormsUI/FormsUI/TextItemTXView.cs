using System;
using System.Windows.Forms;
using TXTextControl;

namespace Tawala.FormsUI
{
	public class TextItemTXView : TextControl, ITextItemView
	{
		TextItemPresenter presenter;

		public TextItemTXView(ITextItemModel model)
		{
			presenter = new TextItemPresenter(this, model);
		}

		public string PlainText
		{
			get
			{
				return Text;
			}
			set
			{
				Text = value;
			}
		}
	}
}
