using System;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class SpecialXmlCharactersInSubjectBoxCauseBadSave711
	{
		private Process process;
		private IDocument document;

		[SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();

			Project.Current.AddForm();

			process = Project.Current.AddProcess();
			document = Project.Current.AddDocument();
		}

		private const string subjectWithSpecialCharacters = "\"<This & That>\"";

		[Test]
		public void SpecialXmlCharactersInSubjectBoxAreRetainedUponSaveAndReload()
		{
			SendStatement sendStatement = createSendStatementWithSpecialCharacters();
			process.Lines.Add(new SendLine(sendStatement));

			Util.SaveAndReloadCurrentProject();

			SendStatement reloadedSendStatement = Project.Current.ProcessList[0].Lines[0].Statement as SendStatement;

			Assert.AreEqual(subjectWithSpecialCharacters, reloadedSendStatement.Subject);

		}

		private SendStatement createSendStatementWithSpecialCharacters()
		{
			SendStatement sendStatement = new SendStatement();
			sendStatement.AddressTo = new FieldOrLiteral("jdf@tawala.com", FieldOrLiteral.StringType.literal);
			sendStatement.SendBody = new SendDocumentBody(document);
			sendStatement.Subject = subjectWithSpecialCharacters;

			return sendStatement;
		}
	}
}
