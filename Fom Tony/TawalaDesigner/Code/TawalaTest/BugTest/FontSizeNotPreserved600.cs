using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class FontSizeNotPreserved600
    {
		[Test]
		public void DefaultTextItemFontSizeIs10AndAHalfPoints()
		{
			string xmlString =
				"<font>Text</font>";

			FormItemFontAttributes fontAttributes = new FormItemFontAttributes(new XmlElement(xmlString));

			Assert.AreEqual(210, fontAttributes.Size);
		}

		[Test]
		public void DefaultDocumentFontSizeIs10AndAHalfPoints()
		{
			string xmlString =
				"<font>Text</font>";

			FontAttributes fontAttributes = new FontAttributes(new XmlElement(xmlString));

			Assert.AreEqual(210, fontAttributes.Size);
		}

	}
}