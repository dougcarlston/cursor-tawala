using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest.Forms.FormItemContents
{
	[TestFixture]
	public class NewChoiceListTest
	{
		IFibItem fibItem;
		NewChoiceList choices;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			IForm form = Project.Current.AddForm();
			fibItem = new NewFibItem();
			form.ItemList.Add(fibItem as IFormItem);

			choices = new NewChoiceList();
			choices.Add(new NewChoice("a", "Choice One"));
			choices.Add(new NewChoice("b", "Choice <<Form 1:Q1:a>> Two"));

			ComponentMaker.UseNewComponents(true);
		}

		[TearDown]
		public void TearDown()
		{
			ComponentMaker.UseNewComponents(false);
		}

		[Test]
		public void ChoiceListYieldsXml()
		{
			string expectedXml =
				@"<choice label=""a"">" +
				@"Choice One" +
				@"</choice>" +
				@"<choice label=""b"">" +
				@"Choice <field name=""Form 1:Q1:a""/> Two" +
				@"</choice>";

			Assert.AreEqual(expectedXml, choices.ToXml());
		}

		[Test]
		public void ChoiceListYieldsXhtml()
		{
			string xhtmlFormat =
				@"<t:Choice>" +
				@"<span class=""choice"">a</span>" +
				@"<input type=""radio"" />" +
				@"<span>Choice One</span><br />" +
				@"</t:Choice>" +
				@"<t:Choice>" +
				@"<span class=""choice"">b</span>" +
				@"<input type=""radio"" />" +
				@"<span>Choice <t:field fieldID=""{0}"">Form 1:Q1:a</t:field> Two</span><br />" +
				@"</t:Choice>";

			string expectedXhtml = string.Format(xhtmlFormat, fibItem.BlankList[0].Id);

			Assert.AreEqual(expectedXhtml, choices.ToXhtml(new NewMcqItem()));
		}

		[Test]
		public void ChoiceListYieldsText()
		{
			string expectedText = "Choice One\r\nChoice <<Form 1:Q1:a>> Two";

			Assert.AreEqual(expectedText, choices.Text);
		}
	}
}
