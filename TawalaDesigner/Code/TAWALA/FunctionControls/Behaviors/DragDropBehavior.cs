// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.Functions.Controls
{
    public class DragDropBehavior : Behavior
    {
        public DragDropBehavior(IPaletteFieldAccepted da) : base(da as Control)
        {
            AssociatedControl.AllowDrop = true;
            AssociatedControl.DragEnter += c_DragEnter;
            AssociatedControl.DragDrop += c_DragDrop;
        }

        public override void Detach(Control c)
        {
            c.DragEnter -= c_DragEnter;
            c.DragDrop -= c_DragDrop;
        }

        private void c_DragDrop(object sender, DragEventArgs e)
        {
            IPaletteField field = ParameterUtils.FieldFromDataObject(e.Data);
            var control = AssociatedControl as IPaletteFieldAccepted;

            if (control.IsAcceptedData(field))
            {
                e.Effect = DragDropEffects.Copy;
                control.AcceptData(field);

                if ((control.AcceptActions & AcceptDataActions.NextControl) == AcceptDataActions.NextControl)
                {
                    NextControl();
                }
                
                if ((control.AcceptActions & AcceptDataActions.Focus) == AcceptDataActions.Focus)
                {
                    AssociatedControl.Focus();
                }

                if ((control.AcceptActions & AcceptDataActions.NoSelection) == AcceptDataActions.NoSelection)
                {
                    if (AssociatedControl is TextBoxBase)
                    {
                        var textBoxBase = AssociatedControl as TextBoxBase;
                        textBoxBase.Select(textBoxBase.Text.Length, 0);
                    }
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void c_DragEnter(object sender, DragEventArgs e)
        {
            var control = AssociatedControl as IPaletteFieldAccepted;
            IPaletteField field = ParameterUtils.FieldFromDataObject(e.Data);
            e.Effect = control.IsAcceptedData(field) ? DragDropEffects.Copy : DragDropEffects.None;
        }
    }
}