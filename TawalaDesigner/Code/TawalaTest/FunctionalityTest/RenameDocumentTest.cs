using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing validity when renaming Documents
	/// </summary>
	[TestFixture]
	public class RenameDocumentTest
	{
		private IForm form1;
		private IForm form2;
		private IDocument document1;
		private IDocument document2;
		private Process process;

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

			// and some documents
			document1 = Project.Current.AddDocument();
			document2 = Project.Current.AddDocument();

			// and a process
			process = Project.Current.AddProcess();
		}

		[Test]
		public void DuplicateDocumentName()
		{
			// remember document 2's original name
			string oldDoc2Name = document2.Name;

			// try renaming second one to a duplicate name
			Assert.AreEqual(false, Project.Current.RenameDocument(document2.Name, document1.Name));
			Assert.AreEqual(oldDoc2Name, document2.Name);

			// now try a legal name
			Assert.AreEqual(true, Project.Current.RenameDocument(document2.Name, "Document 2 Renamed"));
			Assert.AreEqual("Document 2 Renamed", document2.Name);
		}

		[Test]
		public void DuplicateDocumentNameWithWhitespace()
		{
			// name the Documents to something unique
			document1.Name = "Unique Name";
			document2.Name = "Second Document Name";

			// try renaming second one to a duplicate name, except with whitespace
			Assert.AreEqual(false, Project.Current.RenameDocument("Second Document Name", "   Unique Name  "), "   Unique Name   should be invalid");
			Assert.AreEqual("Second Document Name", document2.Name);
		}

		[Test]
		public void NameWithLeadingUnderscores()
		{
			// name with a single underscore should be OK
			Assert.AreEqual(true, Project.Current.RenameDocument(document1.Name, "_Name"), "_Name should be valid");

			// double leading underscores denote reserved labels
			Assert.AreEqual(false, Project.Current.RenameDocument(document1.Name, "__Name"), "__Name should be invalid");
		}

		[Test]
		public void NameWithNumericCharacters()
		{
			// names with numeric + text is ok
			Assert.AreEqual(true, Project.Current.RenameDocument(document1.Name, "Label1"), "Label1 should be valid");
			Assert.AreEqual(true, Project.Current.RenameDocument(document1.Name, "1Label"), "1Label should be valid");

			// numerics only are a no-no
			Assert.AreEqual(false, Project.Current.RenameDocument(document1.Name, "1"), "1 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameDocument(document1.Name, "42.33"), "42.33 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameDocument(document1.Name, ".5"), ".5 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameDocument(document1.Name, "-.5"), "-.5 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameDocument(document1.Name, "5."), "5. should be invalid");
			Assert.AreEqual(false, Project.Current.RenameDocument(document1.Name, "-5."), "5. should be invalid");
		}

		[Test]
		public void NameContainsColon()
		{
			// names that contain a colon are illegal
			Assert.AreEqual(false, Project.Current.RenameDocument(document1.Name, "Label:1"), "Label:1 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameDocument(document1.Name, ":Label1"), ":Label1 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameDocument(document1.Name, "Label1:"), "Label1: should be invalid");
		}
	}
}
