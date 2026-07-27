using System;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Processes;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion of GET statements.
	/// </summary>
	[TestFixture]
	public class ShowRecordStatementTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("ShowRecordStatement.xml");
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


		/// <summary>
		/// Tests the conversion of Delete statements containing a single Form.
		/// </summary>
		[Test]
		public void ShowRecordWithConditions()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that process lines are correct
			ShowRecordLine line = process.Lines[0] as ShowRecordLine;
			ShowRecordStatement statement = line.Statement as ShowRecordStatement;
			Assert.AreEqual("Form 1", statement.Form.Name);
			Assert.AreSame(Project.Current.FormList[0], statement.Form);
			Assert.IsTrue(statement.ModifyOnSubmit);
			Assert.AreEqual("Show stored record from Form 1 where Record:Form 1:Q1:a equals \"foo\"", process.Lines[0].ToString());
		}
	}
}
