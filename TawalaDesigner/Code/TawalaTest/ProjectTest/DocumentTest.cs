using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Class to test Tawala.Common.Document class.
	/// </summary>
	[TestFixture]
	public class DocumentTest
	{
		[Test]
		public void NewDocument() 
		{ 
			Document doc = new Document("Test");

			//Assertions 
			Assert.IsNotNull(doc);
			Assert.AreEqual("Test", doc.Name);
		}

		[Test]
		public void NameDocument() 
		{ 
			Document doc = new Document("Renamed Document");

			//Assertions 
			Assert.AreEqual("Renamed Document", doc.Name);
		} 

		[Test]
		public void NameDocumentViaConstructor() 
		{ 
			Document doc = new Document("MyDocument");

			//Assertions 
			Assert.AreEqual("MyDocument", doc.Name);
		}

		[Test]
		public void TrimWhitespaceFromName()
		{
			Document doc = new Document("    Document 1  ");
			Assert.AreEqual("Document 1", doc.Name);

			doc.Name = "   Renamed Document   ";
			Assert.AreEqual("Renamed Document", doc.Name);
		}

		[Test]
		public void StoreText() 
		{ 
			Document doc = new Document("MyDocument");

			// store text in document
			string text = "This is some text.";
			doc.Text = text;

			//Assertions 
			Assert.AreEqual("MyDocument", doc.Name);
			Assert.AreEqual("This is some text.", doc.Text);
		}

		[Test]
		public void GetXml()
		{
			Document doc = new Document("Document Name");

			string htmlContent =
				Document.RawHtmlPrefix +
				"<p><span style=\"font-size:10pt;\">Here is some document text.</span></p>\r\n" +
				"<p><span style=\"font-size:10pt;\">With two lines.</span></p>" +
				Document.RawHtmlPostfix;

			doc.Text = htmlContent;

			string expString =
				"<document name=\"Document Name\">\r\n" +
				"<htmlData>\r\n" +
				"<![CDATA[" +
				"<p><span style=\"font-size:10pt;\">Here is some document text.</span></p>\r\n" +
				"<p><span style=\"font-size:10pt;\">With two lines.</span></p>" +
				"]]>\r\n" +
				"</htmlData>\r\n" +
				"<rawHtmlData>\r\n" +
				"<![CDATA[" +
				htmlContent +
				"]]>\r\n" +
				"</rawHtmlData>\r\n" +
				"</document>\r\n";

			Assert.AreEqual(expString, doc.ToXml());
		}

//		[Test]
		public void GetTabularXml()
		{
			Document doc = new Document("Document Name");

			string htmlContent =
				Document.RawHtmlPrefix +
				"<p style=\"margin-top : 2pt ;margin-bottom:3pt\">" +
				"<span style=\"font-family:'Arial';font-size:12pt;\">" +
				"Column1&#9;Column2&#9;Column3" +
				"</span>" +
				"</p>" +
				Document.RawHtmlPostfix;

			doc.Text = htmlContent;

			string expString =
				"<document name=\"Document Name\">\r\n" +
				"<htmlData>\r\n" +
				"<![CDATA[" +
				"<table width=\"700\">" +
				"<tr>" +
				"<td width=\"33%\">" +
				"Column1" +
				"</td>" +
				"<td width=\"33%\">" +
				"Column2" +
				"</td>" +
				"<td width=\"33%\">" +
				"Column3" +
				"</td>" +
				"</tr>" +
				"</table>" +
				"]]>\r\n" +
				"</htmlData>\r\n" +
				"<rawHtmlData>\r\n" +
				"<![CDATA[" +
				htmlContent +
				"]]>\r\n" +
				"</rawHtmlData>\r\n" +
				"</document>\r\n";

			Assert.AreEqual(expString, doc.ToXml());
		}

		[Test]
		public void GetXmlWithFields()
		{
			Document doc = new Document("Document Name");

			string htmlContent =
				Document.RawHtmlPrefix +
				"<p><span style=\"font-size:10pt;\">Here is an FIB field &lt;&lt;Q1:a&gt;&gt; and here is an MC field &lt;&lt;Q2&gt;&gt;.</span></p>" +
				Document.RawHtmlPostfix;

			doc.Text = htmlContent;

			string expString =
				"<document name=\"Document Name\">\r\n" +
				"<htmlData>\r\n" +
				"<![CDATA[" +
				"<p><span style=\"font-size:10pt;\">Here is an FIB field &lt;&lt;Q1:a&gt;&gt; and here is an MC field &lt;&lt;Q2&gt;&gt;.</span></p>" +
				"]]>\r\n" +
				"</htmlData>\r\n" +
				"<rawHtmlData>\r\n" +
				"<![CDATA[" +
				htmlContent +
				"]]>\r\n" +
				"</rawHtmlData>\r\n" +
				"</document>\r\n";

			Assert.AreEqual(expString, doc.ToXml());
		}

#if false	// old XML test prior to changing to HTML content
			// keeping it around as guideline for things to test should we return to our own XML format
		[Test]
		public void GetXml()
		{
			Document doc = new Document("Document Name");

			doc.Text = "Here is some document text.";

			string expString = "<document name=\"Document Name\">" +
								"Here is some document text." +
								"</document>\r\n";
			Assert.AreEqual(expString, doc.ToXml());

			doc.Text = "Here is text with a <<field>>.";
			expString = "<document name=\"Document Name\">" +
								"Here is text with a <field name=\"field\"/>." +
								"</document>\r\n";
			Assert.AreEqual(expString, doc.ToXml());

			doc.Text = "Here is text with two <<field1>> separated <<field2>>.";
			expString = "<document name=\"Document Name\">" +
								"Here is text with two <field name=\"field1\"/> separated <field name=\"field2\"/>." +
								"</document>\r\n";
			Assert.AreEqual(expString, doc.ToXml());


			doc.Text = "Here is text with two <<field1>> <<field2>> separated by a space.";
			expString = "<document name=\"Document Name\">" +
								"Here is text with two <field name=\"field1\"/> <field name=\"field2\"/> separated by a space." +
								"</document>\r\n";
			Assert.AreEqual(expString, doc.ToXml());

			// check for illegal XML characters
			doc.Name = "&<Doc's \"Bad\" Name>";
			doc.Text = "Here is text with a <<&field's <\"Bad\"> Name>>.";
			expString = "<document name=\"&amp;&lt;Doc&apos;s &quot;Bad&quot; Name&gt;\">" +
						"Here is text with a <field name=\"&amp;field&apos;s &lt;&quot;Bad&quot;&gt; Name\"/>." +
						"</document>\r\n";
			Assert.AreEqual(expString, doc.ToXml());
		} 
#endif

		[Test]
		public void Fields() 
		{ 
			Document doc = new Document("Document Name");

			string htmlContent =
				Document.RawHtmlPrefix +
				"<p><span style=\"font-size:10pt;\">Here is a field &lt;&lt;Q1:a&gt;&gt; and so is this &lt;&lt;Q2:a&gt;&gt;.</span></p>" +
				Document.RawHtmlPostfix;

			doc.Text = htmlContent;

			FieldList docFields = doc.GetFields();

			Assert.AreEqual(2, docFields.Count);
			Assert.AreEqual("Q1:a", docFields[0].FieldName);
			Assert.AreEqual("Q2:a", docFields[1].FieldName);
		}


		[Test]
		public void TestToString()
		{
			Document doc = new Document("MyDoc");

			//Assertions
			Assert.AreEqual("MyDoc", doc.ToString());
			Assert.AreEqual(doc.Name, doc.ToString());
		}
	}
}
