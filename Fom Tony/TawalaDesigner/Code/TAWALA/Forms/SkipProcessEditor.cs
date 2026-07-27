// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Processes;
using Tawala.Projects;

namespace Tawala.Forms
{
    /// <summary>
    /// Ultimately this will be derived from Processes.ViewBase (which doesn't exist yet)
    /// </summary>
    public partial class SkipProcessEditor : ProcessEditor
    {
        public static readonly Type[] StatementViewTypes = {
                                                               null, typeof(IfStatementView), null, typeof(SkipToStatementView), null,
                                                               typeof(SetStatementView), null, typeof(CommentStatementView), null
                                                           };

        private static StatementPalette skipPalette;

        /// <summary>
        /// a copy of the incoming instructions when the view is first invoked;
        /// used for resetting in case user cancels out of the view
        /// </summary>
        private SkipInstructionsItem skipToItem;

        public SkipProcessEditor()
        {
            InitializeComponent();

            EditPanel.Enabled = true;

            if (skipPalette == null)
            {
                skipPalette = new StatementPalette();
                skipPalette.Dock = DockStyle.Left;
                skipPalette.Init(StatementViewTypes);
            }

            // Initialize map of Statement types to Details objects.  Also set Tag property of corresponding statement
            // button to the type of the statement.

            Init(skipPalette, StatementViewTypes);
        }

        internal MDIFormView ParentView { get { return ParentForm as MDIFormView; } }

        protected override void InitLayout()
        {
            base.InitLayout();
//            ParentView.SetPalette(skipPalette);
        }

        /// <summary>
        /// Edit a SkipToItem that isn't currently being edited.
        /// </summary>
        public void Edit(SkipInstructionsItem item)
        {
            skipPalette.SyncButtonToCurrentStatementType(null);
            // keep a reference to the Project's item, and tell it that we're editing its instructions
            skipToItem = item;
            Process = item.Instructions;
            Process.Lines.SetIndentLevels();
            validateLines = true;
        }
    }
}