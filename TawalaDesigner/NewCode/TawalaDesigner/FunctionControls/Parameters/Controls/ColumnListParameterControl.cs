// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    [ComplexBindingProperties("DataSource")]
    internal partial class ColumnListParameterControl : UserControl, IParameterEditControl
    {
        private readonly BindingSource bindingSource = new BindingSource();
        private bool layoutInitialized;

        public ColumnListParameterControl()
        {
            InitializeComponent();
            base.DoubleBuffered = true;
            ResizeRedraw = true;
            bindingSource.DataSourceChanged += bindingSource_DataSourceChanged;
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

        private void bindingSource_DataSourceChanged(object sender, EventArgs e)
        {
            bindingSource.ListChanged += bindingSource_ListChanged;
            synchronize();
        }

        protected override void InitLayout()
        {
            MinimumSize = new Size(Parent.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 2, 0);
            MaximumSize = MinimumSize;
            Size = MinimumSize;

            flowLayoutPanelColumns.MinimumSize = new Size(ClientSize.Width - 2, 0);
            flowLayoutPanelColumns.MaximumSize = flowLayoutPanelColumns.MinimumSize;
            flowLayoutPanelColumns.Size = flowLayoutPanelColumns.MinimumSize;

            base.InitLayout();

            foreach (Control child in flowLayoutPanelColumns.Controls)
            {
                setChildSize(child);
            }

            layoutInitialized = true;
        }

        private void setChildSize(Control child)
        {
            child.MinimumSize = new Size(flowLayoutPanelColumns.ClientSize.Width - 2, 0);
            child.MaximumSize = child.MinimumSize;
        }

        private void bindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            synchronize();
        }

        private void synchronize()
        {
            if (flowLayoutPanelColumns.Controls.Count == bindingSource.Count)
            {
                return;
            }

            if (flowLayoutPanelColumns.Controls.Count < bindingSource.Count)
            {
                addChildren();
            }
            else
            {
                removeChildren();
            }
        }

        private void removeChildren()
        {
            while (flowLayoutPanelColumns.Controls.Count > bindingSource.Count)
            {
                int index = flowLayoutPanelColumns.Controls.Count - 1;
                flowLayoutPanelColumns.Controls.RemoveAt(index);
            }
        }

        private void addChildren()
        {
            IParameterInfo parameterInfo = ParameterControlManager.LookupParameterInfo(this);

            while (flowLayoutPanelColumns.Controls.Count < bindingSource.Count)
            {
                int index = flowLayoutPanelColumns.Controls.Count;
                var cpc = new ColumnParameterControl(parameterInfo);
                cpc.TabIndex = index + 1;
                cpc.TabStop = true;

                if (layoutInitialized)
                {
                    setChildSize(cpc);
                }

                flowLayoutPanelColumns.Controls.Add(cpc);
                flowLayoutPanelColumns.Controls.SetChildIndex(cpc, index);
                flowLayoutPanelColumns.SetFlowBreak(cpc, true);

                cpc.DataSource = bindingSource[index];
            }
        }
    }
}