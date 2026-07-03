// Copyright © 2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Browser
{
    public partial class SourceView : Form
    {
        public SourceView(string html)
        {
            InitializeComponent();
            textBox.Text = html;
        }
    }
}