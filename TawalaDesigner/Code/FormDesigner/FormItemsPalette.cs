// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.Interfaces;

namespace Tawala.FormDesigner
{
	public partial class FormItemsPalette : UserControl, IFormItemsPalette
	{
		public FormItemsPalette()
		{
			InitializeComponent();
		}

		private void listViewFormItems_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			invokeInsertMethod(FormPresenter.InsertAtEnd);
		}

		private void invokeInsertMethod(int index)
		{
			if (listViewFormItems.SelectedItems.Count == 1)
			{
				ListViewItem listViewItem = listViewFormItems.SelectedItems[0];
				IFormPresenter presenter = activeFormEditPresenter();

				if (presenter != null)
				{

					string insertMethodName = listViewItem.Tag as string;

					if (insertMethodName != null && presenter != null)
					{
						insertMethodName = "Insert" + insertMethodName;
						presenter.GetType().InvokeMember(insertMethodName, System.Reflection.BindingFlags.InvokeMethod, null, presenter, new object[] { index });
					}
				}
			}
		}

		private static IFormPresenter activeFormEditPresenter()
		{
			//MainWindow window = Application.OpenForms[0] as MainWindow;
			Form window = Application.OpenForms[0];
			IFormView view = window.ActiveMdiChild as IFormView;
			return view != null ? view.Presenter as IFormPresenter : null;
		}

		private void listViewFormItems_ItemDrag(object sender, ItemDragEventArgs e)
		{
			ListViewItem lvi = e.Item as ListViewItem;
			IFormPresenter presenter = activeFormEditPresenter();

			if (lvi != null && lvi.Tag != null && presenter != null)
			{
				DataObject dataObject = new DataObject(DataFormats.Text, "#NewFormItem#");
				if (listViewFormItems.DoDragDrop(dataObject, DragDropEffects.Copy) == DragDropEffects.Copy)
				{
					invokeInsertMethod(FormPresenter.InsertAtInsertionPoint);
				}
				presenter.View.NotifyHtmlDragDropEnded();
			}
		}

		public void UpdateChoiceIconsInFormView(Control c)
		{
			IFormPresenter presenter = activeFormEditPresenter();
			presenter.UpdateMcqOptionsInView(c);
		}
	}
}
