using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Tawala.Common;
using Tawala.Projects;
using Tawala.Functions.Runtime;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectXmlRoundTripTest
{
	public class TestBase
	{
		private string projectFilesDir = null;
		private string testDirectory = null;

		protected string ProjectFilesDirectory
		{
			get { return projectFilesDir; }
		}

		protected string TestDirectory
		{
			get { return testDirectory; }
		}
		
		protected void Init(string projectFiles)
		{
			string directory = Directory.GetParent(Util.GetTestFilePath("foo")).Parent.Parent.FullName;
			projectFilesDir = Path.Combine(directory, projectFiles);

			testDirectory = Path.Combine(projectFilesDir, "test");

			if (!Directory.Exists(testDirectory))
			{
				Directory.CreateDirectory(testDirectory);
			}
		}

		private string expectedFormat = null; 

		protected void RoundTripProject(string projectName)
		{
			long before = DateTime.Now.Ticks;

			if (expectedFormat == null)
			{
				string formatVersion = GetFormatVersionWithOneDecimalPlaceFromFolderName();
				if (appearsToHaveTwoDecimalPlaces(formatVersion))
				{
					formatVersion = GetFormatVersionWithTwoDecimalPlacesFromFolderName();				
				}
				expectedFormat = string.Format(" format=\"{0}\"", formatVersion);
			}

			string path = Path.Combine(ProjectFilesDirectory, projectName);

			using (StreamReader sr = File.OpenText(path))
			{
				char[] buffer = new char[200];
				sr.ReadBlock(buffer, 0, 200);
				StringBuilder sb = new StringBuilder();
				sb.Append(buffer);
				string beginning = sb.ToString();
				Assert.IsTrue(beginning.Contains(expectedFormat));
			}


			Project.Open(path);
			Util.SaveAndReloadCurrentProject();

			long after = DateTime.Now.Ticks;

			double time = (after - before) / 10000000.0;

			Console.WriteLine("{0} seconds to roundtrip project {1}", time, projectName);
		}

		private string GetFormatVersionWithOneDecimalPlaceFromFolderName()
		{
			return getEndCharactersOfFolderName(3);
		}

		private string GetFormatVersionWithTwoDecimalPlacesFromFolderName()
		{
			return getEndCharactersOfFolderName(4);
		}

		private string getEndCharactersOfFolderName(int numberOfChars)
		{
			return ProjectFilesDirectory.Substring(ProjectFilesDirectory.Length - numberOfChars, numberOfChars);
		}

		private static bool appearsToHaveTwoDecimalPlaces(string formatVersion)
		{
			return formatVersion[0] == '.';
		}
	}
}
