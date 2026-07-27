using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;


namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 316 (Selecting Image can cause slow down of application).
	/// </summary>
	[TestFixture]
	public class ImageSelectTime316
	{
		private IForm form;
		private TextItem textItem;
		private string rtfString;


		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Assert.AreEqual(true, File.Exists(TestSupport.Util.GetTestFilePath("Blue hills.rtf")));
		}

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();

			textItem = new TextItem();
			form.ItemList.Add(textItem);

			Assert.AreEqual(true, File.Exists(TestSupport.Util.GetTestFilePath("Blue hills.rtf")));
		}

		[Test]
		[Ignore ("New version of NUnit seems to make this run slower")]
		public void RtfToImageInLessThan3Seconds()
		{
			using (StreamReader reader = new StreamReader(TestSupport.Util.GetTestFilePath("Blue hills.rtf")))
			{
				rtfString = reader.ReadToEnd();
			}

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Reset();

			stopWatch.Start();
			textItem.Rtf = rtfString;
			stopWatch.Stop();

			Console.WriteLine("Text Item RTF set in {0} ms", stopWatch.ElapsedMilliseconds);

			int expectedMilliseconds = 3000;
			Assert.Less((int)stopWatch.ElapsedMilliseconds, expectedMilliseconds);
		}

		[Test]
		// added this test when the 3-second test above started running too slow in the new nUnit framework  -  jdf 11/9/07
		public void RtfToImageInLessThan5Seconds()
		{
			using (StreamReader reader = new StreamReader(TestSupport.Util.GetTestFilePath("Blue hills.rtf")))
			{
				rtfString = reader.ReadToEnd();
			}

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Reset();

			stopWatch.Start();
			textItem.Rtf = rtfString;
			stopWatch.Stop();

			Console.WriteLine("Text Item RTF set in {0} ms", stopWatch.ElapsedMilliseconds);

			int expectedMilliseconds = 5000;
			Assert.Less((int)stopWatch.ElapsedMilliseconds, expectedMilliseconds);
		}

	}
}
