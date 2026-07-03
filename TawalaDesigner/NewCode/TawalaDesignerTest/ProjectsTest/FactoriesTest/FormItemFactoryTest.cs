using System;
using NUnit.Framework;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Factories;
using TawalaTest.TestingSupport;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest.FactoriesTest
{
	[TestFixture]
	public class FormItemFactoryTest
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void CanMakeFormWithFibItemFromXml()
		{
			string formXml =
				@"<form name=""Form 1"" startPoint=""true"">" +
				@"<items>" + Environment.NewLine +
				@"<fib label=""Q1"" style=""topLabels"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"[Replace this with your question. Underscores create blanks.] <blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
				@"</paragraph>" +
				@"</fib>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>";

			IForm form = new Form(new XmlElement(formXml));

			addFormToProject(form);

			Assert.AreEqual(formXml, form.ToXml());
		}

		[Test]
		public void CanMakeFormWithTextItemFromXml()
		{
			string formXml =
				@"<form name=""Form 1"" startPoint=""true"">" +
				@"<items>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">[Replace this with text of your own.]</paragraph>" +
				@"</text>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>";

			IForm form = new Form(new XmlElement(formXml));
			addFormToProject(form);

			Assert.AreEqual(formXml, form.ToXml());
		}

		[Test]
		public void CanMakeFormWithHeadingItemFromXml()
		{
			string formXml =
				@"<form name=""Form 1"" startPoint=""true"">" +
				@"<items>" + Environment.NewLine +
				@"<heading label=""H1"" alternateLabel=""alternate"" type=""Sub"">" +
				@"<paragraph indent=""0"" align=""left"">[Replace this with heading of your own.]</paragraph>" +
				@"</heading>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>";

			IForm form = new Form(new XmlElement(formXml));
			addFormToProject(form);

			Assert.AreEqual(formXml, form.ToXml());
		}

		[Test]
		public void CanMakeFormWithMcqItemFromXml()
		{
			string formXml =
				@"<form name=""Form 1"" startPoint=""true"">" +
				@"<items>" + Environment.NewLine +
				@"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">[Replace this with your question. Use Enter key to add " +
				@"choices below.]</paragraph>" +
				@"</question>" +
				@"<choice label=""a"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">One</font>" +
				@"</choice>" +
				@"<choice label=""b"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">Two</font>" +
				@"</choice>" +
				@"</mc>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>";

			IForm form = new Form(new XmlElement(formXml));
			addFormToProject(form);

			Assert.AreEqual(formXml, form.ToXml());
		}

		[Test]
		public void CanMakeFormWithHiddenFieldXml()
		{
			string formXml =
				@"<form name=""Form 1"" startPoint=""true"">" +
				@"<items>" + Environment.NewLine +
				@"<field name=""Field1""/>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>";

			IForm form = new Form(new XmlElement(formXml));

			addFormToProject(form);

			Assert.AreEqual(formXml, form.ToXml());
		}

		[Test]
		public void CanMakeFormWithSkipToFibItemFromXml()
		{
			string formXml =
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<skipInstructions>" + Environment.NewLine +
				@"<skip to=""Q1""/>" + Environment.NewLine +
				@"</skipInstructions>" + Environment.NewLine +
				@"<fib label=""Q1"" style=""topLabels""><paragraph indent=""0"" align=""left"">[Replace this with " +
				@"your question. Underscores create blanks.] <blank label=""a"" length=""20"" height=""1"" " +
				@"required=""false""/></paragraph></fib>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>";

			IForm form = new Form(new XmlElement(formXml));

			addFormToProject(form);

			form.ItemList.ResolveFieldReferences();

			Assert.AreEqual(formXml, form.ToXml());
		}

		[Test]
		public void CanMakeFormWithSkipToMcqItemFromXml()
		{
			string formXml =
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<skipInstructions>" + Environment.NewLine +
				@"<skip to=""Q1""/>" + Environment.NewLine +
				@"</skipInstructions>" + Environment.NewLine +
				@"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"[Replace this with your question. Use Enter key to add choices below.]" +
				@"</font>" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a""></choice>" +
				@"</mc>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>";

			IForm form = new Form(new XmlElement(formXml));

			addFormToProject(form);

			form.ItemList.ResolveFieldReferences();

			Assert.AreEqual(formXml, form.ToXml());
		}

		[Test]
		public void CanMakeFormWithSkipToTextItemFromXml()
		{
			string formXml =
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<skipInstructions>" + Environment.NewLine +
				@"<skip to=""T1""/>" + Environment.NewLine +
				@"</skipInstructions>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal""><paragraph indent=""0"" align=""left"">[Replace this with text of your own.]</paragraph></text>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>";

			IForm form = new Form(new XmlElement(formXml));

			addFormToProject(form);

			form.ItemList.ResolveFieldReferences();

			Assert.AreEqual(formXml, form.ToXml());
		}

	
		private static void addFormToProject(IForm form)
		{
			Reflect<Project>.GetField<FormList>("formList", Project.Current).Add(form);
		}

	}
}
