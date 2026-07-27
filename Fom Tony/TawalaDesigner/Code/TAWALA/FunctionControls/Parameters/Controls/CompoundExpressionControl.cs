// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;

namespace Tawala.Functions.Controls
{
    internal class CompoundExpressionControl : TextBox, IParameterControl, IPaletteFieldAccepted
    {
        #region IParameterControl Members

        public bool CustomControl { get { return false; } }

        public void CommitPendingChanges()
        {
            DataBindings[0].WriteValue();
        }

        #endregion

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            BackColor = SystemColors.Window;
            ForeColor = SystemColors.WindowText;

            new HighlightBehavior(this);
            new DragDropBehavior(this);
            new FieldPaletteDoubleClickBehavior(this);
        }

        protected override void OnLeave(EventArgs e)
        {
            CommitPendingChanges();
            base.OnLeave(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            CommitPendingChanges();
            base.OnLostFocus(e);
        }

        #region IPaletteFieldAccepted

        public bool IsAcceptedData(IPaletteField field)
        {
            if (field is IBlank || field is IHiddenField || field is IFileUploadItem || field is Variable)
            {
                return true;
            }

            if (field is RecordField)
            {
                IField referencedField = ((RecordField)field).ReferenceField;
                if (referencedField is IBlank || referencedField is IHiddenField || field is IFileUploadItem)
                {
                    return true;
                }
            }

            return false;
        }

        public void AcceptData(IPaletteField field)
        {
            IParameterInfo parameterInfo = ControlManager.LookupParameterInfo(this);
            field = FunctionUtil.ApplyParameterRestrictions(parameterInfo, field);

            SelectedText = field.FieldString;

            CommitPendingChanges();
        }

        public AcceptDataActions AcceptActions { get { return AcceptDataActions.Focus | AcceptDataActions.NoSelection; } }

        #endregion
    }
}