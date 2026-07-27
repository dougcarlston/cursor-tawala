// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Expressions;

namespace Tawala.Controls
{
    public class ExpressionTextBox : ProcessTextBox
    {
        private IContainer components;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem toolStripMenuItemCopy;
        private ToolStripMenuItem toolStripMenuItemCut;
        private ToolStripMenuItem toolStripMenuItemPaste;
        private ToolStripMenuItem toolStripMenuItemSelectAll;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;

        public ExpressionTextBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Indicates whether this text box accepts drag/drop of the specified data.
        /// </summary>
        public override bool AcceptsDropOf(IDataObject data)
        {
            if (data.GetDataPresent(typeof(IPaletteField)))
            {
                return true;
            }

            return false;
        }

        public IField DraggedField(IDataObject data)
        {
            IField field = null;

            if (data.GetDataPresent(typeof(IPaletteField)))
            {
                field = (IField)data.GetData(typeof(IPaletteField));
            }

            return field;
        }

        private void InitializeComponent()
        {
            components = new Container();
            ToolStripMenuItem toolStripMenuItemUndo;
            contextMenuStrip = new ContextMenuStrip(components);
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripMenuItemCut = new ToolStripMenuItem();
            toolStripMenuItemCopy = new ToolStripMenuItem();
            toolStripMenuItemPaste = new ToolStripMenuItem();
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
                toolStripMenuItemUndo, toolStripSeparator1, toolStripMenuItemCut, toolStripMenuItemCopy, toolStripMenuItemPaste,
                toolStripSeparator2, toolStripMenuItemSelectAll
            });
            contextMenuStrip.Name = "contextMenuStrip1";
            contextMenuStrip.Size = new Size(116, 126);
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
            // ExpressionTextBox
            // 
            ContextMenuStrip = contextMenuStrip;
            contextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            contextMenuStrip.Items["toolStripMenuItemUndo"].Enabled = false; // for now; revisit

            contextMenuStrip.Items["toolStripMenuItemCut"].Enabled = canCopy();
            contextMenuStrip.Items["toolStripMenuItemCopy"].Enabled = canCopy();

            contextMenuStrip.Items["toolStripMenuItemPaste"].Enabled = canPaste();

            contextMenuStrip.Items["toolStripMenuItemSelectAll"].Enabled = true;
        }

        private bool canCopy()
        {
            return SelectionLength > 0;
        }

        private bool canPaste()
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            return (dataObject != null && (clipboardContainsString(dataObject) || clipboardContainsExpression(dataObject)));
        }

        private static bool clipboardContainsString(IDataObject dataObject)
        {
            return dataObject.GetDataPresent(typeof(string));
        }

        private static bool clipboardContainsExpression(IDataObject dataObject)
        {
            return dataObject.GetDataPresent(typeof(Expression));
        }

        private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Name)
            {
                case "toolStripMenuItemUndo":
                    Undo();
                    break;

                case "toolStripMenuItemCut":
                    if (Tag != null)
                    {
                        var expression = Tag as Expression;
                        Clipboard.SetDataObject(expression);

                        ClearTextAndTag();
                    }
                    else
                    {
                        Cut();
                    }
                    break;

                case "toolStripMenuItemCopy":
                    if (Tag != null)
                    {
                        var expression = Tag as Expression;
                        Clipboard.SetDataObject(expression);
                    }
                    else
                    {
                        Copy();
                    }
                    break;

                case "toolStripMenuItemPaste":
                    IDataObject clipData = Clipboard.GetDataObject();
                    if (clipData != null)
                    {
                        if (clipboardContainsExpression(clipData))
                        {
                            var expression = clipData.GetData(typeof(Expression)) as Expression;
                            Tag = expression;
                            Text = expression.ToString().Trim("<>".ToCharArray());
                        }
                        else if (clipboardContainsString(clipData))
                        {
                            var clipboardText = clipData.GetData(typeof(string)) as string;
                            if (Project.FieldMapByName.ContainsKey(clipboardText))
                            {
                                IField field = Project.FieldMapByName[clipboardText];
                                Tag = new Expression(field);
                                Text = field.QualifiedFieldName;
                            }
                            else if (!clipboardText.Contains(":"))
                            {
                                Paste();
                            }
                        }
                    }
                    break;

                case "toolStripMenuItemSelectAll":
                    SelectAll();
                    break;

                default:
                    break;
            }
        }

        public void ClearTextAndTag()
        {
            Tag = null;
            Clear();
        }
    }
}