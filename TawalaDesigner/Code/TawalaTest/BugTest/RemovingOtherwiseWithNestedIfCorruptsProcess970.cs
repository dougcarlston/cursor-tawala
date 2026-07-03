using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for Mantis issue 970 (Removing the OTHERWISE clause from nested IF can damage statement).
	/// </summary>
	[TestFixture]
	public class RemovingOtherwiseWithNestedIfCorruptsProcess970
	{
		private Process process;
		private SetStatement setStatement1;

		[Test]
		public void RemovingOtherwiseFromOutermostIfLeavesProcessIntact()
		{
			Util.NewTestProject();
			setupProcess();
			verifyProcessLinesBefore();

			removeOtherwiseFromFirstIfStatement();
			verifyProcessLinesAfter();
		}

		private void setupProcess()
		{
			process = Project.Current.AddProcess();

			var setStatment1Xml =
				@"<set field=""Var1"" arithmeticAsText=""false"">" + Environment.NewLine +
				@"<string value=""Foo""/>" + Environment.NewLine +
				@"</set>";
			setStatement1 = new SetStatement(new XmlElement(setStatment1Xml), process);

			var setStatment2Xml =
				@"<set field=""Var2"" arithmeticAsText=""false"">" + Environment.NewLine +
				@"<string value=""Bar""/>" + Environment.NewLine +
				@"</set>";
			var setStatement2 = new SetStatement(new XmlElement(setStatment2Xml), process);

			process.Lines.Add(new ProcessLineList(setStatement1));
			process.Lines.Add(new ProcessLineList(setStatement2));

			var conditions1 = new Conditions(setStatement1.Variable, HybridOperator.List[HybridOperator.Ops.equals], new Expression("Foo"));
			var ifStatement1 = new IfStatement(conditions1, true);
			process.Lines.Add(new ProcessLineList(ifStatement1));

			var conditions2 = new Conditions(setStatement2.Variable, HybridOperator.List[HybridOperator.Ops.equals], new Expression("Bar"));
			var ifStatement2 = new IfStatement(conditions2, true);
			process.Lines.Insert(4, new ProcessLineList(ifStatement2));
		}

		private void verifyProcessLinesBefore()
		{
			Assert.AreEqual(14, process.Lines.Count);

			Assert.AreEqual(@"Set Var1 to ""Foo""", process.Lines[0].ToString());
			Assert.AreEqual(@"Set Var2 to ""Bar""", process.Lines[1].ToString());
			Assert.AreEqual(@"If Var1 equals ""Foo""", process.Lines[2].ToString());
			Assert.AreEqual(@"(", process.Lines[3].ToString());
			Assert.AreEqual(@"If Var2 equals ""Bar""", process.Lines[4].ToString());
			Assert.AreEqual(@"(", process.Lines[5].ToString());
			Assert.AreEqual(@")", process.Lines[6].ToString());
			Assert.AreEqual(@"Otherwise", process.Lines[7].ToString());
			Assert.AreEqual(@"(", process.Lines[8].ToString());
			Assert.AreEqual(@")", process.Lines[9].ToString());
			Assert.AreEqual(@")", process.Lines[10].ToString());
			Assert.AreEqual(@"Otherwise", process.Lines[11].ToString());
			Assert.AreEqual(@"(", process.Lines[12].ToString());
			Assert.AreEqual(@")", process.Lines[13].ToString());
		}

		private void removeOtherwiseFromFirstIfStatement()
		{
			var newConditions1 = new Conditions(setStatement1.Variable, HybridOperator.List[HybridOperator.Ops.equals], new Expression("Foo"));
			var replacementIfStatement1 = new IfStatement(newConditions1, false);
			process.Lines.Replace(2, replacementIfStatement1);

			process.Lines.SetIndentLevels();
		}

		private void verifyProcessLinesAfter()
		{
			Assert.AreEqual(11, process.Lines.Count);

			Assert.AreEqual(@"Set Var1 to ""Foo""", process.Lines[0].ToString());
			Assert.AreEqual(@"Set Var2 to ""Bar""", process.Lines[1].ToString());
			Assert.AreEqual(@"If Var1 equals ""Foo""", process.Lines[2].ToString());
			Assert.AreEqual(@"(", process.Lines[3].ToString());
			Assert.AreEqual(@"If Var2 equals ""Bar""", process.Lines[4].ToString());
			Assert.AreEqual(@"(", process.Lines[5].ToString());
			Assert.AreEqual(@")", process.Lines[6].ToString());
			Assert.AreEqual(@"Otherwise", process.Lines[7].ToString());
			Assert.AreEqual(@"(", process.Lines[8].ToString());
			Assert.AreEqual(@")", process.Lines[9].ToString());
			Assert.AreEqual(@")", process.Lines[10].ToString());
		}
	}
}