using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Class to test Tawala.Common.Document class.
	/// </summary>
	[TestFixture]
	public class DocumentTest
	{
		[Test]
		public void NewDocument() 
		{ 
			IDocument doc = new NewDocument("Test");

			//Assertions 
			Assert.IsNotNull(doc);
			Assert.AreEqual("Test", doc.Name);
		}

		[Test]
		public void NameDocument() 
		{ 
			IDocument doc = new NewDocument("Renamed Document");

			//Assertions 
			Assert.AreEqual("Renamed Document", doc.Name);
		} 

		[Test]
		public void NameDocumentViaConstructor() 
		{ 
			IDocument doc = new NewDocument("MyDocument");

			//Assertions 
			Assert.AreEqual("MyDocument", doc.Name);
		}

		[Test]
		public void TrimWhitespaceFromName()
		{
			IDocument doc = new NewDocument("    Document 1  ");
			Assert.AreEqual("Document 1", doc.Name);

			doc.Name = "   Renamed Document   ";
			Assert.AreEqual("Renamed Document", doc.Name);
		}

		[Test]
		public void TestToString()
		{
			IDocument doc = new NewDocument("MyDoc");

			//Assertions
			Assert.AreEqual("MyDoc", doc.ToString());
			Assert.AreEqual(doc.Name, doc.ToString());
		}
	}
}
