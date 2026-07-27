using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;


namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 688 ("is blank" in conditions in Process statement causes crash).
	/// </summary>
	[TestFixture]
	public class IsBlankConditionCausesCrash688
	{
		Condition isBlankCondition;
		IfLine ifLine;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			IForm form = Project.Current.AddForm();
			FibItem fibItem = new FibItem();
			form.ItemList.Add(fibItem);

			Process process = Project.Current.AddProcess();

			isBlankCondition = new Condition(fibItem.BlankList[0], HybridOperator.List[HybridOperator.Ops.isBlank]);
			IfStatement ifStatement = new IfStatement(new Conditions(isBlankCondition));
			ifLine = new IfLine(ifStatement);
			process.Lines.Add(ifLine);
		}

		[Test]
		public void IsBlankConditionIsValidated()
		{
			try
			{
				ifLine.Validate();
			}
			catch
			{
				Assert.Fail("ifLine.Validate() threw an exception");
			}

			Assert.IsTrue(ifLine.IsValid);
		}

		[Test]
		public void ExpressionInIsBlankConditionProducesNoText()
		{
			try
			{
				string expectedText = "Form 1:Q1:a is blank";
				Assert.AreEqual(expectedText, isBlankCondition.ToString());
				Assert.AreEqual(string.Empty, isBlankCondition.Expression.ToString());
			}
			catch
			{
				Assert.Fail("isBlankCondition.NewExpression.ToString() threw an exception");
			}
		}
	}
}
