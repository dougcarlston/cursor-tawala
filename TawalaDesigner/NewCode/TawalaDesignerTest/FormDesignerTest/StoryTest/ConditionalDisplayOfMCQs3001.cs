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
	public class ConditionalDisplayOfMCQs3001
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		private static readonly string mcqXmlWithConditions =
			@"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
			@"<question>" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">Favorite fruit:</font>" +
			@"</paragraph>" +
			@"</question>" +
			@"<choice label=""a"">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">Apples</font>" +
			@"</choice>" +
			@"<choice label=""b"">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">Oranges</font>" +
			@"</choice>" +
			@"<displayConditions>" + Environment.NewLine +
			@"<equals field=""var"">" + Environment.NewLine +
			@"<string value=""foo""/>" + Environment.NewLine +
			@"</equals>" + Environment.NewLine +
			@"</displayConditions>" +
			@"</mc>" + Environment.NewLine;

		private static readonly string mcqXmlWithoutConditions =
			@"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
			@"<question>" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">Favorite fruit:</font>" +
			@"</paragraph>" +
			@"</question>" +
			@"<choice label=""a"">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">Apples</font>" +
			@"</choice>" +
			@"<choice label=""b"">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">Oranges</font>" +
			@"</choice>" +
			@"</mc>" + Environment.NewLine;

		private static NewMcqItem createMcqItemWithDisplayConditions()
		{
			var mcqItem = new NewMcqItem(new XmlElement(mcqXmlWithoutConditions));

			IField variable = new Variable("var");
			var conditions = new Conditions(new Condition(variable, HybridOperator.List[HybridOperator.Ops.equals], new Expression("foo")));
			mcqItem.DisplayConditions = conditions;

			return mcqItem;
		}

		private static void removeDisplayConditions(IFormItem formItem)
		{
			formItem.DisplayConditions = new Conditions();
		}

		[Test]
		public void McqItemWithDisplayConditionsGeneratesExpectedXml()
		{
			var form = Project.Current.AddForm();
			var mcqItem = createMcqItemWithDisplayConditions();
			form.ItemList.Add(mcqItem);

			Assert.IsTrue(mcqItem.HasDisplayConditions);
			Assert.AreEqual(mcqXmlWithConditions, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void McqItemWithDisplayConditionsRemovedGeneratesExpectedXml()
		{
			var form = Project.Current.AddForm();
			var mcqItem = createMcqItemWithDisplayConditions();
			form.ItemList.Add(mcqItem);

			removeDisplayConditions(mcqItem);

			Assert.IsFalse(mcqItem.HasDisplayConditions);
			Assert.AreEqual(mcqXmlWithoutConditions, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void McqItemWithDisplayConditionsIsConstructedFromXml()
		{
			var mcqItem = new NewMcqItem(new XmlElement(mcqXmlWithConditions));

			Assert.IsTrue(mcqItem.HasDisplayConditions);

			var conditions = mcqItem.DisplayConditions;
			Assert.AreEqual("var equals \"foo\"", conditions.ToString());
		}
	}
}
