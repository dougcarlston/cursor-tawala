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
    public class FormPrePopulateLastEntry2913
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
            "<form name=\"Form 1\" startPoint=\"true\" dataEntryOnly=\"true\"></form>" +
            "</forms>" +
            "</project>";

        [Test]
        public void FormDataEntryOnlyAppearsInXmlIfTrue()
        {
            IForm form = Project.Current.AddForm();
            form.DataEntryOnly = true;

            Match m = Regex.Match(Project.Current.ToXml(), "dataEntryOnly=\"true\"");
            Assert.IsTrue(m.Success);
        }

        [Test]
        public void FormDataEntryOnlyDefaultsToFalse()
        {
            IForm form = Project.Current.AddForm();
            Assert.IsFalse(form.DataEntryOnly);
        }

        [Test]
        public void FormDataEntryOnlyDoesNotAppearInXmlIfFalse()
        {
            IForm form = Project.Current.AddForm();
            Assert.IsFalse(form.DataEntryOnly);

            Match m = Regex.Match(Project.Current.ToXml(), "dataEntryOnly=\"");
            Assert.IsFalse(m.Success);
        }

        [Test]
        public void FormDataEntryOnlyRestoredWhenProjectReloaded()
        {
            Project.Create(new XmlElement(projectXml));

            IForm form = Project.Current.FormList[0];

            Assert.IsTrue(form.DataEntryOnly);
        }
    }
}