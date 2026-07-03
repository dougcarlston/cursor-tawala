using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest.Forms.NewFormItems
{
	[TestFixture]
	public class NewTextItemTest : ImageTestBase
	{
		private ITextItem textItem;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			ComponentMaker.UseNewComponents(true);
		}

		[TearDown]
		public void TearDown()
		{
			ComponentMaker.UseNewComponents(false);
			textItem = null;
		}

		[Test]
		public void NewTextItemIsAnIFormItem()
		{
			textItem = new NewTextItem();

			Assert.IsTrue(textItem is IFormItem);
		}

		[Test]
		public void CanGetContentsXml()
		{
			textItem = new NewTextItem();

			string expectedXml =
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with text of your own.]" +
				"</paragraph>";

            Assert.AreEqual(expectedXml, textItem.Contents.ToXml());
		}

		[Test]
		public void CanGetContentsXhtml()
		{
			IForm form = Project.Current.AddForm();
			textItem = new NewTextItem();
			form.ItemList.Add(textItem);

			string expectedXhtml =
				@"<p style=""margin-left: 0pt"" align=""left"">" +
				@"[Replace this with text of your own.]" +
				@"</p>";

            string xhtmlString = textItem.Contents.ToXhtml(textItem);

			Assert.AreEqual(expectedXhtml, xhtmlString);
		}

		[Test]
		public void CanSetContentsFromXhtml()
		{
			IForm form = Project.Current.AddForm();
			textItem = new NewTextItem();
			form.ItemList.Add(textItem);

			string htmlString =
			"<DIV class=\"headingtitem formitem\" contentEditable=true tabIndex=0>" + Environment.NewLine +
			"<P style=\"MARGIN-LEFT: 0pt\" align=left>" +
			"This is the first paragraph." +
			"</P>" +
			"<P style=\"MARGIN-LEFT: 0pt\" align=left>" +
			"Second paragraph." +
			"</P>" +
			"</DIV>";
			
			string expectedXml =
				"<paragraph indent=\"0\" align=\"left\">" +
				"This is the first paragraph." +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Second paragraph." +
				"</paragraph>";

            textItem.Contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

            Assert.AreEqual(expectedXml, textItem.Contents.ToXml());
		}

		[Test]
		public void ImageInTextItemHtmlYieldsImageInContents()
		{
			Project.NewImages.Add(oneWhitePixelJpgPath);

			IForm form = Project.Current.AddForm();
			textItem = new NewTextItem();
			form.ItemList.Add(textItem);

			string htmlFormatString =
			"<DIV>" +
			"<P style=\"MARGIN-LEFT: 0pt\" align=left>" +
			"This is the first paragraph." +
			"<IMG src=\"{0}\">" +
			"</P>" +
			"</DIV>";

			string htmlString = String.Format(htmlFormatString, oneWhitePixelJpgUrl);
            textItem.Contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

            FormItemContentsCollection imageDescendants = textItem.Contents.GetDescendants(typeof(IImageReference));
			Assert.AreEqual(1, imageDescendants.Count);
		}


		[Test]
		public void CanGetDefaultXml()
		{
			textItem = new NewTextItem();

			string expectedXml =
				"<text label=\"T1\" style=\"normal\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with text of your own.]" +
				"</paragraph>" +
				"</text>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}

		[Test]
		public void FormItemListContainsTextItem()
		{
			textItem = new NewTextItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(textItem);

			Assert.IsTrue(Project.Current.FormList[0].ItemList.Contains(textItem));
		}

		[Test]
		public void FormItemListYieldsTextItemDefaultLabel()
		{
			textItem = new NewTextItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(textItem);

			Assert.AreEqual("T1", Project.Current.FormList[0].ItemList.GetDefaultLabel(textItem));
		}

		[Test]
		public void TextItemWithAlternateLabelGeneratesExpectedTextItemXml()
		{
			textItem = new NewTextItem();
			textItem.AlternateLabel = "TextAlternateLabel";

			string expectedXml =
				"<text label=\"T1\" alternateLabel=\"TextAlternateLabel\" style=\"normal\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with text of your own.]" +
				"</paragraph>" +
				"</text>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}

		[Test]
		public void TextItemWithAlternateLabelRegeneratesCorrectlyFromXml()
		{
			string xmlString =
				"<text label=\"T1\" alternateLabel=\"TextAlternateLabel\" style=\"normal\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with text of your own.]" +
				"</paragraph>" +
				"</text>" + Environment.NewLine;

			textItem = new NewTextItem(new XmlElement(xmlString));

			Assert.AreEqual("TextAlternateLabel", textItem.AlternateLabel);
		}

		[Test]
		public void TextItemWithStyleGeneratesStyleInXml()
		{
			textItem = new NewTextItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(textItem);

			Project.Current.SetAllTextItemStyles("normal");

			string expectedXml =
				"<text label=\"T1\" style=\"normal\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with text of your own.]" +
				"</paragraph>" +
				"</text>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}
	}
}
