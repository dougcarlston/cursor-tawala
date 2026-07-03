// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
    /// <summary>
    /// Summary description for FormTest.
    /// </summary>
    [TestFixture]
    public class FormTest
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        [Test]
        public void NewForm()
        {
            var fc = new Form("Test");

            //Assertions 
            Assert.IsNotNull(fc);
            Assert.AreEqual("Test", fc.Name);
        }

        [Test]
        public void ConstructFromXml()
        {
            const string xmlString = "<form name=\"Form 1\">" +
                                     "</form>";

            IXmlElement element = new XmlElement(xmlString);
            var form = new Form(element);

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
            var form = new Form(element);

            Assert.IsTrue(form.StartingPoint);

            xmlString =
                "<form name=\"Form 1\" startPoint=\"false\">" +
                "</form>";

            element = new XmlElement(xmlString);
            form = new Form(element);

            Assert.IsFalse(form.StartingPoint);
        }

        [Test]
        public void NameForm()
        {
            var fc = new Form("Renamed Form");

            //Assertions 
            Assert.AreEqual("Renamed Form", fc.Name);
        }

        [Test]
        public void NameFormViaConstructor()
        {
            var fc = new Form("new name");

            //Assertions 
            Assert.AreEqual("new name", fc.Name);
        }

        [Test]
        public void TrimWhitespaceFromName()
        {
            var form = new Form("    Form 1  ");
            Assert.AreEqual("Form 1", form.Name);

            form.Name = "   Renamed Form   ";
            Assert.AreEqual("Renamed Form", form.Name);
        }

        [Test]
        public void StartingPoint()
        {
            var form = new Form("Goo");
            Assert.AreEqual(false, form.StartingPoint);
            form.StartingPoint = true;
            Assert.AreEqual(true, form.StartingPoint);
            form.StartingPoint = false;
            Assert.AreEqual(false, form.StartingPoint);
        }

        [Test]
        public void AddAndRetrieveFormItems()
        {
            var fc = new Form("new name");

            var item1 = new FormItem();
            item1.Text = "Content for text item #1.";
            fc.ItemList.Add(item1);

            var item2 = new FormItem();
            item2.Text = "Different content for text item #2.";
            fc.ItemList.Add(item2);

            var retrievedItem1 = ((FormItem)fc.ItemList[0]);
            var retrievedItem2 = ((FormItem)fc.ItemList[1]);

            //Assertions 
            Assert.AreEqual("Content for text item #1.", retrievedItem1.Text);
            Assert.AreEqual("Different content for text item #2.", retrievedItem2.Text);
        }

        [Test]
        public void InsertTextItem()
        {
            var fc = new Form("new name");

            var item1 = new TextItem();
            fc.ItemList.Add(item1);

            var item2 = new TextItem();
            fc.ItemList.Add(item2);

            var item3 = new TextItem();
            fc.ItemList.Insert(1, item3);

            //Assertions 
            Assert.AreEqual(item1, fc.ItemList[0]);
            Assert.AreEqual(item3, fc.ItemList[1]);
            Assert.AreEqual(item2, fc.ItemList[2]);
        }

        [Test]
        public void InsertOnlyOneTextItem()
        {
            var fc = new Form("new name");

            var item1 = new TextItem();
            fc.ItemList.Insert(0, item1);

            //Assertions 
            Assert.AreEqual(item1, fc.ItemList[0]);
        }

        [Test]
        public void ConnectProcessToForm()
        {
            var fc = new Form("TestForm");

            var process = new Process("Connected Process");

            // connect Process
            fc.ConnectedPostProcess = process;

            //Assertions 
            Assert.AreEqual("Connected Process", fc.ConnectedPostProcess.Name);

            // now disconnect it
            fc.ConnectedPostProcess = null;

            //Assertions 
            Assert.IsNull(fc.ConnectedPostProcess);
        }

        [Test]
        public void GetFields()
        {
            var fieldNames = new[]
                             {
                                 "Q1:a",
                                 "Q2:a",
                                 "Q3",
                             };

            // create new form
            IForm form = Project.Current.AddForm();

            var textItem = new TextItem();
            form.ItemList.Add(textItem);

            // add 2 FIB items to form
            var fibItem1 = new FibItem();
            var fibItem2 = new FibItem();
            form.ItemList.Add(fibItem1);
            form.ItemList.Add(fibItem2);

            // add 1 MC item to form
            var mcItem1 = new McqItem();
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
            var fieldNames = new string[3]
                             {
                                 "Q1:a",
                                 "Q2:a",
                                 "Q3",
                             };

            // create new form
            IForm form = Project.Current.AddForm();

            var textItem = new TextItem();
            form.ItemList.Add(textItem);

            // add 2 FIB items to form
            var fibItem1 = new FibItem();
            var fibItem2 = new FibItem();
            form.ItemList.Add(fibItem1);
            form.ItemList.Add(fibItem2);

            // add 1 MC item to form
            var mcItem1 = new McqItem();
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
            var form = new Form("Test");

            // add 2 FIB items to form
            var fibItem1 = new FibItem();
            var fibItem2 = new FibItem();
            form.ItemList.Add(fibItem1);
            form.ItemList.Add(fibItem2);

            // add 1 MC item to form
            var mcItem1 = new McqItem();
            form.ItemList.Add(mcItem1);

            // add 1 text item to form
            var textItem1 = new TextItem();
            form.ItemList.Add(textItem1);

            var fileUploadItem1 = new FileUploadItem();
            form.ItemList.Add(fileUploadItem1);

            //Assertions 
            Assert.AreEqual("Q1", form.GetDefaultLabel(fibItem1));
            Assert.AreEqual("Q2", form.GetDefaultLabel(fibItem2));
            Assert.AreEqual("Q3", form.GetDefaultLabel(mcItem1));
            Assert.AreEqual("T1", form.GetDefaultLabel(textItem1));
            Assert.AreEqual("F1", form.GetDefaultLabel(fileUploadItem1));
        }

        [Test]
        public void GetSkipInstructionsFromStatement()
        {
            // create new form
            var form = new Form("Test");

            // add Skip Instruction items
            var skip1 = new SkipInstructionsItem();
            var skip2 = new SkipInstructionsItem();

            form.ItemList.Add(skip1);
            form.ItemList.Add(skip2);

            // create some statements and put them in the Skip Instructions
            var if1 = new IfStatement();
            var if2 = new IfStatement();
            var set1 = new SetStatement();
            var set2 = new SetStatement();

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
            var form = new Form("Test");

            // add some items
            form.ItemList.Add(new TextItem());
            form.ItemList.Add(new FibItem());
            form.ItemList.Add(new McqItem());

            // and a skip item
            var SkipToItem1 = new SkipInstructionsItem();
            form.ItemList.Add(SkipToItem1);

            Assert.AreEqual(null, form.ActiveSkipToItem);

            // now set the skip item as "active"
            // (this would mean that its instructions are being displayed in the Skip Instructions view)
            form.ActiveSkipToItem = SkipToItem1;
            Assert.AreEqual(SkipToItem1, form.ActiveSkipToItem);

            // create another skip item, but don't add it
            var SkipToItem2 = new SkipInstructionsItem();

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
            var fc = new Form("new name");

            // there should always be an "end of form" destination
            Assert.AreEqual(1, fc.SkipToDestinations.Count);

            // now add a couple of Form items
            var item1 = new TextItem();
            fc.ItemList.Add(item1);

            var fibItem = new FibItem();
            fc.ItemList.Add(fibItem);

            IHeadingItem heading1 = new HeadingItem();
            fc.ItemList.Add(heading1);

            Assert.AreEqual(4, fc.SkipToDestinations.Count);
        }

        [Test]
        public void GetXml()
        {
            var fc = new Form("Form Name");

            var item1 = new TextItem();
            item1.Text = "Item 1 text";
            fc.ItemList.Add(item1);

            var item2 = new TextItem();
            item2.Text = "Item 2 text";
            fc.ItemList.Add(item2);

            string expString =
                "<form name=\"Form Name\" startPoint=\"false\" themePath=\"default\" blockBackButton=\"false\">\r\n" +
                "<items>\r\n" +
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "Item 1 text" +
                "</paragraph>" +
                "</text>\r\n" +
                "<text label=\"T2\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "Item 2 text" +
                "</paragraph>" +
                "</text>\r\n" +
                "</items>\r\n" +
                "</form>\r\n";

            //Assertion
            Assert.AreEqual(expString, fc.ToXml());

            // connect a Process to the Form
            var process = new Process("Connected Process");

            fc.ConnectedPostProcess = process;

            string expStringWithProcess =
                "<form name=\"Form Name\" startPoint=\"false\" themePath=\"default\" process=\"Connected Process\" blockBackButton=\"false\">\r\n" +
                "<items>\r\n" +
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "Item 1 text" +
                "</paragraph>" +
                "</text>\r\n" +
                "<text label=\"T2\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "Item 2 text" +
                "</paragraph>" +
                "</text>\r\n" +
                "</items>\r\n" +
                "</form>\r\n";

            //Assertion
            Assert.AreEqual(expStringWithProcess, fc.ToXml());

            // disconnect the Process
            fc.ConnectedPostProcess = null;

            //Assertion
            Assert.AreEqual(expString, fc.ToXml());

            // check for illegal XML characters
            fc.Name = "&<Form's \"Bad\" Name>";
            process.Name = "&<Process's \"Bad\" Name>";
            fc.ConnectedPostProcess = process;

            expStringWithProcess =
                "<form name=\"&amp;&lt;Form&apos;s &quot;Bad&quot; Name&gt;\" startPoint=\"false\" themePath=\"default\" process=\"&amp;&lt;Process&apos;s &quot;Bad&quot; Name&gt;\" blockBackButton=\"false\">\r\n" +
                "<items>\r\n" +
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "Item 1 text" +
                "</paragraph>" +
                "</text>\r\n" +
                "<text label=\"T2\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "Item 2 text" +
                "</paragraph>" +
                "</text>\r\n" +
                "</items>\r\n" +
                "</form>\r\n";

            //Assertion
            Assert.AreEqual(expStringWithProcess, fc.ToXml());
        }

#if false // test of XML serialization; abandoned, but maybe useful in the future
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
			Form fc1 = new Form();
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
				"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">Content for text item #1.</text>\r\n" +
				"<text label=\"T2\"" + XmlConstants.DefaultTextItemStyleAttribute + ">Different content for text item #2.</text>\r\n" +
				"<fib label=\"Q1\">First Name: <blank label=\"a\" length=\"20\"></blank> Last Name: <blank label=\"b\" length=\"20\"></blank></fib>\r\n" +
				"</items>\r\n" +
				"</form>\r\n";

			//Assertions
			Assert.AreEqual(expString, fc1.ToXml());
			Assert.AreEqual(expString, fc2.ToXml());
		}
#endif
    }
}