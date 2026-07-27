// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;

namespace Tawala.Functions.Controls
{
    public partial class ConditionListControl : UserControl, IParameterControl
    {
        private const string AND = "AND";
        private const string OR = "OR";
        private static readonly ConditionLogicalOperator andOperator = new ConditionLogicalOperator("ALL", new LogicalOperator(AND));
        private static readonly ConditionLogicalOperator orOperator = new ConditionLogicalOperator("ANY", new LogicalOperator(OR));
        private readonly bool bAutoSelectTextBox;

        public ConditionListControl()
        {
            InitializeComponent();
            ResizeRedraw = true;
            bAutoSelectTextBox = true;
            comboBoxAndOr.DisplayMember = "DisplayText";
            comboBoxAndOr.ValueMember = "Operator";
            comboBoxAndOr.DataSource = new[] {andOperator, orOperator};
            comboBoxAndOr.SelectedIndex = 0;
            comboBoxAndOr.SelectedIndexChanged += comboBoxAndOr_SelectedIndexChanged;

            ControlManager.RegisterQueryOkEnableStateCallback(queryOKEnabledState);

            if (!IsHandleCreated)
            {
                CreateControl();
            }
        }

        public string WhereText
        {
            get
            {
                return labelWhere.Text;
            }
            set
            {
                labelWhere.Text = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Conditions Conditions
        {
            get
            {
                var conditions = new Conditions();

                if (Project.Current != null)
                {
                    for (int i = 0; i < flowLayoutConditions.Controls.Count; i++)
                    {
                        var conditionControl = flowLayoutConditions.Controls[i] as ConditionControl;

                        var field = conditionControl.TextBoxField.Tag as IField;

                        if (field != null)
                        {
                            var op = conditionControl.ComboBoxOperator.SelectedItem as ComparisonOperator;
                            Expression expression = (conditionControl.TextBoxExpression.Tag == null
                                                         ? new Expression(conditionControl.TextBoxExpression.Text)
                                                         : conditionControl.TextBoxExpression.Tag as Expression);

                            conditions.Add(new Condition(field, op, expression));

                            if (!isLastGroup(i))
                            {
                                var nextConditionControl = flowLayoutConditions.Controls[i + 1] as ConditionControl;
                                var nextField = nextConditionControl.TextBoxField.Tag as IField;

                                if (nextField != null)
                                {
                                    var logicalOperator = comboBoxAndOr.SelectedValue as LogicalOperator;
                                    conditions.Add(logicalOperator ?? andOperator.Operator);
                                }
                            }
                        }
                    }
                }

                return conditions;
            }

            set
            {
                flowLayoutConditions.Controls.Clear();

                if (value.Count == 0)
                {
                    addConditionControl(new ConditionControl());
                }
                else
                {
                    foreach (IConditionComponent component in value)
                    {
                        if (component is Condition)
                        {
                            var conditionControl = new ConditionControl();

                            var condition = component as Condition;
                            conditionControl.TextBoxField.Text = condition.Field.ToString();
                            conditionControl.TextBoxField.Tag = condition.Field;

                            conditionControl.SetOperatorDataSourceBasedOnFieldType();
                            conditionControl.ComboBoxOperator.SelectedItem = condition.CompOp;

                            string expressionString = (condition.Expression == null) ? "" : condition.Expression.ToString();
                            conditionControl.TextBoxExpression.Text = expressionString.Trim("<>".ToCharArray());
                            conditionControl.TextBoxExpression.Tag = condition.Expression;

                            addConditionControl(conditionControl);
                        }
                        else
                        {
                            var op = component as LogicalOperator;

                            if (op.ToString() == AND)
                            {
                                comboBoxAndOr.SelectedItem = andOperator;
                            }
                            else if (op.ToString() == OR)
                            {
                                comboBoxAndOr.SelectedItem = orOperator;
                            }
                        }
                    }

                    UpdateFieldsPaletteChoices();
                }
            }
        }

        public int Count
        {
            get
            {
                return flowLayoutConditions.Controls.Count;
            }
        }

        //public bool HasAtLeastOneValidCondition
        //{
        //    get
        //    {
        //        foreach (ConditionControl c in flowLayoutConditions.Controls)
        //        {
        //            if (c.IsEntirelyFull())
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //}

        #region IParameterControl Members

        public bool CustomControl
        {
            get
            {
                return true;
            }
        }

        public void CommitPendingChanges()
        {
        }

        #endregion

        private bool queryOKEnabledState()
        {
            if (flowLayoutConditions.Controls.Count == 0)
            {
                return false;
            }

            if (flowLayoutConditions.Controls.Count == 1)
            {
                var control = flowLayoutConditions.Controls[0] as ConditionControl;
                return control.IsEntirelyEmpty() || control.IsEntirelyFull();
            }

            foreach (ConditionControl c in flowLayoutConditions.Controls)
            {
                if (!c.IsEntirelyFull())
                {
                    return false;
                }
            }

            return true;
        }

        private void addConditionControl(ConditionControl control)
        {
            flowLayoutConditions.SuspendLayout();
            flowLayoutConditions.Controls.Add(control);
            flowLayoutConditions.ResumeLayout(true);
        }

        public void SelectTextBox(object sender)
        {
            //            foreach (ConditionControl c in flowLayoutConditions.Controls)
            //            {
            //                {
            ////                    conditionControl.SelectTextBox(sender);
            //                }
            //            }
        }

        //public bool WhereConditionsAreValid()
        //{
        //    foreach (ConditionControl c in flowLayoutConditions.Controls)
        //    {
        //        if (!c.IsComplete())
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        private bool isLastGroup(int index)
        {
            return (index == flowLayoutConditions.Controls.Count - 1);
        }

        private void comboBoxAndOr_SelectedIndexChanged(object sender, EventArgs e)
        {
            //setLogicalOperatorLabels();
        }

        //private void setLogicalOperatorLabels()
        //{
        //    //           string text = (comboBoxAndOr.SelectedIndex == 0) ? "AND" : "OR";
        //}

        private void flowLayoutConditions_ControlAdded(object sender, ControlEventArgs e)
        {
            var conditionControl = e.Control as ConditionControl;
            conditionControl.TextBoxField.ContentsChanged += textBoxField_ContentsChanged;
            conditionControl.TextBoxExpression.ContentsChanged += textBoxExpression_ContentsChanged;
            conditionControl.TextBoxExpression.EnabledChanged += textBoxExpression_EnabledChanged;

            conditionControl.AddClicked += conditionControl_AddClicked;
            conditionControl.DeleteClicked += conditionControl_DeleteClicked;

            if (bAutoSelectTextBox && flowLayoutConditions.Controls.Count == 1)
            {
                SelectTextBox(conditionControl.TextBoxField);
            }

            updatePlusButtonsEnableState();
            updateAndOrButtonsVisibleState();

            //setLogicalOperatorLabels();
        }

        private void textBoxExpression_EnabledChanged(object sender, EventArgs e)
        {
            updatePlusButtonsEnableState();
        }

        private void textBoxExpression_ContentsChanged(object sender, EventArgs e)
        {
            updatePlusButtonsEnableState();
        }

        private void textBoxField_ContentsChanged(object sender, EventArgs e)
        {
            UpdateFieldsPaletteChoices();
        }

        private void conditionControl_DeleteClicked(object sender, EventArgs e)
        {
            var conditionControl = sender as ConditionControl;
            if (flowLayoutConditions.Controls.Count > 1)
            {
                flowLayoutConditions.Controls.Remove(conditionControl);
            }
            else
            {
                conditionControl.Clear();
            }
        }

        private void conditionControl_AddClicked(object sender, EventArgs e)
        {
            var conditionControl = new ConditionControl();
            addConditionControl(conditionControl);
            conditionControl.TextBoxExpression.Focus();
        }

        public void UpdateFieldsPaletteChoices()
        {
            var mcItemCollection = new Collection<IMcqItem>();

            foreach (ConditionControl conditionControl in flowLayoutConditions.Controls)
            {
                if (conditionControl.TextBoxField.Tag is IMcqItem)
                {
                    mcItemCollection.Add(conditionControl.TextBoxField.Tag as IMcqItem);
                }
                else if (conditionControl.TextBoxField.Tag is RecordField)
                {
                    var recordField = conditionControl.TextBoxField.Tag as RecordField;

                    if (recordField.ReferenceField is IMcqItem)
                    {
                        mcItemCollection.Add(recordField.ReferenceField as IMcqItem);
                    }
                }
                else if (conditionControl.TextBoxField.Tag is RecordSetField)
                {
                    var recordSetField = conditionControl.TextBoxField.Tag as RecordSetField;

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

        private void flowLayoutConditions_ControlRemoved(object sender, ControlEventArgs e)
        {
            updatePlusButtonsEnableState();
            updateAndOrButtonsVisibleState();
            UpdateFieldsPaletteChoices();
        }

        private void updatePlusButtonsEnableState()
        {
            ConditionControl conditionControl;

            for (int i = 0; i < flowLayoutConditions.Controls.Count - 1; ++i)
            {
                conditionControl = flowLayoutConditions.Controls[i] as ConditionControl;
                conditionControl.DisablePlus();
            }

            if (flowLayoutConditions.Controls.Count > 0)
            {
                conditionControl = flowLayoutConditions.Controls[flowLayoutConditions.Controls.Count - 1] as ConditionControl;
                if (conditionControl.IsEntirelyFull())
                {
                    conditionControl.EnablePlus();
                }
                else
                {
                    conditionControl.DisablePlus();
                }
            }
        }

        private void updateAndOrButtonsVisibleState()
        {
            comboBoxAndOr.Visible = flowLayoutConditions.Controls.Count > 1;
            labelWhere2.Visible = comboBoxAndOr.Visible;
        }

        #region Nested type: ConditionLogicalOperator

        public class ConditionLogicalOperator
        {
            public ConditionLogicalOperator(string displayText, LogicalOperator logicalOperator)
            {
                DisplayText = displayText;
                Operator = logicalOperator;
            }

            public string DisplayText { get; set; }
            public LogicalOperator Operator { get; set; }

            public override string ToString()
            {
                return Operator.ToString();
            }
        }

        #endregion

        #region Nested type: ConditonsFlowLayoutPanel

        private class ConditonsFlowLayoutPanel : FlowLayoutPanel
        {
            public ConditonsFlowLayoutPanel()
            {
                base.DoubleBuffered = true;
            }

            protected override void OnLayout(LayoutEventArgs levent)
            {
                if (Controls.Count > 0)
                {
                    Controls[0].Dock = DockStyle.None;
                }

                for (int i = 1; i < Controls.Count; ++i)
                {
                    Controls[i].Dock = DockStyle.Top;
                }
                if (Controls.Count > 0)
                {
                    Controls[0].Width = DisplayRectangle.Width - Controls[0].Margin.Horizontal;
                }
                base.OnLayout(levent);
            }
        }

        #endregion

        #region Nested type: DoubleBufferedFlowLayoutPanel

        private class DoubleBufferedFlowLayoutPanel : FlowLayoutPanel
        {
            public DoubleBufferedFlowLayoutPanel()
            {
                base.DoubleBuffered = true;
            }
        }

        #endregion
    }
}