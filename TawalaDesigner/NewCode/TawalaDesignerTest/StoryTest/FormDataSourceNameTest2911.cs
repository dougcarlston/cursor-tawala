using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest
{
    [TestFixture]
    public class FormDataSourceNameTest2911
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

        private const string projectXml =
            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
            "<project name=\"Untitled\" themePath=\"default\" format=\"1.10\" designerBuild=\"0\">" +
            "<forms>" +
            "<form name=\"Form 1\" startPoint=\"true\" dataSourceName=\"My Data Source\"></form>" +
            "</forms>" +
            "</project>";

        [Test]
        public void DataSourceNameRetainedWhenProjectReloaded()
        {
            Project.Create(new XmlElement(projectXml));

            IForm form = Project.Current.FormList[0];

            Assert.IsTrue(form.IsDataSource);
            Assert.AreEqual("My Data Source", form.DataSourceName);
        }

        [Test]
        public void FormDefaultDataSourceNameIsEmptyString()
        {
            IForm form = Project.Current.AddForm();
            Assert.AreEqual(string.Empty, form.DataSourceName);
        }

        [Test]
        public void IsDataSourceReturnTrueIfFormDataSourceIsSet()
        {
            IForm form = Project.Current.AddForm();
            Assert.IsFalse(form.IsDataSource);

            form.DataSourceName = "My Data Source";
            Assert.IsTrue(form.IsDataSource);
        }

        [Test]
        public void SettingFormDataSourceNameResultsInAppearingInProjectXml()
        {
            IForm form = Project.Current.AddForm();

            form.DataSourceName = "My Other Data Source";

            Match m = Regex.Match(Project.Current.ToXml(), "dataSourceName=\"[^\"]+\"");
            Assert.AreEqual("dataSourceName=\"My Other Data Source\"", m.Value);
        }
    }
}