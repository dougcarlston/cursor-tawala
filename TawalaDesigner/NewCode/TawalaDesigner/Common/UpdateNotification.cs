// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Tawala.Common
{
    public partial class UpdateNotification : Form
    {
        private readonly string url = string.Empty;

        public UpdateNotification(string url, bool mandatory)
        {
            InitializeComponent();
            this.url = url;
            labelMandatory.Visible = mandatory;
            linkLabel.VisitedLinkColor = linkLabel.LinkColor;
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs args)
        {
            try
            {
                Process.Start(url);
            }
            finally
            {
                Close();
            }
        }
    }
}