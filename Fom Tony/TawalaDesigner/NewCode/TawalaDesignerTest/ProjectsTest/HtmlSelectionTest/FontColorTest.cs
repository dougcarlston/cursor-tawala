using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest.HtmlSelectionTest
{
	[TestFixture]
	public class FontColorTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void SettingColorInSelectionWithExistingPartialColorReplacesColor()
		{
			string htmlText = @"<SPAN style=""COLOR: #FF0000;"">One </SPAN>Two";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontColor("00FF00");

			string expectedXhtml =
				@"<span style=""color: #00FF00;"">One Two</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void SettingColorInSelectionWithSizesSetsColorWithoutChangingSizes()
		{
			string htmlText = @"<SPAN style=""FONT-SIZE: 16pt;""><SPAN style=""FONT-SIZE: 28pt;"">One </SPAN>Two </SPAN>Three ";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontColor("FF0000");

			string expectedXhtml =
				@"<span style=""color: #FF0000;""><span style=""font-size: 16pt;""><span style=""font-size: 28pt;"">One </span>Two </span>Three </span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void SettingColorInSelectionWithFacesSetsColorWithoutChangingFaces()
		{
			string htmlText = @"<SPAN style=""FONT-FAMILY: Arial;""><SPAN style=""FONT-FAMILY: Impact;"">One </SPAN>Two </SPAN>Three ";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontColor("FF0000");

			string expectedXhtml =
				@"<span style=""color: #FF0000;""><span style=""font-family: Arial;""><span style=""font-family: Impact;"">One </span>Two </span>Three </span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}
	}
}
