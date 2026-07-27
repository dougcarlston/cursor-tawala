using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

using NUnit.Framework;

using Tawala.Common;
using Tawala.Projects;
using Tawala.Functions.Runtime;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectXmlRoundTripTest
{
	[TestFixture]
	public class NormalizedProjectXmlTests : TestBase
	{
		private XmlWriterSettings xmlWriterSettings = null;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Init("NormalizationTestFiles");

			xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.CheckCharacters = true;
			xmlWriterSettings.CloseOutput = true;
			xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
			xmlWriterSettings.Encoding = Encoding.UTF8;
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.IndentChars = "  ";
			xmlWriterSettings.NewLineChars = "\r\n";
			xmlWriterSettings.NewLineHandling = NewLineHandling.Replace;
			xmlWriterSettings.OmitXmlDeclaration = false;
		}

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void MultiquestionPoll_NormalizedOriginalEqualsNormalizedSavedFile()
		{
			compareAgainstOrignalFile("MultiquestionPoll.tawala");
		}

		[Test]
		public void PanAm_NormalizedOriginalEqualsNormalizedSavedFile()
		{
			compareAgainstOrignalFile("PanAm.tawala");
		}

		[Test]
		public void PigskinPickEmtest_NormalizedOriginalEqualsNormalizedSavedFile()
		{
			compareAgainstOrignalFile("PigskinPickEmtest.tawala");
		}

		[Test]
		public void PotluckV3_NormalizedOriginalEqualsNormalizedSavedFile()
		{
			compareAgainstOrignalFile("Potluck.tawala");
		}

		[Test]
		public void OnlineExamBuilderV8EqualsNormalizedSavedFile()
		{
			compareAgainstOrignalFile("OnlineExamBuilderV8.tawala");
		}

		private void compareAgainstOrignalFile(string projectFileName)
		{
			string projectName = projectFileName.Replace(".tawala", "");

			string originalPath = Path.Combine(ProjectFilesDirectory, projectFileName);
			string savePath = Path.Combine(TestDirectory, projectFileName);

			Project.Open(originalPath);
			Project.Save(savePath);

			string originalNormalizedXml = normalizeXmlForComparison(originalPath, Path.Combine(TestDirectory, projectName + "-original-normalized.xml"));
			string savedNormalizedXml = normalizeXmlForComparison(savePath, Path.Combine(TestDirectory, projectName + "-saved-normalized.xml"));

			Assert.AreEqual(originalNormalizedXml, savedNormalizedXml);
		}

		private string normalizeXmlForComparison(string inputPath, string outputPath)
		{
			if (File.Exists(outputPath))
			{
				File.Delete(outputPath);
			}

			XmlDocument doc = new XmlDocument();
			doc.Load(inputPath);
			doc.Normalize();
			doc.PreserveWhitespace = true;

			// makes sure <tag/> is same as <tag><tag>
			XmlNodeList nodes = doc.DocumentElement.SelectNodes("//*");
			foreach (XmlNode n in nodes)
			{
				XmlElement element = n as XmlElement;

				if (element != null)
				{
					if (element.ChildNodes.Count == 0 && element.InnerXml.Length == 0)
					{
						element.IsEmpty = true;
					}
				}
			}

			XmlNode node = doc.DocumentElement.SelectSingleNode("//project");
			XmlElement projectElement = node as XmlElement;
			projectElement.RemoveAttribute("format");
			projectElement.RemoveAttribute("designerBuild");

			doc.PreserveWhitespace = false;

			using (XmlWriter xw = XmlWriter.Create(outputPath, xmlWriterSettings))
			{
				doc.Save(xw);
			}

			string result = File.ReadAllText(outputPath);

			return result;
		}
	}
}
