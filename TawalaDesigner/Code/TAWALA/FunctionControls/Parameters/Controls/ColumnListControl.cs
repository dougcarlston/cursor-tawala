// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    internal partial class ColumnListControl : FlowLayoutPanel, IParameterControl
    {
        private BindingList<ICompositeParameter> bindingList;

        public ColumnListControl()
        {
            InitializeComponent();
            base.DoubleBuffered = true;

            ControlManager.ConfigureFunctionControl.HookButton("Plus", toolStripAdd_Click);
            ControlManager.ConfigureFunctionControl.HookButton("Minus", toolStripRemove_Click);
            ControlManager.ConfigureFunctionControl.HookButton("MoveUp", toolStripMoveUp_Click);
            ControlManager.ConfigureFunctionControl.HookButton("MoveDown", toolStripMoveDown_Click);
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

        private void toolStripAdd_Click(object sender, EventArgs e)
        {
            bindingList.Add(createColumn());
        }

        private void toolStripRemove_Click(object sender, EventArgs e)
        {
            if (bindingList.Count > 1)
            {
                bindingList.RemoveAt(bindingList.Count - 1);
            }
        }

        private void toolStripMoveUp_Click(object sender, EventArgs e)
        {
            moveColumnWithFocus(true);
        }

        private void toolStripMoveDown_Click(object sender, EventArgs e)
        {
            moveColumnWithFocus(false);
        }

        private void moveColumnWithFocus(bool up)
        {
            if (!ContainsFocus)
            {
                return;
            }

            if (bindingList.Count <= 1)
            {
                return;
            }

            int column = getColumnWithFocus();

            int src = column;
            int dst = up ? column - 1 : column + 1;

            if (dst < 0 || dst >= bindingList.Count)
            {
                return;
            }

            ICompositeParameter o = bindingList[src];
            bindingList[src] = bindingList[dst];
            bindingList[dst] = o;

            bindingList.ResetBindings();
        }

        private int getColumnWithFocus()
        {
            foreach (ColumnControl columnControl in Controls)
            {
                if (columnControl.ContainsFocus)
                {
                    return Controls.GetChildIndex(columnControl);
                }
            }

            return -1;
        }

        protected override void InitLayout()
        {
            bindingList = getUnderlyingCollection().CreateBindingList();

            if (bindingList.Count == 0)
            {
                bindingList.Add(createColumn());
            }

            for (int i = 0; i < bindingList.Count; ++i)
            {
                addColumn(i);
            }

            bindingList.ListChanged += bindingList_ListChanged;

            base.InitLayout();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            int width = ClientSize.Width;

            foreach (Control c in Controls)
            {
                c.MinimumSize = new Size(width, c.MinimumSize.Height);
                c.Width = width;
            }
            base.OnLayout(levent);
        }

        private ICompositeParameterCollection getUnderlyingCollection()
        {
            IParameterInfo parameterInfo = ControlManager.LookupParameterInfo(this);
            return ControlManager.Function[parameterInfo.Id] as ICompositeParameterCollection;
        }

        private void bindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                {
                    addColumn(e.NewIndex);
                    break;
                }
                case ListChangedType.ItemDeleted:
                {
                    removeColumn(e.NewIndex);
                    break;
                }
                case ListChangedType.ItemMoved:
                {
                    break;
                }
                case ListChangedType.ItemChanged:
                {
                    //                   raiseConditionsChanged();
                    break;
                }
            }
        }

        private void addColumn(int index)
        {
            var control = new ColumnControl();
            ControlManager.RegisterControl(control, ControlManager.LookupParameterInfo(this), null);

            SuspendLayout();
            control.SuspendLayout();

            control.Bind(bindingList, index);
            Controls.Add(control);
            Controls.SetChildIndex(control, index);

            control.ResumeLayout(false);
            ResumeLayout(false);

            PerformLayout();
        }

        private void removeColumn(int index)
        {
            Controls.RemoveAt(index);
        }

        private ICompositeParameter createColumn()
        {
            ICompositeParameterCollection collection = getUnderlyingCollection();
            ICompositeParameter item = collection.CreateItem();
            return item;
        }
    }
}