// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Tawala.DesignerUI
{
    public class NewProjectPresenter : INewProjectPresenter
    {
        private readonly XmlDocument templatesXml;
        private readonly INewProjectView view;
        private XmlNodeList categoryNodes;
        private bool emptyTemplateSelected;
        private int selectionCount;
        private string templatesPath;
        private string templatesXmlFile;

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
                var tn = new TreeNode(node.Attributes["name"].Value);
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
                var projectNode = lvi.Tag as XmlElement;
                view.TemplateDescription = projectNode["description"].InnerText;
            }
        }

        public string OK(ListViewItem lviSelected)
        {
            var projectNode = lviSelected.Tag as XmlElement;
            string file = projectNode.Attributes["file"].Value;
            string filePath = Path.Combine(templatesPath, file + ".tawala");

            emptyTemplateSelected = lviSelected.Text == "Empty";

            if (File.Exists(filePath))
            {
                return filePath;
            }

            return null;
        }

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
                var lvi = new ListViewItem(label, 0);
                lvi.Tag = node;
                lvi.Group = view.TemplateView.Groups[cid];
                view.TemplateView.Items.Add(lvi);
            }
        }

        private void createListViewGroup(TreeNode tn)
        {
            var group = new ListViewGroup(tn.Text, HorizontalAlignment.Left);
            group.Name = tn.Tag as string;
            group.Tag = tn.Tag;
            view.TemplateView.Groups.Add(group);
        }
    }
}