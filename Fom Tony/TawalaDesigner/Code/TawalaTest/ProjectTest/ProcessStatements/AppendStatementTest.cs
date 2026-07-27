using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Components;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the AppendStatement class
	/// </summary>
	[TestFixture]
	public class AppendStatementTest
	{
		[Test]
		public void Name() 
		{ 
			Assert.AreEqual("Append", new AppendStatement().Name);
		}

		[Test]
		public void ConstructFromXml()
		{
			// create new test project
			Project.NewTestProject();

			// add documents to project
			IDocument document1 = Project.Current.AddDocument();
			IDocument document2 = Project.Current.AddDocument();

			string xmlString =
				"<append document=\"Document 1\" appendage=\"Document 2\"/>";

			IXmlElement element = new XmlElement(xmlString);
			AppendStatement statement = new AppendStatement(element, "No Process");

			Assert.AreEqual("Document 1", statement.Document.Name);
			Assert.AreEqual("Document 2", statement.Appendage.Name);
			Assert.AreSame(document1, statement.Document);
			Assert.AreSame(document2, statement.Appendage);
		}

		[Test]
		public void ConstructAppendVirtualFromXml()
		{
			// create new test project
			Project.NewTestProject();

			// add documents to project
			IDocument document1 = Project.Current.AddDocument();

			string xmlString =
				"<append document=\"Virtual Document\" appendage=\"Document 1\"/>";

			IXmlElement element = new XmlElement(xmlString);
			AppendStatement statement = new AppendStatement(element, "No Process");

			Assert.AreEqual("Virtual Document", statement.Document.Name);
			Assert.IsInstanceOfType(typeof(Document), statement.Document);
			Assert.AreEqual("Document 1", statement.Appendage.Name);
			Assert.AreSame(document1, statement.Appendage);
		}

		[Test]
		public void DocumentNames() 
		{ 
			AppendStatement statement = new AppendStatement(new Document("Appendage"), new Document("Document"));

			Assert.AreEqual("Appendage", statement.Appendage.Name);
			Assert.AreEqual("Document", statement.Document.Name);
		}

		[Test]
		public void GetText() 
		{ 
			// create new test project
			Project.NewTestProject();

			IDocument document1 = Project.Current.AddDocument();
			IDocument document2 = Project.Current.AddDocument();
			ProcessStatement statement = new AppendStatement(document1, document2);
		
			string stString = statement.ToString();
			string defDocName = Tawala.Projects.Properties.Resources.DocumentDefaultBaseName;
			Assert.AreEqual("Append " + defDocName + " 1 to " + defDocName + " 2" , stString);
		} 

		[Test]
		public void DocumentField() 
		{ 
			// create new test project
			Project.NewTestProject();

			// add document to project
			IDocument document1 = Project.Current.AddDocument();
			document1.Text = "Here is a field <<Q1:a>> within some document text.";
			IDocument document2 = Project.Current.AddDocument();

			// add a Form and a Process
			IForm form = Project.Current.AddForm();
			Process proc = Project.Current.AddProcess();
			proc.Name = "Connected Process";

			FibItem fibItem = new FibItem();
			form.ItemList.Add(fibItem);

			// connect the Process to the Form
			Project.Current.ConnectProcessToForm(proc, Tawala.Projects.Properties.Resources.FormDefaultBaseName + " 1");

			// Assertion
            Assert.AreEqual("Connected Process", form.ConnectedPostProcess.Name);
			
			// add show statement referencing documents
			AppendStatement statement = new AppendStatement(document1, document2);
			AppendLine line = new AppendLine(statement);
			proc.Lines.Add(line);

			string stString = statement.ToString();
			Assert.AreEqual("Append " + document1.Name + " to " + document2.Name, stString);
		} 

		[Test]
		public void GetXml() 
		{
			IDocument document = ComponentMaker.MakeDocumentObject("Target");
			IDocument appendage = ComponentMaker.MakeDocumentObject("Appendage");
			AppendStatement statement = new AppendStatement(appendage, document);
		
			string expString =	"<append document=\"Target\" appendage=\"Appendage\"/>";

			//Assertions 
			Assert.AreEqual(expString, statement.ToXml());

			// check for illegal XML characters
			document.Name = "&<Doc's \"Bad\" Name>";
			expString = "<append document=\"&amp;&lt;Doc&apos;s &quot;Bad&quot; Name&gt;\" appendage=\"Appendage\"/>";
			Assert.AreEqual(expString, statement.ToXml());
		} 
	}
}
