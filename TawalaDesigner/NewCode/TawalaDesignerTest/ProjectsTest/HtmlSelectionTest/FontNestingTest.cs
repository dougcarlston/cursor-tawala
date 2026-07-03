using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest.HtmlSelectionTest
{
	[TestFixture]
	public class FontNestingTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void NestedIdenticalSizesAreRemovedFromXhtml()
		{
			string htmlText = @"<SPAN style=""FONT-SIZE: 16pt;""><SPAN style=""FONT-SIZE: 16pt;""><SPAN style=""FONT-SIZE: 16pt;"">Selection</SPAN></SPAN></SPAN>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(16);

			string expectedXhtml = @"<span style=""font-size: 16pt;"">Selection</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}

		[Test]
		public void NestedDifferentSizesAreRemovedFromXhtml()
		{
			string htmlText = @"<SPAN style=""FONT-SIZE: 32pt;""><SPAN style=""FONT-SIZE: 24pt;""><SPAN style=""FONT-SIZE: 16pt;"">Selection</SPAN></SPAN></SPAN>";

			HtmlSelection selection = new HtmlSelection(htmlText);
			selection.SetFontSize(16);

			string expectedXhtml = @"<span style=""font-size: 16pt;"">Selection</span>";

			Assert.AreEqual(expectedXhtml, selection.ToXhtml());
		}
	}
}
