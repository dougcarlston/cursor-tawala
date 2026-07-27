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
using Tawala.ProjectUI;
using Tawala.XmlSupport;
using Tawala.Controls;
using NUnit.Framework;

namespace TawalaTest.ProcessesTest
{
	[TestFixture]
	public class XIfDetailsTest
	{
		IfStatementView testDetails;
		FieldsPalette fieldsPalette;

		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tDetails = typeof(IfStatementView);
		Type tPalette = typeof(FieldsPalette);

		[SetUp]
		public void Setup()
		{
			Project.NewTestProject();

			fieldsPalette = new FieldsPalette();
			testDetails = new IfStatementView();
			testDetails.CreateControl();
		}

		[TearDown]
		public void TearDown()
		{
			testDetails.Dispose();
			fieldsPalette.Dispose();
		}

		/// <summary>
		/// Invokes the event method specified by methodName
		/// </summary>
		public void InvokeEventMethod(Object targetObject, Object sender, string methodName)
		{
			// get method by name
			MethodInfo method = tDetails.GetMethod(methodName, flags);

			Object[] args = new object[2];
			args[0] = sender;
			args[1] = new EventArgs();

			// invoke method
			method.Invoke(targetObject, args);
		}

		/// <summary>
		/// Invokes the method specified by methodName
		/// </summary>
		public void InvokeGroupEventMethod(ConditionGroup group, string methodName)
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
			InvokeGroupEventMethod(group, "buttonPlus_Click");
		}

		/// <summary>
		/// Clicks the - button of the specified group
		/// </summary>
		private void InvokeMinusButton(ConditionGroup group)
		{
			InvokeGroupEventMethod(group, "buttonMinus_Click");
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


		/// <summary>
		/// Invokes the method specified by methodName
		/// </summary>
		public void InvokeGroupEventMethod(ConditionGroupCollection collection, ConditionGroup group, string methodName)
		{
			// get method by name
			Type tGroupCollection = typeof(ConditionGroupCollection);
			MethodInfo method = tGroupCollection.GetMethod(methodName, flags);

			// create arguments appropriate for _Click method
			Object[] args = new object[2];
			args[0] = group;
			args[1] = new EventArgs();

			// invoke method
			method.Invoke(collection, args);
		}

		private Label labelIf()
		{
			FieldInfo controlInfo = tDetails.GetField("labelIf", flags);
			return ((Label)controlInfo.GetValue(testDetails));
		}

		private GroupBox panel1()
		{
			FieldInfo controlInfo = tDetails.GetField("groupBox", flags);
            return ((GroupBox)controlInfo.GetValue(testDetails));
		}

        private ComboBox comboBoxAndOr()
		{
			FieldInfo controlInfo = tDetails.GetField("comboBoxAndOr", flags);
			return ((ComboBox)controlInfo.GetValue(testDetails));
		}

		private Button buttonAddModify()
		{
			FieldInfo controlInfo = tDetails.GetField("buttonAddModify", flags);
			return ((Button)controlInfo.GetValue(testDetails));
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

		private void textBoxExpressionDragDrop(ConditionGroup group, IPaletteField field)
		{
			DataObject data = new DataObject();
			data.SetData(typeof(IPaletteField), field);
			DragEventArgs dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

			InvokeGroupEventMethod(group, "textBoxExpression_DragDrop", dragEventArgs);
		}

		private TextBox textBoxField(ConditionGroup group)
		{
			Type tGroup = typeof(ConditionGroup);
			FieldInfo controlInfo = tGroup.GetField("textBoxField", flags);
			return ((TextBox)controlInfo.GetValue(group));
		}

		private void textBoxFieldDragDrop(ConditionGroup group, IPaletteField field)
		{
			DataObject data = new DataObject();
			data.SetData(typeof(IPaletteField), field);
			DragEventArgs dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.Copy);

			InvokeGroupEventMethod(group, "textBoxField_DragDrop", dragEventArgs);
		}

		private void enterTextBoxField(ConditionGroup group)
		{
			InvokeGroupEventMethod(group, "textBoxField_Enter");
		}

		private void enterTextBoxField(ConditionGroupCollection collection, ConditionGroup group)
		{
			InvokeGroupEventMethod(collection, group, "textBoxField_Enter");
		}

		private void enterTextBoxField(Object targetObject, Object sender)
		{
			InvokeEventMethod(targetObject, sender, "textBoxField_Enter");
		}

		private void enterTextBoxExpression(ConditionGroup group)
		{
			InvokeGroupEventMethod(group, "textBoxExpression_Enter");
		}

		private void enterTextBoxExpression(ConditionGroupCollection collection, ConditionGroup group)
		{
			InvokeGroupEventMethod(collection, group, "textBoxExpression_Enter");
		}

		private void enterTextBoxExpression(Object targetObject, Object sender)
		{
			InvokeEventMethod(targetObject, sender, "textBoxExpression_Enter");
		}

        //[Test]
        //public void ShowingOneConditionDisplaysSingularLabel()
        //{
        //    Assert.AreEqual("Execute first set of commands if the following condition is true:", labelIf().Text);
        //}

        //[Test]
        //public void ShowingMultipleConditionsDisplaysPluralLabel()
        //{
        //    ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
        //    InvokePlusButton(group1);

        //    Assert.AreEqual(2, panel1().Controls.Count);
        //    Assert.AreEqual("Execute first set of commands if the following conditions are true:", labelIf().Text);
        //}

		[Test]
		public void BlankConditionDisablesAddButton()
		{
			testDetails.DoIdle();
			Assert.IsFalse(buttonAddModify().Enabled);
		}

		[Test]
		public void NonBlankSingleConditionEnablesAddButton()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;

			textBoxFieldDragDrop(group1, new Variable("Variable 1"));
			comboBoxOperator(group1).SelectedIndex = 0;
			textBoxExpression(group1).Text = "Foo";

			testDetails.DoIdle();
			Assert.IsTrue(buttonAddModify().Enabled);
		}

