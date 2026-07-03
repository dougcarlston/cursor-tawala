using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing validity when renaming Forms
	/// </summary>
	[TestFixture]
	public class RenameFormTest
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

			// add a Process and a Document
			process = Project.Current.AddProcess();
			document = Project.Current.AddDocument();
		}

		[Test]
		public void DuplicateFormName()
		{
			// remember document 2's original name
			string oldForm2Name = form2.Name;

			// try renaming second one to a duplicate name
			Assert.AreEqual(false, Project.Current.RenameForm(form2.Name, form1.Name));
			Assert.AreEqual(oldForm2Name, form2.Name);

			// now try a legal name
			Assert.AreEqual(true, Project.Current.RenameForm(form2.Name, "Form 2 Renamed"));
			Assert.AreEqual("Form 2 Renamed", form2.Name);
		}

		[Test]
		public void DuplicateFormNameWithWhitespace()
		{
			// name the Forms to something unique
			form1.Name = "Unique Name";
			form2.Name = "Second Form Name";

			// try renaming second one to a duplicate name, except with whitespace
			Assert.AreEqual(false, Project.Current.RenameForm("Second Form Name", "   Unique Name  "), "   Unique Name   should be invalid");
			Assert.AreEqual("Second Form Name", form2.Name);
		}

		[Test]
		public void NameWithLeadingUnderscores()
		{
			// name with a single underscore should be OK
			Assert.AreEqual(true, Project.Current.RenameForm(form1.Name, "_Name"), "_Name should be valid");

			// double leading underscores denote reserved labels
			Assert.AreEqual(false, Project.Current.RenameForm(form1.Name, "__Name"), "__Name should be invalid");
		}

		[Test]
		public void NameWithNumericCharacters()
		{
			// names with numeric + text is ok
			Assert.AreEqual(true, Project.Current.RenameForm(form1.Name, "Label1"), "Label1 should be valid");
			Assert.AreEqual(true, Project.Current.RenameForm(form1.Name, "1Label"), "1Label should be valid");

			// numerics only are a no-no
			Assert.AreEqual(false, Project.Current.RenameForm(form1.Name, "1"), "1 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameForm(form1.Name, "42.33"), "42.33 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameForm(form1.Name, ".5"), ".5 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameForm(form1.Name, "-.5"), "-.5 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameForm(form1.Name, "5."), "5. should be invalid");
			Assert.AreEqual(false, Project.Current.RenameForm(form1.Name, "-5."), "5. should be invalid");
		}

		[Test]
		public void NameContainsColon()
		{
			// names that contain a colon are illegal
			Assert.AreEqual(false, Project.Current.RenameForm(form1.Name, "Label:1"), "Label:1 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameForm(form1.Name, ":Label1"), ":Label1 should be invalid");
			Assert.AreEqual(false, Project.Current.RenameForm(form1.Name, "Label1:"), "Label1: should be invalid");
		}
	}
}
