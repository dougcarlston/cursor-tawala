using System;
using System.Drawing.Imaging;
using System.Reflection;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;
using Tawala.RtfSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the TestFormItem class
	/// </summary>
	[TestFixture]
	public class GraphicImageTest
	{
		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tGraphicImage = typeof(GraphicImage);

		private GraphicImage image;

		/// <summary>
		/// Invokes the GraphicImage method specified by methodName
		/// </summary>
		public Object InvokeGraphicImageMethod(string methodName, params object[] args)
		{
			MethodInfo method = tGraphicImage.GetMethod(methodName, flags);
			return method.Invoke(image, args);
		}

		private uint getRowSize(int widthInPixels)
		{
			return (uint)InvokeGraphicImageMethod("getRowSize", widthInPixels);
		}

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();
			addImageToProject();
		}

		private void addImageToProject()
		{
			// pre-Build 90 format;
			// don't think we need to construct from this anymore, but hanging on to it just in case. jdf - 9/14/06
			//string xmlString =
			//    "<image width=\"10\" height=\"12\" format=\"PNG\">" +
			//    "iVBORw0KGgoAAAANSUhEUgAAAAoAAAAMCAIAAADUCbv3AAAAAXNSR0IArs4c6QAAAARnQU1BAACx\r\n" +
			//    "jwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAABpJREFU\r\n" +
			//    "KFNj/F9ny4AHAKXxIIZRaWzhQyBYAFsXv6i+LNJ8AAAAAElFTkSuQmCC" +
			//    "<wmfFileString>" +
			//    "0100090000033b03000002000502000000000500000007010300000005020000f70000030001000000000d0d0d001a1a1a00282828003535350043434300505050005d5d5d006b6b6b00787878008686860093939300a1a1a100aeaeae00bbbbbb00c9c9c900d6d6d600e4e4e400f1f1f100ffffff00000000000000330000006600000099000000cc000000ff00003300000033330000336600003399000033cc000033ff00006600000066330000666600006699000066cc000066ff00009900000099330000996600009999000099cc000099ff0000cc000000cc330000cc660000cc990000cccc0000ccff0000ff000000ff330000ff660000ff990000ffcc0000ffff00330000003300330033006600330099003300cc003300ff00333300003333330033336600333399003333cc003333ff00336600003366330033666600336699003366cc003366ff00339900003399330033996600339999003399cc003399ff0033cc000033cc330033cc660033cc990033cccc0033ccff0033ff000033ff330033ff660033ff990033ffcc0033ffff00660000006600330066006600660099006600cc006600ff00663300006633330066336600663399006633cc006633ff00666600006666330066666600666699006666cc006666ff00669900006699330066996600669999006699cc006699ff0066cc000066cc330066cc660066cc990066cccc0066ccff0066ff000066ff330066ff660066ff990066ffcc0066ffff00990000009900330099006600990099009900cc009900ff00993300009933330099336600993399009933cc009933ff00996600009966330099666600996699009966cc009966ff00999900009999330099996600999999009999cc009999ff0099cc000099cc330099cc660099cc990099cccc0099ccff0099ff000099ff330099ff660099ff990099ffcc0099ffff00cc000000cc003300cc006600cc009900cc00cc00cc00ff00cc330000cc333300cc336600cc339900cc33cc00cc33ff00cc660000cc663300cc666600cc669900cc66cc00cc66ff00cc990000cc993300cc996600cc999900cc99cc00cc99ff00cccc0000cccc3300cccc6600cccc9900cccccc00ccccff00ccff0000ccff3300ccff6600ccff9900ccffcc00ccffff00ff000000ff003300ff006600ff009900ff00cc00ff00ff00ff330000ff333300ff336600ff339900ff33cc00ff33ff00ff660000ff663300ff666600ff669900ff66cc00ff66ff00ff990000ff993300ff996600ff999900ff99cc00ff99ff00ffcc0000ffcc3300ffcc6600ffcc9900ffcccc00ffccff00ffff0000ffff3300ffff6600ffff9900ffffcc00ffffff0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000400000034020000030000003500050000000b0200000000050000000c020c000a00df00000040092000cc00000000000c000a0000000000280000000a0000000c000000010018000000000080010000000000000000000000000000000000003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00002d000000f7000003140000000000800000000080000080800000000080008000800000808000c0c0c000c0dcc000a6caf000fffbf000a0a0a40080808000ff00000000ff0000ffff00000000ff00ff00ff0000ffff00ffffff00040000003402010004000000f0010000030000000000" +
			//    "</wmfFileString>" +
			//    "</image>";

			string xmlString =
				"<image>" +
				"<metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"827\" numOfObjects=\"2\" maxRecordSize=\"517\" numOfParams=\"0\" />\r\n" +
				"<metafileRecord size=\"5\" function=\"263\">03000000</metafileRecord>\r\n" +
				"<metafileRecord size=\"517\" function=\"247\">00030001000000000d0d0d001a1a1a00282828003535350043434300505050005d5d5d006b6b6b00787878008686860093939300a1a1a100aeaeae00bbbbbb00c9c9c900d6d6d600e4e4e400f1f1f100ffffff00000000000000330000006600000099000000cc000000ff00003300000033330000336600003399000033cc000033ff00006600000066330000666600006699000066cc000066ff00009900000099330000996600009999000099cc000099ff0000cc000000cc330000cc660000cc990000cccc0000ccff0000ff000000ff330000ff660000ff990000ffcc0000ffff00330000003300330033006600330099003300cc003300ff00333300003333330033336600333399003333cc003333ff00336600003366330033666600336699003366cc003366ff00339900003399330033996600339999003399cc003399ff0033cc000033cc330033cc660033cc990033cccc0033ccff0033ff000033ff330033ff660033ff990033ffcc0033ffff00660000006600330066006600660099006600cc006600ff00663300006633330066336600663399006633cc006633ff00666600006666330066666600666699006666cc006666ff00669900006699330066996600669999006699cc006699ff0066cc000066cc330066cc660066cc990066cccc0066ccff0066ff000066ff330066ff660066ff990066ffcc0066ffff00990000009900330099006600990099009900cc009900ff00993300009933330099336600993399009933cc009933ff00996600009966330099666600996699009966cc009966ff00999900009999330099996600999999009999cc009999ff0099cc000099cc330099cc660099cc990099cccc0099ccff0099ff000099ff330099ff660099ff990099ffcc0099ffff00cc000000cc003300cc006600cc009900cc00cc00cc00ff00cc330000cc333300cc336600cc339900cc33cc00cc33ff00cc660000cc663300cc666600cc669900cc66cc00cc66ff00cc990000cc993300cc996600cc999900cc99cc00cc99ff00cccc0000cccc3300cccc6600cccc9900cccccc00ccccff00ccff0000ccff3300ccff6600ccff9900ccffcc00ccffff00ff000000ff003300ff006600ff009900ff00cc00ff00ff00ff330000ff333300ff336600ff339900ff33cc00ff33ff00ff660000ff663300ff666600ff669900ff66cc00ff66ff00ff990000ff993300ff996600ff999900ff99cc00ff99ff00ffcc0000ffcc3300ffcc6600ffcc9900ffcccc00ffccff00ffff0000ffff3300ffff6600ffff9900ffffcc00ffffff000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000</metafileRecord>\r\n" +
				"<metafileRecord size=\"4\" function=\"564\">0000</metafileRecord>\r\n" +
				"<metafileRecord size=\"3\" function=\"53\"></metafileRecord>\r\n" +
				"<metafileRecord size=\"5\" function=\"523\">00000000</metafileRecord>\r\n" +
				"<metafileRecord size=\"5\" function=\"524\">0c000a00</metafileRecord>\r\n" +
				"<metafileRecord size=\"223\" function=\"2368\">2000cc00000000000c000a0000000000280000000a0000000c000000010018000000000080010000000000000000000000000000000000003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff0000</metafileRecord>\r\n" +
				"<metafileRecord size=\"45\" function=\"247\">0003140000000000800000000080000080800000000080008000800000808000c0c0c000c0dcc000a6caf000fffbf000a0a0a40080808000ff00000000ff0000ffff00000000ff00ff00ff0000ffff00ffffff00</metafileRecord>\r\n" +
				"<metafileRecord size=\"4\" function=\"564\">0100</metafileRecord>\r\n" +
				"<metafileRecord size=\"4\" function=\"496\">0000</metafileRecord>\r\n" +
				"<metafileRecord size=\"3\" function=\"0\"></metafileRecord>\r\n" +
				"</image>";

			IXmlElement element = new XmlElement(xmlString);
			image = new GraphicImage(element);

			Assert.AreEqual(1, Project.Images.Count);
			Assert.AreEqual("image1", image.Id);
		}

		[Test]
		public void ConstructFromRtfParserXml()
		{
			Assert.AreEqual(10, image.Width);
			Assert.AreEqual(12, image.Height);
			Assert.AreEqual("PNG", image.Format);

			Assert.AreEqual(10, image.Bitmap.Width);
			Assert.AreEqual(12, image.Bitmap.Height);
			Assert.AreEqual(PixelFormat.Format24bppRgb, image.Bitmap.PixelFormat);
		}

		[Test]
		public void ConstructFromProjectImageListXml()
		{
			string xmlString =
				"<imagedef id=\"1\">" +
				"<imagedata imageFormat=\"PNG\">" +
				"iVBORw0KGgoAAAANSUhEUgAAAAoAAAAMCAIAAADUCbv3AAAAAXNSR0IArs4c6QAAAARnQU1BAACx\r\n" +
				"jwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAABpJREFU\r\n" +
				"KFNj/F9ny4AHAKXxIIZRaWzhQyBYAFsXv6i+LNJ8AAAAAElFTkSuQmCC" +
				"</imagedata>" +
				"</imagedef>";

			IXmlElement element = new XmlElement(xmlString);
			GraphicImage image = new GraphicImageDefinition(element);

			Assert.AreEqual(10, image.Width);
			Assert.AreEqual(12, image.Height);
			Assert.AreEqual("PNG", image.Format);

			Assert.AreEqual(PixelFormat.Format24bppRgb, image.Bitmap.PixelFormat);
		}

		[Test]
		public void GetXml()
		{
			string expectedString =
				"<image id=\"image1\" width=\"10\" height=\"12\">" +
				"<metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"827\" numOfObjects=\"2\" maxRecordSize=\"517\" numOfParams=\"0\" />\r\n" +
				"</image>";

			Assert.AreEqual(expectedString, image.ToXml());
		}

		[Test]
		public void GetBitmapAsMetafileRecord()
		{
			MetafileRecord record = image.BitmapAsMetafileRecord;

			Assert.AreEqual(0xdf, record.Size);
			Assert.AreEqual(0x0940, record.Function);
			Assert.AreEqual(220, record.ParametersLength);

			int rowSize = (image.Bitmap.Width * 3);
			int imageSize = rowSize * image.Bitmap.Height;

			byte[] imageData = new byte[imageSize];
			for (int i = 0; i < imageSize; i++)
			{
				imageData[i] = record.Parameters[i + 56];
			}

			byte[] oneRow = { 0x3d, 0x7e, 0xff, 0x3d, 0x7e, 0xff, 0x3d, 0x7e, 0xff, 0x3d, 0x7e, 0xff, 0x3d, 0x7e, 0xff, 0x3d, 0x7e, 0xff, 0x3d, 0x7e, 0xff, 0x3d, 0x7e, 0xff, 0x3d, 0x7e, 0xff, 0x3d, 0x7e, 0xff };

			for (int y = 0; y < image.Bitmap.Height; y++)
			{
				for (int x = 0; x < rowSize; x++)
				{
					string errorString = string.Format("row = {0}, column = {1}", y, x);
					Assert.AreEqual(oneRow[x], imageData[x], errorString);
				}
			}
		}

		[Test]
		public void RowSizeCorrectForEvenImageWidths()
		{
			image = new GraphicImage();

			Assert.AreEqual(8, getRowSize(2));
			Assert.AreEqual(12, getRowSize(4));
			Assert.AreEqual(20, getRowSize(6));
		}

		[Test]
		public void RowSizeCorrectForOddImageWidths()
		{
			image = new GraphicImage();

			Assert.AreEqual(4, getRowSize(1));
			Assert.AreEqual(12, getRowSize(3));
			Assert.AreEqual(16, getRowSize(5));
		}

	}
}
