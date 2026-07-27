using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.FormDesigner;

using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.XmlSupport;

using TawalaTest.TestSupport;

namespace TawalaTest.FormDesignerTest.Browser.Selection
{
	[TestFixture]
	public class FontColorMultiParagraphTest : TestBase
	{
		[Test]
		public void SettingMiddleParagraphColorAfterSettingMultiParagraphColorProducesExpectedHtml()
		{
			CreateViewWithTextItem("One", "Two", "Three");

			setColorOnMultiParagraphViewSelection("FF0000", "One", "Two", "Three");
			setColorOnViewSelection("00FF00", "Two");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #ff0000"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #00ff00"">Two</SPAN></P>" +
				Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #ff0000"">Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingMiddleParagraphColorAfterSettingMultiParagraphSizeProducesExpectedHtml()
		{
			CreateViewWithTextItem("One", "Two", "Three");

			setSizeOnMultiParagraphViewSelection(16, "One", "Two", "Three");
			setColorOnViewSelection("FF0000", "Two");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt; COLOR: #ff0000"">Two</SPAN></P>" +
				Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingMiddleParagraphColorAfterSettingMultiParagraphFaceProducesExpectedHtml()
		{
			CreateViewWithTextItem("One", "Two", "Three");

			setFaceOnMultiParagraphViewSelection("Courier New", "One", "Two", "Three");
			setColorOnViewSelection("FF0000", "Two");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #ff0000; FONT-FAMILY: Courier New"">Two</SPAN></P>" +
				Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

	}
}
