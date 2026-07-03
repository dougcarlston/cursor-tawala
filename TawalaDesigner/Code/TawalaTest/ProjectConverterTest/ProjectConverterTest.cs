using System;
using System.IO;
//using System.Xml;
using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.ProjectTest;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test public methods and properties of the ProjectConverter class.
	/// </summary>
	[TestFixture]
	public class ProjectConverterTest : TestBase
	{
		TawalaProjectConverter converter;

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

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("TestFile.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void NameProject()
		{
			Project.Current.Name = converter.GetAttributeValue("project", "name");

			Assert.AreEqual("MyProject", Project.Current.Name);
		}


		private Collection<string> getFormNames()
		{
			return (converter.GetFormNames());
		}


		/// <summary>
		/// Gets form names from the XML file.
		/// </summary>
		[Test]
		public void GetFormNames()
		{
			Collection<string> formNames = getFormNames();

			Assert.AreEqual("Form 1", formNames[0]);
			Assert.AreEqual("Form 2", formNames[1]);
		}


		//private FormItemList setFormItems(string formName)
		//{
		//    converter.GetFormsString();
		//    return (converter.SetFormItems(formName));
		//}


		/// <summary>
		/// Gets form items from the XML file.
		/// </summary>
		[Test]
		public void GetFormItems()
		{
			XmlElement formsElement = new XmlElement(converter.GetFormsString());
			FormList formList = new FormList(formsElement);

			foreach (Form form in formList)
			{
				converter.SetFormItems(form);
			}

			FormItemList formItems = Project.Current.FormList[0].ItemList;

			Assert.AreEqual(4, formItems.Count);

			Assert.IsTrue(formItems[0] is TextItem, "formItems[0] should be TextItem");

			Assert.IsTrue(formItems[1] is FibItem, "formItems[1] should be FibItem");

			Assert.IsTrue(formItems[2] is FibItem, "formItems[2] should be FibItem");

			Assert.IsTrue(formItems[3] is McqItem, "formItems[2] should be McqItem");

			McqItem mcItem = (McqItem)formItems[3];

			Assert.AreEqual(3, mcItem.Choices.Count);
			Assert.AreEqual("Choice 1", mcItem.Choices[0].Text);
		}


		private Collection<string> getDocumentNames()
		{
			return (converter.GetDocumentNames());
		}


		/// <summary>
		/// Gets document names from the XML file.
		/// </summary>
		[Test]
		public void GetDocumentNames()
		{
			Collection<string> documentNames = getDocumentNames();

			Assert.AreEqual("Document 1", documentNames[0]);
			Assert.AreEqual("Document 2", documentNames[1]);
			Assert.AreEqual("Document 3", documentNames[2]);
		}


		private RtfDocument getDocument(string documentName)
		{
			return (converter.GetDocument(documentName));
		}


		/// <summary>
		/// Gets documents from the XML file.
		/// </summary>
		[Test]
		public void GetDocuments()
		{
			Collection<string> documentNames = getDocumentNames();

			for (int i = 0; i < documentNames.Count; i++)
			{
				RtfDocument document = getDocument(documentNames[i]);

				string expectedRtf =
					rtfStringPrefix +
					@"{\f0\fs20\cf1 This is Document " + (i + 1) + @". It has a field: <<Q1:a>>}\par " +
					rtfStringPostfix;

				Assert.AreEqual(expectedRtf, document.Rtf);
			}

			Assert.AreEqual(3, documentNames.Count);
			Assert.AreEqual("Document 1", documentNames[0]);
			Assert.AreEqual("Document 2", documentNames[1]);
			Assert.AreEqual("Document 3", documentNames[2]);
		}


		/// <summary>
		/// Event handler for Project.Open
		/// </summary>
		private void project_OpenProject(object sender, ProjectEventArgs e)
		{
			Assert.AreEqual(e.ProjectName, "MyProject");
		}


		private Collection<string> getProcessNames()
		{
			return (converter.GetProcessNames());
		}


		/// <summary>
		/// Gets process names from the XML file.
		/// </summary>
		[Test]
		public void GetProcessNames()
		{
			Collection<string> processNames = getProcessNames();

			Assert.AreEqual("Process 1", processNames[0]);
		}


		/// <summary>
		/// Gets process lines from the XML file.
		/// </summary>
		[Ignore("Compatibility partially broken by recent FieldList optimization - SB (01/14/2008)")]
		[Test]
		public void GetProcessLines()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that form and process are connected
            Assert.AreEqual(process, ((Form)Project.Current.FormList[0]).ConnectedPostProcess);

			Assert.AreEqual("Show Document Document 1", process.Lines[0].ToString());
			Assert.AreEqual("If Form 1:Q1:a is not blank", process.Lines[1].ToString());
			Assert.AreEqual("(", process.Lines[2].ToString());
			Assert.AreEqual("Show Document Document 2", process.Lines[3].ToString());
			Assert.AreEqual(")", process.Lines[4].ToString());
			Assert.AreEqual("Otherwise", process.Lines[5].ToString());
			Assert.AreEqual("(", process.Lines[6].ToString());
			Assert.AreEqual("Show Document Document 3", process.Lines[7].ToString());
			Assert.AreEqual(")", process.Lines[8].ToString());

			Assert.AreEqual("If Form 1:Q1:a equals Form 1:Q2:a", process.Lines[9].ToString());
			Assert.AreEqual("(", process.Lines[10].ToString());
			Assert.AreEqual("Show Document Document 2", process.Lines[11].ToString());
			Assert.AreEqual(")", process.Lines[12].ToString());

			Assert.AreEqual("If Form 1:Q1:a equals \"Bozo\"", process.Lines[13].ToString());
			Assert.AreEqual("(", process.Lines[14].ToString());
			Assert.AreEqual("Show Document Document 2", process.Lines[15].ToString());
			Assert.AreEqual(")", process.Lines[16].ToString());

			Assert.AreEqual(17, process.Lines.Count);
		}

		[Test]
		public void GetFormsString()
		{
			string xmlString = converter.GetFormsString();

			Assert.IsTrue(xmlString.StartsWith("<forms>"));
			Assert.IsTrue(xmlString.EndsWith("</forms>"));
		}

		[Test]
		public void GetDocumentsString()
		{
			string xmlString = converter.GetDocumentsString();

			Assert.IsTrue(xmlString.StartsWith("<documents>"));
			Assert.IsTrue(xmlString.EndsWith("</documents>"));
		}

		[Test]
		public void GetProjectString()
		{
			string xmlString = converter.GetProjectString();

			Assert.IsTrue(xmlString.StartsWith("<project "));
			Assert.IsTrue(xmlString.EndsWith("</project>"));
		}
	}

}
