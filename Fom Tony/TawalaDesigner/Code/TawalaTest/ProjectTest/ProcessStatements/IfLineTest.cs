using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class IfLineTest
	{
		private IForm form;
		private Process process;
		private IfStatement statement;
		private ProcessLine line;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			Project.NewTestProject();

			form = Project.Current.AddForm();

			process = Project.Current.AddProcess();

			statement = new IfStatement();
			line = new IfLine(statement);
			process.Lines.Add(line);
		}

		[Test]
		public void ValidateLineWithField()
		{
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
			Conditions conditions = new Conditions(fibItem1.BlankList[0], equals, new Expression("Steve"));
			statement.Conditions = conditions;
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}

		[Test]
		public void ValidateLineWithVariable()
		{
			Variable var = new Variable("foo");

			ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
			Conditions conditions = new Conditions(var, equals, new Expression("1"));
			statement.Conditions = conditions;
//			Assert.AreEqual(false, line.IsValid);

			process.Variables.Add(var);
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}

		[Test]
		public void ValidateSkipInstructionsLineWithField()
		{
			SkipInstructionsItem skip = new SkipInstructionsItem();
			form.ItemList.Add(skip);

			FibItem fibItem1 = new FibItem();

			ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
			Conditions conditions = new Conditions(fibItem1.BlankList[0], equals, new Expression("Steve"));
			IfStatement statement2 = new IfStatement(conditions);

			ProcessLine line2 = new IfLine(statement2);
			skip.Instructions.Lines.Add(line2);
//			Assert.AreEqual(false, line2.IsValid);

			form.ItemList.Add(fibItem1);
			skip.Instructions.Lines.ValidateLines();
			Assert.AreEqual(true, line2.IsValid);
		}

		[Test]
		public void ValidateSkipInstructionsLineWithVariable()
		{
			SkipInstructionsItem skip = new SkipInstructionsItem();
			form.ItemList.Add(skip);

			Variable var = new Variable("foo");

			ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
			Conditions conditions = new Conditions(var, equals, new Expression("1"));
			IfStatement statement2 = new IfStatement(conditions);

			ProcessLine line2 = new IfLine(statement2);
			skip.Instructions.Lines.Add(line2);
//			Assert.AreEqual(false, line2.IsValid);

			process.Variables.Add(var);
			skip.Instructions.Lines.ValidateLines();
			Assert.AreEqual(true, line2.IsValid);
		}

		[Test]
		public void ValidateLineWithMultipleConditions()
		{
			IMcqItem mcItem = new McqItem();
			mcItem.Choices.Add(new Choice());
			mcItem.Choices.Add(new Choice());
			form.ItemList.Add(mcItem);

			Condition condition1 = new Condition(mcItem, MCOneOperator.List["equals"], new Expression("a"));
			Condition condition2 = new Condition(mcItem, MCOneOperator.List["equals"], new Expression("b"));

			Conditions conditions = new Conditions();
			conditions.Add(condition1);
			conditions.Add(new LogicalOperator("OR"));
			conditions.Add(condition2);

			statement.Conditions = conditions;

			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}

		[Test]
		public void ValidateLineWithExpression()
		{
			Variable var1 = new Variable("Var1");
			Variable var2 = new Variable("Var2");
			process.Variables.Add(var1);
			process.Variables.Add(var2);

			// currently only the Advanced IF statement allows expressions
			ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
			Condition condition = new Condition(var2, equals, new Expression(var1));
			Conditions conditions = new Conditions();
			conditions.Add(condition);

			statement.Conditions = conditions;
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}
	}
}
