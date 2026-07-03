// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    public class DragDropBehavior : Behavior
    {
        public DragDropBehavior(IDataAccepted da) : base(da.GetControl())
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
            object o = e.Data;
            var da = AssociatedControl as IDataAccepted;

            if (da.IsAcceptedData(o))
            {
                e.Effect = DragDropEffects.Copy;
                da.AcceptData(o);
                NextControl();
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void c_DragEnter(object sender, DragEventArgs e)
        {
            AssociatedControl.Focus();
            object o = e.Data;
            var da = AssociatedControl as IDataAccepted;
            e.Effect = da.IsAcceptedData(o) ? DragDropEffects.Copy : DragDropEffects.None;
        }
    }
}