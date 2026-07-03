// $Workfile: Util.cs $
// $Revision: 16 $	$Date: 2/28/08 10:42a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

using NUnit.Framework;

using Tawala.Projects;
using Tawala.XmlSupport;

namespace TawalaTest.TestingSupport
{
    public static class Util
    {
		/// <summary>
		/// Use this function so that we can remove NewTestProject from the Project class.
		/// </summary>
        /// 

        public static string LocalApplicationDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(localApplicationDataPath))
                {
                    localApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    localApplicationDataPath = Path.Combine(localApplicationDataPath, getAttribute<AssemblyCompanyAttribute>().Company);
                    localApplicationDataPath = Path.Combine(localApplicationDataPath, getAttribute<AssemblyProductAttribute>().Product);
                    localApplicationDataPath = Path.Combine(localApplicationDataPath, getAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
                }
                return localApplicationDataPath;
            }

        }

        private static string localApplicationDataPath = string.Empty;

        private static T getAttribute<T>() where T : Attribute
        {
            return typeof(Project).Assembly.GetCustomAttributes(typeof(T), false)[0] as T;
        }

        public static void NewTestProject()
		{
			ClearDataSources();
            Reflect<Project>.SetStaticProperty<Project>("Current", null);
			Reflect<Project>.SetStaticField<ProjectEvents>("projectEvents", new ProjectEvents());
			Reflect<Project>.SetStaticField<int>("nextUniqueID", 1);

			ComponentMaker.UseNewComponents(false);
            Tawala.Projects.Project.New();
		}

		public static int NextUniqueIDValue
		{
			get { return Reflect<Project>.GetStaticField<int>("nextUniqueID"); }
		}

        public static void OpenProjectXml(string projectXml)
        {
            NewTestProject();

			string tempFileName = Path.GetTempFileName();
			try
			{
				File.WriteAllText(tempFileName, projectXml);
				Project.Open(tempFileName);
				Assert.IsNotNull(Project.Current, "Failed to create project from xml string!");
			}
			finally
			{
				File.Delete(tempFileName);
			}
        }

		/// <summary>
		/// Returns the full path corresponding to the specified filename. It is presumed that the test file resides
		/// in the same directory as the calling assembly.
		/// </summary>
		public static string GetTestFilePath(string testFileName)
		{
			string codeBase = Assembly.GetCallingAssembly().CodeBase;
			Uri uri = new Uri(codeBase);
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

		public static bool LoadProject(string path)
		{
			Util.NewTestProject();
			return Project.Open(path);
		}

		public static void SaveAndReloadCurrentProject()
		{
			string savePath = Path.GetTempFileName();

			try
			{
				Project.Save(savePath);
				string saved1Xml = File.ReadAllText(savePath);

				TestingSupport.Util.NewTestProject();

				bool opened = Project.Open(savePath);
				Assert.IsTrue(opened);

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
			foreach (System.Windows.Forms.TreeNode node in nodes)
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

		private const string emptyDataSourceXml =
			"<datasources>" +
			"</datasources>";

		private static void initialize(string dataSourcesXml)
		{
			MethodInfo init = typeof(FieldProviders).GetMethod("initialize", BindingFlags.NonPublic | BindingFlags.Static);
			init.Invoke(null, new object[] { new XmlElement(dataSourcesXml) });
		}
	}


}
