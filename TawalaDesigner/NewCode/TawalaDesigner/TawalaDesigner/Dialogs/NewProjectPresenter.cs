// Copyright © 2005 - 2008  Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;

using Tawala.Common;
using Tawala.Projects;

namespace TawalaDesigner.Dialogs
{
	public class NewProjectPresenter : INewProjectPresenter
	{
		private INewProjectView view;
		private XmlDocument templatesXml;
		private XmlNodeList categoryNodes;
		private string templatesPath;
		private string templatesXmlFile;
		private int selectionCount = 0;

		public NewProjectPresenter(INewProjectView theView)
		{
			view = theView;
			templatesXml = new XmlDocument();
		}

		#region INewProjectPresenter Members

		public void Initialize(string path)
		{
			templatesPath = path;
			templatesXmlFile = Path.Combine(templatesPath, "templates.xml");

			if (File.Exists(templatesXmlFile))
			{
				init(templatesXmlFile);
			}
			else
			{
				templatesXmlFile = null;
			}
		}

		public void PopulateCategories()
		{
			if (templatesXmlFile == null)
			{
				return;
			}

			categoryNodes = templatesXml.SelectNodes("//category");

			foreach (XmlNode node in categoryNodes)
			{
				TreeNode tn = new TreeNode(node.Attributes["name"].Value);
				tn.Tag = node.Attributes["cid"].Value;
				view.CategoryRootNode.Nodes.Add(tn);
			}

			view.CategoryRootNode.ExpandAll();
		}

		public void SelectCategory(string categoryId)
		{
			if (templatesXmlFile == null)
			{
				return;
			}

			view.TemplateView.Clear();

			if (categoryId == null)
			{
				selectAllCategories();
			}
			else
			{
				selectCurrentCategory();
			}
		}

		public void SelectedTemplateChanged()
		{
			if (view.TemplateView.SelectedItems.Count == 0)
			{
				view.TemplateDescription = string.Empty;
			}
			else
			{
				ListViewItem lvi = view.TemplateView.SelectedItems[0];
				XmlElement projectNode = lvi.Tag as XmlElement;
				view.TemplateDescription = projectNode["description"].InnerText;
			}
		}

		public string OK(ListViewItem lviSelected)
		{
			XmlElement projectNode = lviSelected.Tag as XmlElement;
			string file = projectNode.Attributes["file"].Value;
			string filePath = Path.Combine(templatesPath, file + ".tawala");

			emptyTemplateSelected = lviSelected.Text == "Empty";

			if (File.Exists(filePath))
			{
				return filePath;
			}

			return null;
		}

		private bool emptyTemplateSelected = false;

		public bool EmptyTemplateSelected()
		{
			return emptyTemplateSelected;
		}

		#endregion

		private void init(string path)
		{
			templatesXml.Load(path);
		}

		private void selectAllCategories()
		{
			foreach (TreeNode tn in view.CategoryRootNode.Nodes)
			{
				createListViewGroup(tn);
			}

			foreach (XmlNode node in templatesXml.SelectNodes("//project"))
			{
				createListViewItem(node);
			}

			if (selectionCount == 0)
			{
				view.TemplateView.Groups[0].Items[0].Selected = true;

			}
			selectionCount++;
		}

		private void selectCurrentCategory()
		{
			TreeNode tn = view.CategoryRootNode.TreeView.SelectedNode;

			createListViewGroup(tn);

			foreach (XmlNode node in templatesXml.SelectNodes("//project[@cid='" + tn.Tag + "']"))
			{
				createListViewItem(node);
			}
		}

		private void createListViewItem(XmlNode node)
		{
			string cid = node.Attributes["cid"].Value;
			string file = node.Attributes["file"].Value;
			string filePath = Path.Combine(templatesPath, file + ".tawala");
			string label = node.Attributes["label"].Value;

			if (File.Exists(filePath))
			{
				ListViewItem lvi = new ListViewItem(label, 0);
				lvi.Tag = node;
				lvi.Group = view.TemplateView.Groups[cid];
				view.TemplateView.Items.Add(lvi);
			}
		}

		private void createListViewGroup(TreeNode tn)
		{
			ListViewGroup group = new ListViewGroup(tn.Text, HorizontalAlignment.Left);
			group.Name = tn.Tag as string;
			group.Tag = tn.Tag;
			view.TemplateView.Groups.Add(group);
		}
	}
}
