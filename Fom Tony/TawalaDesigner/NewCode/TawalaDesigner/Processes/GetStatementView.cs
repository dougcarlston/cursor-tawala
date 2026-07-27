// $Workfile: GetStatementView.cs $
// $Revision: 10 $	$Date: 2/01/08 2:38p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.   

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Controls;

namespace Tawala.Processes
{
	/// <summary>
	/// GetDetails for GetStatement
	/// </summary>
	public partial class GetStatementView : GetStatementViewBase
	{
		// control size ratio for layout as tabpage changes width

		double ratioTextBoxRecordSet;
		int lastTabPageGetWidth;

		ConditionGroupCollection groups;
		int conditionGroupHeight;

        GetStatementPresenter presenter;

		public GetStatementView()
		{
            presenter = new GetStatementPresenter(this);

            InitializeComponent();

            Dock = DockStyle.Fill;
            ResizeRedraw = true;

			textBoxRecordSet.Text = RecordSetNamer.GetNextName();

			ratioTextBoxRecordSet = textBoxRecordSet.Width / (double)tabPageGet.Width;

            groupBoxWhere.ControlAdded += groupBoxWhere_ControlAdded;
			groupBoxWhere.ControlRemoved += groupBoxWhere_ControlRemoved;

            groupBoxWhere.Width = tabPageGet.Width - groupBoxWhere.Left - 8;
            groupBoxWhere.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

			comboBoxCheckedForms.CheckedListBox.LostFocus += checkedListBox_LostFocus;

			Project.Events.ProcessLineIndexChanged += events_ProcessLineIndexChanged;
        }

		void checkedListBox_LostFocus(object sender, EventArgs e)
		{
			if (ParentView != null)
			{
				ParentView.Process.ActiveGetStatement = scratchStatement;
			}
			broadcastStatementChange();
		}

		void events_ProcessLineIndexChanged(object sender, EventArgs e)
		{
			broadcastStatementChange();
		}

		private void broadcastStatementChange()
		{
			if (scratchStatement != null)
			{
				scratchStatement.Records = getRecordSet(textBoxRecordSet.Text);

				if (ParentView != null)
				{
					Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(ParentView.Process, scratchStatement));
				}
			}
		}

		void groupBoxWhere_ControlAdded(object sender, ControlEventArgs e)
		{
			ConditionGroup group = e.Control as ConditionGroup;

            if (conditionGroupHeight == 0)
            {
                conditionGroupHeight = group.Height;       
            }

            groupBoxWhere.Height = conditionGroupHeight * groupBoxWhere.Controls.Count + conditionGroupHeight;

            group.TextBoxField.Enter += new EventHandler(textBoxField_Enter);          
			group.TextBoxExpression.Enter += new EventHandler(textBoxExpression_Enter);

			group.Top = conditionGroupHeight * (groupBoxWhere.Controls.Count - 1) + conditionGroupHeight/2;

            buttonAddModify.Top = groupBoxWhere.Bottom + 8;

            group.Visible = true;

            autoSizeHeight();

            adjustTabs();
		}

		void textBoxField_Enter(object sender, EventArgs e)
		{
			groups.SelectTextBox(sender);
		}

		void textBoxExpression_Enter(object sender, EventArgs e)
		{
			groups.SelectTextBox(sender);
		}

		private bool panelNeedsToGrow()
		{
			return (((ConditionGroup)groupBoxWhere.Controls[groupBoxWhere.Controls.Count - 1]).Bottom + conditionGroupHeight*2 + 8 >= buttonAddModify.Top);
		}

		void groupBoxWhere_ControlRemoved(object sender, ControlEventArgs e)
		{
            if (groupBoxWhere.Controls.Count >= 1)
            {
                groupBoxWhere.Height -= conditionGroupHeight;
                buttonAddModify.Top = groupBoxWhere.Bottom + 8;
                autoSizeHeight();
            }
            adjustTabs();
        }

