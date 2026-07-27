// $Workfile: ProjectEvents.cs $
// $Revision: 46 $	$Date: 2/16/08 7:22p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace Tawala.Projects
{
	public class ComponentEventArgs : EventArgs
	{
		public ComponentEventArgs(IComponent comp)
		{
			component = comp;
		}

		private IComponent component;

		public IComponent Component
		{
			get
			{
				return component;
			}
		}
    }

    // naming scheme used by Microsoft sometimes for an event that is cancelable - only use for events
    // where cancel has meaning
    public class ComponentCancelEventArgs : ComponentEventArgs
    {
        public ComponentCancelEventArgs(IComponent comp)
            : base(comp)
        {
        }

        private bool canceled;

        public bool Canceled
        {
            get { return canceled; }
            set { canceled = value; }
        }
	}

	public class ProcessEventArgs : ComponentEventArgs
	{
		public ProcessEventArgs(Process process, int processLineIndex) : base(process)
		{
			this.processLineIndex = processLineIndex;
		}

		public ProcessEventArgs(Process process, int processLineIndex, ProcessStatement statement) : this(process, processLineIndex)
		{
			this.statement = statement;
		}

		public ProcessEventArgs(Process process, ProcessStatement statement) : this(process, -1, statement)
		{
		}

		public int ProcessLineIndex
		{
			get
			{
				return processLineIndex;
			}
		}

		private int processLineIndex;

		public ProcessStatement Statement
		{
			get { return statement; }
		}

		private ProcessStatement statement;
	}

	public class MCItemEventArgs : EventArgs
	{
		public MCItemEventArgs()
		{
			this.items = new IMcqItem[] { null };
		}

		public MCItemEventArgs(IMcqItem item)
		{
			this.items = new IMcqItem[] { item };
		}

		public MCItemEventArgs(IMcqItem[] items)
		{
			this.items = items;
		}

		private IMcqItem[] items;

		public IMcqItem[] Items
		{
			get
			{
				return items;
			}
		}
	}

	public class ComponentRenamedEventArgs : ComponentEventArgs
	{
		public ComponentRenamedEventArgs(IComponent comp, string oldname) : base(comp)
		{
			oldName = oldname;
		}

		private string oldName;

		public string OldName
		{
			get
			{
				return oldName;
			}
		}
	}

	public class FormItemEventArgs : EventArgs
	{
		public IForm Form
		{
			get 
			{
				return form;
			}
		}

		public IFormItem Item
		{
			get
			{
				return item;
			}
		}

		public int Order
		{
			get
			{
				return order;
			}
		}

		public FormItemEventArgs(IForm form, IFormItem formItem, int order)
		{
			this.form = form;
			this.item = formItem;
			this.order = order;
		}

		private IForm form;
		private IFormItem item;
		private int order;
	}

	public class ProjectEventArgs : EventArgs
	{
		public ProjectEventArgs(string projectName)
		{
			this.projectName = projectName;
		}

		private string projectName;

		public string ProjectName
		{
			get
			{
				return projectName;
			}
		}
	}

	public class SaveProjectEventArgs : EventArgs
	{
		public SaveProjectEventArgs(string oldProjectName, string newProjectName)
		{
			this.oldProjectName = oldProjectName;
			this.newProjectName = newProjectName;
		}

		private string oldProjectName;

		public string OldProjectName
		{
			get
			{
				return oldProjectName;
			}
		}

		private string newProjectName;

		public string NewProjectName
		{
			get
			{
				return newProjectName;
			}
		}
	}

	public class StatementEventArgs : EventArgs
	{
		public StatementEventArgs(ProcessStatement statement, Process process)
		{
			this.statement = statement;
			this.process = process;
		}

		private ProcessStatement statement;

		public ProcessStatement Statement
		{
			get
			{
				return statement;
			}
		}

		private Process process = Process.NULL;

		public Process Process
		{
			get
			{
				return process;
			}
		}
	}

	public class ProcessConnectionArgs : EventArgs
	{
		public ProcessConnectionArgs(IProcess process, IForm form)
		{
			connectedProcess = process;
			connectedForm = form;
		}

		private IProcess connectedProcess;
		public IProcess ConnectedProcess
		{
			get
			{
				return connectedProcess;
			}
		}

		private IForm connectedForm;
		public IForm ConnectedForm
		{
			get
			{
				return connectedForm;
			}
		}
	}

	public class VariableEventArgs : EventArgs
	{
		public VariableEventArgs(Variable var)
		{
			variable = var;
		}

		private Variable variable;

		public Variable Variable
		{
			get
			{
				return variable;
			}
		}
	}
	
	public class ProjectEvents
	{
		public event EventHandler<ComponentEventArgs> ComponentAdded;

		public void RaiseComponentAddedEvent(ComponentEventArgs args)
		{
			if (ComponentAdded != null)
			{
				ComponentAdded(this, args);
			}
		}

        public event EventHandler<ComponentCancelEventArgs> ComponentRemoving;

        public void RaiseComponentRemovingEvent(ComponentCancelEventArgs args)
        {
            if (ComponentRemoving != null)
            {
                ComponentRemoving(this, args);
            }
        }

        public event EventHandler<ComponentEventArgs> ComponentRemoved;

		public void RaiseComponentRemovedEvent(ComponentEventArgs args)
		{
			if (ComponentRemoved != null)
			{
				ComponentRemoved(this, args);
			}
		}

		public event EventHandler<ComponentRenamedEventArgs> ComponentRenamed;

		public void RaiseComponentRenamedEvent(ComponentRenamedEventArgs args)
		{
			if (ComponentRenamed != null)
			{
				ComponentRenamed(this, args);
			}
		}

		public event EventHandler<ComponentEventArgs> CurrentComponentSet;

		public void RaiseCurrentComponentSetEvent(ComponentEventArgs args)
		{
			// if event has any listeners...
			if(CurrentComponentSet != null)
			{
				// raise event
				CurrentComponentSet(this, args);
			}
		}

        public event EventHandler<ComponentEventArgs> ComponentSerializing;

        public void RaiseComponentSerializingEvent(ComponentEventArgs args)
        {
            if (ComponentSerializing != null)
            {
                ComponentSerializing(this, args);
            }
        }

		public event EventHandler<ComponentEventArgs> FormChanged;

		public void RaiseFormChangedEvent(ComponentEventArgs args)
		{
			if (FormChanged != null)
			{
				FormChanged(this, args);
			}
		}

		public event EventHandler<ComponentEventArgs> DocumentChanged;

		public void RaiseDocumentChangedEvent(ComponentEventArgs args)
		{
			if (DocumentChanged != null)
			{
				DocumentChanged(this, args);
			}
		}

		public event EventHandler<ProcessEventArgs> ProcessChanged;

		public void RaiseProcessChangedEvent(ProcessEventArgs args)
		{
			if (ProcessChanged != null)
			{
				ProcessChanged(this, args);
			}
		}

		public void RaiseProcessChangedEvent()
		{
			if (ProcessChanged != null)
			{
				ProcessChanged(this, new ProcessEventArgs(null, 0));
			}
		}

		public event EventHandler<EventArgs> ProcessLineIndexChanged;

		public void RaiseProcessLineIndexChangedEvent(EventArgs args)
		{
			if (ProcessLineIndexChanged != null)
			{
				ProcessLineIndexChanged(this, args);
			}
		}

		public event EventHandler<MCItemEventArgs> MCItemSelected;

		public void RaiseMCItemSelectedEvent(MCItemEventArgs args)
		{
			if (MCItemSelected != null)
			{
				MCItemSelected(this, args);
			}
		}


		public event EventHandler<ComponentEventArgs> ProcessVariableListChanged;

		public void RaiseProcessVariableListChangedEvent(ComponentEventArgs args)
		{
			if (ProcessVariableListChanged != null)
			{
				ProcessVariableListChanged(this, args);
			}
		}

		public event EventHandler<FormItemEventArgs> FormItemAdded;

		public void RaiseFormItemAddedEvent(FormItemEventArgs args)
		{
			if(FormItemAdded != null)
			{
				FormItemAdded(this, args);
			}
		}

		public event EventHandler<FormItemEventArgs> FormItemRemoved;

		public void RaiseFormItemRemovedEvent(FormItemEventArgs args)
		{
			if (FormItemRemoved != null)
			{
				FormItemRemoved(this, args);
			}
		}

		public event EventHandler<FormItemEventArgs> FormItemChanged;

		public void RaiseFormItemChangedEvent(FormItemEventArgs args)
		{
			if (FormItemChanged != null)
			{
				FormItemChanged(this, args);
			}
		}

		public event EventHandler<ProjectEventArgs> NewProject;

		public void RaiseNewProjectEvent(ProjectEventArgs args)
		{
			if(NewProject != null)
			{
				NewProject(this, args);
			}
		}

		public event EventHandler<EventArgs> SynchronizeProject;

		public void RaiseSynchronizeProjectEvent()
		{
			if (SynchronizeProject != null)
			{
				SynchronizeProject(this, EventArgs.Empty);
			}
		}

		public event EventHandler<SaveProjectEventArgs> SaveProject;

		public void RaiseSaveProjectEvent(SaveProjectEventArgs args)
		{
			if (SaveProject != null)
			{
				SaveProject(this, args);
			}
		}

		public event EventHandler<EventArgs> OpeningProject;

		public void RaiseOpeningProjectEvent()
		{
			if (OpeningProject != null)
			{
				OpeningProject(this, EventArgs.Empty);
			}
		}

		public event EventHandler<ProjectEventArgs> ProjectOpened;

		public void RaiseProjectOpenedEvent(ProjectEventArgs args)
		{
			if (ProjectOpened != null)
			{
				ProjectOpened(this, args);
			}
		}

		public event EventHandler<StatementEventArgs> StatementAdded;

		public void RaiseStatementAddedEvent(StatementEventArgs args)
		{
			if(StatementAdded != null)
			{
				StatementAdded(this, args);
			}
		}

		public event EventHandler<StatementEventArgs> StatementRemoved;

		public void RaiseStatementRemovedEvent(StatementEventArgs args)
		{
			if(StatementRemoved != null)
			{
				StatementRemoved(this, args);
			}
		}

		public event EventHandler<StatementEventArgs> StatementModified;

		public void RaiseStatementModifiedEvent(StatementEventArgs args)
		{
			if (StatementModified != null)
			{
				StatementModified(this, args);
			}
		}

		public event EventHandler<ProcessConnectionArgs> ProcessConnectedToForm;

		public void RaiseProcessConnectedToFormEvent(ProcessConnectionArgs args)
		{
			if (ProcessConnectedToForm != null)
			{
				ProcessConnectedToForm(this, args);
			}
		}

		public event EventHandler<ProcessConnectionArgs> PreProcessConnectedToForm;

		public void RaisePreProcessConnectedToFormEvent(ProcessConnectionArgs args)
		{
			if (PreProcessConnectedToForm != null)
			{
				PreProcessConnectedToForm(this, args);
			}
		}

		public event EventHandler<ProcessConnectionArgs> ProcessDisconnectedFromForm;

		public void RaiseProcessDisconnectedFromFormEvent(ProcessConnectionArgs args)
		{
			if (ProcessDisconnectedFromForm != null)
			{
				ProcessDisconnectedFromForm(this, args);
			}
		}

		public event EventHandler<ProcessConnectionArgs> PreProcessDisconnectedFromForm;

		public void RaisePreProcessDisconnectedFromFormEvent(ProcessConnectionArgs args)
		{
			if (PreProcessDisconnectedFromForm != null)
			{
				PreProcessDisconnectedFromForm(this, args);
			}
		}

		public event EventHandler<VariableEventArgs> VariableChanged;

		public void RaiseVariableChangedEvent(VariableEventArgs args)
		{
			if (VariableChanged != null)
			{
				VariableChanged(this, args);
			}
		}

		public event EventHandler<EventArgs> DeploymentInfoChanged;

		public void RaiseDeploymentInfoChangedEvent(EventArgs args)
		{
			if (DeploymentInfoChanged != null)
			{
				DeploymentInfoChanged(this, args);
			}
		}

		public event EventHandler<EventArgs> FieldProvidersChanged;

		public void RaiseFieldProvidersChangedEvent(EventArgs args)
		{
			if (FieldProvidersChanged != null)
			{
				FieldProvidersChanged(this, args);
			}
		}

		public event EventHandler<EventArgs> ThemeChanged;

		public void RaiseThemeChangedEvent()
		{
			if (ThemeChanged != null)
			{
				ThemeChanged(this, EventArgs.Empty);
			}
		}

		public event EventHandler<EventArgs> PageHeaderChanged;

		public void RaisePageHeaderChangedEvent()
		{
			if (PageHeaderChanged != null)
			{
				PageHeaderChanged(this, EventArgs.Empty);
			}
		}
	}
}
