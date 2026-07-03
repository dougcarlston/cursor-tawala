// $Workfile: TextSelectionTest.cs $
// $Revision: 18 $	$Date: 10/19/07 1:24p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NUnit.Framework;
using Tawala.TextEditor;

namespace TawalaTest.TextEditorTest
{
	[TestFixture]
	public class TextSelectionTest : TextEditTestBase
	{
		[Test]
		public void AssumptionsAboutInitialState()
		{
			Assert.IsTrue(editor.Selection is ITextSelection);
			Assert.IsEmpty(editor.Selection.Text);
			Assert.AreEqual(0, editor.Selection.Start);
			Assert.AreEqual(0, editor.Selection.Length);
			Assert.AreEqual(Tristate.False, editor.Selection.Bold);
			Assert.AreEqual(HorizontalAlignment.Left, editor.Selection.ParagraphHAlignment);
		}

		[Test]
		public void Start_Property()
		{
			string test = "Walk the dog.";
			editor.Selection.Text = test;
			Assert.AreEqual(test.Length, editor.Selection.Start);

			editor.Selection.Start = 1;
			Assert.AreEqual(1, editor.Selection.Start);
		}

		[Test]
		public void Length_Property()
		{
			string test = "Walk the dog.";
			editor.Selection.Text = test;
			Assert.AreEqual(0, editor.Selection.Length);

			editor.Selection.Start = 1;
			editor.Selection.Length = 2;
			Assert.AreEqual(2, editor.Selection.Length);
		}

		[Test]
		public void StartAndLengthSubSelection_Test()
		{
			string test = "Walk the dog.";
			editor.Selection.Text = test;

			editor.Selection.Start = 1;
			editor.Selection.Length = 5;

			Assert.AreEqual(test.Substring(1, 5), editor.Selection.Text);
		}

		[Test]
		public void StartChangedLengthSame_Test()
		{
			string test = "Walk the dog.";
			editor.Selection.Text = test;
			editor.Selection.Start = 3;
			editor.Selection.Length = 5;

			editor.Selection.Start = 1;

			Assert.AreEqual(1, editor.Selection.Start);
			Assert.AreEqual(5, editor.Selection.Length);
			Assert.AreEqual(5, editor.Selection.Text.Length);
		}

		[Test]
		public void Insert_Test()
		{
			string test = "Walk the dog.";
			editor.Selection.Text = test;
			editor.Selection.Start = 9;
			editor.Selection.Length = 0;
			editor.Selection.Text = "big ";
			Assert.AreEqual(test.Length + 4, txTextControl.TextChars.Count);

			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20" +
				@" Walk the big dog." +
				@"\par }";
			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		[Test]
		public void Overwrite_Test()
		{
			string test = "Walk the dog.";
			editor.Selection.Text = test;
			editor.Selection.Start = 3;
			editor.Selection.Length = 5;
			editor.Selection.Text = "";
			Assert.AreEqual(test.Length - 5, txTextControl.TextChars.Count);

			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20" +
				@" Wal dog." +
				@"\par }";
			Assert.AreEqual(rtfString, editor.GetRTF());
		}


		[Test]
		public void Bold_Property()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.Selection.Start = 2;
			editor.Selection.Length = 5;
			editor.Selection.Bold = Tristate.True;

			Assert.AreEqual(Tristate.True, editor.Selection.Bold);

			editor.Selection.Start = 0;

			Assert.AreEqual(Tristate.Undefined, editor.Selection.Bold);

			editor.Selection.Start = 8;
			editor.Selection.Length = 2;
			Assert.AreEqual(Tristate.False, editor.Selection.Bold);
		}

		[Test]
		public void Italic_Property()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.Selection.Start = 2;
			editor.Selection.Length = 5;
			editor.Selection.Italic = Tristate.True;

			Assert.AreEqual(Tristate.True, editor.Selection.Italic);

			editor.Selection.Start = 0;

			Assert.AreEqual(Tristate.Undefined, editor.Selection.Italic);

			editor.Selection.Start = 8;
			editor.Selection.Length = 2;
			Assert.AreEqual(Tristate.False, editor.Selection.Italic);
		}

		[Test]
		public void Underline_Property()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.Selection.Start = 2;
			editor.Selection.Length = 5;
			editor.Selection.Underline = Tristate.True;

			Assert.AreEqual(Tristate.True, editor.Selection.Underline);
			Assert.AreEqual(TXTextControl.FontUnderlineStyle.Single, txTextControl.Selection.Underline);

			editor.Selection.Start = 0;

			Assert.AreEqual(Tristate.Undefined, editor.Selection.Underline);

