using System;
using System.Collections.Specialized;
using System.Text;
using NUnit.Framework;
using Tawala.XmlSupport;
using Tawala.RtfSupport;

namespace TawalaTest.RtfSupportTest
{
	[TestFixture]
	public class RtfColorTableTest
	{
		private RtfColorTable colorTable;
		private const string NEWLINE = "\r\n";

		[SetUp]
		public void SetUp()
		{
			colorTable = new RtfColorTable();

			RtfColorTableEntry redEntry = new RtfColorTableEntry();
			redEntry.Red = 255;
			redEntry.Green = 0;
			redEntry.Blue = 0;

			RtfColorTableEntry greenEntry = new RtfColorTableEntry();
			greenEntry.Red = 0;
			greenEntry.Green = 255;
			greenEntry.Blue = 0;

			RtfColorTableEntry blueEntry = new RtfColorTableEntry();
			blueEntry.Red = 0;
			blueEntry.Green = 0;
			blueEntry.Blue = 255;

			colorTable.Add(redEntry);
			colorTable.Add(greenEntry);
			colorTable.Add(blueEntry);

		}

		[Test]
		public void IndexMatching()
		{
			Assert.AreEqual(0, colorTable.IndexMatching(0xff0000));
			Assert.AreEqual(1, colorTable.IndexMatching(0x00ff00));
			Assert.AreEqual(2, colorTable.IndexMatching(0x0000ff));
		}

		[Test]
		public void ToRtf()
		{
			string expectedString =
				@"{\colortbl;" + NEWLINE +
				@"\red255\green0\blue0;" + NEWLINE +
				@"\red0\green255\blue0;" + NEWLINE +
				@"\red0\green0\blue255;" + NEWLINE +
				@"}" + NEWLINE;

			Assert.AreEqual(expectedString, colorTable.ToRtf());
		}
	}
}
