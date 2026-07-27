// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Functions.Controls;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Forms
{
    /// <summary>
    /// A Multiple Choice Form Item UI control
    /// </summary>
    internal partial class McqItemView : ItemViewBase
    {
        private static readonly McqOptions optionsPanel = new McqOptions();

        public McqItemView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            itemTextEditor.Dock = DockStyle.None;
        }

        public McqItem ProjectMCItem { get { return base.FormItem as McqItem; } }

        internal void EditDataSourceFunction()
        {
            if (ProjectMCItem.DataSourceFunction == null)
            {
                ConfigureFunctionDialog.Presenter.CreateFunction(FunctionLoader.Repository.Functions["dynamic-mcq"],
                                                                 handleDataSourceConfigured);
            }
            else
            {
                ConfigureFunctionDialog.Presenter.EditFunction(ProjectMCItem.DataSourceFunction, handleDataSourceConfigured);
            }
        }

        private void handleDataSourceConfigured(object sender, FunctionConfiguredEventArgs args)
        {
            if (!args.Canceled)
            {
                ProjectMCItem.DataSourceFunction = args.EditedInstance;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string rtfString = ProjectMCItem.Rtf;
            string highlightString = "~@!~";

            bool initialText = rtfString.Contains(highlightString);
            int start = 0, end = 0;
            if (initialText)
            {
                start = rtfString.IndexOf(highlightString);
                end = rtfString.LastIndexOf(highlightString);

                if (start < 0 || start == end)
                {
                    initialText = false;
                }
                else
                {
                    itemTextEditor.SetRTF(rtfString);
                    string rawText = itemTextEditor.GetText();

                    start = rawText.IndexOf(highlightString);
                    end = rawText.LastIndexOf(highlightString);

                    rtfString = rtfString.Replace(highlightString, "");
                }
            }

            itemTextEditor.SetRTF(rtfString);

            if (initialText)
            {
                AllowSetFocus = true;
                itemTextEditor.Select(start, end - start - highlightString.Length);
                itemTextEditor.RetainSelectionOnEntry = true;
            }

            itemTextEditor.GotFocus += itemTextEditor_GotFocus;
            itemTextEditor.Changed += itemTextEditor_Changed;
            itemTextEditor.InputPositionChanged += itemTextEditor_InputPositionChanged;
            Project.Events.SynchronizeProject += events_SynchronizeProject;
        }

        private void events_SynchronizeProject(object sender, EventArgs e)
        {
            if (ContainsFocus)
            {
                ProjectMCItem.Rtf = itemTextEditor.GetRTF();
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            releaseOptionsPanel();

            Project.Events.SynchronizeProject -= events_SynchronizeProject;
            base.OnHandleDestroyed(e);
        }

        private void releaseOptionsPanel()
        {
            if (optionsPanel.OwningMCQItemView == this)
            {
                optionsPanel.OwningMCQItemView = null;
            }
        }

        /// <summary>
        /// Called when laying out items.  
        /// Items should always use the proposized width but they
        /// are free to choose their height (and must specify it).  
        /// </summary>
        /// <remarks>
        /// proposedSize.Height is always 0 to indicate it is unconstrained
        /// </remarks>
        public override Size GetPreferredSize(Size proposedSize)
        {
            Size rtb = itemTextEditor.GetPreferredSize(proposedSize);
            var s = new Size(proposedSize.Width, rtb.Height + 1);

            if (optionsPanel.OwningMCQItemView == this)
            {
                s.Height += optionsPanel.Height;
            }

            // height is that of itemTextEditor + 1 + options panel if its displayed
            return s;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            itemTextEditor.RefreshLabels();
        }

        /// <summary>
        /// When the user leaves the FIBItem, hide the options panel.
        /// </summary>
        protected override void OnValidated(EventArgs e)
        {
            ProjectMCItem.Rtf = itemTextEditor.GetRTF();
            releaseOptionsPanel();
            base.OnValidated(e);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (Parent == null)
            {
                if (optionsPanel.OwningMCQItemView == this)
                {
                    optionsPanel.OwningMCQItemView = null;
                }
            }

            base.OnParentChanged(e);
        }

        private void enableOptions(bool b)
        {
            if (optionsPanel.OwningMCQItemView == this)
            {
                optionsPanel.Enabled = b;
            }
        }

        /// <summary>
        /// The width of the Rich TextBox control needs to be the same as the Width of this Form Item
        /// and the height needs to be the height of the Form Item, less the options panel height (if displayed)
        /// </summary>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (itemTextEditor != null)
            {
                itemTextEditor.Location = new Point(itemTextEditor.Location.X, 0);
                itemTextEditor.Size = itemTextEditor.GetPreferredSize(new Size(Width, 0));

                if (optionsPanel.OwningMCQItemView == this)
                {
                    optionsPanel.Location = new Point(itemTextEditor.Location.X, itemTextEditor.Height);
                }
            }
            base.OnLayout(levent);
        }

        #region itemTextEditor Events

        /// <summary>
        /// When the itemTextEditor gets focus, show the options panel
        /// </summary>
        private void itemTextEditor_GotFocus(object sender, EventArgs e)
        {
            if (optionsPanel.OwningMCQItemView != this)
            {
                optionsPanel.OwningMCQItemView = this;
            }
        }

        private void itemTextEditor_Changed(object sender, EventArgs e)
        {
            ProjectMCItem.Rtf = itemTextEditor.GetRTF();
        }

        private void itemTextEditor_InputPositionChanged(object sender, EventArgs e)
        {
            if (!itemTextEditor.Focused)
            {
                itemTextEditor.Focus();
            }
        }

        #endregion
    }
}