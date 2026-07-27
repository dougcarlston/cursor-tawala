using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
	public class GetStatementTest
	{
#if FIXED
		private IForm form1;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
            Util.NewTestProject();

			// create forms
			form1 = Project.Current.AddForm();
			Project.Current.AddForm();
			Project.Current.AddForm();
		}

		[Test]
		public void Instantiate()
		{
			GetStatement statement = new GetStatement();

			// Assertions
			Assert.IsNotNull(statement);
		}

		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
				"<get recordList=\"Record Set\">" +
				"<forms>" +
				"<form name=\"Form 1\"/>" +
				"<form name=\"Form 3\"/>" +
				"</forms>" +
				"</get>";

			IXmlElement element = new XmlElement(xmlString);
			GetStatement getStatement = new GetStatement(element, new Process("Process 1"));

			Assert.AreEqual("Get", getStatement.Name);
			Assert.AreEqual("Record Set", getStatement.Records.FieldName);
			Assert.AreEqual(2, getStatement.Records.Forms.Count);
			Assert.AreSame(Project.Current.FormList[0], getStatement.Records.Forms[0]);
			Assert.AreSame(Project.Current.FormList[2], getStatement.Records.Forms[1]);
		}

		[Test]
		public void ConstructFromXmlWithSingleCondition()
		{
			Process process = Project.Current.AddProcess();
			
			FibItem fibItem1 = new FibItem();
			fibItem1.BlankList[0].AlternateLabel = "First Name";
			form1.ItemList.Add(fibItem1);

			string xmlString =
				"<get recordList=\"Record Set\">" +
				"<forms>" +
				"<form name=\"Form 1\"/>" +
				"<form name=\"Form 3\"/>" +
				"</forms>" +
				"<conditions>\r\n" +
				"<isBlank field=\"Form 1:First Name\" />\r\n" +
				"</conditions>" +
				"</get>";

			IXmlElement element = new XmlElement(xmlString);
			GetStatement getStatement = new GetStatement(element, process.Name);

			Assert.AreEqual("Form 1:First Name is blank", getStatement.Conditions.ToString());
			Assert.AreEqual("Get Record Set from Form 1, Form 3 where Form 1:First Name is blank", getStatement.ToString());
		}

		[Test]
		public void Name()
		{
			GetStatement statement = new GetStatement();

			Assert.AreEqual("Get", statement.Name);
		}

		[Test]
		public void RecordListName()
		{
			FormList forms = new FormList();
			forms.Add(form1);
			GetStatement statement = new GetStatement(new RecordSet("record list", forms));

			Assert.AreEqual("Get", statement.Name);
			Assert.IsNotNull(statement.Records);
			Assert.AreEqual("record list", statement.Records.FieldName);
		}

		[Test]
		public void FormList()
		{
			FormList forms = new FormList();
			forms.Add(new NewForm("Form 1"));
			GetStatement statement = new GetStatement(new RecordSet("record list", forms));

			Assert.IsNotNull(statement.Records.Forms);
			Assert.AreEqual(1, statement.Records.Forms.Count);
		}

		[Test]
		public void GetXml()
		{
			FormList forms = new FormList();
			forms.Add(form1);
			GetStatement statement = new GetStatement(new RecordSet("record list", forms));

			string expectedXML =
				"<get recordList=\"record list\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"</get>";

			Assert.AreEqual(expectedXML, statement.ToXml());
		}

		[Test]
		public void GetXmlWithSingleCondition()
		{
			FormList forms = new FormList();
			forms.Add(form1);
			
			FibItem fibItem1 = new FibItem();
			Blank blank = fibItem1.BlankList[0];
			blank.AlternateLabel = "First Name";
			form1.ItemList.Add(fibItem1);

			GetStatement statement = new GetStatement(new RecordSet("record list", forms));
			statement.Conditions = new Conditions(blank, HybridOperator.List[HybridOperator.Ops.isBlank]);

			string expectedXML =
				"<get recordList=\"record list\">\r\n" +
				"<forms>\r\n" +
				"<form name=\"Form 1\"/>\r\n" +
				"</forms>\r\n" +
				"<conditions>\r\n" +
				"<isBlank field=\"Form 1:First Name\" />\r\n" +
				"</conditions>" +
				"</get>";

			Assert.AreEqual(expectedXML, statement.ToXml());
		}
#endif
	}
}
