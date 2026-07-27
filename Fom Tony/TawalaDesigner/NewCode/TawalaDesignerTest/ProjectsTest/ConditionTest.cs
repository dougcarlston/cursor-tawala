using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the Condition class
	/// </summary>
    [Ignore("REQUIRES Fix-ups for new classes")]
    [TestFixture]
	public class ConditionTest
	{
#if FIXED
		private IForm form;
		private Process process;
		private FibItem fibItem1;
		private FibItem fibItem2;
		private Blank blank1;
		private McqItem mcItem1;
		private McqItem mcItem2;

		private Field field;
		private Field q1;

		private Condition condition1;
		private Condition condition2;
		private Condition condition3;
		private Condition condition4;
		private Condition condition5;
		private Condition condition6;
		private Condition condition7;
		private Condition condition8;
		private Condition condition9;
		private Condition condition10;
		private Condition condition11;
		private Condition condition12;
		private Condition condition13;
		private Condition condition14;
		private Condition condition15;
		private Condition condition16;
		private Condition condition17;
		private Condition condition18;
		private Condition condition19;
		private Condition condition20;
		private Condition condition21;
		private Condition condition22;
		private Condition condition23;
		private Condition condition24;
		private Condition condition25;
		private Condition condition26;
		private Condition condition27;
		private Condition condition28;
		private Condition condition29;

		private ComparisonOperator equals;
		private ComparisonOperator doesNotEqual;
		private ComparisonOperator contains;
		private ComparisonOperator doesNotContain;
		private ComparisonOperator beginsWith;
		private ComparisonOperator endsWith;
		private ComparisonOperator isLessThan;
		private ComparisonOperator isLessThanOrEqualTo;
		private ComparisonOperator isGreaterThan;
		private ComparisonOperator isGreaterThanOrEqualTo;
		private ComparisonOperator isBlank;
		private ComparisonOperator isNotBlank;
		private ComparisonOperator choiceContains;
		private ComparisonOperator choiceDoesNotEqual;
		private ComparisonOperator choiceEquals;

		/// <summary>
		/// This method executes before any test method
		/// </summary>
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

			// create FIB and MC items ('Q1:a', 'Q2:a', 'Q3', 'Q4')
			fibItem1 = new FibItem();
			blank1 = fibItem1.BlankList[0];
			fibItem2 = new FibItem();
			mcItem1 = new McqItem();
			mcItem2 = new McqItem();

			// add items to form
			form.ItemList.Add(fibItem1);
			form.ItemList.Add(fibItem2);
			form.ItemList.Add(mcItem1);
			form.ItemList.Add(mcItem2);
		}

		/// <summary>
		/// This method executes before each test method
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			// create fields
			field = new Field("Q1:a");
			q1 = new Field("Q1");

			// construct comparison operators
			equals = HybridOperator.List[HybridOperator.Ops.equals];
			doesNotEqual = HybridOperator.List[HybridOperator.Ops.doesNotEqual];
			contains = HybridOperator.List[HybridOperator.Ops.contains];
			doesNotContain = HybridOperator.List[HybridOperator.Ops.doesNotContain];
			beginsWith = HybridOperator.List[HybridOperator.Ops.beginsWith];
			endsWith = HybridOperator.List[HybridOperator.Ops.endsWith];
			isLessThan = HybridOperator.List[HybridOperator.Ops.isLessThan];
			isLessThanOrEqualTo = HybridOperator.List[HybridOperator.Ops.isLessThanOrEqualTo];
			isGreaterThan = HybridOperator.List[HybridOperator.Ops.isGreaterThan];
			isGreaterThanOrEqualTo = HybridOperator.List[HybridOperator.Ops.isGreaterThanOrEqualTo];
			isBlank = HybridOperator.List[HybridOperator.Ops.isBlank];
			isNotBlank = HybridOperator.List[HybridOperator.Ops.isNotBlank];
			choiceContains = MCManyOperator.List[MCManyOperator.Ops.mcContains];
			choiceDoesNotEqual = MCOneOperator.List[MCOneOperator.Ops.mcDoesNotEqual];
			choiceEquals = MCOneOperator.List[MCOneOperator.Ops.mcEquals];

			// create conditions with string expressions
			condition1 = new Condition(blank1, equals, new Expression("Homer"));
			condition2 = new Condition(blank1, doesNotEqual, new Expression("Homer"));
			condition3 = new Condition(blank1, contains, new Expression("ome"));
			condition4 = new Condition(blank1, doesNotContain, new Expression("ome"));
			condition5 = new Condition(blank1, beginsWith, new Expression("Ho"));
			condition6 = new Condition(blank1, endsWith, new Expression("er"));
			condition7 = new Condition(blank1, isLessThan, new Expression("Ralph"));
			condition8 = new Condition(blank1, isLessThanOrEqualTo, new Expression("Ralph"));
			condition9 = new Condition(blank1, isGreaterThan, new Expression("Ralph"));
			condition10 = new Condition(blank1, isGreaterThanOrEqualTo, new Expression("Ralph"));
			condition11 = new Condition(blank1, isBlank);
			condition12 = new Condition(blank1, isNotBlank);

			// create conditions with numeric expressions
			condition13 = new Condition(blank1, equals, new Expression("21"));
			condition14 = new Condition(blank1, doesNotEqual, new Expression("21"));
			condition15 = new Condition(blank1, contains, new Expression("21"));
			condition16 = new Condition(blank1, doesNotContain, new Expression("21"));
			condition17 = new Condition(blank1, beginsWith, new Expression("21"));
			condition18 = new Condition(blank1, endsWith, new Expression("21"));
			condition19 = new Condition(blank1, isLessThan, new Expression("21"));
			condition20 = new Condition(blank1, isLessThanOrEqualTo, new Expression("21"));
			condition21 = new Condition(blank1, isGreaterThan, new Expression("21"));
			condition22 = new Condition(blank1, isGreaterThanOrEqualTo, new Expression("21"));

			// conditions with Fields
			condition23 = new Condition(blank1, equals, new Expression("<<Q2:a>>"));
			condition24 = new Condition(blank1, contains, new Expression("<<Q2:a>>"));
			condition25 = new Condition(blank1, isLessThan, new Expression("<<Q2:a>>"));

			// conditions with MCQs
			condition26 = new Condition(mcItem1, choiceContains, new Expression("a"));
			condition27 = new Condition(mcItem1, choiceDoesNotEqual, new Expression("a"));

			// compound conditions
			condition28 = new Condition(blank1, equals, new Expression("18 + 3"));
			condition29 = new Condition(blank1, equals, new Expression("<<Q2:a>> - 5"));
		}

		[Test]
		public void Construct() 
		{
			// create fields
			Field age = new Field("Age");
			Field name = new Field("Name");
			Field q1 = new Field("Q1");

			// construct comparison operators
			ComparisonOperator equals = ArithmeticOperator.List[ArithmeticOperator.Ops.equals];
			ComparisonOperator stringDoesNotEqual = StringOperator.List[StringOperator.Ops.doesNotEqual];
			ComparisonOperator contains = MCManyOperator.List[MCManyOperator.Ops.mcContains];
			ComparisonOperator choiceDoesNotEqual = MCOneOperator.List[MCOneOperator.Ops.mcDoesNotEqual];

			// create conditions
			Condition condition1 = new Condition(age, equals, new Expression("21"));
			Condition condition2 = new Condition(name, stringDoesNotEqual, new Expression("Homer"));
			Condition condition3 = new Condition(q1, contains, new Expression("a"));
			Condition condition4 = new Condition(q1, choiceDoesNotEqual, new Expression("a"));

			//Assertions 
			Assert.AreEqual(age, condition1.Field);
			Assert.AreEqual(name, condition2.Field);
			Assert.AreEqual(q1, condition3.Field);
			Assert.AreEqual(q1, condition4.Field);

			Assert.AreEqual("Age equals 21", condition1.ToString());
			Assert.AreEqual("Name does not equal \"Homer\"", condition2.ToString());
			Assert.AreEqual("Q1 contains a", condition3.ToString());
			Assert.AreEqual("Q1 does not equal a", condition4.ToString());
		}

		[Test]
		public void ConstructCompound()
		{
			// create fields
			Field age = new Field("Age");

			// construct comparison operators
			ComparisonOperator equals = ArithmeticOperator.List[ArithmeticOperator.Ops.equals];

			// create conditions
			Condition condition1 = new Condition(age, equals, new Expression("18 + 3"));

			//Assertions 
			Assert.AreEqual(age, condition1.Field);
			Assert.AreEqual("Age equals 18 + 3", condition1.ToString());
		}

		[Test]
		public void ToStringForLiterals()
		{
			//Assertions (non-numeric strings always have quotation marks)
			Assert.AreEqual("Form 1:Q1:a equals \"Homer\"", condition1.ToString());
			Assert.AreEqual("Form 1:Q1:a does not equal \"Homer\"", condition2.ToString());
			Assert.AreEqual("Form 1:Q1:a contains \"ome\"", condition3.ToString());
			Assert.AreEqual("Form 1:Q1:a does not contain \"ome\"", condition4.ToString());
			Assert.AreEqual("Form 1:Q1:a begins with \"Ho\"", condition5.ToString());
			Assert.AreEqual("Form 1:Q1:a ends with \"er\"", condition6.ToString());
			Assert.AreEqual("Form 1:Q1:a is less than \"Ralph\"", condition7.ToString());
			Assert.AreEqual("Form 1:Q1:a is less than or equal to \"Ralph\"", condition8.ToString());
			Assert.AreEqual("Form 1:Q1:a is greater than \"Ralph\"", condition9.ToString());
			Assert.AreEqual("Form 1:Q1:a is greater than or equal to \"Ralph\"", condition10.ToString());
			Assert.AreEqual("Form 1:Q1:a is blank", condition11.ToString());
			Assert.AreEqual("Form 1:Q1:a is not blank", condition12.ToString());

			//Assertions (quotation marks are used for non-arithmetic comparisons)
			Assert.AreEqual("Form 1:Q1:a equals 21", condition13.ToString());
			Assert.AreEqual("Form 1:Q1:a does not equal 21", condition14.ToString());
			Assert.AreEqual("Form 1:Q1:a contains \"21\"", condition15.ToString());
			Assert.AreEqual("Form 1:Q1:a does not contain \"21\"", condition16.ToString());
			Assert.AreEqual("Form 1:Q1:a begins with \"21\"", condition17.ToString());
			Assert.AreEqual("Form 1:Q1:a ends with \"21\"", condition18.ToString());
			Assert.AreEqual("Form 1:Q1:a is less than 21", condition19.ToString());
			Assert.AreEqual("Form 1:Q1:a is less than or equal to 21", condition20.ToString());
			Assert.AreEqual("Form 1:Q1:a is greater than 21", condition21.ToString());
			Assert.AreEqual("Form 1:Q1:a is greater than or equal to 21", condition22.ToString());

		}

		[Test]
		public void ToStringForFields()
		{
			//Assertions (Fields never have quotation marks)
			Assert.AreEqual("Form 1:Q1:a equals Form 1:Q2:a", condition23.ToString());
			Assert.AreEqual("Form 1:Q1:a contains Form 1:Q2:a", condition24.ToString());
			Assert.AreEqual("Form 1:Q1:a is less than Form 1:Q2:a", condition25.ToString());

			//Assertions (Fields never have quotation marks)
			Assert.AreEqual("Form 1:Q3 contains a", condition26.ToString());
			Assert.AreEqual("Form 1:Q3 does not equal a", condition27.ToString());
		}

		[Test]
		public void ToStringForCompounds()
		{
			Assert.AreEqual("Form 1:Q1:a equals 18 + 3", condition28.ToString());
			Assert.AreEqual("Form 1:Q1:a equals Form 1:Q2:a - 5", condition29.ToString());
		}

		[Test]
		public void GetXml() 
		{ 
			// create fields
			Field age = new Field("Age");
			Field name = new Field("Name");
			Field q1 = new Field("Q1");

			// construct comparison operators
			ComparisonOperator equals = ArithmeticOperator.List[ArithmeticOperator.Ops.equals];
			ComparisonOperator stringDoesNotEqual = StringOperator.List[StringOperator.Ops.doesNotEqual];
			ComparisonOperator choiceContains = MCManyOperator.List[MCManyOperator.Ops.mcContains];
			ComparisonOperator choiceDoesNotEqual = MCOneOperator.List[MCOneOperator.Ops.mcDoesNotEqual];

			// create and test conditions
			Condition condition = new Condition(age, equals, new Expression("21"));
			string expString = "<equals field=\"Age\">\r\n" +
								"<string value=\"21\"/>\r\n" +
								"</equals>\r\n";
			Assert.AreEqual(expString, condition.ToXml());

			condition = new Condition(name, stringDoesNotEqual, new Expression("Homer"));
			expString = "<doesNotEqual field=\"Name\">\r\n" +
						"<string value=\"Homer\"/>\r\n" +
						"</doesNotEqual>\r\n";
			Assert.AreEqual(expString, condition.ToXml());

			condition = new Condition(q1, choiceContains, new Expression(new ChoiceField("a")));
			expString = "<mcContains field=\"Q1\" value=\"a\"/>\r\n";
			Assert.AreEqual(expString, condition.ToXml());

			condition = new Condition(q1, choiceDoesNotEqual, new Expression(new ChoiceField("b")));
			expString = "<mcDoesNotEqual field=\"Q1\" value=\"b\"/>\r\n";
			Assert.AreEqual(expString, condition.ToXml());

			// check for invalid characters
			Field badXMLAge = new Field("&<Field's \"Bad\" Name>:a");
			condition = new Condition(badXMLAge, equals, new Expression("&<Value's \"Bad\" Name>"));
			expString = "<equals field=\"&amp;&lt;Field&apos;s &quot;Bad&quot; Name&gt;:a\">\r\n" +
						"<string value=\"&amp;&lt;Value&apos;s &quot;Bad&quot; Name&gt;\"/>\r\n" +
						"</equals>\r\n";
			Assert.AreEqual(expString, condition.ToXml());

			// verify output from compound condition
			condition = new Condition(age, equals, new Expression("18 + 3"));
			expString = "<equals field=\"Age\">\r\n" +
						"<add>\r\n" +
						"<operand value=\"18\"/>\r\n" +
						"<operand value=\"3\"/>\r\n" +
						"</add>\r\n" +
						"</equals>\r\n";
			Assert.AreEqual(expString, condition.ToXml());

			condition = new Condition(age, equals, new Expression("<<Q2:a>> - 5"));
			expString = "<equals field=\"Age\">\r\n" +
						"<sub>\r\n" +
						"<operand field=\"Form 1:Q2:a\"/>\r\n" +
						"<operand value=\"5\"/>\r\n" +
						"</sub>\r\n" +
						"</equals>\r\n";
			Assert.AreEqual(expString, condition.ToXml());

		}
		
		[Test]
		public void GetXmlIsBlank()
		{
			string expString = "<isBlank field=\"Form 1:Q1:a\" />\r\n";
			Assert.AreEqual(expString, condition11.ToXml());
		}

		[Test]
		public void GetXmlIsNotBlank()
		{
			string expString = "<isNotBlank field=\"Form 1:Q1:a\" />\r\n";
			Assert.AreEqual(expString, condition12.ToXml());
		}

		[Test]
		public void GetXmlMCEqualsQualified()
		{
			Condition condition = new Condition(mcItem1, choiceEquals, new Expression(new RecordField(new Record("Record"), mcItem1)));

			string expString =
				"<mcEquals field=\"Form 1:Q3\">\r\n" +
				"<string field=\"Record:Form 1:Q3\"/>\r\n" +
				"</mcEquals>\r\n";
			
			Assert.AreEqual(expString, condition.ToXml());
		}

		[Test]
		public void GetXmlQualifiedEqualsMC()
		{
			Expression expression = new Expression("<<Form 1:Q3>>", process.GetValidMCFields(0));
			Condition condition = new Condition(new RecordField(new Record("Record"), mcItem1), choiceEquals, expression);

			string expString =
				"<mcEquals field=\"Record:Form 1:Q3\">\r\n" +
				"<string field=\"Form 1:Q3\"/>\r\n" +
				"</mcEquals>\r\n";

			Assert.AreEqual(expString, condition.ToXml());
		}

		[Test]
		public void GetXmlMCEqualsMC()
		{
			Expression expression = new Expression("<<Form 1:Q4>>", process.GetValidMCFields(0));
			Condition condition = new Condition(mcItem1, choiceEquals, expression);

			string expString =
				"<mcEquals field=\"Form 1:Q3\">\r\n" +
				"<string field=\"Form 1:Q4\"/>\r\n" +
				"</mcEquals>\r\n";

			Assert.AreEqual(expString, condition.ToXml());
		}

		[Test]
		public void GetXmlBlankEqualsBlank()
		{
			Condition condition = new Condition(fibItem1.BlankList[0], equals, new Expression(fibItem2.BlankList[0]));

			string expString =
				"<equals field=\"Form 1:Q1:a\">\r\n" +
				"<string field=\"Form 1:Q2:a\"/>\r\n" +
				"</equals>\r\n";

			Assert.AreEqual(expString, condition.ToXml());
		}
#endif
	}
}
