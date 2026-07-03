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
	/// Test class for the ShowDocumentStatement class
	/// </summary>
	[TestFixture]
	public class ShowDocumentStatementTest
	{
		[Test]
		public void Construct() 
		{
			ShowDocumentStatement statement = new ShowDocumentStatement();

			Assert.IsNull(statement.Document);
			Assert.IsNull(statement.Form);
			Assert.IsFalse(statement.ResetAfterShow);
		}

		[Test]
		public void ConstructWithDocument() 
		{
			ShowDocumentStatement statement = new ShowDocumentStatement(ComponentMaker.MakeDocumentObject("MyDoc"));
			Assert.IsNotNull(statement.Document);
			Assert.AreEqual("MyDoc", statement.Document.Name);
			Assert.AreEqual("Show Document MyDoc", statement.ToString());
			Assert.IsNull(statement.Form);
			Assert.IsFalse(statement.ResetAfterShow);
		}

		[Test]
		public void ConstructFromXml()
		{
			Project.NewTestProject();

			IDocument document1 = Project.Current.AddDocument();

			string xmlString =
				"<show document=\"Document 1\"/>";

			IXmlElement element = new XmlElement(xmlString);
			ShowDocumentStatement statement = new ShowDocumentStatement(element, "No Process");

			Assert.AreSame(document1, statement.Document);
			Assert.IsFalse(statement.ResetAfterShow);
		}
		
		[Test]
		public void ConstructFromXmlWithReset()
		{
			Project.NewTestProject();

			IDocument document1 = Project.Current.AddDocument();

			string xmlString =
				"<show document=\"Document 1\" reset=\"true\"/>";

			IXmlElement element = new XmlElement(xmlString);
			ShowDocumentStatement statement = new ShowDocumentStatement(element, "No Process");

			Assert.AreSame(document1, statement.Document);
			Assert.IsTrue(statement.ResetAfterShow);
		}

		[Test]
		public void Name() 
		{
			ShowDocumentStatement statement = new ShowDocumentStatement();
			Assert.AreEqual("Show", statement.Name);
		} 


		[Test]
		public void TestWithField() 
		{ 
			Project.NewTestProject();

			IDocument document1 = Project.Current.AddDocument();
			document1.Text = "Here is a field <<Q1:a>> within some document text.";

			IForm form = Project.Current.AddForm();
			Process proc = Project.Current.AddProcess();
			proc.Name = "Connected Process";

			FibItem fibItem = new FibItem();
			form.ItemList.Add(fibItem);

			Project.Current.ConnectProcessToForm(proc, Tawala.Projects.Properties.Resources.FormDefaultBaseName + " 1");

			Assert.AreEqual("Connected Process", form.ConnectedPostProcess.Name);
			
			ShowDocumentStatement statement = new ShowDocumentStatement(document1);
			ShowLine line = new ShowDocumentLine(statement);
			proc.Lines.Add(line);

			proc.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
		} 

		[Test]
		public void GetXml() 
		{
			Project.NewTestProject();

			//Document document = new Document("Document 1");
			IDocument document = Project.Current.AddDocument();
			ShowDocumentStatement statement = new ShowDocumentStatement(document);
		
			string expString =	"<show document=\"Document 1\" reset=\"false\"/>";

			Assert.AreEqual(expString, statement.ToXml());

			// check for illegal XML characters
			document.Name = "&<Doc's \"Bad\" Name>";
			expString = "<show document=\"&amp;&lt;Doc&apos;s &quot;Bad&quot; Name&gt;\" reset=\"false\"/>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void GetXmlWithReset()
		{
			Project.NewTestProject();

			//Document document = new Document("Document 1");
			IDocument document = Project.Current.AddDocument();
			ShowDocumentStatement statement = new ShowDocumentStatement(document);
			statement.ResetAfterShow = true;

			string expString = "<show document=\"Document 1\" reset=\"true\"/>";

			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void Copy()
		{
			ShowDocumentStatement statement1 = new ShowDocumentStatement(ComponentMaker.MakeDocumentObject("Document 1"));

			ShowDocumentStatement statement2 = (ShowDocumentStatement)statement1.Copy();

			Assert.AreNotSame(statement1, statement2);
			Assert.AreSame(statement1.Document, statement2.Document);
		}

		[Test]
		public void GetString()
		{
			IDocument document = ComponentMaker.MakeDocumentObject("Document 1");
			ShowDocumentStatement statement = new ShowDocumentStatement(document);
			statement.ResetAfterShow = false;

			string expString = "Show Document Document 1";

			Assert.AreEqual(expString, statement.ToString());
		}

		[Test]
		public void GetStringWithReset()
		{
			IDocument document = ComponentMaker.MakeDocumentObject("Document 1");
			ShowDocumentStatement statement = new ShowDocumentStatement(document);
			statement.ResetAfterShow = true;

			string expString = "Show and reset Document Document 1";

			Assert.AreEqual(expString, statement.ToString());
		}
	}
}
