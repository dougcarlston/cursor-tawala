using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects.ProjectLoader;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest.ProjectLoaderTest
{
	[TestFixture]
	public class ProjectLoaderTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void ProjectLoaderProvidesXmlString()
		{
			string filePath = Util.GetTestFilePath("Single FibItem.xml");
			ProjectLoader.Load(filePath);

			string expectedXml =
				@"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
				@"<project name=""Single FibItem"" themePath=""default"" format=""1.9"" designerBuild=""0"">" + Environment.NewLine +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
				@"<items>" + Environment.NewLine +
				@"<fib label=""Q1"" style=""topLabels"">" + Environment.NewLine +
				@"<paragraph indent=""0"" align=""left"">" + Environment.NewLine +
				@"[Replace this with your question. Underscores create blanks.] <blank label=""a"" length=""20"" required=""false""/>" + Environment.NewLine +
				@"</paragraph>" + Environment.NewLine +
				@"</fib>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>" + Environment.NewLine +
				@"</forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, ProjectLoader.ProjectXmlString);
		}

		[Test]
		public void ProjectLoaderProvidesXmlElement()
		{
			string filePath = Util.GetTestFilePath("Single FibItem.xml");
			ProjectLoader.Load(filePath);

			string expectedXml =
				@"<project name=""Single FibItem"" themePath=""default"" format=""1.9"" designerBuild=""0"">" + Environment.NewLine +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
				@"<items>" + Environment.NewLine +
				@"<fib label=""Q1"" style=""topLabels"">" + Environment.NewLine +
				@"<paragraph indent=""0"" align=""left"">" + Environment.NewLine +
				@"[Replace this with your question. Underscores create blanks.] <blank label=""a"" length=""20"" required=""false"" />" + Environment.NewLine +
				@"</paragraph>" + Environment.NewLine +
				@"</fib>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>" + Environment.NewLine +
				@"</forms>" + Environment.NewLine +
				@"</project>";

			Assert.AreEqual(expectedXml, ProjectLoader.ProjectXmlElement.OuterXml);
		}
	}
}
