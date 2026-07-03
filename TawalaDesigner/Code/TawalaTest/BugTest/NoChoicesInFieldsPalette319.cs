using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Tawala.Projects;
using Tawala.Processes;
using Tawala.Projects.Forms;
using Tawala.ProjectUI;
using Tawala.Controls;
using NUnit.Framework;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 319 (No Choices in Palette for Record:MCQ).
	/// </summary>
	[TestFixture]
	public class NoChoicesInFieldsPalette319
	{
		private IfStatementView testDetails;

		BindingFlags flags = 
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tDetails = typeof(IfStatementView);

		private IForm form;
		private McqItem mcItem;

		private FieldsPalette fieldsPalette;

		private void textBoxFieldDragDrop(ConditionGroup group, IPaletteField field)
		{
			DataObject data = new DataObject();
			data.SetData(typeof(IPaletteField), field);
			DragEventArgs dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

			InvokeGroupEventMethod(group, "textBoxField_DragDrop", dragEventArgs);
		}

		/// <summary>
		/// Invokes the event method specified by methodName
		/// </summary>
		public void InvokeGroupEventMethod(ConditionGroup group, string methodName, EventArgs eventArgs)
		{
			// get method by name
			Type tGroup = typeof(ConditionGroup);
			MethodInfo method = tGroup.GetMethod(methodName, flags);

			Object[] args = new object[2];
			args[0] = this;
			args[1] = eventArgs;

			// invoke method
			method.Invoke(group, args);
		}

		private GroupBox panel1()
		{
			FieldInfo controlInfo = tDetails.GetField("groupBox", flags);
			return ((GroupBox)controlInfo.GetValue(testDetails));
		}

		[SetUp]
		public void SetUp()
		{
			setUpProject();

			System.Windows.Forms.Form windowsForm = new System.Windows.Forms.Form();

			fieldsPalette = new FieldsPalette();

			windowsForm.Controls.Add(fieldsPalette);
			windowsForm.Show();

			testDetails = new IfStatementView();
			testDetails.CreateControl();
		}

		private void setUpProject()
		{
			Project.NewTestProject();
			form = Project.Current.AddForm();

			mcItem = new McqItem();
			form.ItemList.Add(mcItem);
		}

		[Test]
		public void FieldsPaletteInitiallyHasOnlyFormNode()
		{
			fieldsPalette.RefreshFieldList();

			Assert.AreEqual(2, fieldsPalette.FieldsTreeView.Nodes.Count);
			TreeNode formNode = fieldsPalette.FieldsTreeView.Nodes[0];

			Assert.AreEqual("Form 1", formNode.Text);
		}

		[Test]
		public void MCItemInFieldsBoxDisplaysChoicesInFieldsPalette()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;

			textBoxFieldDragDrop(group1, mcItem);
			fieldsPalette.RefreshFieldList();

			Assert.AreEqual(3, fieldsPalette.FieldsTreeView.Nodes.Count);
			TreeNode choiceNode = fieldsPalette.FieldsTreeView.Nodes[2];

			Assert.AreEqual("Choices", choiceNode.Text);
			Assert.AreEqual(1, choiceNode.Nodes.Count);
			Assert.AreEqual("a", choiceNode.Nodes[0].Text);
		}

		[Test]
		public void RecordMCItemInFieldsBoxDisplaysChoicesInFieldsPalette()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;

			textBoxFieldDragDrop(group1, new RecordField(new Record("Record"), mcItem));
			fieldsPalette.RefreshFieldList();

			Assert.AreEqual(3, fieldsPalette.FieldsTreeView.Nodes.Count);
			TreeNode choiceNode = fieldsPalette.FieldsTreeView.Nodes[2];

			Assert.AreEqual("Choices", choiceNode.Text);
			Assert.AreEqual(1, choiceNode.Nodes.Count);
			Assert.AreEqual("a", choiceNode.Nodes[0].Text);
		}
	}
}
