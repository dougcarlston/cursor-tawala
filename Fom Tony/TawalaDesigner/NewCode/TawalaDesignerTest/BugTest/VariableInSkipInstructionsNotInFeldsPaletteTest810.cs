using System;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using TawalaTest.TestingSupport;
using Tawala.ProjectUI;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class VariableInSkipInstructionsNotInFeldsPaletteTest810
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			ComponentMaker.UseNewComponents(true);
		}

		[TearDown]
		public void TearDown()
		{
			ComponentMaker.UseNewComponents(false);
		}

        [Test]
        public void VariableSetInSkipInstructionsAppearsInFieldsPalette()
        {
			FieldsPalette fieldsPalette = new FieldsPalette();

			IForm form = Project.Current.AddForm();

			ISkipInstructionsItem skipInstructionsItem = new NewSkipInstructionsItem();
			skipInstructionsItem.Instructions = new NewSkipInstructions();
			form.ItemList.Add(skipInstructionsItem);

			SetStatement setStatement = new SetStatement(skipInstructionsItem.Instructions);
			setStatement.Variable = new Variable("Var");
			setStatement.Expression = new Expression("100");

			skipInstructionsItem.Instructions.Lines.Add(new SetLine(setStatement));

			fieldsPalette.RefreshFieldList();

			Assert.AreEqual(2, fieldsPalette.FieldsTreeView.Nodes.Count);
			Assert.AreEqual("Variables", fieldsPalette.FieldsTreeView.Nodes[1].Text);
			Assert.AreEqual(2, fieldsPalette.FieldsTreeView.Nodes[1].Nodes.Count);
			Assert.AreEqual("Var", fieldsPalette.FieldsTreeView.Nodes[1].Nodes[1].Text);
		}
	}
}
