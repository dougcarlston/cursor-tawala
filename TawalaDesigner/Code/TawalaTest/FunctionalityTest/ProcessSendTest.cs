using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing SEND statement functionality as relates to the Process
	/// </summary>
	[TestFixture]
	public class ProcessSendTest
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
		}

		[Test]
		public void RepositionByInsertion()
		{
			// create SEND statement ('Send Email to <<Q1:a>>')
			SendStatement sendStatement = new SendStatement();
			sendStatement.AddressTo.Type = FieldOrLiteral.StringType.field;
			sendStatement.AddressTo.Text = "Q1:a";
			sendStatement.Subject = "Test Subject";
			sendStatement.SendBody = new SendEmailBody("Test Body");

			// make process line list from SET statement and add to process
			process.Lines.Add(new ProcessLineList(sendStatement));

			Assert.AreEqual("Send Email to Form 1:Q1:a", process.Lines[0].ToString());

			// create FIB item
			FibItem fibItem2 = new FibItem();

			// insert FIB item at beginning of form
			form.ItemList.Insert(0, fibItem2);

			Assert.AreEqual("Send Email to Form 1:Q2:a", process.Lines[0].ToString());
		}
	}
}
