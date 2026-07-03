using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
	public class SetLineTest
	{
#if FIXED
		private IForm form;
		private Process process;
		private SetStatement statement;
		private ProcessLine line;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
            Util.NewTestProject();

			form = Project.Current.AddForm();

			process = Project.Current.AddProcess();

			statement = new SetStatement();
			statement.Variable = new Variable("Var");

			line = new SetLine(statement);
			process.Lines.Add(line);
		}

		[Test]
		public void ValidateLine()
		{
			// it shouldn't be valid unless it has a variable (field) and an assigned value (expression)
			// (both are null to begin with)
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			// just variable text is not enough
			statement.Variable = new Variable("a variable");
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);
			process.Variables.AddUnique("a variable");

			// now we have both; should be valid
			statement.Expression = new Expression("a value");
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);

			// test empty strings, as well as null
			statement.Variable = new Variable("");
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			statement.Variable = new Variable("a variable");
			statement.Expression = new Expression("");
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);
		}

		[Test]
		public void ValidateLineWithField()
		{
			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			statement.Variable = new Variable("Variable");
			statement.Expression = new Expression("<<Q1:a>>");
			process.Variables.AddUnique("Variable");

			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}

		[Test]
		public void ValidateLineWithComplexExpression()
		{
			FibItem fibItem = new FibItem();
			fibItem.Text = "__ __ __";
			fibItem.BlankList[0].AlternateLabel = "number1";
			fibItem.BlankList[1].AlternateLabel = "number2";
			fibItem.BlankList[2].AlternateLabel = "number3";

			form.ItemList.Add(fibItem);

			Project.Current.ConnectProcessToForm(process, form.Name);

			// check for valid expressions
			statement.Variable = new Variable("Variable");
			statement.Expression = new Expression("(<<number1>> / (<<number2>> + 7) - <<number3>>) * 5");
			process.Variables.AddUnique("Variable");
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);

			statement.Expression = new Expression("(<<number1>> / (<<number2>> ++ 7) - <<number3>>) * 5");
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);
		}

		[Test]
		public void ValidateLineInSkipInstructions()
		{
			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			SkipInstructionsItem skip = new SkipInstructionsItem();
			form.ItemList.Add(skip);

			SetStatement statement2 = new SetStatement();
			statement2.Variable = new Variable("Variable");
			statement2.Expression = new Expression("<<Q1:a>>");

			ProcessLine line2 = new SetLine(statement2);
			skip.Instructions.Lines.Add(line2);
			skip.Instructions.Variables.AddUnique("Variable");

			skip.Instructions.Lines.ValidateLines();
			Assert.AreEqual(true, line2.IsValid);
		}

		[Test]
		[Ignore("Fails due to commented out code in SetLine.IsValidLine for Hidden Fields")]
		public void ValidateLineWithQualifiedVariable()
		{
			process.Variables.AddUnique("variable");

			statement.Variable = new Variable("record:variable");
			statement.Expression = new Expression("a value");
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			// "Get record list from Form 1"
			FormList forms = new FormList();
			forms.Add((IForm)Project.Current.FormList[0]);
			RecordSet recordList = new RecordSet("record list", forms);
			GetStatement getStatement = new GetStatement(recordList);
			process.Lines.Add(new ProcessLineList(getStatement));

			// "For Each record in record list"
			Record record = new Record("record");
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			process.Lines.Add(forEachLines);

			// "Set record:variable to a value"
			SetStatement statement2 = new SetStatement();
			statement2.Variable = new Variable("record:variable");
			statement2.Expression = new Expression("a value");
			SetLine line2 = new SetLine(statement2);

			// insert SetLine inside ForEach block
			process.Lines.Insert(4, line2);
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line2.IsValid);
		}

		[Test]
		public void ValidateLineWithBadArithmeticExpression()
		{
			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			statement.Variable = new Variable("Variable");
			statement.Expression = new Expression("<<Q1:a>> ++ 1");

			process.Variables.AddUnique("Variable");

			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);
		}

		[Test]
		public void ValidateLineWithBadArithmeticExpressionAsText()
		{
			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			statement.Variable = new Variable("Variable");
			statement.Expression = new Expression("<<Q1:a>> ++ 1");
			statement.TreatArithmeticAsText = true;

			process.Variables.AddUnique("Variable");

			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}

		private SetLine getSetLineEnclosedInForEach(string variable, string expression)
		{
			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			FormList recordSetForms = new FormList();
			recordSetForms.Add((IForm)Project.Current.FormList[0]);
			RecordSet recordSet = new RecordSet("Record List 1", recordSetForms);

			GetStatement getStatement = new GetStatement(recordSet);
			process.Lines.Add(new GetLine(getStatement));

			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(new Record("Record"), recordSet);
			process.Lines.Add(new ProcessLineList(forEachStatement));

			SetStatement setStatement2 = new SetStatement();
			setStatement2.Variable = new Variable(variable);
			setStatement2.Expression = new Expression(expression, process.GetValidFields(4));

			SetLine enclosedSetLine = new SetLine(setStatement2);
			process.Lines.Insert(4, enclosedSetLine);

			Assert.AreEqual(6, process.Lines.Count);
			return enclosedSetLine;
		}

        [Ignore("Update test to work with new classes or this test may not be compatible with the current structure")]
        [Test]
		public void ValidateLineWithRecordFieldAsField()
		{
			SetLine enclosedSetLine = getSetLineEnclosedInForEach("Record:Form 1:Q1:a", "value");

			Assert.AreEqual(true, enclosedSetLine.IsValidLine(process.GetValidFields(4)));
		}

        [Ignore("Update test to work with new classes or this test may not be compatible with the current structure")]
        [Test]
		public void ValidateLineWithRecordFieldAsExpression()
		{
			SetLine enclosedSetLine = getSetLineEnclosedInForEach("Variable2", "<<Record:Form 1:Q1:a>>");

			Assert.AreEqual(true, enclosedSetLine.IsValidLine(process.GetValidFields(4)));
		}

        [Ignore("Update test to work with new classes or this test may not be compatible with the current structure")]
        [Test]
		public void ValidateLineWithExtraFieldAsExpression()
		{
			form.ConnectedProcess = process;

			SetLine enclosedSetLine = getSetLineEnclosedInForEach("Variable2", "<<Record:Form 1:Var>>");

			Assert.AreEqual(true, enclosedSetLine.IsValidLine(process.GetValidFields(4)));
		}
#endif
	}
}
