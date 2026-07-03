// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Forms
{
    /// <summary>
    /// A File upload item which allows the end user to browse their file system for the file to upload, 
    /// the file is stored on the server
    /// </summary>
    public partial class FileUploadItemView : ItemViewBase
    {
        private static readonly FileUploadItemViewOptions optionsPanel = new FileUploadItemViewOptions();

        public FileUploadItemView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            itemTextEditor.Dock = DockStyle.None;
        }

        public FileUploadItem FileUploadItem { get { return base.FormItem as FileUploadItem; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string rtfString = FileUploadItem.Rtf;
            bool initialText = rtfString.Contains("~@!~");
            int start = 0, end = 0;
            if (initialText)
            {
                start = rtfString.IndexOf("~@!~");
                end = rtfString.LastIndexOf("~@!~");
                if (start < 0 || start == end)
                {
                    initialText = false;
                }
                else
                {
                    itemTextEditor.SetRTF(rtfString);
                    string rawText = itemTextEditor.GetText();
                    start = rawText.IndexOf("~@!~");
                    end = rawText.LastIndexOf("~@!~");
                    rtfString = Regex.Replace(rtfString, "~@!~", "");
                }
            }

            itemTextEditor.SetRTF(rtfString);

            if (initialText)
            {
                AllowSetFocus = true;
                itemTextEditor.Select(start, end - start - 4);
                itemTextEditor.RetainSelectionOnEntry = true;
            }

            itemTextEditor.ForceOnChanged(); // to make sure control layout encompasses all text

            // don't hook these events until after the above is done
            itemTextEditor.GotFocus += itemTextEditor_GotFocus;
            itemTextEditor.InputPositionChanged += itemTextEditor_InputPositionChanged;
            Project.Events.SynchronizeProject += events_SynchronizeProject;
        }

        private void events_SynchronizeProject(object sender, EventArgs e)
        {
            if (ContainsFocus)
            {
                synchronizeProjectItem();
            }
        }

        private void synchronizeProjectItem()
        {
            FileUploadItem.Rtf = itemTextEditor.GetRTF();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            releaseOptionsPanel();

            Project.Events.SynchronizeProject -= events_SynchronizeProject;
            base.OnHandleDestroyed(e);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size rtb = itemTextEditor.GetPreferredSize(proposedSize);
            var s = new Size(proposedSize.Width, rtb.Height + 1);

            if (optionsPanel.CurrentOwner == this)
            {
                s.Height += optionsPanel.Height;
            }

            return s;
        }

        protected override void OnValidated(EventArgs e)
        {
            synchronizeProjectItem();

            releaseOptionsPanel();
            base.OnValidated(e);
        }

        private void releaseOptionsPanel()
        {
            if (optionsPanel.CurrentOwner == this)
            {
                optionsPanel.CurrentOwner = null; // hide the options panel
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (Parent == null)
            {
                if (optionsPanel.CurrentOwner == this)
                {
                    optionsPanel.CurrentOwner = null;
                }
            }

            base.OnParentChanged(e);
        }

        private void enableOptionsPanel(bool b)
        {
            if (optionsPanel.CurrentOwner == this)
            {
                optionsPanel.Enabled = b;
            }
        }

        private void updateOptionsPanel()
        {
            optionsPanel.Required = FileUploadItem.Required;
            enableOptionsPanel(true);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (itemTextEditor != null)
            {
                itemTextEditor.Location = new Point(itemTextEditor.Location.X, 0);
                itemTextEditor.Size = itemTextEditor.GetPreferredSize(new Size(Width, 0));

                if (optionsPanel.CurrentOwner == this)
                {
                    optionsPanel.Location = new Point(itemTextEditor.Location.X, itemTextEditor.Height);
                }
            }
            base.OnLayout(levent);
        }

        internal void UpdateRequiredFlag()
        {
            FileUploadItem.Required = optionsPanel.Required;
        }

        #region Text Editor Events

        private void itemTextEditor_GotFocus(object sender, EventArgs e)
        {
            optionsPanel.CurrentOwner = this;
            updateOptionsPanel();
        }

        private void itemTextEditor_InputPositionChanged(object sender, EventArgs e)
        {
            updateOptionsPanel();
        }

        #endregion
    }
}