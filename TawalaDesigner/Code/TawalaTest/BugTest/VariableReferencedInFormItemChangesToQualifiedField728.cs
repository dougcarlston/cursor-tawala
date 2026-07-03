using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class VariableReferencedInFormItemChangesToQualifiedField728
	{
		[Test]
		public void TextItemReferencingVariableGeneratesExpectedXml()
		{
			Util.NewTestProject();

			IForm form1 = Project.Current.AddForm();
			IFibItem fibItem = new FibItem();
			fibItem.BlankList[0].AlternateLabel = "MyData";
			form1.ItemList.Add(fibItem);

			Process process = Project.Current.AddProcess();
			process.Variables.AddUnique("MyData");

			string textItemReferencingVariable =
				@"<text label=""T1""" + XmlConstants.DefaultTextItemStyleAttribute + @">" +
				@"<paragraph indent=""0"" align=""left"">" +
				XmlConstants.DefaultTabsXml +
				XmlConstants.DefaultBeginFont + @"Blah: " + XmlConstants.EndFont +
				XmlConstants.DefaultBeginFont + @"<field name=""MyData""/>" + XmlConstants.EndFont +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine;

			ITextItem textItem = new TextItem(new XmlElement(textItemReferencingVariable));

			Assert.AreEqual(textItemReferencingVariable, textItem.ToXml("T1"));
		}
	}
}