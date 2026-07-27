using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing ADD statement functionality as relates to the Process
	/// </summary>
	[TestFixture]
	public class ProcessAddTest
	{
		private IForm form;
		private Process process;

		// execute this once at beginning of tests
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
		}

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			// create clean project
			Project.NewTestProject();

			// create form
			form = Project.Current.AddForm();

			// create process
			process = Project.Current.AddProcess();

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			// add new FIB item to form
			form.ItemList.Add(new FibItem());

			// create SET statement ('Set Variable to <<Q1:a>>')
			SetStatement setStatement = new SetStatement();
			setStatement.Variable = new Variable("Variable");

			Expression expression = new Expression("<<Q1:a>>");
			setStatement.Expression = expression;

			// make process line list from SET statement and add to process
			process.Lines.Add(new ProcessLineList(setStatement));

			Assert.AreEqual("Set Variable to Form 1:Q1:a", process.Lines[0].ToString());
		}

		[Test]
		public void RepositionByInsertion()
		{
			// create ADD statement ('Add <<Q1:a>> to Variable')
			AddStatement addStatement = new AddStatement();
			addStatement.Variable = "Variable";
			addStatement.Value.Text = "Q1:a";
			addStatement.Value.Type = FieldOrLiteral.StringType.field;

			// make process line list from SET statement and add to process
			process.Lines.Add(new ProcessLineList(addStatement));

			Assert.AreEqual("Add Form 1:Q1:a to Variable", process.Lines[1].ToString());

			// create FIB item
			FibItem fibItem2 = new FibItem();

			// insert FIB item at beginning of form
			form.ItemList.Insert(0, fibItem2);

			Assert.AreEqual("Add Form 1:Q2:a to Variable", process.Lines[1].ToString());
		}

	}
}
