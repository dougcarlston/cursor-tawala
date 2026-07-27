// $Workfile: IfStatementView.cs $
// $Revision: 6 $	$Date: 12/12/07 4:57p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Controls;

namespace Tawala.Processes
{
    public partial class IfStatementView : IfStatementViewBase
	{
		ConditionGroupCollection groups;
		int conditionGroupHeight = 0;

		public IfStatementView()
		{
			InitializeComponent();

            Dock = DockStyle.Fill;
            ResizeRedraw = true;

            labelIf2.Left = comboBoxAndOr.Left - 4;
            labelIf2.Text = Properties.Resources.SingleIfCondition;
            comboBoxAndOr.Visible = false;

            comboBoxAndOr.VisibleChanged += new EventHandler(comboBoxAndOr_VisibleChanged);

			groupBox.ControlAdded += new ControlEventHandler(panel1_ControlAdded);
			groupBox.ControlRemoved += new ControlEventHandler(panel1_ControlRemoved);
            groupBox.Layout += new LayoutEventHandler(groupBox_Layout);

            groupBox.Width = tabPageIf.Width - groupBox.Left - 8;
            groupBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        }

        private void comboBoxAndOr_VisibleChanged(object sender, EventArgs e)
        {
            if (comboBoxAndOr.Visible)
            {
                labelIf2.Text = Properties.Resources.MultipleIfConditions;
                labelIf2.Left = comboBoxAndOr.Right + 2;
                if (comboBoxAndOr.SelectedIndex < 0)
                {
                    comboBoxAndOr.SelectedIndex = 0;
                }
            }
            else
            {
                labelIf2.Left = comboBoxAndOr.Left - 4;
                labelIf2.Text = Properties.Resources.SingleIfCondition;
            }
        }

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			splitContainer = getSplitContainer();

            groups = new ConditionGroupCollection(groupBox, comboBoxAndOr, true);
            conditionGroupHeight = ((ConditionGroup)groupBox.Controls[0]).Height;
            splitContainer.SplitterDistance = buttonAddModify.Bottom + 8;
		}

		void panel1_ControlAdded(object sender, ControlEventArgs e)
		{
			ConditionGroup group = e.Control as ConditionGroup;

			group.TextBoxField.Enter += new EventHandler(textBoxField_Enter);
			group.TextBoxExpression.Enter += new EventHandler(textBoxExpression_Enter);

            if (conditionGroupHeight == 0)
            {
                conditionGroupHeight = group.Height;
            }

            groupBox.Height = conditionGroupHeight * groupBox.Controls.Count + conditionGroupHeight;

            group.TextBoxField.Enter += new EventHandler(textBoxField_Enter);
            group.TextBoxExpression.Enter += new EventHandler(textBoxExpression_Enter);

            group.Top = conditionGroupHeight * (groupBox.Controls.Count - 1) + conditionGroupHeight / 2;

            checkBoxOtherwise.Top = groupBox.Bottom + 16;
            buttonAddModify.Top = checkBoxOtherwise.Bottom + 8;

            autoSizeHeight();
		}

		void textBoxField_Enter(object sender, EventArgs e)
		{
			groups.SelectTextBox(sender);
		}

		void textBoxExpression_Enter(object sender, EventArgs e)
		{
			groups.SelectTextBox(sender);
		}

		private bool panel1NeedsToGrow()
		{
			return (((ConditionGroup)groupBox.Controls[groupBox.Controls.Count - 1]).Bottom + conditionGroupHeight >= checkBoxOtherwise.Top);
		}

		void panel1_ControlRemoved(object sender, ControlEventArgs e)
		{
            if (groupBox.Controls.Count >= 1)
            {
                groupBox.Height -= conditionGroupHeight;
                checkBoxOtherwise.Top -= conditionGroupHeight;
                buttonAddModify.Top = checkBoxOtherwise.Bottom + 8;
                autoSizeHeight();
            }
        }

		protected override void OnEdit()
		{
			groupBox.Hide();

			if (ModifyMode)
			{
				groups.Conditions = statement.Conditions;
				checkBoxOtherwise.Checked = statement.HasOtherwise;
			}
			else
			{
				// this forces Fields Palette to update to show / hide Choices
				groups.Conditions = groups.Conditions;
			}

			autoSizeHeight();
			
			groupBox.Show();

			if (ModifyMode)
			{
			}
			else
			{
				((ConditionGroup)groupBox.Controls[0]).TextBoxField.Focus();
			}
		}

        private void autoSizeHeight()
        {
            if (groupBox.Controls.Count >= 1)
            {
                Point pt = tabPageIf.PointToScreen(new Point(buttonAddModify.Left, buttonAddModify.Bottom));
                pt = splitContainer.Panel1.PointToClient(pt);
                if (pt.Y > 0)
                {
                    splitContainer.SplitterDistance = pt.Y + 8;
                    Height = splitContainer.SplitterDistance;
                }
            }
        }

		protected override void OnIdle()
		{
			bool atAddOrModifyPosition = AtInsertPosition() || ModifyMode;
			bool enabled = atAddOrModifyPosition && controlsHoldValidConditions();

			if (buttonAddModify.Enabled != enabled)
			{
				buttonAddModify.Enabled = enabled;
			}
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

        protected override void NewStatement()
        {
            base.NewStatement();
            groups.Conditions = new Conditions();
        }
        
        private void groupBox_Layout(object sender, LayoutEventArgs e)
        {
            foreach (Control c in groupBox.Controls)
            {
                c.Left = 2;
                c.Width = groupBox.Width - 4;
                c.PerformLayout();
            }
        }

        private bool controlsHoldValidConditions()
		{
			bool result = true;

			foreach (ConditionGroup group in groupBox.Controls)
			{
				if (group.TextBoxField.Text.Length == 0 || (group.TextBoxExpression.Visible && group.TextBoxExpression.Text.Length == 0))
				{
					result = false;
					break;
				}
			}
			
			return result;
		}

		private void buttonAddModify_Click(object sender, EventArgs e)
		{
			RememberAction();

			if (ModifyMode)
			{
				statement = new IfStatement();
			}

			statement.Conditions = groups.Conditions;
			statement.HasOtherwise = checkBoxOtherwise.Checked;

			SaveStatement();
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
	}

	// Work around for VSN Form Designer issue with Generics
    public class IfStatementViewBase : StatementView<IfStatement>
	{
	}

}
