using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Processes;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class RecordSetNamerTest
	{
		[SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();
		}
	
		[Test]
		public void FirstNameGenerated()
		{
			Assert.AreEqual("Record List 1", RecordSetNamer.GetNextName());
		}

		[Test]
		public void SecondNameGenerated()
		{
			Assert.AreEqual("Record List 1", RecordSetNamer.GetNextName());
			Assert.AreEqual("Record List 1", RecordSetNamer.GetNextName());
		}

		[Test]
		public void NameBasedOnProcessContents()
		{
			Process process = Project.Current.AddProcess();
			addGetLine(process, "Record List 1");

			Assert.AreEqual(1, process.Lines.Count);
			Assert.AreEqual("Record List 2", RecordSetNamer.GetNextName());
		}

		[Test]
		public void AddThenRemove()
		{
			Process process = Project.Current.AddProcess();
			addGetLine(process, "Record List 1");

			Assert.AreEqual("Record List 2", RecordSetNamer.GetNextName());

			process.Lines.RemoveAt(0);

			Assert.AreEqual("Record List 1", RecordSetNamer.GetNextName());
		}

		[Test]
		public void NameBasedOnMultipleProcessContents()
		{
			Process process1 = Project.Current.AddProcess();
			addGetLine(process1, "Record List 1");

			Process process2 = Project.Current.AddProcess();
			addGetLine(process2, "Record List 2");

			Assert.AreEqual(1, process1.Lines.Count);
			Assert.AreEqual(1, process2.Lines.Count);

			Assert.AreEqual("Record List 3", RecordSetNamer.GetNextName());
		}

		private void addGetLine(Process process, string recordSetName)
		{
			RecordSet rs = new RecordSet(recordSetName, new FormList());
			GetStatement gs = new GetStatement(rs);
			process.Lines.Add(new GetLine(gs));
		}


	}
}
