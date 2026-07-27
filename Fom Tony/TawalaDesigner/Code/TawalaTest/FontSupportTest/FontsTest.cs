using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Drawing;
using NUnit.Framework;
using Tawala.FontSupport;

namespace TawalaTest.FontSupportTest
{
	[TestFixture]
	public class FontsTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void DefaultFontNameIsExpectedName()
		{
			Assert.AreEqual("Default Font", Fonts.DefaultFontName);
		}

		[Test]
		public void WebSafeFontsListContainsExpectedFontNames()
		{
			Collection<string> webSafeFonts = Fonts.WebSafeFonts;

			Assert.AreEqual(10, webSafeFonts.Count);

			Assert.AreEqual("Arial", webSafeFonts[0]);
			Assert.AreEqual("Arial Black", webSafeFonts[1]);
			Assert.AreEqual("Comic Sans MS", webSafeFonts[2]);
			Assert.AreEqual("Courier New", webSafeFonts[3]);
			Assert.AreEqual("Georgia", webSafeFonts[4]);
			Assert.AreEqual("Impact", webSafeFonts[5]);
			Assert.AreEqual("Tahoma", webSafeFonts[6]);
			Assert.AreEqual("Times New Roman", webSafeFonts[7]);
			Assert.AreEqual("Trebuchet MS", webSafeFonts[8]);
			Assert.AreEqual("Verdana", webSafeFonts[9]);
		}

		[Test]
		public void DefaultFontFilenameIsExpectedFilename()
		{
			Assert.AreEqual("TawalaDefault.ttf", Fonts.DefaultFontFilename);
		}

		[Test]
		public void DefaultColorComponentsHaveExpectedValues()
		{
			Assert.AreEqual(0, Fonts.DefaultFontRGB[0]);
			Assert.AreEqual(0, Fonts.DefaultFontRGB[1]);
			Assert.AreEqual(1, Fonts.DefaultFontRGB[2]);
		}

		[Test]
		public void DefaultFontColorIsExpectedColor()
		{
			Color testColor = Color.FromArgb(0, 0, 1);

			Assert.IsTrue(testColor.Equals(Fonts.DefaultFontColor), "Invalid default color.");
		}

		[Test]
		public void DefaultFontSizeIsExpectedSize()
		{
			Assert.AreEqual(10.5, Fonts.DefaultFontSize);
		}
	}
}
