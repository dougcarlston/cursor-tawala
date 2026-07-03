using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using Tawala.Processes;
using Tawala.Proj;
using Tawala.Common;
using NUnit.Framework;

namespace TawalaTest.ProcessesTest
{
	[TestFixture]
	public class ExpressionBuilderDialogTest
	{
		ExpressionBuilderDialog testDialog;

		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tDialog = typeof(ExpressionBuilderDialog);

		Process process1;

		[SetUp]
		public void Setup()
		{
			// start with fresh project
			Project.NewTestProject();

			// add a form
			Tawala.Proj.Form form1 = Project.Current.AddForm();
			form1.ItemList.Add(new FibItem());

			// add 2 FIB items, a "select only one" and a "select one or more"
			form1.ItemList.Add(new MCItem());
			form1.ItemList.Add(new MCItem());
			((MCItem)form1.ItemList[2]).SelectOnlyOne = false;

			process1 = Project.Current.AddProcess();
			Project.Current.ConnectProcessToForm(process1, form1.Name);

			// add a variable
			process1.Variables.AddUnique("variable 1");

			testDialog = new ExpressionBuilderDialog(process1, 0);
			testDialog.Show();
		}

		[TearDown]
		public void TearDown()
		{
			testDialog.Close();
			testDialog.Dispose();
		}

		private Button getAddField2Button()
		{
			// get add field2 button
			FieldInfo buttonInfo = tDialog.GetField("buttonAddField2", flags);
			return ((Button)buttonInfo.GetValue(testDialog));
		}

		private Button getAddConditionButton()
		{
			// get add condition button
			FieldInfo buttonInfo = tDialog.GetField("buttonAddCondition", flags);
			return ((Button)buttonInfo.GetValue(testDialog));
		}

		private Button getRemoveButton()
		{
			// get remove button
			FieldInfo buttonInfo = tDialog.GetField("buttonRemove", flags);
			return ((Button)buttonInfo.GetValue(testDialog));
		}

		private Button getAndButton()
		{
			// get and button
			FieldInfo buttonInfo = tDialog.GetField("buttonAnd", flags);
			return ((Button)buttonInfo.GetValue(testDialog));
		}

		private TextBox getExpressionTextBox()
		{
			// get  text box
			FieldInfo textBoxInfo = tDialog.GetField("textBoxExpression", flags);
			return ((TextBox)textBoxInfo.GetValue(testDialog));
		}

		private TextBox getConditionsTextBox()
		{
			// get  text box
			FieldInfo textBoxInfo = tDialog.GetField("textBoxConditions", flags);
			return ((TextBox)textBoxInfo.GetValue(testDialog));
		}

		private ComboBox getField1ComboBox()
		{
			// get combo box
			FieldInfo comboBoxInfo = tDialog.GetField("comboBoxField1", flags);
			return ((ComboBox)comboBoxInfo.GetValue(testDialog));
		}

		private ComboBox getOperatorsComboBox()
		{
			// get combo box
			FieldInfo comboBoxInfo = tDialog.GetField("comboBoxOperators", flags);
			return ((ComboBox)comboBoxInfo.GetValue(testDialog));
		}

		private ComboBox getField2ComboBox()
		{
			// get combo box
			FieldInfo comboBoxInfo = tDialog.GetField("comboBoxField2", flags);
			return ((ComboBox)comboBoxInfo.GetValue(testDialog));
		}



		/// <summary>
		/// Invokes the "_Click" method specified by methodName
		/// </summary>
		public void InvokeClickMethod(string methodName)
		{
			// get method by name
			MethodInfo method = tDialog.GetMethod(methodName, flags);

			// create arguments appropriate for _Click method
			Object[] args = new object[2];
			args[0] = this;
			args[1] = new EventArgs();

			// invoke method
			method.Invoke(testDialog, args);
		}


		/// <summary>
		/// Invokes the "_Mouse..." method specified by methodName
		/// </summary>
		public void InvokeMouseMethod(string methodName, MouseEventArgs mouseEventArgs)
		{
			// get method by name
			MethodInfo method = tDialog.GetMethod(methodName, flags);

			// create arguments appropriate for _Click method
			Object[] args = new object[2];
			args[0] = this;
			args[1] = mouseEventArgs;

			// invoke method
			method.Invoke(testDialog, args);
		}


