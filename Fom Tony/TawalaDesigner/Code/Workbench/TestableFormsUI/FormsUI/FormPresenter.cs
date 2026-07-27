using System;
using System.Collections.ObjectModel;

namespace Tawala.FormsUI
{
	public class FormPresenter
	{
		private readonly IFormView view;

		public FormPresenter(IFormView view)
		{
			this.view = view;
		}

		public void Add(ITextItemView textItemView)
		{
			items.Add(textItemView);
		}

		private Collection<ITextItemView> items = new Collection<ITextItemView>();

		public Collection<ITextItemView> Items
		{
			get
			{
				return items;
			}
		}
	}
}
