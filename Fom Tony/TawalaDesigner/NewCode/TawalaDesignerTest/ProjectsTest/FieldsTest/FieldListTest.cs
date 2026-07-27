using System;
using System.Windows.Forms;
using System.Drawing;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Class to test FieldList class.
	/// </summary>
    [Ignore("REQUIRES Fix-ups for new classes")]
    [TestFixture]
	public class FieldListTest
	{
#if FIXED
        private IForm form1;
		private FibItem fibItem1;
		private McqItem mcItem1;
		private TextItem textItem1;
		private FieldList fieldList;
		private Variable variable1;
		private VariableList variableList1;
		private Record record1;
		private QualifiedFieldList qualifiedFieldList1;

		[SetUp]
		public void SetUp()
		{
            Util.NewTestProject();
            form1 = Project.Current.AddForm();
			fieldList = new FieldList();

			fibItem1 = new FibItem();
			fibItem1.Text = "Fib Item 1: __________ __________";
			mcItem1 = new McqItem();

			textItem1 = new TextItem();

			form1.ItemList.Add(fibItem1);
			form1.ItemList.Add(mcItem1);
			form1.ItemList.Add(textItem1);

			variable1 = new Variable("Variable 1");

			variableList1 = new VariableList();
			variableList1.Add(new Variable("Variable 2"));
			variableList1.Add(new Variable("Variable 3"));
			variableList1.Add(new Variable("Variable 4"));

			record1 = new Record("Record1");
			qualifiedFieldList1 = new QualifiedFieldList(record1, form1.ItemList);
//			recordField = new RecordField(record1, form1.ItemList);

			fieldList.Add(fibItem1);
			fieldList.Add(mcItem1);
			fieldList.Add(textItem1);
			fieldList.Add(variable1);
			fieldList.Add(variableList1);
			fieldList.Add(form1.ItemList);
			fieldList.Add(qualifiedFieldList1);
//			fieldList.Add(recordField);

			Assert.AreEqual(7, fieldList.Count);
		}

		[Test]
		public void FibItemFieldName() 
		{
			Assert.AreEqual("Q1", fieldList[0].FieldName);
		}


		[Test]
		public void MCItemFieldName()
		{
			Assert.AreEqual("Q2", fieldList[1].FieldName);
		}


		[Test]
		public void TextItemFieldName()
		{
			Assert.AreEqual("T1", fieldList[2].FieldName);
		}


		[Test]
		public void VariableFieldName()
		{
			Assert.AreEqual("Variable 1", fieldList[3].FieldName);
		}


		[Test]
		public void FibItemDefaultEnumerator()
		{
			string[] fieldNames = new string[1] { "Q1" };

			int i = 0;

			foreach (IField field in fibItem1)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}

		[Test]
		public void FibItemRecursiveEnumerator()
		{
			string[] fieldNames = new string[2] { "Q1:a", "Q1:b" };

			int i = 0;

			foreach (IField field in fibItem1.RecursiveEnumerator)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}

		[Test]
		public void MCItemEnumerator()
		{
			string[] fieldNames = new string[1] { "Q2" };

			int i = 0;

			foreach (IField field in mcItem1)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}

		[Test]
		public void VariableEnumerator()
		{
			string[] fieldNames = new string[1] { "Variable 1" };

			int i = 0;

			foreach (IField field in variable1)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}

		[Test]
		public void VariableListEnumerator()
		{
			string[] fieldNames = new string[3] { "Variable 2", "Variable 3", "Variable 4" };

			int i = 0;

			foreach (IField field in variableList1)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}

		[Test]
		public void FormItemListDefaultEnumerator()
		{
			string[] fieldNames = new string[3]
			{
				"Q1",		// fibItem1
				"Q2",		// mcItem1
				"T1"		// textItem1
			};

			int i = 0;

			foreach (IField field in form1.ItemList)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}

		[Test]
		public void FormItemListRecursiveEnumerator()
		{
			string[] fieldNames = new string[3]
			{
				"Q1:a", "Q1:b",
				"Q2",
			};

			int i = 0;

			foreach (IField field in form1.ItemList.RecursiveEnumerator)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}

		[Test]
		public void QualifiedFieldListDefaultEnumerator()
		{
			string[] fieldNames = new string[3]
			{
				"Record1:Q1",
				"Record1:Q2",
				"Record1:T1"
			};

			int i = 0;

			foreach (IField field in qualifiedFieldList1)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}

		[Test]
		public void QualifiedFieldListRecursiveEnumerator()
		{
			string[] fieldNames = new string[3]
			{
				"Record1:Q1:a", "Record1:Q1:b",
				"Record1:Q2",
			};

			int i = 0;

			foreach (IField field in qualifiedFieldList1.RecursiveEnumerator)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}

		[Test]
		public void FieldListDefaultEnumerator()
		{
			string[] fieldNames = new string[]
			{
				"Q1",			// fibItem1
				"Q2",			// mcItem1
				"T1",			// textItem1
				"Variable 1",	// variable1
				"",				// variableList1
				"",				// form1.ItemList
				"Record1:Q2",	// qualifiedFieldList1
			};

			int i = 0;

			foreach (IField field in fieldList)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}

		[Test]
		public void FieldListRecursiveEnumerator()
		{
			string[] fieldNames = new string[]
			{
				"Q1:a", "Q1:b",
				"Q2",
				"Variable 1",
				"Variable 2", "Variable 3", "Variable 4",
				"Q1:a", "Q1:b", "Q2",
				"Record1:Q1:a", "Record1:Q1:b", "Record1:Q2",
			};

			int i = 0;

			foreach (IField field in fieldList.RecursiveEnumerator)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}

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
			string[] fieldNames = new string[]
			{
				"Q1:a", "Q1:b",
				"Q2",
				"Variable 1",
				"Variable 2", "Variable 3", "Variable 4",
				"Q1:a", "Q1:b", "Q2",
				"Record1:Q1:a", "Record1:Q1:b", "Record1:Q2",
			};

			FlatFieldList flatFieldList = new FlatFieldList(fieldList);

			TestForm testForm = new TestForm();
//			testForm.Show();
			testForm.ListBox1.DisplayMember = "FieldName";
			testForm.ListBox1.DataSource = null;
			testForm.ListBox1.DataSource = flatFieldList;

			int i = 0;

			foreach (string name in fieldNames)
			{
				Assert.AreEqual(name, ((IField)testForm.ListBox1.Items[i++]).FieldName);
			}
		}
#endif
	}
}
