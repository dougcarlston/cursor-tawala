using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using NUnit.Framework;
using Tawala.XmlSupport;
using Tawala.Projects.Images;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest.ImagesTest
{
	[TestFixture]
	public class ImageDefinitionTest : ImageTestBase
	{
		[Test]
		public void CanConstructImageDefinitionFromPath()
		{
			string imageId = "image1";

			ImageDefinition imageDefinition = new ImageDefinition(oneWhitePixelJpgPath, imageId);

			Assert.IsNotNull(imageDefinition);
			Assert.AreEqual(imageId, imageDefinition.Id);
			Assert.AreEqual(oneWhitePixelJpgPath, imageDefinition.PathName);
		}

		private static string imageDefinitionXmlString =
			@"<imagedef id=""image1"">" +
			@"<imagedata imageFormat=""JPG"">" +
			oneWhitePixelJpgDataString +
			@"</imagedata>" +
			@"</imagedef>";

		[Test]
		public void ImageDefinitionConstructedFromPathProducesCorrectXml()
		{
			ImageDefinition imageDefinition = new ImageDefinition(oneWhitePixelJpgPath, "image1");

			Assert.AreEqual(imageDefinitionXmlString, imageDefinition.ToXml());
		}

		[Test]
		public void ImageDefinitionConstructedFromXmlProducesCorrectXml()
		{
			IImageDefinition imageDefinition = new ImageDefinition(new XmlElement(imageDefinitionXmlString));

			Assert.AreEqual(imageDefinitionXmlString, imageDefinition.ToXml());
		}

		[Test]
		public void ImageDefinitionConstructedFromXmlProducesValidPath()
		{
			IImageDefinition imageDefinition = new ImageDefinition(new XmlElement(imageDefinitionXmlString));

			Assert.IsTrue(File.Exists(imageDefinition.PathName));
		}

		[Test]
		public void ImageDefinitionConstructedFromXmlProducesValidLoadableImage()
		{
			IImageDefinition imageDefinition = new ImageDefinition(new XmlElement(imageDefinitionXmlString));

			using (var image = Image.FromFile(imageDefinition.PathName))
			{
				Assert.IsNotNull(image);
			}
		}
	}
}
