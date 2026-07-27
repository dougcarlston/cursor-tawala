// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectsTest.FactoriesTest
{
    [TestFixture]
    public class ProjectCreationTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
            ComponentMaker.UseNewComponents(true);
        }

        [TearDown]
        public void TearDown()
        {
            ComponentMaker.UseNewComponents(false);
        }

        #endregion

        [Test]
        public void CanCreateProjectFromXml()
        {
            string projectXml =
                @"<project name=""Single FibItem"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" +
                Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"">" +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1"" style=""topLabels"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"[Replace this with your question. Underscores create blanks.] <blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
                @"</paragraph>" +
                @"</fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CanCreateProjectWithDocumentFromXml()
        {
            string projectXml =
                @"<project name=""Project With Document"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<documents>" + Environment.NewLine +
                @"<document name=""Document 1"">" + Environment.NewLine +
                @"<xmlData>" +
                @"<paragraph indent=""0"" align=""left"">DocumentText</paragraph>" +
                @"</xmlData>" + Environment.NewLine +
                @"</document>" + Environment.NewLine +
                @"</documents>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CanCreateProjectWithFieldInHeadingItemFromXml()
        {
            string projectXml =
                @"<project name=""Field In HeadingItem"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"">" +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1"" style=""topLabels"">" +
                @"<paragraph indent=""0"" align=""left"">FIB <blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
                @"</paragraph>" +
                @"</fib>" + Environment.NewLine +
                @"<heading label=""H1"" alternateLabel=""alternate"" type=""Sub"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"<field name=""Form 1:Q1:a""/>" +
                @"</paragraph>" +
                @"</heading>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CanCreateProjectWithFieldInTextItemFromXml()
        {
            string projectXml =
                @"<project name=""Field In TextItem"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"">" +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1"" style=""topLabels"">" +
                @"<paragraph indent=""0"" align=""left"">FIB <blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
                @"</paragraph>" +
                @"</fib>" + Environment.NewLine +
                @"<text label=""T1"" style=""normal"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"<field name=""Form 1:Q1:a""/>" +
                @"</paragraph>" +
                @"</text>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CanCreateProjectWithFieldReferenceInDocumentFromXml()
        {
            string projectXml =
                @"<project name=""Project With Document"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"">" +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1"" style=""topLabels"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"<font face=""Arial"" size=""200"" color=""000000"">" +
                @"[Replace this with your question. Underscores create blanks.] " +
                @"</font>" +
                @"<blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
                @"</paragraph>" +
                @"</fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"<documents>" + Environment.NewLine +
                @"<document name=""Document 1"">" + Environment.NewLine +
                @"<xmlData>" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"<field name=""Form 1:Q1:a""/>" +
                @"</paragraph>" +
                @"</xmlData>" + Environment.NewLine +
                @"</document>" + Environment.NewLine +
                @"</documents>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CanCreateProjectWithFieldReferenceInProcessFromXml()
        {
            string projectXml =
                @"<project name=""Project With Process"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"">" +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1"" style=""topLabels""><paragraph indent=""0"" align=""left"">" +
                @"<font face=""Arial"" size=""200"" color=""000000"">" +
                @"[Replace this with your question. Underscores create blanks.] " +
                @"</font>" +
                @"<blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
                @"</paragraph>" +
                @"</fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Variable"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Form 1:Q1:a""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CanCreateProjectWithFormAndPostProcessFromXml()
        {
            string projectXml =
                @"<project name=""Form With Post Process"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" process=""Process 1"">" +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CanCreateProjectWithFormAndPreProcessFromXml()
        {
            string projectXml =
                @"<project name=""Form With Post Process"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" preProcess=""Process 1"">" +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CanCreateProjectWithForwardFieldReferenceToSameFormFromXml()
        {
            string projectXml =
                @"<project name=""Field In TextItem"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"">" +
                @"<items>" + Environment.NewLine +
                @"<text label=""T1"" style=""normal"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"<field name=""Form 1:Q1:a""/>" +
                @"</paragraph>" +
                @"</text>" + Environment.NewLine +
                @"<fib label=""Q1"" style=""topLabels"">" +
                @"<paragraph indent=""0"" align=""left"">FIB <blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
                @"</paragraph>" +
                @"</fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CanCreateProjectWithForwardFieldReferenceToSecondFormFromXml()
        {
            string projectXml =
                @"<project name=""Field In TextItem"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"">" +
                @"<items>" + Environment.NewLine +
                @"<text label=""T1"" style=""normal"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"<field name=""Form 2:Q1:a""/>" +
                @"</paragraph>" +
                @"</text>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" +
                @"<form name=""Form 2"" startPoint=""true"">" +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1"" style=""topLabels"">" +
                @"<paragraph indent=""0"" align=""left"">FIB <blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
                @"</paragraph>" +
                @"</fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CanCreateProjectWithProcessFromXml()
        {
            string projectXml =
                @"<project name=""Project With Process"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Variable"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CanCreateProjectWithVariableReferenceInTextItemFromXml()
        {
            string projectXml =
                @"<project name=""Var in Text from Classic"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"">" +
                @"<items>" + Environment.NewLine +
                @"<text label=""T1"" style=""normal""><paragraph indent=""0"" align=""left"">" +
                @"<field name=""Var""/>" +
                @"</paragraph>" +
                @"</text>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Var"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(projectXml, Project.Current.ToXml());
        }

        [Test]
        public void CreatingProjectWithFormFromXmlCausesAllFormsToReturnCountOfOne()
        {
            string projectXml =
                @"<project name=""Single FibItem"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" +
                Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"">" +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1"" style=""topLabels"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"[Replace this with your question. Underscores create blanks.] <blank label=""a"" length=""20"" " +
                @"height=""1"" required=""false""/>" +
                @"</paragraph>" +
                @"</fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            Assert.AreEqual(1, Project.Current.AllForms.Count);
        }

        [Test]
        public void FibBlankCreatedFromProjectXmlHasCorrectDefaultLabel()
        {
            string projectXml =
                @"<project name=""Single FibItem"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" +
                Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"">" +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1"" style=""topLabels"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"[Replace this with your question. Underscores create blanks.] <blank label=""a"" length=""20"" " +
                @"required=""false""/>" +
                @"</paragraph>" +
                @"</fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Project.Create(new XmlElement(projectXml));

            IForm form = Project.Current.FormList[0];
            var fibItem = form.ItemList[0] as IFibItem;
            IBlank blank = fibItem.BlankList[0];

            Assert.AreEqual("Q1:a", blank.FieldName);
        }
    }
}