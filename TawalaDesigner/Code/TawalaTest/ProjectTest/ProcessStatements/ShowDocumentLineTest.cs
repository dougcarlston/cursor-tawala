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
	public class ShowDocumentLineTest
	{
		private IForm form;
		private Process process;
		private ShowDocumentStatement statement;
		private ProcessLine line;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();

			form.ItemList.Add(new FibItem());

			process = Project.Current.AddProcess();
		}

		[Test]
		public void Validate()
		{
			createStatement(createDocumentWithoutField());
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}

		[Test]
		public void ValidateWithField()
		{
			Project.Current.ConnectProcessToForm(process, form.Name);

			createStatement(createDocumentWithField());

			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);

			form.ItemList.Clear();

			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);
		}

		[Test]
		public void ValidateAfterRemoveDocument()
		{
			createStatement(createDocumentWithoutField());
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);

			Project.Current.RemoveDocument(statement.Document);
			process.Lines.ValidateLines();
			Assert.AreEqual(false, line.IsValid);
		}

		[Test]
		public void ValidateWithVariable()
		{
			createStatement(createDocumentWithVariable());

			// variable references should always be valid
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);

			Project.Current.ConnectProcessToForm(process, form.Name);
			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		}

		private void createStatement(Document doc)
		{
			statement = new ShowDocumentStatement(doc);
			line = new ShowDocumentLine(statement);
			process.Lines.Add(line);
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
