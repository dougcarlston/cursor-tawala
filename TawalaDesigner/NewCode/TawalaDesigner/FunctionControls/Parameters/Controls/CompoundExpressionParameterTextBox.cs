// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.ProjectUI;

namespace Tawala.Functions.Controls
{
    internal class CompoundExpressionParameterTextBox : TextBox, IParameterEditControl, IDataAccepted, ICustomDataSource
    {
        private FunctionCompoundExpression compoundExpression;
        private ScrollableControl scrollParent;

        public CompoundExpressionParameterTextBox()
        {
            base.BackColor = SystemColors.Window;
            base.ForeColor = SystemColors.WindowText;

            new HighlightBehavior(this);

            base.AllowDrop = true;
        }

        #region ICustomDataSource

        public object CustomDataSource
        {
            get { return compoundExpression; }
            set
            {
                var expression = value as FunctionCompoundExpression;
                if (expression != null)
                {
                    compoundExpression = expression;
                    if (CustomDataSourceChanged != null)
                    {
                        CustomDataSourceChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public event EventHandler CustomDataSourceChanged;

        #endregion

        #region IDataAccepted

        public bool IsAcceptedData(object o)
        {
            if (o is IDataObject)
            {
                IPaletteField field = ParameterUtils.FieldFromDataObject(o as IDataObject);
                if (isAcceptedData(field))
                {
                    return true;
                }
            }

            return false;
        }

        public void AcceptData(object o)
        {
            if (o is IDataObject)
            {
                o = ParameterUtils.FieldFromDataObject(o as IDataObject);
            }

            IParameterInfo parameterInfo = ParameterControlManager.LookupParameterInfo(this);

            IPaletteField field = FunctionUtil.ApplyParameterRestrictions(parameterInfo, o as IPaletteField);

            SelectedText = field.FieldString;
        }

        #endregion

        #region IParameterEditControl Members

        public void CommitPendingChanges()
        {
        }

        public Control GetControl()
        {
            return this;
        }

        #endregion

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            Focus();
            object o = drgevent.Data;
            drgevent.Effect = IsAcceptedData(o) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            object o = drgevent.Data;

            if (IsAcceptedData(o))
            {
                drgevent.Effect = DragDropEffects.Copy;
                AcceptData(o);
            }
            else
            {
                drgevent.Effect = DragDropEffects.None;
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
            base.OnGotFocus(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            handleChangedText();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            handleChangedText();
        }

        private bool textChanged;

        protected override void OnTextChanged(EventArgs e)
        {
            textChanged = true;
        }

        private void handleChangedText()
        {
            if (textChanged)
            {
                textChanged = false;
                FunctionCompoundExpression expression = FunctionCompoundExpression.Parse(Text);
                CustomDataSource = expression;
            }
        }

        private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null && e.Node.Tag is IField)
            {
                var dataObject = new DataObject();
                dataObject.SetData(e.Node.Tag);

                if (IsAcceptedData(dataObject))
                {
                    AcceptData(dataObject);
                    return;
                }
            }
        }

        private static bool isAcceptedData(IField field)
        {
            if (field is IBlank || field is IHiddenField || field is Variable)
            {
                return true;
            }

            if (field is RecordField)
            {
                IField referencedField = ((RecordField)field).ReferenceField;
                if (referencedField is IBlank || referencedField is IHiddenField)
                {
                    return true;
                }
            }

            return false;
        }
    }
}