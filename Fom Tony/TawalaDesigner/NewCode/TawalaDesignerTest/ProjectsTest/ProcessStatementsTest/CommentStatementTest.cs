using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the CommentStatement class
	/// </summary>
	[TestFixture]
	public class CommentStatementTest
	{
		[Test]
		public void Construct()
		{
			CommentStatement statement = new CommentStatement();

			Assert.AreEqual("Comment", statement.Name);
		}

		[Test]
		public void ConstructText()
		{
			CommentStatement statement = new CommentStatement("Here is a comment.");
			
			Assert.AreEqual("Here is a comment.", statement.Text);
		}

		[Test]
		public void Text()
		{
			CommentStatement statement = new CommentStatement();
			statement.Text = "Here is a comment.";

			Assert.AreEqual("Here is a comment.", statement.Text);
		}

		[Test]
		public void ModifyText()
		{
			CommentStatement statement = new CommentStatement("Here is a comment.");
			Assert.AreEqual("Here is a comment.", statement.Text);

			statement.Text = "Changed comment";
			Assert.AreEqual("Changed comment", statement.Text);
		}

		[Test]
		public void GetString()
		{
			CommentStatement statement = new CommentStatement("Here is a comment.");
			Assert.AreEqual("Here is a comment.", statement.ToString());
		}

		[Test]
		public void GetXml()
		{
			CommentStatement statement = new CommentStatement("Here is a different comment.");
			Assert.AreEqual("<comment>Here is a different comment.</comment>", statement.ToXml());
		}

		[Test]
		public void EscapeTextInXml()
		{
			CommentStatement statement = new CommentStatement("Here &is a comment with <special> characters.");
			Assert.AreEqual("<comment>Here &amp;is a comment with &lt;special&gt; characters.</comment>", statement.ToXml());
		}

		[Test]
		public void ConstructFromXml()
		{
			XmlElement element = new XmlElement("<comment>Your text here.</comment>");
			CommentStatement statement = new CommentStatement(element);
			Assert.AreEqual("Comment", statement.Name);
			Assert.AreEqual("Your text here.", statement.Text);
		}
	}
}
