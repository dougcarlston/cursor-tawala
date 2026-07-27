// $Workfile: Fonts.cs $
// $Revision: 3 $	$Date: 11/13/07 11:34a $
// Copyright © 2005 - 2007 Tawala Systems, Inc. All rights reserved.using System;

using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Tawala.FontSupport
{
	public static class Fonts
	{
		public static string DefaultFontName
		{
			get { return Properties.Resources.TawalaDefaultFontName; }
		}

		public static Collection<string> WebSafeFonts
		{
			get { return webSafeFonts; }
		}

		public static string DefaultFontFilename
		{
			get { return "TawalaDefault.ttf"; }
		}

		public static int[] DefaultFontRGB
		{
			get { return defaultFontRGB; }
		}

		public static Color DefaultFontColor
		{
			get { return defaultFontColor; }
		}

		public static double DefaultFontSize
		{
			get { return 10.5; }
		}

		#region Private

		private static int[] defaultFontRGB = new int[] { 0, 0, 1 };

		private static Color defaultFontColor = Color.FromArgb(defaultFontRGB[0], defaultFontRGB[1], defaultFontRGB[2]);

		private static Collection<string> webSafeFonts = new Collection<string>();

		static Fonts()
		{
			webSafeFonts.Add("Arial");
			webSafeFonts.Add("Arial Black");
			webSafeFonts.Add("Comic Sans MS");
			webSafeFonts.Add("Courier New");
			webSafeFonts.Add("Georgia");
			webSafeFonts.Add("Impact");
			webSafeFonts.Add("Tahoma");
			webSafeFonts.Add("Times New Roman");
			webSafeFonts.Add("Trebuchet MS");
			webSafeFonts.Add("Verdana");
		}

		#endregion
	}
}
