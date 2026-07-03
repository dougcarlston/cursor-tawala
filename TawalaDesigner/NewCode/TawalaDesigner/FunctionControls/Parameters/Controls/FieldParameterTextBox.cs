// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.Functions.Controls
{
    [ToolboxItem(false), DesignerCategory("code")]
    [DefaultBindingProperty("CustomDataSource")]
    internal abstract class FieldParameterTextBox<T> : TextBox, IParameterEditControl, IDataAccepted, ICustomDataSource
        where T : FunctionFieldWrapper
    {
        private IContainer components;
        private ContextMenuStrip contextMenuStrip;
        private T functionData;
        private ToolStripMenuItem toolStripMenuItemCopy;
        private ToolStripMenuItem toolStripMenuItemCut;
        private ToolStripMenuItem toolStripMenuItemDelete;
        private ToolStripMenuItem toolStripMenuItemPaste;
        private ToolStripMenuItem toolStripMenuItemSelectAll;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;

        protected FieldParameterTextBox()
        {
            ReadOnly = true;
            base.BackColor = SystemColors.Window;
            base.ForeColor = SystemColors.WindowText;

            new DebugInitEventsBehavior(this);
            new HighlightBehavior(this);
            new DragDropBehavior(this);
            new FieldPaletteDoubleClickBehavior(this);

            InitializeComponent();
        }

        #region ICustomDataSource

        public object CustomDataSource
        {
            get { return functionData; }
            set
            {
                if ((value as T) != null)
                {
                    functionData = value as T;
                    if (CustomDataSourceChanged != null)
                    {
                        CustomDataSourceChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public event EventHandler CustomDataSourceChanged;

        #endregion

        #region IDataAccepted Members

        public abstract bool IsAcceptedData(object o);
        public abstract void AcceptData(object o);

        #endregion

        #region IParameterEditControl Members

        public void CommitPendingChanges()
        {
        }

        public Control GetControl()
        {
            return this;
        }

        #endregion

        private void InitializeComponent()
        {
            components = new Container();
            ToolStripMenuItem toolStripMenuItemUndo;
            contextMenuStrip = new ContextMenuStrip(components);
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripMenuItemCut = new ToolStripMenuItem();
            toolStripMenuItemCopy = new ToolStripMenuItem();
            toolStripMenuItemPaste = new ToolStripMenuItem();
            toolStripMenuItemDelete = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripMenuItemSelectAll = new ToolStripMenuItem();
            toolStripMenuItemUndo = new ToolStripMenuItem();
            contextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripMenuItemUndo
            // 
            toolStripMenuItemUndo.Name = "toolStripMenuItemUndo";
            toolStripMenuItemUndo.ShortcutKeys = (((Keys.Control | Keys.Z)));
            toolStripMenuItemUndo.ShowShortcutKeys = false;
            toolStripMenuItemUndo.Size = new Size(115, 22);
            toolStripMenuItemUndo.Text = "&Undo";
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Items.AddRange(new ToolStripItem[]
                                            {
                                                toolStripMenuItemUndo,
                                                toolStripSeparator1,
                                                toolStripMenuItemCut,
                                                toolStripMenuItemCopy,
                                                toolStripMenuItemPaste,
                                                toolStripMenuItemDelete,
                                                toolStripSeparator2,
                                                toolStripMenuItemSelectAll
                                            });
            contextMenuStrip.Name = "contextMenuStrip1";
            contextMenuStrip.Size = new Size(116, 148);
            contextMenuStrip.ItemClicked += contextMenuStrip_ItemClicked;
            contextMenuStrip.Opening += contextMenuStrip_Opening;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(112, 6);
            // 
            // toolStripMenuItemCut
            // 
            toolStripMenuItemCut.Name = "toolStripMenuItemCut";
            toolStripMenuItemCut.ShortcutKeys = (((Keys.Control | Keys.T)));
            toolStripMenuItemCut.ShowShortcutKeys = false;
            toolStripMenuItemCut.Size = new Size(115, 22);
            toolStripMenuItemCut.Text = "&Cut";
            // 
            // toolStripMenuItemCopy
            // 
            toolStripMenuItemCopy.Name = "toolStripMenuItemCopy";
            toolStripMenuItemCopy.ShortcutKeys = (((Keys.Control | Keys.C)));
            toolStripMenuItemCopy.ShowShortcutKeys = false;
            toolStripMenuItemCopy.Size = new Size(115, 22);
            toolStripMenuItemCopy.Text = "&Copy";
            // 
            // toolStripMenuItemPaste
            // 
            toolStripMenuItemPaste.Name = "toolStripMenuItemPaste";
            toolStripMenuItemPaste.ShortcutKeys = (((Keys.Control | Keys.V)));
            toolStripMenuItemPaste.ShowShortcutKeys = false;
            toolStripMenuItemPaste.Size = new Size(115, 22);
            toolStripMenuItemPaste.Text = "&Paste";
            // 
            // toolStripMenuItemDelete
            // 
            toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
            toolStripMenuItemDelete.ShortcutKeys = Keys.Delete;
            toolStripMenuItemDelete.ShowShortcutKeys = false;
            toolStripMenuItemDelete.Size = new Size(115, 22);
            toolStripMenuItemDelete.Text = "&Delete";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(112, 6);
            // 
            // toolStripMenuItemSelectAll
            // 
            toolStripMenuItemSelectAll.Name = "toolStripMenuItemSelectAll";
            toolStripMenuItemSelectAll.ShortcutKeys = (((Keys.Control | Keys.A)));
            toolStripMenuItemSelectAll.ShowShortcutKeys = false;
            toolStripMenuItemSelectAll.Size = new Size(115, 22);
            toolStripMenuItemSelectAll.Text = "&Select All";
            // 
            // FieldParameterTextBox
            // 
            ContextMenuStrip = contextMenuStrip;
            contextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            contextMenuStrip.Items["toolStripMenuItemUndo"].Enabled = false; // for now; revisit

            contextMenuStrip.Items["toolStripMenuItemCut"].Enabled = false;
            contextMenuStrip.Items["toolStripMenuItemCopy"].Enabled = canCopyOrDelete();

            contextMenuStrip.Items["toolStripMenuItemPaste"].Enabled = false;

            contextMenuStrip.Items["toolStripMenuItemDelete"].Enabled = canCopyOrDelete();

            contextMenuStrip.Items["toolStripMenuItemSelectAll"].Enabled = true;
        }

        private bool canCopyOrDelete()
        {
            return Text.Length > 0;
        }

        private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Name)
            {
                case "toolStripMenuItemUndo":
                    Undo();
                    break;

                case "toolStripMenuItemCut":
                    break;

                case "toolStripMenuItemCopy":
                    Copy();
                    break;

                case "toolStripMenuItemPaste":
                    break;

                case "toolStripMenuItemDelete":
                    deleteCustomDataSource();
                    break;

                case "toolStripMenuItemSelectAll":
                    SelectAll();
                    break;

                default:
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Delete || keyData == Keys.Back)
            {
                deleteCustomDataSource();
                return true;
            }

            return false;
        }

        private void deleteCustomDataSource()
        {
            functionData = null;
            if (CustomDataSourceChanged != null)
            {
                CustomDataSourceChanged(this, EventArgs.Empty);
            }
            Clear();
        }
    }
}