// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Tawala.Functions.Controls.Design
{
    public class ConfigureParametersLayoutPanelDesigner : ControlDesigner
    {
        protected override void PostFilterProperties(IDictionary properties)
        {
            properties.Remove("MaximumSize");
            properties.Remove("MinimumSize");
            base.PostFilterProperties(properties);
        }

        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            using (var font = new Font("Arial", 16.0f, FontStyle.Bold))
            {
                pe.Graphics.DrawString("ConfigureParametersLayoutPanel", font, Brushes.Black, 20.0f, 100.0f);
            }
            base.OnPaintAdornments(pe);
        }
    }
}