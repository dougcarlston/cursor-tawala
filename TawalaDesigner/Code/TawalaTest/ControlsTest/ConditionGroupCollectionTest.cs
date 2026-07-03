using System;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using Tawala.Projects;
using Tawala.Controls;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Tawala.XmlSupport;
using NUnit.Framework;

namespace TawalaTest.ControlsTest
{
	[TestFixture]
	public class ConditionGroupCollectionTest
	{
		ConditionGroupCollection testCollection;
		FieldsPalette testPalette;
		Panel panel;
        ComboBox comboBoxAndOr;

		IForm form;
		FibItem fibItem1;
		McqItem mcItem1;
		McqItem mcItem2;

		Process process;

		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tCollection = typeof(ConditionGroupCollection);

		[SetUp]
		public void Setup()
		{
			Project.NewTestProject();

			testPalette = new FieldsPalette();
			testPalette.Show();

			form = Project.Current.AddForm();
			fibItem1 = new FibItem();
			mcItem1 = new McqItem();
			mcItem2 = new McqItem();
			mcItem2.Choices.Add(new Choice());

			form.ItemList.Add(fibItem1);
			form.ItemList.Add(mcItem1);
			form.ItemList.Add(mcItem2);

			process = Project.Current.AddProcess();

			initalizePanel();

			initializeRadioButtons();

			testCollection = new ConditionGroupCollection(panel, comboBoxAndOr, true);

			panel.ControlAdded += new ControlEventHandler(panelGroups_ControlAdded);

			letterOffset = 1;
		}

		private void initalizePanel()
		{
			panel = new Panel();
			panel.Width = 475;
		}

		private void initializeRadioButtons()
		{
            comboBoxAndOr = new ComboBox();
            comboBoxAndOr.Items.Add("ALL");
            comboBoxAndOr.Items.Add("ANY");
            comboBoxAndOr.SelectedIndex = 0;
            comboBoxAndOr.Visible = false;
		}

		[TearDown]
		public void TearDown()
		{
            comboBoxAndOr.Dispose();
			panel.Dispose();
			testPalette.Dispose();
		}

		/// <summary>
		/// Invokes the method specified by methodName
		/// </summary>
		public void InvokeCollectionMethod(string methodName, params object[] args)
		{
			// get method by name
			MethodInfo method = tCollection.GetMethod(methodName, flags);

			// invoke method
			method.Invoke(testCollection, args);
		}


		/// <summary>
		/// Invokes the event method specified by methodName
		/// </summary>
		public void InvokeCollectionEventMethod(string methodName, EventArgs eventArgs)
		{
			// get method by name
			MethodInfo method = tCollection.GetMethod(methodName, flags);

			Object[] args = new object[2];
			args[0] = this;
			args[1] = eventArgs;

			// invoke method
			method.Invoke(testCollection, args);
		}

		/// <summary>
		/// Invokes the "_Click" method specified by methodName
		/// </summary>
		public void InvokeCollectionClickMethod(string methodName)
		{
			// get method by name
			MethodInfo method = tCollection.GetMethod(methodName, flags);

			// create arguments appropriate for _Click method
			Object[] args = new object[2];
			args[0] = this;
			args[1] = new EventArgs();

			// invoke method
			method.Invoke(testCollection, args);
		}

		/// <summary>
		/// Invokes the "_Click" method specified by methodName
		/// </summary>
		public void InvokeGroupClickMethod(ConditionGroup group, string methodName)
		{
			// get method by name
			Type tGroup = group.GetType();
			MethodInfo method = tGroup.GetMethod(methodName, flags);

			// create arguments appropriate for _Click method
			Object[] args = new object[2];
			args[0] = this;
			args[1] = new EventArgs();

			// invoke method
			method.Invoke(group, args);
		}

		/// <summary>
		/// Clicks the + button of the specified group
		/// </summary>
		private void InvokePlusButton(ConditionGroup group)
		{
			InvokeGroupClickMethod(group, "buttonPlus_Click");
		}

		/// <summary>
		/// Clicks the - button of the specified group
		/// </summary>
		private void InvokeMinusButton(ConditionGroup group)
		{
			InvokeGroupClickMethod(group, "buttonMinus_Click");
		}

