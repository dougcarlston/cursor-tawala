using System;
using NUnit.Framework;
using Tawala.Projects;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
	public class ExpressionElementTest
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
		public void ConstructField()
		{
			// create field from FIB item and blank index
			FibItem fibItem = new FibItem();

			// add FIB item to form
			form.ItemList.Add(fibItem);

			ExpressionElement element = new FieldElement(fibItem.BlankList[0]);

			Assert.AreEqual("<<Form 1:Q1:a>>", element.ToString());
		}

		[Test]
		public void ConstructString()
		{
			ExpressionElement element = new StringElement(" + 2");

			Assert.AreEqual(" + 2", element.ToString());
		}
		
		[Test]
		public void PastedCopyFromClipboardContainsCorrectVariableAndFieldReferences()
		{
			form.ItemList.Add(new McqItem());

			FieldElement element = new FieldElement(form.ItemList[0]);
			Assert.AreEqual("<<Form 1:Q1>>", element.ToString());

			System.Windows.Forms.Clipboard.SetDataObject(element);

			FieldElement copiedElement = System.Windows.Forms.Clipboard.GetDataObject().GetData(typeof(FieldElement)) as FieldElement;
			Assert.AreNotSame(element, copiedElement, "Original element and copied element are same reference");

			Assert.AreEqual(element.Field.Id, copiedElement.Field.Id);

			Assert.AreSame(element.Field, copiedElement.Field, "Original Field and copied are not the same!");
			Assert.AreEqual("<<Form 1:Q1>>", copiedElement.ToString());

		}
#endif
	}
}
