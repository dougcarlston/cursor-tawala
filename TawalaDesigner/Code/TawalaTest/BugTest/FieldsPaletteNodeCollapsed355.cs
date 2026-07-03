using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.ProjectUI;
using NUnit.Framework;
//using NUnit.Extensions.Forms;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 355 (Field Palette Node should expand when the first field is added to a form).
	/// </summary>
	[TestFixture]
	public class FieldsPaletteNodeCollapsed355// : NUnitFormTest
	{
		//private class FieldsPaletteTester : NUnit.Extensions.Forms.ControlTester
		//{
		//    public FieldsPaletteTester(string name) : base(name)
		//    {
		//    }
		//}

		private IForm form;

		private FieldsPalette fieldsPalette;
		//private FieldsPaletteTester fieldsPaletteTester;
		//private TreeViewTester treeViewTester;

		[SetUp]
		public /*override*/ void Setup()
		{
			//base.Setup();

			Project.NewTestProject();
			form = Project.Current.AddForm();

			System.Windows.Forms.Form windowsForm = new System.Windows.Forms.Form();

			fieldsPalette = new FieldsPalette();
			fieldsPalette.Name = "testFieldsPalette";
			windowsForm.Controls.Add(fieldsPalette);

			//fieldsPaletteTester = new FieldsPaletteTester(fieldsPalette.Name);

			fieldsPalette.FieldsTreeView.Name = "testTreeView";
			//treeViewTester = new TreeViewTester(fieldsPalette.FieldsTreeView.Name);

			windowsForm.Show();
		}

		[Test]
		public void TreeViewInitiallyShowsEmptyFormNode()
		{
			fieldsPalette.RefreshFieldList();

			//Assert.AreEqual(2, treeViewTester.Properties.Nodes.Count);
			//Assert.AreEqual("Form 1", treeViewTester.Properties.Nodes[0].Text);
			//Assert.AreEqual(0, treeViewTester.Properties.Nodes[0].Nodes.Count);
		}

		[Test]
		public void AddingItemToFormAddsItemToFormNode()
		{
			form.ItemList.Add(new FibItem());
			fieldsPalette.RefreshFieldList();

			//Assert.AreEqual(2, treeViewTester.Properties.Nodes.Count);
			//Assert.AreEqual(1, treeViewTester.Properties.Nodes[0].Nodes.Count);
			//Assert.AreEqual("Q1:a", treeViewTester.Properties.Nodes[0].Nodes[0].Text);
		}

		[Test]
		public void AddingFirstItemToFormExpandsFormNode()
		{
			form.ItemList.Add(new FibItem());
			fieldsPalette.RefreshFieldList();

			//TreeNode formNode = treeViewTester.Properties.Nodes[0];

			//Assert.IsTrue(formNode.IsExpanded, "IsExpanded should be true");
		}

		[Test]
		public void AddingSecondItemToFormDoesNotExpandFormNode()
		{
			form.ItemList.Add(new FibItem());
			fieldsPalette.RefreshFieldList();

			//TreeNode formNode = treeViewTester.Properties.Nodes[0];

			//formNode.Collapse();
			//Assert.IsFalse(formNode.IsExpanded, "IsExpanded should be false");

			form.ItemList.Add(new FibItem());
			fieldsPalette.RefreshFieldList();

			//Assert.IsFalse(formNode.IsExpanded, "IsExpanded should be false");
		}

		[Test]
		public void AddingNonPaletteItemToFormDoesNotExpandFormNode()
		{
			form.ItemList.Add(new TextItem());
			fieldsPalette.RefreshFieldList();

			//TreeNode formNode = treeViewTester.Properties.Nodes[0];

			//Assert.IsFalse(formNode.IsExpanded, "IsExpanded should be false");
		}

		[Test]
		public void AddingFirstPaletteItemToFormExpandsFormNode()
		{
			form.ItemList.Add(new TextItem());
			fieldsPalette.RefreshFieldList();

			//TreeNode formNode = treeViewTester.Properties.Nodes[0];

			//Assert.IsFalse(formNode.IsExpanded, "IsExpanded should be false");

			form.ItemList.Add(new FibItem());
			fieldsPalette.RefreshFieldList();

			//Assert.IsTrue(formNode.IsExpanded, "IsExpanded should be true");
		}
	}
}
