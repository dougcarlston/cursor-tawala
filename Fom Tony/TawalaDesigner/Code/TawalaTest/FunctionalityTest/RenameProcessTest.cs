using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing validity when renaming Processes
	/// </summary>
	[TestFixture]
	public class RenameProcessTest
	{
		private IForm form1;
		private IForm form2;
		private Process process1;
		private Process process2;
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

			// and some processes and a document
			process1 = Project.Current.AddProcess();
			process2 = Project.Current.AddProcess();
			document = Project.Current.AddDocument();
		}

		[Test]
		public void DuplicateProcessName()
		{
			// remember process 2's original name
			string oldProc2Name = process2.Name;

			// try renaming second one to a duplicate name
			Assert.AreEqual(false, Project.Current.RenameProcess(process2.Name, process1.Name));
			Assert.AreEqual(oldProc2Name, process2.Name);

			// now try a legal name
			Assert.AreEqual(true, Project.Current.RenameProcess(process2.Name, "Process 2 Renamed"));
			Assert.AreEqual("Process 2 Renamed", process2.Name);
		}

		[Test]
		public void DuplicateProcessNameWithWhitespace()
		{
			// name the Processs to something unique
			process1.Name = "Unique Name";
			process2.Name = "Second Process Name";

			// try renaming second one to a duplicate name, except with whitespace
			Assert.AreEqual(false, Project.Current.RenameProcess("Second Process Name", "   Unique Name  "), "   Unique Name   should be invalid");
			Assert.AreEqual("Second Process Name", process2.Name);
		}

		[Test]
		public void NameWithLeadingUnderscores()
		{
			// name with a single underscore should be OK
			Assert.AreEqual(true, Project.Current.RenameProcess(process1.Name, "_Name"), "_Name should be valid");

			// double leading underscores denote reserved labels
			Assert.AreEqual(false, Project.Current.RenameProcess(process1.Name, "__Name"), "__Name should be invalid");
		}

		[Test]
		public void NameWithNumericCharacters()
		{
			// names with numeric + text is ok
			Assert.AreEqual(true, Project.Current.RenameProcess(process1.Name, "Label1"), "Label1 should be valid");
			Assert.AreEqual(true, Project.Current.RenameProcess(process1.Name, "1Label"), "1Label should be valid");

			// numerics only are a no-no
			Assert.AreEqual(false, Project.Current.RenameProcess(process1.Name, "1"), "1 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameProcess(process1.Name, "42.33"), "42.33 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameProcess(process1.Name, ".5"), ".5 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameProcess(process1.Name, "-.5"), "-.5 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameProcess(process1.Name, "5."), "5. should be invalid");
			Assert.AreEqual(false, Project.Current.RenameProcess(process1.Name, "-5."), "5. should be invalid");
		}

		[Test]
		public void NameContainsColon()
		{
			// names that contain a colon are illegal
			Assert.AreEqual(false, Project.Current.RenameProcess(process1.Name, "Label:1"), "Label:1 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameProcess(process1.Name, ":Label1"), ":Label1 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameProcess(process1.Name, "Label1:"), "Label1: should be invalid");
		}
	}
}
