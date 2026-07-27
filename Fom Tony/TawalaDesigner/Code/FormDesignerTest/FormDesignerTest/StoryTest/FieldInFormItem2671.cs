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
using Tawala.Proj.Forms.FormItemContents;
using Tawala.XmlSupport;
using Tawala.Interfaces;

namespace TawalaTest.FormDesignerTest.StoryTest
{
	[TestFixture]
	public class FieldInFormItem2671
	{
		private IFormPresenter formEditPresenter;
		private FormView formView;
		private WebBrowser browser;
		private HtmlElement body;

		private IForm form;
		private NewFibItem fibItem;

		private HtmlElement getTextElement()
		{
			HtmlElementCollection elements = body.GetElementsByTagName("TextItem");
			return elements[0];
		}

		private HtmlElement getMcqElement()
		{
			HtmlElementCollection elements = body.GetElementsByTagName("McqItem");
			return elements[0];
		}

		private void insertField(IFormField field)
		{
			formEditPresenter.InsertField(field);
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

		private void insertMcqItemAtEndOfForm()
		{
			formEditPresenter.InsertMcqItem(-1);
		}

		private void setFocusToTextItemContents(int textItemIndex)
		{
			HtmlElementCollection elements = browser.Document.GetElementsByTagName("TextItem");
			HtmlElement container = elements[textItemIndex].FirstChild;
			TestUtil.SetHtmlElementFocus(container);
		}

		private void setFocusToMcqItemContents(int mcqItemIndex)
		{
			HtmlElementCollection elements = browser.Document.GetElementsByTagName("McqItem");
			HtmlElement container = elements[mcqItemIndex].FirstChild;
			TestUtil.SetHtmlElementFocus(container);
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

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();

			form = Project.Current.AddForm();
			fibItem = new NewFibItem();
			form.ItemList.Add(fibItem);

			formView = new FormView(form);

			formEditPresenter = formView.Presenter;

			browser = TestUtil.PumpMessagesUntilUIReady(formView);

			body = browser.Document.Body;
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

			Assert.IsTrue(textItemElement.InnerHtml.Contains("<t:field class=field"));
		}

		[Test]
		public void RepositioningBlankInViewUpdatesExistingFieldNameInTextItem()
		{
			insertFibItemAtEndOfForm();

			insertTextItemAtEndOfForm();
			setFocusToTextItemContents(0);

			NewFibItem fibItem = Project.Current.FormList[0].GetFormItem("Q1") as NewFibItem;
			IBlank blank = fibItem.BlankList[0];
			insertField(blank);

			insertFibItemAtBeginningOfForm();
			Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(form));

			HtmlElement textItemElement = getTextElement();

			string expectedHtml = "Form 1:Q2:a";
			string actualHtml = extractFieldTextFromElement(textItemElement);
			Assert.AreEqual(expectedHtml, actualHtml);
		}

		[Test]
		public void RepositioningBlankInViewUpdatesExistingFieldNameInMcqItem()
		{
			insertFibItemAtEndOfForm();

			insertMcqItemAtEndOfForm();
			setFocusToMcqItemContents(0);

			NewFibItem fibItem = Project.Current.FormList[0].GetFormItem("Q1") as NewFibItem;
			IBlank blank = fibItem.BlankList[0];
			insertField(blank);

			insertFibItemAtBeginningOfForm();
			Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(form));

			HtmlElement mcItemElement = getMcqElement();

			string expectedHtml = "Form 1:Q2:a";
			string actualHtml = extractFieldTextFromElement(mcItemElement);
			Assert.AreEqual(expectedHtml, actualHtml);
		}

		[Test]
		public void CanConstructFibItemWithField()
		{
			string xmlString =
				@"<fib label=""Q2"" style=""topLabels"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"<sp/>" +
				@"<blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
				@"</paragraph>" +
				@"</fib>" + Environment.NewLine;

			NewFibItem fibItem = new NewFibItem(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, fibItem.ToXml("Q2"));
		}

		[Test]
		public void CanConstructTextItemWithField()
		{
			string xmlString =
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine;

			ITextItem textItem = new NewTextItem(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, textItem.ToXml("T1"));
		}

		[Test]
		public void CanConstructMCItemWithFieldInQuestion()
		{
			string xmlString =
				@"<mc label=""Q2"" onlyone=""true"" required=""false"" style=""vertical"">" +
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a"">" +
				@"</choice>" +
				@"</mc>" + Environment.NewLine;

			NewMcqItem mcItem = new NewMcqItem(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, mcItem.ToXml("Q2"));
		}

		[Test]
		public void CanConstructMCItemWithFieldInChoice()
		{
			string xmlString =
				@"<mc label=""Q2"" onlyone=""true"" required=""false"" style=""vertical"">" +
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"Question" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"</choice>" +
				@"</mc>" + Environment.NewLine;

			NewMcqItem mcItem = new NewMcqItem(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, mcItem.ToXml("Q2"));
		}

		private static string extractFieldTextFromElement(HtmlElement mcItemElement)
		{
			string actualHtml = Regex.Match(mcItemElement.InnerHtml, @"<t:field [^>]+>(.+)</t:field>").Groups[1].Value;
			return actualHtml;
		}
	}
}
