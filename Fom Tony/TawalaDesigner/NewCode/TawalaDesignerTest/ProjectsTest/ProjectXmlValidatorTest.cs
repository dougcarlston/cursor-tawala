// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectsTest
{
    [TestFixture]
    public class ProjectXmlValidatorTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
            projectXmlValidator = new ProjectXmlValidator();
        }

        [TearDown]
        public void TearDown()
        {
            projectXmlValidator = null;
        }

        #endregion

        private ProjectXmlValidator projectXmlValidator;

        private static readonly string tableInDocumentXml
            = @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
              @"<project name=""table"" themePath=""default"" format=""1.11"" designerBuild=""31"">" +
              Environment.NewLine +
              @"<pageHeader></pageHeader><documents>" + Environment.NewLine +
              @"<document name=""Document 1"">" + Environment.NewLine +
              @"<xmlData>" + Environment.NewLine +
              @"<table indent=""0""><row><cell width=""5400""><division indent=""0"" " +
              @"align=""left""><font></font></division></cell><cell width=""5400""><division indent=""0"" " +
              @"align=""left""><font></font></division></cell></row><row><cell width=""5400""><division " +
              @"indent=""0"" align=""left""><font></font></division></cell><cell width=""5400""><division " +
              @"indent=""0"" align=""left""><font></font></division></cell></row></table>" +
              Environment.NewLine +
              @"</xmlData>" + Environment.NewLine +
              @"</document>" + Environment.NewLine +
              @"</documents>" + Environment.NewLine +
              @"</project>";

        [Test]
        public void EmptyProjectValidationSucceeds()
        {
            Assert.IsTrue(projectXmlValidator.ValidateXML(), projectXmlValidator.Message);
        }

        [Test]
        public void ProjectWithTableValidationSucceeds()
        {
            Util.OpenProjectXml(tableInDocumentXml);
            Assert.IsTrue(projectXmlValidator.ValidateXML(), projectXmlValidator.Message);
        }

        [Test]
        public void ProjectRoundTripTestWithTableSucceeds()
        {
            Util.OpenProjectXml(tableInDocumentXml);
            string beforeXml = Project.Current.ToXmlForSaving();

            Util.OpenProjectXml(beforeXml);
            string afterXml = Project.Current.ToXmlForSaving();

            Assert.AreEqual(beforeXml, afterXml);
        }

        [Test]
        public void SimpleProjectValidationSucceeds()
        {
            Project.Current.AddForm();
            Project.Current.AddDocument();
            Project.Current.AddProcess();

            Assert.IsTrue(projectXmlValidator.ValidateXML());
        }

        [Test]
        public void SimpleProjectWithFormItemsValidationSucceeds()
        {
            IForm form = Project.Current.AddForm();
            form.ItemList.Add(new NewHeadingItem());
            form.ItemList.Add(new NewTextItem());
            form.ItemList.Add(new NewFibItem());
            form.ItemList.Add(new NewMcqItem());
            form.ItemList.Add(new NewSkipInstructionsItem());

            Project.Current.AddDocument();
            Project.Current.AddProcess();

            Assert.IsTrue(projectXmlValidator.ValidateXML());
        }

        private readonly string simpleDocumentXml =
            @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
            @"<project name=""Untitled"" themePath=""default"" format=""1.11"" designerBuild=""31"">" +
            Environment.NewLine +
            @"<pageHeader></pageHeader><documents>" + Environment.NewLine +
            @"<document name=""Document 1"">" + Environment.NewLine +
            @"<xmlData>" + Environment.NewLine +
            @"sdgdffgsdfsfsfffffdsdfdfs" + Environment.NewLine +
            @"</xmlData>" + Environment.NewLine +
            @"</document>" + Environment.NewLine +
            @"</documents>" + Environment.NewLine +
            @"</project>";

        [Test]
        public void SimpleProjectDocumentDoesNotAddAdditionalNewlinesAroundXmlDataTag()
        {
            Util.OpenProjectXml(simpleDocumentXml);
            Assert.IsTrue(projectXmlValidator.ValidateXML(), projectXmlValidator.Message);
        }
    }
}