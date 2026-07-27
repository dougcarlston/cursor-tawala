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
	/// Class to test Form-Process connections in the XML converter.
	/// </summary>
	[TestFixture]
	public class FormProcessConnectionTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void SetUp()
		{
			converter = GetConverter("FormProcessConnection.xml");
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
			Assert.AreEqual(3, Project.Current.FormList.Count);
			Assert.AreEqual("Form - No connection", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Form - Connected", ((Form)Project.Current.FormList[1]).Name);
			Assert.AreEqual("Form - Null process", ((Form)Project.Current.FormList[2]).Name);
		}

		[Test]
		public void ConnectionTest()
		{
            Assert.IsNull(((Form)Project.Current.FormList[0]).ConnectedPostProcess, "Form 1 should have no connected Process");

            IProcess process = ((Form)Project.Current.FormList[1]).ConnectedPostProcess;
			Assert.IsNotNull(process, "Form 2 should be connected to a Process");
			Assert.AreSame(process, (Process)Project.Current.ProcessList[0], "Connected process does not match Process in list");

            Assert.IsNull(((Form)Project.Current.FormList[2]).ConnectedPostProcess, "Form 3 should have no connected Process");
		}
	}
}
