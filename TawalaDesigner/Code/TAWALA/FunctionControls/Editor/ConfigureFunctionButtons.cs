// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    public partial class ConfigureFunctionButtons : UserControl
    {
        public ConfigureFunctionButtons()
        {
            InitializeComponent();

            toolStripButtonOk.Click += toolStripButtonOk_Click;
            toolStripButtonCancel.Click += toolStripButtonCancel_Click;
        }

        public bool OKEnabled
        {
            get
            {
                return toolStripButtonOk.Enabled;
            }
            set
            {
                toolStripButtonOk.Enabled = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Application.Idle += application_Idle;
            base.OnLoad(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Application.Idle -= application_Idle;
        }

        private void application_Idle(object sender, EventArgs e)
        {
            toolStripButtonOk.Enabled = ControlManager.QueryOKEnabled();
        }

        private void toolStripButtonCancel_Click(object sender, EventArgs e)
        {
            if (CancelClick != null)
            {
                CancelClick(this, EventArgs.Empty);
            }
        }

        private void toolStripButtonOk_Click(object sender, EventArgs e)
        {
            if (OKClick != null)
            {
                OKClick(this, EventArgs.Empty);
            }
        }

        public void HookButton(string suffix, EventHandler clickHandler)
        {
            ToolStripItem toolStripButton = toolStrip.Items["toolStripButton" + suffix];
            toolStripButton.Click += clickHandler;
            toolStripButton.Visible = true;
        }

        public event EventHandler OKClick;
        public event EventHandler CancelClick;
    }
}