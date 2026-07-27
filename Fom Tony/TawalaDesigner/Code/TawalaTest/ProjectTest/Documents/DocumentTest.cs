using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Tawala.Projects.Components;
using Tawala.Projects.Documents;

using TawalaTest.TestSupport;

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
			ComponentMaker.UseNewComponents(true);
			document = ComponentMaker.MakeDocumentObject("My Document");
		}

		[TearDown]
		public void TearDown()
		{
			ComponentMaker.UseNewComponents(false);
		}

		[Test]
		public void DocumentToStringReturnsName()
		{
			Assert.AreEqual("My Document", document.ToString());
		}
	}
}
