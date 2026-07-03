using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;


namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 330 (Image Data Remains in XML after Image is removed).
	/// </summary>
	[TestFixture]
	public class ImageDataRemains330
	{
		private IForm form;
		private TextItem textItem;

		string orange2x2BlockImageRtfString =
			@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
			@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
			@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
			@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
			@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
			@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
			@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
			@"\itap0\plain\f0\fs24{\pict\wmetafile8\picw25\pich25\picwgoal15" +
			@"\pichgoal15\picscalex100\picscaley100\blipupi200 010009000003830200000200050200000000050000000701030" +
			@"0000005020000f70000030001000000000d0d0d001a1a1a00282828003535350043434300505050005d5d5d006b6b6b00787" +
			@"878008686860093939300a1a1a100aeaeae00bbbbbb00c9c9c900d6d6d600e4e4e400f1f1f100ffffff00000000000000330" +
			@"000006600000099000000cc000000ff00003300000033330000336600003399000033cc000033ff000066000000663300006" +
			@"66600006699000066cc000066ff00009900000099330000996600009999000099cc000099ff0000cc000000cc330000cc660" +
			@"000cc990000cccc0000ccff0000ff000000ff330000ff660000ff990000ffcc0000ffff00330000003300330033006600330" +
			@"099003300cc003300ff00333300003333330033336600333399003333cc003333ff003366000033663300336666003366990" +
			@"03366cc003366ff00339900003399330033996600339999003399cc003399ff0033cc000033cc330033cc660033cc990033c" +
			@"ccc0033ccff0033ff000033ff330033ff660033ff990033ffcc0033ffff00660000006600330066006600660099006600cc0" +
			@"06600ff00663300006633330066336600663399006633cc006633ff00666600006666330066666600666699006666cc00666" +
			@"6ff00669900006699330066996600669999006699cc006699ff0066cc000066cc330066cc660066cc990066cccc0066ccff0" +
			@"066ff000066ff330066ff660066ff990066ffcc0066ffff00990000009900330099006600990099009900cc009900ff00993" +
			@"300009933330099336600993399009933cc009933ff00996600009966330099666600996699009966cc009966ff009999000" +
			@"09999330099996600999999009999cc009999ff0099cc000099cc330099cc660099cc990099cccc0099ccff0099ff000099f" +
			@"f330099ff660099ff990099ffcc0099ffff00cc000000cc003300cc006600cc009900cc00cc00cc00ff00cc330000cc33330" +
			@"0cc336600cc339900cc33cc00cc33ff00cc660000cc663300cc666600cc669900cc66cc00cc66ff00cc990000cc993300cc9" +
			@"96600cc999900cc99cc00cc99ff00cccc0000cccc3300cccc6600cccc9900cccccc00ccccff00ccff0000ccff3300ccff660" +
			@"0ccff9900ccffcc00ccffff00ff000000ff003300ff006600ff009900ff00cc00ff00ff00ff330000ff333300ff336600ff3" +
			@"39900ff33cc00ff33ff00ff660000ff663300ff666600ff669900ff66cc00ff66ff00ff990000ff993300ff996600ff99990" +
			@"0ff99cc00ff99ff00ffcc0000ffcc3300ffcc6600ffcc9900ffcccc00ffccff00ffff0000ffff3300ffff6600ffff9900fff" +
			@"fcc00ffffff00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
			@"0000000000000000000000000000000000000000000000000000000000000000000000000040000003402000003000000350" +
			@"0050000000b0200000000050000000c02020002002700000040092000cc00000000000200020000000000280000000200000" +
			@"002000000010018000000000010000000000000000000000000000000000000003f7fff3f7fff00003f7fff3f7fff00002d0" +
			@"00000f7000003140000000000800000000080000080800000000080008000800000808000c0c0c000c0dcc000a6caf000fff" +
			@"bf000a0a0a40080808000ff00000000ff0000ffff00000000ff00ff00ff0000ffff00ffffff0004000000340201000400000" +
			@"0f0010000030000000000}\par }";

		string blue2x2BlockImageRtfString =
			@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
			@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
			@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
			@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
			@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
			@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
			@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
			@"\itap0\plain\f0\fs24{\pict\wmetafile8\picw53\pich53\picwgoal30" +
			@"\pichgoal30\picscalex100\picscaley100\blipupi96 0100090000038302000002000502000000000500000007010300" +
			@"000005020000f70000030001000000000d0d0d001a1a1a00282828003535350043434300505050005d5d5d006b6b6b007878" +
			@"78008686860093939300a1a1a100aeaeae00bbbbbb00c9c9c900d6d6d600e4e4e400f1f1f100ffffff000000000000003300" +
			@"00006600000099000000cc000000ff00003300000033330000336600003399000033cc000033ff0000660000006633000066" +
			@"6600006699000066cc000066ff00009900000099330000996600009999000099cc000099ff0000cc000000cc330000cc6600" +
			@"00cc990000cccc0000ccff0000ff000000ff330000ff660000ff990000ffcc0000ffff003300000033003300330066003300" +
			@"99003300cc003300ff00333300003333330033336600333399003333cc003333ff0033660000336633003366660033669900" +
			@"3366cc003366ff00339900003399330033996600339999003399cc003399ff0033cc000033cc330033cc660033cc990033cc" +
			@"cc0033ccff0033ff000033ff330033ff660033ff990033ffcc0033ffff00660000006600330066006600660099006600cc00" +
			@"6600ff00663300006633330066336600663399006633cc006633ff00666600006666330066666600666699006666cc006666" +
			@"ff00669900006699330066996600669999006699cc006699ff0066cc000066cc330066cc660066cc990066cccc0066ccff00" +
			@"66ff000066ff330066ff660066ff990066ffcc0066ffff00990000009900330099006600990099009900cc009900ff009933" +
			@"00009933330099336600993399009933cc009933ff00996600009966330099666600996699009966cc009966ff0099990000" +
			@"9999330099996600999999009999cc009999ff0099cc000099cc330099cc660099cc990099cccc0099ccff0099ff000099ff" +
			@"330099ff660099ff990099ffcc0099ffff00cc000000cc003300cc006600cc009900cc00cc00cc00ff00cc330000cc333300" +
			@"cc336600cc339900cc33cc00cc33ff00cc660000cc663300cc666600cc669900cc66cc00cc66ff00cc990000cc993300cc99" +
			@"6600cc999900cc99cc00cc99ff00cccc0000cccc3300cccc6600cccc9900cccccc00ccccff00ccff0000ccff3300ccff6600" +
			@"ccff9900ccffcc00ccffff00ff000000ff003300ff006600ff009900ff00cc00ff00ff00ff330000ff333300ff336600ff33" +
			@"9900ff33cc00ff33ff00ff660000ff663300ff666600ff669900ff66cc00ff66ff00ff990000ff993300ff996600ff999900" +
			@"ff99cc00ff99ff00ffcc0000ffcc3300ffcc6600ffcc9900ffcccc00ffccff00ffff0000ffff3300ffff6600ffff9900ffff" +
			@"cc00ffffff000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
			@"0000000000000000000000000000000000000000000000000000000000000000000000000400000034020000030000003500" +
			@"050000000b0200000000050000000c02020002002700000040092000cc000000000002000200000000002800000002000000" +
			@"0200000001001800000000001000000000000000000000000000000000000000ff0000ff00000000ff0000ff000000002d00" +
			@"0000f7000003140000000000800000000080000080800000000080008000800000808000c0c0c000c0dcc000a6caf000fffb" +
			@"f000a0a0a40080808000ff00000000ff0000ffff00000000ff00ff00ff0000ffff00ffffff00040000003402010004000000" +
			@"f0010000030000000000}\par }";

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			Project.Current.Name = "TestProject";

			form = Project.Current.AddForm();

			textItem = new TextItem();
			form.ItemList.Add(textItem);

			textItem.Rtf = orange2x2BlockImageRtfString;
		}
		
		[Test]
		public void ImageRtfYieldsImageDefinitionXml()
		{
			string expectedXml =
				"<project name=\"TestProject\" themePath=\"default\" format=\"" + Project.XmlFormatVersion + "\" designerBuild=\"0\">\r\n" +
				"<pageHeader></pageHeader>" +
				"<forms>\r\n" +
				"<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" blockBackButton=\"false\">\r\n" +
				"<items>\r\n" +
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"<image id=\"image1\" width=\"2\" height=\"2\">" +
				"<metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"643\" numOfObjects=\"2\" maxRecordSize=\"517\" numOfParams=\"0\" />\r\n" +
				"</image>" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</text>\r\n" +
				"</items>\r\n" +
				"</form>\r\n" +
				"</forms>\r\n" +
				"<images>" +
				"<imagedef id=\"image1\">" +
				"<imagedata imageFormat=\"PNG\">" +
				"iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAIAAAD91JpzAAAABGdBTUEAALGPC/xhBQAAABFJREFU\r\n" +
				"GFdj/F9vzwAEQAqIACNjBTkbFEBEAAAAAElFTkSuQmCC" +
				"</imagedata>" +
				"</imagedef>" +
				"</images>" +
				"</project>\r\n";

			Assert.AreEqual(expectedXml, Project.Current.ToXml());
		}

		[Test]
		public void DeletingTextItemRemovesImageDefinitionXml()
		{
			const string expectedXml = "<project name=\"TestProject\" themePath=\"default\" format=\"" + Project.XmlFormatVersion + "\" designerBuild=\"0\">\r\n" +
			                           "<pageHeader></pageHeader>" +
			                           "<forms>\r\n" +
			                           "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" blockBackButton=\"false\">\r\n" +
			                           "</form>\r\n" +
			                           "</forms>\r\n" +
			                           "</project>\r\n";

			form.ItemList.Remove(textItem);

			Assert.AreEqual(expectedXml, Project.Current.ToXml());
		}

		[Test]
		public void CreatingDuplicateTextItemsMaintainsImageListCount()
		{
			TextItem textItem2 = new TextItem();
			form.ItemList.Add(textItem2);

			textItem2.Rtf = orange2x2BlockImageRtfString;

			Assert.AreEqual(1, Project.Images.Count);

			TextItem textItem3 = new TextItem();
			form.ItemList.Add(textItem3);

			textItem3.Rtf = orange2x2BlockImageRtfString;

			Assert.AreEqual(1, Project.Images.Count);
		}

		[Test]
		public void TwoImageRtfYieldsTwoImageDefinitionXml()
		{
			TextItem textItem2 = new TextItem();
			form.ItemList.Add(textItem2);

			textItem2.Rtf = blue2x2BlockImageRtfString;

			Assert.AreEqual(twoImageProjectXml, Project.Current.ToXml());
		}

		private string xmlVersion =
			"<?xml version=\"1.0\" encoding=\"utf-8\" ?>";

		private string twoImageProjectXml =
			"<project name=\"TestProject\" themePath=\"default\" format=\"" + Project.XmlFormatVersion + "\" designerBuild=\"0\">\r\n" +
			"<pageHeader></pageHeader>" +
			"<forms>\r\n" +
            "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" blockBackButton=\"false\">\r\n" +
			"<items>\r\n" +
			"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			XmlConstants.FullBeginFont +
			"<image id=\"image1\" width=\"2\" height=\"2\">" +
			"<metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"643\" numOfObjects=\"2\" maxRecordSize=\"517\" numOfParams=\"0\" />\r\n" +
			"</image>" +
			XmlConstants.EndFont +
			"</paragraph>" +
			"</text>\r\n" +
			"<text label=\"T2\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			XmlConstants.FullBeginFont +
			"<image id=\"image2\" width=\"2\" height=\"2\">" +
			"<metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"643\" numOfObjects=\"2\" maxRecordSize=\"517\" numOfParams=\"0\" />\r\n" +
			"</image>" +
			XmlConstants.EndFont +
			"</paragraph>" +
			"</text>\r\n" +
			"</items>\r\n" +
			"</form>\r\n" +
			"</forms>\r\n" +
			"<images>" +
			"<imagedef id=\"image1\">" +
			"<imagedata imageFormat=\"PNG\">" +
			"iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAIAAAD91JpzAAAABGdBTUEAALGPC/xhBQAAABFJREFU\r\n" +
			"GFdj/F9vzwAEQAqIACNjBTkbFEBEAAAAAElFTkSuQmCC" +
			"</imagedata>" +
			"</imagedef>" +
			"<imagedef id=\"image2\">" +
			"<imagedata imageFormat=\"PNG\">" +
			"iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAIAAAD91JpzAAAABGdBTUEAALGPC/xhBQAAABFJREFU\r\n" +
			"GFdjZGD4zwAGQOo/ABAMAv9DCFk5AAAAAElFTkSuQmCC" +
			"</imagedata>" +
			"</imagedef>" +
			"</images>" +
			"</project>\r\n";

		private void newProject(string xmlString)
		{
			string tempFileName = Path.GetTempFileName();

			try
			{
				File.WriteAllText(tempFileName, xmlVersion + xmlString);
				Project.Open(tempFileName);
			}
			finally
			{
				File.Delete(tempFileName);
			}
		}

		[Test]
		public void ProjectXmlYieldsProjectXml()
		{
			newProject(twoImageProjectXml);

			Assert.AreEqual(twoImageProjectXml, Project.Current.ToXml());
		}

		[Test]
		public void SaveAndReopenProjectYieldsSameProjectXml()
		{
			newProject(twoImageProjectXml);

			Project.Save(TestSupport.Util.GetTestFilePath("TestProject.xml"));
			Project.Open(TestSupport.Util.GetTestFilePath("TestProject.xml"));
			Project.Save(TestSupport.Util.GetTestFilePath("TestProject.xml"));
			Project.Open(TestSupport.Util.GetTestFilePath("TestProject.xml"));

			Assert.AreEqual(twoImageProjectXml, Project.Current.ToXml());
		}


	}
}
