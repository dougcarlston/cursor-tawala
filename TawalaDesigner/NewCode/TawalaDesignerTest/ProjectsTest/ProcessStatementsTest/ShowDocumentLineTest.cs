using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
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
			TestingSupport.Util.NewTestProject();

			form = Project.Current.AddForm();

			form.ItemList.Add(new NewFibItem());

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

		private void createStatement(IDocument doc)
		{
			statement = new ShowDocumentStatement(doc);
			line = new ShowDocumentLine(statement);
			process.Lines.Add(line);
		}

		private IDocument createDocumentWithoutField()
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

		private IDocument createDocumentWithField()
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

		private IDocument createDocumentWithVariable()
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

		private IDocument addDocument(string xml)
		{
			IXmlElement element = new XmlElement(xml);
			IDocument document = new NewDocument(element);
			Project.Current.AddDocument(document);
			return document;
		}
	}
}
