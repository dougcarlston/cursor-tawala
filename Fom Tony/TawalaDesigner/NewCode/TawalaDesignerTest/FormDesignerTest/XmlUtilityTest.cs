using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class XmlUtilityTest
	{
		[Test]
		public void ToXhtmlMethodFixesParagraphWithOneMoreOpeningSpanTagsThanClosingSpanTags()
		{
			string inputHtml =
				@"<P><SPAN style=""FONT-SIZE: 32pt"">One</P>";

			string expectedXhtml =
				@"<p><span style=""FONT-SIZE: 32pt"">One</span></p>";

			Assert.AreEqual(expectedXhtml, XmlUtility.ToXhtml(inputHtml));
		}

		[Test]
		public void ToXhtmlMethodFixesParagraphWithTwoMoreOpeningSpanTagsThanClosingSpanTags()
		{
			string inputHtml =
				@"<P><SPAN style=""FONT-SIZE: 32pt"">One <SPAN style=""FONT-SIZE: 16pt"">Two</P>";

			string expectedXhtml =
				@"<p><span style=""FONT-SIZE: 32pt"">One <span style=""FONT-SIZE: 16pt"">Two</span></span></p>";

			Assert.AreEqual(expectedXhtml, XmlUtility.ToXhtml(inputHtml));
		}

		[Test]
		public void ToXhtmlMethodFixesParagraphWithOneMoreClosingSpanTagsThanOpeningSpanTags()
		{
			string inputHtml =
				@"<P><SPAN style=""FONT-SIZE: 32pt"">One</SPAN></SPAN></P>";

			string expectedXhtml =
				@"<p><span style=""FONT-SIZE: 32pt"">One</span></p>";

			Assert.AreEqual(expectedXhtml, XmlUtility.ToXhtml(inputHtml));
		}

		[Test]
		public void ToXhtmlMethodFixesParagraphWithTwoMoreClosingSpanTagsThanOpeningSpanTags()
		{
			string inputHtml =
				@"<P><SPAN style=""FONT-SIZE: 32pt"">One</SPAN></SPAN></SPAN></P>";

			string expectedXhtml =
				@"<p><span style=""FONT-SIZE: 32pt"">One</span></p>";

			Assert.AreEqual(expectedXhtml, XmlUtility.ToXhtml(inputHtml));
		}

		[Test]
		public void ToXhtmlMethodFixesImbalancedSpanTags()
		{
			string inputHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 32pt"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 32pt"">Two&nbsp;<SPAN style=""FONT-SIZE: 24pt"">Three Four</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 24pt"">Five</SPAN></SPAN><SPAN style=""FONT-SIZE: 32pt""> Six</SPAN></P>";

			string expectedXhtml =
				@"<p style=""MARGIN-LEFT: 0pt"" align=""left""><span style=""FONT-SIZE: 32pt"">One</span></p>" +
				"<p></p>" +
				@"<p style=""MARGIN-LEFT: 0pt"" align=""left""><span style=""FONT-SIZE: 32pt"">Two <span style=""FONT-SIZE: 24pt"">Three Four</span></span></p>" +
				"<p></p>"+
				@"<p style=""MARGIN-LEFT: 0pt"" align=""left""><span style=""FONT-SIZE: 24pt"">Five</span><span style=""FONT-SIZE: 32pt""> Six</span></p>";

			Assert.AreEqual(expectedXhtml, XmlUtility.ToXhtml(inputHtml));
		}

		[Test]
		public void ToXhtmlMethodFixesSingleUnclosedParagraph()
		{
			string inputHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 32pt"">One</SPAN>";

			string expectedXhtml =
				@"<p style=""MARGIN-LEFT: 0pt"" align=""left""><span style=""FONT-SIZE: 32pt"">One</span></p>";

			Assert.AreEqual(expectedXhtml, XmlUtility.ToXhtml(inputHtml));
		}

		[Test]
		public void ToXhtmlMethodFixesContainedSingleUnclosedParagraph()
		{
			string inputHtml =
				@"<container><P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 32pt"">One</SPAN></container>";

			string expectedXhtml =
				@"<container><p style=""MARGIN-LEFT: 0pt"" align=""left""><span style=""FONT-SIZE: 32pt"">One</span></p></container>";

			Assert.AreEqual(expectedXhtml, XmlUtility.ToXhtml(inputHtml));
		}

		[Test]
		public void ToXhtmlMethodFixesMultipleUnclosedParagraphs()
		{
			string inputHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 32pt"">One</SPAN>"+
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 32pt"">Two</SPAN>"+
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 32pt"">Three</SPAN>";

			string expectedXhtml =
				@"<p style=""MARGIN-LEFT: 0pt"" align=""left""><span style=""FONT-SIZE: 32pt"">One</span></p>"+
				@"<p style=""MARGIN-LEFT: 0pt"" align=""left""><span style=""FONT-SIZE: 32pt"">Two</span></p>"+
				@"<p style=""MARGIN-LEFT: 0pt"" align=""left""><span style=""FONT-SIZE: 32pt"">Three</span></p>";

			Assert.AreEqual(expectedXhtml, XmlUtility.ToXhtml(inputHtml));
		}

		[Test]
		public void ToXhtmlMethodFixesUnclosedParagraphs()
		{
			string inputHtml =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 32pt"">One</SPAN></P>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 32pt"">Two&nbsp;<SPAN style=""FONT-SIZE: 24pt"">Three Four</SPAN>" +
				Environment.NewLine + "<P></P>" + Environment.NewLine +
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 24pt"">Five</SPAN></SPAN><SPAN style=""FONT-SIZE: 32pt""> Six</SPAN></P>";

			string expectedXhtml =
				@"<p style=""MARGIN-LEFT: 0pt"" align=""left""><span style=""FONT-SIZE: 32pt"">One</span></p>" +
				"<p></p>" +
				@"<p style=""MARGIN-LEFT: 0pt"" align=""left""><span style=""FONT-SIZE: 32pt"">Two <span style=""FONT-SIZE: 24pt"">Three Four</span></span></p>" +
				"<p></p>" +
				@"<p style=""MARGIN-LEFT: 0pt"" align=""left""><span style=""FONT-SIZE: 24pt"">Five</span><span style=""FONT-SIZE: 32pt""> Six</span></p>";

			Assert.AreEqual(expectedXhtml, XmlUtility.ToXhtml(inputHtml));
		}

		[Test]
		public void ToXhtmlMethodFixesUnclosedImageTag()
		{
			string inputHtml =
				@"<IMG onresizestart=fnCancel() src=""file:///C:/Temp/AnyFileName.jpg"">";

			string expectedXhtml =
				@"<img onresizestart=""fnCancel()"" src=""file:///C:/Temp/AnyFileName.jpg"" />";

			Assert.AreEqual(expectedXhtml, XmlUtility.ToXhtml(inputHtml));
		}
	}
}
