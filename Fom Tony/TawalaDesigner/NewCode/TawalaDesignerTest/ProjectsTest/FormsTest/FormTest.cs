using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("Update test to work with new classes")]
    [TestFixture]
	public class FormTest
	{
#if FIXED
		[TestFixtureSetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();
		}

		[Test]
		public void NewForm() 
		{ 
			IForm fc = new NewForm("Test");

			//Assertions 
			Assert.IsNotNull(fc);
			Assert.AreEqual("Test", fc.Name);
		}

		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
                "<form name=\"Form 1\" startPoint=\"true\">" +
				"</form>";

			IXmlElement element = new XmlElement(xmlString);
            IForm form = new NewForm(element);

			Assert.AreEqual("Form 1", form.Name);
			Assert.IsTrue(form.StartingPoint);
			Assert.IsNotNull(form.ItemList);
		}

		[Test]
		public void ConstructFromXmlWithStartpoints()
		{
			string xmlString =
				"<form name=\"Form 1\" startPoint=\"true\">" +
				"</form>";

			IXmlElement element = new XmlElement(xmlString);
            IForm form = new NewForm(element);

			Assert.IsTrue(form.StartingPoint);

			xmlString =
				"<form name=\"Form 1\" startPoint=\"false\">" +
				"</form>";

			element = new XmlElement(xmlString);
            form = new NewForm(element);

			Assert.IsFalse(form.StartingPoint);
		}

		[Test]
		public void NameForm() 
		{
            IForm fc = new NewForm("Renamed Form");

			//Assertions 
			Assert.AreEqual("Renamed Form", fc.Name);
		} 

		[Test]
		public void NameFormViaConstructor() 
		{
            IForm fc = new NewForm("new name");

			//Assertions 
			Assert.AreEqual("new name", fc.Name);
		}
		
		[Test]
		public void TrimWhitespaceFromName()
		{
            IForm form = new NewForm("    Form 1  ");
			Assert.AreEqual("Form 1", form.Name);

			form.Name = "   Renamed Form   ";
			Assert.AreEqual("Renamed Form", form.Name);
		}

		[Test]
		public void StartingPoint()
		{
            IForm form = new NewForm("Goo");
			Assert.AreEqual(false, form.StartingPoint);
			form.StartingPoint = true;
			Assert.AreEqual(true, form.StartingPoint);
			form.StartingPoint = false;
			Assert.AreEqual(false, form.StartingPoint);
		}

		[Test]
		public void AddAndRetrieveFormItems() 
		{
            IForm fc = new NewForm("new name");

			FormItem item1 = new FormItem();
			item1.Text = "Content for text item #1.";
			fc.ItemList.Add(item1);

			FormItem item2 = new FormItem();
			item2.Text = "Different content for text item #2.";
			fc.ItemList.Add(item2);

			FormItem retrievedItem1 = ((FormItem)fc.ItemList[0]);
			FormItem retrievedItem2 = ((FormItem)fc.ItemList[1]);

			//Assertions 
			Assert.AreEqual("Content for text item #1.", retrievedItem1.Text);
			Assert.AreEqual("Different content for text item #2.", retrievedItem2.Text);
		} 

		[Test]
		public void InsertTextItem() 
		{
            IForm fc = new NewForm("new name");

			TextItem item1 = new TextItem();
			fc.ItemList.Add(item1);

			TextItem item2 = new TextItem();
			fc.ItemList.Add(item2);

			TextItem item3 = new TextItem();
			fc.ItemList.Insert(1, item3);

			//Assertions 
			Assert.AreEqual(item1, fc.ItemList[0]);
			Assert.AreEqual(item3, fc.ItemList[1]);
			Assert.AreEqual(item2, fc.ItemList[2]);
		} 

		[Test]
		public void InsertOnlyOneTextItem() 
		{
            IForm fc = new NewForm("new name");

			TextItem item1 = new TextItem();
			fc.ItemList.Insert(0, item1);

			//Assertions 
			Assert.AreEqual(item1, fc.ItemList[0]);
		} 

		[Test]
		public void ConnectProcessToForm() 
		{
            IForm fc = new NewForm("TestForm");

			Process process = new Process("Connected Process");

			// connect Process
			fc.ConnectedProcess = process;

			//Assertions 
			Assert.AreEqual("Connected Process", fc.ConnectedProcess.Name);

			// now disconnect it
			fc.ConnectedProcess = null;

			//Assertions 
			Assert.IsNull(fc.ConnectedProcess);
		} 

		[Test]
		public void GetFields() 
		{
			string[] fieldNames = new string[]
			{
				"Q1:a",
				"Q2:a",
				"Q3",
			};

			// create new form
			IForm form = Project.Current.AddForm();

			TextItem textItem = new TextItem();
			form.ItemList.Add(textItem);

			// add 2 FIB items to form
			FibItem fibItem1 = new FibItem();
			FibItem fibItem2 = new FibItem();
			form.ItemList.Add(fibItem1);
			form.ItemList.Add(fibItem2);

			// add 1 MC item to form
			McqItem mcItem1 = new McqItem();
			form.ItemList.Add(mcItem1);

			// get fields from form
			FieldList formFields = form.GetFields();

			int i = 0;

			foreach (IField field in formFields.RecursiveEnumerator)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}


		[Test]
		public void GetProcessableFields()
		{
			string[] fieldNames = new string[3]
			{
				"Q1:a",
				"Q2:a",
				"Q3",
			};

			// create new form
			IForm form = Project.Current.AddForm();

			TextItem textItem = new TextItem();
			form.ItemList.Add(textItem);

			// add 2 FIB items to form
			FibItem fibItem1 = new FibItem();
			FibItem fibItem2 = new FibItem();
			form.ItemList.Add(fibItem1);
			form.ItemList.Add(fibItem2);

			// add 1 MC item to form
			McqItem mcItem1 = new McqItem();
			form.ItemList.Add(mcItem1);

			// get fields from form
			FieldList formFields = form.GetFormItemFieldsAndRecordVariables();

			int i = 0;

			foreach (IField field in formFields.RecursiveEnumerator)
			{
				Assert.AreEqual(fieldNames[i++], field.FieldName);
			}
		}


		[Test]
		public void GetDefaultLabel()
		{
			// create new form
            IForm form = new NewForm("Test");

			// add 2 FIB items to form
			FibItem fibItem1 = new FibItem();
			FibItem fibItem2 = new FibItem();
			form.ItemList.Add(fibItem1);
			form.ItemList.Add(fibItem2);

			// add 1 MC item to form
			McqItem mcItem1 = new McqItem();
			form.ItemList.Add(mcItem1);

			// add 1 text item to form
			TextItem textItem1 = new TextItem();
			form.ItemList.Add(textItem1);

			//Assertions 
			Assert.AreEqual("Q1", form.GetDefaultLabel(fibItem1));
			Assert.AreEqual("Q2", form.GetDefaultLabel(fibItem2));
			Assert.AreEqual("Q3", form.GetDefaultLabel(mcItem1));
			Assert.AreEqual("T1", form.GetDefaultLabel(textItem1));
		}


		[Test]
		public void GetSkipInstructionsFromStatement()
		{
			// create new form
            IForm form = new NewForm("Test");

			// add Skip Instruction items
			SkipInstructionsItem skip1 = new SkipInstructionsItem();
			SkipInstructionsItem skip2 = new SkipInstructionsItem();

			form.ItemList.Add(skip1);
			form.ItemList.Add(skip2);

			// create some statements and put them in the Skip Instructions
			IfStatement if1 = new IfStatement();
			IfStatement if2 = new IfStatement();
			SetStatement set1 = new SetStatement();
			SetStatement set2 = new SetStatement();

			skip1.Instructions.Lines.Add(new ProcessLineList(if1));
			skip1.Instructions.Lines.Add(new ProcessLineList(set1));
			skip2.Instructions.Lines.Add(new ProcessLineList(if2));

			Assert.AreEqual(skip1.Instructions, form.GetSkipInstructions(if1));
			Assert.AreEqual(skip1.Instructions, form.GetSkipInstructions(set1));
			Assert.AreEqual(skip2.Instructions, form.GetSkipInstructions(if2));
			Assert.AreEqual(null, form.GetSkipInstructions(set2));
		}

		[Test]
		public void ActiveSkipToItem()
		{
			// create new form
            IForm form = new NewForm("Test");

			// add some items
			form.ItemList.Add(new TextItem());
			form.ItemList.Add(new FibItem());
			form.ItemList.Add(new McqItem());

			// and a skip item
			SkipInstructionsItem SkipToItem1 = new SkipInstructionsItem();
			form.ItemList.Add(SkipToItem1);

			Assert.AreEqual(null, form.ActiveSkipToItem);

			// now set the skip item as "active"
			// (this would mean that its instructions are being displayed in the Skip Instructions view)
			form.ActiveSkipToItem = SkipToItem1;
			Assert.AreEqual(SkipToItem1, form.ActiveSkipToItem);

			// create another skip item, but don't add it
			SkipInstructionsItem SkipToItem2 = new SkipInstructionsItem();

			// "activating" a skip item that's not in the form is a no-no
			// (this shouldn't happen in real life)
			form.ActiveSkipToItem = SkipToItem2;
			Assert.AreEqual(null, form.ActiveSkipToItem);

			// now add it and try again
			form.ItemList.Add(SkipToItem2);
			form.ActiveSkipToItem = SkipToItem2;
			Assert.AreEqual(SkipToItem2, form.ActiveSkipToItem);

			// remove the skip item from the Form
			form.ItemList.Remove(SkipToItem2);
			Assert.AreEqual(null, form.ActiveSkipToItem);
		}

		[Test]
		public void SkipToDestinationList()
		{
            IForm fc = new NewForm("new name");

			// there should always be an "end of form" destination
			Assert.AreEqual(1, fc.SkipToDestinations.Count);

			// now add a couple of Form items
			TextItem item1 = new TextItem();
			fc.ItemList.Add(item1);

			FibItem fibItem = new FibItem();
			fc.ItemList.Add(fibItem);

			IHeadingItem heading1 = new HeadingItem();
			fc.ItemList.Add(heading1);

			Assert.AreEqual(4, fc.SkipToDestinations.Count);
		}
        [Ignore("FIX: Uses old FormItems, should be split up, also does a test for Form name with special char's which NewForm fails")]
		[Test]
		public void GetXml() 
		{
            IForm fc = new NewForm("Form Name");

			TextItem item1 = new TextItem();
			item1.Text = "Item 1 text";
			fc.ItemList.Add(item1);

			TextItem item2 = new TextItem();
			item2.Text = "Item 2 text";
			fc.ItemList.Add(item2);

			string expString =
				"<form name=\"Form Name\" startPoint=\"false\">" +
				"<items>\r\n" +
				"<text label=\"T1\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Item 1 text" +
				"</paragraph>" +
				"</text>\r\n" +
				"<text label=\"T2\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Item 2 text" +
				"</paragraph>" +
				"</text>\r\n" +
				"</items>\r\n" +
				"</form>";

			//Assertion
			Assert.AreEqual(expString, fc.ToXml());

			// connect a Process to the Form
			Process process = new Process("Connected Process");

			fc.ConnectedProcess = process;

			string expStringWithProcess =
				"<form name=\"Form Name\" startPoint=\"false\" process=\"Connected Process\">" +
				"<items>\r\n" +
				"<text label=\"T1\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Item 1 text" +
				"</paragraph>" +
				"</text>\r\n" +
				"<text label=\"T2\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Item 2 text" +
				"</paragraph>" +
				"</text>\r\n" +
				"</items>\r\n" +
				"</form>";

			//Assertion
			Assert.AreEqual(expStringWithProcess, fc.ToXml());

			// disconnect the Process
			fc.ConnectedProcess = null;

			//Assertion
			Assert.AreEqual(expString, fc.ToXml());

			// check for illegal XML characters
			fc.Name = "&<Form's \"Bad\" Name>";
			process.Name = "&<Process's \"Bad\" Name>";
			fc.ConnectedProcess = process;

			expStringWithProcess =
				"<form name=\"&amp;&lt;Form&apos;s &quot;Bad&quot; Name&gt;\" startPoint=\"false\" process=\"&amp;&lt;Process&apos;s &quot;Bad&quot; Name&gt;\">" +
				"<items>\r\n" +
				"<text label=\"T1\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Item 1 text" +
				"</paragraph>" +
				"</text>\r\n" +
				"<text label=\"T2\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Item 2 text" +
				"</paragraph>" +
				"</text>\r\n" +
				"</items>\r\n" +
				"</form>";

			//Assertion
			Assert.AreEqual(expStringWithProcess, fc.ToXml());
		}

