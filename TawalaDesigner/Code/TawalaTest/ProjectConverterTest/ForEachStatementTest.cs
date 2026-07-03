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
	/// Class to test conversion of FOr EACH statements.
	/// </summary>
	[TestFixture]
	public class ForEachStatementTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void SetUp()
		{
			converter = GetConverter("ForEachStatements.xml");
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
			// verify that project contains 1 process and 2 forms
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual("Process 1", ((Process)Project.Current.ProcessList[0]).Name);
			Assert.AreEqual(2, Project.Current.FormList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Form 2", ((Form)Project.Current.FormList[1]).Name);
		}

		[Test]
		public void CheckProcess()
		{
			Console.WriteLine("ForEachQuestionStatementTest.CheckProcess:");

			Process process = (Process)Project.Current.ProcessList[0];

			int i = 0;
			Assert.AreEqual("Get Record Set 1 from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("Get Record Set 2 from Form 2", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record 1 in Record Set 1", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record 2 in Record Set 2", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("Add Record 1:Form 1:Q1:a to Score", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}

	}
}
