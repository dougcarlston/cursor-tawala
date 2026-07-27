using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing validity of variable names in the Process
	/// </summary>
	[TestFixture]
	public class ProcessVariableNameTest
	{
		private IForm form1;
		private IForm form2;
		private Process process;
		private IDocument document;

		// execute this once at beginning of tests
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
		}

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			// create clean project
			Project.NewTestProject();

			// create a form with a FIB item
			form1 = Project.Current.AddForm();
			form1.ItemList.Add(new FibItem());

			// create another form with an MC item
			form2 = Project.Current.AddForm();
			form2.ItemList.Add(new McqItem());

			// create a process
			process = Project.Current.AddProcess();

			// and a document
			document = Project.Current.AddDocument();
		}

		[Test]
		public void VariableNameDuplicatesAlternateLabel()
		{
			// add an alternate label to the FIB blank in form 1
			((FibItem)form1.ItemList[0]).BlankList[0].AlternateLabel = "Name";

			// and an alternate label to the FIB blank in form 1
			((McqItem)form2.ItemList[0]).AlternateLabel = "Choice";

			// should be able name variables the same as Form Item labels
			Assert.AreEqual(true, process.ValidVariableName("Name"));
			Assert.AreEqual(true, process.ValidVariableName("Choice"));
			Assert.AreEqual(true, process.ValidVariableName("SomethingElse"));
		}

		[Test]
		public void VariableNameWithLeadingUnderscores()
		{
			// name with a single underscore should be OK
			Assert.AreEqual(true, process.ValidVariableName("_Name"));

			// double leading underscores denote reserved labels
			Assert.AreEqual(false, process.ValidVariableName("__Name"));
		}

		[Test]
		public void VariableNameWithNumericCharacters()
		{
			// names with numeric + text is ok
			Assert.AreEqual(true, process.ValidVariableName("Label1"));
			Assert.AreEqual(true, process.ValidVariableName("1Label"));

			// numerics only are a no-no
			Assert.AreEqual(false, process.ValidVariableName("1"), "1 should be invalid");
			Assert.AreEqual(false, process.ValidVariableName("42.33"), "42.33 should be invalid");
			Assert.AreEqual(false, process.ValidVariableName(".5"), ".5 should be invalid");
			Assert.AreEqual(false, process.ValidVariableName("-.5"), "-.5 should be invalid");
			Assert.AreEqual(false, process.ValidVariableName("5."), "5. should be invalid");
			Assert.AreEqual(false, process.ValidVariableName("-5."), "5. should be invalid");
		}

		[Test]
		public void VariableNameContainsColon()
		{
			// names that contain a colon are illegal
			Assert.AreEqual(false, process.ValidVariableName("Label:1"));
			Assert.AreEqual(false, process.ValidVariableName(":Label1"));
			Assert.AreEqual(false, process.ValidVariableName("Label1:"));
		}

		[Test]
		public void VariableNameInDefaultLabelFormat()
		{
			// names that look like default Form Item labels are illegal
			Assert.AreEqual(false, process.ValidVariableName("Q14"));
			Assert.AreEqual(false, process.ValidVariableName("q2"));
			Assert.AreEqual(false, process.ValidVariableName("T9987563"));
			Assert.AreEqual(false, process.ValidVariableName("t121"));

			// these look close, but are legal constructs
			Assert.AreEqual(true, process.ValidVariableName("Q14s"));
			Assert.AreEqual(true, process.ValidVariableName("t"));
		}

		[Test]
		public void VariableNameWithWhitespace()
		{
			// add an alternate label to the FIB blank in form 1
			((FibItem)form1.ItemList[0]).BlankList[0].AlternateLabel = "Name";

			// leading/trailing whitespace should be ignored when comparing names
			Assert.AreEqual(true, process.ValidVariableName("  Name   "));
		}

        [Test]
        public void VariableNameValidAsRecordNameIfRecordNameAlreadyExists()
        {
            // Get record list from Form 1
            FormList forms = new FormList();
            forms.Add((Form)Project.Current.FormList[0]);
            RecordSet recordList1 = new RecordSet("record list", forms);
            GetStatement getStatement1 = new GetStatement(recordList1);
            process.Lines.Add(new ProcessLineList(getStatement1));
            process.RecordSets.AddUnique("record list");

            // For Each record in record list
            Record record1 = new Record("record1");
            ForEachRecordStatement forEachStatement1 = new ForEachRecordStatement(record1, recordList1);
            ProcessLineList forEachLines1 = new ProcessLineList(forEachStatement1);
            process.Lines.Add(forEachLines1);
            process.Records.AddUnique("record1");

            Assert.IsTrue(process.ValidRecordVariableName("record1"));
            Assert.IsFalse(process.ValidVariableName("record1"));
            Assert.IsFalse(process.ValidRecordVariableName("record list"));
        }

		[Test]
		public void QualifiedVariableName()
		{
			// Get record list from Form 1
			FormList forms = new FormList();
			forms.Add((Form)Project.Current.FormList[0]);
			RecordSet recordList1 = new RecordSet("record list", forms);
			GetStatement getStatement1 = new GetStatement(recordList1);
			process.Lines.Add(new ProcessLineList(getStatement1));
			process.RecordSets.AddUnique("record list");

			// Get record list 2 from Form 1
			RecordSet recordList2 = new RecordSet("record list 2", forms);
			GetStatement getStatement2 = new GetStatement(recordList2);
			process.Lines.Add(new ProcessLineList(getStatement2));
			process.RecordSets.AddUnique("record list 2");

			// For Each record in record list
			Record record1 = new Record("record1");
			ForEachRecordStatement forEachStatement1 = new ForEachRecordStatement(record1, recordList1);
			ProcessLineList forEachLines1 = new ProcessLineList(forEachStatement1);
			process.Lines.Add(forEachLines1);
			process.Records.AddUnique("record1");

			// For Each record2 in record list 2 - inserted at line 4, within the above ForEach block
			Record record2 = new Record("record2");
			ForEachRecordStatement forEachStatement2 = new ForEachRecordStatement(record2, recordList2);
			ProcessLineList forEachLines2 = new ProcessLineList(forEachStatement2);
			process.Lines.Insert(4, forEachLines2);
			process.Records.AddUnique("record2");

			// check validity outside both ForEach blocks
			Assert.AreEqual(false, process.ValidVariableName("record1:Form 1:Score", 0));
			Assert.AreEqual(false, process.ValidVariableName("record2:Form 1:Score", 0));

			// within the outermost block
			Assert.AreEqual(true, process.ValidVariableName("record1:Form 1:Score", 4));
			Assert.AreEqual(false, process.ValidVariableName("record2:Form 1:Score", 4));

			// within the innermost block
			Assert.AreEqual(true, process.ValidVariableName("record1:Form 1:Score", 6));
			Assert.AreEqual(true, process.ValidVariableName("record2:Form 1:Score", 6));

			// check some illegal names
			Assert.AreEqual(false, process.ValidVariableName("record3:Form 1:Score", 6));
			Assert.AreEqual(false, process.ValidVariableName("record1:Q1:a", 6));
			Assert.AreEqual(false, process.ValidVariableName("record2:Form 1:__Score", 6));
			Assert.AreEqual(false, process.ValidVariableName("record2:Document 1", 6));
			Assert.AreEqual(false, process.ValidVariableName("record2:", 6));
			Assert.AreEqual(false, process.ValidVariableName("record1", 0));
			Assert.AreEqual(false, process.ValidVariableName("record list 2", 0));
		}
	}
}
