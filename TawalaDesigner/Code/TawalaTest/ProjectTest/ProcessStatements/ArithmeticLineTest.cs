using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class ArithmeticLineTest
	{
		private IForm form;
		private Process process;
		private ArithmeticStatement statement;
		private ProcessLine line;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			Project.NewTestProject();

			form = Project.Current.AddForm();

			process = Project.Current.AddProcess();

			statement = new ArithmeticStatement();
			line = new ArithmeticLine(statement);
			process.Lines.Add(line);
		}

		[Test]
		public void ValidateLineWithField()
		{
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			statement.Variable = "Q1:a";
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			Project.Current.ConnectProcessToForm(process, "Form 1");

			// Note: for now we don't allow Form fields to be modified
//			Assert.AreEqual(true, line.IsValid);
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);
		}

		[Test]
		public void ValidateLineWithVariable()
		{
			statement.Variable = "a variable";
			statement.Value.Text = "2";
			statement.Value.Type = FieldOrLiteral.StringType.literal;
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			process.Variables.AddUnique("a variable");
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}

	    [Test]
	    public void ValidateSkipInstructionsLineWithField()
	    {
	        SkipInstructionsItem skip = new SkipInstructionsItem();
	        form.ItemList.Add(skip);

			ArithmeticStatement statement2 = new ArithmeticStatement();
			statement2.Variable = "Q1:a";
			statement2.Value.Text = "2";
			statement2.Value.Type = FieldOrLiteral.StringType.literal;

	        ProcessLine line2 = new ArithmeticLine(statement2);
	        skip.Instructions.Lines.Add(line2);
			skip.Instructions.Lines.ValidateLines();
			Assert.AreEqual(false, line2.IsValid);

			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			// Note: for now we don't allow Form fields to be modified
//			Assert.AreEqual(true, line2.IsValid);
			skip.Instructions.Lines.ValidateLines();
			Assert.AreEqual(false, line2.IsValid);
		}

		[Test]
		public void ValidateSkipInstructionsLineWithVariable()
		{
			SkipInstructionsItem skip = new SkipInstructionsItem();
			form.ItemList.Add(skip);

			ArithmeticStatement statement2 = new ArithmeticStatement();
			statement2.Variable = "a variable";
			statement2.Value.Text = "2";
			statement2.Value.Type = FieldOrLiteral.StringType.literal;

			ProcessLine line2 = new ArithmeticLine(statement2);
			skip.Instructions.Lines.Add(line2);
			skip.Instructions.Lines.ValidateLines();
			Assert.AreEqual(false, line2.IsValid);

			skip.Instructions.Variables.AddUnique("a variable");
			skip.Instructions.Lines.ValidateLines();
			Assert.AreEqual(true, line2.IsValid);
		}
	}
}
