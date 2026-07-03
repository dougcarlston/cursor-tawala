using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class AppendLineTest
	{
		private IForm form;
		private Process process;
		private AppendStatement statement;
		private ProcessLine line;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();
			form.ItemList.Add(new FibItem());

			process = Project.Current.AddProcess();

			statement = new AppendStatement();
			line = new AppendLine(statement);
			process.Lines.Add(line);
		}


		[Test]
		public void Validate()
		{
			Document documentWithoutField = createDocumentWithoutField();

			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			statement.Document = documentWithoutField;
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);

			statement.Appendage = documentWithoutField;
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}

		[Test]
		public void ValidateWithField()
		{
			Project.Current.ConnectProcessToForm(process, form.Name);

			statement.Document = createDocumentWithoutField();
			statement.Appendage = createDocumentWithField();

			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);

			form.ItemList.Clear();

			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);
		}

		[Test]
		public void ValidateAfterRemoveDocument()
		{
			Document documentWithoutField = createDocumentWithoutField();
			statement.Document = documentWithoutField;
			statement.Appendage = documentWithoutField;
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);

			Project.Current.RemoveDocument(documentWithoutField);
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);
		}

		[Test]
		public void ValidateWithVariable()
		{
			statement.Document = createDocumentWithoutField();
			statement.Appendage = createDocumentWithVariable();

			// variable references should always be valid
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);

			Project.Current.ConnectProcessToForm(process, form.Name);
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}

		private Document createDocumentWithoutField()
		{
			string xmlNoField =
				"<document name=\"Document 1\">" + Environment.NewLine +
				"<xmlData>" + Environment.NewLine +
				"<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font>Document without " +
				"field</font></paragraph>" + Environment.NewLine +
				"</xmlData>" + Environment.NewLine +
				"</document>";

			return addDocument(xmlNoField);
		}

		private Document createDocumentWithField()
		{
			string xmlWithField =
				"<document name=\"Document 2\">" + Environment.NewLine +
				"<xmlData>" + Environment.NewLine +
				"<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font>Document with " +
				"field:</font><font><field name=\"Form 1:Q1:a\"/></font><sp/></paragraph>" + Environment.NewLine +
				"</xmlData>" + Environment.NewLine +
				"</document>";

			return addDocument(xmlWithField);
		}

		private Document createDocumentWithVariable()
		{
			string xmlWithVariable =
				"<document name=\"Document 3\">" + Environment.NewLine +
				"<xmlData>" + Environment.NewLine +
				"<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font>Document with " +
				"variable:</font><font><field name=\"Var\"/></font><sp/></paragraph>" + Environment.NewLine +
				"</xmlData>" + Environment.NewLine +
				"</document>";

			return addDocument(xmlWithVariable);
		}

		private Document addDocument(string xml)
		{
			IXmlElement element = new XmlElement(xml);
			RtfDocument document = new RtfDocument(element);
			Project.Current.AddDocument(document);
			return document;
		}

	}
}
