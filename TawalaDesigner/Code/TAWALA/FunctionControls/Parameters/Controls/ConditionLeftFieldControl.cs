// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Controls;
using Tawala.Projects;
using Tawala.Projects.Fields;

namespace Tawala.Functions.Controls
{
    internal class ConditionLeftFieldControl : FieldTextBox, IPaletteFieldAccepted
    {
        private bool wasEmpty = true;

        #region IPaletteFieldAccepted Members

        public bool IsAcceptedData(IPaletteField field)
        {
            return field != null;
        }

        public void AcceptData(IPaletteField field)
        {
            field = FieldUtil.RecordQualifySharedDataField(field) as IPaletteField;

            bool typeChange = TypeChanging(field);

            Tag = field;
            Text = field.QualifiedFieldName;

            if (typeChange)
            {
                raiseTypeChanged();
            }

            raiseContentsChanged();
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

        public event EventHandler TypeChanged;

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

        private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null && e.Node.Tag is IPaletteField)
            {
                var field = e.Node.Tag as IPaletteField;

                if (IsAcceptedData(field))
                {
                    AcceptData(field);
                }

                if (Parent != null)
                {
                    int index = Parent.Controls.GetChildIndex(this);
                    Parent.Controls[index + 2].Focus();
                }
            }
        }

        private void raiseContentsChanged()
        {
            if (ContentsChanged != null)
            {
                ContentsChanged(this, EventArgs.Empty);
            }
        }

        private void raiseTypeChanged()
        {
            if (TypeChanged != null)
            {
                TypeChanged(this, EventArgs.Empty);
            }
        }
    }
}