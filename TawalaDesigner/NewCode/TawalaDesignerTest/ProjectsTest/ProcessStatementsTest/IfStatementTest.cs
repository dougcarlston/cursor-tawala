using System;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the IfStatement class
	/// </summary>
	[TestFixture]
	public class IfStatementTest
	{
		[SetUp]
		public void Setup()
		{
			TawalaTest.TestingSupport.Util.NewTestProject();
			Project.Current.AddForm();		// avoids exceptions when construction Conditions
		}

		[Test]
		public void Construct() 
		{ 
			ProcessStatement statement = new IfStatement();
		
			Assert.IsNotNull(statement);
		} 

		[Test]
		public void Name() 
		{ 
			ProcessStatement statement = new IfStatement();
		
			string name = statement.Name;

			Assert.AreEqual("If", name);
		} 

		[Test]
		public void GetText() 
		{ 
			Field firstName = new Field("First Name");
			ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
			Conditions conditions = new Conditions(firstName, equals, new Expression("Steve"));

			ProcessStatement statement = new IfStatement(conditions);

			string expString = "If First Name equals \"Steve\"";

			Assert.AreEqual(expString, statement.ToString());
		}

		[Test]
		public void GetXml() 
		{ 
			Field firstName = new Field("First Name");
			ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
			Conditions conditions = new Conditions(firstName, equals, new Expression("Steve"));

			IfStatement ifStatement1 = new IfStatement(conditions);
		
			string expString1 =	"<if>\r\n" +
								"<conditions>\r\n" +
								"<equals field=\"First Name\">\r\n" +
								"<string value=\"Steve\"/>\r\n" +
								"</equals>\r\n" +
								"</conditions>";

			Assert.AreEqual(expString1, ifStatement1.ToXml());

		}

		[Test]
		public void GetXmlIsBlank()
		{
			Field firstName = new Field("First Name");
			ComparisonOperator isBlank = HybridOperator.List[HybridOperator.Ops.isBlank];
			Conditions conditions = new Conditions(firstName, isBlank);

			IfStatement ifStatement1 = new IfStatement(conditions);

			string expString1 = "<if>\r\n" +
								"<conditions>\r\n" +
								"<isBlank field=\"First Name\" />\r\n" +
								"</conditions>";

			Assert.AreEqual(expString1, ifStatement1.ToXml());

		}

		[Test]
		public void Copy()
		{
			Field firstName = new Field("First Name");
			ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
			Conditions conditions = new Conditions(firstName, equals, new Expression("Steve"));

			IfStatement statement = new IfStatement(conditions);

			IfStatement copiedStatement = (IfStatement)statement.Copy();

			Assert.IsFalse(copiedStatement == statement);
		}

		[Test]
		public void CopyAdvancedIf()
		{
			Field firstName = new Field("First Name");
			Condition condition1 = new Condition(firstName, HybridOperator.List["is not blank"]);

			Field lastName = new Field("Last Name");
			//FieldOrLiteral expression = new FieldOrLiteral("a", FieldOrLiteral.StringType.literal);
			//Condition condition2 = new Condition(lastName, MCOneOperator.List["equals"], expression);
			Expression expression = new Expression("a");
			Condition condition2 = new Condition(lastName, MCOneOperator.List["equals"], expression);

			Conditions conditions = new Conditions();
			conditions.Add(condition1);
			conditions.Add(new LogicalOperator("AND"));
			conditions.Add(condition2);

			// create IF statement ('If Q1:a is not blank AND Q2 equals a')
			IfStatement statement = new IfStatement();
			statement.Conditions = conditions;

			IfStatement copiedStatement = (IfStatement)statement.Copy();

			Assert.IsFalse(copiedStatement == statement);
			Assert.AreEqual(3, copiedStatement.Conditions.Count);

			Condition copiedCondition1 = (Condition)copiedStatement.Conditions[0];
			Assert.AreSame(condition1.Field, copiedCondition1.Field);

			Condition copiedCondition2 = (Condition)copiedStatement.Conditions[2];
			Assert.AreSame(condition2.Field, copiedCondition2.Field);
		}
	}
}
