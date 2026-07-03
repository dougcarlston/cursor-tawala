using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Images;
using TawalaTest.TestingSupport;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest.Forms.FormItemContents
{
	[TestFixture]
	public class ImageReferenceTest : ImageTestBase
	{

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
		}

		[Test]
		public void ImageGeneratesProperXml()
		{
			Project.NewImages.Add(oneWhitePixelJpgPath);

			string htmlFormatString =
				"<DIV>" +
				"<IMG src=\"{0}\">" +
				"</DIV>";

			string htmlString = String.Format(htmlFormatString, oneWhitePixelJpgUrl);
			IFormItemContents contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

			string expectedXml = @"<image id=""image1"" width=""0"" height=""0""></image>";

			Assert.AreEqual(expectedXml, contents.ToXml());
		}

		[Test]
		public void ContentsWithTwoIdenticalImagesGeneratesProperXml()
		{
			Project.NewImages.Add(oneWhitePixelJpgPath);

			string htmlFormatString =
				"<DIV>" +
				"<IMG src=\"{0}\">" +
				"<IMG src=\"{1}\">" +
				"</DIV>";

			string htmlString = String.Format(htmlFormatString, oneWhitePixelJpgUrl, oneWhitePixelJpgUrl);
			IFormItemContents contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

			string expectedXml =
				@"<image id=""image1"" width=""0"" height=""0""></image>" +
				@"<image id=""image1"" width=""0"" height=""0""></image>";

			Assert.AreEqual(expectedXml, contents.ToXml());
		}

		[Test]
		public void ContentsWithTwoDifferentImagesGeneratesProperXml()
		{
			Project.NewImages.Add(oneWhitePixelJpgPath);
			Project.NewImages.Add(oneBlackPixelJpgPath);

			string htmlFormatString =
				"<DIV>" +
				"<IMG src=\"{0}\">" +
				"<IMG src=\"{1}\">" +
				"</DIV>";

			string htmlString = String.Format(htmlFormatString, oneWhitePixelJpgUrl, oneBlackPixelJpgUrl);
			IFormItemContents contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

			string expectedXml =
				@"<image id=""image1"" width=""0"" height=""0""></image>" +
				@"<image id=""image2"" width=""0"" height=""0""></image>";

			Assert.AreEqual(expectedXml, contents.ToXml());
		}

		[Test]
		public void ImageUrlWithEncodingHasLegalFileName()
		{
			Project.NewImages.Add(unpronounceableJpgPath);

			string htmlFormatString =
				"<DIV>" +
				"<IMG src=\"{0}\">" +
				"</DIV>";

			string htmlString = String.Format(htmlFormatString, unpronounceableJpgUrl);
			IImageReference imageReference = ((FormItemContentsCollection)FormItemContentsFactory.MakeChildrenFromHtml(htmlString))[0] as IImageReference;

			Assert.AreEqual("{} []^`~.jpg", Path.GetFileName(imageReference.PathName));
		}

		[Test]
		public void ImageGeneratesProperXhtml()
		{
			Project.NewImages.Add(oneWhitePixelJpgPath);

			string htmlFormatString =
				"<DIV>" +
				"<IMG src=\"{0}\">" +
				"</DIV>";

			string htmlString = String.Format(htmlFormatString, oneWhitePixelJpgUrl);
			IFormItemContents contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

			string xhtmlFormatString = @"<img src=""{0}"" />";
			string expectedXhtml = String.Format(xhtmlFormatString, oneWhitePixelJpgUrl);

			Assert.AreEqual(expectedXhtml, contents.ToXhtml(null));
		}
	}
}
