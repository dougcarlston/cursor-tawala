using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.XmlSupport;
using Tawala.Projects.Images;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest.ImagesTest
{
	[TestFixture]
	public class ImageDefinitionCollectionTest : ImageTestBase
	{
		[Test]
		public void CanAddImageByPath()
		{
			IImageDefinitionCollection imageDefinitions = new ImageDefinitionCollection();
			imageDefinitions.Add(oneWhitePixelJpgPath);

			Assert.AreEqual(1, ((ImageDefinitionCollection)imageDefinitions).Count);
			Assert.IsInstanceOfType(typeof(IImageDefinition), imageDefinitions[0]);
		}

		[Test]
		public void AddingSameImageTwiceResultsInSingleImageDefinition()
		{
			IImageDefinitionCollection imageDefinitions = new ImageDefinitionCollection();
			imageDefinitions.Add(oneWhitePixelJpgPath);

			IImageDefinition originalImageDefinition = imageDefinitions[0];

			imageDefinitions.Add(oneWhitePixelJpgPath);

			Assert.AreEqual(1, ((ImageDefinitionCollection)imageDefinitions).Count);
			Assert.AreSame(originalImageDefinition, imageDefinitions[0]);
		}

		[Test]
		public void AddingSameImageTwiceReturnsSameImageId()
		{
			IImageDefinitionCollection imageDefinitions = new ImageDefinitionCollection();
			string idImage1 = imageDefinitions.Add(oneWhitePixelJpgPath);
			string idImage2 = imageDefinitions.Add(oneWhitePixelJpgPath);

			Assert.AreEqual(idImage1, idImage2);
			Assert.IsNotNull(imageDefinitions[idImage1]);
		}

		[Test]
		public void AddingTwoDifferentImagesResultsInTwoImageDefinitions()
		{
			IImageDefinitionCollection imageDefinitions = new ImageDefinitionCollection();
			imageDefinitions.Add(oneWhitePixelJpgPath);
			imageDefinitions.Add(oneBlackPixelJpgPath);

			Assert.AreEqual(2, ((ImageDefinitionCollection)imageDefinitions).Count);
			Assert.AreNotSame(imageDefinitions[0], imageDefinitions[1]);
		}

		[Test]
		public void AddingTwoDifferentImagesReturnsTwoImageIds()
		{
			IImageDefinitionCollection imageDefinitions = new ImageDefinitionCollection();
			string idImage1 = imageDefinitions.Add(oneWhitePixelJpgPath);
			string idImage2 = imageDefinitions.Add(oneBlackPixelJpgPath);

			Assert.AreNotEqual(idImage1, idImage2);
		}

		[Test]
		public void CanConstructFromXml()
		{
			string xmlString =
				@"<images>" +
				@"<imagedef id=""image1"">" +
				@"<imagedata imageFormat=""JPG"">" +
				oneWhitePixelJpgDataString +
				@"</imagedata>" +
				@"</imagedef>" +
				@"</images>";

			IImageDefinitionCollection imageDefinitions = new ImageDefinitionCollection(new XmlElement(xmlString));

			Assert.AreEqual(1, ((ImageDefinitionCollection)imageDefinitions).Count);
		}

		[Test]
		public void CollectionConstructedFromXmlContainsImageDefinition()
		{
			string xmlString =
				@"<images>" +
				@"<imagedef id=""image1"">" +
				@"<imagedata imageFormat=""JPG"">" +
				oneWhitePixelJpgDataString +
				@"</imagedata>" +
				@"</imagedef>" +
				@"</images>";

			IImageDefinitionCollection imageDefinitions = new ImageDefinitionCollection(new XmlElement(xmlString));

			Assert.IsInstanceOfType(typeof(IImageDefinition), imageDefinitions[0]);
		}

		[Test]
		public void CollectionConstructedFromXmlProducesCorrectXml()
		{
			string xmlString =
				@"<images>" +
				@"<imagedef id=""image1"">" +
				@"<imagedata imageFormat=""JPG"">" +
				oneWhitePixelJpgDataString +
				@"</imagedata>" +
				@"</imagedef>" +
				@"</images>";

			IImageDefinitionCollection imageDefinitions = new ImageDefinitionCollection(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, imageDefinitions.ToXml());
		}

		[Test]
		public void GettingImageDefinitionByPathNameAddsToCollection()
		{
			ImageDefinitionCollection imageDefinitions = new ImageDefinitionCollection();

			IImageDefinition imageDefinition = ((IImageDefinitionCollection)imageDefinitions).GetImageDefinitionByPathName(oneWhitePixelJpgPath);
			Assert.AreEqual(1, imageDefinitions.Count);
		}
	}
}
