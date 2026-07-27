using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Summary description for FormListTest.
	/// </summary>
	[TestFixture]
	public class FormListTest
	{
		[SetUp]
		public void SetUp()
		{
			TestingSupport.Util.NewTestProject();
		}

		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
				"<forms>" +
				"<form name=\"Form 1\" startPoint=\"true\">" +
				"</form>" +
				"<form name=\"Form 2\" startPoint=\"false\">" +
				"</form>" +
				"<form name=\"Form 3\" startPoint=\"true\">" +
				"</form>" +
				"</forms>";

			IXmlElement element = new XmlElement(xmlString);
			FormList list = new FormList(element);

			Assert.AreEqual(3, list.Count);
			Assert.AreEqual("Form 1", list[0].Name);
			Assert.AreEqual("Form 2", list[1].Name);
			Assert.AreEqual("Form 3", list[2].Name);
		}

		[Test]
		public void AddForm() 
		{ 
			IForm fc = new Form("Form1");

			FormList list = new FormList();
			list.Add(fc);

			//Assertions 
			Assert.IsNotNull(fc);
			Assert.AreEqual(1, list.Count);
		} 

		[Test]
		public void AddTwoForms() 
		{ 
			IForm fc1 = new Form("Form1");
            IForm fc2 = new Form("Form2");

			FormList list = new FormList();
			list.Add(fc1);
			list.Add(fc2);

			//Assertions 
			Assert.IsNotNull(fc1);
			Assert.IsNotNull(fc2);
			Assert.AreEqual(2, list.Count);
		} 

		[Test]
		public void AddTwoFormsRetrieveOne() 
		{
            IForm fc1 = new Form("name 1");
            IForm fc2 = new Form("name 2");

			FormList list = new FormList();
			list.Add(fc1);
			list.Add(fc2);

			IForm test = list[1];

			//Assertions 
			Assert.AreEqual("name 2", test.Name);
		} 

		[Test]
		public void ConnectProcessToForm() 
		{
            IForm fc1 = new Form("Form One");
            IForm fc2 = new Form("Form Two");

			FormList list = new FormList();
			list.Add(fc1);
			list.Add(fc2);

			Process process = new Process("Connected Process");

			// connect the Process to Form 2
			list.ConnectProcessToForm(process, fc2.Name);

			//Assertions 
			Assert.AreEqual(null, fc1.ConnectedProcess);
			Assert.AreEqual("Connected Process", fc2.ConnectedProcess.Name);

			// now "disconnect" the Process from Form 1
			// should have no effect, since there was no Process connected to that Form
			list.DisconnectProcessFromForm(fc1.Name);

			//Assertions 
			Assert.AreEqual(null, fc1.ConnectedProcess);
			Assert.AreEqual("Connected Process", fc2.ConnectedProcess.Name);

			// OK, now really disconnect the Process from Form 2
			list.DisconnectProcessFromForm(fc2.Name);

			//Assertions 
			Assert.AreEqual(null, fc1.ConnectedProcess);
			Assert.AreEqual(null, fc2.ConnectedProcess);
		}

		[Test]
		public void DisconnectProcessFromAllForms() 
		{ 
			IForm fc1 = new Form("Form One");
			IForm fc2 = new Form("Form Two");

			FormList list = new FormList();
			list.Add(fc1);
			list.Add(fc2);

			Process process1 = new Process("Connected Process");

			Process process2 = new Process("Unconnected Process");

			// connect process1 to both Forms
			list.ConnectProcessToForm(process1, fc1.Name);
			list.ConnectProcessToForm(process1, fc2.Name);

			//Assertions 
			Assert.AreEqual("Connected Process", fc1.ConnectedProcess.Name);
			Assert.AreEqual("Connected Process", fc2.ConnectedProcess.Name);

			// now "disconnect" the process2 from all Forms
			// should have no effect, since process2 was never connected to any Form
			list.DisconnectProcessFromAllForms(process2);

			//Assertions 
			Assert.AreEqual("Connected Process", fc1.ConnectedProcess.Name);
			Assert.AreEqual("Connected Process", fc2.ConnectedProcess.Name);

			// OK, now really disconnect process1 from Form 2
			list.DisconnectProcessFromAllForms(process1);

			//Assertions 
			Assert.AreEqual(null, fc1.ConnectedProcess);
			Assert.AreEqual(null, fc2.ConnectedProcess);
		}

		// handlers for "process connected to form" and "process disconnected from form" events
		// Note: Do not use [Test] attribute for event handlers
		public void HandleProcessConnectedToFormEvent(object sender, ProcessConnectionArgs args)
		{
			Assert.AreEqual("Connected Process", args.ConnectedProcess.Name);
			Assert.AreEqual("Test Form", args.ConnectedForm.Name);
		}
		public void HandleProcessDisconnectedFromFormEvent(object sender, ProcessConnectionArgs args)
		{
			Assert.AreEqual("Connected Process", args.ConnectedProcess.Name);
			Assert.AreEqual("Test Form", args.ConnectedForm.Name);
		}

		[Test]
		public void ConnectProcessToFormEvents()
		{
			// add event handlers for connecting and disconnecting
			Project.Events.ProcessConnectedToForm += HandleProcessConnectedToFormEvent;
			Project.Events.ProcessDisconnectedFromForm += HandleProcessDisconnectedFromFormEvent;

			IForm form = Project.Current.AddForm();
			form.Name = "Test Form";

			Process process = Project.Current.AddProcess();
			process.Name = "Connected Process";

			// connect the Process to Form; the above event handler will do the Assertions
			Project.Current.ConnectProcessToForm(process, form.Name);

			// now disconnect the Process; the above event handler will do the Assertions
			Project.Current.DisconnectProcessFromForm(form.Name);
		}

		// handler for "process disconnected from form" event
		// Note: Do not use [Test] attribute for event handlers
		static int pass = 1;
		public void HandleProcessDisconnectedFromAllFormsEvent(object sender, ProcessConnectionArgs args)
		{
			Assert.AreEqual("Connected Process", args.ConnectedProcess.Name);

			// a different form name on each call
			if (pass++ == 1)
			{
				Assert.AreEqual("Form One", args.ConnectedForm.Name);
			}
			else
			{
				Assert.AreEqual("Form Two", args.ConnectedForm.Name);
			}
		}

		[Test]
		public void DisconnectProcessFromAllFormsEvent()
		{
			// start with new project
			TestingSupport.Util.NewTestProject();
			pass = 1;

			// add event handlers for connecting and disconnecting
			Project.Events.ProcessDisconnectedFromForm += HandleProcessDisconnectedFromAllFormsEvent;

			IForm form1 = Project.Current.AddForm();
			IForm form2 = Project.Current.AddForm();
			form1.Name = "Form One";
			form2.Name = "Form Two";

			Process process = Project.Current.AddProcess();
			process.Name = "Connected Process";

			// connect the Process to both Forms
			Project.Current.ConnectProcessToForm(process, form1.Name);
			Project.Current.ConnectProcessToForm(process, form2.Name);

			// now disconnect the Process from both Forms (by deleting it
			// the above event handler will do the Assertions
			Project.Current.RemoveProcess(process.Name);
		}

        [Ignore("Either convert test to use a different property or eliminate it if it duplicates another test")]
		[Test]
		public void GetXml() 
		{ 
			ITextItem item1 = new NewTextItem();
	//		item1.Text = "Item 1 text";

			ITextItem item2 = new NewTextItem();
	//		item2.Text = "Item 2 text";

            IForm fc1 = new Form("Form 1");
			fc1.ItemList.Add(item1);
			fc1.ItemList.Add(item2);

            IForm fc2 = new Form("Form 2");

			FormList fcList = new FormList();
			fcList.Add(fc1);
			fcList.Add(fc2);

			string expString =
				"<forms>\r\n" +
				"<form name=\"Form 1\" startPoint=\"false\">" +
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
				"</form>" +
				"<form name=\"Form 2\" startPoint=\"false\">" +
				"</form>" +
				"</forms>\r\n";

			//Assertion
			Assert.AreEqual(expString, fcList.ToXml());

			// connect a Process to Form 2
			Process process = new Process("Connected Process");

			fcList.ConnectProcessToForm(process, fc2.Name);

			string expStringWithProcess =
				"<forms>\r\n" +
				"<form name=\"Form 1\" startPoint=\"false\">" +
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
				"</form>" +
				"<form name=\"Form 2\" startPoint=\"false\" process=\"Connected Process\">" +
				"</form>" +
				"</forms>\r\n";

			//Assertion
			Assert.AreEqual(expStringWithProcess, fcList.ToXml());

			// disconnect the Process
			fcList.DisconnectProcessFromForm(fc2.Name);

			//Assertion
			Assert.AreEqual(expString, fcList.ToXml());
		}

		[Test]
		public void GetXmlWhenEmpty() 
		{ 
			// make list with no forms
			FormList fcList = new FormList();

			//Assertions 
			Assert.AreEqual("", fcList.ToXml());
		}
	}
}
