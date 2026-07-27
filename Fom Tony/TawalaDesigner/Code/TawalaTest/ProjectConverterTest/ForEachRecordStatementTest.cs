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
	/// Class to test conversion of FOR EACH RECORD statements.
	/// </summary>
	[TestFixture]
	public class ForEachRecordStatementTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void SetUp()
		{
			converter = GetConverter("ForEachRecordStatements.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void ConvertXmlToProject()
		{
			// verify that project contains 1 form and 1 process
			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Process 1", ((Process)Project.Current.ProcessList[0]).Name);
		}

		[Test]
		public void BasicForEach()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that record set is available via process
			Assert.AreEqual("Record Set 1", process.RecordSets[0].FieldName);

			// verify that process lines are correct
			Assert.AreEqual("For Each Record in Record Set 1", process.Lines[1].ToString());

			// verify that record is available via process
			Assert.AreEqual("Record", process.Records[0].FieldName);
		}


		[Test]
		public void SetLiteral()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that process lines are correct
			Assert.AreEqual("Set Variable 1 to 1", process.Lines[3].ToString());
		}

		[Test]
		public void SetFibDefault()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that process lines are correct
			Assert.AreEqual("Set Variable 1 to Form 1:Q1:a", process.Lines[4].ToString());
		}

		[Test]
		public void StatementsWithQualifiedFibDefault()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			ArrayList expectedLines = new ArrayList();
			expectedLines.Add("Set Variable 1 to Record:Form 1:Q1:a");
			expectedLines.Add("Add Record:Form 1:Q1:a to Variable 1");
			expectedLines.Add("Subtract Record:Form 1:Q1:a from Variable 1");
			expectedLines.Add("Multiply Variable 1 by Record:Form 1:Q1:a");
			expectedLines.Add("Divide Variable 1 by Record:Variable 1");
			expectedLines.Add("If Record:Form 1:Q1:a equals Record:Form 1:Q2:a");
			expectedLines.Add("(");
			expectedLines.Add("Send Document 1 to Record:Form 1:Q1:a");
			expectedLines.Add(")");
			expectedLines.Add("If Record:Form 1:Q1:a begins with Record:Form 1:Q2:a OR Record:Form 1:Q1:a ends with Record:Form 1:Q2:a");
			expectedLines.Add("(");
			expectedLines.Add("Send Email to Record:Form 1:Q1:a");
			expectedLines.Add(")");

			for (int expIndex = 0, procIndex = 5; expIndex < expectedLines.Count; expIndex++)
			{
				Assert.AreEqual(expectedLines[expIndex], process.Lines[procIndex++].ToString(), "QualifiedFibDefault: expectedLines[{0}] failed.", expIndex);
			}
		}

	}
}
