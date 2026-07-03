using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Tawala.Projects;
using Tawala.Forms;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TXTextControl;
using NUnit.Framework;
//using NUnit.Extensions.Forms;

namespace TawalaTest.FormsTest
{
	[TestFixture]
	public class ItemTextEditorTest// : NUnitFormTest
	{
		//private class ItemTextEditorTester : NUnit.Extensions.Forms.ControlTester
		//{
		//    public ItemTextEditorTester(string name) : base(name)
		//    {
		//    }
		//}

		private ItemTextEditor textEditor;
		//private ItemTextEditorTester textEditorTester;

		private TXTextControl.TextControl txTextControl;
		//private ControlTester txTextControlTester;

		private TXTextControl.TextControl getTxControl(object o)
		{
			Type t = typeof(Tawala.TextEditor.TextEdit);
			FieldInfo fi = t.GetField("txTextControl", BindingFlags.Instance | BindingFlags.NonPublic);
			return fi.GetValue(o) as TXTextControl.TextControl;
		}

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

		[SetUp]
		public /*override*/ void Setup()
		{
			//base.Setup();

			System.Windows.Forms.Form windowsForm = new System.Windows.Forms.Form();

			textEditor = new ItemTextEditor();
			textEditor.Name = "testTextEditor";
			windowsForm.Controls.Add(textEditor);

			//textEditorTester = new ItemTextEditorTester("testTextEditor");

			windowsForm.Show();

			txTextControl = getTxControl(textEditor);
			//txTextControlTester = new NUnit.Extensions.Forms.ControlTester(txTextControl.Name);
		}

		[Test]
		public void TextEditorPreservesNativeImageRtf()
		{
			textEditor.SetRTF(orange2x2BlockImageRtfString);

			Assert.AreEqual(orange2x2BlockImageRtfString, textEditor.GetRTF());
		}

		[Test]
		public void TextEditorPreservesProjectImageRtf()
		{
			TextItem textItem = new TextItem();
			textItem.Rtf = orange2x2BlockImageRtfString;
			textEditor.SetRTF(textItem.Rtf);

			string rtfString1 = textEditor.GetRTF();

			textItem.Rtf = rtfString1;
			textEditor.SetRTF(textItem.Rtf);

			string rtfString2 = textEditor.GetRTF();

			Console.WriteLine(rtfString2);
			Assert.AreEqual(rtfString1, rtfString2);
		}

	}
}
