using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using Tawala.Projects;
using Tawala.Controls;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Tawala.XmlSupport;
using NUnit.Framework;

namespace TawalaTest.ControlsTest
{
	[TestFixture]
	public class ConditionGroupTest
	{
		ConditionGroup testGroup;
		FieldsPalette fieldsPalette;

		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tDetails = typeof(ConditionGroup);
		Type tPalette = typeof(FieldsPalette);

		private IForm form1;
		private FormList forms;
		private Process process;
		private FibItem fibItem1;
		private McqItem mcItem1;
		private McqItem mcItem2;
		private Record record;
		private RecordSet recordList;


		[SetUp]
		public void Setup()
		{
			// start with fresh project
			TestSupport.Util.NewTestProject();

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
			process.Variables.AddUnique("variable 1");

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

			testGroup = new ConditionGroup();
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
			testGroup.Dispose();
			fieldsPalette.Dispose();
		}

		private TextBox textBoxField()
		{
			FieldInfo controlInfo = tDetails.GetField("textBoxField", flags);
			return ((TextBox)controlInfo.GetValue(testGroup));
		}

		private TextBox textBoxExpression()
		{
			FieldInfo controlInfo = tDetails.GetField("textBoxExpression", flags);
			return ((TextBox)controlInfo.GetValue(testGroup));
		}

		private void textBoxFieldDragDrop(IPaletteField field)
		{
			DataObject data = new DataObject();
			data.SetData(typeof(IPaletteField), field);
			DragEventArgs dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

			InvokeGroupEventMethod("textBoxField_DragDrop", dragEventArgs);
		}

		private void textBoxExpressionDragDrop(IPaletteField field)
		{
			DataObject data = new DataObject();
			data.SetData(typeof(IPaletteField), field);
			DragEventArgs dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

			InvokeGroupEventMethod("textBoxExpression_DragDrop", dragEventArgs);
		}

		private void syncTextBoxExpression(Expression expression)
		{
			InvokeGroupMethod("syncTextBoxExpression", expression);
		}

		private void selectTextBoxField()
		{
			InvokeGroupEventMethod("textBoxField_Enter", new EventArgs());
		}

