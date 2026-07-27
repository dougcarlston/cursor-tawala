using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest.Forms.FormItemContents
{
	[TestFixture]
	public class FormItemContentsTest
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();

			ComponentMaker.UseNewComponents(true);
		}

		[TearDown]
		public void TearDown()
		{
			ComponentMaker.UseNewComponents(false);
		}

		[Test]
		public void GetDescendantsReturnsIBlanks()
		{
			string xmlString =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"<blank label=\"b\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"<blank label=\"c\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine;

			NewFibItem fibItem = new NewFibItem(new XmlElement(xmlString));

			IForm form = Project.Current.AddForm();
			form.ItemList.Add(fibItem);

            FormItemContentsCollection descendantsCollection = fibItem.Contents.GetDescendants(typeof(IBlank));

			int i = 0;
			Assert.IsInstanceOfType(typeof(IBlank), descendantsCollection[i++]);
			Assert.IsInstanceOfType(typeof(IBlank), descendantsCollection[i++]);
			Assert.IsInstanceOfType(typeof(IBlank), descendantsCollection[i++]);
			Assert.AreEqual(i, descendantsCollection.Count);
		}

		[Test]
		public void GetDescendantsReturnsParagraphContents()
		{
			string xmlString =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Paragraph 1 " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Paragraph 2 " +
				"<blank label=\"b\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine;

			NewFibItem fibItem = new NewFibItem(new XmlElement(xmlString));

			IForm form = Project.Current.AddForm();
			form.ItemList.Add(fibItem);

            FormItemContentsCollection descendantsCollection = fibItem.Contents.GetDescendants(typeof(ParagraphContents));

			int i = 0;
			Assert.IsInstanceOfType(typeof(ParagraphContents), descendantsCollection[i++]);
			Assert.IsInstanceOfType(typeof(ParagraphContents), descendantsCollection[i++]);
			Assert.AreEqual(i, descendantsCollection.Count);
		}

		[Test]
		public void GetDescendantsReturnsTextContents()
		{
			string xmlString =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Paragraph 1 " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Paragraph 2 " +
				"<blank label=\"b\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine;

			NewFibItem fibItem = new NewFibItem(new XmlElement(xmlString));

			IForm form = Project.Current.AddForm();
			form.ItemList.Add(fibItem);

            FormItemContentsCollection descendantsCollection = fibItem.Contents.GetDescendants(typeof(TextContents));

			int i = 0;
			Assert.IsInstanceOfType(typeof(TextContents), descendantsCollection[i++]);
			Assert.IsInstanceOfType(typeof(TextContents), descendantsCollection[i++]);
			Assert.AreEqual(i, descendantsCollection.Count);
		}

		[Test]
		public void GetDescendantsReturnsFontContents()
		{
			string xmlString =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font>" +
				"Paragraph 1 " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</font>" +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font>" +
				"Paragraph 2 " +
				"<blank label=\"b\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</font>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine;

			NewFibItem fibItem = new NewFibItem(new XmlElement(xmlString));

			IForm form = Project.Current.AddForm();
			form.ItemList.Add(fibItem);

            FormItemContentsCollection descendantsCollection = fibItem.Contents.GetDescendants(typeof(NewFont));

			int i = 0;
			Assert.IsInstanceOfType(typeof(NewFont), descendantsCollection[i++]);
			Assert.IsInstanceOfType(typeof(NewFont), descendantsCollection[i++]);
			Assert.AreEqual(i, descendantsCollection.Count);
		}

		[Test]
		public void GetDescendantsReturnsFontContentsFromHtmlSpans()
		{
			string htmlString =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 12pt;"">One </SPAN><SPAN style=""FONT-SIZE: 12pt;"">Two</SPAN></P>";

			IFormItemContents paragraph = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

			FormItemContentsCollection fonts = paragraph.GetDescendants(typeof(NewFont));

			int i = 0;
			Assert.IsInstanceOfType(typeof(NewFont), fonts[i++]);
			Assert.IsInstanceOfType(typeof(NewFont), fonts[i++]);
			Assert.AreEqual(i, fonts.Count);
		}

		[Test]
		public void GetDescendantsReturnsFontContentsFromNestedHtmlSpans()
		{
			string htmlString =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left><SPAN style=""FONT-SIZE: 12pt;"">One <SPAN style=""FONT-SIZE: 16pt;"">Two</SPAN></SPAN></P>";

			IFormItemContents paragraph = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

			FormItemContentsCollection fonts = paragraph.GetDescendants(typeof(NewFont));

			int i = 0;
			Assert.IsInstanceOfType(typeof(NewFont), fonts[i++]);
			Assert.IsInstanceOfType(typeof(NewFont), fonts[i++]);
			Assert.AreEqual(i, fonts.Count);
		}

		[Test]
		public void GetDescendantsReturnsFontContentsFromHighlyNestedHtmlSpans()
		{
			string htmlString =
				@"<P style=""MARGIN-LEFT: 0pt"" align=left>" +
				@"<SPAN style=""FONT-SIZE: 14pt"">" +
				@"<SPAN style=""FONT-FAMILY: Courier New"">" +
				@"<SPAN style=""FONT-SIZE: 14pt"">" +
				@"<SPAN style=""FONT-SIZE: 14pt; FONT-FAMILY: Impact"">" +
				@"One " +
				@"</SPAN>" +
				@"Two" +
				@"</SPAN>" +
				@"</SPAN>" +
				@"</SPAN>" +
				@"</P>";

			IFormItemContents paragraph = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

			FormItemContentsCollection fonts = paragraph.GetDescendants(typeof(NewFont));

			int i = 0;
			Assert.IsInstanceOfType(typeof(NewFont), fonts[i++]);
			Assert.IsInstanceOfType(typeof(NewFont), fonts[i++]);
			Assert.IsInstanceOfType(typeof(NewFont), fonts[i++]);
			Assert.IsInstanceOfType(typeof(NewFont), fonts[i++]);
			Assert.AreEqual(i, fonts.Count);
		}

		[Test]
		public void GetDescendantsReturnsQuestionContents()
		{
			string xmlString =
				@"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"[Replace this with your question. Use Enter key to add choices below.]" +
				@"</font>" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left""></paragraph>" +
				@"</choice>" +
				@"</mc>" + Environment.NewLine;

			NewMcqItem mcqItem = new NewMcqItem(new XmlElement(xmlString));

			IForm form = Project.Current.AddForm();
			form.ItemList.Add(mcqItem);

            FormItemContentsCollection descendantsCollection = mcqItem.Contents.GetDescendants(typeof(Question));

			int i = 0;
			Assert.IsInstanceOfType(typeof(Question), descendantsCollection[i++]);
			Assert.AreEqual(i, descendantsCollection.Count);
		}

		[Test]
		public void GetDescendantsReturnsChoiceContents()
		{
			string xmlString =
				@"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"[Replace this with your question. Use Enter key to add choices below.]" +
				@"</font>" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left"">Choice one</paragraph>" +
				@"</choice>" +
				@"<choice label=""b"">" +
				@"<paragraph indent=""0"" align=""left"">Choice two</paragraph>" +
				@"</choice>" +
				@"</mc>" + Environment.NewLine;

			NewMcqItem mcqItem = new NewMcqItem(new XmlElement(xmlString));

			IForm form = Project.Current.AddForm();
			form.ItemList.Add(mcqItem);

            FormItemContentsCollection descendantsCollection = mcqItem.Contents.GetDescendants(typeof(IChoice));

			int i = 0;
			Assert.IsInstanceOfType(typeof(IChoice), descendantsCollection[i++]);
			Assert.IsInstanceOfType(typeof(IChoice), descendantsCollection[i++]);
			Assert.AreEqual(i, descendantsCollection.Count);
		}
	}
}
