using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class RenamingFormDoesNotUpdateFieldsPalette808
	{
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

        [Test]
        public void ProjectComponentListsEventsAreEnabledOnNewProject()
        {
            Assert.IsTrue(Reflect<Project>.GetField<FormList>("formList", Project.Current).EnableEvents);
            Assert.IsTrue(Reflect<Project>.GetField<ProcessList>("processList", Project.Current).EnableEvents);
            Assert.IsTrue(Reflect<Project>.GetField<DocumentList>("documentList", Project.Current).EnableEvents);
        }

        [Test]
        public void ProjectComponentListsEventsAreEnabledAfterLoadingProject()
        {
            const string projectXml =
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                "<project name=\"empty\" themePath=\"default\" format=\"1.10\" designerBuild=\"0\">" +
                "</project>";

            Project.Create(new XmlElement(projectXml));

            Assert.IsTrue(Reflect<Project>.GetField<FormList>("formList", Project.Current).EnableEvents);
            Assert.IsTrue(Reflect<Project>.GetField<ProcessList>("processList", Project.Current).EnableEvents);
            Assert.IsTrue(Reflect<Project>.GetField<DocumentList>("documentList", Project.Current).EnableEvents);
        }
	}
}
