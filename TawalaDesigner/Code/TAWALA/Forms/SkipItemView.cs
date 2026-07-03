// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Forms.Dialogs.SkipInstructionsDialog;
using Tawala.Projects;
using Form=System.Windows.Forms.Form;

namespace Tawala.Forms
{
    public partial class SkipItemView : ItemViewBase
    {
        private static string defaultLinkText;
        private static Font labelFont;
        private SkipInstructionsView skipInstructionsView;

        public SkipItemView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            if (defaultLinkText == null)
            {
                defaultLinkText = linkLabel.Text;
            }
        }

        public override string DefaultLabel { get { return "SKIP"; } set { } }

        public override Brush LabelTextBrush { get { return Brushes.DarkRed; } }

        public override Font LabelTextFont
        {
            get
            {
                if (labelFont == null)
                {
                    labelFont = new Font(Font, FontStyle.Italic);
                }
                return labelFont;
            }
        }

        public override bool AlternateLabelEditable { get { return false; } }

        public new SkipInstructionsItem FormItem { get { return base.FormItem as SkipInstructionsItem; } }

        /// <summary>
        /// Called when laying out items.  
        /// Items should always use the proposized width but they
        /// are free to choose there height (and must specify it).  
        /// </summary>
        /// <remarks>
        /// proposedSize.Height is always 0 to indicate it is unconstrained
        /// </remarks>
        public override Size GetPreferredSize(Size proposedSize)
        {
            if (Controls[0] is LinkLabel)
            {
                return new Size(proposedSize.Width, 24);
            }
            return new Size(proposedSize.Width, Controls[0].Height + 1);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            if (Controls.Count != 0)
            {
                Controls[0].Width = Width - LabelWidth;
                Controls[0].Left = LabelWidth;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Debug.Assert(FormItem != null);
            base.OnLoad(e);
            updateSummaryText();
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            ((MDIFormView)ParentForm).PerformDelayedLayout();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            updateSummaryText();
            PerformDelayedLayout();
        }

        protected override void OnSelectedChanged(EventArgs e)
        {
            if (Selected)
            {
                ((MDIFormView)ParentForm).TargetTextEditor = null;
            }
            else
            {
                closeSkipInstructionsDialog();
            }

            base.OnSelectedChanged(e);

            PerformDelayedLayout();
        }

        // Fix for Mantis issue 382: Gray area always at top of some Forms (KM - 2006/12/11)
        private void PerformDelayedLayout()
        {
            var form = ParentForm as MDIFormView;

            if (form != null && form.IsHandleCreated)
            {
                form.PerformDelayedLayout();
            }
        }

        private void showSkipInstructionsDialog()
        {
            if (!skipInstructionsDialogIsOpen())
            {
                skipInstructionsView = new SkipInstructionsView(this, FormItem);
                centerSkipInstructionsDialogInApplicationWindow();
                skipInstructionsView.Show(Parent);
            }
        }

        private bool skipInstructionsDialogIsOpen()
        {
            return skipInstructionsView != null;
        }

        private void centerSkipInstructionsDialogInApplicationWindow()
        {
            Form mainWindow = Application.OpenForms[0];
            int x = (mainWindow.Width/2) - (skipInstructionsView.Width/2) + mainWindow.Left;
            int y = (mainWindow.Height/2) - (skipInstructionsView.Height/2) + mainWindow.Top;
            skipInstructionsView.Location = new Point(x, y);
        }

        private void closeSkipInstructionsDialog()
        {
            if (skipInstructionsDialogIsOpen())
            {
                skipInstructionsView.Close();
            }
        }

        public void SkipInstructionsEdited()
        {
            updateSummaryText();
            skipInstructionsView = null;
        }

        private void updateSummaryText()
        {
            linkLabel.Text = string.Format(defaultLinkText, FormItem.GetSummary());
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            showSkipInstructionsDialog();
        }

        private void linkLabel_Click(object sender, EventArgs e)
        {
            Parent.SetAnchor(this);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            Project.Events.FormItemAdded += events_FormItemChanged;
            Project.Events.FormItemRemoved += events_FormItemChanged;
            Project.Events.FormItemChanged += events_FormItemChanged;
        }

        private void events_FormItemChanged(object sender, FormItemEventArgs e)
        {
            updateSummaryText();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Project.Events.FormItemAdded -= events_FormItemChanged;
            Project.Events.FormItemRemoved -= events_FormItemChanged;
            Project.Events.FormItemChanged -= events_FormItemChanged;

            base.OnHandleDestroyed(e);
        }
    }
}