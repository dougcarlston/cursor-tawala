using System;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using Tawala.Proj;
using Tawala.Controls;
using Tawala.ProjectUI;
using Tawala.XmlSupport;
using NUnit.Framework;

namespace TawalaTest.ControlsTest
{
	[TestFixture]
	public class ConditionsBuilderTest
	{
		ConditionsBuilder testBuilder;
		FieldsPalette fieldsPalette;

		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tBuilder = typeof(ConditionsBuilder);

		[SetUp]
		public void Setup()
		{
			fieldsPalette = new FieldsPalette();

			testBuilder = new ConditionsBuilder();
			letterOffset = 1;
		}

		[TearDown]
		public void TearDown()
		{
			testBuilder.Dispose();
			fieldsPalette.Dispose();
		}

		private Panel panelGroups
		{
			get
			{
				FieldInfo controlInfo = tBuilder.GetField("panelGroups", flags);
				return ((Panel)controlInfo.GetValue(testBuilder));
			}
		}

		/// <summary>
		/// Invokes the method specified by methodName
		/// </summary>
		public void InvokeBuiderMethod(string methodName, params object[] args)
		{
			// get method by name
			MethodInfo method = tBuilder.GetMethod(methodName, flags);

			// invoke method
			method.Invoke(testBuilder, args);
		}


		/// <summary>
		/// Invokes the event method specified by methodName
		/// </summary>
		public void InvokeBuilderEventMethod(string methodName, EventArgs eventArgs)
		{
			// get method by name
			MethodInfo method = tBuilder.GetMethod(methodName, flags);

			Object[] args = new object[2];
			args[0] = this;
			args[1] = eventArgs;

			// invoke method
			method.Invoke(testBuilder, args);
		}

		/// <summary>
		/// Invokes the "_Click" method specified by methodName
		/// </summary>
		public void InvokeBuilderClickMethod(string methodName)
		{
			// get method by name
			MethodInfo method = tBuilder.GetMethod(methodName, flags);

			// create arguments appropriate for _Click method
			Object[] args = new object[2];
			args[0] = this;
			args[1] = new EventArgs();

			// invoke method
			method.Invoke(testBuilder, args);
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

		private RadioButton radioButtonAnd()
		{
			FieldInfo fi = tBuilder.GetField("radioButtonAnd", flags);
			return (RadioButton)fi.GetValue(testBuilder);
		}

		private RadioButton radioButtonOr()
		{
			FieldInfo fi = tBuilder.GetField("radioButtonOr", flags);
			return (RadioButton)fi.GetValue(testBuilder);
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
			return panelGroups.Controls.Count;
		}

		[Test]
		public void InitialControlConditionsWhenNew()
		{
			int visibleCount = 0;

			foreach (Control c in testBuilder.Controls)
			{
				if (c.Visible)
				{
					++visibleCount;
				}
			}

			ConditionGroup group = panelGroups.Controls[0] as ConditionGroup;

			Assert.IsTrue(group.Visible);
			Assert.IsFalse(buttonMinus(group).Visible);
			Assert.AreEqual(1, visibleCount);
			Assert.AreEqual(3, testBuilder.Controls.Count);
			Assert.AreEqual(1, getConditionCount());
		}
	
		[Test]
		public void ClickingPlusButtonAddsConditionGroup()
		{
			ConditionGroup group = panelGroups.Controls[0] as ConditionGroup; 

			InvokePlusButton(group);

			Assert.AreEqual(2, getConditionCount());
		}

		[Test]
		public void TopConditionGroupFitsPanel()
		{
			ConditionGroup group = panelGroups.Controls[0] as ConditionGroup;
			Rectangle rGroup = group.Bounds;
			Rectangle rPanel = panelGroups.Bounds;
			rPanel = new Rectangle(0, 0, rPanel.Width, rPanel.Height);

			Assert.IsTrue(rPanel.Contains(rGroup));
		}

		[Test]
		public void TopConditionGroupProperlyPositioned()
		{
			ConditionGroup group = panelGroups.Controls[0] as ConditionGroup;

			Assert.AreEqual(0, group.Left);
			Assert.AreEqual(0, group.Top);
		}

		[Test]
		public void SecondConditionGroupProperlyPositioned()
		{
			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);

			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;

			Assert.AreEqual(0, group2.Left);
			Assert.AreEqual(group1.Height, group2.Top);
		}

		[Test]
		public void ClickingMinusOnSecondConditionGroupRemovesGroup()
		{
			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);

			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;

			InvokeMinusButton(group2);

			Assert.AreEqual(1, panelGroups.Controls.Count);
		}

		[Test]
		public void ClickingMinusOnMiddleGroupLeavesSurroundingGroups()
		{
			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			group1.Name = "A";
			panelGroups.ControlAdded += new ControlEventHandler(panelGroups_ControlAdded);
			InvokePlusButton(group1);

			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);

			Assert.AreEqual("A", panelGroups.Controls[0].Name);
			Assert.AreEqual("B", panelGroups.Controls[1].Name);
			Assert.AreEqual("C", panelGroups.Controls[2].Name);

