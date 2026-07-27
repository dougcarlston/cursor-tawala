using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;


namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 220 (Field in Text Item causes error).
	/// </summary>
	[TestFixture]
	public class FieldInTextItem220
	{
		private IForm form;
		private TextItem textItem;
		private FibItem fibItem;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();
		}

		[Test]
		public void FieldReferenceWithoutTargetIsUnresolved()
		{
			textItem = new TextItem();
			textItem.Text = "<<Form 1:Q1:a>>";
			form.ItemList.Add(textItem);

			FormItemParagraph paragraph = (FormItemParagraph)textItem.Contents[0];
			FormItemNamedField namedField = (FormItemNamedField)paragraph.Contents[0];

			Assert.IsInstanceOfType(typeof(UnresolvedPaletteField), namedField.Field);
		}

		[Test]
		public void FieldReferenceWithoutTargetYieldsFieldXml()
		{
			textItem = new TextItem();
			textItem.Text = "<<Form 1:Q1:a>>";

			string expectedXml =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<field name=\"Form 1:Q1:a\"/>" +
				"</paragraph>" +
				"</text>\r\n";

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}

		[Test]
		public void FieldReferenceWithTargetYieldsFieldXml()
		{
			textItem = new TextItem();
			textItem.Text = "<<Form 1:Q1:a>>";

			fibItem = new FibItem();
			form.ItemList.Add(fibItem);

			string expectedXml =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<field name=\"Form 1:Q1:a\"/>" +
				"</paragraph>" +
				"</text>\r\n";

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}

		[Test]
		public void CallingToXmlResolvesField()
		{
			textItem = new TextItem();
			textItem.Text = "<<Form 1:Q1:a>>";
			form.ItemList.Add(textItem);

			fibItem = new FibItem();
			form.ItemList.Add(fibItem);

			FormItemParagraph paragraph = (FormItemParagraph)textItem.Contents[0];
			FormItemNamedField namedField = (FormItemNamedField)paragraph.Contents[0];

			Assert.AreNotSame(fibItem.BlankList[0], namedField.Field);

			textItem.ToXml("T1");

			Assert.AreSame(fibItem.BlankList[0], namedField.Field);
		}

	}
}
