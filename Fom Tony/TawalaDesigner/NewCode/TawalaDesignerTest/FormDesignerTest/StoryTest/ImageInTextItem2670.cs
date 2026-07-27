using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestingSupport;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using Tawala.Interfaces;

namespace TawalaTest.FormDesignerTest.StoryTest
{
	[TestFixture]
	public class ImageInTextItem2670 : ImageTestBase
	{
		private IFormView formView;
		private WebBrowser browser;

		private IForm form;
		private ITextItem textItem;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			ComponentMaker.UseNewComponents(true);
			formView = null;
		}

		[TearDown]
		public void TearDown()
		{
			if (formView != null)
			{
				((System.Windows.Forms.Form)formView).Close();
			}

			formView = null;
            browser = null;
            form = null;
            textItem = null;

			ComponentMaker.UseNewComponents(false);
		}

		[Test]
		[Ignore("Ignored because form item contents cannot reliably be selected - SB 09/19/2008")]
		public void InsertingImageIntoTextItemResultsInImageNameInHtml()
		{
			createFormViewWithTextItem();

			TestUtil.SelectFormItemContents(formView, textItem);
			formView.Presenter.InsertImage(oneWhitePixelJpgPath);
			displayFormEditView();

			string expectedHtml = createExpectedImageElement(oneWhitePixelJpgPath, 1, 1);
			string actualHtml = findImageElementInBrowser();
			Console.WriteLine(actualHtml);
			Assert.AreEqual(expectedHtml, actualHtml);
		}

		private void displayFormEditView()
		{
			((System.Windows.Forms.Form)formView).Show();
			TestUtil.PumpMessages();
		}

		[Test]
		public void TextItemWithImageGeneratesCorrectProjectXml()
		{
			IForm form = Project.Current.AddForm();
			NewTextItem textItem = new NewTextItem();
			form.ItemList.Add(textItem);

			Project.NewImages.Add(oneWhitePixelJpgPath);

			string htmlString = getTextContentsHtmlString(oneWhitePixelJpgFileName);

            textItem.Contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

			string expectedXml =
				@"<project name=""Untitled"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" + Environment.NewLine +
				@"<pageHeader></pageHeader>" +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<image id=""image1"" width=""1"" height=""1""></image>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>" +
				@"</forms>" + Environment.NewLine +
				@"<images>" +
				@"<imagedef id=""image1"">" +
				@"<imagedata imageFormat=""JPG"">" +
				oneWhitePixelJpgDataString +
				@"</imagedata>" +
				@"</imagedef>" +
				@"</images>" +
				@"</project>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, Project.Current.ToXml());
		}

		private string findImageElementInBrowser()
		{
			return Regex.Match(browser.Document.Body.InnerHtml, @"<IMG[^>]+>").Value;
		}

		private static string createExpectedImageElement(string testFilePath, int width, int height)
		{
			return string.Format("<IMG src=\"file:///{0}\" onload=fnImageLoad() ImageWidth=\"{1}\" ImageHeight=\"{2}\">", replaceBackslashesWithForwardSlashes(testFilePath), width, height);
		}

		private static string replaceBackslashesWithForwardSlashes(string path)
		{
			return path.Replace(@"\", "/");
		}

		[Test]
		public void TextItemWithImageGeneratesCorrectXml()
		{
			Project.NewImages.Add(oneWhitePixelJpgPath);

			IForm form = Project.Current.AddForm();
			NewTextItem textItem = new NewTextItem();
			form.ItemList.Add(textItem);

			string htmlString = getTextContentsHtmlString(oneWhitePixelJpgFileName);

            textItem.Contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

			string expectedXml =
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<image id=""image1"" width=""1"" height=""1""></image>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}

		private static string getTextContentsHtmlString(string imageFileName)
		{
			string htmlFormatString =
				"<DIV>" +
				"<P style=\"MARGIN-LEFT: 0pt\" align=left>" +
				"<IMG onload=fnImageLoad() src=\"file:///{0}\" ImageWidth=\"1\" ImageHeight=\"1\">" +
				"</P>" +
				"</DIV>";

			string testFilePath = Util.GetTestFilePath(imageFileName);
			string htmlString = string.Format(htmlFormatString, replaceBackslashesWithForwardSlashes(testFilePath));
			return htmlString;
		}

		[Test]
		public void CanConstructProjectWithImageInTextItemFromXml()
		{
			string projectXml =
				@"<project name=""Untitled"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" + Environment.NewLine +
				@"<pageHeader></pageHeader>" +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<image id=""image1"" width=""1"" height=""1""></image>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>" +
				@"</forms>" + Environment.NewLine +
				@"<images>" +
				@"<imagedef id=""image1"">" +
				@"<imagedata imageFormat=""JPG"">" +
				oneWhitePixelJpgDataString +
				@"</imagedata>" +
				@"</imagedef>" +
				@"</images>" +
				@"</project>" + Environment.NewLine;

			Project.Create(new XmlElement(projectXml));

			Assert.AreEqual(projectXml, Project.Current.ToXml());
		}

		private void createFormViewWithTextItem()
		{
			form = Project.Current.AddForm();
			textItem = new NewTextItem();
			form.ItemList.Add(textItem);

			formView = new FormView(form);

			browser = TestUtil.PumpMessagesUntilUIReady(formView);
		}
	}
}
