using System;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Processes;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion Append statements.
	/// </summary>
	[TestFixture]
	public class AppendStatementTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("AppendStatements.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		/// <summary>
		/// Tests the conversion of the XML file to a Tawala Project.
		/// </summary>
		[Test]
		public void ConvertXmlToProject()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that project contains 1 process
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual("Process 1", process.Name);

			// get references to the documents in the "real" document list
			Document doc1 = (Document)Project.Current.DocumentList[0];
			Document doc2 = (Document)Project.Current.DocumentList[1];
			
			// verify that process contains an Append "real document" statement
			Assert.AreEqual("Append Document 2 to Document 1", process.Lines[0].ToString());

			// verify that identical statements reference same document
			Assert.AreEqual(doc1, ((AppendStatement)process.Lines[0].Statement).Document);
			Assert.AreEqual(doc2, ((AppendStatement)process.Lines[0].Statement).Appendage);

			// verify that process contains an Append "virtual document" statement
			Assert.AreEqual("Append Document 2 to VirtualDoc 1", process.Lines[1].ToString());

			Assert.AreEqual(2, process.Lines.Count);
		}
	}
}
