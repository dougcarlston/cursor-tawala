using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.FormDesigner;
using Tawala.Proj.Forms.FormItemContents;
using System.Windows.Forms;
using TawalaTest.TestSupport;
using Tawala.Browser;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class TextItemUITest : UIOrientedTestBase
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

		[Test]
		public void AppendTextItem()
		{
			Assert.IsFalse(body.InnerHtml.Contains("<TD"));

			formEditPresenter.InsertTextItem(FormPresenter.InsertAtEnd);

			VerifyItemTag("TextItem");
			Assert.IsNotNull(GetFormItem<ITextItem>());
		}

		[Test]
		public void InsertTextItem()
		{
			formEditPresenter.InsertBreakItem(FormPresenter.InsertAtEnd);
			formEditPresenter.InsertBreakItem(FormPresenter.InsertAtEnd);
			formEditPresenter.InsertTextItem(1);
			
			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(ITextItem), Project.Current.FormList[0].ItemList[1]);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int textItemIndex = body.InnerHtml.IndexOf("t:TextItem");
			int breakItem2Index = body.InnerHtml.LastIndexOf("t:BreakItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(textItemIndex > breakItem1Index);
			Assert.IsTrue(textItemIndex < breakItem2Index);
		}

		[Test]
		public void InsertedTextItemContainsDefaultText()
		{
			formEditPresenter.InsertTextItem(FormPresenter.InsertAtEnd);

			Assert.IsTrue(body.InnerHtml.Contains("[Replace this with text of your own.]"));
		}

		[Test]
		public void InsertingTextItemAtIndexOfNegativeOneAppends()
		{
			formEditPresenter.InsertBreakItem(FormPresenter.InsertAtEnd);
			formEditPresenter.InsertTextItem(FormPresenter.InsertAtEnd);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int textItemIndex = body.InnerHtml.IndexOf("t:TextItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(textItemIndex > breakItem1Index);
		}

		[Test]
		public void InsertedTextItemsHaveCorrectDefaultLabels()
		{
			formEditPresenter.InsertTextItem(FormPresenter.InsertAtEnd);
			formEditPresenter.InsertTextItem(0);

			int textItem1Label = body.InnerHtml.IndexOf("T2</DIV>");
			int textItem2Label = body.InnerHtml.IndexOf("T1</DIV>");

			Assert.IsTrue(textItem1Label > 0);
			Assert.IsTrue(textItem2Label > 0);
			Assert.IsTrue(textItem1Label > textItem2Label);
		}

		[Test]
		public void InsertTableInTextItem()
		{
			formEditPresenter.InsertTextItem(FormPresenter.InsertAtEnd);
			HtmlElement div = getTextItemDiv();

			TestUtil.SetHtmlElementFocus(div);

            ((BrowserControl)browser).InsertTable(216, 2, 2);

			TestUtil.PumpMessages();

			string innerHtml = div.InnerHtml;
			Assert.IsTrue(innerHtml.Contains("<TABLE"));
		}

		[Test]
		public void InsertedTableAppearsInXml()
		{
			formEditPresenter.InsertTextItem(FormPresenter.InsertAtEnd);
			HtmlElement div = getTextItemDiv();

			TestUtil.SetHtmlElementFocus(div);


            ((BrowserControl)browser).InsertTable(216, 2, 2);
            TestUtil.PumpMessages();

			string projectXml = Project.Current.ToXml();

			Assert.IsTrue(projectXml.Contains("<table "));
		}


		[Test]
		public void InsertedTableInTextItemAppearsInModel()
		{
			formEditPresenter.InsertTextItem(FormPresenter.InsertAtEnd);

			HtmlElementCollection tags = body.GetElementsByTagName("TextItem");
			Assert.AreEqual(1, tags.Count);
			HtmlElement element = tags[0];
			HtmlElement div = element.Children[0];

			TestUtil.SetHtmlElementFocus(div);

            ((BrowserControl)browser).InsertTable(216, 2, 2);
            TestUtil.PumpMessages();

			Project.Current.ToXml();

			ITextItem textItem = Project.Current.FormList[0].ItemList[0] as ITextItem;
			Assert.IsTrue(textItem.NewContents.ToXml().Contains("<table "));
		}

		[Test]
		public void TextItemGeneratesCorrectXml()
		{
			string textItemXml =
				"<text label=\"T1\" style=\"normal\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with text of your own.]" +
				"</paragraph></text>" + Environment.NewLine;

			formEditPresenter.InsertTextItem(FormPresenter.InsertAtEnd);

			string actualTextItemXml = Regex.Match(Project.Current.ToXml(), "<text .*</text>\r\n").Value;

			Assert.AreEqual(textItemXml, actualTextItemXml);
		}

		[Test]
		public void ModifyingTextUpdatesProjectTextItem()
		{
			string textItemXml =
				"<text label=\"T1\" style=\"normal\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"modified text" +
				"</paragraph></text>";

			formEditPresenter.InsertTextItem(FormPresenter.InsertAtEnd);
			ITextItem textItem = GetFormItem<ITextItem>();

			formView.SetAttribute(textItem.Id.ToString(), "Contents", "<P style=\"MARGIN-LEFT: 0pt\">modified text</P>");

			string projectXml = Project.Current.ToXml();

			Assert.IsTrue(projectXml.Contains(textItemXml));
		}

		private HtmlElement getTextItemDiv()
		{
			HtmlElementCollection tags = body.GetElementsByTagName("TextItem");
			Assert.AreEqual(1, tags.Count);
			HtmlElement element = tags[0];
			HtmlElement div = element.Children[0];
			return div;
		}
	}

	[TestFixture]
	public class TextItemModelTest : ModelOrientedTestBase
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
		public void TextItemInProjectGeneratesTextItemInView()
		{
			ITextItem textItem = new NewTextItem();
			form.ItemList.Add(textItem);

			CreateViewFromForm();

			string html = XmlUtility.ToXhtml(body.InnerHtml);

			VerifyItemTag("TextItem");
			string textItemXhtml = textItem.NewContents.ToXhtml(textItem);

			string actualHtml = Regex.Match(html, @"(<p[^<]+</p>)").Groups[1].Value;

			Assert.AreEqual(textItemXhtml.ToLower(), actualHtml.ToLower());
		}


		private static string oneRowXml =
			"<row>" +
			"<cell width=\"5400\">" +
			"<division indent=\"0\" align=\"left\"><sp/></division>" +
			"</cell>" +
			"<cell width=\"5400\">" +
			"<division indent=\"0\" align=\"left\"><sp/></division>" +
			"</cell>" +
			"</row>";

		private static string rowXml =
			oneRowXml + oneRowXml + oneRowXml + oneRowXml;

		private static string tableXml = 
			"<text label=\"T1\" style=\"normal\">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"</paragraph>" +
			"<table indent=\"0\">" +
			rowXml +
			"</table>" +
			"</text>";

		[Test]
		public void TableInTextItemAppearsInViewHtml()
		{
			ITextItem textItem = new NewTextItem(new XmlElement(tableXml));
			form.ItemList.Add(textItem);

			CreateViewFromForm();

			string html = body.InnerHtml;

			int textItemIndex = html.IndexOf("TextItem");
			int tableIndex = html.LastIndexOf("<TABLE class=user");
			Assert.IsTrue(textItemIndex > 0);
			Assert.IsTrue(tableIndex > textItemIndex);
		}
	}
}
