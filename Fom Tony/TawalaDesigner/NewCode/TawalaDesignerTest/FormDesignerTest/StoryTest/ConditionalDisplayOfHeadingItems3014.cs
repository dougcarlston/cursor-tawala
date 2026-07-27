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
	public class ConditionalDisplayOfHeadingItems3014
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		private static readonly string headingItemXmlWithConditions =
			@"<heading label=""H1"" type=""Main"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"<font face=""Arial"" size=""360"" color=""000000"">Heading</font>" +
			@"</paragraph>" +
			@"<displayConditions>" + Environment.NewLine +
			@"<equals field=""var"">" + Environment.NewLine +
			@"<string value=""foo""/>" + Environment.NewLine +
			@"</equals>" + Environment.NewLine +
			@"</displayConditions>" +
			@"</heading>" + Environment.NewLine;

		private static readonly string headingItemXmlWithoutConditions =
			@"<heading label=""H1"" type=""Main"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"<font face=""Arial"" size=""360"" color=""000000"">Heading</font>" +
			@"</paragraph>" +
			@"</heading>" + Environment.NewLine;

		private static NewHeadingItem createHeadingItemItemWithDisplayConditions()
		{
			var headingItem = new NewHeadingItem(new XmlElement(headingItemXmlWithoutConditions));

			IField variable = new Variable("var");
			var conditions = new Conditions(new Condition(variable, HybridOperator.List[HybridOperator.Ops.equals], new Expression("foo")));
			headingItem.DisplayConditions = conditions;

			return headingItem;
		}

		private static void removeDisplayConditions(IFormItem formItem)
		{
			formItem.DisplayConditions = new Conditions();
		}

		[Test]
		public void HeadingItemWithDisplayConditionsGeneratesExpectedXml()
		{
			var form = Project.Current.AddForm();
			var headingItem = createHeadingItemItemWithDisplayConditions();
			form.ItemList.Add(headingItem);

			Assert.IsTrue(headingItem.HasDisplayConditions);
			Assert.AreEqual(headingItemXmlWithConditions, headingItem.ToXml("H1"));
		}

		[Test]
		public void HeadingItemWithDisplayConditionsRemovedGeneratesExpectedXml()
		{
			var form = Project.Current.AddForm();
			var headingItem = createHeadingItemItemWithDisplayConditions();
			form.ItemList.Add(headingItem);

			removeDisplayConditions(headingItem);

			Assert.IsFalse(headingItem.HasDisplayConditions);
			Assert.AreEqual(headingItemXmlWithoutConditions, headingItem.ToXml("H1"));
		}

		[Test]
		public void HeadingItemWithDisplayConditionsIsConstructedFromXml()
		{
			var headingItem = new NewHeadingItem(new XmlElement(headingItemXmlWithConditions));

			Assert.IsTrue(headingItem.HasDisplayConditions);

			var conditions = headingItem.DisplayConditions;
			Assert.AreEqual("var equals \"foo\"", conditions.ToString());
		}
	}
}
