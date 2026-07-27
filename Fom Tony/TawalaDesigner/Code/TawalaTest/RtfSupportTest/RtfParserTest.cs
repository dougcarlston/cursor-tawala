using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using NUnit.Framework;
using Tawala.XmlSupport;
using Tawala.RtfSupport;

namespace TawalaTest.RtfSupportTest
{
	[TestFixture]
	public class RtfParserTest
	{
		private const string NEWLINE = "\r\n";

		private string rtfPrefixString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + NEWLINE +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + NEWLINE +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + NEWLINE +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + NEWLINE +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + NEWLINE +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + NEWLINE;

		private const string tenPointFontStartTag = "<font face=\"Arial\" size=\"200\" color=\"000000\">";
		private const string fontEndTag = "</font>";

		[Test]
		public void BasicTokens()
		{
			string rtfString = @"{\pard \{text\}}";

			RtfParser parser = new RtfParser(rtfString);

			string[] expectedTokenStrings = new string[] {
				@"{",
				@"\pard",
				@"{",
				@"text",
				@"}",
				@"}"
			};

			Collection<RtfToken> tokens = parser.Tokens;

			Assert.AreEqual(6, tokens.Count);

			for (int i = 0; i < expectedTokenStrings.Length; i++)
			{
				Assert.AreEqual(expectedTokenStrings[i], tokens[i].ToString());
			}
		}

		[Test]
		public void BasicTokensToXml()
		{
			string rtfString = @"{\ignoredcommand\pard text\par}";

			RtfParser parser = new RtfParser(rtfString);

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"text" +
				"</paragraph>";

			parser.Parse();
			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void BackslashToXml()
		{
			string rtfString = @"{\pard text with backslash \\\par}";

			RtfParser parser = new RtfParser(rtfString);

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				@"text with backslash \" +
				"</paragraph>";

			Console.WriteLine(expectedString);

			parser.Parse();
			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void Groups()
		{
			string rtfString =
				@"{\rtf1\ansi\deff0" +
				@"{\fonttbl{\f0\fswiss\fcharset0\fprq2 Arial;}}" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();
			Assert.AreEqual(3, parser.GroupCount);
		}

		[Test]
		public void FontTable()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard text\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			Assert.AreEqual(2, parser.FontTable.Count);

			Assert.AreEqual("swiss", parser.FontTable[0].FontFamily);
			Assert.AreEqual("Arial", parser.FontTable[0].FontName);

			Assert.AreEqual("roman", parser.FontTable[1].FontFamily);
			Assert.AreEqual("Symbol", parser.FontTable[1].FontName);
		}

		[Test]
		public void PlainTextToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard text\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"text" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}
		
		[Test]
		public void BoldTextToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\b Bold text\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<b>Bold text</b>" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void BoldandPlainTextToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\b Bold text \plain\f0\fs20 and not bold\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<b>Bold text </b>" +
				tenPointFontStartTag +
				"and not bold" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void EmptyBoldTextToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard Plain text with empty\b \plain\f0\fs20  bold tags in the middle.\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"Plain text with empty" +
				tenPointFontStartTag +
				" bold tags in the middle." +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void ItalicTextToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\i Italicized text \plain\f0\fs20 and not italicized\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<i>Italicized text </i>" +
				tenPointFontStartTag +
				"and not italicized" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void UnderlinedTextToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\ul Underlined text \plain\f0\fs20 and not underlined\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<u>Underlined text </u>" +
				tenPointFontStartTag +
				"and not underlined" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}
		
