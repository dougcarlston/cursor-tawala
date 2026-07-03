using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the Condition class
	/// </summary>
	[TestFixture]
	public class ConditionFromXmlTest
	{
		private IForm form;
		private Process process;
		private FibItem fibItem1;
		private FibItem fibItem2;
		private McqItem mcItem1;
		private McqItem mcItem2;
		private FormList forms;
		private RecordSet recordList1;
		private Record record1;

		/// <summary>
		/// This method executes before any test method
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
		}

		/// <summary>
		/// This method executes before each test method
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			// create clean project
			Project.NewTestProject();

			// create form
			form = Project.Current.AddForm();

			// create process
			process = Project.Current.AddProcess();

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			// create FIB and MC items ('Q1:a', 'Q2:a', 'Q3', 'Q4')
			fibItem1 = new FibItem();
			fibItem2 = new FibItem();
			mcItem1 = new McqItem();
			mcItem2 = new McqItem();

			// add items to form
			form.ItemList.Add(fibItem1);
			form.ItemList.Add(fibItem2);
			form.ItemList.Add(mcItem1);
			form.ItemList.Add(mcItem2);

			// create GET statement ('Get record list from Form 1')
			forms = new FormList();
			forms.Add(Project.Current.FormList[0]);
			recordList1 = new RecordSet("record list", forms);
			GetStatement getStatement = new GetStatement(recordList1);
			process.Lines.Add(new ProcessLineList(getStatement));

			// create FOR EACH statement ('For Each record in record list')
			record1 = new Record("record");
			ProcessLineList forEachLines1 = getForEachLines(recordList1, record1);
			process.Lines.Add(forEachLines1);

			process.MappedFields.Fields.Add(fibItem1);
			process.MappedFields.Fields.Add(fibItem2);
			process.MappedFields.Fields.Add(mcItem1);
			process.MappedFields.Fields.Add(mcItem2);
			process.MappedFields.Qualifiers.Add("record");
			process.MappedFields.Map();
		}

		private static ProcessLineList getForEachLines(RecordSet recordList, Record record)
		{
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			return forEachLines;
		}

		[Test]
		public void CheckProcess()
		{
			int i = 0;
			Assert.AreEqual("Get record list from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each record in record list", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}

		[Test]
		public void FibEqualsValue()
		{
			string xmlString =
				"<equals field=\"Q1:a\">\r\n" +
				"<string value=\"21\"/>\r\n" +
				"</equals>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			Condition condition = new FibCondition(element, process.Name);

			Assert.AreEqual("Form 1:Q1:a equals 21", condition.ToString());
		}

		[Test]
		public void QualifiedFibEqualsValue()
		{
			string xmlString =
				"<equals field=\"record:Q1:a\">\r\n" +
				"<string value=\"21\"/>\r\n" +
				"</equals>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			Condition condition = new FibCondition(element, process.Name);

			Assert.AreEqual("record:Form 1:Q1:a equals 21", condition.ToString());
		}

		[Test]
		public void FibEqualsFib()
		{
			string xmlString =
				"<equals field=\"Form 1:Q1:a\">\r\n" +
				"<string field=\"Q2:a\"/>\r\n" +
				"</equals>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			Condition condition = new FibCondition(element, process.Name);

			Assert.AreEqual("Form 1:Q1:a equals Form 1:Q2:a", condition.ToString());
		}


		[Test]
		public void QualifiedFibEqualsFib()
		{
			string xmlString =
				"<equals field=\"record:Q1:a\">\r\n" +
				"<string field=\"Q2:a\"/>\r\n" +
				"</equals>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			Condition condition = new FibCondition(element, process.Name);

			Assert.AreEqual("record:Form 1:Q1:a equals Form 1:Q2:a", condition.ToString());
		}

		[Test]
		public void FibEqualsQualifiedFib()
		{
			string xmlString =
				"<equals field=\"Q1:a\">\r\n" +
				"<string field=\"record:Q2:a\"/>\r\n" +
				"</equals>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			Condition condition = new FibCondition(element, process.Name);

			Assert.AreEqual("Form 1:Q1:a equals record:Form 1:Q2:a", condition.ToString());
		}

		[Test]
		public void QualifiedFibEqualsQualifiedFib()
		{
			string xmlString =
				"<equals field=\"record:Q1:a\">\r\n" +
				"<string field=\"record:Q2:a\"/>\r\n" +
				"</equals>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			Condition condition = new FibCondition(element, process.Name);

			Assert.AreEqual("record:Form 1:Q1:a equals record:Form 1:Q2:a", condition.ToString());
		}


		[Test]
		public void FibIsBlank()
		{
			string xmlString = "<isBlank field=\"Q1:a\"/>";

			IXmlElement element = new XmlElement(xmlString);
			Condition condition = new FibCondition(element, process.Name);

			Assert.AreEqual("Form 1:Q1:a is blank", condition.ToString());
		}

		[Test]
		public void QualifiedFibIsBlank()
		{
			string xmlString = "<isBlank field=\"record:Q1:a\"/>";

			IXmlElement element = new XmlElement(xmlString);
			Condition condition = new FibCondition(element, process.Name);

			Assert.AreEqual("record:Form 1:Q1:a is blank", condition.ToString());
		}

		[Test]
		public void MCEqualsValue()
		{
			string xmlString = "<mcEquals field=\"Form 1:Q3\" value=\"a\"/>";

			IXmlElement element = new XmlElement(xmlString);
			Condition condition = new MCValueCondition(element, process.Name);

			Assert.AreEqual("Form 1:Q3 equals a", condition.ToString());
		}

		[Test]
		public void MCEqualsMC()
		{
			string xmlString =
				"<mcEquals field=\"Form 1:Q3\">\r\n" +
				"<string field=\"Form 1:Q4\"/>\r\n" +
				"</mcEquals>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			Condition condition = new MCFieldCondition(element, process.Name);

			Assert.AreEqual("Form 1:Q3 equals Form 1:Q4", condition.ToString());
		}

	}
}
