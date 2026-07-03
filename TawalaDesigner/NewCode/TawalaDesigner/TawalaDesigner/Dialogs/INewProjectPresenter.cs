// Copyright © 2005 - 2008  Tawala Systems, Inc. All rights reserved.

using System.Windows.Forms;

namespace TawalaDesigner.Dialogs
{
	public interface INewProjectPresenter
	{
		void Initialize(string templatesPath);
		void PopulateCategories();
		void SelectCategory(string categoryId);
		void SelectedTemplateChanged();
		bool EmptyTemplateSelected();
		string OK(ListViewItem lviSelected);
	}
}
