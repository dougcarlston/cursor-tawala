using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the Condition class
	/// </summary>
	[TestFixture]
	public class ComparisonOperatorTest
	{
		[Test]
		public void ConstructArithmetic() 
		{ 
			Field age = new Field("Age");

			ArithmeticOperator equals = ArithmeticOperator.List[ArithmeticOperator.Ops.equals];
			ArithmeticOperator doesNotEqual = ArithmeticOperator.List[ArithmeticOperator.Ops.doesNotEqual];
			ArithmeticOperator isLessThan = ArithmeticOperator.List[ArithmeticOperator.Ops.isLessThan];
			ArithmeticOperator isLessThanOrEqualTo = ArithmeticOperator.List[ArithmeticOperator.Ops.isLessThanOrEqualTo];
			ArithmeticOperator isGreaterThan = ArithmeticOperator.List[ArithmeticOperator.Ops.isGreaterThan];
			ArithmeticOperator isGreaterThanOrEqualTo = ArithmeticOperator.List[ArithmeticOperator.Ops.isGreaterThanOrEqualTo];

			Condition condition1 = new Condition(age, equals, new Expression("21"));
			Condition condition2 = new Condition(age, doesNotEqual, new Expression("21"));
			Condition condition3 = new Condition(age, isLessThan, new Expression("21"));
			Condition condition4 = new Condition(age, isLessThanOrEqualTo, new Expression("21"));
			Condition condition5 = new Condition(age, isGreaterThan, new Expression("21"));
			Condition condition6 = new Condition(age, isGreaterThanOrEqualTo, new Expression("21"));

			//Assertions 
			Assert.AreEqual(6, ArithmeticOperator.List.Count);
			Assert.AreEqual("Age equals 21", condition1.ToString());
			Assert.AreEqual("Age does not equal 21", condition2.ToString());
			Assert.AreEqual("Age is less than 21", condition3.ToString());
			Assert.AreEqual("Age is less than or equal to 21", condition4.ToString());
			Assert.AreEqual("Age is greater than 21", condition5.ToString());
			Assert.AreEqual("Age is greater than or equal to 21", condition6.ToString());
		} 

		[Test]
		public void ConstructString() 
		{ 
			Field name = new Field("Name");

			StringOperator equals = StringOperator.List[StringOperator.Ops.equals];
			StringOperator doesNotEqual = StringOperator.List[StringOperator.Ops.doesNotEqual];
			StringOperator contains = StringOperator.List[StringOperator.Ops.contains];
			StringOperator doesNotContain = StringOperator.List[StringOperator.Ops.doesNotContain];
			StringOperator beginsWith = StringOperator.List[StringOperator.Ops.beginsWith];
			StringOperator endsWith = StringOperator.List[StringOperator.Ops.endsWith];

			Condition condition1 = new Condition(name, equals, new Expression("Steve"));
			Condition condition2 = new Condition(name, doesNotEqual, new Expression("Homer"));
			Condition condition3 = new Condition(name, contains, new Expression("tev"));
			Condition condition4 = new Condition(name, doesNotContain, new Expression("ome"));
			Condition condition5 = new Condition(name, beginsWith, new Expression("S"));
			Condition condition6 = new Condition(name, endsWith, new Expression("e"));

			//Assertions 
			Assert.AreEqual(6, StringOperator.List.Count);
			Assert.AreEqual("Name equals \"Steve\"", condition1.ToString());
			Assert.AreEqual("Name does not equal \"Homer\"", condition2.ToString());
			Assert.AreEqual("Name contains \"tev\"", condition3.ToString());
			Assert.AreEqual("Name does not contain \"ome\"", condition4.ToString());
			Assert.AreEqual("Name begins with \"S\"", condition5.ToString());
			Assert.AreEqual("Name ends with \"e\"", condition6.ToString());
		}

		[Test]
		public void ConstructHybrid()
		{
			Field name = new Field("Name");
			Field age = new Field("Age");

			HybridOperator equals = HybridOperator.List[HybridOperator.Ops.equals];
			HybridOperator doesNotEqual = HybridOperator.List[HybridOperator.Ops.doesNotEqual];
			HybridOperator contains = HybridOperator.List[HybridOperator.Ops.contains];
			HybridOperator doesNotContain = HybridOperator.List[HybridOperator.Ops.doesNotContain];
			HybridOperator beginsWith = HybridOperator.List[HybridOperator.Ops.beginsWith];
			HybridOperator endsWith = HybridOperator.List[HybridOperator.Ops.endsWith];
			HybridOperator isLessThan = HybridOperator.List[HybridOperator.Ops.isLessThan];
			HybridOperator isLessThanOrEqualTo = HybridOperator.List[HybridOperator.Ops.isLessThanOrEqualTo];
			HybridOperator isGreaterThan = HybridOperator.List[HybridOperator.Ops.isGreaterThan];
			HybridOperator isGreaterThanOrEqualTo = HybridOperator.List[HybridOperator.Ops.isGreaterThanOrEqualTo];
			HybridOperator isBlank = HybridOperator.List[HybridOperator.Ops.isBlank];
			HybridOperator isNotBlank = HybridOperator.List[HybridOperator.Ops.isNotBlank];

			Condition condition1 = new Condition(name, equals, new Expression("Steve"));
			Condition condition2 = new Condition(name, doesNotEqual, new Expression("Homer"));
			Condition condition3 = new Condition(name, contains, new Expression("tev"));
			Condition condition4 = new Condition(name, doesNotContain, new Expression("ome"));
			Condition condition5 = new Condition(name, beginsWith, new Expression("S"));
			Condition condition6 = new Condition(name, endsWith, new Expression("e"));
			Condition condition7 = new Condition(age, isLessThan, new Expression("21"));
			Condition condition8 = new Condition(age, isLessThanOrEqualTo, new Expression("21"));
			Condition condition9 = new Condition(age, isGreaterThan, new Expression("21"));
			Condition condition10 = new Condition(age, isGreaterThanOrEqualTo, new Expression("21"));
			Condition condition11 = new Condition(name, isBlank);
			Condition condition12 = new Condition(name, isNotBlank);


			//Assertions 
			Assert.AreEqual(6, StringOperator.List.Count);
			Assert.AreEqual("Name equals \"Steve\"", condition1.ToString());
			Assert.AreEqual("Name does not equal \"Homer\"", condition2.ToString());
			Assert.AreEqual("Name contains \"tev\"", condition3.ToString());
			Assert.AreEqual("Name does not contain \"ome\"", condition4.ToString());
			Assert.AreEqual("Name begins with \"S\"", condition5.ToString());
			Assert.AreEqual("Name ends with \"e\"", condition6.ToString());
			Assert.AreEqual("Age is less than 21", condition7.ToString());
			Assert.AreEqual("Age is less than or equal to 21", condition8.ToString());
			Assert.AreEqual("Age is greater than 21", condition9.ToString());
			Assert.AreEqual("Age is greater than or equal to 21", condition10.ToString());
			Assert.AreEqual("Name is blank", condition11.ToString());
			Assert.AreEqual("Name is not blank", condition12.ToString());

		}

		[Test]
		public void ConstructHybridFromFriendlyName()
		{
			Field name = new Field("Name");
			Field age = new Field("Age");

			HybridOperator equals = HybridOperator.List["equals"];
			HybridOperator doesNotEqual = HybridOperator.List["does not equal"];
			HybridOperator contains = HybridOperator.List["contains"];
			HybridOperator doesNotContain = HybridOperator.List["does not contain"];
			HybridOperator beginsWith = HybridOperator.List["begins with"];
			HybridOperator endsWith = HybridOperator.List["ends with"];
			HybridOperator isLessThan = HybridOperator.List["is less than"];
			HybridOperator isLessThanOrEqualTo = HybridOperator.List["is less than or equal to"];
			HybridOperator isGreaterThan = HybridOperator.List["is greater than"];
			HybridOperator isGreaterThanOrEqualTo = HybridOperator.List["is greater than or equal to"];
			HybridOperator isBlank = HybridOperator.List["is blank"];
			HybridOperator isNotBlank = HybridOperator.List["is not blank"];

			Condition condition1 = new Condition(name, equals, new Expression("Steve"));
			Condition condition2 = new Condition(name, doesNotEqual, new Expression("Homer"));
			Condition condition3 = new Condition(name, contains, new Expression("tev"));
			Condition condition4 = new Condition(name, doesNotContain, new Expression("ome"));
			Condition condition5 = new Condition(name, beginsWith, new Expression("S"));
			Condition condition6 = new Condition(name, endsWith, new Expression("e"));
			Condition condition7 = new Condition(age, isLessThan, new Expression("21"));
			Condition condition8 = new Condition(age, isLessThanOrEqualTo, new Expression("21"));
			Condition condition9 = new Condition(age, isGreaterThan, new Expression("21"));
			Condition condition10 = new Condition(age, isGreaterThanOrEqualTo, new Expression("21"));
			Condition condition11 = new Condition(name, isBlank);
			Condition condition12 = new Condition(name, isNotBlank);


			//Assertions 
			Assert.AreEqual(6, StringOperator.List.Count);
			Assert.AreEqual("Name equals \"Steve\"", condition1.ToString());
			Assert.AreEqual("Name does not equal \"Homer\"", condition2.ToString());
			Assert.AreEqual("Name contains \"tev\"", condition3.ToString());
			Assert.AreEqual("Name does not contain \"ome\"", condition4.ToString());
			Assert.AreEqual("Name begins with \"S\"", condition5.ToString());
			Assert.AreEqual("Name ends with \"e\"", condition6.ToString());
			Assert.AreEqual("Age is less than 21", condition7.ToString());
			Assert.AreEqual("Age is less than or equal to 21", condition8.ToString());
			Assert.AreEqual("Age is greater than 21", condition9.ToString());
			Assert.AreEqual("Age is greater than or equal to 21", condition10.ToString());
			Assert.AreEqual("Name is blank", condition11.ToString());
			Assert.AreEqual("Name is not blank", condition12.ToString());

		}

		[Test]
		public void ConstructHybridFromXmlTagName()
		{
			Field name = new Field("Name");
			Field age = new Field("Age");

			HybridOperator equals = HybridOperator.List["equals"];
			HybridOperator doesNotEqual = HybridOperator.List["doesNotEqual"];
			HybridOperator contains = HybridOperator.List["contains"];
			HybridOperator doesNotContain = HybridOperator.List["doesNotContain"];
			HybridOperator beginsWith = HybridOperator.List["beginsWith"];
			HybridOperator endsWith = HybridOperator.List["endsWith"];
			HybridOperator isLessThan = HybridOperator.List["isLessThan"];
			HybridOperator isLessThanOrEqualTo = HybridOperator.List["isLessThanOrEqualTo"];
			HybridOperator isGreaterThan = HybridOperator.List["isGreaterThan"];
			HybridOperator isGreaterThanOrEqualTo = HybridOperator.List["isGreaterThanOrEqualTo"];
			HybridOperator isBlank = HybridOperator.List["isBlank"];
			HybridOperator isNotBlank = HybridOperator.List["isNotBlank"];

			Condition condition1 = new Condition(name, equals, new Expression("Steve"));
			Condition condition2 = new Condition(name, doesNotEqual, new Expression("Homer"));
			Condition condition3 = new Condition(name, contains, new Expression("tev"));
			Condition condition4 = new Condition(name, doesNotContain, new Expression("ome"));
			Condition condition5 = new Condition(name, beginsWith, new Expression("S"));
			Condition condition6 = new Condition(name, endsWith, new Expression("e"));
			Condition condition7 = new Condition(age, isLessThan, new Expression("21"));
			Condition condition8 = new Condition(age, isLessThanOrEqualTo, new Expression("21"));
			Condition condition9 = new Condition(age, isGreaterThan, new Expression("21"));
			Condition condition10 = new Condition(age, isGreaterThanOrEqualTo, new Expression("21"));
			Condition condition11 = new Condition(name, isBlank);
			Condition condition12 = new Condition(name, isNotBlank);


			//Assertions 
			Assert.AreEqual(6, StringOperator.List.Count);
			Assert.AreEqual("Name equals \"Steve\"", condition1.ToString());
			Assert.AreEqual("Name does not equal \"Homer\"", condition2.ToString());
			Assert.AreEqual("Name contains \"tev\"", condition3.ToString());
			Assert.AreEqual("Name does not contain \"ome\"", condition4.ToString());
			Assert.AreEqual("Name begins with \"S\"", condition5.ToString());
			Assert.AreEqual("Name ends with \"e\"", condition6.ToString());
			Assert.AreEqual("Age is less than 21", condition7.ToString());
			Assert.AreEqual("Age is less than or equal to 21", condition8.ToString());
			Assert.AreEqual("Age is greater than 21", condition9.ToString());
			Assert.AreEqual("Age is greater than or equal to 21", condition10.ToString());
			Assert.AreEqual("Name is blank", condition11.ToString());
			Assert.AreEqual("Name is not blank", condition12.ToString());

		}

		[Test]
		public void ConstructMCMany() 
		{ 
			Field q1 = new Field("Q1");

			MCManyOperator equals = MCManyOperator.List[MCManyOperator.Ops.mcEquals];
			MCManyOperator doesNotEqual = MCManyOperator.List[MCManyOperator.Ops.mcDoesNotEqual];
			MCManyOperator contains = MCManyOperator.List[MCManyOperator.Ops.mcContains];
			MCManyOperator doesNotContain = MCManyOperator.List[MCManyOperator.Ops.mcDoesNotContain];
			MCManyOperator isBlank = MCManyOperator.List[MCManyOperator.Ops.mcIsBlank];
			MCManyOperator isNotBlank = MCManyOperator.List[MCManyOperator.Ops.mcIsNotBlank];

			Condition condition1 = new Condition(q1, equals, new Expression("a"));
			Condition condition2 = new Condition(q1, doesNotEqual, new Expression("b"));
			Condition condition3 = new Condition(q1, contains, new Expression("a"));
			Condition condition4 = new Condition(q1, doesNotContain, new Expression("b"));
			Condition condition5 = new Condition(q1, isBlank);
			Condition condition6 = new Condition(q1, isNotBlank);

			//Assertions 
			Assert.AreEqual(6, MCManyOperator.List.Count);
			Assert.AreEqual("Q1 equals a", condition1.ToString());
			Assert.AreEqual("Q1 does not equal b", condition2.ToString());
			Assert.AreEqual("Q1 contains a", condition3.ToString());
			Assert.AreEqual("Q1 does not contain b", condition4.ToString());
			Assert.AreEqual("Q1 is blank", condition5.ToString());
			Assert.AreEqual("Q1 is not blank", condition6.ToString());
		} 

		[Test]
		public void ConstructMCOne() 
		{ 
			Field q1 = new Field("Q1");

			MCOneOperator equals = MCOneOperator.List[MCOneOperator.Ops.mcEquals];
			MCOneOperator doesNotEqual = MCOneOperator.List[MCOneOperator.Ops.mcDoesNotEqual];
			MCOneOperator isBlank = MCOneOperator.List[MCOneOperator.Ops.mcIsBlank];
			MCOneOperator isNotBlank = MCOneOperator.List[MCOneOperator.Ops.mcIsNotBlank];

			Condition condition1 = new Condition(q1, equals, new Expression("a"));
			Condition condition2 = new Condition(q1, doesNotEqual, new Expression("a"));
			Condition condition3 = new Condition(q1, isBlank);
			Condition condition4 = new Condition(q1, isNotBlank);

			//Assertions 
			Assert.AreEqual(4, MCOneOperator.List.Count);
			Assert.AreEqual("Q1 equals a", condition1.ToString());
			Assert.AreEqual("Q1 does not equal a", condition2.ToString());
			Assert.AreEqual("Q1 is blank", condition3.ToString());
			Assert.AreEqual("Q1 is not blank", condition4.ToString());
		} 
	}
}
