using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;


namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 703 (Changing MCQ comparison from "equals" to "is blank" causes bad save).
	///
	/// Note: The bug is a result of the fact that the ConditionsGroup UI does not clear the expression box
	///       when the comparison operator is changed, so a residual value is left in the box that gets
	///       transferred to the Condition class.
	///															jdf - 3/20/08
	/// </summary>
	[TestFixture]
	public class McqIsBlankConditionCausesBadSave703
	{
		private McqItem mcItem;
		private Process process;

		[SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();

			IForm form = Project.Current.AddForm();
			mcItem = new McqItem();
			form.ItemList.Add(mcItem);

			process = Project.Current.AddProcess();
		}

		[Test]
		public void IsBlankConditionGeneratesProperXml()
		{
			mimicEqualsConditionProducedByConditionsGroupUI();
			Condition isBlankCondition = mimicIsBlankConditionProducedByConditionsGroupUI();

			Console.WriteLine(isBlankCondition.ToXml());
			Assert.AreEqual("<mcIsBlank field=\"Form 1:Q1\" />\r\n", isBlankCondition.ToXml());
		}

		[Test]
		public void ProjectSavedAfterChangingEqualsConditionToIsBlank()
		{
			mimicEqualsConditionProducedByConditionsGroupUI();
			Condition isBlankCondition = mimicIsBlankConditionProducedByConditionsGroupUI();
			IfStatement ifStatement = new IfStatement(new Conditions(isBlankCondition));
			addIfLinesToProcess(ifStatement);

			Util.SaveAndReloadCurrentProject();
			Console.WriteLine(Project.Current.ToXml());

			Util.SaveAndReloadCurrentProject();
		}

		private Expression residualExpression = null;

		private Condition mimicEqualsConditionProducedByConditionsGroupUI()
		{
			ChoiceField choiceField = new ChoiceField("<<a>>");
			residualExpression = new Expression(choiceField);
			return new Condition(mcItem, MCOneOperator.List[MCOneOperator.Ops.mcEquals], residualExpression);
		}

		private Condition mimicIsBlankConditionProducedByConditionsGroupUI()
		{
			return new Condition(mcItem, MCOneOperator.List[MCOneOperator.Ops.mcIsBlank], residualExpression);
		}

		private void addIfLinesToProcess(IfStatement ifStatement)
		{
			IfLine ifLine = new IfLine(ifStatement);
			process.Lines.Add(ifLine);
			process.Lines.Add(new BlockOpenLine(ifStatement, "(", "<trueSet>"));
			process.Lines.Add(new BlockCloseLine(ifStatement, ")", "</trueSet>\r\n</if>"));
		}
	}
}