			editor.Selection.Start = 8;
			editor.Selection.Length = 2;
			Assert.AreEqual(Tristate.False, editor.Selection.Underline);
			Assert.AreEqual(TXTextControl.FontUnderlineStyle.None, txTextControl.Selection.Underline);
		}

		[Test]
		public void FontName_Property1()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.FontName = "Tahoma";
			editor.Selection.FontPointSize = 11.0;

			Assert.AreEqual("Tahoma", editor.Selection.FontName);

			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\fswiss\fcharset0\fprq2 Tahoma;}" + "\r\n" +
				@"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f1\fs22" +
				@" Walk the dog." +
				@"\par }";

			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		[Test]
		public void FontName_Property2()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.Select(0,4);
			editor.Selection.FontName = "Courier New";
			editor.Select(5, 3);
			editor.Selection.FontName = "Verdana";
			editor.Select(0, 7);

			Assert.AreEqual(string.Empty, editor.Selection.FontName);

			editor.Selection.Start = 0;
			editor.Selection.Length = 4;

			Assert.AreEqual("Courier New", editor.Selection.FontName);
			
			editor.Selection.Start = 5;
			editor.Selection.Length = 3;

			Assert.AreEqual("Verdana", editor.Selection.FontName);

			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\fmodern\fcharset0\fprq1 Courier New;}" + "\r\n" +
				@"{\f2\fswiss\fcharset0\fprq2 Verdana;}" + "\r\n" +
				@"{\f3\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f1\fs20" +
				@" Walk" +
				@"\plain\f0\fs20" +
				@"  " +
				@"\plain\f2\fs20" +
				@" the" +
				@"\plain\f0\fs20" +
				@"  dog." +
				@"\par }";

			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		[Test]
		public void FontPointSize_Property1()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.SelectAll();
			editor.Selection.FontPointSize = 9.0;

			Assert.AreEqual(9.0, editor.Selection.FontPointSize);

			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs18" +
				@" Walk the dog." +
				@"\par }";

			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		[Test]
		public void FontPointSize_Property2()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.Select(0, 4);
			editor.Selection.FontPointSize = 14.0;
			editor.Select(5, 3);
			editor.Selection.FontPointSize = 8.0;
			editor.Select(0, 7);

			Assert.AreEqual(0.0, editor.Selection.FontPointSize);

			editor.Selection.Start = 0;
			editor.Selection.Length = 4;

			Assert.AreEqual(14.0, editor.Selection.FontPointSize);

			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs28" +
				@" Walk" +
				@"\plain\f0\fs20" +
				@"  " +
				@"\plain\f0\fs16" +
				@" the" +
				@"\plain\f0\fs20" +
				@"  dog." +
				@"\par }";

			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		[Test]
		public void FontColor_Methods1()
		{
			editor.Selection.Text = "Walk the dog.";
			editor.Select(0, 4);
			editor.Selection.SetFontColor(Color.Aquamarine);
			editor.Select(5, 3);
			editor.Selection.SetFontColor(Color.Red);

			editor.Select(0, 7);
			Color c;
			Assert.IsFalse(editor.Selection.GetFontColor(out c));

			editor.Select(0, 4);
			Assert.IsTrue(editor.Selection.GetFontColor(out c));
			Color c2 = Color.Aquamarine;
			c2 = Color.FromArgb(c2.A, c2.R, c2.G, c2.B);
			Assert.AreEqual(c2, c);

			editor.Select(5, 3);
			Assert.IsTrue(editor.Selection.GetFontColor(out c));
			c2 = Color.Red;
			c2 = Color.FromArgb(c2.A, c2.R, c2.G, c2.B);
			Assert.AreEqual(c2, c);
		}

		[Test]
		public void ParagraphHAlignment_Property1()
		{
			editor.Selection.Text = "Walk the dog.";
			Assert.AreEqual(HorizontalAlignment.Left, editor.Selection.ParagraphHAlignment);
			editor.Selection.ParagraphHAlignment = HorizontalAlignment.Right;
			Assert.AreEqual(HorizontalAlignment.Right, editor.Selection.ParagraphHAlignment);
			editor.Selection.ParagraphHAlignment = HorizontalAlignment.Center;
			Assert.AreEqual(HorizontalAlignment.Center, editor.Selection.ParagraphHAlignment);
			editor.Selection.ParagraphHAlignment = HorizontalAlignment.Justify;
			Assert.AreEqual(HorizontalAlignment.Justify, editor.Selection.ParagraphHAlignment);
		}

		[Test]
		public void ResetFormatting()
		{
			editor.Selection.Text = "Walk the dog.";
			Assert.AreEqual(editor.Selection.FontName, "Arial");
			Assert.AreEqual(editor.Selection.FontPointSize, 10.0);
			Color c;
			editor.Selection.GetFontColor(out c);
			Assert.AreEqual(Color.Black.ToArgb(), c.ToArgb());

			editor.Select(5, 3);
			editor.Selection.FontName = "Tahoma";
			editor.Selection.FontPointSize = 20.0;
			editor.Selection.SetFontColor(Color.Red);

			c = Color.Red;
			Assert.AreEqual("the", editor.Selection.Text);
			Assert.AreEqual("Tahoma", editor.Selection.FontName);
			Assert.AreEqual(20.0, editor.Selection.FontPointSize);
			c = Color.Blue;
			editor.Selection.GetFontColor(out c);
			Assert.AreEqual(Color.Red.ToArgb(), c.ToArgb());

			c = Color.White;
			editor.Selection.ResetFormatting();
			Assert.AreEqual("Arial", editor.Selection.FontName);
			Assert.AreEqual(10.5, editor.Selection.FontPointSize);
			editor.Selection.GetFontColor(out c);
			Assert.AreEqual(c.ToArgb(), Color.FromArgb(255, 0, 0, 1).ToArgb());
		}
	}
}
