using System;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Class to test Tawala.Proj.Variable class.
	/// </summary>
	[TestFixture]
	public class VariableTest
	{
		[SetUp]
		public void Setup()
		{
			TestingSupport.Util.NewTestProject();
		}

		[Test]
		public void VariableString() 
		{
			Variable var1 = new Variable("Variable 1");

			Assert.AreEqual("Variable 1", var1.FieldName);
		} 

		[Test]
		public void ToStringTestWithVariableInProcess() 
		{
			Variable var1 = new Variable("Variable 1");
			
			Process process = Project.Current.AddProcess();
			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new Variable("Variable 1");
			process.Lines.Add(new SetLine(setStatement));

			Assert.AreEqual("Variable 1", var1.ToString());
		}

		//[Test]
		//public void ToStringOnVariableNotInProcessResultsInUnknownField()
		//{
		//    Variable var1 = new Variable("Variable 1");

		//    Assert.AreEqual("Unknown Field", var1.ToString());
		//}

		[Test]
		public void ToStringOnVariableNotInProcessResultsInVariableName()
		{
			Variable var1 = new Variable("Variable 1");

			Assert.AreEqual("Variable 1", var1.ToString());
		}

		[Test]
		public void TrimWhitespace()
		{
			Variable var = new Variable("   Variable 1   ");

			Assert.AreEqual("Variable 1", var.FieldName);
		}

		[Test]
		public void FieldName()
		{
			Variable variable = new Variable("Variable 1");

			Assert.AreEqual("Variable 1", variable.FieldName);
		}

		[Test]
		public void FieldString()
		{
			Variable variable = new Variable("Variable 1");

			Assert.AreEqual("<<Variable 1>>", variable.FieldString);
		}

		[Test]
		public void OperatorDataSource()
		{
			Variable variable = new Variable("Variable 1");
			Assert.AreEqual(HybridOperator.List.DataSource, variable.OperatorDataSource);
		}

	}
}
