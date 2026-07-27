// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;

namespace Tawala.Controls
{
    public partial class ConditionsEditorControl : UserControl
    {
        public ConditionsEditorControl()
        {
            InitializeComponent();
            ResizeRedraw = true;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("Before")]
        [Category("Conditions Editor")]
        public string SingleConditionBeforeAndOrText { get; set; }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("After")]
        [Category("Conditions Editor")]
        public string SingleConditionAfterAndOrText { get; set; }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("Before")]
        [Category("Conditions Editor")]
        public string MultipleConditionsBeforeAndOrText { get; set; }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("After")]
        [Category("Conditions Editor")]
        public string MultipleConditionsAfterAndOrText { get; set; }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(true)]
        [Category("Conditions Editor")]
        public bool AutoSelectText { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Conditions Conditions
        {
            get
            {
                return getConditions();
            }

            set
            {
                setConditions(value);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            comboBoxAndOr.Visible = DesignMode;
            labelBefore.Visible = true;
            labelAfter.Visible = DesignMode;

            base.OnLoad(e);
        }

        private Conditions getConditions()
        {
            var conditions = new Conditions();

            if (Project.Current != null)
            {
                for (int i = 0; i < editControlsLayout.Controls.Count; i++)
                {
                    ConditionEditControl editControl = getConditionEditControl(i);

                    var field = editControl.TextBoxField.Tag as IField;

                    if (field != null)
                    {
                        var op = editControl.ComboBoxOperator.SelectedItem as ComparisonOperator;
                        Expression expression = (editControl.TextBoxExpression.Tag == null
                                                     ? new Expression(editControl.TextBoxExpression.Text)
                                                     : editControl.TextBoxExpression.Tag as Expression);

                        conditions.Add(new Condition(field, op, expression));

                        if (!isLastEditControl(i))
                        {
                            ConditionEditControl nextGroup = getConditionEditControl(i + 1);
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

        private void setConditions(Conditions conditions)
        {
            clearControls();

            foreach (IConditionComponent component in conditions)
            {
                if (component is Condition)
                {
                    addConditionToUI(component);
                }
                else
                {
                    addMultipleConditionsOperatorToUI(component);
                }
            }

            updateFieldsPaletteChoices();
        }

        private void addMultipleConditionsOperatorToUI(IConditionComponent component)
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

        private void addConditionToUI(IConditionComponent component)
        {
            ConditionEditControl editControl = getNewCondtionEditControl(); 
            var condition = component as Condition;

            editControl.TextBoxField.Text = condition.Field.ToString();
            editControl.TextBoxField.Tag = condition.Field;

            editControl.SetOperatorDataSourceBasedOnFieldType();
            editControl.ComboBoxOperator.SelectedItem = condition.CompOp;

            string expressionString = (condition.Expression == null) ? string.Empty : condition.Expression.ToString();
            editControl.TextBoxExpression.Text = expressionString.Trim("<>".ToCharArray());
            editControl.TextBoxExpression.Tag = condition.Expression;

            if (!editControlsLayout.Controls.Contains(editControl))
            {
                editControlsLayout.Controls.Add(editControl);
            }
        }

        private ConditionEditControl getNewCondtionEditControl()
        {
            return flowLayoutPanel.Controls.Count == 1 ? getConditionEditControl(0) : new ConditionEditControl();
        }


        private void clearControls()
        {
            editControlsLayout.SuspendLayout();

            while (editControlsLayout.Controls.Count > 1)
            {
                editControlsLayout.Controls.RemoveAt(1);
            }

            editControlsLayout.ResumeLayout(true);
        }

        private ConditionEditControl getConditionEditControl(int i)
        {
            return editControlsLayout.Controls[i] as ConditionEditControl;
        }

        private void comboBoxAndOr_VisibleChanged(object sender, EventArgs e)
        {
            if (comboBoxAndOr.Visible)
            {
                labelBefore.Text = MultipleConditionsBeforeAndOrText;
                labelAfter.Text = MultipleConditionsAfterAndOrText;

                if (comboBoxAndOr.SelectedIndex < 0)
                {
                    comboBoxAndOr.SelectedIndex = 0;
                }
            }
            else
            {
                labelBefore.Text = SingleConditionBeforeAndOrText;
                labelAfter.Text = SingleConditionAfterAndOrText;
            }
        }

        private void editControlsLayout_ControlAdded(object sender, ControlEventArgs e)
        {
            var editControl = e.Control as ConditionEditControl;

            editControl.TextBoxField.Enter += textBoxField_Enter;
            editControl.TextBoxExpression.Enter += textBoxExpression_Enter;

            editControl.TextBoxField.TextChanged += textBoxField_TextChanged;

            if (AutoSelectText && editControlsLayout.Controls.Count == 1)
            {
                SelectTextBox(editControl.TextBoxField);
            }

            updatePlusButtonsEnableState();
            updateAndOrButtonsVisibleState();

            editControl.SelectTextBox(editControl.TextBoxField);
        }

        private void editControlsLayout_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (editControlsLayout.Controls.Count > 0)
            {
                ((ConditionEditControl)editControlsLayout.Controls[0]).TextBoxField.Focus();
            }

            updatePlusButtonsEnableState();
            updateAndOrButtonsVisibleState();
            updateFieldsPaletteChoices();
        }

        private void textBoxField_Enter(object sender, EventArgs e)
        {
            SelectTextBox(sender);
        }

        private void textBoxExpression_Enter(object sender, EventArgs e)
        {
            SelectTextBox(sender);
        }

        private void SelectTextBox(object sender)
        {
            foreach (ConditionEditControl editControl in editControlsLayout.Controls)
            {
                editControl.SelectTextBox(sender);
            }
        }

        public bool ConditionsAreValid()
        {
            foreach (ConditionEditControl editControl in editControlsLayout.Controls)
            {
                if (!editControl.IsComplete())
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateRecordSetNames(string recordSetName)
        {
            foreach (ConditionEditControl editControl in editControlsLayout.Controls)
            {
                editControl.SetRecordSetNameInTextBoxField(recordSetName);
            }
        }

        private bool isLastEditControl(int index)
        {
            return (index == editControlsLayout.Controls.Count - 1);
        }

        private void textBoxField_TextChanged(object sender, EventArgs e)
        {
            updateFieldsPaletteChoices();
        }

        private void updateFieldsPaletteChoices()
        {
            var mcItemCollection = new Collection<IMcqItem>();

            foreach (ConditionEditControl editControl in editControlsLayout.Controls)
            {
                if (editControl.TextBoxField.Tag is IMcqItem)
                {
                    mcItemCollection.Add(editControl.TextBoxField.Tag as IMcqItem);
                }
                else if (editControl.TextBoxField.Tag is RecordField)
                {
                    var recordField = editControl.TextBoxField.Tag as RecordField;

                    if (recordField.ReferenceField is IMcqItem)
                    {
                        mcItemCollection.Add(recordField.ReferenceField as IMcqItem);
                    }
                }
                else if (editControl.TextBoxField.Tag is RecordSetField)
                {
                    var recordSetField = editControl.TextBoxField.Tag as RecordSetField;

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

        private void updatePlusButtonsEnableState()
        {
            ConditionEditControl editControl;

            for (int i = 0; i < editControlsLayout.Controls.Count - 1; ++i)
            {
                editControl = getConditionEditControl(i);
                editControl.DisablePlus();
            }

            if (editControlsLayout.Controls.Count > 0)
            {
                editControl = getConditionEditControl(editControlsLayout.Controls.Count - 1);
                editControl.EnablePlus();
            }
        }

        private void updateAndOrButtonsVisibleState()
        {
            comboBoxAndOr.Visible = editControlsLayout.Controls.Count > 1;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            int width = ClientSize.Width;
            int height = ClientSize.Height;
            groupBox.MinimumSize = new Size(width, height);
            groupBox.Size = groupBox.MinimumSize;
            base.OnLayout(e);
        }

        private void groupBox_Layout(object sender, LayoutEventArgs e)
        {
            int width = ClientSize.Width;
            int height = ClientSize.Height;
            editControlsLayout.MinimumSize = new Size(width, height);
            editControlsLayout.Size = new Size(width, height);
            editControlsLayout.PerformLayout();
        }

        private void editControlsLayout_Layout(object sender, LayoutEventArgs e)
        {
            if (editControlsLayout.Controls.Count > 0)
            {
                foreach (ConditionEditControl control in editControlsLayout.Controls)
                {
                    control.PerformLayout();
                }
            }
        }

    }
}