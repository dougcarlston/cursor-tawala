using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Processes;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class SkipToLineTest
	{
		[Test]
		public void Validate()
		{
			Project.NewTestProject();

			IForm form = Project.Current.AddForm();

			SkipInstructionsItem skip = new SkipInstructionsItem();
			form.ItemList.Add(skip);

			SkipToStatement statement = new SkipToStatement();
			ProcessLine line = new SkipToLine(statement);
			skip.Instructions.Lines.Add(line);
			skip.Instructions.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			statement.Destination = new SkipToDestinationItem();
			skip.Instructions.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}
	}
}