#if false	// test of XML serialization; abandoned, but maybe useful in the future
			// for this to work the following must be added to this file:
			//			using System.Xml.Serialization;
			//			using System.IO;
			//
			// and the following must be added to Form.cs:
			//			using System.Xml.Serialization;
			//				[XmlInclude(typeof(FormItem))]
			//				[XmlInclude(typeof(TextItem))]
			//				[XmlInclude(typeof(FibItem))]

		[Test]
		public void TestXMLSerialization()
		{
			Form fc1 = new NewForm();
			fc1.Name = "Renamed Form";

			TextItem textItem1 = new TextItem();
			textItem1.Text = "Content for text item #1.";
			int textItem1ID = fc1.AddItem(textItem1);

			TextItem textItem2 = new TextItem();
			textItem2.Text = "Different content for text item #2.";
			int textItem2ID = fc1.AddItem(textItem2);

			FibItem fibItem1 = new FibItem();
			fibItem1.Text = "First Name: ____________________ Last Name: ____________________";
			int fibItem1ID = fc1.AddItem(fibItem1);

			// connect a Process to the Form
			Process process = new Process();
			process.Name = "Connected Process";

			fc1.ConnectedProcess = process.Name;

			XmlSerializer serializer = new XmlSerializer(typeof(Form));
			TextWriter writer = new StreamWriter("formTest.xml");

			serializer.Serialize(writer, fc1);
			writer.Close();

			XmlSerializer serializer2 = new XmlSerializer(typeof(Form));

#if false
// to be considered if we use XML serialization for Project files - jdf 4/13/05
			/* If the XML document has been altered with unknown 
			nodes or attributes, handle them with the 
			UnknownNode and UnknownAttribute events.*/
			serializer.UnknownNode+= new 
				XmlNodeEventHandler(serializer_UnknownNode);
			serializer.UnknownAttribute+= new 
				XmlAttributeEventHandler(serializer_UnknownAttribute);
#endif

			FileStream fs = new FileStream("formTest.xml", FileMode.Open);

			Form fc2;
			fc2 = (Form)serializer.Deserialize(fs);

			FormItem retrievedTextItem1 = fc2.GetItem(textItem1ID);
			FormItem retrievedTextItem2 = fc2.GetItem(textItem2ID);
			FormItem retrievedFibItem1 = fc2.GetItem(fibItem1ID);

			//Assertions 
			Assert.AreEqual("Renamed Form", fc2.Name);
			Assert.AreEqual(retrievedTextItem1.Id, textItem1ID);
			Assert.AreEqual("Content for text item #1.", retrievedTextItem1.Text);
			Assert.AreEqual("Different content for text item #2.", retrievedTextItem2.Text);

			// test our deployment XML output
			string expString =	"<form name=\"Renamed Form\" process=\"Connected Process\">\r\n" +
				"<items>\r\n" +
				"<text label=\"T1\">Content for text item #1.</text>\r\n" +
				"<text label=\"T2\">Different content for text item #2.</text>\r\n" +
				"<fib label=\"Q1\">First Name: <blank label=\"a\" length=\"20\"/> Last Name: <blank label=\"b\" length=\"20\"/></fib>\r\n" +
				"</items>\r\n" +
				"</form>\r\n";

			//Assertions
			Assert.AreEqual(expString, fc1.ToXml());
			Assert.AreEqual(expString, fc2.ToXml());
		}
#endif
#endif
	}
}
