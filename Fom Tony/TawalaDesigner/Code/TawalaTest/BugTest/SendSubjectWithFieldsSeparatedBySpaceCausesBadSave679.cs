using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;
using Tawala.Projects;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class SendSubjectWithFieldsSeparatedBySpaceCausesBadSave679
	{
		private string projectXmlString =
			"<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine +
			"<project name=\"679test\" themePath=\"default\" format=\"1.8\" designerBuild=\"179\">" + Environment.NewLine +
			"<forms>" + Environment.NewLine +
			"<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\">" + Environment.NewLine +
			"<items>" + Environment.NewLine +
            "<fib label=\"Q1\" style=\"topLabels\"><paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font face=\"Arial\" size=\"200\" color=\"000000\">[Replace this with your question. Underscores create blanks.] </font><blank label=\"a\" length=\"20\" required=\"false\"></blank></paragraph></fib>" + Environment.NewLine +
            "<fib label=\"Q2\" style=\"topLabels\"><paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font face=\"Arial\" size=\"200\" color=\"000000\">[Replace this with your question. Underscores create blanks.] </font><blank label=\"a\" length=\"20\" required=\"false\"></blank></paragraph></fib>" + Environment.NewLine +
			"</items>" + Environment.NewLine +
			"</form>" + Environment.NewLine +
			"</forms>" + Environment.NewLine +
			"<processes>" + Environment.NewLine +
			"<process name=\"Process 1\">" + Environment.NewLine +
			"<send>" + Environment.NewLine +
			"<to addressLiteral=\"test\"/>" + Environment.NewLine +
			"<subject><field name=\"Form 1:Q1:a\"/> <field name=\"Form 1:Q2:a\"/></subject>" + Environment.NewLine +
			"<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>" + Environment.NewLine +
			"</send>" + Environment.NewLine +
			"</process>" + Environment.NewLine +
			"</processes>" + Environment.NewLine +
			"<documents>" + Environment.NewLine +
			"<document name=\"Document 1\">" + Environment.NewLine +
			"<xmlData>" + Environment.NewLine +
			"<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions></paragraph>" + Environment.NewLine +
			"</xmlData>" + Environment.NewLine +
			"</document>" + Environment.NewLine +
			"</documents>" + Environment.NewLine +
			"</project>";

		private string sendStatementXmlString =
			"<send>" + Environment.NewLine +
			"<to addressLiteral=\"test\"/>" + Environment.NewLine +
			"<subject><field name=\"Form 1:Q1:a\"/> <field name=\"Form 1:Q2:a\"/></subject>" + Environment.NewLine +
			"<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>" + Environment.NewLine +
			"</send>";

		[Test]
		public void SpaceCharacterBetweenFieldsIsPreservedOnLoad()
		{
			Util.NewTestProject();

			using (MemoryStream xmlStream = new MemoryStream())
			{
				byte[] xmlByteArray = System.Text.Encoding.UTF8.GetBytes(projectXmlString);
				xmlStream.Write(xmlByteArray, 0, xmlByteArray.Length);

				TawalaProjectConverter converter = new TawalaProjectConverter(xmlStream);
				converter.ConvertXmlToProject();
			}

			SendStatement sendStatement = Project.Current.ProcessList[0].Lines[0].Statement as SendStatement;

			Assert.AreEqual("<<Form 1:Q1:a>> <<Form 1:Q2:a>>", sendStatement.Subject);
			Assert.AreEqual(sendStatementXmlString, sendStatement.ToXml());
		}
	}
}
