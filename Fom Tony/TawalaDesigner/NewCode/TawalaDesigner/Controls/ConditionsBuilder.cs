// $Workfile: ConditionsBuilder.cs $
// $Revision: 9 $	$Date: 11/09/06 12:12p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Diagnostics;
using Tawala.Proj;
using Tawala.XmlSupport;

namespace Tawala.Controls
{
    [Obsolete("Is ConditionsBuilder obsolete? I don't see it used anywhere")]
	[Designer(typeof(ParentControlDesigner))]
	public partial class ConditionsBuilder : UserControl
	{
		public ConditionsBuilder()
		{
			InitializeComponent();

			AddTopGroup();
		}

		[System.ComponentModel.DefaultValue(0)]
		public Conditions Conditions
		{
			get
			{
				Conditions conditions = new Conditions();

				if (Project.Current != null)
				{
					for (int i = 0; i < panelGroups.Controls.Count; i++)
					{
						ConditionGroup group = panelGroups.Controls[i] as ConditionGroup;

						IField field = group.TextBoxField.Tag as IField;
						ComparisonOperator op = group.ComboBoxOperator.SelectedItem as ComparisonOperator;
						Expression expression = new Expression(group.TextBoxExpression.Text);

						conditions.Add(new Condition(field, op, expression));

						if (!isLastGroup(i))
						{
							conditions.Add(radioButtonAnd.Checked ? new LogicalOperator("AND") : new LogicalOperator("OR"));
						}
					}
				}

				return conditions;
			}

			set
			{
				panelGroups.Controls.Clear();

				foreach (IConditionComponent component in value)
				{
					if (component is Condition)
					{
						ConditionGroup group = new ConditionGroup();

						Condition condition = component as Condition;
						group.TextBoxField.Text = condition.Field.ToString();
						group.TextBoxField.Tag = condition.Field;

						group.SetOperatorDataSourceBasedOnFieldType();
						group.ComboBoxOperator.SelectedItem = condition.CompOp;

						group.TextBoxExpression.Text = condition.NewExpression.ToString();

						panelGroups.Controls.Add(group);
					}
					else
					{
						LogicalOperator op = component as LogicalOperator;

						if (op.ToString() == "AND")
						{
							radioButtonAnd.Checked = true;
						}
						else if (op.ToString() == "OR")
						{
							radioButtonOr.Checked = true;
						}
					}
				}
			}
		}

		private bool isLastGroup(int index)
		{
			return (index == panelGroups.Controls.Count - 1);
		}

		private void AddTopGroup()
		{
			panelGroups.Controls.Add(new ConditionGroup());
		}

		private void radioButtonAnd_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonAnd.Checked)
			{
				radioButtonOr.Checked = false;

				setLogicalOperatorLabels();
			}
		}

		private void radioButtonOr_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonOr.Checked)
			{
				radioButtonAnd.Checked = false;

				setLogicalOperatorLabels();
			}
		}

		private void setLogicalOperatorLabels()
		{
            ////REVISIT: get text from control
            //string text = radioButtonAnd.Checked ? "AND" : "OR";

            //for (int i = 2; i < panelGroups.Controls.Count; i++)
            //{
            //    ConditionGroup group = panelGroups.Controls[i] as ConditionGroup;
            //    group.LabelLogicalOperator.Text = text;
            //}
		}

		private void panelGroups_ControlAdded(object sender, ControlEventArgs e)
		{
			if (panelGroups.Controls.Count > 0)
			{
				e.Control.Top = e.Control.Height * (panelGroups.Controls.Count - 1);
			}

			updatePlusButtonsEnableState();
			updateAndOrButtonsVisibleState();

			setLogicalOperatorLabels();
		}

		private void panelGroups_ControlRemoved(object sender, ControlEventArgs e)
		{
			fillGapLeftByRemovedGroup(e);

			updatePlusButtonsEnableState();
			updateAndOrButtonsVisibleState();
		}

		private void fillGapLeftByRemovedGroup(ControlEventArgs e)
		{
			int topOfNextControl = e.Control.Bottom;

			foreach (Control c in panelGroups.Controls)
			{
				if (c.Top >= topOfNextControl)
				{
					c.Top -= e.Control.Height;
				}
			}
		}

		private void updatePlusButtonsEnableState()
		{
			ConditionGroup group;

			for (int i = 0; i < panelGroups.Controls.Count - 1; ++i)
			{
				group = panelGroups.Controls[i] as ConditionGroup;
				group.DisablePlus();
			}

			if (panelGroups.Controls.Count > 0)
			{
				group = panelGroups.Controls[panelGroups.Controls.Count - 1] as ConditionGroup;
				group.EnablePlus();
			}
		}

		private void updateAndOrButtonsVisibleState()
		{
			radioButtonAnd.Visible = panelGroups.Controls.Count > 1;
			radioButtonOr.Visible = panelGroups.Controls.Count > 1;
		}

	}
}
