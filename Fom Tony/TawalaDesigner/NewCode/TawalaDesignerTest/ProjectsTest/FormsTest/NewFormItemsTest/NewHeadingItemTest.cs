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
	public class NewHeadingItemTest
	{
		private IHeadingItem headingItem;

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
			headingItem = null;
		}

		[Test]
		public void NewHeadingItemIsAnIFormItem()
		{
			headingItem = new NewHeadingItem();

			Assert.IsTrue(headingItem is IFormItem);
		}

		[Test]
		public void CanGetContentsXml()
		{
			headingItem = new NewHeadingItem();

			string expectedXml = defaultHeadingParagraph;

            Assert.AreEqual(expectedXml, headingItem.Contents.ToXml());
		}

		[Test]
		public void CanGetContentsXhtml()
		{
			IForm form = Project.Current.AddForm();
			headingItem = new NewHeadingItem();
			form.ItemList.Add(headingItem);

			string expectedXhtml =
				@"<p style=""margin-left: 0pt"" align=""left"">" +
				@"[Replace this with heading of your own.]" +
				@"</p>";

            string xhtmlString = headingItem.Contents.ToXhtml(headingItem);

			Assert.AreEqual(expectedXhtml, xhtmlString);
		}

		[Test]
		public void CanSetContentsFromXhtml()
		{
			IForm form = Project.Current.AddForm();
			headingItem = new NewHeadingItem();
			form.ItemList.Add(headingItem);

			string htmlString =
			"<DIV class=\"headingitem formitem\" contentEditable=true tabIndex=0>" + Environment.NewLine +
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

            headingItem.Contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

            Assert.AreEqual(expectedXml, headingItem.Contents.ToXml());
		}


		[Test]
		public void CanGetDefaultXml()
		{
			headingItem = new NewHeadingItem();

			string expectedXml =
				"<heading label=\"H1\" type=\"Main\">" +
				defaultHeadingParagraph +
				"</heading>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, headingItem.ToXml("H1"));
		}

		[Test]
		public void FormItemListContainsHeadingItem()
		{
			headingItem = new NewHeadingItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(headingItem);

			Assert.IsTrue(Project.Current.FormList[0].ItemList.Contains(headingItem));
		}

		[Test]
		public void FormItemListYieldsHeadingItemDefaultLabel()
		{
			headingItem = new NewHeadingItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(headingItem);

			Assert.AreEqual("H1", Project.Current.FormList[0].ItemList.GetDefaultLabel(headingItem));
		}

		[Test]
		public void HeadingItemWithAlternateLabelGeneratesExpectedHeadingItemXml()
		{
			headingItem = new NewHeadingItem();
			headingItem.AlternateLabel = "HeadingAlternateLabel";

			string expectedXml =
				"<heading label=\"H1\" alternateLabel=\"HeadingAlternateLabel\" type=\"Main\">" +
				defaultHeadingParagraph +
				"</heading>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, headingItem.ToXml("H1"));
		}

		[Test]
		public void HeadingItemWithAlternateLabelRegeneratesCorrectlyFromXml()
		{
			string xmlString =
				"<heading label=\"H1\" alternateLabel=\"HeadingAlternateLabel\" type=\"Main\">" +
				defaultHeadingParagraph +
				"</heading>" + Environment.NewLine;

			headingItem = new NewHeadingItem(new XmlElement(xmlString));

			Assert.AreEqual("HeadingAlternateLabel", headingItem.AlternateLabel);
		}

		[Test]
		public void HeadingItemWithTypeSubGeneratesCorrectXml()
		{
			headingItem = new NewHeadingItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(headingItem);

			headingItem.HeadingType = HeadingType.Sub;

			string expectedXml =
				"<heading label=\"H1\" type=\"Sub\">" +
				defaultHeadingParagraph +
				"</heading>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, headingItem.ToXml("H1"));
		}

		private const string defaultHeadingParagraph =
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with heading of your own.]" +
				"</paragraph>";
	}
}
