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
	public class ConditionalDisplayOfTextItems3003
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		private static readonly string textItemXmlWithConditions =
			@"<text label=""T1"" style=""normal"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"Text" +
			@"</paragraph>" +
			@"<displayConditions>" + Environment.NewLine +
			@"<equals field=""var"">" + Environment.NewLine +
			@"<string value=""foo""/>" + Environment.NewLine +
			@"</equals>" + Environment.NewLine +
			@"</displayConditions>" +
			@"</text>" + Environment.NewLine;

		private static readonly string textItemXmlWithoutConditions =
			@"<text label=""T1"" style=""normal"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"Text" +
			@"</paragraph>" +
			@"</text>" + Environment.NewLine;

		private static NewTextItem createTextItemItemWithDisplayConditions()
		{
			var textItem = new NewTextItem(new XmlElement(textItemXmlWithoutConditions));

			IField variable = new Variable("var");
			var conditions = new Conditions(new Condition(variable, HybridOperator.List[HybridOperator.Ops.equals], new Expression("foo")));
			textItem.DisplayConditions = conditions;

			return textItem;
		}

		private static void removeDisplayConditions(IFormItem formItem)
		{
			formItem.DisplayConditions = new Conditions();
		}

		[Test]
		public void TextItemWithDisplayConditionsGeneratesExpectedXml()
		{
			var form = Project.Current.AddForm();
			var textItem = createTextItemItemWithDisplayConditions();
			form.ItemList.Add(textItem);

			Assert.IsTrue(textItem.HasDisplayConditions);
			Assert.AreEqual(textItemXmlWithConditions, textItem.ToXml("T1"));
		}

		[Test]
		public void TextItemWithDisplayConditionsRemovedGeneratesExpectedXml()
		{
			var form = Project.Current.AddForm();
			var textItem = createTextItemItemWithDisplayConditions();
			form.ItemList.Add(textItem);

			removeDisplayConditions(textItem);

			Assert.IsFalse(textItem.HasDisplayConditions);
			Assert.AreEqual(textItemXmlWithoutConditions, textItem.ToXml("T1"));
		}

		[Test]
		public void TextItemWithDisplayConditionsIsConstructedFromXml()
		{
			var textItem = new NewTextItem(new XmlElement(textItemXmlWithConditions));

			Assert.IsTrue(textItem.HasDisplayConditions);

			var conditions = textItem.DisplayConditions;
			Assert.AreEqual("var equals \"foo\"", conditions.ToString());
		}
	}
}
