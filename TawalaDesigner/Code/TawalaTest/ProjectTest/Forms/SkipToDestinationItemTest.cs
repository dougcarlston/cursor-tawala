using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Common;
using Tawala.Projects.Forms;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Class to test the ItemLabel class.
	/// </summary>
	[TestFixture]
	public class SkipToDestinationItemTest
	{
		[Test]
		public void NewSkipToDestinationItem()
		{
			SkipToDestinationItem destItem = new SkipToDestinationItem();

			Assert.AreEqual("End of Form", destItem.ToString());
		}

		[Test]
		public void GetString()
		{
			// create clean project
			Project.NewTestProject();

			// create Form and add a FIB item
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new FibItem());
			FibItem fib = ((FibItem)form.ItemList[0]);

			// create an SkipToDestinationItem for the FIB
			SkipToDestinationItem destItem = new SkipToDestinationItem(fib);
			Assert.AreEqual("Q1", destItem.ToString());

			// try a MCQ item
			form.ItemList.Add(new McqItem());
			McqItem mc = ((McqItem)form.ItemList[1]);

			destItem = new SkipToDestinationItem(mc);
			Assert.AreEqual("Q2", destItem.ToString());

			// and a text item
			form.ItemList.Add(new TextItem());
			TextItem text = ((TextItem)form.ItemList[2]);

			destItem = new SkipToDestinationItem(text);
			Assert.AreEqual("T1", destItem.ToString());
		}

		[Test]
		public void AlternateLabel()
		{
			// create clean project
			Project.NewTestProject();

			// create Form and add a FIB item
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new FibItem());
			FibItem fib = ((FibItem)form.ItemList[0]);

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
			// create clean project
			Project.NewTestProject();

			// create Form and add a FIB item
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new FibItem());
			FibItem fib = ((FibItem)form.ItemList[0]);

			// create an SkipToDestinationItem for the FIB
			SkipToDestinationItem destItem1 = new SkipToDestinationItem(fib);
			Assert.AreEqual("Q1", destItem1.ToString());

			// and an MCQ item
			form.ItemList.Add(new McqItem());
			McqItem mc = ((McqItem)form.ItemList[1]);

			SkipToDestinationItem destItem2 = new SkipToDestinationItem(mc);
			Assert.AreEqual("Q2", destItem2.ToString());

			// serialize and deserialize the initial list
			// Note: this is intended to simulate a cut/paste operation.
			//  In-memory Clone (serialiation/deserialization) is used
			//  because clipboard operations cannot be used in
			//  the NUnit test framework.
			McqItem mc2 = Cloner.Clone(mc);
			form.ItemList.Remove(mc);
			form.ItemList.Insert(0, mc2);

			Assert.AreEqual("Q2", destItem1.ToString());
			Assert.AreEqual("Q1", destItem2.ToString());
		}

		[Test]
		public void AttributeString()
		{
			// create clean project
			Project.NewTestProject();

			// create Form and add a FIB item
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new FibItem());
			FibItem fib = ((FibItem)form.ItemList[0]);

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
			// create clean project
			Project.NewTestProject();

			// create Form and add a FIB item
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new FibItem());
			FibItem fib = ((FibItem)form.ItemList[0]);

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
