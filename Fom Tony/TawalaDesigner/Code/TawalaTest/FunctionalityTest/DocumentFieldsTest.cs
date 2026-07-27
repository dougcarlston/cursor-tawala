using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.RtfSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing Fields in Documents
	/// </summary>
	[TestFixture]
	public class DocumentFieldsTest
	{
		private IForm form1;
		private FibItem fibItem1;
		private McqItem mcItem1;
		private RtfDocument rtfDocument;

		private const string NEWLINE = "\r\n";

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
		}

		[SetUp]
		public void SetUp()
		{
			Project.NewTestProject();

			form1 = Project.Current.AddForm();

			fibItem1 = new FibItem();
			form1.ItemList.Add(fibItem1);

			mcItem1 = new McqItem();
			form1.ItemList.Add(mcItem1);

			rtfDocument = Project.Current.AddDocument() as RtfDocument;
		}

		[Test]
		public void ResolveFieldReference()
		{
			Blank blank = fibItem1.BlankList[0];
			int blankId = blank.Id;

			string rtfString =
				RtfDocument.RtfStringPrefix +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", blankId) +
				@"\txfielddata 540046002400510031003a0061000000}" +
				@"<<Q1:a>>{" +
				@"\*\txfieldend}\par }";

			rtfDocument.Rtf = rtfString;

			Paragraph paragraph = (Paragraph)rtfDocument.Contents[0];
			Assert.IsInstanceOfType(typeof(DocumentField), paragraph.Contents[0]);
			Assert.AreEqual("Q1:a", ((DocumentField)paragraph.Contents[0]).Field.FieldName);
		}

		[Test]
		public void QualifiedFieldsToObject()
		{
			string rtfFieldsString =
				RtfDocument.RtfStringPrefix +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", fibItem1.BlankList[0].Id) +
				@"\txfielddata 540046002400510031003a0061000000}" +
				@"<<Q1:a>>{" +
				@"\*\txfieldend}\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", mcItem1.Id) +
				@"\txfielddata 540046002400510032000000}" +
				@"<<Q2>>{" +
				@"\*\txfieldend}\par }";

			rtfDocument.Rtf = rtfFieldsString;

			Paragraph paragraph = (Paragraph)rtfDocument.Contents[0];

			Assert.IsInstanceOfType(typeof(DocumentField), paragraph.Contents[0]);
			Assert.AreEqual("Form 1:Q1:a", ((DocumentField)paragraph.Contents[0]).Field.QualifiedFieldName);

			Assert.IsInstanceOfType(typeof(DocumentField), paragraph.Contents[1]);
			Assert.AreEqual("Form 1:Q2", ((DocumentField)paragraph.Contents[1]).Field.QualifiedFieldName);
		}

		[Test]
		public void QualifiedFieldsToXml()
		{
			string rtfFieldsString =
				RtfDocument.RtfStringPrefix +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", fibItem1.BlankList[0].Id) +
				@"\txfielddata 540046002400510031003a0061000000}" +
				@"<<Q1:a>>{" +
				@"\*\txfieldend}\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", mcItem1.Id) +
				@"\txfielddata 540046002400510032000000}" +
				@"<<Q2>>{" +
				@"\*\txfieldend}\par }";

			rtfDocument.Rtf = rtfFieldsString;

			string xmlString =
				"<document name=\"Document 1\">\r\n" +
				"<xmlData>\r\n" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<field name=\"Form 1:Q1:a\"/>" +
				"<field name=\"Form 1:Q2\"/>" +
				"</paragraph>" +
				"\r\n</xmlData>\r\n" +
				"</document>\r\n";

			Assert.AreEqual(xmlString, rtfDocument.ToXml());
		}

		[Test]
		public void QualifiedFieldsToRtf()
		{
			string rtfFieldsString =
				RtfDocument.RtfStringPrefix +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", fibItem1.BlankList[0].Id) +
				@"\txfielddata 540046002400510031003a0061000000}" +
				@"<<Q1:a>>{" +
				@"\*\txfieldend}\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", mcItem1.Id) +
				@"\txfielddata 540046002400510032000000}" +
				@"<<Q2>>{" +
				@"\*\txfieldend}\par }";

			rtfDocument.Rtf = rtfFieldsString;

			string expectedString =
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
				@"\pard " +
				@"{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", fibItem1.BlankList[0].Id) +
				@"\txfielddata 540046002400510031003a0061000000}" +
				@"<<Form 1:Q1:a>>{" +
				@"\*\txfieldend}" +
				@"{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", mcItem1.Id) +
				@"\txfielddata 540046002400510032000000}" +
				@"<<Form 1:Q2>>{" +
				@"\*\txfieldend}\par }";

			Assert.AreEqual(expectedString, rtfDocument.ToRtf());
		}

		[Test]
		public void DeletedFieldsToXml()
		{
			string rtfFieldsString =
				RtfDocument.RtfStringPrefix +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", fibItem1.BlankList[0].Id) +
				@"\txfielddata 540046002400510031003a0061000000}" +
				@"<<Q1:a>>{" +
				@"\*\txfieldend}\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", mcItem1.Id) +
				@"\txfielddata 540046002400510032000000}" +
				@"<<Q2>>{" +
				@"\*\txfieldend}\par }";

			rtfDocument.Rtf = rtfFieldsString;

			string xmlString =
				"<document name=\"Document 1\">\r\n" +
				"<xmlData>\r\n" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<field name=\"Unknown Field\"/>" +
				"<field name=\"Unknown Field\"/>" +
				"</paragraph>" +
				"\r\n</xmlData>\r\n" +
				"</document>\r\n";

			form1.ItemList.Remove(fibItem1);
			form1.ItemList.Remove(mcItem1);

			Assert.AreEqual(xmlString, rtfDocument.ToXml());
		}
	}
}
