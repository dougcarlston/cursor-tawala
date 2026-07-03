using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.FormDesignerTest.StoryTest
{
	[TestFixture]
	public class ConditionalDisplayOfFIBs3002
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		private static readonly string fibXmlWithConditions =
			@"<fib label=""Q1"" style=""topLabels"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"<font face=""Arial"" size=""180"" color=""000000"">Name: </font><blank label=""a"" length=""10"" height=""1"" required=""false""/>" +
			@"</paragraph>" +
			@"<displayConditions>" + Environment.NewLine +
			@"<equals field=""var"">" + Environment.NewLine +
			@"<string value=""foo""/>" + Environment.NewLine +
			@"</equals>" + Environment.NewLine +
			@"</displayConditions>" +
			@"</fib>" + Environment.NewLine;

		private static readonly string fibXmlWithoutConditions =
			@"<fib label=""Q1"" style=""topLabels"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"<font face=""Arial"" size=""180"" color=""000000"">Name: </font><blank label=""a"" length=""10"" height=""1"" required=""false""/>" +
			@"</paragraph>" +
			@"</fib>" + Environment.NewLine;
        
		private static NewFibItem createFIBItemWithDisplayConditions()
		{
			var fibItem = new NewFibItem(new XmlElement(fibXmlWithoutConditions));

			IField variable = new Variable("var");
			var conditions = new Conditions(new Condition(variable, HybridOperator.List[HybridOperator.Ops.equals], new Expression("foo")));
			fibItem.DisplayConditions = conditions;

			return fibItem;
		}

		private static void removeDisplayConditions(IFormItem formItem)
		{
			formItem.DisplayConditions = new Conditions();
		}

		[Test]
		public void FibItemWithDisplayConditionsGeneratesExpectedXml()
		{
			var form = Project.Current.AddForm();
			var fibItem = createFIBItemWithDisplayConditions();
			form.ItemList.Add(fibItem);

			Assert.IsTrue(fibItem.HasDisplayConditions);
			Assert.AreEqual(fibXmlWithConditions, fibItem.ToXml("Q1"));
		}

		[Test]
		public void FibItemWithDisplayConditionsRemovedGeneratesExpectedXml()
		{
			var form = Project.Current.AddForm();
			var fibItem = createFIBItemWithDisplayConditions();
			form.ItemList.Add(fibItem);

			removeDisplayConditions(fibItem);

			Assert.IsFalse(fibItem.HasDisplayConditions);
			Assert.AreEqual(fibXmlWithoutConditions, fibItem.ToXml("Q1"));
		}

		[Test]
		public void FibItemWithDisplayConditionsIsConstructedFromXml()
		{
			var fibItem = new NewFibItem(new XmlElement(fibXmlWithConditions));

			Assert.IsTrue(fibItem.HasDisplayConditions);

			var conditions = fibItem.DisplayConditions;
			Assert.AreEqual("var equals \"foo\"", conditions.ToString());
		}
	}
}
