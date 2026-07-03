using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Common;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
	public class SkipToDestinationItemTest
	{
        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

		[Test]
		public void NewSkipToDestinationItem()
		{
			SkipToDestinationItem destItem = new SkipToDestinationItem();

			Assert.AreEqual("End of Form", destItem.ToString());
		}

		[Test]
		public void GetString()
		{

			// create Form and add a FIB item
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new NewFibItem());
			IFibItem fib = ((IFibItem)form.ItemList[0]);

			// create an SkipToDestinationItem for the FIB
			SkipToDestinationItem destItem = new SkipToDestinationItem(fib);
			Assert.AreEqual("Q1", destItem.ToString());

			// try a MCQ item
			form.ItemList.Add(new NewMcqItem());
			IMcqItem mc = ((IMcqItem)form.ItemList[1]);

			destItem = new SkipToDestinationItem(mc);
			Assert.AreEqual("Q2", destItem.ToString());

			// and a text item
			form.ItemList.Add(new NewTextItem());
			ITextItem text = ((ITextItem)form.ItemList[2]);

			destItem = new SkipToDestinationItem(text);
			Assert.AreEqual("T1", destItem.ToString());
		}

		[Test]
		public void AlternateLabel()
		{
			// create Form and add a FIB item
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new NewFibItem());
			IFibItem fib = ((IFibItem)form.ItemList[0]);

			// create an SkipToDestinationItem for the FIB
			SkipToDestinationItem destItem = new SkipToDestinationItem(fib);
			Assert.AreEqual("Q1", destItem.ToString());

			// give the fib an alternate label
			fib.AlternateLabel = "User Label";
			Assert.AreEqual("User Label", destItem.ToString());
		}

		[Test]
		public void CopyPaste()
		{
			// create Form and add a FIB item
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new NewFibItem());
			IFibItem fib = ((IFibItem)form.ItemList[0]);

			// create an SkipToDestinationItem for the FIB
			SkipToDestinationItem destItem1 = new SkipToDestinationItem(fib);
			Assert.AreEqual("Q1", destItem1.ToString());

			// and an MCQ item
			form.ItemList.Add(new NewMcqItem());
			IMcqItem mc = ((IMcqItem)form.ItemList[1]);

			SkipToDestinationItem destItem2 = new SkipToDestinationItem(mc);
			Assert.AreEqual("Q2", destItem2.ToString());

			// serialize and deserialize the initial list
			// Note: this is intended to simulate a cut/paste operation.
			//  In-memory Clone (serialiation/deserialization) is used
			//  because clipboard operations cannot be used in
			//  the NUnit test framework.
			IMcqItem mc2 = Cloner.Clone(mc);
			form.ItemList.Remove(mc);
			form.ItemList.Insert(0, mc2);

			Assert.AreEqual("Q2", destItem1.ToString());
			Assert.AreEqual("Q1", destItem2.ToString());
		}

		[Test]
		public void AttributeString()
		{
			// create Form and add a FIB item
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new NewFibItem());
			IFibItem fib = ((IFibItem)form.ItemList[0]);

			// create an SkipToDestinationItem for the FIB
			SkipToDestinationItem destItem1 = new SkipToDestinationItem(fib);
			Assert.AreEqual(destItem1.ToString(), destItem1.AttributeString());
		}

		[Test]
		public void EndOfFormItem()
		{
			// the default constructor creates an "end-of-form" destination item
			SkipToDestinationItem item = new SkipToDestinationItem();

			Assert.AreEqual("End of Form", item.ToString());
			Assert.AreEqual("__EndOfForm__", item.AttributeString());
		}

		[Test]
		public void InvalidItem()
		{
			// create Form and add a FIB item
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new NewFibItem());
			IFibItem fib = ((IFibItem)form.ItemList[0]);

			// create an SkipToDestinationItem for the FIB, which should be valid
			SkipToDestinationItem destItem = new SkipToDestinationItem(fib);
			Assert.AreEqual(true, destItem.Valid);

			// if we delete the FIB item, the destination item should be invalid
			form.ItemList.Remove(fib);
			Assert.AreEqual(false, destItem.Valid);

			// an "end-of-form" destination item is valid
			destItem = new SkipToDestinationItem();
			Assert.AreEqual(true, destItem.Valid);
		}
	}
}
