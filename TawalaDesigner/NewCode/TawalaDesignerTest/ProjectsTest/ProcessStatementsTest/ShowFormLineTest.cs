using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class ShowFormLineTest
	{
		private IForm form;
		private Process process;
		private ShowFormStatement statement;
		private ProcessLine line;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
            Util.NewTestProject();

			form = Project.Current.AddForm();
			form.Name = "Form1";

			process = Project.Current.AddProcess();

			statement = new ShowFormStatement(form);
			line = new ShowFormLine(statement);
			process.Lines.Add(line);
		}

		[Test]
		public void Validate()
		{
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}

		[Test]
		public void ValidateAfterRemoveForm()
		{
			Project.Current.RemoveForm("Form1");
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);
		}
	}
}
