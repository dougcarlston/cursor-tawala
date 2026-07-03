using Tawala.Projects;
using Tawala.Projects.Documents;
using NUnit.Framework;
using Tawala.Projects.Processes;

namespace TawalaTest.BugTest
{
    [TestFixture]
	public class CopyProcessWithNullDocument662 : ClipboardTester<Process>
    {
		[Test]
		public void CopyingProcessWithNullReferenceDoesNotAddDocument()
		{
			TestSupport.Util.NewTestProject();
			Process process = Project.Current.AddProcess();
			
			IDocument document = Project.Current.AddDocument();
			Assert.AreEqual(1, Project.Current.DocumentList.Count);
			
			ShowDocumentStatement showStatement = new ShowDocumentStatement(document);
			process.Lines.Add(new ProcessLineList(showStatement));

			Project.Current.RemoveDocument(document);
			Assert.AreEqual(0, Project.Current.DocumentList.Count);

			Process clipboardCopy = CopyPaste();
			Assert.IsNotNull(clipboardCopy, ErrorMessage);

			Assert.AreEqual(0, Project.Current.DocumentList.Count);
		}

		protected override Process GetComponent()
		{
			return Project.Current.ProcessList[0] as Process;
		}
    }
}
