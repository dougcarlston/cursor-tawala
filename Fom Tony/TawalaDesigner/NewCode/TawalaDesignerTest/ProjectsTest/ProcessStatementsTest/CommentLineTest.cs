using System;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class CommentLineTest
	{
		private Process process;
		private CommentStatement statement;

		[SetUp]
		public void SetUp()
		{
            Util.NewTestProject();

			process = Project.Current.AddProcess();

			statement = new CommentStatement();
		}

		[Test]
		public void ConstructFromStatement()
		{
			CommentLine line = new CommentLine(statement);
			
			Assert.AreEqual(statement, line.Statement);
			Assert.AreEqual(false, line.SelectsGroup);
			Assert.AreEqual(true, line.IsSelectable);
			Assert.AreEqual(true, line.IsDeletable);
			Assert.AreEqual(true, line.CanInsertBefore);
		}

		[Test]
		public void ValidateLine()
		{
			CommentLine line = new CommentLine(statement);
			process.Lines.Add(line);
			process.Lines.ValidateLines();
			
			Assert.AreEqual(true, line.IsValid);
		}
	}
}
