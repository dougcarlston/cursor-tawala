using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Images;
using Tawala.Projects.Factories;
using TawalaTest.TestSupport;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest.FactoriesTest
{
	[TestFixture]
	public class ProjectComponentFactoryTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		private string oneWhitePixelJpgDataString =
			@"/9j/4AAQSkZJRgABAgEAYABgAAD/wAARCAABAAEDAREAAhEBAxEB/9sAhAABAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBA" +
			@"QEBAQICAgECAgIBAQIDAgICAgMDAwECAwMDAgMCAgMCAQEBAQEBAQEBAQECAQEBAQICAgICAgICAgICAgICAgICAgICAgICAgICAg" +
			@"ICAgICAgICAgICAgICAgICAgICAgL/xAGiAAABBQEBAQEBAQAAAAAAAAAAAQIDBAUGBwgJCgsQAAIBAwMCBAMFBQQEAAABfQECAwA" +
			@"EEQUSITFBBhNRYQcicRQygZGhCCNCscEVUtHwJDNicoIJChYXGBkaJSYnKCkqNDU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hp" +
			@"anN0dXZ3eHl6g4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2drh4uPk5ebn6Onq8fLz9" +
			@"PX29/j5+gEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoLEQACAQIEBAMEBwUEBAABAncAAQIDEQQFITEGEkFRB2FxEyIygQgUQp" +
			@"GhscEJIzNS8BVictEKFiQ04SXxFxgZGiYnKCkqNTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqCg4SFhoeIiYq" +
			@"Sk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2dri4+Tl5ufo6ery8/T19vf4+fr/2gAMAwEAAhEDEQA/" +
			@"AP7+KAP/2Q==";

		[Test]
		public void CanMakeImageDefinitionCollectionFromXml()
		{
			string imageDefinitionsXml =
				@"<images>" +
				@"<imagedef id=""image1"">" +
				@"<imagedata imageFormat=""JPG"">" +
				oneWhitePixelJpgDataString +
				@"</imagedata>" +
				@"</imagedef>" +
				@"</images>";

			ImageDefinitionCollection imageDefinitions = ProjectComponentFactory.MakeObject(new XmlElement(imageDefinitionsXml)) as ImageDefinitionCollection;

			Assert.IsInstanceOfType(typeof(ImageDefinition), imageDefinitions[0]);
		}

		[Test]
		public void CanMakeImageDefinitionFromXml()
		{
			string imageDefinitionXml =
				@"<imagedef id=""image1"">" +
				@"<imagedata imageFormat=""JPG"">" +
				oneWhitePixelJpgDataString +
				@"</imagedata>" +
				@"</imagedef>";

			IImageDefinition imageDefinition = ProjectComponentFactory.MakeObject(new XmlElement(imageDefinitionXml)) as IImageDefinition;

			Assert.IsInstanceOfType(typeof(ImageDefinition), imageDefinition);
		}

	}
}
