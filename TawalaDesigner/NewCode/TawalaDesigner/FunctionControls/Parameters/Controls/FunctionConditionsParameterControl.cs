// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Controls;
using Tawala.Projects;
using Tawala.ProjectUI;

namespace Tawala.Functions.Controls
{
    [DefaultBindingProperty("Conditions")]
    public partial class FunctionConditionsParameterControl : UserControl, IParameterEditControl
    {
        private readonly ConditionGroupCollection groups;

        public FunctionConditionsParameterControl()
        {
            new DebugInitEventsBehavior(this);
            InitializeComponent();

            ResizeRedraw = true;
            groups = new ConditionGroupCollection(groupBoxWhere, comboBoxAndOr, false);

            groupBoxWhere.ControlAdded += groupBoxWhere_ControlAdded;
            groupBoxWhere.ControlRemoved += groupBoxWhere_ControlRemoved;
        }

        public string WhereText { get { return labelWhere.Text; } set { labelWhere.Text = value; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Conditions Conditions { get { return groups.Conditions; } set { groups.Conditions = value; } }

        #region IParameterEditControl Members

        public void CommitPendingChanges()
        {
            RaiseConditionsChanged();
        }

        #endregion

        #region IParameterControl

        public Control GetControl()
        {
            return this;
        }

        #endregion

        public event EventHandler ConditionsChanged;

        public void RaiseConditionsChanged()
        {
            if (ConditionsChanged != null)
            {
                ConditionsChanged(this, EventArgs.Empty);
            }
        }

        protected override void InitLayout()
        {
            MinimumSize = new Size(Parent.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4, 0);
            MaximumSize = MinimumSize;
            Size = MinimumSize;

            base.InitLayout();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!DesignMode)
            {
                FieldsPalette.Palette.ConditionsForms = FunctionFormCollection.NULL;
            }
            base.OnHandleDestroyed(e);
        }

        private void groupBoxWhere_ControlAdded(object sender, ControlEventArgs e)
        {
            var group = e.Control as ConditionGroup;

            group.Leave += conditionControl_Change;
            group.TextBoxField.LostFocus += conditionControl_Change;
            group.ComboBoxOperator.SelectedIndexChanged += conditionControl_Change;
            group.TextBoxField.LostFocus += conditionControl_Change;

            new HighlightBehavior(group.TextBoxField);
            new HighlightBehavior(group.ComboBoxOperator);
            new HighlightBehavior(group.TextBoxExpression);

            group.Visible = true;

            adjustTabs();

            RaiseConditionsChanged();
        }

        private void groupBoxWhere_ControlRemoved(object sender, ControlEventArgs e)
        {
            adjustTabs();
        }

        private void conditionControl_Change(object sender, EventArgs e)
        {
            RaiseConditionsChanged();
        }

        private void adjustTabs()
        {
            for (int i = 0; i < groupBoxWhere.Controls.Count; ++i)
            {
                groupBoxWhere.Controls[i].TabIndex = i;
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (Enabled)
            {
                groups.UpdateFieldsPaletteChoices();
                RaiseConditionsChanged();
            }
        }

        private void groupBoxWhere_Layout(object sender, LayoutEventArgs e)
        {
            int top = 14;

            foreach (Control c in groupBoxWhere.Controls)
            {
                c.Top = top;
                c.Left = 2;
                c.Width = groupBoxWhere.ClientSize.Width - 4;
                c.PerformLayout();
                top = c.Bottom;
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
                PerformLayout();
            }
            else
            {
                labelWhere2.Visible = false;
            }
            RaiseConditionsChanged();
        }
    }
}