using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing validity of alternate labels for Form Items
	/// </summary>
	[TestFixture]
	public class FormItemAlternateLabelTest
	{
		private IForm form;
		private FormItemList list;
		private FibItem fibItem;
		private TextItem textItem;
		private McqItem mcItem;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();

			form = Project.Current.AddForm();
			list = form.ItemList;
			list.Add(new FibItem());
			fibItem = (FibItem)form.ItemList[0];

			list.Add(new TextItem());
			textItem = (TextItem)form.ItemList[1];
			
			list.Add(new McqItem());
			mcItem = (McqItem)form.ItemList[2];
		}

		[Test]
		public void AlternateItemLabel()
		{
			// set the alternate label of the FIB item
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, null, "Label One"));
			fibItem.AlternateLabel = "Label One";

			// it should be legal to rename it to the same name
			Assert.AreEqual("Label One", fibItem.AlternateLabel);
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, null, "Label One"));

			// or another name
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, null, "Label Uno"));

			// or nothing
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, null, ""));
		}

		[Test]
		public void DuplicateItemLabelInSameList()
		{
			Blank blank = fibItem.BlankList[0];

			// we should be able to call any item "Label One"
			Assert.AreEqual(true, list.ValidAlternateLabel(textItem, null, "Label One"));
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, null, "Label One"));
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, blank, "Label One"));
			Assert.AreEqual(true, list.ValidAlternateLabel(mcItem, null, "Label One"));

			// set the blank's alternate label
			blank.AlternateLabel = "Label One";

			// now "Label One" is only considered valid for the blank because it now has that name
			Assert.AreEqual(false, list.ValidAlternateLabel(textItem, null, "Label One"));
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "Label One"));
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, blank, "Label One"));
			Assert.AreEqual(false, list.ValidAlternateLabel(mcItem, null, "Label One"));

			// Just because one item was named doesn't mean we can't consider other names
			Assert.AreEqual(true, list.ValidAlternateLabel(textItem, null, "Label Two"));
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, null, "Label Two"));
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, blank, "Label Two"));
			Assert.AreEqual(true, list.ValidAlternateLabel(mcItem, null, "Label Two"));
		}

		[Test]
		public void DuplicateItemLabelInDifferentForms()
		{
			// give the MC in form one an alternate label
			mcItem.AlternateLabel = "Label One";

			// create another form with an MC item
			IForm form2 = Project.Current.AddForm();
			form2.ItemList.Add(new McqItem());
			McqItem mcItem2 = (McqItem)form2.ItemList[0];

			// check to see if it can have the same label
			Assert.AreEqual(true, form2.ItemList.ValidAlternateLabel(mcItem2, null, "Label One"));
		}

		[Test]
		public void ItemLabelWithLeadingUnderscores()
		{
			// try an alternate label with a single underscore - should be OK
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, null, "_Label One"));

			// double leading underscores denote reserved labels
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "__Label One"));
		}

		[Test]
		public void ItemLabelWithNumericCharacters()
		{
			// try an alternate label that contains a number
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, null, "Label1"));

			// a leading numeric is also OK
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, null, "1Label"));

			// numerics only are a no-no
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "1"), "1 should be invalid");
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "42.33"), "42.33 should be invalid");
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, ".5"), ".5 should be invalid");
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "-.5"), "-.5 should be invalid");
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "5."), "5. should be invalid");
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "-5."), "5. should be invalid");
		}

		[Test]
		public void ItemLabelContainsColon()
		{
			// try an alternate label that contains a colon
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "Label:1"));
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, ":Label1"));
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "Label1:"));
		}

		[Test]
		public void AlternateItemLabelInDefaultLabelFormat()
		{
			// Alternate Labels that look like default labels are illegal
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "Q14"));
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "q2"));
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "T9987563"));
			Assert.AreEqual(false, list.ValidAlternateLabel(fibItem, null, "t121"));

			// these look close, but are legal constructs
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, null, "Q14s"));
			Assert.AreEqual(true, list.ValidAlternateLabel(fibItem, null, "t"));
		}

		[Test]
		public void ItemLabelWithWhitespace()
		{
			// create new empty form item list
			FormItemList list = new FormItemList();

			// add an item and set its alternate label
			list.Add(new FormItem());
			IFormItem item1 = list[0];
			item1.AlternateLabel = "Foo";

			// add a second item and try to use a label that's identical, but with whitespace
			list.Add(new FormItem());
			IFormItem item2 = list[1];
			Assert.AreEqual(false, list.ValidAlternateLabel(item2, null, "   Foo   "));
		}

		[Test]
		public void BlankLabelWithWhitespace()
		{
			// set the FIB blank's alternate label
			Blank blank1 = fibItem.BlankList[0];
			blank1.AlternateLabel = "Foo";

			// add a second FIB and try to use a label that's identical, but with whitespace
			list.Add(new FibItem());
			FibItem item2 = (FibItem)list[3];
			Blank blank2 = item2.BlankList[0];
			Assert.AreEqual(false, list.ValidAlternateLabel(item2, blank2, "   Foo   "));
		}
	}
}
