using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using Tawala.RtfSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the TestFormItem class
	/// </summary>
	[TestFixture]
	public class TextItemTest
	{
		private IForm form;
		private FibItem fibItem1;
		private Process process;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();

			fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			process = Project.Current.AddProcess();
			process.Variables.Add(new Variable("Variable 1"));
		}

		[Test]
		public void NewTextItem() 
		{ 
			TextItem item = new TextItem();

			Assert.IsNotNull(item);
			Assert.AreEqual(TestSupport.RtfConstants.TextItemDefaultRTF, item.Rtf);
		}

		[Test]
		public void XmlTagName()
		{
			TextItem item = new TextItem();

			string tagName = Reflect<TextItem>.GetProperty<string>("XmlTagName", item);
			Assert.AreEqual("text", tagName);
		}

		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Hello World!" +
				"</paragraph>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			TextItem item = new TextItem(element);

			Assert.AreEqual("Hello World!", item.Text);
		}


		[Test]
		public void ConstructFromXmlWithField()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<field name=\"Form 1:Q1:a\"/>" +
				"</paragraph>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			TextItem item = new TextItem(element);

			Assert.AreEqual("<<Form 1:Q1:a>>", item.Text);

			Assert.AreEqual(1, item.Contents.Count);
			Assert.IsInstanceOfType(typeof(Paragraph), item.Contents[0]);

			Paragraph paragraph = (Paragraph)item.Contents[0];

			Assert.IsInstanceOfType(typeof(FormItemNamedField), paragraph.Contents[0]);
			Assert.IsNotNull(((FormItemNamedField)paragraph.Contents[0]).Field);
			Assert.AreSame(fibItem1.BlankList[0], ((FormItemNamedField)paragraph.Contents[0]).Field);
		}

		[Test]
		public void ConstructFromXmlWithImage()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
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
				"</image>" +
				"</paragraph>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			TextItem item = new TextItem(element);

			Assert.AreEqual(1, item.Contents.Count);
			Assert.IsInstanceOfType(typeof(Paragraph), item.Contents[0]);

			Paragraph paragraph = (Paragraph)item.Contents[0];
			Assert.IsInstanceOfType(typeof(GraphicImage), paragraph.Contents[0]);

			GraphicImage image = (GraphicImage)paragraph.Contents[0];
			Assert.AreEqual(10, image.Width);
			Assert.AreEqual(12, image.Height);
		}

		[Test]
		public void ConstructFromXmlWithVariable()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<field name=\"Variable 1\"/>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			TextItem item = new TextItem(element);

			Assert.AreEqual("<<Variable 1>>", item.Text);

			Assert.AreEqual(1, item.Contents.Count);
			Assert.IsInstanceOfType(typeof(Paragraph), item.Contents[0]);

			Paragraph paragraph = (Paragraph)item.Contents[0];

			Assert.IsInstanceOfType(typeof(FormItemNamedField), paragraph.Contents[0]);
			Assert.IsNotNull(((FormItemNamedField)paragraph.Contents[0]).Field);
			Assert.AreSame(process.Variables[0], ((FormItemNamedField)paragraph.Contents[0]).Field);
		}

		[Test]
		public void ConstructFromXmlWithEntities()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Mom &amp; Pop" +
				"</paragraph>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			TextItem item = new TextItem(element);

			Assert.AreEqual("Mom & Pop", item.Text);
		}

		[Test]
		public void ConstructFromFontXml()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.BeginFont +
				"Hello World!" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			TextItem item = new TextItem(element);

			Assert.AreEqual("Hello World!", item.Text);
		}

		[Test]
		public void ConstructFromFontXmlWithField()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.BeginFont +
				"<field name=\"Form 1:Q1:a\"/>" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			TextItem item = new TextItem(element);

			Assert.AreEqual(1, item.Contents.Count);
			Assert.IsInstanceOfType(typeof(Paragraph), item.Contents[0]);

			Paragraph paragraph = (Paragraph)item.Contents[0];

			Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[0]);

			FontAttributes fontAttributes = (FontAttributes)paragraph.Contents[0];

			Assert.IsInstanceOfType(typeof(FormItemNamedField), fontAttributes.Contents);
			Assert.IsNotNull(((FormItemNamedField)fontAttributes.Contents).Field);
			Assert.AreSame(fibItem1.BlankList[0], ((FormItemNamedField)fontAttributes.Contents).Field);

			Assert.AreEqual("<<Form 1:Q1:a>>", item.Text);
		}

		[Test]
		public void GetXml()
		{
			TextItem item = new TextItem();

			item.Text = "Hello World!";

			string expString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Hello World!" +
				"</paragraph>" +
				"</text>\r\n";

			Assert.AreEqual(expString, item.ToXml("T1"));
		}

		[Test]
		public void GetXmlWithSpecialCharacters()
		{
			TextItem item = new TextItem();

			item.Text = "Mom & Pop";

			string expString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Mom &amp; Pop" +
				"</paragraph>" +
				"</text>\r\n";

			Assert.AreEqual(expString, item.ToXml("T1"));
		}

		[Test]
		public void GetXmlWithField()
		{
			TextItem item = new TextItem();

			item.Text = "Hello <<Form 1:Q1:a>>!";

			string expString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Hello <field name=\"Form 1:Q1:a\"/>!" +
				"</paragraph>" +
				"</text>\r\n";

			Assert.AreEqual(expString, item.ToXml("T1"));
		}

		[Test]
		public void GetXmlWithAlternateLabel()
		{
			TextItem item = new TextItem();
			item.AlternateLabel = "MyTextLabel";
			item.Text = "Hello World!";

			string expString =
				"<text label=\"T1\" alternateLabel=\"MyTextLabel\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Hello World!" +
				"</paragraph>" +
				"</text>\r\n";


			Assert.AreEqual(expString, item.ToXml("T1"));
		}

		[Test]
		public void IsTextItem() 
		{ 
			TextItem item = new TextItem();

			Assert.AreEqual(true, item.IsTextItem);
			Assert.AreEqual(false, item.IsQuestionItem);
		}

		[Test]
		public void GetXmlFromRtf()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + Environment.NewLine +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + Environment.NewLine +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + Environment.NewLine +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + Environment.NewLine +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + Environment.NewLine +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + Environment.NewLine +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0\fs20 Hello World!\par }";

			TextItem item = new TextItem();

			item.Rtf = rtfString;

			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Hello World!" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</text>\r\n";

			Assert.AreEqual(xmlString, item.ToXml("T1"));
		}

		private string rtfFieldString1 =
			RtfDocument.RtfStringPrefix +
			@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
			@"\itap0\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219";

		private string rtfFieldString2 =
			@"\txfielddataval{0}";

		private string rtfFieldString3 =
			@"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
			@"<<Form 1:Q1:a>>{" +
			@"\*\txfieldend}\par }";

		[Test]
		public void FieldRtfToObject()
		{
			string rtfString =
				rtfFieldString1 +
				string.Format(rtfFieldString2, fibItem1.BlankList[0].Id) +
				rtfFieldString3;

			TextItem item = new TextItem();
			item.Rtf = rtfString;

			Assert.AreEqual(1, item.Contents.Count);
			Assert.IsInstanceOfType(typeof(Paragraph), item.Contents[0]);

			Paragraph paragraph = (Paragraph)item.Contents[0];

			Assert.IsInstanceOfType(typeof(FormItemNamedField), paragraph.Contents[0]);
			Assert.IsNotNull(((FormItemNamedField)paragraph.Contents[0]).Field);
			Assert.AreSame(fibItem1.BlankList[0], ((FormItemNamedField)paragraph.Contents[0]).Field);
		}

		[Test]
		public void FieldRtfToRtf()
		{
			string rtfString =
				rtfFieldString1 +
				string.Format(rtfFieldString2, fibItem1.BlankList[0].Id) +
				rtfFieldString3;

			TextItem item = new TextItem();
			item.Rtf = rtfString;

			string expectedString =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard " +
				@"{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", fibItem1.BlankList[0].Id) +
				@"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
				@"<<Form 1:Q1:a>>{" +
				@"\*\txfieldend}\par }";

			Assert.AreEqual(expectedString, item.ToRtf());
		}

		[Test]
		public void FieldRtfToXml()
		{
			string rtfString =
				rtfFieldString1 +
				string.Format(rtfFieldString2, fibItem1.BlankList[0].Id) +
				rtfFieldString3;

			TextItem item = new TextItem();
			item.Rtf = rtfString;

			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<field name=\"Form 1:Q1:a\"/>" +
				"</paragraph>" +
				"</text>\r\n";

			Assert.AreEqual(xmlString, item.ToXml("T1"));
		}

		[Test]
		public void FontXmlToXml()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.BeginFont +
				"Text" +
				XmlConstants.EndFont +
				XmlConstants.BeginFont +
				"<field name=\"Form 1:Q1:a\"/>" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			TextItem item = new TextItem(element);

			Assert.AreEqual(xmlString, item.ToXml("T1"));
		}

		[Test]
		public void FontXmlToRtf()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">" +
				"Text" +
				XmlConstants.EndFont +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">" +
				"<field name=\"Form 1:Q1:a\"/>" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			TextItem item = new TextItem(element);

			string expectedString =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard " +
				@"{\f0\fs20\cf1 Text}" +
				@"{\f0\fs20\cf1 " +
				@"{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", fibItem1.BlankList[0].Id) +
				@"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
				@"<<Form 1:Q1:a>>{" +
				@"\*\txfieldend}" +
				@"}" +
				@"\par }";

			Assert.AreEqual(expectedString, item.ToRtf());
		}

		[Test]
		public void OldTabXmlToXml()
		{
			string oldXmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"Column1\tColumn2" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(oldXmlString);
			TextItem item = new TextItem(element);

			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Column1" +
				"<tab/>" +
				"Column2" +
				"</paragraph>" +
				"</text>\r\n";

			Assert.AreEqual(xmlString, item.ToXml("T1"));
		}

		[Test]
		public void OldTabXmlToRtf()
		{
			string oldXmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"Column1\tColumn2" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(oldXmlString);
			TextItem item = new TextItem(element);

			string expectedString =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard " +
				@"Column1\tab Column2" +
				@"\par }";

			Assert.AreEqual(expectedString, item.ToRtf());
		}

		[Test]
		public void OldNewlineXmlToXml()
		{
			string oldXmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"Paragraph1\n" +
				"Paragraph2" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(oldXmlString);
			TextItem item = new TextItem(element);

			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Paragraph1" +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Paragraph2" +
				"</paragraph>" +
				"</text>\r\n";

			Assert.AreEqual(xmlString, item.ToXml("T1"));
		}

		[Test]
		public void OldNewlineXmlToRtf()
		{
			string oldXmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"Paragraph1\n" +
				"Paragraph2" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(oldXmlString);
			TextItem item = new TextItem(element);

			string expectedString =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard Paragraph1\par " +
				@"\pard Paragraph2\par " +
				@"}";

			Assert.AreEqual(expectedString, item.ToRtf());
		}

		[Test]
		public void ImageRtfToRtf()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20{\pict\wmetafile8\picw265\pich318\picwgoal150" +
				@"\pichgoal180\picscalex100\picscaley100\blipupi96 0100090000033b03000002000502000000000500000007010300000005020000f70000030001000000000d0d0d001a1a1a00282828003535350043434300505050005d5d5d006b6b6b00787878008686860093939300a1a1a100aeaeae00bbbbbb00c9c9c900d6d6d600e4e4e400f1f1f100ffffff00000000000000330000006600000099000000cc000000ff00003300000033330000336600003399000033cc000033ff00006600000066330000666600006699000066cc000066ff00009900000099330000996600009999000099cc000099ff0000cc000000cc330000cc660000cc990000cccc0000ccff0000ff000000ff330000ff660000ff990000ffcc0000ffff00330000003300330033006600330099003300cc003300ff00333300003333330033336600333399003333cc003333ff00336600003366330033666600336699003366cc003366ff00339900003399330033996600339999003399cc003399ff0033cc000033cc330033cc660033cc990033cccc0033ccff0033ff000033ff330033ff660033ff990033ffcc0033ffff00660000006600330066006600660099006600cc006600ff00663300006633330066336600663399006633cc006633ff00666600006666330066666600666699006666cc006666ff00669900006699330066996600669999006699cc006699ff0066cc000066cc330066cc660066cc990066cccc0066ccff0066ff000066ff330066ff660066ff990066ffcc0066ffff00990000009900330099006600990099009900cc009900ff00993300009933330099336600993399009933cc009933ff00996600009966330099666600996699009966cc009966ff00999900009999330099996600999999009999cc009999ff0099cc000099cc330099cc660099cc990099cccc0099ccff0099ff000099ff330099ff660099ff990099ffcc0099ffff00cc000000cc003300cc006600cc009900cc00cc00cc00ff00cc330000cc333300cc336600cc339900cc33cc00cc33ff00cc660000cc663300cc666600cc669900cc66cc00cc66ff00cc990000cc993300cc996600cc999900cc99cc00cc99ff00cccc0000cccc3300cccc6600cccc9900cccccc00ccccff00ccff0000ccff3300ccff6600ccff9900ccffcc00ccffff00ff000000ff003300ff006600ff009900ff00cc00ff00ff00ff330000ff333300ff336600ff339900ff33cc00ff33ff00ff660000ff663300ff666600ff669900ff66cc00ff66ff00ff990000ff993300ff996600ff999900ff99cc00ff99ff00ffcc0000ffcc3300ffcc6600ffcc9900ffcccc00ffccff00ffff0000ffff3300ffff6600ffff9900ffffcc00ffffff0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000400000034020000030000003500050000000b0200000000050000000c020c000a00df00000040092000cc00000000000c000a0000000000280000000a0000000c000000010018000000000080010000000000000000000000000000000000003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00002d000000f7000003140000000000800000000080000080800000000080008000800000808000c0c0c000c0dcc000a6caf000fffbf000a0a0a40080808000ff00000000ff0000ffff00000000ff00ff00ff0000ffff00ffffff00040000003402010004000000f0010000030000000000}\par }";

			TextItem item = new TextItem();
			item.Rtf = rtfString;

			string expectedString =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard " +
				@"{\f0\fs20\cf1 " +
				@"{\pict\wmetafile8\picw265\pich318\picwgoal150" +
				@"\pichgoal180\picscalex100\picscaley100\blipupi96 0100090000033b03000002000502000000000500000007010300000005020000f70000030001000000000d0d0d001a1a1a00282828003535350043434300505050005d5d5d006b6b6b00787878008686860093939300a1a1a100aeaeae00bbbbbb00c9c9c900d6d6d600e4e4e400f1f1f100ffffff00000000000000330000006600000099000000cc000000ff00003300000033330000336600003399000033cc000033ff00006600000066330000666600006699000066cc000066ff00009900000099330000996600009999000099cc000099ff0000cc000000cc330000cc660000cc990000cccc0000ccff0000ff000000ff330000ff660000ff990000ffcc0000ffff00330000003300330033006600330099003300cc003300ff00333300003333330033336600333399003333cc003333ff00336600003366330033666600336699003366cc003366ff00339900003399330033996600339999003399cc003399ff0033cc000033cc330033cc660033cc990033cccc0033ccff0033ff000033ff330033ff660033ff990033ffcc0033ffff00660000006600330066006600660099006600cc006600ff00663300006633330066336600663399006633cc006633ff00666600006666330066666600666699006666cc006666ff00669900006699330066996600669999006699cc006699ff0066cc000066cc330066cc660066cc990066cccc0066ccff0066ff000066ff330066ff660066ff990066ffcc0066ffff00990000009900330099006600990099009900cc009900ff00993300009933330099336600993399009933cc009933ff00996600009966330099666600996699009966cc009966ff00999900009999330099996600999999009999cc009999ff0099cc000099cc330099cc660099cc990099cccc0099ccff0099ff000099ff330099ff660099ff990099ffcc0099ffff00cc000000cc003300cc006600cc009900cc00cc00cc00ff00cc330000cc333300cc336600cc339900cc33cc00cc33ff00cc660000cc663300cc666600cc669900cc66cc00cc66ff00cc990000cc993300cc996600cc999900cc99cc00cc99ff00cccc0000cccc3300cccc6600cccc9900cccccc00ccccff00ccff0000ccff3300ccff6600ccff9900ccffcc00ccffff00ff000000ff003300ff006600ff009900ff00cc00ff00ff00ff330000ff333300ff336600ff339900ff33cc00ff33ff00ff660000ff663300ff666600ff669900ff66cc00ff66ff00ff990000ff993300ff996600ff999900ff99cc00ff99ff00ffcc0000ffcc3300ffcc6600ffcc9900ffcccc00ffccff00ffff0000ffff3300ffff6600ffff9900ffffcc00ffffff0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000400000034020000030000003500050000000b0200000000050000000c020c000a00df00000040092000cc00000000000c000a0000000000280000000a0000000c000000010018000000000080010000000000000000000000000000000000003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00002d000000f7000003140000000000800000000080000080800000000080008000800000808000c0c0c000c0dcc000a6caf000fffbf000a0a0a40080808000ff00000000ff0000ffff00000000ff00ff00ff0000ffff00ffffff00040000003402010004000000f0010000030000000000}}\par }";

			Assert.AreEqual(expectedString, item.ToRtf());
		}

		private static void verifyObjectsForSingleFormatText(TextItem item, string expectedText, Type expectedFormatType)
		{
			Assert.AreEqual(1, item.Contents.Count);
			Assert.IsInstanceOfType(typeof(Paragraph), item.Contents[0]);

			Paragraph paragraph = (Paragraph)item.Contents[0];
			Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[0]);

			FontAttributes fontAttributes = (FontAttributes)paragraph.Contents[0];
			Assert.AreEqual(expectedText, item.Text);
			Assert.IsInstanceOfType(expectedFormatType, fontAttributes.Contents);
		}

		private string boldRtfString =
			RtfDocument.RtfStringPrefix +
			@"\b Bold text\par " +
			RtfDocument.RtfStringPostfix;

		private string boldXmlString =
			"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			XmlConstants.FullBeginFont +
			"<b>Bold text</b>" +
			XmlConstants.EndFont +
			"</paragraph>" +
			"</text>\r\n";

		[Test]
		public void ConstructFromFontXmlWithBoldText()
		{
			IXmlElement element = new XmlElement(boldXmlString);
			TextItem item = new TextItem(element);

			verifyObjectsForSingleFormatText(item, "Bold text", typeof(BoldText));
		}

		[Test]
		public void BoldTextRtfToObject()
		{
			TextItem item = new TextItem();
			item.Rtf = boldRtfString;

			verifyObjectsForSingleFormatText(item, "Bold text", typeof(BoldText));
		}

		[Test]
		public void BoldTextRtfToXml()
		{
			TextItem item = new TextItem();
			item.Rtf = boldRtfString;

			Assert.AreEqual(boldXmlString, item.ToXml("T1"));
		}

		[Test]
		public void BoldTextRtfToRtf()
		{
			TextItem item = new TextItem();
			item.Rtf = boldRtfString;
			
			string expectedRtf =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard " +
				@"{\f0\fs20\cf1 \b Bold text\b0 }\par }";

			Assert.AreEqual(expectedRtf, item.ToRtf());
		}

		private string italicRtfString =
			RtfDocument.RtfStringPrefix +
			@"\i Italic text\par " +
			RtfDocument.RtfStringPostfix;

		private string italicXmlString =
			"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			XmlConstants.FullBeginFont +
			"<i>Italic text</i>" +
			XmlConstants.EndFont +
			"</paragraph>" +
			"</text>\r\n";

		[Test]
		public void ConstructFromFontXmlWithItalicText()
		{
			IXmlElement element = new XmlElement(italicXmlString);
			TextItem item = new TextItem(element);

			verifyObjectsForSingleFormatText(item, "Italic text", typeof(ItalicText));
		}

		[Test]
		public void ItalicTextRtfToObject()
		{
			TextItem item = new TextItem();
			item.Rtf = italicRtfString;

			verifyObjectsForSingleFormatText(item, "Italic text", typeof(ItalicText));
		}

		[Test]
		public void ItalicTextRtfToXml()
		{
			TextItem item = new TextItem();
			item.Rtf = italicRtfString;

			Assert.AreEqual(italicXmlString, item.ToXml("T1"));
		}

		[Test]
		public void ItalicTextRtfToRtf()
		{
			TextItem item = new TextItem();
			item.Rtf = italicRtfString;

			string expectedRtf =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard " +
				@"{\f0\fs20\cf1 \i Italic text\i0 }\par }";

			Assert.AreEqual(expectedRtf, item.ToRtf());
		}

		private string underlineRtfString =
			RtfDocument.RtfStringPrefix +
			@"\ul Underline text\par " +
			RtfDocument.RtfStringPostfix;

		private string underlineXmlString =
			"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			XmlConstants.FullBeginFont +
			"<u>Underline text</u>" +
			XmlConstants.EndFont +
			"</paragraph>" +
			"</text>\r\n";

		[Test]
		public void ConstructFromFontXmlWithUnderlineText()
		{
			IXmlElement element = new XmlElement(underlineXmlString);
			TextItem item = new TextItem(element);

			verifyObjectsForSingleFormatText(item, "Underline text", typeof(UnderlineText));
		}

		[Test]
		public void UnderlineTextRtfToObject()
		{
			TextItem item = new TextItem();
			item.Rtf = underlineRtfString;

			verifyObjectsForSingleFormatText(item, "Underline text", typeof(UnderlineText));
		}

		[Test]
		public void UnderlineTextRtfToXml()
		{
			TextItem item = new TextItem();
			item.Rtf = underlineRtfString;

			Assert.AreEqual(underlineXmlString, item.ToXml("T1"));
		}

		[Test]
		public void UnderlineTextRtfToRtf()
		{
			TextItem item = new TextItem();
			item.Rtf = underlineRtfString;

			string expectedRtf =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard " +
				@"{\f0\fs20\cf1 \ul Underline text\ul0 }\par }";

			Assert.AreEqual(expectedRtf, item.ToRtf());
		}

		private string mixedFormatRtfString =
			RtfConstants.DefaultRtfPrologue +
			@"Plain text, \plain\f0\fs20\b now bold\plain\f0\fs20 , \plain\f0\fs20\i now italic, \plain\f0\fs20\b\i now combined\plain\f0\fs20 , back to plain.\par " +
			RtfDocument.RtfStringPostfix;

		private string mixedFormatXmlString =
			"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<tabPositions>" +
			"<tabStop position=\"2880\"/>" +
			"</tabPositions>" +
			XmlConstants.FullBeginFont +
			"Plain text, " +
			XmlConstants.EndFont +
			XmlConstants.FullBeginFont +
			"<b>now bold</b>" +
			XmlConstants.EndFont +
			XmlConstants.FullBeginFont +
			", " +
			XmlConstants.EndFont +
			XmlConstants.FullBeginFont +
			"<i>now italic, </i>" +
			XmlConstants.EndFont +
			XmlConstants.FullBeginFont +
			"<b><i>now combined</i></b>" +
			XmlConstants.EndFont +
			XmlConstants.FullBeginFont +
			", back to plain." +
			XmlConstants.EndFont +
			"</paragraph>" +
			"</text>\r\n";

		private static void verifyObjectsForMixedFormatText(TextItem item)
		{
			Assert.AreEqual(1, item.Contents.Count);
			Assert.IsInstanceOfType(typeof(Paragraph), item.Contents[0]);

			Paragraph paragraph = (Paragraph)item.Contents[0];
			Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[0]);

			Assert.AreEqual(6, paragraph.Contents.Count);
			Assert.AreEqual(6, paragraph.Contents.Count);
			Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[0]);
			Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[1]);
			Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[2]);
			Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[3]);
			Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[4]);
			Assert.IsInstanceOfType(typeof(FontAttributes), paragraph.Contents[5]);

			Type[] types = new Type[]
			{
				typeof(BoldText), typeof(ItalicText), typeof(DocumentText)
			};

			FontAttributes fontAttributes = (FontAttributes)paragraph.Contents[4];
			int i = 0;

			foreach (IParagraphComponent component in fontAttributes.Contents.RecursiveEnumerator)
			{
				Assert.AreEqual(types[i++], component.GetType());
			}
		}

		[Test]
		public void ConstructFromFontXmlWithMixedFormats()
		{
			IXmlElement element = new XmlElement(mixedFormatXmlString);
			TextItem item = new TextItem(element);

			verifyObjectsForMixedFormatText(item);
		}

		[Test]
		public void MixedFormatRtfToObject()
		{
			TextItem item = new TextItem();
			item.Rtf = mixedFormatRtfString;

			verifyObjectsForMixedFormatText(item);
		}

		[Test]
		public void MixedFormatRtfToXml()
		{
			TextItem item = new TextItem();
			item.Rtf = mixedFormatRtfString;

			Assert.AreEqual(mixedFormatXmlString, item.ToXml("T1"));
		}

		[Test]
		public void MixedFormatRtfToRtf()
		{
			TextItem item = new TextItem();
			item.Rtf = mixedFormatRtfString;

			string expectedRtf =
				RtfConstants.BasicRtfThemePrologue +
				@"\pard " +
				@"\tx2880" +
				@"{\f0\fs20\cf1 Plain text, }{\f0\fs20\cf1 \b now bold\b0 }{\f0\fs20\cf1 , }{\f0\fs20\cf1 \i now italic, \i0 }{\f0\fs20\cf1 \b \i now combined\i0 \b0 }{\f0\fs20\cf1 , back to plain.}\par }";

			Assert.AreEqual(expectedRtf, item.ToRtf());
		}

		[Test]
		public void UnformattedXmlFromUnformattedXmlWithSpace()
		{
			string xmlString =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				" " +
				"</paragraph>" +
				"</text>\r\n";

			IXmlElement element = new XmlElement(xmlString, true);
			TextItem item = new TextItem(element);

			Assert.AreEqual(xmlString, item.ToXml("T1"));
		}
	}
}
