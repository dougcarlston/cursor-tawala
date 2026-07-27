using System;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
    [TestFixture]
	public class SpecialXmlCharactersInFieldNameInDocumentCauseBadSave519
    {
		private Blank blank;

		[SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();

			IForm form = Project.Current.AddForm();
			FibItem fib = new FibItem();
			form.ItemList.Add(fib);
			blank = fib.BlankList[0];
			blank.AlternateLabel = testLabel;
		}

		private const string testLabel = "\"<This & That>\"";

		[Test]
		public void SpecialCharactersInFieldNameInDocumentGenerateValidXML()
		{
			RtfDocument document = Project.Current.AddDocument() as RtfDocument;

			document.Rtf =
				RtfConstants.DefaultRtfPrologue +
				@"{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", blank.Id) +
				@"\txfielddata 54004600240046006f0072006d00200031003a0022003c00540068006900730020002600200054006800610074003e0022000000}" +
				"<<Form 1:\"< This & That >\">>" +
				@"{\*\txfieldend}\par " +
				RtfConstants.DefaultRtfEpilogue;

			string expectedXml =
				"<document name=\"Document 1\">" + Environment.NewLine +
				"<xmlData>" + Environment.NewLine +
				"<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions>" +
				XmlConstants.FullBeginFont +
				"<field name=\"Form 1:&quot;&lt;This &amp; That&gt;&quot;\"/>" +
				XmlConstants.EndFont +
				"</paragraph>" + Environment.NewLine +
				"</xmlData>" + Environment.NewLine +
				"</document>" + Environment.NewLine;
			
			Assert.AreEqual(expectedXml, document.ToXml());
		}

		[Test]
		public void SpecialCharactersInFieldNameInTextItemGenerateValidXML()
		{
			IForm form2 = Project.Current.AddForm();
			TextItem textItem = new TextItem();
			form2.ItemList.Add(textItem);

			textItem.Rtf =
				RtfConstants.DefaultRtfPrologue +
				@"{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", blank.Id) +
				@"\txfielddata 54004600240046006f0072006d00200031003a0022003c00540068006900730020002600200054006800610074003e0022000000}" +
				"<<Form 1:\"< This & That >\">>" +
				@"{\*\txfieldend}\par " +
				RtfConstants.DefaultRtfEpilogue;

			string expectedXml =
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions>" +
				XmlConstants.FullBeginFont +
				"<field name=\"Form 1:&quot;&lt;This &amp; That&gt;&quot;\"/>" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</text>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}
	}
}
