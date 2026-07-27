using System;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
	public class ProcessElementListTest
	{
#if FIXED
		private IForm form;
		private Process process;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
            Util.NewTestProject();

			// create form
			form = Project.Current.AddForm();

			// create process
			process = Project.Current.AddProcess();

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			// create FIB items
			FibItem fibItem1 = new FibItem();
			FibItem fibItem2 = new FibItem();

			// add new FIB items to form
			form.ItemList.Add(fibItem1);
			form.ItemList.Add(fibItem2);
		}

		[Test]
		public void ConstructAppend()
		{
			// create process line list from statement
			Document doc1 = new Document("Document 1");
			Document doc2 = new Document("Document 2");
			ProcessStatement statement = new AppendStatement(doc1, doc2);

			ProcessElementList list = new ProcessElementList(statement);

			//Assertions 
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("Append Document 1 to Document 2", list[0].ToString());
		}

		[Test]
		public void ConstructIf()
		{
			// create process line list from if statement
			Field field = new Field("Q1:a");
			ComparisonOperator compOp = StringOperator.List[StringOperator.Ops.beginsWith];
			string expressionString = "S";
			Conditions conditions = new Conditions(field, compOp, new Expression(expressionString));
			ProcessStatement ifStatement1 = new IfStatement(conditions);
			ProcessElementList ifList1 = new ProcessElementList(ifStatement1);

			string[] expectedStrings = new string[] {
				"If Q1:a begins with \"S\"",
				"(",
				")"
			};

			int i = 0;

			foreach (IProcessElement element in ifList1.RecursiveEnumerator)
			{
				Assert.AreEqual(expectedStrings[i++], element.ToString());
			}
		}


		[Test]
		public void ConstructIfWithOtherwise()
		{
			// create process line list from if statement
			Field field = new Field("Q1:a");
			ComparisonOperator compOp = StringOperator.List[StringOperator.Ops.beginsWith];
			string expressionString = "S";
			Conditions conditions = new Conditions(field, compOp, new Expression(expressionString));
			ProcessStatement ifStatement1 = new IfStatement(conditions, true);
			ProcessElementList ifList1 = new ProcessElementList(ifStatement1);

			string[] expectedStrings = new string[] {
				"If Q1:a begins with \"S\"",
				"(",
				")",
				"Otherwise",
				"(",
				")"
			};

			int i = 0;

			foreach (IProcessElement element in ifList1.RecursiveEnumerator)
			{
				Assert.AreEqual(expectedStrings[i++], element.ToString());
			}
		}

		[Test]
		public void Index()
		{
			// create process line list from if statement
			Field field = new Field("Q1:a");
			ComparisonOperator compOp = StringOperator.List[StringOperator.Ops.beginsWith];
			string expressionString = "S";
			Conditions conditions = new Conditions(field, compOp, new Expression(expressionString));
			ProcessStatement ifStatement1 = new IfStatement(conditions);
			ProcessElementList ifList1 = new ProcessElementList(ifStatement1);

			string[] expectedStrings = new string[] {
				"If Q1:a begins with \"S\"",
				"(",
				")"
			};

			int i = 0;

			ProcessElementList.Index index;

			for (index.value = 0; index.value < ifList1.Count; index.value++)
			{
				Assert.AreEqual(expectedStrings[i++], ifList1[index].ToString());
			}
		}
#endif
	}
}
