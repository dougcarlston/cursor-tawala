// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TawalaDesigner.Dialogs
{
	public partial class NewProjectDialog : Form, INewProjectView
	{
		private INewProjectPresenter presenter;

		public NewProjectDialog(string templatesPath)
		{
			InitializeComponent();

			presenter = new NewProjectPresenter(this);
			presenter.Initialize(templatesPath);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			presenter.PopulateCategories();
		}

		#region INewProjectView Members

		public TreeNode CategoryRootNode
		{
			get { return treeViewCategories.Nodes[0]; }
		}

		public ListView TemplateView
		{
			get { return listViewTemplates; }
		}

		public string TemplateDescription
		{
			get { return labelTemplateDescription.Text; }
			set { labelTemplateDescription.Text = value; }
		}

		public string SelectedTemplateFile
		{
			get { return selectedTemplateFile; }
		}

		#endregion

		private string selectedTemplateFile;

		private void treeViewCategories_AfterSelect(object sender, TreeViewEventArgs e)
		{
			presenter.SelectCategory(e.Node.Tag as string);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (listViewTemplates.SelectedItems.Count != 0)
			{
				selectedTemplateFile = presenter.OK(listViewTemplates.SelectedItems[0]);
			}
		}

		public bool EmptyTemplateSelected
		{
			get { return presenter.EmptyTemplateSelected(); }
		}


		private void buttonCancel_Click(object sender, EventArgs e)
		{
			selectedTemplateFile = null;
		}

		private void listViewTemplates_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			buttonOK.Enabled = listViewTemplates.SelectedItems.Count != 0;
			presenter.SelectedTemplateChanged();
		}

		private void listViewTemplates_DoubleClick(object sender, EventArgs e)
		{
			buttonOK.PerformClick();
		} 
	}
}