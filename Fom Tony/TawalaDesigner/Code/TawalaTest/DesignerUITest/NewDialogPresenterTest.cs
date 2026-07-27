using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using NUnit.Framework;

using Tawala.DesignerUI;
using Tawala.ProjectUI;
using TawalaTest.TestSupport;

namespace TawalaTest.DesignerUITest
{
	[TestFixture]
	public class NewProjectPresenterTest
	{
		private NewProjectDialog dialog;
		private INewProjectView view;
		private string projectDir;

		[SetUp]
		public void Setup()
		{
			projectDir = Directory.GetParent(Util.GetTestFilePath("foo")).Parent.Parent.FullName;
			dialog = new NewProjectDialog(Path.Combine(projectDir, "Templates"));
			dialog.Show();
			view = dialog as INewProjectView;
			Application.DoEvents();
		}

		[TearDown]
		public void TearDown()
		{
			dialog.Close();
		}

		[Test]
		public void Categories()
		{
			TreeNode treeNode = dialog.CategoryRootNode;

			Assert.AreEqual(5, treeNode.Nodes.Count);

			Assert.AreEqual("Basic", treeNode.Nodes[0].Text);
			Assert.AreEqual("1", treeNode.Nodes[0].Tag);
			Assert.AreEqual("Activities", treeNode.Nodes[1].Text);
			Assert.AreEqual("2", treeNode.Nodes[1].Tag);
			Assert.AreEqual("E-mail Lists", treeNode.Nodes[2].Text);
			Assert.AreEqual("3", treeNode.Nodes[2].Tag);
			Assert.AreEqual("Meetings", treeNode.Nodes[3].Text);
			Assert.AreEqual("4", treeNode.Nodes[3].Tag);
			Assert.AreEqual("Polls and Surveys", treeNode.Nodes[4].Text);
			Assert.AreEqual("5", treeNode.Nodes[4].Tag);
		}

		[Test]
		public void InitialSelectedCategoryIsAll()
		{
			Assert.IsTrue(view.CategoryRootNode.IsSelected);
		}

		[Test]
		public void InitialTemplatesDisplayed()
		{
			Assert.AreEqual(7, view.TemplateView.Items.Count);
			Assert.AreEqual("Empty", view.TemplateView.Items[0].Text);
		}

		[Test]
		public void InitialSelectedTemplateIsEmptyProject()
		{
			Assert.AreEqual(1, view.TemplateView.SelectedItems.Count);
			Assert.AreEqual("Empty", view.TemplateView.SelectedItems[0].Text);

			XmlElement projectNode = view.TemplateView.SelectedItems[0].Tag as XmlElement;
			Assert.AreEqual("Empty Project", projectNode.Attributes["file"].Value);
			Assert.AreEqual("Start with an empty project.", projectNode["description"].InnerText);
		}

		[Test]
		public void OkSetsSelectedTemplateFileName()
		{
			Assert.IsNull(dialog.SelectedTemplateFile);
			dialog.AcceptButton.PerformClick();
			Application.DoEvents();
			Assert.IsTrue(dialog.SelectedTemplateFile.EndsWith("Empty Project.tawala"));
			Assert.IsTrue(File.Exists(dialog.SelectedTemplateFile));
		}
	}
}
