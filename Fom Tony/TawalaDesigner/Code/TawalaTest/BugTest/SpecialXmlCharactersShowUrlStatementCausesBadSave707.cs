using System;
using Tawala.Projects;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
    [TestFixture]
	public class SpecialXmlCharactersShowUrlStatementCausesBadSave707
    {
		private const string testString = "www.\"<This\'&\'That>\".com";

		[Test]
		public void SpecialXmlCharsInBlankAlternateLabelDoNotCauseBadSave()
		{
			TestSupport.Util.NewTestProject();

			Process process = Project.Current.AddProcess();
			ShowUrlStatement statement = new ShowUrlStatement(testString);
			process.Lines.Add(new ShowUrlLine(statement));

			try
			{
				Util.SaveAndReloadCurrentProject();
			}
			catch (Exception e)
			{
				Assert.Fail(e.ToString());
			}

			Process reloadedProcess = Project.Current.ProcessList[0];
			statement = reloadedProcess.Lines[0].Statement as ShowUrlStatement;
			Assert.AreEqual(testString, statement.Url);
		}
	}
}
