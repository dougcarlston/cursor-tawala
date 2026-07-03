using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;


namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 899 (Changing MCQ from multi-select to single-select causes bad save).
	/// </summary>
	[TestFixture]
	public class InvalidMCQConditionCausesBadSave899
	{
		private IMcqItem mcqItem;
		private Process process;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			IForm form = Project.Current.AddForm();
			process = Project.Current.AddProcess();

			mcqItem = new McqItem();
			mcqItem.SelectOnlyOne = false;
			form.ItemList.Add(mcqItem);
		}

		[Test]
		public void ChangingMultiSelectMcqToSingleSelectChangesOperatorFromContainsToEquals()
		{
			string mcContainsConditionsXml =
				@"<conditions>" + Environment.NewLine +
				@"<mcContains field=""Form 1:Q1"" value=""a""/>" + Environment.NewLine +
				@"</conditions>";

			var mcContainsCondition = new Conditions(new XmlElement(mcContainsConditionsXml), process);
			var ifLine = new IfLine(new IfStatement(mcContainsCondition));
			process.Lines.Add(ifLine);

			mcqItem.SelectOnlyOne = true;

			string mcEqualsIfXml =
				@"<if>" + Environment.NewLine +
				@"<conditions>" + Environment.NewLine +
				@"<mcEquals field=""Form 1:Q1"" value=""a""/>" + Environment.NewLine +
				@"</conditions>";

			Assert.AreEqual(mcEqualsIfXml, ifLine.ToXml());
		}

		[Test]
		public void ChangingMultiSelectMcqToSingleSelectChangesOperatorFromDoesNotContainToDoesNotEqual()
		{
			string mcContainsConditionsXml =
				@"<conditions>" + Environment.NewLine +
				@"<mcDoesNotContain field=""Form 1:Q1"" value=""a""/>" + Environment.NewLine +
				@"</conditions>";

			var mcContainsCondition = new Conditions(new XmlElement(mcContainsConditionsXml), process);
			var ifLine = new IfLine(new IfStatement(mcContainsCondition));
			process.Lines.Add(ifLine);

			mcqItem.SelectOnlyOne = true;

			string mcDoesNotEqualIfXml =
				@"<if>" + Environment.NewLine +
				@"<conditions>" + Environment.NewLine +
				@"<mcDoesNotEqual field=""Form 1:Q1"" value=""a""/>" + Environment.NewLine +
				@"</conditions>";

			Assert.AreEqual(mcDoesNotEqualIfXml, ifLine.ToXml());
		}
	}
}
