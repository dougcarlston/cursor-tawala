// $Workfile: DeleteStatementView.cs $
// $Revision: 8 $	$Date: 12/12/07 4:57p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.   
#define CODE_ANALYSIS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.ProjectUI;
using Tawala.Controls;

namespace Tawala.Processes
{
	public partial class DeleteStatementView : DeleteStatementViewBase
	{
		ConditionGroupCollection groups;
		int conditionGroupHeight;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification="KM: Don't want to eliminate presenter, we should use it some day")]
		DeleteStatementPresenter presenter;

		public DeleteStatementView()
		{
            presenter = new DeleteStatementPresenter(this);

            InitializeComponent();

            Dock = DockStyle.Fill;
            ResizeRedraw = true;

            groupBoxWhere.ControlAdded += groupBoxWhere_ControlAdded;
			groupBoxWhere.ControlRemoved += groupBoxWhere_ControlRemoved;

            groupBoxWhere.Width = tabPageDelete.Width - groupBoxWhere.Left - 8;
            groupBoxWhere.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

			Project.Events.ProcessLineIndexChanged += events_ProcessLineIndexChanged;

			var formSource = new Tawala.ProjectUI.FormBindingSource();

			if (components == null)
			{
				components = new Container();
			}

			components.Add(formSource);

			comboBoxForms.DisplayMember = "Name";
			comboBoxForms.ValueMember = "";
			comboBoxForms.DataSource = formSource;

		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);

			if (Parent != null && comboBoxForms.Items.Count > 0)
			{
				FieldsPalette.Palette.ConditionsForms = new Tawala.Projects.FunctionFormCollection(comboBoxForms.SelectedItem as IForm);
			}
			else
			{
				FieldsPalette.Palette.ConditionsForms = Tawala.Projects.FunctionFormCollection.NULL;
			}
		}

		void checkedListBox_LostFocus(object sender, EventArgs e)
		{
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
                groupBoxWhere.Controls[i].TabIndex = i;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			splitContainer = getSplitContainer();

            groups = new ConditionGroupCollection(groupBoxWhere, comboBoxAndOr, true);
        }

		/// <summary>
		/// This function handles editing a new or existing statement.
		/// </summary>
		protected override void OnEdit()
		{
			groupBoxWhere.Hide();

			if (ModifyMode)
			{
				scratchStatement = DeleteStatement.ShallowCopy(statement);
			}

			if (scratchStatement.Form != null)
            {
				comboBoxForms.SelectedItem = scratchStatement.Form;
				groups.Conditions = scratchStatement.Conditions;
            }

			broadcastStatementChange();
			
			autoSizeHeight();

			deselectAllConditionsControls();

			groupBoxWhere.Show();

			comboBoxForms.Focus();

			updateConditionForms();
        }

		private void deselectAllConditionsControls()
		{
			groups.SelectTextBox(null);
		}

        private void autoSizeHeight()
        {
            if (groupBoxWhere.Controls.Count >= 1)
            {
                Point pt = tabPageDelete.PointToScreen(new Point(buttonAddModify.Left, buttonAddModify.Bottom));
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
				formIsSelected() &&
				whereConditionsAreValid();

			buttonAddModify.Enabled = enabled;
		}

		private bool atAddOrModifyPosition()
		{
			return (AtInsertPosition() || ModifyMode);
		}

		private bool whereConditionsAreValid()
		{
			foreach (ConditionGroup group in groupBoxWhere.Controls)
			{
				if (!group.IsComplete())
				{
					return false;
				}
			}

			return true;
		}

		protected override void OnMdiWindowDeactivated()
		{
			Project.Events.RaiseMCItemSelectedEvent(new MCItemEventArgs());
			FieldsPalette.Palette.ConditionsForms = Tawala.Projects.FunctionFormCollection.NULL;
			base.OnMdiWindowDeactivated();
		}

		protected override void OnMdiWindowActivated()
		{
			base.OnMdiWindowActivated();
			groups.UpdateFieldsPaletteChoices();
			updateConditionForms();
		}

		DeleteStatement scratchStatement;

		protected override void NewStatement()
		{
			base.NewStatement();
			scratchStatement = DeleteStatement.ShallowCopy(statement);

			groupBoxWhere.Hide();
            groups.Conditions = new Conditions();
			groupBoxWhere.Show();

			deselectAllConditionsControls();

			updateConditionForms();
		}

		private bool formIsSelected()
		{
			return (comboBoxForms.Text.Length > 0);
		}


		#region Control Events

        /// <summary>
		/// User clicked the Add / Modify button
		/// </summary>
		private void buttonAddModify_Click(object sender, EventArgs e)
		{
			RememberAction();

			statement.Form = comboBoxForms.SelectedItem as IForm;
			statement.Conditions = groups.Conditions;

			SaveStatement();
		}


		#endregion

		private void tabPageGet_Click(object sender, EventArgs e)
		{
			tabPageDelete.Focus();
		}

		/// <summary>
		/// The split container holding the statment details panel.
		/// </summary>
		private SplitContainer splitContainer;

		private SplitContainer getSplitContainer()
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

		private void comboBoxForms_SelectionChangeCommitted(object sender, EventArgs e)
		{
			updateConditionForms();
		}

		private void updateConditionForms()
		{
			IForm form = comboBoxForms.SelectedItem as IForm;
			FieldsPalette.Palette.ConditionsForms = form != null ? new Tawala.Projects.FunctionFormCollection(form) : Tawala.Projects.FunctionFormCollection.NULL;
		}
	}


	// Work around for VSN Form Designer issue with Generics

	public class DeleteStatementViewBase : StatementView<DeleteStatement>
	{
	}
}
