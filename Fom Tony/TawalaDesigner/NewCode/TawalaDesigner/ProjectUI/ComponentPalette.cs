// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.ProjectUI
{
    public partial class ComponentPalette : UserControl
    {
        public ComponentPalette()
        {
            InitializeComponent();
            Dock = DockStyle.Left;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            Width = e.Control.Width;
            e.Control.Dock = DockStyle.Fill;
        }
    }
}