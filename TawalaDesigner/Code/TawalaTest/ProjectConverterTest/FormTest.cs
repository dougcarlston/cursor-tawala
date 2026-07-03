using System;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion of Documents.
	/// </summary>
	[TestFixture]
	public class FormTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("Form.xml");
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
			Assert.AreEqual(3, Project.Current.FormList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Form 2", ((Form)Project.Current.FormList[1]).Name);
			Assert.AreEqual("Form 3", ((Form)Project.Current.FormList[2]).Name);
		}


		[Test]
		public void StartingPoints()
		{
			Assert.IsTrue(((Form)Project.Current.FormList[0]).StartingPoint);
			Assert.IsFalse(((Form)Project.Current.FormList[1]).StartingPoint);
			Assert.IsTrue(((Form)Project.Current.FormList[2]).StartingPoint);
		}
	}
}
