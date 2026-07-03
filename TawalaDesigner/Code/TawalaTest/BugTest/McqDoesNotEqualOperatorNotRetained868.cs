using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Bug 868: Operator in IF Statement Details panel within a SKIP is not preserved after Save.
	/// </summary>
	[TestFixture]
	public class McqDoesNotEqualOperatorNotRetained868
	{
		private IForm form;
		private Process process;
		private McqItem selectOnlyOneMcqItem;
		private McqItem selectManyMcqItem;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();

			selectOnlyOneMcqItem = new McqItem();
			form.ItemList.Add(selectOnlyOneMcqItem);

			selectManyMcqItem = new McqItem();
			selectManyMcqItem.SelectOnlyOne = false;
			form.ItemList.Add(selectManyMcqItem);

			process = Project.Current.AddProcess();
		}

		[Test]
		public void XmlWithSelectOnlyOneMcqProducesConditionWithMCOneOperator()
		{
			string xmlWithSelectOnlyOneMcq =
				@"<conditions>" + Environment.NewLine +
				@"<mcDoesNotEqual field=""Form 1:Q1"" value=""a""/>" + Environment.NewLine +
				@"</conditions>" + Environment.NewLine;

			var conditions = new Conditions(new XmlElement(xmlWithSelectOnlyOneMcq), process);

			var mcqCondition = conditions[0] as Condition;

			Assert.IsInstanceOfType(typeof(MCOneOperator), mcqCondition.CompOp);
		}

		[Test]
		public void XmlWithSelectManyMcqProducesConditionWithMCOneOperator()
		{
			string xmlWithSelectManyMcq =
				@"<conditions>" + Environment.NewLine +
				@"<mcDoesNotEqual field=""Form 1:Q2"" value=""a""/>" + Environment.NewLine +
				@"</conditions>" + Environment.NewLine;

			var conditions = new Conditions(new XmlElement(xmlWithSelectManyMcq), process);

			var mcqCondition = conditions[0] as Condition;

			Assert.IsInstanceOfType(typeof(MCManyOperator), mcqCondition.CompOp);
		}

		[Test]
		public void XmlWithSelectOnlyOneMcqComparedToFieldProducesConditionWithMCOneOperator()
		{
			string xmlWithSelectManyMcq =
				@"<conditions>" + Environment.NewLine +
				@"<mcDoesNotEqual field=""Form 1:Q1"">" + Environment.NewLine +
				@"<string field=""Form 1:Q1""/>" + Environment.NewLine +
				@"</mcDoesNotEqual>" + Environment.NewLine +
				@"</conditions>" + Environment.NewLine;

			var conditions = new Conditions(new XmlElement(xmlWithSelectManyMcq), process);

			var mcqCondition = conditions[0] as Condition;

			Assert.IsInstanceOfType(typeof(MCOneOperator), mcqCondition.CompOp);
		}

		[Test]
		public void XmlWithSelectManyMcqComparedToFieldProducesConditionWithMCOneOperator()
		{
			string xmlWithSelectManyMcq =
				@"<conditions>" + Environment.NewLine +
				@"<mcDoesNotEqual field=""Form 1:Q2"">" + Environment.NewLine +
				@"<string field=""Form 1:Q2""/>" + Environment.NewLine +
				@"</mcDoesNotEqual>" + Environment.NewLine +
				@"</conditions>" + Environment.NewLine;

			var conditions = new Conditions(new XmlElement(xmlWithSelectManyMcq), process);

			var mcqCondition = conditions[0] as Condition;

			Assert.IsInstanceOfType(typeof(MCManyOperator), mcqCondition.CompOp);
		}
	}
}
