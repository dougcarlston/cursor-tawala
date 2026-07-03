// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.PageHeaders
{
    [TestFixture]
    public class PageHeaderText2349
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        [Test]
        public void DefaultHeaderNotNull()
        {
            Assert.IsNotNull(Project.Current.PageHeader);
        }

        [Test]
        public void DefaultHeaderTextEmpty()
        {
            Assert.AreEqual(string.Empty, Project.Current.PageHeader.Text);
        }

        [Test]
        public void DefaultHeaderXmlEmpty()
        {
            Assert.AreEqual("<pageHeader></pageHeader>", Project.Current.PageHeader.ToXml());
        }

        [Test]
        public void DefaultProjectXmlHasNoHeader()
        {
            Assert.IsFalse(Project.Current.ToXml().Contains("<pageHeader "));
        }

        [Test]
        public void HeaderTextEscapedInXml()
        {
            const string expectedXml = "<pageHeader><text>&lt;This &amp; That&gt;</text></pageHeader>";

            Project.Current.PageHeader.Text = "<This & That>";

            Assert.AreEqual(expectedXml, Project.Current.PageHeader.ToXml());
        }

        [Test]
        public void RoundTripHeaderInProjectXml()
        {
            const string expectedXml = "<pageHeader><text>My Header</text></pageHeader>";

            Project.Current.PageHeader.Text = "My Header";

            Assert.IsTrue(Project.Current.ToXml().Contains(expectedXml));

            string tempFile = Path.GetTempFileName();

            Project.Save(tempFile);

            Util.NewTestProject();
            Assert.IsFalse(Project.Current.ToXml().Contains(expectedXml));

            Util.LoadProject(tempFile);
            Assert.IsTrue(Project.Current.ToXml().Contains(expectedXml));
        }

        [Test]
        public void SettingHeaderTextResultsInHeaderInProjectXml()
        {
            const string expectedXml = "<pageHeader><text>My Project Header</text></pageHeader>";

            Project.Current.PageHeader.Text = "My Project Header";

            Assert.IsTrue(Project.Current.ToXml().Contains(expectedXml));

            Project.Current.PageHeader.Text = string.Empty;

            Assert.IsFalse(Project.Current.ToXml().Contains("<pageHeader "));
        }
    }
}