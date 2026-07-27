using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class VariableSetInSkipAndReferencedInFibCausesBadSave954
	{

		private static readonly string fibItemXml =
			@"<fib label=""Q1"" style=""topLabels"">" +
			@"<paragraph indent=""0"" align=""left""><tabPositions><tabStop position=""2880""/></tabPositions>" +
			@"<font face=""Arial"" size=""200"" color=""000000"">FIB </font>" +
			@"<font face=""Arial"" size=""200"" color=""000000""><field name=""Var""/></font>" +
			@"<sp/>" +
            @"<blank label=""a"" length=""20"" required=""false""></blank>" +
			@"</paragraph>" +
			@"</fib>" + Environment.NewLine;

		private readonly string projectXmlWithVariableSetInSkipInstructionsAndReferencedInFib =
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
			@"<project name=""Untitled"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" + Environment.NewLine +
			@"<pageHeader></pageHeader><forms>" + Environment.NewLine +
			@"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
			@"<items>" + Environment.NewLine +
			@"<skipInstructions>" + Environment.NewLine +
			@"<set field=""Var"" arithmeticAsText=""false"">" + Environment.NewLine +
			@"<string value=""0""/>" + Environment.NewLine +
			@"</set>" + Environment.NewLine +
			@"</skipInstructions>" + Environment.NewLine +
			fibItemXml +
			@"</items>" + Environment.NewLine +
			@"</form>" + Environment.NewLine +
			@"</forms>" + Environment.NewLine +
			@"</project>";

		private static readonly string mcqItemXml =
			@"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical""><question><paragraph " +
			@"indent=""0"" align=""left""><tabPositions><tabStop position=""2880""/></tabPositions><font " +
			@"face=""Arial"" size=""200"" color=""000000""><field " +
			@"name=""Var""/></font></paragraph></question><choice label=""a""><paragraph indent=""0"" " +
			@"align=""left""><tabPositions><tabStop position=""2880""/></tabPositions></paragraph></choice></mc>" +
			Environment.NewLine;

		private readonly string projectXmlWithVariableSetInSkipInstructionsAndReferencedInMcq =
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
			@"<project name=""Untitled"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" + Environment.NewLine +
			@"<pageHeader></pageHeader><forms>" + Environment.NewLine +
			@"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
			@"<items>" + Environment.NewLine +
			@"<skipInstructions>" + Environment.NewLine +
			@"<set field=""Var"" arithmeticAsText=""false"">" + Environment.NewLine +
			@"<string value=""0""/>" + Environment.NewLine +
			@"</set>" + Environment.NewLine +
			@"</skipInstructions>" + Environment.NewLine +
			mcqItemXml +
			@"</items>" + Environment.NewLine +
			@"</form>" + Environment.NewLine +
			@"</forms>" + Environment.NewLine +
			@"</project>";

		[Test]
		public void VariableSetInSkipInstructionsIsReferencedProperlyInFib()
		{
			Util.OpenProjectXml(projectXmlWithVariableSetInSkipInstructionsAndReferencedInFib);

			var fibItem = Project.Current.FormList[0].ItemList[1] as FibItem;

			Assert.AreEqual(fibItemXml, fibItem.ToXml("Q1"));
		}

		[Test]
		public void VariableSetInSkipInstructionsIsReferencedProperlyInMcq()
		{
			Util.OpenProjectXml(projectXmlWithVariableSetInSkipInstructionsAndReferencedInMcq);

			var mcqItem = Project.Current.FormList[0].ItemList[1] as McqItem;

			Assert.AreEqual(mcqItemXml, mcqItem.ToXml("Q1"));
		}
	}
}