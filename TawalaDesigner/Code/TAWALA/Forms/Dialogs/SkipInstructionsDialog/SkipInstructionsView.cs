// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Processes;
using Tawala.Projects.Forms;
using Tawala.ProjectUI;

namespace Tawala.Forms.Dialogs.SkipInstructionsDialog
{
    public partial class SkipInstructionsView : Form
    {
        private static readonly Type[] statementViewTypes = {
                                                                typeof(IfStatementView), null, typeof(SkipToStatementView), null,
                                                                typeof(SetStatementView), null, typeof(CommentStatementView), null
                                                            };

        private readonly SkipItemView formSkipItemView;
        private readonly ISkipInstructionsItem skipInstructionsItem;

        public SkipInstructionsView(SkipItemView formSkipItemView, ISkipInstructionsItem skipInstructionsItem)
        {
            InitializeComponent();

            this.formSkipItemView = formSkipItemView;
            this.skipInstructionsItem = skipInstructionsItem;

            skipInstructionsStatementSelector.Init(statementViewTypes);
            skipInstructionsStatementSelector.ProcessEditor = processEditor;

            processEditor.Init(skipInstructionsStatementSelector, statementViewTypes);
            processEditor.Process = skipInstructionsItem.Instructions;
            processEditor.ConnectProjectEvents(true);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SkipInstructionsView_FormClosed(object sender, FormClosedEventArgs e)
        {
            processEditor.ConnectProjectEvents(false);
            formSkipItemView.SkipInstructionsEdited();
            Application.Idle -= application_Idle;
        }

        private void toolStripMenuItemCut_Click(object sender, EventArgs e)
        {
            ((IEditMenu)processEditor).Cut();
        }

        private void toolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            ((IEditMenu)processEditor).Copy();
        }

        private void toolStripMenuItemPaste_Click(object sender, EventArgs e)
        {
            ((IEditMenu)processEditor).Paste();
        }

        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            ((IEditMenu)processEditor).Delete();
        }

        private void toolStripMenuItemUndo_Click(object sender, EventArgs e)
        {
            ((IEditMenu)processEditor).Undo();
        }

        private void toolStripMenuItemRedo_Click(object sender, EventArgs e)
        {
            ((IEditMenu)processEditor).Redo();
        }

        private void SkipInstructionsView_Load(object sender, EventArgs e)
        {
            Application.Idle += application_Idle;
        }

        private void application_Idle(object sender, EventArgs e)
        {
            toolStripButtonCut.Enabled = toolStripMenuItemCut.Enabled = ((IEditMenu)processEditor).CanCut();

            toolStripButtonCopy.Enabled = toolStripMenuItemCopy.Enabled = ((IEditMenu)processEditor).CanCopy();

            toolStripButtonPaste.Enabled = toolStripMenuItemPaste.Enabled = ((IEditMenu)processEditor).CanPaste();

            toolStripButtonDelete.Enabled = toolStripMenuItemDelete.Enabled = ((IEditMenu)processEditor).CanDelete();

            toolStripButtonUndo.Enabled = toolStripMenuItemUndo.Enabled = ((IEditMenu)processEditor).CanUndo();
            toolStripButtonUndo.Text = toolStripMenuItemUndo.Text = "Undo " + ((IEditMenu)processEditor).UndoActionText;

            toolStripButtonRedo.Enabled = toolStripMenuItemRedo.Enabled = ((IEditMenu)processEditor).CanRedo();
            toolStripButtonRedo.Text = toolStripMenuItemRedo.Text = "Redo " + ((IEditMenu)processEditor).RedoActionText;
        }
    }
}