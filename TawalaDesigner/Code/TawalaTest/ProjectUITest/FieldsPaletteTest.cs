using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Tawala.Documents;
using Tawala.Projects;
using Tawala.Common;
using Tawala.XmlSupport;
using NUnit.Framework;

namespace ProjectUITest
{
	[TestFixture]
	public class FieldsPaletteTest
	{
		FieldsPalette testPalette;

		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		private Process process;
		private IForm form1;
		private IForm form2;
		private FormList forms;
		private RecordSet recordList1;
		private Record record1;
		private Blank form1Blank1;
		private McqItem mcItem1;
		private McqItem mcItem2;

		[SetUp]
		public void Setup()
		{
			setupWithSingleFormGet();
		}

		private void setupWithSingleFormGet()
		{
			Project.NewTestProject();

			form1 = Project.Current.AddForm();

			form1.ItemList.Add(new TextItem());

			FibItem fibItem1 = new FibItem();
			form1.ItemList.Add(fibItem1);
			form1Blank1 = fibItem1.BlankList[0];

			mcItem1 = new McqItem();
			mcItem1.Choices.Add(new Choice());
			form1.ItemList.Add(mcItem1);

			mcItem2 = new McqItem();
			form1.ItemList.Add(mcItem2);
			mcItem2.SelectOnlyOne = false;

			form2 = Project.Current.AddForm();

			form2.ItemList.Add(new FibItem()); // Q1:a
			FibItem fibItem2 = new FibItem();
			fibItem2.BlankList[0].AlternateLabel = "Name";
			form2.ItemList.Add(fibItem2);

			McqItem mcItem3 = new McqItem();
			mcItem3.AlternateLabel = "Choice";
			form2.ItemList.Add(mcItem3);

			// create process
			process = Project.Current.AddProcess();

			// create SET statement ('Set Variable 3 to 3')
			process.Lines.Add(getSetLines(new Variable("Variable 3"), new Expression("3")));
			process.Lines.Add(getSetLines(new Variable("Variable 1"), new Expression("1")));
			process.Lines.Add(getSetLines(new Variable("Variable 2"), new Expression("2")));

			// create GET statement ('Get record list from Form 1')
			forms = new FormList();
			forms.Add(form1);
			recordList1 = new RecordSet("record list", forms);
			process.Lines.Add(getGetLines(recordList1));

			// create FOR EACH statement ('For Each record in record list')
			record1 = new Record("record");
			ProcessLineList forEachLines1 = getForEachLines(record1, recordList1);
			process.Lines.Add(forEachLines1);

			process.Records.AddUnique(record1);

			testPalette = new FieldsPalette();
			testPalette.Show();
		}

		private void setupWithMultiFormGet()
		{
			Project.NewTestProject();

			form1 = Project.Current.AddForm();

			form1.ItemList.Add(new TextItem());

			FibItem fibItem1 = new FibItem();
			form1.ItemList.Add(fibItem1);
			form1Blank1 = fibItem1.BlankList[0];

			mcItem1 = new McqItem();
			mcItem1.Choices.Add(new Choice());
			form1.ItemList.Add(mcItem1);

			mcItem2 = new McqItem();
			form1.ItemList.Add(mcItem2);
			mcItem2.SelectOnlyOne = false;

			form2 = Project.Current.AddForm();

			form2.ItemList.Add(new FibItem()); // Q1:a
			FibItem fibItem2 = new FibItem();
			fibItem2.BlankList[0].AlternateLabel = "Name";
			form2.ItemList.Add(fibItem2);

			McqItem mcItem3 = new McqItem();
			mcItem3.AlternateLabel = "Choice";
			form2.ItemList.Add(mcItem3);

			// create process
			process = Project.Current.AddProcess();

			// create GET statement ('Get record list from Form 1, Form2')
			forms = new FormList();
			forms.Add(form1);
			forms.Add(form2);
			recordList1 = new RecordSet("record list", forms);
			process.Lines.Add(getGetLines(recordList1));

			// create FOR EACH statement ('For Each record in record list')
			record1 = new Record("record");
			ProcessLineList forEachLines1 = getForEachLines(record1, recordList1);
			process.Lines.Add(forEachLines1);

			process.Records.AddUnique(record1);

			testPalette = new FieldsPalette();
			testPalette.Show();
		}

