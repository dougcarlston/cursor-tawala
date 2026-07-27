using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("REQUIRES Fix-ups for new classes")]
    [TestFixture]
	public class SetStatementTest
	{
#if FIXED
        private IForm form;
		private Process process;
		private FibItem fibItem1;
		private FibItem fibItem2;
		private McqItem mcItem1;

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
			fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);
			fibItem2 = new FibItem();
			form.ItemList.Add(fibItem2);

			// create MC item
			mcItem1 = new McqItem();
			form.ItemList.Add(mcItem1);

			// add fields to Project's field mapper
			process.MappedFields.Fields.Add(fibItem1.BlankList[0]);
			process.MappedFields.Fields.Add(fibItem2.BlankList[0]);
			process.MappedFields.Map();
		}

		[Test]
		public void Construct() 
		{ 
			SetStatement statement = new SetStatement();

			Assert.IsNotNull(statement);
			Assert.IsFalse(statement.TreatArithmeticAsText);
		}

		[Test]
		public void ConstructFieldFromXml()
		{
			string xmlString =
				"<set field=\"Variable\">\r\n" +
				"<string field=\"Q1:a\"/>\r\n" +
				"</set>";

			IXmlElement element = new XmlElement(xmlString);
			SetStatement statement = new SetStatement(element, process.Name);

			Assert.AreEqual("Set Variable to Form 1:Q1:a", statement.ToString());

			// assert that blank and statement value reference same field
			Assert.AreSame(((FibItem)form.ItemList[0]).BlankList[0], ((FieldElement)statement.Expression.Elements[0]).Field);
		}

		[Test]
		public void ConstructValueFromXml()
		{
			string xmlString =
				"<set field=\"Variable\">\r\n" +
				"<string value=\"10\"/>\r\n" +
				"</set>";

			IXmlElement element = new XmlElement(xmlString);
			SetStatement statement = new SetStatement(element, process.Name);

			Assert.AreEqual("Set Variable to 10", statement.ToString());
			Assert.IsFalse(statement.TreatArithmeticAsText);
		}

		[Test]
		public void ConstructCompoundFromXml()
		{
			string xmlString =
				"<set field=\"Variable\">\r\n" +
				"<string field=\"Q1:a\"/>\r\n" +
				"<string value=\" \"/>\r\n" +
				"<string field=\"Q2:a\"/>\r\n" +
				"</set>";

			IXmlElement element = new XmlElement(xmlString);
			SetStatement statement = new SetStatement(element, process.Name);

			Assert.AreEqual("Set Variable to \"<<Form 1:Q1:a>> <<Form 1:Q2:a>>\"", statement.ToString());

			// assert that blank and statement value reference same field
			Assert.AreSame(((FibItem)form.ItemList[0]).BlankList[0], ((FieldElement)statement.Expression.Elements[0]).Field);
		}

		[Test]
		public void ConstructVariableFromValueXml()
		{
			string xmlString =
				"<set field=\"Score\">\r\n" +
				"<string value=\"100\"/>\r\n" +
				"</set>";

			IXmlElement element = new XmlElement(xmlString);
			SetStatement statement = new SetStatement(element, process.Name);

			Assert.AreEqual("Set Score to 100", statement.ToString());
		}

		[Test]
		public void ConstructVariableFromFieldXml()
		{
			string xmlString =
				"<set field=\"Score\">\r\n" +
				"<string field=\"Form 1:Q1:a\"/>\r\n" +
				"</set>";

			IXmlElement element = new XmlElement(xmlString);
			SetStatement statement = new SetStatement(element, process.Name);

			Assert.AreEqual("Set Score to Form 1:Q1:a", statement.ToString());
		}

		[Test]
		public void Name() 
		{ 
			SetStatement statement = new SetStatement();

			// Assertion
			Assert.AreEqual("Set", statement.Name);
		}

		[Test]
		public void GetText()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("a variable");

			// expression is a string
			statement.Expression = new Expression("a value");
			Assert.AreEqual("Set a variable to \"a value\"", statement.ToString());

			// expression is numeric
			statement.Expression = new Expression("2");
			Assert.AreEqual("Set a variable to 2", statement.ToString());

			statement.Expression = new Expression("-2");
			Assert.AreEqual("Set a variable to -2", statement.ToString());

			statement.Expression = new Expression("2.1");
			Assert.AreEqual("Set a variable to 2.1", statement.ToString());

			statement.Expression = new Expression("-2.1");
			Assert.AreEqual("Set a variable to -2.1", statement.ToString());

			statement.Expression = new Expression("2 + 1");
			Assert.AreEqual("Set a variable to 2 + 1", statement.ToString());

			// (A SET statement with fields can never be valid unless the fields
			// are present in the project's field list, and the statement is
			// contained in a process.)
			IForm form = Project.Current.AddForm();
			Process process = Project.Current.AddProcess();

			// create FIB item and assign alternate labels to blanks
			FibItem fibItem1 = new FibItem();
			fibItem1.Text = "__ __ __ __ __";
			fibItem1.BlankList[0].AlternateLabel = "number1";
			fibItem1.BlankList[1].AlternateLabel = "number2";
			fibItem1.BlankList[2].AlternateLabel = "number3";
			fibItem1.BlankList[3].AlternateLabel = "string1";
			fibItem1.BlankList[4].AlternateLabel = "string2";

			// add FIB item to form
			form.ItemList.Add(fibItem1);

			// make process line list from SET statement and add to process
			process.Lines.Add(new ProcessLineList(statement));

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			// arithmetic expression
			statement.Expression = new Expression("(<<Form 1:number1>> / (<<Form 1:number2>> + 7) - <<Form 1:number3>>) * 5");
			Assert.AreEqual("Set a variable to (<<Form 1:number1>> / (<<Form 1:number2>> + 7) - <<Form 1:number3>>) * 5", statement.ToString());

			// string concatenation
			statement.Expression = new Expression("<<Form 1:string1>> and <<Form 1:string2>>");
			Assert.AreEqual("Set a variable to \"<<Form 1:string1>> and <<Form 1:string2>>\"", statement.ToString());

			statement.Expression = new Expression("<<Form 1:string1>> <<Form 1:string2>>");
			Assert.AreEqual("Set a variable to \"<<Form 1:string1>> <<Form 1:string2>>\"", statement.ToString());

			statement.Expression = new Expression("2 <<Form 1:string1>>");
			Assert.AreEqual("Set a variable to \"2 <<Form 1:string1>>\"", statement.ToString());

			statement.Expression = new Expression("2<<Form 1:string1>>");
			Assert.AreEqual("Set a variable to \"2<<Form 1:string1>>\"", statement.ToString());

			// fields made up of only field(s) - should have no quotes
			statement.Expression = new Expression("<<Form 1:string1>>");
			Assert.AreEqual("Set a variable to Form 1:string1", statement.ToString());

			statement.Expression = new Expression("<<Form 1:string1>><<Form 1:string2>>");
			Assert.AreEqual("Set a variable to <<Form 1:string1>><<Form 1:string2>>", statement.ToString());

			// unbalanced parentheses
			statement.Expression = new Expression("(<<Form 1:number1>> / (<<Form 1:number2>> + 7) - <<Form 1:number3>> * 5");
			Assert.AreEqual("Set a variable to \"(<<Form 1:number1>> / (<<Form 1:number2>> + 7) - <<Form 1:number3>> * 5\"", statement.ToString());

			// invalid expression
			statement.Expression = new Expression("<<Form 1:number1>> ** 7");
			Assert.AreEqual("Set a variable to \"<<Form 1:number1>> ** 7\"", statement.ToString());
		}

		[Test]
		public void GetXml()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("a variable");
			statement.Expression = new Expression("a value");

			// (A SET statement with fields can never be valid unless the fields
			// are present in the project's field list, and the statement is
			// contained in a process.)
			Process process = Project.Current.AddProcess();

			// create FIB item and assign alternate labels to blanks
			FibItem fibItem1 = new FibItem();
			fibItem1.Text = "__ __ __ __ __ __ __ __";
			fibItem1.BlankList[0].AlternateLabel = "number";
			fibItem1.BlankList[1].AlternateLabel = "number1";
			fibItem1.BlankList[2].AlternateLabel = "number2";
			fibItem1.BlankList[3].AlternateLabel = "number3";
			fibItem1.BlankList[4].AlternateLabel = "FirstName";
			fibItem1.BlankList[5].AlternateLabel = "MI";
			fibItem1.BlankList[6].AlternateLabel = "LastName";
			fibItem1.BlankList[7].AlternateLabel = "&<Field's \"Bad\" Name>:b";

			// add FIB item to form
			form.ItemList.Add(fibItem1);

			// make process line list from SET statement and add to process
			process.Lines.Add(new ProcessLineList(statement));

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			string expString = "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
								"<string value=\"a value\"/>\r\n" +
								"</set>";
			Assert.AreEqual(expString, statement.ToXml());


			// check for math expressions
			// simple addition
			statement.Expression = new Expression("2 + 1");
			expString = "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
						"<add>\r\n" +
						"<operand value=\"2\"/>\r\n" +
						"<operand value=\"1\"/>\r\n" +
						"</add>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());

			// simple subtraction
			statement.Expression = new Expression("2 - 1");
			expString = "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
						"<sub>\r\n" +
						"<operand value=\"2\"/>\r\n" +
						"<operand value=\"1\"/>\r\n" +
						"</sub>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());

			// simple multiplication
			statement.Expression = new Expression("2* 1");
			expString = "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
						"<mul>\r\n" +
						"<operand value=\"2\"/>\r\n" +
						"<operand value=\"1\"/>\r\n" +
						"</mul>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());

			// simple division
			statement.Expression = new Expression("2/1");
			expString = "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
						"<div>\r\n" +
						"<operand value=\"2\"/>\r\n" +
						"<operand value=\"1\"/>\r\n" +
						"</div>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());

			// now check for fields
			statement.Expression = new Expression("<<Form 1:number>> / 1");
			expString = "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
						"<div>\r\n" +
						"<operand field=\"Form 1:number\"/>\r\n" +
						"<operand value=\"1\"/>\r\n" +
						"</div>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());

			// something a little more complex
			statement.Expression = new Expression("(<<Form 1:number1>> / (<<Form 1:number2>> + 7) - <<Form 1:number3>>) * 5");
			expString = "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
						"<mul>\r\n" +
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
						"</mul>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());

			// how about some poorly formed expressions:
			// unbalanced parentheses
			statement.Expression = new Expression("(<<Form 1:number1>> / (<<Form 1:number2>> + 7 - <<Form 1:number3>>) * 5");
			expString = "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
						"<string value=\"(&lt;&lt;Form 1:number1&gt;&gt; / (&lt;&lt;Form 1:number2&gt;&gt; + 7 - &lt;&lt;Form 1:number3&gt;&gt;) * 5\"/>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());

			// double "+" signs
			statement.Expression = new Expression("(<<Form 1:number1>> / (<<Form 1:number2>> ++ 7) - <<Form 1:number3>>) * 5");
			expString = "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
						"<string value=\"(&lt;&lt;Form 1:number1&gt;&gt; / (&lt;&lt;Form 1:number2&gt;&gt; ++ 7) - &lt;&lt;Form 1:number3&gt;&gt;) * 5\"/>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());

			// unmatched angle brackets
			statement.Expression = new Expression("(<<Form 1:number1>> / (<<Form 1:number2>>> ++ 7) - <<Form 1:number3>>) * 5");
			expString = "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
						"<string value=\"(&lt;&lt;Form 1:number1&gt;&gt; / (&lt;&lt;Form 1:number2&gt;&gt;&gt; ++ 7) - &lt;&lt;Form 1:number3&gt;&gt;) * 5\"/>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());

			// how about parens with no operators
			statement.Expression = new Expression("(Hello World!)");
			expString = "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
						"<string value=\"(Hello World!)\"/>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());

			// concatenation of fields and literal text
			statement.Variable = new Variable("Full Name");
			statement.Expression = new Expression("<<Form 1:FirstName>> <<Form 1:MI>>, <<Form 1:LastName>>");
			expString = "<set field=\"Full Name\" arithmeticAsText=\"false\">\r\n" +
						"<string field=\"Form 1:FirstName\"/>\r\n" +
						"<string value=\" \"/>\r\n" +
						"<string field=\"Form 1:MI\"/>\r\n" +
						"<string value=\", \"/>\r\n" +
						"<string field=\"Form 1:LastName\"/>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());

			// throw in some special characters
			statement.Variable = new Variable("&<Field's \"Bad\" Name>:a");
			statement.Expression = new Expression("Joe's <<Form 1:FirstName>> is \"<\" mine & \">\" his		<<&<Field's \"Bad\" Name>:b>>!");
			//                                                                               (tabs) ^
			expString = "<set field=\"&amp;&lt;Field&apos;s &quot;Bad&quot; Name&gt;:a\" arithmeticAsText=\"false\">\r\n" +
						"<string value=\"Joe&apos;s \"/>\r\n" +
						"<string field=\"Form 1:FirstName\"/>\r\n" +
						"<string value=\" is &quot;\"/>\r\n" +
						"<string value=\"&lt;\"/>\r\n" +
						"<string value=\"&quot; mine &amp; &quot;\"/>\r\n" +
						"<string value=\"&gt;\"/>\r\n" +
						"<string value=\"&quot; his		\"/>\r\n" +
						"<string field=\"Form 1:&amp;&lt;Field&apos;s &quot;Bad&quot; Name&gt;:b\"/>\r\n" +
						"<string value=\"!\"/>\r\n" +
						"</set>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void GetTextQualifiedName()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("record:variable");
			statement.Expression = new Expression("a value");
			Assert.AreEqual("Set record:variable to \"a value\"", statement.ToString());
		}

		[Test]
		public void GetXmlQualifiedName()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("record:variable");
			statement.Expression = new Expression("a value");

			string expString =
				"<set field=\"record:variable\" arithmeticAsText=\"false\">\r\n" +
				"<string value=\"a value\"/>\r\n" +
				"</set>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void ConstructArithmeticExpressionFromXml()
		{
			string xmlString =
				"<set field=\"variable\" arithmeticAsText=\"false\">\r\n" +
				"<sub>\r\n" +
				"<operand value=\"10\"/>\r\n" +
				"<operand value=\"5\"/>\r\n" +
				"</sub>\r\n" +
				"</set>";

			IXmlElement element = new XmlElement(xmlString);
			SetStatement statement = new SetStatement(element, process.Name);

			Assert.AreEqual("variable", statement.Variable.FieldName);
			Assert.AreEqual("10 - 5", statement.Expression.ToString());
			Assert.IsFalse(statement.TreatArithmeticAsText);
		}

		[Test]
		public void GetTextArithmeticExpression()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("variable");
			statement.Expression = new Expression("10 - 5");

			Assert.AreEqual("Set variable to 10 - 5", statement.ToString());
		}
		
		[Test]
		public void GetXmlArithmeticExpression()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("variable");
			statement.Expression = new Expression("10 - 5");

			string expString =
				"<set field=\"variable\" arithmeticAsText=\"false\">\r\n" +
				"<sub>\r\n" +
				"<operand value=\"10\"/>\r\n" +
				"<operand value=\"5\"/>\r\n" +
				"</sub>\r\n" +
				"</set>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void ConstructArithmeticExpressionAsTextFromXml()
		{
			string xmlString =
				"<set field=\"variable\" arithmeticAsText=\"true\">\r\n" +
				"<string value=\"10 - 5\"/>\r\n" +
				"</set>";

			IXmlElement element = new XmlElement(xmlString);
			SetStatement statement = new SetStatement(element, process.Name);

			Assert.AreEqual("variable", statement.Variable.FieldName);
			Assert.AreEqual("10 - 5", statement.Expression.ToString());
			Assert.IsTrue(statement.TreatArithmeticAsText);
		}

		[Test]
		public void GetTextArithmeticExpressionAsText()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("variable");
			statement.Expression = new Expression("10 - 5");
			statement.TreatArithmeticAsText = true;

			Assert.AreEqual("Set variable to \"10 - 5\"", statement.ToString());
		}

		[Test]
		public void GetXmlArithmeticExpressionAsText()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("variable");
			statement.Expression = new Expression("10 - 5");
			statement.TreatArithmeticAsText = true;

			string expString =
				"<set field=\"variable\" arithmeticAsText=\"true\">\r\n" +
				"<string value=\"10 - 5\"/>\r\n" +
				"</set>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void ConstructFieldInArithmeticExpression()
		{
			string xmlString =
				"<set field=\"variable\" arithmeticAsText=\"false\">\r\n" +
				"<sub>\r\n" +
				"<operand field=\"Form 1:Q1:a\"/>\r\n" +
				"<operand value=\"5\"/>\r\n" +
				"</sub>\r\n" +
				"</set>";

			IXmlElement element = new XmlElement(xmlString);
			SetStatement statement = new SetStatement(element, process.Name);

			Assert.AreEqual("variable", statement.Variable.FieldName);
			Assert.AreEqual("<<Form 1:Q1:a>> - 5", statement.Expression.ToString());
			Assert.IsFalse(statement.TreatArithmeticAsText);
		}

		[Test]
		public void GetTextFieldInArithmeticExpression()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("variable");
			statement.Expression = new Expression("<<Form 1:Q1:a>> - 5");

			Assert.AreEqual("Set variable to <<Form 1:Q1:a>> - 5", statement.ToString());
		}

		[Test]
		public void GetXmlFieldInArithmeticExpression()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("variable");
			statement.Expression = new Expression("<<Form 1:Q1:a>> - 5");

			string expString =
				"<set field=\"variable\" arithmeticAsText=\"false\">\r\n" +
				"<sub>\r\n" +
				"<operand field=\"Form 1:Q1:a\"/>\r\n" +
				"<operand value=\"5\"/>\r\n" +
				"</sub>\r\n" +
				"</set>";
			Assert.AreEqual(expString, statement.ToXml());
		}
		
		[Test]
		public void ConstructFieldInArithmeticExpressionAsText()
		{
			string xmlString =
				"<set field=\"variable\" arithmeticAsText=\"true\">\r\n" +
				"<string value=\"&lt;&lt;Form 1:Q1:a&gt;&gt; - 5\"/>\r\n" +
				"</set>";

			IXmlElement element = new XmlElement(xmlString);
			SetStatement statement = new SetStatement(element, process.Name);

			Assert.AreEqual("variable", statement.Variable.FieldName);
			Assert.AreEqual("<<Form 1:Q1:a>> - 5", statement.Expression.ToString());
			Assert.IsTrue(statement.TreatArithmeticAsText);
		}

		[Test]
		public void GetTextFieldInArithmeticExpressionAsText()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("variable");
			statement.Expression = new Expression("<<Form 1:Q1:a>> - 5");
			statement.TreatArithmeticAsText = true;

			Assert.AreEqual("Set variable to \"<<Form 1:Q1:a>> - 5\"", statement.ToString());
		}

		[Test]
		public void GetXmlFieldInArithmeticExpressionAsText()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("variable");
			statement.Expression = new Expression("<<Form 1:Q1:a>> - 5");
			statement.TreatArithmeticAsText = true;

			string expString =
				"<set field=\"variable\" arithmeticAsText=\"true\">\r\n" +
				"<string field=\"Form 1:Q1:a\"/>\r\n" +
				"<string value=\" - 5\"/>\r\n" +
				"</set>";
			Assert.AreEqual(expString, statement.ToXml());
		}
#endif
	}
}
