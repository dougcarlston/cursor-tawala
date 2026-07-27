using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 289 (Value in far right text box of get statement (ConditionGroup) is retained).
	/// </summary>
	[TestFixture]
	public class ExpressionRetained289
	{
		private IForm form;
		private FibItem fibItem;
		private Blank blank;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();
			fibItem = new FibItem();
			form.ItemList.Add(fibItem);
			blank = fibItem.BlankList[0];
		}

		[Test]
		public void ConditionWithExpressionProducesExpressionInXml()
		{
			Condition condition = new Condition(blank, HybridOperator.List["equals"], new Expression("1"));

			string expectedXml =
				"<equals field=\"Form 1:Q1:a\">\r\n" +
				"<string value=\"1\"/>\r\n" +
				"</equals>\r\n";

			Assert.AreEqual(expectedXml,condition.ToXml());
		}

		[Test]
		public void ConditionRequiringNoExpressionProducesNoExpressionInXml()
		{
			Condition condition = new Condition(blank, HybridOperator.List["is not blank"]);

			string expectedXml =
				"<isNotBlank field=\"Form 1:Q1:a\" />\r\n";

			Assert.AreEqual(expectedXml, condition.ToXml());
		}

		[Test]
		public void ModifiedConditionRequiringNoExpressionProducesNoExpressionInXml()
		{
			Condition condition = new Condition(blank, HybridOperator.List["equals"], new Expression("1"));
			condition.CompOp = HybridOperator.List["is not blank"];

			string expectedXml =
				"<isNotBlank field=\"Form 1:Q1:a\" />\r\n";

			Assert.AreEqual(expectedXml, condition.ToXml());
		}

	}
}
