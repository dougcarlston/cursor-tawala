using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NUnit.Framework;
using Tawala.Projects.Forms.FormItemContents;
using System.Windows.Forms;
using TawalaTest.TestingSupport;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class McqItemContentsConversionTest : ModelOrientedTestBase
	{
		[SetUp]
		public void SetUp()
		{
			ModelTestSetUp();
		}

		[TearDown]
		public void TearDown()
		{
			ModelTestTearDown();
		}

		[Test]
		public void CanConvertQuestionContentsToXhtml()
		{
			string xmlString = "<container><question><paragraph indent=\"0\" align=\"left\">Question</paragraph></question></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			Assert.AreEqual("<question><p style=\"margin-left: 0pt\" align=\"left\">Question</p></question>", formItemContentsConverter.ToXhtml(FormItem.NULL));
		}

		[Test]
		public void CanConvertChoiceContentsToXhtml()
		{
			string xmlString = "<container><choice label=\"a\">Choice one</choice></container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			IMcqItem mcItem = new NewMcqItem();

			Assert.AreEqual("<t:Choice><span class=\"choice\">a</span><input type=\"radio\" /><span>Choice one</span><br /></t:Choice>", formItemContentsConverter.ToXhtml(mcItem));
		}

		[Test]
		public void CanConvertMcqContentsToXml()
		{
			string htmlString =
				"<container>" +
				"<question><P style=\"MARGIN-LEFT: 0pt\">Question</P></question>" +
				"<t:Choice><SPAN class=choice>a</SPAN><INPUT type=radio><SPAN>Choice one</SPAN></t:Choice>" +
				"</container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			string expectedXml =
				"<container>" +
				"<question><paragraph indent=\"0\" align=\"left\">Question</paragraph></question>" +
				"<choice label=\"a\">Choice one</choice>" +
				"</container>";

			Assert.AreEqual(expectedXml, formItemContentsConverter.ToXml());
		}

        [Ignore("REQUIRES Fix-ups for new classes")]
        [Test]
		public void CanConvertMcqContentsWithFieldInQuestionToXml()
		{
#if FIXED
            form.ItemList.Add(new FibItem());

			string htmlString =
				"<container>" +
				"<question><P style=\"MARGIN-LEFT: 0pt\">" +
				"<t:field class=field id=\"\" contentEditable=\"false\" fieldID=\"1014\">&lt;&lt;Form 1:Q1:a&gt;&gt;</t:field>" +
				"</P></question>" +
				"<t:Choice><SPAN class=choice>a</SPAN><INPUT type=radio><SPAN>" +
				"Choice One" +
				"</SPAN></t:Choice>" +
				"</container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			string expectedXml =
				"<container>" +
				"<question><paragraph indent=\"0\" align=\"left\"><field name=\"Form 1:Q1:a\"/></paragraph></question>" +
				"<choice label=\"a\">Choice One</choice>" +
				"</container>";

			Assert.AreEqual(expectedXml, formItemContentsConverter.ToXml());
#endif
        }

        [Ignore("REQUIRES Fix-ups for new classes")]
        [Test]
		public void CanConvertMcqContentsWithFieldInChoiceToXml()
		{
#if FIXED
			form.ItemList.Add(new FibItem());

			string htmlString =
				"<container>" +
				"<question><P style=\"MARGIN-LEFT: 0pt\">" +
				"Question" +
				"</P></question>" +
				"<t:Choice><SPAN class=choice>a</SPAN><INPUT type=radio><SPAN>" +
				"<t:field class=field id=\"\" contentEditable=\"false\" fieldID=\"1014\">&lt;&lt;Form 1:Q1:a&gt;&gt;</t:field>" +
				"</SPAN></t:Choice>" +
				"</container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			string expectedXml =
				"<container>" +
				"<question><paragraph indent=\"0\" align=\"left\">Question</paragraph></question>" +
				"<choice label=\"a\"><field name=\"Form 1:Q1:a\"/></choice>" +
				"</container>";

			Assert.AreEqual(expectedXml, formItemContentsConverter.ToXml());
#endif
        }

		[Test]
		public void CanConvertMcqContentsWithFieldInChoiceToXhtml()
		{
			IFibItem fibItem = new NewFibItem();
			form.ItemList.Add(fibItem);

			string xmlString =
				"<container>" +
				"<question>" +
				"<paragraph indent=\"0\" align=\"left\">Question</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"<field name=\"Form 1:Q1:a\"/>" +
				"</choice>" +
				"</container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			string xhtmlFormat =
				"<question>" +
				"<p style=\"margin-left: 0pt\" align=\"left\">" +
				"Question" +
				"</p>" +
				"</question>" +
				"<t:Choice>" +
				"<span class=\"choice\">a</span>" +
				"<input type=\"radio\" />" +
				"<span>" +
				"<t:field fieldID=\"{0}\">Form 1:Q1:a</t:field>" +
				"</span><br />" +
				"</t:Choice>";

			int fieldId = fibItem.BlankList[0].Id;
			string expectedXhtml = String.Format(xhtmlFormat, fieldId);

			IMcqItem mcItem = new NewMcqItem();

			Assert.AreEqual(expectedXhtml, formItemContentsConverter.ToXhtml(mcItem));
		}

		[Test]
		public void CanConvertMcqContentsWithTextAndFieldInChoiceToXml()
		{
			form.ItemList.Add(new NewFibItem());

			string htmlString =
				"<container>" +
				"<question><P style=\"MARGIN-LEFT: 0pt\">" +
				"Question" +
				"</P></question>" +
				"<t:Choice>" +
				"<SPAN class=choice>a</SPAN><INPUT type=radio>" +
				"<SPAN>" +
				"Text" +
				"<t:field class=field id=\"\" contentEditable=\"false\" fieldID=\"1014\">&lt;&lt;Form 1:Q1:a&gt;&gt;</t:field>" +
				"</SPAN>" +
				"</t:Choice>" +
				"</container>";

			IXhtmlElement xhtmlElement = new XhtmlElement(htmlString, true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xhtmlElement);

			string expectedXml =
				"<container>" +
				"<question><paragraph indent=\"0\" align=\"left\">Question</paragraph></question>" +
				"<choice label=\"a\">Text<field name=\"Form 1:Q1:a\"/></choice>" +
				"</container>";

			Assert.AreEqual(expectedXml, formItemContentsConverter.ToXml());
		}

		[Test]
		public void CanConvertMcqContentsWithTextAndFieldInChoiceToXhtml()
		{
			IFibItem fibItem = new NewFibItem();
			form.ItemList.Add(fibItem as IFormItem);

			string xmlString =
				"<container>" +
				"<question>" +
				"<paragraph indent=\"0\" align=\"left\">Question</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"Text" +
				"<field name=\"Form 1:Q1:a\"/>" +
				"</choice>" +
				"</container>";

			IXmlElement xmlElement = new XmlElement((xmlString), true);
			FormItemContentsConverter formItemContentsConverter = new FormItemContentsConverter(xmlElement);

			string xhtmlFormat =
				"<question>" +
				"<p style=\"margin-left: 0pt\" align=\"left\">" +
				"Question" +
				"</p>" +
				"</question>" +
				"<t:Choice>" +
				"<span class=\"choice\">a</span>" +
				"<input type=\"radio\" />" +
				"<span>" +
				"Text" +
				"<t:field fieldID=\"{0}\">Form 1:Q1:a</t:field>" +
				"</span><br />" +
				"</t:Choice>";

			string expectedXhtml = String.Format(xhtmlFormat, fibItem.BlankList[0].Id);

			Assert.AreEqual(expectedXhtml, formItemContentsConverter.ToXhtml(new NewMcqItem()));
		}

	}
}