		private static ProcessLineList getGetLines(RecordSet recordList)
		{
			GetStatement getStatement = new GetStatement(recordList);
			return(new ProcessLineList(getStatement));
		}

		private static ProcessLineList getForEachLines(Record record, RecordSet recordList)
		{
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			return (new ProcessLineList(forEachStatement));
		}

		private static ProcessLineList getSetLines(Variable variable, Expression expression)
		{
			SetStatement setStatement = new SetStatement();
			setStatement.Variable = variable;
			setStatement.Expression = expression;
			return (new ProcessLineList(setStatement));
		}

		[TearDown]
		public void TearDown()
		{
			testPalette.Dispose();
		}


		/// <summary>
		/// Invokes the "_DoubleClick" method specified by methodName
		/// </summary>
		public void InvokeDoubleClickMethod(string methodName)
		{
			// get method by name
			MethodInfo method = typeof(FieldsPalette).GetMethod(methodName, flags);

			// create arguments appropriate for _Click method
			Object[] args = new object[2];
			args[0] = this;
			args[1] = new EventArgs();

			// invoke method
			method.Invoke(testPalette, args);
		}


		[Test]
		public void RefreshFieldList()
		{
			testPalette.RefreshFieldList();

			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode formNode = testPalette.FieldsTreeView.Nodes[0];
			Assert.AreEqual("Form 1", formNode.Text);
			Assert.IsNull(formNode.Tag);

			Assert.AreEqual(3, formNode.Nodes.Count);

			Assert.AreEqual("Q1:a", formNode.Nodes[0].Text);
			Assert.IsInstanceOfType(typeof(IField), formNode.Nodes[0].Tag);
			Assert.AreEqual("Q2", formNode.Nodes[1].Text);
			Assert.IsInstanceOfType(typeof(IField), formNode.Nodes[1].Tag);
			Assert.AreEqual("Q3", formNode.Nodes[2].Text);
			Assert.IsInstanceOfType(typeof(IField), formNode.Nodes[2].Tag);
		}

		[Test]
		public void MultipleForms()
		{
			testPalette.RefreshFieldList();

			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode formNode1 = testPalette.FieldsTreeView.Nodes[0];
			Assert.AreEqual("Form 1", formNode1.Text);

			Assert.AreEqual(3, formNode1.Nodes.Count);

			TreeNode formNode2 = testPalette.FieldsTreeView.Nodes[1];
			Assert.AreEqual("Form 2", formNode2.Text);

			Assert.AreEqual(3, formNode2.Nodes.Count);

			Assert.AreEqual("Q1:a", formNode2.Nodes[0].Text);
			Assert.AreEqual("Name", formNode2.Nodes[1].Text);
			Assert.AreEqual("Choice", formNode2.Nodes[2].Text);
		}

		[Test]
		public void Variables()
		{
			testPalette.RefreshFieldList();

			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode formNode1 = testPalette.FieldsTreeView.Nodes[0];
			Assert.AreEqual("Form 1", formNode1.Text);

			TreeNode formNode2 = testPalette.FieldsTreeView.Nodes[1];
			Assert.AreEqual("Form 2", formNode2.Text);

			TreeNode variableNode = testPalette.FieldsTreeView.Nodes[2];
			Assert.AreEqual("Variables", variableNode.Text);

			Assert.AreEqual(4, variableNode.Nodes.Count);

			Assert.AreEqual(Tawala.Projects.Properties.Resources.PrivateInvitationVariableLabel, variableNode.Nodes[0].Text);
			Assert.AreEqual("Variable 1", variableNode.Nodes[1].Text);
			Assert.AreEqual("Variable 2", variableNode.Nodes[2].Text);
			Assert.AreEqual("Variable 3", variableNode.Nodes[3].Text);
		}


