using System;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using NUnit.Framework;
using Tawala.Projects;
using System.ComponentModel;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Class to test FieldSource class.
	/// </summary>
	[TestFixture]
	public class FieldSourceTest
	{
		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tFieldSource = typeof(FieldSource);

		/// <summary>
		/// Invokes the Fieldsource.notify method
		/// </summary>
		private void fieldSourceNotify(FieldSource fieldSource, string info)
		{
			// get method by name
			MethodInfo method = tFieldSource.GetMethod("notify", flags);

			// create arguments appropriate for method
			Object[] args = new object[1];
			args[0] = info;

			// invoke method
			method.Invoke(fieldSource, args);
		}


		FieldList testFieldList;

		private IField generateFieldList()
		{
			return (testFieldList);
		}

		private IField generateVariableList()
		{
			return (Project.Current.AllVariables);
		}

		IForm form;
		McqItem mcItem;

		private IField generateMCItemFieldList()
		{
			FieldList fieldList = new FieldList();

			MCItemProxy mcItemProxy = new MCItemProxy();
			Record record = new Record("Record");

			fieldList.Add(mcItem);
			fieldList.Add(new RecordField(record, mcItem));

			fieldList.Add(mcItemProxy);
			fieldList.Add(new RecordField(record, mcItemProxy));

			return fieldList;
		}

		private void postProcess()
		{
		}

		[SetUp]
		public void SetUp()
		{
			testFieldList = new FieldList();
		}

		[Test]
		public void ConstructGenerator() 
		{
			FieldSource fieldSource = new FieldSource(generateFieldList);

			Assert.IsNotNull(fieldSource);
		}

		[Test]
		public void ConstructPostProcessor()
		{
			FieldSource fieldSource = new FieldSource(generateFieldList, postProcess);

			Assert.IsNotNull(fieldSource);
		}

		[Test]
		public void BindingSource()
		{
			testFieldList.Add(new Field("Field 1"));
			testFieldList.Add(new Field("Field 2"));

			FieldSource fieldSource = new FieldSource(generateFieldList);

			BindingSource bindingSource = new BindingSource();
			
			Assert.AreEqual(0, bindingSource.List.Count);

			bindingSource.DataSource = fieldSource;

			fieldSource.Update();
			Assert.AreEqual(2, bindingSource.List.Count);
			Assert.AreEqual("Field 1", bindingSource.List[0].ToString());
			Assert.AreEqual("Field 2", bindingSource.List[1].ToString());
		}

		BindingSource listBox1BindingSource;

		public class TestForm : System.Windows.Forms.Form
		{
			public ListBox ListBox1 = new ListBox();

			public TestForm()
			{
				this.ClientSize = new Size(400, 300);
				this.Text = "TestForm";

				ListBox1.Location = new Point(25, 25);
				ListBox1.Name = "ListBox1";
				ListBox1.Size = new Size(100, 200);

				this.Controls.Add(ListBox1);
			}

			private void InitializeComponent()
			{
			}
		}

		[Test]
		public void ListBox()
		{
			FieldSource fieldSource = new FieldSource(generateFieldList);

			listBox1BindingSource = new BindingSource();
			listBox1BindingSource.DataSource = fieldSource;

			TestForm testForm = new TestForm();
			testForm.ListBox1.DisplayMember = "FieldName";
			testForm.ListBox1.DataSource = listBox1BindingSource;

			fieldSourceNotify(fieldSource, "");
			Assert.AreEqual(0, testForm.ListBox1.Items.Count);

			testFieldList.Add(new Field("Field 1"));
			fieldSourceNotify(fieldSource, "");
			Assert.AreEqual(1, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Field 1", testForm.ListBox1.Items[0].ToString());

			testFieldList.Add(new Field("Field 2"));
			fieldSourceNotify(fieldSource, "");
			Assert.AreEqual(2, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Field 2", testForm.ListBox1.Items[1].ToString());
		}

		[Test]
		public void FormItemAdded()
		{
			Project.NewTestProject();
			Tawala.Projects.IForm form = Project.Current.AddForm();

			FieldSource fieldSource = new FieldSource(form.GetFields);

			listBox1BindingSource = new BindingSource();
			listBox1BindingSource.DataSource = fieldSource;

			TestForm testForm = new TestForm();
			testForm.ListBox1.DisplayMember = "FieldName";
			testForm.ListBox1.DataSource = listBox1BindingSource;

			Assert.AreEqual(0, testForm.ListBox1.Items.Count);

			form.ItemList.Add(new FibItem());
			Assert.AreEqual(1, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Form 1:Q1:a", testForm.ListBox1.Items[0].ToString());

			form.ItemList.Add(new McqItem());
			Assert.AreEqual(2, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Form 1:Q2", testForm.ListBox1.Items[1].ToString());
		}

		[Test]
		public void FormItemRemoved()
		{
			Project.NewTestProject();
			Tawala.Projects.IForm form = Project.Current.AddForm();

			FieldSource fieldSource = new FieldSource(form.GetFields);

			listBox1BindingSource = new BindingSource();
			listBox1BindingSource.DataSource = fieldSource;

			TestForm testForm = new TestForm();
			testForm.ListBox1.DisplayMember = "FieldName";
			testForm.ListBox1.DataSource = listBox1BindingSource;

			form.ItemList.Add(new FibItem());
			form.ItemList.Add(new FibItem());
			form.ItemList.Add(new McqItem());
			Assert.AreEqual(3, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Form 1:Q1:a", testForm.ListBox1.Items[0].ToString());
			Assert.AreEqual("Form 1:Q2:a", testForm.ListBox1.Items[1].ToString());
			Assert.AreEqual("Form 1:Q3", testForm.ListBox1.Items[2].ToString());

			form.ItemList.RemoveAt(1);
			Assert.AreEqual(2, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Form 1:Q1:a", testForm.ListBox1.Items[0].ToString());
			Assert.AreEqual("Form 1:Q2", testForm.ListBox1.Items[1].ToString());

		}

		[Test]
		public void FormItemRepositioned()
		{
			Project.NewTestProject();
			Tawala.Projects.IForm form = Project.Current.AddForm();

			FieldSource fieldSource = new FieldSource(form.GetFields);

			listBox1BindingSource = new BindingSource();
			listBox1BindingSource.DataSource = fieldSource;

			TestForm testForm = new TestForm();
			testForm.ListBox1.DisplayMember = "FieldName";
			testForm.ListBox1.DataSource = listBox1BindingSource;

			form.ItemList.Add(new FibItem());
			form.ItemList.Add(new FibItem());
			Assert.AreEqual(2, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Form 1:Q1:a", testForm.ListBox1.Items[0].ToString());
			Assert.AreEqual("Form 1:Q2:a", testForm.ListBox1.Items[1].ToString());

			form.ItemList.Insert(1, new McqItem());
			Assert.AreEqual(3, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Form 1:Q1:a", testForm.ListBox1.Items[0].ToString());
			Assert.AreEqual("Form 1:Q2", testForm.ListBox1.Items[1].ToString());
			Assert.AreEqual("Form 1:Q3:a", testForm.ListBox1.Items[2].ToString());

		}

		[Test]
		public void BlankRenamed()
		{
			Project.NewTestProject();
			Tawala.Projects.IForm form = Project.Current.AddForm();

			FieldSource fieldSource = new FieldSource(form.GetFields);

			listBox1BindingSource = new BindingSource();
			listBox1BindingSource.DataSource = fieldSource;

			TestForm testForm = new TestForm();
			testForm.ListBox1.DisplayMember = "FieldName";
			testForm.ListBox1.DataSource = listBox1BindingSource;

			Assert.AreEqual(0, testForm.ListBox1.Items.Count);

			FibItem fibItem1 = new FibItem();
			fibItem1.BlankList[0].AlternateLabel = "FIB Item 1";
			form.ItemList.Add(fibItem1);
			Assert.AreEqual(1, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Form 1:FIB Item 1", testForm.ListBox1.Items[0].ToString());

			fibItem1.BlankList[0].AlternateLabel = "FIB Item 1 Renamed";
			Assert.AreEqual("Form 1:FIB Item 1 Renamed", testForm.ListBox1.Items[0].ToString());
		}

		[Test]
		public void MCItemRenamed()
		{
			Project.NewTestProject();
			Tawala.Projects.IForm form = Project.Current.AddForm();

			FieldSource fieldSource = new FieldSource(form.GetFields);

			listBox1BindingSource = new BindingSource();
			listBox1BindingSource.DataSource = fieldSource;

			TestForm testForm = new TestForm();
			testForm.ListBox1.DisplayMember = "FieldName";
			testForm.ListBox1.DataSource = listBox1BindingSource;

			Assert.AreEqual(0, testForm.ListBox1.Items.Count);

			McqItem mcItem1 = new McqItem();
			mcItem1.AlternateLabel = "MC Item 1";
			form.ItemList.Add(mcItem1);
			Assert.AreEqual(1, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Form 1:MC Item 1", testForm.ListBox1.Items[0].ToString());

			mcItem1.AlternateLabel = "MC Item 1 Renamed";
			Assert.AreEqual("Form 1:MC Item 1 Renamed", testForm.ListBox1.Items[0].ToString());
		}

		[Test]
		public void VariableAdded()
		{
			Project.NewTestProject();

			FieldSource fieldSource = new FieldSource(generateVariableList);

			listBox1BindingSource = new BindingSource();
			listBox1BindingSource.DataSource = fieldSource;

			TestForm testForm = new TestForm();
			testForm.ListBox1.DisplayMember = "FieldName";
			testForm.ListBox1.DataSource = listBox1BindingSource;

			Assert.AreEqual(0, testForm.ListBox1.Items.Count);

			Process process = Project.Current.AddProcess();
			process.Variables.AddUnique("Variable 1");
			Assert.AreEqual(2, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Variable 1", testForm.ListBox1.Items[0].ToString());
		}

		[Test]
		public void PostProcess()
		{
			Project.NewTestProject();
			Tawala.Projects.IForm form = Project.Current.AddForm();

			FieldSource fieldSource = new FieldSource(form.GetFields, postProcessTester);

			listBox1BindingSource = new BindingSource();
			listBox1BindingSource.DataSource = fieldSource;

			TestForm testForm = new TestForm();
			testForm.ListBox1.DisplayMember = "FieldName";
			testForm.ListBox1.DataSource = listBox1BindingSource;

			Assert.AreEqual(0, testForm.ListBox1.Items.Count);

			postProcessTestResult = false;

			form.ItemList.Add(new FibItem());
			Assert.AreEqual(1, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Form 1:Q1:a", testForm.ListBox1.Items[0].ToString());

			Assert.AreEqual(true, postProcessTestResult);
		}

		private bool postProcessTestResult;
		private void postProcessTester()
		{
			postProcessTestResult = true;
		}

		[Test]
		public void MCItems()
		{
			Project.NewTestProject();
			form = Project.Current.AddForm();
			mcItem = new McqItem();
			form.ItemList.Add(mcItem);

			FieldSource fieldSource = new FieldSource(generateMCItemFieldList);

			listBox1BindingSource = new BindingSource();
			listBox1BindingSource.DataSource = fieldSource;

			TestForm testForm = new TestForm();
			testForm.ListBox1.DisplayMember = "FieldName";
			testForm.ListBox1.DataSource = listBox1BindingSource;

			fieldSourceNotify(fieldSource, "");
			Console.WriteLine("MCItems: testForm.ListBox1.Items[0].GetType() = {0}", testForm.ListBox1.Items[0].GetType());

			Assert.AreEqual(4, testForm.ListBox1.Items.Count);
			Assert.AreEqual("Form 1:Q1", testForm.ListBox1.Items[0].ToString());
			Assert.AreEqual("Record:Form 1:Q1", testForm.ListBox1.Items[1].ToString());
			Assert.AreEqual("(selection)", testForm.ListBox1.Items[2].ToString());
			Assert.AreEqual("Record:(selection)", testForm.ListBox1.Items[3].ToString());
		}

	}
}
