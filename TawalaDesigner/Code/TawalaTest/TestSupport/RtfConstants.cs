using System;
using System.Collections.Generic;
using System.Text;

namespace TawalaTest.TestSupport
{
	public static class RtfConstants
	{
		public static string FibPrologue
		{
			get { return fibPrologue; }
		}

		// this is here because we can't get at Project resources from ProjectTest

		public static string TextItemDefaultRTF
		{
			get { return textItemDefaultRTF; }
		}

		public static string DefaultTabsRtf
		{
			get { return defaultTabsRtf; }
		}

		public static string DefaultFontNameRtf
		{
			get { return defaultFontNameRtf; }
		}

		public static string DefaultRtfPrologue
		{
			get { return defaultRtfPrologue; }
		}

		public static string DefaultRtfEpilogue
		{
			get { return defaultRtfEpilogue; }
		}

		public static string BasicRtfPrologue
		{
			get { return basicRtfPrologue; }
		}

		public static string BasicRtfThemePrologue
		{
			get { return RtfConstants.basicRtfThemePrologue; }
		}

		public static string DocumentPrologue
		{
			get { return documentPrologue; }
		}

		#region Private

		private static string documentPrologue = 
			"{\\rtf1\\ansi\\ansicpg1252\\uc1\\deff0{\\fonttbl" + Environment.NewLine +
			"{\\f0\\fswiss\\fcharset0\\fprq2 Arial;}" + Environment.NewLine +
			"{\\f1\\froman\\fcharset2\\fprq2 Symbol;}}" + Environment.NewLine +
			"{\\colortbl;\\red0\\green0\\blue0;\\red255\\green255\\blue255;}" + Environment.NewLine +
			"{\\stylesheet{\\s0\\itap0\\f0\\fs24 [Normal];}{\\*\\cs10\\additive Default Paragraph Font;}}" + Environment.NewLine +
			"{\\*\\generator TX_RTF32 12.0.500.502;}" + Environment.NewLine +
			"\\deftab1134\\paperw12240\\paperh15840\\margl720\\margt720\\margr720\\margb720\\notabind\\pard\\itap0" +
			"\\tx2880\\plain\\f0\\fs20";

		private static string defaultTabsRtf = @"\deftab0\tx2880";

		private static string defaultFontNameRtf = @"{\f1\fnil Default Font;}";

		public static string fibPrologue =
			"{\\rtf1\\ansi\\ansicpg1252\\uc1\\deff0{\\fonttbl" + Environment.NewLine +
			"{\\f0\\fswiss\\fcharset0\\fprq2 Arial;}" + Environment.NewLine +
			"{\\f1\\froman\\fcharset2\\fprq2 Symbol;}}" + Environment.NewLine +
			"{\\colortbl;\\red0\\green0\\blue0;\\red255\\green255\\blue255;\\red0\\green0\\blue0;}" + Environment.NewLine +
			"{\\stylesheet{\\s0\\itap0\\f0\\fs24 [Normal];}{\\*\\cs10\\additive Default Paragraph Font;}}" + Environment.NewLine +
			"{\\*\\generator TX_RTF32 12.0.500.502;}" + Environment.NewLine +
			defaultTabsRtf +
			"\\paperw12240\\paperh15840\\margl720\\margt720\\margr720\\margb720\\notabind\\pard\\itap0" +
			"\\tx2880\\plain\\f0\\fs20\\cf3 ";


		private static string textItemDefaultRTF = 
			"{\\rtf1\\ansi\\ansicpg1252\\uc1\\deff0" + Environment.NewLine +
			"{\\fonttbl" + Environment.NewLine +
			"{\\f0\\fswiss Arial;}" + Environment.NewLine +
			defaultFontNameRtf + Environment.NewLine +
			"}" + Environment.NewLine +
			"\\fs20{\\colortbl;" + Environment.NewLine +
			"\\red0\\green0\\blue0;" + Environment.NewLine +
			"\\red255\\green255\\blue255;" + Environment.NewLine +
			"\\red0\\green0\\blue1;" + Environment.NewLine +
			"}" + Environment.NewLine +
			defaultTabsRtf +
			"\\pard \\tx2880{\\f1\\fs21\\cf3 [Replace this with text of your own.]}\\par }";

		private static string defaultRtfPrologue =
			@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + Environment.NewLine +
			@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + Environment.NewLine +
			@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + Environment.NewLine +
			@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + Environment.NewLine +
			@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + Environment.NewLine +
			@"{\*\generator TX_RTF32 12.0.500.502;}" + Environment.NewLine +
			defaultTabsRtf +
			@"\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind" + Environment.NewLine +
			@"{\pard\itap0\sb2\sa2\plain\f0\fs20 ";

		private static string defaultRtfEpilogue = @"}";

		private static string basicRtfPrologue =
			@"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
			@"{\fonttbl" + Environment.NewLine +
			@"{\f0\fswiss Arial;}" + Environment.NewLine +
			@"{\f1\froman Symbol;}" + Environment.NewLine +
			@"}" + Environment.NewLine +
			@"\fs20{\colortbl;" + Environment.NewLine +
			@"\red0\green0\blue0;" + Environment.NewLine +
			@"\red255\green255\blue255;" + Environment.NewLine +
			@"}" + Environment.NewLine +
			RtfConstants.DefaultTabsRtf;

		private static string basicRtfThemePrologue =
			@"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
			@"{\fonttbl" + Environment.NewLine +
			@"{\f0\fswiss Arial;}" + Environment.NewLine +
			defaultFontNameRtf + Environment.NewLine +
			@"}" + Environment.NewLine +
			@"\fs20{\colortbl;" + Environment.NewLine +
			@"\red0\green0\blue0;" + Environment.NewLine +
			@"\red255\green255\blue255;" + Environment.NewLine +
			@"\red0\green0\blue1;" + Environment.NewLine +
			@"}" + Environment.NewLine +
			RtfConstants.DefaultTabsRtf;

		#endregion
	}
}
