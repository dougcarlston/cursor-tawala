// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.ProjectUI;

namespace Tawala.Functions.Controls
{
    public class FieldPaletteDoubleClickBehavior : Behavior
    {
        public FieldPaletteDoubleClickBehavior(IDataAccepted da) : base(da as Control)
        {
            AssociatedControl.Enter += c_Hook;
            AssociatedControl.GotFocus += c_Hook;
        }

        public override void Detach(Control c)
        {
            c.Enter -= c_Hook;
            c.GotFocus -= c_Hook;
        }

        private void c_Hook(object sender, EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick += Palette_FieldNodeDoubleClick;
        }

        private void Palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var da = AssociatedControl as IDataAccepted;

            if (e.Node != null && e.Node.Tag != null && e.Node.Tag is IField)
            {
                if (da.IsAcceptedData(e.Node.Tag))
                {
                    da.AcceptData(e.Node.Tag);
                    NextControl();
                    return;
                }
            }
        }
    }
}