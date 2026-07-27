using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NUnit.Framework;
using Tawala.FormDesigner;
using TawalaTest.TestSupport;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest.ModelTest
{
	[TestFixture]
	public class HeadingItemTest
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();

			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new NewHiddenField());
		}

		[Test]
		public void CanConstructHeadingItemFromFieldXml()
		{
			string inputXml =
				"<heading label=\"H1\" type=\"Main\">" + 
				"<paragraph indent=\"0\" align=\"left\">" +
				"Before<field name=\"Form 1:Field1\"/>After" +
				"</paragraph>" + 
				"</heading>" + Environment.NewLine;

			IHeadingItem headingItem = new NewHeadingItem(new XmlElement(inputXml, true));

			Assert.AreEqual(inputXml, headingItem.ToXml("H1"));
		}
	}
}
