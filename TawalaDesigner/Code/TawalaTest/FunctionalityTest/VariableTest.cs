using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	[TestFixture]
	public class VariableTest
	{
		private Process process1;
		private Process process2;

		// execute this once at beginning of tests
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
		}

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
            TestSupport.Util.NewTestProject();

			process1 = Project.Current.AddProcess();
			process2 = Project.Current.AddProcess();

			process1.Variables.Add(new Variable("Variable1"));
			process2.Variables.Add(new Variable("Variable1"));
			process2.Variables.Add(new Variable("Variable2"));
		}

		[Test]
		public void ToolTipMultiProcess()
		{
			Variable var = process1.Variables[0];

			Assert.AreEqual("Process 1, Process 2", var.GetToolTipText());
		}

		[Test]
		public void ToolTipSingleProcess()
		{
			Variable var = process2.Variables[1];

			Assert.AreEqual("Process 2", var.GetToolTipText());
		}

	}
}
