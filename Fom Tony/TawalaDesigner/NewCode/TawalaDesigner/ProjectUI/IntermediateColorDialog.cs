// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.ProjectUI
{
    public partial class IntermediateColorDialog : Form
    {
        public IntermediateColorDialog()
        {
            InitializeComponent();
        }

        public bool UseProjectThemeColor
        {
            get { return radioButtonUseThemeColor.Checked; }
        }
    }
}