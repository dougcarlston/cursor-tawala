using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for ProcessList class.
	/// </summary>
	[TestFixture]
	public class ProcessListTest
	{
		[SetUp]
		public void SetUp()
		{
			TestingSupport.Util.NewTestProject();
		}

		[Test]
		public void AddProcess() 
		{ 
			Process process = new Process("Test");

			ProcessList list = new ProcessList();
			list.Add(process);

			//Assertions 
			Assert.IsNotNull(process);
			Assert.AreEqual(1, list.Count);
		} 

		[Test]
		public void RemoveProcess() 
		{ 
			Process test;

			Process process1 = new Process("Process 1");
			Process process2 = new Process("Process 2");

			// add processes to list
			ProcessList list = new ProcessList();
			list.Add(process2);
			list.Add(process1);

			// assert that process 1 is in list
			test = list["Process 1"];
			Assert.IsNotNull(test);
			Assert.AreEqual("Process 1", test.Name);

			// assert that process 2 is in list
			test = list[process2.Name];
			Assert.IsNotNull(test);
			Assert.AreEqual("Process 2", test.Name);

			// remove process 1
			list.Remove(process1);

			// assert that process 1 is not in list
			test = list["Process 1"];
			Assert.IsNull(test);
		} 

		[Test]
		public void GetProcessFromStatement() 
		{ 
			// create processes
			Process process1 = new Process("Test1");
			Process process2 = new Process("Test2");

			// add processes to list
			ProcessList list = new ProcessList();
			list.Add(process1);
			list.Add(process2);

			// add show statement lines to processes
			IDocument document1 = new NewDocument("Document 1");
			ShowDocumentStatement statement1 = new ShowDocumentStatement(document1);
			ProcessLine line1 = new ShowDocumentLine(statement1);

			IDocument document2 = new NewDocument("Document 2");
			ShowDocumentStatement statement2 = new ShowDocumentStatement(document2);
			ProcessLine line2 = new ShowDocumentLine(statement2);

			process1.Lines.Add(line1);
			process2.Lines.Add(line2);

			//Assertions 
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(process1, list[statement1]);
			Assert.AreEqual(process2, list[statement2]);
		} 

		[Test]
		public void GetXml() 
		{ 
			Process process1 = new Process("Process 1");

			ProcessList processList = new ProcessList();
			processList.Add(process1);

			// add show line to process
			//Document document1 = new Document("Document 1");
			IDocument document1 = Project.Current.AddDocument();
			ShowDocumentStatement st1 = new ShowDocumentStatement(document1);
			ProcessLine line1 = new ShowDocumentLine(st1);
			process1.Lines.Add(line1);

			string expString =	"<processes>\r\n" +
								"<process name=\"Process 1\">\r\n" +
								"<show document=\"Document 1\" reset=\"false\"/>\r\n" +
								"</process>\r\n" +
								"</processes>\r\n";

			//Assertions 
			Assert.AreEqual(expString, processList.ToXml());
		}

		[Test]
		public void GetXmlWhenEmpty() 
		{ 
			// make list with no processes
			ProcessList processList = new ProcessList();

			//Assertions 
			Assert.AreEqual("", processList.ToXml());
		}

		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
				"<processes>" +
				"<process name=\"Process 1\">" +
				"<comment>" +
				"This is Process 1" +
				"</comment>" +
				"</process>" +
				"<process name=\"Process 2\">" +
				"<comment>" +
				"This is Process 2" +
				"</comment>" +
				"</process>" +
				"</processes>";

			IXmlElement element = new XmlElement(xmlString);
			ProcessList processList = new ProcessList(element);

			Assert.AreEqual(2, processList.Count);
			Assert.IsInstanceOfType(typeof(CommentStatement), processList[0].Lines[0].Statement);
			Assert.IsInstanceOfType(typeof(CommentStatement), processList[1].Lines[0].Statement);
		}
	}
}
