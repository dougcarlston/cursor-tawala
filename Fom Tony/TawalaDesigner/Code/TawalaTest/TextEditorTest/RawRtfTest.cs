using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace TawalaTest.TextEditorTest
{
	[TestFixture]
	public class RawRtfTest : TextEditTestBase
	{
		[Test]
		public void EmptyDocument()
		{
			string playgroundAppRtf = 
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" + 
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" + 
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" + 
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" + 
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" + 
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" + 
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" + 
				@"\fs20\par }";

			string rtf = editor.GetRTF();
			Assert.AreEqual(playgroundAppRtf, rtf);
		}

		private static readonly string preRoundTrip = 
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs20 This is \plain\f0\fs20\b bold\plain\f0\fs20  and not bold\par }";

		private string postRoundTrip =
			preRoundTrip.Replace(@"and not bold\par }", @"and not bold\par }");

		[Test]
		public void Anomaly_RoundTrip_Final_1par_To_2par()
		{
			editor.SetRTF(preRoundTrip);
			string rtf = editor.GetRTF();
			Assert.AreEqual(postRoundTrip, rtf);
		}

		[Test]
		public void Anomaly_RoundTrips_Final_1par_To_2par()
		{
			editor.SetRTF(preRoundTrip);
			string rtf = editor.GetRTF();
			editor.SetRTF(rtf);
			rtf = editor.GetRTF();
			Assert.AreEqual(postRoundTrip, rtf);
		}

		[Test]
		public void Anomaly_UsesMixedCaseCommandsIn_Tables()
		{
			string playgroundAppTable = 
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs20\par\trowd\irow0\irowband0\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trp" +
				@"addr36\trpaddfl3\trpaddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth5400\cellx5400\clvertal" +
				@"t\clftsWidth3\clwWidth5400\cellx10800\pard\intbl\cell\cell\row\trowd\irow1\irowband1\trgaph36\trleft" +
				@"0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpaddft3\trpaddfr3\trpaddfb3\" +
				@"clvertalt\clftsWidth3\clwWidth5400\cellx5400\clvertalt\clftsWidth3\clwWidth5400\cellx10800\pard\intb" +
				@"l\cell\cell\row\trowd\irow2\irowband2\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl" +
				@"36\trpaddr36\trpaddfl3\trpaddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth5400\cellx5400\cl" +
				@"vertalt\clftsWidth3\clwWidth5400\cellx10800\pard\intbl\cell\cell\row\trowd\irow3\irowband3\lastrow\t" +
				@"rgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpaddft3\trpadd" +
				@"fr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth5400\cellx5400\clvertalt\clftsWidth3\clwWidth5400\cellx1" +
				@"0800\pard\intbl\cell\cell\row\pard\itap0 }";

			editor.InsertTable(7.5 * 100.0, 4, 2);
			string rtf = editor.GetRTF();
			Assert.AreEqual(playgroundAppTable, rtf);
		}
		
		[Test]
		public void BoldText()
		{
			editor.Selection.Text = "This is ";
            editor.Selection.Bold = Tawala.TextEditor.Tristate.True;
			editor.Selection.Text = "bold";
            editor.Selection.Bold = Tawala.TextEditor.Tristate.False;
			editor.Selection.Text = " and not bold";

			string rtf = editor.GetRTF();
			Assert.AreEqual(preRoundTrip, rtf);
		}

		private static readonly string rtfBIULPrefix = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
			@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
			@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
			@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
			@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
			@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
			@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0\fs20 ";

		private string b0Rtf =
			rtfBIULPrefix + @"This is \plain\f0\fs20\b bold\b0  and not bold\par }";

		private string b0RtfRoundTrip =
			rtfBIULPrefix + @"This is \plain\f0\fs20\b bold\plain\f0\fs20  and not bold\par }";

		[Test]
		public void Supports_b0()
		{
			editor.SetRTF(b0Rtf);
			string rtf = editor.GetRTF();
			Assert.AreEqual(b0RtfRoundTrip, rtf);
		}

		private string i0Rtf =
			rtfBIULPrefix + @"This is \plain\f0\fs20\i italic\i0  and not italic\par }";

		private string i0RtfRoundTrip =
			rtfBIULPrefix + @"This is \plain\f0\fs20\i italic\plain\f0\fs20  and not italic\par }";

		[Test]
		public void Supports_i0()
		{
			editor.SetRTF(i0Rtf);
			string rtf = editor.GetRTF();
			Assert.AreEqual(i0RtfRoundTrip, rtf);
		}

		private string ul0Rtf =
			rtfBIULPrefix + @"This is \plain\f0\fs20\ul underlined\ul0  and not underlined\par }";

		private string ul0RtfRoundTrip =
			rtfBIULPrefix + @"This is \plain\f0\fs20\ul underlined\plain\f0\fs20  and not underlined\par }";

		[Test]
		public void Supports_ul0()
		{
			editor.SetRTF(ul0Rtf);
			string rtf = editor.GetRTF();
			Assert.AreEqual(ul0RtfRoundTrip, rtf);
		}

		[Test]
		public void Supports_FormatNesting()
		{
			string prefix = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0\fs20 ";

			string nestedString = prefix +
				@"Plain {\b Bold {\i BoldItalic {\ul BoldItalicUnderline }}Bold }Plain\par\par }";

			string rtfString = prefix + 
				@"Plain \plain\f0\fs20\b Bold \plain\f0\fs20\b\i BoldItalic \plain\f0\fs20\b\i\ul BoldItalicUnde" +
				@"rline \plain\f0\fs20\b Bold \plain\f0\fs20 Plain\par }";

			editor.SetRTF(nestedString);
			string rtf = editor.GetRTF();
			Assert.AreEqual(rtfString, rtf);
		}

		[Test]
		public void ChangeTabStops()
		{
			// tabs are in twips; make them start at 0.5 inch and increase from there
			// the amount of tabs is always 14, here we got lucky, otherwise undefined
			// tabs at the end of the array would be 0.

			int[] tabs = new int[14]
				{ 720, 1440, 2160, 2880, 3600, 4320, // 0.5, 1.0, 1.5, 2.0, 2.5, 3.0
				5040, 5760, 6480, 7200, 7920, 8640,  // 3.5, 4.0, 4.5, 5.0, 5.5, 6.0
				9360, 10080 };						 // 6.5, 7.0

			txTextControl.ParagraphFormat.TabPositions = tabs;

			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760" +
				@"\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080\plain\f0\fs20" +
				@"\par }";

			Assert.AreEqual(rtfString, editor.GetRTF());
		}
	}
}
