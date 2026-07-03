// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.TextEditor;

namespace Tawala.Forms
{
    public partial class TextItemView : ItemViewBase
    {
        public TextItemView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            itemTextEditor.Dock = DockStyle.None;

            Project.Events.SynchronizeProject += events_SynchronizeProject;
        }

        public TextItem ProjectTextItem { get { return base.FormItem as TextItem; } }

        public ItemTextEditor TextEdit { get { return itemTextEditor; } }

        private void events_SynchronizeProject(object sender, EventArgs e)
        {
            if (ContainsFocus)
            {
                ProjectTextItem.Rtf = itemTextEditor.GetRTF();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            itemTextEditor.SetRTF(ProjectTextItem.Rtf);

            itemTextEditor.FunctionFieldDoubleClicked += itemTextEditor_FunctionFieldDoubleClicked;
            itemTextEditor.InvitationFieldDoubleClicked += itemTextEditor_InvitationFieldDoubleClicked;
            itemTextEditor.ForceOnChanged(); // to make sure control layout encompasses all text
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

        private void itemTextEditor_InvitationFieldDoubleClicked(object sender, InvitationFieldEventArgs e)
        {
            ((MDIFormView)ParentForm).EditLinkField(e.Id);
        }

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
            Size rtb = itemTextEditor.GetPreferredSize(proposedSize);
            var s = new Size(proposedSize.Width, rtb.Height + 1); // plus one for separator line
            // height is that of richTextBox + 1 + options panel if its displayed
            return s;
        }

        protected override void OnLeave(EventArgs e)
        {
            ProjectTextItem.Rtf = itemTextEditor.GetRTF();
            // Destroys selection when combobox dropdown triggers this even when set font name / size
            //if (itemTextEditor.Selection.Length > 0)
            //{
            //    itemTextEditor.Select(itemTextEditor.Selection.Start, 0);
            //}
            base.OnLeave(e);
        }

        /// <summary>
        /// Called when the label has been set to a new value
        /// </summary>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (itemTextEditor != null)
            {
                itemTextEditor.Location = new Point(itemTextEditor.Location.X, 0);
                itemTextEditor.Size = itemTextEditor.GetPreferredSize(new Size(Width, 0));
            }
            base.OnLayout(levent);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Project.Events.SynchronizeProject -= events_SynchronizeProject;
            itemTextEditor.FunctionFieldDoubleClicked -= itemTextEditor_FunctionFieldDoubleClicked;
            itemTextEditor.InvitationFieldDoubleClicked -= itemTextEditor_InvitationFieldDoubleClicked;
            base.OnHandleDestroyed(e);
        }
    }
}