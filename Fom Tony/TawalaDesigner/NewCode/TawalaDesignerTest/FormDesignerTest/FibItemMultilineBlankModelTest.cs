using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestingSupport;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class FibItemMultilineBlankModelTest : ModelOrientedTestBase
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
		public void MultilineBlankInModelProducesMultilineBlankInView()
		{
			string xmlString =
				"<fib label=\"Q1\"><blank label=\"a\" length=\"20\" height=\"2\"/>Text<blank label=\"b\" length=\"20\"/></fib>";

			IXmlElement element = new XmlElement(xmlString, true);
			form.ItemList.Add(new NewFibItem(element));

			CreateViewFromForm();

			string expectedHtml = "style=\"HEIGHT: 42px\"";
			string actualHtml = Regex.Match(body.InnerHtml, @"style=""HEIGHT: \d+px""").Value;

			Assert.AreEqual(expectedHtml, actualHtml);
		}

		[Test]
		public void MultilineBlankInFibProducesMultilineBlankInXml()
		{
			string xmlString =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Name: " +
				"<blank label=\"a\" length=\"20\" height=\"2\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>";

			IXmlElement element = new XmlElement(xmlString, true);
			form.ItemList.Add(new NewFibItem(element));

			CreateViewFromForm();

			string projectXml = Project.Current.ToXml();

			string actualXml = Regex.Match(projectXml,  @"<fib.*</fib>").Value;

			Assert.AreEqual(xmlString, actualXml);
		}
	}
}
