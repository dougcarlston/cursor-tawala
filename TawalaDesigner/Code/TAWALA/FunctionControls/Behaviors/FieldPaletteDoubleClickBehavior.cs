// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.ProjectUI;

namespace Tawala.Functions.Controls
{
    public class FieldPaletteDoubleClickBehavior : Behavior
    {
        public FieldPaletteDoubleClickBehavior(IPaletteFieldAccepted da) : base(da as Control)
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
            var control = AssociatedControl as IPaletteFieldAccepted;

            if (e.Node != null & e.Node.Tag is IPaletteField)
            {
                if (control.IsAcceptedData(e.Node.Tag as IPaletteField))
                {
                    control.AcceptData(e.Node.Tag as IPaletteField);
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
            }
        }
    }
}