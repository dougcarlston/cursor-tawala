using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class ShowStatementMissingDocument390
    {
		private Process process;

		[SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();

			process = Project.Current.AddProcess();
		}

		[Test]
		public void RemovingDocumentProducesNullDocumentInShowStatement()
		{
			IDocument document = Project.Current.AddDocument();

			ShowDocumentStatement statement = new ShowDocumentStatement(document);
			ShowDocumentLine line = new ShowDocumentLine(statement);
			process.Lines.Add(line);

			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
			Assert.AreSame(document, statement.Document);
			Assert.AreEqual("Show Document Document 1", line.ToString());

			Project.Current.RemoveDocument(document);

			Assert.AreSame(Document.NULL, statement.Document);
			Assert.AreEqual("Show Document Null Document", line.ToString());
		}

        [Test]
        public void NullDocumentXmlProducesNullDocumentXml()
        {
			string xmlString =
				"<show document=\"Null Document\" reset=\"false\"/>";

			IXmlElement element = new XmlElement(xmlString);
			ShowDocumentStatement statement = new ShowDocumentStatement(element, process);

			Assert.AreEqual(xmlString, statement.ToXml());
        }
	}
}
