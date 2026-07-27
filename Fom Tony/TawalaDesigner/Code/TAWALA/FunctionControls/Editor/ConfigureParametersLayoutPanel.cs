// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Functions.Controls.Design;
using Tawala.Functions.Runtime;
using Tawala.ProjectUI;

namespace Tawala.Functions.Controls
{
    [Designer(typeof(ConfigureParametersLayoutPanelDesigner))]
    public partial class ConfigureParametersLayoutPanel : FlowLayoutPanel
    {
        public ConfigureParametersLayoutPanel()
        {
            base.DoubleBuffered = true;
            InitializeComponent();

            if (FieldsPalette.Palette != null)
            {
                FieldsPalette.Palette.StatusChanged += fieldsPaletteStatusChanged;
            }
        }

        public override Size MinimumSize { get { return new Size(540, 428); } }
        private Timer dragScrollTimer { get; set; }

        public void CreateParameterControls()
        {
            SuspendLayout();

            var panels = new Collection<SingleLineEditPanel>();

            foreach (IParameterInfo info in ControlManager.Function.Info.Parameters)
            {
                addControl(info, panels);
            }

            optimizeSingleLineEditPanels(panels);

            ResumeLayout(false);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (FieldsPalette.Palette != null)
            {
                FieldsPalette.Palette.StatusChanged -= fieldsPaletteStatusChanged;
            }
            destroyDragScrollTimer();

            base.OnHandleDestroyed(e);
        }

        private void fieldsPaletteStatusChanged(object sender, FieldsPaletteStatusEventArgs e)
        {
            switch (e.Status)
            {
                case FieldsPaletteStatusChange.BeginNodeDrag:
                {
                    createDragScrollTimer();
                    break;
                }
                case FieldsPaletteStatusChange.EndNodeDrag:
                {
                    destroyDragScrollTimer();
                    break;
                }
            }
        }

        private void createDragScrollTimer()
        {
            if (dragScrollTimer == null)
            {
                dragScrollTimer = new Timer();
                dragScrollTimer.Tick += dragScrollTimer_Tick;
                dragScrollTimer.Interval = 300;
                dragScrollTimer.Start();
            }
        }

        private void destroyDragScrollTimer()
        {
            if (dragScrollTimer != null)
            {
                dragScrollTimer.Tick -= dragScrollTimer_Tick;
                dragScrollTimer.Stop();
                dragScrollTimer.Dispose();
                dragScrollTimer = null;
            }
        }

        private static readonly int scrollIncrement = SystemInformation.HorizontalScrollBarHeight * 2;

        private void dragScrollTimer_Tick(object sender, EventArgs e)
        {
            if ((MouseButtons & MouseButtons.Left) == 0)
            {
                destroyDragScrollTimer();
                return;
            }

            if (!VerticalScroll.Visible)
            {
                return;
            }

            Point pt = PointToClient(MousePosition);
            int y = pt.Y;
            int x = pt.X;

            if (x < 0 && x > Width)
            {
                return;
            }

            if (y >= 0 && y < 10)
            {
                VerticalScroll.Value = Math.Max(VerticalScroll.Minimum, VerticalScroll.Value - scrollIncrement);
            }
            else if (y > Height - 10 && y < Height)
            {
                VerticalScroll.Value = Math.Min(VerticalScroll.Maximum, VerticalScroll.Value + scrollIncrement);
            }
        }

        private static void optimizeSingleLineEditPanels(IList<SingleLineEditPanel> panels)
        {
            int panelWidth = 0;

            foreach (SingleLineEditPanel panel in panels)
            {
                panelWidth = Math.Max(panelWidth, panel.LabelWidth);
            }

            foreach (SingleLineEditPanel panel in panels)
            {
                panel.OptimizeLayout(panelWidth);
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            int width = Width - SystemInformation.VerticalScrollBarWidth - 16;

            foreach (Control c in Controls)
            {
                c.SuspendLayout();
                c.MinimumSize = new Size(width, c.MinimumSize.Height);
                c.MaximumSize = new Size(width, c.MaximumSize.Height);
                c.Width = width;
                c.ResumeLayout(false);
                c.PerformLayout();
            }
            base.OnLayout(levent);
        }

        private void addControl(IParameterInfo info, IList<SingleLineEditPanel> panels)
        {
            IParameterControl parameterControl = ControlManager.CreateControl(info);
            var control = parameterControl as Control;
            control.Enter += updateParameterDescription;

            if (!parameterControl.CustomControl)
            {
                var editPanel = new SingleLineEditPanel(parameterControl);
                control = editPanel;
                panels.Add(editPanel);
            }

            int index = Controls.Count;
            control.TabIndex = index + 1;
            Controls.Add(control);
            Controls.SetChildIndex(control, index);
            SetFlowBreak(control, true);
        }

        private static void updateParameterDescription(object sender, EventArgs args)
        {
            IParameterInfo parameterInfo = ControlManager.LookupParameterInfo(sender as IParameterControl);
            ControlManager.SetCurrentParameterInfo(parameterInfo, null);
        }
    }
}