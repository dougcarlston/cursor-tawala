using System;
using NUnit.Framework;
using Tawala.Projects;
using System.Collections;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Class for testing the Variables list in a Process
	/// </summary>
	[TestFixture]
	public class ProcessVariableListTest
	{
		private Process process;
		private SetStatement setStatement1;
		private SetStatement setStatement2;

		// used by some test methods to make sure an event is raised
		private int eventRaisedCount = 65535;

		// execute this once at beginning of tests
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
		}

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			TestingSupport.Util.NewTestProject();

			// create process
			process = Project.Current.AddProcess();

			setStatement1 = new SetStatement();
			setStatement1.Variable = new Variable("Var1");
			setStatement1.Expression = new Expression("0");
			process.Lines.Add(new ProcessLineList(setStatement1));

			setStatement2 = new SetStatement();
			setStatement2.Variable = new Variable("Var2");
			setStatement2.Expression = new Expression("0");
			process.Lines.Add(new ProcessLineList(setStatement2));

			Assert.AreEqual(2, process.Variables.Count);

			Project.Events.ProcessVariableListChanged += events_ProcessVariableListChanged;
		}

		private void events_ProcessVariableListChanged(object sender, ComponentEventArgs e)
		{
			eventRaisedCount++;
			Assert.IsInstanceOfType(typeof(Process), e.Component);
			Assert.AreEqual("Process 1", ((Process)e.Component).Name);
		}

		[Test]
		public void AddStatementWithDuplicateVariable()
		{
			AddStatement addStatement = new AddStatement();
			addStatement.Variable = "Var1";
			addStatement.Value = new FieldOrLiteral("10", FieldOrLiteral.StringType.literal);

			eventRaisedCount = 0;
			process.Lines.Add(new ProcessLineList(addStatement));

			Assert.AreEqual(2, process.Variables.Count);
			Assert.AreEqual(1, eventRaisedCount);
		}

		[Test]
		public void RemoveStatement()
		{
			eventRaisedCount = 0;
			process.Lines.RemoveAt(0);

			Assert.AreEqual(1, process.Variables.Count);
			Assert.AreEqual(1, eventRaisedCount);
		}

		[Test]
		public void ModifyStatement()
		{
			AddStatement addStatement = new AddStatement();
			addStatement.Variable = "Var1";
			addStatement.Value = new FieldOrLiteral("10", FieldOrLiteral.StringType.literal);
			process.Lines.Add(new ProcessLineList(addStatement));

			Assert.AreEqual(2, process.Variables.Count);

			addStatement.Variable = "Var3";

			eventRaisedCount = 0;
			process.Lines.Replace(2, addStatement);

			Assert.AreEqual(3, process.Variables.Count);
			Assert.AreEqual(1, eventRaisedCount);
		}
	}
}
