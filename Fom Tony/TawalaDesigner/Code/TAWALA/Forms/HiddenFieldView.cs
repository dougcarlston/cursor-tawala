// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Forms.Properties;
using Tawala.Projects;

namespace Tawala.Forms
{
    public partial class HiddenFieldView : ItemViewBase
    {
        private static Font labelFont;
        private static Font textBoxActiveFont;
        private static Font textBoxInactiveFont;

        public HiddenFieldView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            if (labelFont == null)
            {
                labelFont = new Font(Font, FontStyle.Italic);
                textBoxInactiveFont = textBox.Font;
                textBoxInactiveFont = textBoxActiveFont; //textBoxActiveFont = new Font(textBox.Font, FontStyle.Bold);
            }

            textBox.LostFocus += textBox_Leave;
        }

        public HiddenField ProjectHiddenField { get { return base.FormItem as HiddenField; } }

        public override string DefaultLabel { get { return "FIELD"; } set { } }

        public override Brush LabelTextBrush { get { return Brushes.DarkGreen; } }

        public override Font LabelTextFont { get { return labelFont; } }

        public override bool AlternateLabelEditable { get { return false; } }

        public override void AfterAddedToFormByUser()
        {
            textBox.Focus();
            textBox.SelectAll();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Project.Events.SynchronizeProject += events_SynchronizeProject;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Project.Events.SynchronizeProject -= events_SynchronizeProject;
            base.OnHandleDestroyed(e);
        }

        private void events_SynchronizeProject(object sender, EventArgs e)
        {
            if (IsHandleCreated && ContainsFocus)
            {
                ProjectHiddenField.Name = textBox.Text;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            textBox.Text = ProjectHiddenField.Name;
            errorProvider.SetIconAlignment(textBox, ErrorIconAlignment.MiddleRight);
            errorProvider.SetIconPadding(textBox, 2);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            var s = new Size(proposedSize.Width, 28); // plus one for separator line
            return s;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (textBox != null)
            {
                labelName.Location = new Point(LabelWidth + 1, (Height - labelName.Height)/2);
                textBox.Location = new Point(labelName.Right + 1, (Height - textBox.Height - 1)/2);
                textBox.Size = new Size(Right - labelName.Right - 1 - errorProvider.Icon.Size.Width - 2, textBox.Height);
            }
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            textBox.Font = textBoxInactiveFont;

            if (ProjectHiddenField.Name.CompareTo(textBox.Text) != 0)
            {
                ProjectHiddenField.Name = textBox.Text;
                Project.Events.RaiseFormItemChangedEvent(new FormItemEventArgs(null, ProjectHiddenField, -1));
            }
        }

        private void textBox_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !isValidText();
            setErrorProvider(e.Cancel);
        }

        private void setErrorProvider(bool error)
        {
            errorProvider.SetError(textBox, error ? Resources.InvalidFieldName : "");
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            textBox.Font = textBoxActiveFont;
        }

        private bool isValidText()
        {
            if (textBox.Text.Length == 0)
            {
                return false;
            }

            if (textBox.Text.Contains(":"))
            {
                return false;
            }

            return ProjectHiddenField.IsNameUniqueInForm(textBox.Text);
        }
    }
}