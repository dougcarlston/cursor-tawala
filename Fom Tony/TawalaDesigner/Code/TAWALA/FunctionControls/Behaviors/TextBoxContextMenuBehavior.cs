// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    public class TextBoxContextMenuBehavior : Behavior
    {
        private static ContextMenuStrip contextMenuStrip;
        private static ToolStripMenuItem toolStripMenuItemCopy;
        private static ToolStripMenuItem toolStripMenuItemCut;
        private static ToolStripMenuItem toolStripMenuItemDelete;
        private static ToolStripMenuItem toolStripMenuItemPaste;
        private static ToolStripMenuItem toolStripMenuItemSelectAll;
        private static ToolStripMenuItem toolStripMenuItemUndo;
        private static ToolStripSeparator toolStripSeparator1;
        private static ToolStripSeparator toolStripSeparator2;

        public TextBoxContextMenuBehavior(Control c) : base(c)
        {
        }

        public override void Detach(Control c)
        {
        }

        private static void createContextMenu()
        {
            if (contextMenuStrip != null)
            {
                return;
            }

            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                toolStripSeparator1 = new ToolStripSeparator(), toolStripMenuItemCut = new ToolStripMenuItem("Cut"),
                toolStripMenuItemCopy = new ToolStripMenuItem("Copy"), toolStripMenuItemPaste = new ToolStripMenuItem("Paste"),
                toolStripMenuItemDelete = new ToolStripMenuItem("Delete"), toolStripSeparator2 = new ToolStripSeparator(),
                toolStripMenuItemSelectAll = new ToolStripMenuItem("Select All"), toolStripMenuItemUndo = new ToolStripMenuItem("Undo")
            });
        }

        private static void destroyContextMenu()
        {
        }
    }
}