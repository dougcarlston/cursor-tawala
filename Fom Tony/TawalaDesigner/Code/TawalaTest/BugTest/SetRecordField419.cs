using System;
using System.Windows.Forms;
using System.Reflection;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using TawalaTest.TestSupport;
using Tawala.Processes;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 419 (Cannot add SET statement when left-hand box references Record Field).
	/// </summary>
	[TestFixture]
	public class SetRecordField419
	{
		private SetStatementView testView;
		private FieldsPalette fieldsPalette;

		private Process process;
		private IForm form;
		private FibItem fibItem;
		private Blank blank;

		private void selectFieldBox()
		{
			object[] args = new object[2];
			args[0] = this;
			args[1] = EventArgs.Empty;

			Reflect<SetStatementView>.InvokeMethod("textBoxVariable_Enter", testView, args);
		}

		private void fieldNodeDoubleClick(IPaletteField field)
		{
			MouseButtons button = MouseButtons.Left;
			int clicks = 2;

			object[] args = new object[2];
			args[0] = this;
			args[1] = new TreeNodeMouseClickEventArgs(makeLeafNode(field), button, clicks, 0, 0);

			Reflect<FieldsPalette>.InvokeMethod("fieldsTreeView_NodeMouseDoubleClick", fieldsPalette, args);
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

		private TextBox fieldBox()
		{
			return Reflect<SetStatementView>.GetField<TextBox>("textBoxVariable", testView);
		}

		private TextBox expressionBox()
		{
			return Reflect<SetStatementView>.GetField<TextBox>("textBoxExpression", testView);
		}

		private Button addButton()
		{
			return Reflect<SetStatementView>.GetField<Button>("buttonAddModify", testView);
		}

		private void onIdle()
		{
			InvokeDetailsMethod("OnIdle");
		}

		/// <summary>
		/// Invokes the method specified by methodName
		/// </summary>
		public void InvokeDetailsMethod(string methodName, params object[] args)
		{
			BindingFlags flags =
				BindingFlags.NonPublic |
				BindingFlags.Public |
				BindingFlags.Static |
				BindingFlags.Instance;

			Type tView = typeof(SetStatementView);
			MethodInfo method = tView.GetMethod(methodName, flags);

			method.Invoke(testView, args);
		}

		[SetUp]
		public void SetUp()
		{
			setUpProject();

			System.Windows.Forms.Form windowsForm = new System.Windows.Forms.Form();

			fieldsPalette = new FieldsPalette();

			windowsForm.Controls.Add(fieldsPalette);
			windowsForm.Show();

			testView = new SetStatementView();
			testView.CreateControl();
		}

		private void setUpProject()
		{
			Util.NewTestProject();

			process = Project.Current.AddProcess();

			form = Project.Current.AddForm();
			fibItem = new FibItem();
			form.ItemList.Add(fibItem);

			blank = fibItem.BlankList[0];
		}

		[Test]
		public void FieldBoxAcceptsRecordField()
		{
			selectFieldBox();
			fieldNodeDoubleClick(new RecordField(new Record("Record"), blank));

			Assert.AreEqual("Record:Form 1:Q1:a", fieldBox().Text);
		}

		[Test]
		public void RecordFieldInFieldBoxEnablesAddButton()
		{
			expressionBox().Text = "100";

			onIdle();
			Assert.AreEqual(false, addButton().Enabled);

			selectFieldBox();
			fieldNodeDoubleClick(new RecordField(new Record("Record"), blank));

			onIdle();
			Assert.AreEqual(true, addButton().Enabled);
		}

	}
}