		private ProcessLineList getIfWithQualifiedField()
		{
			// create process line 'If record:Q1:a equals Q1:a'
			QualifiedFieldList qualifiedField = new QualifiedFieldList(record1, form1.GetFields()["Q1:a"]);
			//FieldOrLiteral expression = new FieldOrLiteral("Q1:a", FieldOrLiteral.StringType.field, form1.GetFields());
			Expression expression = new Expression(form1Blank1);
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(qualifiedField, HybridOperator.List[HybridOperator.Ops.equals], expression);
			ProcessLineList ifLines = new ProcessLineList(ifStatement);
			return ifLines;
		}

		private ProcessLineList getSetVariableToConstant()
		{
			// create process line 'Set Score to 100'
			SetStatement setStatement = new SetStatement(process);
			setStatement.Variable = new Variable("Score");
			setStatement.Expression = new Expression("100");
			ProcessLineList setLines = new ProcessLineList(setStatement);
			return setLines;
		}

		private ProcessLineList getSetRecordAndFormQualifiedBlankToBlank()
		{
			// create process line 'Set Record:Form 1:Q1:a to Form 1:Q1:a'
			SetStatement setStatement = new SetStatement(process);
			setStatement.Variable = new RecordField(new Record("record"), form1Blank1);
			setStatement.Expression = new Expression(form1Blank1);
			ProcessLineList setLines = new ProcessLineList(setStatement);
			return setLines;
		}

