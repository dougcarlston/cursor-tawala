using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NUnit.Framework;
using Tawala.FormDesigner;
using TawalaTest.TestSupport;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest.ModelTest
{
	[TestFixture]
	public class McqItemTest
	{
		private IForm form;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();

			form = Project.Current.AddForm();
		}

		[Test]
		public void CanConvertFieldStringToExpression()
		{
			NonArithmeticExpression expression = new NonArithmeticExpression("Text<<Form 1:Q1:a>>Text");

			Assert.AreEqual("Text", expression.Elements[0].ToXml());
			Assert.AreEqual("<field name=\"Form 1:Q1:a\"/>", expression.Elements[1].ToXml());
			Assert.AreEqual("Text", expression.Elements[2].ToXml());
		}

		[Test]
		public void CanConstructChoiceWithFieldFromExpressionString()
		{
			form.ItemList.Add(new NewFibItem());

			string xmlString =
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"</font>" +
				@"</paragraph>" +
				@"</choice>";
				
			Choice choice = new Choice("<<Form 1:Q1:a>>");

			Assert.AreEqual(xmlString, choice.ToXml("a"));
			Assert.AreEqual("Form 1:Q1:a", choice.QualifiedFieldName);
		}

		[Test]
		public void CanConstructChoiceWithFieldFromXml()
		{
			form.ItemList.Add(new NewFibItem());

			string xmlString =
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"</font>" +
				@"</paragraph>" +
				@"</choice>";

			Choice choice = new Choice(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, choice.ToXml("a"));
		}

		[Test]
		public void CanConstructChoiceWithTextFromExpressionString()
		{
			form.ItemList.Add(new NewFibItem());

			string xmlString =
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"Text" +
				@"</font>" +
				@"</paragraph>" +
				@"</choice>";

			Choice choice = new Choice("Text");

			Assert.AreEqual(xmlString, choice.ToXml("a"));
			Assert.AreEqual("Text", choice.QualifiedFieldName);
		}

		[Test]
		public void CanConstructChoiceWithTextFromXml()
		{
			form.ItemList.Add(new NewFibItem());

			string xmlString =
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"Text" +
				@"</font>" +
				@"</paragraph>" +
				@"</choice>";

			Choice choice = new Choice(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, choice.ToXml("a"));
		}

		[Test]
		public void CanConstructChoiceWithFieldAndTextFromExpressionString()
		{
			form.ItemList.Add(new NewFibItem());

			string xmlString =
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"Text" +
				@"</font>" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"</font>" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"Text" +
				@"</font>" +
				@"</paragraph>" +
				@"</choice>";

			Choice choice = new Choice("Text<<Form 1:Q1:a>>Text");

			Assert.AreEqual(xmlString, choice.ToXml("a"));
		}

		[Test]
		public void CanConstructChoiceWithFieldAndTextFromXml()
		{
			form.ItemList.Add(new NewFibItem());

			string xmlString =
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"Text" +
				@"</font>" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"</font>" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"Text" +
				@"</font>" +
				@"</paragraph>" +
				@"</choice>";

			Choice choice = new Choice(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, choice.ToXml("a"));
		}
	}
}
