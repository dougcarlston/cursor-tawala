// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    public class HighlightBehavior : Behavior
    {
        private static readonly Color SELECTED = Color.FromArgb(210, 255, 210);
        private static readonly Color UNSELECTED = SystemColors.Window;

        public HighlightBehavior(Control c) : base(c)
        {
            c.Enter += c_Selected;
            c.GotFocus += c_Selected;
            c.Leave += c_Deselected;
        }

        public override void Detach(Control c)
        {
            c.Enter -= c_Selected;
            c.GotFocus -= c_Selected;
            c.Leave -= c_Deselected;
        }

        private void c_Deselected(object sender, EventArgs e)
        {
            AssociatedControl.BackColor = UNSELECTED;
        }

        private void c_Selected(object sender, EventArgs e)
        {
            resetRenegades();

            AssociatedControl.BackColor = SELECTED;
        }

        private void resetRenegades()
        {
            Control parent = AssociatedControl.Parent;

            while (parent != null && !(parent is IConfigureFunctionControl))
            {
                parent = parent.Parent;
            }

            if (parent != null)
            {
                recurse(parent.Controls);
            }
        }

        private void recurse(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c.BackColor == SELECTED && c != AssociatedControl)
                {
                    c.BackColor = UNSELECTED;
                }

                recurse(c.Controls);
            }
        }
    }
}