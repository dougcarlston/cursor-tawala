using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the Expression class
	/// </summary>
    [Ignore("REQUIRES Fix-ups for new classes")]
    [TestFixture]
	public class ExpressionTest
	{
#if FIXED
        private IForm form;
		private FibItem fibItem1;
		private Process process;

		// execute this once at beginning of tests
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
            Util.NewTestProject();

			// create form
			form = Project.Current.AddForm();

			// create process
			process = Project.Current.AddProcess();

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			// create FIB item
			fibItem1 = new FibItem();

			// add FIB item to form
			form.ItemList.Add(fibItem1);

			// create fields from FIB item and blank indexes
			FibItem fibItem2 = new FibItem();
			fibItem2.Text = "__ __ __ __ __";
			fibItem2.BlankList[0].AlternateLabel = "number1";
			fibItem2.BlankList[1].AlternateLabel = "number2";
			fibItem2.BlankList[2].AlternateLabel = "number3";
			fibItem2.BlankList[3].AlternateLabel = "FirstName";
			fibItem2.BlankList[4].AlternateLabel = "&<Field's \"Bad\" Name>:b";

			// add FIB item to form
			form.ItemList.Add(fibItem2);

			// add MC item to form
			form.ItemList.Add(new McqItem());
		}

		[Test]
		public void RecordField()
		{
			Record record = new Record("Record");
			RecordField recordField = new RecordField(record, fibItem1.BlankList[0]);
			Expression expression = new Expression(recordField);
			Assert.IsNotNull(expression);

			Assert.AreEqual(1, expression.Elements.Count);
			Assert.AreEqual("<<Record:Form 1:Q1:a>>", expression.Elements[0].ToString());
		}

		[Test]
		public void OneField()
		{
			Expression expression = new Expression("<<Q1:a>>", process.GetValidFields(0));
			Assert.IsNotNull(expression);

			Assert.AreEqual(1, expression.Elements.Count);
			Assert.AreEqual("<<Form 1:Q1:a>>", expression.Elements[0].ToString());
		}

		[Test]
		public void OneString()
		{
			Expression expression = new Expression("2", process.GetValidFields(0));
			Assert.IsNotNull(expression);

			Assert.AreEqual(1, expression.Elements.Count);
			Assert.AreEqual("2", expression.Elements[0].ToString());
		}

		[Test]
		public void OneFieldOneString()
		{
			Expression expression = new Expression("<<Q1:a>> + 2", process.GetValidFields(0));
			Assert.IsNotNull(expression);

			Assert.AreEqual(2, expression.Elements.Count);
			Assert.AreEqual("<<Form 1:Q1:a>>", expression.Elements[0].ToString());
			Assert.AreEqual(" + 2", expression.Elements[1].ToString());
		}

		[Test]
		public void StringFieldString()
		{
			Expression expression = new Expression("3 * <<Q1:a>> + 2", process.GetValidFields(0));
			Assert.IsNotNull(expression);

			Assert.AreEqual(3, expression.Elements.Count);
			Assert.AreEqual("3 * ", expression.Elements[0].ToString());
			Assert.AreEqual("<<Form 1:Q1:a>>", expression.Elements[1].ToString());
			Assert.AreEqual(" + 2", expression.Elements[2].ToString());
		}

		[Test]
		public void ParenthesizedFieldsAndStrings()
		{
			Expression expression = new Expression("(<<Form 1:number1>> / (<<Form 1:number2>> + 7) - <<Form 1:number3>>) * 5", process.GetValidFields(0));
			Assert.IsNotNull(expression);

			Assert.AreEqual(7, expression.Elements.Count);
			Assert.AreEqual("(", expression.Elements[0].ToString());
			Assert.AreEqual("<<Form 1:number1>>", expression.Elements[1].ToString());
			Assert.AreEqual(" / (", expression.Elements[2].ToString());
			Assert.AreEqual("<<Form 1:number2>>", expression.Elements[3].ToString());
			Assert.AreEqual(" + 7) - ", expression.Elements[4].ToString());
			Assert.AreEqual("<<Form 1:number3>>", expression.Elements[5].ToString());
			Assert.AreEqual(") * 5", expression.Elements[6].ToString());
		}

		[Test]
		public void NestedBrackets()
		{
			Expression expression = new Expression("<<&<Field's \"Bad\" Name>:b>>", process.GetValidFields(0));

			Assert.AreEqual(1, expression.Elements.Count);
			Assert.IsTrue(expression.Elements[0] is FieldElement, "Element is not FieldElement");
			Assert.AreEqual("<<Form 1:&<Field's \"Bad\" Name>:b>>", expression.Elements[0].ToString());
		}

		[Test]
		public void EscapableCharacters()
		{
			Expression expression = new Expression("Joe's <<Form 1:FirstName>> is \"<\" mine & \">\" his		<<&<Field's \"Bad\" Name>:b>>!", process.GetValidFields(0));

			Assert.AreEqual(5, expression.Elements.Count);
			Assert.AreEqual("Joe's ", expression.Elements[0].ToString());
			Assert.AreEqual("<<Form 1:FirstName>>", expression.Elements[1].ToString());
			Assert.AreEqual(" is \"<\" mine & \">\" his		", expression.Elements[2].ToString());
			Assert.AreEqual("<<Form 1:&<Field's \"Bad\" Name>:b>>", expression.Elements[3].ToString());
			Assert.AreEqual("!", expression.Elements[4].ToString());
			Assert.AreEqual("Joe's <<Form 1:FirstName>> is \"<\" mine & \">\" his		<<Form 1:&<Field's \"Bad\" Name>:b>>!", expression.ToString());
		}

		[Test]
		public void TestToString()
		{
			Expression expression = new Expression("(<<Form 1:number1>> / (<<Form 1:number2>> + 7) - <<Form 1:number3>>) * 5", process.GetValidFields(0));

			Assert.AreEqual("(<<Form 1:number1>> / (<<Form 1:number2>> + 7) - <<Form 1:number3>>) * 5", expression.ToString());
		}

		[Test]
		public void GetXml()
		{
			string expString;

			Expression expression = new Expression("(<<Form 1:number1>> / (<<Form 1:number2>> + 7) - <<Form 1:number3>>) * 5", process.GetValidFields(0));

			expString = "<mul>\r\n" +
						"<sub>\r\n" +
						"<div>\r\n" +
						"<operand field=\"Form 1:number1\"/>\r\n" +
						"<add>\r\n" +
						"<operand field=\"Form 1:number2\"/>\r\n" +
						"<operand value=\"7\"/>\r\n" +
						"</add>\r\n" +
						"</div>\r\n" +
						"<operand field=\"Form 1:number3\"/>\r\n" +
						"</sub>\r\n" +
						"<operand value=\"5\"/>\r\n" +
						"</mul>\r\n";

			Assert.AreEqual(expString, expression.ToXml());

		}
#endif
	}
}
