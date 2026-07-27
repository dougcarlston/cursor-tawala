using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using TawalaTest.TestSupport;


namespace TawalaTest.BugTest
{
	[TestFixture]
	public class DeletingFormReferencedByFunctionBug572
	{
		[Test]
		public void DeletedFormsBecomeNullForms()
		{
			loadAndTestProject("Bug572", xmlRemovedForm);
		}

		[Test]
		public void FunctionBlankParameters()
		{
			loadAndTestProject("Bug572-FunctionBlank", xmlFunctionBlank);
		}

		[Test]
		public void FunctionMCItemParameters()
		{
			loadAndTestProject("Bug572-FunctionMCQ", xmlFunctionMCQ);
		}

		[Test]
		public void FunctionHiddenFieldParameters()
		{
			loadAndTestProject("Bug572-FunctionHidden", xmlFunctionHidden);
		}

		#region Private

		private void loadAndTestProject(string name, string expectedXml)
		{
			string fileName = name + ".tawala";
			Util.LoadProject(Util.GetTestFilePath(fileName));

			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual(1, Project.Current.DocumentList.Count);

			Project.Current.RemoveForm(Project.Current.FormList[0].Name);

			string savedFile = Path.GetTempFileName();

			try
			{
				Project.Save(savedFile);
				string savedXml = Util.StripProjectHeader(File.ReadAllText(savedFile));
				Assert.AreEqual(expectedXml, savedXml);

				Util.LoadProject(savedFile);
				Project.Save(savedFile);
				savedXml = Util.StripProjectHeader(File.ReadAllText(savedFile));
				Assert.AreEqual(expectedXml, savedXml);
			}
			finally
			{
				File.Delete(savedFile);
			}
		}

	
		private static string xmlRemovedForm =
			"<pageHeader></pageHeader>" +
			"<documents>" + Environment.NewLine +
			"<document name=\"Document 1\">" + Environment.NewLine +
			"<xmlData>" + Environment.NewLine +
			"<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font><record-count version=\"3\">" +
			"<form-name>Null Form</form-name><conditions><form name=\"Null Form\" /></conditions></record-count></font></paragraph>" + Environment.NewLine +
			"</xmlData>" + Environment.NewLine +
			"</document>" + Environment.NewLine +
			"</documents>" + Environment.NewLine +
			"</project>" + Environment.NewLine;

		private static readonly string xmlFunctionBlank =
			"<pageHeader></pageHeader>" +
			"<documents>" + Environment.NewLine +
			"<document name=\"Document 1\">" + Environment.NewLine +
			"<xmlData>" + Environment.NewLine +
			"<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font><sum " +
			"version=\"1\"><field>Record:Unknown Field</field><conditions><form name=\"Null Form\" " +
			"/></conditions></sum></font></paragraph>" + Environment.NewLine +
			"</xmlData>" + Environment.NewLine +
			"</document>" + Environment.NewLine +
			"</documents>" + Environment.NewLine +
			"</project>" + Environment.NewLine;

		private static readonly string xmlFunctionMCQ =
			"<pageHeader></pageHeader>" +
			"<documents>" + Environment.NewLine +
			"<document name=\"Document 1\">" + Environment.NewLine +
			"<xmlData>" + Environment.NewLine +
			"<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop " +
			"position=\"2880\"/></tabPositions><font><choice-tally-table version=\"1\"><field>Record:Unknown " +
			"Field</field><conditions><form name=\"Null Form\" /></conditions></choice-tally-table></font></paragraph>" + Environment.NewLine +
			"</xmlData>" + Environment.NewLine +
			"</document>" + Environment.NewLine +
			"</documents>" + Environment.NewLine +
			"</project>" + Environment.NewLine;

		private static readonly string xmlFunctionHidden =
			"<pageHeader></pageHeader>" +
			"<documents>" + Environment.NewLine +
			"<document name=\"Document 1\">" + Environment.NewLine +
			"<xmlData>" + Environment.NewLine +
			"<paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font><sum " +
			"version=\"1\"><field>Record:Unknown Field</field><conditions><form name=\"Null Form\" " +
			"/></conditions></sum></font></paragraph>" + Environment.NewLine +
			"</xmlData>" + Environment.NewLine +
			"</document>" + Environment.NewLine +
			"</documents>" + Environment.NewLine +
			"</project>" + Environment.NewLine;


		#endregion
	}
}
