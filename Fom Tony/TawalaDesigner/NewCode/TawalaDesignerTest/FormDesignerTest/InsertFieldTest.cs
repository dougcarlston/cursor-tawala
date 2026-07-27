using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestingSupport;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class InsertFieldUITest : UIOrientedTestBase
	{
		[SetUp]
		public void SetUp()
		{
			UITestSetUp();
		}

		[TearDown]
		public void TearDown()
		{
			UITestTearDown();
		}

		private void insertFibItemAtBeginningOfForm()
		{
			formEditPresenter.InsertFibItem(0);
		}

		private void insertFibItemAtEndOfForm()
		{
			formEditPresenter.InsertFibItem(-1);
		}

		private void insertTextItemAtEndOfForm()
		{
			formEditPresenter.InsertTextItem(-1);
		}

		private void insertField(IField field)
		{
			formEditPresenter.InsertField(field);
		}

		private void insertField(IFormField field)
		{
			formEditPresenter.InsertField(field);
		}

		private void setFocusToFibItemContents(int fibItemIndex)
		{
			HtmlElementCollection elements = document.GetElementsByTagName("FibItem");
			TestUtil.SetHtmlElementFocus(elements[fibItemIndex].FirstChild);
		}

		private void setFocusToTextItemContents(int textItemIndex)
		{
			HtmlElementCollection elements = document.GetElementsByTagName("TextItem");
			TestUtil.SetHtmlElementFocus(elements[textItemIndex].FirstChild);
		}

		private HtmlElement getBlankElement(int blankIndex)
		{
			HtmlElementCollection elements = body.GetElementsByTagName("FibItem");
			HtmlElement container = elements[0].FirstChild;
			HtmlElementCollection blankElements = container.GetElementsByTagName("Blank");

			return blankElements[blankIndex];
		}

		private HtmlElement getTextElement()
		{
			HtmlElementCollection elements = body.GetElementsByTagName("TextItem");
			return elements[0];
		}

		[Test]
		public void InsertingFieldProducesFieldElementInHtml()
		{
			insertFibItemAtEndOfForm();

			insertTextItemAtEndOfForm();
			setFocusToTextItemContents(0);

			NewFibItem fibItem = Project.Current.FormList[0].GetFormItem("Q1") as NewFibItem;
			IBlank blank = fibItem.BlankList[0];
			insertField(blank);

			HtmlElement textItemElement = getTextElement();

			string expectedHtml = "<t:field class=field";
			string actualHtml = Regex.Match(textItemElement.InnerHtml, @"<t:field class=field").Value;
			Assert.AreEqual(expectedHtml, actualHtml);
		}

		[Test]
		public void TextItemWithFieldGeneratesCorrectXml()
		{
			string formItemXml =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine +
				"<text label=\"T1\" style=\"normal\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<field name=\"Form 1:Q1:a\"/>" +
				"[Replace this with text of your own.]" +
				"</paragraph>" +
				"</text>";

			insertFibItemAtEndOfForm();

			insertTextItemAtEndOfForm();

			IFibItem fibItem = Project.Current.FormList[0].GetFormItem("Q1") as IFibItem;
			IBlank blank = fibItem.BlankList[0];

			setFocusToTextItemContents(0);
			insertField(blank);

			string projectXml = Project.Current.ToXml();

			string actualFormItemXml = Regex.Match(projectXml, "<fib .*</text>", RegexOptions.Singleline).Value;

			Assert.AreEqual(formItemXml, actualFormItemXml);
		}

		[Test]
		public void FibItemWithFieldGeneratesCorrectXml()
		{
			insertFibItemAtEndOfForm();
			insertFibItemAtEndOfForm();

			NewFibItem fibItem = Project.Current.FormList[0].GetFormItem("Q1") as NewFibItem;
			IBlank blank = fibItem.BlankList[0];

			setFocusToFibItemContents(1);
			insertField(blank);

			string expectedXml =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine +
				"<fib label=\"Q2\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<field name=\"Form 1:Q1:a\"/>" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>";

			string actualXml = Regex.Match(Project.Current.ToXml(), @"<fib.+</fib>\r\n<fib.+</fib>").Value;
	
			Assert.AreEqual(expectedXml, actualXml);
		}

		[Test]
		public void InsertingFibItemUpdatesExistingFieldName()
		{
			string formItemXml =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine +
				"<fib label=\"Q2\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine +
				"<fib label=\"Q3\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +

				"<field name=\"Form 1:Q2:a\"/>" +

				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>";

			insertFibItemAtEndOfForm();
			insertFibItemAtEndOfForm();

			IForm form = Project.Current.FormList[0];
			IFibItem fibItem = form.GetFormItem("Q1") as IFibItem;
			IBlank blank = fibItem.BlankList[0];

			setFocusToFibItemContents(1);
			insertField(blank);

			insertFibItemAtBeginningOfForm();
			Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(form));

			string projectXml = Project.Current.ToXml();

			string actualFormItemXml = Regex.Match(projectXml, "<fib .*</fib>.*</fib>.*</fib>", RegexOptions.Singleline).Value;

			Assert.AreEqual(formItemXml, actualFormItemXml);
		}
	}

	[TestFixture]
	public class InsertFieldModelTest : ModelOrientedTestBase
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

		private int getBlankCount()
		{
			HtmlElementCollection elements = body.GetElementsByTagName("FibItem");
			HtmlElement container = elements[0].FirstChild;
			HtmlElementCollection blankElements = container.GetElementsByTagName("Blank");

			return blankElements.Count;
		}

		[Test]
		public void FieldInModelProducesFieldInView()
		{
			string xmlString =
				"<fib label=\"Q1\"><blank label=\"a\" length=\"20\"/>Text<blank label=\"b\" length=\"20\"/></fib>";

			IXmlElement element = new XmlElement(xmlString, true);
			NewFibItem fibItem = new NewFibItem(element);
			form.ItemList.Add(fibItem);

			CreateViewFromForm();

			int fibItemIndex = body.InnerHtml.IndexOf("<t:FibItem");
			int blank1Index = body.InnerHtml.IndexOf("<t:Blank");
			int blank2Index = body.InnerHtml.LastIndexOf("<t:Blank");

			Assert.AreEqual(2, getBlankCount());

			Assert.IsTrue(fibItemIndex >= 0);
			Assert.IsTrue(blank1Index > fibItemIndex);
			Assert.IsTrue(blank2Index > blank1Index);
		}
	}
}
