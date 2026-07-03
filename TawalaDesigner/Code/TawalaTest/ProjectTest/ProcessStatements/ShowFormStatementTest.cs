using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the ShowFormStatement class
	/// </summary>
	[TestFixture]
	public class ShowFormStatementTest
	{
		[Test]
		public void Construct()
		{
			ShowFormStatement statement = new ShowFormStatement();

			Assert.IsNull(statement.Form);
			Assert.IsNull(statement.Document);
		}

		[Test]
		public void ConstructWithForm()
		{
			ShowFormStatement statement = new ShowFormStatement(new Form("MyForm"));
			Assert.IsNotNull(statement.Form);
			Assert.AreEqual("MyForm", statement.Form.Name);
			Assert.AreEqual("Show Form MyForm", statement.ToString());
			Assert.IsNull(statement.Document);
		}

		[Test]
		public void ConstructFromXml()
		{
			Project.NewTestProject();

			IForm form = Project.Current.AddForm();

			string xmlString =
				"<show form=\"Form 1\"/>";

			IXmlElement element = new XmlElement(xmlString);
			ShowFormStatement statement = new ShowFormStatement(element, "No Process");

			Assert.AreSame(form, statement.Form);
		}

		[Test]
		public void Name() 
		{
			ShowFormStatement statement = new ShowFormStatement();
			Assert.AreEqual("Show", statement.Name);
		} 

		[Test]
		public void GetXml()
		{
			//Form form = new Form("Form 1");
			Project.NewTestProject();
			IForm form = Project.Current.AddForm();
			
			ShowFormStatement statement = new ShowFormStatement(form);

			string expString = "<show form=\"Form 1\"/>";

			//Assertions 
			Assert.AreEqual(expString, statement.ToXml());

			// check for illegal XML characters
			form.Name = "&<Form's \"Bad\" Name>";
			expString = "<show form=\"&amp;&lt;Form&apos;s &quot;Bad&quot; Name&gt;\"/>";
			Assert.AreEqual(expString, statement.ToXml());
		}

		[Test]
		public void Copy()
		{
			ShowFormStatement statement1 = new ShowFormStatement(new Form("Form 1"));

			ShowFormStatement statement2 = (ShowFormStatement)statement1.Copy();

			Assert.AreNotSame(statement1, statement2);
			Assert.AreSame(statement1.Form, statement2.Form);
		}
	}
}
