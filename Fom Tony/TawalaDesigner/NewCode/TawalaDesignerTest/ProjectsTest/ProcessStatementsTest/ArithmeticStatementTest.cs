using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the AddStatement class
	/// </summary>
    [Ignore("REQUIRES Fix-ups for new classes")]
	[TestFixture]
	public class ArithmeticStatementTest
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

			// create FIB item in which one blank has an alternate label
			fibItem1 = new FibItem();
			fibItem1.Text = "__ __";
			fibItem1.BlankList[1].AlternateLabel = "&<Value's \"Bad\" Name>:b";
			form.ItemList.Add(fibItem1);

			// create FIB item
			fibItem2 = new FibItem();
			form.ItemList.Add(fibItem2);

			// create MC item
			mcItem1 = new McqItem();
			form.ItemList.Add(mcItem1);
		}

		[Test]
		public void Instantiate()
		{
			ArithmeticStatement statement = new ArithmeticStatement();

			// Assertions
			Assert.IsNotNull(statement);
		}

		[Test]
		public void ConstructAddFromXml()
		{
			process.MappedFields.Fields.Add(((FibItem)form.ItemList[0]).BlankList[0]);
			process.MappedFields.Map();

			Assert.IsTrue(process.MappedFields.ContainsKey("Q1:a"));

			string xmlString =
				"<addTo field=\"Score\">" +
				"<operand field=\"Q1:a\"/>" +
				"</addTo>";

			IXmlElement element = new XmlElement(xmlString);
			AddStatement statement = new AddStatement(element, process.Name);

			Assert.AreEqual("Add", statement.Name);
			Assert.AreEqual("Score", statement.Variable);
			Assert.AreEqual("Add Form 1:Q1:a to Score", statement.ToString());

			// assert that blank and statement value reference same field
			Assert.AreSame(((FibItem)form.ItemList[0]).BlankList[0], ((FieldElement)statement.Value.Expression.Elements[0]).Field);
		}

		[Test]
		public void GetAddName()
		{
			AddStatement statement = new AddStatement();

			// Assertion
			Assert.AreEqual("Add", statement.Name);
		}

		[Test]
		public void GetAddText()
		{
			// make process line list from ADD statement and add to process
			AddStatement statement = new AddStatement();
			process.Lines.Add(new ProcessLineList(statement));

			statement.Variable = "a variable";
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;

			Assert.AreEqual("Add 2 to a variable", statement.ToString());

			statement.Value.Text = "2.1";
			Assert.AreEqual("Add 2.1 to a variable", statement.ToString());

			statement.Value.Text = "-2.1";
			Assert.AreEqual("Add -2.1 to a variable", statement.ToString());

			statement.Value.Text = "- 2.1";
			Assert.AreEqual("Add \"- 2.1\" to a variable", statement.ToString());

			statement.Value.Text = "foo";
			Assert.AreEqual("Add \"foo\" to a variable", statement.ToString());

			statement.Value.Text = "Q1:a";
			statement.Value.Type = FieldOrLiteral.StringType.field;

			Assert.AreEqual(1, statement.Value.Expression.Elements.Count);
			Assert.AreEqual("Form 1:Q1:a", statement.Value.Text);

			Assert.AreEqual("Add Form 1:Q1:a to a variable", statement.ToString());
		}

		[Test]
		public void GetAddTextQualifiedName()
		{
			AddStatement statement = new AddStatement();

			statement.Variable = "record:variable";
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;

			Assert.AreEqual("Add 2 to record:variable", statement.ToString());
		}

		[Test]
		public void GetAddXml()
		{
			AddStatement statement = new AddStatement();
			statement.Variable = "a variable";
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;

			string expString =	"<addTo field=\"a variable\">\r\n" +
								"<operand value=\"2\"/>\r\n" +
								"</addTo>";
			Assert.AreEqual(expString, statement.ToXml());

			statement.Value.Text = "Q1:a";
			statement.Value.Type = FieldOrLiteral.StringType.field;

			expString = "<addTo field=\"a variable\">\r\n" +
						"<operand field=\"Form 1:Q1:a\"/>\r\n" +
						"</addTo>";
			Assert.AreEqual(expString, statement.ToXml());

			// check for invalid XML characters
			statement.Variable = "&<Field's \"Bad\" Name>:a";
			statement.Value.Text = "&<Value's \"Bad\" Name>:b";

			expString = "<addTo field=\"&amp;&lt;Field&apos;s &quot;Bad&quot; Name&gt;:a\">\r\n" +
						"<operand field=\"Form 1:&amp;&lt;Value&apos;s &quot;Bad&quot; Name&gt;:b\"/>\r\n" +
						"</addTo>";
			Assert.AreEqual(expString, statement.ToXml());

			statement.Value.Type = FieldOrLiteral.StringType.literal;

			expString = "<addTo field=\"&amp;&lt;Field&apos;s &quot;Bad&quot; Name&gt;:a\">\r\n" +
						"<operand value=\"&amp;&lt;Value&apos;s &quot;Bad&quot; Name&gt;:b\"/>\r\n" +
						"</addTo>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void GetAddXmlQualifiedName()
		{
			AddStatement statement = new AddStatement();
			statement.Variable = "record:variable";
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;

			string expString = "<addTo field=\"record:variable\">\r\n" +
								"<operand value=\"2\"/>\r\n" +
								"</addTo>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void GetSubtractName()
		{
			SubtractStatement statement = new SubtractStatement();

			// Assertion
			Assert.AreEqual("Subtract", statement.Name);
		}

		[Test]
		public void GetSubtractText()
		{
			// make process line list from DIVIDE statement and add to process
			SubtractStatement statement = new SubtractStatement();
			process.Lines.Add(new ProcessLineList(statement));

			statement.Variable = "a variable";
			statement.Value.Text = "-2.1";
			statement.Value.Type = FieldOrLiteral.StringType.literal;
			Assert.AreEqual("Subtract -2.1 from a variable", statement.ToString());

			statement.Value.Text = "foo";
			Assert.AreEqual("Subtract \"foo\" from a variable", statement.ToString());

			statement.Value.Text = "Q1:a";
			statement.Value.Type = FieldOrLiteral.StringType.field;
			Assert.AreEqual("Subtract Form 1:Q1:a from a variable", statement.ToString());
		}

		[Test]
		public void GetSubtractXml()
		{
			SubtractStatement statement = new SubtractStatement();
			statement.Variable = "a variable";
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;

			string expString = "<subtractFrom field=\"a variable\">\r\n" +
								"<operand value=\"2\"/>\r\n" +
								"</subtractFrom>";
			Assert.AreEqual(expString, statement.ToXml());

			statement.Value.Text = "Q1:a";
			statement.Value.Type = FieldOrLiteral.StringType.field;

			expString = "<subtractFrom field=\"a variable\">\r\n" +
						"<operand field=\"Form 1:Q1:a\"/>\r\n" +
						"</subtractFrom>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void GetMultiplyName()
		{
			MultiplyStatement statement = new MultiplyStatement();

			// Assertion
			Assert.AreEqual("Multiply", statement.Name);
		}

		[Test]
		public void GetMultiplyText()
		{
			// make process line list from MULTIPLY statement and add to process
			MultiplyStatement statement = new MultiplyStatement();
			process.Lines.Add(new ProcessLineList(statement));

			statement.Variable = "a variable";
			statement.Value.Text = "-2.1";
			statement.Value.Type = FieldOrLiteral.StringType.literal;
			Assert.AreEqual("Multiply a variable by -2.1", statement.ToString());

			statement.Value.Text = "foo";
			Assert.AreEqual("Multiply a variable by \"foo\"", statement.ToString());

			statement.Value.Text = "Q1:a";
			statement.Value.Type = FieldOrLiteral.StringType.field;
			Assert.AreEqual("Multiply a variable by Form 1:Q1:a", statement.ToString());
		}

		[Test]
		public void GetMultiplyXml()
		{
			MultiplyStatement statement = new MultiplyStatement();
			statement.Variable = "a variable";
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;

			string expString =	"<multiplyBy field=\"a variable\">\r\n" +
								"<operand value=\"2\"/>\r\n" +
								"</multiplyBy>";
			Assert.AreEqual(expString, statement.ToXml());

			statement.Value.Text = "Q1:a";
			statement.Value.Type = FieldOrLiteral.StringType.field;

			expString = "<multiplyBy field=\"a variable\">\r\n" +
						"<operand field=\"Form 1:Q1:a\"/>\r\n" +
						"</multiplyBy>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void GetDivideName()
		{
			DivideStatement statement = new DivideStatement();

			// Assertion
			Assert.AreEqual("Divide", statement.Name);
		}

		[Test]
		public void GetDivideText()
		{
			// make process line list from DIVIDE statement and add to process
			DivideStatement statement = new DivideStatement();
			process.Lines.Add(new ProcessLineList(statement));

			statement.Variable = "a variable";
			statement.Value.Text = "-2.1";
			statement.Value.Type = FieldOrLiteral.StringType.literal;
			Assert.AreEqual("Divide a variable by -2.1", statement.ToString());

			statement.Value.Text = "foo";
			Assert.AreEqual("Divide a variable by \"foo\"", statement.ToString());

			statement.Value.Text = "Q1:a";
			statement.Value.Type = FieldOrLiteral.StringType.field;
			Assert.AreEqual("Divide a variable by Form 1:Q1:a", statement.ToString());
		}

		[Test]
		public void GetDivideXml()
		{
			DivideStatement statement = new DivideStatement();
			statement.Variable = "a variable";
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;

			string expString =	"<divideBy field=\"a variable\">\r\n" +
								"<operand value=\"2\"/>\r\n" +
								"</divideBy>";
			Assert.AreEqual(expString, statement.ToXml());

			statement.Value.Text = "Q1:a";
			statement.Value.Type = FieldOrLiteral.StringType.field;

			expString = "<divideBy field=\"a variable\">\r\n" +
						"<operand field=\"Form 1:Q1:a\"/>\r\n" +
						"</divideBy>";
			Assert.AreEqual(expString, statement.ToXml());
		}
#endif
	}
}