		[Test]
		public void InitialConditions()
		{
			// add "Q1:a equals <<variable 1>>" to the Conditions box
			addCondition1();

			Assert.AreEqual("Form 1:Q1:a equals variable 1", getConditionsTextBox().Text);

			// click the OK button
			InvokeClickMethod("buttonOK_Click");

			Conditions conditions = testDialog.Conditions;

			testDialog = new ExpressionBuilderDialog(process1, 0);
			testDialog.Conditions = conditions;

			testDialog.Show();

			Assert.AreEqual("Form 1:Q1:a equals variable 1", getConditionsTextBox().Text);

		}

		[Test]
		public void Field1DataSource()
		{
			// get combo box
			ComboBox comboBoxField1 = getField1ComboBox();

			// click the dropdown arrow
			InvokeClickMethod("comboBoxField1_DropDown");

			// verify that datasource items are accessible through combo box
			Assert.AreEqual(4, comboBoxField1.Items.Count);

			Assert.IsTrue(comboBoxField1.Items[0] is Blank, "should be Blank");
			Assert.IsTrue(comboBoxField1.Items[1] is MCItem, "should be MCItem");
			Assert.IsTrue(comboBoxField1.Items[2] is MCItem, "should be MCItem");
			Assert.IsTrue(comboBoxField1.Items[3] is Variable, "should be Variable");
		}


		/// <summary>
		/// Verify that clicking the AddField2 button places field text in the Expression box
		/// </summary>
		[Test]
		public void AddFieldToExpression()
		{
			// get combo box
			ComboBox comboBoxField2 = getField2ComboBox();

			// click the dropdown arrow
			InvokeClickMethod("comboBoxField2_DropDown");

			// select first item in combo box
			comboBoxField2.SelectedIndex = 0;

			// click the down arrow button
			InvokeClickMethod("buttonAddField2_Click");

			// verify that field2 item is now in expression box.
			TextBox textBoxExpression = getExpressionTextBox();
			Assert.AreEqual("<<Q1:a>>", textBoxExpression.Text);
		}


		/// <summary>
		/// Verify that clicking the AddField2 button places choice text in the Expression box
		/// </summary>
		[Test]
		public void AddChoiceToExpression()
		{
			// get controls
			ComboBox comboBoxField1 = getField1ComboBox();
			ComboBox comboBoxOperators = getOperatorsComboBox();
			ComboBox comboBoxField2 = getField2ComboBox();

			// select Q2 field from Field1 box
			InvokeClickMethod("comboBoxField1_DropDown");
			comboBoxField1.SelectedIndex = 1;

			// select equals operator
			InvokeClickMethod("comboBoxOperators_DropDown");
			comboBoxOperators.SelectedIndex = 0;

			// select choice "a" from Field2 box
			InvokeClickMethod("comboBoxField2_DropDown");
			comboBoxField2.SelectedIndex = 0;

			// click the down arrow button
			InvokeClickMethod("buttonAddField2_Click");

			// verify that field2 item is now in expression box.
			TextBox textBoxExpression = getExpressionTextBox();
			Assert.AreEqual("a", textBoxExpression.Text);
		}


		/// <summary>
		/// Verify that an expression can be built in the Expression box
		/// </summary>
		[Test]
		public void BuildExpression()
		{
			// get combo box
			ComboBox comboBoxField2 = getField2ComboBox();

			// click the dropdown arrow
			InvokeClickMethod("comboBoxField2_DropDown");

			// get expression box
			TextBox textBoxExpression = getExpressionTextBox();

			// add FibField to expression box
			comboBoxField2.SelectedIndex = 0;
			InvokeClickMethod("buttonAddField2_Click");
			Assert.AreEqual("<<Q1:a>>", textBoxExpression.Text);

			// add arithmetic operator and constant to expression box
			textBoxExpression.SelectionStart = textBoxExpression.Text.Length;
			textBoxExpression.SelectionLength = 0;
			textBoxExpression.Paste(" + 5");
			Assert.AreEqual("<<Q1:a>> + 5", textBoxExpression.Text);

			// add arithmetic operator to expression box
			textBoxExpression.SelectionStart = textBoxExpression.Text.Length;
			textBoxExpression.SelectionLength = 0;
			textBoxExpression.Paste(" - ");
			Assert.AreEqual("<<Q1:a>> + 5 - ", textBoxExpression.Text);

			// add Variable to expression box
			textBoxExpression.SelectionStart = textBoxExpression.Text.Length;
			textBoxExpression.SelectionLength = 0;
			comboBoxField2.SelectedIndex = 3;
			InvokeClickMethod("buttonAddField2_Click");
			Assert.AreEqual("<<Q1:a>> + 5 - <<variable 1>>", textBoxExpression.Text);

			// replace FibField in expression box with variable
			textBoxExpression.SelectionStart = 0;
			textBoxExpression.SelectionLength = 12;
			comboBoxField2.SelectedIndex = 3;
			InvokeClickMethod("buttonAddField2_Click");
			Assert.AreEqual("<<variable 1>> - <<variable 1>>", textBoxExpression.Text);

		}

