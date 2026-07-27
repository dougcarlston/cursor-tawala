// $Workfile: ImagesTest.cs $
// $Revision: 1 $	$Date: 6/28/06 11:29p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
//using System.Collections.Generic;
using System.Reflection;
//using System.Text;
//using System.Windows.Forms;
using NUnit.Framework;
using Tawala.TextEditor;
using TawalaTest.ProjectTest;

namespace TextEditorTest
{
	[TestFixture]
	public class ImagesTest : TextEditTestBase
	{
		private static string getImagePath(string filename)
		{
			string imagePath = Util.GetTestFilePath(filename);
			Assert.IsTrue(File.Exists(imagePath), "Image file is missing: {0}", imagePath);
			return imagePath;
		}

		private static string getExpectedRtfString(string rtfFilename)
		{
			string rtfPath = Util.GetTestFilePath(rtfFilename);
			Assert.IsTrue(File.Exists(rtfPath), "RTF file is missing: {0}", rtfPath);

			return File.ReadAllText(rtfPath);
		}

		[Test]
		public void InsertBMP()
		{
			string imagePath = getImagePath("Gone Fishing.bmp");
			string rtfString = getExpectedRtfString("Gone Fishing.rtf");

			editor.InsertImage(imagePath);
			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		[Test]
		public void InsertJPG()
		{
			string imagePath = getImagePath("Blue hills.jpg");
			string rtfString = getExpectedRtfString("Blue hills.rtf");

			editor.InsertImage(imagePath);
			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		[Test]
		public void InsertPNG()
		{
			string imagePath = getImagePath("Yin Yang Skype.png");
			string rtfString = getExpectedRtfString("Yin Yang Skype.rtf");

			editor.InsertImage(imagePath);
			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		[Test]
		public void InsertGIF()
		{
			string imagePath = getImagePath("dialup.gif");
			string rtfString = getExpectedRtfString("dialup.rtf");

			editor.InsertImage(imagePath);
			Assert.AreEqual(rtfString, editor.GetRTF());
		}
	}
}
