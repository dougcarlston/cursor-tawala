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
	[TestFixture]
	public class DeleteStatementTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("DeleteStatements.xml");
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
			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Process 1", ((Process)Project.Current.ProcessList[0]).Name);

		}


		/// <summary>
		/// Tests the conversion of Delete statements containing a single Form.
		/// </summary>
		[Test]
		public void SimpleDelete()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that process lines are correct
			Assert.AreEqual("Delete records from Form 1", process.Lines[0].ToString());
		}
	}
}
