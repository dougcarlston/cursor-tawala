using System;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;
using Tawala.Projects;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class SpaceBetweenFieldsInChoiceNotPreserved914
	{
		private static readonly string mcqWithSpaceBetweenFieldsInChoice =
			@"<mc label=""Q2"" onlyone=""true"" required=""false""" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
			@"<question>" +
			@"<paragraph indent=""0"" align=""left"">" + XmlConstants.DefaultTabsXml +
			XmlConstants.FullBeginFont + "Question text" + XmlConstants.EndFont +
			@"</paragraph>" +
			@"</question>" +
			@"<choice label=""a"">" +
			@"<paragraph indent=""0"" align=""left"">" + XmlConstants.DefaultTabsXml +
			XmlConstants.FullBeginFont +
			@"<field name=""Form 1:Q1:a""/>" +
			XmlConstants.EndFont +
			@"<sp/>" +
			XmlConstants.FullBeginFont +
			@"<field name=""Form 1:Q1:a""/>" +
			XmlConstants.EndFont +
			@"</paragraph>" +
			@"</choice>" +
			@"</mc>" + Environment.NewLine;

		[Test]
		public void SetupTestMethod()
		{
			Util.NewTestProject();
			IForm form = Project.Current.AddForm();
			var fibItem = new FibItem();
			form.ItemList.Add(fibItem);

			var mcqItem = new McqItem(new XmlElement(mcqWithSpaceBetweenFieldsInChoice));

			Assert.AreEqual(mcqWithSpaceBetweenFieldsInChoice, mcqItem.ToXml("Q2"));
		}

	}
}
