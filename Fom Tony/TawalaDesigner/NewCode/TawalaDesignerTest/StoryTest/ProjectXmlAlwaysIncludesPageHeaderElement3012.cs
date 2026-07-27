using System;
using NMock2;
using NUnit.Framework;
using Tawala.Interfaces;
using Tawala.ProjectExplorer;
using Tawala.Projects;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest
{
	[TestFixture]
	public class ProjectXmlAlwaysIncludesPageHeaderElement3012		// Required to allow online Theme Builder to add a Page Header when none has been assigned in the Designer
	{
		[Test]
		public void XmlForEmptyProjectIncludesPageHeaderElement()
        {
			Util.NewTestProject();

			string expectedXml =
				 @"<project name=""Untitled"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" +
				Environment.NewLine +
				@"<pageHeader></pageHeader>" +
				@"</project>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, Project.Current.ToXml());
		}
	}
}
