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
	public class GetStatementTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("GetStatements.xml");
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
		/// Tests the conversion of GET statements containing a single Form.
		/// </summary>
		[Test]
		public void SimpleGet()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that process lines are correct
			Assert.AreEqual("Get Record List 1 from Form 1", process.Lines[0].ToString());

			// verify that record set is available via process
			Assert.AreEqual("Record List 1", process.RecordSets[0].FieldName);
		}



	}
}