		[Test]
		public void Field2DataSource()
		{
			// get combo box
			ComboBox comboBoxField2 = getField2ComboBox();

			// click the dropdown arrow
			InvokeClickMethod("comboBoxField2_DropDown");

			// verify that datasource items are accessible through combo box
			Assert.AreEqual(4, comboBoxField2.Items.Count);

			Assert.IsTrue(comboBoxField2.Items[0] is Blank, "should be Blank");
			Assert.IsTrue(comboBoxField2.Items[1] is MCItem, "should be MCItem");
			Assert.IsTrue(comboBoxField2.Items[2] is MCItem, "should be MCItem");
			Assert.IsTrue(comboBoxField2.Items[3] is Variable, "should be Variable");
		}


		/// <summary>
		/// Verify that Operators combo box can be populated with operators
		/// </summary>
		[Test]
		public void OperatorsDataSource()
		{
			// get combo box
			ComboBox comboBoxOperators = getOperatorsComboBox();

			// establish data source
			comboBoxOperators.DataSource = HybridOperator.List.DataSource;

			// verify that datasource items are accessible through combo box
			Assert.AreEqual(HybridOperator.List.DataSource[0], comboBoxOperators.Items[0]);
		}


		/// <summary>
		/// Verify that Operators combo box contents change based on field type in Field1 combo box
		/// </summary>
		[Test]
		public void RelevantOperators()
		{
			// get field 1 combo box
			ComboBox comboBoxField1 = getField1ComboBox();

			// get operators combo box
			ComboBox comboBoxOperators = getOperatorsComboBox();

			// select FibField
			InvokeClickMethod("comboBoxField1_DropDown");
			comboBoxField1.SelectedIndex = 0;
			Assert.AreEqual(comboBoxOperators.Items[0], HybridOperator.List.DataSource[0], "Should be HybridOperator");

			// select MCField
			InvokeClickMethod("comboBoxField1_DropDown");
			comboBoxField1.SelectedIndex = 1;
			Assert.AreEqual(comboBoxOperators.Items[0], MCOneOperator.List.DataSource[0], "Should be MCOneOperator");

			// select MCField
			InvokeClickMethod("comboBoxField1_DropDown");
			comboBoxField1.SelectedIndex = 2;
			Assert.AreEqual(comboBoxOperators.Items[0], MCManyOperator.List.DataSource[0], "Should be MCManyOperator");

			// select Variable
			InvokeClickMethod("comboBoxField1_DropDown");
			comboBoxField1.SelectedIndex = 3;
			Assert.AreEqual(comboBoxOperators.Items[0], HybridOperator.List.DataSource[0], "Should be HybridOperator");

		}


		/// <summary>
		/// Verify that the correct operator names appear in the Operators combo box
		/// </summary>
		[Test]
		public void OperatorNames()
		{
			// get field 1 combo box
			ComboBox comboBoxField1 = getField1ComboBox();

			// get operators combo box
			ComboBox comboBoxOperators = getOperatorsComboBox();

			// select FibField
			InvokeClickMethod("comboBoxField1_DropDown");
			comboBoxField1.SelectedIndex = 0;

			InvokeClickMethod("comboBoxOperators_DropDown");
			comboBoxOperators.SelectedIndex = 0;

			Assert.AreEqual("equals", comboBoxOperators.Text);

			// select MCField
//			comboBoxField1.SelectedIndex = 1;
//			Assert.AreEqual(comboBoxOperators.Items[0], MCOneOperator.List.DataSource[0], "Should be MCOneOperator");

		}