        private void adjustTabs()
        {
            for (int i = 0; i < groupBoxWhere.Controls.Count; ++i)
            {
                int oldTabIndex = groupBoxWhere.Controls[i].TabIndex;
                System.Diagnostics.Debug.WriteLineIf(oldTabIndex != i, "old tabindex =" + oldTabIndex + " new = " + i);
                groupBoxWhere.Controls[i].TabIndex = i;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

            Project.Events.ComponentAdded += events_ComponentAdded;
			Project.Events.ComponentRemoved += events_ComponentRemoved;

			splitContainer = GetSplitContainer();

            groups = new ConditionGroupCollection(groupBoxWhere, comboBoxAndOr, true);
        }

		void events_ComponentAdded(object sender, ComponentEventArgs e)
		{
			if (e.Component is IForm)
			{
				refreshDataSources();
			}
		}

		void events_ComponentRemoved(object sender, ComponentEventArgs e)
		{
			if (e.Component is IForm)
			{
				refreshDataSources();
			}
		}

		protected override void OnProcessFormConnectionChange(ProcessConnectionArgs e)
		{
			refreshDataSources();
		}

		/// <summary>
		/// This function handles editing a new or existing statement.
		/// </summary>
		protected override void OnEdit()
		{
			groupBoxWhere.Hide();

			refreshDataSources();

			if (ModifyMode)
			{
				scratchStatement = GetStatement.ShallowCopy(statement);
			}

			if (scratchStatement.Records != null)
            {
				comboBoxCheckedForms.CheckedForms = scratchStatement.Records.Forms;
				textBoxRecordSet.Text = scratchStatement.Records.FieldName;
				groups.Conditions = scratchStatement.Conditions;
            }
            else
            {
                comboBoxCheckedForms.CheckedForms = presenter.GetDefaultCheckedFormList();
				scratchStatement.Records = getRecordSet(textBoxRecordSet.Text);
			}
			
			ParentView.Process.ActiveGetStatement = scratchStatement;
			broadcastStatementChange();
			
			autoSizeHeight();

			deselectAllConditionsControls();

			groupBoxWhere.Show();

			textBoxRecordSet.Focus();
        }

		private void deselectAllConditionsControls()
		{
			groups.SelectTextBox(null);
		}

        private void autoSizeHeight()
        {
            if (groupBoxWhere.Controls.Count >= 1)
            {
                Point pt = tabPageGet.PointToScreen(new Point(buttonAddModify.Left, buttonAddModify.Bottom));
                pt = splitContainer.Panel1.PointToClient(pt);
                if (pt.Y > 0)
                {
                    splitContainer.SplitterDistance = pt.Y + 8;
                    Height = splitContainer.SplitterDistance;
                }
            }
        }

		/// <summary>
		/// On application idle update state of UI
		/// </summary>
		protected override void OnIdle()
		{
			bool enabled =
				atAddOrModifyPosition() &&
				recordSetNameIsValid() &&
				formIsSelected() &&
				groups.WhereConditionsAreValid();

			if (enabled)
			{
				if (ParentView != null && ParentView.Process != null)
				{
					enabled = Project.Current.ValidLabelFormat(textBoxRecordSet.Text);
				}
			}

			buttonAddModify.Enabled = enabled;
		}

		private bool atAddOrModifyPosition()
		{
			return (AtInsertPosition() || ModifyMode);
		}

		protected override void OnMdiWindowDeactivated()
		{
			Project.Events.RaiseMCItemSelectedEvent(new MCItemEventArgs());
			base.OnMdiWindowDeactivated();
		}

		protected override void OnMdiWindowActivated()
		{
			base.OnMdiWindowActivated();
			groups.UpdateFieldsPaletteChoices();
		}

		GetStatement scratchStatement;

		protected override void NewStatement()
		{
			base.NewStatement();
			//scratchStatement = GetStatement.ShallowCopy(statement);
			scratchStatement = new GetStatement();

			comboBoxCheckedForms.CheckedForms = presenter.GetDefaultCheckedFormList();
			textBoxRecordSet.Text = RecordSetNamer.GetNextName();

			// Process.ActiveGetStatement gets properly set in OnEdit(). Nulling it out here keeps Record
			// List nodes from appearing inappropriately in the Fields Palette. Part of fix for Bug 678.
			//																		jdf - 2/1/08
			ParentView.Process.ActiveGetStatement = null;

			groupBoxWhere.Hide();
            groups.Conditions = new Conditions();
			groupBoxWhere.Show();

			deselectAllConditionsControls();
		}

		private Boolean recordSetNameIsValid()
		{
			bool validName = recordSetPresent();

			return validName;
		}

		private bool recordSetPresent()
		{
			return (textBoxRecordSet.Text.Length > 0);
		}

		private bool formIsSelected()
		{
			return (comboBoxCheckedForms.Text.Length > 0);
		}

		/// <summary>
		/// Refresh the data source and try to resync previous combobox selection
		/// </summary>
		private void refreshDataSources()
		{
 			comboBoxCheckedForms.Forms = presenter.GetFormList();
		}

		#region Project Events

		protected override void OnCurrentComponentSet(Tawala.Projects.Component c)
		{
			if (c is Projects.Process)
			{
				refreshDataSources();
			}
		}

		#endregion

		#region Control Events

		/// <summary>
		/// Relayout the controls based on space available
		/// </summary>
		private void tabPageGet_Layout(object sender, LayoutEventArgs e)
		{
			if (ratioTextBoxRecordSet != 0.0 && lastTabPageGetWidth != tabPageGet.Width && tabPageGet.Width >= 240)
			{
				int width = tabPageGet.Width;
				int oldRight = textBoxRecordSet.Right;
				textBoxRecordSet.Width = (int)(width * ratioTextBoxRecordSet); ;
				int offset = textBoxRecordSet.Right - oldRight;
				labelFrom.Left += offset;
				comboBoxCheckedForms.Left += offset;
				comboBoxCheckedForms.Width = width - 8 - comboBoxCheckedForms.Left;
			}
			lastTabPageGetWidth = tabPageGet.Width;
		}

        /// <summary>
		/// User clicked the Add / Modify button
		/// </summary>
		private void buttonAddModify_Click(object sender, EventArgs e)
		{
			RememberAction();

			textBoxRecordSet.Text = textBoxRecordSet.Text.Trim();

			statement.Records = getRecordSet(textBoxRecordSet.Text);
			statement.Conditions = groups.Conditions;

			ParentView.Process.ActiveGetStatement = null;

			SaveStatement();
		}

		private RecordSet getRecordSet(string recordSetName)
		{
			RecordSet recordSet = null;

			if (recordSetExists(recordSetName))
			{
				int index = ParentView.Process.RecordSets.IndexOf(recordSetName);
				recordSet = RecordSet.ShallowCopy((RecordSet)ParentView.Process.RecordSets[index]);
				recordSet.Forms = comboBoxCheckedForms.CheckedForms;
			}
			else
			{
				if (ParentView != null)
				{
					recordSet = new RecordSet(recordSetName, comboBoxCheckedForms.CheckedForms);
					ParentView.Process.RecordSets.Add(recordSet);
				}
			}

			return recordSet;
		}

		private bool recordSetExists(string recordSetName)
		{
			return (ParentView != null && ParentView.Process.RecordSets.IndexOf(recordSetName) >= 0);
		}

		#endregion

		private void tabPageGet_Click(object sender, EventArgs e)
		{
			tabPageGet.Focus();
		}

		private SplitContainer splitContainer;

        private void groupBoxWhere_Layout(object sender, LayoutEventArgs e)
        {
            foreach (Control c in groupBoxWhere.Controls)
            {
                c.Left = 2;
                c.Width = groupBoxWhere.Width - 4;
                c.PerformLayout();
            }
        }

        private void comboBoxAndOr_VisibleChanged(object sender, EventArgs e)
        {
            if (comboBoxAndOr.Visible)
            {
                labelWhere2.Visible = true;
                if (comboBoxAndOr.SelectedIndex < 0)
                {
                    comboBoxAndOr.SelectedIndex = 0;
                }
            }
            else
            {
                labelWhere2.Visible = false;
            }
        }

		private void textBoxRecordSet_TextChanged(object sender, EventArgs e)
		{
			broadcastStatementChange();
		}
	}


	// Work around for VSN Form Designer issue with Generics

	public class GetStatementViewBase : StatementView<GetStatement>
	{
	}
}
