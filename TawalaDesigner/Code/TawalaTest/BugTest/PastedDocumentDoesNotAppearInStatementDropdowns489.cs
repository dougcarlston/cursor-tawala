using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
    [TestFixture]
	public class PastedDocumentDoesNotAppearInStatementDropdowns489
    {
		[SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();
		}

		[Test]
		public void RealOrVirtualDocumentListUpdatesCorrectly()
		{
			IDocument document1 = Project.Current.AddDocument();

			Assert.AreEqual(1, Project.Current.DocumentList.Count);
			Assert.AreEqual(1, Project.Current.RealOrVirtualDocumentList.Count);

			IDocument document2 = new RtfDocument("Document 2");
			Project.Current.PasteDocument(document2);

			Assert.AreEqual(2, Project.Current.DocumentList.Count);
			Assert.AreEqual(2, Project.Current.RealOrVirtualDocumentList.Count);
		}
	}
}
