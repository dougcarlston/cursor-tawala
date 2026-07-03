// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Functions.Runtime;
using Tawala.ProjectUI;

namespace Tawala.Functions.Controls
{
    public partial class ConfigureFunctionControl : UserControl, IConfigureFunctionControl
    {
        private IFunction functionInstance;
        private BindingSource parameterInfoSource;

        public ConfigureFunctionControl()
        {
            new DebugInitEventsBehavior(this);

            InitializeComponent();

            int minWidth = base.MinimumSize.Width;

            foreach (Control c in Controls)
            {
                c.MinimumSize = new Size(minWidth, c.MinimumSize.Height);
            }

            ResizeRedraw = true;

            parameterInfoSource = new BindingSource();
            parameterInfoSource.AllowNew = false;
        }

        public override Size MinimumSize { get { return new Size(400, 300); } set { base.MinimumSize = new Size(400, 300); } }

        #region IConfigureFunctionControl Members

        public bool IsOK()
        {
            return functionInstance != null && functionInstance.HasValidParameterValues();
        }

        public void EditFunction(IFunction function)
        {
            functionInstance = function;

            ParameterControlManager.BeginSession(function, this);

            labelFunctionName.Text = functionInstance.Info.Name;
            labelDescription.Text = functionInstance.Info.Description;
            createControls();
        }

        public void CommitPendingEdits()
        {
            ParameterControlManager.CommitPendingChanges();
        }

        public void FreezeLayout()
        {
            SuspendLayout();
            flowLayout.SuspendLayout();
        }

        public void ThawLayout()
        {
            flowLayout.ResumeLayout(false);
            ResumeLayout(false);

            PerformLayout();
            flowLayout.PerformLayout();
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FieldsPalette.Palette.ConfigureFunctionActive = true;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            FieldsPalette.Palette.ConfigureFunctionActive = false;
            parameterInfoSource = null;
            base.OnHandleDestroyed(e);

            ParameterControlManager.EndSession();
        }

        private void createControls()
        {
            FreezeLayout();

            foreach (IParameterInfo info in functionInstance.Info.Parameters)
            {
                addControl(info);
            }

            ThawLayout();
        }

        private void addControl(IParameterInfo info)
        {
            IParameterControl pc = ParameterControlManager.CreateBoundControl(info);
            Control control = pc.GetControl();
            control.Enter += control_EnterAndUpdateDescriptions;

            if (!(control is UserControl))
            {
                var editPanel = new SingleLineEditPanel(pc);
                control = editPanel;
            }

            int width = flowLayout.ClientSize.Width - 20;

            int index = flowLayout.Controls.Count;
            control.TabIndex = index + 1;
            control.Width = width;
            control.Left = 10;
            flowLayout.Controls.Add(control);
            flowLayout.Controls.SetChildIndex(control, index);
            flowLayout.SetFlowBreak(control, true);
        }

        private void control_EnterAndUpdateDescriptions(object sender, EventArgs e)
        {
            var ipc = sender as IParameterControl;
            IParameterBinder ipb = ParameterControlManager.LookupBinder(ipc);
            ipb.SetLabels(labelParamName, labelParamDescription);
        }
    }
}