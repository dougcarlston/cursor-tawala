// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.TextEditor;

namespace Tawala.Forms
{
    public partial class HeadingView : ItemViewBase
    {
        private static readonly HeadingOptions optionsPanel = new HeadingOptions();
        private static Font labelFont;

        public HeadingView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            itemTextEditor.Dock = DockStyle.None;

            Project.Events.SynchronizeProject += events_SynchronizeProject;
        }

        public HeadingItem ProjectHeadingItem { get { return base.FormItem as HeadingItem; } }

        public override Font LabelTextFont
        {
            get
            {
                if (labelFont == null)
                {
                    labelFont = new Font(Font, FontStyle.Bold);
                }
                return labelFont;
            }
        }

        public ItemTextEditor TextEdit { get { return itemTextEditor; } }

        private void events_SynchronizeProject(object sender, EventArgs e)
        {
            if (ContainsFocus)
            {
                ProjectHeadingItem.Rtf = itemTextEditor.GetRTF();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            itemTextEditor.SetRTF(ProjectHeadingItem.Rtf);

            itemTextEditor.GotFocus += itemTextEditor_GotFocus;
            itemTextEditor.FunctionFieldDoubleClicked += itemTextEditor_FunctionFieldDoubleClicked;
            itemTextEditor.ForceOnChanged(); // to make sure control layout encompasses all text
        }

        private void itemTextEditor_GotFocus(object sender, EventArgs e)
        {
            optionsPanel.OwningHeading = this;
            enableOptionsPanel(true);
        }

        public override void AfterAddedToFormByUser()
        {
            AllowSetFocus = true;
            itemTextEditor.RetainSelectionOnEntry = true;
            itemTextEditor.ForceOnChanged(); // to make sure control layout encompasses all text
            itemTextEditor.Focus();
            itemTextEditor.SelectAll();
        }

        private void itemTextEditor_FunctionFieldDoubleClicked(object sender, FunctionFieldEventArgs e)
        {
            itemTextEditor.EditSelectedFunctionConfiguration(e.InstanceId);
        }

        protected override void OnLeave(EventArgs e)
        {
            ProjectHeadingItem.Rtf = itemTextEditor.GetRTF();
            base.OnLeave(e);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (itemTextEditor != null)
            {
                itemTextEditor.Location = new Point(itemTextEditor.Location.X, 0);
                itemTextEditor.Size = itemTextEditor.GetPreferredSize(new Size(Width, 0));

                if (optionsPanel.OwningHeading == this)
                {
                    optionsPanel.Location = new Point(itemTextEditor.Location.X, itemTextEditor.Height);
                }
            }
            base.OnLayout(levent);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            releaseOptionsPanel();
            Project.Events.SynchronizeProject -= events_SynchronizeProject;
            base.OnHandleDestroyed(e);
        }

        protected override void OnValidated(EventArgs e)
        {
            releaseOptionsPanel();
            base.OnValidated(e);
        }

        private void releaseOptionsPanel()
        {
            if (optionsPanel.OwningHeading == this)
            {
                optionsPanel.OwningHeading = null; // hide the options panel
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size rtb = itemTextEditor.GetPreferredSize(proposedSize);
            var s = new Size(proposedSize.Width, rtb.Height + 1);

            if (optionsPanel.OwningHeading == this)
            {
                s.Height += optionsPanel.Height;
            }

            return s;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (Parent == null)
            {
                releaseOptionsPanel();
            }

            base.OnParentChanged(e);
        }

        private void enableOptionsPanel(bool b)
        {
            if (optionsPanel.OwningHeading == this)
            {
                optionsPanel.Enabled = b;
            }
        }
    }
}