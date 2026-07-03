using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.XmlSupport;
using Tawala.Projects.Images;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest.ImagesTest
{
	[TestFixture]
	public class ImageDataTest : ImageTestBase
	{
		[Test]
		public void ImageDataProducesCorrectJpgXml()
		{
			string xmlString =
				@"<imagedata imageFormat=""JPG"">" +
				oneWhitePixelJpgDataString +
				@"</imagedata>";

			IImageData imageData = new ImageData(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, imageData.ToXml());
		}

		[Test]
		public void ImageDataProducesCorrectPngXml()
		{
			string xmlString =
				@"<imagedata imageFormat=""PNG"">" +
				oneWhitePixelPngDataString +
				@"</imagedata>";

			IImageData imageData = new ImageData(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, imageData.ToXml());
		}

		[Test]
		public void ImageDataProducesCorrectGifXml()
		{
			string xmlString =
				@"<imagedata imageFormat=""GIF"">" +
				oneWhitePixelGifDataString +
				@"</imagedata>";

			IImageData imageData = new ImageData(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, imageData.ToXml());
		}

		[Test]
		public void ImageFileHasJpgExtension()
		{
			string xmlString =
				@"<imagedata imageFormat=""JPG"">" +
				oneWhitePixelJpgDataString +
				@"</imagedata>";

			IImageData imageData = new ImageData(new XmlElement(xmlString));

			Assert.AreEqual(".jpg", Path.GetExtension(imageData.CreateImageFile()));
		}

		[Test]
		public void ImageFileHasPngExtension()
		{
			string xmlString =
				@"<imagedata imageFormat=""PNG"">" +
				oneWhitePixelPngDataString +
				@"</imagedata>";

			IImageData imageData = new ImageData(new XmlElement(xmlString));

			Assert.AreEqual(".png", Path.GetExtension(imageData.CreateImageFile()));
		}

		[Test]
		public void ImageFileHasGifExtension()
		{
			string xmlString =
				@"<imagedata imageFormat=""GIF"">" +
				oneWhitePixelGifDataString +
				@"</imagedata>";

			IImageData imageData = new ImageData(new XmlElement(xmlString));

			Assert.AreEqual(".gif", Path.GetExtension(imageData.CreateImageFile()));
		}

		[Test]
		public void PathWithJpgExtensionProduceCorrectXml()
		{
			IImageData imageData = new ImageData(oneWhitePixelJpgPath);

			string xmlString =
				@"<imagedata imageFormat=""JPG"">" +
				oneWhitePixelJpgDataString +
				@"</imagedata>";

			Assert.AreEqual(xmlString, imageData.ToXml());
		}

		[Test]
		public void PathWithPngExtensionProduceCorrectXml()
		{
			IImageData imageData = new ImageData(Util.GetTestFilePath("OneWhitePixel.png"));

			string xmlString =
				@"<imagedata imageFormat=""PNG"">" +
				oneWhitePixelPngDataString +
				@"</imagedata>";

			Assert.AreEqual(xmlString, imageData.ToXml());
		}

		[Test]
		public void PathWithGifExtensionProduceCorrectXml()
		{
			IImageData imageData = new ImageData(Util.GetTestFilePath("OneWhitePixel.gif"));

			string xmlString =
				@"<imagedata imageFormat=""GIF"">" +
				oneWhitePixelGifDataString +
				@"</imagedata>";

			Assert.AreEqual(xmlString, imageData.ToXml());
		}
	}
}