		private void textBoxExpressionDragDrop(ConditionGroup group, IPaletteField field)
		{
			DataObject data = new DataObject();
			data.SetData(typeof(IPaletteField), field);
			DragEventArgs dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

			InvokeGroupEventMethod(group, "textBoxExpression_DragDrop", dragEventArgs);
		}

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

		private Button buttonMinus(ConditionGroup group)
		{
			Type tGroup = group.GetType();
			FieldInfo fi = tGroup.GetField("buttonMinus", flags);
			return (Button)fi.GetValue(group);
		}

		private TextBox textBoxField(ConditionGroup group)
		{
			Type tGroup = typeof(ConditionGroup);
			FieldInfo controlInfo = tGroup.GetField("textBoxField", flags);
			return ((TextBox)controlInfo.GetValue(group));
		}

		private ComboBox comboBoxOperator(ConditionGroup group)
		{
			Type tGroup = typeof(ConditionGroup);
			FieldInfo controlInfo = tGroup.GetField("comboBoxOperator", flags);
			return ((ComboBox)controlInfo.GetValue(group));
		}

		private TextBox textBoxExpression(ConditionGroup group)
		{
			Type tGroup = typeof(ConditionGroup);
			FieldInfo controlInfo = tGroup.GetField("textBoxExpression", flags);
			return ((TextBox)controlInfo.GetValue(group));
		}

		private int getConditionCount()
		{
			return panel.Controls.Count;
		}

		[Test]
		public void InitialControlConditionsWhenNew()
		{
			int visibleCount = 0;

			foreach (Control c in panel.Controls)
			{
				if (c.Visible)
				{
					++visibleCount;
				}
			}

			ConditionGroup group = panel.Controls[0] as ConditionGroup;

			Assert.AreEqual(1, getConditionCount());
			Assert.IsTrue(group.Visible);
			Assert.IsTrue(buttonMinus(group).Visible);
			Assert.AreEqual(1, visibleCount);
			Assert.AreEqual(1, panel.Controls.Count);
		}
	
		[Test]
		public void ClickingPlusButtonAddsConditionGroup()
		{
			ConditionGroup group = panel.Controls[0] as ConditionGroup; 

			InvokePlusButton(group);

			Assert.AreEqual(2, getConditionCount());
		}

		[Test]
		public void TopConditionGroupProperlyPositioned()
		{
			ConditionGroup group = panel.Controls[0] as ConditionGroup;

			Assert.AreEqual(0, group.Left);
			Assert.AreEqual(0, group.Top);
		}

		[Test]
		public void ClickingMinusOnSecondConditionGroupRemovesGroup()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);

			ConditionGroup group2 = panel.Controls[1] as ConditionGroup;

			InvokeMinusButton(group2);

