using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing validity of Document names for use as the appended document in an Append statement
	/// </summary>
	[TestFixture]
	public class AppendDocumentNameTest
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
		public void NameWithLeadingUnderscores()
		{
			// name with a single underscore should be OK
			Assert.AreEqual(true, Project.Current.ValidDocumentNameForAppendStatement("_Name"));

			// double leading underscores denote reserved labels
			Assert.AreEqual(false, Project.Current.ValidDocumentNameForAppendStatement("__Name"));
		}

		[Test]
		public void NameWithNumericCharacters()
		{
			// names with numeric + text is ok
			Assert.AreEqual(true, Project.Current.ValidDocumentNameForAppendStatement("Label1"));
			Assert.AreEqual(true, Project.Current.ValidDocumentNameForAppendStatement("1Label"));

			// numerics only are a no-no
			Assert.AreEqual(false, Project.Current.ValidDocumentNameForAppendStatement("1"), "1 should be invalid");
			Assert.AreEqual(false, Project.Current.ValidDocumentNameForAppendStatement("42.33"), "42.33 should be invalid");
			Assert.AreEqual(false, Project.Current.ValidDocumentNameForAppendStatement(".5"), ".5 should be invalid");
			Assert.AreEqual(false, Project.Current.ValidDocumentNameForAppendStatement("-.5"), "-.5 should be invalid");
			Assert.AreEqual(false, Project.Current.ValidDocumentNameForAppendStatement("5."), "5. should be invalid");
			Assert.AreEqual(false, Project.Current.ValidDocumentNameForAppendStatement("-5."), "5. should be invalid");
		}

		[Test]
		public void NameContainsColon()
		{
			// names that contain a colon are illegal
			Assert.AreEqual(false, Project.Current.ValidDocumentNameForAppendStatement("Label:1"));
			Assert.AreEqual(false, Project.Current.ValidDocumentNameForAppendStatement(":Label1"));
			Assert.AreEqual(false, Project.Current.ValidDocumentNameForAppendStatement("Label1:"));
		}

		[Test]
		public void ExistingDocumentName()
		{
			document.Name = "DocName";

			// we should be able use an existing virtual document name for an Append
			Assert.AreEqual(true, Project.Current.ValidDocumentNameForAppendStatement("DocName"));
		}

		[Test]
		public void ExistingVirtualDocumentName()
		{
			// create a virtual document
			Project.Current.GetRealOrVirtualDocument("VirtualDoc", true);

			// we should be able use an existing virtual document name for an Append
			Assert.AreEqual(true, Project.Current.ValidDocumentNameForAppendStatement("VirtualDoc"));
		}
	}
}
