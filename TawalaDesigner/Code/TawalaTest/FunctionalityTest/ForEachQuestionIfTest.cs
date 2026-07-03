using System;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.FunctionalityTest
{
#if false
	[TestFixture]
	public class ForEachQuestionIfTest
	{
		private IForm form;
		private Process process;
		private FormList forms;
		private RecordSet recordList1;
		private Record record1;

		// execute this before each test method runs
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

			// add new FIB item to form
			form.ItemList.Add(new FibItem());

			// add new MC item to form
			form.ItemList.Add(new MCItem());

			// create GET statement ('Get record list from Form 1')
			forms = new FormList();
			forms.Add((Form)Project.Current.FormList[0]);
			recordList1 = new RecordSet("Record Set", forms);
			GetStatement getStatement = new GetStatement(recordList1);
			process.Lines.Add(new ProcessLineList(getStatement));

			// create FOR EACH RECORD statement ('For Each record in record list')
			record1 = new Record("Record");
			ProcessLineList forEachRecordLines = getForEachRecordLines(recordList1, record1);
			process.Lines.Add(forEachRecordLines);

			// create FOR EACH QUESTION statement ('For Each Multiple Choice Question')
			ProcessLineList forEachQuestionLines = getForEachQuestionLines();
			process.Lines.Insert(3, forEachQuestionLines);

			// create IF statement ('If (selection) equals Record:(selection)')
			ProcessLineList ifLines = getIfLines();
			process.Lines.Insert(5, ifLines);
		}

		private static ProcessLineList getForEachRecordLines(RecordSet recordList, Record record)
		{
			ForEachRecordStatement forEachStatement = new ForEachRecordStatement(record, recordList);
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			return forEachLines;
		}

		private static ProcessLineList getForEachQuestionLines()
		{
			ForEachQuestionStatement forEachStatement = new ForEachQuestionStatement();
			ProcessLineList forEachLines = new ProcessLineList(forEachStatement);
			return forEachLines;
		}

		private ProcessLineList getIfLines()
		{
			// create process line 'If (selection) equals Record:(selection)'
			IField field = new Field("(selection)");
			Expression expression = new Expression(new QualifiedFieldList(record1, new Field("(selection)")));
			IfStatement ifStatement = new IfStatement();
			ifStatement.Conditions = new Conditions(field, MCOneOperator.List[MCOneOperator.Ops.mcEquals], expression);
			ProcessLineList ifLines = new ProcessLineList(ifStatement);
			return ifLines;
		}


		[Test]
		public void CheckProcess()
		{
			int i = 0;
			Assert.AreEqual("Get Record Set from Form 1", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Record in Record Set", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("For Each Multiple Choice Question", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual("If (selection) equals Record:(selection)", process.Lines[i++].ToString());
			Assert.AreEqual("(", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
			Assert.AreEqual(")", process.Lines[i++].ToString());
		}

		[Test]
		public void GetXml()
		{
			string expString =
				"<if>\r\n" +
				"<conditions>\r\n" +
				"<mcEquals field=\"(selection)\">\r\n" +
				"<string field=\"Record:(selection)\"/>\r\n" +
				"</mcEquals>\r\n" +
				"</conditions>";

			Assert.AreEqual(expString, process.Lines[5].Statement.ToXml());
		}



	}
#endif
}
