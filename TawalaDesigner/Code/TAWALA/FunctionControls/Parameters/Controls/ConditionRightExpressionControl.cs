// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Controls;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;

namespace Tawala.Functions.Controls
{
    internal class ConditionRightExpressionControl : ExpressionTextBox, IPaletteFieldAccepted
    {
        private bool wasEmpty = true;

        public Expression Expression
        {
            get
            {
                return Tag as Expression;
            }
            set
            {
                if (value != null)
                {
                    Tag = value;
                    Text = value.Elements[0].Text;
                    raiseContentsChanged();
                }
            }
        }

        #region IPaletteFieldAccepted Members

        public bool IsAcceptedData(IPaletteField field)
        {
            return field != null;
        }

        public void AcceptData(IPaletteField field)
        {
            field = FieldUtil.RecordQualifySharedDataField(field) as IPaletteField;

            if (field != null)
            {
                Expression = new Expression(field);
            }
        }

        public AcceptDataActions AcceptActions
        {
            get { return AcceptDataActions.NextControl; }
        }

        public bool CustomControl
        {
            get
            {
                return false;
            }
        }

        public void CommitPendingChanges()
        {
        }

        #endregion

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            new HighlightBehavior(this);
            new DragDropBehavior(this);

        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            ProjectUI.FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            ProjectUI.FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
        }

        /// <summary>
        /// Empty to non-empty or vice versa or field drop
        /// </summary>
        public event EventHandler ContentsChanged;

        protected override void OnTextChanged(EventArgs e)
        {
            bool isEmpty = Text.Length == 0;

            if (wasEmpty != isEmpty)
            {
                wasEmpty = isEmpty;
                raiseContentsChanged();
            }
            base.OnTextChanged(e);
        }

        private void raiseContentsChanged()
        {
            if (ContentsChanged != null)
            {
                ContentsChanged(this, EventArgs.Empty);
            }
        }

        private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null && e.Node.Tag is IPaletteField)
            {
                AcceptData(e.Node.Tag as IPaletteField);
            }

            if (Parent != null)
            {
                Parent.SelectNextControl(this, true, true, true, false);
            }
        }
    }
}