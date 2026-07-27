// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Components;
using Tawala.XmlSupport;

namespace TawalaTest.TestSupport
{
    public static class Util
    {
        private const string emptyDataSourceXml =
            "<datasources>" +
            "</datasources>";

        private const string testDataSourceXml =
            "<datasources>" +
            "<datasource name=\"ClientInfo\">" +
            "<field name=\"Q1:a\" type=\"string\"/>" +
            "<field name=\"name\" type=\"string\"/>" +
            "<field name=\"Q3\" type=\"mcq\" choices=\"2\" onlyone=\"true\"/>" +
            "</datasource>" +
            "<datasource name=\"DataSource2\">" +
            "<field name=\"Q1:a\" type=\"string\"/>" +
            "<field name=\"Q2\" type=\"mcq\" choices=\"3\" onlyone=\"false\"/>" +
            "</datasource>" +
            "</datasources>";

        public static int NextUniqueIDValue { get { return Reflect<Project>.GetStaticField<int>("nextUniqueID"); } }

        /// <summary>
        /// Use this function so that we can remove NewTestProject from the Project class.
        /// </summary>
        public static void NewTestProject()
        {
            ClearDataSources();
            Reflect<Project>.SetStaticField<Project>("current", null);
            Reflect<Project>.SetStaticField("projectEvents", new ProjectEvents());
            Reflect<Project>.SetStaticField("nextUniqueID", 1);
            Reflect<Project>.SetStaticField("invitationMapById", new ProjectInvitationMapById());

            ComponentMaker.UseNewComponents(false);
            Project.New();
        }

        public static Project.ProjectFileOpenResult OpenProjectXml(string projectXml)
        {
            Project.ProjectFileOpenResult result = Project.ProjectFileOpenResult.OK;

            NewTestProject();

            string tempFileName = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFileName, projectXml);
                result = Project.Open(tempFileName);
                Assert.IsNotNull(Project.Current, "Failed to create project from xml string!");
            }
            finally
            {
                File.Delete(tempFileName);
            }

            return result;
        }

        /// <summary>
        /// Returns the full path corresponding to the specified filename. It is presumed that the test file resides
        /// in the same directory as the calling assembly.
        /// </summary>
        public static string GetTestFilePath(string testFileName)
        {
            string codeBase = Assembly.GetCallingAssembly().CodeBase;
            var uri = new Uri(codeBase);
            string testDirectory = Path.GetDirectoryName(uri.AbsolutePath);
            string testFilePath = Path.Combine(testDirectory, testFileName);

            return testFilePath;
        }

        public static string SaveCurrentProject()
        {
            string savePath = Path.GetTempFileName();
            Project.Save(savePath);
            return savePath;
        }

        public static Project.ProjectFileOpenResult LoadProject(string path)
        {
            NewTestProject();
            return Project.Open(path);
        }

        public static void SaveAndReloadCurrentProject()
        {
            string savePath = Path.GetTempFileName();

            try
            {
                Project.Save(savePath);
                string saved1Xml = File.ReadAllText(savePath);

                NewTestProject();

                Project.ProjectFileOpenResult opened = Project.Open(savePath);
                Assert.AreEqual(Project.ProjectFileOpenResult.OK, opened);

                File.Delete(savePath);
                Assert.IsFalse(File.Exists(savePath));

                Project.Save(savePath);
                string saved2Xml = File.ReadAllText(savePath);

                Assert.AreEqual(saved1Xml, saved2Xml);
            }
            finally
            {
                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                }
            }
        }

        public static TreeNode FindTreeNodeByText(TreeNodeCollection nodes, string text)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.CompareTo(text) == 0)
                {
                    return node;
                }
            }
            return null;
        }

        public static void CreateTestDataSources()
        {
            initialize(testDataSourceXml);
        }

        public static void ClearDataSources()
        {
            initialize(emptyDataSourceXml);
        }

        public static string StripProjectHeader(string xml)
        {
            Match m = Regex.Match(xml, "<project [^>]+>\r\n");
            return xml.Substring(m.Index + m.Length);
        }

        private static void initialize(string dataSourcesXml)
        {
            MethodInfo init = typeof(FieldProviders).GetMethod("initialize", BindingFlags.NonPublic | BindingFlags.Static);
            init.Invoke(null, new object[] {new XmlElement(dataSourcesXml)});
        }
    }
}