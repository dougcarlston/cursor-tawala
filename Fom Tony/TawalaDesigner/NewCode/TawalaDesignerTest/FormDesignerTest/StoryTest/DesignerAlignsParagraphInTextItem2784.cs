using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Tawala.Interfaces;

namespace TawalaTest.FormDesignerTest.StoryTest
{
	[TestFixture]
	public class DesignerAlignsParagraphInTextItem2784
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
		public void CenterAligningTextItemResultsInCenterAlignedParagraphXml()
		{
			setupAndInsertTextItemInView();

			setFocusToTextItem();
			clickAlignCenterToolbarButton();

			Project.Events.RaiseSynchronizeProjectEvent();

			Assert.AreEqual(getExpectedXml("center"), textItem.ToXml("T1"));
		}

		[Test]
		public void RightAligningTextItemResultsInRightAlignedParagraphXml()
		{
			setupAndInsertTextItemInView();

			setFocusToTextItem();
			clickAlignRightToolbarButton();

			Project.Events.RaiseSynchronizeProjectEvent();

			Assert.AreEqual(getExpectedXml("right"), textItem.ToXml("T1"));
		}

		[Test]
		public void JustifyAligningTextItemResultsInJustifyAlignedParagraphXml()
		{
			setupAndInsertTextItemInView();

			setFocusToTextItem();
			clickAlignJustifyToolbarButton();

			Project.Events.RaiseSynchronizeProjectEvent();

			Assert.AreEqual(getExpectedXml("justify"), textItem.ToXml("T1"));
		}

		[Test]
		public void RightAligningThenLeftAligningTextItemResultsInLeftAlignedParagraphXml()
		{
			setupAndInsertTextItemInView();

			setFocusToTextItem();
			clickAlignRightToolbarButton();

			Project.Events.RaiseSynchronizeProjectEvent();

			clickAlignLeftToolbarButton();

			Project.Events.RaiseSynchronizeProjectEvent();
			Assert.AreEqual(getExpectedXml("left"), textItem.ToXml("T1"));
		}

		private string getExpectedXml(string alignment)
		{
			return string.Format(
				"<text label=\"T1\" style=\"normal\">" +
				"<paragraph indent=\"0\" align=\"{0}\">" +
				"[Replace this with text of your own.]" +
				"</paragraph>" +
				"</text>" + Environment.NewLine, alignment);
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
			IForm form = Tawala.Projects.Project.Current.AddForm();

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

		private void clickAlignLeftToolbarButton()
		{
			TestUtil.ClickFormEditViewToolStripButton(formView, "alignLeftToolStripMenuItem");
		}
		private void clickAlignCenterToolbarButton()
		{
			TestUtil.ClickFormEditViewToolStripButton(formView, "alignCenterToolStripMenuItem");
		}
		private void clickAlignRightToolbarButton()
		{
			TestUtil.ClickFormEditViewToolStripButton(formView, "alignRightToolStripMenuItem");
		}
		private void clickAlignJustifyToolbarButton()
		{
			TestUtil.ClickFormEditViewToolStripButton(formView, "justifyToolStripMenuItem");
		}
	}
}
