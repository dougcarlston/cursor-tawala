using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class FormItemParagraphTest
	{
		private FibItem fibItem = null;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			IForm form = Project.Current.AddForm();
			fibItem = new FibItem();
			fibItem.BlankList.Clear();
			form.ItemList.Add(fibItem);
		}
		
		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
				"<paragraph indent=\"720\" align=\"left\">" +
				"</paragraph>";

			IXmlElement element = new XmlElement(xmlString);
			FormItemParagraph paragraph = new FormItemParagraph(element);

			Assert.AreEqual(720, paragraph.Indent);
			Assert.AreEqual("left", paragraph.Align);
		}

		[Test]
		public void ConstructFormattedTextFromXml()
		{
			string xmlString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">Formatted Text</font>" +
				"</paragraph>";

			IXmlElement element = new XmlElement(xmlString);
			FormItemParagraph paragraph = new FormItemParagraph(element);

			Assert.AreEqual("Formatted Text", paragraph.Text);
		}

		[Test]
		public void ConstructFormattedBlankFromXml()
		{
			string xmlString =
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FaceColorBeginFont +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
				XmlConstants.EndFont +
				"</paragraph>";

			IXmlElement element = new XmlElement(xmlString);
			FormItemParagraph paragraph = new FormItemParagraph(element, fibItem);

			Assert.AreEqual(1, paragraph.Contents.Count);
			Assert.AreEqual(xmlString, paragraph.ToXml());
		}

		[Test]
		public void ConstructUnformattedBlankFromXml()
		{
			string xmlString =
				"<paragraph indent=\"0\" align=\"left\">" +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
				"</paragraph>";

			IXmlElement element = new XmlElement(xmlString);
			FormItemParagraph paragraph = new FormItemParagraph(element, fibItem);

			Assert.AreEqual(1, paragraph.Contents.Count);
			Assert.AreEqual(xmlString, paragraph.ToXml());
		}

	}

}
