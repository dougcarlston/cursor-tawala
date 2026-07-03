// $Workfile: UtilTest.cs $
// $Revision: 2 $	$Date: 2/28/08 10:42a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using TawalaTest.TestSupport;

using System.Text.RegularExpressions;

using Tawala.Projects;

namespace TawalaTest.TestSupportTest
{
    [TestFixture]
    public class UtilTest
    {
        [Test]
        public void NewTestProject()
        {
            Util.NewTestProject();

            Project project1 = Reflect<Project>.GetStaticField<Project>("current");
			ProjectEvents projectEvents1 = Reflect<Project>.GetStaticField<ProjectEvents>("projectEvents");

            Util.NewTestProject();

			Project project2 = Reflect<Project>.GetStaticField<Project>("current");
			ProjectEvents projectEvents2 = Reflect<Project>.GetStaticField<ProjectEvents>("projectEvents");

            Assert.IsNotNull(project1);
            Assert.IsNotNull(projectEvents1);
            Assert.IsNotNull(project2);
            Assert.IsNotNull(projectEvents2);
            Assert.AreNotSame(project1, project2);
            Assert.AreNotSame(projectEvents1, projectEvents2);
        }

		[Test]
		public void SaveAndReloadCurrentProject()
		{
			Util.NewTestProject();

			Project.Current.AddForm();
			Project.Current.AddProcess();
			Project.Current.AddDocument();

			Project oldProject = Project.Current;
			string oldXml = getNameNormalizedProjectXml();

			Util.SaveAndReloadCurrentProject();

			string newXml = getNameNormalizedProjectXml();

			Assert.AreNotSame(oldProject, Project.Current);
			Assert.IsNotNull(oldProject);
			Assert.IsNotNull(Project.Current);

			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual(1, Project.Current.DocumentList.Count);

			Assert.AreEqual(oldXml, newXml);
		}

		private string getNameNormalizedProjectXml()
		{
			return Regex.Replace(Project.Current.ToXml(), "project name=\"[^\"]*\"", "project name=\"replaced\"");
		}
    }
}