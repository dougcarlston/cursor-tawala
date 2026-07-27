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
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class FibItemInsertBlankUITest : UIOrientedTestBase
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

		private void insertFibItemAtEndOfForm()
		{
			formEditPresenter.InsertFibItem(-1);
		}

		private void insertBlankAtBeginningOfFibItem()
		{
			setFocusToFibItemContents();
			NewFibItem fibItem = Project.Current.FormList[0].ItemList[0] as NewFibItem;
			formEditPresenter.InsertBlank(fibItem.Id.ToString());
		}

		private void setFocusToFibItemContents()
		{
			HtmlElementCollection elements = document.GetElementsByTagName("FibItem");
			HtmlElement container = elements[0].FirstChild;
			TestUtil.SetHtmlElementFocus(container);
		}

		private HtmlElement getBlankElement(int blankIndex)
		{
			HtmlElementCollection elements = body.GetElementsByTagName("FibItem");
			HtmlElement container = elements[0].FirstChild;
			HtmlElementCollection blankElements = container.GetElementsByTagName("Blank");

			return blankElements[blankIndex];
		}

		private int getBlankCount()
		{
			HtmlElementCollection elements = body.GetElementsByTagName("FibItem");
			HtmlElement container = elements[0].FirstChild;
			HtmlElementCollection blankElements = container.GetElementsByTagName("Blank");

			return blankElements.Count;
		}

		[Test]
		public void FibItemCanHaveMultipleBlanks()
		{
			insertFibItemAtEndOfForm();

			insertBlankAtBeginningOfFibItem();

			int fibItemIndex = body.InnerHtml.IndexOf("<t:FibItem");
			int blank1Index = body.InnerHtml.IndexOf("<t:Blank");
			int blank2Index = body.InnerHtml.LastIndexOf("<t:Blank");

			Assert.IsTrue(fibItemIndex >= 0);
			Assert.IsTrue(blank1Index > fibItemIndex);
			Assert.IsTrue(blank2Index > blank1Index);
		}

		[Test]
		public void BlankHasOnlyOneInputElement()
		{
			insertFibItemAtEndOfForm();

			HtmlElementCollection inputElements = body.GetElementsByTagName("INPUT");

			Assert.AreEqual(1, inputElements.Count);
		}

		[Test]
		[Ignore("Revisit: Why does default INPUT element size not appear, but non-default sizes do? - SB 02/02/2008")]
		public void InputElementHasDefaultSizeAttribute()
		{
			insertFibItemAtEndOfForm();

			HtmlElementCollection inputElements = body.GetElementsByTagName("INPUT");

			Assert.IsTrue(inputElements[0].OuterHtml.Contains("size=20"));
		}

		[Test]
		public void InsertingBlankUpdatesBlankLabels()
		{
			insertFibItemAtEndOfForm();

			Assert.AreEqual(1, getBlankCount());

			HtmlElement blankElement = getBlankElement(0);
			HtmlElement inputElement = blankElement.FirstChild;

			Assert.AreEqual("Q1:a", inputElement.GetAttribute("value"));

			insertBlankAtBeginningOfFibItem();

			Assert.AreEqual(2, getBlankCount());

			HtmlElement blank1Element = getBlankElement(0);
			HtmlElement input1Element = blank1Element.FirstChild;

			HtmlElement blank2Element = getBlankElement(1);
			HtmlElement input2Element = blank2Element.FirstChild;

			Assert.AreEqual("Q1:a", input1Element.GetAttribute("value"));
			Assert.AreEqual("Q1:b", input2Element.GetAttribute("value"));
		}

		[Test]
		public void DefaultFibItemGeneratesCorrectXml()
		{
			insertFibItemAtEndOfForm();

			string fibProjectXml = Regex.Match(Project.Current.ToXml(), @"<fib.+</fib>").Value;

			string expectedXml =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>";

			Assert.AreEqual(expectedXml, fibProjectXml);
		}

		[Test]
		public void FibItemWithTwoBlanksGeneratesCorrectXml()
		{
			insertFibItemAtEndOfForm();
			insertBlankAtBeginningOfFibItem();

			string expectedXml =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"b\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>";

			string actualXml = Regex.Match(Project.Current.ToXml(), @"<fib.+</fib>").Value;

			Assert.AreEqual(expectedXml, actualXml);
		}

	}

	[TestFixture]
	public class FibItemInsertBlankModelTest : ModelOrientedTestBase
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
		public void TwoBlanksInModelProduceTwoBlanksInView()
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

		[Test]
		public void TwoParagraphsInFibProduceTwoParagraphsInXml()
		{
			string fibItemXml =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Name: " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Email: " +
				"<blank label=\"b\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>";

			IXmlElement element = new XmlElement(fibItemXml, true);
			IFibItem fibItem = new NewFibItem(element);
			form.ItemList.Add(fibItem as IFormItem);

			CreateViewFromForm();

			string projectXml = Project.Current.ToXml();

			Assert.IsTrue(projectXml.Contains(fibItemXml));
		}
	}
}
