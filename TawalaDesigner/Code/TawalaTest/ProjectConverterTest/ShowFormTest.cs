using System;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Processes;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion Show Form statements.
	/// </summary>
	[TestFixture]
	public class ShowFormTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("ShowFormStatements.xml");
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
			Form form = (Form)Project.Current.FormList[0];
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that project contains 2 forms
			Assert.AreEqual(2, Project.Current.FormList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Form 2", ((Form)Project.Current.FormList[1]).Name);

			// verify that project contains 1 process
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual("Process 1", process.Name);

			// verify that process contains 1 show form statement
			Assert.AreEqual("Show Form Form 1", process.Lines[0].ToString());

		}


	}
}
