using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using NUnit.Framework;
//using NUnit.Extensions.Forms;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 337 (Field Palette should maintain it's position regardless of insertion point within a process).
	/// </summary>
	[TestFixture]
	public class FieldsPalettePosition337// : NUnitFormTest
	{
		//private class FieldsPaletteTester : NUnit.Extensions.Forms.ControlTester
		//{
		//    public FieldsPaletteTester(string name) : base(name)
		//    {
		//    }
		//}

		private Process process;

		private FieldsPalette fieldsPalette;
		//private FieldsPaletteTester fieldsPaletteTester;
		//private TreeViewTester treeViewTester;

		[SetUp]
		public /*override*/ void Setup()
		{
			//base.Setup();

			Project.NewTestProject();
			process = Project.Current.AddProcess();

			System.Windows.Forms.Form windowsForm = new System.Windows.Forms.Form();

			fieldsPalette = new FieldsPalette();
			fieldsPalette.Name = "testFieldsPalette";

			windowsForm.Height = fieldsPalette.FieldsTreeView.ItemHeight * 9;

			windowsForm.Controls.Add(fieldsPalette);

			//fieldsPaletteTester = new FieldsPaletteTester(fieldsPalette.Name);

			fieldsPalette.FieldsTreeView.Name = "testTreeView";
			//treeViewTester = new TreeViewTester(fieldsPalette.FieldsTreeView.Name);

			windowsForm.Show();
		}

		[Test]
		public void TreeViewIsInitiallyEmpty()
		{
			//Assert.AreEqual(0, treeViewTester.Properties.Nodes.Count);
		}

		private void addVariables(int numVariables)
		{
			for (int i = 0; i < numVariables; i++)
			{
				process.Variables.Add(new Variable(String.Format("Variable {0:D2}", i)));
			}
		}

		[Test]
		public void FieldsPaletteShowsVariables()
		{
			addVariables(2);

			fieldsPalette.RefreshFieldList();

			//Assert.AreEqual(1, treeViewTester.Properties.Nodes.Count);
			//Assert.AreEqual("Variables", treeViewTester.Properties.Nodes[0].Text);

			//Assert.AreEqual(3, treeViewTester.Properties.Nodes[0].Nodes.Count);
			//Assert.AreEqual("_InviteeID", treeViewTester.Properties.Nodes[0].Nodes[0].Text);
			//Assert.AreEqual("Variable 00", treeViewTester.Properties.Nodes[0].Nodes[1].Text);
			//Assert.AreEqual("Variable 01", treeViewTester.Properties.Nodes[0].Nodes[2].Text);
		}

		[Test]
		public void VariablesNodeIsInitialTopNode()
		{
			addVariables(10);

			fieldsPalette.RefreshFieldList();

			//Assert.AreEqual("Variables", treeViewTester.Properties.TopNode.Text);
		}

		[Test]
		public void CanChangeTopNode()
		{
			addVariables(10);

			fieldsPalette.RefreshFieldList();

			//treeViewTester.Properties.Nodes[0].Nodes[10].EnsureVisible();

			//Assert.AreEqual("Variable 05", treeViewTester.Properties.TopNode.Text);
		}

		[Test]
		public void RefreshDoesNotResetTopNode()
		{
			addVariables(10);

			fieldsPalette.RefreshFieldList();

			//treeViewTester.Properties.Nodes[0].Nodes[10].EnsureVisible();

			//Assert.AreEqual("Variable 05", treeViewTester.Properties.TopNode.Text);

			fieldsPalette.RefreshFieldList();

			//Assert.AreEqual(1, treeViewTester.Properties.Nodes.Count);
			//Assert.AreEqual("Variables", treeViewTester.Properties.Nodes[0].Text);

			//Assert.AreEqual(11, treeViewTester.Properties.Nodes[0].Nodes.Count);
			//Assert.AreEqual("_InviteeID", treeViewTester.Properties.Nodes[0].Nodes[0].Text);
			//Assert.AreEqual("Variable 00", treeViewTester.Properties.Nodes[0].Nodes[1].Text);
			//Assert.AreEqual("Variable 01", treeViewTester.Properties.Nodes[0].Nodes[2].Text);
			//Assert.AreEqual("Variable 09", treeViewTester.Properties.Nodes[0].Nodes[10].Text);

			//Assert.AreEqual("Variable 04", treeViewTester.Properties.TopNode.Text);
		}
	}
}
