// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;

namespace Tawala.Controls
{
    public class ConditionGroupCollection
    {
        private readonly bool bAutoSelectTextBox;
        private readonly ComboBox comboBoxAndOr;
        private readonly Control container;

        public ConditionGroupCollection(Control container, ComboBox comboBoxAndOr, bool autoSelectTextBox)
        {
            bAutoSelectTextBox = autoSelectTextBox;
            this.container = container;
            this.comboBoxAndOr = comboBoxAndOr;

            container.ControlAdded += container_ControlAdded;
            container.ControlRemoved += container_ControlRemoved;
            container.Layout += container_Layout;

            addTopGroup();
        }

        [DefaultValue(0)]
        public Conditions Conditions
        {
            get
            {
                var conditions = new Conditions();

                if (Project.Current != null)
                {
                    for (int i = 0; i < container.Controls.Count; i++)
                    {
                        var group = container.Controls[i] as ConditionGroup;

                        var field = group.TextBoxField.Tag as IField;

                        if (field != null)
                        {
                            var op = group.ComboBoxOperator.SelectedItem as ComparisonOperator;
                            Expression expression = (group.TextBoxExpression.Tag == null
                                                         ? new Expression(group.TextBoxExpression.Text)
                                                         : group.TextBoxExpression.Tag as Expression);

                            conditions.Add(new Condition(field, op, expression));

                            if (!isLastGroup(i))
                            {
                                var nextGroup = container.Controls[i + 1] as ConditionGroup;
                                var nextField = nextGroup.TextBoxField.Tag as IField;

                                if (nextField != null)
                                {
                                    conditions.Add(comboBoxAndOr.SelectedIndex == 0 ? new LogicalOperator("AND") : new LogicalOperator("OR"));
                                }
                            }
                        }
                    }
                }

                return conditions;
            }

            set
            {
                container.Controls.Clear();

                if (value.Count == 0)
                {
                    addTopGroup();
                }
                else
                {
                    foreach (IConditionComponent component in value)
                    {
                        if (component is Condition)
                        {
                            var group = new ConditionGroup();

                            var condition = component as Condition;
                            group.TextBoxField.Text = condition.Field.ToString();
                            group.TextBoxField.Tag = condition.Field;

                            group.SetOperatorDataSourceBasedOnFieldType();
                            group.ComboBoxOperator.SelectedItem = condition.CompOp;

                            string expressionString = (condition.Expression == null) ? "" : condition.Expression.ToString();
                            group.TextBoxExpression.Text = expressionString.Trim("<>".ToCharArray());
                            group.TextBoxExpression.Tag = condition.Expression;

                            addGroup(group);
                        }
                        else
                        {
                            var op = component as LogicalOperator;

                            if (op.ToString() == "AND")
                            {
                                comboBoxAndOr.SelectedIndex = 0;
                            }
                            else if (op.ToString() == "OR")
                            {
                                comboBoxAndOr.SelectedIndex = 1;
                            }
                        }
                    }

                    UpdateFieldsPaletteChoices();
                }
            }
        }

        private void container_Layout(object sender, LayoutEventArgs e)
        {
            int width = container.ClientSize.Width;

            for (int i = 0; i < container.Controls.Count; ++i)
            {
                var group = container.Controls[i] as ConditionGroup;
                group.Width = width;
            }
        }

        private void addGroup(ConditionGroup group)
        {
            container.Controls.Add(group);
        }

        public void SelectTextBox(object sender)
        {
            foreach (Control c in container.Controls)
            {
                var group = c as ConditionGroup;
                if (group != null)
                {
                    group.SelectTextBox(sender);
                }
            }
        }

        public bool WhereConditionsAreValid()
        {
            foreach (ConditionGroup group in container.Controls)
            {
                if (!group.IsComplete())
                {
                    return false;
                }
            }

            return true;
        }

        private void addTopGroup()
        {
            var group = new ConditionGroup();
            addGroup(group);

            if (bAutoSelectTextBox)
            {
                SelectTextBox(group.TextBoxField);
            }
        }

        private bool isLastGroup(int index)
        {
            return (index == container.Controls.Count - 1);
        }

        private void container_ControlAdded(object sender, ControlEventArgs e)
        {
            var group = e.Control as ConditionGroup;
            group.TextBoxField.TextChanged += textBoxField_TextChanged;

            if (bAutoSelectTextBox && container.Controls.Count == 1)
            {
                SelectTextBox(group.TextBoxField);
            }

            updatePlusButtonsEnableState();
            updateAndOrButtonsVisibleState();
        }

        private void textBoxField_TextChanged(object sender, EventArgs e)
        {
            UpdateFieldsPaletteChoices();
        }

        public void UpdateFieldsPaletteChoices()
        {
            var mcItemCollection = new Collection<IMcqItem>();

            foreach (ConditionGroup group in container.Controls)
            {
                if (group.TextBoxField.Tag is IMcqItem)
                {
                    mcItemCollection.Add(group.TextBoxField.Tag as IMcqItem);
                }
                else if (group.TextBoxField.Tag is RecordField)
                {
                    var recordField = group.TextBoxField.Tag as RecordField;

                    if (recordField.ReferenceField is IMcqItem)
                    {
                        mcItemCollection.Add(recordField.ReferenceField as IMcqItem);
                    }
                }
                else if (group.TextBoxField.Tag is RecordSetField)
                {
                    var recordSetField = group.TextBoxField.Tag as RecordSetField;

                    if (recordSetField.ReferenceField is IMcqItem)
                    {
                        mcItemCollection.Add(recordSetField.ReferenceField as IMcqItem);
                    }
                }
            }

            var mcItems = new IMcqItem[mcItemCollection.Count];
            mcItemCollection.CopyTo(mcItems, 0);
            Project.Events.RaiseMCItemSelectedEvent(new MCItemEventArgs(mcItems));
        }

        private void container_ControlRemoved(object sender, ControlEventArgs e)
        {
            fillGapLeftByRemovedGroup(e);

            if (container.Controls.Count > 0)
            {
                ((ConditionGroup)container.Controls[0]).TextBoxField.Focus();
            }

            updatePlusButtonsEnableState();
            updateAndOrButtonsVisibleState();
            UpdateFieldsPaletteChoices();
        }

        private void fillGapLeftByRemovedGroup(ControlEventArgs e)
        {
            int topOfNextControl = e.Control.Bottom;

            foreach (Control c in container.Controls)
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

            for (int i = 0; i < container.Controls.Count - 1; ++i)
            {
                group = container.Controls[i] as ConditionGroup;
                group.DisablePlus();
            }

            if (container.Controls.Count > 0)
            {
                group = container.Controls[container.Controls.Count - 1] as ConditionGroup;
                group.EnablePlus();
            }
        }

        private void updateAndOrButtonsVisibleState()
        {
            comboBoxAndOr.Visible = container.Controls.Count > 1;
        }

        public void UpdateRecordSetNames(string recordSetName)
        {
            foreach (ConditionGroup group in container.Controls)
            {
                group.SetRecordSetNameInTextBoxField(recordSetName);
            }
        }
    }
}