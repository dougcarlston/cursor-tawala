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
	/// Class to test conversion Show Document statements.
	/// </summary>
	[TestFixture]
	public class ShowDocumentTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("ShowDocumentStatements.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void DocumentReferences()
		{
			Document doc = (Document)Project.Current.DocumentList[0];

			Process process = (Process)Project.Current.ProcessList[0];

			// verify that identical statements reference same document
			Assert.AreEqual(doc, ((ShowStatement)process.Lines[0].Statement).Document);
			Assert.AreEqual(doc, ((ShowStatement)process.Lines[1].Statement).Document);
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

			// verify that process contains 3 show document statement
			Assert.AreEqual("Show Document Document 1", process.Lines[0].ToString());
			Assert.AreEqual("Show Document Document 1", process.Lines[1].ToString());
			Assert.AreEqual("Show Document Document 2", process.Lines[2].ToString());
		}
	}
}
