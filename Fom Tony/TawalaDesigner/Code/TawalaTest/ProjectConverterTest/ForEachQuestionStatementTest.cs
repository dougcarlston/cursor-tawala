using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
#if false
	/// <summary>
	/// Class to test conversion of FOr EACH statements.
	/// </summary>
	[TestFixture]
	public class ForEachQuestionStatementTest
	{
		TawalaProjectConverter converter;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			converter = new TawalaProjectConverter(TawalaTest.TestSupport.Util.GetTestFilePath("ForEachQuestionStatements.xml"));
		}


		[SetUp]
		public void Setup()
		{
			File.Delete(TawalaTest.TestSupport.Util.GetTestFilePath("ForEachQuestionStatements.tawala"));

			converter.ConvertXmlToProjectFile();

			// verify that project file was created
			Assert.IsTrue(File.Exists(TawalaTest.TestSupport.Util.GetTestFilePath("ForEachQuestionStatements.tawala")));
		}


		/// <summary>
		/// Tests the conversion of the XML file to a Tawala Project.
		/// </summary>
		[Test]
		public void ConvertXmlToProject()
		{
			// verify that project contains 1 process and 1 form
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual("Process 1", ((Process)Project.Current.ProcessList[0]).Name);
			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
		}

		[Test]
		public void CheckProcess()
		{
			Console.WriteLine("ForEachQuestionStatementTest.CheckProcess:");

			Process process = (Process)Project.Current.ProcessList[0];

			int i = 0;
			Assert.AreEqual("Get Record Set from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record in Record Set", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Multiple Choice Question", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If (selection) equals Record:(selection)", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}

		[Test]
		public void GetProcessXml()
		{
			string expectedString =
				"<process name=\"Process 1\">\r\n" +
				"<get recordList=\"Record Set\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>\r\n" +
				"<foreach record=\"Record\" recordList=\"Record Set\">\r\n" +
				"<forEachMc>\r\n" +
				"<if>\r\n" +
				"<conditions>\r\n" +
				"<mcEquals field=\"(selection)\">\r\n" +
				"<string field=\"Record:(selection)\"/>\r\n" +
				"</mcEquals>\r\n" +
				"</conditions>\r\n" +
				"<trueSet>\r\n" +
				"</trueSet>\r\n" +
				"</if>\r\n" +
				"</forEachMc>\r\n" +
				"</foreach>\r\n" +
				"</process>\r\n";

			Assert.AreEqual(expectedString, Project.Current.GetProcess("Process 1").ToXml());
		}

	}
#endif
}
