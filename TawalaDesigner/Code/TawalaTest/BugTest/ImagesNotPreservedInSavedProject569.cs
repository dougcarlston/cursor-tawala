using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class ImagesNotPreservedInSavedProject569
	{
		private string projectXmlString = 
			"<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine +
			"<project name=\"Untitled\" themePath=\"default\" format=\"1.6\">" + Environment.NewLine +
			"<documents>" + Environment.NewLine +
			"<document name=\"Document 1\">" + Environment.NewLine +
			"<xmlData>" + Environment.NewLine +
			"<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font face=\"Arial\" size=\"200\" color=\"000000\"><image id=\"image1\" width=\"10\" height=\"12\"><metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"253\" numOfObjects=\"0\" maxRecordSize=\"226\" numOfParams=\"0\" />" + Environment.NewLine +
			"</image></font></paragraph>" + Environment.NewLine +
			"</xmlData>" + Environment.NewLine +
			"</document>" + Environment.NewLine +
			"</documents>" + Environment.NewLine +
			"<images><imagedef id=\"image1\"><imagedata imageFormat=\"PNG\">iVBORw0KGgoAAAANSUhEUgAAAAoAAAAMCAIAAADUCbv3AAAABGdBTUEAALGPC/xhBQAAABpJREFU" + Environment.NewLine +
			"KFNj/F9ny4AHAKXxIIZRaWzhQyBYAFsXv6i+LNJ8AAAAAElFTkSuQmCC</imagedata></imagedef></images></project>";

		[Test]
		public void WmfImageDataIncludesEndOfMetafileRecord()
		{
			using (MemoryStream xmlStream = new MemoryStream())
			{
				byte[] xmlByteArray = System.Text.Encoding.UTF8.GetBytes(projectXmlString);
				xmlStream.Write(xmlByteArray, 0, xmlByteArray.Length);

				TawalaProjectConverter converter = new TawalaProjectConverter(xmlStream);
				converter.ConvertXmlToProject();
			}

			RtfDocument document = (RtfDocument)Project.Current.DocumentList[0];
			string actualRtf = document.ToRtf();

			int index = 0;
			string rtfPriorToRGBData = 
				RtfConstants.BasicRtfThemePrologue +
				@"\pard \tx2880{\f0\fs20\cf1 " +
				@"{\pict\wmetafile8\picw265\pich318\picwgoal150\pichgoal180\picscalex100\picscaley100\blipupi96 " +
				"010009000003fd0000000000e20000000000df00000040092000cc00000000000c000a0000000000280000000a0000000c00000001001800000000008001000000000000000000000000000000000000";

			Assert.AreEqual(rtfPriorToRGBData, actualRtf.Substring(index, rtfPriorToRGBData.Length));

			// padding bytes of rgb data can contain garbage; ignore them
			string unpaddedRgbData = "3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff";
			int paddingLength = 4;
			index += rtfPriorToRGBData.Length;
			for (int y = 0; y < 12; y++)
			{
				Assert.AreEqual(unpaddedRgbData, actualRtf.Substring(index, unpaddedRgbData.Length));
				index += unpaddedRgbData.Length + paddingLength;
			}

			// THIS is what we're looking for; without this record, image will not appear
			// in TX control after Windows Security Update for Windows XP (KB938829) 
			string endOfMetafileRecord = "030000000000" + RtfConstants.DefaultRtfEpilogue;
			Assert.AreEqual(endOfMetafileRecord, actualRtf.Substring(index, endOfMetafileRecord.Length));
			index += endOfMetafileRecord.Length;

			Assert.AreEqual(@"}\par }", actualRtf.Substring(index));
		}
	}
}
