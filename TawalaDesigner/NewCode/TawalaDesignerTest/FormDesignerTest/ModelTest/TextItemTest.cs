using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestingSupport;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest.ModelTest
{
	[TestFixture]
	public class TextItemTest
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void CanConstructTextItemFromNewBoldXml()
		{
			string xmlString =
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<b>Bold</b>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine;

			ITextItem textItem = new NewTextItem(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, textItem.ToXml("T1"));
		}

		[Test]
		public void CanConstructTextItemFromNewItalicXml()
		{
			string xmlString =
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<i>Italic</i>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine;

			ITextItem textItem = new NewTextItem(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, textItem.ToXml("T1"));
		}

		[Test]
		public void CanConstructTextItemFromNewUnderlineXml()
		{
			string xmlString =
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<u>Underline</u>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine;

			ITextItem textItem = new NewTextItem(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, textItem.ToXml("T1"));
		}

		[Test]
		public void CanConstructTextItemFromNewMixedInlineStylesXml()
		{
			string xmlString =
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<b><i><u>BoldItalicUnderline</u>BoldItalic</i>Bold</b>Plain" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine;

			ITextItem textItem = new NewTextItem(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, textItem.ToXml("T1"));
		}

		[Test]
		public void CanConstructTextItemFromSimpleInlineStyleXml()
		{
			string xmlString =
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<b><i>BoldItalic</i>Bold</b>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine;

			ITextItem textItem = new NewTextItem(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, textItem.ToXml("T1"));
		}

		[Test]
		public void CanConstructTextItemFromEmbeddedWhitespaceXml()
		{
			string xmlString =
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<b><i>BoldItalic</i></b><sp/><b>Bold</b>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine;

			ITextItem textItem = new NewTextItem(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, textItem.ToXml("T1"));
		}
	}
}
