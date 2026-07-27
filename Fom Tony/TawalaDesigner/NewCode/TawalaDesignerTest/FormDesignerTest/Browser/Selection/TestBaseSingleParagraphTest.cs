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
	public class TestBaseSingleParagraphTest : TestBase
	{
		[Test]
		public void CreatePartialParagraphSelection()
		{
			CreateViewWithTextItem("One Two Three Four");
			SelectViewContents("Two Three ");

			Assert.AreEqual("Two Three ", View.GetSelection());
		}

		[Test]
		public void CreateCompletelySpanEnclosedSelection()
		{
			CreateViewWithTextItem("<font size=\"480\">One </font>Two Three Four");
			SelectViewContents("One ");

			Assert.AreEqual(@"<SPAN style=""FONT-SIZE: 24pt"">One </SPAN>", View.GetSelection());
		}

		[Test]
		public void CreatePartialSpanEnclosedSelection()
		{
			CreateViewWithTextItem("<font size=\"480\">One </font>Two Three Four");
			SelectViewContents("One Two ");

			Assert.AreEqual(@"<SPAN style=""FONT-SIZE: 24pt"">One </SPAN>Two ", View.GetSelection());
		}
	}
}
