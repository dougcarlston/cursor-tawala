using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Processes;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion of IF statements inside FOR EACH RECORD statements.
	/// </summary>
	[TestFixture]
	public class ForEachRecordIfTest : TestBase
	{

		TawalaProjectConverter converter;

		[SetUp]
		public void SetUp()
		{
			converter = GetConverter("ForEachRecordIfStatements.xml");
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
			// verify that project contains 1 form and 1 process
			Assert.AreEqual(2, Project.Current.FormList.Count);
			Assert.AreEqual(2, Project.Current.ProcessList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Form 2", ((Form)Project.Current.FormList[1]).Name);
			Assert.AreEqual("Process 1", ((Process)Project.Current.ProcessList[0]).Name);
			Assert.AreEqual("Process 2", ((Process)Project.Current.ProcessList[1]).Name);
		}


		[Test]
		public void CheckProcess1()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			ArrayList expectedLines = new ArrayList();
			expectedLines.Add("Get Record Set from Form 2");
			expectedLines.Add("For Each Record in Record Set");
			expectedLines.Add("(");
			expectedLines.Add("If Record:Variable 1 is not blank");
			expectedLines.Add("(");
			expectedLines.Add("Set Variable 1 to \"X\"");
			expectedLines.Add(")");
			expectedLines.Add(")");

			for (int expIndex = 0, procIndex = 0; expIndex < expectedLines.Count; expIndex++)
			{
				Assert.AreEqual(expectedLines[expIndex], process.Lines[procIndex++].ToString(), "CheckProcess2: expectedLines[{0}] failed.", expIndex);
			}
		}

		[Test]
		public void CheckProcess2()
		{
			Process process = (Process)Project.Current.ProcessList[1];

			ArrayList expectedLines = new ArrayList();
			expectedLines.Add("If Variable 1 is not blank");
			expectedLines.Add("(");
			expectedLines.Add("Set Variable 1 to \"X\"");
			expectedLines.Add(")");

			for (int expIndex = 0, procIndex = 0; expIndex < expectedLines.Count; expIndex++)
			{
				Assert.AreEqual(expectedLines[expIndex], process.Lines[procIndex++].ToString(), "CheckProcess2: expectedLines[{0}] failed.", expIndex);
			}
		}

	}
}
