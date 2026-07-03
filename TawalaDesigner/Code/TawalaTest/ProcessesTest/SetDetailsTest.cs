using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Tawala.Processes;
using Tawala.Projects;
using Tawala.Common;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Tawala.XmlSupport;
using NUnit.Framework;

namespace TawalaTest.ProcessesTest
{
	[TestFixture]
	public class SetDetailsTest
	{
		SetStatementView testDetails;
		FieldsPalette fieldsPalette;

		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tDetails = typeof(SetStatementView);
		Type tPalette = typeof(FieldsPalette);

		private IForm form1;
		private FormList forms;
		private Process process;
		private FibItem fibItem1;
		private McqItem mcItem1;
		private McqItem mcItem2;
		private Record record;
		private RecordSet recordList;
		private Variable variable;


		[SetUp]
		public void Setup()
		{
			// start with fresh project
			Project.NewTestProject();

			fieldsPalette = new FieldsPalette();

			// add a form
			form1 = Project.Current.AddForm();

			// add an FIB
			fibItem1 = new FibItem();
			form1.ItemList.Add(fibItem1);

			// add 2 MC items, a "select only one" and a "select one or more"
			mcItem1 = new McqItem();
			mcItem1.Choices.Add(new Choice("Choice 1"));
			
			mcItem2 = new McqItem();
			mcItem2.SelectOnlyOne = false;
			form1.ItemList.Add(mcItem1);
			form1.ItemList.Add(mcItem2);

			process = Project.Current.AddProcess();
			Project.Current.ConnectProcessToForm(process, form1.Name);

			// add a variable
			variable = new Variable("Variable 1");
			process.Variables.Add(variable);

			// add a record
			record = new Record("Record");
			process.Records.AddUnique(record);

			// create GET statement ('Get Record Set from Form 1')
			forms = new FormList();
			forms.Add(form1);
			recordList = new RecordSet("Record Set", forms);
			GetStatement getStatement = new GetStatement(recordList);
			process.Lines.Add(new ProcessLineList(getStatement));

			// create FOR EACH RECORD statement ('For Each Record in Record Set')
			ProcessLineList forEachRecordLines = getForEachRecordLines(recordList, record);
			process.Lines.Add(forEachRecordLines);

			testDetails = new SetStatementView();
		}

		private static ProcessLineList getForEachRecordLines(RecordSet recordList, Record record)
		{
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			return forEachLines;
		}

		[TearDown]
		public void TearDown()
		{
			testDetails.Dispose();
			fieldsPalette.Dispose();
		}

		private TextBox textBoxVariable()
		{
			FieldInfo controlInfo = tDetails.GetField("textBoxVariable", flags);
			return ((TextBox)controlInfo.GetValue(testDetails));
		}

		private TextBox textBoxExpression()
		{
			FieldInfo controlInfo = tDetails.GetField("textBoxExpression", flags);
			return ((TextBox)controlInfo.GetValue(testDetails));
		}

		private void textBoxVariableDragDrop(IAssignableField assignableField)
		{
			DataObject data = new DataObject();
			data.SetData(typeof(IAssignableField), assignableField);
			DragEventArgs dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

			InvokeDetailsEventMethod("textBoxVariable_DragDrop", dragEventArgs);
		}

		private void textBoxExpressionDragDrop(IPaletteField field)
		{
			DataObject data = new DataObject();
			data.SetData(typeof(IPaletteField), field);
			DragEventArgs dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

			InvokeDetailsEventMethod("textBoxExpression_DragDrop", dragEventArgs);
		}

		private void syncTextBoxExpression(Expression expression)
		{
			InvokeDetailsMethod("syncTextBoxExpression", expression);
		}

		private void selectTextBoxVariable()
		{
			InvokeDetailsEventMethod("textBoxVariable_Enter", new EventArgs());
		}

		private void selectTextBoxExpression()
		{
			InvokeDetailsEventMethod("textBoxExpression_Enter", new EventArgs());
		}

		private void fieldNodeDoubleClick(IPaletteField field)
		{
			TreeNode node = makeLeafNode(field);

			MouseButtons button = MouseButtons.Left;
			int clicks = 2;

			InvokePaletteEventMethod("fieldsTreeView_NodeMouseDoubleClick", new TreeNodeMouseClickEventArgs(node, button, clicks, 0, 0));
		}

		private void fieldNodeClick(IPaletteField field)
		{
			TreeNode node = makeLeafNode(field);

			MouseButtons button = MouseButtons.Left;
			int clicks = 1;

			InvokePaletteEventMethod("fieldsTreeView_NodeMouseClick", new TreeNodeMouseClickEventArgs(node, button, clicks, 0, 0));
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

		private void fieldNodePressEnterKey()
		{
			InvokePaletteEventMethod("fieldsTreeView_KeyPress", new KeyPressEventArgs((char)13));
		}

		/// <summary>
		/// Invokes the method specified by methodName
		/// </summary>
		public void InvokeDetailsMethod(string methodName, params object[] args)
		{
			// get method by name
			MethodInfo method = tDetails.GetMethod(methodName, flags);

			// invoke method
			method.Invoke(testDetails, args);
		}


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


		/// <summary>
		/// Invokes the FieldsPalette method specified by methodName
		/// </summary>
		public Object InvokePaletteMethod(string methodName, params object[] args)
		{
			// get method by name
			MethodInfo method = tDetails.GetMethod(methodName, flags);

			// invoke method
			return method.Invoke(fieldsPalette, args);
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

		[Test]
		public void CheckProcess()
		{
			int i = 0;
			Assert.AreEqual("Get Record Set from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record in Record Set", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}

		[Test]
		public void TextBoxVariableDragDrop()
		{
			textBoxVariableDragDrop(variable);
			Assert.AreEqual("Variable 1", textBoxVariable().Text);
		}

		[Test]
		public void TextBoxVariableDragDropRecordBlank()
		{
			RecordField recordField = new RecordField(record, fibItem1.BlankList[0]);

			textBoxVariableDragDrop(recordField);
			Assert.AreEqual("Record:Form 1:Q1:a", textBoxVariable().Text);
		}

		[Test]
		public void TextBoxVariablePaletteDoubleClick()
		{
			selectTextBoxVariable();
			fieldNodeDoubleClick(variable);

			Assert.AreEqual("Variable 1", textBoxVariable().Text);
		}

		[Test]
		public void TextBoxExpressionDragDropMC()
		{
			textBoxExpressionDragDrop(mcItem2);
			Assert.AreEqual("<<Form 1:Q3>>", textBoxExpression().Text);
		}

		[Test]
		public void TextBoxExpressionTextAndInsertMC()
		{
			textBoxExpression().Text = "Text";
			textBoxExpression().SelectionStart = 4;
			textBoxExpressionDragDrop(mcItem2);
			Assert.AreEqual("Text<<Form 1:Q3>>", textBoxExpression().Text);

			textBoxExpression().Text = "Text";
			textBoxExpression().SelectionStart = 2;
			textBoxExpressionDragDrop(mcItem2);
			Assert.AreEqual("Te<<Form 1:Q3>>xt", textBoxExpression().Text);

			textBoxExpression().Text = "Text";
			textBoxExpression().SelectionStart = 0;
			textBoxExpressionDragDrop(mcItem2);
			Assert.AreEqual("<<Form 1:Q3>>Text", textBoxExpression().Text);
		}

		[Test]
		public void TextBoxExpressionPaletteDoubleClickMC()
		{
			selectTextBoxExpression();
			fieldNodeDoubleClick(mcItem2);

			Assert.AreEqual("<<Form 1:Q3>>", textBoxExpression().Text);
		}
	}
}
