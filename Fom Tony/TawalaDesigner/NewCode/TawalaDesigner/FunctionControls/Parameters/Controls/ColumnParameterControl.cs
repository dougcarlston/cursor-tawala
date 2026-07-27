// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    [ComplexBindingProperties("DataSource")]
    internal partial class ColumnParameterControl : UserControl, IParameterEditControl
    {
        private readonly BindingSource bindingSource = new BindingSource();
        private readonly IParameterInfo rootParameterInfo;

        public ColumnParameterControl(IParameterInfo parameterInfo)
        {
            rootParameterInfo = parameterInfo;
            ParameterControlManager.RegisterBoundControl(this, rootParameterInfo, null);

            InitializeComponent();
            ResizeRedraw = true;

            ParameterControlManager.RegisterBoundControl(textBoxHeading, rootParameterInfo.Parameters[0], null);
            ParameterControlManager.RegisterBoundControl(textBoxColumnContents, rootParameterInfo.Parameters[1], null);
            ParameterControlManager.RegisterBoundControl(conditionsControl, rootParameterInfo.Parameters[2], null);

            bindingSource.DataSourceChanged += bindingSource_DataSourceChanged;

            setConditionsLinkText();
        }

        public object DataSource { get { return bindingSource.DataSource; } set { bindingSource.DataSource = value; } }

        #region IParameterEditControl Members

        public void CommitPendingChanges()
        {
        }

        public Control GetControl()
        {
            return this;
        }

        #endregion

        protected override void OnTabIndexChanged(EventArgs e)
        {
            base.OnTabIndexChanged(e);
            groupBoxColumnInfo.Text = string.Format(groupBoxColumnInfo.Text, TabIndex);
        }

        private void bindingSource_DataSourceChanged(object sender, EventArgs e)
        {
            bindHeader();
            bindContents();
            bindConditions();
        }

        private void bindConditions()
        {
            var conditionsEdit = new Binding("Conditions", bindingSource, rootParameterInfo.Parameters[2].PropertyName, true);
            conditionsEdit.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            conditionsEdit.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            conditionsEdit.DataSourceNullValue = null;
            conditionsEdit.NullValue = null;

            conditionsControl.DataBindings.Add(conditionsEdit);
        }

        private void bindContents()
        {
            var contentsText = new Binding("Text", bindingSource, rootParameterInfo.Parameters[1].PropertyName, true);
            contentsText.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            contentsText.DataSourceUpdateMode = DataSourceUpdateMode.Never;
            contentsText.DataSourceNullValue = null;
            contentsText.NullValue = string.Empty;

            var contentsEdit = new Binding("CustomDataSource", bindingSource, rootParameterInfo.Parameters[1].PropertyName, true);
            contentsEdit.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            contentsEdit.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            contentsEdit.DataSourceNullValue = null;
            contentsEdit.NullValue = string.Empty;

            textBoxColumnContents.DataBindings.Add(contentsText);
            textBoxColumnContents.DataBindings.Add(contentsEdit);
        }

        private void bindHeader()
        {
            var headingText = new Binding("Text", bindingSource, rootParameterInfo.Parameters[0].PropertyName, true);
            headingText.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            headingText.DataSourceUpdateMode = DataSourceUpdateMode.Never;
            headingText.DataSourceNullValue = null;
            headingText.NullValue = string.Empty;

            var headingEdit = new Binding("CustomDataSource", bindingSource, rootParameterInfo.Parameters[0].PropertyName, true);
            headingEdit.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            headingEdit.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            headingEdit.DataSourceNullValue = null;
            headingEdit.NullValue = string.Empty;

            textBoxHeading.DataBindings.Add(headingText);
            textBoxHeading.DataBindings.Add(headingEdit);
        }

        private void linkLabelConditions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SuspendLayout();

            bool conditionsVisible = !conditionsControl.Visible;

            RowStyle rowStyle = tableLayoutPanel.RowStyles[3];

            if (conditionsVisible)
            {
                rowStyle.SizeType = SizeType.AutoSize;
            }
            else
            {
                rowStyle.SizeType = SizeType.Absolute;
                rowStyle.Height = 0;
            }

            conditionsControl.Visible = conditionsVisible;

            setConditionsLinkText();

            ResumeLayout(false);
            PerformLayout();
        }

        private void setConditionsLinkText()
        {
            if (conditionsControl.Visible)
            {
                linkLabelConditions.Text = "- Close conditions panel";
            }
            else
            {
                if (conditionsControl.Conditions.Count > 0)
                {
                    linkLabelConditions.Text = "+ Column is displayed conditionally";
                }
                else
                {
                    linkLabelConditions.Text = "+ Column is always displayed";
                }
            }
        }
    }
}