		/// <summary>
		/// Verify that Field2 combo box contents change based on field type in Field1 combo box
		/// </summary>
		[Test]
		public void RelevantFields()
		{
			// get field 1 combo box
			ComboBox comboBoxField1 = getField1ComboBox();

			// get operators combo box
			ComboBox comboBoxField2 = getField2ComboBox();

			// select FibField
			comboBoxField1.SelectedIndex = 0;
			InvokeClickMethod("comboBoxField2_DropDown");

			Assert.AreEqual("Q1:a", ((IField)comboBoxField2.Items[0]).FieldName);
			Assert.AreEqual("Q2", ((IField)comboBoxField2.Items[1]).FieldName);
			Assert.AreEqual("Q3", ((IField)comboBoxField2.Items[2]).FieldName);
			Assert.AreEqual("variable 1", ((IField)comboBoxField2.Items[3]).FieldName);

			// select MCField
			comboBoxField1.SelectedIndex = 1;
			InvokeClickMethod("comboBoxField2_DropDown");
			Assert.AreEqual("a", comboBoxField2.Text);

			// select MCField
			comboBoxField1.SelectedIndex = 2;
			InvokeClickMethod("comboBoxField2_DropDown");
			Assert.AreEqual("a", comboBoxField2.Text);
		}


		/// <summary>
		/// Selects "Q1:a equals <<variable 1>>" in the Comparison controls
		/// </summary>
		private void selectCondition1()
		{
			// get controls
			ComboBox comboBoxField1 = getField1ComboBox();
			ComboBox comboBoxOperators = getOperatorsComboBox();
			ComboBox comboBoxField2 = getField2ComboBox();

			// select Q1:a field from Field1 box
			InvokeClickMethod("comboBoxField1_DropDown");
			comboBoxField1.SelectedIndex = 0;

			// select equals operator
			InvokeClickMethod("comboBoxOperators_DropDown");
			comboBoxOperators.SelectedIndex = 0;

			// select variable 1 field from Field2 box
			InvokeClickMethod("comboBoxField2_DropDown");
			comboBoxField2.SelectedIndex = 3;

			// add variable 1 field to expression box
			InvokeClickMethod("buttonAddField2_Click");

		}


		/// <summary>
		/// Selects "Q1:a is not blank" in the Comparison controls
		/// </summary>
		private void selectCondition3()
		{
			// get controls
			ComboBox comboBoxField1 = getField1ComboBox();
			ComboBox comboBoxOperators = getOperatorsComboBox();
			ComboBox comboBoxField2 = getField2ComboBox();

			// select Q1:a field from Field1 box
			InvokeClickMethod("comboBoxField1_DropDown");
			comboBoxField1.SelectedIndex = 0;

			// select is not blank operator
			InvokeClickMethod("comboBoxOperators_DropDown");
			comboBoxOperators.SelectedIndex = 11;

		}


		/// <summary>
		/// Adds "Q1:a equals <<variable 1>>" to the Conditions box
		/// </summary>
		private int addCondition1()
		{
			// get controls
			ComboBox comboBoxField1 = getField1ComboBox();
			ComboBox comboBoxOperators = getOperatorsComboBox();
			ComboBox comboBoxField2 = getField2ComboBox();
			TextBox textBoxExpression = getExpressionTextBox();

			// select Q1:a field from Field1 box
			InvokeClickMethod("comboBoxField1_DropDown");
			comboBoxField1.SelectedIndex = 0;

			// select equals operator
			InvokeClickMethod("comboBoxOperators_DropDown");
			comboBoxOperators.SelectedIndex = 0;

			// select variable 1 field from Field2 box
			InvokeClickMethod("comboBoxField2_DropDown");
			comboBoxField2.SelectedIndex = 3;

			// empty expression box
			textBoxExpression.Text = "";

			// add variable 1 field to expression box
			InvokeClickMethod("buttonAddField2_Click");

			// add condition to conditions box
			InvokeClickMethod("buttonAddCondition_Click");

			// verify that complete condition is now in conditions box.
			TextBox textBoxConditions = getConditionsTextBox();

			return textBoxConditions.Text.Length;
		}


