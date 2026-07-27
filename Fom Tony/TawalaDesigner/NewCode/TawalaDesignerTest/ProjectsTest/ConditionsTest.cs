using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
	public class ConditionsTest
	{
#if FIXED
		private IForm form;
		private Process process;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
            Util.NewTestProject();

			// create form
			form = Project.Current.AddForm();

			// create process
			process = Project.Current.AddProcess();

			// connect process to form
			Project.Current.ConnectProcessToForm(process, form.Name);

			// create FIB items
			FibItem fibItem1 = new FibItem();
			FibItem fibItem2 = new FibItem();

			// add new FIB items to form
			form.ItemList.Add(fibItem1);
			form.ItemList.Add(fibItem2);

			// add fields to Project's field mapper
			process.MappedFields.Fields.Add(((FibItem)form.ItemList[0]).BlankList[0]);
			process.MappedFields.Fields.Add(((FibItem)form.ItemList[1]).BlankList[0]);
			process.MappedFields.Map();
		}

		[Test]
		public void SingleConditionFromXml() 
		{
			string xmlString =
				" <conditions>" +
				"  <equals field=\"Q1:a\">" +
				"   <string value=\"One\"/>" +
				"  </equals>" +
				" </conditions>";

			IXmlElement element = new XmlElement(xmlString);
			Conditions conditions = new Conditions(element, process.Name);
			Assert.AreEqual("Form 1:Q1:a equals \"One\"", conditions.ToString());

			// assert that blank and condition reference same field
			Assert.AreSame(((FibItem)form.ItemList[0]).BlankList[0], (((Condition)conditions[0]).Field));
		}

		[Test]
		public void TwoConditionsFromXml()
		{
			string xmlString =
				"<conditions>" +
				" <and>" +
				"  <equals field=\"Q1:a\">" +
				"   <string value=\"One\"/>" +
				"  </equals>" +
				"  <equals field=\"Q2:a\">" +
				"   <string value=\"Two\"/>" +
				"  </equals>" +
				" </and>" +
				"</conditions>";

			IXmlElement element = new XmlElement(xmlString);
			Conditions conditions = new Conditions(element, process.Name);

			Assert.AreEqual(3, conditions.Count);
			Assert.AreEqual("Form 1:Q1:a equals \"One\" AND Form 1:Q2:a equals \"Two\"", conditions.ToString());

			// assert that blanks and conditions reference same field
			Assert.AreSame(((FibItem)form.ItemList[0]).BlankList[0], (((Condition)conditions[0]).Field));
			Assert.AreSame(((FibItem)form.ItemList[1]).BlankList[0], (((Condition)conditions[2]).Field));
		}

		[Test]
		public void ThreeConditionsFromXml()
		{
			string xmlString =
				"<conditions>" +
				" <or>" +
				"  <and>" +
				"   <equals field=\"Q1:a\">" +
				"    <string value=\"One\"/>" +
				"   </equals>" +
				"   <equals field=\"Q2:a\">" +
				"    <string value=\"Two\"/>" +
				"   </equals>" +
				"  </and>" +
				"  <equals field=\"Q1:a\">" +
				"   <string value=\"Three\"/>" +
				"  </equals>" +
				" </or>" +
				"</conditions>";

			IXmlElement element = new XmlElement(xmlString);
			Conditions conditions = new Conditions(element, process.Name);

			Assert.AreEqual(5, conditions.Count);
			Assert.AreEqual("Form 1:Q1:a equals \"One\" AND Form 1:Q2:a equals \"Two\" OR Form 1:Q1:a equals \"Three\"", conditions.ToString());

			// assert that blanks and conditions reference same field
			Assert.AreSame(((FibItem)form.ItemList[0]).BlankList[0], (((Condition)conditions[0]).Field));
			Assert.AreSame(((FibItem)form.ItemList[1]).BlankList[0], (((Condition)conditions[2]).Field));
		}
#endif
	}
}