		[Test]
		public void UnderlinedTextEndingBeforeSpaceToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\ul Underlined;  text\plain\f0\fs20  and not underlined!\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<u>Underlined;  text</u>" +
				tenPointFontStartTag +
				" and not underlined!" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void BoldAndItalicTextCombinedToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\itap0\sb2\sa2\plain\f0\fs20\b\i Bold and italics combined \plain\f0\fs20 and not\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"<b><i>Bold and italics combined </i></b>" +
				fontEndTag +
				tenPointFontStartTag +
				"and not" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void BoldAndItalicTextOverlappingToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\itap0\sb2\sa2\plain\f0\fs20\b Bold text \plain\f0\fs20\b\i and \plain\f0\fs20\i italic text \plain\f0\fs20 overlapping\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"<b>Bold text </b>" +
				fontEndTag +
				tenPointFontStartTag +
				"<b><i>and </i></b>" +
				fontEndTag +
				tenPointFontStartTag +
				"<i>italic text </i>" +
				fontEndTag +
				tenPointFontStartTag +
				"overlapping" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void UnderlinedAndBoldTextNestedToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\itap0\sb2\sa2\plain\f0\fs20 Here are \plain\f0\fs20\ul underlined and \plain\f0\fs20\b\ul bold text \plain\f0\fs20\ul nested \plain\f0\fs20 in a line\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Here are " +
				fontEndTag +
				tenPointFontStartTag +
				"<u>underlined and </u>" +
				fontEndTag +
				tenPointFontStartTag +
				"<b><u>bold text </u></b>" +
				fontEndTag +
				tenPointFontStartTag +
				"<u>nested </u>" +
				fontEndTag +
				tenPointFontStartTag +
				"in a line" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void FormattingAcrossParagraphsToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\itap0\sb2\sa2\plain\f0\fs20 Here is some \plain\f0\fs20\b bold text and some \plain\f0\fs20\b\i italic text\par " +
				@"Italic text \plain\f0\fs20\b and bold text \plain\f0\fs20 extend to the next paragraph\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Here is some " +
				fontEndTag +
				tenPointFontStartTag +
				"<b>bold text and some </b>" +
				fontEndTag +
				tenPointFontStartTag +
				"<b><i>italic text</i></b>" +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"<b><i>Italic text </i></b>" +
				fontEndTag +
				tenPointFontStartTag +
				"<b>and bold text </b>" +
				fontEndTag +
				tenPointFontStartTag +
				"extend to the next paragraph" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void CloseFormattingInFinalParagraphToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\itap0\sb2\sa2\plain\f0\fs20\b Bold and \plain\f0\fs20\b\i italics in the last paragraph\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"<b>Bold and </b>" +
				fontEndTag +
				tenPointFontStartTag +
				"<b><i>italics in the last paragraph</i></b>" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void SpecialCharactersToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\itap0\plain\f0\fs20 Text with \{RTF\} command* characters\par" +
				@"}";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Text with " +
				fontEndTag +
				tenPointFontStartTag +
				"{" +
				fontEndTag +
				tenPointFontStartTag +
				"RTF" +
				fontEndTag +
				tenPointFontStartTag +
				"}" +
				fontEndTag +
				tenPointFontStartTag +
				" command* characters" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void Fields()
		{
			string rtfString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs20 Here is a Form field <<Q1:a>> and and a variable <<Variable>>.\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Here is a Form field &lt;&lt;Q1:a&gt;&gt; and and a variable &lt;&lt;Variable&gt;&gt;." +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void ParagraphAlignment()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\itap0\plain\f0" +
				@"\fs20 Left aligned paragraph.\par\pard\itap0\qc Centered paragraph.\par\pard\itap0\qr Right aligned " +
				@"paragraph.\par\pard\itap0\qj Justified paragraph.\par\pard\itap0 Left aligned paragraph.\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Left aligned paragraph." +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"center\">" +
				tenPointFontStartTag +
				"Centered paragraph." +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"right\">" +
				tenPointFontStartTag +
				"Right aligned paragraph." +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"justify\">" +
				tenPointFontStartTag +
				"Justified paragraph." +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Left aligned paragraph." +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void ParagraphIndent()
		{
			string rtfString =
				rtfPrefixString +
				@"\pard\itap0\plain\f0" +
				@"\fs20 No indentation.\par\pard\itap0\li720 Indent level one.\par\pard\itap0\li1440 Indent level two." +
				@"\par\pard\itap0 No indentation.\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"No indentation." +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"720\" align=\"left\">" +
				tenPointFontStartTag +
				"Indent level one." +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"1440\" align=\"left\">" +
				tenPointFontStartTag +
				"Indent level two." +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"No indentation." +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}
		
		[Test]
		public void BlankLinesBetweenParagraphs()
		{
			string rtfString =
				rtfPrefixString +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20" +
				@" Line one." +
				@"\par\par" +
				@" Line two after 2 CRs." +
				@"\par\par\par" +
				@" Line three after 3 CRs." +
				@"\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Line one." +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" + "</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Line two after 2 CRs." +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" + "</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" + "</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Line three after 3 CRs." +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}
		
		[Test]
		public void DefaultFontFaceToXml()
		{
			string rtfString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs20 Default Font\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Default Font" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void NonDefaultFontFaceToXml()
		{
			string rtfString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\fmodern\fcharset0\fprq1 Courier New;}" + "\r\n" +
				@"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f1" +
				@"\fs20 Non-default Font\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font face=\"Courier New\" size=\"200\" color=\"000000\">" +
				"Non-default Font" +
				"</font>" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void MixedFontFacesToXml()
		{
			string rtfString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\fmodern\fcharset0\fprq1 Courier New;}" + "\r\n" +
				@"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs20 Default \plain\f1\fs20 Non-default \plain\f0\fs20 Default\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Default " +
				fontEndTag +
				"<font face=\"Courier New\" size=\"200\" color=\"000000\">" +
				"Non-default " +
				"</font>" +
				tenPointFontStartTag +
				"Default" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}


		[Test]
		public void NonDefaultFontSizeToXml()
		{
			string rtfString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs20 Non-default Font Size\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Non-default Font Size" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void DefaultFontSizeToXml()
		{
			string rtfString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs24 Default Font Size\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"Default Font Size" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void MixedFontSizesToXml()
		{
			string rtfString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs20 Non-default \plain\f0\fs24 Default \plain\f0\fs20 Non-default\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Non-default " +
				fontEndTag +
				"Default " +
				tenPointFontStartTag +
				"Non-default" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}


		[Test]
		public void ColorTable()
		{
			string rtfString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red255\green0\blue0;\red0\green255\blue0;\re" +
				@"d0\green0\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs20 Black \plain\f0\fs20\cf3 Red\plain\f0\fs20  \plain\f0\fs20\cf4 Green\plain\f0\fs20  \plain\f0\" +
				@"fs20\cf5 Blue\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			Assert.AreEqual(5, parser.ColorTable.Count);

			Assert.AreEqual(0, parser.ColorTable[0].Red);
			Assert.AreEqual(0, parser.ColorTable[0].Green);
			Assert.AreEqual(0, parser.ColorTable[0].Blue);

			Assert.AreEqual(255, parser.ColorTable[1].Red);
			Assert.AreEqual(255, parser.ColorTable[1].Green);
			Assert.AreEqual(255, parser.ColorTable[1].Blue);

			Assert.AreEqual(255, parser.ColorTable[2].Red);
			Assert.AreEqual(0, parser.ColorTable[2].Green);
			Assert.AreEqual(0, parser.ColorTable[2].Blue);

			Assert.AreEqual(0, parser.ColorTable[3].Red);
			Assert.AreEqual(255, parser.ColorTable[3].Green);
			Assert.AreEqual(0, parser.ColorTable[3].Blue);

			Assert.AreEqual(0, parser.ColorTable[4].Red);
			Assert.AreEqual(0, parser.ColorTable[4].Green);
			Assert.AreEqual(255, parser.ColorTable[4].Blue);

			parser.Parse();
			Assert.AreEqual(5, parser.ColorTable.Count);
		}

		[Test]
		public void FontColorToXml()
		{
			string rtfString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red255\green0\blue0;\red0\green255\blue0;\re" +
				@"d0\green0\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs20 Black \plain\f0\fs20\cf3 Red \plain\f0\fs20 \plain\f0\fs20\cf4 Green \plain\f0\fs20 \plain\f0\" +
				@"fs20\cf5 Blue\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Black " +
				fontEndTag +
				"<font face=\"Arial\" size=\"200\" color=\"FF0000\">" +
				"Red " +
				"</font>" +
				"<font face=\"Arial\" size=\"200\" color=\"00FF00\">" +
				"Green " +
				"</font>" +
				"<font face=\"Arial\" size=\"200\" color=\"0000FF\">" +
				"Blue" +
				"</font>" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}


		[Test]
		public void FontColorAndBoldToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs20\b\cf3 Bold \plain\f0\fs20\cf3 Plain Blue\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font face=\"Arial\" size=\"200\" color=\"0000FF\">" +
				"<b>" +
				"Bold " +
				"</b>" +
				"</font>" +
				"<font face=\"Arial\" size=\"200\" color=\"0000FF\">" +
				"Plain Blue" +
				"</font>" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void TableToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0" +
				@"\fs20\par\trowd\irow0\irowband0\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trp" +
				@"addr36\trpaddfl3\trpaddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth5000\cellx5400\clvertal" +
				@"t\clftsWidth3\clwWidth5800\cellx10800\pard\intbl Cell 1\cell Cell 2\cell\row\trowd\irow1\irowband1\l" +
				@"astrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpaddft" +
				@"3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth5000\cellx5400\clvertalt\clftsWidth3\clwWidth580" +
				@"0\cellx10800\pard\intbl Cell 3\cell Cell 4\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			Assert.AreEqual(2, parser.CellWidths.Count);

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>" +
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"5000\">" +
				"<division indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Cell 1" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"<cell width=\"5800\">" +
				"<division indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Cell 2" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"<row>" +
				"<cell width=\"5000\">" +
				"<division indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Cell 3" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"<cell width=\"5800\">" +
				"<division indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Cell 4" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void TableFollowedByTextToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd" +
				@"\irow0\irowband0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36" +
				@"\trpaddr36\trpaddfl3\trpaddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800" +
				@"\pard\intbl\plain\f0\fs20" +
				@" Table with one cell." +
				@"\cell\row\pard\itap0" +
				@" Text immediately following the table." +
				@"\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Table with one cell." +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Text immediately following the table." +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void BoldAndPlainTextInCellToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\fmodern\fcharset0\fprq1 Courier New;}" + "\r\n" +
				@"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
				@"d0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpa" +
				@"ddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800\pard\intbl\plain\f0\fs20\b " +
				@"Bold Text \plain\f0\fs20 Plain Text\plain\f1\fs20\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"<b>Bold Text </b>" +
				fontEndTag +
				tenPointFontStartTag +
				"Plain Text" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void MultipleDivisionsPerCellToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
				@"d0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpa" +
				@"ddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800\pard\intbl\plain\f0\fs20 Di" +
				@"vision 1\par Division 2\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Division 1" +
				fontEndTag +
				"</division>" +
				"<division indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Division 2" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void MultipleDivisionsWithNonDefaultFontFaceToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\fmodern\fcharset0\fprq1 Courier New;}" + "\r\n" +
				@"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
				@"d0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpa" +
				@"ddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800\pard\intbl\plain\f1\fs20 Di" +
				@"vision 1\plain\f0\fs20\par Division 2\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"0\" align=\"left\">" +
				"<font face=\"Courier New\" size=\"200\" color=\"000000\">" +
				"Division 1" +
				"</font>" +
				"</division>" +
				"<division indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Division 2" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void TableIndentToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
				@"d0\lastrow\trgaph36\trleft1425\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\t" +
				@"rpaddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth9375\cellx10800\pard\intbl\plain\f0\fs20 " +
				@"Indented\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"1425\">" +
				"<row>" +
				"<cell width=\"9375\">" +
				"<division indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"Indented" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void TableCellIndentToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
				@"d0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpa" +
				@"ddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800\pard\intbl\li720\plain\f0\f" +
				@"s20 Indented\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"720\" align=\"left\">" +
				tenPointFontStartTag +
				"Indented" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void TableCellAlignCenterToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
				@"d0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpa" +
				@"ddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800\pard\intbl\qc\plain\f0\fs20" +
				@" Center\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"0\" align=\"center\">" +
				tenPointFontStartTag +
				"Center" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void TableCellAlignRightToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
				@"d0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpa" +
				@"ddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800\pard\intbl\qr\plain\f0\fs20" +
				@" Right\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"0\" align=\"right\">" +
				tenPointFontStartTag +
				"Right" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void TableCellAlignJustifyToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
				@"d0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpa" +
				@"ddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800\pard\intbl\qj\plain\f0\fs20" +
				@" Justify\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"0\" align=\"justify\">" +
				tenPointFontStartTag +
				"Justify" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void TableFontColorToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red255\green0\blue0;\red0\green255\blue0;\re" +
				@"d0\green0\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd\irow0\irowban" +
				@"d0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36\trpaddr36\trpaddfl3\trpa" +
				@"ddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800\pard\intbl\plain\f0\fs20\cf" +
				@"3 Red \plain\f0\fs20\cf4 Green \plain\f0\fs20\cf5 Blue\plain\f0\fs20\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"0\" align=\"left\">" +
				"<font face=\"Arial\" size=\"200\" color=\"FF0000\">" +
				"Red " +
				"</font>" +
				"<font face=\"Arial\" size=\"200\" color=\"00FF00\">" +
				"Green " +
				"</font>" +
				"<font face=\"Arial\" size=\"200\" color=\"0000FF\">" +
				"Blue" +
				"</font>" +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void TableCellWithNoTextToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd" +
				@"\irow0\irowband0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36" +
				@"\trpaddr36\trpaddfl3\trpaddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800" +
				@"\pard\intbl\plain\f0\fs20\cell\row\pard\itap0" +
				@"\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void TableCellWithSpaceToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd" +
				@"\irow0\irowband0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36" +
				@"\trpaddr36\trpaddfl3\trpaddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800" +
				@"\pard\intbl\plain\f0\fs20" +
				@"  " +
				@"\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"0\" align=\"left\">" +
				"<sp/>" +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		string rtfTabPositionsString =
			@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
			@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
			@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
			@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
			@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
			@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
			@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\tx720\tx" +
			@"2160\plain\f0\fs20 Text1\tab Text2\tab Text3\par }";

		[Test]
		public void TabPositions()
		{
			RtfParser parser = new RtfParser(rtfTabPositionsString);
			parser.Parse();

			Assert.AreEqual(2, parser.TabPositions.Count);
			Assert.AreEqual(720, parser.TabPositions[0]);
			Assert.AreEqual(2160, parser.TabPositions[1]);
		}

		[Test]
		public void TabPositionsToXml()
		{
			RtfParser parser = new RtfParser(rtfTabPositionsString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<tabPositions>" +
				"<tabStop position=\"720\"/>" +
				"<tabStop position=\"2160\"/>" +
				"</tabPositions>" +
				tenPointFontStartTag +
				"Text1" +
				fontEndTag +
				"<tab/>" +
				tenPointFontStartTag +
				"Text2" +
				fontEndTag +
				"<tab/>" +
				tenPointFontStartTag +
				"Text3" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void ResetTabPositionsToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\tx720\tx" +
				@"2160\plain\f0\fs20 Text1\tab Text2\tab Text3\par\pard\itap0\tx1440\tx2880 Text4\tab Text5\tab Text6\" +
				@"par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<tabPositions>" +
				"<tabStop position=\"720\"/>" +
				"<tabStop position=\"2160\"/>" +
				"</tabPositions>" +
				tenPointFontStartTag +
				"Text1" +
				fontEndTag +
				"<tab/>" +
				tenPointFontStartTag +
				"Text2" +
				fontEndTag +
				"<tab/>" +
				tenPointFontStartTag +
				"Text3" +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<tabPositions>" +
				"<tabStop position=\"1440\"/>" +
				"<tabStop position=\"2880\"/>" +
				"</tabPositions>" +
				tenPointFontStartTag +
				"Text4" +
				fontEndTag +
				"<tab/>" +
				tenPointFontStartTag +
				"Text5" +
				fontEndTag +
				"<tab/>" +
				tenPointFontStartTag +
				"Text6" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void SpaceToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20 " +
				@" " +
				@"\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<sp/>" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void BoldAndItalicSeparatedBySpacesToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20\b" +
				@" Bold" +
				@"\plain\f0\fs20 " +
				@"  " +
				@"\plain\f0\fs20\i" +
				@" Italic" +
				@"\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"<b>Bold</b>" +
				fontEndTag +
				"<sp/><sp/>" +
				tenPointFontStartTag +
				"<i>Italic</i>" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void MultipleSpacesToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20" +
				@" One two  three   four    five     done." +
				@"\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"One two  three   four    five     done." +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void InheritedTabPositionsToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\tx720\tx" +
				@"2880\plain\f0\fs20 Text1\tab Text2\tab Text3\par Text4\tab Text5\tab Text6\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<tabPositions>" +
				"<tabStop position=\"720\"/>" +
				"<tabStop position=\"2880\"/>" +
				"</tabPositions>" +
				tenPointFontStartTag +
				"Text1" +
				fontEndTag +
				"<tab/>" +
				tenPointFontStartTag +
				"Text2" +
				fontEndTag +
				"<tab/>" +
				tenPointFontStartTag +
				"Text3" +
				fontEndTag +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<tabPositions>" +
				"<tabStop position=\"720\"/>" +
				"<tabStop position=\"2880\"/>" +
				"</tabPositions>" +
				tenPointFontStartTag +
				"Text4" +
				fontEndTag +
				"<tab/>" +
				tenPointFontStartTag +
				"Text5" +
				fontEndTag +
				"<tab/>" +
				tenPointFontStartTag +
				"Text6" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void TabWithNonDefaultFontFaceToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\fmodern\fcharset0\fprq1 Courier New;}" + "\r\n" +
				@"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f1\fs20" +
				@" Courier" +
				@"\tab" +
				@" New" +
				@"\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font face=\"Courier New\" size=\"200\" color=\"000000\">" +
				"Courier" +
				"</font>" +
				"<tab/>" +
				"<font face=\"Courier New\" size=\"200\" color=\"000000\">" +
				"New" +
				"</font>" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void FieldToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags216\txfielddataval3141" +
				@"\txfielddata 540046002400510031003a0061000000}" +
				@"<<Q1:a>>{" +
				@"\*\txfieldend}\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<field name=\"Q1:a\" id=\"3141\"/>" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void BoldFieldToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs24\b{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				@"\txfielddataval1898\txfielddata 540046002400510031003a0061000000}" +
				@"<<Q1:a>>{" +
				@"\*\txfieldend}\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<b>" +
				"<field name=\"Q1:a\" id=\"1898\"/>" +
				"</b>" +
				"</paragraph>";

			Console.WriteLine(parser.ToXml());
			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void NonDefaultFontSizeFieldToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags216\txfielddataval3141" +
				@"\txfielddata 540046002400510031003a0061000000}" +
				@"<<Q1:a>>{" +
				@"\*\txfieldend}\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"<field name=\"Q1:a\" id=\"3141\"/>" +
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void FieldInTableToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd" +
				@"\irow0\irowband0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36" +
				@"\trpaddr36\trpaddfl3\trpaddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800" +
				@"\pard\intbl\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags216" +
				@"\txfielddataval2\txfielddata 540046002400510031003a0061000000}" +
				@"<<Q1:a>>{" +
				@"\*\txfieldend}\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"0\" align=\"left\">" +
				"<field name=\"Q1:a\" id=\"2\"/>" +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void NonDefaultFontSizeFieldInTableToXml()
		{
			string rtfString =
				rtfPrefixString +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\trowd" +
				@"\irow0\irowband0\lastrow\trgaph36\trleft0\trftsWidth1\trftsWidthB3\trftsWidthA3\trpaddl36" +
				@"\trpaddr36\trpaddfl3\trpaddft3\trpaddfr3\trpaddfb3\clvertalt\clftsWidth3\clwWidth10800\cellx10800" +
				@"\pard\intbl\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags216" +
				@"\txfielddataval2\txfielddata 540046002400510031003a0061000000}" +
				@"<<Q1:a>>{" +
				@"\*\txfieldend}\cell\row\pard\itap0\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<table indent=\"0\">" +
				"<row>" +
				"<cell width=\"10800\">" +
				"<division indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"<field name=\"Q1:a\" id=\"2\"/>" +
				fontEndTag +
				"</division>" +
				"</cell>" +
				"</row>" +
				"</table>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}

		[Test]
		public void ImageToXml()
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

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedXml =
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"<image w=\"265\" h=\"318\" wgoal=\"150\" hgoal=\"180\" scalex=\"100\" scaley=\"100\" upi=\"96\">" +
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
				fontEndTag +
				"</paragraph>";

			Assert.AreEqual(expectedXml, parser.ToXml());
		}

		[Test]
		public void InvitationFieldToXml()
		{
			int id = 5001;
			string encryptedInvitationData = RtfUtility.EncodeHexString(@"IF$" + id);

			string rtfString =
				rtfPrefixString +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags216" +
				@"\txfielddataval" + id +
				@"\txfielddata " + encryptedInvitationData + "}" +
				@"Click here{" +
				@"\*\txfieldend}\par }";

			RtfParser parser = new RtfParser(rtfString);
			parser.Parse();

			string expectedString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<invitation id=\"5001\"/>" +
				"</paragraph>";

			Assert.AreEqual(expectedString, parser.ToXml());
		}
	}
}
