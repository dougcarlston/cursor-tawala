using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the Blank class
	/// </summary>
	[TestFixture]
	public class BlankTest
	{
		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();
		}

		[Test]
		public void Construct() 
		{
			var blank = new Blank(new FibItem(), 10);

			Assert.AreEqual(10, blank.Length);
			Assert.IsFalse(blank.Required);
			Assert.AreEqual("", blank.AlternateLabel);

			blank.Required = true;
			Assert.IsTrue(blank.Required);
		}

		[Test]
		public void ConstructFromXml()
		{
			const string xmlString = "<blank label=\"a\" length=\"10\" required=\"true\" alternateLabel=\"Alternate\"></blank>";

			var element = new XmlElement(xmlString);
			var blank = new Blank(element, new FibItem());

			Assert.AreEqual(10, blank.Length);
			Assert.IsTrue(blank.Required);
			Assert.AreEqual("Alternate", blank.AlternateLabel);
		}

		[Test]
		public void AlternateLabel()
		{
			var blank = new Blank(new FibItem(), 5);
			blank.AlternateLabel = "Foo";
			Assert.AreEqual("Foo", blank.AlternateLabel);
		}

		[Test]
		public void AlternateLabelWithWhitespace()
		{
			var blank = new Blank(new FibItem(), 6);
			blank.AlternateLabel = "   Foo   ";
			Assert.AreEqual("Foo", blank.AlternateLabel);
		}

		[Test]
		public void FieldName()
		{
			Project.NewTestProject();
			IForm form = Project.Current.AddForm();

			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			Assert.AreEqual("Q1:a", fibItem1.BlankList[0].FieldName);
		}

		[Test]
		public void FieldString()
		{
			Project.NewTestProject();
			IForm form = Project.Current.AddForm();

			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			Assert.AreEqual("<<Form 1:Q1:a>>", fibItem1.BlankList[0].FieldString);
		}

		[Test]
		public void OperatorDataSource()
		{
			Blank blank = new Blank(new FibItem(), 20);
			Assert.AreEqual(HybridOperator.List.DataSource, blank.OperatorDataSource);
		}

		[Test]
		public void GetText()
		{
			IForm form = Project.Current.AddForm();

			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			Assert.AreEqual("____________________", fibItem1.BlankList[0].Text);
		}

		[Test]
		public void GetDefaultXml()
		{
			IForm form = Project.Current.AddForm();

			FibItem fibItem1 = new FibItem();
			form.ItemList.Add(fibItem1);

			string expString =
				"<blank label=\"a\" length=\"20\" required=\"false\"></blank>";

			Assert.AreEqual(expString, fibItem1.BlankList[0].ToXml());
		}

		[Test]
		public void GetAlternateXml()
		{
			IForm form = Project.Current.AddForm();

			FibItem fibItem1 = new FibItem();
			fibItem1.BlankList[0].AlternateLabel = "Alternate";
			form.ItemList.Add(fibItem1);

			string expString =
                "<blank label=\"a\" length=\"20\" required=\"false\" alternateLabel=\"Alternate\"></blank>";

			Assert.AreEqual(expString, fibItem1.BlankList[0].ToXml());
		}

	}
}
