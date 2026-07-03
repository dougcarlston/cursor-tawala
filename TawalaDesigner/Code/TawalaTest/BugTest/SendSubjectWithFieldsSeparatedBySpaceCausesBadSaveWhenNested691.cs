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
	public class SendSubjectWithFieldsSeparatedBySpaceCausesBadSaveWhenNested691
	{
		[SetUp]
		public void Setup()
		{
			Util.NewTestProject();
		}

		private static string projectXmlStart =
			"<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine +
			"<project name=\"691test\" themePath=\"default\" format=\"1.8\" designerBuild=\"192\">" + Environment.NewLine +
			"<forms>" + Environment.NewLine +
			"<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\">" + Environment.NewLine +
			"<items>" + Environment.NewLine +
            "<fib label=\"Q1\" style=\"topLabels\"><paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font face=\"Arial\" size=\"200\" color=\"000000\">[Replace this with your question. Underscores create blanks.] </font><blank label=\"a\" length=\"20\" required=\"false\"></blank></paragraph></fib>" + Environment.NewLine +
            "<fib label=\"Q2\" style=\"topLabels\"><paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font face=\"Arial\" size=\"200\" color=\"000000\">[Replace this with your question. Underscores create blanks.] </font><blank label=\"a\" length=\"20\" required=\"false\"></blank></paragraph></fib>" + Environment.NewLine +
			"</items>" + Environment.NewLine +
			"</form>" + Environment.NewLine +
			"</forms>" + Environment.NewLine +
			"<processes>" + Environment.NewLine +
			"<process name=\"Process 1\">" + Environment.NewLine;

		private static string projectXmlEnd =
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

		private static string xmlWithSendNestedInIf =
			projectXmlStart +
			"<if>" + Environment.NewLine +
			"<conditions>" + Environment.NewLine +
			"<equals field=\"Form 1:Q1:a\">" + Environment.NewLine +
			"<string value=\"asdf\"/>" + Environment.NewLine +
			"</equals>" + Environment.NewLine +
			"</conditions>" + Environment.NewLine +
			"<trueSet>" + Environment.NewLine +
			"<send>" + Environment.NewLine +
			"<to addressLiteral=\"test\"/>" + Environment.NewLine +
			"<subject><field name=\"Form 1:Q1:a\"/> <field name=\"Form 1:Q2:a\"/></subject>" + Environment.NewLine +
			"<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>" + Environment.NewLine +
			"</send>" + Environment.NewLine +
			"</trueSet>" + Environment.NewLine +
			"</if>" + Environment.NewLine +
			projectXmlEnd;

		[Test]
		public void SpaceCharacterBetweenFieldsIsPreservedOnLoadWhenNestedInIf()
		{
			convertXmlToProject(xmlWithSendNestedInIf);
			verifySendSubjectContents(2);
		}

		private static string xmlWithSendNestedInForEach =
			projectXmlStart +

			"<get recordList=\"Record List 1\">" + Environment.NewLine +
			"<forms>" + Environment.NewLine +
			"<form name=\"Form 1\"/>" + Environment.NewLine +
			"</forms>" + Environment.NewLine +
			"</get>" + Environment.NewLine +
			"<foreach record=\"Record\" recordList=\"Record List 1\">" + Environment.NewLine +
			"<send>" + Environment.NewLine +
			"<to addressLiteral=\"test\"/>" + Environment.NewLine +
			"<subject><field name=\"Form 1:Q1:a\"/> <field name=\"Form 1:Q2:a\"/></subject>" + Environment.NewLine +
			"<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>" + Environment.NewLine +
			"</send>" + Environment.NewLine +
			"</foreach>" + Environment.NewLine +
			projectXmlEnd;

		[Test]
		public void SpaceCharacterBetweenFieldsIsPreservedOnLoadWhenNestedInForEach()
		{
			convertXmlToProject(xmlWithSendNestedInForEach);
			verifySendSubjectContents(3);
		}

		private void convertXmlToProject(string xmlString)
		{
			using (MemoryStream xmlStream = new MemoryStream())
			{
				byte[] xmlByteArray = System.Text.Encoding.UTF8.GetBytes(xmlString);
				xmlStream.Write(xmlByteArray, 0, xmlByteArray.Length);

				TawalaProjectConverter converter = new TawalaProjectConverter(xmlStream);
				converter.ConvertXmlToProject();
			}
		}

		private static string sendStatementXmlString =
			"<send>" + Environment.NewLine +
			"<to addressLiteral=\"test\"/>" + Environment.NewLine +
			"<subject><field name=\"Form 1:Q1:a\"/> <field name=\"Form 1:Q2:a\"/></subject>" + Environment.NewLine +
			"<body document=\"Document 1\" reset=\"false\" showHeader=\"true\"/>" + Environment.NewLine +
			"</send>";

		private void verifySendSubjectContents(int lineIndex)
		{
			SendStatement sendStatement = Project.Current.ProcessList[0].Lines[lineIndex].Statement as SendStatement;

			Assert.AreEqual("<<Form 1:Q1:a>> <<Form 1:Q2:a>>", sendStatement.Subject);
			Assert.AreEqual(sendStatementXmlString, sendStatement.ToXml());
		}
	}
}
