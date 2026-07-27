// $Workfile: StatementView.cs $
// $Revision: 66 $	$Date: 12/12/07 4:57p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Tawala.Projects;

namespace Tawala.Processes
{
	/// <summary>
	/// Statement Details base class.
	/// This class would be abstract but the VSN Form Designer doesn't like abstract base classes!!
	/// </summary>
	public class StatementView<T> : System.Windows.Forms.UserControl, IStatementEditor, IStatementView where T : ProcessStatement, new()
	{
		protected static readonly Color SelectedColor = Color.FromArgb(210, 255, 210);
		protected static readonly Color UnselectedColor = Color.White;

		protected static readonly Type statementType = typeof(T);

		// currently duplicates StatementType property but StatementType might change
		public Type BaseStatementType
		{
			get
			{
				return statementType;
			}
		}

		public Type StatementType
		{
			get
			{
				return statementType;
			}
		}

		protected virtual void NewStatement()
		{
			statement = new T();
		}

		public ProcessEditor ParentView
		{
			get
			{
				Control parent = Parent;
				while (parent != null)
				{
					if (parent is ProcessEditor)
						return parent as ProcessEditor;
					parent = parent.Parent;
				}
				return null;
			}
		}

		protected SplitContainer GetSplitContainer()
		{
			Control parentControl = Parent;

			while (parentControl != null)
			{
				if (parentControl is SplitContainer)
				{
					return parentControl as SplitContainer;
				}
				else
				{
					parentControl = parentControl.Parent;
				}
			}

			return new SplitContainer();
		}


		// holds a reference of the statement being edited (for derived classes)
		protected T statement = null;

		public void SwitchToAddMode()
		{
			if (modifyMode)
			{
				modifyMode = false;
				NewStatement();
				setAddModifyButtons();
			}
		}

        private TabControl tabControl;

		public TabControl TabControl
		{
			get
			{
				return tabControl;
			}
		}

		public StatementView()
		{
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			Project.Events.ProcessConnectedToForm += processConnectedToForm;
			Project.Events.ProcessDisconnectedFromForm += processConnectedToForm;
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);

			if (e.Control is TabControl)
			{
				tabControl = e.Control as TabControl;
                tabControl.TabStop = (tabControl.TabCount > 1);
                tabControl.MouseClick += new MouseEventHandler(tabControl_MouseClick);
            }
		}

        void tabControl_MouseClick(object sender, MouseEventArgs e)
        {
            SelectNextControl(tabControl, true, true, true, false);
        }

 		private void processConnectedToForm(object sender, ProcessConnectionArgs e)
		{
			OnProcessFormConnectionChange(e);
		}
		
		/// <summary>
		/// Derived classes can override this is interested in this event
		/// Base currently does nothing so no need to call it.
		/// </summary>
		protected virtual void OnProcessFormConnectionChange(ProcessConnectionArgs e)
		{
		}

		private bool modifyMode = false;

		// modifyMode is only set initially internally by the control
		// you cannot switch to modify mode from add mode for a given edit.
		// To switch to modify mode Edit() the statement to be modified.
		// You can switch from modifyMode to add mode (!modifyMode)
		protected bool ModifyMode
		{
			get
			{
				return modifyMode;
			}
		}

		/// <summary>
		/// Edit an existing or new process statement
		/// </summary>
		/// <param name="statement">null for new statement, otherwise statement to edit</param>
		public void Edit(ProcessStatement ps)
		{
			statement = ps as T;
			modifyMode = statement != null;

			if (!modifyMode)
			{
				NewStatement();
			}

			setAddModifyButtons();

			// cause FieldSource-bound controls to update
			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(ParentView.Process, ParentView.InsertionPoint, statement));

			setDetailsPanelHeight(188);

			SelectNextControl(tabControl, true, true, true, false);

