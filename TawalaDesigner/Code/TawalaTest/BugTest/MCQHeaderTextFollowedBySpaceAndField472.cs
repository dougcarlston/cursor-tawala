using System;
using System.Collections.Generic;
using System.Text;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;
using Tawala.Projects;


using NUnit.Framework;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class MCQHeaderTextFollowedBySpaceAndField472
	{
		[Test]
		public void SpaceInDocumentXmlPreservedWhenProjectSaveAndReloaded()
		{
			//"NOTE: offending space has been replaced with <sp/> - this test passes if FormItemSpace is modified"
			string projectXml =
				@"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
				@"<project name=""Untitled"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" + Environment.NewLine +
				@"<pageHeader></pageHeader>" +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
				@"<items>" + Environment.NewLine +
				@"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
				@"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
				@"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
				@"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
				@"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont + @"FIB " + XmlConstants.EndFont + @"<blank label=""a"" length=""10"" required=""false""></blank></paragraph></fib>" + Environment.NewLine +
				@"<mc label=""Q2"" onlyone=""true"" required=""false""" + XmlConstants.DefaultMcqItemStyleAttribute + @"><question><paragraph indent=""0"" " +
				@"align=""left""><tabPositions><tabStop position=""720""/><tabStop position=""1440""/><tabStop " +
				@"position=""2160""/><tabStop position=""2880""/><tabStop position=""3600""/><tabStop " +
				@"position=""4320""/><tabStop position=""5040""/><tabStop position=""5760""/><tabStop " +
				@"position=""6480""/><tabStop position=""7200""/><tabStop position=""7920""/><tabStop " +
				@"position=""8640""/><tabStop position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont + @"TEST" + XmlConstants.EndFont + @"" + XmlConstants.BeginFont + @"<field name=""Form 1:Q1:a""/>" + XmlConstants.EndFont + @"<sp/></paragraph></question><choice " +
				@"label=""a""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
				@"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
				@"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
				@"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
				@"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
				@"position=""9360""/><tabStop position=""10080""/></tabPositions></paragraph></choice></mc>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>" + Environment.NewLine +
				@"</forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;

			Util.OpenProjectXml(projectXml);
			string xml = Tawala.Projects.Project.Current.ToXmlForSaving();
			Assert.AreEqual(projectXml, xml);
		}
	}
}
