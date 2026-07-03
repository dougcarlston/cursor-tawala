using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;


namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 311 (Appoximately 18 seconds to save each 28k Jpeg in a form).
	/// </summary>
	[TestFixture]
	public class ImageSaveTime311
	{
		private IForm form;
		private TextItem textItem;
		private string rtfString;


		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();

			textItem = new TextItem();
			form.ItemList.Add(textItem);
		}

		[Test]
		public void VerifyTestImageFilePresent()
		{
			Assert.AreEqual(true, File.Exists(TestSupport.Util.GetTestFilePath("Blue hills.jpg")));
		}

		[Test]
		public void ImageToXmlInLessThan1Second()
		{
			GraphicImageDefinition image = new GraphicImageDefinition(TestSupport.Util.GetTestFilePath("Blue hills.jpg"));

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Reset();

			stopWatch.Start();
			string xmlString = image.ToXml();
			stopWatch.Stop();

			Console.WriteLine("Image XML produced in {0} ms", stopWatch.ElapsedMilliseconds);

			int expectedMilliseconds = 1000;
			Assert.Less((int)stopWatch.ElapsedMilliseconds, expectedMilliseconds);
		}

		[Test]
		public void VerifyTestRtfFilePresent()
		{
			Assert.AreEqual(true, File.Exists(TestSupport.Util.GetTestFilePath("Blue hills.rtf")));
		}

		[Test]
		public void ImageToRtfInLessThan1Second()
		{
			using (StreamReader reader = new StreamReader(TestSupport.Util.GetTestFilePath("Blue hills.rtf")))
			{
				rtfString = reader.ReadToEnd();
			}

			textItem.Rtf = rtfString;

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Reset();

			stopWatch.Start();
			string rtfGetString = textItem.Rtf;
			stopWatch.Stop();

			Console.WriteLine("Text Item RTF gotten in {0} ms", stopWatch.ElapsedMilliseconds);

			int expectedMilliseconds = 1000;
			Assert.Less((int)stopWatch.ElapsedMilliseconds, expectedMilliseconds);
		}

	}
}
