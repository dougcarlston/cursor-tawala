using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class ProjectEventsTest 
	{
		private ProjectEvents projectEvents;
		string raisedMethodName = "Initial Value";

		[SetUp]
		public void SetUp()
		{
			TestingSupport.Util.NewTestProject();

			// make sure there's a ProjectEvents object
			projectEvents = new ProjectEvents();
			raisedMethodName = "Not Set";
		}

		// handler 1 for "document added" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleDocumentAddedEvent1(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("Document 1", args.Component.Name);
		}

		// handler 2 for "document added" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleDocumentAddedEvent2(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("Document 2", args.Component.Name);
		}

		[Test]
		public void DocumentAddedEvent() 
		{ 
			// attach one event handler to "document added" event
            projectEvents.ComponentAdded += HandleDocumentAddedEvent1;

			// create new document and raise "document added" event
			IDocument doc1 = new NewDocument("Document 1");
			projectEvents.RaiseComponentAddedEvent(new ComponentEventArgs(doc1));

			// detach one event handler from "document added" event
			projectEvents.ComponentAdded -= HandleDocumentAddedEvent1;

			// attach other event handler to "document added" event
			projectEvents.ComponentAdded += HandleDocumentAddedEvent2;

			// create new document and raise "document added" event
			IDocument doc2 = new NewDocument("Document 2");
			projectEvents.RaiseComponentAddedEvent(new ComponentEventArgs(doc2));
		} 

		// handler for "document removed" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleDocumentRemovedEvent(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("MyDocument 1", args.Component.Name);
		}

		[Test]
		public void DocumentRemovedEvent() 
		{ 
			// attach event handler to "document removed" event
			projectEvents.ComponentRemoved += HandleDocumentRemovedEvent;

			// create new document and raise "document removed" event
			IDocument doc1 = new NewDocument("MyDocument 1");
			projectEvents.RaiseComponentRemovedEvent(new ComponentEventArgs(doc1));
		} 

		// handler for "document renamed" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleDocumentRenamedEvent(object sender, ComponentRenamedEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("MyDocument 1", args.OldName);
			Assert.AreEqual("MyDocument 1 Renamed", args.Component.Name);
		}

		[Test]
		public void DocumentRenamedEvent() 
		{ 
			// attach event handler to "document removed" event
			projectEvents.ComponentRenamed +=  HandleDocumentRenamedEvent;

			// create new document, rename it and raise "document renamed" event
			IDocument doc1 = new NewDocument("MyDocument 1");
			doc1.Name = "MyDocument 1 Renamed";
			projectEvents.RaiseComponentRenamedEvent(new ComponentRenamedEventArgs(doc1, "MyDocument 1"));
		} 

		// handler for "current document set" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleCurrentComponentSetEventDocument(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("Document 1", args.Component.Name);
		}

		[Test]
		public void CurrentComponentSetEventDocument() 
		{ 
			// attach event handler to "document removed" event
			projectEvents.CurrentComponentSet += HandleCurrentComponentSetEventDocument;

			// create new document and raise "current document set" event
			IDocument doc1 = new NewDocument("Document 1");
			projectEvents.RaiseCurrentComponentSetEvent(new ComponentEventArgs(doc1));
		} 

		
		// handler 1 for "form added" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleFormAddedEvent1(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("Form 1", args.Component.Name);
		}

		// handler 2 for "form added" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleFormAddedEvent2(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("Form 2", args.Component.Name);
		}

		[Test]
		public void FormAddedEvent() 
		{ 
			projectEvents.ComponentAdded += HandleFormAddedEvent1;

			IForm form1 = new Form("Form 1");
			projectEvents.RaiseComponentAddedEvent(new ComponentEventArgs(form1));

			// detach one event handler from "form added" event
			projectEvents.ComponentAdded -= HandleFormAddedEvent1;

			// attach other event handler to "form added" event
			projectEvents.ComponentAdded += HandleFormAddedEvent2;

			// create new form and raise "form added" event
            IForm form2 = new Form("Form 2");
			projectEvents.RaiseComponentAddedEvent(new ComponentEventArgs(form2));
		}

        public void handleFormRemovingEvent(object sender, ComponentCancelEventArgs args)
        {
            Assert.IsFalse(args.Canceled);
            args.Canceled = true;
            Assert.IsTrue(args.Canceled);
        }

        [Test]
        public void FormRemovingEvent()
        {
            projectEvents.ComponentRemoving += handleFormRemovingEvent;

            IForm form11 = new Form("Cancel Me");
            ComponentCancelEventArgs args = new ComponentCancelEventArgs(form11);
            Assert.IsFalse(args.Canceled);
            projectEvents.RaiseComponentRemovingEvent(args);
            Assert.IsTrue(args.Canceled);
        }

		// handler for "form removed" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleFormRemovedEvent(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("MyForm 1", args.Component.Name);
		}

		[Test]
		public void FormRemovedEvent() 
		{ 
			// attach event handler to "form removed" event
			projectEvents.ComponentRemoved += HandleFormRemovedEvent;

			// create new form and raise "form removed" event
			IForm form1 = new Form("MyForm 1");
			projectEvents.RaiseComponentRemovedEvent(new ComponentEventArgs(form1));
		} 

		// handler for "form renamed" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleFormRenamedEvent(object sender, ComponentRenamedEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("MyForm 1", args.OldName);
			Assert.AreEqual("MyForm 1 Renamed", args.Component.Name);
		}

		[Test]
		public void FormRenamedEvent() 
		{ 
			projectEvents.ComponentRenamed += HandleFormRenamedEvent;

			IForm form1 = new Form("MyForm 1");
			form1.Name = "MyForm 1 Renamed";
			projectEvents.RaiseComponentRenamedEvent(new ComponentRenamedEventArgs(form1, "MyForm 1"));
		} 

		// handler for "current form set" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleCurrentComponentSetEventForm(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("Form 1", args.Component.Name);
		}

		[Test]
		public void CurrentComponentSetEventForm() 
		{ 
			projectEvents.CurrentComponentSet += HandleCurrentComponentSetEventForm;

			IForm form1 = new Form("Form 1");
			projectEvents.RaiseCurrentComponentSetEvent(new ComponentEventArgs(form1));
		} 
		
		public void HandleNewTextItemEvent(object sender, FormItemEventArgs args)
		{
			Assert.IsNotNull(args.Item as ITextItem);
		}

		[Test]
		public void NewTextItemEvent() 
		{ 
			// attach event handler
			projectEvents.FormItemAdded += HandleNewTextItemEvent;

			// create new text item and raise event
			ITextItem item = new NewTextItem();
			projectEvents.RaiseFormItemAddedEvent(new FormItemEventArgs(null, item, 0));
		}
	
		// handler for "new FIB item" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleNewFibItemEvent(object sender, FormItemEventArgs args)
		{
			Assert.IsNotNull(args.Item as IFibItem);

			IFibItem item = (IFibItem)(args.Item);
			Assert.AreEqual(1, item.BlankList.Count);
		}

		// handler for "remove text item" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleTextItemRemovedEvent(object sender, FormItemEventArgs args)
		{
			Assert.IsNotNull(args.Item as ITextItem);
		}

		[Test]
		public void RemoveTextItemEvent() 
		{ 
			// attach event handler
			projectEvents.FormItemRemoved += HandleTextItemRemovedEvent;

			// create new text item and raise event
			ITextItem item = new NewTextItem();
			projectEvents.RaiseFormItemRemovedEvent(new FormItemEventArgs(null, item, 0));
		}
	
		[Test]
		public void NewFibItemEvent() 
		{ 
			// make sure there's a ProjectEvents object
			ProjectEvents projectEvents = new ProjectEvents();

			// attach event handler
			projectEvents.FormItemAdded += HandleNewFibItemEvent;

			// create new FIB item and raise event
			IFibItem item = new NewFibItem();
			projectEvents.RaiseFormItemAddedEvent(new FormItemEventArgs(null, item, 0));
		}

		// handler for "new MC item" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleNewMCItemEvent(object sender, FormItemEventArgs args)
		{
			Assert.IsNotNull(args.Item as IMcqItem);
		}

		[Test]
		public void NewMCItemEvent() 
		{ 
			// attach event handler
			projectEvents.FormItemAdded += HandleNewMCItemEvent;

			// create new FIB item and raise event
			IMcqItem item = new NewMcqItem();
			projectEvents.RaiseFormItemAddedEvent(new FormItemEventArgs(null, item, 0));
		}

		// handler 1 for "process added" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleProcessAddedEvent1(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("Process 1", args.Component.Name);
		}

		// handler 2 for "process added" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleProcessAddedEvent2(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("Process 2", args.Component.Name);
		}

		[Test]
		public void ProcessAddedEvent() 
		{ 
			// attach one event handler to "process added" event
			projectEvents.ComponentAdded += HandleProcessAddedEvent1;

			// create new process and raise "process added" event
			Process process1 = new Process("Process 1");
			projectEvents.RaiseComponentAddedEvent(new ComponentEventArgs(process1));

			// detach one event handler from "process added" event
			projectEvents.ComponentAdded -= HandleProcessAddedEvent1;

			// attach other event handler to "process added" event
			projectEvents.ComponentAdded +=HandleProcessAddedEvent2;

			// create new process and raise "process added" event
			Process process2 = new Process("Process 2");
			projectEvents.RaiseComponentAddedEvent(new ComponentEventArgs(process2));
		} 

		// handler for "process removed" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleProcessRemovedEvent(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("MyProcess 1", args.Component.Name);
		}

		[Test]
		public void ProcessRemovedEvent() 
		{ 
			projectEvents.ComponentRemoved +=HandleProcessRemovedEvent;

			Process process1 = new Process("MyProcess 1");
			projectEvents.RaiseComponentRemovedEvent(new ComponentEventArgs(process1));
		} 

		public void HandleProcessRenamedEvent(object sender, ComponentRenamedEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("MyProcess 1", args.OldName);
			Assert.AreEqual("MyProcess 1 Renamed", args.Component.Name);
		}

		[Test]
		public void ProcessRenamedEvent() 
		{ 
			projectEvents.ComponentRenamed += HandleProcessRenamedEvent;

			// create new process, rename it and raise "process renamed" event
			Process process1 = new Process("MyProcess 1");
			process1.Name = "MyProcess 1 Renamed";
			projectEvents.RaiseComponentRenamedEvent(new ComponentRenamedEventArgs(process1, "MyProcess 1"));
		} 

		// handler for "current process set" event
		// Note: Do not use [Test] attribute for event handlers
		public void HandleCurrentComponentSetEventProcess(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("Process 1", args.Component.Name);
		}

		[Test]
		public void CurrentComponentSetEventProcess() 
		{ 
			// attach event handler to "process removed" event
			projectEvents.CurrentComponentSet += HandleCurrentComponentSetEventProcess;

			// create new process and raise "current process set" event
			Process process1 = new Process("Process 1");
			projectEvents.RaiseCurrentComponentSetEvent(new ComponentEventArgs(process1));
		} 

		private void handleNewProjectEvent(object sender, ProjectEventArgs args)
		{
			raisedMethodName = new System.Diagnostics.StackFrame(false).GetMethod().Name;
			Assert.IsNotNull(args.ProjectName);
			Assert.AreEqual("New Project", args.ProjectName);
		}

		[Test]
		public void NewProjectEvent() 
		{
			projectEvents.NewProject += handleNewProjectEvent;
			projectEvents.RaiseNewProjectEvent(new ProjectEventArgs("New Project"));
			Assert.AreEqual("handleNewProjectEvent", raisedMethodName);
		}

		private void handleProjectOpenedEvent(object sender, ProjectEventArgs args)
		{
			raisedMethodName = new System.Diagnostics.StackFrame(false).GetMethod().Name;
			Assert.AreEqual("Open Project", args.ProjectName);
		}

		[Test]
		public void OpenProjectEvent()
		{
			projectEvents.ProjectOpened += handleProjectOpenedEvent;
			projectEvents.RaiseProjectOpenedEvent(new ProjectEventArgs("Open Project"));
			Assert.AreEqual("handleProjectOpenedEvent", raisedMethodName);
		}

		private void handleSynchronizeProjectEvent(object sender, EventArgs args)
		{
			raisedMethodName = new System.Diagnostics.StackFrame(false).GetMethod().Name;
		}

		[Test]
		public void SynchronizeProjectEvent()
		{
			projectEvents.SynchronizeProject += handleSynchronizeProjectEvent;
			projectEvents.RaiseSynchronizeProjectEvent();
			Assert.AreEqual("handleSynchronizeProjectEvent", raisedMethodName);
		}

		private void handleSaveProjectEvent(object sender, SaveProjectEventArgs args)
		{
			raisedMethodName = new System.Diagnostics.StackFrame(false).GetMethod().Name;
			Assert.AreEqual("Old Project Name", args.OldProjectName);
			Assert.AreEqual("New Project Name", args.NewProjectName);
		}

		[Test]
		public void SaveProjectEvent()
		{
			projectEvents.SaveProject += handleSaveProjectEvent;
			projectEvents.RaiseSaveProjectEvent(new SaveProjectEventArgs("Old Project Name", "New Project Name"));
			Assert.AreEqual("handleSaveProjectEvent", raisedMethodName);
		}

		private void handleStatementAddedEvent(object sender, StatementEventArgs args)
		{
			raisedMethodName = new System.Diagnostics.StackFrame(false).GetMethod().Name;
			Assert.IsNotNull(args.Statement);
			Assert.AreEqual("Show Document New Document X", args.Statement.ToString());
			Assert.IsNotNull(args.Process);
			Assert.AreEqual("Test Process", args.Process.Name);
		}

		[Test]
		public void StatementAddedEvent() 
		{ 
			projectEvents.StatementAdded += handleStatementAddedEvent;

			ProcessStatement st1 = new ShowDocumentStatement(new NewDocument("New Document X"));
			Process proc = new Process("Test Process");
			projectEvents.RaiseStatementAddedEvent(new StatementEventArgs(st1, proc));

			Assert.AreEqual("handleStatementAddedEvent", raisedMethodName);
		} 

		private void handleStatementRemovedEvent(object sender, StatementEventArgs args)
		{
			raisedMethodName = new System.Diagnostics.StackFrame(false).GetMethod().Name;
			Assert.IsNotNull(args.Statement);
			Assert.AreEqual("Show Document Document Y", args.Statement.ToString());
			Assert.IsNotNull(args.Process);
			Assert.AreEqual("Test Process", args.Process.Name);
		}

		[Test]
		public void StatementRemovedEvent() 
		{ 
			projectEvents.StatementRemoved += handleStatementRemovedEvent;

			ProcessStatement st1 = new ShowDocumentStatement(new NewDocument("Document Y"));
			Process proc = new Process("Test Process");
			projectEvents.RaiseStatementRemovedEvent(new StatementEventArgs(st1, proc));

			Assert.AreEqual("handleStatementRemovedEvent", raisedMethodName);
		} 

		private void handleStatementModifiedEvent(object sender, StatementEventArgs args)
		{
			raisedMethodName = new System.Diagnostics.StackFrame(false).GetMethod().Name;
			Assert.IsNotNull(args.Statement);
			Assert.AreEqual("Show Document Document Z", args.Statement.ToString());
			Assert.IsNotNull(args.Process);
			Assert.AreEqual("Test Process", args.Process.Name);
		}

		[Test]
		public void StatementModifiedEvent() 
		{ 
			projectEvents.StatementModified += handleStatementModifiedEvent;

			ProcessStatement st1 = new ShowDocumentStatement(new NewDocument("Document Z"));
			Process proc = new Process("Test Process");
			projectEvents.RaiseStatementModifiedEvent(new StatementEventArgs(st1, proc));

			Assert.AreEqual("handleStatementModifiedEvent", raisedMethodName);
		}

		private void handleProcessConnectedToFormEvent(object sender, ProcessConnectionArgs args)
		{
			raisedMethodName = new System.Diagnostics.StackFrame(false).GetMethod().Name;
			Assert.AreEqual("Process One", args.ConnectedProcess.Name);
			Assert.AreEqual("Form One", args.ConnectedForm.Name);
		}

		[Test]
		public void ProcessConnectedToFormEvent()
		{
			projectEvents.ProcessConnectedToForm += handleProcessConnectedToFormEvent;

			IForm form = new Form("Form One");
			Process process = new Process("Process One");
			form.ConnectedProcess = process;

			projectEvents.RaiseProcessConnectedToFormEvent(new ProcessConnectionArgs(process, form));

			Assert.AreEqual("handleProcessConnectedToFormEvent", raisedMethodName);
		}

		private void handleProcessDisconnectedFromFormEvent(object sender, ProcessConnectionArgs args)
		{
			raisedMethodName = new System.Diagnostics.StackFrame(false).GetMethod().Name;
			Assert.AreEqual("Process One", args.ConnectedProcess.Name);
			Assert.AreEqual("Form One", args.ConnectedForm.Name);
		}

		[Test]
		public void ProcessDisonnectedFromFormEvent()
		{
			projectEvents.ProcessDisconnectedFromForm += handleProcessDisconnectedFromFormEvent;

			IForm form = new Form("Form One");
			Process process = new Process("Process One");

			projectEvents.RaiseProcessDisconnectedFromFormEvent(new ProcessConnectionArgs(process, form));

			Assert.AreEqual("handleProcessDisconnectedFromFormEvent", raisedMethodName);
		}

		public void HandleFormChangedEvent(object sender, ComponentEventArgs args)
		{
			Assert.AreEqual("Form 1", args.Component.Name);
		}

		[Test]
		public void FormChangedEvent()
		{
			projectEvents.FormChanged += HandleFormChangedEvent;

			IForm form1 = new Form("Form 1");
			projectEvents.RaiseFormChangedEvent(new ComponentEventArgs(form1));
		}

		public void HandleFormItemChangedEvent(object sender, FormItemEventArgs args)
		{
			Assert.AreEqual("Form 1", args.Form.Name);
			Assert.AreEqual("FIB Item 1", args.Item.AlternateLabel);
			Assert.AreEqual(0, args.Order);
		}

		[Test]
		public void FormItemChangedEvent()
		{
			projectEvents.FormItemChanged += HandleFormItemChangedEvent;

			IForm form1 = new Form("Form 1");
			IFibItem fibItem1 = new NewFibItem();
			fibItem1.AlternateLabel = "FIB Item 1";
			form1.ItemList.Add(fibItem1);
			projectEvents.RaiseFormItemChangedEvent(new FormItemEventArgs(form1, fibItem1, 0));
		}


		public void HandleDocumentChangedEvent(object sender, ComponentEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("Document 1", args.Component.Name);
		}

		[Test]
		public void DocumentChangedEvent()
		{
			projectEvents.DocumentChanged += HandleDocumentChangedEvent;

			IDocument document1 = new NewDocument("Document 1");
			projectEvents.RaiseDocumentChangedEvent(new ComponentEventArgs(document1));
		}

		public void HandleProcessChangedEvent(object sender, ProcessEventArgs args)
		{
			Assert.IsNotNull(args.Component);
			Assert.AreEqual("Process 1", args.Component.Name);
		}

		[Test]
		public void ProcessChangedEvent()
		{
			projectEvents.ProcessChanged += HandleProcessChangedEvent;

			Process process1 = new Process("Process 1");
			projectEvents.RaiseProcessChangedEvent(new ProcessEventArgs(process1, 0));
		}

		public void HandleVariableChangedEvent(object sender, VariableEventArgs args)
		{
			Assert.IsNotNull(args.Variable);
			Assert.AreEqual("Variable 1", args.Variable.FieldName);
		}

		[Test]
		public void VariableChangedEvent()
		{
			// attach event handler to "process changed" event
			projectEvents.VariableChanged += HandleVariableChangedEvent;

			// create new process and raise "process changed" event
			Variable variable1 = new Variable("Variable 1");
			projectEvents.RaiseVariableChangedEvent(new VariableEventArgs(variable1));
		}

		// handler for "deployment info changed" event
		private void handleDeploymentInfoChangedEvent(object sender, EventArgs args)
		{
			raisedMethodName = new System.Diagnostics.StackFrame(false).GetMethod().Name;
			Assert.AreSame(EventArgs.Empty, args);
		}

		[Test]
		public void DeploymentInfoChangedEvent()
		{
			// attach event handler to "process changed" event
			projectEvents.DeploymentInfoChanged += handleDeploymentInfoChangedEvent;
			projectEvents.RaiseDeploymentInfoChangedEvent(EventArgs.Empty);
			Assert.AreEqual("handleDeploymentInfoChangedEvent", raisedMethodName);
		}

		[Test]
		public void ThemeChangedEvent()
		{
			projectEvents.ThemeChanged += projectEvents_ThemeChanged;

			Assert.AreEqual(0, themeChangedCount);

			projectEvents.RaiseThemeChangedEvent();

			Assert.AreEqual(1, themeChangedCount);
		}

		private int themeChangedCount = 0;

		private void projectEvents_ThemeChanged(object sender, EventArgs e)
		{
			themeChangedCount++;
		}
	} 
}
