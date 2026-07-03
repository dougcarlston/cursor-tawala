// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Tawala.Function.Controls.Properties;
using Tawala.Functions.Runtime;
using Tawala.Projects;

namespace Tawala.Functions.Controls
{
    internal partial class ColumnControl : UserControl, IParameterControl
    {
        private static string groupBoxFormat;
        private BindingSource bindingSource;
        private BindingSource conditionsBindingSource;
        private IParameterInfo rootParameterInfo;

        public ColumnControl()
        {
            InitializeComponent();
            if (groupBoxFormat == null)
            {
                groupBoxFormat = groupBox.Text;
            }
            TabStop = true;
            conditionListControl.Visible = false;
            conditionListControl.TabStop = false;
            conditionListControl.VisibleChanged += conditionListControl_VisibleChanged;
            base.Dock = DockStyle.Fill;
        }

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

        public void Bind(BindingList<ICompositeParameter> bindingList, int index)
        {
            rootParameterInfo = ControlManager.LookupParameterInfo(this);

            IParameterInfo headingInfo = rootParameterInfo.Parameters[0];
            IParameterInfo contentInfo = rootParameterInfo.Parameters[1];
            IParameterInfo conditionsInfo = rootParameterInfo.Parameters[2];

            ControlManager.RegisterControl(headingControl, headingInfo, null);
            ControlManager.RegisterControl(contentControl, contentInfo, null);
            ControlManager.RegisterControl(conditionListControl, conditionsInfo, null);

            TabIndex = index+1;
            groupBox.Text = string.Format(groupBoxFormat, TabIndex);

            headingControl.Enter += control_Enter;
            contentControl.Enter += control_Enter;
            conditionListControl.Enter += control_Enter;

            bindingSource = new BindingSource();
            bindingSource.DataSource = bindingList[index];

            headingControl.Bind(bindingSource, headingInfo);
            contentControl.Bind(bindingSource, contentInfo);
            bindDisplayConditionList(conditionsInfo);

            setConditionsLinkText();
        }

        private void control_Enter(object sender, EventArgs e)
        {
            IParameterInfo parameterInfo = ControlManager.LookupParameterInfo(sender as IParameterControl);
            var replacementDictionary = new Dictionary<string, string> {{"$n$", TabIndex.ToString()}};
            ControlManager.SetCurrentParameterInfo(parameterInfo, replacementDictionary);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent == null)
            {
                bindingSource.SuspendBinding();
                bindingSource = null;
            }
        }

        private void bindDisplayConditionList(IParameterInfo parameterInfo)
        {
            conditionsBindingSource = new BindingSource(bindingSource, parameterInfo.PropertyName);
            var conditionsBinding = new Binding("Conditions", conditionsBindingSource, "Conditions", true);
            conditionsBinding.DataSourceNullValue = new Conditions();
            conditionsBinding.NullValue = new Conditions();
            conditionListControl.DataBindings.Add(conditionsBinding);
            conditionsBinding.BindingComplete += conditionsBinding_BindingComplete;
        }

        private void conditionsBinding_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate)
            {
                setConditionsLinkText();
            }
        }

        private void setConditionsLinkText()
        {
            string text;

            if (conditionListControl.Visible)
            {
                text = Resources.CloseConditons;
            }
            else
            {
                var functionParameterConditions = conditionsBindingSource.Current as FunctionParameterConditions;
                if (functionParameterConditions.Conditions.Count > 0)
                {
                    text = Resources.OpenConditionsWithConditions;
                }
                else
                {
                    text = Resources.OpenConditionsNone;
                }
            }

            linkLabelConditions.Text = text;
        }

        private void linkLabelConditions_MouseUp(object sender, MouseEventArgs e)
        {
            conditionListControl.Visible = !conditionListControl.Visible;
        }

        private void conditionListControl_VisibleChanged(object sender, EventArgs e)
        {
            conditionListControl.TabStop = conditionListControl.Visible;

            setConditionsLinkText();

            PerformLayout();

            if (conditionListControl.Visible)
            {
                conditionListControl.Focus();
            }
            else
            {
                headingControl.Focus();
            }
        }

        private void groupBox_Layout(object sender, LayoutEventArgs e)
        {
            conditionListControl.Width = groupBox.Width - conditionListControl.Left - 8;
        }
    }
}