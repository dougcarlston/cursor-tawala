// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Images
{
    /// <summary>
    /// Acceptance tests for story 1060 (Images in Documents).
    /// </summary>
    [TestFixture]
    public class ImageInDocumentTest1060
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            addImageToProject();
        }

        #endregion

        private const string NEWLINE = "\r\n";

        private const string rtfString =
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

        private void addImageToProject()
        {
            string imageDefString =
                "<imagedef id=\"image1\">" +
                "<imagedata imageFormat=\"PNG\">" +
                "iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAIAAAD91JpzAAAABGdBTUEAALGPC/xhBQAAABFJREFU" +
                "GFdjZGD4zwAGQOo/ABAMAv9DCFk5AAAAAElFTkSuQmCC" +
                "</imagedata>" +
                "</imagedef>";

            IXmlElement definitionElement = new XmlElement(imageDefString);
            Project.Images.AddUnique(new GraphicImageDefinition(definitionElement));
        }

        [Test]
        public void ImageRtfProducesImageRtf()
        {
            var document = new RtfDocument("Document 1");
            document.Rtf = rtfString;

            string expectedRtf =
                RtfConstants.BasicRtfPrologue +
                @"\pard {\f0\fs20\cf1 {\pict\wmetafile8\picw53\pich53\picwgoal30\pichgoal30\picscalex100\picscaley100\blipupi96 0100090000038302000002000502000000000500000007010300000005020000f70000030001000000000d0d0d001a1a1a00282828003535350043434300505050005d5d5d006b6b6b00787878008686860093939300a1a1a100aeaeae00bbbbbb00c9c9c900d6d6d600e4e4e400f1f1f100ffffff00000000000000330000006600000099000000cc000000ff00003300000033330000336600003399000033cc000033ff00006600000066330000666600006699000066cc000066ff00009900000099330000996600009999000099cc000099ff0000cc000000cc330000cc660000cc990000cccc0000ccff0000ff000000ff330000ff660000ff990000ffcc0000ffff00330000003300330033006600330099003300cc003300ff00333300003333330033336600333399003333cc003333ff00336600003366330033666600336699003366cc003366ff00339900003399330033996600339999003399cc003399ff0033cc000033cc330033cc660033cc990033cccc0033ccff0033ff000033ff330033ff660033ff990033ffcc0033ffff00660000006600330066006600660099006600cc006600ff00663300006633330066336600663399006633cc006633ff00666600006666330066666600666699006666cc006666ff00669900006699330066996600669999006699cc006699ff0066cc000066cc330066cc660066cc990066cccc0066ccff0066ff000066ff330066ff660066ff990066ffcc0066ffff00990000009900330099006600990099009900cc009900ff00993300009933330099336600993399009933cc009933ff00996600009966330099666600996699009966cc009966ff00999900009999330099996600999999009999cc009999ff0099cc000099cc330099cc660099cc990099cccc0099ccff0099ff000099ff330099ff660099ff990099ffcc0099ffff00cc000000cc003300cc006600cc009900cc00cc00cc00ff00cc330000cc333300cc336600cc339900cc33cc00cc33ff00cc660000cc663300cc666600cc669900cc66cc00cc66ff00cc990000cc993300cc996600cc999900cc99cc00cc99ff00cccc0000cccc3300cccc6600cccc9900cccccc00ccccff00ccff0000ccff3300ccff6600ccff9900ccffcc00ccffff00ff000000ff003300ff006600ff009900ff00cc00ff00ff00ff330000ff333300ff336600ff339900ff33cc00ff33ff00ff660000ff663300ff666600ff669900ff66cc00ff66ff00ff990000ff993300ff996600ff999900ff99cc00ff99ff00ffcc0000ffcc3300ffcc6600ffcc9900ffcccc00ffccff00ffff0000ffff3300ffff6600ffff9900ffffcc00ffffff0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000400000034020000030000003500050000000b0200000000050000000c02020002002700000040092000cc0000000000020002000000000028000000020000000200000001001800000000001000000000000000000000000000000000000000ff0000ff00000000ff0000ff000000002d000000f7000003140000000000800000000080000080800000000080008000800000808000c0c0c000c0dcc000a6caf000fffbf000a0a0a40080808000ff00000000ff0000ffff00000000ff00ff00ff0000ffff00ffffff00040000003402010004000000f0010000030000000000}}\par }";

            Console.WriteLine(document.Rtf);
            Assert.AreEqual(expectedRtf, document.Rtf);
        }

        [Test]
        public void ImageRtfProducesImageXml()
        {
            var document = new RtfDocument("Document 1");
            document.Rtf = rtfString;

            string expectedXml =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "<image id=\"image1\" width=\"2\" height=\"2\">" +
                "<metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"643\" numOfObjects=\"2\" maxRecordSize=\"517\" numOfParams=\"0\" />\r\n" +
                "</image>" +
                XmlConstants.EndFont +
                "</paragraph>\r\n" +
                "</xmlData>\r\n" +
                "</document>\r\n";

            Console.WriteLine(document.ToXml());
            Assert.AreEqual(expectedXml, document.ToXml());
        }

        [Test]
        public void ImageXmlProducesImageRtf()
        {
            string xmlString =
                "<document name=\"Document 1\">" +
                "<xmlData>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "<image id=\"image1\" width=\"2\" height=\"2\">" +
                "<metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"69\" numOfObjects=\"0\" maxRecordSize=\"42\" numOfParams=\"0\" />" +
                "</image>" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</xmlData>" +
                "</document>";

            var document = new RtfDocument(new XmlElement(xmlString));

            string actualRtf = document.Rtf;

            int index = 0;
            string rtfPriorToRGBData =
                RtfConstants.BasicRtfThemePrologue +
                @"\pard {\f0\fs20\cf1 " +
                @"{\pict\wmetafile8\picw53\pich53\picwgoal30\pichgoal30\picscalex100\picscaley100\blipupi96 " +
                "0100090000034500000000002a00000000002700000040092000cc0000000000020002000000000028000000020000000200000001001800000000001000000000000000000000000000000000000000";

            Assert.AreEqual(rtfPriorToRGBData, actualRtf.Substring(index, rtfPriorToRGBData.Length));

            // padding bytes of rgb data can contain garbage; ignore them
            string unpaddedRgbData = "ff0000ff0000";
            int paddingLength = 4;
            index += rtfPriorToRGBData.Length;
            for (int y = 0; y < 2; y++)
            {
                Assert.AreEqual(unpaddedRgbData, actualRtf.Substring(index, unpaddedRgbData.Length));
                index += unpaddedRgbData.Length + paddingLength;
            }

            string endOfMetafileRecord = "030000000000" + RtfConstants.DefaultRtfEpilogue;
            Assert.AreEqual(endOfMetafileRecord, actualRtf.Substring(index, endOfMetafileRecord.Length));
            index += endOfMetafileRecord.Length;

            Assert.AreEqual(@"}\par }", actualRtf.Substring(index));
        }

        [Test]
        public void ImageXmlProducesImageXml()
        {
            string xmlString =
                "<document name=\"Document 1\">" +
                "<xmlData>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.BeginFont +
                "<image id=\"image1\" width=\"2\" height=\"2\">" +
                "<metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"69\" numOfObjects=\"0\" maxRecordSize=\"42\" numOfParams=\"0\" />" +
                "</image>" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</xmlData>" +
                "</document>";

            var document = new RtfDocument(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, xmlString);
        }

        [Test]
        public void ProjectConversionProducesProperDocumentXml()
        {
            string xmlString =
                "<project name=\"Story1060Test\" themePath=\"default\" format=\"1.4\">" +
                "<documents>" +
                "<document name=\"Document 1\">" +
                "<xmlData>" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.BeginFont +
                "<image id=\"image1\" width=\"2\" height=\"2\">" +
                "<metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"69\" numOfObjects=\"0\" maxRecordSize=\"42\" numOfParams=\"0\" />" +
                "</image>" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</xmlData>" +
                "</document>" +
                "</documents>" +
                "<images>" +
                "<imagedef id=\"image1\">" +
                "<imagedata imageFormat=\"PNG\">" +
                "iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAIAAAD91JpzAAAABGdBTUEAALGPC/xhBQAAABFJREFU" +
                "GFdjZGD4zwAGQOo/ABAMAv9DCFk5AAAAAElFTkSuQmCC" +
                "</imagedata>" +
                "</imagedef>" +
                "</images>" +
                "</project>";

            using (var xmlStream = new MemoryStream())
            {
                byte[] xmlByteArray = Encoding.UTF8.GetBytes(xmlString);
                xmlStream.Write(xmlByteArray, 0, xmlByteArray.Length);

                var converter = new TawalaProjectConverter(xmlStream);
                converter.ConvertXmlToProject();
            }

            var document = (Document)Project.Current.DocumentList[0];

            string expectedXml =
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>\r\n" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.BeginFont +
                "<image id=\"image1\" width=\"2\" height=\"2\">" +
                "<metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"69\" numOfObjects=\"0\" maxRecordSize=\"42\" numOfParams=\"0\" />\r\n" +
                "</image>" +
                XmlConstants.EndFont +
                "</paragraph>\r\n" +
                "</xmlData>\r\n" +
                "</document>\r\n";

            Console.WriteLine(document.ToXml());
            Assert.AreEqual(expectedXml, document.ToXml());
        }
    }
}