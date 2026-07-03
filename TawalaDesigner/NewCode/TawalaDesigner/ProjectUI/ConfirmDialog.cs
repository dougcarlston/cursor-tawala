// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.ProjectUI.Properties;

namespace Tawala.ProjectUI
{
    public partial class ConfirmDialog : Form
    {
        // op becomes part of title and message

        protected ConfirmDialog(string op, string typeName, string instanceName)
        {
            InitializeComponent();

            Text = string.Format(Text, op, typeName);
            labelMessage.Text = string.Format(labelMessage.Text, op, typeName, instanceName);
            buttonDoOp.Text = string.Format(buttonDoOp.Text, op, typeName);
            buttonDoOp.AutoSize = true;
            buttonDoOp.Left = Width/4 - buttonDoOp.Width/2;
            buttonCancel.Width = buttonDoOp.Width;
            buttonCancel.Left = (Width/4)*3 - buttonDoOp.Width/2;
        }

        public static bool Confirm(string op, string typeName, string instanceName)
        {
            var dialog = new ConfirmDialog(op, typeName, instanceName);
            return dialog.ShowDialog() == DialogResult.OK;
        }

        public static bool ConfirmDelete(string typeName, string instanceName)
        {
            return Confirm(Resources.ConfirmOpDelete, typeName, instanceName);
        }
    }
}