using System;
using NUnit.Framework;
using Tawala.Common;
using Tawala.Projects;
using Tawala.Projects.Components;
using Tawala.Projects.Documents;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	[TestFixture]
	public class ProjectAddComponentDuplicateName
	{
		// execute this once at beginning of tests
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Project.NewTestProject();
		}

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void AddFormDuplicateNameTest()
		{
			// add a form 
			IForm form1 = Project.Current.AddForm();

			// create a second form with the same name as the first form's default name
			// paste it in, which forces a "copy of" name
			IForm form2 = new Form("Form 1");
			Project.Current.PasteForm(form2);
			Assert.AreEqual("Copy of Form 1", form2.Name);

			// now rename the second form to the next default default name
			Project.Current.RenameForm("Copy of Form 1", "Form 2");
			Assert.AreEqual("Form 2", form2.Name);

			// now add a new form and make sure it's default name is unique
			IForm form3 = Project.Current.AddForm();
			Assert.AreEqual("Form 3", form3.Name);
		}

		[Test]
		public void AddProcessDuplicateNameTest()
		{
			// same as above Form test, but with Processes
			Process proc1 = Project.Current.AddProcess();

			Process proc2 = new Process("Process 1");
			Project.Current.PasteProcess(proc2);
			Assert.AreEqual("Copy of Process 1", proc2.Name);

			// now rename the second form to the next default default name
			Project.Current.RenameProcess("Copy of Process 1", "Process 2");
			Assert.AreEqual("Process 2", proc2.Name);

			// now add a new form and make sure it's default name is unique
			Process proc3 = Project.Current.AddProcess();
			Assert.AreEqual("Process 3", proc3.Name);
		}

		[Test]
		public void AddDocumentDuplicateNameTest()
		{
			// same as above Form test, but with Processes
			IDocument doc1 = Project.Current.AddDocument();

			IDocument doc2 = ComponentMaker.MakeDocumentObject("Document 1");
			Project.Current.PasteDocument(doc2);
			Assert.AreEqual("Copy of Document 1", doc2.Name);

			// now rename the second form to the next default default name
			Project.Current.RenameDocument("Copy of Document 1", "Document 2");
			Assert.AreEqual("Document 2", doc2.Name);

			// now add a new form and make sure it's default name is unique
			IDocument doc3 = Project.Current.AddDocument();
			Assert.AreEqual("Document 3", doc3.Name);
		}
	}
}
