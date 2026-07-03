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
	public class NewChoiceTest
	{
		IFibItem fibItem;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			IForm form = Project.Current.AddForm();
			fibItem = new NewFibItem();
			form.ItemList.Add(fibItem as IFormItem);
		}

		[Test]
		public void TextPropertyOfXmlChoiceWithTextOnlyReturnsText()
		{
			string xmlString =
				@"<choice label=""a"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"Choice One" +
				@"</font>" +
				@"</choice>";

			NewChoice choice = new NewXmlChoice(new XmlElement(xmlString));

			Assert.AreEqual("Choice One", choice.Text);
		}

		[Test]
		public void TextPropertyOfXmlChoiceWithFieldOnlyReturnsFieldString()
		{
			string xmlString =
				@"<choice label=""a"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"</font>" +
				@"</choice>";

			NewChoice choice = new NewXmlChoice(new XmlElement(xmlString));

			Assert.AreEqual("<<Form 1:Q1:a>>", choice.Text);
		}

		[Test]
		public void TextPropertyOfXmlChoiceWithTextAndFieldReturnsTextAndFieldString()
		{
			string xmlString =
				@"<choice label=""a"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"Choice " +
				@"<field name=""Form 1:Q1:a""/>" +
				@" One" +
				@"</font>" +
				@"</choice>";

			NewChoice choice = new NewXmlChoice(new XmlElement(xmlString));

			Assert.AreEqual("Choice <<Form 1:Q1:a>> One", choice.Text);
		}

		[Test]
		public void ChoiceXmlWithFontProducesChoiceXmlWithFont()
		{
			string xmlString =
				@"<choice label=""a"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"Choice One" +
				@"</font>" +
				@"</choice>";

			NewChoice choice = new NewXmlChoice(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, choice.ToXml());
		}

		[Test]
		public void ChoiceXmlWithoutFontProducesChoiceXmlWithoutFont()
		{
			string xmlString =
				@"<choice label=""a"">" +
				@"Choice One" +
				@"</choice>";

			NewChoice choice = new NewXmlChoice(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, choice.ToXml());
		}

		[Test]
		public void ChoiceXmlWithEmptyParagraphProducesChoiceXmlWithoutParagraph()
		{
			string xmlStringWithParagraph =
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"</paragraph>" +
				@"</choice>";

			NewChoice choice = new NewXmlChoice(new XmlElement(xmlStringWithParagraph));

			string xmlStringWithoutParagraph =
				@"<choice label=""a"">" +
				@"</choice>";

			Assert.AreEqual(xmlStringWithoutParagraph, choice.ToXml());
		}

		[Test]
		public void ChoiceXmlWithParagraphProducesChoiceXmlWithoutParagraph()
		{
			string xmlStringWithParagraph =
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"Choice Text" +
				@"</paragraph>" +
				@"</choice>";

			NewChoice choice = new NewXmlChoice(new XmlElement(xmlStringWithParagraph));

			string xmlStringWithoutParagraph =
				@"<choice label=""a"">" +
				@"Choice Text" +
				@"</choice>";

			Assert.AreEqual(xmlStringWithoutParagraph, choice.ToXml());
		}

		[Test]
		public void TextPropertyOfXhtmlChoiceWithTextOnlyReturnsText()
		{
			string htmlString =
				"<t:Choice><SPAN class=choice>a</SPAN><INPUT type=radio><SPAN>Choice One</SPAN></t:Choice>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			NewChoice choice = new NewXhtmlChoice(new XhtmlElement(htmlString, true));

			Assert.AreEqual("Choice One", choice.Text);
		}

		[Test]
		public void TextPropertyOfXhtmlChoiceWithFieldOnlyReturnsFieldString()
		{
			string htmlString =
				"<t:Choice>" +
				"<SPAN class=choice>a</SPAN>" +
				"<INPUT type=radio>" +
				"<SPAN><t:field class=field id=\"\" contentEditable=\"false\" fieldID=\"1014\">&lt;&lt;Form 1:Q1:a&gt;&gt;</t:field></SPAN>" +
				"</t:Choice>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			NewChoice choice = new NewXhtmlChoice(new XhtmlElement(htmlString, true));

			Assert.AreEqual("<<Form 1:Q1:a>>", choice.Text);
		}

		[Test]
		public void TextPropertyOfXhtmlChoiceWithTextAndFieldReturnsTextAndFieldString()
		{
			string htmlString =
				"<t:Choice>" +
				"<SPAN class=choice>a</SPAN>" +
				"<INPUT type=radio>" +
				"<SPAN>Choice <t:field class=field id=\"\" contentEditable=\"false\" fieldID=\"1014\">&lt;&lt;Form 1:Q1:a&gt;&gt;</t:field> One</SPAN>" +
				"</t:Choice>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			NewChoice choice = new NewXhtmlChoice(new XhtmlElement(htmlString, true));

			Assert.AreEqual("Choice <<Form 1:Q1:a>> One", choice.Text);
		}

		[Test]
		public void ConstructingWithLabelAndTextProducesNonNullContents()
		{
			NewChoice choice = new NewChoice("a", "Choice One");

			Assert.IsNotNull(choice.Contents);
		}

		[Test]
		public void ConstructingWithLabelAndTextStringsProducesCorrectXml()
		{
			NewChoice choice = new NewChoice("a", "Choice One");

			string xmlString =
				@"<choice label=""a"">" +
				@"Choice One" +
				@"</choice>";

			Assert.AreEqual(xmlString, choice.ToXml());
		}

		[Test]
		public void ConstructingWithLabelAndFieldStringsProducesCorrectXml()
		{
			NewChoice choice = new NewChoice("a", "<<Form 1:Q1:a>>");

			string xmlString =
				@"<choice label=""a"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"</choice>";

			Assert.AreEqual(xmlString, choice.ToXml());
		}

		[Test]
		public void ConstructingWithLabelAndTextAndFieldStringsProducesCorrectXml()
		{
			NewChoice choice = new NewChoice("a", "Choice <<Form 1:Q1:a>> One");

			string xmlString =
				@"<choice label=""a"">" +
				@"Choice " +
				@"<field name=""Form 1:Q1:a""/>" +
				@" One" +
				@"</choice>";

			Assert.AreEqual(xmlString, choice.ToXml());
		}

		[Test]
		public void ConstructingWithLabelAndTextAndFieldXmlProducesCorrectXml()
		{
			string xmlString =
				@"<choice label=""a"">" +
				@"Choice " +
				@"<field name=""Form 1:Q1:a""/>" +
				@" One" +
				@"</choice>";

			NewChoice choice = new NewXmlChoice(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, choice.ToXml());
		}

		[Test]
		public void ConstructingWithLabelAndTextAndFieldXmlProducesCorrectXhtml()
		{
			string xmlString =
				@"<choice label=""a"">" +
				@"Choice " +
				@"<field name=""Form 1:Q1:a""/>" +
				@" One" +
				@"</choice>";

			NewChoice choice = new NewXmlChoice(new XmlElement(xmlString));

			string xhtmlFormatString =
				@"<t:Choice>" +
				@"<span class=""choice"">a</span>" +
				@"<input type=""radio"" />" +
				@"<span>Choice <t:field fieldID=""{0}"">Form 1:Q1:a</t:field> One</span><br />" +
				@"</t:Choice>";

			Assert.AreEqual(string.Format(xhtmlFormatString, fibItem.BlankList[0].Id), choice.ToXhtml(new NewMcqItem()));
		}
	}
}
