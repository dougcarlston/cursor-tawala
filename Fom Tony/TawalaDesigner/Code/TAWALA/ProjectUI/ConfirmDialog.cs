using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
            buttonDoOp.Left = Width / 4 - buttonDoOp.Width / 2;
            buttonCancel.Width = buttonDoOp.Width;
            buttonCancel.Left = (Width / 4) * 3 - buttonDoOp.Width / 2;
        }

        static public bool Confirm(string op, string typeName, string instanceName)
        {
            ConfirmDialog dialog = new ConfirmDialog(op, typeName, instanceName);
            return dialog.ShowDialog() == DialogResult.OK;
        }

        static public bool ConfirmDelete(string typeName, string instanceName)
        {
            return Confirm(Properties.Resources.ConfirmOpDelete, typeName, instanceName);
        }
    }
}