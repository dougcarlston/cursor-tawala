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
	public class TestBaseCrossParagraphTest : TestBase
	{
		[Test]
		public void CreatePartialSelection()
		{
			CreateViewWithTextItem("One Two", "Three Four");
			SelectViewContentsAcrossParagraphs("Two", "Three");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>Two</P>" + 
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>Three</P>";

			Assert.AreEqual(expectedHtml, View.GetSelection());
		}

		[Test]
		public void CreateFullPlusPartialSelection()
		{
			CreateViewWithTextItem("One Two", "Three Four");
			SelectViewContentsAcrossParagraphs("One Two", "Three");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>One Two</P>" +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>Three</P>";

			Assert.AreEqual(expectedHtml, View.GetSelection());
		}

		[Test]
		public void CreatePartialPlusFullSelection()
		{
			CreateViewWithTextItem("One Two", "Three Four");
			SelectViewContentsAcrossParagraphs("Two", "Three Four");

			string expectedHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>Two</P>" + 
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>Three Four</P>";

			Assert.AreEqual(expectedHtml, View.GetSelection());
		}
	}
}
