// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.DesignerUI
{
    public partial class ProjectTree : TreeView, IMessageFilter
    {
        public ProjectTree()
        {
            InitializeComponent();
        }

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m)
        {
            cancelLabelEditOnButtonMessage(ref m);

            return false;
        }

        #endregion

        public int GetIdealWidth(TreeNodeCollection nodes, int width)
        {
            foreach (TreeNode n in nodes)
            {
                if (n.Bounds.Left + n.Bounds.Width > width)
                {
                    width = n.Bounds.Left + n.Bounds.Width;
                }

                width = GetIdealWidth(n.Nodes, width);
            }

            return width;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            Application.AddMessageFilter(this);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Application.RemoveMessageFilter(this);

            base.OnHandleDestroyed(e);
        }

        private void cancelLabelEditOnButtonMessage(ref Message m)
        {
            if (Win32.IsButtonClick(ref m))
            {
                if (SelectedNode != null && SelectedNode.IsEditing && !Win32.IsDescendant(Handle, m.HWnd))
                {
                    SelectedNode.EndEdit(false);
                }
            }
        }
    }
}