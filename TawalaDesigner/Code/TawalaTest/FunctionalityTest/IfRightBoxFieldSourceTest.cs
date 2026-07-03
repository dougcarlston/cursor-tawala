using System;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using NUnit.Framework;
using Tawala.Projects;
using System.ComponentModel;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	[TestFixture]
	public class IfRightBoxFieldSourceTest
	{
		private IForm form;
		private Process process;
		private FormList forms;
		private RecordSet recordList1;
		private Record record1;
		private int processLineIndex = 0;
		private McqItem selectedMCItem;

		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tFieldSource = typeof(FieldSource);

		/// <summary>
		/// Invokes the Fieldsource.notify method
		/// </summary>
		public void FieldSource_notify(FieldSource fieldSource, string info)
		{
			// get method by name
			MethodInfo method = tFieldSource.GetMethod("notify", flags);

			// create arguments appropriate for method
			Object[] args = new object[1];
			args[0] = info;

			// invoke method
			method.Invoke(fieldSource, args);
		}


		private IField generateFieldList()
		{
			FieldList fieldList = new FieldList();

			fieldList.Add(selectedMCItem.Choices);
			fieldList.Add(process.GetValidMCFields(processLineIndex));
			
			return fieldList;
		}

		private McqItem mcItem;

		[SetUp]
		public void SetUp()
		{
			// create clean project
			Project.NewTestProject();

			// create form
			form = Project.Current.AddForm();

			// create process
			process = Project.Current.AddProcess();

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			// add new MC item to form
			mcItem = new McqItem();
			form.ItemList.Add(mcItem);

			// add new FIB item to form
			form.ItemList.Add(new FibItem());

			// create GET statement ('Get record list from Form 1')
			forms = new FormList();
			forms.Add(Project.Current.FormList[0]);
			recordList1 = new RecordSet("record list", forms);
			GetStatement getStatement = new GetStatement(recordList1);
			process.Lines.Add(new ProcessLineList(getStatement));

			// create FOR EACH statement ('For Each record in record list')
			record1 = new Record("record");
			ProcessLineList forEachLines1 = getForEachLines(recordList1, record1);
			process.Lines.Add(forEachLines1);

			// create IF STATEMENT 'If Q1 equals record:Q1'
			ProcessLineList ifLines = getIfWithQualifiedMCField();
			process.Lines.Insert(3, ifLines);

			// initialize "selected" MC item
			selectedMCItem = new McqItem();
			selectedMCItem.Choices.Add(new Choice("Choice B"));
		}

		private static ProcessLineList getForEachLines(RecordSet recordList, Record record)
		{
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			return forEachLines;
		}

		private ProcessLineList getIfWithQualifiedMCField()
		{
			// create process line 'If Q1 equals record:Q1'
			IfStatement ifStatement = new IfStatement();
			Expression expression = new Expression(new RecordField(record1, mcItem));
			ifStatement.Conditions = new Conditions(form.GetFields()["Q1"], MCOneOperator.List[MCOneOperator.Ops.mcEquals], expression);
			ProcessLineList ifLines = new ProcessLineList(ifStatement);
			return ifLines;
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
		public void CheckProcess()
		{
			int i = 0;
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If Form 1:Q1 equals record:Form 1:Q1", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());

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

			processLineIndex = 3;
			FieldSource_notify(fieldSource, "");

			Assert.AreEqual("a", testForm.ListBox1.Items[0].ToString());
			Assert.AreEqual("b", testForm.ListBox1.Items[1].ToString());
			Assert.AreEqual("Form 1:Q1", testForm.ListBox1.Items[2].ToString());
			Assert.AreEqual("record:Form 1:Q1", testForm.ListBox1.Items[3].ToString());
		}

	}
}
