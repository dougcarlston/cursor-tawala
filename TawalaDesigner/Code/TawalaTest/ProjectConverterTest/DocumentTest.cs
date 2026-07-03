using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion of Documents.
	/// </summary>
	[TestFixture]
	public class DocumentTest : TestBase
	{
		[Test]
		public void XmlValidationOfDocumentWithFields()
		{
			TawalaProjectConverter converter = GetConverter("DocumentWithFields.tawala");
			converter.ConvertXmlToProject();

			RoundtripProjectXml();
		}

		[Test]
		public void DocumentWithFields()
		{
			TawalaProjectConverter converter = GetConverter("DocumentWithFields.tawala");

			converter.ConvertXmlToProject();

			Form form = (Form)Project.Current.FormList[0];
			Assert.AreEqual(2, form.ItemList.Count);
			Blank blank = ((FibItem)form.ItemList[0]).BlankList[0];
			McqItem mcItem = ((McqItem)form.ItemList[1]);

			Process process = (Process)Project.Current.ProcessList[0];
			Variable var = process.Variables[0];

			Assert.AreEqual(1, Project.Current.DocumentList.Count);
			RtfDocument rtfDocument = (RtfDocument)Project.Current.DocumentList[0];
			Assert.AreEqual(3, rtfDocument.Contents.Count);
			Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);
			Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[1]);
			Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[2]);

			Paragraph paragraph = (Paragraph)rtfDocument.Contents[0];
			Assert.IsInstanceOfType(typeof(DocumentField), paragraph.Contents[1]);
			Assert.AreSame(blank, ((DocumentField)paragraph.Contents[1]).Field);
			Assert.AreEqual("Q1:a", ((DocumentField)paragraph.Contents[1]).Field.FieldName);

			paragraph = (Paragraph)rtfDocument.Contents[1];
			Assert.IsInstanceOfType(typeof(DocumentField), paragraph.Contents[1]);
			Assert.AreSame(mcItem, ((DocumentField)paragraph.Contents[1]).Field);
			Assert.AreEqual("Q2", ((DocumentField)paragraph.Contents[1]).Field.FieldName);

			paragraph = (Paragraph)rtfDocument.Contents[2];
			Assert.IsInstanceOfType(typeof(DocumentField), paragraph.Contents[1]);
			Assert.AreSame(var, ((DocumentField)paragraph.Contents[1]).Field);
			Assert.AreEqual("Var", ((DocumentField)paragraph.Contents[1]).Field.FieldName);
		}

		private const string NEWLINE = "\r\n";

		private string rtfStringPrefix =
			@"{\rtf1\ansi\ansicpg1252\uc1\deff0" + NEWLINE +
			@"{\fonttbl" + NEWLINE +
			@"{\f0\fswiss Arial;}" + NEWLINE +
			@"{\f1\froman Symbol;}" + NEWLINE +
			@"}" + NEWLINE +
			@"\fs20" +
			@"{\colortbl;" + NEWLINE +
			@"\red0\green0\blue0;" + NEWLINE +
			@"\red255\green255\blue255;" + NEWLINE +
			@"}" + NEWLINE +
			RtfConstants.DefaultTabsRtf +
			@"\pard ";

		private const string rtfStringPostfix = @"}";

		[Test]
		public void ConvertOldHtmlFormat()
		{
			TawalaProjectConverter converter = GetConverter("DocumentHtml.tawala");

			converter.ConvertXmlToProject();

			Assert.AreEqual(1, Project.Current.DocumentList.Count);

			RtfDocument document = (RtfDocument)Project.Current.DocumentList[0];
			Assert.AreEqual("My Document", document.Name);

			string expectedRtf =
				rtfStringPrefix +
				@"{\f0\fs20\cf1 A simple document test.}\par " +
				rtfStringPostfix;

			Assert.AreEqual(expectedRtf, document.Rtf);
			Assert.AreEqual("", document.Text);
		}

		[Test]
		public void ConvertOldHtmlFormatWithFields()
		{
			TawalaProjectConverter converter = GetConverter("DocumentHtmlWithFields.tawala");
			converter.ConvertXmlToProject();

			Assert.AreEqual(1, Project.Current.DocumentList.Count);

			RtfDocument document = (RtfDocument)Project.Current.DocumentList[0];
			Assert.AreEqual("Document 1", document.Name);

			string expectedRtf =
				rtfStringPrefix +
				@"{\f0\fs20\cf1 A document with a field <<Q1:a>>.}\par \pard " +
				@"\par \pard " +
				@"{\f0\fs20\cf1 Another line and another field <<Q2>>.}\par " +
				rtfStringPostfix;

			Assert.AreEqual(expectedRtf, document.Rtf);
			Assert.AreEqual("", document.Text);
		}

		[Test]
		public void PreHtmlFormat()
		{
			TawalaProjectConverter converter = GetConverter("DocumentOldTextFormat.xml");
			converter.ConvertXmlToProject();

			RtfDocument document = (RtfDocument)Project.Current.DocumentList[0];

			///	NOTE:	When fields separated by only white space are read in, the white space is "lost" by the time
			///			it becomes an XmlElement. Since we are reading an obsolete format, this isn't worth fixing.
			///			Dropped white space can be re-entered manually in the Document editor.
			///			
			//string expectedRtf =
			//    RtfDocument.RtfStringPrefix +
			//    @"First name is <<First>>. Last name is <<Last>>. Composited name is <<First>> <<Last>>. Full name is <<Full Name>>. Favorite fruits are <<Q3>>.\par " +
			//    RtfDocument.RtfStringPostfix;

			string expectedRtf =
				rtfStringPrefix +
				@"{\f0\fs20\cf1 First name is <<First>>. Last name is <<Last>>. Composited name is <<First>><<Last>>. Full name is <<Full Name>>. Favorite fruits are <<Q3>>.}\par " +
				rtfStringPostfix;

			Assert.AreEqual(expectedRtf, document.Rtf);
			Assert.AreEqual("", document.Text);
		}

		[Test]
		public void PreHtmlConcatenatedFields()
		{
			TawalaProjectConverter converter = GetConverter("DocumentOldConcatenatedFields.tawala");
			converter.ConvertXmlToProject();

			RtfDocument document = (RtfDocument)Project.Current.DocumentList[0];

			///	NOTE:	When fields separated by only white space are read in, the white space is "lost" by the time
			///			it becomes an XmlElement. Since we are reading an obsolete format, this isn't worth fixing.
			///			Dropped white space can be re-entered manually in the Document editor.
			///			
			//string expectedRtf =
			//    RtfDocument.RtfStringPrefix +
			//    @"Two fields separated by a space: <<Q1:a>> <<Q2:a>>. " +
			//    @"Two fields with no space between them: <<Q1:a>><<Q2:a>>. " +
			//    @"Two fields separated by a word: <<Q1:a>> word <<Q2:a>>." +
			//    @"\par " +
			//    RtfDocument.RtfStringPostfix;

			string expectedRtf =
				rtfStringPrefix +
				@"{\f0\fs20\cf1 Two fields separated by a space: <<Q1:a>><<Q2:a>>. " +
				@"Two fields with no space between them: <<Q1:a>><<Q2:a>>. " +
				@"Two fields separated by a word: <<Q1:a>> word <<Q2:a>>.}" +
				@"\par " +
				rtfStringPostfix;

			Assert.AreEqual(expectedRtf, document.Rtf);
			Assert.AreEqual("", document.Text);
		}

		[Test]
		public void PreHtmlMultiLine()
		{
			TawalaProjectConverter converter = GetConverter("DocumentOldMultiLine.tawala");
			converter.ConvertXmlToProject();

			RtfDocument document = (RtfDocument)Project.Current.DocumentList[0];

			//string expectedText =
			//    Document.RawHtmlPrefix +
			//    "<p><span style=\"font-size:10pt;\">One line followed by a CR/LF.</span></p>\r\n" +
			//    "<p><span style=\"font-size:10pt;\">Second line followed by two CR/LFs.</span></p>\r\n" +
			//    "<p><span style=\"font-size:10pt;\">&nbsp;</span></p>\r\n" +
			//    "<p><span style=\"font-size:10pt;\">Third line followed by three CR/LF's</span></p>\r\n" +
			//    "<p><span style=\"font-size:10pt;\">&nbsp;</span></p>\r\n" +
			//    "<p><span style=\"font-size:10pt;\">&nbsp;</span></p>\r\n" +
			//    "<p><span style=\"font-size:10pt;\">Fourth and final line.</span></p>" +
			//    Document.RawHtmlPostfix;

			//Assert.AreEqual(expectedText, document.Text);

			string expectedRtf =
				rtfStringPrefix +
				@"{\f0\fs20\cf1 One line followed by a CR/LF.}\par \pard " +
				@"{\f0\fs20\cf1 Second line followed by two CR/LFs.}\par \pard " +
				@"\par \pard " +
				@"{\f0\fs20\cf1 Third line followed by three CR/LF's}\par \pard " +
				@"\par \pard " +
				@"\par \pard " +
				@"{\f0\fs20\cf1 Fourth and final line.}" +
				@"\par " +
				rtfStringPostfix;

			Assert.AreEqual(expectedRtf, document.Rtf);

			///	NOTE:	When a Document that begins with one or more CRLFs is read in, one of those CRLFs will
			///			not be "dropped" by the conversion process. Since we are reading an obsolete format,
			///			this isn't worth fixing. Dropped newlines can be re-entered manually in the Document editor.
		}
	}
}
