using System;
using NUnit.Framework;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
    public class FormItemListTest
    {
#if FIXED
		private bool addHandlerCalled = false;
		private string defaultFibStyleAtttribute = " style=\"topLabels\"";

		[SetUp]
		public void MethodSetup()
		{
			TestSupport.Util.NewTestProject();
		}

		[Test]
		public void AddFormItem() 
		{ 
			Project.Events.FormItemAdded += addFormItemEventHandler;

			// list not owned by a form shouldn't generate events
			FormItemList list = new FormItemList();
			list.Add(new FormItem());
			Assert.IsFalse(addHandlerCalled);
			
			// list owned by a form should generate events
			list = new FormItemList(ComponentMaker.MakeFormObject("Test"));
			FormItem item = new FormItem();
			list.Add(item);

			//Assertions 

			Assert.IsTrue(addHandlerCalled);
			Assert.AreEqual(1, list.Count);
			Assert.IsTrue(list.Contains(item));
			Assert.AreEqual(list[0], item);
			Assert.IsTrue(list.Contains(item));
		} 

		private void addFormItemEventHandler(object sender, FormItemEventArgs e)
		{
			addHandlerCalled = true;
			Assert.IsTrue(e.Form != null);
			Assert.IsTrue(e.Item != null);
			Assert.IsTrue(e.Order == 0);
		}

		public bool insertHandlerCalled = false;

		[Test]
		public void InsertFormItem() 
		{ 
			FormItemList list = new FormItemList(ComponentMaker.MakeFormObject("Test"));
			list.Add(new FormItem());
			list.Add(new FormItem());
			list.Add(new FormItem());
			list.Add(new FormItem());

			Project.Events.FormItemAdded += insertFormItemEventHandler;
			FormItem item = new FormItem();
			list.Insert(2, item);

			//Assertions 
			Assert.AreEqual(5, list.Count);
			Assert.IsTrue(insertHandlerCalled);
			Assert.IsTrue(list.Contains(item));
			Assert.AreEqual(item, list[2]);
		} 

		private void insertFormItemEventHandler(object sender, FormItemEventArgs e)
		{
			insertHandlerCalled = true;
			Assert.IsTrue(e.Form != null);
			Assert.IsTrue(e.Item != null);
			Assert.AreEqual(2, e.Order);
		}

		private bool removeHandlerCalled = false;

		[Test]
		public void RemoveFormItem() 
		{ 
			Project.Events.FormItemRemoved += RemoveFormItemEventHandler;

			FormItem item = new FormItem();

			// List not owned by form shouldn't generate events
			FormItemList list = new FormItemList();
			list.Add(item);
			list.Remove(item);
			Assert.IsFalse(removeHandlerCalled);


			// List owned by form should generate events.  Also do more rigorous tests.

			list = new FormItemList(ComponentMaker.MakeFormObject("Test"));

			list.Add(new FormItem());
			list.Add(new FormItem());
			list.Add(new FormItem());
			list.Add(new FormItem());
			list.Add(new FormItem());

			list.Insert(3, item);
			Assert.IsFalse(removeHandlerCalled);

			//Assertions 
			Assert.AreEqual(6, list.Count);
			Assert.IsTrue(list.Contains(item));
			Assert.AreEqual(list[3], item);
			Assert.IsTrue(list.Contains(item));

			list.Remove(item);
			Assert.AreEqual(5, list.Count);
			Assert.IsTrue(removeHandlerCalled);
		} 

		void RemoveFormItemEventHandler(object sender, FormItemEventArgs e)
		{
			removeHandlerCalled = true;
			Assert.IsTrue(e.Form != null);
			Assert.IsTrue(e.Item != null);
			Assert.AreEqual(3, e.Order);
		}

		[Test]
		public void AddTwoFormItemsRetrieveOne() 
		{ 
			FormItem item1 = new FormItem();
			item1.Text = "Text 1";
			FormItem item2 = new FormItem();
			item2.Text = "Text 2";

			FormItemList list = new FormItemList();
			list.Add(item1);
			list.Add(item2);

			FormItem item2ByIndex = (FormItem)list[1];

			//Assertions 
			Assert.AreEqual("Text 2", item2ByIndex.Text);
		}

		[Test]
		public void CutPaste()
		{
			// create new empty form item list
			FormItemList list = new FormItemList();

			// add form items to list
			list.Add(new FormItem());
			list.Add(new FormItem());
			list.Add(new FormItem());

			Assert.AreEqual(list.Id+1, list[0].Id);
			Assert.AreEqual(list.Id+2, list[1].Id);
			Assert.AreEqual(list.Id+3, list[2].Id);

			// serialize and deserialize the initial list
			// (Note: this is intended to simulate a cut/paste operation.
			//  In-memory serialiation/deserialization is used instead
			//  because clipboard operations cannot be used in
			//  the NUnit test framework.
			DataObject dataObject = new DataObject();

			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
				bf.Serialize(ms, list);
				ms.Seek(0, SeekOrigin.Begin);
				dataObject.SetData(typeof(FormItemList), (FormItemList)bf.Deserialize(ms));
			}

			// create new list from deserialized data
			FormItemList list2 = (FormItemList)dataObject.GetData(typeof(FormItemList));

			// verify that form items have same ids as before the "cut"
			Assert.AreEqual(list2.Id+1, list2[0].Id);
			Assert.AreEqual(list2.Id+2, list2[1].Id);
			Assert.AreEqual(list2.Id+3, list2[2].Id);
		}

		[Test]
		public void CopyPaste()
		{
			// create new empty form item list
			FormItemList list = new FormItemList();

			// add form items to list
			list.Add(new FormItem());
			list.Add(new FormItem());
			list.Add(new FormItem());

			Assert.AreEqual(list.Id+1, list[0].Id);
			Assert.AreEqual(list.Id+2, list[1].Id);
			Assert.AreEqual(list.Id+3, list[2].Id);

			// set form item ids to zero to indicate a copy (rather than cut) operation
			list[0].ClearId();
			list[1].ClearId();
			list[2].ClearId();

			// serialize and deserialize the initial list
			// (Note: this is intended to simulate a copy/paste operation.
			//  In-memory serialiation/deserialization is used instead
			//  because clipboard operations cannot be used in
			//  the NUnit test framework.
			DataObject dataObject = new DataObject();

			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
				bf.Serialize(ms, list);
				ms.Seek(0, SeekOrigin.Begin);
				dataObject.SetData(typeof(FormItemList), (FormItemList)bf.Deserialize(ms));
			}

			// create new list from deserialized data
			FormItemList list2 = (FormItemList)dataObject.GetData(typeof(FormItemList));

			// verify that form items have new, unique ids
			Assert.AreEqual(list2.Id+4, list2[0].Id);
			Assert.AreEqual(list2.Id+5, list2[1].Id);
			Assert.AreEqual(list2.Id+6, list2[2].Id);
		}

		[Test]
		public void ContainsAlternateLabel()
		{
			Tawala.Proj.IForm form = Project.Current.AddForm();

			ITextItem textItem = new TextItem();
			textItem.AlternateLabel = "Bar";
			FibItem fibItem = new FibItem();
			Blank blank1 = new Blank(new FibItem(), 10);
			blank1.AlternateLabel = "Foo";
			fibItem.BlankList.Add(blank1);

			form.ItemList.Add(textItem);
			form.ItemList.Add(fibItem);

			IHeadingItem heading = new HeadingItem();
			heading.AlternateLabel = "MyHeading";
			form.ItemList.Add(heading);

			Assert.IsTrue(form.ItemList.ContainsAlternateLabel("Bar"));
			Assert.IsTrue(form.ItemList.ContainsAlternateLabel("Foo"));
			Assert.IsFalse(form.ItemList.ContainsAlternateLabel("T1"));
			Assert.IsTrue(form.ItemList.ContainsAlternateLabel("MyHeading"));
		}

		/// <summary>
		/// Make sure conflicting alternate labels are handled properly.
		/// FormList.Paste was added as an alternative to Insert when conflicts are possible.
		/// </summary>
		[Test]
		public void PasteFormItem()
		{
			Tawala.Proj.IForm form = Project.Current.AddForm();

			TextItem textItem = new TextItem();
			textItem.AlternateLabel = "Bar";
			FibItem fibItem = new FibItem();
			Blank blank1 = new Blank(new FibItem(), 1);
			blank1.AlternateLabel = "Foo";
			fibItem.BlankList.Add(blank1);

			form.ItemList.Add(textItem);
			form.ItemList.Add(fibItem);

			TextItem pasteTextItem1 = new TextItem();
			pasteTextItem1.AlternateLabel = "Anything";
			form.ItemList.Paste(0, pasteTextItem1);
			Assert.AreEqual("Anything", pasteTextItem1.AlternateLabel);

			TextItem pasteTextItem2 = new TextItem();
			pasteTextItem2.AlternateLabel = "Foo";
			form.ItemList.Paste(0, pasteTextItem2);
			Assert.AreEqual("Foo1", pasteTextItem2.AlternateLabel);

			FibItem pasteFibItem = new FibItem();
			Blank pasteBlank = new Blank(pasteFibItem, 5);
			pasteBlank.AlternateLabel = "Bar";
			pasteFibItem.BlankList.Add(pasteBlank);
			form.ItemList.Paste(0, pasteFibItem);
			Assert.AreEqual("Bar1", pasteBlank.AlternateLabel);
		}

		[Test]
		public void MoveToBottom()
		{
			// create new empty form item list
			FormItemList list = new FormItemList();

			// add form items to list
			FormItem textItem = new TextItem();
			FormItem fibItem = new FibItem();
			FormItem mcItem = new McqItem();

			list.Add(textItem);
			list.Add(fibItem);
			list.Add(mcItem);

			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(textItem, list[0]);
			Assert.AreEqual(fibItem, list[1]);
			Assert.AreEqual(mcItem, list[2]);

			list.Move(textItem, 3);
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(fibItem, list[0]);
			Assert.AreEqual(mcItem, list[1]);
			Assert.AreEqual(textItem, list[2]);
		}

		[Test]
		public void MoveToTop()
		{
			// create new empty form item list
			FormItemList list = new FormItemList();

			// add form items to list
			FormItem textItem = new TextItem();
			FormItem fibItem = new FibItem();
			FormItem mcItem = new McqItem();

			list.Add(textItem);
			list.Add(fibItem);
			list.Add(mcItem);

			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(textItem, list[0]);
			Assert.AreEqual(fibItem, list[1]);
			Assert.AreEqual(mcItem, list[2]);

			list.Move(mcItem, 0);
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(mcItem, list[0]);
			Assert.AreEqual(textItem, list[1]);
			Assert.AreEqual(fibItem, list[2]);

			list.Move(fibItem, 0);
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(fibItem, list[0]);
			Assert.AreEqual(mcItem, list[1]);
			Assert.AreEqual(textItem, list[2]);
		}

		[Test]
		public void MoveToSame()
		{
			// create new empty form item list
			FormItemList list = new FormItemList();

			// add form items to list
			FormItem textItem = new TextItem();
			FormItem fibItem = new FibItem();
			FormItem mcItem = new McqItem();

			list.Add(textItem);
			list.Add(fibItem);
			list.Add(mcItem);

			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(textItem, list[0]);
			Assert.AreEqual(fibItem, list[1]);
			Assert.AreEqual(mcItem, list[2]);

			list.Move(fibItem, 1);
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(textItem, list[0]);
			Assert.AreEqual(fibItem, list[1]);
			Assert.AreEqual(mcItem, list[2]);
		}

		[Test]
		public void GetXml() 
		{
			// create new form items
			TextItem textItem1 = new TextItem();
			textItem1.Text = "Text item 1 text";

			TextItem textItem2 = new TextItem();
			textItem2.Text = "Text item 2 text";

			FibItem fibItem1 = new FibItem();

			fibItem1.Rtf =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0" +
				@"\blue0;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20\cf3" +
				@" Fib item 1: _____" +
				@"\par }";

			FibItem fibItem2 = new FibItem();

			fibItem2.Rtf =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0" +
				@"\blue0;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20\cf3" +
				@" Fib item 2: __" +
				@"\par }";

			fibItem2.BlankList[0].Required = true;

			McqItem mcItem1 = new McqItem();
			mcItem1.Text = "Multiple choice question 1";

			McqItem mcItem2 = new McqItem();
			mcItem2.Text = "Multiple choice question 2";

			FormItemList list = new FormItemList();
			list.Add(textItem1);
			list.Add(fibItem1);
			list.Add(mcItem1);
			list.Add(textItem2);
			list.Add(mcItem2);
			list.Add(fibItem2);

			string expString =
				"<items>\r\n" +
				"<text label=\"T1\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Text item 1 text" +
				"</paragraph>" +
				"</text>\r\n" +
				"<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Fib item 1: " +
				XmlConstants.EndFont +
				"<blank label=\"a\" length=\"5\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>\r\n" +
				"<mc label=\"Q2\" onlyone=\"true\" required=\"false\">" +
				"<question>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Multiple choice question 1" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</choice>" +
				"</mc>\r\n" +
				"<text label=\"T2\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Text item 2 text" +
				"</paragraph>" +
				"</text>\r\n" +
				"<mc label=\"Q3\" onlyone=\"true\" required=\"false\">" +
				"<question>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Multiple choice question 2" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</choice>" +
				"</mc>\r\n" +
				"<fib label=\"Q4\"" + defaultFibStyleAtttribute + ">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Fib item 2: " +
				XmlConstants.EndFont +
				"<blank label=\"a\" length=\"2\" required=\"true\"/>" +
				"</paragraph>" +
				"</fib>\r\n" +
				"</items>\r\n";

			//Assertions 
			Assert.AreEqual(expString, list.ToXml());
		}

		[Test]
		public void FieldName()
		{
			FormItemList list = new FormItemList();

			Assert.AreEqual("", list.FieldName);
		}

		[Test]
		public void FieldString()
		{
			FormItemList list = new FormItemList();

			Assert.AreEqual("", list.FieldString);
		}
#endif
    }
}