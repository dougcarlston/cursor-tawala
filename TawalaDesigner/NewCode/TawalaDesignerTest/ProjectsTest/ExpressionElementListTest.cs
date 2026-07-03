using System;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
	public class ExpressionElementListTest
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
		}

		[Test]
		public void Field()
		{
			ExpressionElementList list = new ExpressionElementList();

			// create field from FIB item and blank index
			FibItem fibItem = new FibItem();

			// add FIB item to form
			form.ItemList.Add(fibItem);

			list.Add(new FieldElement(fibItem.BlankList[0]));

			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("<<Form 1:Q1:a>>", list[0].ToString());
			Assert.AreEqual("<<Form 1:Q1:a>>", list.ToString());
		}

		[Test]
		public void String()
		{
			ExpressionElementList list = new ExpressionElementList();

			list.Add(new StringElement("2"));

			Assert.AreEqual(1, list.Count);
			Assert.AreEqual("2", list[0].ToString());
			Assert.AreEqual("2", list.ToString());
		}

		[Test]
		public void FieldAndString()
		{
			ExpressionElementList list = new ExpressionElementList();

			// create field from FIB item and blank index
			FibItem fibItem = new FibItem();

			// add FIB item to form
			form.ItemList.Add(fibItem);

			list.Add(new FieldElement(fibItem.BlankList[0]));
			list.Add(new StringElement(" + 2"));

			Assert.AreEqual(2, list.Count);
			Assert.AreEqual("<<Form 1:Q1:a>>", list[0].ToString());
			Assert.AreEqual(" + 2", list[1].ToString());
			Assert.AreEqual("<<Form 1:Q1:a>> + 2", list.ToString());
		}
#endif
	}
}
