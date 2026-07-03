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
	/// Tests for bug 322 (Unable to enter field in left box of SET statement via Double Click).
	/// </summary>
	[TestFixture]
	public class SetStatementFieldDoubleclick322
	{
		private SetStatementView testDetails;

		BindingFlags flags = 
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		private IForm form;
		private FibItem fibItem;
		private Blank blank;

		private FieldsPalette fieldsPalette;

		Type tDetails = typeof(SetStatementView);
		Type tPalette = typeof(FieldsPalette);

		/// <summary>
		/// Invokes the event method specified by methodName
		/// </summary>
		public void InvokeDetailsEventMethod(string methodName, EventArgs eventArgs)
		{
			// get method by name
			MethodInfo method = tDetails.GetMethod(methodName, flags);

			Object[] args = new object[2];
			args[0] = this;
			args[1] = eventArgs;

			// invoke method
			method.Invoke(testDetails, args);
		}

		private void selectFieldBox()
		{
			InvokeDetailsEventMethod("textBoxVariable_Enter", new EventArgs());
		}

		private TextBox fieldBox()
		{
			FieldInfo controlInfo = tDetails.GetField("textBoxVariable", flags);
			return ((TextBox)controlInfo.GetValue(testDetails));
		}

		/// <summary>
		/// Returns a child TreeNode referencing the specified field.
		/// </summary>
		private static TreeNode makeLeafNode(IPaletteField field)
		{
			TreeNode node = new TreeNode();
			node.Tag = field;
			TreeNode parentNode = new TreeNode();
			parentNode.Nodes.Add(node);
			return node;
		}

		private void fieldNodeDoubleClick(IPaletteField field)
		{
			TreeNode node = makeLeafNode(field);

			MouseButtons button = MouseButtons.Left;
			int clicks = 2;

			InvokePaletteEventMethod("fieldsTreeView_NodeMouseDoubleClick", new TreeNodeMouseClickEventArgs(node, button, clicks, 0, 0));
		}

		/// <summary>
		/// Invokes the FieldsPalette event method specified by methodName
		/// </summary>
		public void InvokePaletteEventMethod(string methodName, EventArgs eventArgs)
		{
			// get method by name
			MethodInfo method = tPalette.GetMethod(methodName, flags);

			Object[] args = new object[2];
			args[0] = this;
			args[1] = eventArgs;

			// invoke method
			method.Invoke(fieldsPalette, args);
		}
		
		[SetUp]
		public void SetUp()
		{
			setUpProject();

			System.Windows.Forms.Form windowsForm = new System.Windows.Forms.Form();

			fieldsPalette = new FieldsPalette();

			windowsForm.Controls.Add(fieldsPalette);
			windowsForm.Show();

			testDetails = new SetStatementView();
			testDetails.CreateControl();
		}

		private void setUpProject()
		{
			Project.NewTestProject();
			form = Project.Current.AddForm();

			fibItem = new FibItem();
			form.ItemList.Add(fibItem);

			blank = fibItem.BlankList[0];
		}

		[Test]
		public void DoubleClickingRecordBlankPlacesRecordBlankInFieldBox()
		{
			selectFieldBox();
			fieldNodeDoubleClick(new RecordField(new Record("Record"), blank));

			Assert.AreEqual("Record:Form 1:Q1:a", fieldBox().Text);
		}
	}
}
