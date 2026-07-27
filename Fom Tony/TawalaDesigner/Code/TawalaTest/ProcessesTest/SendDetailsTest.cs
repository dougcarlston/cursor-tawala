using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using Tawala.Processes;
using Tawala.Projects;
using Tawala.Common;
using NUnit.Framework;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;

namespace TawalaTest.ProcessesTest
{
	[TestFixture]
	public class SendDetailsTest
	{
		SendStatementView testDetails;
		FieldsPalette fieldsPalette;

		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tDetails = typeof(SendStatementView);
		Type tPalette = typeof(FieldsPalette);

		private IForm form1;
		private Process process;
		private FibItem fibItem1;
		private McqItem mcItem1;
		private McqItem mcItem2;
		private Record record;

		[SetUp]
		public void Setup()
		{
			Project.NewTestProject();
			fieldsPalette = new FieldsPalette();

			form1 = Project.Current.AddForm();

			fibItem1 = new FibItem();
			form1.ItemList.Add(fibItem1);

			mcItem1 = new McqItem();
			mcItem1.Choices.Add(new Choice("Choice 1"));

			mcItem2 = new McqItem();
			mcItem2.SelectOnlyOne = false;
			form1.ItemList.Add(mcItem1);
			form1.ItemList.Add(mcItem2);

			process = Project.Current.AddProcess();

			record = new Record("Record");
			process.Records.AddUnique(record);

			// create GET statement ('Get Record Set from Form 1')
			FormList forms = new FormList();
			forms.Add(form1);
			RecordSet recordList = new RecordSet("Record Set", forms);
			GetStatement getStatement = new GetStatement(recordList);
			process.Lines.Add(new ProcessLineList(getStatement));

			// create FOR EACH RECORD statement ('For Each Record in Record Set')
			ProcessLineList forEachRecordLines = getForEachRecordLines(recordList, record);
			process.Lines.Add(forEachRecordLines);

			testDetails = new SendStatementView();
//			testDetails.Show();
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
		}

		private TabControl tabControl()
		{
			FieldInfo controlInfo = tDetails.GetField("tabControl", flags);
			return ((TabControl)controlInfo.GetValue(testDetails));
		}

		private TabPage tabPageEmail()
		{
			FieldInfo controlInfo = tDetails.GetField("tabPageEmail", flags);
			return ((TabPage)controlInfo.GetValue(testDetails));
		}

		private TabPage tabPageInvitation()
		{
			FieldInfo controlInfo = tDetails.GetField("tabPageInvitation", flags);
			return ((TabPage)controlInfo.GetValue(testDetails));
		}

		private TextBox textBoxEmailRecipient()
		{
			FieldInfo controlInfo = tDetails.GetField("textBoxEmailRecipient", flags);
			return ((TextBox)controlInfo.GetValue(testDetails));
		}

		private void textBoxEmailRecipientDragDrop(IPaletteField field)
		{
			DataObject data = new DataObject();
			data.SetData(typeof(IPaletteField), field);
			DragEventArgs dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

			InvokeDetailsEventMethod("textBoxEmailRecipient_DragDrop", dragEventArgs);
		}

		private void selectTextBoxEmailRecipient()
		{
			InvokeDetailsEventMethod("textBoxEmailRecipient_Enter", new EventArgs());
		}

		private TextBox textBoxInvitationRecipient()
		{
			FieldInfo controlInfo = tDetails.GetField("textBoxInvitationRecipient", flags);
			return ((TextBox)controlInfo.GetValue(testDetails));
		}

		private void textBoxInvitationRecipientDragDrop(IPaletteField field)
		{
			DataObject data = new DataObject();
			data.SetData(typeof(IPaletteField), field);
			DragEventArgs dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

			InvokeDetailsEventMethod("textBoxInvitationRecipient_DragDrop", dragEventArgs);
		}

		private void selectTextBoxInvitationRecipient()
		{
			InvokeDetailsEventMethod("textBoxInvitationRecipient_Enter", new EventArgs());
		}

