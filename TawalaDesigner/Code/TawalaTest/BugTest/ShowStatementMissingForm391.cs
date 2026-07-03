using Tawala.Projects;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class ShowStatementMissingForm391
    {
		private Process process;

		[SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();

			process = Project.Current.AddProcess();
		}

		[Test]
		public void RemovingFormProducesNullFormInShowStatement()
		{
			IForm form = Project.Current.AddForm();

			ShowFormStatement statement = new ShowFormStatement(form);
			ShowFormLine line = new ShowFormLine(statement);
			process.Lines.Add(line);

			process.Lines.ValidateLines();
			Assert.AreEqual(true, line.IsValid);
			Assert.AreSame(form, statement.Form);
			Assert.AreEqual("Show Form Form 1", line.ToString());

			Project.Current.RemoveForm("Form 1");

			Assert.AreSame(Form.NULL, statement.Form);
			Assert.AreEqual("Show Form Null Form", line.ToString());
		}

		[Test]
		public void NullFormXmlProducesNullFormXml()
		{
			string xmlString =
				"<show form=\"Null Form\"/>";

			IXmlElement element = new XmlElement(xmlString);
			ShowFormStatement statement = new ShowFormStatement(element, process);

			Assert.AreEqual(xmlString, statement.ToXml());
		}
	}
}