		/// <summary>
		/// Adds "Q2 equals a" to the Conditions box
		/// </summary>
		private int addCondition2()
		{
			// get controls
			ComboBox comboBoxField1 = getField1ComboBox();
			ComboBox comboBoxOperators = getOperatorsComboBox();
			ComboBox comboBoxField2 = getField2ComboBox();
			TextBox textBoxExpression = getExpressionTextBox();


			// select Q2 field from Field1 box
			InvokeClickMethod("comboBoxField1_DropDown");
			comboBoxField1.SelectedIndex = 1;

			// select equals operator
			InvokeClickMethod("comboBoxOperators_DropDown");
			comboBoxOperators.SelectedIndex = 0;

			// select choice "a" from Field2 box
			InvokeClickMethod("comboBoxField2_DropDown");
			comboBoxField2.SelectedIndex = 0;

			// empty expression box
			textBoxExpression.Text = "";

			// add choice a to expression box
			InvokeClickMethod("buttonAddField2_Click");

			// add condition to conditions box
			InvokeClickMethod("buttonAddCondition_Click");

			// verify that complete condition is now in conditions box.
			TextBox textBoxConditions = getConditionsTextBox();

			return textBoxConditions.Text.Length;
		}


		/// <summary>
		/// Verify that clicking the AddCondition button places text in the Conditions box
		/// </summary>
		[Test]
		public void AddCondition1()
		{
			// add "Q1:a equals <<variable 1>>" condition to Conditions box
			addCondition1();

			// verify that complete condition is now in conditions box.
			TextBox textBoxConditions = getConditionsTextBox();
			Assert.AreEqual("Form 1:Q1:a equals variable 1", textBoxConditions.Text);
		}

		/// <summary>
		/// Verify that clicking the AddCondition button places text in the Conditions box
		/// </summary>
		[Test]
		public void AddCondition2()
		{
			// add "Q2 equals a" condition to Conditions box
			addCondition2();

			// verify that complete condition is now in conditions box.
			TextBox textBoxConditions = getConditionsTextBox();
			Assert.AreEqual("Form 1:Q2 equals a", textBoxConditions.Text);
		}

		/// <summary>
		/// Adds "AND" to the Conditions box
		/// </summary>
		private int addAnd()
		{
			// click the AND button
			InvokeClickMethod("buttonAnd_Click");

			// get text box
			TextBox textBoxConditions = getConditionsTextBox();

			return textBoxConditions.Text.Length;
		}


		/// <summary>
		/// Verify that clicking the AND button places an AND operator in the Conditions box
		/// </summary>
		[Test]
		public void And()
		{
			// add "AND" to the Conditions box
			addAnd();

			// get text box
			TextBox textBoxConditions = getConditionsTextBox();

			// verify that AND operator is now in expression box.
			Assert.AreEqual("AND", textBoxConditions.Text);
		}


		/// <summary>
		/// Adds "OR" to the Conditions box
		/// </summary>
		private int or()
		{
			// click the OR button
			InvokeClickMethod("buttonOr_Click");

			// get text box
			TextBox textBoxConditions = getConditionsTextBox();

			return textBoxConditions.Text.Length;
		}


		/// <summary>
		/// Verify that clicking the OR button places an OR operator in the Conditions box
		/// </summary>
		[Test]
		public void Or()
		{
			// add "OR" to the Conditions box
			or();

			// get text box
			TextBox textBoxConditions = getConditionsTextBox();

			// verify that AND operator is now in expression box.
			Assert.AreEqual("OR", textBoxConditions.Text);
		}


		/// <summary>
		/// Verify that adding conditions and operators to the Conditions box properly updates the selection location
		/// </summary>
		[Test]
		public void ConditionsDefaultSelection()
		{
			// get text boxes
			TextBox textBoxConditions = getConditionsTextBox();
			TextBox textBoxExpression = getExpressionTextBox();

			// verify that selection is at beginning of Conditions box
			Assert.AreEqual(0, textBoxConditions.SelectionStart);

			// add "Q1:a equals <<variable 1>>" condition to Conditions box
			int conditionLength = addCondition1();

			// verify that selection is at end of added condition
			Assert.AreEqual("Form 1:Q1:a equals variable 1", textBoxConditions.Text);
			Assert.AreEqual(conditionLength, textBoxConditions.SelectionStart);

			// add "AND" to the Conditions box
			conditionLength = addAnd();

			// verify that selection is at end of added condition
			Assert.AreEqual("Form 1:Q1:a equals variable 1 AND", textBoxConditions.Text);
			Assert.AreEqual(conditionLength, textBoxConditions.SelectionStart);

			// add "Q2 equals a" condition to Conditions box
			conditionLength = addCondition2();

			// verify that selection is at end of added condition
			Assert.AreEqual("a", textBoxExpression.Text);
			Assert.AreEqual("Form 1:Q1:a equals variable 1 AND Form 1:Q2 equals a", textBoxConditions.Text);
			Assert.AreEqual(conditionLength, textBoxConditions.SelectionStart);

		}