		private void selectTextBoxExpression()
		{
			InvokeGroupEventMethod("textBoxExpression_Enter", new EventArgs());
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
		public void InvokeGroupMethod(string methodName, params object[] args)
		{
			// get method by name
			MethodInfo method = tDetails.GetMethod(methodName, flags);

			// invoke method
			method.Invoke(testGroup, args);
		}


		/// <summary>
		/// Invokes the event method specified by methodName
		/// </summary>
		public void InvokeGroupEventMethod(string methodName, EventArgs eventArgs)
		{
			// get method by name
			MethodInfo method = tDetails.GetMethod(methodName, flags);

			Object[] args = new object[2];
			args[0] = this;
			args[1] = eventArgs;

			// invoke method
			method.Invoke(testGroup, args);
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
		public void TextBoxFieldDragDropMC()
		{
			textBoxFieldDragDrop(mcItem1);

			Assert.AreSame(mcItem1, textBoxField().Tag);
			Assert.AreEqual("Form 1:Q2", ((IPaletteField)textBoxField().Tag).QualifiedFieldName);
			Assert.AreEqual("Form 1:Q2", textBoxField().Text);
		}

		[Test]
		public void TextBoxFieldDragDropRecordMC()
		{
			RecordField recordField = new RecordField(record, mcItem1);
			textBoxFieldDragDrop(recordField);

			Assert.AreSame(recordField, textBoxField().Tag);
			Assert.AreEqual("Record:Form 1:Q2", ((IPaletteField)textBoxField().Tag).QualifiedFieldName);
			Assert.AreEqual("Record:Form 1:Q2", textBoxField().Text);
		}

		[Test]
		public void TextBoxFieldDragDropBlank()
		{
			textBoxFieldDragDrop(fibItem1.BlankList[0]);

			Assert.AreSame(fibItem1.BlankList[0], textBoxField().Tag);
			Assert.AreEqual("Form 1:Q1:a", ((IPaletteField)textBoxField().Tag).QualifiedFieldName);
			Assert.AreEqual("Form 1:Q1:a", textBoxField().Text);
		}

		[Test]
		public void TextBoxExpressionDragDropMC()
		{
			textBoxExpressionDragDrop(mcItem2);

			Assert.IsInstanceOfType(typeof(Expression), textBoxExpression().Tag);

			Expression expression = (Expression)textBoxExpression().Tag;

			Assert.IsTrue(expression.HasSingleFieldElement);

			FieldElement element = (FieldElement)expression.Elements[0];
			IPaletteField field = (IPaletteField)element.Field;

			Assert.AreEqual("Form 1:Q3", field.QualifiedFieldName);
			Assert.AreEqual("Form 1:Q3", textBoxExpression().Text);
		}

		[Test]
		public void TextBoxExpressionDragDropBlank()
		{
			textBoxExpressionDragDrop(fibItem1.BlankList[0]);

			Assert.IsInstanceOfType(typeof(Expression), textBoxExpression().Tag);

			Expression expression = (Expression)textBoxExpression().Tag;

			Assert.IsTrue(expression.HasSingleFieldElement);

			FieldElement element = (FieldElement)expression.Elements[0];
			IPaletteField field = (IPaletteField)element.Field;

			Assert.AreSame(fibItem1.BlankList[0], element.Field);
			Assert.AreEqual("Form 1:Q1:a", field.QualifiedFieldName);
			Assert.AreEqual("Form 1:Q1:a", textBoxExpression().Text);
		}

		[Test]
		public void TextBoxExpressionDragDropRecordField()
		{
			RecordField recordField = new RecordField(record, fibItem1.BlankList[0]);
			textBoxExpressionDragDrop(recordField);

			Assert.IsInstanceOfType(typeof(Expression), textBoxExpression().Tag);

			Expression expression = (Expression)textBoxExpression().Tag;

			Assert.IsTrue(expression.HasSingleFieldElement);

			FieldElement element = (FieldElement)expression.Elements[0];
			IPaletteField field = (IPaletteField)element.Field;

			Assert.AreSame(recordField, element.Field);
			Assert.AreEqual("Record:Form 1:Q1:a", field.QualifiedFieldName);
			Assert.AreEqual("Record:Form 1:Q1:a", textBoxExpression().Text);
		}


		[Test]
		public void TextBoxFieldDragDropRecordField()
		{
			RecordField recordField = new RecordField(record, fibItem1.BlankList[0]);
			textBoxFieldDragDrop(recordField);

			Assert.AreSame(recordField, textBoxField().Tag);
			Assert.AreEqual("Record:Form 1:Q1:a", ((IPaletteField)textBoxField().Tag).QualifiedFieldName);
			Assert.AreEqual("Record:Form 1:Q1:a", textBoxField().Text);
		}

		[Test]
		public void TextBoxFieldReplaceMC()
		{
			textBoxFieldDragDrop(mcItem2);
			textBoxExpressionDragDrop(new ChoiceField(mcItem1.Choices.GetLabel(0)));
			Assert.AreEqual("a", textBoxExpression().Text);

			textBoxFieldDragDrop(mcItem1);
			Assert.AreEqual("a", textBoxExpression().Text);

			textBoxExpressionDragDrop(new ChoiceField(mcItem1.Choices.GetLabel(1)));
			Assert.AreEqual("b", textBoxExpression().Text);

			textBoxFieldDragDrop(mcItem2);
			Assert.AreEqual("", textBoxExpression().Text);
		}

		[Test]
		public void TextBoxExpressionSyncFieldElement()
		{
			syncTextBoxExpression(new Expression(mcItem1));

			Expression expression = (Expression)textBoxExpression().Tag;
			Assert.IsTrue(expression.HasSingleFieldElement);
		}

		[Test]
		public void TextBoxExpressionSyncStringElement()
		{
			syncTextBoxExpression(new Expression("Archie"));

			Expression expression = (Expression)textBoxExpression().Tag;
			Assert.IsTrue(expression.HasSingleStringElement);
		}

		[Test]
		public void TextBoxExpressionDragDropChoice()
		{
			textBoxExpressionDragDrop(new ChoiceField(mcItem1.Choices.GetLabel(0)));

			Assert.IsInstanceOfType(typeof(Expression), textBoxExpression().Tag);

			Expression expression = (Expression)textBoxExpression().Tag;

			Assert.IsTrue(expression.HasSingleFieldElement);

			FieldElement element = (FieldElement)expression.Elements[0];
			IPaletteField field = (IPaletteField)element.Field;

			Assert.AreEqual("a", textBoxExpression().Text);
		}

		[Test]
		public void TextBoxFieldIsReadOnly()
		{
			Assert.IsTrue(textBoxField().ReadOnly);
		}

	}
}
