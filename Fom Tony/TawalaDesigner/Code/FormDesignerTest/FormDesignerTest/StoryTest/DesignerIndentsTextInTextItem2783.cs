using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestSupport;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.XmlSupport;
using Tawala.Interfaces;

namespace TawalaTest.FormDesignerTest.StoryTest
{
	[TestFixture]
	public class DesignerIndentsTextInTextItem2783
	{
		private ITextItem textItem;
		private IFormPresenter formEditPresenter;
		private FormView formView;
		private WebBrowser browser;
		private HtmlElement body;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void IndentedTextItemResultsInIndentedParagraphXml()
		{
			setupAndInsertTextItemInView();

			string expected =
				"<text label=\"T1\" style=\"normal\">" + 
				"<paragraph indent=\"720\" align=\"left\">" +
				"[Replace this with text of your own.]" +
				"</paragraph>" +
				"</text>" + Environment.NewLine;

			setFocusToTextItem();
			clickIndentToolbarButton();

			Project.Events.RaiseSynchronizeProjectEvent();

			Assert.AreEqual(expected, textItem.ToXml("T1"));
		}

		[Test]
		public void OutdentingIndentedTextItemResultsInCorrectlyIndentedXml()
		{
			setupAndInsertTextItemInView();

			string expected =
				"<text label=\"T1\" style=\"normal\">" +
				"<paragraph indent=\"1440\" align=\"left\">" +
				"[Replace this with text of your own.]" +
				"</paragraph>" +
				"</text>" + Environment.NewLine;

			setFocusToTextItem();
			clickIndentToolbarButton();
			clickIndentToolbarButton();
			clickIndentToolbarButton();
			clickOutdentToolbarButton();

			Project.Events.RaiseSynchronizeProjectEvent();

			Assert.AreEqual(expected, textItem.ToXml("T1"));
		}

		[Test]
		public void OutdentingNonIndentedTextItemResultsInCorrectXml()
		{
			setupAndInsertTextItemInView();

			string expected =
				"<text label=\"T1\" style=\"normal\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with text of your own.]" +
				"</paragraph>" +
				"</text>" + Environment.NewLine;

			setFocusToTextItem();
			clickOutdentToolbarButton();

			Project.Events.RaiseSynchronizeProjectEvent();

			Assert.AreEqual(expected, textItem.ToXml("T1"));
		}

		[Test]
		public void TextItemInViewHasParagraphTags()
		{
			setupAndInsertTextItemInView();

			string expectedHtml = "<P style=\"MARGIN-LEFT: 0pt\" align=left>[Replace this with text of your own.]</P>";
			string paragraphHtml = Regex.Match(body.InnerHtml, "<t:TextItem .*(<P .*</P>).*</t:TextItem>", RegexOptions.Singleline).Groups[1].Value;
			Assert.AreEqual(expectedHtml, paragraphHtml);
		}

		[Test]
		public void IndentedTextItemInViewHasNonZeroLeftMargin()
		{
			setupAndInsertTextItemInView();

			setFocusToTextItem();
			clickIndentToolbarButton();

			string paragraphHtml = Regex.Match(body.InnerHtml, "<t:TextItem .*(<P .*</P>).*</t:TextItem>", RegexOptions.Singleline).Groups[1].Value;
			Assert.IsTrue(paragraphHtml.Contains("style=\"MARGIN-LEFT: 36pt\""), paragraphHtml);
		}

		[Test]
		public void TwiceIndentedTextItemInViewHasCorrectLeftMargin()
		{
			setupAndInsertTextItemInView();

			setFocusToTextItem();
			clickIndentToolbarButton();
			clickIndentToolbarButton();

			string paragraphHtml = Regex.Match(body.InnerHtml, "<t:TextItem .*(<P .*</P>).*</t:TextItem>", RegexOptions.Singleline).Groups[1].Value;
			Assert.IsTrue(paragraphHtml.Contains("style=\"MARGIN-LEFT: 72pt\""), paragraphHtml);
		}

		[Test]
		public void IndentedTextItemXmlResultsInCorrectHtml()
		{
			string inputXml =
				"<text label=\"T1\" style=\"normal\">" +
				"<paragraph indent=\"720\" align=\"left\">" +
				"[Replace this with text of your own.]" +
				"</paragraph>" +
				"</text>" + Environment.NewLine;

			string expectedHtml = "<P style=\"MARGIN-LEFT: 36pt\" align=left>[Replace this with text of your own.]</P>";

			IForm form = Tawala.Proj.Project.Current.AddForm();
			ITextItem textItem = new NewTextItem(new XmlElement(inputXml));
			form.ItemList.Add(textItem);

			createTextItemInViewFromXml(form);

			string paragraphHtml = Regex.Match(body.InnerHtml, "<t:TextItem .*(<P .*</P>).*</t:TextItem>", RegexOptions.Singleline).Groups[1].Value;
			Assert.AreEqual(expectedHtml, paragraphHtml);
		}

		private void setFocusToTextItem()
		{
			HtmlElementCollection tags = body.GetElementsByTagName("TextItem");
			Assert.AreEqual(1, tags.Count);
			HtmlElement element = tags[0];
			HtmlElement div = element.Children[0];
			HtmlElement paragraph = div.Children[0];
			TestUtil.PumpMessages();

			div.Focus();
		}

		private void setupAndInsertTextItemInView()
		{
			IForm form = Tawala.Proj.Project.Current.AddForm();

			formView = new FormView(form);

			formEditPresenter = formView.Presenter;

			browser = TestUtil.PumpMessagesUntilUIReady(formView);

			formEditPresenter.InsertTextItem(0);

			textItem = form.ItemList[0] as ITextItem;

			body = browser.Document.Body;
		}

		private void createTextItemInViewFromXml(IForm form)
		{
			formView = new FormView(form);

			formEditPresenter = formView.Presenter;

			browser = TestUtil.PumpMessagesUntilUIReady(formView);

			formEditPresenter.InsertTextItem(0);

			textItem = form.ItemList[0] as ITextItem;

			body = browser.Document.Body;
		}

		private void clickIndentToolbarButton()
		{
			TestUtil.ClickFormEditViewToolStripButton(formView, "toolStripButtonIndent");
		}

		private void clickOutdentToolbarButton()
		{
			TestUtil.ClickFormEditViewToolStripButton(formView, "toolStripButtonOutdent");
		}

	}
}