			Assert.AreEqual(1, panel.Controls.Count);
		}

		[Test]
		public void ClickingMinusOnMiddleGroupLeavesSurroundingGroups()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			group1.Name = "A";
			InvokePlusButton(group1);

			ConditionGroup group2 = panel.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);

			Assert.AreEqual("A", panel.Controls[0].Name);
			Assert.AreEqual("B", panel.Controls[1].Name);
			Assert.AreEqual("C", panel.Controls[2].Name);

			InvokeMinusButton(group2);

			Assert.AreEqual(2, panel.Controls.Count);
			Assert.AreEqual("A", panel.Controls[0].Name);
			Assert.AreEqual("C", panel.Controls[1].Name);
		}

		[Test]
		public void ClickingMinusThenPlusRetainsGroupIdentities()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			group1.Name = "A";
			InvokePlusButton(group1);

			ConditionGroup group2 = panel.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);

			Assert.AreEqual("A", panel.Controls[0].Name);
			Assert.AreEqual("B", panel.Controls[1].Name);
			Assert.AreEqual("C", panel.Controls[2].Name);

			InvokeMinusButton(group2);

			Assert.AreEqual(2, panel.Controls.Count);
			Assert.AreEqual("A", panel.Controls[0].Name);
			Assert.AreEqual("C", panel.Controls[1].Name);

			group2 = panel.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);
			ConditionGroup group3 = panel.Controls[2] as ConditionGroup;

			Assert.AreEqual(3, panel.Controls.Count);
			Assert.AreEqual("D", panel.Controls[2].Name);
		}

		private int letterOffset = 1;

		private void panelGroups_ControlAdded(object sender, ControlEventArgs e)
		{
			string letters = "ABCDEFGHIJKLMNOP";
			e.Control.Name = letters.Substring(letterOffset++, 1);
		}

		[Test]
		public void ClickingPlusOnAnyButLastGroupDoesNothing()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);

			ConditionGroup group2 = panel.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);

			ConditionGroup group3 = panel.Controls[2] as ConditionGroup;

			Assert.AreEqual(3, panel.Controls.Count);

			InvokePlusButton(group1);
			InvokePlusButton(group2);

			Assert.AreEqual(3, panel.Controls.Count);
		}

		[Test]
		public void AndOrVisibleForTwoOrMoreGroups()
		{
			Assert.IsFalse(comboBoxAndOr.Visible);

			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);

            Assert.IsTrue(comboBoxAndOr.Visible);
		}

		[Test]
		public void ToggleAndOrRadioButtons()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);

            Assert.IsTrue(comboBoxAndOr.Text.CompareTo("ALL") == 0);

            comboBoxAndOr.SelectedIndex = 1;

            Assert.IsTrue(comboBoxAndOr.Text.CompareTo("ANY") == 0);

            comboBoxAndOr.SelectedIndex = 0;

            Assert.IsTrue(comboBoxAndOr.Text.CompareTo("ALL") == 0);
        }

		private Variable addSetStatementToProcess(string variableName)
		{
			SetStatement setStatement = new SetStatement();
			Variable variable = new Variable(variableName);
			setStatement.Variable = variable;
			process.Lines.Add(new SetLine(setStatement));

			return variable;
		}

		[Test]
		public void CheckSingleGroupConditions()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;

			Variable variable = addSetStatementToProcess("Variable 1");

			textBoxFieldDragDrop(group1, variable);
			comboBoxOperator(group1).SelectedIndex = 0;
			textBoxExpression(group1).Text = "Foo";

			Assert.AreEqual("Variable 1", textBoxField(group1).Text);

			Assert.AreEqual("Variable 1 equals \"Foo\"", testCollection.Conditions.ToString());
		}

		[Test]
		public void CheckConditionWithBlankExpression()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;

			Variable variable1 = addSetStatementToProcess("Variable 1");

			textBoxFieldDragDrop(group1, variable1);
			comboBoxOperator(group1).SelectedIndex = 0;
			textBoxExpressionDragDrop(group1, fibItem1.BlankList[0]);

			Expression expression = textBoxExpression(group1).Tag as Expression;

			Assert.IsInstanceOfType(typeof(FieldElement), expression.Elements[0]);
			Assert.AreSame(fibItem1.BlankList[0], ((FieldElement)expression.Elements[0]).Field);
			Assert.AreEqual("Variable 1 equals Form 1:Q1:a", testCollection.Conditions.ToString());
		}

		[Test]
		public void CheckMultipleGroupAndedConditions()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panel.Controls[1] as ConditionGroup;
            comboBoxAndOr.SelectedIndex = 0;
            
            Variable variable1 = addSetStatementToProcess("Variable 1");
			textBoxFieldDragDrop(group1, variable1);
			comboBoxOperator(group1).SelectedIndex = 0;
			textBoxExpression(group1).Text = "Foo";

			Variable variable2 = addSetStatementToProcess("Variable 2");
			textBoxFieldDragDrop(group2, variable2);
			comboBoxOperator(group2).SelectedIndex = 1;
			textBoxExpression(group2).Text = "Bar";

			Assert.AreEqual("Variable 1 equals \"Foo\" AND Variable 2 does not equal \"Bar\"", testCollection.Conditions.ToString());
		}

		[Test]
		public void CheckMultipleGroupOredConditions()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panel.Controls[1] as ConditionGroup;
            comboBoxAndOr.SelectedIndex = 1;

			Variable variable1 = addSetStatementToProcess("Variable 1");
			textBoxFieldDragDrop(group1, variable1);
			comboBoxOperator(group1).SelectedIndex = 0;
			textBoxExpression(group1).Text = "Foo";

			Variable variable2 = addSetStatementToProcess("Variable 2");
			textBoxFieldDragDrop(group2, variable2);
			comboBoxOperator(group2).SelectedIndex = 1;
			textBoxExpression(group2).Text = "Bar";

			Assert.AreEqual("Variable 1 equals \"Foo\" OR Variable 2 does not equal \"Bar\"", testCollection.Conditions.ToString());
		}

		[Test]
		public void SetSingleGroupConditions()
		{
			Conditions conditions = new Conditions();
			
			Variable variable = addSetStatementToProcess("Variable 1");
			ComparisonOperator op = HybridOperator.List[HybridOperator.Ops.equals];
			Expression expression = new Expression("Foo");

			conditions.Add(new Condition(variable, op, expression));

			testCollection.Conditions = conditions;

			Assert.AreEqual(1, panel.Controls.Count);

			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;

			Assert.AreEqual("Variable 1", textBoxField(group1).Text);
			Assert.AreSame(variable, textBoxField(group1).Tag);

			Assert.AreEqual(0, comboBoxOperator(group1).SelectedIndex);

			Assert.AreEqual("Foo", textBoxExpression(group1).Text);

			Assert.AreEqual(true, buttonMinus(group1).Visible);
		}

		[Test]
		public void SetConditionWithBlankExpression()
		{
			Conditions conditions = new Conditions();

			Variable variable = addSetStatementToProcess("Variable 1");
			ComparisonOperator op = HybridOperator.List[HybridOperator.Ops.equals];
			Expression expression = new Expression(fibItem1.BlankList[0]);

			conditions.Add(new Condition(variable, op, expression));

			testCollection.Conditions = conditions;

			Assert.AreEqual(1, panel.Controls.Count);

			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;

			Assert.AreEqual("Variable 1", textBoxField(group1).Text);
			Assert.AreSame(variable, textBoxField(group1).Tag);

			Assert.AreEqual(0, comboBoxOperator(group1).SelectedIndex);

			Assert.AreEqual("Form 1:Q1:a", textBoxExpression(group1).Text);

			Assert.AreEqual(true, buttonMinus(group1).Visible);
		}

		[Test]
		public void SetMultipleGroupAndedConditions()
		{
			Conditions conditions = new Conditions();

			Variable variable1 = addSetStatementToProcess("Variable 1");
			ComparisonOperator op1 = HybridOperator.List[HybridOperator.Ops.equals];
			Expression expression1 = new Expression("Foo");

			Condition condition1 = new Condition(variable1, op1, expression1);

			Variable variable2 = addSetStatementToProcess("Variable 2");
			ComparisonOperator op2 = HybridOperator.List[HybridOperator.Ops.doesNotEqual];
			Expression expression2 = new Expression("Foo");

			Condition condition2 = new Condition(variable2, op2, expression2);

			conditions.Add(condition1);
			conditions.Add(new LogicalOperator("AND"));
			conditions.Add(condition2);

			testCollection.Conditions = conditions;

			Assert.AreEqual(2, panel.Controls.Count);

			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			ConditionGroup group2 = panel.Controls[1] as ConditionGroup;

			Assert.AreSame(variable1, textBoxField(group1).Tag);
			Assert.AreEqual("Variable 1", textBoxField(group1).Text);
			Assert.AreEqual(0, comboBoxOperator(group1).SelectedIndex);
			Assert.AreEqual("Foo", textBoxExpression(group1).Text);

			Assert.AreSame(variable2, textBoxField(group2).Tag);
			Assert.AreEqual("Variable 2", textBoxField(group2).Text);
			Assert.AreEqual(1, comboBoxOperator(group2).SelectedIndex);
			Assert.AreEqual("Foo", textBoxExpression(group2).Text);

            Assert.IsTrue(comboBoxAndOr.Text.CompareTo("ALL") == 0);

            Assert.AreEqual(true, buttonMinus(group1).Visible);
			Assert.AreEqual(true, buttonMinus(group2).Visible);
		}

		[Test]
		public void SetMultipleGroupOredConditions()
		{
			Conditions conditions = new Conditions();

			Variable variable1 = addSetStatementToProcess("Variable 1");
			ComparisonOperator op1 = HybridOperator.List[HybridOperator.Ops.equals];
			Expression expression1 = new Expression("Foo");

			Condition condition1 = new Condition(variable1, op1, expression1);

			Variable variable2 = addSetStatementToProcess("Variable 2");
			ComparisonOperator op2 = HybridOperator.List[HybridOperator.Ops.doesNotEqual];
			Expression expression2 = new Expression("Bar");

			Condition condition2 = new Condition(variable2, op2, expression2);

			conditions.Add(condition1);
			conditions.Add(new LogicalOperator("OR"));
			conditions.Add(condition2);

			testCollection.Conditions = conditions;

			Assert.AreEqual(2, panel.Controls.Count);

			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			ConditionGroup group2 = panel.Controls[1] as ConditionGroup;

			Assert.AreSame(variable1, textBoxField(group1).Tag);
			Assert.AreEqual("Variable 1", textBoxField(group1).Text);
			Assert.AreEqual(0, comboBoxOperator(group1).SelectedIndex);
			Assert.AreEqual("Foo", textBoxExpression(group1).Text);

			Assert.AreSame(variable2, textBoxField(group2).Tag);
			Assert.AreEqual("Variable 2", textBoxField(group2).Text);
			Assert.AreEqual(1, comboBoxOperator(group2).SelectedIndex);
			Assert.AreEqual("Bar", textBoxExpression(group2).Text);

            Assert.IsTrue(comboBoxAndOr.Text.CompareTo("ANY") == 0);
        }

		[Test]
		public void MCQInFieldTextBoxAddsChoicesToFieldsPalette()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;

			textBoxFieldDragDrop(group1, mcItem1);
			testPalette.RefreshFieldList();

			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);
		}

		[Test]
		public void MCQInAnyFieldTextBoxAddsChoicesToFieldsPalette()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panel.Controls[1] as ConditionGroup;

			textBoxFieldDragDrop(group1, mcItem1);
			textBoxFieldDragDrop(group2, fibItem1.BlankList[0]);
			testPalette.RefreshFieldList();

			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);
		}

		[Test]
		public void RemovingMCQFromFieldTextBoxRemovesChoicesFromFieldsPalette()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;

			textBoxFieldDragDrop(group1, mcItem1);
			testPalette.RefreshFieldList();

			Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);

			textBoxFieldDragDrop(group1, fibItem1.BlankList[0]);
			testPalette.RefreshFieldList();

			Assert.AreEqual(2, testPalette.FieldsTreeView.Nodes.Count);
		}

		[Test]
		public void FieldsPaletteReflectsMCQWithMostChoices()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;

			textBoxFieldDragDrop(group1, mcItem1);
			testPalette.RefreshFieldList();

			Assert.AreEqual(1, testPalette.FieldsTreeView.Nodes[1].Nodes.Count);

			InvokePlusButton(group1);
			ConditionGroup group2 = panel.Controls[1] as ConditionGroup;

			textBoxFieldDragDrop(group2, mcItem2);
			testPalette.RefreshFieldList();

			Assert.AreEqual(2, testPalette.FieldsTreeView.Nodes[2].Nodes.Count);
		}

		[Test]
		public void RemovingGroupUpdatesChoicesInFieldsPalette()
		{
			ConditionGroup group1 = panel.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panel.Controls[1] as ConditionGroup;

			textBoxFieldDragDrop(group1, mcItem1);
			textBoxFieldDragDrop(group2, mcItem2);
			testPalette.RefreshFieldList();

			Assert.AreEqual(2, testPalette.FieldsTreeView.Nodes[2].Nodes.Count);

			InvokeMinusButton(group2);
			testPalette.RefreshFieldList();

			Assert.AreEqual(1, testPalette.FieldsTreeView.Nodes[2].Nodes.Count);
		}
	}
}