		[Test]
		public void RecordNode()
		{
			// place 'If' lines within 'For Each' lines
			process.Lines.Insert(6, getIfWithQualifiedField());

			int i = 0;
			Assert.AreEqual("Set Variable 3 to 3", process.Lines[i++].ToString());
			Assert.AreEqual("Set Variable 1 to 1", process.Lines[i++].ToString());
			Assert.AreEqual("Set Variable 2 to 2", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If record:Q1:a equals Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 0));
			testPalette.RefreshFieldList();
			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 6));
			testPalette.RefreshFieldList();
			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode recordNode1 = testPalette.FieldsTreeView.Nodes[3];
			Assert.AreEqual("record", recordNode1.Text);
		}

		[Test]
		public void RecordResponseFieldsForMultipleForms()
		{
			setupWithMultiFormGet();
            form1.ConnectedPostProcess = process;

			int i = 0;
			Assert.AreEqual("Get record list from Form 1, Form 2", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 3));
			testPalette.RefreshFieldList();
			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode recordNode1 = testPalette.FieldsTreeView.Nodes[3];
			Assert.AreEqual("record", recordNode1.Text);

			Assert.AreEqual(6, recordNode1.Nodes.Count);

			i = 0;
			Assert.AreEqual("Form 1:Q1:a", recordNode1.Nodes[i++].Text);
			Assert.AreEqual("Form 1:Q2", recordNode1.Nodes[i++].Text);
			Assert.AreEqual("Form 1:Q3", recordNode1.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Q1:a", recordNode1.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Name", recordNode1.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Choice", recordNode1.Nodes[i++].Text);
		}

		[Test]
		public void NoExtraFieldsForOneConnectedForm()
		{
			setupWithMultiFormGet();
			form1.ConnectedPostProcess = process;

			// place 'Set' line ahead of 'For Each' lines
			process.Lines.Insert(0, getSetVariableToConstant());

			int i = 0;
			Assert.AreEqual("Set Score to 100", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1, Form 2", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 4));
			testPalette.RefreshFieldList();
			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode recordNode = testPalette.FieldsTreeView.Nodes[3];
			Assert.AreEqual("record", recordNode.Text);

			i = 0;
			Assert.AreEqual("Form 1:Q1:a", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 1:Q2", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 1:Q3", recordNode.Nodes[i++].Text);
//			Assert.AreEqual("Form 1:Score", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Q1:a", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Name", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Choice", recordNode.Nodes[i++].Text);
			Assert.AreEqual(i, recordNode.Nodes.Count);

			//Assert.IsInstanceOfType(typeof(RecordField), recordNode.Nodes[3].Tag);
			//RecordField recordField = recordNode.Nodes[3].Tag as RecordField;

			//Assert.IsInstanceOfType(typeof(ExtraField), recordField.ReferenceField);
		}

		[Test]
		public void NoExtraFieldsForMultipleConnectedForms()
		{
			setupWithMultiFormGet();
            form1.ConnectedPostProcess = process;
            form2.ConnectedPostProcess = process;

			// place 'Set' line ahead of 'For Each' lines
			process.Lines.Insert(0, getSetVariableToConstant());

			int i = 0;
			Assert.AreEqual("Set Score to 100", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1, Form 2", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 4));
			testPalette.RefreshFieldList();
			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode recordNode1 = testPalette.FieldsTreeView.Nodes[3];
			Assert.AreEqual("record", recordNode1.Text);

			dumpChildNodes(recordNode1, "ExtraFieldsForMultipleConnectedForms");

			Assert.AreEqual(6, recordNode1.Nodes.Count);

			i = 0;
			Assert.AreEqual("Form 1:Q1:a", recordNode1.Nodes[i++].Text);
			Assert.AreEqual("Form 1:Q2", recordNode1.Nodes[i++].Text);
			Assert.AreEqual("Form 1:Q3", recordNode1.Nodes[i++].Text);
//			Assert.AreEqual("Form 1:Score", recordNode1.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Q1:a", recordNode1.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Name", recordNode1.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Choice", recordNode1.Nodes[i++].Text);
//			Assert.AreEqual("Form 2:Score", recordNode1.Nodes[i++].Text);
		}

		[Test]
		public void ExtraFieldsForNoConnectedForms()
		{
			setupWithMultiFormGet();

			// place 'Set' line ahead of 'For Each' lines
			process.Lines.Insert(0, getSetVariableToConstant());

			// place 'Set' line inside 'For Each' lines
			process.Lines.Insert(4, getSetRecordAndFormQualifiedBlankToBlank());

			int i = 0;
			Assert.AreEqual("Set Score to 100", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1, Form 2", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Set record:Form 1:Q1:a to Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 4));
			testPalette.RefreshFieldList();
			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode recordNode = testPalette.FieldsTreeView.Nodes[3];
			Assert.AreEqual("record", recordNode.Text);

			i = 0;
			Assert.AreEqual("Form 1:Q1:a", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 1:Q2", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 1:Q3", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Q1:a", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Name", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 2:Choice", recordNode.Nodes[i++].Text);
			Assert.AreEqual(i, recordNode.Nodes.Count);
		}

		[Test]
		public void VariablesForNoConnectedForms()
		{
			setupWithMultiFormGet();

			// place 'Set' line ahead of 'For Each' lines
			process.Lines.Insert(0, getSetVariableToConstant());

			// place 'Set' line inside 'For Each' lines
			process.Lines.Insert(4, getSetRecordAndFormQualifiedBlankToBlank());

			int i = 0;
			Assert.AreEqual("Set Score to 100", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1, Form 2", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Set record:Form 1:Q1:a to Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 4));
			testPalette.RefreshFieldList();
			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode variablesNode = testPalette.FieldsTreeView.Nodes[2];
			Assert.AreEqual("Variables", variablesNode.Text);

			i = 0;
			Assert.AreEqual(Tawala.Projects.Properties.Resources.PrivateInvitationVariableLabel, variablesNode.Nodes[i++].Text);
			Assert.AreEqual("Score", variablesNode.Nodes[i++].Text);
			Assert.AreEqual(i, variablesNode.Nodes.Count);
		}

		private static void dumpChildNodes(TreeNode treeNode, string methodName)
		{
			Console.WriteLine("Children of '{0}' node:", treeNode.Text);

			for (int i = 0; i < treeNode.Nodes.Count; i++)
			{
				Console.WriteLine(" FieldsPaletteTest.{0}, {1}", methodName, treeNode.Nodes[i].Text);
			}
		}

		[Test]
		public void RecordResponseFields()
		{
			// place 'If' lines within 'For Each' lines
			process.Lines.Insert(6, getIfWithQualifiedField());

			int i = 0;
			Assert.AreEqual("Set Variable 3 to 3", process.Lines[i++].ToString());
			Assert.AreEqual("Set Variable 1 to 1", process.Lines[i++].ToString());
			Assert.AreEqual("Set Variable 2 to 2", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If record:Q1:a equals Form 1:Q1:a", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 6));
			testPalette.RefreshFieldList();
			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode recordNode1 = testPalette.FieldsTreeView.Nodes[3];
			Assert.AreEqual("record", recordNode1.Text);

			Assert.AreEqual(3, recordNode1.Nodes.Count);

			Assert.AreEqual("Form 1:Q1:a", recordNode1.Nodes[0].Text);
			Assert.IsInstanceOfType(typeof(RecordField), recordNode1.Nodes[0].Tag);
			Assert.AreEqual("Form 1:Q2", recordNode1.Nodes[1].Text);
			Assert.IsInstanceOfType(typeof(RecordField), recordNode1.Nodes[1].Tag);
			Assert.AreEqual("Form 1:Q3", recordNode1.Nodes[2].Text);
			Assert.IsInstanceOfType(typeof(RecordField), recordNode1.Nodes[2].Tag);
		}

		[Test]
		public void SetQualifiedBlank()
		{
            form1.ConnectedPostProcess = process;

			SetStatement setStatement = new SetStatement();
			Record record = new Record("record");
			setStatement.Variable = new RecordField(record, form1Blank1);
			setStatement.Expression = new Expression("100");
			SetLine line = new SetLine(setStatement);
			process.Lines.Insert(6, line);

			int i = 0;
			Assert.AreEqual("Set Variable 3 to 3", process.Lines[i++].ToString());
			Assert.AreEqual("Set Variable 1 to 1", process.Lines[i++].ToString());
			Assert.AreEqual("Set Variable 2 to 2", process.Lines[i++].ToString());
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Set record:Form 1:Q1:a to 100", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 7));
			testPalette.RefreshFieldList();
			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode variableNode = testPalette.FieldsTreeView.Nodes[2];
			Assert.AreEqual("Variables", variableNode.Text);

			for (int j = 0; j < variableNode.Nodes.Count; j++)
			{
				Console.WriteLine("FieldsPaletteTest.SettingQualifiedBlank, {0}", variableNode.Nodes[j].Text);
			}

			i = 0;
			Assert.AreEqual(Tawala.Projects.Properties.Resources.PrivateInvitationVariableLabel, variableNode.Nodes[i++].Text);
			Assert.AreEqual("Variable 1", variableNode.Nodes[i++].Text);
			Assert.AreEqual("Variable 2", variableNode.Nodes[i++].Text);
			Assert.AreEqual("Variable 3", variableNode.Nodes[i++].Text);
			Assert.AreEqual(i, variableNode.Nodes.Count);

			TreeNode recordNode = testPalette.FieldsTreeView.Nodes[3];
			Assert.AreEqual("record", recordNode.Text);

			for (int j = 0; j < recordNode.Nodes.Count; j++)
			{
				Console.WriteLine("FieldsPaletteTest.SettingQualifiedBlank, {0}", recordNode.Nodes[j].Text);
			}

			i = 0;
			Assert.AreEqual("Form 1:Q1:a", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 1:Q2", recordNode.Nodes[i++].Text);
			Assert.AreEqual("Form 1:Q3", recordNode.Nodes[i++].Text);
			//Assert.AreEqual("Form 1:Variable 3", recordNode.Nodes[i++].Text);
			//Assert.AreEqual("Form 1:Variable 1", recordNode.Nodes[i++].Text);
			//Assert.AreEqual("Form 1:Variable 2", recordNode.Nodes[i++].Text);
			Assert.AreEqual(i, recordNode.Nodes.Count);
		}


		private ProcessLineList getIfMCEqualsChoice()
		{
			// create process line 'If Form 1:Q2 equals a'
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(mcItem1,  MCOneOperator.List[MCOneOperator.Ops.mcEquals], new Choice("a"));
			ProcessLineList ifLines = new ProcessLineList(ifStatement);
			return ifLines;
		}

		[Test]
		public void ChoiceNode()
		{
			// insert 'If' lines at beginning of process
			process.Lines.Insert(0, getIfMCEqualsChoice());

			int i = 0;
			Assert.AreEqual("If Form 1:Q2 equals a", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

			Project.Events.RaiseMCItemSelectedEvent(new MCItemEventArgs(mcItem1));
			testPalette.RefreshFieldList();
			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode choiceNode = testPalette.FieldsTreeView.Nodes[3];
			Assert.AreEqual("Choices", choiceNode.Text);
		}

		[Test]
		public void ChoiceFields()
		{
			Project.Events.RaiseMCItemSelectedEvent(new MCItemEventArgs(mcItem1));
			testPalette.RefreshFieldList();
			Assert.AreEqual(4, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode choiceNode = testPalette.FieldsTreeView.Nodes[3];
			Assert.AreEqual("Choices", choiceNode.Text);

			Project.Events.RaiseMCItemSelectedEvent(new MCItemEventArgs());
			testPalette.RefreshFieldList();
			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);
		}

		[Test]
		public void ChoiceFieldsFromMultipleMCItems()
		{
			McqItem[] mcItems = new McqItem[] { mcItem1, mcItem2 };

			Project.Events.RaiseMCItemSelectedEvent(new MCItemEventArgs(mcItems));
			testPalette.RefreshFieldList();

			TreeNode choiceNode = testPalette.FieldsTreeView.Nodes[3];
			Assert.AreEqual("Choices", choiceNode.Text);
			Assert.AreEqual(choiceNode.Nodes.Count, 2);
			Assert.AreEqual("a", choiceNode.Nodes[0].Text);
			Assert.AreEqual("b", choiceNode.Nodes[1].Text);
		}

		[Test]
		public void SettingUnqualifiedVariableFromXmlAddsVariableNode()
		{
			string xmlString =
				"<process name=\"Process 2\">" +
				"<set field=\"Score\" arithmeticAsText=\"false\">" +
				"<string value=\"100\"/>" +
				"</set>" +
				"</process>";

			IXmlElement element = new XmlElement(xmlString);
			Process process = new Process(element);
			Project.Current.AddProcess(process);

			Assert.AreEqual("Process 2", process.Name);

			int i = 0;
			Assert.AreEqual("Set Score to 100", process.Lines[i++].ToString());
			Assert.AreEqual(i, process.Lines.Count);

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 0));
			testPalette.RefreshFieldList();
			
			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode formNode1 = testPalette.FieldsTreeView.Nodes[0];
			Assert.AreEqual("Form 1", formNode1.Text);

			TreeNode formNode2 = testPalette.FieldsTreeView.Nodes[1];
			Assert.AreEqual("Form 2", formNode2.Text);

			TreeNode variableNode = testPalette.FieldsTreeView.Nodes[2];
			Assert.AreEqual("Variables", variableNode.Text);

			dumpChildNodes(variableNode, "SettingUnqualifiedVariableFromXmlAddsVariableNode");

			Assert.AreEqual(5, variableNode.Nodes.Count);
			Assert.AreEqual(Tawala.Projects.Properties.Resources.PrivateInvitationVariableLabel, variableNode.Nodes[0].Text);
			Assert.AreEqual("Score", variableNode.Nodes[1].Text);
			Assert.AreEqual("Variable 1", variableNode.Nodes[2].Text);
			Assert.AreEqual("Variable 2", variableNode.Nodes[3].Text);
			Assert.AreEqual("Variable 3", variableNode.Nodes[4].Text);
		}

		[Test]
		public void SettingQualifiedFieldFromXmlDoesNotAddVariableNode()
		{
			string xmlString =
				"<process name=\"Process 2\">" +
				"<get recordList=\"Record List 1\">" +
				"<forms>" +
				"<form name=\"Form 1\"/>" +
				"</forms>" +
				"</get>" +
				"<foreach record=\"Record\" recordList=\"Record List 1\">" +
				"<set field=\"Record:Form 1:Q1:a\" arithmeticAsText=\"false\">" +
				"<string value=\"100\"/>" +
				"</set>" +
				"</foreach>" +
				"</process>";

			IXmlElement element = new XmlElement(xmlString);
			Process process = new Process(element);
			Project.Current.AddProcess(process);

			Assert.AreEqual("Process 2", process.Name);

			int i = 0;
			Assert.AreEqual("Get Record List 1 from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record in Record List 1", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Set Record:Form 1:Q1:a to 100", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(i, process.Lines.Count);

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 0));
			testPalette.RefreshFieldList();

			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode formNode1 = testPalette.FieldsTreeView.Nodes[0];
			Assert.AreEqual("Form 1", formNode1.Text);

			TreeNode formNode2 = testPalette.FieldsTreeView.Nodes[1];
			Assert.AreEqual("Form 2", formNode2.Text);

			TreeNode variableNode = testPalette.FieldsTreeView.Nodes[2];
			Assert.AreEqual("Variables", variableNode.Text);

			dumpChildNodes(variableNode, "SettingQualifiedFieldFromXmlDoesNotAddVariableNode");

			Assert.AreEqual(4, variableNode.Nodes.Count);
			Assert.AreEqual(Tawala.Projects.Properties.Resources.PrivateInvitationVariableLabel, variableNode.Nodes[0].Text);
			Assert.AreEqual("Variable 1", variableNode.Nodes[1].Text);
			Assert.AreEqual("Variable 2", variableNode.Nodes[2].Text);
			Assert.AreEqual("Variable 3", variableNode.Nodes[3].Text);
		}

		[Test]
		public void SettingQualifiedBlankFromXmlDoesNotAddVariableNode()
		{
			string xmlString =
				"<process name=\"Process 2\">" +
				"<get recordList=\"Record List 1\">" +
				"<forms>" +
				"<form name=\"Form 1\"/>" +
				"</forms>" +
				"</get>" +
				"<foreach record=\"Record\" recordList=\"Record List 1\">" +
				"<set field=\"Record:Form 1:Q1:a\" arithmeticAsText=\"false\">" +
				"<string value=\"100\"/>" +
				"</set>" +
				"</foreach>" +
				"</process>";

			IXmlElement element = new XmlElement(xmlString);
			Process process = new Process(element);
			Project.Current.AddProcess(process);

			Assert.AreEqual("Process 2", process.Name);

			int i = 0;
			Assert.AreEqual("Get Record List 1 from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record in Record List 1", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Set Record:Form 1:Q1:a to 100", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(i, process.Lines.Count);

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 0));
			testPalette.RefreshFieldList();

			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);

			TreeNode formNode1 = testPalette.FieldsTreeView.Nodes[0];
			Assert.AreEqual("Form 1", formNode1.Text);

			TreeNode formNode2 = testPalette.FieldsTreeView.Nodes[1];
			Assert.AreEqual("Form 2", formNode2.Text);

			TreeNode variableNode = testPalette.FieldsTreeView.Nodes[2];
			Assert.AreEqual("Variables", variableNode.Text);

			dumpChildNodes(variableNode, "SettingQualifiedBlankFromXmlDoesNotAddVariableNode");

			Assert.AreEqual(4, variableNode.Nodes.Count);
			Assert.AreEqual(Tawala.Projects.Properties.Resources.PrivateInvitationVariableLabel, variableNode.Nodes[0].Text);
			Assert.AreEqual("Variable 1", variableNode.Nodes[1].Text);
			Assert.AreEqual("Variable 2", variableNode.Nodes[2].Text);
			Assert.AreEqual("Variable 3", variableNode.Nodes[3].Text);
		}
	}
}