			InvokeMinusButton(group2);

			Assert.AreEqual(2, panelGroups.Controls.Count);
			Assert.AreEqual("A", panelGroups.Controls[0].Name);
			Assert.AreEqual("C", panelGroups.Controls[1].Name);
		}

		[Test]
		public void ClickingMinusThenPlusRetainsGroupIdentities()
		{
			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			group1.Name = "A";
			panelGroups.ControlAdded += new ControlEventHandler(panelGroups_ControlAdded);
			InvokePlusButton(group1);

			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);

			Assert.AreEqual("A", panelGroups.Controls[0].Name);
			Assert.AreEqual("B", panelGroups.Controls[1].Name);
			Assert.AreEqual("C", panelGroups.Controls[2].Name);

			InvokeMinusButton(group2);

			Assert.AreEqual(2, panelGroups.Controls.Count);
			Assert.AreEqual("A", panelGroups.Controls[0].Name);
			Assert.AreEqual("C", panelGroups.Controls[1].Name);

			group2 = panelGroups.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);
			ConditionGroup group3 = panelGroups.Controls[2] as ConditionGroup;

			Assert.AreEqual(3, panelGroups.Controls.Count);
			Assert.AreEqual("D", panelGroups.Controls[2].Name);
		}

		private int letterOffset = 1;

		private void panelGroups_ControlAdded(object sender, ControlEventArgs e)
		{
			string letters = "ABCDEFGHIJKLMNOP";

			e.Control.Name = letters.Substring(letterOffset++, 1);
		}

		[Test]
		public void ClickingMinusThenPlusMaintainsGroupPositions()
		{
			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);

			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);

			InvokeMinusButton(group2);

			group2 = panelGroups.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);

			ConditionGroup group3 = panelGroups.Controls[2] as ConditionGroup;

			Assert.AreEqual(0, group1.Top);
			Assert.AreEqual(group1.Bottom, group2.Top);
			Assert.AreEqual(group2.Bottom, group3.Top);
		}

		[Test]
		public void ClickingPlusOnAnyButLastGroupDoesNothing()
		{
			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);

			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);

			ConditionGroup group3 = panelGroups.Controls[2] as ConditionGroup;

			Assert.AreEqual(3, panelGroups.Controls.Count);

			InvokePlusButton(group1);
			InvokePlusButton(group2);

			Assert.AreEqual(3, panelGroups.Controls.Count);
		}

		[Test]
		public void AndOrVisibleForTwoOrMoreGroups()
		{
			Assert.IsFalse(radioButtonAnd().Visible);
			Assert.IsFalse(radioButtonOr().Visible);

			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);

			Assert.IsTrue(radioButtonAnd().Visible);
			Assert.IsTrue(radioButtonOr().Visible);
		}

		[Test]
		public void ToggleAndOrRadioButtons()
		{
			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);

			Assert.IsTrue(radioButtonAnd().Checked);
			Assert.IsFalse(radioButtonOr().Checked);

			radioButtonOr().PerformClick();
			Assert.IsTrue(radioButtonOr().Checked);
			Assert.IsFalse(radioButtonAnd().Checked);

			radioButtonAnd().PerformClick();
			Assert.IsFalse(radioButtonOr().Checked);
			Assert.IsTrue(radioButtonAnd().Checked);
		}

		[Test]
		public void CheckSingleGroupConditions()
		{
			Project.NewTestProject();

			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;

			textBoxFieldDragDrop(group1, new Variable("Variable 1"));
			comboBoxOperator(group1).SelectedIndex = 0;
			textBoxExpression(group1).Text = "Foo";

			Assert.AreEqual("Variable 1", textBoxField(group1).Text);

			Assert.AreEqual("Variable 1 equals \"Foo\"", testBuilder.Conditions.ToString());
		}

		[Test]
		public void CheckMultipleGroupAndedConditions()
		{
			Project.NewTestProject();

			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;
			radioButtonAnd().PerformClick();

			textBoxFieldDragDrop(group1, new Variable("Variable 1"));
			comboBoxOperator(group1).SelectedIndex = 0;
			textBoxExpression(group1).Text = "Foo";

			textBoxFieldDragDrop(group2, new Variable("Variable 2"));
			comboBoxOperator(group2).SelectedIndex = 1;
			textBoxExpression(group2).Text = "Bar";

			Assert.AreEqual("Variable 1 equals \"Foo\" AND Variable 2 does not equal \"Bar\"", testBuilder.Conditions.ToString());
		}

		[Test]
		public void CheckMultipleGroupOredConditions()
		{
			Project.NewTestProject();

			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;
			radioButtonOr().PerformClick();

			textBoxFieldDragDrop(group1, new Variable("Variable 1"));
			comboBoxOperator(group1).SelectedIndex = 0;
			textBoxExpression(group1).Text = "Foo";

			textBoxFieldDragDrop(group2, new Variable("Variable 2"));
			comboBoxOperator(group2).SelectedIndex = 1;
			textBoxExpression(group2).Text = "Bar";

			Assert.AreEqual("Variable 1 equals \"Foo\" OR Variable 2 does not equal \"Bar\"", testBuilder.Conditions.ToString());
		}

		[Test]
		public void SetSingleGroupConditions()
		{
			Project.NewTestProject();

			Conditions conditions = new Conditions();
			
			Variable variable = new Variable("Variable 1");
			ComparisonOperator op = HybridOperator.List[HybridOperator.Ops.equals];
			Expression expression = new Expression("Foo");

			conditions.Add(new Condition(variable, op, expression));

			testBuilder.Conditions = conditions;

			Assert.AreEqual(1, panelGroups.Controls.Count);

			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;

			Assert.AreEqual("Variable 1", textBoxField(group1).Text);
			Assert.AreSame(variable, textBoxField(group1).Tag);

			Assert.AreEqual(0, comboBoxOperator(group1).SelectedIndex);

			Assert.AreEqual("Foo", textBoxExpression(group1).Text);

		}

		[Test]
		public void SetMultipleGroupAndedConditions()
		{
			Project.NewTestProject();

			Conditions conditions = new Conditions();

			Variable variable1 = new Variable("Variable 1");
			ComparisonOperator op1 = HybridOperator.List[HybridOperator.Ops.equals];
			Expression expression1 = new Expression("Foo");

			Condition condition1 = new Condition(variable1, op1, expression1);

			Variable variable2 = new Variable("Variable 2");
			ComparisonOperator op2 = HybridOperator.List[HybridOperator.Ops.doesNotEqual];
			Expression expression2 = new Expression("Foo");

			Condition condition2 = new Condition(variable2, op2, expression2);

			conditions.Add(condition1);
			conditions.Add(new LogicalOperator("AND"));
			conditions.Add(condition2);

			testBuilder.Conditions = conditions;

			Assert.AreEqual(2, panelGroups.Controls.Count);

			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;

			Assert.AreSame(variable1, textBoxField(group1).Tag);
			Assert.AreEqual("Variable 1", textBoxField(group1).Text);
			Assert.AreEqual(0, comboBoxOperator(group1).SelectedIndex);
			Assert.AreEqual("Foo", textBoxExpression(group1).Text);

			Assert.AreSame(variable2, textBoxField(group2).Tag);
			Assert.AreEqual("Variable 2", textBoxField(group2).Text);
			Assert.AreEqual(1, comboBoxOperator(group2).SelectedIndex);
			Assert.AreEqual("Foo", textBoxExpression(group2).Text);

			Assert.IsTrue(radioButtonAnd().Checked);
			Assert.IsFalse(radioButtonOr().Checked);
		}

		[Test]
		public void SetMultipleGroupOredConditions()
		{
			Project.NewTestProject();

			Conditions conditions = new Conditions();

			Variable variable1 = new Variable("Variable 1");
			ComparisonOperator op1 = HybridOperator.List[HybridOperator.Ops.equals];
			Expression expression1 = new Expression("Foo");

			Condition condition1 = new Condition(variable1, op1, expression1);

			Variable variable2 = new Variable("Variable 2");
			ComparisonOperator op2 = HybridOperator.List[HybridOperator.Ops.doesNotEqual];
			Expression expression2 = new Expression("Bar");

			Condition condition2 = new Condition(variable2, op2, expression2);

			conditions.Add(condition1);
			conditions.Add(new LogicalOperator("OR"));
			conditions.Add(condition2);

			testBuilder.Conditions = conditions;

			Assert.AreEqual(2, panelGroups.Controls.Count);

			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;

			Assert.AreSame(variable1, textBoxField(group1).Tag);
			Assert.AreEqual("Variable 1", textBoxField(group1).Text);
			Assert.AreEqual(0, comboBoxOperator(group1).SelectedIndex);
			Assert.AreEqual("Foo", textBoxExpression(group1).Text);

			Assert.AreSame(variable2, textBoxField(group2).Tag);
			Assert.AreEqual("Variable 2", textBoxField(group2).Text);
			Assert.AreEqual(1, comboBoxOperator(group2).SelectedIndex);
			Assert.AreEqual("Bar", textBoxExpression(group2).Text);

			Assert.IsTrue(radioButtonOr().Checked);
			Assert.IsFalse(radioButtonAnd().Checked);
		}

		[Test]
		public void CheckAndLabels()
		{
			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);
			ConditionGroup group3 = panelGroups.Controls[2] as ConditionGroup;

			radioButtonAnd().Checked = true;

			Assert.AreEqual("AND", group3.LabelLogicalOperator.Text);
		}

		[Test]
		public void CheckOrLabels()
		{
			ConditionGroup group1 = panelGroups.Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panelGroups.Controls[1] as ConditionGroup;
			InvokePlusButton(group2);
			ConditionGroup group3 = panelGroups.Controls[2] as ConditionGroup;

			radioButtonOr().Checked = true;

			Assert.AreEqual("OR", group3.LabelLogicalOperator.Text);
		}

		// check that "AND" appears beside third and subsequent groups
		// check that "OR" appears beside third and subsequent groups
	}
}
