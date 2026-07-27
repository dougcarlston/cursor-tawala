using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class SkipToLineTest
	{
		[Test]
		public void Validate()
		{
            Util.NewTestProject();

			IForm form = Project.Current.AddForm();

			ISkipInstructionsItem skip = new NewSkipInstructionsItem();
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
