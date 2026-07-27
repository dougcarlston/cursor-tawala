// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.ProjectXml
{
    /// <summary>
    /// Acceptance tests for story 3024 (Designer cannot load Project with newer XML format)
    /// </summary>
    [TestFixture]
    public class CannotLoadNewerXmlFormatVersion3024
    {
        [Test]
        public void ProjectXmlWithFormatOlderThanCurrentLoadsCorrectly()
        {
            Project.ProjectFileOpenResult result = Project.ProjectFileOpenResult.OK;

            string projectXmlWithOlderFormat =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Project 1"" themePath=""default"" format=""1.11"" " +
                @"designerBuild=""210"">" + Environment.NewLine +
                @"<pageHeader></pageHeader><forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            try
            {
                result = Util.OpenProjectXml(projectXmlWithOlderFormat);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreEqual(Project.ProjectFileOpenResult.OK, result);
        }

        [Test]
        public void ProjectXmlWithMajorFormatNewerThanCurrentIsNotLoaded()
        {
            Project.ProjectFileOpenResult result = Project.ProjectFileOpenResult.OK;

            string projectXmlWithNewerMajorFormat =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Project 1"" themePath=""default"" format=""999.11"" " +
                @"designerBuild=""210"">" + Environment.NewLine +
                @"<pageHeader></pageHeader><forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            try
            {
                result = Util.OpenProjectXml(projectXmlWithNewerMajorFormat);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreEqual(Project.ProjectFileOpenResult.NewerXmlFormat, result);
        }

        [Test]
        public void ProjectXmlWithMinorFormatNewerThanCurrentIsNotLoaded()
        {
            Project.ProjectFileOpenResult result = Project.ProjectFileOpenResult.OK;

            string projectXmlWithNewerMinorFormat =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Project 1"" themePath=""default"" format=""1.99999"" " +
                @"designerBuild=""210"">" + Environment.NewLine +
                @"<pageHeader></pageHeader><forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            try
            {
                result = Util.OpenProjectXml(projectXmlWithNewerMinorFormat);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.AreEqual(Project.ProjectFileOpenResult.NewerXmlFormat, result);
        }
    }
}