using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Tawala.Projects;
using Tawala.Projects.Documents;

using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest.Documents
{
	[TestFixture]
	public class DocumentTest
	{
		private IDocument document;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			document = ComponentMaker.MakeDocumentObject("My Document");
		}

		[Test]
		public void DocumentToStringReturnsName()
		{
			Assert.AreEqual("My Document", document.ToString());
		}
	}
}