		/// <summary>
		/// Verify that selecting a complete condition enables the add condition button
		/// </summary>
		[Test]
		public void EnableAddConditionButton()
		{
			// get add condition button
			Button buttonAddCondition = getAddConditionButton();

			// verify that button is disabled by default
			Assert.IsFalse(buttonAddCondition.Enabled, "add condition button should be disabled");

			// verify that button is enabled under "Q1:a equals <<variable 1>> condition
			selectCondition1();
			Assert.IsTrue(buttonAddCondition.Enabled, "add condition button should be enabled");

			// verify that button is disabled when expresion box is cleared
			TextBox textBoxExpression = getExpressionTextBox();
			textBoxExpression.Text = ""	;
			Assert.IsFalse(buttonAddCondition.Enabled, "add condition button should be disabled");

			// verify that button is enabled under "Q1:a is not blank"" condition
			selectCondition3();
			Assert.IsTrue(buttonAddCondition.Enabled, "add condition button should be enabled");
		}



		/// <summary>
		/// Verify that selecting one or more condition elements enables the remove button
		/// </summary>
		[Test]
		public void EnableRemoveButton()
		{
			// get remove button
			Button buttonRemove = getRemoveButton();

			// verify that button is disabled by default
			Assert.IsFalse(buttonRemove.Enabled, "remove button should be disabled");

			// add "Q1:a equals <<variable 1>> AND Q2 equals a" to conditions box
			addCondition1();
			addAnd();
			addCondition2();

			// get conditions text box
			TextBox textBoxConditions = getConditionsTextBox();

			// select first condition element
			textBoxConditions.SelectionStart = 0;
			textBoxConditions.SelectionLength = 1;

			// click in conditions box
			InvokeClickMethod("textBoxConditions_Click");
			
			// verify that button is enabled
			Assert.IsTrue(buttonRemove.Enabled, "remove button should be enabled");

		}


		/// <summary>
		/// Verify that clicking the Remove button removes a single condition
		/// </summary>
		[Test]
		public void RemoveSingleCondition()
		{
			// get remove button
			Button buttonRemove = getRemoveButton();

			// get conditions text box
			TextBox textBoxConditions = getConditionsTextBox();

			// add "Q1:a equals <<variable 1>> AND Q2 equals a" to conditions box
			addCondition1();
			addAnd();
			addCondition2();

			// verify that complete set of conditions is now in conditions box
			Assert.AreEqual(3, testDialog.Conditions.Count);
			Assert.AreEqual("Form 1:Q1:a equals variable 1 AND Form 1:Q2 equals a", testDialog.Conditions.ToString());

			// select second condition element (AND)
			textBoxConditions.SelectionStart = 30;
			textBoxConditions.SelectionLength = 3;

			// click in conditions box
			InvokeClickMethod("textBoxConditions_Click");
			
			// click the Remove button
			InvokeClickMethod("buttonRemove_Click");

			// verify that modified set of conditions is now in conditions box
			Assert.AreEqual("Form 1:Q1:a equals variable 1 Form 1:Q2 equals a", testDialog.Conditions.ToString());
			Assert.AreEqual("Form 1:Q1:a equals variable 1 Form 1:Q2 equals a", textBoxConditions.Text);
		}


