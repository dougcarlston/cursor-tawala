using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class ComponentMakerTest
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void ComponentMakerCanProduceNewFormFromXml()
		{
			string xmlString =
				@"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" +
				@"<items>" + Environment.NewLine +
				@"<fib label=""Q1"" style=""topLabels"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"[Replace this with your question. Underscores create blanks.] <blank label=""a"" length=""20"" " +
				@"required=""false""/>" +
				@"</paragraph>" +
				@"</fib>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>";

			ComponentMaker.UseNewComponents(true);

			IForm form = ComponentMaker.MakeFormObject(new XmlElement(xmlString));

			Assert.IsInstanceOfType(typeof(Form), form);
		}

		[Test]
		public void ComponentMakerCanProduceNewFormWithSpecificName()
		{
			ComponentMaker.UseNewComponents(true);

			IForm form = ComponentMaker.MakeFormObject("New Form");

			Assert.IsInstanceOfType(typeof(Form), form);
			Assert.AreEqual("New Form", form.Name);
		}

		[Test]
		public void ComponentMakerCanProduceNewDocumentFromXml()
		{
			string xmlString =
				@"<document name=""Document 1"">" + Environment.NewLine +
				@"<xmlData>" + Environment.NewLine +
				@"<paragraph indent=""0"" align=""left""><tabPositions><tabStop position=""2880""/></tabPositions><font>Test " +
				@"</font><font><b>document.</b></font></paragraph>" + Environment.NewLine +
				@"</xmlData>" + Environment.NewLine +
				@"</document>";

			ComponentMaker.UseNewComponents(true);

			IDocument document = ComponentMaker.MakeDocumentObject(new XmlElement(xmlString));

			Assert.IsInstanceOfType(typeof(NewDocument), document);
		}

		[Test]
		public void ComponentMakerCanProduceNewDocumentWithSpecificName()
		{
			ComponentMaker.UseNewComponents(true);

			IDocument document = ComponentMaker.MakeDocumentObject("New Document");

			Assert.IsInstanceOfType(typeof(NewDocument), document);
			Assert.AreEqual("New Document", document.Name);
		}
	}
}
