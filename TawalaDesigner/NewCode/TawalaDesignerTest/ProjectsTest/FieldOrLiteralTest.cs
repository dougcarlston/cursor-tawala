using System;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the FieldOrLiteral class
	/// </summary>
    [Ignore("REQUIRES Fix-ups for new classes")]
    [TestFixture]
	public class FieldOrLiteralTest
	{
#if FIXED
        private IForm form;
		private FibItem fibItem1;
		private McqItem mcItem1;
		private Process process;
		private FormList forms;
		private RecordSet recordList1;
		private Record record1;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
            Util.NewTestProject();

			// create form
			form = Project.Current.AddForm();

			// create process
			process = Project.Current.AddProcess();

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			// create FIB item in which one blank has an laternate label
			fibItem1 = new FibItem();
			fibItem1.Text = "__ __";
			fibItem1.BlankList[1].AlternateLabel = "Test String";

			// add new FIB item to form
			form.ItemList.Add(fibItem1);

			mcItem1 = new McqItem();
			form.ItemList.Add(mcItem1);

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
		public void Instantiate() 
		{ 
			FieldOrLiteral str = new FieldOrLiteral();
		
			Assert.IsNotNull(str);
			Assert.AreEqual(FieldOrLiteral.StringType.literal, str.Type);

			FieldOrLiteral str2 = new FieldOrLiteral("Test String", FieldOrLiteral.StringType.field);

			Assert.IsNotNull(str2);
			Assert.AreEqual("Test String", str2.Text);
			Assert.AreEqual(FieldOrLiteral.StringType.field, str2.Type);
		}

		[Test]
		public void ConstructLiteral()
		{
			FieldOrLiteral fol = new FieldOrLiteral();
			fol.Type = FieldOrLiteral.StringType.literal;
			fol.Text = "Test String";

			Assert.AreEqual("Test String", fol.Text);
			Assert.AreEqual(FieldOrLiteral.StringType.literal, fol.Type);
		}

		[Test]
		public void ConstructField()
		{
			FieldOrLiteral fol = new FieldOrLiteral();
			fol.Type = FieldOrLiteral.StringType.field;
			fol.Text = "Q1:a";

			Assert.AreEqual("Form 1:Q1:a", fol.Text);
			Assert.AreEqual(FieldOrLiteral.StringType.field, fol.Type);
		}

		[Test]
		public void ConstructCompound()
		{
			FieldOrLiteral fol = new FieldOrLiteral();
			fol.Type = FieldOrLiteral.StringType.compound;
			fol.Text = "Q1:a + 5";

			Assert.AreEqual("Q1:a + 5", fol.Text);
			Assert.AreEqual(FieldOrLiteral.StringType.compound, fol.Type);
		}

		[Test]
		public void ConstructRecordField()
		{
			Record record = new Record("Record");
			RecordField recordField = new RecordField(record, fibItem1.BlankList[0]);
			FieldOrLiteral fol = new FieldOrLiteral(recordField);

			Assert.AreEqual(1, fol.Expression.Elements.Count);
			Assert.AreEqual(FieldOrLiteral.StringType.field, fol.Type);
			Assert.AreEqual("Record:Form 1:Q1:a", fol.Text);
		}

		[Test]
		public void GetText() 
		{
			FieldOrLiteral str = new FieldOrLiteral();
			str.Text = "Test String";

			// type should have no effect on the text itself
			str.Type = FieldOrLiteral.StringType.field;
			Assert.AreEqual("Test String", str.Text);

			// type should have no effect on the text itself
			str.Type = FieldOrLiteral.StringType.literal;
			Assert.AreEqual("Test String", str.Text);
		}

		[Test]
		public void GetString()
		{
			FieldOrLiteral str = new FieldOrLiteral();
			str.Text = "Test String";

			// field should have no formatting
			str.Type = FieldOrLiteral.StringType.field;
			Assert.AreEqual("Test String", str.ToString());

			// literal text has quotes
			str.Type = FieldOrLiteral.StringType.literal;
			Assert.AreEqual("\"Test String\"", str.ToString());

			// numeric has no quotes
			str.Text = "21";
			Assert.AreEqual("21", str.ToString());
		}

		[Test]
		public void GetXml()
		{
			FieldOrLiteral fol = new FieldOrLiteral(new RecordField(record1, mcItem1));

			string expString = "<string field=\"record:Form 1:Q2\"/>\r\n";

			Assert.AreEqual(expString, fol.ToXml());

		}
#endif
	}
	
}