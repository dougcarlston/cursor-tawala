using System.IO;
using Tawala.Projects;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class GetStatementInvalidForm468
    {
		private Process process;
		private IForm form1;
		private IForm form2;
		private FormList formList;

		private GetStatement statement;
		private GetLine line;

		[SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();

			form1 = Project.Current.AddForm();
			form2 = Project.Current.AddForm();
			process = Project.Current.AddProcess();

			formList = new FormList();
			formList.Add(form1);
			formList.Add(form2);

			statement = new GetStatement(new RecordSet("record list", formList));
			line = new GetLine(statement);
			process.Lines.Add(line);
		}


		[Test]
		public void NoValidFormReferencesProducesXmlWithNoForms()
		{
			string expectedBeforeXML =
				"<get recordList=\"record list\">\r\n" +
				"<forms>\r\n"+
				"<form name=\"Form 1\"/>\r\n" +
				"<form name=\"Form 2\"/>\r\n" +
				"</forms>\r\n" +
				"</get>";

			string expectedAfterRemoveXML =
				"<get recordList=\"record list\">\r\n" +
				"<forms>\r\n"+
				"</forms>\r\n" +
				"</get>";


			Assert.AreEqual(expectedBeforeXML, statement.ToXml());

			Project.Current.RemoveForm(form1.Name);
			Project.Current.RemoveForm(form2.Name);

			Assert.AreEqual(expectedAfterRemoveXML, statement.ToXml());
		}

		[Test]
		public void StatementWithOneOfTwoDeletedFormsDoesNotCauseBadSave()
		{
			Project.Current.RemoveForm(form1.Name);

			validateProjectXml();
		}

		[Test]
		public void StatementWithTwoOfTwoDeletedFormsDoesNotCauseBadSave()
		{
			Project.Current.RemoveForm(form1.Name);
			Project.Current.RemoveForm(form2.Name);

			validateProjectXml();
		}

		private void validateProjectXml()
		{
			string savedXml = Project.Current.ToXml();

			TestSupport.Util.NewTestProject();

			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms))
				{
					sw.Write(savedXml);
					sw.Flush();
					TawalaProjectConverter converter = new TawalaProjectConverter(ms);
					converter.ConvertXmlToProject();
				}
			}

			Assert.AreEqual(savedXml, Project.Current.ToXml());
		}
	}
}
