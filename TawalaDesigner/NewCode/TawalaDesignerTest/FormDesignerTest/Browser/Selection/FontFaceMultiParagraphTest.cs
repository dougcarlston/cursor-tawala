using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.FormDesigner;

using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

using TawalaTest.TestingSupport;

namespace TawalaTest.FormDesignerTest.Browser.Selection
{
	[TestFixture]
	public class FontFaceMultiParagraphTest : TestBase
	{
		[Test]
		public void SettingMiddleParagraphFaceAfterSettingMultiParagraphColorProducesExpectedHtml()
		{
			CreateViewWithTextItem("One", "Two", "Three");

			setColorOnMultiParagraphViewSelection("FF0000", "One", "Two", "Three");
			setFaceOnViewSelection("Courier New", "Two");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #ff0000"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #ff0000; FONT-FAMILY: Courier New"">Two</SPAN></P>" +
				Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""COLOR: #ff0000"">Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingMiddleParagraphFaceAfterSettingMultiParagraphSizeProducesExpectedHtml()
		{
			CreateViewWithTextItem("One", "Two", "Three");

			setSizeOnMultiParagraphViewSelection(16, "One", "Two", "Three");
			setFaceOnViewSelection("Courier New", "Two");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt; FONT-FAMILY: Courier New"">Two</SPAN></P>" +
				Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 16pt"">Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}

		[Test]
		public void SettingMiddleParagraphFaceAfterSettingMultiParagraphFaceProducesExpectedHtml()
		{
			CreateViewWithTextItem("One", "Two", "Three");

			setFaceOnMultiParagraphViewSelection("Arial", "One", "Two", "Three");
			setFaceOnViewSelection("Courier New", "Two");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Arial"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Courier New"">Two</SPAN></P>" +
				Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-FAMILY: Arial"">Three</SPAN></P>";

			Assert.AreEqual(expectedHtml, GetParagraphHtmlFromBody());
		}
	}
}