		private void fieldNodeDoubleClick(IPaletteField field)
		{
			TreeNode node = makeLeafNode(field);

			MouseButtons button = MouseButtons.Left;
			int clicks = 2;

			InvokePaletteEventMethod("fieldsTreeView_NodeMouseDoubleClick", new TreeNodeMouseClickEventArgs(node, button, clicks, 0, 0));
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
		
		/// <summary>
		/// Invokes the "_Click" method specified by methodName
		/// </summary>
		public void InvokeClickMethod(string methodName)
		{
			// get method by name
			MethodInfo method = tDetails.GetMethod(methodName, flags);

			// create arguments appropriate for _Click method
			Object[] args = new object[2];
			args[0] = this;
			args[1] = new EventArgs();

			// invoke method
			method.Invoke(testDetails, args);
		}
		
		/// <summary>
		/// Invokes the "_Mouse..." method specified by methodName
		/// </summary>
		public void InvokeMouseMethod(string methodName, MouseEventArgs mouseEventArgs)
		{
			// get method by name
			MethodInfo method = tDetails.GetMethod(methodName, flags);

			// create arguments appropriate for _Click method
			Object[] args = new object[2];
			args[0] = this;
			args[1] = mouseEventArgs;

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
		public void Basics()
		{
			Assert.IsTrue(testDetails.Visible);
			Assert.IsTrue(testDetails.CanSelect);

			Assert.AreEqual("Email", tabPageEmail().Text);
			//Assert.AreEqual("Invitation", tabPageInvitation().Text);
		}

		[Test]
		public void TextBoxEmailRecipientDragDrop()
		{
			textBoxEmailRecipientDragDrop(fibItem1.BlankList[0]);

			Assert.AreSame(fibItem1.BlankList[0], textBoxEmailRecipient().Tag);
			Assert.AreEqual("Form 1:Q1:a", ((IPaletteField)textBoxEmailRecipient().Tag).QualifiedFieldName);
			Assert.AreEqual("Form 1:Q1:a", textBoxEmailRecipient().Text);
		}

		[Test]
		public void TextBoxEmailRecipientDragDropRecordField()
		{
			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 0));
			fieldsPalette.RefreshFieldList();

			Assert.AreEqual(2, FieldsPalette.Palette.FieldsTreeView.Nodes.Count);
			Assert.AreEqual("Form 1", FieldsPalette.Palette.FieldsTreeView.Nodes[0].Text);

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 3));
			fieldsPalette.RefreshFieldList();

			Assert.AreEqual(3, FieldsPalette.Palette.FieldsTreeView.Nodes.Count);
			Assert.AreEqual("Record", FieldsPalette.Palette.FieldsTreeView.Nodes[2].Text);

			RecordField recordField = new RecordField(record, fibItem1.BlankList[0]);
			textBoxEmailRecipientDragDrop(recordField);
			fieldsPalette.RefreshFieldList();

			Assert.AreSame(recordField, textBoxEmailRecipient().Tag);
			Assert.AreEqual("Record:Form 1:Q1:a", ((IPaletteField)textBoxEmailRecipient().Tag).QualifiedFieldName);
			Assert.AreEqual("Record:Form 1:Q1:a", textBoxEmailRecipient().Text);
		}

		[Test]
		public void TextBoxEmailRecipientPaletteDoubleClick()
		{
			selectTextBoxEmailRecipient();
			fieldNodeDoubleClick(fibItem1.BlankList[0]);

			Assert.AreSame(fibItem1.BlankList[0], textBoxEmailRecipient().Tag);
			Assert.AreEqual("Form 1:Q1:a", ((IPaletteField)textBoxEmailRecipient().Tag).QualifiedFieldName);
			Assert.AreEqual("Form 1:Q1:a", textBoxEmailRecipient().Text);
		}

#if false
		// tests for obsolete version of the Send Details panel - 7/28/06

		[Test]
		public void SelectTabPages()
		{
			tabControl().SelectedIndex = 0;
			Assert.AreEqual(tabPageEmail(), tabControl().SelectedTab);

			tabControl().SelectedIndex = 1;
			Assert.AreEqual(tabPageInvitation(), tabControl().SelectedTab);
		}

		[Test]
		public void TextBoxInvitationRecipientDragDrop()
		{
			textBoxInvitationRecipientDragDrop(fibItem1.BlankList[0]);

			Assert.AreSame(fibItem1.BlankList[0], textBoxInvitationRecipient().Tag);
			Assert.AreEqual("Form 1:Q1:a", ((IPaletteField)textBoxInvitationRecipient().Tag).QualifiedFieldName);
			Assert.AreEqual("Form 1:Q1:a", textBoxInvitationRecipient().Text);
		}

		[Test]
		public void TextBoxInvitationRecipientDragDropRecordField()
		{
			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 3));

			Assert.AreEqual(2, FieldsPalette.Palette.FieldsTreeView.Nodes.Count);
			Assert.AreEqual("Record", FieldsPalette.Palette.FieldsTreeView.Nodes[1].Text);

			RecordField recordField = new RecordField(record, fibItem1.BlankList[0]);
			textBoxInvitationRecipientDragDrop(recordField);

			Assert.AreSame(recordField, textBoxInvitationRecipient().Tag);
			Assert.AreEqual("Record:Q1:a", ((IPaletteField)textBoxInvitationRecipient().Tag).QualifiedFieldName);
			Assert.AreEqual("Record:Q1:a", textBoxInvitationRecipient().Text);
		}

		[Test]
		public void TextBoxInvitationRecipientPaletteDoubleClick()
		{
			selectTextBoxInvitationRecipient();
			fieldNodeDoubleClick(fibItem1.BlankList[0]);

			Assert.AreSame(fibItem1.BlankList[0], textBoxInvitationRecipient().Tag);
			Assert.AreEqual("Form 1:Q1:a", ((IPaletteField)textBoxInvitationRecipient().Tag).QualifiedFieldName);
			Assert.AreEqual("Form 1:Q1:a", textBoxInvitationRecipient().Text);
		}
#endif
	}
}