			// call derived class to handle edit
			OnEdit();
		}

		protected override Size DefaultSize
		{
			get { return new Size(432, 176); }
		}

		protected void setDetailsPanelHeight(int height)
		{
			ParentView.SplitContainer.SplitterDistance = height;
		}


		/// <summary>
		/// Called when editing a new or existing statement.
		/// Derived classes must override this. Would be abstract if VSN Form Designer allowed it.
		/// </summary>
		protected virtual void OnEdit()
		{
			throw new NotImplementedException("Details<T>.OnEdit() must be overridden");
		}

		/// <summary>
		/// View calls this if details panel is active and Application Idle event occurs.
		/// Used for keeping UI states up-to-date
		/// </summary>
		public void DoIdle()
		{
			OnIdle();
		}

		/// <summary>
		/// Update UI state for Details panel.
		/// Derived classes must override this. Would be abstract if VSN Form Designer allowed it.
		/// </summary>
		protected virtual void OnIdle()
		{
			throw new NotImplementedException("Details<T>.OnIdle() must be overridden");
		}

		/// <summary>
		/// Add new statement or modify existing one.
		/// </summary>
		protected void SaveStatement()
		{
			if (modifyMode)
			{
				ParentView.ModifyStatement(statement);
			}
			else
			{
				ParentView.AddStatement(statement);

				// immediately create new statement after adding new one to project
				// we are still in add mode and so if add is pressed again we want
				// to add another new statement
				NewStatement();
			}
		}

		/// <summary>
		/// Remembers the Add or Modify action for subsequent undo
		/// </summary>
		protected void RememberAction()
		{
			string actionText = (modifyMode ? "Modify" : "Add");
			ParentView.RememberAction(actionText);
		}

		/// <summary>
		/// Wrapper that calls ultimate function up parent window chain
		/// insulating derived classes from details of window hierarchy.
		/// </summary>
		/// <returns></returns>
		protected bool AtInsertPosition()
		{
			return (ParentView == null ? true : ParentView.AtInsertPosition());
		}

		/// <summary>
		/// Derived classes can override if they are interested in this event
		/// </summary>
		protected virtual void OnCurrentComponentSet(Tawala.Projects.Component c)
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!DesignMode)
			{
				Dock = DockStyle.Fill;
			}
		}

		/// <summary>
		/// This updates a binding source based data source
		/// </summary>
		/// <remarks>REVISIT: 0 items causes combo box to throw exception</remarks>
		/// <param name="mayBeNewRef"></param>
		protected void UpdateDataSource(BindingSource bs, Object mayBeNewRef)
		{
			try
			{
				if (bs.DataSource != mayBeNewRef)
				{
					try
					{
						bs.DataSource = mayBeNewRef; // ignore first exception should it occur
					}
					catch
					{
						bs.DataSource = mayBeNewRef;
					}
				}
				bs.ResetBindings(false);
			}
			catch
			{
			}
		}

		/// <summary>
		/// Set a combo box data source to an IList.
		/// Use when not using a BindingSource
		/// </summary>
		protected void SetDataSource(ComboBox cb, IList list)
		{
			cb.DataSource = null;

			if (list != null && list.Count > 0)
			{
				if (cb.SelectedIndex >= list.Count)
				{
					// keep the index in range if the list has gotten smaller
					cb.SelectedIndex = list.Count - 1;
				}
				cb.DataSource = list;
			}
		}

		/// <summary>
		/// Set a combo box data source to a FlatFieldList.
		/// </summary>
		protected void SetDataSource(ComboBox cb, FlatFieldList list)
		{
			cb.DataSource = null;

			if (list != null && list.Count > 0)
			{
				if (cb.SelectedIndex >= list.Count)
				{
					// keep the index in range if the list has gotten smaller
					cb.SelectedIndex = list.Count - 1;
				}
				cb.DataSource = list;
			}
		}

		/// <summary>
		/// Sync all add/modify buttons to modifyMode flag boolean
		/// </summary>
		private void setAddModifyButtons()
		{
			Debug.Assert(tabControl != null);

			foreach (TabPage page in tabControl.TabPages)
			{
				foreach (Control c in page.Controls)
				{
					AddModifyButton b = c as AddModifyButton;

					if (b != null)
					{
						b.Modify = modifyMode;
					}
				}
			}
		}

		/// <summary>
		/// ProcessEditor calls these when its parent MDI window (the Process window) is activated / deactivated
		/// </summary>
		public void MDIWindowActivated()
		{
			OnMdiWindowActivated();
		}

		public void MDIWindowDeactivated()
		{
			OnMdiWindowDeactivated();
		}

		protected virtual void OnMdiWindowActivated()
		{
		}

		protected virtual void OnMdiWindowDeactivated()
		{
		}


		protected override bool ProcessKeyPreview(ref Message m)
		{
			if (enterKeyPressed(m))
			{
				foreach (Control control in tabControl.SelectedTab.Controls)
				{
					AddModifyButton addModifyButton = control as AddModifyButton;

					if (addModifyButton != null)
					{
						if (!addModifyButton.Focused)
						{
							addModifyButton.PerformClick();
							return true;
						}
					}
				}
			}

			return base.ProcessKeyPreview(ref m);
		}

		const int WM_CHAR = 0x102;

		private bool enterKeyPressed(Message m)
		{
			return ((m.Msg == WM_CHAR) && (int)m.WParam == 0x0d);
		}


        #region IStatementView Members

        public Tawala.Projects.Process Process
        {
            get 
            {
                ProcessEditor parentView = ParentView;
                return parentView != null ? ParentView.Process : null;
            }
        }

        #endregion
    }
}
