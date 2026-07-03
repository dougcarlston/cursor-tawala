using System;
using NUnit.Framework;
using System.Text;
using Tawala.Projects;
using Tawala.Common;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using Tawala.RtfSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the FibItem class
	/// </summary>
	[TestFixture]
	public class FibItemTest
	{
		private IForm form;

		private const string ninePointFontStartTag = "<font size=\"180\" color=\"000000\">";
		private const string tenPointFontStartTag = "<font size=\"200\">";

		private string defaultFibStyleAtttribute = " style=\"topLabels\"";

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();
			form = Project.Current.AddForm();
		}

		[Test]
		public void NewFibItem() 
		{ 
			FibItem item = new FibItem();

			Assert.AreEqual(1, item.BlankList.Count);
		}

		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
				"<fib label=\"Q1\" alternateLabel=\"My FIB\">Fib Item 1: <blank label=\"a\" length=\"10\" required=\"true\"></blank></fib>";

			IXmlElement element = new XmlElement(xmlString, true);
			FibItem item = new FibItem(element);

			Assert.AreEqual("Fib Item 1: __________", item.Text);
			Assert.AreEqual("My FIB", item.AlternateLabel);
			Assert.AreEqual(1, item.BlankList.Count);
			Assert.AreEqual(true, item.BlankList[0].Required);
		}

		[Test]
		public void ConstructFromFormattedXml()
		{
			string xmlString =
				"<fib label=\"Q1\" alternateLabel=\"My FIB\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">Fib Item 1: </font>" +
                "<blank label=\"a\" length=\"10\" required=\"true\"></blank>" +
				"</paragraph>" +
				"</fib>";

			IXmlElement element = new XmlElement(xmlString, true);
			FibItem item = new FibItem(element);

			Assert.AreEqual("Fib Item 1: __________", item.Text);
			Assert.AreEqual("My FIB", item.AlternateLabel);
			Assert.AreEqual(1, item.BlankList.Count);
			Assert.AreEqual(true, item.BlankList[0].Required);
		}

		[Test]
		public void ConstructFromXmlWithTabsAndTabstops()
		{
			string xmlString =
				"<fib label=\"Q1\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<tabPositions>" +
				"<tabStop position=\"720\"/>" +
				"<tabStop position=\"1440\"/>" +
				"<tabStop position=\"2160\"/>" +
				"<tabStop position=\"2880\"/>" +
				"<tabStop position=\"3600\"/>" +
				"<tabStop position=\"4320\"/>" +
				"<tabStop position=\"5040\"/>" +
				"<tabStop position=\"5760\"/>" +
				"<tabStop position=\"6480\"/>" +
				"<tabStop position=\"7200\"/>" +
				"<tabStop position=\"7920\"/>" +
				"<tabStop position=\"8640\"/>" +
				"<tabStop position=\"9360\"/>" +
				"<tabStop position=\"10080\"/>" +
				"</tabPositions>" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">Column 1</font>" +
				"<tab/>" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">Column 2</font>" +
				"<tab/>" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">Column 3</font>" +
				"</paragraph>" +
				"</fib>";

			IXmlElement element = new XmlElement(xmlString, true);
			FibItem item = new FibItem(element);

			Assert.IsInstanceOfType(typeof(FormItemParagraph), item.Contents[0]);
			FormItemParagraph paragraph = item.Contents[0] as FormItemParagraph;

			int i = 0;
			Assert.AreEqual(720, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(1440, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(2160, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(2880, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(3600, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(4320, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(5040, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(5760, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(6480, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(7200, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(7920, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(8640, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(9360, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(10080, paragraph.TabPositions[i++].Position);
			Assert.AreEqual(i, paragraph.TabPositions.Count);

			i = 0;
			Assert.IsInstanceOfType(typeof(FormItemFontAttributes), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(DocumentTab), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(FormItemFontAttributes), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(DocumentTab), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(FormItemFontAttributes), paragraph.Contents[i++]);
			Assert.AreEqual(i, paragraph.Contents.Count);
		}

		[Test]
		public void ConstructFromXmlWithBlankBetweenTabs()
		{
			string xmlString =
				"<fib label=\"Q1\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<tabPositions>" +
				"<tabStop position=\"1440\"/>" +
				"<tabStop position=\"2880\"/>" +
				"</tabPositions>" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">Column 1</font>" +
				"<tab/>" +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
				"<tab/>" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">Column 3</font>" +
				"</paragraph>" +
				"</fib>";

			IXmlElement element = new XmlElement(xmlString, true);
			FibItem item = new FibItem(element);

			Assert.IsInstanceOfType(typeof(FormItemParagraph), item.Contents[0]);
			FormItemParagraph paragraph = item.Contents[0] as FormItemParagraph;

			int i = 0;
			Assert.IsInstanceOfType(typeof(FormItemFontAttributes), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(DocumentTab), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(Blank), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(DocumentTab), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(FormItemFontAttributes), paragraph.Contents[i++]);
			Assert.AreEqual(i, paragraph.Contents.Count);

			Assert.AreEqual("Column 1\t__________\tColumn 3", item.Text);
		}

		[Test]
		public void FormattedXmlFromUnformattedXml()
		{
			string xmlString =
                "<fib label=\"Q1\" alternateLabel=\"My FIB\">Fib Item 1: <blank label=\"a\" length=\"10\" required=\"true\"></blank></fib>";

			string expString =
				"<fib label=\"Q1\" alternateLabel=\"My FIB\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font size=\"180\" color=\"000000\">Fib Item 1: </font>" +
                "<blank label=\"a\" length=\"10\" required=\"true\"></blank>" +
				"</paragraph>" +
				"</fib>\r\n";

			IXmlElement element = new XmlElement(xmlString, true);
			FibItem item = new FibItem(element);

			Assert.AreEqual(expString, item.ToXml("Q1"));
		}

		[Test]
		public void FormattedXmlFromUnformattedXmlWithSpaceBetweenBlanks()
		{
			string xmlString =
                "<fib label=\"Q1\">Fib Item 1: <blank label=\"a\" length=\"10\" required=\"true\"></blank> <blank label=\"b\" length=\"10\" required=\"true\"></blank></fib>";

			string expString =
				"<fib label=\"Q1\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font size=\"180\" color=\"000000\">Fib Item 1: </font>" +
                "<blank label=\"a\" length=\"10\" required=\"true\"></blank>" +
				"<font size=\"180\" color=\"000000\"> </font>" +
                "<blank label=\"b\" length=\"10\" required=\"true\"></blank>" +
				"</paragraph>" +
				"</fib>\r\n";

			IXmlElement element = new XmlElement(xmlString, true);
			FibItem item = new FibItem(element);

			Assert.AreEqual(expString, item.ToXml("Q1"));
		}

		[Test]
		public void FormattedXmlFromFormattedXmlWithSpaceBetweenBlanks()
		{
			string xmlString =
				"<fib label=\"Q1\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont + "Fib Item 1: " + XmlConstants.EndFont +
                "<blank label=\"a\" length=\"10\" required=\"true\"></blank>" +
				ninePointFontStartTag + " " + XmlConstants.EndFont +
                "<blank label=\"b\" length=\"5\" required=\"true\"></blank>" +
				"</paragraph>" +
				"</fib>\r\n";

			IXmlElement element = new XmlElement(xmlString, true);
			FibItem item = new FibItem(element);

			Assert.AreEqual(xmlString, item.ToXml("Q1"));
		}

		[Test]
		public void ConstructFromFormattedXmlWithMultipleParagraphs()
		{
			string xmlString =
				"<fib label=\"Q1\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">Name: </font>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\"><blank label=\"a\" length=\"10\" required=\"false\"></blank></font>" +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">Address: </font>" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\"><blank label=\"a\" length=\"10\" required=\"false\"></blank></font>" +
				"</paragraph>" +
				"</fib>";

			IXmlElement element = new XmlElement(xmlString, true);
			FibItem item = new FibItem(element);

			string expectedText =
				"Name: __________" +
				"Address: __________";

			Assert.AreEqual(expectedText, item.Text);
			Assert.AreEqual(2, item.BlankList.Count);
		}

		[Test]
		public void ConstructFromXmlWithSpaceBetweenBlanks()
		{
			string xmlString =
                "<fib label=\"Q1\" alternateLabel=\"My FIB\">Fib Item 1: <blank label=\"a\" length=\"10\" required=\"true\"></blank> <blank label=\"b\" length=\"10\" required=\"true\"></blank></fib>";

			IXmlElement element = new XmlElement(xmlString, true);
			FibItem item = new FibItem(element);

			Assert.AreEqual("Fib Item 1: __________ __________", item.Text);
			Assert.AreEqual(2, item.BlankList.Count);
		}

		[Test]
		public void ConstructFromXmlWithLinefeedAtEnd()
		{
			string xmlString =
                "<fib label=\"Q1\">Fib Item 1: <blank label=\"a\" length=\"10\" required=\"false\"></blank>\n</fib>";

			IXmlElement element = new XmlElement(xmlString, true);
			FibItem item = new FibItem(element);

			Assert.AreEqual(1, item.Contents.Count);
			Assert.IsInstanceOfType(typeof(FormItemParagraph), item.Contents[0]);

			FormItemParagraph paragraph = item.Contents[0] as FormItemParagraph;
			Assert.AreEqual(3, paragraph.Contents.Count);
			Assert.IsInstanceOfType(typeof(FormItemText), paragraph.Contents[0]);

			Assert.AreEqual("Fib Item 1: __________\n", item.Text);
		}

		[Test]
		public void GetXmlWithLinefeedAtEnd()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\fmodern\fcharset0\fprq1 Courier New;}" + "\r\n" +
				@"{\f2\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0" +
				@"\blue0;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20\cf3" +
				@" Fib Item 1: __________" +
				@"\plain\f1\fs20\cf3\par\par }";

			FibItem item = new FibItem();
			item.Rtf = rtfString;

			string expString =
				"<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Fib Item 1: " +
				XmlConstants.EndFont +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"</paragraph>" +
				"</fib>\r\n";

			Assert.AreEqual(expString, item.ToXml("Q1"));
		}

		[Test]
		public void GetXmlWithLabel() 
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0" +
				@"\blue0;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20\cf3" +
				@" First Name: ____________________ Last Name: ____________________" +
				@"\par }";

			FibItem item = new FibItem();
			form.ItemList.Add(item);

			item.Rtf = rtfString;

			string expString =
				"<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"First Name: " +
				XmlConstants.EndFont +
                "<blank label=\"a\" length=\"20\" required=\"false\"></blank>" +
				XmlConstants.FullBeginFont +
				" Last Name: " +
				XmlConstants.EndFont +
                "<blank label=\"b\" length=\"20\" required=\"false\"></blank>" +
				"</paragraph>" +
				"</fib>\r\n";

			Assert.AreEqual(expString, item.ToXml("Q1"));
		}

		[Test]
		public void GetXmlWithAlternateLabel()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0" +
				@"\blue0;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20\cf3" +
				@" First Name: ____________________ Last Name: ____________________" +
				@"\par }";

			FibItem item = new FibItem();
			item.AlternateLabel = "MyFIB";
			item.Rtf = rtfString;

			string expString = 
				"<fib label=\"Q1\" alternateLabel=\"MyFIB\"" + defaultFibStyleAtttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"First Name: " +
				XmlConstants.EndFont +
                "<blank label=\"a\" length=\"20\" required=\"false\"></blank>" +
				XmlConstants.FullBeginFont +
				" Last Name: " +
				XmlConstants.EndFont +
                "<blank label=\"b\" length=\"20\" required=\"false\"></blank>" +
				"</paragraph>" +
				"</fib>\r\n";

			Assert.AreEqual(expString, item.ToXml("Q1"));
		}

		[Test]
		public void GetXmlWithAlternateBlankLabel()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0" +
				@"\blue0;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20\cf3" +
				@" First Name: ____________________ Last Name: ____________________" +
				@"\par }";

			FibItem item = new FibItem();
			item.Rtf = rtfString;

			Blank blank1 = item.BlankList[0];
			blank1.AlternateLabel = "First";
			Assert.AreEqual("First", blank1.AlternateLabel);

			Blank blank2 = item.BlankList[1];
			blank2.AlternateLabel = "Last";
			Assert.AreEqual("Last", blank2.AlternateLabel);

			string expString =
				"<fib label=\"Q1\"" + defaultFibStyleAtttribute +  ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"First Name: " +
				XmlConstants.EndFont +
                "<blank label=\"a\" length=\"20\" required=\"false\" alternateLabel=\"First\"></blank>" +
				XmlConstants.FullBeginFont +
				" Last Name: " +
				XmlConstants.EndFont +
                "<blank label=\"b\" length=\"20\" required=\"false\" alternateLabel=\"Last\"></blank>" +
				"</paragraph>" +
				"</fib>\r\n";

			Assert.AreEqual(expString, item.ToXml("Q1"));
		}

		[Test]
		public void VerifyBlankLabels() 
		{
			const int arraysize = 78;

			string[] xmlBlankStrings = new string[arraysize];
			string[] labelStrings = new string[arraysize]	{"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
															 "aa", "bb", "cc", "dd", "ee", "ff", "gg", "hh", "ii", "jj", "kk", "ll", "mm", "nn", "oo", "pp", "qq", "rr", "ss", "tt", "uu", "vv", "ww", "xx", "yy", "zz",
															 "aaa", "bbb", "ccc", "ddd", "eee", "fff", "ggg", "hhh", "iii", "jjj", "kkk", "lll", "mmm", "nnn", "ooo", "ppp", "qqq", "rrr", "sss", "ttt", "uuu", "vvv", "www", "xxx", "yyy", "zzz"};
			
			for (int i = 0; i < arraysize; i++)
			{
                xmlBlankStrings[i] = "<blank label=\"" + labelStrings[i] + "\" length=\"1\" required=\"false\"></blank>";
			}

			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0" +
				@"\blue0;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20\cf3" +
				@" Blanks: _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _" +
				@" _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _" +
				@"\par }";

			FibItem item = new FibItem();
			item.Rtf = rtfString;

			StringBuilder expString = new StringBuilder("<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">");
			expString.Append("<paragraph indent=\"0\" align=\"left\">");

			for (int i = 0; i < arraysize; i++)
			{
				expString.Append(XmlConstants.FullBeginFont);

				if (i == 0)
				{
					expString.Append("Blanks: ");
				}
				else if (i < arraysize)
				{
					expString.Append(" ");
				}

				expString.Append(XmlConstants.EndFont);

				expString.Append (xmlBlankStrings[i]);
			}

			expString.Append("</paragraph>");
			expString.Append("</fib>\r\n");

			//Assertions
			Assert.AreEqual(expString.ToString(), item.ToXml("Q1"));
			Assert.AreEqual(78, item.BlankList.Count);
		}

		[Test]
		public void IsQuestionItem() 
		{ 
			FibItem item = new FibItem();

			//Assertions 
			Assert.AreEqual(true, item.IsQuestionItem);
			Assert.AreEqual(false, item.IsTextItem);
		}

		[Test]
		public void FieldName()
		{
			Project.NewTestProject();
			IForm form = Project.Current.AddForm();

			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			Assert.AreEqual("Q1", fibItem1.FieldName);
		}

		[Test]
		public void FieldString()
		{
			Project.NewTestProject();
			IForm form = Project.Current.AddForm();

			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			Assert.AreEqual("<<Q1>>", fibItem1.FieldString);
		}

		[Test]
		public void OperatorDataSource()
		{
			FibItem fibItem = new FibItem();
			Assert.AreEqual(HybridOperator.List.DataSource, fibItem.OperatorDataSource);
		}

		[Test]
		public void ModifyFibText()
		{
			FibItem item = new FibItem();
			form.ItemList.Add(item);

			item.Text = "Name: __________";
			Blank blank1 = item.BlankList[0];
			Assert.AreEqual(10, blank1.Length);
			Assert.AreEqual("Q1:a", blank1.FieldName);

			item.Text = "First Name: __________";
			Blank blank2 = item.BlankList[0];
			Assert.AreEqual(10, blank2.Length);
			Assert.AreEqual("Q1:a", blank2.FieldName);
			Assert.AreEqual("Q1:a", blank1.FieldName);
			Assert.AreSame(blank1, blank2);
		}

		[Test]
		public void ModifyFibBlanks()
		{
			FibItem item = new FibItem();
			form.ItemList.Add(item);

			item.Text = "Name: __________";
			Blank blank1 = item.BlankList[0];
			Assert.AreEqual(10, blank1.Length);
			Assert.AreEqual("Q1:a", blank1.FieldName);

			item.Text = "Name: __________ __________";
			Blank blank2 = item.BlankList[0];
			Assert.AreEqual(10, blank2.Length);
			Assert.AreEqual("Q1:a", blank2.FieldName);
			Assert.AreEqual("Q1:a", blank1.FieldName);
			Assert.AreSame(blank1, blank2);

			item.Text = "Name: __________";
			Blank blank3 = item.BlankList[0];
			Assert.AreEqual(10, blank2.Length);
			Assert.AreEqual("Q1:a", blank3.FieldName);
			Assert.AreEqual("Q1:a", blank2.FieldName);
			Assert.AreEqual("Q1:a", blank1.FieldName);
			Assert.AreSame(blank1, blank2);
			Assert.AreSame(blank2, blank3);
		}

		[Test]
		public void RemovingFibBlankReducesBlankListCount()
		{
			FibItem item = new FibItem();
			form.ItemList.Add(item);

			item.Text = "Name: __________";
			Assert.AreEqual(1, item.BlankList.Count);

			item.Text = "Name: __________ __________";
			Assert.AreEqual(2, item.BlankList.Count);

			item.Text = "Name: __________";
			Assert.AreEqual(1, item.BlankList.Count);
		}

		[Test]
		public void RtfToObject()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + Environment.NewLine +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + Environment.NewLine +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + Environment.NewLine +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + Environment.NewLine +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + Environment.NewLine +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + Environment.NewLine +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0\fs20 Name: __________\par }";

			FibItem item = new FibItem();
			item.Rtf = rtfString;

			Assert.AreEqual(1, item.BlankList.Count);
			Assert.AreEqual(10, item.BlankList[0].Length);
		}

		[Test]
		public void RtfWithMultipleBlanksToObject()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20" +
				@" Name: __________" +
				@"\par" +
				@" Address: __________" +
				@"\par }";

			FibItem item = new FibItem();
			form.ItemList.Add(item);

			item.Rtf = rtfString;

			Assert.AreEqual(2, item.BlankList.Count);
			
			Assert.AreEqual(10, item.BlankList[0].Length);
			Assert.AreEqual("Q1:a", item.BlankList[0].FieldName);

			Assert.AreEqual(10, item.BlankList[1].Length);
			Assert.AreEqual("Q1:b", item.BlankList[1].FieldName);
		}

		[Test]
		public void RtfWithBlankBetweenTabsToObject()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20" +
				@" Column 1" +
				@"\tab" +
				@" __________" +
				@"\tab" +
				@" Column 3" +
				@"\par }";

			FibItem item = new FibItem();
			form.ItemList.Add(item);

			item.Rtf = rtfString;

			Assert.IsInstanceOfType(typeof(FormItemParagraph), item.Contents[0]);
			FormItemParagraph paragraph = item.Contents[0] as FormItemParagraph;

			int i = 0;
			Assert.IsInstanceOfType(typeof(FormItemFontAttributes), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(DocumentTab), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(Blank), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(DocumentTab), paragraph.Contents[i++]);
			Assert.IsInstanceOfType(typeof(FormItemFontAttributes), paragraph.Contents[i++]);
			Assert.AreEqual(i, paragraph.Contents.Count);

			Assert.AreEqual("Column 1\t__________\tColumn 3", item.Text);
		}

		[Test]
		public void RtfToRtf()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + Environment.NewLine +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + Environment.NewLine +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + Environment.NewLine +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + Environment.NewLine +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + Environment.NewLine +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + Environment.NewLine +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0\fs20 Name: __________\par }";

			FibItem item = new FibItem();
			item.Rtf = rtfString;

			string expectedRtf =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
				@"{\fonttbl" + Environment.NewLine +
				@"{\f0\fswiss Arial;}" + Environment.NewLine +
				@"}" + Environment.NewLine +
				@"\fs20" +
				@"{\colortbl;" + Environment.NewLine +
				@"\red0\green0\blue0;" + Environment.NewLine +
				@"\red255\green255\blue255;" + Environment.NewLine +
				@"}" + Environment.NewLine +
				RtfConstants.DefaultTabsRtf +
				@"\pard " +
				@"{\f0\fs20\cf1 Name: }__________\par }";

			Assert.AreEqual(expectedRtf, item.ToRtf());
		}

		[Test]
		public void RtfWithMultipleBlanksToRtf()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20" +
				@" Name: __________" +
				@"\par" +
				@" Address: ____________________" +
				@"\par }";

			FibItem item = new FibItem();
			item.Rtf = rtfString;

			string expectedRtf =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
				@"{\fonttbl" + Environment.NewLine +
				@"{\f0\fswiss Arial;}" + Environment.NewLine +
				@"}" + Environment.NewLine +
				@"\fs20" +
				@"{\colortbl;" + Environment.NewLine +
				@"\red0\green0\blue0;" + Environment.NewLine +
				@"\red255\green255\blue255;" + Environment.NewLine +
				@"}" + Environment.NewLine +
				RtfConstants.DefaultTabsRtf +
				@"\pard " +
				@"{\f0\fs20\cf1 Name: }__________\par " +
				@"\pard " +
				@"{\f0\fs20\cf1 Address: }____________________\par }";

			Assert.AreEqual(expectedRtf, item.ToRtf());
		}

		[Test]
		public void RtfToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + Environment.NewLine +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + Environment.NewLine +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + Environment.NewLine +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + Environment.NewLine +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + Environment.NewLine +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + Environment.NewLine +
				@"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\plain\f0\fs20 Name: __________\par }";

			FibItem item = new FibItem();

			item.Rtf = rtfString;

			string xmlString =
				"<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Name: " +
				XmlConstants.EndFont +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
				"</paragraph>" +
				"</fib>\r\n";

			Assert.AreEqual(xmlString, item.ToXml("Q1"));
		}

		[Test]
		public void RtfWithMultipleBlanksToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20" +
				@" Name: __________" +
				@"\par" +
				@" Address: __________" +
				@"\par }";

			FibItem item = new FibItem();
			form.ItemList.Add(item);

			item.Rtf = rtfString;

			string xmlString =
				"<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Name: " +
				XmlConstants.EndFont +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Address: " +
				XmlConstants.EndFont +
                "<blank label=\"b\" length=\"10\" required=\"false\"></blank>" +
				"</paragraph>" +
				"</fib>\r\n";

			Assert.AreEqual(xmlString, item.ToXml("Q1"));
		}

		[Test]
		public void RtfWithMultipleBlanksAndSpacesOnSecondLineToXml()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20" +
				@" Name: __________" +
				@"\par" +
				@"        __________" +
				@"\par }";

			FibItem item = new FibItem();
			form.ItemList.Add(item);

			item.Rtf = rtfString;

			string xmlString =
				"<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Name: " +
				XmlConstants.EndFont +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"       " +
				XmlConstants.EndFont +
                "<blank label=\"b\" length=\"10\" required=\"false\"></blank>" +
				"</paragraph>" +
				"</fib>\r\n";

			Assert.AreEqual(xmlString, item.ToXml("Q1"));
		}

		[Test]
		public void RtfWithMultipleBlanksOnSameLineToXml()
		{
			string halfInchTabsRTF =
				@"\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080";

			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0" +
				@"\blue0;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0" +
				halfInchTabsRTF +
				@"\plain\f0\fs20" +
				@"\cf3" +
				@" FIB " +
				@"\plain\f0\fs20" +
				@" __________" +
				@"\plain\f0\fs20\cf3" +
				@"  " +
				@"\plain\f0\fs20" +
				@" _____" +
				@"\par\par }";

			FibItem item = new FibItem();
			form.ItemList.Add(item);

			item.Rtf = rtfString;

			string halfInchTabsXML =
				"<tabPositions>" +
				"<tabStop position=\"720\"/>" +
				"<tabStop position=\"1440\"/>" +
				"<tabStop position=\"2160\"/>" +
				"<tabStop position=\"2880\"/>" +
				"<tabStop position=\"3600\"/>" +
				"<tabStop position=\"4320\"/>" +
				"<tabStop position=\"5040\"/>" +
				"<tabStop position=\"5760\"/>" +
				"<tabStop position=\"6480\"/>" +
				"<tabStop position=\"7200\"/>" +
				"<tabStop position=\"7920\"/>" +
				"<tabStop position=\"8640\"/>" +
				"<tabStop position=\"9360\"/>" +
				"<tabStop position=\"10080\"/>" +
				"</tabPositions>";

			string xmlString =
				"<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				halfInchTabsXML +
				XmlConstants.FullBeginFont +
				"FIB " +
				XmlConstants.EndFont +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
				"<sp/>" +
                "<blank label=\"b\" length=\"5\" required=\"false\"></blank>" +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				halfInchTabsXML +
				"</paragraph>" +
				"</fib>\r\n";

			Assert.AreEqual(xmlString, item.ToXml("Q1"));
		}

		private static void verifyObjectsForFibWithField(FibItem fibItem)
		{
			Assert.AreEqual(1, fibItem.BlankList.Count);
			Assert.AreEqual(10, fibItem.BlankList[0].Length);

			Assert.AreEqual(1, fibItem.Contents.Count);
			Assert.IsInstanceOfType(typeof(Paragraph), fibItem.Contents[0]);

			Paragraph paragraph = (Paragraph)fibItem.Contents[0];
			Assert.AreEqual(4, paragraph.Contents.Count);

			Assert.IsInstanceOfType(typeof(FormItemFontAttributes), paragraph.Contents[1]);
			FormItemFontAttributes fontAttributes = (FormItemFontAttributes)paragraph.Contents[1];
			Assert.AreEqual(@"<<Form 1:Q1:a>>", fontAttributes.Text);
		}

		[Test]
		public void InitializeWithRtfContainingField()
		{
			string rtfFieldString1 =
				RtfDocument.RtfStringPrefix +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20" +
				@" FIB with field " +
				@"\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags216";

			string rtfFieldString2 =
				@"\txfielddataval{0}";

			string rtfFieldString3 =
				@"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
				@"<<Form 1:Q1:a>>{" +
				@"\*\txfieldend}\plain\f0\fs20" +
				@"  __________" +
				@"\par }";

			FibItem referenceField = new FibItem();
			form.ItemList.Add(referenceField);

			string rtfString =
				rtfFieldString1 +
				string.Format(rtfFieldString2, referenceField.BlankList[0].Id) +
				rtfFieldString3;

			FibItem fibItem = new FibItem();
			fibItem.Rtf = rtfString;

			verifyObjectsForFibWithField(fibItem);
		}

		[Test]
		public void ConstructFromXmlWithField()
		{
			FibItem referenceField = new FibItem();
			form.ItemList.Add(referenceField);

			string xmlString =
				"<fib label=\"Q1\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				tenPointFontStartTag +
				"FIB with field " +
				XmlConstants.EndFont +
				tenPointFontStartTag +
				"<field name=\"Form 1:Q1:a\"/>" +
				XmlConstants.EndFont +
				tenPointFontStartTag +
				" " +
				XmlConstants.EndFont +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
				"</paragraph>" +
				"</fib>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			FibItem fibItem = new FibItem(element);

			verifyObjectsForFibWithField(fibItem);
		}
	}
}
