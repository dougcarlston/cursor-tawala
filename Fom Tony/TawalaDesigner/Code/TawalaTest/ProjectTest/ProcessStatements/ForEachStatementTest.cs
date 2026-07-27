//#define ForEachQuestionStatement
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the ForEachStatement class
	/// </summary>
	[TestFixture]
	public class ForEachStatementTest
	{
		private Process process;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			// create clean project
			Project.NewTestProject();

			Project.Current.AddForm();
			process = Project.Current.AddProcess();

			FormList recordSetForms = new FormList();
			recordSetForms.Add((Form)Project.Current.FormList[0]);
			GetStatement getStatement = new GetStatement(new RecordSet("Record Set", recordSetForms));

			if (!process.RecordSets.Contains(getStatement.Records))
			{
				process.RecordSets.Add(getStatement.Records);
			}

		}

		[Test]
		public void ConstructForEachRecord()
		{
			ForEachRecordStatement statement = new ForEachRecordStatement();
			Assert.AreEqual("For Each", statement.Name);
			Assert.AreEqual(typeof(ForEachStatement), statement.GetStatementType());
		}

#if ForEachQuestionStatement
		[Test]
		public void ConstructForEachQuestion()
		{
			ForEachQuestionStatement statement = new ForEachQuestionStatement();
			Assert.AreEqual("For Each", statement.Name);
			Assert.AreEqual(typeof(ForEachStatement), statement.GetStatementType());
		}
#endif

		[Test]
		public void ConstructForEachRecordFromXml()
		{
			string xmlString =
				"<foreach record=\"Record\" recordList=\"Record Set\">" +
				"</foreach>";

			IXmlElement element = new XmlElement(xmlString);
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(element, "Process 1");

			Assert.AreEqual("For Each", forEachStatement.Name);
			Assert.AreEqual("Record", forEachStatement.Record.FieldName);
			Assert.AreEqual("Record Set", forEachStatement.RecordList.FieldName);

			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual(1, forEachStatement.RecordList.Forms.Count);
			Assert.AreSame(Project.Current.FormList[0], forEachStatement.RecordList.Forms[0]);
		}

#if ForEachQuestionStatement
		[Test]
		public void ConstructForEachQuestionFromXml()
		{
			string xmlString =
				"<forEachMc>" +
				"</forEachMc>";

			IXmlElement element = new XmlElement(xmlString);
			ForEachQuestionStatement forEachStatement = new ForEachQuestionStatement(element, process.Name);

			Assert.AreEqual("For Each", forEachStatement.Name);
		}
#endif
		[Test]
		public void RecordAndRecordListNames()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			ForEachRecordStatement statement = new ForEachRecordStatement(new Record("Record Name"), new RecordSet("Record List Name", forms));

			Assert.AreEqual("Record Name", statement.Record.FieldName);
			Assert.AreEqual("Record List Name", statement.RecordList.FieldName);
		}

		[Test]
		public void GetForEachRecordXml()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			ForEachRecordStatement statement = new ForEachRecordStatement(new Record("Record Name"), new RecordSet("Record List Name", forms));

			string expectedXML = "<foreach record=\"Record Name\" recordList=\"Record List Name\">";

			Assert.AreEqual(expectedXML, statement.ToXml());
		}

		[Test]
		public void GetForEachRecordText()
		{
			FormList forms = new FormList();
			forms.Add(new Form("Form 1"));
			ForEachRecordStatement statement = new ForEachRecordStatement(new Record("Record Name"), new RecordSet("Record List Name", forms));

			string expectedText = "For Each Record Name in Record List Name";

			Assert.AreEqual(expectedText, statement.ToString());
		}

#if ForEachQuestionStatement
		[Test]
		public void GetForEachQuestionXml()
		{
			ForEachQuestionStatement statement = new ForEachQuestionStatement();

			string expectedXML = "<forEachMc>";

			Assert.AreEqual(expectedXML, statement.ToXml());
		}

		[Test]
		public void GetForEachQuestionText()
		{
			ForEachQuestionStatement statement = new ForEachQuestionStatement();
			Assert.AreEqual("For Each Multiple Choice Question", statement.ToString());
		}
#endif
	}
}
