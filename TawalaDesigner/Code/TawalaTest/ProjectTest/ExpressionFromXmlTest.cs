using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the Expression class
	/// </summary>
	[TestFixture]
	public class ExpressionFromXmlTest
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

		}

		private static ProcessLineList getForEachLines(RecordSet recordList, Record record)
		{
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			return forEachLines;
		}

		[Test]
		public void Value()
		{
			string xmlString = "<string value=\"21\"/>";

			IXmlElement element = new XmlElement(xmlString);
			Expression expression = new ValueExpression(element);

			Assert.AreEqual("21", expression.ToString());
		}

		[Test]
		public void FibField()
		{
			string xmlString = "<string field=\"Q1:a\"/>";

			IXmlElement element = new XmlElement(xmlString);
			Expression expression = new FieldExpression(element, process.GetValidFields(0));

			Assert.AreEqual("<<Form 1:Q1:a>>", expression.ToString());
			Assert.AreEqual(1, expression.Elements.Count);
			Assert.IsTrue(expression.HasSingleFieldElement);
		}

		[Test]
		public void MCField()
		{
			string xmlString = "<string field=\"Form 1:Q3\"/>";

			IXmlElement element = new XmlElement(xmlString);
			Expression expression = new FieldExpression(element, process.GetValidFields(0));

			Assert.AreEqual("<<Form 1:Q3>>", expression.ToString());
			Assert.AreEqual(1, expression.Elements.Count);
			Assert.IsTrue(expression.HasSingleFieldElement);
		}

		[Test]
		public void SimpleAddition()
		{
			string xmlString = 
				"<add>" +
				"<operand field=\"Q1:a\"/>" +
				"<operand value=\"1\"/>" +
				"</add>";

			IXmlElement element = new XmlElement(xmlString);

			IXmlElement currentElement = element.GetChild(0);
			Assert.AreEqual("Q1:a", currentElement.GetAttribute("field"));

			currentElement = element.GetChild(1);
			Assert.AreEqual("1", currentElement.GetAttribute("value"));

			Expression expression = new ArithmeticExpression(element, process.GetValidFields(0));

			Assert.AreEqual("<<Form 1:Q1:a>> + 1", expression.ToString());
		}

		[Test]
		public void MultiplyAndAdd()
		{
			string xmlString =
				"<add>" +
				"<mul>" +
				"<operand field=\"Q1:a\"/>" +
				"<operand value=\"10\"/>" +
				"</mul>" +
				"<operand value=\"1\"/>" +
				"</add>";

			IXmlElement element = new XmlElement(xmlString);

			Expression expression = new ArithmeticExpression(element, process.GetValidFields(0));

			Assert.AreEqual("(<<Form 1:Q1:a>> * 10) + 1", expression.ToString());
		}
		[Test]
		public void CompoundExpressionFromParentElement()
		{
			process.MappedFields.Fields.Add(fibItem1);
			process.MappedFields.Map();

			string xmlString =
				"<set field=\"Variable\">" +
				"<string field=\"Q1:a\"/>" +
				"<string value=\" \"/>" +
				"<string field=\"Q1:a\"/>" +
				"</set>";

			IXmlElement element = new XmlElement(xmlString);

			Expression expression = new CompoundExpression(element, process.Name);

			Assert.AreEqual("<<Form 1:Q1:a>> <<Form 1:Q1:a>>", expression.ToString());
		}

	}
}