		/// <summary>
		/// Verify that clicking the Remove button removes multiple conditions
		/// </summary>
		[Test]
		public void RemoveMultipleConditions()
		{
			// get remove button
			Button buttonRemove = getRemoveButton();

			// get conditions text box
			TextBox textBoxConditions = getConditionsTextBox();

			// add "Q1:a equals <<variable 1>> AND Q2 equals a" to conditions box
			addCondition1();
			addAnd();
			addCondition2();

			// verify that complete set of conditions is now in conditions box
			Assert.AreEqual(3, testDialog.Conditions.Count);
			Assert.AreEqual("Form 1:Q1:a equals variable 1 AND Form 1:Q2 equals a", testDialog.Conditions.ToString());

			// select "Q2 equals a"
			textBoxConditions.SelectionStart = 41;
			textBoxConditions.SelectionLength = 18;

			// click in conditions box
			InvokeClickMethod("textBoxConditions_Click");

			// click the Remove button
			InvokeClickMethod("buttonRemove_Click");

			// verify that modified set of conditions is now in conditions box
			Assert.AreEqual("Form 1:Q1:a equals variable 1 AND", testDialog.Conditions.ToString());
			Assert.AreEqual("Form 1:Q1:a equals variable 1 AND", textBoxConditions.Text);

			// select "Q1:a equals <<variable 1>>"
			textBoxConditions.SelectionStart = 0;
			textBoxConditions.SelectionLength = 29;

			// click in conditions box
			InvokeClickMethod("textBoxConditions_Click");
			
			// click the Remove button
			InvokeClickMethod("buttonRemove_Click");

			// verify that modified set of conditions is now in conditions box
			Assert.AreEqual("AND", testDialog.Conditions.ToString());
			Assert.AreEqual("AND", textBoxConditions.Text);
		}


		/// <summary>
		/// Verify that And button is properly enabled and disabled based on conditions in conditions box.
		/// </summary>
		[Test]
		public void EnableAndButton()
		{
			// get and button
			Button buttonAnd = getAndButton();

			// get conditions text box
			TextBox textBoxConditions = getConditionsTextBox();

			// verify that button is disabled by default
			Assert.IsFalse(buttonAnd.Enabled, "and button should be disabled");

			// add "Q1:a equals <<variable 1>>" to conditions box
			addCondition1();

			// verify that button is enabled
			Assert.IsTrue(buttonAnd.Enabled, "and button should be enabled");

			// add "and" to conditions box
			addAnd();

			// verify that button is disabled
			Assert.IsFalse(buttonAnd.Enabled, "and button should be disabled");

			// add "Q2 equals a" to conditions box
			addCondition2();

			// verify that button is enabled
			Assert.IsTrue(buttonAnd.Enabled, "and button should be enabled");

		}


		/// <summary>
		/// Verify that operators may be inserted between conditions
		/// </summary>
		[Test]
		public void InsertOperators()
		{
			// get conditions text box
			TextBox textBoxConditions = getConditionsTextBox();

			// add "Q1:a equals <<variable 1>> Q2 equals a" to conditions box
			addCondition1();
			addCondition2();

			// verify that 2 conditions are in conditions box
			Assert.AreEqual("Form 1:Q1:a equals variable 1 Form 1:Q2 equals a", textBoxConditions.Text);

			// set selection location between elements
			textBoxConditions.SelectionStart = 33;
			textBoxConditions.SelectionLength = 0;

			// click the And button
			InvokeClickMethod("buttonAnd_Click");

			// verify that modified set of conditions is now in conditions box
			Assert.AreEqual("Form 1:Q1:a equals variable 1 AND Form 1:Q2 equals a", textBoxConditions.Text);
		}


		/// <summary>
		/// Verify that clicking a condition selects (highlights) it
		/// </summary>
		[Test]
		public void ClickCondition()
		{
			// get conditions text box
			TextBox textBoxConditions = getConditionsTextBox();

			// add "Q1:a equals <<variable 1>> AND Q2 equals a" to conditions box
			addCondition1();
			addAnd();
			addCondition2();

			// verify that complete set of conditions is now in conditions box
			Assert.AreEqual(3, testDialog.Conditions.Count);
			Assert.AreEqual("Form 1:Q1:a equals variable 1 AND Form 1:Q2 equals a", testDialog.Conditions.ToString());

			// set selection to between 'A' and 'N'
			textBoxConditions.SelectionStart = 31;
			textBoxConditions.SelectionLength = 0;

			// double-click in conditions box
			InvokeClickMethod("textBoxConditions_Click");

			// verify that entire third condition element is selected
			Assert.AreEqual(30, textBoxConditions.SelectionStart);
			Assert.AreEqual(3, textBoxConditions.SelectionLength);
		}

	}
}