		[Test]
		public void BlankSecondConditionDisablesAddButton()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panel1().Controls[1] as ConditionGroup;

			textBoxFieldDragDrop(group1, new Variable("Variable 1"));
			comboBoxOperator(group1).SelectedIndex = 0;
			textBoxExpression(group1).Text = "Foo";

			testDetails.DoIdle();
			Assert.IsFalse(buttonAddModify().Enabled);
		}

		[Test]
		public void NonBlankMultipleConditionsEnableAddButton()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panel1().Controls[1] as ConditionGroup;

			textBoxFieldDragDrop(group1, new Variable("Variable 1"));
			comboBoxOperator(group1).SelectedIndex = 0;
			textBoxExpression(group1).Text = "Foo";

			textBoxFieldDragDrop(group2, new Variable("Variable 2"));
			comboBoxOperator(group2).SelectedIndex = 0;
			textBoxExpression(group2).Text = "Bar";

			testDetails.DoIdle();
			Assert.IsTrue(buttonAddModify().Enabled);
		}

		[Test]
		public void BlankFieldTextBoxDisablesAddButton()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;

			textBoxExpression(group1).Text = "Foo";

			testDetails.DoIdle();
			Assert.IsFalse(buttonAddModify().Enabled);
		}

		[Test]
		public void BlankExpressionTextBoxDisablesAddButton()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;

			textBoxFieldDragDrop(group1, new Variable("Variable 1"));

			testDetails.DoIdle();
			Assert.IsFalse(buttonAddModify().Enabled);
		}

		protected static Color selectedColor = Color.FromArgb(210, 255, 210);
		protected static Color unSelectedColor = Color.White;

		[Test]
		public void SelectFieldTextBox()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;

			enterTextBoxField(testDetails, group1.TextBoxField);

			Assert.IsTrue(group1.TextBoxFieldSelected);
			Assert.IsFalse(group1.TextBoxExpressionSelected);

			Assert.AreEqual(selectedColor, textBoxField(group1).BackColor);
			Assert.AreEqual(unSelectedColor, textBoxExpression(group1).BackColor);
		}

		[Test]
		public void SelectExpressionTextBox()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;

			enterTextBoxExpression(testDetails, group1.TextBoxExpression);

			Assert.IsFalse(group1.TextBoxFieldSelected);
			Assert.IsTrue(group1.TextBoxExpressionSelected);

			Assert.AreEqual(unSelectedColor, textBoxField(group1).BackColor);
			Assert.AreEqual(selectedColor, textBoxExpression(group1).BackColor);
		}

		[Test]
		public void SelectFirstGroupFieldTextBox()
		{
			ConditionGroupCollection collection = new ConditionGroupCollection(panel1(), comboBoxAndOr(), true);
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panel1().Controls[1] as ConditionGroup;

			enterTextBoxField(testDetails, group2.TextBoxField);
			enterTextBoxField(testDetails, group1.TextBoxField);

			Assert.IsTrue(group1.TextBoxFieldSelected);
			Assert.IsFalse(group2.TextBoxFieldSelected);

			Assert.AreEqual(selectedColor, textBoxField(group1).BackColor);
			Assert.AreEqual(unSelectedColor, textBoxField(group2).BackColor);
		}

		[Test]
		public void SelectSecondGroupFieldTextBox()
		{
			ConditionGroupCollection collection = new ConditionGroupCollection(panel1(), comboBoxAndOr(), true);
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panel1().Controls[1] as ConditionGroup;

			enterTextBoxField(testDetails, group1.TextBoxField);
			enterTextBoxField(testDetails, group2.TextBoxField);

			Assert.IsFalse(group1.TextBoxFieldSelected);
			Assert.IsTrue(group2.TextBoxFieldSelected);

			Assert.AreEqual(unSelectedColor, textBoxField(group1).BackColor);
			Assert.AreEqual(selectedColor, textBoxField(group2).BackColor);
		}

		[Test]
		public void SelectFirstGroupExpressionTextBox()
		{
            ConditionGroupCollection collection = new ConditionGroupCollection(panel1(), comboBoxAndOr(), true);
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panel1().Controls[1] as ConditionGroup;

			enterTextBoxExpression(testDetails, group2.TextBoxExpression);
			enterTextBoxExpression(testDetails, group1.TextBoxExpression);

			Assert.IsTrue(group1.TextBoxExpressionSelected);
			Assert.IsFalse(group2.TextBoxExpressionSelected);

			Assert.AreEqual(selectedColor, textBoxExpression(group1).BackColor);
			Assert.AreEqual(unSelectedColor, textBoxExpression(group2).BackColor);
		}

		[Test]
		public void SelectSecondGroupExpressionTextBox()
		{
            ConditionGroupCollection collection = new ConditionGroupCollection(panel1(), comboBoxAndOr(), true);
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			InvokePlusButton(group1);
			ConditionGroup group2 = panel1().Controls[1] as ConditionGroup;

			enterTextBoxExpression(testDetails, group1.TextBoxExpression);
			enterTextBoxExpression(testDetails, group2.TextBoxExpression);

			Assert.IsFalse(group1.TextBoxExpressionSelected);
			Assert.IsTrue(group2.TextBoxExpressionSelected);

			Assert.AreEqual(unSelectedColor, textBoxExpression(group1).BackColor);
			Assert.AreEqual(selectedColor, textBoxExpression(group2).BackColor);
		}

		[Test]
		public void IsBlankOperatorHidesExpressionTextBox()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			textBoxFieldDragDrop(group1, new Variable("Variable 1"));
			comboBoxOperator(group1).SelectedItem = HybridOperator.List["is blank"];
			Assert.IsFalse(textBoxExpression(group1).Visible);

			comboBoxOperator(group1).SelectedItem = HybridOperator.List["contains"];
			Assert.IsTrue(textBoxExpression(group1).Visible);
		}

		[Test]
		public void IsBlankOperatorWithBlankExpressionTextBoxEnablesAddButton()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			textBoxFieldDragDrop(group1, new Variable("Variable 1"));
			comboBoxOperator(group1).SelectedItem = HybridOperator.List["is blank"];

			testDetails.DoIdle();
			Assert.IsTrue(buttonAddModify().Enabled);
		}

		[Test]
		public void IsNotBlankOperatorHidesExpressionTextBox()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			textBoxFieldDragDrop(group1, new Variable("Variable 1"));
			comboBoxOperator(group1).SelectedItem = HybridOperator.List["is not blank"];
			Assert.IsFalse(textBoxExpression(group1).Visible);

			comboBoxOperator(group1).SelectedItem = HybridOperator.List["contains"];
			Assert.IsTrue(textBoxExpression(group1).Visible);
		}

		[Test]
		public void IsNotBlankOperatorWithBlankExpressionTextBoxEnablesAddButton()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			textBoxFieldDragDrop(group1, new Variable("Variable 1"));
			comboBoxOperator(group1).SelectedItem = HybridOperator.List["is not blank"];

			testDetails.DoIdle();
			Assert.IsTrue(buttonAddModify().Enabled);
		}

		[Test]
		public void SecondConditionGroupProperlyPositioned()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			InvokePlusButton(group1);

			ConditionGroup group2 = panel1().Controls[1] as ConditionGroup;

			Assert.AreEqual(group1.Left, group2.Left);
			Assert.AreEqual(group1.Top + group1.Height, group2.Top);
		}

		[Test]
		public void ClickingMinusThenPlusMaintainsGroupPositions()
		{
			ConditionGroup group1 = panel1().Controls[0] as ConditionGroup;
			int initialGroup1TopPositon = group1.Top;
			InvokePlusButton(group1);

			ConditionGroup group2 = panel1().Controls[1] as ConditionGroup;
			Assert.AreEqual(group1.Bottom, group2.Top);
			InvokePlusButton(group2);

			InvokeMinusButton(group2);

			group2 = panel1().Controls[1] as ConditionGroup;
			InvokePlusButton(group2);

			ConditionGroup group3 = panel1().Controls[2] as ConditionGroup;

			Assert.AreEqual(initialGroup1TopPositon, group1.Top);
			Assert.AreEqual(group1.Bottom, group2.Top);
			Assert.AreEqual(group2.Bottom, group3.Top);
		}
	}
}
